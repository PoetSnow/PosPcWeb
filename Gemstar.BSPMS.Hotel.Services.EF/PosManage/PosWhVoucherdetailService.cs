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
    public class PosWhVoucherdetailService : CRUDService<WhVoucherdetail>, IPosWhVoucherdetailService
    {
        public PosWhVoucherdetailService(DbHotelPmsContext db) : base(db, db.WhVoucherdetail)
        {
            _pmsContext = db;
        }


        protected override WhVoucherdetail GetTById(string id)
        {
            return new WhVoucherdetail { Voucherid = int.Parse(id) };
        }

        private DbHotelPmsContext _pmsContext;

        public List<WhVoucherdetail> GetList(string hid, string voucherid)
        {
            var sql = "up_VoucherDetailQuery @hid='" + hid + "' ,@voucherid='" + voucherid + "'";
            return _pmsContext.Database.SqlQuery<WhVoucherdetail>(sql).ToList();
        }

        public int Del(string hid, string id)
        {
            var sql = " delete whvoucher WHERE voucherid='" + id + "'and hid='" + hid + "'  delete whVoucherdetail WHERE voucherid='" + id + "' and hid='" + hid + "'";
            return _pmsContext.Database.ExecuteSqlCommand(sql);
        }
    }
}
