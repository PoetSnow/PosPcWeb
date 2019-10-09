using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos作法基础资料服务实现
    /// </summary>
    public class PosActionService : CRUDService<PosAction>, IPosActionService
    {
        private DbHotelPmsContext _db;

        public PosActionService(DbHotelPmsContext db) : base(db, db.PosActions)
        {
            _db = db;
        }

        protected override PosAction GetTById(string id)
        {
            return new PosAction { Id = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的作法基础资料是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">作法基础资料代码</param>
        /// <param name="name">作法基础资料名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的作法基础资料了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosActions.Any(w => w.Hid == hid && (w.Code == code || w.Cname == name));
        }

        /// <summary>
        /// 判断指定的代码或者名称的作法基础资料是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">作法基础资料代码</param>
        /// <param name="name">作法基础资料名称</param>
        /// <param name="exceptId">要排队的作法基础资料id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的作法基础资料了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosActions.Any(w => w.Hid == hid && w.Id != exceptId && (w.Code == code || w.Cname == name));
        }

        /// <summary>
        /// 获取指定酒店和模块下的作法基础资料列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的作法基础资料列表</returns>
        public List<PosAction> GetActionByModule(string hid, string moduleCode)
        {
            return _db.PosActions.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块下的作法列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的作法列表</returns>
        public List<PosAction> GetPosActionByModule(string hid, string moduleCode)
        {
            return _db.PosActions.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块、类型下的作法基础资料列表
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="moduleCode">模板代码</param>
        /// <param name="actionTypeId">作法类型</param>
        /// <returns>指定酒店和模块下的作法基础资料列表</returns>
        public List<PosAction> GetActionByModuleAndType(string hid, string moduleCode, string actionTypeId, int pageIndex, int pageSize)
        {
            var list = _db.PosActions.Where(w => w.Hid == hid && w.Module == moduleCode && w.ActionTypeID == actionTypeId && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).OrderBy(o => o.SeqId).ToList();
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块、类型下的作法基础资料页索引
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="moduleCode">模板代码</param>
        /// <param name="actionTypeId">作法类型Id</param>
        /// <param name="actionId">作法Id</param>
        /// <returns>指定酒店和模块下的作法基础资料列表</returns>
        public int GetActionPageIndex(string hid, string moduleCode, string actionTypeId, string actionId, int pageSize)
        {
            int pageIndex = 0;
            var posActionList = _db.PosActions.Where(w => w.Hid == hid && w.Module == moduleCode && w.ActionTypeID == actionTypeId && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).OrderBy(o => o.SeqId).ToList();
            if (posActionList != null && posActionList.Count > 0)
            {
                var index = posActionList.FindIndex(f => f.Id == actionId);
                pageIndex = (index % pageSize) == 0 ? index / pageSize : index / pageSize + 1;
            }

            return pageIndex;
        }

        /// <summary>
        /// 获取指定酒店和模块下的要求列表总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <param name="actionTypeId">作法类型Id</param>
        /// <returns>指定酒店和模块下的要求列表</returns>
        public int GetActionByModuleTotal(string hid, string moduleCode, string actionTypeId)
        {
            return _db.PosActions.Count(w => w.Hid == hid && w.Module == moduleCode && w.ActionTypeID == actionTypeId && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null));
        }

        /// <summary>
        /// 根据酒店id、代码和名称获取作法
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">代码</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public PosAction GetActionByCode(string hid, string code, string name)
        {
            return _db.PosActions.Where(w => w.Hid == hid && w.Code == code && w.Cname == name).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店ID、作法ID获取作法
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="actionid">作法ID</param>
        /// <returns></returns>
        public PosAction GetActionByID(string hid, string actionid)
        {
            return _db.PosActions.Where(w => w.Hid == hid && w.Id == actionid).FirstOrDefault();
        }

        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="ids">主键ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public JsonResultData BatchUpdateStatus(string ids, EntityStatus status)
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
                    var update = _db.PosActions.Find(id);

                    if (update.IStatus != (byte)status)
                    {
                        update.IStatus = (byte)status;
                        _db.Entry(update).State = EntityState.Modified;
                    }
                }
                _db.AddDataChangeLogs(OpLogType.Pos作法基础资料启用禁用);
                _db.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
    }
}