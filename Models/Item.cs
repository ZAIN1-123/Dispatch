using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinishGoodStock.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public decimal Rate { get; set; }
        public int Status { get; set; }

    }
    public class ItemValidator : AbstractValidator<Item>
    {
        public ItemValidator()
        {


            RuleFor(o => o.Name).NotEmpty();
           

        }
    }
}
