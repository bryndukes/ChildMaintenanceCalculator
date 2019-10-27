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

        public SharedCare NightsPayingParentCaresForChildPerYearLow { get; set; }

        public SharedCare NightsPayingParentCaresForChildPerYearHigh { get; set; }

        public bool PreExistingMaintenanceArrangements { get; set; }

        public decimal PreExistingMaintenanceArrangementsAmount { get; set; }
        
        //Before Deduction of Pre-existing Arrangements
        private decimal childMaintenanceAmount;
        public decimal ChildMaintenanceAmount
        {
            get { return childMaintenanceAmount; }

            set { childMaintenanceAmount = value; }
        }

        //Need to add a final child amount after deductions?

        public enum SharedCare
        {
            LessThan52 = 1,
            MoreThanOrEqualTo52 = 2,
            From52To103 = 3,
            From104To155 = 4,
            From156To174 = 5,
            MoreThan175 = 6
        }

    }

}
