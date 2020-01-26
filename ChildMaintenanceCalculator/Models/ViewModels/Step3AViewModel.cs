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
        [Required(ErrorMessage = "Please enter an income amount")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid amount")] 
        public decimal? PayingParentAnnualIncome { get; set; }

        [Display(Prompt = "0.00")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid amount")] 
        public decimal? PayingParentAnnualPension { get; set; }
    }
}
