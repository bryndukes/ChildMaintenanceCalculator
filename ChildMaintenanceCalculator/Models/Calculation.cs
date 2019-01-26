using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models
{
    public class Calculation
    {
        public string RateBand { get; set; } //Should this be a class
        public decimal TotalMaintenancePayable { get; set; }
    }

}
