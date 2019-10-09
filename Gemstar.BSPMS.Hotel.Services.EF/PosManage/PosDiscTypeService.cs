using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos折扣类型服务实现
    /// </summary>
    public class PosDiscTypeService : CRUDService<PosDiscType>, IPosDiscTypeService
    {
        private DbHotelPmsContext _db;

        public PosDiscTypeService(DbHotelPmsContext db) : base(db, db.PosDiscTypes)
        {
            _db = db;
        }

        protected override PosDiscType GetTById(string id)
        {
            return new PosDiscType { Id = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的折扣类型是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">折扣类型代码</param>
        /// <param name="name">折扣类型名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的折扣类型了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosDiscTypes.Any(w => w.Hid == hid && (w.Code == code || w.Cname == name));
        }

        /// <summary>
        /// 判断指定的代码或者名称的折扣类型是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">折扣类型代码</param>
        /// <param name="name">折扣类型名称</param>
        /// <param name="exceptId">要排队的折扣类型id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的折扣类型了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosDiscTypes.Any(w => w.Hid == hid && w.Id != exceptId && (w.Code == code || w.Cname == name));
        }

        /// <summary>
        /// 获取指定酒店和模块下的折扣类型列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的折扣类型列表</returns>
        public List<PosDiscType> GetDiscTypeByModule(string hid, string moduleCode)
        {
            return _db.PosDiscTypes.Where(w => w.Hid == hid && w.Module == moduleCode).ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块下的折扣类型列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的折扣类型列表</returns>
        public List<up_pos_discTypeList> GetPosDiscTypeByModule(string hid, string moduleCode, string discType, int pageIndex, int pageSize)
        {
            //return _db.PosDiscTypes.Where(w => w.Hid == hid && w.Module == moduleCode).OrderBy(o => o.Seqid).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var list = _db.Database.SqlQuery<up_pos_discTypeList>("exec up_pos_discTypeList @hid=@hid,@Module=@Module,@discType=@discType", new SqlParameter("@hid", hid), new SqlParameter("@Module", moduleCode), new SqlParameter("@discType", discType)).ToList();
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 获取指定酒店和模块下的折扣类型列表总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的折扣类型列表</returns>
        public int GetPosDiscTypeByModuleTotal(string hid, string moduleCode, string discType)
        {
            var result = _db.Database.SqlQuery<up_pos_discTypeList>("exec up_pos_discTypeList @hid=@hid,@Module=@Module,@discType=@discType", new SqlParameter("@hid", hid), new SqlParameter("@Module", moduleCode), new SqlParameter("@discType", discType)).ToList();

            return result.Count;
        }
    }
}