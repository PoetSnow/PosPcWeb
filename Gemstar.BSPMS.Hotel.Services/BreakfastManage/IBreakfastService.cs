using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Services.BreakfastManage
{
    /// <summary>
    /// 电子早餐服务接口
    /// </summary>
    public interface IBreakfastService
    {
        /// <summary>
        /// 获取今天的早餐记录
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<ResFolioBreakfastInfo> Today(string hid);

        /// <summary>
        /// 刷卡吃早餐
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="cardNoOrRoomNo">门锁卡卡号</param>
        /// <returns></returns>
        JsonResultData ToHaveBreakfastByCardNo(string hid, string cardNo);

        /// <summary>
        /// 刷卡吃早餐
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="cardNoOrRoomNo">房号</param>
        /// <returns></returns>
        JsonResultData ToHaveBreakfastByRoomNo(string hid, string roomNo);
    }
}
