using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models
{

    public class ReceivingParent
    {
        public string FirstName { get; set; }

        public List<Child> Children { get; set; }

        public decimal MaintenanceEntitlementAmount { get; set; }
    }
}
