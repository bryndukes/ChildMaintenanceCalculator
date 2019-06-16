using ChildMaintenanceCalculator.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

    public static class HtmlHelperExtensionMethods
    {
        public static ModelExplorer GetModelExplorer<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            return ExpressionMetadataProvider.FromLambdaExpression(expression, htmlHelper.ViewData, htmlHelper.MetadataProvider);
        }

        public static IHtmlContent PartialFor<TModel, TResult>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, string partialViewName, string prefix = "")
        {
            var modelExplorer = helper.GetModelExplorer(expression);
            var viewData = new ViewDataDictionary(helper.ViewData);
            viewData.TemplateInfo.HtmlFieldPrefix += prefix;
            viewData["firstChild"] = true;
            return helper.Partial(partialViewName, modelExplorer.Model, viewData);
        }
    }

}
