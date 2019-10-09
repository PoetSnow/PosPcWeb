using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Extensions
{
    public static class TimeExtension
    {
        /// <summary>
        /// 验证时间
        /// </summary>
        /// <param name="date">datetime</param>
        /// <param name="time">hh:mm</param>
        /// <returns></returns>
        public static DateTime SetTime(DateTime date, string time)
        {
            DateTime result = DateTime.MinValue;

            if (!string.IsNullOrWhiteSpace(time))
            {
                var timelist = time.Split(':');
                if (timelist != null && timelist.Length == 2)
                {
                    //小时
                    var timeOne = timelist[0];
                    int timeOneInt = -1;
                    if (string.IsNullOrWhiteSpace(timeOne))
                    {
                        if (Int32.TryParse(timeOne, out timeOneInt))
                        {
                            if (!(timeOneInt >= 0 && timeOneInt <= 23))
                            {
                                return result;
                            }
                        }
                    }
                    //分钟
                    var timeTwo = timelist[1];
                    int timeTwoInt = -1;
                    if (string.IsNullOrWhiteSpace(timeTwo))
                    {
                        if (Int32.TryParse(timeTwo, out timeTwoInt))
                        {
                            if (!(timeTwoInt >= 0 && timeTwoInt <= 59))
                            {
                                return result;
                            }
                        }
                    }

                    //转换时间
                    var datetimeStr = date.ToDateString() + " " + time;
                    if (DateTime.TryParse(datetimeStr, out result))
                    {
                        if (result != DateTime.MinValue)
                        {
                            return result;
                        }
                    }

                }
            }
            return DateTime.MinValue;
        }
    }
}
