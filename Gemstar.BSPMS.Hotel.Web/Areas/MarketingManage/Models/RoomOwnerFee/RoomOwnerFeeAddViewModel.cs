using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.RoomOwnerFee
{
    public class RoomOwnerFeeAddViewModel
    {
        
        [Display(Name = "房间id")]
        public string roomId { get; set; }
         
        [Display(Name = "房号")]
        public string roomNo { get; set; }

        [Required(ErrorMessage = "请输入费用项目")]
        [Display(Name = "费用项目")]
        public string itemId { get; set; }

        [Required(ErrorMessage = "请输入费用日期")]
        [Display(Name = "费用日期")]
        public DateTime? FeeDate { get; set; }

        [Display(Name = "上次抄表数")]
        public decimal? preReadQty { get; set; }

        [Display(Name = "本次抄表数")]
        public decimal? currentReadQty { get; set; }
        
        [Display(Name = "数量")]
        public decimal? qty { get; set; }
        
        [Display(Name = "单价")]
        public decimal? price { get; set; }
         
        [Display(Name = "金额")]
        public decimal? amount { get; set; } 

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "业主id")]
        public Guid Profileid { get; set; }
    }
}