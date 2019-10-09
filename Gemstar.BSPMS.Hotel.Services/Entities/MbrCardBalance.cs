using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ProfileBalance")]
    [LogCName("会员余额表")]
    public class MbrCardBalance
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("客人id")]
        [LogAnywayWhenEdit]
        public Guid profileId { get; set; }

        [LogCName("最近消费日期")]
        public DateTime? LastDate { get; set; }

        [LogCName("最后入住日期")]
        public DateTime? LastIn { get; set; }

        [LogCName("累计间夜数")]
        public decimal? Nigths { get; set; }

        [LogCName("累计消费")]
        public decimal? Amounts { get; set; }

        [LogCName("余额")]
        public decimal? Balance { get; set; }

        [LogCName("余额_总获取")]
        public decimal? BalanceGet { get; set; }

        [LogCName("余额_总使用")]
        public decimal? BalanceUse { get; set; }

        [LogCName("赠送余额")]
        public decimal? Free { get; set; }

        [LogCName("赠送_总获取")]
        public decimal? FreeGet { get; set; }

        [LogCName("赠送_总使用")]
        public decimal? FreeUse { get; set; }

        [LogCName("积分余额")]
        public int? Score { get; set; }

        [LogCName("积分总获取")]
        public int? ScoreGet { get; set; }

        [LogCName("积分总使用")]
        public int? ScoreUse { get; set; }

        [LogCName("现金券可用张数")]
        public int? CashTicket { get; set; }

        [LogCName("现金券总张数")]
        public int? CashTicketGet { get; set; }

        [LogCName("现金券已使用张数")]
        public int? CashTicketUse { get; set; }

        [LogCName("项目券可用张数")]
        public int? ItemTicket { get; set; }

        [LogCName("项目券总张数")]
        public int? ItemTicketGet { get; set; }

        [LogCName("项目券已使用张数")]
        public int? ItemTicketUse { get; set; }

        [LogCName("业主分余额")]
        public int? scoreOwner { get; set; }

        [LogCName("业主分总获取")]
        public int? scoreOwnerGet { get; set; }

        [LogCName("业主分总使用")]
        public int? scoreOwnerUse { get; set; }

    }
}
