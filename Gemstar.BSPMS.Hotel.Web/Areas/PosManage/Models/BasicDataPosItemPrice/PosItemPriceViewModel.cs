using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemPrice
{
    public class PosItemPriceViewModel
    {
        [Display(Name = "id")]
        public Guid Id { get; set; }

        [Display(Name = "消费项目")]
        public string itemName { get; set; }

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
        public string IsDefault { get; set; }

        [Display(Name = "所属餐台类型")]
        public string tabTypeName { get; set; }

        [Display(Name = "排列序号")]
        public int? Seqid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? Modified { get; set; }
    }
}