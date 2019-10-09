using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosProdPrinter")]
    [LogCName("出品打印机")]
    public class PosProdPrinter
    {
        [LogIgnore]
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("代码")]
        public string Code { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("名称")]
        public string Cname { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("英文名")]
        public string Ename { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("分区号[A-Z]")]
        public string Section { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("串口号")]
        public string Comno { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("备份分区号[A-Z]")]
        public string Section1 { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("备份串口号")]
        public string Comno1 { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("关联分区号[A-Z]")]
        public string Section2 { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("关联串口号")]
        public string Comno2 { get; set; }

        [LogIgnore]
        [LogCName("逐条打印")]
        public bool? IsTabeachbreak { get; set; }

        [LogIgnore]
        [LogCName("虚拟端口")]
        public bool? IsVirtual { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("二级仓库")]
        public string Whid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("网络打印机")]
        public string Printer { get; set; }

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

        [LogCName("状态（1：启用，2：故障，51：禁用）")]
        public byte? IStatus { get; set; }
    }
 }
