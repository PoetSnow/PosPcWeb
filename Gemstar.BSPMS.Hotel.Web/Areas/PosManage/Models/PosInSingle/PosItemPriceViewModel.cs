using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    public class PosItemPriceViewModel
    {
        public PosItemPriceViewModel()
        {
            PageIndex = 1;
            PageSize = 1;
            PageTotal = 1;
        }
        
        [Display(Name = "项目")]
        public string Itemid { get; set; }
        
        [Display(Name = "当前页")]
        public int PageIndex { get; set; }

        [Display(Name = "每页记录数")]
        public int PageSize { get; set; }

        [Display(Name = "总记录数")]
        public int PageTotal { get; set; }

        /// <summary>
        /// 账单ID
        /// </summary>
        public string BillId { get; set; }

        public string itagperiod { get; set; }
    }
}