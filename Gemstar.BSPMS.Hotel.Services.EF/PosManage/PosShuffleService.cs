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
    public class PosShuffleService : CRUDService<PosShuffle>, IPosShuffleService
    {
        private DbHotelPmsContext _db;
        public PosShuffleService(DbHotelPmsContext db) : base(db, db.PosShuffles)
        {
            _db = db;
        }

        protected override PosShuffle GetTById(string id)
        {
            return new PosShuffle { Id = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的市别是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">市别代码</param>
        /// <param name="name">市别名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的市别了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosShuffles.Any(w => w.Hid == hid && w.Code == code);
        }
        /// <summary>
        /// 判断指定的代码或者名称的市别是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">市别代码</param>
        /// <param name="name">市别名称</param>
        /// <param name="exceptId">要排队的市别id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的市别了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosShuffles.Any(w => w.Hid == hid && w.Id != exceptId && w.Code == code);
        }
        /// <summary>
        /// 根据酒店和市别id获取市别
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="id">市别id</param>
        /// <returns></returns>
        public PosShuffle GetEntity(string hid, string id)
        {
            return _db.PosShuffles.Where(w => w.Hid == hid && w.Id == id).FirstOrDefault();
        }
        /// <summary>
        /// 获取指定酒店下的市别列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店和模块下的市别列表</returns>
        public List<PosShuffle> GetPosShuffle(string hid)
        {
            return _db.PosShuffles.Where(w => w.Hid == hid && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }
        /// <summary>
        /// 获取指定酒店和模块下的市别列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的市别列表</returns>
        public List<PosShuffle> GetPosShuffleByModule(string hid, string moduleCode)
        {
            return _db.PosShuffles.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 根据酒店、营业点和模块获取班次列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns></returns>
        public List<PosShuffle> GetPosShuffleList(string hid, string refeid, string moduleCode)
        {
            return _db.PosShuffles.Where(w => w.Hid == hid && w.Refeid == refeid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 根据指定酒店、营业点获取更换市别信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <returns></returns>
        public up_pos_query_shuffleChangeResult GetShuffleChange(string hid, string refeid)
        {
            return _db.Database.SqlQuery<up_pos_query_shuffleChangeResult>("exec up_pos_query_shuffleChange @hid=@hid,@refeid=@refeid", new SqlParameter("@hid", refeid), new SqlParameter("@refeid", refeid)).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店ID与编码判断数据是否存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsExists(string hid, string code)
        {
            return _db.PosShuffles.Any(w => w.Hid == hid && w.Code == code);
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
                    var refe = _db.PosShuffles.Find(id);

                    if (refe.IStatus != (byte)status)
                    {
                        refe.IStatus = (byte)status;
                        _db.Entry(refe).State = EntityState.Modified;
                    }
                }
                _db.AddDataChangeLogs(OpLogType.Pos市别启用禁用);
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
