using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosBillCost")]
    [LogCName("消费项目仓库耗用表")]
    public class PosBillCost
    {
        [LogIgnore]
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("类型名称")]
        public string PostSysName { get; set; }

        [LogIgnore]
        [LogCName("账单消费ID")]
        public Int64? BillID { get; set; }

        [LogIgnore]
        [LogCName("营业日")]
        public DateTime? BillBsnsDate { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("项目id")]
        public string Itemid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("单位id")]
        public string Unitid { get; set; }

        [LogIgnore]
        [LogCName("计费状态")]
        public byte? Status { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("仓库单号")]
        public string WhBillNo { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("二级仓库")]
        public string WhCode { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("库存物品id")]
        public string CostItemid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("库存物品单位id")]
        public string CostItemUnitid { get; set; }

        [LogIgnore]
        [LogCName("组成数量")]
        public decimal? OriQuan { get; set; }

        [LogIgnore]
        [LogCName("实际数量")]
        public decimal? Quantity { get; set; }

        [LogIgnore]
        [LogCName("辅助数量")]
        public decimal? Quantity2 { get; set; }

        [LogIgnore]
        [LogCName("出成率")]
        public decimal? XRate { get; set; }

        [LogIgnore]
        [LogCName("单价")]
        public decimal? Price { get; set; }

        [LogIgnore]
        [LogCName("金额")]
        public decimal? Amount { get; set; }

        [LogIgnore]
        [LogCName("是否生成耗用单")]
        public bool? IsBuildBill { get; set; }

        [LogIgnore]
        [LogCName("是否核对单价")]
        public bool? IsCheckPrice { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("创建操作员")]
        public string TransUser { get; set; }

        [LogIgnore]
        [LogCName("创建时间")]
        public DateTime? TransDate { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("修改操作员")]
        public string ModiUser { get; set; }

        [LogIgnore]
        [LogCName("修改时间")]
        public DateTime? Modifieddate { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("库存物品外部代码")]
        public string OutCodeNo { get; set; }

    }
}
