using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosAction
{
    public class ActionEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public string Id { get; set; }

        [Display(Name = "作法分类")]
        public string ActionTypeID { get; set; }

        [Display(Name = "代码")]
        [Required(ErrorMessage = "请输入代码")]
        public string Code { get; set; }

        [Display(Name = "中文名称")]
        [Required(ErrorMessage = "请输入中文名")]
        public string Cname { get; set; }

        [Display(Name = "英文名称")]
        public string Ename { get; set; }

        [Display(Name = "出品打印机")]
        public string ProdPrinter { get; set; }

        [Display(Name = "出品打印机")]
        public string[] ProdPrinters
        {
            get { return string.IsNullOrEmpty(ProdPrinter) ? new string[] { } : (string[])GetSeparateSubString(ProdPrinter, 3).ToArray(typeof(string)); }
            set { ProdPrinters = value; }
        }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "标准加价")]
        public decimal? AddPrice { get; set; }

        [Display(Name = "单价倍数")]
        public decimal? Multiple { get; set; }

        [Display(Name = "输入价格")]
        public bool? IsInputPrice { get; set; }

        [Display(Name = "加价数量相关")]
        public bool? IsByQuan { get; set; }

        [Display(Name = "加价人数相关")]
        public bool? IsByGuest { get; set; }

        [Display(Name = "加价条数相关")]
        public bool? IsByPiece { get; set; }

        [Display(Name = "分单出品")]
        public bool? IsSubProd { get; set; }

        [Display(Name = "排列序号")]
        public int? SeqId { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? ModifiedDate { get; set; }
    }
}