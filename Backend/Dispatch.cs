using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Models
{
    public class Dispatch
    {
        public int Id { get; set; }
        public int Date { get; set; }
        public int DispatchDate { get; set; }
        public int VNumber { get; set; }
        public int Enteredby { get; set; }
        public int CreateDate { get; set; }
        public int FYID { get; set; }
        public int CreateTime { get; set; }
        public int Status { get; set; }
        public string VehicleNo { get; set; }
        public string Remark { get; set; }

        public List<Stock> stock { get; set; }

      
        public int Serial { get; set; }
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
        public decimal NetWeight { get; set; }
        public string FormattedNo { get; set; }
        public string ReelNumber { get; set; }
        public string GSM { get; set; }
        public string Size { get; set; }
        public string BFName { get; set; }
        public string ItemName { get; set; }
        public string ReelDiaName { get; set; }
        public string ShortName { get; set; }
        public string Unit { get; set; }
        public string Unitname { get; set; }
        public string EnteredName { get; set; }
        public string VNetWeight { get; set; }

        public int WastageType { get; set; }
    }
}
