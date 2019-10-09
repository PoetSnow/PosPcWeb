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
    public class PosPrintBillService : CRUDService<PosPrintBill>, IPosPrintBillService
    {
        private DbHotelPmsContext _db;

        public PosPrintBillService(DbHotelPmsContext db) : base(db, db.PosPrintBills)
        {
            _db = db;
        }

        protected override PosPrintBill GetTById(string id)
        {
            return new PosPrintBill { Id = new Guid(id) };
        }
    }
}
