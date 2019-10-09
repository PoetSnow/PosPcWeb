using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("PosOperDiscount")]
    [LogCName("操作员折扣设置")]
    public class PosOperDiscount
    {
        [LogIgnore]
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("营业点id")]
        public string Refeid { get; set; }

        [LogIgnore]
        [LogCName("日期类型")]
        public byte? ITagperiod { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("开始时间")]
        public string StartTime { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("结束时间")]
        public string EndTime { get; set; }

        [LogIgnore]
        [LogCName("最低折扣")]
        public decimal? Discount { get; set; }

        [LogIgnore]
        [LogCName("点菜限额")]
        public decimal? Orderlimit { get; set; }

        [LogIgnore]
        [LogCName("赠送限额")]
        public decimal? Presentlimit { get; set; }

        [LogIgnore]
        [LogCName("赠送限额统计方式")]
        public byte? ICountType { get; set; }

        [LogIgnore]
        [LogCName("赠送限额计算标准")]
        public byte? ICmpType { get; set; }

        [LogIgnore]
        [LogCName("赠送比例计算")]
        public byte? IRateType { get; set; }

        [LogIgnore]
        [LogCName("天赠送限额")]
        public decimal? DayPresentlimit { get; set; }

        [LogIgnore]
        [LogCName("传菜限额")]
        public decimal? Sentlimit { get; set; }

        [LogIgnore]
        [LogCName("金额折限额")]
        public decimal? DiscAmount { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogIgnore]
        [LogCName("修改时间")]
        public DateTime? ModifiedDate { get; set; }


        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("用户Id")]
        public string UserId { get; set; }

    }
}
