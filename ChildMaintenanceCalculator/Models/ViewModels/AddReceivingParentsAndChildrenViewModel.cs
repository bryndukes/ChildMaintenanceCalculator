using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models.ViewModels
{
    public class AddReceivingParentsAndChildrenViewModel
    {
        //Do I need a constructor to instatiate the lists and add at least one item to each?
        public AddReceivingParentsAndChildrenViewModel()
        {
            AddReceivingParentViewModels = new List<AddReceivingParentViewModel>();
            AddReceivingParentViewModels.Add(new AddReceivingParentViewModel());
        }

        public List<AddReceivingParentViewModel> AddReceivingParentViewModels { get; set; }

    }
    public class AddReceivingParentViewModel
    {
        public AddReceivingParentViewModel()
        {
            FirstName = string.Empty;
            AddChildViewModels = new List<AddChildViewModel>();
            AddChildViewModels.Add(new AddChildViewModel());
        }

        public string FirstName { get; set; }
        public List<AddChildViewModel> AddChildViewModels { get; set; }
    }
    public class AddChildViewModel
    {
        public AddChildViewModel()
        {
            FirstName = string.Empty;
        }

        public string FirstName { get; set; }
    }
}
