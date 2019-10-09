using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System.Data.SqlClient;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class GuestService : CRUDService<Guest>, IGuestService
    {
        private DbHotelPmsContext _pmsContext;
        public GuestService(DbHotelPmsContext db) : base(db, db.Guests)
        {
            _pmsContext = db;
        }

        public List<Guest> GetGuest(string hid)
        {
            return _pmsContext.Guests.Where(w => w.Hid == hid).ToList();
        }

        protected override Guest GetTById(string id)
        {
            var item = new Guest { Id = Guid.Parse(id) };
            return item;
        }

        public List<Guest> GetGuest(string hid, string guestName)
        {
            return _pmsContext.Guests.Where(w => w.Hid == hid && w.GuestName.Contains(guestName)).ToList();
        }
        /// <summary>
        /// 根据证件号获取熟客
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="cerType">证件类型</param>
        /// <param name="cerId">证件号</param>
        /// <returns></returns>
        public Guest GetGuestByCerId(string hid, string cerType, string cerId)
        {
            return _pmsContext.Guests.Where(w => w.Hid == hid && w.CerType == cerType && w.Cerid == cerId).FirstOrDefault();
        }
        /// <summary>
        /// 获取客历消费记录
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="guestid"></param>
        /// <returns></returns>
        public List<UpQueryGuestTrans> GetGuestTrans(string hid, string guestid)
        {
           return  _pmsContext.Database.SqlQuery<UpQueryGuestTrans>(@"exec up_list_GuestTrans @h99hid=@h99hid,@t00客户id=@t00客户id",
                 new SqlParameter("@h99hid",hid),
                 new SqlParameter("@t00客户id", guestid)
                ).ToList();
        }

    }
}
