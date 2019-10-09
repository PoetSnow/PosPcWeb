using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosTabLog")]
    [LogCName("锁台列表")]
    public class PosTabLog
    {
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("营业点id")]
        public string Refeid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("台号id")]
        public string Tabid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("台号")]
        public string TabNo { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("开台单号")]
        public string Billid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("信息")]
        public string Msg { get; set; }
        
        [LogCName("状态")]
        public byte? Status { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("操作电脑")]
        public string Computer { get; set; }
        
        [LogCName("连接时间")]
        public DateTime? ConnectDate { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("操作员")]
        public string TransUser { get; set; }
        
        [LogCName("创建时间")]
        public DateTime? CreateDate { get; set; }
    }
}
