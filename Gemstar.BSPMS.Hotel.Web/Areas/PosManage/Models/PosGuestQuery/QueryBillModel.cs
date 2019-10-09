using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosGuestQuery
{
    public class QueryBillModel
    {
        public string Hid { get; set; }

        [Display(Name = "营业日")]
        [Required(ErrorMessage = "请选择营业日")]
        public DateTime? BillBsnsDate { get; set; }

        [Display(Name = "消费项目")]
        public string ItemName { get; set; }

        [Display(Name = "营业额")]
        public decimal? MinAmount { get; set; }

        [Display(Name = "营业额")]
        public decimal? MaxAmount { get; set; }

        [Display(Name = "付款方式")]
        public string PayMethod { get; set; }

        [Display(Name = "收银点")]
        public string PosId { get; set; }

        [Display(Name = "营业点")]
        public string RefeId { get; set; }

        [Display(Name = "台号")]
        public string tabNo { get; set; }
    }
}