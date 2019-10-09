using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common
{
    /// <summary>
    /// 日志级别：0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 不输出日志
        /// </summary>
        None = 0,
        /// <summary>
        /// 只输出错误信息
        /// </summary>
        ErrorOnly = 1,
        /// <summary>
        /// 输出错误和正常信息
        /// </summary>
        ErrorAndInfo = 2,
        /// <summary>
        /// 输出错误信息、正常信息和调试信息
        /// </summary>
        ErrorAndInfoAndDebug = 3
    }
}
