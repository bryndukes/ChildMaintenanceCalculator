using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models
{
    public class User
    {
        public string FirstName { get; set; }

        public string EmailAddress { get; set; }

        public List<string> AssociateEmailAddressList { get; set; }

    }
}
