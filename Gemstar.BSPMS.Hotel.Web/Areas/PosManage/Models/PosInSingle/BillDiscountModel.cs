using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    public class BillDiscountModel
    {
        /// <summary>
        /// billId
        /// </summary>
        public string Id { get; set; }

        [Display(Name ="折扣率或者折扣金额")]        
        public decimal? disCount{ get; set; }

        [Display(Name = "折扣类型")]
        public string discType { get; set; }

        [Display(Name = "明细ID集合")]
        public string detailIdList { get; set; }

      
    }
}