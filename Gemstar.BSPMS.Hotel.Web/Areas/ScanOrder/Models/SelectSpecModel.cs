using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models
{
    /// <summary>
    /// 选规格Model
    /// </summary>
    public class SelectSpecModel
    {
        /// <summary>
        /// 账单明细
        /// </summary>
        public ScanPosBillDetail BillDetail { get; set; }

        /// <summary>
        /// 消费项目对应价格
        /// </summary>
        public List<up_pos_list_ItemPriceByItemidResult> PosItemPrices { get; set; }

        /// <summary>
        /// 消费项目对应作法
        /// </summary>
        public List<up_pos_list_ItemActionByItemidResult> PosItemActions { get; set; }

        /// <summary>
        /// 要求
        /// </summary>
        public List<PosRequest> PosRequests { get; set; }

        /// <summary>
        /// 账单明细对应作法
        /// </summary>
        public List<ScanPosBillDetailAction> PosBillDetailActions { get; set; }

        /// <summary>
        /// 账单明细作法分组
        /// </summary>
        public List<ActionGroup> ActionGroups { get; set; }

        /// <summary>
        /// 消费大类ID
        /// </summary>
        public string Subclassid { get; set; }

    }
}