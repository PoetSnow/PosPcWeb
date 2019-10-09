using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    public enum PosItemActionType
    {
        [Description("消费项目")]
        消费项目 = 0,
        [Description("分类")]
        分类 = 1,
        [Description("大类")]
        大类 = 2
    }
}
