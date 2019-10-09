using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class up_pos_cmp_billDetailStatisticsResult
    {
        [Display(Name = "折前金额")]
        public decimal? dueamount{get;set;}

        [Display(Name = "折后金额")]
        public decimal? amount{get;set;}

        [Display(Name = "折扣金额")]
        public decimal? discount{get;set;}

        [Display(Name = "折扣率")]
        public decimal? discountRate{get;set;}

        [Display(Name = "毛利率")]
        public decimal? grossrate{get;set;}

        [Display(Name = "服务费")]
        public decimal? service{get;set;}

        [Display(Name = "服务费率")]
        public decimal? serviceRate { get; set; }
        
        [Display(Name = "最低消费")]
        public decimal? limit{get;set;}

        [Display(Name = "最低消费余额")]
        public decimal? remaining { get; set; }
    }
}
