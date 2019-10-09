using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("PercentagesPolicyOperator")]
    [LogCName("操作员提成政策定义表")]
    public class PercentagesPolicyOperator
    {
        [LogIgnore]
        [Key]
        public Guid PolicyId { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string PolicyDesciption { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string AmountSource { get; set; }

        [LogIgnore]
        public bool? IsInPlan { get; set; }

        [LogIgnore]
        public decimal AmountBegin { get; set; }

        [LogIgnore]
        public decimal AmountEnd { get; set; }

        [LogIgnore]
        public bool IsAllAmount { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string AmountSumType { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string CalcType { get; set; }

        [LogIgnore]
        public decimal? CalcValue { get; set; }

    }
}
