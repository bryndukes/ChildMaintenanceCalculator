using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models.ViewModels
{
    public class Step3BViewModel
    {
        public Step3BViewModel()
        {
            Step3BChildren = new List<Step3BChild>();
            Step3BChildren.Add(new Step3BChild());
        }
        public List<Step3BChild> Step3BChildren { get; set; }
    }

    public class Step3BChild
    {
        public Step3BChild()
        {
            NightsPayingParentCaresForChildPerYearLow = string.Empty;
        }
        public string NightsPayingParentCaresForChildPerYearLow { get; set; }
    }   
}
