using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Models.Account
{
    /// <summary>
    /// 登录视图模型
    /// </summary>
    public class LoginViewModel
    {
        [Display(Name ="酒店代码")]
        [Required(ErrorMessage = "请输入酒店代码")]
        public string HotelId { get; set; }

        [Display(Name = "用户名")]
        [Required(ErrorMessage = "请输入用户名")]
        public string Username { get; set; }

        [Display(Name ="密码")]
        [Required(ErrorMessage = "请输入密码")]
        public string Password { get; set; }

        [Display(Name ="验证码")] 
        public string CheckCode { get; set; }
        public string TryHotelId { get; set; }
        public string TryUserName { get; set; }
        public string TryUserPass { get; set; }
    }
}