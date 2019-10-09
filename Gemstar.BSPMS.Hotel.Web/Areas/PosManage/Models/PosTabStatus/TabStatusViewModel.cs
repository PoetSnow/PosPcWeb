using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosTabStatus
{
    public class TabStatusViewModel
    {
        public TabStatusViewModel()
        {
            Code = "";
            Tabid = "";
            Refeid = "";
            Tabtype = "";
            PageIndex = 1;
            PageSize = 1;
            PageTotal = 1;
        }

        [Display(Name = "餐台")]
        public string Tabid { get; set; }

        [Display(Name = "代码")]
        public string Code { get; set; }

        [Display(Name = "营业点")]
        public string Refeid { get; set; }
        
        [Display(Name = "当前营业日")]
        public DateTime? Business { get; set; }

        [Display(Name = "开台是否刷卡")]
        public byte? IsOpenBrush { get; set; }

        [Display(Name = "餐台类型")]
        public string Tabtype { get; set; }

        [Display(Name = "餐台状态")]
        public byte? TabStatus { get; set; }

        [Display(Name = "当前页")]
        public int PageIndex { get; set; }

        [Display(Name = "每页条数")]
        public int PageSize { get; set; }

        [Display(Name = "总条数")]
        public int PageTotal { get; set; }

        [Display(Name = "餐饮类型")]
        public string Mode { get; set; }
    }
}