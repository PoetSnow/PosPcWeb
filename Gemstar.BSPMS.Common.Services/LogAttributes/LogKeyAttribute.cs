using System;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 操作日志忽略字段属性，有此属性的字段将不会记录到日志中
    /// </summary>
    public class LogKeyAttribute : Attribute
    {
    }
}