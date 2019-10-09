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
    /// pos部门类别服务实现
    /// </summary>
    public class PosDeptClassService : CRUDService<PosDeptClass>, IPosDeptClassService
    {
        private DbHotelPmsContext _db;
        public PosDeptClassService(DbHotelPmsContext db) : base(db, db.PosDeptClasss)
        {
            _db = db;
        }

        protected override PosDeptClass GetTById(string id)
        {
            return new PosDeptClass { Id = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的部门类别是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">部门类别代码</param>
        /// <param name="name">部门类别名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的部门类别了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosDeptClasss.Any(w => w.Hid == hid && (w.Code == code || w.Cname == name));
        }
        /// <summary>
        /// 判断指定的代码或者名称的部门类别是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">部门类别代码</param>
        /// <param name="name">部门类别名称</param>
        /// <param name="exceptId">要排队的部门类别id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的部门类别了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosDeptClasss.Any(w => w.Hid == hid && w.Id != exceptId && (w.Code == code || w.Cname == name));
        }
        /// <summary>
        /// 获取指定酒店和模块下的部门类别列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的部门类别列表</returns>
        public List<PosDeptClass> GetDeptClassByModule(string hid, string moduleCode)
        {
            return _db.PosDeptClasss.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
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
                    var update = _db.PosDeptClasss.Find(id);

                    if (update.IStatus != (byte)status)
                    {
                        update.IStatus = (byte)status;
                        _db.Entry(update).State = EntityState.Modified;
                    }
                }
                _db.AddDataChangeLogs(OpLogType.Pos部门类别启用禁用);
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
