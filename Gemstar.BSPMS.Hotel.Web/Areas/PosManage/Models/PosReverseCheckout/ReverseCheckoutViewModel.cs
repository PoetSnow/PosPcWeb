using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosReverseCheckout
{
    public class ReverseCheckoutViewModel
    {
        /// <summary>
        /// 当前反结的付款方式ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 主单号
        /// </summary>
        public string MBillid { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public string Billid { get; set; }
        /// <summary>
        /// 取消原因
        /// </summary>
        public string CanReason { get; set; }
    }
}