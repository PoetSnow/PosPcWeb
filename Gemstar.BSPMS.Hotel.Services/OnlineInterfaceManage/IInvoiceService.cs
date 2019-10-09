using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Hotel.Services.OnlineInterfaceManage.Invoice;

namespace Gemstar.BSPMS.Hotel.Services.OnlineInterfaceManage
{
    /// <summary>
    /// 发票接口服务
    /// </summary>
    public interface IInvoiceService
    {
        /// <summary>
        /// 增加消费记录
        /// </summary>
        string ConsumInfo(string billid);

        /// <summary>
        /// 撤销消费记录
        /// </summary>
        void ConsumInfoRepeal(string billid);

        /// <summary>
        /// 获取商品代码列表
        /// </summary>
        InvoiceParameter.BaseResponse<List<InvoiceParameter.FindAllSP.Response>> FindAllSP(InvoiceParameter.FindAllSP.Request value);

        /// <summary>
        /// 获取商品代码
        /// </summary>
        InvoiceParameter.BaseResponse<InvoiceParameter.FindSP.Response> FindSP(InvoiceParameter.FindSP.Request value);



    }
}
