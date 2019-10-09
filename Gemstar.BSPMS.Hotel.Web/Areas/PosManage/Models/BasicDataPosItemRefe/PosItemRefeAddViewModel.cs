using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemRefe
{
    public class PosItemRefeAddViewModel
    {
        [Display(Name = "消费项目")]
        [Required(ErrorMessage = "请选择消费项目")]
        public string Itemid { get; set; }

        [Display(Name = "所属营业点")]
        [Required(ErrorMessage = "请选择营业点")]
        public string Refeid { get; set; }

        [Display(Name = "出品打印机")]
        public string ProdPrinter { get; set; }

        [Display(Name = "所属市别")]
        public string Shuffleid { get; set; }

        [Display(Name = "合并出品部门")]
        public bool? IsDepartPrint { get; set; }

        [Display(Name = "合并餐台")]
        public bool? IsTabPrint { get; set; }

        [Display(Name = "排列序号")]
        public int? Seqid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "传菜打印机")]
        public string SentPrtNo { get; set; }
    }
}