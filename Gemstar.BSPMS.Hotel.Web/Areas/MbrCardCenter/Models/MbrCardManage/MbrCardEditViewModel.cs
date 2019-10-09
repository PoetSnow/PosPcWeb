using System;
using Gemstar.BSPMS.Hotel.Web.Models;
using System.ComponentModel.DataAnnotations;


namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage
{
    public class MbrCardEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "错误信息，请关闭后再重试")]
        public Guid Id { get; set; }

        [Display(Name = "会员卡号")]
        public string MbrCardNo { get; set; }

        [Display(Name = "会员卡类型")]
        public string MbrCardTypeId { get; set; }

        [Display(Name = "姓名")]
        [Required(ErrorMessage = "请输入姓名")]
        [StringLength(12, ErrorMessage = "{0}长度至少2位", MinimumLength = 2)]
        public string GuestName { get; set; }

        [Display(Name = "性别")]
        [Required(ErrorMessage = "请选择性别")]
        public string Gender { get; set; }

        [Display(Name = "手机号")]
        //[Required(ErrorMessage = "请输入手机号")]
        [RegularExpression(RegexHelper.MobileRegexString, ErrorMessage = "请输入有效的手机号")]
        public string Mobile { get; set; }

        [Display(Name = "证件类型")]
        //[Required(ErrorMessage = "请选择证件类型")]
        public string CerType { get; set; }

        [Display(Name = "证件号")]
        //[Required(ErrorMessage = "请输入证件号")]
        public string CerId { get; set; }

        [Display(Name = "籍贯")]
        public string City { get; set; }

        [Display(Name = "地址")]
        public string Address { get; set; }

        [Display(Name = "生日")]
        public DateTime? BirthDay { get; set; }

        [Display(Name = "QQ号")]
        [RegularExpression("[1-9][0-9]{4,}", ErrorMessage = "请输入有效的QQ号")]
        public string QQ { get; set; }

        [Display(Name = "邮箱")]
        [RegularExpression(RegexHelper.EmailRegexString, ErrorMessage = "请输入有效的邮箱")]
        public string Email { get; set; }

        [Display(Name = "微信号")]
        public string WeiXin { get; set; }

        [Display(Name = "兴趣爱好")]
        public string Interest { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "车牌号")]
        public string CarNo { get; set; }

        [Display(Name = "业务员")]
        public string Sales { get; set; }

        [Display(Name = "感应卡号")]
        public string InductionCar { get; set; }

        [Display(Name = "接收交易短信")]
        public bool IsTransactionMsg { get; set; }

        [Display(Name = "接收营销短信")]
        public bool IsAdvertisementMsg { get; set; }

        [Display(Name = "确认未输入手机号是否保存")]
        public int IsNeedPhone { get; set; }

        [Display(Name = "是否业主")]
        public bool IsOwner { get; set; }
    }
}