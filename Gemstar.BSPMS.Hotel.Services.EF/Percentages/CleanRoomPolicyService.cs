using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.Percentages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EF.Percentages
{
    public class CleanRoomPolicyService : CRUDService<PercentagesPolicyCleanRoom>, ICleanRoomPolicyService
    {
        public CleanRoomPolicyService(DbHotelPmsContext db) : base(db, db.PercentagesPolicyCleanRooms)
        {
            _pmsContext = db;
        }
        protected override PercentagesPolicyCleanRoom GetTById(string id)
        {
            return new PercentagesPolicyCleanRoom { PolicyId = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;


        /// <summary>
        /// 添加或修改价格
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public JsonResultData AddOrUpdatePrice(string hid, List<PercentagesPolicyCleanRoom> values)
        {
            if (string.IsNullOrWhiteSpace(hid) || values == null || values.Count <= 0) { return JsonResultData.Failure("参数错误"); }

            foreach (var item in values)
            {
                if(item.PolicyId == null || item.PolicyId == Guid.Empty) { item.PolicyId = Guid.NewGuid(); }
                item.Hid = hid;
            }

            var roomTypeids = values.Select(c => c.RoomTypeId).ToList();

            //update
            var list = _pmsContext.PercentagesPolicyCleanRooms.Where(c => c.Hid == hid && roomTypeids.Contains(c.RoomTypeId)).ToList();
            foreach (var item in list)
            {
                var entity = values.Where(c => c.Hid == hid && c.RoomTypeId == item.RoomTypeId).FirstOrDefault();
                if (entity != null)
                {
                    bool isUpdate = false;
                    if (item.ContinuedToLivePrice != entity.ContinuedToLivePrice)
                    {
                        item.ContinuedToLivePrice = entity.ContinuedToLivePrice;
                        isUpdate = true;
                    }
                    if (item.CheckOutPrice != entity.CheckOutPrice)
                    {
                        item.CheckOutPrice = entity.CheckOutPrice;
                        isUpdate = true;
                    }
                    if (item.PolicyDesciption != entity.PolicyDesciption)
                    {
                        item.PolicyDesciption = entity.PolicyDesciption;
                        isUpdate = true;
                    }
                    if (isUpdate)
                    {
                        _pmsContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }
                }
            }

            //add
            var roomTypeids_exists = list.Select(c => c.RoomTypeId).ToList();
            foreach (var item in values)
            {
                if (!roomTypeids_exists.Contains(item.RoomTypeId))
                {
                    _pmsContext.PercentagesPolicyCleanRooms.Add(item);
                }
            }

            _pmsContext.SaveChanges();
            return JsonResultData.Successed();
        }

    }
}
