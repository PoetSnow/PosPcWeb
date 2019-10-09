using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Models.Account
{
    /// <summary>
    /// 授权登录视图模型
    /// </summary>
    public class AuthLoginViewModel
    {
        [Display(Name ="授权码")]
        [Required(ErrorMessage = "请输入授权码")]
        public string AuthCode { get; set; }

        [Display(Name = "用户名")]
        [Required(ErrorMessage = "请输入用户名")]
        public string Username { get; set; }

        [Display(Name ="密码")]
        [Required(ErrorMessage = "请输入密码")]
        public string Password { get; set; }
    }
}