﻿using Gemstar.BSPMS.Common.Services.EF;
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
    /// pos消费项目对应作法服务实现
    /// </summary>
    public class PosItemActionService : CRUDService<PosItemAction>, IPosItemActionService
    {
        private DbHotelPmsContext _db;

        public PosItemActionService(DbHotelPmsContext db) : base(db, db.PosItemActions)
        {
            _db = db;
        }

        protected override PosItemAction GetTById(string id)
        {
            return new PosItemAction { Id = Guid.Parse(id) };
        }

        /// <summary>
        /// 判断指定的代码或者名称的消费项目对应作法是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemid">消费项目对应作法代码</param>
        /// <param name="actionid">消费项目对应作法名称</param>
        /// <param name="exceptId">要排队的消费项目对应作法id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的消费项目对应作法了，false：没有相同的</returns>
        public bool IsExists(string hid, string itemid, string actionid)
        {
            return _db.PosItemActions.Any(w => w.Hid == hid && w.Itemid == itemid && w.Actionid == actionid);
        }

        /// <summary>
        /// 判断指定的代码或者名称的消费项目对应作法是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemid">消费项目对应作法代码</param>
        /// <param name="actionid">消费项目对应作法名称</param>
        /// <param name="exceptId">要排队的消费项目对应作法id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的消费项目对应作法了，false：没有相同的</returns>
        public bool IsExists(string hid, string itemid, string actionid, Guid exceptId)
        {
            return _db.PosItemActions.Any(w => w.Hid == hid && w.Id != exceptId && w.Itemid == itemid && w.Actionid == actionid);
        }

        /// <summary>
        /// 根据消费项目获取消费项目对应作法
        /// </summary>
        /// <param name="itemId">消费项目ID</param>
        /// <returns></returns>
        public List<up_pos_list_ItemActionByItemidResult> GetPosItemActionListByItemId(string hid, string itemId)
        {
            var list = _db.Database.SqlQuery<up_pos_list_ItemActionByItemidResult>("exec up_pos_list_ItemActionByItemid @hid=@hid,@itemId=@itemId",
                 new SqlParameter("@hid", hid),
                 new SqlParameter("@itemId", itemId)
               ).ToList();
            return list;
        }

        /// <summary>
        /// 根据消费项目获取消费项目对应作法
        /// </summary>
        /// <param name="hid">酒店</param>
        /// <param name="itemId">消费项目</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        public List<up_pos_list_ItemActionByItemidResult> GetPosItemActionListByItemId(string hid, string itemId, int pageIndex, int pageSize)
        {
            var list = _db.Database.SqlQuery<up_pos_list_ItemActionByItemidResult>("exec up_pos_list_ItemActionByItemid @hid=@hid,@itemId=@itemId",
                 new SqlParameter("@hid", hid),
                 new SqlParameter("@itemId", itemId)
               ).ToList();
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(); ;
        }

        /// <summary>
        /// 根据消费项目获取消费项目对应作法总数
        /// </summary>
        /// <param name="hid">酒店</param>
        /// <param name="itemid">消费项目</param>
        /// <returns></returns>
        public int GetPosItemActionTotal(string hid, string itemid)
        {
            return _db.PosItemActions.Count(w => w.Hid == hid && w.Itemid == itemid);
        }

        /// <summary>
        /// 根据作法ID获取消费项目对应作法
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="actionId">作法ID</param>
        /// <returns></returns>
        public PosItemAction GetPosItemActionListByActionId(string hid, string actionId)
        {
            return _db.PosItemActions.Where(w => w.Hid == hid && w.Actionid == actionId).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店和ID获取消费项目对应作法
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="Id">ID</param>
        /// <returns></returns>
        public PosItemAction GetPosItemActionListById(string hid, Guid id)
        {
            return _db.PosItemActions.Where(w => w.Hid == hid && w.Id == id).FirstOrDefault();
        }
    }
}