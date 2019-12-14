using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models.ViewModels
{
    //TODO: Update all properties to have the name on the Display Attrbiute, and use label for in the view!
    
    public class Step1ViewModel
    {
        public Step1ViewModel()
        {
            //TODO: Fix this it isn't adding a parent
            Step1ReceivingParents = new List<Step1ReceivingParent>();
            Step1ReceivingParents.Add(new Step1ReceivingParent());
        }

        public List<Step1ReceivingParent> Step1ReceivingParents { get; set; }

    }
    public class Step1ReceivingParent
    {
        public Step1ReceivingParent()
        {
            FirstName = string.Empty;
            Step1Children = new List<Step1Child>();
            Step1Children.Add(new Step1Child());
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public List<Step1Child> Step1Children { get; set; }
    }
    public class Step1Child
    {
        private static int instanceCounter; //TODO: Fix Id Counter
        public Step1Child()
        {
            FirstName = string.Empty;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public bool PreExistingArrangements { get; set; }

        [Display(Prompt = "0.00")]
        public decimal? PreExisingArrangementsAmount { get; set; }
    }
}
