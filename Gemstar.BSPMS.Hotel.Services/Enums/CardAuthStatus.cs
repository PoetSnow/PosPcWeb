using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.Enums
{
    /// <summary>
    /// 预授权状态
    /// </summary>
    public enum CardAuthStatus
    {
        [Description("授权")]
        授权 = 1,
        [Description("完成")]
        完成 = 2,
        [Description("取消")]
        取消 = 51,
    }
}
