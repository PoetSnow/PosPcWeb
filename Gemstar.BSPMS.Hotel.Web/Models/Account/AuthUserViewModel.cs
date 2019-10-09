using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Models.Account
{
    /// <summary>
    /// 授权他人登录视图模型
    /// </summary>
    public class AuthUserViewModel
    {
        [Display(Name ="生效时间")]        
        [Required(ErrorMessage ="请选择生效时间")]
        public DateTime? BeginDate { get; set; }
        [Display(Name ="失效时间")]
        [Required(ErrorMessage ="请选择失效时间")]
        public DateTime? EndDate { get; set; }
        [Display(Name ="授权码")]
        public string AuthCode { get; set; }
        [Display(Name ="授权人")]
        public string UserName { get; set; }
    }
}