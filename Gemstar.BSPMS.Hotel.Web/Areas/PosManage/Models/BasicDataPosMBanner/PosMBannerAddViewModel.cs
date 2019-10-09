using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosMBanner
{
    public class PosMBannerAddViewModel
    {
        [Display(Name = "图片文件")]
        public string FileName { get; set; }

        [Display(Name = "排列序号")]
        public int? OrderBy { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}