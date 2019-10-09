using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System;
namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class HotelDayService : CRUDService<HotelDay>, IHotelDayService
    {
        public HotelDayService(DbHotelPmsContext db) : base(db, db.HotelDays)
        {
            _pmsContext = db;
        }
        protected override HotelDay GetTById(string id)
        {
            return new HotelDay { Id = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 获取指定酒店的酒店日历
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public List<HotelDay> List(string hid)
        {
            return _pmsContext.HotelDays.Where(c => c.Hid == hid).ToList();
        }

    }
}
