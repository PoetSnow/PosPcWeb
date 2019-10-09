using System;

namespace Gemstar.BSPMS.Hotel.Services.AuthManages
{
    /// <summary>
    /// 不需要权限控制的属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class NotAuthAttribute : Attribute
    {
    }
    public class GridAttribute : Attribute
    {

    }
}
