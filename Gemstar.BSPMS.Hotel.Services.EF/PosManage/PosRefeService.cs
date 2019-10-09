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
    public class PosRefeService : CRUDService<PosRefe>, IPosRefeService
    {
        private DbHotelPmsContext _db;
        public PosRefeService(DbHotelPmsContext db) : base(db, db.PosRefes)
        {
            _db = db;
        }

        protected override PosRefe GetTById(string id)
        {
            return new PosRefe { Id = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的营业点是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">营业点代码</param>
        /// <param name="name">营业点名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的营业点了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosRefes.Any(w => w.Hid == hid && (w.Code == code || w.Cname == name));
        }

        /// <summary>
        /// 判断指定的代码或者名称的营业点是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">营业点代码</param>
        /// <param name="name">营业点名称</param>
        /// <param name="exceptId">要排队的营业点id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的营业点了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosRefes.Any(w => w.Hid == hid && w.Id != exceptId && (w.Code == code || w.Cname == name));
        }

        /// <summary>
        /// 获取指定酒店和模块下的营业厅列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店和模块下的营业厅列表</returns>
        public List<PosRefe> GetRefe(string hid)
        {
            return _db.PosRefes.Where(w => w.Hid == hid).ToList();
        }

        /// <summary>
        /// 获取指定酒店和营业点的营业点信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="id">营业点id</param>
        /// <returns></returns>
        public PosRefe GetEntity(string hid, string id)
        {
            return _db.PosRefes.Where(w => w.Hid == hid && w.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// 指定酒店和收银点下的营业厅列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <returns>指定酒店和收银点下的营业厅列表</returns>
        public List<PosRefe> GetRefeByPosid(string hid, string posid)
        {
            return _db.PosRefes.Where(w => w.Hid == hid && w.PosId == posid && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 指定酒店、收银点和模块下的营业厅列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店、收银点和模块下的营业厅列表</returns>
        public List<PosRefe> GetRefeByPosAndModule(string hid, string posid,string moduleCode)
        {
            return _db.PosRefes.Where(w => w.Hid == hid && w.PosId == posid && w.Module == moduleCode&&((w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null))).ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块下的营业厅列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的营业厅列表</returns>
        public List<PosRefe> GetRefeByModule(string hid, string moduleCode)
        {
            return _db.PosRefes.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 根据指定酒店、收银点、模块获取营业点
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns></returns>
        public List<PosRefe> GetRefeByPos(string hid, string posid, string moduleCode)
        {
            return _db.PosRefes.Where(w => w.Hid == hid && w.PosId == posid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 根据酒店ID与编码判断营业点是否存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsExists(string hid, string code)
        {
            return _db.PosRefes.Any(w => w.Hid == hid && w.Code == code);
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
                    var refe = _db.PosRefes.Find(id);

                    if (refe.IStatus != (byte)status)
                    {
                        refe.IStatus = (byte)status;
                        _db.Entry(refe).State = EntityState.Modified;
                    }
                }
                _db.AddDataChangeLogs(OpLogType.Pos营业点启用禁用);
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
