using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChildMaintenanceCalculator.Models;
using ChildMaintenanceCalculator.Models.ViewModels;
using ChildMaintenanceCalculator.Services;
using ExtensionMethods;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;

namespace ChildMaintenanceCalculator.Controllers
{
    public class HomeController : Controller
    {
        private readonly IViewRenderService _viewRenderService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ILogger _logger;
        private string _pdfFooter;

        public HomeController(IViewRenderService viewRenderService, IEmailSenderService emailSenderService, ILogger<HomeController> logger)
        {
            this._viewRenderService = viewRenderService;
            this._emailSenderService = emailSenderService;
            this._logger = logger;

            this._pdfFooter = " --footer-center \"" + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
        }

        public IActionResult Index()
        {
            return View();
        }

        //Step 1 - Add receiving parents and children
        [HttpGet]
        public IActionResult Step1()
        {
            Step1ViewModel viewModel = new Step1ViewModel();

            return View("Step1", viewModel);

        }

        [HttpPost]
        public IActionResult Step1(Step1ViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Step1", vm);

            Calculation calculation = new Calculation();

            calculation.PayingParent.ReceivingParents = vm.Step1ReceivingParents.Select(r => new ReceivingParent
            {
                Id = r.Id,
                FirstName = r.FirstName,
                Children = r.Step1Children.Select(x => new Child
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    PreExistingMaintenanceArrangements = x.PreExistingArrangements,
                    PreExistingMaintenanceArrangementsAmount = x.PreExisingArrangementsAmount ?? 0
                }).ToList()
            }).ToList();

            //Store the Domin Model in Temp Data for persistence between steps
            this.StoreModel(calculation);

            return RedirectToAction("Step2");

        }

        [HttpPost]
        public IActionResult Step1AddNewReceivingParent(int parentindex, int childindex)
        {
            var newReceivingParent = new Step1ReceivingParent();
            //Add new parent index to ViewData so that it can be used in the partial to set parent ID
            ViewData["receivingParentIndex"] = parentindex;
            ViewData["childIndex"] = childindex;
            ViewData["firstChild"] = true;

            return PartialView("_AddReceivingParentPartial", newReceivingParent);
        }

        [HttpPost]
        public IActionResult Step1AddNewChild(int index, string parentHtmlFieldPrefix)
        {
            var newChild = new Step1Child();
            //Add new child index to ViewData so that it can be used in the partial to set child ID
            ViewData["childIndex"] = index;
            ViewData["firstChild"] = false;
            ViewData.TemplateInfo.HtmlFieldPrefix = parentHtmlFieldPrefix;

            return PartialView("_AddChildPartial", newChild);
        }

        //Step 2 - Does the paying parent receive a relevant benefit
        [HttpGet]
        public IActionResult Step2()
        {
            Step2ViewModel viewModel = new Step2ViewModel();

            return View("Step2", viewModel);
        }

        [HttpPost]
        public IActionResult Step2(Step2ViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Step2", vm);
            
            //Retrieve calculation data from tempData, update with data from step 2 and store in tempData again
            Calculation calculation = this.GetModel();
            calculation.PayingParent.RelevantBenefit = vm.PayingParentReceivesBenefit;
            this.StoreModel(calculation);

            if(calculation.PayingParent.RelevantBenefit == false)
            {
                return RedirectToAction("Step3A");
            }
            else if(calculation.PayingParent.RelevantBenefit == true)
            {
                return RedirectToAction("Step3B");
            }
            else
            {
                return View("Error");
            }
        }

        // Step 3a - Capture the paying parents income and pension contributions
        [HttpGet]
        public IActionResult Step3A()
        {
            Step3AViewModel viewModel = new Step3AViewModel();
            return View("Step3A", viewModel);
        }

        [HttpPost]
        public IActionResult Step3A(Step3AViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Step3A", vm);
            
            Calculation calculation = this.GetModel();

            calculation.PayingParent.AnnualIncome = vm.PayingParentAnnualIncome ?? 0;
            calculation.PayingParent.AnnualPension = vm.PayingParentAnnualPension ?? 0;

            this.StoreModel(calculation);

            if(calculation.PayingParent.GrossWeeklyIncome <= 100)
            {
                return RedirectToAction("Result");
            }
            else
            {
                return RedirectToAction("Step4");
            }

        }

        //Step 3b - How many nights per yer do the children stay with the paying prent (low bracket)
        [HttpGet]
        public IActionResult Step3B()
        {
            //Get all the children from the domain model into a list of children to pass in to the Step3BViewModel
            List<Step3BChild> children = 
                this.PeekModel().PayingParent.ReceivingParents
                .Where(r => r.Children.Any())
                .SelectMany(r => r.Children)
                .Select(c => new Step3BChild(c.Id, c.FirstName))
                .ToList();

            Step3BViewModel viewModel = new Step3BViewModel(children);
            return View("Step3B", viewModel);
        }

        [HttpPost]
        public IActionResult Step3B(Step3BViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Step3B", vm);
            
            Calculation calculation = this.GetModel();

            //Set the nights per year lower value for each child, using the child ID as the reference
            foreach(ReceivingParent receivingParent in calculation.PayingParent.ReceivingParents)
            {
                foreach(Child child in receivingParent.Children)
                {
                    child.NightsPayingParentCaresForChildPerYearLow = vm.Step3BChildren
                        .First(c => c.Id == child.Id).NightsPayingParentCaresForChildPerYearLow;
                }
            }

            this.StoreModel(calculation);
            return RedirectToAction("Result");
        }

        //Step 4 - How many nights per yer do the children stay with the paying prent (high bracket)
        [HttpGet]
        public IActionResult Step4()
        {
            List<Step4Child> children = 
                this.PeekModel().PayingParent.ReceivingParents
                    .Where(r => r.Children.Any())
                    .SelectMany(r => r.Children)
                    .Select(c => new Step4Child(c.Id, c.FirstName)).ToList();

            Step4ViewModel viewModel = new Step4ViewModel(children);
            return View("Step4", viewModel);
        }

        [HttpPost]
        public IActionResult Step4(Step4ViewModel vm)
        {
            Calculation calculation = this.GetModel();

            foreach (ReceivingParent receivingParent in calculation.PayingParent.ReceivingParents)
            {
                foreach (Child child in receivingParent.Children)
                {
                    child.NightsPayingParentCaresForChildPerYearHigh = vm.Step4Children
                        .First(c => c.Id == child.Id).NightsPayingParentCaresForChildPerYearHigh;
                }
            }

            this.StoreModel(calculation);

            return RedirectToAction("Step5");
        }

        //Step 5 - How many other children does the paying parent support
        [HttpGet]
        public IActionResult Step5()
        {
            Step5ViewModel model = new Step5ViewModel();
            return View("Step5", model);
        }

        [HttpPost]
        public IActionResult Step5(Step5ViewModel vm)
        {
            var calculation = this.GetModel();
            calculation.PayingParent.OtherSupportedChildren = vm.OtherSupportedChildren;
            this.StoreModel(calculation);

            return RedirectToAction("Result");
        }

        [HttpGet]
        public IActionResult Result()
        {
            var calculation = this.GetModel();
            calculation.Calculate();
            this.StoreModel(calculation);

            return View("Result", calculation);
        }

        [HttpGet]
        public IActionResult EmailResult()
        {
            EmailViewModel vm = new EmailViewModel();
            return View("Email", vm);
        }

        [HttpPost]
        public async Task<ContentResult> EmailResult(EmailViewModel model)
        {
            var calculation = this.PeekModel();
            var contextAccessor = new ActionContextAccessor();
            contextAccessor.ActionContext = ControllerContext;

            var vm = new EmailTemplateViewModel()
            {
                Calculation = calculation,
                UserName = model.User.FirstName,
                Associate = false
            };
            if (!String.IsNullOrWhiteSpace(model.User.FirstName))
            {
                vm.RecipientName = model.User.FirstName;
            }

            var byteArray = new byte[0];

            try
            {
                var pdf = new ViewAsPdf("ResultPdfTemplate", calculation)
                {
                    PageSize = Size.A4,
                    FileName = "ChildMaintenanceCalculationResult.pdf",
                    CustomSwitches = _pdfFooter
                };

                var task = pdf.BuildFile(ControllerContext);

                byteArray = task.Result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to render PDF: {nameof(e)}: {e.Message}.");
            }

            var userEmailBody = await _viewRenderService.RenderToStringAsync("ResultEmailTemplate", contextAccessor, vm);

            var userStream = new MemoryStream(byteArray);
            var userAttachment = new Attachment(userStream, "ChildMaintenanceCalculationResult.pdf");

            if(String.IsNullOrEmpty(userEmailBody))
                return Content("Unable to send email(s). Please check the email address(es) entered are valid and try again or use the PDF download option");

            var userSuccess = _emailSenderService.SendEmail(userEmailBody, model.User.EmailAddress, userAttachment);
            userStream.Close();

            var associateSuccess = true;

            vm.Associate = true;

            foreach (var contact in model.Associates.Where(contact => !String.IsNullOrWhiteSpace(contact.EmailAddress)))
            {
                vm.RecipientName = string.IsNullOrWhiteSpace(contact.FirstName) ? string.Empty : contact.FirstName;

                var assEmailBody = await _viewRenderService.RenderToStringAsync("ResultEmailTemplate", contextAccessor, vm);

                var assStream = new MemoryStream(byteArray);
                var assAttachment = new Attachment(assStream, "ChildMaintenanceCalculationResult.pdf");

                associateSuccess = _emailSenderService.SendEmail(assEmailBody, contact.EmailAddress, assAttachment);
                assStream.Close();
            }

            if (userSuccess && associateSuccess)
                return Content("Email(s) sent successfully");

            return Content("Unable to send one or more of your emails. Please check the email address(es) entered are valid and try again or use the PDF download option");
        }

        [HttpGet]
        public IActionResult GetResultAsPdf()
        {
            var model = this.PeekModel();

            return new ViewAsPdf("ResultPdfTemplate", model)
            {
                PageSize = Size.A4,
                FileName = "ChildMaintenanceCalculationResult.pdf",
                CustomSwitches = _pdfFooter
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                string routeWhereExceptionOccurred = exceptionFeature.Path;

                Exception exception = exceptionFeature.Error;

                _logger.LogError($"{nameof(exception)}: {exception.Message}.");
            }

            return View();
        }
    }
}
