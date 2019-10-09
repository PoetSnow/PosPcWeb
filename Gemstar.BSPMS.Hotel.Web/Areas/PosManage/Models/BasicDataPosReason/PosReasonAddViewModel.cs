using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosReason
{
    public class PosReasonAddViewModel
    {
        [Display(Name = "原因代码")]
        [Required(ErrorMessage = "请输入原因代码")]
        public string Code { get; set; }

        [Display(Name = "原因名称")]
        [Required(ErrorMessage = "请输入原因名称")]
        public string Cname { get; set; }

        [Display(Name = "英文名")]
        public string Ename { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "类型")]
        public byte? IstagType { get; set; }

        [Display(Name = "是否加回库存")]
        public bool? Isreuse { get; set; }

        [Display(Name = "是否自动沽清")]
        public bool? Isautosellout { get; set; }

        [Display(Name = "是否出品")]
        public bool? Isproduce { get; set; }

        [Display(Name = "排列序号")]
        public int? Seqid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}