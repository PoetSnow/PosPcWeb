using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("Profile")]
    [LogCName("会员")]
    public class MbrCard
    {
        [Column(TypeName = "varchar")]
        [LogCName("集团编号")]
        public string Grpid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("会员ID")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("会员卡号")]
        [LogAnywayWhenEdit]
        public string MbrCardNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogRefrenceName(Sql ="SELECT name FROM mbrCardType WHERE id = {0} AND hid = {1}",OtherParaFieldNames = "Hid")]
        [LogCName("会员卡类型")]
        public string MbrCardTypeid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("姓名")]
        public string GuestName { get; set; }

        [Column(TypeName = "varchar")]
        [LogEnum(typeof(BSPMS.Common.Enumerator.Gender))]
        [LogCName("性别")]
        public string Gender { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("业务员")]
        public string Sales { get; set; }

        [LogDatetimeFormat("yyyy-MM-dd")]
        [LogCName("入会日期")]
        public DateTime? JoinDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("证件类型")]
        public string CerType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("证件号")]
        public string Cerid { get; set; }

        [LogDatetimeFormat("yyyy-MM-dd")]
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
        [LogCName("邮箱")]
        public string Email { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("微信号")]
        public string Weixin { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("兴趣爱好")]
        public string Interest { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogIgnore]
        [LogCName("会员卡密码")]
        public string Pwd { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("主卡号")]
        public string MasterCardNo { get; set; }

        [LogCName("有效期")]
        [LogDatetimeFormat("yyyy-MM-dd")]
        public DateTime? ValidDate { get; set; }

        [LogBool("已审核", "未审核")]
        [LogCName("审核状态")]
        public bool? IsAudit { get; set; }

        [LogEnum(typeof(BSPMS.Common.Enumerator.MbrCardStatus))]
        [LogCName("卡状态")]
        public byte? Status { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("车牌号")]
        public string CarNo { get; set; }

        [LogCName("发票类型")]
        [LogEnum(typeof(BSPMS.Common.Enumerator.InvoiceType))]
        public byte? TaxType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("税务登记号")]
        public string TaxNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票抬头")]
        public string TaxName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票地址和电话")]
        public string TaxAddTel { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票银行和账号")]
        public string TaxBankAccount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("登录名")]
        public string NetName { get; set; }

        [Column(TypeName = "varchar")]
        [LogIgnore]
        [LogCName("登录密码")]
        public string NetPwd { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("微信id")]
        public string Wxid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("感应卡号")]
        public string InductionCar { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("操作员")]
        [LogIgnore]
        public string CreateUser { get; set; }

        [LogBool("接收", "不接收")]
        [LogCName("是否接收交易类型短信")]
        public bool IsTransactionMsg { get; set; }

        [LogBool("接收", "不接收")]
        [LogCName("是否接收广告类型短信")]
        public bool IsAdvertisementMsg { get; set; }

        [LogCName("发展来源")]
        [Column(TypeName = "varchar")]
        public string Source { get; set; }

        [LogCName("是否业主")]
        public bool IsOwner { get; set; }

        [LogCName("发送营销短信时间")]
        public DateTime? MarketSmsDate { get; set; }
    }
}
