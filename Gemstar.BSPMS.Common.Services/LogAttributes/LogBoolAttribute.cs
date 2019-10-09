using System;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 操作日志布尔值字段属性，有此属性的字段将显示布尔值中文名称
    /// </summary>
    public class LogBoolAttribute : Attribute
    {
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="trueName">真时名称</param>
        /// <param name="falseName">假时名称</param>
        public LogBoolAttribute(string trueName, string falseName)
        {
            if (string.IsNullOrWhiteSpace(trueName) || string.IsNullOrWhiteSpace(falseName))
            {
                throw new ArgumentException("传入的参数不能为空！", "trueName，falseName");
            }
            TrueName = trueName;
            FalseName = falseName;
        }

        /// <summary>
        /// 真
        /// </summary>
        public string TrueName { get; set; }

        /// <summary>
        /// 假
        /// </summary>
        public string FalseName { get; set; }
    }
}
