using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    public enum PosPrintBillStatus : byte
    {
        [Description("账单打印")]
        账单打印 = 1,
        [Description("储值")]
        点菜单打印 = 2,
        [Description("储值")]
        埋脚打印 = 3,
    }
}
