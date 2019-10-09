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
    public class RoomOwnerFeeService : CRUDService<RoomOwnerFee>, IRoomOwnerFeeService
    {

        private DbHotelPmsContext _db;
        public RoomOwnerFeeService(DbHotelPmsContext db) : base(db, db.RoomOwnerFees)
        {
            _db = db;
        }

        public bool delCurdayImport(string hid, string itemid)
        {
            DateTime today = DateTime.Parse(DateTime.Now.ToShortDateString());
            try
            {
                List<RoomOwnerFee> a = null;
                if (itemid == "")
                {
                    a = _db.RoomOwnerFees.Where(w => w.hid == hid && w.inputDate >= today && w.isImport == true).ToList();
                }
                else
                {
                    a = _db.RoomOwnerFees.Where(w => w.hid == hid && w.inputDate >= today && w.isImport == true && w.itemId == itemid).ToList();
                }
                _db.RoomOwnerFees.RemoveRange(a);
                _db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public decimal? getlastpreReadQty(string hid, string roomno, string itemid)
        {
            var roomOwner = _db.RoomOwnerFees.Where(w => w.hid == hid && w.roomNo == roomno && w.itemId == itemid).OrderByDescending(w => w.inputDate).FirstOrDefault();
            if (roomOwner != null)
            {
                return roomOwner.currentReadQty;
            }
            else
            {
                return 0;
            }
        }

        protected override RoomOwnerFee GetTById(string id)
        {
            return _db.RoomOwnerFees.Find(Guid.Parse(id));
        }
    }
}
