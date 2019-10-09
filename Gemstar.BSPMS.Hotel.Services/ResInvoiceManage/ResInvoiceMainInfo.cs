using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.ResInvoiceManage
{
    /// <summary>
    /// 发票的信息
    /// 其中包含发票界面需要的所有信息
    /// </summary>
    public class ResInvoiceMainInfo
    {
        /// <summary>
        /// 发票界面中左侧显示的发票信息模型列表
        /// </summary>
        public List<ResInvoiceInfo> InvoiceInfos { get; set; }
    }
}