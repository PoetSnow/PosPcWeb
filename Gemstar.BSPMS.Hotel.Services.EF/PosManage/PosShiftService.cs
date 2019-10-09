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
    /// pos班次服务实现
    /// </summary>
    public class PosShiftService : CRUDService<PosShift>, IPosShiftService
    {
        private DbHotelPmsContext _db;
        public PosShiftService(DbHotelPmsContext db) : base(db, db.PosShifts)
        {
            _db = db;
        }

        protected override PosShift GetTById(string id)
        {
            return new PosShift { Id = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的班次是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">班次代码</param>
        /// <param name="name">班次名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的班次了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosShifts.Any(w => w.Hid == hid && w.Code == code);
        }
        /// <summary>
        /// 判断指定的代码或者名称的班次是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">班次代码</param>
        /// <param name="name">班次名称</param>
        /// <param name="exceptId">要排队的班次id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的班次了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosShifts.Any(w => w.Hid == hid && w.Id != exceptId && w.Code == code);
        }

        /// <summary>
        /// 根据酒店和班次id获取班次
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="id">班次id</param>
        /// <returns></returns>
        public PosShift GetEntity(string hid, string id)
        {
            return _db.PosShifts.Where(w => w.Hid == hid && w.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店、收银点和模块获取班次列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns></returns>
        public List<PosShift> GetPosShiftList(string hid, string posid, string moduleCode)
        {
            return _db.PosShifts.Where(w => w.Hid == hid && w.PosId == posid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).OrderBy(m => m.Seqid).ToList();
        }

        /// <summary>
        /// 根据酒店、收银点和模块获取班次
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <returns></returns>
        public PosShift GetPosShift(string hid, string posid, string moduleCode)
        {
            return _db.PosShifts.Where(w => w.Hid == hid && w.PosId == posid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).OrderBy(m => m.Seqid).FirstOrDefault();
        }

        /// <summary>
        /// 获取酒店所用班次
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public List<PosShift> GetShiftList(string hid)
        {
            return _db.PosShifts.Where(w => w.Hid == hid && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 根据酒店ID与编码判断数据是否存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsExists(string hid, string code)
        {
            return _db.PosShifts.Any(w => w.Hid == hid && w.Code == code);
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
                    var refe = _db.PosShifts.Find(id);

                    if (refe.IStatus != (byte)status)
                    {
                        refe.IStatus = (byte)status;
                        _db.Entry(refe).State = EntityState.Modified;
                    }
                }
                _db.AddDataChangeLogs(OpLogType.Pos班次启用禁用);
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
