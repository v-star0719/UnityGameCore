using System;

namespace GameCore.Core
{
    class TimeUtils
    {
        public const long TICKS_19700101 = 621355968000000000;	//C#中1970年的时间ticks，用于处理java时间戳

        public static DateTime GetDateTimeBy1970Utc(long seconds)
        {
            long tricksCSharp = TimeSpan.TicksPerSecond * seconds + TICKS_19700101;
            DateTime wantData = new DateTime(tricksCSharp);
            return wantData;
        }

        public static string GetHmsString(long ticks)
        {
            var hour = ticks / TimeSpan.TicksPerHour;
            ticks -= ticks * TimeSpan.TicksPerHour;
            var minute = ticks / TimeSpan.TicksPerMinute;
            ticks -= ticks * TimeSpan.TicksPerMinute;
            var second = ticks / TimeSpan.TicksPerSecond;
            return $"{hour:00}:{minute:00}:{second:00}";
        }

        public static string GetHmsString(int seconds)
        {
            var hour = seconds / 3600;
            seconds -= seconds * 3600;
            var minute = seconds / 60;
            var second = seconds - seconds * 60;
            return $"{hour:00}:{minute:00}:{second:00}";
        }

        public static string GetMsString(long ticks)
        {
            var minute = ticks / TimeSpan.TicksPerMinute;
            ticks -= ticks * TimeSpan.TicksPerMinute;
            var second = ticks / TimeSpan.TicksPerSecond;
            return $"{minute:00}:{second:00}";
        }

        public static string GetMsString(int seconds)
        {
            var minute = seconds / 60;
            var second = seconds - 60 * minute;
            return $"{minute:00}:{second:00}";
        }

        /// <summary>
        /// 返回从1970/1/1 00:00:00至今的所经过的毫秒数
        /// </summary>
        /// <returns></returns>
        public static long GetDateTime1970Utc()
        {
            return (DateTime.UtcNow.Ticks - TICKS_19700101) / 10000;
        }

        public static string GetTimeString1970(long seconds, string format = "yyyy/MM/dd HH:mm:ss")//yyyy/mm/dd HH:mm:ss
        {
            long tricksCSharp = 10000 * seconds + TICKS_19700101;
            DateTime wantData = new DateTime(tricksCSharp);
            return string.Format("{0:" + format + "}", wantData);
        }

        public static string GetTimeString(DateTime time)
        {
            return time.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}
