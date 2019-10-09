using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 出品状态(0：不出品；1：出品；2：全单出品一张单；3：全单出品传菜部)
    /// </summary>
    public enum PosRequestProduce : byte
    {
        //出品状态(0：不出品；1：出品；2：全单出品一张单；3：全单出品传菜部)

        [Description("不出品")]
        不出品 = 0,
        [Description("出品")]
        出品 = 1,
        [Description("全单出品一张单")]
        全单出品一张单 = 2,
        [Description("全单出品传菜部")]
        全单出品传菜部 = 3
    }
}
