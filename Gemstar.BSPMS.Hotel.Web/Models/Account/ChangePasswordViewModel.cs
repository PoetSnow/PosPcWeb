using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Models.Account
{
    /// <summary>
    /// 修改密码视图模型
    /// </summary>
    public class ChangePasswordViewModel
    {
        /// <summary>
        /// 原密码，主要是为了防止别人随意点击界面上的修改密码后就直接修改密码了，所以要求输入原密码
        /// </summary>
        [Display(Name ="原密码")]
        [Required(ErrorMessage ="请输入原密码")]
        public string OriginPassword { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        [Display(Name = "新密码")]
        [Required(ErrorMessage = "请输入新密码")]
        public string NewPassword { get; set; }
        /// <summary>
        /// 确认密码
        /// </summary>
        [Display(Name = "确认密码")]
        [Required(ErrorMessage = "请输入确认密码")]
        [Compare("NewPassword",ErrorMessage ="确认密码必须与新密码一致")]
        public string ConfirmPassword { get; set; }
    }
}