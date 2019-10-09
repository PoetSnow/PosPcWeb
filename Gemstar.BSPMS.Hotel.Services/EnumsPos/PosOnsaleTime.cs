using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 取特价菜时间
    /// </summary>
    public enum PosOnsaleTime:byte
    {
        //取特价菜时间：0：点菜时间；1：开台时间
        [Description("点菜时间")]
        点菜时间 =0,
        [Description("开台时间")]
        开台时间 =1
    }
}
