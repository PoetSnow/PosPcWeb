using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 自动标志
    /// </summary>
    public enum PosTabStatusEnum : byte
    {
        //1：已买单未离座 -2：已打单超时未买单 - 3：落单后超时未有再点菜 -4：就座 - 餐台已开台，5：预订 - 没有就座，6：维修 - 在维修登记里未完成维修的餐台。7：空净 - 其他状态
        已买单未离座 = 1,
        已打单超时未买单 = 2,
        落单后超时未有再点菜 = 3,
        就座 = 4,
        预订 = 5,
        维修 = 6,
        空净 = 7,
    }
}
