using System;

namespace Gemstar.BSPMS.Common.Services
{

    /// <summary>
    /// 操作日志以酒店ID开头的字段属性，有此属性的字段将去除酒店ID
    /// </summary>
    public class LogStartsWithHidAttribute : Attribute
    {
    }
}