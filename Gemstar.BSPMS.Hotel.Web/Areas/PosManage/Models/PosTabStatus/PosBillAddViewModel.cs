using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosTabStatus
{
    public class PosBillAddViewModel
    {
        [Display(Name = "客人姓名")]
        public string Name { get; set; }

        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        [Display(Name = "人数")]
        public int? IGuest { get; set; }

        [Display(Name = "登记营业日")]
        public DateTime? BillBsnsDate { get; set; }

        [Display(Name = "离店营业日")]
        public DateTime? DepBsnsDate { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "营业点id")]
        public string Refeid { get; set; }

        [Display(Name = "当前餐台id")]
        public string Tabid { get; set; }

        [Display(Name = "餐台号")]
        public string TabNo { get; set; }

        [Display(Name = "当前锁牌id")]
        public string Keyid { get; set; }

        [Display(Name = "开台卡号")]
        public string Invno { get; set; }

        [Display(Name = "业务员")]
        public string Sale { get; set; }

        [Display(Name = "关联号")]
        public string LinkNo { get; set; }

        [Display(Name = "会员id")]
        public string Profileid { get; set; }

        [Display(Name = "会员卡号")]
        public string CardNo { get; set; }

        [Display(Name = "合约单位id")]
        public string Cttid { get; set; }

        [Display(Name = "合约单位名称")]
        public string CttName { get; set; }

        [Display(Name = "消费人")]
        public string Consumer { get; set; }

        [Display(Name = "折扣批准人")]
        public string Approver { get; set; }

        [Display(Name = "折扣类型")]
        public byte? IsForce { get; set; }

        [Display(Name = "金额折类型")]
        public byte? DaType { get; set; }

        [Display(Name = "金额折")]
        public decimal? DiscAmount { get; set; }

        [Display(Name = "是否预订单")]
        public bool? IsOrder { get; set; }

        [Display(Name = "是否迟付")]
        public bool? IsOver { get; set; }

        [Display(Name = "客人类型id")]
        public string CustomerTypeid { get; set; }

        [Display(Name = "账单打单次数")]
        public int? IPrint { get; set; }

        [Display(Name = "埋脚单打单次数")]
        public int? IPaidPrint { get; set; }

        [Display(Name = "KTV开台类型")]
        public byte? IKtvStatus { get; set; }

        [Display(Name = "开台备注")]
        public string OpenMemo { get; set; }

        [Display(Name = "收银备注")]
        public string CashMemo { get; set; }

        [Display(Name = "返回类型")]
        public byte ReturnType { get; set; }

        [Display(Name = "用户计算机名称")]
        public string ComputerName { get; set; }

        public string OriginposBillJsonData { get; set; }


        public string BillId { get; set; }
        [Display(Name = "推销员")]
        public string SalesMan { get; set; }
    }
}