using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemPrice
{
    public class PosItemPriceEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public Guid Id { get; set; }

        [Display(Name = "消费项目")]
        [Required(ErrorMessage = "请选择消费项目")]
        public string Itemid { get; set; }

        [Display(Name = "单位")]
        [Required(ErrorMessage = "请选择单位")]
        public string Unitid { get; set; }

        [Display(Name = "单位代码")]
        public string UnitCode { get; set; }

        [Display(Name = "单位")]
        public string Unit { get; set; }

        [Display(Name = "价格")]
        public decimal? Price { get; set; }

        [Display(Name = "倍数")]
        public decimal? Multiple { get; set; }

        [Display(Name = "毛利率")]
        public decimal? Grossrate { get; set; }

        [Display(Name = "成本价")]
        public decimal? CostPrice { get; set; }

        [Display(Name = "油味")]
        public decimal? OilAmount { get; set; }

        [Display(Name = "提成")]
        public decimal? Percent { get; set; }

        [Display(Name = "会员价")]
        public decimal? MemberPrice { get; set; }

        [Display(Name = "是否缺省单位")]
        public bool? IsDefault { get; set; }

        [Display(Name = "所属餐台类型")]
        public string TabTypeid { get; set; }

        [Display(Name = "排列序号")]
        public int? Seqid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? Modified { get; set; }
    }
}