using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models.ViewModels
{
    public class Step4ViewModel
    {
        public Step4ViewModel()
        {
        }
        public Step4ViewModel(List<Step4Child> children)
        {
            Step4Children = children;
        }

        public List<Step4Child> Step4Children { get; set; }

    }

    public class Step4Child
    {
        public Step4Child()
        {
        }
        public Step4Child(int id, string fName)
        {
            Id = id;
            FirstName = fName;
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public Child.SharedCare NightsPayingParentCaresForChildPerYearHigh { get; set; }
    }
}
