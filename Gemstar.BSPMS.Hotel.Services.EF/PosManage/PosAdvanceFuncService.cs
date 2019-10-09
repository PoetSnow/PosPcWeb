using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System.Collections.Generic;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    public class PosAdvanceFuncService : CRUDService<PosAdvanceFunc>, IPosAdvanceFuncService
    {
        private DbHotelPmsContext _db;

        public PosAdvanceFuncService(DbHotelPmsContext db) : base(db, db.PosAdvanceFuncs)
        {
            _db = db;
        }

        protected override PosAdvanceFunc GetTById(string id)
        {
            return new PosAdvanceFunc { Id = id };
        }

        /// <summary>
        /// 根据酒店ID 与编码查询出对应的数据对象
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="FuncCode">编码</param>
        /// <returns></returns>
        public PosAdvanceFunc GetPosAdvanceFuncByFuncCode(string hid, string FuncCode)
        {
            return _db.PosAdvanceFuncs.Where(m => m.Hid == hid && m.FuncCode == FuncCode).FirstOrDefault();
        }

        /// <summary>
        /// 查询高级功能列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="Module">模块</param>
        /// <returns></returns>
        public List<PosAdvanceFunc> GetPosAdvanceFuncList(string hid, string refeId, string Module, int pageIndex, int pageSize)
        {
            var list = _db.PosAdvanceFuncs.Where(m => m.Hid == hid && m.Module == Module && (m.RefeId.Contains(refeId) || refeId == null || refeId == "" || m.RefeId == "" || m.RefeId == null) && m.IsUsed == true).OrderBy(m => m.FuncCode).ToList();
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 统计酒店ID与营业点获取高级功能
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="Module"></param>
        /// <returns></returns>
        public int GetPosAdvanceFuncCount(string hid, string refeId, string Module)
        {
            return _db.PosAdvanceFuncs.Where(m => m.Hid == hid && m.Module == Module && (m.RefeId.Contains(refeId) || refeId == "")).ToList().Count;
        }

        /// <summary>
        /// 验证数据是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="Code">编码</param>
        /// <returns></returns>

        public bool IsExists(string hid, string Code, string Id = "")
        {
            return _db.PosAdvanceFuncs.Any(w => w.Hid == hid && w.FuncCode == Code && w.Id != Id);
        }
    }
}