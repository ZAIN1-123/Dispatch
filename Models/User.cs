using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinishGoodStock.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public int Godown { get; set; } = 0;
        public int GSM { get; set; } = 0;
        public int GodownTransfer { get; set; } = 0;
        public int LocationTransfer { get; set; } = 0;
        public int Size { get; set; } = 0;
        public int Business { get; set; } = 0;
        public int BF { get; set; } = 0;
        public int Quality { get; set; } = 0;
        public int Slip { get; set; } = 0;
        public int IsEditAllowed { get; set; } = 0;
        public int IsDeleteAllowed { get; set; } = 0;
        public int BackDateAllowed { get; set; } = 0;
        public int manual { get; set; } = 0;
        public int userallowed { get; set; } = 0;
        public int Reprintallowed { get; set; } = 0;
        public int Dispatch { get; set; } = 0;
        public int Report { get; set; } = 0;
        public int ReelDia { get; set; } = 0;
        public int Location { get; set; } = 0;


    }
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(o => o.UserName).NotEmpty();
         

        }
    }
}
