using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.UserManage
{
    public class UserAddGroupViewModel
    {
        [Display(Name ="登录名")]
        [Required(ErrorMessage = "请输入登录名")]
        public string Code { get; set; }

        [Display(Name ="操作员")]
        [Required(ErrorMessage = "请输入操作员")]
        public string Name { get; set; }

        [Display(Name ="邮箱")]
        public string Email { get; set; }

        [Display(Name = "QQ号")]
        public string Qq { get; set; }

        [Display(Name = "手机号")]
        [Required(ErrorMessage ="请输入手机")]
        [RegularExpression(RegexHelper.MobileRegexString,ErrorMessage ="请输入有效的手机号码")]
        public string Mobile { get; set; }

        [Display(Name = "所属分店及角色")]
        public string HotelRoleIds { get; set; }
        public List<PmsHotel> Hotels { get; set; }
        public List<Role> Roles { get; set; }

        [Display(Name = "总裁驾驶舱账号")]
        public string AnalysisUserCode { get; set; }

        [Display(Name = "所属分店")]
        public string Belonghotel { get; set; }

        [Display(Name = "卡号")]
        public string CardId { get; set; }
    }
}