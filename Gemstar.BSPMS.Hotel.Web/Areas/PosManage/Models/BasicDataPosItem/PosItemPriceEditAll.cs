using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItem
{
    /// <summary>
    /// 批量修改使用
    /// </summary>
    public class PosItemPriceEditAll
    {
        /// <summary>
        /// 单位ID
        /// </summary>
        public string unitId { get; set; }

        /// <summary>
        /// 单位编码
        /// </summary>
        public string unitCode { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string unitName { get; set; }


        /// <summary>
        /// 价格
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// 差价
        /// </summary>
        public decimal differencePrice { get; set; }

    }
}