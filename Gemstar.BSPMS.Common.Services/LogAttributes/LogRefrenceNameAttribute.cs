using System;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 操作日志中，取外键关联的名称
    /// </summary>
    public class LogRefrenceNameAttribute:Attribute
    {
        /// <summary>
        /// 需要执行的sql语句，用来执行后获取对应的外键名称
        /// 要求此语句只返回一个值，并且至少接收一个参数，就是属性所在字段的值，如果接收多个参数，则属性所在字段的值始终是第一个参数
        /// 例如SELECT name FROM mbrCardType WHERE id = {0}
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// 执行sql语句时需要的其他参数的字段名称,以逗号分隔，要求顺序与sql语句中的相对应
        /// </summary>
        public string OtherParaFieldNames { get; set; }
    }
}
