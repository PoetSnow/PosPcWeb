using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.MarketingManage
{
    public interface IBookingNotesService: ICRUDService<BookingNotes>
    {
        /// <summary>
        /// 获取指定酒店的酒店日历
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<BookingNotes> GetBookingNotes(string hid);


    }
}
