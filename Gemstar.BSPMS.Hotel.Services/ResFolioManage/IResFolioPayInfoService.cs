using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;

namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    /// <summary>
    /// 支付记录表服务接口
    /// </summary>
    public interface IResFolioPayInfoService:ICRUDService<ResFolioPayInfo>
    {
        /// <summary>
        /// 获取指定酒店的指定id的待支付记录
        /// </summary>
        /// <param name="id">待支付id</param>
        /// <param name="hid">酒店id</param>
        /// <returns>如果存在，则为存在的对象，不存在则为null</returns>
        ResFolioPayInfo GetHotelPayInfo(int id, string hid);


        /// <summary>
        ///  获取指定酒店的指定条件的待支付记录
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="wherefunc">lambda筛选表达式</param>
        /// <returns></returns>
        ResFolioPayInfo GetHotelPayInfo(string hid, Func<ResFolioPayInfo, bool> wherefunc);

    }
}
