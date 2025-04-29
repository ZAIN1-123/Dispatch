using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinishGoodStock.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Roles { get; set; }

    }
    public class RoleValidator : AbstractValidator<Role>
    {
        public RoleValidator()
        {


            RuleFor(o => o.Roles).NotEmpty();
          
        }
    }
}
