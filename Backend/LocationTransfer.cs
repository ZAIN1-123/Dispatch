using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Models
{
    public class LocationTransfer
    {
        public int Id { get; set; }
        public int Date { get; set; }
        public int FromLocation { get; set; }
        public int ToLocation { get; set; }
        public string Number { get; set; }
        public string Remark { get; set; }
        public string FromLocationName { get; set; }
        public string ToLocationName { get; set; }


        public string SizeName { get; set; }
        public string ItemName { get; set; }
        public string GSMName { get; set; }
        public string ReelDiaName { get; set; }
        public decimal VNetWeight { get; set; }
        public List<LocationTransferMeta> locationmeta { get; set; }
    }
}
