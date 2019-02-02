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
        // will be bound to the view model. The data from the view model will then be transferred to the domain model using AutoMapper.
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
            CalculatorWrapper calculatorWrapper = new CalculatorWrapper();

            //Map data from view model to domain model - This needs to be separated into a mapper class or use AutoMapper
            calculatorWrapper.PayingParent.ReceivingParents = vm.Step1ReceivingParents.Select(r => new ReceivingParent
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
            this.StoreModel(calculatorWrapper);

            //Work out which step should be shown next and redirect to the Get Action for that step
            return RedirectToAction("Step2");

        }

        [HttpGet]
        public IActionResult Step2()
        {
            Step2ViewModel viewModel = new Step2ViewModel();

            return View("Step2", viewModel);
        }

        [HttpPost]
        public IActionResult Step2(Step2ViewModel vm)
        {
            //Get data from TempData into calculatorWrapper
            CalculatorWrapper calculatorWrapper = this.GetModel();
            // Add null error handling here

            //Add data from view model into calculatorWrapper
            calculatorWrapper.PayingParent.RelevantBenefit = vm.PayingParentReceivesBenefit;

            //Put the calculatorWrapper back into TempData
            this.StoreModel(calculatorWrapper);

            //Decide which step to go to next
            if(calculatorWrapper.PayingParent.RelevantBenefit == false)
            {
                return RedirectToAction("Step3A");
            }
            else if(calculatorWrapper.PayingParent.RelevantBenefit == true)
            {
                return RedirectToAction("Step3B");
            }
            else
            {
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Step3A()
        {
            Step3AViewModel viewModel = new Step3AViewModel();
            return View("Step3A", viewModel);
        }

        [HttpPost]
        public IActionResult Step3A(Step3AViewModel vm)
        {
            CalculatorWrapper calculatorWrapper = this.GetModel();

            calculatorWrapper.PayingParent.AnnualIncome = vm.PayingParentAnnualIncome;
            calculatorWrapper.PayingParent.AnnualPension = vm.PayingParentAnnualPension;

            this.StoreModel(calculatorWrapper);

            //Decide what comes next
            return View("Index");
        }

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
            CalculatorWrapper calculatorWrapper = this.GetModel();

            //Set the nights per year lower value for each child, using the child ID as the reference
            foreach(ReceivingParent receivingParent in calculatorWrapper.PayingParent.ReceivingParents) //TODO:Improve this with Linq
            {
                foreach(Child child in receivingParent.Children)
                {
                    child.NightsPayingParentCaresForChildPerYearLow = vm.Step3BChildren.First(c => c.Id == child.Id).NightsPayingParentCaresForChildPerYearLow;
                }
            }

            this.StoreModel(calculatorWrapper);

            return View("index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
