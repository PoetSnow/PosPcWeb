using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosPos
{
    public class PosAddViewModel
    {
        [Display(Name = "代码")]
        [Required(ErrorMessage = "请输入代码")]
        public string Code { get; set; }

        [Display(Name = "中文名称")]
        [Required(ErrorMessage = "请输入中文名称")]
        public string Name { get; set; }

        [Display(Name = "英文名称")]
        public string Ename { get; set; }

        [Display(Name = "排序号")]
        public int? Seqid { get; set; }

        [Display(Name = "模块代码")]
        public string Module { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "当前营业日")]
        [Required(ErrorMessage = "请输入当前营业日")]
        public DateTime? Business { get; set; }
        
        [Display(Name = "内部编码")]
        public string CodeIn { get; set; }

        [Display(Name = "云Pos属性")]
        [Required(ErrorMessage = "请选择云Pos属性")]
        public string PosMode { get; set; }

        [Display(Name = "结转设置")]
        public PosBusinessEnd? IsBusinessend { get; set; }

        [Display(Name = "最早结转时间")]
        public string BusinessTime { get; set; }
    }
}