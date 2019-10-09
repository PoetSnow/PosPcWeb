namespace Gemstar.BSPMS.Hotel.Services.NotifyManage
{
    /// <summary>
    /// 通知服务
    /// 用于通知通知处理程序，程序中的某些数据变化，需要上传到ota上
    /// </summary>
    public interface INotifyService
    {
        /// <summary>
        /// 通知ota信息
        /// 在适用于ota的房型，价格码等信息变更时通知
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="isEnvTest">是否测试环境</param>
        /// <param name="channelId">渠道id</param>
        void NotifyOtaInfo(string hid,bool isEnvTest, string channelId);
        /// <summary>
        /// 通知ota价格
        /// 在适用于ota的价格码明细价格变更时通知
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="isEnvTest">是否测试环境</param>
        /// <param name="channelId">渠道id</param>
        /// <param name="rateId">价格id</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        void NotifyOtaRatePrice(string hid, bool isEnvTest, string channelId, string rateId, string beginDate, string endDate);
        /// <summary>
        /// 通知ota房间配额
        /// 在适用于ota的房型配额变更时通知
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="isEnvTest">是否测试环境</param>
        /// <param name="channelId">渠道id</param>
        /// <param name="roomTypeId">房型id</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        void NotifyOtaRoomQty(string hid, bool isEnvTest, string channelId, string roomTypeId, string beginDate, string endDate);
        /// <summary>
        /// 通知ota订单状态变更
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="isEnvTest">是否测试环境</param>
        /// <param name="channelId">渠道id</param>
        /// <param name="resId">订单id</param>
        void NotifyOtaResChanged(string hid, bool isEnvTest, string channelId, string resId);
    }
}
