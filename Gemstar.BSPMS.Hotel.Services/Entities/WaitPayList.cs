using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("WaitPayList")]
    [LogCName("待支付记录")]
    public class WaitPayList
    {
        [Key]
        [LogCName("待支付id")]
        public Guid WaitPayId { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("产品类型")]
        public string ProductType { get; set; }
        [LogCName("业务参数")]
        public string BusinessPara { get; set; }
        [LogCName("状态")]
        public byte Status { get; set; }
        [LogCName("创建日期")]
        public DateTime CreateDate { get; set; }
        [LogCName("支付日期")]
        public DateTime? PayDate { get; set; }

    }
}