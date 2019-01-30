using ChildMaintenanceCalculator.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtensionMethods
{
    public static class ControllerExtensionMethods
    {
        public static void StoreModel(this Controller instance, CalculatorWrapper model)
        {
            instance.TempData["model"] = JsonConvert.SerializeObject(model);
        }

        public static CalculatorWrapper GetModel(this Controller instance)
        {
            string modelString = instance.TempData["model"] as String;
            //Add null error handling

            CalculatorWrapper model = JsonConvert.DeserializeObject<CalculatorWrapper>(modelString);

            return model;
        }
    }
}
