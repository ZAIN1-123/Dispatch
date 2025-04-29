using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinishGoodStock.Models
{
    public class Godown
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
    public class GodownValidator : AbstractValidator<Godown>
    {
        public GodownValidator()
        {


            RuleFor(o => o.Name).NotEmpty();
           

        }
    }
}
