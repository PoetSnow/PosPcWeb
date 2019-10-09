using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Models.Account
{
    /// <summary>
    /// 体验试用视图模型
    /// </summary>
    public class TryInfoViewModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [Required(ErrorMessage ="请输入手机号")]
        [Display(Name ="手机号")]
        public string MobileNo { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "请输入验证码")]
        [Display(Name = "验证码")]
        public string CheckCode { get; set; }
    }
}