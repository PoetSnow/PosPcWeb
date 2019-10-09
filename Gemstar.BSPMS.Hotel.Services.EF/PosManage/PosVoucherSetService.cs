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
    public class PosVoucherSetService : CRUDService<VoucherSet>, IPosVoucherSetService
    {
        public PosVoucherSetService(DbHotelPmsContext db) : base(db, db.VoucherSet)
        {
            _pmsContext = db;
        }

        protected override VoucherSet GetTById(string id)
        {
            return new VoucherSet { Id = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;
    }
}
