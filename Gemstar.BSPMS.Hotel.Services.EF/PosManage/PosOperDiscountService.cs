using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.PosManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    public class PosOperDiscountService : CRUDService<PosOperDiscount>, IPosOperDiscountService
    {
        private DbHotelPmsContext _db;

        public PosOperDiscountService(DbHotelPmsContext db) : base(db, db.PosOperDiscounts)
        {
            _db = db;
        }

        protected override PosOperDiscount GetTById(string id)
        {
            return new PosOperDiscount { Id = new Guid(id) };
        }

        public List<PmsUser> GetPmsUserList(string grpId)
        {
            return _db.PmsUsers.Where(w => w.Grpid == grpId).ToList();
        }

        public bool IsExists(string hid, string userId, string module)
        {

            return _db.PosOperDiscounts.Any(w => w.Hid == hid && w.UserId == userId && w.Module == module);
        }

        public void cmpOperDiscount(string hid, string billId, string module, string refeId)
        {
            _db.Database.ExecuteSqlCommand("exec up_pos_cmp_OperDiscount @hid = @hid,@billId=@billId,@module=@module,@refeId=@refeId",
                 new SqlParameter("@hid", hid),
                  new SqlParameter("@billId", billId),
                   new SqlParameter("@module", module),
                    new SqlParameter("@refeId", refeId));
        }

        public PosOperDiscount GetOperDiscountByUserId(string hid, string userId, string refeId, string module)
        {
            return _db.PosOperDiscounts.Where(w => w.Hid == hid && w.UserId == userId && w.Module == module && (string.IsNullOrEmpty(w.Refeid) || w.Refeid.Contains(refeId))).FirstOrDefault();

        }
    }
}
