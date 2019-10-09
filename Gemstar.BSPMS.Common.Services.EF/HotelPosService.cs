using Gemstar.BSPMS.Common.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Gemstar.BSPMS.Common.Services.EF {
    public class HotelPosService : CRUDService<HotelPos>, IHotelPosService {
        public HotelPosService(DbCommonContext db) : base(db, db.HotelPos) {
        }

        protected override HotelPos GetTById(string id) {
            return new HotelPos {
                PosId = id
            };
        }
    }
}
