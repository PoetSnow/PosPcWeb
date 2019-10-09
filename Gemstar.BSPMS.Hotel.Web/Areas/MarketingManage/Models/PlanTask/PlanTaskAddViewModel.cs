using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.PlanTask
{ 
    public class PlanTaskAddViewModel
    { 
        [Key]
        [Column(TypeName = "varchar")]
        public string Id { get; set; }

        [Column(TypeName = "char")]
        public string Hid { get; set; }

        [Display(Name = "任务类型")]
        [Column(TypeName = "varchar")]
        public byte? PType { get; set; }

        [Display(Name = "日期")] 
        public DateTime cDate { get; set; }

        [Display(Name = "收入")]
        [Column(TypeName = "decimal")]
        public decimal Amount { get; set; }


    }
}