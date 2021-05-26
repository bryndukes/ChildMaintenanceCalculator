using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models.ViewModels
{
    public class Step3BViewModel
    {
        public Step3BViewModel()
        {
        }
        public Step3BViewModel(List<Step3BChild> children)
        {
            Step3BChildren = children;
        }

        public List<Step3BChild> Step3BChildren { get; set; }
    }

    public class Step3BChild
    {
        public Step3BChild()
        {
        }
        public Step3BChild(int id, string fName, Child.SharedCare sharedCare = Child.SharedCare.LessThan52)
        {
            Id = id;
            FirstName = fName;
            NightsPayingParentCaresForChildPerYearLow = sharedCare;
        }
        public int Id { get; set; }
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please make a selection")]
        public Child.SharedCare NightsPayingParentCaresForChildPerYearLow { get; set; }
    }   
}
