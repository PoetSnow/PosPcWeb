using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.PlanTask
{
    [Table("PlanTask")]
    public class PlanTaskEditViewModel : BaseEditViewModel
    {
        [Key] 
        public Guid Id { get; set; }

        [Column(TypeName = "char")]
        public string Hid { get; set; }

        [Display(Name = "任务类型")] 
        public byte? PType { get; set; }

        [Display(Name = "日期")]
        public DateTime cDate { get; set; }

        [Display(Name = "收入")]
        [Column(TypeName = "decimal")]
        public decimal? Amount { get; set; }


    }
}