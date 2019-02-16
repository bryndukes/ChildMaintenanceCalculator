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
        //Model object must be serialized into a json string before being stored in TempData
        public static void StoreModel(this Controller instance, Calculation model)
        {
            instance.TempData["model"] = JsonConvert.SerializeObject(model);
        }

        //Deserialize string from TempData into a Calculation before returning it to the caller
        public static Calculation GetModel(this Controller instance)
        {
            string modelString = instance.TempData["model"] as String;
            //Add null error handling

            Calculation model = JsonConvert.DeserializeObject<Calculation>(modelString);

            return model;
        }

        //Retrieve the model stored in TempData without marking for deletion after request
        public static Calculation PeekModel(this Controller instance)
        {
            string modelString = instance.TempData.Peek("model") as String;
            //Add null error handling

            Calculation model = JsonConvert.DeserializeObject<Calculation>(modelString);

            return model;
        }
    }
}
