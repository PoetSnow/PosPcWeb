using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItem
{
    public class PosItemAddViewModel
    {
        [Display(Name = "项目代码")]
        [Required(ErrorMessage = "请输入项目代码")]
        public string Code { get; set; }

        [Display(Name = "中文名称")]
        [Required(ErrorMessage = "请输入中文名称")]
        public string Cname { get; set; }

        [Display(Name = "英文名称")]
        public string Ename { get; set; }

        [Display(Name = "名称三")]
        public string Oname { get; set; }
        
        [Display(Name = "条形码")]
        public string Barcode { get; set; }

        [Display(Name = "项目大类")]
        [Required(ErrorMessage = "请选择项目大类")]
        public string ItemClassid { get; set; }

        [Display(Name = "部门类别")]
        [Required(ErrorMessage = "请选择部门类别")]
        public string DeptClassid { get; set; }

        [Display(Name = "项目分类")]
        public string SubClassid { get; set; }

        [Display(Name = "默认要求")]
        public string Requestid { get; set; }

        [Display(Name = "优先折扣")]
        public decimal? PreferDiscount { get; set; }

        [Display(Name = "默认厨师")]
        public string CookID { get; set; }

        [Display(Name = "出品名称")]
        public byte? IsProdName { get; set; }

        [Display(Name = "单位")]
        public string Unitid { get; set; }
        
        [Display(Name = "价格")]
        public decimal? Price { get; set; }

        [Display(Name = "增值税率")]
        public decimal? TaxRate { get; set; }

        [Display(Name = "状态")]
        public byte? Status { get; set; }

        [Display(Name = "发票开票项目")]
        public string ItemTaxid { get; set; }

        [Display(Name = "免开台项目")]
        public bool? IsAvoidOpenItem { get; set; }

        [Display(Name = "是否计最低消费")]
        public bool? IsLimit { get; set; }

        [Display(Name = "免全单最低消费")]
        public bool? IsAvoidLimit { get; set; }

        [Display(Name = "是否收服务费")]
        public bool? IsService { get; set; }

        [Display(Name = "是否可折")]
        public bool? IsDiscount { get; set; }

        [Display(Name = "是否可赠送")]
        public bool? IsLargess { get; set; }

        [Display(Name = "赠送服务费")]
        public bool? IsLgService { get; set; }

        [Display(Name = "数量")]
        public bool? IsQuan { get; set; }

        [Display(Name = "是否手写单")]
        public bool? IsHandWrite { get; set; }

        [Display(Name = "是否可手工录入手写菜名")]
        public bool? IsInput { get; set; }

        [Display(Name = "是否套餐")]
        public bool? IsSuite { get; set; }

        [Display(Name = "是否酒席")]
        public bool? IsFeast { get; set; }

        [Display(Name = "套餐或酒席自定义")]
        public bool? IsUserDefined { get; set; }

        [Display(Name = "是否海鲜")]
        public bool? IsSeaFood { get; set; }

        [Display(Name = "是否称重")]
        public bool? IsWeight { get; set; }

        [Display(Name = "上榜")]
        public bool? IsSort { get; set; }

        [Display(Name = "显示作法")]
        public bool? IsAutoAction { get; set; }

        [Display(Name = "点作法选项")]
        public byte? IsOrderAction { get; set; }

        [Display(Name = "时价菜")]
        public bool? IsCurrent { get; set; }

        [Display(Name = "是否茶位费")]
        public bool? IsTea { get; set; }

        [Display(Name = "自动沽清")]
        public bool? IsAutoSellout { get; set; }

        [Display(Name = "是否自助餐")]
        public bool? IsSelfhelp { get; set; }

        [Display(Name = "主打菜")]
        public bool? IsFreq { get; set; }

        [Display(Name = "收银台商品")]
        public bool? IsCash { get; set; }

        [Display(Name = "是否点心")]
        public bool? IsNosh { get; set; }

        [Display(Name = "是否打点菜单")]
        public bool? IsOrderPrt { get; set; }

        [Display(Name = "自选菜")]
        public bool? IsDiy { get; set; }

        [Display(Name = "减库存")]
        public byte? IsStock { get; set; }

        [Display(Name = "会员本金")]
        public bool? IsMbrBaseamt { get; set; }

        [Display(Name = "酒水")]
        public bool? IsWineBar { get; set; }

        [Display(Name = "推销员")]
        public bool? IsSale { get; set; }

        [Display(Name = "代收")]
        public bool? IsEra { get; set; }

        [Display(Name = "是否分摊金额")]
        public bool? IsSplit { get; set; }

        [Display(Name = "是否计提成")]
        public bool? IsPresent { get; set; }

        [Display(Name = "是否积分")]
        public bool? IsScore { get; set; }

        [Display(Name = "微信显示")]
        public bool? IsWxShow { get; set; }

        [Display(Name = "IPAD显示")]
        public bool? IsPaidShow { get; set; }

        [Display(Name = "处理方式")]
        public string PayType { get; set; }

        [Display(Name = "币种")]
        public string Montypeno { get; set; }

        [Display(Name = "汇率")]
        public decimal? Rate { get; set; }

        [Display(Name = "计收入")]
        public bool? IsInCome { get; set; }

        [Display(Name = "可找赎")]
        public bool? IsChange { get; set; }

        [Display(Name = "可作订金")]
        public bool? IsSubscription { get; set; }

        [Display(Name = "可支出")]
        public bool? IsPayout { get; set; }

        [Display(Name = "是否可充值")]
        public bool? IsCharge { get; set; }

        [Display(Name = "是否开发票")]
        public bool? IsInvoice { get; set; }

        [Display(Name = "所属营业点")]
        public string Refeid { get; set; }

        [Display(Name = "是否分类")]
        public bool? IsSubClass { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "排列顺序")]
        public int? Seqid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "是否开台项目")]
        public bool? IsOpenItem { get; set; }

        [Display(Name = "是否计时项目")]
        public bool? IsTimes { get; set; }

        [Display(Name = "消费项目显示")]
        public string ShowSet { get; set; }
        [Display(Name = "是否库存物品")]
        public bool? IsCostItem { get; set; }
        [Display(Name = "外部代码")]
        public string OutCodeNo { get; set; }

        [Display(Name = "是否可修改单价")]
        public bool? IsModiPrice { get; set; }

        public string[] ShowSets
        {
            get { return string.IsNullOrEmpty(ShowSet) ? new string[] { } : ShowSet.Split(','); }
            set { ShowSets = value; }
        }
    }
}