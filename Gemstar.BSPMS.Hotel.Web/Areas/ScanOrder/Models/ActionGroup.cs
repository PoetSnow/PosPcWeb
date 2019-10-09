namespace Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models
{
    /// <summary>
    /// 作法分组
    /// </summary>
    public class ActionGroup
    {
        /// <summary>
        /// 分组ID
        /// </summary>
        public int? Igroupid { get; set; }

        /// <summary>
        /// 分组ID集合
        /// </summary>
        public string ActionIds { get; set; }

        /// <summary>
        /// 分组名称集合
        /// </summary>
        public string ActionNames { get; set; }

        /// <summary>
        /// 消费项目ID 
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// 账单明细行号
        /// </summary>
        public int OrderId { get; set; }
    }
}