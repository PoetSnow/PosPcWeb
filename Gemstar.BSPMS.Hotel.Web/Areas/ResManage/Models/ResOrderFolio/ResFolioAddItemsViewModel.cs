using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Models.ResOrderFolio
{
    public class ResFolioAddItemsViewModel
    {
        public string ItemId { get; set; }
        public string Id { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int ItemQty { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        public decimal? ItemPrice { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public decimal ItemSumPrice { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public string InvoNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }

}