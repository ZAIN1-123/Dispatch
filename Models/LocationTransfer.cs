using System;
using System.Collections.Generic;
using System.Text;

namespace FinishGoodStock.Models
{
    public class LocationTransfer
    {
        public int Id { get; set; }
        public int Date { get; set; }
        public int FromLocation { get; set; }
        public int ToLocation { get; set; }
        public int SlipId { get; set; }
        public string Number { get; set; }
        public string SizeName { get; set; }
        public string Remark { get; set; }
        public string ItemName { get; set; }
        public string GSMName { get; set; }
        public string ReelDiaName { get; set; }
        public string ToLocationName { get; set; }
        public string FromLocationName { get; set; }
        public int s { get; set; }
        public decimal VNetWeight { get; set; }
        public List<LocationTransferMeta> locationmeta { get; set; } = new List<LocationTransferMeta>();
        public string DateV
        {
            get
            {
                if (Date == 0)
                {
                    return DateTime.Now.ToString("dd-MMM-yyyy");
                }
                else
                {
                    return Date.ToDate().ToString("dd-MMM-yyyy");
                }
            }
        }

    }
}
