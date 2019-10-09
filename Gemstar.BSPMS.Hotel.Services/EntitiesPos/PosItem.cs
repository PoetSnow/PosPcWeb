using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosItem")]
    [LogCName(" 消费项目表")]
    public class PosItem
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("项目id")]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("项目代码")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("中文名")]
        public string Cname { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("英文名")]
        public string Ename { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("名称三")]
        public string Oname { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("检索码")]
        public string IndexNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("拼音码")]
        public string PYCode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("条形码")]
        public string Barcode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("大类id")]
        public string ItemClassid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("大类名称")]
        public string ItemClassName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("部门类别id")]
        public string DeptClassid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("部门类别名称")]
        public string DeptClassName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("分类id")]
        public string SubClassid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("分类名称")]
        public string SubClassName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("默认要求")]
        public string Requestid { get; set; }

        [LogCName("优先折扣")]
        public decimal? PreferDiscount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("默认厨师")]
        public string CookID { get; set; }

        [LogCName("出品名称")]
        public byte? IsProdName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("单位ID")]
        public string Unitid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("单位名称")]
        public string Unit { get; set; }

        [LogCName("价格")]
        public decimal? Price { get; set; }

        [LogCName("增值税率")]
        public decimal? TaxRate { get; set; }

        [LogCName("状态")]
        public byte? Status { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票开票项目")]
        public string ItemTaxid { get; set; }

        [LogCName("免开台项目")]
        public bool? IsAvoidOpenItem { get; set; }

        [LogCName("是否计最低消费")]
        public bool? IsLimit { get; set; }

        [LogCName("免全单最低消费")]
        public bool? IsAvoidLimit { get; set; }

        [LogCName("是否收服务费")]
        public bool? IsService { get; set; }

        [LogCName("是否可折")]
        public bool? IsDiscount { get; set; }

        [LogCName("是否可赠送")]
        public bool? IsLargess { get; set; }

        [LogCName("赠送服务费")]
        public bool? IsLgService { get; set; }

        [LogCName("数量")]
        public bool? IsQuan { get; set; }

        [LogCName("是否手写单")]
        public bool? IsHandWrite { get; set; }

        [LogCName("是否可手工录入手写菜名")]
        public bool? IsInput { get; set; }

        [LogCName("是否套餐")]
        public bool? IsSuite { get; set; }

        [LogCName("是否酒席")]
        public bool? IsFeast { get; set; }

        [LogCName("自定义")]
        public bool? IsUserDefined { get; set; }

        [LogCName("是否海鲜")]
        public bool? IsSeaFood { get; set; }

        [LogCName("是否称重")]
        public bool? IsWeight { get; set; }

        [LogCName("上榜")]
        public bool? IsSort { get; set; }

        [LogCName("显示作法")]
        public bool? IsAutoAction { get; set; }

        [LogCName("点作法选项")]
        public byte? IsOrderAction { get; set; }

        [LogCName("时价菜")]
        public bool? IsCurrent { get; set; }

        [LogCName("是否茶位费")]
        public bool? IsTea { get; set; }

        [LogCName("自动沽清")]
        public bool? IsAutoSellout { get; set; }

        [LogCName("是否自助餐")]
        public bool? IsSelfhelp { get; set; }

        [LogCName("主打菜")]
        public bool? IsFreq { get; set; }

        [LogCName("收银台商品")]
        public bool? IsCash { get; set; }

        [LogCName("是否点心")]
        public bool? IsNosh { get; set; }

        [LogCName("是否打点菜单")]
        public bool? IsOrderPrt { get; set; }

        [LogCName("自选菜")]
        public bool? IsDiy { get; set; }

        [LogCName("减库存")]
        public byte? IsStock { get; set; }

        [LogCName("会员本金")]
        public bool? IsMbrBaseamt { get; set; }

        [LogCName("酒水")]
        public bool? IsWineBar { get; set; }

        [LogCName("推销员")]
        public bool? IsSale { get; set; }

        [LogCName("代收")]
        public bool? IsEra { get; set; }

        [LogCName("是否分摊金额")]
        public bool? IsSplit { get; set; }

        [LogCName("是否计提成")]
        public bool? IsPresent { get; set; }

        [LogCName("是否积分")]
        public bool? IsScore { get; set; }

        [LogCName("微信显示")]
        public bool? IsWxShow { get; set; }

        [LogCName("IPAD显示")]
        public bool? IsPaidShow { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("付款还是消费")]
        public string DcFlag { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("处理方式")]
        public string PayType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("币种")]
        public string Montypeno { get; set; }

        [LogCName("汇率")]
        public decimal? Rate { get; set; }

        [LogCName("计收入")]
        public bool? IsInCome { get; set; }

        [LogCName("可找赎")]
        public bool? IsChange { get; set; }

        [LogCName("可作订金")]
        public bool? IsSubscription { get; set; }

        [LogCName("可支出")]
        public bool? IsPayout { get; set; }

        [LogCName("是否可充值")]
        public bool? IsCharge { get; set; }

        [LogCName("是否开发票")]
        public bool? IsInvoice { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("所属营业点")]
        public string Refeid { get; set; }

        [LogCName("是否分类")]
        public bool? IsSubClass { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }

        [LogCName("排列顺序")]
        public int? Seqid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("操作员")]
        public string OperName { get; set; }

        [LogCName("修改时间")]
        public DateTime? ModifiedDate { get; set; }

        [LogCName("是否开台项目")]
        public bool? IsOpenItem { get; set; }

        [LogCName("是否计时项目")]
        public bool? IsTimes { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("内部编码（捷云营业点外部代码）")]
        public string CodeIn { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("外部代码：第三方软件的消费项目的编码")]
        public string OutCodeNo { get; set; }

        [LogCName("是否库存物品，如果是则它是当作消费项目的组成物品来定义，到时在组成定义里就可以显示此物品")]
        public bool? IsCostItem { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("消费项目显示：主要是设置消费项目、分类可以在电脑端、手机端程序上的显示设置。所以需要设置一个视图来定义相关的内容。目前可以知道的是PC端；手机端；IPAD端等。")]
        public string ShowSet { get; set; }

        [LogCName("是否不可反结")]
        public bool? IsRepay { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("图片地址")]
        public string Bmp { get; set; }

        [LogCName("是否可修改单价")]    //新增的是否可修改单价字段- 20
        public bool? IsModiPrice { get; set; }
    }
}