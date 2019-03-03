using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models.ViewModels
{
    public class Step1ViewModel
    {
        public Step1ViewModel()
        {
            Step1ReceivingParents = new List<Step1ReceivingParent>();
            Step1ReceivingParents.Add(new Step1ReceivingParent());
            Step1ReceivingParents.Add(new Step1ReceivingParent()); //FOR TESTING - REMOVE ONCE ADD AND REMOVE BUTTONS ARE IMPLEMENTED
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
            Step1Children.Add(new Step1Child()); //FOR TESTING - REMOVE ONCE ADD AND REMOVE BUTTONS ARE IMPLEMENTED
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
        public decimal PreExisingArrangementsAmount { get; set; }
    }
}
