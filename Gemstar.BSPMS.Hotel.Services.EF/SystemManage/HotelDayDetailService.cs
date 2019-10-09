using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class HotelDayDetailService : CRUDService<HotelDayDetail>, IHotelDayDetailService
    {
        public HotelDayDetailService(DbHotelPmsContext db) : base(db, db.HotelDayDetails)
        {
            _pmsContext = db;
        }
        protected override HotelDayDetail GetTById(string id)
        {
            return new HotelDayDetail { Id = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;
    }
}
