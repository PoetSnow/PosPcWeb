using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("Guest")]
    [LogCName("客历")]
    public class Guest
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        [LogIgnore]
        public string Hid { get; set; }

        [Key]
        [LogCName("客人id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("姓名")]
        [LogAnywayWhenEdit]
        public string GuestName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("性别")]
        public string Gender { get; set; }

        [LogCName("首次入住日期")]
        public DateTime? JoinDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("证件类型")]
        public string CerType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("证件号")]
        public string Cerid { get; set; }

        [LogCName("生日")]
        public DateTime? Birthday { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("住址")]
        public string Address { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("籍贯")]
        public string City { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("手机号")]
        public string Mobile { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("qq号")]
        public string Qq { get; set; }

       
        [Column(TypeName = "varchar")]
        [LogCName("国籍")]
        public string Nation { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("邮箱")]
        public string Email { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("合约单位")]
        public string CompanyName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("微信号")]
        public string Weixin { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("喜好")]
        public string Interest { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("车牌号")]
        public string CarNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("最近消费日期")]
        public string LastDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("最后入住日期")]
        public string LastIn { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("累计间夜数")]
        public string Nigths { get; set; }

        [LogCName("发票类型")]
        public byte? TaxType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票抬头")]
        public string TaxName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("税务登记号")]
        public string TaxNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票地址和电话")]
        public string TaxAddTel { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票银行和账号")]
        public string TaxBankAccount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("黑名单原因")]
        public string BlacklistReason { get; set; }
    }
}