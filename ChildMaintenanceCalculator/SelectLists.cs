using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChildMaintenanceCalculator
{
    public static class SelectLists
    {
        public static IList<SelectListItem> AssociateEmails(IList<string> associateEmails, bool addEmpty, string selected = "")
        {
            var list = new List<SelectListItem>();
            if(addEmpty) list.Add(new SelectListItem());
            list.AddRange(associateEmails.Select(email => new SelectListItem {Selected = selected == email, Text = email, Value = email}));

            return list;
        }
    }
}
