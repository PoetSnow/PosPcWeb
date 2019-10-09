using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PostCost")]
    [LogCName("消费项目库存物品组成")]
    public class PostCost
    {
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("类型名称")]
        public string PostSysName { get; set; }

       
        [Column(TypeName = "varchar")]
        [LogCName("项目id")]
        public string Itemid { get; set; }

       
        [Column(TypeName = "varchar")]
        [LogCName("单位id")]
        public string Unitid { get; set; }

       
        [Column(TypeName = "varchar")]
        [LogCName("库存物品id")]
        public string CostItemid { get; set; }

       
        [Column(TypeName = "varchar")]
        [LogCName("库存物品单位id")]
        public string CostItemUnitid { get; set; }

       
        [LogCName("组成数量")]
        public decimal? OriQuan { get; set; }

       
        [LogCName("实际数量:不能手工输入，必须通过计算方式计算出来：实际数量=组成数量/ 出成率")]
        public decimal? Quantity { get; set; }

       
        [LogCName("辅助数量")]
        public decimal? Quantity2 { get; set; }

       
        [LogCName("出成率:默认为1")]
        public decimal? XRate { get; set; }

       
        [LogCName("单价:库存物品的单价")]
        public decimal? Price { get; set; }

       
        [LogCName("金额:=实际数量 * 单价")]
        public decimal? Amount { get; set; }

       
        [LogCName("修改时间")]
        public DateTime? Modifieddate { get; set; }

       
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

    }
}
