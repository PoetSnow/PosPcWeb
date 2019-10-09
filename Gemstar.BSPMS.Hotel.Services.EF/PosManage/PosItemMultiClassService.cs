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
    /// pos项目大类服务实现
    /// </summary>
    public class PosItemMultiClassService : CRUDService<PosItemMultiClass>, IPosItemMultiClassService
    {
        private DbHotelPmsContext _db;
        public PosItemMultiClassService(DbHotelPmsContext db) : base(db, db.PosItemMultiClasss)
        {
            _db = db;
        }
        protected override PosItemMultiClass GetTById(string id)
        {
            return new PosItemMultiClass { Id = new Guid(id) };
        }
        /// <summary>
        /// 判断指定的代码或者名称的消费项目大类是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="itemClassid">消费项目大类id</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的项目大类了，false：没有相同的</returns>
        public bool IsExists(string hid, string itemId, string itemClassid)
        {
            return _db.PosItemMultiClasss.Any(w => w.Hid == hid && w.Itemid == itemId && w.ItemClassid == itemClassid);
        }
        /// <summary>
        /// 判断指定的代码或者名称的项目大类是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="itemClassid">消费项目大类id</param>
        /// <param name="exceptId">要排队的项目大类id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的项目大类了，false：没有相同的</returns>
        public bool IsExists(string hid, string itemId, string itemClassid, Guid exceptId)
        {
            return _db.PosItemMultiClasss.Any(w => w.Hid == hid && w.Id != exceptId && w.Itemid == itemId && w.ItemClassid == itemClassid);
        }
        /// <summary>
        /// 获取指定酒店和模块下的项目大类列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">模块代码</param>
        /// <returns>指定酒店和模块下的项目大类列表</returns>
        public List<PosItemMultiClass> GetPosItemClassByModule(string hid, string itemId)
        {
            return _db.PosItemMultiClasss.Where(w => w.Hid == hid && w.Itemid == itemId).ToList();
        }
        /// <summary>
        /// 获取指定酒店和项目下的单位价格列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目Id</param>
        /// <returns>指定酒店和项目下的项目大类列表</returns>
        public List<up_pos_list_ItemMultiClassByItemidResult> GetPosItemMultiClassByItemId(string hid, string itemId)
        {
            var results = _db.Database.SqlQuery<up_pos_list_ItemMultiClassByItemidResult>("exec up_pos_list_ItemMultiClassByItemid @hid=@hid,@itemID=@itemID", new SqlParameter("@hid", hid), new SqlParameter("@itemID", itemId)).ToList();
            return results;
        }
        /// <summary>
        /// 获取指定酒店和模块下的项目大类列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">模块代码</param>
        /// <returns>指定酒店和模块下的项目大类列表</returns>

        public List<PosItemMultiClass> GetPosItemMultiClassByItemIdForCopy(string hid, string itemId)
        {
            return _db.PosItemMultiClasss.Where(w => w.Hid == hid && w.Itemid == itemId).ToList();
        }


        public PosItemMultiClass GetPosItemMultiClassByItemEditAll(string hid, string itemId, string itemClassId)
        {
            return _db.PosItemMultiClasss.Where(w => w.Hid == hid && w.Itemid == itemId && w.ItemClassid == itemClassId).FirstOrDefault();
        }
    }
}
