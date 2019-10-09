using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosUserToRefe
{
    public class UserToRefeViewModel: BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public Guid Id { get; set; }

        [Display(Name = "登录名")]
        [Required(ErrorMessage = "请输入登录名")]
        public string Code { get; set; }

        [Display(Name = "操作员")]
        [Required(ErrorMessage = "请输入操作员")]
        public string Name { get; set; }

        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Display(Name = "QQ号")]
        public string Qq { get; set; }

        [Display(Name = "手机号")]
        [Required(ErrorMessage = "请输入手机")]
        [RegularExpression(RegexHelper.MobileRegexString, ErrorMessage = "请输入有效的手机号码")]
        public string Mobile { get; set; }

        [Display(Name = "所属角色")]
        public List<Guid> RoleList { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "总裁驾驶舱账号")]
        public string AnalysisUserCode { get; set; }

        [Display(Name = "卡号")]
        public string CardId { get; set; }

        [Display(Name = "营业点")]
        public string RefeId { get; set; }
        public string[] RefeIds
        {
            get { return string.IsNullOrEmpty(RefeId) ? new string[] { } : RefeId.Split(','); }
            set { RefeIds = value; }
        }

        [Display(Name = "收银点")]
        public string PosId { get; set; }
        public string[] PosIds
        {
            get { return string.IsNullOrEmpty(PosId) ? new string[] { } : PosId.Split(','); }
            set { PosIds = value; }
        }
    }
}