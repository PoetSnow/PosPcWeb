using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EF.MarketingManage
{
    public class RoomOwnerCalcDispParaService : CRUDService<RoomOwnerCalcDispPara>, IRoomOwnerCalcDispParaService
    {
        private DbHotelPmsContext _db;
        public RoomOwnerCalcDispParaService(DbHotelPmsContext db) : base(db, db.RoomOwnerCalcDispParas)
        {
            _db = db;
        }

        public List<RoomOwnerCalcDispPara> List(string hid)
        {
            return _db.RoomOwnerCalcDispParas.Where(c => c.Hid == hid).ToList();
        }

        protected override RoomOwnerCalcDispPara GetTById(string id)
        {
            return _db.RoomOwnerCalcDispParas.Find(Guid.Parse(id));
        }

    }
}
