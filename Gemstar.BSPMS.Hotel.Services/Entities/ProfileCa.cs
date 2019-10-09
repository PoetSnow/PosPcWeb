using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ProfileCa")]
    [LogCName("会员账务表")]
    public class ProfileCa
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [LogCName("客历号")]
        [LogAnywayWhenEdit]
        public Guid Profileid { get; set; }

        [LogCName("日期")]
        public DateTime TransDate { get; set; }

        [LogCName("营业日")]
        public DateTime TransBsnsdate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("操作员")]
        public string InputUser { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("帐户类型")]
        public string BalanceType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("交易说明")]
        public string Type { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("营业点")]
        public string OutletCode { get; set; }

        [LogCName("交易金额")]
        public decimal Amount { get; set; }

        [LogCName("余额")]
        public decimal? Balance { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("单号")]
        public string Invno { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("外部参考")]
        public string Refno { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("付款方式")]
        public string Itemid { get; set; }

        [LogCName("原币金额")]
        public decimal? OriginAmount { get; set; }

        [LogIgnore]
        [LogCName("班次id")]
        public string ShiftId { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("集团id")]
        public string Grpid { get; set; }
        /// <summary>
        /// 是否已经退款
        /// </summary>
        [LogCName("已经退款")]
        [LogBool("是","否")]
        public bool IsRefunded { get; set; }
    }
}
