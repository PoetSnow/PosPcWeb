using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.Enums;
using System.Data.SqlClient;
using System;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class RoomService : CRUDService<Room>, IRoomService
    {
        public RoomService(DbHotelPmsContext db) : base(db, db.Rooms)
        {
            _pmsContext = db;
        }
        protected override Room GetTById(string id)
        {
            return new Room { Id = id };
        }
        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 批量更改状态（启用，禁用）
        /// </summary>
        /// <param name="ids">要更改的房间类型id，多项之间以逗号分隔</param>
        /// <param name="status">更新为当前状态</param>
        /// <returns>更改结果</returns>
        public string BatchUpdateStatus(string ids, EntityStatus status)
        {
            string log = "";
            try
            {
                var idArray = ids.Split(',');
                foreach (var id in idArray)
                {
                    Room update = _pmsContext.Rooms.Find(id);
                    if (update.Status != status)
                    {
                        log += update.RoomNo + "、";
                        update.Status = status;
                        _pmsContext.Entry(update).State = EntityState.Modified;
                    }
                }
                _pmsContext.SaveChanges();
                return log;
            }
            catch (System.Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// 批量更新房间中的房型代码
        /// 因为批量增加时，只赋值了房型id，代码没赋值，在批量增加完成后调用此方法来给所有代码为null值的记录赋值
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>更新是否成功</returns>
        public JsonResultData BatchUpdateRoomTypeCode(string hid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hid))
                {
                    return JsonResultData.Failure("请指定酒店id");
                }
                _pmsContext.Database.ExecuteSqlCommand("UPDATE room SET roomTypeCode = t.code FROM room AS r INNER JOIN roomType AS t ON t.id = r.roomTypeid WHERE r.hid = @hid AND r.roomTypeCode IS NULL", new SqlParameter("@hid", hid));
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        /// <summary>
        /// 更新房间表内房间类型字段
        /// </summary>
        /// <param name="oldRoomTypeId">老的 房间类型ID</param>
        /// <param name="newid">新的 房间类型实体</param>
        public void UpdateRoomType(string oldRoomTypeId, RoomType newRoomType)
        {
            _pmsContext.Database.ExecuteSqlCommand("update room set roomTypeid={0},roomTypeCode={1},roomTypeName={2},roomTypeShortName={3} where roomTypeid={4}", newRoomType.Id, newRoomType.Code, newRoomType.Name, newRoomType.ShortName, oldRoomTypeId);
        }

        /// <summary>
        /// 在房间增删改后，调用
        /// </summary>
        /// <param name="hid">酒店ID</param>
        public void UpdateRoomStatusReset(string hid)
        {
            _pmsContext.Database.ExecuteSqlCommand("EXEC [up_RoomStatusReset] @hid={0}", hid);
            _pmsContext.Database.ExecuteSqlCommand("update roomType set totalRooms=(select count(1) from room where room.hid={0} and room.roomTypeid=roomType.id and status<51) where hid={0}", hid);
        }

        /// <summary>
        /// 是否有房间已使用此房间类型
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomTypeId">房间类型ID</param>
        /// <returns></returns>
        public bool IsExistsRoomType(string hid, string roomTypeId)
        {
            return _pmsContext.Rooms.Where(c => c.Hid == hid && c.RoomTypeid == roomTypeId).Any();
        }

        /// <summary>
        /// 此楼层内是否有房间
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="floorid">楼层ID</param>
        /// <returns></returns>
        public bool IsExistsFloor(string hid, string floorid)
        {
            return _pmsContext.Rooms.Where(c => c.Hid == hid && c.Floorid == floorid).Any();
        }

        /// <summary>
        /// 是否此存在此房号
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomNo">房间号</param>
        /// <param name="notRoomId">不包含的房间ID</param>
        /// <returns></returns>
        public bool IsExistsRoomNo(string hid, string roomNo, string notRoomId)
        {
            var sql = _pmsContext.Rooms.Where(c => c.Hid == hid && c.RoomNo == roomNo);
            if (!string.IsNullOrWhiteSpace(notRoomId))
            {
                sql = sql.Where(c => c.Id != notRoomId);
            }
            return sql.Any();
        }

        /// <summary>
        /// 是否存在此房间ID
        /// </summary>
        /// <param name="hid">当前酒店</param>
        /// <param name="roomId">房间ID</param>
        /// <returns>true存在，false不存在或禁用</returns>
        public bool IsExistsRoomId(string hid, string roomId)
        {
            return _pmsContext.Rooms.Where(c => c.Id == roomId && c.Hid == hid && c.Status == EntityStatus.启用).Any();
        }

        /// <summary>
        /// 根据房间ID获取房间类型
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomId">房间ID</param>
        /// <returns>房间类型ID</returns>
        public string GetRoomType(string hid, string roomId)
        {
            return _pmsContext.Rooms.Where(c => c.Id == roomId && c.Hid == hid && c.Status == EntityStatus.启用).Select(c => c.RoomTypeid).FirstOrDefault();
        }
        public bool UpdateRoom(string hid, string remark, string roomid)
        {
            var room = _pmsContext.Rooms.FirstOrDefault(f => f.Id == roomid);
            if (room == null)
                return false;
            room.Description = remark;
            _pmsContext.Entry(room).State = EntityState.Modified;
            AddDataChangeLog(OpLogType.房间修改);
            return _pmsContext.SaveChanges() > 0;
        }
        /// <summary>
        /// 排除假房和已委托房
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public List<Room> roomforisOwner(string hid, string CurRoomid, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                var result = _pmsContext.Database.SqlQuery<Room>("select * from room where (roomno not in (select roomno from RoomOwnerRoomInfos where hid={0}) or id={1}) and hid={0} and roomtypeid not in(select id from roomType where isNotRoom=1) order by roomno ", hid, CurRoomid).ToList();
                return result;
            }
            else
            {
                var result = _pmsContext.Database.SqlQuery<Room>("select * from room where roomno not in (select roomno from RoomOwnerRoomInfos where hid={0})  and hid={0} and roomtypeid not in(select id from roomType where isNotRoom=1) order by roomno", hid).ToList();
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    result = result.Where(w => (w.RoomNo.Contains(keyword))).ToList();
                }
                return result;
            }
        }

        /// <summary>
        /// 获取房间信息
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Room GetEntity(string hid, string id)
        {
            return _pmsContext.Rooms.FirstOrDefault(c => c.Hid == hid && c.Id == id);
        }
    }
}
