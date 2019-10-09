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
    /// pos餐台服务实现
    /// </summary>
    public class PosTabService : CRUDService<PosTab>, IPosTabService
    {
        private DbHotelPmsContext _db;
        public PosTabService(DbHotelPmsContext db) : base(db, db.PosTabs)
        {
            _db = db;
        }

        protected override PosTab GetTById(string id)
        {
            return new PosTab { Id = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的餐台是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="tabNo">台号</param>
        /// <param name="name">名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的餐台了，false：没有相同的</returns>
        public bool IsExists(string hid, string tabNo, string name)
        {
            return _db.PosTabs.Any(w => w.Hid == hid && (w.TabNo == tabNo || w.Cname == name));
        }

        /// <summary>
        /// 判断指定的代码或者名称的餐台是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="tabNo">台号</param>
        /// <param name="name">餐台名称</param>
        /// <param name="exceptId">要排队的餐台id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的餐台了，false：没有相同的</returns>
        public bool IsExists(string hid, string tabNo, string name, string exceptId)
        {
            return _db.PosTabs.Any(w => w.Hid == hid && w.Id != exceptId && (w.TabNo == tabNo || w.Cname == name));
        }

        /// <summary>
        /// 根据酒店ID和餐台ID获取餐台信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">餐台ID</param>
        /// <returns></returns>
        public PosTab GetEntity(string hid, string id)
        {
            return _db.PosTabs.Where(w => w.Hid == hid && w.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店ID和模块编码获取餐台信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="moduleCode">模块编码</param>
        /// <returns></returns>
        public List<PosTab> GetPosTabByModule(string hid, string moduleCode)
        {
            return _db.PosTabs.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
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
                    var update = _db.PosTabs.Find(id);

                    if (update.IStatus != (byte)status)
                    {
                        update.IStatus = (byte)status;
                        _db.Entry(update).State = EntityState.Modified;
                    }
                }
                _db.AddDataChangeLogs(OpLogType.Pos餐台资料启用禁用);
                _db.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        public List<up_pos_scan_list_TabListByHidResult> GetPosTabByHid(string hid, string code, string name)
        {
            var list = _db.Database.SqlQuery<up_pos_scan_list_TabListByHidResult>("exec up_pos_scan_list_TabListByHid @h99hid=@h99hid,@t00代码=@t00代码,@t00中文名称=@t00中文名称,@t00英文名称=@t00英文名称",
                     new SqlParameter("@h99hid", hid),
                     new SqlParameter("@t00代码", code),
                     new SqlParameter("@t00中文名称", name),
                     new SqlParameter("@t00英文名称", name)
                   ).ToList();
            return list;
        }

        /// <summary>
        /// 根据酒店id、营业点id获取餐台列表
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="refeid"></param>
        /// <returns></returns>
        public List<PosTab> GetEntityByRefeId(string hid, string refeid)
        {
            return _db.PosTabs.Where(w => w.Hid == hid && w.Refeid == refeid && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }
    }
}