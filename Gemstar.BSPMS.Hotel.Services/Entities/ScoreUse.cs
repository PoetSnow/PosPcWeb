using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ScoreUse")]
    [LogCName("积分兑换明细")]
    public class ScoreUse
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
        public DateTime? TransDate { get; set; }

        [LogCName("营业日")]
        public DateTime? TransBsnsDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("营业点")]
        public string Outletid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("兑换项目id")]
        [LogAnywayWhenEdit]
        public string ScoreItemid { get; set; }

        [LogCName("积分")]
        public int? Score { get; set; }

        [LogCName("金额")]
        public decimal? Amount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("付款方式")]
        public string Itemid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("操作员")]
        public string InputUser { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("单号")]
        public string Invno { get; set; }

        [LogIgnore]
        [LogCName("班次id")]
        public string ShiftId { get; set; }

        [LogCName("原币金额")]
        public decimal? OriginAmount { get; set; }
    }
}
