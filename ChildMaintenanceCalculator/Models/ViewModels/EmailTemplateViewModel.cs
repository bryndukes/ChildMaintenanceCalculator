using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models.ViewModels
{
    public class EmailTemplateViewModel
    {
        public Calculation Calculation { get; set; }
        public string RecipientName { get; set; }
        public bool Associate { get; set; }
    }
}
