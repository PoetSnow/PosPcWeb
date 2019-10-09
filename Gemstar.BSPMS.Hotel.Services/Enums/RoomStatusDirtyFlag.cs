using System.ComponentModel;
namespace Gemstar.BSPMS.Hotel.Services.Enums
{
    /// <summary>
    /// 房态中的脏净标志位
    /// </summary>
    public enum RoomStatusDirtyFlag:byte
    {
        /// <summary>
        /// 净房
        /// </summary>
        Clean = 0,
        /// <summary>
        /// 脏房
        /// </summary>
        Dirty = 1,
        /// <summary>
        /// 清洁房
        /// </summary>
        WaitClean = 2
    }

    /// <summary>
    /// 房态中的维修停用标志位
    /// </summary>
    public enum RoomStatusServiceAndStopFlag : byte
    {
        /// <summary>
        /// 维修
        /// </summary>
        [Description("维修")]
        Service = 1,
        /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")]
        Stop = 2
    }
}
