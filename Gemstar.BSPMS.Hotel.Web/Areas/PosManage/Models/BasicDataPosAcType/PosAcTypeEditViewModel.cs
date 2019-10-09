using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosAcType
{
    public class PosAcTypeEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public string Id { get; set; }

        [Display(Name = "代码")]
        [Required(ErrorMessage = "请输入代码")]
        public string Code { get; set; }

        [Display(Name = "中文名称")]
        [Required(ErrorMessage = "请输入中文名")]
        public string Cname { get; set; }

        [Display(Name = "英文名称")]
        public string Ename { get; set; }

        [Display(Name = "财务类型")]
        public string AcClass { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "排序号")]
        public int? Seqid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}