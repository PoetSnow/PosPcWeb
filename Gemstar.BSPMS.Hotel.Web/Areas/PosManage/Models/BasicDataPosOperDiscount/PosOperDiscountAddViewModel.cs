using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosOperDiscount
{
    public class PosOperDiscountAddViewModel
    {
        [Display(Name = "营业点id")]
        public string Refeid { get; set; }


        [Display(Name = "日期类型")]
        public byte? ITagperiod { get; set; }

        [Display(Name = "开始时间")]
        public string StartTime { get; set; }

        [Display(Name = "结束时间")]
        public string EndTime { get; set; }


        [Display(Name = "最低折扣")]
        public decimal? Discount { get; set; }

        [Display(Name = "点菜限额")]
        public decimal? Orderlimit { get; set; }


        [Display(Name = "赠送限额")]
        public decimal? Presentlimit { get; set; }


        [Display(Name = "赠送限额统计方式")]
        public byte? ICountType { get; set; }


        [Display(Name = "赠送限额计算标准")]
        public byte? ICmpType { get; set; }


        [Display(Name = "赠送比例计算")]
        public byte? IRateType { get; set; }


        [Display(Name = "天赠送限额")]
        public decimal? DayPresentlimit { get; set; }


        [Display(Name = "传菜限额")]
        public decimal? Sentlimit { get; set; }


        [Display(Name = "金额折限额")]
        public decimal? DiscAmount { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }        

        [Display(Name = "用户")]
        [Required(ErrorMessage = "请选择操作员")]
        public string UserId { get; set; }
    }
}