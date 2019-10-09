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
    /// pos同组作法服务实现
    /// </summary>
    public class PosActionMultisubService : CRUDService<PosActionMultisub>, IPosActionMultisubService
    {
        private DbHotelPmsContext _db;

        public PosActionMultisubService(DbHotelPmsContext db) : base(db, db.PosActionMultisubs)
        {
            _db = db;
        }

        protected override PosActionMultisub GetTById(string id)
        {
            return new PosActionMultisub { Id = new Guid(id) };
        }

        /// <summary>
        /// 判断指定的代码或者名称的同组作法是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="actionid">作法id</param>
        /// <param name="actionid2">同组作法id</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的同组作法了，false：没有相同的</returns>
        public bool IsExists(string hid, string actionid, string actionid2)
        {
            return _db.PosActionMultisubs.Any(w => w.Hid == hid && w.Actionid == actionid && w.Actionid2 == actionid2);
        }

        /// <summary>
        /// 判断指定的代码或者名称的同组作法是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="actionid">作法id</param>
        /// <param name="actionid2">同组作法id</param>
        /// <param name="exceptId">要排队的同组作法id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的同组作法了，false：没有相同的</returns>
        public bool IsExists(string hid, string actionid, string actionid2, Guid exceptId)
        {
            return _db.PosActionMultisubs.Any(w => w.Hid == hid && w.Id != exceptId && w.Actionid == actionid && w.Actionid2 == actionid2);
        }

        /// <summary>
        /// 获取指定酒店和模块下的同组作法列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="actionid">模块代码</param>
        /// <returns>指定酒店和模块下的同组作法列表</returns>
        public List<PosActionMultisub> GetPosItemClassByModule(string hid, string actionid)
        {
            return _db.PosActionMultisubs.Where(w => w.Hid == hid && w.Actionid == actionid).ToList();
        }

        /// <summary>
        /// 获取指定酒店和作法下的单位价格列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="actionid">作法Id</param>
        /// <returns>指定酒店和作法下的同组作法列表</returns>
        public List<up_pos_list_ActionMultisubByActionidResult> GetPosActionMultisubByactionid(string hid, string actionid)
        {
            var results = _db.Database.SqlQuery<up_pos_list_ActionMultisubByActionidResult>("exec up_pos_list_ActionMultisubByActionid @hid=@hid,@actionid=@actionid", new SqlParameter("@hid", hid), new SqlParameter("@actionid", actionid)).ToList();
            return results;
        }

        /// <summary>
        /// 根据消费项目获取消费项目对应作法
        /// </summary>
        /// <param name="hid">酒店</param>
        /// <param name="actionid">作法Id</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        public List<up_pos_list_ActionMultisubByActionidResult> GetPosActionMultisubByactionid(string hid, string actionid, int pageIndex, int pageSize)
        {
            var list = _db.Database.SqlQuery<up_pos_list_ActionMultisubByActionidResult>("exec up_pos_list_ActionMultisubByActionid @hid=@hid,@actionid=@actionid",
                 new SqlParameter("@hid", hid),
                 new SqlParameter("@actionid", actionid)
               ).ToList();
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块下的同组作法列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="actionid">模块代码</param>
        /// <returns>指定酒店和模块下的同组作法列表</returns>

        public List<PosActionMultisub> GetPosActionMultisubByactionidForCopy(string hid, string actionid)
        {
            return _db.PosActionMultisubs.Where(w => w.Hid == hid && w.Actionid == actionid).ToList();
        }
    }
}