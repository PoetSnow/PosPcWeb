using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosAdvanceFunc
{
    public class PosAdvanceFuncAddViewModel
    {
   

        [Display(Name = "营业点")]
        public string RefeId { get; set; }

        [Display(Name = "功能代码")]
        public string FuncCode { get; set; }

        [Display(Name = "功能名称一")]
        public string Name1 { get; set; }

        [Display(Name = "功能名称二")]
        public string Name2 { get; set; }

        [Display(Name = "功能名称三")]
        public string Name3 { get; set; }

        [Display(Name = "是否使用")]
        public bool? IsUsed { get; set; }

        [Display(Name = "是否立即落单")]
        public bool? IsSave { get; set; }

        [Display(Name = "是否Ipad使用")]
        public bool? IsIpad { get; set; }


        [Display(Name = "功能类型")]
        public string FuncType { get; set; }

        [Display(Name = "功能级别")]
        public string FuncGrade { get; set; }



        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "操作员")]
        public string TransUser { get; set; }


    }
}