using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models
{
    public class PayingParent
    {
        public bool RelevantBenefit { get; set; }

        public decimal AnnualIncome { get; set; }

        public decimal AnnualPension { get; set; }

        public decimal GrossWeeklyIncome { get; set; }

        public decimal ReducedWeeklyIncome { get; set; } //After care reductions

        public int OtherSupportedChildren { get; set; }

        public List<ReceivingParent> ReceivingParents { get; set; }
        
    }
}
