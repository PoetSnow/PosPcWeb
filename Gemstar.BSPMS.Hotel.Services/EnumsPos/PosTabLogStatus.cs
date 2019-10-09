using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 锁台状态
    /// </summary>
    public enum PosTabLogStatus : byte
    {
        //0：开台自动锁台；1：锁消费
        [Description("开台自动锁台")]
        开台自动锁台 = 0,
        [Description("锁消费")]
        锁消费 = 1,
    }
}
