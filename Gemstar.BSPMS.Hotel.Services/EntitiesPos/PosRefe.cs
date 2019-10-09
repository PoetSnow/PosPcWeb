using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosRefe")]
    [LogCName("营业点定义")]
    public class PosRefe
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("代码")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("名称")]
        public string Cname { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("英文名")]
        public string Ename { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("所属收银点")]
        public string PosId { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("当前市别")]
        public string ShuffleId { get; set; }

        //[LogCName("当前营业日")]
        //public DateTime? Business { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("开台录入信息内容")]
        public string OpenInfo { get; set; }

        [LogCName("开台属性")]
        public PosRegType? RegType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("二级仓库")]
        public string DepartNo { get; set; }

        [LogCName("是否录入锁牌")]
        public bool? Iskey { get; set; }

        [LogCName("是否需要显示作法")]
        public bool? IsAction { get; set; }

        [LogCName("是否需要显示要求")]
        public bool? IsRequest { get; set; }

        [LogCName("是否出品")]
        public bool? IsProdurce { get; set; }

        [LogCName("是否即开即买单")]
        public bool? IsPay { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("PDA打印机")]
        public string PdaPrinter { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("可选打印机")]
        public string Selprinter { get; set; }

        [LogCName("尾数处理方式")]
        public PosTagDecend? ITagDecend { get; set; }

        [LogCName("小数点保留位数")]
        public byte? IDecPlace { get; set; }

        [LogCName("买单未打单")]
        public PosTagPrintBill? ITagPrintBill { get; set; }

        [LogCName("落单后报警时间")]
        public byte? ISendAlert { get; set; }

        [LogCName("打单后报警时间")]
        public byte? IPrintAlert { get; set; }

        [LogCName("就座台预订时间")]
        public byte? ISeatTime { get; set; }

        [LogCName("预订提示时间")]
        public byte? IHoldAlert { get; set; }

        [LogCName("预订保留时间")]
        public byte? IOrderKeep { get; set; }

        [LogCName("是否打印点菜单")]
        public PosPrintBillss? IPrintBillss { get; set; }

        [LogCName("是否可调整价格")]
        public bool? ITagModifyCurrent { get; set; }

        [LogCName("清机时全部买单")]
        public bool? IsClearBill { get; set; }

        [LogCName("套餐是否联单打印")]
        public bool? IsSuitePrint { get; set; }

        [LogCName("酒席是否连单打印")]
        public bool? IsFeastPrint { get; set; }

        [LogCName("账单上是否打印取消项目")]
        public bool? IsDispCanDish { get; set; }

        [LogCName("是否启用会员预付功能")]
        public bool? IsOrderPay { get; set; }

        [LogCName("免最低消费照收服务费")]
        public bool? IsNoLimitSrv { get; set; }

        [LogCName("消费余额是否收服务费")]
        public PosTagLimitSrv? IsTagLimitSrv { get; set; }

        [LogCName("服务费是否可折")]
        public PosTagSrvDisc? IsTagSrvDisc { get; set; }

        [LogCName("最低消费是否可折")]
        public PosTagLimitDisc? IsTagLimitDisc { get; set; }

        [LogCName("食品价格包价服务费")]
        public bool? IsDishPriceService { get; set; }

        [LogCName("转台通知单是否出品")]
        public PosTabProduce? IsTabProduce { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("转台通知单指定打印机")]
        public string TabChgProdPrinter { get; set; }

        [LogCName("零价项目打单处理")]
        public PosTagZeroBill? IsZeroPrintBill { get; set; }

        [LogCName("买单零价项目处理")]
        public PosTagZeroBill? IsBuyZeroBill { get; set; }

        [LogCName("打单后是否可修改")]
        public bool? IsPrintEdit { get; set; }

        [LogCName("买单后埋脚方式")]
        public PosTagPromptFoot? IsTagPromptFoot { get; set; }

        [LogCName("赠送是否出品")]
        public bool? IsLargProduce { get; set; }

        [LogCName("服务费政策时间")]
        public PosTagServicesTime? IServicesTime { get; set; }

        [LogCName("买单后清台")]
        public bool? Isclrtab { get; set; }

        [LogCName("是否有外卖台")]
        public bool? Isoutsell { get; set; }

        [LogCName("开外卖台是否打印外卖单")]
        public bool? Issellprintbill { get; set; }

        [LogCName("输入时价菜价格")]
        public bool? Isinputcur { get; set; }

        [LogCName("输入自选菜价格")]
        public bool? Isinputdiy { get; set; }

        [LogCName("账单续打")]
        public bool? Isbillcontinue { get; set; }

        [LogCName("埋脚续打")]
        public bool? Ispaidcontinue { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("周末定义")]
        public string Weekend { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("周末开始时间")]
        public string Weekendstart { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("周末结束时间")]
        public string Weekendend { get; set; }

        [LogCName("显示编码")]
        public bool? Isshowcode { get; set; }

        [LogCName("显示价格")]
        public bool? Isshowprice { get; set; }

        [LogCName("显示作法加价")]
        public bool? Isshowactionprice { get; set; }

        [LogCName("结转设置")]
        public PosBusinessEnd? IsBusinessend { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("最早结转时间")]
        public string BusinessTime { get; set; }

        [LogCName("特价菜时间")]
        public PosOnsaleTime? IsOnsaleTime { get; set; }

        [LogCName("点同项目处理")]
        public PosOrderSameItem? IsOrderSameItem { get; set; }

        [LogCName("排列序号")]
        public int? Seqid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("开台是否刷卡")]
        public byte? IsOpenBrush { get; set; }

        [LogCName("是否显示餐台属性")]
        public bool? IsShowTableproperty { get; set; }

        [LogCName("楼面台号显示内容")]
        public string FloorShowData { get; set; }

        [LogCName("取消项目是否出品")]
        public bool? IsCanItemProd { get; set; }

        [LogCName("取消项目是否本地打印取消单")]
        public bool? isCanItemPrint { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("内部编码（捷云营业点外部代码）")]
        public string CodeIn { get; set; }

        [LogCName("状态（1：启用，51：禁用）")]
        public byte? IStatus { get; set; }

        [LogCName("落单是否退出")]
        public bool? IsCommitQuit { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("账单默认格式")]
        public string PosBillPrint { get; set; }
    }
}