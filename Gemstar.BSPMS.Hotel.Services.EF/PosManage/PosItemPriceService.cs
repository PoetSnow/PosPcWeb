using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos单位价格服务实现
    /// </summary>
    public class PosItemPriceService : CRUDService<PosItemPrice>, IPosItemPriceService
    {
        private DbHotelPmsContext _db;

        public PosItemPriceService(DbHotelPmsContext db) : base(db, db.PosItemPrices)
        {
            _db = db;
        }

        protected override PosItemPrice GetTById(string id)
        {
            return new PosItemPrice { Id = new Guid(id) };
        }

        /// <summary>
        /// 判断指定的代码或者名称的单位价格是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="unitid">单位id</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的单位价格了，false：没有相同的</returns>
        public bool IsExists(string hid, string itemId, string unitid)
        {
            return _db.PosItemPrices.Any(w => w.Hid == hid && w.Itemid == itemId && w.Unitid == unitid);
        }

        /// <summary>
        /// 判断指定的代码或者名称的单位价格是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="unitid">单位</param>
        /// <param name="exceptId">要排队的单位价格id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的单位价格了，false：没有相同的</returns>
        public bool IsExists(string hid, string itemId, string unitid, Guid exceptId)
        {
            return _db.PosItemPrices.Any(w => w.Hid == hid && w.Id != exceptId && w.Itemid == itemId && w.Unitid == unitid);
        }

        /// <summary>
        /// 获取指定酒店、项目、单位下的单位价格
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="unitid">单位</param>
        /// <returns></returns>
        public PosItemPrice GetPosItemPriceByUnitid(string hid, string itemid, string unitid)
        {
            return _db.PosItemPrices.Where(w => w.Hid == hid && w.Itemid == itemid && w.Unitid == unitid).FirstOrDefault();
        }

        /// <summary>
        /// 获取指定酒店、项目下的默认单位价格
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <returns></returns>
        public PosItemPrice GetPosItemDefaultPriceByUnitid(string hid, string itemid)
        {
            return _db.PosItemPrices.Where(w => w.Hid == hid && w.Itemid == itemid && w.IsDefault == true).FirstOrDefault();
        }

        /// <summary>
        /// 获取指定酒店和项目下的单位价格列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目Id</param>
        /// <returns>指定酒店和项目下的单位价格列表</returns>
        public List<up_pos_list_ItemPriceByItemidResult> GetPosItemPriceByItemId(string hid, string itemId)
        {
            var results = _db.Database.SqlQuery<up_pos_list_ItemPriceByItemidResult>("exec up_pos_list_ItemPricebyItemid @hid=@hid,@itemID=@itemID", new SqlParameter("@hid", hid), new SqlParameter("@itemID", itemId)).ToList();
            return results;
        }

        /// <summary>
        /// 获取酒店和项目下的单位价格总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目Id</param>
        /// <returns></returns>
        public int GetPosItemPriceTotal(string hid, string itemId)
        {
            var results = _db.Database.SqlQuery<up_pos_list_ItemPriceByItemidResult>("exec up_pos_list_ItemPricebyItemid @hid=@hid,@itemID=@itemID", new SqlParameter("@hid", hid), new SqlParameter("@itemID", itemId)).Count();
            return results;
        }

        /// <summary>
        /// 获取指定酒店和项目下的单位价格列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>酒店和项目下的单位价格列表</returns>
        public List<up_pos_list_ItemPriceByItemidResult> GetPosItemPriceByItemId(string hid, string itemId, int pageIndex, int pageSize)
        {
            var results = _db.Database.SqlQuery<up_pos_list_ItemPriceByItemidResult>("exec up_pos_list_ItemPricebyItemid @hid=@hid,@itemID=@itemID", new SqlParameter("@hid", hid), new SqlParameter("@itemID", itemId)).OrderBy(o => o.Seqid).ToList();
            return results.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(); ;
        }

        public List<PosItemPrice> GetPosItemPriceForCopy(string hid, string itemid)
        {
            return _db.PosItemPrices.Where(w => w.Hid == hid && w.Itemid == itemid).ToList();
        }

        /// <summary>
        /// 获取指定酒店和项目下的单位价格列表
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="itemid"></param>
        /// <param name="unitid"></param>
        /// <param name="func">lambda表达式</param>
        /// <returns></returns>
        public PosItemPrice GetPosItemPriceCountByItemID(string hid, string itemid,string unitid, Func<PosItemPrice, bool> func)
        {
            return _db.Set<PosItemPrice>().Where(u => u.Hid ==hid && u.Itemid == itemid && u.Unitid == unitid).Where(func).FirstOrDefault();
        }

    }
}