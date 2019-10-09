using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 出品状态
    /// </summary>
    public enum PosBillDetailIsProduce:byte
    {
        //0：未出品，1：已出品，2：修改，3：新单全单；4：修改全单
        [Description("未出品")]
        未出品 = 0,
        [Description("已出品")]
        已出品 = 1,
        [Description("修改")]
        修改 = 2,
        [Description("新单全单")]
        新单全单 = 3,
        [Description("修改全单")]
        修改全单 = 4,
    }
}
