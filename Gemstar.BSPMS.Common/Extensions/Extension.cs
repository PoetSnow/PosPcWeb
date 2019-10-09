using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gemstar.BSPMS.Common.Extensions
{
    public static class Extension
    {
        public static string[] SplitString(this string str, string split)
        {
            str = str.Replace(",", "~[ReplaceDot]~").Replace(split, ",");
            var arr = str.Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = arr[i].Replace("~[ReplaceDot]~", ",");
            }
            return arr;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static Guid ParseGuid(this string str)
        {
            Guid id;
            if (Guid.TryParse(str, out id))
            {
                return id;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public static string UrlDecode(this string url)
        {
            try
            {
                return System.Web.HttpUtility.UrlDecode(url);
            }
            catch (Exception)
            {
                return url;
                throw;
            }
        }

        public static string UrlEncode(this string url)
        {
            try
            {
                return System.Web.HttpUtility.UrlEncode(url);
            }
            catch (Exception)
            {
                return url;
                throw;
            }
        }

        public static string ToFormatStringByLength(this string str, int length)
        {
            if (str.Length > length)
            {
                str = str.Substring(0, length) + "...";
            }
            return str;
        }

        public static string ToDataTimeStr(this DateTime now)
        {
            return now.Year.ToString() + now.Month.ToString() + now.Day.ToString() + now.Hour.ToString() + now.Minute.ToString() + now.Second.ToString() + now.Millisecond.ToString();
        }

        public static DataSet ToDataSet<T>(this IList<T> list)
        {
            Type elementType = typeof(T);
            var ds = new DataSet();
            var t = new DataTable();
            ds.Tables.Add(t);
            elementType.GetProperties().ToList().ForEach(propInfo => t.Columns.Add(propInfo.Name, Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType));
            foreach (T item in list)
            {
                var row = t.NewRow();
                elementType.GetProperties().ToList().ForEach(propInfo => row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value);
                t.Rows.Add(row);
            }
            return ds;
        }

        /// <summary>
        /// 截取两位小数
        /// </summary>
        /// <param name="num">值</param>
        /// <param name="scale">小数位数</param>
        /// <returns></returns>
        public static string ToString(this decimal num, int scale)
        {
            string numToString = num.ToString();

            int index = numToString.IndexOf(".");
            int length = numToString.Length;

            if (index != -1)
            {
                return string.Format("{0}.{1}",
                  numToString.Substring(0, index),
                  numToString.Substring(index + 1, Math.Min(length - index - 1, scale)));
            }
            else
            {
                return num.ToString();
            }
        }
        #region Json序列化时间格式转换

        /// <summary>
        /// 将Json序列化后时间戳转换为"yyyy-MM-dd HH:mm:ss"格式
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string ReplaceJsonDateToDateString(this string json)
        {
            if (json.IndexOf("\\/Date") > -1)
            {
                return Regex.Replace(json, @"\\/Date\((\d+)\)\\/", match =>
                {
                    DateTime dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                    dt = dt.ToLocalTime();
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
                });
            } else
            {
                return Regex.Replace(json, @"\/Date\((\d+)\)\/", match =>
                {
                    DateTime dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                    dt = dt.ToLocalTime();
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
                });
            }
        }

        #endregion Json序列化时间格式转换
    }
}
