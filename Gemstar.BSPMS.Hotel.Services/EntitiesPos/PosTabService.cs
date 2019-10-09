using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosTabService")]
    [LogCName("服务费政策")]
    public class PosTabService
    {
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("客人类型id")]
        public string CustomerTypeid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("营业点id")]
        public string Refeid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("餐台类型id")]
        public string TabTypeid { get; set; }

        [LogCName("日期类型")]
        public byte? ITagperiod { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("开始时间")]
        public string StartTime { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("结束时间")]
        public string EndTime { get; set; }

        [LogCName("服务费率")]
        public decimal? Servicerate { get; set; }

        [LogCName("默认折扣")]
        public decimal? Discount { get; set; }

        [LogCName("最低消费")]
        public decimal? NLimit { get; set; }

        [LogCName("最低消费计法")]
        public byte? IsByPerson { get; set; }

        [LogCName("最低消费时长")]
        public int? LimitTime { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("修改时间")]
        public DateTime? ModifiedDate { get; set; }

    }
}
