using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemAction
{
    public class PosItemActionViewModel : BaseEditViewModel
    {
        [Display(Name = "id")]
        public Guid Id { get; set; }

        [Display(Name = "酒店代码")]
        public string Hid { get; set; }

        [Display(Name = "项目id")]
        public string Itemid { get; set; }

        [Display(Name = "作法id")]
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

        public byte? iType { get; set; }
    }
}