using System;

namespace Nexter.Fintech.Application
{
    public class TimedReminderRequest
    {
        public bool IsEnabled { get; set; }
        public DateTime Time { get; set; }
        public string FormId { get; set; }
    }
}