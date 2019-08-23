
using System;

namespace FinTech.Domain
{
    public class TimedReminder
    {
        public TimedReminder() { }
        public TimedReminder(long memberId, DateTime time)
        {
            MemberId = memberId;
            IsEnabled = true;
            SetCron(time);
        }

        public long Id { get; set; } // Id (Primary key)
        public long MemberId { get; set; }
        public bool IsEnabled{ get; set; }
        /// <summary>
        /// ±Ì¥Ô Ω0 27 21 * * ? *
        /// </summary>
        public string Cron { get; set; }
        public DateTime? LastReminderAt { get; set; }

        public void SetCron(DateTime time)
        {
            Cron = $"0 {time.Hour} {time.Minute} * * ? *";
        }

        public void SetEnabled(bool requestSwitch)
        {
            IsEnabled = requestSwitch;
        }

        public void SendSuccess()
        {
            LastReminderAt=DateTime.Now;
        }
    }

}
