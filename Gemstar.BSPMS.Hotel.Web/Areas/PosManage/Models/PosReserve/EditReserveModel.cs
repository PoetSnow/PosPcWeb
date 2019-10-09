using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosReserve
{
    public class EditReserveModel
    {
        /// <summary>
        /// 账单Id
        /// </summary>
        public string BillId { get; set; }

        [Display(Name = "外部订单号")]
        public string ExternalOrder { get; set; }


        [Display(Name = "客人姓名")]
        public string Name { get; set; }


        [Display(Name = "团队名称")]
        public string TeamName { get; set; }


        [Display(Name = "手机号")]
        public string MobilePhone { get; set; }

        [Display(Name = "会员卡Id")]
        public string ProfileId { get; set; }

        [Display(Name = "会员卡卡号")]
        public string ProfileNo { get; set; }

        [Display(Name = "合约单位Id")]
        public string CttId { get; set; }

        [Display(Name = "合约单位名称")]
        public string CttName { get; set; }

        [Display(Name = "返佣金额")]
        public  decimal? FYAmount { get; set; }

        [Display(Name = "营业经理")]
        public string Sale { get; set; }

        [Display(Name = "预抵日期")]
        public DateTime? OrderDate { get; set; }

        [Display(Name = "市别")]
        public string Shuffle { get; set; }

        [Display(Name = "客人类型")]
        public string GuestType { get; set; }


        [Display(Name = "预定方式")]
        public string ReservationMode { get; set; }


        [Display(Name = "结束日期")]
        public DateTime? EndeDate { get; set; }


        [Display(Name = "定金")]
        public decimal? EarnestMoney { get; set; }

        [Display(Name = "公司名称")]
        public string Company { get; set; }

        [Display(Name = "预定时长")]
        public decimal? ReservedTime { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "餐台Id")]
        public string TabId { get; set; }

        [Display(Name = "营业点Id")]
        public string RefeId { get; set; }
        
        [Display(Name = "当前营业日")]
        public DateTime? Business { get; set; }

        [Display(Name = "人数")]
        public int? IGuest { get; set; }

        [Display(Name = "会议返佣金额")]
        public decimal? HYFYAmount { get; set; }

        [Display(Name = "餐饮返佣金额")]
        public decimal? CYFYAmount { get; set; }


        [Display(Name = "餐台号")]
        public string TabNo { get; set; }

    }
}