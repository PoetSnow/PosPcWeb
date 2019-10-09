using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    /// <summary>
    /// 手写作法
    /// </summary>
    public class PosHandActionViewModel
    {
        /// <summary>
        /// 作法名称
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// 作法价格
        /// </summary>
        public decimal? ActionPrice { get; set; }

        /// <summary>
        /// 是否数量相关
        /// </summary>
        [Display(Name = "是否数量相关")]
        public bool? iByQuan { get; set; }


        /// <summary>
        /// 是否人数相关
        /// </summary>
        [Display(Name = "是否人数相关")]
        public bool? iByGuest { get; set; }

        /// <summary>
        /// 分组ID
        /// </summary>
        public int Igroupid { get; set; }

        /// <summary>
        /// 账单明细ID
        /// </summary>
        public long Mid { get; set; }

        public string ItemName { get; set; }
    }
}