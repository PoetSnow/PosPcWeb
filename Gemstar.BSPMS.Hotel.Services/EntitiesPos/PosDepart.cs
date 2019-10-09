using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosDepart")]
    [LogCName("出品部门")]
    public class PosDepart
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("部门类别id")]
        public string DeptClassID { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("代码")]
        public string Code { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("中文名")]
        public string Cname { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("英文名")]
        public string Ename { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("出品打印机")]
        public string ProdPrinter { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("二级仓库")]
        public string WhCode { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }
        
        [LogCName("排列序号")]
        public int? Seqid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("状态（1：启用，51：禁用）")]
        public byte? IStatus { get; set; }
    }
}
