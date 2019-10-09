using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.Enums
{
    /// <summary>
    /// 门锁卡状态
    /// </summary>
    public enum LockStatus
    {
        /// <summary>
        /// 无卡 未发卡
        /// </summary>
        [Description("无卡")]
        无卡 = -1,

        /// <summary>
        /// 发卡
        /// </summary>
        [Description("发卡")]
        发卡 = 0,

        /// <summary>
        /// 注销
        /// </summary>
        [Description("注销")]
        注销 = 21,

        /// <summary>
        /// 无卡注销
        /// </summary>
        [Description("无卡注销")]
        无卡注销 = 22
    }
}
