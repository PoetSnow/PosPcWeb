using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosBillDetail")]
    [LogCName("账单明细表")]
    public class PosBillDetail
    {
        [Key]
        [LogCName("id")]
        public Int64 Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("账单id")]
        public string MBillid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("业务单id")]
        public string Billid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("项目id")]
        public string Itemid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("项目代码")]
        public string ItemCode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("项目名称")]
        public string ItemName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("单位id")]
        public string Unitid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("单位代码")]
        public string UnitCode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("单位名称")]
        public string UnitName { get; set; }

        [LogCName("数量")]
        public decimal? Quantity { get; set; }

        [LogCName("称重条只")]
        public decimal? Piece { get; set; }

        [LogCName("扣钝倍数")]
        public decimal? Multiple { get; set; }

        [LogCName("单价")]
        public decimal? Price { get; set; }

        [LogCName("作法加价")]
        public decimal? AddPrice { get; set; }

        [LogCName("折前金额")]
        public decimal? Dueamount { get; set; }

        [LogCName("金额")]
        public decimal? Amount { get; set; }

        [LogCName("服务费")]
        public decimal? Service { get; set; }

        [LogCName("税额")]
        public decimal? Tax { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("客位")]
        public string Place { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("作法")]
        public string Action { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("要求")]
        public string Request { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("消费还是付款")]
        public string DcFlag { get; set; }

        [LogCName("结账状态")]
        public bool? IsCheck { get; set; }

        [LogCName("自动标志")]
        public byte? Isauto { get; set; }

        [LogCName("计费状态")]
        public byte? Status { get; set; }

        [LogCName("出品状态")]
        public byte? IsProduce { get; set; }

        [LogCName("求和套餐")]
        public bool? SP { get; set; }

        [LogCName("求和套餐明细")]
        public bool? SD { get; set; }

        [LogCName("所属套餐")]
        public Guid? Upid { get; set; }

        [LogCName("套餐分摊金额")]
        public decimal? OverSuite { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("餐台id")]
        public string Tabid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("锁牌号")]
        public string Keyid { get; set; }

        [LogCName("单道折扣")]
        public decimal? Discount { get; set; }

        [LogCName("是否单道折扣")]
        public bool? IsDishDisc { get; set; }

        [LogCName("单道金额折")]
        public decimal? DiscAmount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("批次")]
        public string BatchTime { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("业务员")]
        public string Sale { get; set; }

        [LogCName("消费营业日")]
        public DateTime? TransBsnsDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("消费市别")]
        public string TransShuffleid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("消费班次")]
        public string TransShiftid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("入账操作员")]
        public string TransUser { get; set; }

        [LogCName("消费时间")]
        public DateTime? TransDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("修改操作员")]
        public string ModiUser { get; set; }

        [LogCName("修改时间")]
        public DateTime? ModiDate { get; set; }

        [LogCName("结账id")]
        public Guid? Settleid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("结账交易号")]
        public string SettleTransno { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("微信支付宝交易号")]
        public string SettlePayId { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("结账交易名称")]
        public string SettleTransName { get; set; }

        [LogCName("结账营业日")]
        public DateTime? SettleBsnsDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("结账市别")]
        public string SettleShuffleid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("结账班次")]
        public string SettleShiftId { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("结账操作员")]
        public string SettleUser { get; set; }

        [LogCName("结账时间")]
        public DateTime? SettleDate { get; set; }

        [LogCName("原价")]
        public decimal? PriceOri { get; set; }

        [LogCName("会员价")]
        public decimal? PriceClub { get; set; }

        [LogCName("会员折扣")]
        public decimal? DiscountClub { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("批准人")]
        public string Approver { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("取消原因")]
        public string CanReason { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Memo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("手工单号")]
        public string Acbillno { get; set; }

        [LogCName("打单次数")]
        public byte? iPrint { get; set; }

        [LogCName("点菜单次数")]
        public byte? iOrderPrint { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("点菜类型")]
        public string OrderType { get; set; }


        [LogCName("称重原数量")]
        public decimal? OriQuan { get; set; }

        [LogCName("是否称重")]
        public bool? IsWeight { get; set; }


        [Column(TypeName = "varchar")]
        [LogCName("微信OpenId")]
        public string OpenWxid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("扫码点餐上级分类ID")]
        public string ItemSubid { get; set; }

        [LogCName("公司账")]
        public decimal? outComeAmount { get; set; }

        [LogCName("传菜状态")] //（0：未传菜；1：已传菜）
        public byte? sentStatus { get; set; }
    }
}