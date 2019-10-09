using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    public class PosBillDetailActionAddViewModel
    {
        [Display(Name = "账单id")]
        public string MBillid { get; set; }

        [Display(Name = "明细id")]
        public Int64? Mid { get; set; }

        [Display(Name = "作法组别")]
        public int? Igroupid { get; set; }

        [Display(Name = "数量")]
        public decimal? Quan { get; set; }

        [Display(Name = "人数")]
        public decimal? IGuest { get; set; }

        [Display(Name = "折扣类型")]
        public byte? IsForce { get; set; }

        [Display(Name = "折扣")]
        public decimal? Discount { get; set; }

        [Display(Name = "部门类别id")]
        public string DeptClassid { get; set; }

        [Display(Name = "备注")]
        public string Memo { get; set; }

        [Display(Name = "作法id")]
        public string ActionId { get; set; }

        [Display(Name = "手写作法")]
        public string HandActionName { get; set; }

        [Display(Name = "区分手写作法")]
        public string ActionType { get; set; }

        /// <summary>
        /// 是否数量相关
        /// </summary>
        public bool? IByQuan { get; set; }

        /// <summary>
        /// 是否人数相关
        /// </summary>
        public bool? IByGuest { get; set; }

        /// <summary>
        /// 作法加价
        /// </summary>
        public decimal? AddPrice { get; set; }
    }
}