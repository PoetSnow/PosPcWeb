using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosItemPrice")]
    [LogCName("定义消费项目不同单位的价格")]
    public class PosItemPrice
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
        [LogCName("单位id")]
        public string Unitid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("单位代码")]
        public string UnitCode { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("单位")]
        public string Unit { get; set; }
        
        [LogCName("价格")]
        public decimal? Price { get; set; }
        
        [LogCName("倍数")]
        public decimal? Multiple { get; set; }
        
        [LogCName("毛利率")]
        public decimal? Grossrate { get; set; }
        
        [LogCName("成本价")]
        public decimal? CostPrice { get; set; }
        
        [LogCName("油味")]
        public decimal? OilAmount { get; set; }
        
        [LogCName("提成")]
        public decimal? Percent { get; set; }
        
        [LogCName("会员价")]
        public decimal? MemberPrice { get; set; }
        
        [LogCName("是否缺省单位")]
        public bool? IsDefault { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("所属餐台类型id")]
        public string TabTypeid { get; set; }
        
        [LogCName("排列序号")]
        public int? Seqid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }
        
        [LogCName("修改时间")]
        public DateTime? Modified { get; set; }
    }
}
