using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models
{
    public class CalculatorWrapper
    {
        public PayingParentModel PayingParent = new PayingParentModel();

        public Calculation Calculation = new Calculation();

        public User User = new User();
        //TODO: Build This
    }
}
