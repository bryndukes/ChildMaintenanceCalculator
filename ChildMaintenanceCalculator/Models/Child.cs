using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            [Description("Less than 52 (less than once a week)")]
            LessThan52 = 1,
            [Description("More than 52 (once a week or more)")]
            MoreThanOrEqualTo52 = 2,
            [Description("52 - 103 (approximately once a week)")]
            From52To103 = 3,
            [Description("104 - 155 (approximately twice a week)")]
            From104To155 = 4,
            [Description("156 - 174 (approximately three times a week)")]
            From156To174 = 5,
            [Description("More than 175 (more than three times a week)")]
            MoreThan175 = 6
        }

    }

}
