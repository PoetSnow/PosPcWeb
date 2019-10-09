using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Extensions
{
    /// <summary>
    /// 星期扩展
    /// </summary>
    public static class DayOfWeekExtension
    {
        /// <summary>
        /// 将星期转换为周几的中文显示
        /// </summary>
        /// <param name="value">要转换的星期</param>
        /// <returns>转换后的周几文本</returns>
        public static string ToChineseString(this DayOfWeek value)
        {
            string result="";
            switch (value) {
                case DayOfWeek.Monday:
                    result = "周一";
                    break;
                case DayOfWeek.Tuesday:
                    result = "周二";
                    break;
                case DayOfWeek.Wednesday:
                    result = "周三";
                    break;
                case DayOfWeek.Thursday:
                    result = "周四";
                    break;
                case DayOfWeek.Friday:
                    result = "周五";
                    break;
                case DayOfWeek.Saturday:
                    result = "周六";
                    break;
                case DayOfWeek.Sunday:
                    result = "周日";
                    break;
            }
            return result;
        }
    }
}
