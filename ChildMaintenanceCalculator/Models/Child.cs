using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models
{

    public class Child
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string NameOfReceivingParent { get; set; }

        public string NightsPayingParentCaresForChildPerYearLow { get; set; }

        public string NightsPayingParentCaresForChildPerYearHigh { get; set; }

        public bool PreExistingMaintenanceArrangements { get; set; }

        public decimal PreExistingMaintenanceArrangementsAmount { get; set; }
        
        //Before Deduction of Pre-existing Arrangements
        private decimal childMaintenanceAmount;
        public decimal ChildMaintenanceAmount
        {
            get { return Math.Round(childMaintenanceAmount, 2); }

            set { childMaintenanceAmount = value; }
        }

        //Need to add a final child amount after deductions?
    }

}
