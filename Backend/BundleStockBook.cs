using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Models
{
    public class BundleStockBook
    {
        public int Id { get; set; }
        public int Date { get; set; }
        public int BundleId { get; set; }
        public decimal Quantity { get; set; }
        public string BundleNo { get; set; }
        public string BundleNumber { get; set; }
        public string Item { get; set; }
        public string Location { get; set; }
        public int Quality { get; set; }
        public int BF { get; set; }
        public int GSM { get; set; }
        public int BundleSize { get; set; }
        public int TotalQuantity { get; set; }
        public string Unit { get; set; }
        public string Shift { get; set; }
        public string VoucherType { get; set; }
      
        public int VoucherId { get; set; }
        public int SlipId { get; set; }       
        public decimal OpnQty { get; set; }
        public decimal OpnWht { get; set; }
        public decimal InwardQty { get; set; }
        public decimal CWeight { get; set; }
        public decimal CQuantity { get; set; }
        public decimal ClsWht { get; set; }
        public decimal ClsQty { get; set; }

        public decimal TotalQty { get; set; }
        public decimal TotalWht { get; set; }
        public decimal OutwardQty { get; set; }
        public decimal OutwardWht { get; set; }
        public string Qty { get; set; }
        public decimal BundleWeight { get; set; }
        public decimal NoOfRim { get; set; }
        public decimal RimWeight { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal Weight { get; set; }
        public decimal OQuantity { get; set; }
        public decimal OWeight { get; set; }
        public decimal InQuantity { get; set; }
        public decimal RWeight { get; set; }
        public decimal RQuantity { get; set; }
        public decimal RepackWeight { get; set; }
        public decimal RepackQuantity { get; set; }
        public decimal Inweight { get; set; }
        public decimal InwardWht { get; set; }
        public decimal IssueQuantity { get; set; }
        public decimal IssueWeight { get; set; }
        public decimal IssueQty { get; set; }
        public decimal IssueWht { get; set; }
        public string VNetWeight { get; set; }

        public string SizeName { get; set; }
        public string ItemName { get; set; }
        public string GSMName { get; set; }
        public string BFName { get; set; }
        public string ReelDiaName { get; set; }

        public decimal Amount { get; set; }
        public decimal Rate { get; set; }

        public string RollNo { get; set; }

    }
}
