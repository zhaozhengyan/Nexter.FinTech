using System;

namespace FinTech.API.Wechat.Dto
{
    public class TimedReminderRequest
    {
        public bool IsEnabled { get; set; }
        public DateTime Time { get; set; }
        public string FormId { get; set; }
    }
}