using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosRefe
{
    public class PosRefeAddViewModel
    {
        [Display(Name = "代码")]
        [Required(ErrorMessage = "请输入代码")]
        public string Code { get; set; }

        [Display(Name = "名称")]
        [Required(ErrorMessage = "请输入名称")]
        public string Cname { get; set; }

        [Display(Name = "英文名称")]
        public string Ename { get; set; }

        [Display(Name = "所属收银点")]
        public string PosId { get; set; }

        [Display(Name = "当前市别")]
        public string ShuffleId { get; set; }

        //[Display(Name = "当前营业日")]
        //[Required(ErrorMessage = "请输入当前营业日")]
        //public DateTime? Business { get; set; }

        [Display(Name = "开台录入信息内容")]
        public string OpenInfo { get; set; }

        [Display(Name = "开台属性")]
        public PosRegType? RegType { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "二级仓库")]
        public string DepartNo { get; set; }

        [Display(Name = "是否录入锁牌")]
        public bool? Iskey { get; set; }

        [Display(Name = "是否需要显示作法")]
        public bool? IsAction { get; set; }

        [Display(Name = "是否需要显示要求")]
        public bool? IsRequest { get; set; }

        [Display(Name = "是否出品")]
        public bool? IsProdurce { get; set; }

        [Display(Name = "是否即开即买单")]
        public bool? IsPay { get; set; }

        [Display(Name = "PDA打印机")]
        public string PdaPrinter { get; set; }

        [Display(Name = "可选打印机")]
        public string Selprinter { get; set; }

        [Display(Name = "尾数处理方式")]
        public PosTagDecend? ITagDecend { get; set; }

        [Display(Name = "小数点保留位数")]
        public byte? IDecPlace { get; set; }

        [Display(Name = "买单未打单")]
        public PosTagPrintBill? ITagPrintBill { get; set; }

        [Display(Name = "落单后报警时间")]
        public byte? ISendAlert { get; set; }

        [Display(Name = "打单后报警时间")]
        public byte? IPrintAlert { get; set; }

        [Display(Name = "就座台预订时间")]
        public byte? ISeatTime { get; set; }

        [Display(Name = "预订提示时间")]
        public byte? IHoldAlert { get; set; }

        [Display(Name = "预订保留时间")]
        public byte? IOrderKeep { get; set; }

        [Display(Name = "是否打印点菜单")]
        public PosPrintBillss? IPrintBillss { get; set; }

        [Display(Name = "是否可调整价格")]
        public bool? ITagModifyCurrent { get; set; }

        [Display(Name = "清机时全部买单")]
        public bool? IsClearBill { get; set; }

        [Display(Name = "套餐是否联单打印")]
        public bool? IsSuitePrint { get; set; }

        [Display(Name = "酒席是否连单打印")]
        public bool? IsFeastPrint { get; set; }

        [Display(Name = "账单上是否打印取消项目")]
        public bool? IsDispCanDish { get; set; }

        [Display(Name = "是否启用会员预付功能")]
        public bool? IsOrderPay { get; set; }

        [Display(Name = "免最低消费照收服务费")]
        public bool? IsNoLimitSrv { get; set; }

        [Display(Name = "消费余额是否收服务费")]
        public PosTagLimitSrv? IsTagLimitSrv { get; set; }

        [Display(Name = "服务费是否可折")]
        public PosTagSrvDisc? IsTagSrvDisc { get; set; }

        [Display(Name = "最低消费是否可折")]
        public PosTagLimitDisc? IsTagLimitDisc { get; set; }

        [Display(Name = "食品价格包价服务费")]
        public bool? IsDishPriceService { get; set; }

        [Display(Name = "转台通知单是否出品")]
        public PosTabProduce? IsTabProduce { get; set; }

        [Display(Name = "转台通知单指定打印机")]
        public string TabChgProdPrinter { get; set; }

        [Display(Name = "零价项目打单处理")]
        public PosTagZeroBill? IsZeroPrintBill { get; set; }

        [Display(Name = "买单零价项目处理")]
        public PosTagZeroBill? IsBuyZeroBill { get; set; }

        [Display(Name = "打单后是否可修改")]
        public bool? IsPrintEdit { get; set; }

        [Display(Name = "买单后埋脚方式")]
        public PosTagPromptFoot? IsTagPromptFoot { get; set; }

        [Display(Name = "账单默认格式")]
        public string PosBillPrint { get; set; }

        [Display(Name = "赠送是否出品")]
        public bool? IsLargProduce { get; set; }

        [Display(Name = "服务费政策时间")]
        public PosTagServicesTime? IServicesTime { get; set; }

        [Display(Name = "买单后清台")]
        public bool? Isclrtab { get; set; }

        [Display(Name = "是否有外卖台")]
        public bool? Isoutsell { get; set; }

        [Display(Name = "开外卖台是否打印外卖单")]
        public bool? Issellprintbill { get; set; }

        [Display(Name = "输入时价菜价格")]
        public bool? Isinputcur { get; set; }

        [Display(Name = "输入自选菜价格")]
        public bool? Isinputdiy { get; set; }

        [Display(Name = "账单续打")]
        public bool? Isbillcontinue { get; set; }

        [Display(Name = "埋脚续打")]
        public bool? Ispaidcontinue { get; set; }

        [Display(Name = "周末定义")]
        public string Weekend { get; set; }

        [Display(Name = "周末开始时间")]
        public string Weekendstart { get; set; }

        [Display(Name = "周末结束时间")]
        public string Weekendend { get; set; }

        [Display(Name = "显示编码")]
        public bool? Isshowcode { get; set; }

        [Display(Name = "显示价格")]
        public bool? Isshowprice { get; set; }

        [Display(Name = "显示作法加价")]
        public bool? Isshowactionprice { get; set; }

        [Display(Name = "结转设置")]
        public PosBusinessEnd? IsBusinessend { get; set; }

        [Display(Name = "最早结转时间")]
        public string BusinessTime { get; set; }

        [Display(Name = "特价菜时间")]
        public PosOnsaleTime? IsOnsaleTime { get; set; }

        [Display(Name = "点同项目处理")]
        public PosOrderSameItem? IsOrderSameItem { get; set; }

        [Display(Name = "排列序号")]
        public int? Seqid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "进入台号是否刷卡")]
        public byte? IsOpenBrush { get; set; }

        [Display(Name = "是否显示餐台属性")]
        public bool? IsShowTableproperty { get; set; }

        [Display(Name = "楼面台号显示内容")]
        public string FloorShowData { get; set; }

        [Display(Name = "取消项目是否出品")]
        public bool? IsCanItemProd { get; set; }

        [Display(Name = "取消项目是否本地打印取消单")]
        public bool? isCanItemPrint { get; set; }

        [Display(Name = "内部编码")]
        public string CodeIn { get; set; }

        [Display(Name = "落单是否退出")]
        public bool? IsCommitQuit { get; set; }
    }
}