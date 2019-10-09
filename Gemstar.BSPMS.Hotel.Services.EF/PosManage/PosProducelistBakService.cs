using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos出品明细服务实现
    /// </summary>
    public class PosProducelistBakService : CRUDService<PosProducelistBak>, IPosProducelistBakService
    {
        private DbHotelPmsContext _db;
        public PosProducelistBakService(DbHotelPmsContext db) : base(db, db.PosProducelistBaks)
        {
            _db = db;
        }

        protected override PosProducelistBak GetTById(string id)
        {
            return new PosProducelistBak { Id = Convert.ToInt64(id)};
        }
    }
}
