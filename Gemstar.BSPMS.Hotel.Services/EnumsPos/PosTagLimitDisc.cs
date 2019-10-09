using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 最低消费是否可折
    /// </summary>
    public enum PosTagLimitDisc : byte
    {
        //0：折后不可低于最低消费；1：折后可低于最低消费；2：超出最低消费才可折

        [Description("折后不可低于最低消费")]
        折后不可低于最低消费 = 0,
        [Description("折后可低于最低消费")]
        折后可低于最低消费 = 1,
        [Description("超出最低消费才可折")]
        超出最低消费才可折 = 2
    }
}
