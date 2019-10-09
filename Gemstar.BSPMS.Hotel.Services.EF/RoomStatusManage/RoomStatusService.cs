using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.RoomStatusManage;
using Gemstar.BSPMS.Common.Extensions;

namespace Gemstar.BSPMS.Hotel.Services.EF.RoomStatusManage
{
    /// <summary>
    /// 房态服务
    /// </summary>
    public class RoomStatusService : IRoomStatusService
    {
        public RoomStatusService(DbHotelPmsContext pmsContext,ICurrentInfo currentInfo)
        {
            _pmsContext = pmsContext;
            _currentInfo = currentInfo;
        }
        /// <summary>
        /// 获取指定酒店的当前房态信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的当前所有房态信息</returns>
        public List<UpQueryRoomStatusResult> GetCurrentRoomStatus(string hid)
        {
            return _pmsContext.Database.SqlQuery<UpQueryRoomStatusResult>("exec up_QueryRoomStatus @hid=@hid", new SqlParameter("@hid", hid)).ToList();
        }
        /// <summary>
        /// 获取指定酒店的房间类型
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public List<RoomType> GetRoomType(string hid)
        {
            return _pmsContext.RoomTypes.Where(w => w.Hid == hid).ToList();
        }
        /// <summary>
        /// 获取指定房间的状态信息
        /// </summary>
        /// <param name="roomId">房间id</param>
        /// <returns>指定房间id对应的状态信息，如果指定房间id不存在，则返回一个新的空实例</returns>
        public List<UpQueryRoomStatuDetailInfoResult> GetRoomStatu(string roomId)
        {
            var statu = _pmsContext.Database.SqlQuery<UpQueryRoomStatuDetailInfoResult>("exec up_queryRoomStatuDetailInfo @roomId=@roomId", new SqlParameter("@roomId", roomId)).ToList();
            if (statu == null)
            {
                statu = new List<UpQueryRoomStatuDetailInfoResult>();
            }
            return statu;
        }
        /// <summary>
        /// 设置指定房间的脏净标志位
        /// </summary>
        /// <param name="roomId">房间id</param>
        /// <param name="flag">要设置到的脏净标志</param>
        /// <returns>设置结果</returns>
        public JsonResultData SetRoomStatusDirty(string roomId, RoomStatusDirtyFlag flag,bool isChangeRoom=false, string waiter = null,string isContinue=null,string remark=null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomId))
                {
                    return JsonResultData.Failure("请指定要设置脏净标志的房间");
                }
                var roomStatu = _pmsContext.RoomStatuses.SingleOrDefault(w => w.Roomid == roomId);
                if (roomStatu == null)
                {
                    return JsonResultData.Failure("指定的房间不存在");
                }
                //记录房态操作
                var isServiceOrStop = roomStatu.IsService == 1 ? "维修房，" : roomStatu.IsStop == 1 ? "停用房，" : "";
                var empty =!isChangeRoom? (string.IsNullOrWhiteSpace(roomStatu.Regid) ? "空房" : "在住房"):"在住房";
                var dirty = roomStatu.IsDirty == 0 ? "净房" : roomStatu.IsDirty == 1 ? "脏房" : "清洁房";
                var newdirty=(byte)flag== 0 ? "净房" : (byte)flag == 1 ? "脏房" : "清洁房";
                var oldValue = empty + "，" + isServiceOrStop + dirty;
                var newValue =(isChangeRoom?"空房": empty) + "，" + isServiceOrStop + newdirty;
                SetRoomStatusLog(roomId, roomId.Replace(_currentInfo.HotelId, ""), isChangeRoom? "changRoom":"isDirty", oldValue,newValue, roomStatu.Regid,remark,waiter,isContinue);
                roomStatu.IsDirty = (byte)flag;
                _pmsContext.Entry(roomStatu).State = System.Data.Entity.EntityState.Modified;
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 获取指定酒店的当前房态信息，并且按房间类型进行分组统计，用于房态表的内容显示
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的当前房态表内容</returns>
        public List<UpQueryRoomStatusInfosByRoomTypeResult> GetCurrentRoomStatusGroupByRoomType(string hid)
        {
            return _pmsContext.Database.SqlQuery<UpQueryRoomStatusInfosByRoomTypeResult>("exec up_QueryRoomStatusInfosByRoomType @hid = @hid", new SqlParameter("@hid", hid)).ToList();
        }
        /// <summary>
        /// 获取指定酒店的当前房态信息，按日期进行查看远期房态
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="days">查看天数</param>
        /// <returns>指定酒店，指定时间段内的远期房态</returns>
        public List<UpQueryRoomStatusInfoByDateResultForshow> GetCurrentRoomStatusByDate(string hid, DateTime beginDate, int days)
        {
            var roomType = GetRoomType(hid);
            var result = new List<UpQueryRoomStatusInfoByDateResultForshow>();
            var roomStatus = _pmsContext.Database.SqlQuery<UpQueryRoomStatusInfoByDateResult>("exec up_queryRoomStatusInfoByDate @hid=@hid,@beginDate=@beginDate,@days=@days", new SqlParameter("@hid", hid), new SqlParameter("@beginDate", beginDate), new SqlParameter("@days", days));
            //将行转换成列再返回
            var typeOfShow = typeof(UpQueryRoomStatusInfoByDateResultForshow);
            foreach (var roomStatu in roomStatus)
            {
                var row = result.SingleOrDefault(w => w.RoomTypeId == roomStatu.RoomTypeId);
                if (row == null)
                {
                    row = new UpQueryRoomStatusInfoByDateResultForshow();
                    row.RoomTypeId = roomStatu.RoomTypeId;
                    row.RoomTypeName = roomStatu.RoomTypeName;
                    row.TotalRooms = roomStatu.TotalRooms;

                    result.Add(row);
                }
                var qtyDate = DateTime.Parse(roomStatu.QtyDate);
                var day = (qtyDate - beginDate).Days;
                var room = roomType.Where(r => r.Id == roomStatu.RoomTypeId).FirstOrDefault();
                var depQ = _pmsContext.Database.SqlQuery<int>("SELECT COUNT(*) FROM dbo.roomStatus WHERE hid = @hid AND roomTypeid = @roomTypeid AND CONVERT(VARCHAR(10), depDate, 120) =@qtyDate", new SqlParameter("@hid", hid), new SqlParameter("@roomTypeid", roomStatu.RoomTypeId), new SqlParameter("@qtyDate", qtyDate)).FirstOrDefault();
                var qtyInfo = new UpQueryRoomStatusInfoByDateResultQtyForshow
                {
                    BookingQty = roomStatu.BookingQty,
                    LivedQty = roomStatu.LivedQty,
                    ServiceQty = roomStatu.ServiceQty,
                    StopQty = roomStatu.StopQty,
                    QuotaAvailableQty = roomStatu.QuotaAvailableQty,
                    OtherQty = roomStatu.OtherQty,
                    OverQauntity = room == null ? 0 : room.OverQauntity ?? 0,
                    AvailableQty = roomStatu.AvailableQty,
                    DepQty = depQ
                };
                var fieldName = string.Format("Day{0}", (day + 1).ToString().PadLeft(2, '0'));
                var property = typeOfShow.GetProperty(fieldName);
                property.SetValue(row, qtyInfo);
            }
            return result;
        }

        /// <summary>
        /// 获取子单ID
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomId">房间ID</param>
        /// <returns></returns>
        public string GetRegId(string hid, string roomId)
        {
            var entity = _pmsContext.RoomStatuses.Where(c => c.Roomid == roomId && c.Hid == hid).FirstOrDefault();
            if (entity != null)
            {
                return entity.Regid;
            }
            return null;
        }
        public RoomStatus GetRoomStatus(string hid, string roomid)
        {
            return _pmsContext.RoomStatuses.Where(c => c.Roomid == roomid && c.Hid == hid).FirstOrDefault();
        }
        #region 房间 维修/停用
        /// <summary>
        /// 获取房间（维修或停用）信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomId">房间ID</param>
        /// <param name="flag">（维修或停用）标志位</param>
        /// <param name="userName">设置人</param>
        /// <returns>返回null，则说明存在错误。</returns>
        public RoomStatusServiceAndStopInfo GetRoomStatusServiceOrStop(string hid, string roomId, RoomStatusServiceAndStopFlag flag, string userName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(roomId) || string.IsNullOrWhiteSpace(userName))
                {
                    return null;
                }
                var roomStatu = _pmsContext.RoomStatuses.SingleOrDefault(w => w.Roomid == roomId && w.Hid == hid);
                if (roomStatu == null)
                {
                    return null;
                }
                byte type = (byte)flag;
                RoomStatusServiceAndStopInfo resultEntity = _pmsContext.RoomServiceLogs.Where(c => c.Hid == hid && c.Roomid == roomId && c.Type == type && c.EndBsnsDate == null && c.EndDate == null && c.EndUser == null).Select(
                    s => new RoomStatusServiceAndStopInfo
                    {
                        Id = s.Id,
                        RoomId = s.Roomid,
                        RoomNo = s.RoomNo,
                        Reason = s.Reasons,
                        Remark = s.Remark,
                        CreateUser = s.CreateUser,
                        EndUser = s.EndUser,
                        StartDateTime = s.BeginDate,
                        PlanEndDateTime = s.OrderEndDate,
                        _Type = s.Type,
                        ServiceUser=s.Worker,
                        IsRoomClean=s.IsRoomClean,
                        CleanWaiter=s.CleanWaiter
                    }
                    ).SingleOrDefault();
                if (resultEntity == null)
                {
                    resultEntity = new RoomStatusServiceAndStopInfo
                    {
                        RoomId = roomStatu.Roomid,
                        RoomNo = roomStatu.RoomNo,
                        _Type = (byte)flag,
                        StartDateTime = DateTime.Now,
                        CreateUser = userName,
                    };
                }
                resultEntity.Type = (RoomStatusServiceAndStopFlag)resultEntity._Type;
                return resultEntity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 设置房间（维修或停用）信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="para">表单提交信息</param>
        /// <param name="bsnsDate">营业日</param>
        /// <param name="userName">设置人</param>
        /// <returns></returns>
        public JsonResultData SetRoomStatusServiceOrStop(string hid, RoomStatusServiceAndStopPara para, DateTime bsnsDate, string userName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hid) || para == null || string.IsNullOrWhiteSpace(para.RoomId) || string.IsNullOrWhiteSpace(userName))
                {
                    return JsonResultData.Failure("参数错误");
                }
                if (string.IsNullOrWhiteSpace(para.Reason))
                {
                    return JsonResultData.Failure(string.Format("请输入{0}原因", EnumExtension.GetDescription(para.Type)));
                }
                var roomStatu = _pmsContext.RoomStatuses.SingleOrDefault(w => w.Roomid == para.RoomId && w.Hid == hid);
                if (roomStatu == null)
                {
                    return JsonResultData.Failure("在房态中找不到此房间信息");
                }
                byte type = (byte)para.Type;
                bool isSave = false;
                var logEntity = _pmsContext.RoomServiceLogs.Where(c => c.Hid == hid && c.Roomid == para.RoomId && c.Type == type && c.EndBsnsDate == null && c.EndDate == null && c.EndUser == null).SingleOrDefault();
               
                if (logEntity == null)
                {
                    //添加
                    #region
                    _pmsContext.RoomServiceLogs.Add(new RoomServiceLog
                    {
                        Hid = hid,
                        Id = Guid.NewGuid(),
                        Type = (byte)para.Type,
                        Roomid = para.RoomId,
                        Reasons = para.Reason,
                        Remark = para.Remark,
                        OrderEndDate = para.PlanEndDateTime,
                        BeginDate = DateTime.Now,
                        RoomNo = roomStatu.RoomNo,
                        BeginBsnsDate = bsnsDate,
                        CreateUser = userName,
                        Worker = para.ServiceUser,
                        IsRoomClean = para.IsRoomClean,
                    });
                    isSave = true;
                    #endregion
                }
                else
                {
                    //修改
                    #region
                    bool isUpdate = false;
                    if (logEntity.OrderEndDate != para.PlanEndDateTime)
                    {
                        logEntity.OrderEndDate = para.PlanEndDateTime;
                        isUpdate = true;
                    }
                    if (logEntity.Reasons != para.Reason)
                    {
                        logEntity.Reasons = para.Reason;
                        isUpdate = true;
                    }
                    if (logEntity.Remark != para.Remark)
                    {
                        logEntity.Remark = para.Remark;
                        isUpdate = true;
                    }
                    if (logEntity.Worker !=para.ServiceUser&& para.Type == RoomStatusServiceAndStopFlag.Service)
                    {
                        logEntity.Worker = para.ServiceUser;
                        isUpdate = true;
                    }
                    if(logEntity.IsRoomClean != para.IsRoomClean)
                    {
                        logEntity.IsRoomClean = para.IsRoomClean;
                        isUpdate = true;
                    }
                    if (para.IsRoomClean && logEntity.CleanWaiter != para.CleanWaiter)
                    {
                        logEntity.CleanWaiter = para.CleanWaiter;
                        isUpdate = true;
                    }
                    else if (!para.IsRoomClean && !string.IsNullOrEmpty(logEntity.CleanWaiter))
                    {
                        logEntity.CleanWaiter = "";
                    }
                    if (isUpdate)
                    {
                        _pmsContext.Entry(logEntity).State = System.Data.Entity.EntityState.Modified;
                        isSave = true;
                    }
                    #endregion
                }
                //修改状态
                #region
                bool isEdit = false;
                if (para.Type == RoomStatusServiceAndStopFlag.Service)
                {
                    if (roomStatu.IsService != 1)
                    {
                        roomStatu.IsService = 1;
                        isEdit = true;
                    }
                    if (roomStatu.ServiceUser != userName)
                    {
                        roomStatu.ServiceUser = userName;
                        isEdit = true;
                    }
                    if (roomStatu.ServiceEndDate != para.PlanEndDateTime)
                    {
                        roomStatu.ServiceEndDate = para.PlanEndDateTime;
                        isEdit = true;
                    }
                }
                else if (para.Type == RoomStatusServiceAndStopFlag.Stop)
                {
                    if (roomStatu.IsStop != 1)
                    {
                        roomStatu.IsStop = 1;
                        isEdit = true;
                    }
                    if (roomStatu.StorpUser != userName)
                    {
                        roomStatu.StorpUser = userName;
                        isEdit = true;
                    }
                    if (roomStatu.StopEndDate != para.PlanEndDateTime)
                    {
                        roomStatu.StopEndDate = para.PlanEndDateTime;
                        isEdit = true;
                    }
                }
                if (isEdit)
                {
                    _pmsContext.Entry(roomStatu).State = System.Data.Entity.EntityState.Modified;
                    isSave = true;
                }
                #endregion
                //保存
                if (isSave)
                {
                    _pmsContext.SaveChanges();
                    _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=2,@ids=@ids", new SqlParameter("@hid", hid), new SqlParameter("@ids", para.RoomId));
                }
                #region 记录房态
                //记录房态
                var dirty = roomStatu.IsDirty == 0 ? "净房" : roomStatu.IsDirty == 1 ? "脏房" : "清洁房";
                var actionType = type == 1 ? "isService" : "isStop";
                var oldValue = "空房，" + dirty;
                var newValue = "空房，" +dirty+"，"+ (type == 1 ? "维修房" : "停用房");
                SetRoomStatusLog(para.RoomId, para.RoomId.Replace(_currentInfo.HotelId, ""), actionType,oldValue,newValue, roomStatu.Regid, para.Remark);
                #endregion
                return JsonResultData.Successed();
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure("设置失败");
            }
        }

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
        public JsonResultData EndRoomStatusServiceOrStop(string hid, string roomId, RoomStatusServiceAndStopFlag flag, DateTime bsnsDate, string userName, string remark,string serviceUser, RoomStatusServiceAndStopPara para)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(roomId) || string.IsNullOrWhiteSpace(userName)) 
                {
                    return JsonResultData.Failure("参数错误");
                }
                bool isSave = false;
                var roomStatu = _pmsContext.RoomStatuses.SingleOrDefault(w => w.Roomid == roomId && w.Hid == hid);
                if (roomStatu == null)
                {
                    return JsonResultData.Failure("在房态中找不到此房间信息");
                }
                byte type = (byte)flag;
                var logEntity = _pmsContext.RoomServiceLogs.Where(c => c.Hid == hid && c.Roomid == roomId && c.Type == type && c.EndBsnsDate == null && c.EndDate == null && c.EndUser == null).SingleOrDefault();
                if (logEntity != null)
                {
                    //修改
                    logEntity.EndBsnsDate = bsnsDate;
                    logEntity.EndDate = DateTime.Now;
                    logEntity.EndUser = userName;
                    logEntity.Remark = remark;
                    logEntity.IsRoomClean = para.IsRoomClean;
                    logEntity.CleanWaiter = para.CleanWaiter;
                    if(flag==RoomStatusServiceAndStopFlag.Service)
                       logEntity.Worker = serviceUser;
                    _pmsContext.Entry(logEntity).State = System.Data.Entity.EntityState.Modified;
                    isSave = true;
                }
                //修改
                #region
                bool isUpdate = false;
                if (flag == RoomStatusServiceAndStopFlag.Service)
                {
                    if (roomStatu.IsService != 0)
                    {
                        roomStatu.IsService = 0;
                        roomStatu.ServiceEndDate = null;
                        roomStatu.ServiceUser = null;
                        isUpdate = true;
                    }
                }
                else if (flag == RoomStatusServiceAndStopFlag.Stop)
                {
                    if (roomStatu.IsStop != 0)
                    {
                        roomStatu.IsStop = 0;
                        roomStatu.StopEndDate = null;
                        roomStatu.StorpUser = null;
                        isUpdate = true;
                    }
                }
                if (isUpdate)
                {
                    _pmsContext.Entry(roomStatu).State = System.Data.Entity.EntityState.Modified;
                    isSave = true;
                }
                #endregion
                //保存
                if (isSave)
                {
                    _pmsContext.SaveChanges();
                    _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=2,@ids=@ids", new SqlParameter("@hid", hid), new SqlParameter("@ids", roomId));
                }
                //记录房态
                var dirty = roomStatu.IsDirty == 0 ? "净房" : roomStatu.IsDirty == 1 ? "脏房" : "清洁房";
                var actionType = type == 1 ? "isService" : "isStop";
                var oldValue = "空房，" +dirty+"，"+ (type == 1 ? "维修房" : "停用房");
                var newValue = "空房，" + dirty;
                SetRoomStatusLog(roomId, roomId.Replace(_currentInfo.HotelId, ""), actionType, oldValue, newValue, roomStatu.Regid, remark);
                return JsonResultData.Successed(roomStatu.IsDirty);
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure("结束失败");
            }
        }
        #endregion

        /// <summary>
        /// 记录房态
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="roomNo"></param>
        /// <param name="actionType"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="regid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public JsonResultData SetRoomStatusLog(string roomId, string roomNo, string actionType, string oldValue, string newValue, string regid, string remark,string waiter=null,string dirtyType=null)
        {
            try
            {
                var data = new RoomStatusLog();
                data.Id = Guid.NewGuid();
                data.CDate = DateTime.Now;
                data.Hid = _currentInfo.HotelId;
                data.InputUser = _currentInfo.UserName;
                data.Roomid = roomId;
                data.RoomNo = roomNo;
                data.ActionType = actionType;
                data.OldValue = oldValue;
                data.NewValue = newValue;
                data.Regid =string.IsNullOrWhiteSpace(regid)?"":regid.Replace(data.Hid,"");
                data.Remark = remark;
                data.Waiter = waiter;
                data.DirtyType = dirtyType;
                _pmsContext.RoomStatusLogs.Add(data);
                _pmsContext.SaveChanges();
                return JsonResultData.Successed();
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
                
            }
           

        }
        private DbHotelPmsContext _pmsContext;
        private ICurrentInfo _currentInfo;
    }
}
