using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosTabStatus
{
    public class PosSellOutAddViewModel
    {
        /// <summary>
        /// 消费项目ID
        /// </summary>
        public string itemId { get; set; }

        /// <summary>
        /// 消费项目编码
        /// </summary>
        public string itemCode { get; set; }

        /// <summary>
        /// 消费项目名称
        /// </summary>
        public string itemName { get; set; }

        /// <summary>
        /// 营业点ID
        /// </summary>
        public string refeId { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>
        public string unitId { get; set; }

         
    }
}