using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Enums
{
    /// <summary>
    /// 预订明细状态
    /// </summary>
    public enum ResDetailStatus
    {

        /// <summary>
        /// 预订状态
        /// </summary>
        [Description("预订")]
        R,
        /// <summary>
        /// noshow状态
        /// </summary>
        [Description("noshow")]
        N,
        /// <summary>
        /// 取消状态
        /// </summary>
        [Description("取消")]
        X,
        /// <summary>
        /// 在住状态
        /// </summary>
        [Description("在住")]
        I,
        /// <summary>
        /// 离店迟付状态
        /// </summary>
        [Description("迟付")]
        O,
        /// <summary>
        /// 离店且结账状态
        /// </summary>
        [Description("已结")]
        C,
        /// <summary>
        /// Z(或其它) :未入住 Z(或其它):非预订单
        /// </summary>
        [Description("其它")]
        Z
    }
}
