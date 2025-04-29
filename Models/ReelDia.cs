using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinishGoodStock.Models
{
    public class ReelDia
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; } = "CM";

    }
    public class ReelDiaValidator : AbstractValidator<ReelDia>
    {
        public ReelDiaValidator()
        {
            RuleFor(o => o.Name).NotEmpty();

        }
    }
}
