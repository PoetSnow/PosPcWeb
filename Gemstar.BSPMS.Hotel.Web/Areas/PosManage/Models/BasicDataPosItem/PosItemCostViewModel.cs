using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItem
{
    public class PosItemCostViewModel
    {
        [Display(Name ="id")]
        public Guid Id { get; set; }

        
        [Display(Name ="酒店代码")]
        public string Hid { get; set; }

        
        [Display(Name ="类型名称")]
        public string PostSysName { get; set; }


        
        [Display(Name ="项目id")]
        public string Itemid { get; set; }
        

        [Display(Name ="单位id")]
        public string Unitid { get; set; }

        
        [Display(Name ="库存物品id")]
        public string CostItemid { get; set; }
        /// <summary>
        /// 库存物品名称
        /// </summary>
        public string CostItemName { get; set; }
        [Display(Name ="库存物品单位id")]
        public string CostItemUnitid { get; set; }
        /// <summary>
        /// 库存物品单位
        /// </summary>
        public string CostItemUnitName { get; set; }

        [Display(Name ="组成数量")]
        public decimal? OriQuan { get; set; }


        [Display(Name ="实际数量:不能手工输入，必须通过计算方式计算出来：实际数量=组成数量/ 出成率")]
        public decimal? Quantity { get; set; }


        [Display(Name ="辅助数量")]
        public decimal? Quantity2 { get; set; }


        [Display(Name ="出成率:默认为1")]
        public decimal? XRate { get; set; }


        [Display(Name ="单价:库存物品的单价")]
        public decimal? Price { get; set; }


        [Display(Name ="金额:=实际数量 * 单价")]
        public decimal? Amount { get; set; }


        [Display(Name ="修改时间")]
        public DateTime? Modifieddate { get; set; }


        
        [Display(Name ="备注")]
        public string Remark { get; set; }
    }
}