using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models
{
    public class PaymentViewModel:PaymentOperatePara
    {

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int PageTotal { get; set; }

        /// <summary>
        /// 回调函数
        /// </summary>
        public string Callback { get; set; }

        /// <summary>
        /// 请求随机数
        /// </summary>
        public string SendIndex { get; set; }

        /// <summary>
        /// 付款方式列表
        /// </summary>
        public List<PosItem> PayWayList { get; set; }

    }
}