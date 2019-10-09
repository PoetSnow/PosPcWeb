using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.Enums
{
    /// <summary>
    /// 订单账务明细状态
    /// </summary>
    public enum ResFolioStatus
    {
        /// <summary>
        /// 有效未结
        /// </summary>
        [Description("有效未结")]
        Valid = 1,
        /// <summary>
        /// 已结
        /// </summary>
        [Description("已结")]
        Closed = 2,
        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Invalid = 51
    }
    /// <summary>
    /// 预定类型
    /// </summary>
    public enum ExtType
    {
        /// <summary>
        /// 普通预订
        /// </summary>
        [Description("普通订单")]
        普通订单 = 0,

        /// <summary>
        /// 携程预订
        /// </summary>
        [Description("携程闪住")]
        携程闪住=1,

        /// <summary>
        /// 阿里预订
        /// </summary>
        [Description("阿里信用住")]
        阿里信用住 =2
    }
}
