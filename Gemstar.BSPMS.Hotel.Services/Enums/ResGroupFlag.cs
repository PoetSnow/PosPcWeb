using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Enums
{
    /// <summary>
    /// 订单的团散标志
    /// </summary>
    public enum ResGroupFlag:byte
    {
        /// <summary>
        /// 散客
        /// </summary>
        散客 =0,
        /// <summary>
        /// 团体
        /// </summary>
        团体=1
    }
}
