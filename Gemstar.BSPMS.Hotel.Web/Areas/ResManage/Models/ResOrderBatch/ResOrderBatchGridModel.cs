namespace Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Models.ResOrderBatch
{
    /// <summary>
    /// 批量入住，批量预订房型列表模型
    /// </summary>
    public class ResOrderBatchGridModel
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int? seqid { get; set; }
        /// <summary>
        /// 房间类型ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 房间类型名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 属于此房间类型的房间数量
        /// </summary>
        public int? roomqty { get; set; }
        public string rate { get; set; }
        public int? bbf { get; set; }
        public int selectedQty { get; set; }
    }
}