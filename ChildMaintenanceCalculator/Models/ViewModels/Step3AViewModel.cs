using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models.ViewModels
{
    public class Step3AViewModel
    {
        [Display(Prompt = "0.00")]
        public decimal? PayingParentAnnualIncome { get; set; }

        [Display(Prompt = "0.00")]
        public decimal? PayingParentAnnualPension { get; set; }
    }
}
