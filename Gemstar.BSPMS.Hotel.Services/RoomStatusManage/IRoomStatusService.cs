using System;
using System.Collections.Generic;
using System.Data;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.RoomStatusManage;

namespace Gemstar.BSPMS.Hotel.Services.RoomStatusManage
{
    /// <summary>
    /// 房态服务
    /// </summary>
    public interface IRoomStatusService
    {
        /// <summary>
        /// 获取指定酒店的当前房态信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的当前所有房态信息</returns>
        List<UpQueryRoomStatusResult> GetCurrentRoomStatus(string hid);
        /// <summary>
        /// 获取指定房间的状态信息
        /// </summary>
        /// <param name="roomId">房间id</param>
        /// <returns>指定房间id对应的状态信息，如果指定房间id不存在，则返回一个新的空实例</returns>
        List<UpQueryRoomStatuDetailInfoResult> GetRoomStatu(string roomId);
        /// <summary>
        /// 设置指定房间的脏净标志位
        /// </summary>
        /// <param name="roomId">房间id</param>
        /// <param name="flag">要设置到的脏净标志</param>
        /// <returns>设置结果</returns>
        JsonResultData SetRoomStatusDirty(string roomId, RoomStatusDirtyFlag flag, bool isChangeRoom = false, string waiter = null,string isContinue=null,string remark=null);
        /// <summary>
        /// 获取指定酒店的房间类型
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        List<RoomType> GetRoomType(string hid);
        /// <summary>
        /// 获取指定酒店的当前房态信息，并且按房间类型进行分组统计，用于房态表的内容显示
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的当前房态表内容</returns>
        List<UpQueryRoomStatusInfosByRoomTypeResult> GetCurrentRoomStatusGroupByRoomType(string hid);
        /// <summary>
        /// 获取指定酒店的当前房态信息，按日期进行查看远期房态
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="days">查看天数</param>
        /// <returns>指定酒店，指定时间段内的远期房态</returns>
        List<UpQueryRoomStatusInfoByDateResultForshow> GetCurrentRoomStatusByDate(string hid, DateTime beginDate, int days);
        /// <summary>
        /// 获取子单ID
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomId">房间ID</param>
        /// <returns></returns>
        string GetRegId(string hid, string roomId);
        RoomStatus GetRoomStatus(string hid, string roomid);
        /// <summary>
        /// 获取房间（维修或停用）信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomId">房间ID</param>
        /// <param name="flag">（维修或停用）标志位</param>
        /// <param name="userName">设置人</param>
        /// <returns>返回null，则说明存在错误。</returns>
        RoomStatusServiceAndStopInfo GetRoomStatusServiceOrStop(string hid, string roomId, RoomStatusServiceAndStopFlag flag, string userName);

        /// <summary>
        /// 设置房间（维修或停用）信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="para">表单提交信息</param>
        /// <param name="bsnsDate">营业日</param>
        /// <param name="userName">设置人</param>
        /// <returns></returns>
        JsonResultData SetRoomStatusServiceOrStop(string hid, RoomStatusServiceAndStopPara para, DateTime bsnsDate, string userName);

        /// <summary>
        /// 结束房间（维修或停用）状态
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomId">房间ID</param>
        /// <param name="flag">（维修或停用）标志位</param>
        /// <param name="bsnsDate">营业日</param>
        /// <param name="userName">设置人</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        JsonResultData EndRoomStatusServiceOrStop(string hid, string roomId, RoomStatusServiceAndStopFlag flag, DateTime bsnsDate, string userName, string remark,string serviceUser, RoomStatusServiceAndStopPara para);
        /// <summary>
        ///记录房态
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="roomNo"></param>
        /// <param name="actionType"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="regid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        JsonResultData SetRoomStatusLog(string roomId, string roomNo, string actionType, string oldValue, string newValue, string regid, string remark, string waiter = null, string dirtyType = null);
    }
}
