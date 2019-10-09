using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos出品明细服务实现
    /// </summary>
    public class PosProducelistService : CRUDService<PosProducelist>, IPosProducelistService
    {
        private DbHotelPmsContext _db;
        public PosProducelistService(DbHotelPmsContext db) : base(db, db.PosProducelists)
        {
            _db = db;
        }

        protected override PosProducelist GetTById(string id)
        {
            return new PosProducelist { Id = Convert.ToInt64(id)};
        }

        /// <summary>
        /// 根据指定酒店和ID获取出品打印记录
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public PosProducelist GetEntity(string hid, long id)
        {
            return _db.PosProducelists.Where(w => w.Hid == hid && w.Seqno == id).FirstOrDefault();
        }
    }
}
