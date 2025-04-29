using FluentValidation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinishGoodStock.Models
{
    public class CutterVMaster
    {
        public int Id { get; set; }
        public int Serial { get; set; }
        public string ReelNo { get; set; }
        public string Cutter { get; set; }
        public int CutterId { get; set; }
        public int SlipId { get; set; }
        public string Quality { get; set; }
        public decimal GSM { get; set; }
        public decimal Size { get; set; }
        public decimal NetWeight { get; set; }

        public int Date { get; set; }
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
        public ObservableCollection<GodownVMasterMeta> locationmeta { get; set; } = new ObservableCollection<GodownVMasterMeta>();

    }

    public class CutterVMasterValidator : AbstractValidator<CutterVMaster>
    {
        public CutterVMasterValidator()
        {

            RuleFor(o => o.CutterId).NotEmpty();
            //RuleFor(o => o.ReelNo).NotEmpty();


        }
    }
}
