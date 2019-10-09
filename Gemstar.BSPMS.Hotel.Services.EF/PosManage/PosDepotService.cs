using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// 二级仓库服务
    /// </summary>
    public class PosDepotService : CRUDService<PosDepot>, IPosDepotService
    {
        private DbHotelPmsContext _db;

        public PosDepotService(DbHotelPmsContext db) : base(db, db.PosDepots)
        {
            _db = db;
        }

        protected override PosDepot GetTById(string id)
        {
            return new PosDepot { Id = id };
        }

        /// <summary>
        ///验证代码名称是否重复
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="code">代码</param>
        /// <param name="cName">中文名称</param>
        /// <returns></returns>
        public bool IsExists(string hid, string code, string cName)
        {
            return _db.PosDepots.Any(w => w.Hid == hid && (w.Code == code || w.Cname == cName));
        }

        /// <summary>
        ///验证代码名称是否重复
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="code">代码</param>
        /// <param name="cName">中文名称</param>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public bool IsExists(string hid, string code, string cName, string id)
        {
            return _db.PosDepots.Any(w => w.Hid == hid && (w.Code == code || w.Cname == cName) && w.Id != id);
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
                    var update = _db.PosDepots.Find(id);

                    if (update.IStatus != (byte)status)
                    {
                        update.IStatus = (byte)status;
                        _db.Entry(update).State = EntityState.Modified;
                    }
                }
                _db.AddDataChangeLogs(OpLogType.Pos二级仓库修改);
                _db.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        /// <summary>
        /// 获取酒店二级仓库
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="module">模块</param>
        /// <returns></returns>
        public List<PosDepot> GetPosDepotList(string hid, string module)
        {
            return _db.PosDepots.Where(w => w.Hid == hid && w.Module == module && w.IStatus == (byte)EntityStatus.启用).ToList();
        }
    }
}
