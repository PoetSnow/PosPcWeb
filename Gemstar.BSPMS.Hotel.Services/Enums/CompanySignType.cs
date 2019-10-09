using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Enums
{
    /// <summary>
    /// 合约单位签单联系人类型
    /// </summary>
    public enum CompanySignType:byte
    {
        /// <summary>
        /// 是签单人
        /// </summary>
        [Description("签单人")]
        签单人=0,
        /// <summary>
        /// 是联系人
        /// </summary>
        [Description("联系人")]
        联系人=1,
    }
}
