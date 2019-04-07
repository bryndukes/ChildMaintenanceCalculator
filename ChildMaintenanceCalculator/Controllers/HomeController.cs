using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChildMaintenanceCalculator.Models;
using ChildMaintenanceCalculator.Models.ViewModels;
using ExtensionMethods;

namespace ChildMaintenanceCalculator.Controllers
{
    public class HomeController : Controller
    {
        //Each step in the form has an individual view and view model. When that step of the form is submitted with the 'next' button,
        // will be bound to the view model. The data from the view model will then be transferred to the domain model.
        // The domain model will then be stored in TempData, and the post action will redirect to the get action for the next step.

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
            //Take Step1 View Model and validate

            //Create a new Domain Model instance
            Calculation calculation = new Calculation();

            //Map data from view model to domain model - This needs to be separated into a mapper class or use AutoMapper
            calculation.PayingParent.ReceivingParents = vm.Step1ReceivingParents.Select(r => new ReceivingParent
            {
                Id = r.Id,
                FirstName = r.FirstName,
                Children = r.Step1Children.Select(x => new Child
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    PreExistingMaintenanceArrangements = x.PreExistingArrangements,
                    PreExistingMaintenanceArrangementsAmount = x.PreExisingArrangementsAmount
                }).ToList()
            }).ToList();

            //Store the Domin Model in Temp Data
            this.StoreModel(calculation);

            //Work out which step should be shown next and redirect to the Get Action for that step
            return RedirectToAction("Step2");

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
            //Get data from TempData into Calculation
            Calculation calculation = this.GetModel();
            // Add null error handling here

            //Add data from view model into Calculation
            calculation.PayingParent.RelevantBenefit = vm.PayingParentReceivesBenefit;

            //Put the Calculation back into TempData
            this.StoreModel(calculation);

            //Decide which step to go to next
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
            Calculation calculation = this.GetModel();

            calculation.PayingParent.AnnualIncome = vm.PayingParentAnnualIncome;
            calculation.PayingParent.AnnualPension = vm.PayingParentAnnualPension;

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
            List<Step3BChild> children = this.PeekModel().PayingParent.ReceivingParents.Where(r => r.Children.Any()).SelectMany(r => r.Children).Select(c => new Step3BChild(c.Id, c.FirstName)).ToList();
            Step3BViewModel viewModel = new Step3BViewModel(children);
            return View("Step3B", viewModel);
        }

        [HttpPost]
        public IActionResult Step3B(Step3BViewModel vm)
        {
            Calculation calculation = this.GetModel();

            //Set the nights per year lower value for each child, using the child ID as the reference
            foreach(ReceivingParent receivingParent in calculation.PayingParent.ReceivingParents) //TODO:Improve this with Linq
            {
                foreach(Child child in receivingParent.Children)
                {
                    child.NightsPayingParentCaresForChildPerYearLow = vm.Step3BChildren.First(c => c.Id == child.Id).NightsPayingParentCaresForChildPerYearLow;
                }
            }

            this.StoreModel(calculation);
            return RedirectToAction("Result");
        }

        //Step 4 - How many nights per yer do the children stay with the paying prent (high bracket)
        [HttpGet]
        public IActionResult Step4()
        {
            List<Step4Child> children = this.PeekModel().PayingParent.ReceivingParents.Where(r => r.Children.Any()).SelectMany(r => r.Children).Select(c => new Step4Child(c.Id, c.FirstName)).ToList();
            Step4ViewModel viewModel = new Step4ViewModel(children);
            return View("Step4", viewModel);
        }

        [HttpPost]
        public IActionResult Step4(Step4ViewModel vm)
        {
            Calculation calculation = this.GetModel();

            foreach (ReceivingParent receivingParent in calculation.PayingParent.ReceivingParents) //TODO:Improve this with Linq
            {
                foreach (Child child in receivingParent.Children)
                {
                    child.NightsPayingParentCaresForChildPerYearHigh = vm.Step4Children.First(c => c.Id == child.Id).NightsPayingParentCaresForChildPerYearHigh;
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

            return View("Result", calculation);
        }

        [HttpPost]
        public IActionResult Step1AddNewReceivingParent(int index)
        {

            //Create new item
            var newReceivingParent = new Step1ReceivingParent();
            ViewData["receivingParentIndex"] = index;

            //Return partial for modified list
            return PartialView("_AddReceivingParentPartial", newReceivingParent);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
