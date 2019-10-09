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
    public class PosHolidayService : CRUDService<PosHoliday>, IPosHolidayService
    {
        private DbHotelPmsContext _db;
        public PosHolidayService(DbHotelPmsContext db) : base(db, db.PosHolidays)
        {
            _db = db;
        }

        protected override PosHoliday GetTById(string id)
        {
            return new PosHoliday { Id = new Guid(id) };
        }
        /// <summary>
        /// 验证节假日是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="vDate">日期</param>
        /// <param name="daysName">节日名称</param>
        /// <returns></returns>
        public bool IsExists(string hid, string vDate, string daysName, Guid id)
        {
            return _db.PosHolidays.Any(w => w.Hid == hid && w.Id != id && (w.VDate == vDate || w.DaysName == daysName));
        }
    }
}
