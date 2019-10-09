using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosTabOpenItem")]
    [LogCName("定义开台项目")]
    public class PosTabOpenItem
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
        
        [Column(TypeName = "varchar")]
        [LogCName("项目id")]
        public string Itemid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("单位")]
        public string Unitid { get; set; }
        
        [LogCName("数量")]
        public decimal Quantity { get; set; }
        
        [LogCName("价格")]
        public decimal? Price { get; set; }
        
        [LogCName("数量方式")]
        public byte QuanMode { get; set; }
        
        [LogCName("收费状态")]
        public byte? IsCharge { get; set; }
        
        [LogCName("是否飞单")]
        public bool? IsProduce { get; set; }
        
        [LogCName("是否可取消")]
        public bool? IsCancel { get; set; }
        
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
