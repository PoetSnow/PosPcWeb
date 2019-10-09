using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 捷云会员交易类型
    /// </summary>
    public enum PaymentDesc : byte
    {
        //01:充值     02:扣款     10:获得积分       11:使用积分      12:调整积分      30:获得券        31:使用券
        [Description("充值")]
        充值 = 01,
        [Description("扣款")]
        扣款 = 02,
        [Description("获得积分")]
        获得积分 = 10,
        [Description("使用积分")]
        使用积分 = 11,
        [Description("调整积分")]
        调整积分 = 12,
        [Description("获得券")]
        获得券 = 30,
        [Description("项目券")]
        使用券 = 31,
    }
}
