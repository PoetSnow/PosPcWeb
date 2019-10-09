using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    public class PosBillChangeService : CRUDService<PosBillChange>, IPosBillChangeService
    {
        private DbHotelPmsContext _db;
        public PosBillChangeService(DbHotelPmsContext db) : base(db, db.PosBillChanges)
        {
            _db = db;
        }

        protected override PosBillChange GetTById(string id)
        {
            return new PosBillChange { Id = new Guid(id) };
        }
    }
}
