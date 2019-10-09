using System;
using Gemstar.BSPMS.Hotel.Web.Models;
using Gemstar.BSPMS.Common.Enumerator;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage
{
    public class MbrCardEditSingleViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "错误信息，请关闭后再重试")]
        public Guid Id { get; set; }

        [Display(Name = "类型")]
        [Required(ErrorMessage = "错误信息，请关闭后再重试")]
        public string Type { get; set; }

        [Display(Name = "会员卡号")]
        public string MbrCardNo { get; set; }

        [Display(Name = "感应卡号")]
        public string InductionCar { get; set; }

        [Display(Name = "会员姓名")]
        public string GuestName { get; set; }

        [Display(Name = "业务员")]
        public string Sales { get; set; }

        [Display(Name = "会员卡状态")]
        public byte? Status { get; set; }

        [Display(Name = "审核状态")]
        public bool? IsAudit { get; set; }

        [Display(Name = "有效期")]
        public DateTime? ValidDate { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}
