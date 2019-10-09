using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosDepart
{
    public class PosDepartAddViewModel
    {
        [Display(Name = "部门类别")]
        public string DeptClassID { get; set; }

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

        [Display(Name = "二级仓库")]
        public string WhCode { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "排列序号")]
        public int? Seqid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}