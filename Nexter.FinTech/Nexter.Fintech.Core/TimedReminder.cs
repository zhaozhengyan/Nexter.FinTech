
using Furion.DatabaseAccessor;
using NCrontab;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTech.Domain
{
    [Table("TimedReminders")]
    public class TimedReminder : IEntity
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
        /// ����ʽ0 27 21 * * ? *
        /// </summary>
        public string Cron { get; set; }
        public DateTime? LastReminderAt { get; set; }
        /// <summary>
        /// ����ID�����ڷ���ģ������
        /// </summary>
        public string FormId { get; set; }

        public void SetFormId(string formId)
        {
            FormId = formId;
        }

        public void SetCron(DateTime time)
        {
            Cron = $"{time.Minute} {time.Hour} * * *";
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
            FormId = null;
            LastReminderAt = DateTime.Now;
        }

        public void SendFail()
        {
            FormId = null;
        }
    }

}