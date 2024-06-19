using System;

namespace Kernel.Core
{
    class TimeUtils
    {
        public const long TICKS_19700101 = 621355968000000000;	//C#中1970年的时间ticks，用于处理java时间戳
        public const long TICKS_SECOND = 1000;
        public const long TICKS_MINUTE = 1000 * 60;
        public const long TICKS_HOUR = 1000 * 60 * 60;
        public const long TICKS_DAY = 1000 * 60 * 60 * 24;

        public static DateTime GetDateTimeBy1970Utc(long seconds)
        {
            long tricksCSharp = 10000 * seconds + TICKS_19700101;
            DateTime wantData = new DateTime(tricksCSharp);
            return wantData;
        }

        public static string GetHmsString(long t)
        {
            long hour = t / TICKS_HOUR;
            long minute = (t % TICKS_HOUR) / TICKS_MINUTE;
            long second = (t % TICKS_MINUTE) / TICKS_SECOND;
            return $"{hour:00}:{minute:00}:{second:00}";
        }

        public static string GetMsString(long t)
        {
            long minute = (t / TICKS_MINUTE);
            long second = (t % TICKS_MINUTE) / TICKS_SECOND;
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
