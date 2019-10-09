using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.PermanentRoomManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PermanentRoomManage
{
    /// <summary>
    /// 长租房价
    /// </summary>
    public class PermanentRoomPricePlanService : CRUDService<PermanentRoomPricePlan>, IPermanentRoomPricePlanService
    {
        public PermanentRoomPricePlanService(DbHotelPmsContext db) : base(db, db.PermanentRoomPricePlans)
        {
            _pmsContext = db;
        }
        protected override PermanentRoomPricePlan GetTById(string id)
        {
            return new PermanentRoomPricePlan { Roomid = id };
        }
        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 添加或修改价格
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public JsonResultData AddOrUpdatePrice(string hid, List<PermanentRoomPricePlan> values)
        {
            if(string.IsNullOrWhiteSpace(hid) || values == null || values.Count <= 0) { return JsonResultData.Failure("参数错误"); }

            foreach(var item in values)
            {
                item.Hid = hid;
            }

            var roomids = values.Select(c => c.Roomid).ToList();
            
            //update
            var list = _pmsContext.PermanentRoomPricePlans.Where(c => c.Hid == hid && roomids.Contains(c.Roomid)).ToList();
            foreach(var item in list)
            {
                var entity = values.Where(c => c.Hid == hid && c.Roomid == item.Roomid).FirstOrDefault();
                if(entity != null)
                {
                    bool isUpdate = false;
                    if(item.RoomPriceByDay != entity.RoomPriceByDay)
                    {
                        item.RoomPriceByDay = entity.RoomPriceByDay;
                        isUpdate = true;
                    }
                    if (item.RoomPriceByMonth != entity.RoomPriceByMonth)
                    {
                        item.RoomPriceByMonth = entity.RoomPriceByMonth;
                        isUpdate = true;
                    }
                    if (isUpdate)
                    {
                        _pmsContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }
                }
            }

            //add
            var roomids_exists = list.Select(c => c.Roomid).ToList();
            foreach(var item in values)
            {
                if (!roomids_exists.Contains(item.Roomid))
                {
                    _pmsContext.PermanentRoomPricePlans.Add(item);
                }
            }

            _pmsContext.SaveChanges();
            return JsonResultData.Successed();
        }

    }
}
