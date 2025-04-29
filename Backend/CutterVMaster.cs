using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Models
{
    public class CutterVMaster
    {
        public int Id { get; set; }
        public int Serial { get; set; }
        public string ReelNo { get; set; }
        public string Cutter { get; set; }
        public string Quality { get; set; }
        public decimal GSM { get; set; }
        public decimal Size { get; set; }
        public decimal NetWeight { get; set; }
        public int CutterId { get; set; }
        public int SlipId { get; set; }

        public int Date { get; set; }

        //public ObservableCollection<GodownVMasterMeta> locationmeta { get; set; } = new ObservableCollection<GodownVMasterMeta>();
    }
}
