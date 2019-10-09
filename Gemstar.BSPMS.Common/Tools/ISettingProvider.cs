using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 设置提供接口，用于session共享模块
    /// </summary>
    public interface ISettingProvider
    {
        /// <summary>
        /// 当前应用程序设置的值
        /// </summary>
        SettingInfo SettingInfo{ get; }
    }
}
