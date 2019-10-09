using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosMBanner
{
    public class PosMBannerEditViewModel : BaseEditViewModel
    {
        [Display(Name = "id")]
        public Guid Id { get; set; }

        [Display(Name = "酒店hid")]
        public string Hid { get; set; }

        [Display(Name = "图片文件")]
        public string FileName { get; set; }

        [Display(Name = "排列序号")]
        public int? OrderBy { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}