using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinishGoodStock.Models
{
    public class GSM
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
    public class GSMValidator : AbstractValidator<GSM>
    {
        public GSMValidator()
        {


            RuleFor(o => o.Name).NotEmpty();
           

        }
    }
}
