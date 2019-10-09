using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Gemstar.BSPMS.Hotel.Services.EF.MarketingManage
{
    public class RoomOwnerCalcTypeService : CRUDService<RoomOwnerCalcType>, IRoomOwnerCalcTypeService
    {
        private DbHotelPmsContext _db;
        public RoomOwnerCalcTypeService(DbHotelPmsContext db) : base(db, db.RoomOwnerCalcTypes)
        {
            _db = db;
        }

        public List<RoomOwnerCalcType> List(string hid)
        {
            return _db.RoomOwnerCalcTypes.Where(c => c.Hid == hid).ToList();
        }

        protected override RoomOwnerCalcType GetTById(string id)
        {
            return _db.RoomOwnerCalcTypes.Find(Guid.Parse(id));
        }         
    }
}
