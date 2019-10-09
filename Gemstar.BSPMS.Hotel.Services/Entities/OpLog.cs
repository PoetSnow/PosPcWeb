using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("OpLog")]
    [LogCName("操作日志")]
    public class OpLog
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [LogCName("操作时间")]
        public DateTime CDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("操作员")]
        public string CUser { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("操作ip")]
        public string Ip { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("操作类型")]
        public string XType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("操作内容")]
        public string CText { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("操作数据关键字")]
        public string Keys { get; set; }

        [Key]
        [LogCName("自增长id")]
        public Int64 LogId { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("掩码")]
        public string Mask { get; set; }


    }
}