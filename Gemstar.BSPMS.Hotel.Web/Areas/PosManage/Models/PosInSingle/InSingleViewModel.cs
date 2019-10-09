using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    public class InSingleViewModel
    {
        public InSingleViewModel()
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

        [Display(Name = "开台是否刷卡")]
        public byte? IsOpenBrush { get; set; }

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

        /// <summary>
        /// 标识从哪里进入的入单界面，换台之后根据这个值点击退出按钮退到原界面
        /// </summary>
        [Display(Name = "打开方式")]
        public string openFlag { get; set; }

        /// <summary>
        /// 折扣类型{0：全单折扣，1：照单全折；2：全单金额折；3：照单金额折；4：单道折扣；5：单道金额折}
        /// </summary>
        [Display(Name = "折扣类型")]
        public string discType { get; set; }

        [Display(Name = "取消项目是否出品")]
        public bool? isCanItemProd { get; set; }

        [Display(Name = "取消项目是否本地打印取消单")]
        public bool? isCanItemPrint { get; set; }
    }
}