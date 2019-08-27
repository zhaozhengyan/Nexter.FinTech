
using NCrontab;
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
        public bool IsEnabled { get; set; }
        /// <summary>
        /// 表达式0 27 21 * * ? *
        /// </summary>
        public string Cron { get; set; }
        public DateTime? LastReminderAt { get; set; }
        /// <summary>
        /// 表单ID，用于发送模板提醒
        /// </summary>
        public string FormId { get; set; }

        public void SetFormId(string formId)
        {
            FormId = formId;
        }

        public void SetCron(DateTime time)
        {
            //0 1 21 1/1 * ? 
            Cron = $"* {time.Minute} {time.Hour} * *";
        }

        public DateTime? GetNexterExecuteTime()
        {
            var schedule = CrontabSchedule.Parse(Cron);
            var nextTime = schedule.GetNextOccurrence(LastReminderAt ?? DateTime.Now);
            return nextTime;
        }

        public void SetEnabled(bool requestSwitch)
        {
            IsEnabled = requestSwitch;
        }

        public void SendSuccess()
        {
            LastReminderAt = DateTime.Now;
        }

        public void ClearFormId()
        {
            FormId = null;
        }
    }

}
