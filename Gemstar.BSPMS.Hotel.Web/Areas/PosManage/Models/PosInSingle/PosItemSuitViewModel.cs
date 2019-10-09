using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    public class PosItemSuitViewModel
    {
        public PosItemSuitViewModel()
        { 
            PageIndex = 1;
            PageSize = 1;
            PageTotal = 1;
        }

        [Display(Name = "账单id")]
        public string Billid { get; set; }

        [Display(Name = "所属套餐ID")]
        public string Upid { get; set; }

        [Display(Name = "消费明细ID")]
        public string BillDetailId { get; set; }

        [Display(Name = "id")]
        public Guid Id { get; set; }
        
        [Display(Name = "酒店代码")]
        public string Hid { get; set; }
        
        [Display(Name = "套餐酒席ID")]
        public string ItemId { get; set; }

        [Display(Name = "套餐酒席代码")]
        public string ItemCode { get; set; }

        [Display(Name = "级数")]
        public int? IGrade { get; set; }

        [Display(Name = "自选")]
        public bool IsAuto { get; set; }

        [Display(Name = "明细ID")]
        public string ItemId2 { get; set; }

        [Display(Name = "明细代码")]
        public string ItemCode2 { get; set; }

        [Display(Name = "明细名称")]
        public string ItemName { get; set; }

        [Display(Name = "单位ID")]
        public string Unitid { get; set; }

        [Display(Name = "单位代码")]
        public string UnitCode { get; set; }

        [Display(Name = "单位名称")]
        public string UnitName { get; set; }

        [Display(Name = "数量")]
        public decimal? Quantity { get; set; }

        [Display(Name = "单价")]
        public decimal? Price { get; set; }

        [Display(Name = "分摊金额")]
        public decimal? AddPrice { get; set; }

        [Display(Name = "金额")]
        public decimal? Amount { get; set; }

        [Display(Name = "是否分摊金额")]
        public bool IsPrice { get; set; }

        [Display(Name = "是否参与自动组合套餐")]
        public bool IsBuild { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? Modifieddate { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "当前页")]
        public int PageIndex { get; set; }

        [Display(Name = "每页记录数")]
        public int PageSize { get; set; }

        [Display(Name = "总记录数")]
        public int PageTotal { get; set; }
    }
}