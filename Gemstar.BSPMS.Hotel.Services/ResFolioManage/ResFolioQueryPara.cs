using Gemstar.BSPMS.Hotel.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    /// <summary>
    /// 客账中的账务明细查询参数类
    /// </summary>
    public class ResFolioQueryPara
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 以逗号分隔的订单明细id
        /// </summary>
        public string RegIds { get; set; }
        /// <summary>
        /// 账务明细状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 账务发生开始日期
        /// </summary>
        public string TransDateBegin { get; set; }
        /// <summary>
        /// 账务发生结束日期
        /// </summary>
        public string TransDateEnd { get; set; }
        /// <summary>
        /// 以逗号分隔的项目类型id
        /// </summary>
        public string ItemTypeIds { get; set; }

        /// <summary>
        /// 房号
        /// </summary>
        public string RoomNo { get; set; }

        /// <summary>
        /// 账单设置代码 分账单使用（A账单、B账单、C账单）
        /// </summary>
        public char ResBillCode { get; set; }
    }
}
