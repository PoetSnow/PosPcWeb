using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosTabStatus")]
    [LogCName("这个表用来显示餐台状态")]
    public class PosTabStatus
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }
        
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("餐台id")]
        public string Tabid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("台号")]
        public string TabNo { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("台名")]
        public string TabName { get; set; }
        
        [LogCName("可容纳人数")]
        public int? IGuest { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("营业点id")]
        public string Refeid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("营业点代码")]
        public string RefeCode { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("营业点名称")]
        public string RefeName { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("餐台类型id")]
        public string Tabtypeid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("餐台类型代码")]
        public string TabTypeCode { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("餐台类型名称")]
        public string TabTypeName { get; set; }
        
        [LogCName("台状态")]
        public byte? TabStatus { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("当前开台id")]
        public string OpTabid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("客人姓名")]
        public string GuestName { get; set; }
        
        [LogCName("开台人数")]
        public int? OpenGuest { get; set; }
        
        [LogCName("开台时间")]
        public DateTime? OpenRecord { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("预订id")]
        public string ResTabid { get; set; }
        
        [LogCName("预订状态")]
        public byte? TabResStatus { get; set; }
        
        [LogCName("最近预订时间")]
        public DateTime? ArrDate { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }
        
        [LogCName("排列序号")]
        public int? Seqid { get; set; }
    }
}
