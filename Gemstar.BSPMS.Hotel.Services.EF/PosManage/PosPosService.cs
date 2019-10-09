using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    public class PosPosService : CRUDService<PosPos>, IPosPosService
    {
        private DbHotelPmsContext _db;
        public PosPosService(DbHotelPmsContext db) : base(db, db.PosPoses)
        {
            _db = db;
        }

        protected override PosPos GetTById(string id)
        {
            return new PosPos { Id = id };
        }

        /// <summary>
        /// 判断指定Id的收银点是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="id">收银点Id</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的收银点了，false：没有相同的</returns>
        public bool IsExists(string hid, string id)
        {
            return _db.PosPoses.Any(w => w.Hid == hid && w.Id == id);
        }

        /// <summary>
        /// 判断指定的代码或者名称的收银点是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">收银点代码</param>
        /// <param name="name">收银点名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的收银点了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosPoses.Any(w => w.Hid == hid && (w.Code == code || w.Name == name));
        }

        /// <summary>
        /// 判断指定的代码或者名称的收银点是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">收银点代码</param>
        /// <param name="name">收银点名称</param>
        /// <param name="exceptId">要排队的收银点id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的收银点了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosPoses.Any(w => w.Hid == hid && w.Id != exceptId && (w.Code == code || w.Name == name));
        }

        /// <summary>
        /// 获取指定酒店下的收银点列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店和模块下的收银点列表</returns>
        public List<PosPos> GetPosByHid(string hid)
        {
            return _db.PosPoses.Where(w => w.Hid == hid).ToList();
        }

        /// <summary>
        /// 获取指定酒店下的收银点列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店和模块下的收银点列表</returns>
        public List<PosPos> GetPosByHids(string hids)
        {
            return _db.PosPoses.Where(w => hids.Contains(w.Hid) && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 获取指定酒店下的收银点
        /// </summary>
        /// <param name="hid">酒店id</param>
        ///  <param name="id">收银点id</param>
        /// <returns>指定酒店和模块下的收银点</returns>
        public PosPos GetPosByHid(string hid,string id)
        {
            return _db.PosPoses.Where(w => w.Hid == hid && w.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// 获取指定酒店和模块下的收银点列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的收银点列表</returns>
        public List<PosPos> GetPosByModule(string hid, string moduleCode)
        {
            return _db.PosPoses.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }

        /// <summary>
        /// 根据酒店和收银点查询需要更换的班次信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <returns></returns>
        public up_pos_query_shiftChangeResult GetShiftChange(string hid, string posid)
        {
            return _db.Database.SqlQuery<up_pos_query_shiftChangeResult>("exec up_pos_query_shiftChange @hid=@hid,@posid=@posid", new SqlParameter("@hid", hid), new SqlParameter("@posid", posid)).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店和收银点获取清机的信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <returns></returns>
        public up_pos_query_cleaningMachineResult GetCleaningMachine(string hid, string posid)
        {
            return _db.Database.SqlQuery<up_pos_query_cleaningMachineResult>("exec up_pos_query_cleaningMachine @hid=@hid,@posid=@posid", new SqlParameter("@hid", hid), new SqlParameter("@posid", posid)).FirstOrDefault();
        }


        /// <summary>
        /// 用存储过程实现清机
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="posId">收银点ID</param>
        /// <param name="errFlag">是否提示错误信息（0,：不执行，1：执行）</param>
        public void CleaningMachineBusiness(string hid, string posId, string errFlag = "1")
        {
            // _db.Database.ExecuteSqlCommand("");
            _db.Database.ExecuteSqlCommand("exec up_pos_CleaningMachine @hid=@hid,@posPosId=@posPosId,@errFlag=@errFlag"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@posPosId", posId)
               , new SqlParameter("@errFlag", errFlag)
                );
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
                    var update = _db.PosPoses.Find(id);

                    if (update.IStatus != (byte)status)
                    {
                        update.IStatus = (byte)status;
                        _db.Entry(update).State = EntityState.Modified;
                    }
                }
                _db.AddDataChangeLogs(OpLogType.Pos收银点启用禁用);
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
