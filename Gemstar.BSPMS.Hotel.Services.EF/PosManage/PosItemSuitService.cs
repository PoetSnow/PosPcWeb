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
    public class PosItemSuitService : CRUDService<PosItemSuit>, IPosItemSuitService
    {
        private DbHotelPmsContext _db;

        public PosItemSuitService(DbHotelPmsContext db) : base(db, db.PosItemSuits)
        {
            _db = db;
        }

        protected override PosItemSuit GetTById(string id)
        {
            return new PosItemSuit { Id = new Guid(id) };
        }

        /// <summary>
        /// 判断是否有重复的套餐明细与单位
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">套餐ID</param>
        /// <param name="itemId2">套餐明细ID</param>
        /// <param name="iGrade">套餐明细级别</param>
        /// <param name="unitId">单位ID</param>
        /// <param name="Id">主键</param>
        /// <returns></returns>

        public bool IsExists(string hid, string itemId, string itemId2, int? iGrade, string unitId, string Id = "")
        {
            if (string.IsNullOrEmpty(Id))
            {
                return _db.PosItemSuits.Any(w => w.Hid == hid && w.ItemId == itemId && w.ItemId2 == itemId2 && w.IGrade == iGrade && w.Unitid == unitId);
            }
            else
            {
                return _db.PosItemSuits.Any(w => w.Hid == hid && w.ItemId == itemId && w.ItemId2 == itemId2 && w.IGrade == iGrade && w.Unitid == unitId && w.Id != new Guid(Id));
            }
        }

        /// <summary>
        /// 通过酒店ID，套餐ID，计算金额
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">套餐ID</param>
        public void CalculationItemSuitAmount(string hid, string itemId)
        {
            _db.Database.ExecuteSqlCommand("exec up_pos_CalculationItemSuitAmount @hid=@hid,@itemId=@itemId"
               , new SqlParameter("@hid", hid)
               , new SqlParameter("@itemId", itemId)
               );
        }

        /// <summary>
        /// 根据消费项目获取套餐明细数据
        /// </summary>
        /// <param name="itemId">消费项目ID</param>
        /// <returns></returns>
        public List<up_pos_list_itemSuitByItemIdResult> GetPosItemSuitListByItemId(string hid, string itemId)
        {
            var list = _db.Database.SqlQuery<up_pos_list_itemSuitByItemIdResult>("exec  up_pos_list_itemSuitByItemId @hid=@hid,@itemId=@itemId",
                 new SqlParameter("@hid", hid),
                 new SqlParameter("@itemId", itemId)
               ).ToList();
            return list;
        }

        /// <summary>
        /// 根据消费项目获取套餐明细数据
        /// </summary>
        /// <param name="hid">酒店</param>
        /// <param name="itemId">消费项目</param>
        /// <param name="iGrade">级数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        public List<up_pos_list_itemSuitByItemIdResult> GetPosItemSuitListByItemId(string hid, string itemId, int? iGrade, int pageIndex, int pageSize)
        {
            var list = _db.Database.SqlQuery<up_pos_list_itemSuitByItemIdResult>("exec  up_pos_list_itemSuitByItemId @hid=@hid,@itemId=@itemId",
                 new SqlParameter("@hid", hid),
                 new SqlParameter("@itemId", itemId)
               ).Where(w => w.IGrade == iGrade).OrderBy(o => o.IGrade).ToList();
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}