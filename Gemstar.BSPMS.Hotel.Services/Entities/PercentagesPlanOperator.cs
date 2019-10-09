using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("PercentagesPlanOperator")]
    [LogCName("操作员提成任务定义表")]
    public class PercentagesPlanOperator
    {
        [LogIgnore]
        [Key]
        public Guid PlanId { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [LogIgnore]
        public Guid OperatorId { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string PlanSource { get; set; }

        [LogIgnore]
        public DateTime PlanDate { get; set; }

        [LogIgnore]
        public decimal PlanAmount { get; set; }

    }
}
