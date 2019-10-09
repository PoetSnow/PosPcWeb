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
    /// pos单位定义服务实现
    /// </summary>
    public class PosUnitService : CRUDService<PosUnit>, IPosUnitService
    {
        private DbHotelPmsContext _db;
        public PosUnitService(DbHotelPmsContext db) : base(db, db.PosUnits)
        {
            _db = db;
        }

        protected override PosUnit GetTById(string id)
        {
            return new PosUnit { Id = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的单位定义是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">单位定义代码</param>
        /// <param name="name">单位定义名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的单位定义了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosUnits.Any(w => w.Hid == hid && (w.Code == code || w.Cname == name));
        }
        /// <summary>
        /// 判断指定的代码或者名称的单位定义是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">单位定义代码</param>
        /// <param name="name">单位定义名称</param>
        /// <param name="exceptId">要排队的单位定义id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的单位定义了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosUnits.Any(w => w.Hid == hid && w.Id != exceptId && (w.Code == code || w.Cname == name));
        }
        /// <summary>
        /// 获取指定酒店下的单位定义列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店下的单位定义列表</returns>
        public List<PosUnit> GetUnit(string hid)
        {
            return _db.PosUnits.Where(w => w.Hid == hid).ToList();
        }
        /// <summary>
        /// 根据酒店和单位ID获取单位
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="unitid"></param>
        /// <returns></returns>
        public PosUnit GetEntity(string hid,string unitid)
        {
            return _db.PosUnits.Where(w => w.Hid == hid && w.Id == unitid).FirstOrDefault();
        }
        /// <summary>
        /// 获取指定酒店和模块下的单位定义列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的单位定义列表</returns>
        public List<PosUnit> GetUnitByModule(string hid, string moduleCode)
        {
            return _db.PosUnits.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
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
                    var update = _db.PosUnits.Find(id);

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
    }
}
