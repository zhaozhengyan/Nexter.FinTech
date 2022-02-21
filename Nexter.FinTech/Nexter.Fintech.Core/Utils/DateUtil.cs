using System;

namespace Nexter.Fintech.Core
{
    public static class DateUtil
    {

        public static DateTime? DayEnd(this DateTime? time)
        {
            if (time == null) return null;
            return new DateTime(time.Value.Year, time.Value.Month, time.Value.Day, 23, 59, 59);
        }
        public static DateTime? DayStart(this DateTime? time)
        {
            if (time == null) return null;
            return new DateTime(time.Value.Year, time.Value.Month, time.Value.Day);
        }
        public static DateTime DayEnd(this DateTime time)
        {
            return new DateTime(time.Year, time.Month, time.Day, 23, 59, 59);
        }
        public static DateTime DayStart(this DateTime time)
        {
            return new DateTime(time.Year, time.Month, time.Day);
        }

        public static int? DaysBetween(this DateTime? newer, DateTime? older)
        {
            if (newer == null || older == null) return null;
            return DaysBetween(newer.Value, older.Value);
        }

        public static DateTime GetDayEnd(DateTime time)
        {
            return new DateTime(time.Year, time.Month, time.Day, 23, 59, 59);
        }

        public static DateTime GetDayStart(DateTime time)
        {
            return new DateTime(time.Year, time.Month, time.Day);
        }

        public static int DaysBetween(this DateTime newer, DateTime older)
        {
            return (int)(
                new DateTime(newer.Year, newer.Month, newer.Day)
                - new DateTime(older.Year, older.Month, older.Day)
                ).TotalDays;
        }

        public static long Epoch => (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;

        public static DateTime ToDateTime(long timestamp)
        {
            if (timestamp > 64060560000)
            {
                return DateTime.MaxValue;
            }
            return new DateTime(timestamp * 10000000 + 621355968000000000, DateTimeKind.Utc).ToLocalTime();
        }

        public static long ToEpoch(DateTime localTime)
        {
            return (localTime.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        public static DateTime FirstDayOfNextMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1);
        }

        public static DateTime LastDayOfThisMonth(this DateTime dateTime)
        {
            return FirstDayOfNextMonth(dateTime).AddDays(-1);
        }

        public static DateTime FirstDayOfThisMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static DateTime LastDayOfPreviousMonth(this DateTime dateTime)
        {
            return FirstDayOfThisMonth(dateTime).AddDays(-1);
        }

        public static int DaysToMonthEnd(this DateTime dateTime)
        {
            return (int)(LastDayOfThisMonth(dateTime) - dateTime).TotalDays;
        }

        public static int DaysFromMonthStart(this DateTime dateTime)
        {
            return (int)(dateTime - (FirstDayOfThisMonth(dateTime))).TotalDays;
        }

        public static int DaysOfMonth(this DateTime dateTime)
        {
            return (int)(FirstDayOfNextMonth(dateTime) - FirstDayOfThisMonth(dateTime)).TotalDays;
        }

        public static DateTime Max(DateTime date1, DateTime date2)
        {
            return date1 >= date2 ? date1 : date2;
        }

        public static DateTime Min(DateTime date1, DateTime date2)
        {
            return date1 <= date2 ? date1 : date2;
        }
    }
}
