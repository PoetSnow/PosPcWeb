using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosProdPrinter
{
    public class ProdPrinterAddViewModel : BaseEditViewModel
    {

        [Display(Name = "代码")]
        [Required(ErrorMessage = "请输入代码")]
        public string Code { get; set; }

        [Display(Name = "中文名称")]
        [Required(ErrorMessage = "请输入中文名")]
        public string Cname { get; set; }

        [Display(Name = "英文名称")]
        public string Ename { get; set; }

        [Display(Name = "分区号[A-Z]")]
        public string Section { get; set; }

        [Display(Name = "串口号")]
        public string Comno { get; set; }

        [Display(Name = "备份分区号[A-Z]")]
        public string Section1 { get; set; }

        [Display(Name = "备份串口号")]
        public string Comno1 { get; set; }

        [Display(Name = "关联分区号[A-Z]")]
        public string Section2 { get; set; }

        [Display(Name = "关联串口号")]
        public string Comno2 { get; set; }

        [Display(Name = "逐条打印")]
        public bool? isTabeachbreak { get; set; }

        [Display(Name = "虚拟端口")]
        public bool? isVirtual { get; set; }

        [Display(Name = "二级仓库")]
        public string whid { get; set; }

        [Display(Name = "网络打印机")]
        public string Printer { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "排列序号")]
        public int? Seqid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}