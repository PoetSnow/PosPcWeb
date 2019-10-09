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
    /// pos原因服务实现
    /// </summary>
    public class PosReasonService : CRUDService<PosReason>, IPosReasonService
    {
        private DbHotelPmsContext _db;

        public PosReasonService(DbHotelPmsContext db) : base(db, db.PosReasons)
        {
            _db = db;
        }

        protected override PosReason GetTById(string id)
        {
            return new PosReason { Id = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的原因是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">原因代码</param>
        /// <param name="name">原因名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的原因了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosReasons.Any(w => w.Hid == hid && (w.Code == code || w.Cname == name));
        }

        /// <summary>
        /// 判断指定的代码或者名称的原因是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">原因代码</param>
        /// <param name="name">原因名称</param>
        /// <param name="exceptId">要排队的原因id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的原因了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosReasons.Any(w => w.Hid == hid && w.Id != exceptId && (w.Code == code || w.Cname == name));
        }

        /// <summary>
        /// 获取指定酒店和模块下的原因列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的原因列表</returns>
        public List<PosReason> GetPosReasonByModule(string hid, string moduleCode, byte istagtype, int pageIndex, int pageSize)
        {
            var list = _db.PosReasons.Where(w => w.Hid == hid && w.Module == moduleCode && w.IstagType == istagtype && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).OrderBy(o => o.Seqid).ToList();
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块下的原因列表总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的原因列表</returns>
        public int GetPosReasonByModuleTotal(string hid, string moduleCode, byte istagtype)
        {
            return _db.PosReasons.Count(w => w.Hid == hid && w.Module == moduleCode && w.IstagType == istagtype && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null));
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
                    var update = _db.PosReasons.Find(id);

                    if (update.IStatus != (byte)status)
                    {
                        update.IStatus = (byte)status;
                        _db.Entry(update).State = EntityState.Modified;
                    }
                }
                _db.AddDataChangeLogs(OpLogType.Pos要求启用禁用);
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