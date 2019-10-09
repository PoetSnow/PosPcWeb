using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItem
{
    public class PosItemSuitViewModel : BaseEditViewModel
    {
        [Display(Name = "id")]
        public Guid Id { get; set; }

        [Display(Name = "酒店代码")]
        public string Hid { get; set; }

        [Display(Name = "套餐酒席ID")]
        public string ItemId { get; set; }

        [Display(Name = "套餐酒席代码")]
        public string ItemCode { get; set; }

        [Display(Name = "级数")]
        [Required(ErrorMessage = "请填写级数")]
        public int? IGrade { get; set; }

        [Display(Name = "是否自选")]
        public bool IsAuto { get; set; }

        [Display(Name = "套餐明细")]
        [Required(ErrorMessage = "请选择套餐明细")]
        public string ItemId2 { get; set; }

        [Display(Name = "明细代码")]
        public string ItemCode2 { get; set; }

        [Display(Name = "明细名称")]
        public string ItemName { get; set; }

        [Display(Name = "单位")]
        [Required(ErrorMessage = "请选择单位")]
        public string Unitid { get; set; }

        [Display(Name = "单位代码")]
        public string UnitCode { get; set; }

        [Display(Name = "数量")]
        public decimal? Quantity { get; set; }

        [Display(Name = "单价")]
        public decimal? Price { get; set; }

        [Display(Name = "分摊金额")]
        public decimal? AddPrice { get; set; }

        [Display(Name = "金额")]
        public decimal? Amount { get; set; }

        [Display(Name = "不分摊金额")]
        public bool IsPrice { get; set; }

        [Display(Name = "是否参与自动组合套餐")]
        public bool IsBuild { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? Modifieddate { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}