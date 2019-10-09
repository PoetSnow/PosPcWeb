using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    public class OpenTakeoutTabAddViewModel
    {
        [Display(Name = "账单id")]
        public string Billid { get; set; }

        [Display(Name = "客人姓名")]
        public string Name { get; set; }

        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        [Display(Name = "人数")]
        public int? IGuest { get; set; }

        [Display(Name = "登记营业日")]
        public DateTime? BillBsnsDate { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "营业点")]
        public string Refeid { get; set; }

        [Display(Name = "当前餐台")]
        public string Tabid { get; set; }

        [Display(Name = "餐台号")]
        public string TabNo { get; set; }
        
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

    }
}