using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItem
{
    public class PostCostItemEditAll: BaseEditViewModel
    {
        [Display(Name = "id")]
        public Guid Id { get; set; }

        [Display(Name = "酒店代码")]
        public string Hid { get; set; }

        [Display(Name = "类型名称")]
        public string PostSysName { get; set; }

        [Display(Name = "项目")]
        public string Itemid { get; set; }
        [Required(ErrorMessage ="{0}不能为空")]
        [Display(Name = "消费项目单位")]
        public string Unitid { get; set; }

        [Display(Name = "库存物品")]
        public string CostItemid { get; set; }

        [Display(Name = "库存物品单位")]
        public string CostItemUnitid { get; set; }

        [Display(Name = "组成数量")]
        public decimal? OriQuan { get; set; }
        /// <summary>
        /// 实际数量:不能手工输入，必须通过计算方式计算出来：实际数量=组成数量/ 出成率
        /// </summary>
        [Display(Name = "实际数量")]
        public decimal? Quantity { get; set; }

        [Display(Name = "辅助数量")]
        public decimal? Quantity2 { get; set; }
        /// <summary>
        ///  出成率:默认为1
        /// </summary>
        [Display(Name = "出成率")]
        public decimal? XRate { get; set; }
        /// <summary>
        /// 单价:库存物品的单价
        /// </summary>
        [Display(Name = "单价")]
        public decimal? Price { get; set; }
        /// <summary>
        /// 金额:=实际数量 * 单价
        /// </summary>
        [Display(Name = "金额")]
        public decimal? Amount { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? Modifieddate { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}