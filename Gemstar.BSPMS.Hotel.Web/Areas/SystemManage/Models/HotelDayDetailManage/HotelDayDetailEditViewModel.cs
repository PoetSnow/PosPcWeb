using System.ComponentModel.DataAnnotations;
using System;
using Gemstar.BSPMS.Hotel.Web.Models;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.HotelDayDetailManage
{
    public class HotelDayDetailEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public Guid Id { get; set; }

        [Display(Name = "说明")]
        [Required(ErrorMessage = "请填写说明")]
        public string Name { get; set; }

        [Display(Name = "开始日期")]
        [Required(ErrorMessage = "请输入开始日期")]
        public string BeginDay { get; set; }

        [Display(Name = "结束日期")]
        [Required(ErrorMessage = "请输入结束日期")]
        public string EndDay { get; set; }

        [Display(Name = "包含星期几")]
        [Required(ErrorMessage = "请选择包含星期几")]
        public string Week { get; set; }
    }
}
