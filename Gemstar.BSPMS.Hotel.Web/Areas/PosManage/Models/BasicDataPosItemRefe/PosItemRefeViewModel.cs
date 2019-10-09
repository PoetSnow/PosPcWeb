using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemRefe
{
    public class PosItemRefeViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "消费项目")]
        public string itemName { get; set; }

        [Display(Name = "营业点名称")]
        public string RefeName { get; set; }

        [Display(Name = "出品打印机")]
        public string ProdPrinter { get; set; }

        [Display(Name = "所属市别")]
        public string shuffleName { get; set; }

        [Display(Name = "合并出品部门")]
        public string IsDepartPrintStr { get; set; }

        [Display(Name = "合并餐台")]
        public string IsTabPrintStr { get; set; }

        [Display(Name = "排列序号")]
        public int? Seqid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "修改时间")]
        public string ModifiedStr { get; set; }


        [Display(Name = "传菜打印机")]
        public string SentPrtNo { get; set; }
    }
}