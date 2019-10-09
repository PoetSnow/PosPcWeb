using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos账单作法明细服务实现
    /// </summary>
    public class PosBillDetailActionService : CRUDService<PosBillDetailAction>, IPosBillDetailActionService
    {
        private DbHotelPmsContext _db;

        public PosBillDetailActionService(DbHotelPmsContext db) : base(db, db.PosBillDetailActions)
        {
            _db = db;
        }

        protected override PosBillDetailAction GetTById(string id)
        {
            return new PosBillDetailAction { Id = Convert.ToInt64(id) };
        }

        /// <summary>
        /// 判断指定的作法是否存在
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="mBillid">主单号</param>
        /// <param name="mid">账单明细id</param>
        /// <param name="actionNo">作法编码</param>
        /// <returns></returns>
        public bool IsExists(string hid, string mBillid, long? mid, string actionNo)
        {
            return _db.PosBillDetailActions.Any(w => w.Hid == hid && w.MBillid == mBillid && w.Mid == mid && w.ActionNo == actionNo);
        }

        /// <summary>
        /// 获取指定酒店和模块下的账单作法明细列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="mBillid">模块代码</param>
        /// <returns>指定酒店和模块下的账单作法明细列表</returns>
        public List<PosBillDetailAction> GetPosBillDetailActionByModule(string hid, string mBillid)
        {
            return _db.PosBillDetailActions.Where(w => w.Hid == hid && w.MBillid == mBillid).ToList();
        }

        /// <summary>
        /// 根据酒店、主单号、消费明细id获取作法分组id
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="mBillid">主单号</param>
        /// <param name="mid">消费明细id</param>
        /// <returns></returns>
        public int GetIgroupidByMid(string hid, string mBillid, long? mid)
        {
            int? result = _db.PosBillDetailActions.Where(w => w.Hid == hid && w.MBillid == mBillid && w.Mid == mid).Max(m => m.Igroupid);
            return result == null ? 1 : Convert.ToInt32(result) + 1;
        }

        /// <summary>
        /// 根据酒店、主单号、消费明细id获取作法
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="mBillid">主单号</param>
        /// <param name="mid">账单明细id</param>
        /// <param name="actionNo">作法编码</param>
        /// <returns></returns>
        public PosBillDetailAction GetBillDetailActionByMid(string hid, string mBillid, long? mid, string actionNo, string actionType)
        {
            if (actionType == "1")//菜式基本作法 可能其他系统点击作法没有赋值 需要查询出没有赋值的项目
            {
                return _db.PosBillDetailActions.Where(w => w.Hid == hid && w.MBillid == mBillid && w.Mid == mid && w.ActionNo == actionNo && (w.ActionType == actionType || string.IsNullOrEmpty(w.ActionType))).FirstOrDefault();
            }
            else
            {
                return _db.PosBillDetailActions.Where(w => w.Hid == hid && w.MBillid == mBillid && w.Mid == mid && w.ActionNo == actionNo && w.ActionType == actionType).FirstOrDefault();
            }

        }

        /// <summary>
        /// 根据酒店、主单号、消费明细id获取作法列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="mBillid">主单号</param>
        /// <param name="mid">账单明细id</param>
        /// <returns></returns>
        public List<PosBillDetailAction> GetBillDetailActionByMid(string hid, string mBillid, long? mid)
        {
            return _db.PosBillDetailActions.Where(w => w.Hid == hid && w.MBillid == mBillid && w.Mid == mid).OrderBy(m => m.Igroupid).ToList();
        }

        /// <summary>
        /// 根据酒店、主单号、消费明细id获取作法分组列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="mBillid">主单号</param>
        /// <param name="mid">消费明细id</param>
        /// <returns></returns>
        public List<up_pos_list_BillDetailActionGroupResult> GetBillDetailActionGroupByMid(string hid, string mBillid, long? mid)
        {
            return _db.Database.SqlQuery<up_pos_list_BillDetailActionGroupResult>("exec up_pos_list_BillDetailActionGroup @hid=@hid,@mBillid=@mBillid,@mid=@mid", new SqlParameter("@hid", hid), new SqlParameter("@mBillid", mBillid), new SqlParameter("@mid", mid)).ToList();
        }

        public List<PosAction> GetBillDetailAction(string hid, string mBillid, long? mid, int igroupid, string ActionTypeID, int pageIndex, int pageSize)
        {
            var list = _db.Database.SqlQuery<PosAction>("exec up_pos_ActionDetailList @hid=@hid,@mBillid=@mBillid,@mid=@mid,@igroupid=@igroupid,@ActionTypeID=@ActionTypeID",
                new SqlParameter("@hid", hid),
                new SqlParameter("@mBillid", mBillid),
                new SqlParameter("@igroupid", igroupid),
                new SqlParameter("@ActionTypeID", ActionTypeID),
                new SqlParameter("@mid", mid)).ToList();
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}