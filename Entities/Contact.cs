using CMS.Core.Features;
using CMS.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CMS.Core.Entities.Chat
{
    public  class Contact
    {
        public string Account { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
    }
}
