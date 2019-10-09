using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// Pos原因类型
    /// </summary>
    public enum PosReasonIstagtype : byte
    {
        //0：取消；1：赠送
        [Description("取消")]
        取消 = 0,
        [Description("赠送")]
        赠送 = 1,
    }
}
