using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosOnSale
{
    public class PosOnSaleAddViewModel
    {
        [Display(Name = "客人类型")]
        public string CustomerTypeid { get; set; }

        [Display(Name = "营业点")]
        public string Refeid { get; set; }

        [Display(Name = "餐台类型")]
        public string TabTypeid { get; set; }


        [Display(Name = "日期类型")]
        public byte? ITagperiod { get; set; }


        [Display(Name = "开始时间")]
        [Required(ErrorMessage = "请输入开始时间")]
        public string StartTime { get; set; }


        [Display(Name = "结束时间")]
        [Required(ErrorMessage = "请输入结束时间")]
        public string EndTime { get; set; }

        [Display(Name = "消费项目")]
        [Required(ErrorMessage = "请选择消费项目")]
        public string Itemid { get; set; }

        [Display(Name = "单位")]
        [Required(ErrorMessage = "请选择单位")]
        public string Unitid { get; set; }

        [Display(Name = "价格")]
        [Required(ErrorMessage = "请输入价格")]
        public decimal? Price { get; set; }


        [Display(Name = "折扣率")]
        public decimal? Discount { get; set; }

        [Display(Name = "是否计最低消费")]
        public bool? IsLimit { get; set; }

        [Display(Name = "是否计服务费")]
        public bool? IsService { get; set; }

        [Display(Name = "是否打折")]
        public bool? IsDiscount { get; set; }

        [Display(Name = "计算类型")]
        public byte? ICmpType { get; set; }

        [Display(Name = "是否启用")]
        public bool? IsUsed { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? ModifiedDate { get; set; }
    }
}