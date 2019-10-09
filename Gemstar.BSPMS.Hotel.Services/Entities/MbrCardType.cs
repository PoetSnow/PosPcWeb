using Gemstar.BSPMS.Common.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("MbrCardType")]
    [LogCName("会员卡类型表")]
    public class MbrCardType
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("会员卡类型代码")]
        public string Code { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("会员卡类型名称")]
        [LogAnywayWhenEdit]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("价格代码")]
        public string RateCodeid { get; set; }

        [LogCName("升级间夜数")]
        public int? Nights { get; set; }

        [LogCName("升级积分数")]
        public int? Score { get; set; }

        [LogCName("会员类型类型等级顺序")]
        public int Seqid { get; set; } 
       
        [LogCName("是否自动升级")]
        [LogBool("是", "否")]
        public bool? IsAutoUp { get; set; }

        [LogCName("积分比率")]
        public decimal? ScoreRate { get; set; }

        [LogCName("升级消费金额")]
        public decimal? Amount { get; set; }

        [LogCName("会员有效时长(月份)")]
        public int Validdate { get; set; }

        [LogCName("积分有效时长(月份)")]
        public int? ScoreVdate { get; set; }
        /// <summary>
        /// 卡费
        /// </summary>
        [LogCName("卡费")]
        public decimal CardFee { get; set; }
        /// <summary>
        /// 积分单位
        /// </summary>
        [LogCName("积分单位")]
        public decimal? scoreUnit { get; set; }
    }
}