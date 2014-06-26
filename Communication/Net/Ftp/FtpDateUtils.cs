using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Zongsoft.Communication.Net.Ftp
{
    internal static class FtpDateUtils
    {
        private const string FtpDataFormats = "yyyyMMddHHmmss";

        /// <summary>
        /// 格式化成Unix样式的时间
        /// </summary>
        public static string FormatUnixDate(DateTime dateTime)
        {
            string mouth = dateTime.ToString("MMM", new CultureInfo("en-US", false).DateTimeFormat);
            if (dateTime.Year == DateTime.Now.Year)
                return string.Format("{0} {1:00} {2:00}:{3:00}", mouth, dateTime.Day, dateTime.Hour, dateTime.Minute);
            else
                return string.Format("{0} {1:00} {2}", mouth, dateTime.Day, dateTime.Year);
        }

        /// <summary>
        /// 解析GTM时间yyyyMMddHHmmss
        /// </summary>
        public static DateTime ParseFtpDate(string dateStr)
        {
            var dateTime = DateTime.ParseExact(dateStr, FtpDataFormats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal);
            return dateTime;
        }

        /// <summary>
        /// 格式化GTM时间yyyyMMddHHmmss
        /// </summary>
        public static string FormatFtpDate(DateTime dateTime)
        {
            return dateTime.ToString(FtpDataFormats, DateTimeFormatInfo.InvariantInfo);
        }
    }
}
