using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("PlanTask")]
    [LogCName("计划任务")]
    public class PlanTask
    {  
        [Display(Name = "")]
        public Guid id { get; set; }

        [Required(ErrorMessage = "请输入")]
        [Display(Name = "")]
        public string hid { get; set; }

        [Display(Name = "任务类型")]
        public byte? pType { get; set; }

        [Display(Name = "项目")]
        public string itemid { get; set; }

        [Display(Name = "日期")]
        public DateTime cdate { get; set; }

        [Display(Name = "间夜数")]
        public int? night { get; set; }

        [Display(Name = "房价")]
        public decimal? price { get; set; }

        [Display(Name = "出租率")]
        public decimal? roomrate { get; set; }

        [Display(Name = "房租收入")]
        public decimal? rentamount { get; set; }

        [Display(Name = "收入")]
        public decimal? amount { get; set; }

    }
}
