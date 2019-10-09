using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 自动标志
    /// </summary>
    public enum PosBillDetailIsauto : byte
    {
        //0:录入(默认) 项目   1：登记项目 2：开台项目   3:特价菜 4:服务费  5:消费余额  6:抹零 20:付款 21:找赎
        [Description("录入项目(默认)")]
        录入项目 = 0,
        [Description("登记项目")]
        登记项目 = 1,
        [Description("开台项目")]
        开台项目 = 2,
        [Description("特价菜")]
        特价菜 = 3,
        [Description("服务费")]
        服务费 = 4,
        [Description("消费余额")]
        消费余额 = 5,
        [Description("抹零")]
        抹零 = 6,
        [Description("付款")]
        付款 = 20,
        [Description("找赎")]
        找赎 = 21,
    }
}
