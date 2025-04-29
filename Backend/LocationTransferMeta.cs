using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Models
{
    public class LocationTransferMeta
    {
        public int Id { get; set; }
        public int LocationTransferId { get; set; }
        public int SlipId { get; set; }
        public string Number { get; set; }
        public string SizeName { get; set; }
        public string ItemName { get; set; }
        public string GSMName { get; set; }
        public string ReelDiaName { get; set; }
        public string VNetWeight { get; set; }
    }
}
