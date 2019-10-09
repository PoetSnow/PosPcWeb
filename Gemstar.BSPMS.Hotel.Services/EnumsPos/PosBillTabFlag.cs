using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 餐台标识
    /// </summary>
    public enum PosBillTabFlag : byte
    {
        //0：物理台，1：快餐台
        [Description("物理台")]
        物理台 = 0,
        [Description("快餐台")]
        快餐台 = 1,
        [Description("外卖台")]
        外卖台 = 2,
    }
}
