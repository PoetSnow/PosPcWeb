using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 账单状态
    /// </summary>
    public enum PosBillStatus : byte
    {
        //0:预订   1： 开台   2：结账  3：清台　4：迟付  51：取消
        [Description("预订")]
        预订 = 0,
        [Description("开台")]
        开台 = 1,
        [Description("结账")]
        结账 = 2,
        [Description("清台")]
        清台 = 3,
        [Description("迟付")]
        迟付 = 4,
        [Description("取消")]
        取消 = 51,
        [Description("扫码点餐默认状态")]
        扫码点餐默认状态 = 52,
    }
}
