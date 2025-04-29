using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Models
{
    public class GodownTransfer
    {
        public int Id { get; set; }
        public int Date { get; set; }
        public int FromGodown { get; set; }
        public int ToGodown { get; set; }
        public string Number { get; set; }
        public string Remark { get; set; }
        public string FromGodownName { get; set; }
        public string ToGodownName { get; set; }

     
        public string SizeName { get; set; }
        public string ItemName { get; set; }
        public string GSMName { get; set; }
        public string BFName { get; set; }
        public decimal VNetWeight { get; set; }
        public List<GodownTransferMeta> godownmeta { get; set; }
    }
}
