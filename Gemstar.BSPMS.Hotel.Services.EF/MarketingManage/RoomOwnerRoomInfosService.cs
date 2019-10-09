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
    public class RoomOwnerRoomInfosService : CRUDService<RoomOwnerRoomInfos>, IRoomOwnerRoomInfosService
    {
        private DbHotelPmsContext _db;
        public RoomOwnerRoomInfosService(DbHotelPmsContext db) : base(db, db.RoomOwnerRoomInfoss)
        {
            _db = db;
        }

        public RoomOwnerRoomInfos getOwnernamebyRoomId(string hid, string roomId)
        {
            return _db.RoomOwnerRoomInfoss.Where(c => c.Hid == hid && c.RoomId == roomId).FirstOrDefault();
        }

        public List<RoomOwnerRoomInfos> getOwnerRoomList(string hid, Guid profile)
        {
            return _db.RoomOwnerRoomInfoss.Where(c => c.Hid == hid && c.ProfileId == profile).ToList();
        }

        public List<RoomOwnerRoomInfos> List(string hid)
        {
            return _db.RoomOwnerRoomInfoss.Where(c => c.Hid == hid).ToList();
        }

        protected override RoomOwnerRoomInfos GetTById(string id)
        {
            return _db.RoomOwnerRoomInfoss.Find(Guid.Parse(id));
        }


        public string getRoomIdbyRoomno(string hid, string roomno)
        {

            return _db.RoomOwnerRoomInfoss.Where(c => c.Hid == hid && c.RoomNo == roomno).Select(w => w.RoomId).FirstOrDefault();
        }

        public Guid getprofileidByRoomno(string hid, string roomno)
        {
            return _db.RoomOwnerRoomInfoss.Where(c => c.Hid == hid && c.RoomNo == roomno).Select(w => w.ProfileId).FirstOrDefault();
        }

        public List<RoomOwnerRoomInfos> getOwnerRoomListbycalcTypeId(Guid calcTypeId)
        {
            return _db.RoomOwnerRoomInfoss.Where(c => c.CalcTypeId== calcTypeId).ToList();
        }

    }
}
