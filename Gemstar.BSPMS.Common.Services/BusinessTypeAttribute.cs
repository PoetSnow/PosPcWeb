using System;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 业务操作类型名称，用于记录系统异常日志时，区分当前的业务类型
    /// </summary>
    public class BusinessTypeAttribute:Attribute
    {
        /// <summary>
        /// 创建业务操作类型名称实例
        /// </summary>
        /// <param name="typeName">业务操作类型名称</param>
        public BusinessTypeAttribute(string typeName)
        {
            Name = typeName;
        }
        public string Name { get; set; }
    }
}
