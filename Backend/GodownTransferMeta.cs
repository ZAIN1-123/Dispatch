using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Models
{
    public class GodownTransferMeta
    {
        public int Id { get; set; }
        public int GodownTransferId { get; set; }
        public int SlipId { get; set; }
        public string Number { get; set; }
        public string SizeName { get; set; }
        public string ItemName { get; set; }
        public string GSMName { get; set; }
        public string BFName { get; set; }
        public string VNetWeight { get; set; }
    }
}
