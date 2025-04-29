using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinishGoodStock.Models
{
    public class GodownLocation
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
    public class GodownLocationValidator : AbstractValidator<GodownLocation>
    {
        public GodownLocationValidator()
        {


            RuleFor(o => o.Name).NotEmpty();


        }
    }
}
