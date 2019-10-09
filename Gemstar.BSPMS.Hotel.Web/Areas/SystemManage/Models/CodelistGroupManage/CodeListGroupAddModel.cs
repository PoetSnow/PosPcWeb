using Gemstar.BSPMS.Hotel.Web.Models.BasicDatas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.CodelistGroupManage
{
    public class CodeListGroupAddModel : BasicDataGroupAddViewModel
    {
        [Display(Name = "代码")]
        [Required(ErrorMessage = "请输入代码")]
        public string Code { get; set; }

        [Display(Name = "名称")]
        [Required(ErrorMessage = "请输入名称")]
        public string Name { get; set; }

        [Display(Name = "名称2")]
        public string Name2 { get; set; }

        [Display(Name = "名称3")]
        public string Name3 { get; set; }

        [Display(Name = "名称4")]
        public string Name4 { get; set; }

        [Display(Name = "排序号")]
        public int? Seqid { get; set; }

        [Display(Name = "上级代码")]
        [Required(ErrorMessage = "请选择代码类别")]
        public string TypeCode { get; set; }
    }
}