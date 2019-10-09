using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.RoomStatusManage;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.MarketingManage
{
    public class WakeCallService : CRUDService<WakeCall>, IWakeCallService
    {
        private DbHotelPmsContext _db;
        private ICurrentInfo _currentInfo;
        public WakeCallService(DbHotelPmsContext db,ICurrentInfo currentInfo) : base(db, db.WakeCalls)
        {
            _db = db;
            _currentInfo = currentInfo;
        }

        protected override WakeCall GetTById(string id)
        {
            var bnId = Guid.Parse(id);
            var wak = _db.WakeCalls.Find(bnId);
            return wak;
        }
        /// <summary>
        /// 根据房间号查询提醒消息
        /// </summary>
        /// <param name="roomNo"></param>
        /// <param name="hid"></param>
        /// <returns></returns>
        public WakeCall GetWakeCall(string roomNo, string hid)
        {
            return _db.WakeCalls.Where(c => c.RoomId == roomNo && c.Hid == hid && (c.Status == 0 || c.Status == 1)).OrderByDescending(c=> c.CreateTime).FirstOrDefault();
        }
        /// <summary>
        /// 新增或修改提醒
        /// </summary>
        /// <param name="wk"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        public bool AddNotify(WakeCall wk,bool isAdd = true)
        {
            if (isAdd)
            {
                _db.Entry(wk).State = EntityState.Added;
                AddDataChangeLog(OpLogType.设置提醒);
            }
            else
            {
                _db.Entry(wk).State = EntityState.Modified;
                AddDataChangeLog(OpLogType.设置提醒);
            }
                return _db.SaveChanges() > 0;
            
        }
        /// <summary>
         /// 新增或修改提醒明细表
         /// </summary>
         /// <param name="wkdetil"></param>
         /// <param name="isAdd"></param>
         /// <returns></returns>
        public bool AddNotifyDetil(WakeCallDetil wkdetil, bool isAdd = true)
        {
            if (isAdd)
            {
                _db.Entry(wkdetil).State = EntityState.Added;
            }
            else
            {
                _db.Entry(wkdetil).State = EntityState.Modified;
            }
            return _db.SaveChanges() > 0;

        }
        /// <summary>
        /// 查询提醒
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="roomId"></param>
        /// <param name="userId"></param>
        /// <param name="notifyDateBegin"></param>
        /// <param name="notifyDateEnd"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<UpQueryNotifyResult> GetNotify(string hid,string roomId,string userId,string notifyDateBegin,string notifyDateEnd,int status)
        {
            return _db.Database.SqlQuery<UpQueryNotifyResult>(@"
            EXEC dbo.up_queryNotify @hid = @hid,@roomId = @roomId, @userId=@userId,@notifyDateBegin = @notifyDateBegin,
            @notifyDateEnd = @notifyDateEnd,
            @status = @status"
             , new SqlParameter("@hid", hid)
             , new SqlParameter("@roomId", roomId)
             , new SqlParameter("@userId", userId)
             , new SqlParameter("@notifyDateBegin", notifyDateBegin)
             , new SqlParameter("@notifyDateEnd", notifyDateEnd)
             , new SqlParameter("@status", status)).ToList();
        }
        /// <summary>
        /// 批量提醒消息设置
        /// </summary>
        /// <param name="id">多条提醒ID</param>
        /// <param name="userId">操作员ID</param>
        /// <param name="type">类型：1已读，2处理，3作废</param>
        /// <param name="content">作废原因或处理说明</param>
        /// <returns></returns>
        public JsonResultData BatchRead(string id, string userId,int type,string content)
        {
            try
            {
                var ids = id.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    var json = FinanceAction(Guid.Parse(ids[i]), userId, type, content);
                    if (!json.Success)
                    {
                        return JsonResultData.Failure(json.Data);
                    }
                }
                return JsonResultData.Successed("操作成功");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure("操作失败,"+ ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 提醒消息设置
        /// </summary>
        /// <param name="nitfyId">提醒ID</param>
        /// <param name="userid">操作员ID</param>
        /// <param name="type">类型：1接单，2处理，3作废</param>
        /// <param name="Remark">作废或处理内容</param>
        /// <returns></returns>
        private JsonResultData FinanceAction(Guid notifyId, string userid,int type,string remark)
        {
            try
            {
                var result = _db.Database.SqlQuery<upJsonResultData<string>>("EXEC up_NotifyDeal @nitfyId=@nitfyId, @userid = @userid,@type=@type, @remark = @remark",
                    new SqlParameter("@nitfyId", notifyId),
                    new SqlParameter("@userid", userid),
                    new SqlParameter("@type", type),
                    new SqlParameter("@remark", remark)
                    ).FirstOrDefault();
                if (result.Success)
                {
                    var entity = _db.WakeCalls.FirstOrDefault(w => w.Id == notifyId);
                    if(entity != null)
                    {
                        AddOperationLog(_currentInfo, OpLogType.接收提醒, string.Format("房号：{0}，操作：{1}提醒消息，{1}说明:{2}", entity.RoomNo, (Notifytype)type,remark), notifyId.ToString());
                    }
                    

                    return JsonResultData.Successed();
                }
                return JsonResultData.Failure(result.Data);
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        public enum Notifytype
        {
            接单 = 1,
            处理 = 2,
            作废 = 3
        }
        public string GetRoomNo(string RoomId,string hid)
        {
            return _db.Rooms.FirstOrDefault(r => r.Id == RoomId && r.Hid == hid).RoomNo;
        }

        public WakeCallDetil GetDetil(int status,Guid notifyId)
        {
            if(status == 2)
            {
                return _db.WakeCallDetils.FirstOrDefault(d => d.NotifyId == notifyId && d.Status == 2);
            }
            else
            {
                return _db.WakeCallDetils.FirstOrDefault(d => d.NotifyId == notifyId && d.ReadTime != null);
            }
        }
        /// <summary>
        /// 获取提醒提前分钟数
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public double NotifyTimeBef(string hid)
        {
            double result = 0;
            var time = _db.PmsParas.SingleOrDefault(w => w.Hid == hid && w.Code == "NotifyTimeBef");
            if (time == null || time.Value == null)
            {
                result = 5;
            }
            else
            {
                if(double.TryParse(time.Value,out result) && result>=0)
                {
                    return result;
                }
                else
                {
                    result = 5;
                }
            }
            return result;
        }

        #region 操作日志
        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="currentInfo">登录信息</param>
        /// <param name="type">日志类型</param>
        /// <param name="text">内容</param>
        /// <param name="keys">关键字</param>
        public void AddOperationLog(ICurrentInfo currentInfo, OpLogType type, string text, string keys)
        {
            _db.Database.ExecuteSqlCommandAsync("INSERT INTO [opLog]([hid],[cDate],[cUser],[ip],[xType],[cText],[keys])VALUES(@hid,@cDate,@cUser,@ip,@xType,@cText,@keys)",
                new SqlParameter("@hid", currentInfo.HotelId),
                new SqlParameter("@cDate", DateTime.Now),
                new SqlParameter("@cUser", currentInfo.UserName),
                new SqlParameter("@ip", UrlHelperExtension.GetRemoteClientIPAddress()),
                new SqlParameter("@xType", type.ToString()),
                new SqlParameter("@cText", (text.Length > 4000 ? text.Substring(0, 4000) : text)),
                new SqlParameter("@keys", keys)
                ).Wait();
        }
        #endregion
    }
}
