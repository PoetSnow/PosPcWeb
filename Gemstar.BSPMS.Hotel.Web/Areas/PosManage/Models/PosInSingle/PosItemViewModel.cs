using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    public class PosItemViewModel
    {
        public PosItemViewModel()
        {
            Itemid = "";
            Refeid = "";
            Code = "";
            Keyword = "";
            Quantity = 1;
            PageIndex = 1;
            PageSize = 1;
            PageTotal = 1;
        }

        [Display(Name = "项目")]
        public string Itemid { get; set; }

        [Display(Name = "营业点")]
        public string Refeid { get; set; }

        [Display(Name = "编码")]
        public string Code { get; set; }

        [Display(Name = "关键字")]
        public string Keyword { get; set; }

        [Display(Name = "数量")]
        public int Quantity { get; set; }

        [Display(Name = "当前页")]
        public int PageIndex { get; set; }

        [Display(Name = "每页记录数")]
        public int PageSize { get; set; }

        [Display(Name = "总记录数")]
        public int PageTotal { get; set; }

        /// <summary>
        /// 特价菜日期属性
        /// </summary>
        [Display(Name = "特价菜日期属性")]
        public string itagperiod { get; set; }
        

        /// <summary>
        /// 账单ID
        /// </summary>
        public string billId { get; set; }
    }
}