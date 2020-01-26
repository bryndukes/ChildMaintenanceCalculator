using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ChildMaintenanceCalculator.Attributes;

namespace ChildMaintenanceCalculator.Models.ViewModels
{
    //TODO: Update all properties to have the name on the Display Attrbiute, and use label for in the view!
    
    public class Step1ViewModel
    {
        public Step1ViewModel()
        {
            Step1ReceivingParents = new List<Step1ReceivingParent>();
            Step1ReceivingParents.Add(new Step1ReceivingParent());
        }

        [MinListLength(1, ErrorMessage = "Please add at least one receiving parent")]
        [MaxListLength(10, ErrorMessage = "Maximum 10 receiving parents")]
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

        [Required(ErrorMessage = "Please enter a first name")]
        public string FirstName { get; set; }

        [MinListLength(1, ErrorMessage = "Please add at least one child per receiving parent")]
        [MaxListLength(10, ErrorMessage = "Maximum 10 children per receiving parent")]
        public List<Step1Child> Step1Children { get; set; }
    }

    public class Step1Child
    {
        public Step1Child()
        {
            FirstName = string.Empty;
            PreExistingArrangements = false;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please select yes or no to pre-existing arrangements")]
        public bool PreExistingArrangements { get; set; }

        [Display(Prompt = "0.00")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid amount")] 
        public decimal? PreExisingArrangementsAmount { get; set; }
    }
}
