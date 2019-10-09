namespace Gemstar.BSPMS.Hotel.Services.Enums
{
    /// <summary>
    /// 班次登录状态
    /// </summary>
    public enum ShiftLoginStatus : byte
    {
        /// <summary>
        /// 当前日期没有登录
        /// </summary>
        未开 = 0,
        /// <summary>
        /// 已开
        /// </summary>
        已开 = 1,
        /// <summary>
        /// 已关闭
        /// </summary>
        已关闭 = 2
    }
}
