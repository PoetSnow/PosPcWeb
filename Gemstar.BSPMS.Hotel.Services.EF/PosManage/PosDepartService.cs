using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos出品部门服务实现
    /// </summary>
    public class PosDepartService : CRUDService<PosDepart>, IPosDepartService
    {
        private DbHotelPmsContext _db;
        public PosDepartService(DbHotelPmsContext db) : base(db, db.PosDeparts)
        {
            _db = db;
        }

        protected override PosDepart GetTById(string id)
        {
            return new PosDepart { Id = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的出品部门是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">出品部门代码</param>
        /// <param name="name">出品部门名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的出品部门了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosDeparts.Any(w => w.Hid == hid && (w.Code == code || w.Cname == name));
        }
        /// <summary>
        /// 判断指定的代码或者名称的出品部门是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">出品部门代码</param>
        /// <param name="name">出品部门名称</param>
        /// <param name="exceptId">要排队的出品部门id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的出品部门了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosDeparts.Any(w => w.Hid == hid && w.Id != exceptId && (w.Code == code || w.Cname == name));
        }
        /// <summary>
        /// 获取指定酒店和模块下的出品部门列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的出品部门列表</returns>
        public List<PosDepart> GetDepartByModule(string hid, string moduleCode)
        {
            return _db.PosDeparts.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
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
                    var update = _db.PosDeparts.Find(id);

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

        public List<up_pos_list_DepartResult> GetDepartByProc(string hid)
        {
            return _db.Database.SqlQuery<up_pos_list_DepartResult>(" exec up_pos_list_Depart @h99hid = @h99hid",
                new SqlParameter("@h99hid", hid)).ToList();
        }

    }
}
