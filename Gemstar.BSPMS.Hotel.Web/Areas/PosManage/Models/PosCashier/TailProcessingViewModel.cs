using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosCashier
{
    public class TailProcessingViewModel
    {
        /// <summary>
        /// 单号
        /// </summary>
        public string Billid { get; set; }
        
        /// <summary>
        /// 付款方式ID
        /// </summary>
        public string Itemid { get; set; }

        /// <summary>
        /// 需要尾数处理的金额
        /// </summary>
        public decimal Amount { get; set; }
    }
}