
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Models
{
    public class Business
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PrintName { get; set; }
        public int StartDate { get; set; }
        public int EndDate { get; set; }

        public string WhatsappPassword { get; set; }
       // public string WhatsappApi { get; set; }
        public string Whatsappuser { get; set; }
        public string WhatsappGroupId { get; set; }

    }
}
