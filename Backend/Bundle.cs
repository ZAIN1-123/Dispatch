using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Models
{
    public class Bundle
    {
        public int Date { get; set; }
        public int BundleDate { get; set; }
        public int SlipId { get; set; }
        public int Id { get; set; }
        public int Time { get; set; }
        public string Quantity { get; set; }
        public string QualityName { get; set; }
        //public string BundleNo { get; set; }
        public string SStatus { get; set; }
        public int BundleNo { get; set; }
        public int GSMId { get; set; }
        public int BF { get; set; }

        public decimal Weight { get; set; }
        public int GodownId { get; set; }
        public int Quality { get; set; }
        public decimal RimWeight { get; set; }
        public int NoOfRim { get; set; }
        public decimal BundleWeight { get; set; }

        public int EntryTime { get; set; }
        public int EntryDate { get; set; }
        public int EntredBy { get; set; }

        public int BundleSizeId { get; set; }
        public decimal NetWeight { get; set; }
        public string FormattedNo { get; set; }
        public string ReelNumber { get; set; }
        public string GSM { get; set; }
        public string Size { get; set; }
        public string BFName { get; set; }
        public string ReelDiaName { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public string Unitname { get; set; }
        public string EnteredName { get; set; }
        public string GodownName { get; set; }
        public int WastageType { get; set; }
        public int ReelNum { get; set; }
        public int SlipStatus { get; set; }

        //public string RollNo { get; set; }
        public string SlipIdV
        {
            get
            {
                return SlipId.ToString();
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

        public string BundleDateV
        {
            get
            {
                if (BundleDate == 0)
                {
                    return DateTime.Now.ToString("dd-MMM-yyyy");
                }
                else
                {
                    return BundleDate.ToDate().ToString("dd-MMM-yyyy");
                }
            }
        }

    }
}
