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
    public class PosWhVoucherService : CRUDService<WhVoucher>, IPosWhVoucherService
    {
        public PosWhVoucherService(DbHotelPmsContext db) : base(db, db.WhVoucher)
        {
            _pmsContext = db;
        }
        protected override WhVoucher GetTById(string id)
        {
            return new WhVoucher { Voucherid = int.Parse(id) };
        }

        private DbHotelPmsContext _pmsContext;
    }
}
