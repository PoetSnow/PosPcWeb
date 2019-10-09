using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.RoomStatusManage
{
   public interface IWakeCallService : ICRUDService<WakeCall>
    {
        /// <summary>
        /// 根据房间号获取提醒设置信息
        /// </summary>
        /// <param name="roomNo"></param>
        /// <returns></returns>
        WakeCall GetWakeCall(string roomNo, string hid);
        /// <summary>
        /// 新增或者修改提醒
        /// </summary>
        /// <param name="roomid"></param>
        /// <param name="hid"></param>
        /// <param name="notifyId"></param>
        /// <param name="notifyTime"></param>
        /// <param name="notifyContent"></param>
        /// <param name="notifyRemarks"></param>
        /// <returns></returns>
        bool AddNotify(WakeCall wk, bool isAdd = true);
        /// <summary>
        /// 新增或修改提醒明细表
        /// </summary>
        /// <param name="wkdetil"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        bool AddNotifyDetil(WakeCallDetil wkdetil, bool isAdd = true);
        /// <summary>
        /// 查询提醒
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomId">房间号</param>
        /// <param name="userId"></param>
        /// <param name="notifyDateBegin"></param>
        /// <param name="notifyDateEnd"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        List<UpQueryNotifyResult> GetNotify(string hid, string roomId, string userId, string notifyDateBegin, string notifyDateEnd, int status);
        /// <summary>
        /// 批量已读
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        JsonResultData BatchRead(string ids,string userId,int type,string content);

        /// <summary>
        /// 获取房间号
        /// </summary>
        /// <param name="RoomId"></param>
        /// <param name="hid"></param>
        /// <returns></returns>
        string GetRoomNo(string RoomId, string hid);
        /// <summary>
        /// 获取提醒详情
        /// </summary>
        /// <param name="status">1已读，2已处理</param>
        /// <param name="notifyId"></param>
        /// <returns></returns>
        WakeCallDetil GetDetil(int status, Guid notifyId);

        /// <summary>
        /// 获取配置的叫醒提醒提前分钟数
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        double NotifyTimeBef(string hid);
    }
}
