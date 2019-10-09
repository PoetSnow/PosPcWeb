using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("RateDetail")]
    [LogCName("价格体系明细")]
    public class RateDetail
    {
        [LogCName("取消提前小时数")]
        public int? CancelHours { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("取消政策id")]
        public string Cancelid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("担保政策id")] 
        public string Guaranteeid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("分时时间")]
        public string GuaranteeTime { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [LogCName("是否关闭")] 
        [LogBool("是", "否")]
        public bool? IsClose { get; set; }

        [LogCName("房价")]
        public decimal? Rate { get; set; }

        [LogCName("房价日期")]
        [LogAnywayWhenEdit]
        public DateTime RateDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("价格体系id")]
        [LogAnywayWhenEdit]
        public string Rateid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房间类型id")]
        public string RoomTypeid { get; set; }


    }
}
