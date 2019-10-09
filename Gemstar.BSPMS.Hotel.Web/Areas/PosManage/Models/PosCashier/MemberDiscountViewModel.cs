using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosCashier
{
    public class MemberDiscountViewModel
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int SortID { get; set; } //序号

        /// <summary>
        /// 数据项名称
        /// </summary>
        public string ItemName { get; set; } //名称

        /// <summary>
        ///  数据项的ID
        /// </summary>
        public string ItemID { get; set; }  //ID
       /// <summary>
       /// 数据类型
       /// </summary>
        public string ItemTypeName { get { return IsPosItemClass == true ? "大类" : "消费项目"; } }
        /// <summary>
        /// 是否是消费大类
        /// </summary>
        public bool IsPosItemClass { get; set; } //是否是消费大类， true 大类 ，false 消费项目
        /// <summary>
        /// 是否设置了折扣
        /// </summary>
        public bool IsHasDisCount { get; set; } //是否设置了折扣
        /// <summary>
        /// 消费项目的大类ID
        /// </summary>
        public string ParentID { get; set; }  
        /// <summary>
        /// 是否可以编辑折扣
        /// </summary>
        public bool IsCanEdit { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal? DisCount { get; set; }  //折扣

        public string  Batch { get; set; }  //批次

        public decimal? Price { get; set; } //原价

        public decimal? DueAmount { get { return Price * Count; } } //折前金额

        public decimal? Amount { get; set; } //金额

        public string UnitName { get; set; } //单位名称

        public string UnitID { get; set; } //单位iD

        public decimal? Count { get; set; }//数量



    }
}