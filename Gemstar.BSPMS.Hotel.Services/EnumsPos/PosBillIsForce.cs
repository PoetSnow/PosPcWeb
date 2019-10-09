using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 折扣类型
    /// </summary>
    public enum PosBillIsForce : byte
    {
        //0：默认折扣；1：全单折；2：照单折（不可折的菜式也强行折）；3：会员价
        [Description("默认折扣")]
        默认折扣 = 0,
        [Description("全单折")]
        全单折 = 1,
        [Description("照单折")]
        照单折 = 2,
        [Description("会员价")]
        会员价 = 3
    }
}
