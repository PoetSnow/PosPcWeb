using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{

    public enum PosBillChangeStatus : byte
    {
        [Description("全单转台")]
        全单转台 = 1,
        [Description("并台")]
        并台 = 2,
        [Description("转菜")]
        转菜 = 3,
    }
}
