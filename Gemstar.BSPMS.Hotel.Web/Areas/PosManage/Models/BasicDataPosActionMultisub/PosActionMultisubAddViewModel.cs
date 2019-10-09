using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosActionMultisub
{
    public class PosActionMultisubAddViewModel
    {
        [Display(Name = "当前作法")]
        public string Actionid { get; set; }

        [Display(Name = "同组作法")]
        public string Actionid2 { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? Modified { get; set; }
    }
}