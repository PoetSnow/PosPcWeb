using System;

namespace Gemstar.BSPMS.Hotel.Services.AuthManages
{
    /// <summary>
    /// 用于匹配集团管控的基础资料代码
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]

    public class AuthBasicDataAttribute : Attribute
    {
        /// <summary>
        /// 以指定的基础资料代码初始化
        /// </summary>
        /// <param name="code">基础资料代码</param>
        public AuthBasicDataAttribute(string code)
        {
            Code = code;
        }
        /// <summary>
        /// 对应的基础资料代码
        /// </summary>
        public string Code { get; set; }
    }
}
