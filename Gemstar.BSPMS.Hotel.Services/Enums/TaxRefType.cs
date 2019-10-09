using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.Enums
{
    /// <summary>
    /// 发票关联类型
    /// </summary>
    public enum TaxRefType
    {
        [Description("订单号")]
        订单号 = 0,
        [Description("会员账务")]
        会员账务 = 1,
        [Description("合约单位账务")]
        合约单位账务 = 2,
    }
}
