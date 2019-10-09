using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Models.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "请输入所属集团代码")]
        [Display(Name = "所属集团代码")]
        public string grpid { get; set; }

        [Required(ErrorMessage = "请输入酒店代码")]
        [Display(Name = "酒店代码")]
        public string hid { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "酒店名称")]
        public string name { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = " 姓 　 名")]
        public string loginName { get; set; }

        [Required(ErrorMessage = "请输入登录名")]
        [Display(Name = "  登 录 名")]
        public string loginCode { get; set; }

        [Required(ErrorMessage = "请输入登录密码")]
        [Display(Name = "登录密码")]
        public string pwd { get; set; }

        [Required(ErrorMessage = "请输入确认密码")]
        [Display(Name = "确认密码")]
        public string confirmPwd { get; set; }

        [Display(Name = "省  份")]
        public string provinces { get; set; }

        [Display(Name = "城市选择")]
        public string city { get; set; }

        [Display(Name = "星级")]
        public string star { get; set; }

        [Required(ErrorMessage = "请输入邮箱")]
        [Display(Name = "邮 　 箱")]
        public string email { get; set; }

        [Display(Name = "Q Q 号")]
        public string qq { get; set; }

        [Required(ErrorMessage = "请输入手机号")]
        [Display(Name = " 手 机 号")]
        public string mobile { get; set; }

        [Required(ErrorMessage = "请输入验证码")]
        [Display(Name = " 验 证 码")]
        public string checkCode { get; set; }
    }
}