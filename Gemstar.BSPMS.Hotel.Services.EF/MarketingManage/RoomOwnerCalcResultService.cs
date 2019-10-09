using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.EF.MarketingManage
{
    public class RoomOwnerCalcResultService : CRUDService<RoomOwnerCalcResult>, IRoomOwnerCalcResultService
    {
        private DbHotelPmsContext _db;
        public RoomOwnerCalcResultService(DbHotelPmsContext db) : base(db, db.RoomOwnerCalcResults)
        {
            _db = db;
        }
        protected override RoomOwnerCalcResult GetTById(string id)
        {
            return _db.RoomOwnerCalcResults.Find(id);
        }
        /// <summary>
        /// 重新生成报表
        /// </summary>
        /// <returns></returns>
        public JsonResultData regenerateRoomOwnerCalcResult(string hid, DateTime dt)
        {
            try
            {
                var timeout = _db.Database.CommandTimeout;
                _db.Database.CommandTimeout = 600;
                _db.Database.ExecuteSqlCommand("exec up_regenerateRoomOwnerCalcResult @h99hid={0},@datetime={1}", hid, dt);
                _db.Database.CommandTimeout = timeout;
                return JsonResultData.Successed("生成成功");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        ///  月度分成
        /// </summary>
        /// <returns></returns>
        public DataTable getRoomOwnerMonthCalc(string hid, DateTime dt)
        {
            try
            {
                DataTable datatb = _db.Database.SqlQuery<DataTable>("exec up_list_RoomOwnerMonthCalc @h99hid={0},@年月={1}", hid, dt).FirstOrDefault();
                return datatb;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        /// <summary>
        /// 房租情况
        /// </summary>
        /// <returns></returns>
        public List<RentSituation> getRoomOwnerRentSituat(string hid, DateTime? dt, string profileid)
        {
            try
            {
                List<RentSituation> datatb = _db.Database.SqlQuery<RentSituation>("exec up_list_RoomOwnerRentSituation @h99hid={0},@年月={1},@profileid={2}", hid, dt, profileid).OrderBy(w => w.Roomno).ToList();
                return datatb;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public string getStatusPublishCalcResult(string hid, DateTime dt)
        {
            List<RoomOwnerCalcResult> c = _db.RoomOwnerCalcResults.Where(w => w.hid == hid && w.calcDate == dt).ToList();
            if (c.Count > 0)
            {
                List<RoomOwnerCalcResult> ispub = c.Where(w => w.isPublish == true).ToList();
                if (ispub.Count > 0)
                {
                    return "撤回";
                }
                else
                {
                    return "发布";
                }
            }
            else
            {
                return "未生成";
            }
        }

        public JsonResultData setStatusPublishCalcResult(string hid, DateTime dt, bool ispublish)
        {
            try
            {
                _db.Database.ExecuteSqlCommand("update RoomOwnerCalcResult set isPublish={0} where hid={1} and calcdate={2}", ispublish, hid, dt);
                return JsonResultData.Successed((ispublish ? "发布" : "撤回") + "成功");
            }
            catch
            {
                return JsonResultData.Failure((ispublish ? "发布" : "撤回") + "失败");
            }
        }
    }
}
