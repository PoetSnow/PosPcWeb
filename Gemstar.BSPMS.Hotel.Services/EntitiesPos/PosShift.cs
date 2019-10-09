using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosShift")]
    [LogCName("pos班次")]
    public class PosShift
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("班次id")]
        public string Id { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("酒店id")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("代码")]
        public string Code { get; set; }

        [LogCName("中文名称")]
        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("英文名称")]
        public string Ename { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("所属收银点")]
        public string PosId { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("模块代码")]
        public string Module { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("开始时间")]
        public string Stime { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("结束时间")]
        public string Etime { get; set; }

        [LogCName("排序号")]
        public int? Seqid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("状态（1：启用，51：禁用）")]
        public byte? IStatus { get; set; }
    }
}
