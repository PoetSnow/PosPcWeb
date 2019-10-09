using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface IHotelDayService : ICRUDService<HotelDay>
    {
        /// <summary>
        /// 获取指定酒店的酒店日历
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<HotelDay> List(string hid);
    }
}
