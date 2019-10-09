using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Extensions
{
    /// <summary>
    /// 日期时间类型扩展
    /// </summary>
    public static class DateTimeExtension
    {
        public const string DateFormatStr = "yyyy-MM-dd";
        public const string DateTimeWithoutSecondFormatStr = "yyyy-MM-dd HH:mm";
        public const string DateTimeFormatStr = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 将指定的日期转换为yyyy-MM-dd格式的日期字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDateString(this DateTime value)
        {
            return value.ToString(DateFormatStr);
        }
        /// <summary>
        /// 将指定的日期转换为yyyy-MM-dd格式的日期字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDateString(this DateTime? value)
        {
            if (value.HasValue)
            {
                return value.Value.ToString(DateFormatStr);
            }
            return "";
        }
        /// <summary>
        /// 将指定的日期转换为yyyy-MM-dd格式的日期字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime value)
        {
            return value.ToString(DateTimeFormatStr);
        }
        /// <summary>
        /// 将指定的日期转换为yyyy-MM-dd格式的日期字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime? value)
        {
            if (value.HasValue)
            {
                return value.Value.ToString(DateTimeFormatStr);
            }
            return "";
        }
        /// <summary>
        /// 将指定的日期转换为yyyy-MM-dd HH:mm格式的日期字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDateTimeWithoutSecondString(this DateTime value)
        {
            return value.ToString(DateTimeWithoutSecondFormatStr);
        }
        /// <summary>
        /// 将指定的日期转换为yyyy-MM-dd HH:mm格式的日期字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDateTimeWithoutSecondString(this DateTime? value)
        {
            if (value.HasValue)
            {
                return value.Value.ToString(DateTimeWithoutSecondFormatStr);
            }
            return "";
        }

    }
}
