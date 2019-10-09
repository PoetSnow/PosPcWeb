using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    public class PosItemClassViewModel
    {
        public PosItemClassViewModel()
        {
            Refeid = "";
            PageIndex = 1;
            PageSize = 1;
            PageTotal = 1;
        }
        
        [Display(Name = "营业点")]
        public string Refeid { get; set; }
        
        [Display(Name = "当前页")]
        public int PageIndex { get; set; }

        [Display(Name = "每页记录数")]
        public int PageSize { get; set; }

        [Display(Name = "总记录数")]
        public int PageTotal { get; set; }
    }
}