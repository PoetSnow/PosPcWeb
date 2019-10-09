using System;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Models.ResOrderBatch
{
    /// <summary>
    /// 批量入住，批量预订房型列表查询参数
    /// </summary>
    public class RoomTypeQueryPara
    {
        /// <summary>
        /// 预抵日期
        /// </summary>
        public DateTime? ArrDate { get; set; }
        /// <summary>
        /// 预离日期
        /// </summary>
        public DateTime? DepDate { get; set; }
        /// <summary>
        /// 价格码id
        /// </summary>
        public string RateId { get; set; }
        /// <summary>
        /// 是否入住
        /// </summary>
        public int? IsCheckIn { get; set; }
    }
}