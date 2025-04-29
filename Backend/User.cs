using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DISPATCHAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Auth { get; set; }
        
        public int Role { get; set; }
        public int StartDate { get; set; }
        public int EndDate { get; set; }

    
        public int Godown { get; set; } 
        public int GSM { get; set; } 
        public int Size { get; set; } 
        public int ReelDia { get; set; } 
        public int Business { get; set; } 
        public int BF { get; set; } 
        public int Quality { get; set; } 
        public int Slip { get; set; } 
        public int IsEditAllowed { get; set; } 
        public int IsDeleteAllowed { get; set; } 
        public int BackDateAllowed { get; set; } 
        public int manual { get; set; } 
        public int userallowed { get; set; } 
        public int Reprintallowed { get; set; } 
        public int Dispatch { get; set; } 
        public int Report { get; set; } 
        public int GodownTransfer { get; set; } 
        public int LocationTransfer { get; set; } 
        public int Location { get; set; } 

    }
}
