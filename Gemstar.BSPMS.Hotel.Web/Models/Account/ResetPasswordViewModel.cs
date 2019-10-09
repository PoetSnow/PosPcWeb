using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Models.Account
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "酒店代码")]
        [Required(ErrorMessage = "请输入酒店代码")]
        public string Hid { get; set; }

        [Display(Name = "登  录   名")]
        [Required(ErrorMessage = "请输入用户名")]
        public string Account { get; set; }

        [Display(Name = "验证方式")]
        [Required(ErrorMessage = "请选择验证方式")]
        public string GetMethod { get; set; }

        [Display(Name = "手   机    号")]
        [Required(ErrorMessage = "请输入手机号或邮箱")]
        public string GetMethodValue { get; set; }

        [Display(Name = "验   证    码")]
        [Required(ErrorMessage = "请输入验证码")]
        public string CheckCode { get; set; }

        [Display(Name = "新   密    码")]
        [Required(ErrorMessage = "请输入新密码")]
        public string NewPass { get; set; }

        [Display(Name = "确认密码")]
        [Required(ErrorMessage = "请输入确认新密码")]
        public string ConfirmNewPass { get; set; }
    }
}