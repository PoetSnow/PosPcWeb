using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ResDetailPlan")]
    [LogCName("房价计划，因为不支持批量查询，暂不做酒店代码的字段。")]
    public class ResDetailPlan
    {
        [Key]
        [Column(Order = 1)]
        [LogCName("登记号")]
        public string Regid { get; set; }

        [Key]
        [Column(Order = 2)]
        [LogCName("日期")]
        [LogAnywayWhenEdit]
        public DateTime Ratedate { get; set; }

        [LogCName("价格")]
        public decimal? Price { get; set; }

        [LogCName("源价格")]
        public decimal? OriginPrice { get; set; }


    }
}
