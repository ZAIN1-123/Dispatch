using System;
using System.Collections.Generic;
using System.Text;

namespace FinishGoodStock.Models
{
    public class LocationTransferMeta
    {
        public int Id { get; set; }
        public int Serial { get; set; }
        public int LocationTransferId { get; set; }
        public string Number { get; set; }
        public string VNetWeight { get; set; }
        public string GSMName { get; set; }
        public string SizeName { get; set; }
        public string BFName { get; set; }
        public string ReelDiaName { get; set; }
        public string ItemName { get; set; }
        public int SlipId { get; set; }
    }
}
