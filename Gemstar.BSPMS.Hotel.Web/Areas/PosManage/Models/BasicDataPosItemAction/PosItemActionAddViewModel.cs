using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemAction
{
    public class PosItemActionAddViewModel
    {
        [Display(Name = "酒店代码")]
        public string Hid { get; set; }

        [Display(Name = "项目")]
        [Required(ErrorMessage = "请选择项目")]
        public string Itemid { get; set; }

        [Display(Name = "作法")]
        [Required(ErrorMessage = "请选择作法")]
        public string Actionid { get; set; }

        [Display(Name = "数量相关")]
        public bool? IsByQuan { get; set; }

        [Display(Name = "数量相关最低数量")]
        public decimal? LimitQuan { get; set; }

        [Display(Name = "人数相关")]
        public bool? IsByGuest { get; set; }

        [Display(Name = "常用作法")]
        public bool? IsCommon { get; set; }

        [Display(Name = "必选作法")]
        public bool? IsNeed { get; set; }

        [Display(Name = "作法加价")]
        public decimal? AddPrice { get; set; }

        [Display(Name = "加价倍数")]
        public decimal? Multiple { get; set; }

        [Display(Name = "出品打印机")]
        public string ProdPrinter { get; set; }

        [Display(Name = "排列序号")]
        public int? SeqID { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? Modified { get; set; }

        /// <summary>
        /// 类型(0:消费项目；1:分类；2:大类)
        /// </summary>
        public byte? iType { get; set; }
    }
}