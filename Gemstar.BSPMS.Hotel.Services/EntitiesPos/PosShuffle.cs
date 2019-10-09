using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosShuffle")]
    [LogCName("Pos市别")]
    public class PosShuffle
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("市别id")]
        public string Id { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("市别代码")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("名称")]
        public string Cname { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("英文名")]
        public string Ename { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("所属营业点")]
        public string Refeid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("开始时间")]
        public string Stime { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("结束时间")]
        public string Etime { get; set; }

        [LogIgnore]
        [LogCName("显示临时台")]
        public int? IsHideTab { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }

        [LogIgnore]
        [LogCName("排列序号")]
        public int? Seqid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("状态（1：启用，51：禁用）")]
        public byte? IStatus { get; set; }

        [LogIgnore]
        [LogCName("公用市别ID")]
        public string newShuffleid { get; set; }
    }
}
