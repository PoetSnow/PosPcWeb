using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Hotel.Services.SystemManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.SystemManage
{
    public class HotelStatusService: IHotelStatusService
    {
        public HotelStatusService(DbHotelPmsContext pmsContext)
        {
            _pmsContext = pmsContext;
        }
        /// <summary>
        /// 获取指定酒店的当前营业日
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的当前营业日，以yyyy-MM-dd格式的日期字符串</returns>
        public DateTime GetBusinessDate(string hid)
        {
            var hotelStatus = _pmsContext.HotelStatuses.FirstOrDefault(w => w.Hid == hid);
            if(hotelStatus == null)
            {
                return DateTime.Today;
            }
            return hotelStatus.BsnsDate;
        }
        private DbHotelPmsContext _pmsContext;
    }
}
