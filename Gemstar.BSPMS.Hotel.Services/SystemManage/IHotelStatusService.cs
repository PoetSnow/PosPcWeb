using System;

namespace Gemstar.BSPMS.Hotel.Services.SystemManage
{
    public interface IHotelStatusService
    {
        /// <summary>
        /// 获取指定酒店的当前营业日
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的当前营业日，以yyyy-MM-dd格式的日期字符串</returns>
        DateTime GetBusinessDate(string hid);
    }
}
