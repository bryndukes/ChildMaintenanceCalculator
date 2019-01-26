using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models
{

    public class Child
    {
        public string FirstName { get; set; }

        public string NameOfReceivingParent { get; set; }

        public string NightsPayingParentCaresForChildPerYearLow { get; set; }

        public string NightsPayingParentCaresForChildPerYearHigh { get; set; }

        public bool PreExistingMaintenanceArrangements { get; set; }

        public decimal PreExistingMaintenanceArrangementsAmount { get; set; }

        public decimal ChildMaintenanceAmount { get; set; } //Before Deduction of Pre-existing Arrangements
        //Need to add a final child amount after deductions?
    }

}
