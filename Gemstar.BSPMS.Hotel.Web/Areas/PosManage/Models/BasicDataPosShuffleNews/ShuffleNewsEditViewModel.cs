using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;
namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosShuffleNews
{
    /// <summary>
    /// 公用市别
    /// </summary>
    public class ShuffleNewsEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public string Id { get; set; }

        [Display(Name = "代码")]
        [Required(ErrorMessage = "请输入代码")]
        public string Code { get; set; }

        [Display(Name = "中文名称")]
        [Required(ErrorMessage = "请输入中文名称")]
        public string Cname { get; set; }

        [Display(Name = "英文名称")]
        public string Ename { get; set; }

        [Display(Name = "开始时间")]
        public string Stime { get; set; }

        [Display(Name = "结束时间")]
        public string Etime { get; set; }
        [Display(Name = "模块代码")]
        public string Module { get; set; }

        [Display(Name = "排序号")]
        public int? Seqid { get; set; }
        
        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "酒店ID")]
        public string Hid { get; set; }
    }
}