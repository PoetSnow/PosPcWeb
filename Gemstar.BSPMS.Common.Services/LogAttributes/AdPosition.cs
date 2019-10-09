using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Services
{ 
    /// <summary>
    ///  广告位置 
    /// </summary>
    public enum AdPosition : byte
    {
        /// <summary>
        /// 登录背景轮播位置
        /// </summary>
        [Description("登录背景轮播位置 ")]
        登录背景轮播位置 = 1,
        /// <summary>
        /// 系统标题栏广告
        /// </summary>
        [Description("系统标题栏广告")]
        系统标题栏广告 = 2
    }
}
