using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosReserve
{
    public class AddPrepayViewModel
    {
        //[Required(ErrorMessage = "请输入手机号")]
        //[RegularExpression(RegexHelper.MobileRegexString,ErrorMessage ="请输入有效的手机号")]


        //[Required(ErrorMessage = "请输入酒店hid")]
        //[Display(Name = "酒店hid")]
        //public string HId { get; set; }
        public string OriginJsonData { get; set; }

        [Display(Name = "会员")]
        public string CardId { get; set; }

        [Display(Name = "会员卡号")]
        public string folioMbrCardNo { get; set; }

        [Display(Name = "姓名")]
        public string VGuest { get; set; }

        [Display(Name = "原单号")]
        public string OriBillNo { get; set; }

        [Display(Name = "押金单号")]
        public string BillNo { get; set; }

        [Display(Name = "营业点")]
        public string PosNo { get; set; }

        [Display(Name = "押金类型")]
        public byte? IType { get; set; }

        [Display(Name = "付款方式")]
        public string PayModeNo { get; set; }

        [Display(Name = "金额")]
        public decimal? Amount { get; set; }

        [Display(Name = "本位币金额")]
        public decimal? Amountbwb { get; set; }

        [Display(Name = "批准人")]
        public string Approver { get; set; }

        [Display(Name = "付款单号")]
        public string PaidNo { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "创建人")]
        public string Creator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "修改人")]
        public string ModifiCator { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "押金状态")]
        public byte? IPrepay { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "营业日")]
        public DateTime? DBusiness { get; set; }

        public string _DBusiness { get; set; }

        [Display(Name = "班次")]
        public string Shiftid { get; set; }

        [Display(Name = "收银点")]
        public string PosName { get; set; }

        [Display(Name = "班次")]
        public string ShiftName { get; set; }

        /// <summary>
        /// 押金类型
        /// </summary>
        [Display(Name = "押金类型")]
        public string ITypeName { get; set; }

        [Display(Name = "联系电话")]
        public string Mobile { get; set; }


        [Display(Name = "收据号码")]
        public string HandBillno { get; set; }

        [Display(Name = "使用时间")]
        public DateTime? UsedDate { get; set; }

        [Display(Name = "使用说明")]
        public string UsedDesc { get; set; }


        [Display(Name = "是否短信通知")]
        public bool? IsMsg { get; set; }

        [Display(Name = "付款码")]
        public string BarCode { get; set; }

        /// <summary>
        /// 支付参数
        /// </summary>
        public string FolioItemActionJsonPara { get; set; }

        /// <summary>
        /// 打开方式(用于区分从哪里打开添加窗口)
        /// </summary>
        public string OpenFlag { get; set; }
    }
}