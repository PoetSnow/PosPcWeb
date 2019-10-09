using System;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 操作日志枚举字段属性，有此属性的字段将显示枚举中文名称
    /// </summary>
    public class LogEnumAttribute : Attribute
    {
        /// <summary>
        /// 创建枚举类型示例
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        public LogEnumAttribute(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", "enumType");
            }
            EnumType = enumType;
        }

        /// <summary>
        /// 枚举类型
        /// </summary>
        public Type EnumType { get; set; }
    }
}
