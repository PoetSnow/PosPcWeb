using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosProducelist")]
    [LogCName("出品记录表")]
    public class PosProducelist
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("单号")]
        public string Billid { get; set; }
        
        [LogCName("消费id")]
        public Int64? Id { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("营业点代码")]
        public string RefeNo { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("营业点名称")]
        public string RefeName { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("台号")]
        public string TabNo { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("台名")]
        public string TabName { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("客人姓名")]
        public string VGuest { get; set; }
        
        [LogCName("人数")]
        public int? IGuest { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("点菜人")]
        public string OperName { get; set; }
        
        [LogCName("点菜时间")]
        public DateTime? OrderRecord { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("酒席编码")]
        public string DishSuitNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒席名称")]
        public string DishSuitName { get; set; }
        
        [LogCName("酒席数量")]
        public decimal? DishSuitQuan { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("酒席单位")]
        public string DishSuitUnit { get; set; }
        
        [LogCName("酒席单价")]
        public decimal? DishSuitPrice { get; set; }
        
        [LogCName("酒席金额")]
        public decimal? DishSuitAmount { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("项目编码")]
        public string DishNo { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("项目名称")]
        public string DishName { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("项目英文名")]
        public string DishEname { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("项目其它名")]
        public string DishOname { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("单位")]
        public string DishUnit { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("单位英文名")]
        public string DishUnitE { get; set; }
        
        [LogCName("数量")]
        public decimal? Quan { get; set; }
        
        [LogCName("条只")]
        public decimal? Piece { get; set; }
        
        [LogCName("单价")]
        public decimal? Price { get; set; }
        
        [LogCName("金额")]
        public decimal? Amount { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("出品条码")]
        public string BarCode { get; set; }
        
        [LogCName("出品状态")]
        public byte? TagProduce { get; set; }
        
        [LogCName("出品次数")]
        public byte? ProdTime { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("出品端口")]
        public string PrtNo { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("作法")]
        public string Action { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("要求")]
        public string Request { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("客位")]
        public string Place { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("厨师")]
        public string Cookie { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("推销员")]
        public string Sale { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("部门类别")]
        public string DeptClass { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("酒卡号")]
        public string BarCard { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("消费状态")]
        public string Status { get; set; }

        [Key]
        [LogCName("流水号")]
        public long Seqno { get; set; }
    }
}