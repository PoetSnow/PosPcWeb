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
    /// pos财务分类服务实现
    /// </summary>
    public class PosAcTypeService : CRUDService<PosAcType>, IPosAcTypeService
    {
        private DbHotelPmsContext _db;
        public PosAcTypeService(DbHotelPmsContext db) : base(db, db.PosAcTypes)
        {
            _db = db;
        }

        protected override PosAcType GetTById(string id)
        {
            return new PosAcType { Id = id };
        }

        /// 判断指定的代码或者名称的财务分类是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">财务分类代码</param>
        /// <param name="name">财务分类名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的财务分类了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosAcTypes.Any(w => w.Hid == hid && (w.Code == code || w.Cname == name));
        }
        /// <summary>
        /// 判断指定的代码或者名称的财务分类是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">财务分类代码</param>
        /// <param name="name">财务分类名称</param>
        /// <param name="exceptId">要排队的财务分类id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的财务分类了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosAcTypes.Any(w => w.Hid == hid && w.Id != exceptId && (w.Code == code || w.Cname == name));
        }
        /// <summary>
        /// 获取指定酒店和模块下的财务分类列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的财务分类列表</returns>
        public List<PosAcType> GetAcTypeByModule(string hid, string moduleCode)
        {
            return _db.PosAcTypes.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
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
                    var update = _db.PosAcTypes.Find(id);

                    if (update.IStatus != (byte)status)
                    {
                        update.IStatus = (byte)status;
                        _db.Entry(update).State = EntityState.Modified;
                    }
                }
                _db.AddDataChangeLogs(OpLogType.Pos财务分类修改);
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
