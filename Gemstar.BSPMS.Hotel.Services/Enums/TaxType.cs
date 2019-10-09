using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.Enums
{
    /// <summary>
    /// 发票类型
    /// </summary>
    public enum TaxType
    {
        [Description("普通发票")]
        普通发票 = 0,
        [Description("专用发票")]
        专用发票 = 1,
    }
}
