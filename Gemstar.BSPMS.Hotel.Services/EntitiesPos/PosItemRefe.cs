using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosItemRefe")]
    [LogCName("消费项目对应营业点")]
    public class PosItemRefe
    {
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("项目id")]
        public string Itemid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("所属营业点")]
        public string Refeid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("营业点名称")]
        public string RefeName { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("出品打印机")]
        public string ProdPrinter { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("所属市别")]
        public string Shuffleid { get; set; }
        
        [LogCName("合并出品部门")]
        public bool? IsDepartPrint { get; set; }
        
        [LogCName("合并餐台")]
        public bool? IsTabPrint { get; set; }
        
        [LogCName("排列序号")]
        public int? Seqid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }
        
        [LogCName("修改时间")]
        public DateTime? Modified { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("传菜打印机")]
        public string SentPrtNo { get; set; }
    }
}
