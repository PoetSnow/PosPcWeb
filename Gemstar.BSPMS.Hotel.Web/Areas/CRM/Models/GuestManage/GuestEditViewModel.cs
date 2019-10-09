using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Web.Areas.CRM.Models.GuestManage
{
    [Table("Guest")]
    public class GuestEditViewModel: BaseEditViewModel
    { 
        [Display(Name = "酒店id")]
        [Column(TypeName = "char")]
        public string Hid { get; set; }

        [Key]
        public Guid Id { get; set; }

        [Display(Name = "姓名")]
        [Column(TypeName = "varchar")]
        [Required(ErrorMessage = "请输入姓名")]
        public string GuestName { get; set; }

        [Display(Name = "性别")]
        [Column(TypeName = "char")]
        public string Gender { get; set; }

        [Display(Name = "首次入住日期")]
        public DateTime? JoinDate { get; set; }

        [Display(Name = "证件类型")]
        [Column(TypeName = "varchar")]
        public string CerType { get; set; }

        [Display(Name = "证件号")]
        [Column(TypeName = "varchar")]
        [Required(ErrorMessage = "请输入证件号")]
        public string Cerid { get; set; }

        [Display(Name = "生日")]
        public DateTime? Birthday { get; set; }

        [Display(Name = "地址")]
        [Column(TypeName = "varchar")]
        public string Address { get; set; }

        [Display(Name = "籍贯")]
        [Column(TypeName = "varchar")]
        public string City { get; set; }

        [Display(Name = "手机号")]
        [Column(TypeName = "varchar")]
        public string Mobile { get; set; }

        [Display(Name = "qq号")]
        [Column(TypeName = "varchar")]
        public string Qq { get; set; }

        [Display(Name = "国籍")]
        [Column(TypeName = "varchar")]
        public string Nation { get; set; }

        [Display(Name = "邮箱")]
        [Column(TypeName = "varchar")]
        public string Email { get; set; }

        [Display(Name = "合约单位")]
        [Column(TypeName = "varchar")]
        public string CompanyName { get; set; }

        [Display(Name = "微信号")]
        [Column(TypeName = "varchar")]
        public string Weixin { get; set; }

        [Display(Name = "喜好")]
        [Column(TypeName = "varchar")]
        public string Interest { get; set; }

        [Display(Name = "备注")]
        [Column(TypeName = "varchar")]
        public string Remark { get; set; }

        [Display(Name = "车牌号")]
        [Column(TypeName = "varchar")]
        public string CarNo { get; set; }

        [Display(Name = "最近消费日期")]  
        [Column(TypeName = "varchar")]
        public string LastDate { get; set; }

        [Display(Name = "最后入住日期")]
        [Column(TypeName = "varchar")]
        public string LastIn { get; set; }

        [Display(Name = "累计间夜数")]
        [Column(TypeName = "varchar")]
        public string Nigths { get; set; }

        [Display(Name = "黑名单原因")]
        [Column(TypeName = "varchar")]
        public string BlacklistReason { get; set; }

        [Display(Name = "最后离店日期")]
        public string DepDate { get; set; }

        [Display(Name = "累计消费金额")]
        public string Amount { get; set; }

        [Display(Name = "最后结账房号")]
        public string RoomNo { get; set; }
    }
}