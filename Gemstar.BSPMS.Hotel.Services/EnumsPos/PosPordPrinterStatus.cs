using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 出品打印机状态
    /// </summary>
    public enum PosPordPrinterStatus
    {
        启用 = 1,
        故障 = 2,
        禁用 = 51,
        报废 = 60,
    }
}
