using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("Company")]
    [LogCName("合约单位资料")]
    public class Company
    {
        [Column(TypeName = "varchar")]
        [LogCName("集团代码")]
        public string Grpid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("名称")]
        [LogAnywayWhenEdit]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("代码")]
        [LogAnywayWhenEdit]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("类型")]
        public string CompanyTypeid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("价格代码")]
        [LogRefrenceName(Sql = "SELECT name FROM rate WHERE id={0}")]
        public string RateCode { get; set; }

        [LogCName("发票类型")]
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
        [LogCName("开户银行")]
        public string Bank { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("帐号")]
        public string BankAccount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("地址")]
        public string Address { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("电话")]
        public string Tel { get; set; }

        [LogCName("生效日期")]
        public DateTime? BeginDate { get; set; }

        [LogCName("有效日期")]
        public DateTime? ValidDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("联系人")]
        public string Contact { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("联系人手机")]
        public string ContactMobile { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("联系人称呼")]
        public string Position { get; set; }

        [LogCName("挂帐限额")]
        public int? LimitAmount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("业务员")]
        public string Sales { get; set; }

        [LogCName("累计消费")]
        public decimal? Amount { get; set; }

        [LogCName("累计间夜数")]
        public decimal? Nights { get; set; }

        [LogCName("当前应收")]
        public decimal? Balance { get; set; }

        [LogCName("状态")]
        public EntityStatus Status { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("拼音码")]
        public string Py { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("联系人邮箱")]
        public string TaxEmail { get; set; }
        
        [LogCName("发送营销短信时间")]
        public DateTime? MarketSmsDate { get; set; }
    }
}
