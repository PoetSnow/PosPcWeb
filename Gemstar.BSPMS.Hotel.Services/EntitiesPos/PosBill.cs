using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosBill")]
    [LogCName("账单主表每个分营业点登记的时候都产生一条记录，但都同时对应一个主单id，每一条消费同时记录主单id和分单id，结账时使用主单来关联明细消费，但消费统计时使用分单id来统计。")]
    public class PosBill
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("单号")]
        public string Billid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("可视单号")]
        public string BillNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("主单号")]
        public string MBillid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("客人姓名")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("手机号码")]
        public string Mobile { get; set; }

        [LogCName("人数")]
        public int? IGuest { get; set; }

        [LogCName("登记营业日")]
        public DateTime? BillBsnsDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("登记操作员")]
        public string InputUser { get; set; }

        [LogCName("登记时间")]
        public DateTime? BillDate { get; set; }

        [LogCName("离店营业日")]
        public DateTime? DepBsnsDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("离店操作员")]
        public string MoveUser { get; set; }

        [LogCName("离店时间")]
        public DateTime? DepDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("营业点id")]
        public string Refeid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("当前餐台id")]
        public string Tabid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("餐台号")]
        public string TabNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("当前锁牌id")]
        public string Keyid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("开台卡号")]
        public string Invno { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("业务员")]
        public string Sale { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("关联号")]
        public string LinkNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("会员id")]
        public string Profileid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("会员卡号")]
        public string CardNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("合约单位id")]
        public string Cttid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("合约单位名称")]
        public string CttName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("消费人")]
        public string Consumer { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("折扣批准人")]
        public string Approver { get; set; }

        [LogCName("折扣类型")]
        public byte? IsForce { get; set; }

        [LogCName("全单折扣")]
        public decimal? Discount { get; set; }

        [LogCName("金额折类型")]
        public byte? DaType { get; set; }

        [LogCName("金额折")]
        public decimal? DiscAmount { get; set; }

        [LogCName("状态")]
        public byte? Status { get; set; }

        [LogCName("是否预订单")]
        public bool? IsOrder { get; set; }

        [LogCName("是否迟付")]
        public bool? IsOver { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("客人类型id")]
        public string CustomerTypeid { get; set; }

        [LogCName("是否收服务费")]
        public bool? IsService { get; set; }

        [LogCName("服务费率")]
        public decimal? ServiceRate { get; set; }

        [LogCName("是否计最低消费")]
        public bool? IsLimit { get; set; }

        [LogCName("最低消费")]
        public decimal? Limit { get; set; }

        [LogCName("最低消费是否按人计")]
        public bool? IsByPerson { get; set; }

        [LogCName("最低消费时长")]
        public int? IHour { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("开台班次")]
        public string Shiftid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("开台市别")]
        public string Shuffleid { get; set; }

        [LogCName("账单打单次数")]
        public int? IPrint { get; set; }

        [LogCName("埋脚单打单次数")]
        public int? IPaidPrint { get; set; }

        [LogCName("KTV开台类型")]
        public byte? IKtvStatus { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("开台备注")]
        public string OpenMemo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("收银备注")]
        public string CashMemo { get; set; }

        [LogCName("餐台标识")]
        public byte? TabFlag { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("自动备注")]
        public string Memo { get; set; }

        [LogCName("财务ID")]
        public Int64? Accid { get; set; }

        [LogCName("税额")]
        public decimal? TaxAmt { get; set; }

        [LogCName("服务费")]
        public decimal? ServiceAmt { get; set; }

        [LogCName("消费余额")]
        public decimal? LimitBalance { get; set; }

        [LogCName("抹零")]
        public decimal? BlotAmt { get; set; }

        [LogCName("赠送金额")]
        public decimal? LargessAmt { get; set; }

        [LogCName("付款金额")]
        public decimal? PayAmt { get; set; }

        [LogCName("发票金额")]
        public decimal? InvoiceAmt { get; set; }

        [LogCName("最后修改时间（落单）")]
        public DateTime? LastRecord { get; set; }

        [LogCName("账单打印时间")]
        public DateTime? PrintRecord { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("微信OpenId")]
        public string OpenWxid { get; set; }

        [LogIgnore]
        [LogCName("预抵时间")]
        public DateTime? OrderDate { get; set; }

        [LogIgnore]
        [LogCName("预订时间")]
        public DateTime? OrderOperDate { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("预订操作员")]
        public string OrderUser { get; set; }

        [LogIgnore]
        [LogCName("预订截止日期")]
        public DateTime? OrderExpired { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("推销员")]
        public string SalesMan { get; set; }
    }
}