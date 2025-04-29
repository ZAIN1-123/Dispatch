using System;
using System.Collections.Generic;
using System.Text;

namespace FinishGoodStock.Models
{
    public class BundleDispatch
    {
        public int Id { get; set; }
        public int s { get; set; }
        public int Date { get; set; } 
        public int VNumber { get; set; }
        public int Status { get; set; }
        public int Serial { get; set; }

        public string VehicleNo { get; set; }
        public string Remark { get; set; }
        public List<BundleStock> BundleStock { get; set; } = new List<BundleStock>();
        public bool IsDone
        {
            get
            {
                if (Status == 0)
                {
                    return false;
                }
                else
                {
                    return true;
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


        public int Time { get; set; }
        public int ReelNo { get; set; }
        public int GSMId { get; set; }
        public int BF { get; set; }
        public decimal Weight { get; set; }
        public int GodownId { get; set; }
        public int Quality { get; set; }
        public decimal Wastage { get; set; }
        public string SetNo { get; set; }
        public int EntryTime { get; set; }
        public int EntryDate { get; set; }
        public int EntredBy { get; set; }
        public int Ismanual { get; set; }
        public string ProductionType { get; set; }
        public int SizeId { get; set; }
        public decimal BundleWeight { get; set; }
        public string FormattedNo { get; set; }
        public string BundleNumber { get; set; }
        public string GSM { get; set; }
        public string Size { get; set; }
        public string BFName { get; set; }
        public string ItemName { get; set; }
        public string ShortName { get; set; }
        public string Unit { get; set; }
        public string Unitname { get; set; }
        public string EnteredName { get; set; }
        public string VBundleWeight { get; set; }

        public int WastageType { get; set; }
    }
}
