using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage
{
    public class MbrCardChangeValidDateViewModel
    {
        public Guid ID { get; set; }
        [Display(Name = "会员卡号")]
        public string CardNo { get; set; }

        [Display(Name = "原到期时间")]
        public DateTime? OldValidDate { get; set; }

        [Display(Name = "新到期时间")]
        [Required(ErrorMessage = "请选择新到期时间")]
        public DateTime? NewValidDate { get; set; }

        [Display(Name = "备注")]
        [Required(ErrorMessage = "请输入延期备注")]
        public string Remark { get; set; }
    }
}