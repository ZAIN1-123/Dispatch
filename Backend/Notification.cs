using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISPATCHAPI
{
    public class Notification
    {
        public string DbKey { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string GroupId { get; set; }
        public string ReportName { get; set; }
        public string FileName { get; set; }
        public string Credentials { get; set; } = "";
        public string URL { get; set; } = "";
        public int SendWhatsApp { get; set; }
        public int SendSMS { get; set; }
        public int SendMail { get; set; }
        public string TriggerTime { get; set; }
        public int IncludeToday { get; set; } = 0;
        public int NoOfDays { get; set; } = 1;
    }
}
