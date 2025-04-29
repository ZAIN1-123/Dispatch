using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinishGoodStock.Models
{
    public class BundleSizeMaster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SizeName { get; set; }
        public string Unit { get; set; } = "INCH";

    }

    public class BundleSizeMasterValidator : AbstractValidator<BundleSizeMaster>
    {
        public BundleSizeMasterValidator()
        {


            RuleFor(o => o.Name).NotEmpty();

        }
    }
}
