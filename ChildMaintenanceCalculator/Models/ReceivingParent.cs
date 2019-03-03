using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models
{

    public class ReceivingParent
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public List<Child> Children { get; set; }

        public decimal MaintenanceEntitlementAmount
        {
            get { return Math.Round(Children.Sum(c => c.ChildMaintenanceAmount), 2); }
        }
    }
}
