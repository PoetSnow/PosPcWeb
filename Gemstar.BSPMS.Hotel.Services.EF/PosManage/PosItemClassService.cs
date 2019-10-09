using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos消费项目大类服务实现
    /// </summary>
    public class PosItemClassService : CRUDService<PosItemClass>, IPosItemClassService
    {
        private DbHotelPmsContext _db;

        public PosItemClassService(DbHotelPmsContext db) : base(db, db.PosItemClasss)
        {
            _db = db;
        }

        protected override PosItemClass GetTById(string id)
        {
            return new PosItemClass { Id = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的消费项目大类是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">消费项目大类代码</param>
        /// <param name="name">消费项目大类名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的消费项目大类了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosItemClasss.Any(w => w.Hid == hid && (w.Code == code || w.Cname == name) && w.Module == "CY");
        }

        /// <summary>
        /// 判断指定的代码或者名称的消费项目大类是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">消费项目大类代码</param>
        /// <param name="name">消费项目大类名称</param>
        /// <param name="exceptId">要排队的消费项目大类id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的消费项目大类了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosItemClasss.Any(w => w.Hid == hid && w.Id != exceptId && (w.Code == code || w.Cname == name) && w.Module == "CY");
        }

        /// <summary>
        /// 获取指定酒店下的消费项目大类列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店下的消费项目大类列表</returns>
        public List<PosItemClass> GetPosItemClass(string hid)
        {
            return _db.PosItemClasss.Where(w => w.Hid == hid && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null) && w.Module == "CY").ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块下的消费项目大类列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的消费项目大类列表</returns>
        public List<PosItemClass> GetPosItemClassByModule(string hid, string moduleCode)
        {
            return _db.PosItemClasss.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块下的消费项目大类列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店和模块下的消费项目大类列表</returns>
        public List<v_pos_ItemClassUnionItemResult> GetPosItemClassAndSubClass(string hid)
        {
            return _db.Database.SqlQuery<v_pos_ItemClassUnionItemResult>(string.Format("SELECT * FROM v_pos_ItemClassUnionItem WHERE hid = {0} And Module='CY'", hid)).ToList();
        }

        /// <summary>
        /// 获取指定营业点下的项目大类总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="posId">收银点ID</param>
        /// <param name="customerTypeid">客人类型ID</param>
        /// <param name="TabTypeId">餐台类型ID</param>
        /// <returns></returns>
        public int GetPosItemClassTotal(string hid, string refeid, string posId, string customerTypeid, string TabTypeId)
        {
            //if (string.IsNullOrWhiteSpace(refeid))
            //{
            //    return _db.PosItemClasss.Count(c => c.Hid == hid && (refeid == null || refeid == "" || c.Refeid.Contains(refeid)) && (c.IStatus == (byte)EntityStatus.启用 || c.IStatus == null));
            //}
            //else
            //{
            //    return _db.PosItemClasss.Count(c => c.Hid == hid && (c.Refeid == null || c.Refeid == "" || c.Refeid.Contains(refeid)) && (c.IStatus == (byte)EntityStatus.启用 || c.IStatus == null));
            //}
            var results = _db.Database.SqlQuery<up_pos_list_ItemClassBySingleResult>("exec up_pos_list_ItemClassBySingle @hid=@hid,@posId=@posId,@refeId=@refeId,@customerTypeid=@customerTypeid,@TabTypeId=@TabTypeId",
              new SqlParameter("@hid", hid),
              new SqlParameter("@posId", posId.TrimEnd()),
              new SqlParameter("@refeid", refeid),
              new SqlParameter("@customerTypeid", customerTypeid),
              new SqlParameter("@TabTypeId", TabTypeId))
              .Count();
            return results;

        }

        /// <summary>
        /// 根据酒店和营业点获取消费项目大类列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="posId">收银点ID</param>
        /// <param name="customerTypeid">客人类型ID</param>
        /// <param name="TabTypeId">餐台类型ID</param>
        /// <returns></returns>
        public List<up_pos_list_ItemClassBySingleResult> GetPosItemClassByRefe(string hid, string refeid, int pageIndex, int pageSize, string posId, string customerTypeid, string TabTypeId)
        {
            //  if (string.IsNullOrWhiteSpace(refeid))
            //  {
            //      return _db.PosItemClasss.Where(w => w.Hid == hid && (w.Refeid.Contains(refeid) || refeid == "" || refeid == null) && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null))
            //      .OrderBy(o => o.Seqid).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            //  }
            //  else
            //  {
            //      return _db.PosItemClasss.Where(w => w.Hid == hid && (w.Refeid.Contains(refeid) || w.Refeid == "" || w.Refeid == null) && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null))
            //.OrderBy(o => o.Seqid).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            //  }
            var results = _db.Database.SqlQuery<up_pos_list_ItemClassBySingleResult>("exec up_pos_list_ItemClassBySingle @hid=@hid,@posId=@posId,@refeId=@refeId,@customerTypeid=@customerTypeid,@TabTypeId=@TabTypeId",
              new SqlParameter("@hid", hid),
              new SqlParameter("@posId", posId),
              new SqlParameter("@refeid", refeid),
              new SqlParameter("@customerTypeid", customerTypeid),
              new SqlParameter("@TabTypeId", TabTypeId))
                 .OrderBy(o => o.Seqid).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return results;
        }

        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="ids">主键ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public override JsonResultData BatchUpdateStatus(string ids, EntityStatus status, OpLogType opType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ids))
                {
                    return JsonResultData.Failure("请指定要修改的记录id，多项之间以逗号分隔");
                }
                var idArray = ids.Split(',');
                foreach (var id in idArray)
                {
                    var update = _db.PosItemClasss.Find(id);

                    if (update.IStatus != (byte)status)
                    {
                        update.IStatus = (byte)status;
                        _db.Entry(update).State = EntityState.Modified;
                    }
                }
                _db.AddDataChangeLogs(opType);
                _db.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }




        /// <summary>
        /// 查询消费项目大类
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="wherefunc"></param>
        /// <returns></returns>
        public List<PosItemClass> GetPosItemClass(string hid, Func<PosItemClass, bool> wherefunc)
        {
            return _db.PosItemClasss.Where(u => u.Hid == hid).Where(wherefunc).ToList();
        }


        public List<up_pos_list_ItemActionByItemidResult> GetPosItemActionListByItemClassId(string hid, string itemClassId)
        {
            var list = _db.Database.SqlQuery<up_pos_list_ItemActionByItemidResult>("exec up_pos_list_ItemActionByItemid @hid=@hid,@itemId=@itemId",
                 new SqlParameter("@hid", hid),
                 new SqlParameter("@itemId", itemClassId)
               ).Where(w => w.iType == (byte)PosItemActionType.大类).ToList();
            return list;
        }
    }
}