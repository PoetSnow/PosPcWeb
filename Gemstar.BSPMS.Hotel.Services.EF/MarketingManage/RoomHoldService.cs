using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System.Data.SqlClient;

namespace Gemstar.BSPMS.Hotel.Services.EF.MarketingManage
{
    public class RoomHoldService : CRUDService<RoomHold>, IRoomHoldService
    {
        private DbHotelPmsContext _db;
        public RoomHoldService(DbHotelPmsContext db) : base(db, db.RoomHolds)
        {
            _db = db;
        }

        public SelectList GetChannel(string hid)
        {
            return new SelectList(_db.Channels.Where(w => w.Hid == hid && w.isvalid == true), "Id", "Name");
        }
        public SelectList GetRoomType(string hid)
        {
            return new SelectList(_db.RoomTypes.Where(w => w.Hid == hid), "Id", "Name");
        }
        public List<RoomHold> GetRoomHold(string hid, string channelid, string roomtype, int year, int month)
        {
            return _db.RoomHolds.Where(w => w.Hid == hid && w.Channelid == channelid.Trim() && w.RoomTypeid == roomtype.Trim() && w.HoldDate.Year == year && w.HoldDate.Month == month).ToList();
        }
        protected override RoomHold GetTById(string id)
        {
            var bnId = Guid.Parse(id);
            var RHold = _db.RoomHolds.Find(bnId);
            return RHold;
        }

        public string UpdateRoomHold(string begintime, string endtime, string strarr, string channelid, string roomtype, string hid)
        {
            string[] arr = strarr.Trim(',').Split(',');
            int a = _db.Database.ExecuteSqlCommand("exec up_setRoomHold @hid={0},@channelid={1},@begindate={2},@endDate={3},@roomtypeid={4},@q1={5},@q2={6},@q3={7},@q4={8},@q5={9},@q6={10},@q7={11}", hid, channelid.Trim(), DateTime.Parse(begintime), DateTime.Parse(endtime), roomtype.Trim(), arr[0], arr[1], arr[2], arr[3], arr[4], arr[5], arr[6]);
            return a > 0 ? "成功" : "失败";
        }
        /// <summary>
        /// 查询酒店的渠道可售房设置和可用信息
        /// </summary>
        /// <param name="hid">酒店编号</param>
        /// <param name="channelId">渠道id，为空表示全部</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="days">查询天数</param>
        /// <returns>酒店的渠道可售房设置和可用信息</returns>
        public List<UpQueryRoomHoldInfosResultForshow> QueryRoomHoldInfos(string hid, string channelId, DateTime beginDate, int days)
        {
            var result = new List<UpQueryRoomHoldInfosResultForshow>();
            var roomStatus = _db.Database.SqlQuery<UpQueryRoomHoldInfosResult>("exec up_queryRoomHoldInfos @hid=@hid,@channelId=@channelId,@startDay=@beginDate,@day=@days"
                , new SqlParameter("@hid", hid ?? "")
                , new SqlParameter("@channelId", channelId ?? "")
                , new SqlParameter("@beginDate", beginDate)
                , new SqlParameter("@days", days)
                );
            //将行转换成列再返回
            var typeOfShow = typeof(UpQueryRoomHoldInfosResultForshow);
            foreach (var roomStatu in roomStatus)
            {
                var row = result.SingleOrDefault(w => w.RoomTypeId == roomStatu.RoomTypeid);
                if (row == null)
                {
                    row = new UpQueryRoomHoldInfosResultForshow();
                    row.RoomTypeId = roomStatu.RoomTypeid;
                    row.RoomTypeName = roomStatu.RoomTypeName;
                    row.TotalRooms = roomStatu.TotalRooms;

                    result.Add(row);
                }
                var qtyDate = roomStatu.HoldDate;
                var day = (qtyDate - beginDate).Days;
                var qtyInfo = new UpQueryRoomHoldInfosResultQtyForshow
                {
                    AvailbleRoomQty = roomStatu.AvailbleRoomQty,
                    SettingRoomQty = roomStatu.SettingRoomQty
                };
                var fieldName = string.Format("Day{0}", (day + 1).ToString().PadLeft(2, '0'));
                var property = typeOfShow.GetProperty(fieldName);
                property.SetValue(row, qtyInfo);
            }
            return result;
        }
    }
}
