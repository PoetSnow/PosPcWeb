using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;
using System.Data;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface IRoomTypeService:ICRUDService<RoomType>
    {
        /// <summary>
        /// 批量更改状态（启用，禁用）
        /// </summary>
        /// <param name="ids">要更改的房间类型id，多项之间以逗号分隔</param>
        /// <param name="status">更新为当前状态</param>
        /// <returns>更改结果</returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status);

        /// <summary>
        /// 获取房间类型键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<KeyValuePair<string, string>> List(string hid);
        /// <summary>
        /// 增加房型图片地址
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="roomtypeid"></param>
        /// <param name="picaddress"></param>
        /// <returns></returns>
        JsonResultData AddRoomtypePic(string hid, string roomtypeid, string picaddress);

        /// <summary>
        /// 获取房型客房用品列表
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="roomtypeid"></param>
        /// <returns></returns>
        DataTable getroomtypeEquipment(string hid,string roomtypeid);
        /// <summary>
        /// 设置房型客房用品数量
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="roomtypeid"></param>
        /// <returns></returns>
        void setroomtypeEquipment(List<RtEqList> para, string rtid);
    }
}
