using System;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 操作日志对应的表或者字段中文名称属性
    /// </summary>
    public class LogCNameAttribute:Attribute
    {
        /// <summary>
        /// 创建中文名称实例
        /// </summary>
        /// <param name="cname">中文名称</param>
        public LogCNameAttribute(string cname)
        {
            Name = cname;
        }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string Name { get; set; }
    }
}
