namespace Gemstar.BSPMS.Hotel.Services.AuthManages
{
    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum AuthType:byte
    {
        /// <summary>
        /// 集团管理公司菜单
        /// </summary>
        Group = 1,
        /// <summary>
        /// 集团分店菜单
        /// </summary>
        GroupHotel = 2,
        /// <summary>
        /// 单体酒店菜单
        /// </summary>
        SingleHotel = 3
    }
}
