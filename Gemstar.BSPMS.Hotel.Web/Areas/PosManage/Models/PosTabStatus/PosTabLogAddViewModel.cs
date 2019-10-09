using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosTabStatus
{
    public class PosTabLogAddViewModel
    {
        [Display(Name = "营业点id")]
        public string Refeid { get; set; }

        [Display(Name = "台号id")]
        public string Tabid { get; set; }
        
        [Display(Name = "台号")]
        public string TabNo { get; set; }
        
        [Display(Name = "开台单号")]
        public string Billid { get; set; }
        
        [Display(Name = "模块")]
        public string Module { get; set; }
        
        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "用户计算机名称")]
        public string ComputerName { get; set; }

    
    }
}