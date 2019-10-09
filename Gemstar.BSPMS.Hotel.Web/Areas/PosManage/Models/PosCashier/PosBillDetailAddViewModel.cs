using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosCashier
{
    public class PosBillDetailAddViewModel
    {
        /// <summary>
        /// 主单号
        /// </summary>
        public string MBillid { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string Billid { get; set; }

        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string Itemid { get; set; }
    }
}