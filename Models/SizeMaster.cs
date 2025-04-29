using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinishGoodStock.Models
{
    public class SizeMaster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SizeName { get; set; }
        public string Unit { get; set; } = "INCH";

    }

    public class SizeMasterValidator : AbstractValidator<SizeMaster>
    {
        public SizeMasterValidator()
        {


            RuleFor(o => o.Name).NotEmpty();
          
        }
    }
}
