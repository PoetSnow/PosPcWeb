using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.Enums
{
    /// <summary>
    /// 发票打印类型
    /// </summary>
    public enum TaxPrintType
    {
        [Description("纸质发票")]
        纸质发票 = 0,
        [Description("电子发票")]
        电子发票 = 1,
    }
}
