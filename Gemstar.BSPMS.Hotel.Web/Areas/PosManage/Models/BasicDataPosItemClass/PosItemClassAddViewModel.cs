using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemClass
{
    public class PosItemClassAddViewModel
    {
        [Display(Name = "代码")]
        [Required(ErrorMessage = "请输入代码")]
        public string Code { get; set; }

        [Display(Name = "中文名称")]
        [Required(ErrorMessage = "请输入中文名")]
        public string Cname { get; set; }

        [Display(Name = "英文名称")]
        public string Ename { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "是否分类")]
        public bool? IsSubClass { get; set; }

        [Display(Name = "所属营业点")]
        public string Refeid { get; set; }

        [Display(Name = "是否IPAD显示")]
        public bool? IsIpadShow { get; set; }

        [Display(Name = "背景图片")]
        public string Bmp { get; set; }

        [Display(Name = "排列序号")]
        public int? Seqid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}