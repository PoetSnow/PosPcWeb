namespace Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models
{
    /// <summary>
    /// 单位
    /// </summary>
    public class UnitModel
    {
        /// <summary>
        /// 消费项目ID
        /// </summary>
        public string Itemid { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>
        public string Unitid { get; set; }

        /// <summary>
        /// 单位代码
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
    }
}