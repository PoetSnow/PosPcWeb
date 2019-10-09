using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// Pos扫码点餐Banner
    /// </summary>
    public class PosMScrollService : CRUDService<PosMScroll>, IPosMScrollService
    {
        private DbHotelPmsContext _db;

        public PosMScrollService(DbHotelPmsContext db) : base(db, db.PosMScrolls)
        {
            _db = db;
        }

        protected override PosMScroll GetTById(string id)
        {
            return new PosMScroll { Id = Guid.Parse(id) };
        }

        /// <summary>
        /// 判断指定Id的Banner是否已经存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsExists(string hid, string id)
        {
            return _db.PosAcTypes.Any(w => w.Hid == hid && id.Contains(w.Id));
        }

        /// <summary>
        /// 获取Banner列表
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public List<PosMScroll> GetPosMScrollList(string hid)
        {
            return _db.PosMScrolls.Where(w => w.Hid == hid).ToList();
        }

        /// <summary>
        /// 获取滚动菜式列表
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public List<up_pos_MScrollItemListResult> GetPosMScrollItemList(string hid)
        {
            var results = _db.Database.SqlQuery<up_pos_MScrollItemListResult>("exec up_pos_MScrollItemList @hid=@hid", new SqlParameter("@hid", hid)).ToList();
            return results;
        }
    }
}