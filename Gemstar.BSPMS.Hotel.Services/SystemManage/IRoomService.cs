using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface IRoomService : ICRUDService<Room>
    {
        /// <summary>
        /// 批量更改状态（启用，禁用）
        /// </summary>
        /// <param name="ids">要更改的房间类型id，多项之间以逗号分隔</param>
        /// <param name="status">更新为当前状态</param>
        /// <returns>更改结果</returns>
        string BatchUpdateStatus(string ids, EntityStatus status);
        /// <summary>
        /// 批量更新房间中的房型代码
        /// 因为批量增加时，只赋值了房型id，代码没赋值，在批量增加完成后调用此方法来给所有代码为null值的记录赋值
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>更新是否成功</returns>
        JsonResultData BatchUpdateRoomTypeCode(string hid);
        /// <summary>
        /// 更新房间表内房间类型字段
        /// </summary>
        /// <param name="oldRoomTypeId">老的 房间类型ID</param>
        /// <param name="newid">新的 房间类型实体</param>
        void UpdateRoomType(string oldRoomTypeId, RoomType newRoomType);

        /// <summary>
        /// 在房间增删改后，调用
        /// </summary>
        /// <param name="hid">酒店ID</param>
        void UpdateRoomStatusReset(string hid);

        /// <summary>
        /// 是否有房间已使用此房间类型
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomTypeId">房间类型ID</param>
        /// <returns></returns>
        bool IsExistsRoomType(string hid, string roomTypeId);

        /// <summary>
        /// 此楼层内是否有房间
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="floorid">楼层ID</param>
        /// <returns></returns>
        bool IsExistsFloor(string hid, string floorid);

        /// <summary>
        /// 是否此存在此房号
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomNo">房间号</param>
        /// <param name="notRoomId">不包含的房间ID</param>
        /// <returns></returns>
        bool IsExistsRoomNo(string hid, string roomNo, string notRoomId);

        /// <summary>
        /// 是否存在此房间ID
        /// </summary>
        /// <param name="hid">当前酒店</param>
        /// <param name="roomId">房间ID</param>
        /// <returns>true存在，false不存在或禁用</returns>
        bool IsExistsRoomId(string hid, string roomId);

        /// <summary>
        /// 根据房间ID获取房间类型
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomId">房间ID</param>
        /// <returns>房间类型ID</returns>
        string GetRoomType(string hid, string roomId);

        /// <summary>
        /// 修改房间备注
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="remark"></param>
        /// <param name="roomid"></param>
        /// <returns></returns>
        bool UpdateRoom(string hid, string remark, string roomid);

        /// <summary>
        /// 得到没有设为业主房的房间
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        List<Room> roomforisOwner(string hid,string CurRoomid,string text);

        /// <summary>
        /// 获取房间信息
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Room GetEntity(string hid, string id);
    }
}
