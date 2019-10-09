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
    /// pos作法分类服务实现
    /// </summary>
    public class PosActionTypeService : CRUDService<PosActionType>, IPosActionTypeService
    {
        private DbHotelPmsContext _db;

        public PosActionTypeService(DbHotelPmsContext db) : base(db, db.PosActionTypes)
        {
            _db = db;
        }

        protected override PosActionType GetTById(string id)
        {
            return new PosActionType { Id = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的作法分类是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">作法分类代码</param>
        /// <param name="name">作法分类名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的作法分类了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosActionTypes.Any(w => w.Hid == hid && (w.Code == code || w.Cname == name));
        }

        /// <summary>
        /// 判断指定的代码或者名称的作法分类是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">作法分类代码</param>
        /// <param name="name">作法分类名称</param>
        /// <param name="exceptId">要排队的作法分类id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的作法分类了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosActionTypes.Any(w => w.Hid == hid && w.Id != exceptId && (w.Code == code || w.Cname == name));
        }

        /// <summary>
        /// 获取指定酒店和模块下的作法分类列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的作法分类列表</returns>
        public List<PosActionType> GetActionTypeByModule(string hid, string moduleCode)
        {
            return _db.PosActionTypes.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块下的原因列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的原因列表</returns>
        public List<PosActionType> GetPosActionTypeByModule(string hid, string moduleCode, int pageIndex, int pageSize)
        {
            var list = _db.PosActionTypes.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).OrderBy(o => o.Seqid).ToList();
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块下的原因列表总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的原因列表</returns>
        public int GetPosActionTypeByModuleTotal(string hid, string moduleCode)
        {
            return _db.PosActionTypes.Count(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null));
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
                    var update = _db.PosActionTypes.Find(id);

                    if (update.IStatus != (byte)status)
                    {
                        update.IStatus = (byte)status;
                        _db.Entry(update).State = EntityState.Modified;
                    }
                }
                _db.AddDataChangeLogs(OpLogType.Pos作法分类启用禁用);
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