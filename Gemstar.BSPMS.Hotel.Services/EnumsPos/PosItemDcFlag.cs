using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 付款还是消费（D：消费，C：付款）
    /// </summary>
    public enum PosItemDcFlag : byte
    {
        /// <summary>
        /// 消费
        /// </summary>
        D,
        /// <summary>
        /// 付款
        /// </summary>
        C
    }
}
