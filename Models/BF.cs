using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinishGoodStock.Models
{
    public class BF
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class BFValidator : AbstractValidator<BF>
    {
        public BFValidator()
        {
            RuleFor(o => o.Name).NotEmpty();
           
        }
    }
}
