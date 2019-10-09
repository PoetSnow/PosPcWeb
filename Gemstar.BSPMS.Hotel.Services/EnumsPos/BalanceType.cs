using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 捷云会员账户类型
    /// </summary>
    public enum BalanceType : byte
    {
        //01:储值     02:赠送金额     11:积分       12:业主分      31:现金券      32:项目券
        [Description("储值")]
        储值 = 01,
        [Description("赠送金额")]
        赠送金额 = 02,
        [Description("积分")]
        积分 = 11,
        [Description("业主分")]
        业主分 = 12,
        [Description("现金券")]
        现金券 = 31,
        [Description("项目券")]
        项目券 = 32,
    }
}
