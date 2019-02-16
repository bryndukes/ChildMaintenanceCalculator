using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models
{
    public class Calculation
    {
        public PayingParent PayingParent = new PayingParent();

        public string RateBand { get; set; } //Should this be a class

        public decimal TotalMaintenancePayable { get; set; }

        public User User = new User();
        //TODO: Build This

        public static void Calculate()
        {

        }
    }
}
