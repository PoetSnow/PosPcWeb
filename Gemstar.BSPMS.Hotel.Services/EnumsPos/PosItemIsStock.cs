using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 消费项目是否减库存
    /// </summary>
    public enum PosItemIsStock : byte
    {
        [Description("不减库存")]
        不减库存 = 0,
        [Description("减库存")]
        减库存 = 1,
    }
}
