using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 押金状态
    /// </summary>
    public enum PrePayStatus: byte
    {
        [Description("交押金")]
        交押金 = 0,
        [Description("退押金")]
        退押金 = 1,
        [Description("押金付款")]
        押金付款 = 2,
        [Description("待支付")]
        待支付 = 56,
        [Description("取消")]
        取消 = 51,
    }
}
