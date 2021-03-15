using System;

namespace DashboardETL
{
    public static class DateTimeUtils
    {
        private static DateTime first = new DateTime(2010, 1, 1);
        private static DateTime unix = DateTime.Parse("1970-01-01");

        public static int GetDateId(this DateTime date)
        {
            var dateId = (int)(date - first).TotalSeconds;
            if (dateId <= 0) throw new ApplicationException($"Wrong date {date.ToString()}");

            return dateId;
        }

        public static string SqlDateFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public static long UnixTime()
        {
            return (long)(new DateTime() - unix).TotalSeconds;
        }
    }
}
