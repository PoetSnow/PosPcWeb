using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosShuffleNews")]
    [LogCName("Pos公用市别")]
    public class PosShuffleNews
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("公用市别id")]
        public string Id { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("公用市别代码")]
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
        [LogCName("开始时间")]
        public string Stime { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("结束时间")]
        public string Etime { get; set; }

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
    }
}
