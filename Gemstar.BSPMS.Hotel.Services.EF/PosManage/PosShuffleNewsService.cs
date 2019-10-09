using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    public class PosShuffleNewsService : CRUDService<PosShuffleNews>, IPosShuffleNewsService
    {
         private DbHotelPmsContext _db;
        public PosShuffleNewsService(DbHotelPmsContext db) : base(db, db.PosShuffleNews)
        {
            _db = db;
        }
        protected override PosShuffleNews GetTById(string id)
        {
            return new PosShuffleNews { Id = id };
        }
        /// <summary>
        /// 判断指定的代码或者名称的公用市别是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">公用市别代码</param>
        /// <param name="name">公用市别名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的公用市别了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosShuffleNews.Any(w => w.Hid == hid && w.Code == code);
        }
        /// <summary>
        /// 判断指定的代码或者名称的市别是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">公用市别代码</param>
        /// <param name="name">公用市别名称</param>
        /// <param name="exceptId">要排队的公用市别id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的公用市别了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosShuffleNews.Any(w => w.Hid == hid && w.Id != exceptId && w.Code == code);
        }

        /// <summary>
        /// 根据酒店、模块获取公用市别列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns></returns>
        public List<PosShuffleNews> GetPosShuffleNewsList(string hid, string moduleCode)
        {
            return _db.PosShuffleNews.Where(w => w.Hid == hid && w.Module == moduleCode && (w.IStatus == (byte)EntityStatus.启用 || w.IStatus == null)).ToList();
        }
    }
}
