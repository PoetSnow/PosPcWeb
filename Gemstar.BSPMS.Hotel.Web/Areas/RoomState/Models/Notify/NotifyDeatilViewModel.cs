using System;
using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Hotel.Web.Models;

namespace Gemstar.BSPMS.Hotel.Web.Areas.RoomState.Models.Notify
{
    public class NotifyDeatilViewModel
    {
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Display(Name = "房号：")]
        public string RoomNo { get; set; }

        [Display(Name = "提醒时间：")]
        public string NotifyTime { get; set; }

        [Display(Name = "提醒内容：")]
        public string Content { get; set; }

        [Display(Name = "备注：")]
        public string Remarks { get; set; }

        [Display(Name = "创建人：")]
        public string Cteater { get; set; }

        [Display(Name = "创建时间：")]
        public string CteateTime { get; set; }

        [Display(Name = "状态：")]
        public string Status { get; set; }

        [Display(Name = "接单时间：")]
        public string ReadTime { get; set; }

        [Display(Name = "接单人：")]
        public string Reader{ get; set; }

        [Display(Name = "处理时间：")]
        public string DealTime { get; set; }

        [Display(Name = "处理人：")]
        public string DealMan { get; set; }

        [Display(Name = "处理说明：")]
        public string DealContent { get; set; }

        [Display(Name = "作废时间：")]
        public string InvalidTime { get; set; }

        [Display(Name = "作废人：")]
        public string InvalidMan { get; set; }

        [Display(Name = "作废原因：")]
        public string InvalidReason { get; set; }

        [Display(Name = "外部单号：")]
        public string Refno { get; set; }

        [Display(Name = "提醒类型：")]
        public string WakeCallTypeName { get; set; }
    }
}
