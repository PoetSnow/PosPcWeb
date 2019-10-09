using System;

namespace Gemstar.BSPMS.Hotel.Services.AuthManages
{
    /// <summary>
    /// 用于匹配controller中的action方法与数据库中对应的权限项的属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AuthButtonAttribute:Attribute
    {
        /// <summary>
        /// 以指定的权限项id和功能按钮权限初始化权限项
        /// </summary>
        /// <param name="flag">功能按钮权限值</param>
        public AuthButtonAttribute(AuthFlag flag)
        {
            AuthButtonValue = flag;
        }
        /// <summary>
        /// 对应的权限按钮值
        /// </summary>
        public AuthFlag AuthButtonValue { get; set; }
    }
}
