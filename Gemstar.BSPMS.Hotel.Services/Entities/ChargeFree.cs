using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ChargeFree")]
    [LogCName("充值赠送规则")]
    public class ChargeFree
    {
        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("会员卡类型id")]
        [LogAnywayWhenEdit]
        public string MbrCardTypeid { get; set; }

        [LogCName("开始金额")]
        public decimal BeginAmount { get; set; }

        [LogCName("结束金额")]
        public decimal EndAmount { get; set; }

        [LogCName("赠送金额")]
        public decimal? Amount { get; set; }

        [LogCName("赠送比例")]
        public decimal? Rate { get; set; }
    }
}
