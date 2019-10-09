using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 是否打印点菜单
    /// </summary>
    public enum PosPrintBillss : byte
    {
        //是否打印点菜单(0：不打印；1：打印；2：提示)

        [Description("不打印")]
        不打印 = 0,
        [Description("打印")]
        打印 = 1,
        [Description("提示")]
        提示 = 2
    }
}
