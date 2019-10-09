using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ScoreUseRule")]
    [LogCName("积分兑换规则")]
    public class ScoreUseRule
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("积分兑换项目_id")]
        [LogAnywayWhenEdit]
        public string ItemScoreid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("会员卡类型id")]
        [LogAnywayWhenEdit]
        public string MbrCardTypeid { get; set; }

        [LogCName("仅积分")]
        public int? OnlyScore { get; set; }

        [LogCName("积分部分")]
        public int? ScoreAndAmount { get; set; }

        [LogCName("金额部分")]
        public int? AmountAndScore { get; set; }

        [LogCName("顺序号")]
        public int? Seqid { get; set; }


    }
}
