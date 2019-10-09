using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
  public  class GuestTransService : CRUDService<GuestTrans>, IGuestTransService
    {
        public GuestTransService(DbHotelPmsContext db) : base(db, db.GuestTranss)
        {
            _pmContext = db;
        }
        private DbHotelPmsContext _pmContext;
        public List<GuestTrans> GetGuestTrans(string id)
        {
           return  _pmContext.GuestTranss.Where(p => p.Id == Guid.Parse(id)).ToList();
        }

        protected override GuestTrans GetTById(string id)
        {
            return  new GuestTrans { Id = Guid.Parse(id) };
        }
    }
}
