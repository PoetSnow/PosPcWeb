using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    public class BillDetailViewModel
    {
        public BillDetailViewModel()
        {
            Id = "";
            Itemid = "";
            Tabid = "";
            Refeid = "";
            Code = "";
            Keyword = "";
            Quantity = 1;
            PageIndex = 1;
            PageSize = 1;
            PageTotal = 1;
        }
        [Display(Name = "项目")]
        public string Id { get; set; }

        [Display(Name = "项目")]
        public string Itemid { get; set; }

        [Display(Name = "餐台")]
        public string Tabid { get; set; }

        [Display(Name = "营业点")]
        public string Refeid { get; set; }

        [Display(Name = "账单")]
        public string Billid { get; set; }

        [Display(Name = "主单号")]
        public string MBillid { get; set; }

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
    }
}