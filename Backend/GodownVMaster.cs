using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Models
{
    public class GodownVMaster
    {
        public int Id { get; set; }
        public string ReelNo { get; set; }
        public string Location  { get; set; }
        public int LocationId { get; set; }
        public int SlipId { get; set; }
        public int Date { get; set; }

        //public ObservableCollection<GodownVMasterMeta> locationmeta { get; set; } = new ObservableCollection<GodownVMasterMeta>();
    }
}
