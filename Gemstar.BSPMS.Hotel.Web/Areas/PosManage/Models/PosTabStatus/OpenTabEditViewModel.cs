using Gemstar.BSPMS.Hotel.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosTabStatus
{
    public class OpenTabEditViewModel : BaseEditViewModel
    {
        [Display(Name = "单号")]
        public string Billid { get; set; }

        [Display(Name = "营业点")]
        public string Refeid { get; set; }

        [Display(Name = "台号")]
        public string TabNo { get; set; }

        [Display(Name = "客人姓名")]
        public string Name { get; set; }

        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        [Display(Name = "人数")]
        public int? IGuest { get; set; }

        [Display(Name = "开台卡号")]
        public string Invno { get; set; }

        [Display(Name = "营业经理")]
        public string Sale { get; set; }

        [Display(Name = "会员卡号")]
        public string CardNo { get; set; }

        [Display(Name = "客人类型")]
        public string CustomerTypeid { get; set; }

        [Display(Name = "开台备注")]
        public string OpenMemo { get; set; }

        [Display(Name = "开台录入信息内容")]
        public string OpenInfo { get; set; }
        [Display(Name = "推销员")]
        public string SalesMan { get; set; }
    }
}