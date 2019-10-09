using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosGuestQuery
{
    public class QueryDelayedModel
    {
        public string Hid { get; set; }

        [Display(Name = "营业日")]
        [Required(ErrorMessage = "请选择营业日")]
        public DateTime? BillBsnsDate { get; set; }

        [Display(Name = "收银点")]
        public string PosId { get; set; }

        [Display(Name = "台号")]
        public string tabNo { get; set; }
    }
}