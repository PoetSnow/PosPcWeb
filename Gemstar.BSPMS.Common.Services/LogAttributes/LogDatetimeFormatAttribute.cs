using System;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 操作日志日期时间字段属性，有此属性的字段将指定日期时间格式
    /// </summary>
    public class LogDatetimeFormatAttribute : Attribute
    {
        /// <summary>
        /// 创建日期时间格式实例
        /// </summary>
        /// <param name="format">日期时间格式</param>
        public LogDatetimeFormatAttribute(string format)
        {
            Format = format;
        }
        /// <summary>
        /// 日期时间格式
        /// </summary>
        public string Format { get; set; }
    }
}
