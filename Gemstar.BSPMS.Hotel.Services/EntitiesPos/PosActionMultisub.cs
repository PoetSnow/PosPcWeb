using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosActionMultisub")]
    [LogCName("同组作法")]
    public class PosActionMultisub
    {
        [Key]
        [LogCName("Id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("当前作法")]
        public string Actionid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("同组作法")]
        public string Actionid2 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("修改时间")]
        public DateTime? Modified { get; set; }
    }
}