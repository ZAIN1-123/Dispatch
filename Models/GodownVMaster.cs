using FluentValidation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinishGoodStock.Models
{
    public class GodownVMaster
    {
        public int Id { get; set; }
        public int Serial { get; set; }
        public string ReelNo { get; set; }
        public string Location { get; set; }
        public int LocationId { get; set; }
        public int SlipId { get; set; }


        public ObservableCollection<GodownVMasterMeta> locationmeta { get; set; } = new ObservableCollection<GodownVMasterMeta>();

    }

    public class GodownVMasterValidator : AbstractValidator<GodownVMaster>
    {
        public GodownVMasterValidator()
        {

            RuleFor(o => o.LocationId).NotEmpty();
            //RuleFor(o => o.ReelNo).NotEmpty();


        }
    }
}
