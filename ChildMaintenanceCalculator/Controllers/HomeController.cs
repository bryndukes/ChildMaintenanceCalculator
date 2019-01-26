using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChildMaintenanceCalculator.Models;
using ChildMaintenanceCalculator.Models.ViewModels;

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

        //Step 1
        [HttpGet]
        public IActionResult AddReceivingParentsAndChildren()
        {
            AddReceivingParentsAndChildrenViewModel model = new AddReceivingParentsAndChildrenViewModel();

            return View("AddReceivingParentsAndChildren", model);

        }

        [HttpPost]
        public IActionResult AddReceivingParentsAndChildren(AddReceivingParentsAndChildrenViewModel vm)
        {
            //Take Step1 View Model

            //Add the data to a new Domain Model
            CalculatorWrapper calculatorWrapper = new CalculatorWrapper();
            Mapper.Copy<AddReceivingParentsAndChildrenViewModel, CalculatorWrapper>(vm, calculatorWrapper);

            return View("Index");

            //Store the Domin Model in Temp Data
            //Work out which step should be shown next and redirect to the Get Action for that step
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
