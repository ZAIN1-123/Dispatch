using System;
using System.Collections.Generic;
using System.Text;

namespace FinishGoodStock.Models
{
    public class BundleStock
    {

        public int Id { get; set; }
        public int Serial { get; set; }
        public int Date { get; set; }
        public int BundleDispatchId { get; set; }
        public int Status { get; set; }
        public int BundleId { get; set; }
        public int Godown { get; set; }
        public string VehicleNo { get; set; }
        public string GodownName { get; set; }
        public string ReelDiaName { get; set; }
        public string ReelNumber { get; set; }
        public string Number { get; set; }
        public string GSMName { get; set; }
        public string SizeName { get; set; }
        public string BFName { get; set; }
        public string ItemName { get; set; }
        public string VBundleWeight { get; set; }
        public string VNetWeight1 { get; set; }
        public string BundleNo { get; set; }
        public string Quantity { get; set; }
        public string Quantity1 { get; set; }
        public string SlipId1 { get; set; }
        public int SlipDate { get; set; }
        public string SlipDateV
        {
            get
            {
                if (SlipDate == 0)
                {
                    return DateTime.Now.ToString("dd-MMM-yyyy");
                }
                else
                {
                    return SlipDate.ToDate().ToString("dd-MMM-yyyy");
                }
            }

        }
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
