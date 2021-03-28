using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ChildMaintenanceCalculator.Models.ViewModels
{
    public class EmailViewModel
    {
        public EmailViewModel()
        {
            User = new Contact();
            Associates = new List<Contact>();
            Associates.Add(new Contact());
        }

        public Contact User { get; set; }
        public List<Contact> Associates { get; set; }
    }

    public class Contact
    {
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Email Address")]
        public string EmailAddress { get; set; }
    }
}
