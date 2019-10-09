using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 记录数据更改日志接口
    /// </summary>
    public interface IDataChangeLog
    {
        /// <summary>
        /// 增加数据增删改查的日志
        /// 由ef自动检测本次savechange之前的所有增删改的记录进行记录
        /// </summary>
        /// <param name="opType">当前操作类型</param>
        /// <param name="entityName">实体名称，如果传值，则使用指定的值，如果没有，则取实体的中文名称或者表名称</param>
        /// <param name="addPrefixToEntityName">是否添加前缀，如果true，则添加XXX、修改XXX、删除XXX。如果没有，则只有XXX。</param>
        void AddDataChangeLogs(OpLogType opType, string entityName = null, bool addPrefixToEntityName = true);
    }
    public enum LogValueType
    {
        /// <summary>
        /// 当前值
        /// </summary>
        CurrentValue,
        /// <summary>
        /// 原始值
        /// </summary>
        OriginValue
    }
}
