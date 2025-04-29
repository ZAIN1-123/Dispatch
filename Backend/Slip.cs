using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Models
{
    public class Slip
    {
        public int Date { get; set; }
        public int SlipDate { get; set; }
        public int SlipId { get; set; }
        public int Id { get; set; }
        public int Time { get; set; }
        public string Quantity { get; set; }
        public string SStatus { get; set; }
        public string Shift { get; set; }
        public int ReelNo { get; set; }
        //public string RollNo { get; set; }
        public int GSMId { get; set; }
        public int BF { get; set; }
        public decimal Weight { get; set; }
        public int GodownId { get; set; }
        public int Quality { get; set; }
        public int ReelDia { get; set; }
        public decimal Wastage { get; set; }
        public string SetNo { get; set; }
        public string Unit1 { get; set; }
        public int EntryTime { get; set; }
        public int EntryDate { get; set; }
        public int EntredBy { get; set; }
        public string VNetWeight { get; set; }

        public int Ismanual { get; set; }
        public string GodownName { get; set; }
        public string ProductionType { get; set; }
        public int SizeId { get; set; }
        public int OpeningBalanceReels { get; set; }
        public int ProductionReels { get; set; }
        public int ProductionNetWeight { get; set; }
        public int OpeningBalanceNetWeight { get; set; }
        public int DispatchNetWeight { get; set; }
        public int TotalReels { get; set; }
        public int DispatchReels { get; set; }
        public int TotalNetWeight { get; set; }
        public decimal NetWeight { get; set; }
        public string FormattedNo { get; set; }
        public string QualityName { get; set; }
        public string ReelNumber { get; set; }
        public string GSM { get; set; }
        public string Size { get; set; }
        public string BFName { get; set; }
        public string ItemName { get; set; }
        public string ReelDiaName { get; set; } 
        public string Unit { get; set; }
        public string Unitname { get; set; }
        public string EnteredName { get; set; }
        public int WastageType { get; set; }
        public int ReelNum { get; set; }
        public int SlipStatus { get; set; }
        
        


    }
}
