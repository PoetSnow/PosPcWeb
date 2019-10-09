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
    public class PosSeaFoodPoolService : CRUDService<PosBillDetail>, IPosSeaFoodPoolService
    {
        private DbHotelPmsContext _db;

        public PosSeaFoodPoolService(DbHotelPmsContext db) : base(db, db.PosBillDetails)
        {
            _db = db;
        }

        protected override PosBillDetail GetTById(string id)
        {
            return new PosBillDetail { Id = Convert.ToInt32(id) };
        }

        /// <summary>
        /// 获取酒店下所有称重的数据
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public List<up_pos_SeafoodPoolListResult> GetSeaFoodPoolList(string hid, string tabId, int pageIndex, int pageSize)
        {
            var list = _db.Database.SqlQuery<up_pos_SeafoodPoolListResult>(" exec up_pos_SeafoodPoolList @hid=@hid,@tabId=@tabId",
                new SqlParameter("@hid", hid),
                new SqlParameter("@tabId", tabId)).ToList();
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }


        /// <summary>
        /// 获取酒店下称重数量
        /// </summary>
        /// <param name="tabId"></param>
        /// <returns></returns>
        public int GetSeaFoodPoolListCount(string hid, string tabId)
        {
            return _db.Database.SqlQuery<up_pos_SeafoodPoolListResult>(" exec up_pos_SeafoodPoolList @hid=@hid,@tabId=@tabId",
               new SqlParameter("@hid", hid),
               new SqlParameter("@tabId", tabId)).Count();

        }

        /// <summary>
        /// 已称重列表
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="querytext"></param>
        /// <returns></returns>
        public List<up_pos_WeighedListResult> GetWeighedList(string hid, string querytext)
        {
            return _db.Database.SqlQuery<up_pos_WeighedListResult>(" exec up_pos_WeighedList @hid=@hid,@querytext=@querytext",
                new SqlParameter("@hid", hid), new SqlParameter("@querytext", querytext)).ToList();
        }
    }
}
