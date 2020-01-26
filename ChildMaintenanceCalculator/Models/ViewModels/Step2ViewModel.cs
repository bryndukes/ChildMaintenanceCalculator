using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models.ViewModels
{
    public class Step2ViewModel
    {
        [Required(ErrorMessage = "Please select yes or no")]
        public bool PayingParentReceivesBenefit { get; set; }

    }
}
