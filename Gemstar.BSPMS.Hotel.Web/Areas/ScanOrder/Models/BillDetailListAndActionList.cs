using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models
{
    /// <summary>
    /// 用于接收账单明细集合（包括账单明细以及作法）
    /// </summary>
    public class BillDetailListAndActionList
    {
        /// <summary>
        /// 账单明细列表
        /// </summary>
        public string billDetailList { get; set; }

        /// <summary>
        /// 作法列表
        /// </summary>
        public string ActionList { get; set; }

        /// <summary>
        /// 作法分组
        /// </summary>
        public string GroupList { get; set; }


    }
}