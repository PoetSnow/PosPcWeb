using System.Linq;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Gemstar.BSPMS.Hotel.Web.Models;
using Gemstar.BSPMS.Hotel.Services.RoomStatusManage;
using Kendo.Mvc.Extensions;
using Gemstar.BSPMS.Hotel.Web.Areas.RoomState.Models.Picture;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using System.Collections.Generic;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;

namespace Gemstar.BSPMS.Hotel.Web.Areas.RoomState.Controllers
{
    /// <summary>
    /// 房态图
    /// </summary>
    [AuthPage("10001")]
    public class PictureController : BaseController
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            //获取当前酒店是否启用清洁房检查功能
            var paraService = GetService<IPmsParaService>();
            var isRoomCheck = paraService.IsRoomCheck(CurrentInfo.HotelId);
            var isDirtyRoomCheckIn = paraService.GetValue(CurrentInfo.HotelId, "isDirtyRoomCheckIn");
            var isDirty = isDirtyRoomCheckIn == "1" || isDirtyRoomCheckIn == "2";
            var isDirtyLog= paraService.GetValue(CurrentInfo.HotelId, "isDirtyLog");
            var refreshIntervalSeconds = paraService.GetValue(CurrentInfo.HotelId, "refreshIntervalSeconds");
            var isPermanentRoom = IsPermanentRoom();
            if (string.IsNullOrWhiteSpace(refreshIntervalSeconds))
            {
                refreshIntervalSeconds = "0";
            }
            var PicMenuContainNeworder = paraService.GetValue(CurrentInfo.HotelId, "PicMenuContainNeworder");
            if(string.IsNullOrWhiteSpace(PicMenuContainNeworder))
            {
                PicMenuContainNeworder = "1";
            }
            var model = new IndexViewModel
            {
                IsRoomCheck = isRoomCheck,
                IsDirtyRoomCheckIn = isDirty,
                IsDirtyLog = isDirtyLog=="1",
                RefreshIntervalSeconds = int.Parse(refreshIntervalSeconds),
                IsPermanentRoom = isPermanentRoom,
                IsContainNewOrder = PicMenuContainNeworder == "1"
        };
            return View(model);
        }
        /// <summary>
        /// 查询房间状态
        /// </summary>
        /// <param name="queryModel"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        [KendoGridDatasourceException]
        public ActionResult GetRoomStatus(RoomStatusQueryModel queryModel, [DataSourceRequest] DataSourceRequest request)
        {
            var services = GetService<IRoomStatusService>();
            var roomStatus = services.GetCurrentRoomStatus(CurrentInfo.HotelId);
            //从所有的当前房态中，按用户指定的过滤条件进行过滤，只返回满足条件的记录
            var resultList = roomStatus.Where(w=>w.RoomId == w.RoomId);
            var dataList = new List<UpQueryRoomStatusResult>();
            if (!string.IsNullOrWhiteSpace(queryModel.Keyword))
            {
                resultList = resultList.Where(w => w.RoomNo.IndexOf(queryModel.Keyword, StringComparison.OrdinalIgnoreCase) >=0 || w.GuestName.Contains(queryModel.Keyword)||(string.IsNullOrWhiteSpace(w.Cname)?false:w.Cname.Contains(queryModel.Keyword)) || (string.IsNullOrWhiteSpace(w.ArrGuestname)?false:w.ArrGuestname.Contains(queryModel.Keyword)) ||  (string.IsNullOrWhiteSpace(w.ResName) ? false : w.ResName.Contains(queryModel.Keyword)));
            }
            if (!string.IsNullOrWhiteSpace(queryModel.Source))
            {
                resultList = resultList.Where(w => w.SourceId == queryModel.Source);
            }
            if (queryModel.RoomType!=null&&queryModel.RoomType.Length>0)
            {
                resultList = resultList.Where(w =>queryModel.RoomType.Contains(w.RoomTypeId));
            }
            if (queryModel.IsEmpty == 1)
            {
                resultList = resultList.Where(w => w.IsEmpty == 1);
            }
            if(queryModel.InUsing == 1)
            {
                resultList = resultList.Where(w => w.IsEmpty == 0);
            }
            if (queryModel.IsDirty == 1)
            {
                resultList = resultList.Where(w => w.IsDirty == 1);
            }
            if (queryModel.IsClean == 1)
            {
                resultList = resultList.Where(w => w.IsDirty == 2);
            }
            if(queryModel.IsArr == 1)
            {
                resultList = resultList.Where(w => w.IsArr == 1);
            }
            if(queryModel.IsDep == 1)
            {
                resultList = resultList.Where(w => w.IsDep == 1);
            }
            if(queryModel.IsService == 1)
            {
                resultList = resultList.Where(w => w.IsService == 1);
            }
            if(queryModel.IsStop == 1)
            {
                resultList = resultList.Where(w => w.IsStop == 1);
            }
            if(queryModel.IsContinue == 1)
            {
                resultList = resultList.Where(w => w.IsContinue == 1);
            }
            if (queryModel.IsHour == 1)
            {
                resultList = resultList.Where(w => w.HouStatus != 0);
            }
            if (!string.IsNullOrWhiteSpace(queryModel.MarketType))
            {
                resultList = resultList.Where(w => w.Marketid == queryModel.MarketType);
            }
            //勾选左边条件排除假房
            //if (queryModel.IsEan==1|| queryModel.IsEmpty == 1 || queryModel.InUsing == 1 || queryModel.IsArr == 1 || queryModel.IsDep == 1 || queryModel.IsService == 1 || queryModel.IsStop == 1 || queryModel.IsDirty != 0 || queryModel.IsHour == 1 || queryModel.IsContinue == 1)
            //    resultList = resultList.Where(w => !w.IsNotRoom);
            if(queryModel.IsNotRoom != 1)
            {
                resultList = resultList.Where(w => !w.IsNotRoom);
            }
            //if (queryModel.IsRoom != 1)
            //{
            //    resultList = resultList.Where(w => w.IsNotRoom);
            //}
            //净房过滤 维修 停用 脏房
            if (queryModel.IsEan == 1)
                resultList = resultList.Where(w => w.IsDirty == 0 && w.IsService == 0 && w.IsStop == 0);
            //最后执行这个条件
            if (queryModel.Features!=null&& queryModel.Features.Length > 0)
            {
                foreach (var feature in queryModel.Features)
                {
                    if (!string.IsNullOrWhiteSpace(feature))
                    {
                        dataList = resultList.Where(w =>string.IsNullOrWhiteSpace(w.Feature)?false : w.Feature.Contains(feature)).Union(dataList).ToList();
                    }
                }
            }
            else {
                dataList = resultList.ToList();
            }
            return Json(dataList.ToDataSourceResult(request));
        } 
        /// <summary>
        /// 获取指定房间的客人信息
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult RoomInfo(string id)
        {
            //要求此方法执行不会抛出异常，如果参数有问题，可以返回空数据，以便前端能正常显示
            var roomStatuService = GetService<IRoomStatusService>();
            var roomStatu = roomStatuService.GetRoomStatu(id);
            return PartialView("_RoomInfo",roomStatu);
        }

        #endregion

        /// <summary>
        /// 房态图的数量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult RoomQty()
        {
            RoomStausQty roomqty = new RoomStausQty();
            var services = GetService<IRoomStatusService>();
            var roomStatus = services.GetCurrentRoomStatus(CurrentInfo.HotelId);
            var resultList = roomStatus.Where(w => w.RoomId == w.RoomId);
            roomqty.NotRoomCount = resultList.Where( w=> w.IsNotRoom).Count();
            var resultListNotRoom = resultList.Where(w => !w.IsNotRoom);
            int NoArrQty = 0, NoContinueQty = 0, NoDepQty = 0, NoServiceQty = 0, NoStopQty = 0, NotQty = 0, NoUsingQty=0;
            foreach (var list in resultListNotRoom)
            {
                if (list.IsArr == 1)
                {
                    NoArrQty++;
                }
                if (list.IsContinue == 1)
                {
                    NoContinueQty++;
                }
                if (list.IsDep == 1)
                {
                    NoDepQty++;
                }
                if (list.IsService == 1)
                {
                    NoServiceQty++;
                }
                if (list.IsStop == 1)
                {
                    NoStopQty++;
                }
                if (list.IsEmpty == 0)
                {
                    NoUsingQty++;
                }
                NotQty++;
            }
            roomqty.NoArrQty = NoArrQty;
            roomqty.NoContinueQty = NoContinueQty;
            roomqty.NoDepQty = NoDepQty;
            roomqty.NoServiceQty = NoServiceQty;
            roomqty.NoStopQty = NoStopQty;
            roomqty.NotQty = NotQty;
            roomqty.NoUsingQty = NoUsingQty;
            return Json(JsonResultData.Successed(roomqty));
        }

        #region 设置脏净标志
            /// <summary>
            /// 设置为脏房
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
        [AuthButton(AuthFlag.SetDirty)]
        public ActionResult SetDirty(string id,string isdirty)
        {
            var service = GetService<IRoomStatusService>();
            var result = service.SetRoomStatusDirty(id, RoomStatusDirtyFlag.Dirty);
            var roomstatus = isdirty == "2" ? "清洁房" : isdirty == "1" ? "脏房" : "净房";
            var newid = id.Replace(CurrentInfo.HotelId, "");
            if (string.IsNullOrWhiteSpace(isdirty))
                roomstatus = "维修完成后自动设置为";
            AddOperationLog(OpLogType.房态修改, string.Format("修改房态{2}房：{0}=>{1}",roomstatus,"脏房",newid),newid);
            return Json(result);
        }
        /// <summary>
        /// 设置为净房
        /// </summary>
        /// <param name="id"></param>
        /// waiter 服务员 isContinue 是否续住房 1：续住 0：离店
        /// <returns></returns>
        [AuthButton(AuthFlag.SetClean)]
        public ActionResult SetClean(string id,string isdirty,string waiter,string iscontinue,string remark)
        {
            var service = GetService<IRoomStatusService>();
            var result = service.SetRoomStatusDirty(id, RoomStatusDirtyFlag.Clean,false,waiter, iscontinue,remark);
            var roomstatus = isdirty == "2" ? "清洁房" : isdirty == "1" ? "脏房" : "净房";
            var newid = id.Replace(CurrentInfo.HotelId, "");
            AddOperationLog(OpLogType.房态修改, string.Format("修改房态{2}房：{0}=>{1}", roomstatus, "净房", newid), newid);
            return Json(result);
        }
        /// <summary>
        /// 设置为清洁房
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.SetWaitClean)]
        public ActionResult SetWaitClean(string id,string isdirty)
        {
            var service = GetService<IRoomStatusService>();
            var result = service.SetRoomStatusDirty(id, RoomStatusDirtyFlag.WaitClean);
            var roomstatus = isdirty == "2" ? "清洁房" : isdirty == "1" ? "脏房" : "净房";
            var newid = id.Replace(CurrentInfo.HotelId, "");
            AddOperationLog(OpLogType.房态修改, string.Format("修改房态{2}房：{0}=>{1}", roomstatus, "清洁房",newid), newid);
            return Json(result);
        }
        /// <summary>
        /// 维修和停用后选择了服务员后设置为净房
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.SetClean)]
        public ActionResult SetCleanEnd(string id, string isdirty)
        {
            var service = GetService<IRoomStatusService>();
            var result = service.SetRoomStatusDirty(id, RoomStatusDirtyFlag.Clean);
            var roomstatus = isdirty == "2" ? "清洁房" : isdirty == "1" ? "脏房" : "净房";
            var newid = id.Replace(CurrentInfo.HotelId, "");
            AddOperationLog(OpLogType.房态修改, string.Format("修改房态{2}房：{0}=>{1}", roomstatus, "清洁房", newid), newid);
            return Json(result);
        }
        #endregion

        #region 设置维修房 设置停用房
        /// <summary>
        /// 获取维修房状态
        /// </summary>
        [HttpPost]
        [AuthButton(AuthFlag.Service)]
        public ActionResult GetRoomStatusService(string roomId)
        {
            return GetRoomStatusServiceOrStop(roomId, RoomStatusServiceAndStopFlag.Service);
        }
        /// <summary>
        /// 获取停用房状态
        /// </summary>
        [HttpPost]
        [AuthButton(AuthFlag.Stop)]
        public ActionResult GetRoomStatusStop(string roomId)
        {
            return GetRoomStatusServiceOrStop(roomId, RoomStatusServiceAndStopFlag.Stop);
        }
        /// <summary>
        /// 获取房间（维修房或停用房）状态
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="type">类型（1：Service维修，2：Stop停用）</param>
        /// <returns></returns>
        [HttpPost]
        private ActionResult GetRoomStatusServiceOrStop(string roomId, RoomStatusServiceAndStopFlag type)
        {
            var result = GetService<IRoomStatusService>().GetRoomStatusServiceOrStop(CurrentInfo.HotelId, roomId, type, CurrentInfo.UserName);
            return Json(JsonResultData.Successed(result), JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 设置维修房状态
        /// </summary>
        [HttpPost]
        [AuthButton(AuthFlag.Service)]
        public ActionResult SetRoomStatusService(RoomStatusServiceAndStopPara para)
        {
            para.Type = RoomStatusServiceAndStopFlag.Service;
            return SetRoomStatusServiceOrStop(para);
        }
        /// <summary>
        /// 设置停用房状态
        /// </summary>
        [HttpPost]
        [AuthButton(AuthFlag.Stop)]
        public ActionResult SetRoomStatusStop(RoomStatusServiceAndStopPara para)
        {
            para.Type = RoomStatusServiceAndStopFlag.Stop;
            return SetRoomStatusServiceOrStop(para);
        }
        /// <summary>
        /// 设置房间（维修房或停用房）状态
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="type">类型（1：Service维修，2：Stop停用）</param>
        /// <returns></returns>
        [HttpPost]
        private ActionResult SetRoomStatusServiceOrStop(RoomStatusServiceAndStopPara para)
        {
            var businessDate = GetService<Gemstar.BSPMS.Hotel.Services.SystemManage.IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId);
            var result = GetService<IRoomStatusService>().SetRoomStatusServiceOrStop(CurrentInfo.HotelId, para, businessDate, CurrentInfo.UserName);
            var id = para.RoomId.Replace(CurrentInfo.HotelId, "");
            var IsServcie = para.Type == RoomStatusServiceAndStopFlag.Service;
            AddOperationLog(OpLogType.房态修改, string.Format("房态修改{4}房，设置为{0}，结束日期：{1}，原因：{2}，说明：{3}，{5}",
                IsServcie?"维修房":"停用房",para.PlanEndDateTime,para.Reason,para.Remark,id,IsServcie?"维修人："+para.ServiceUser:""
                ), 
                id);
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 结束维修房状态
        /// </summary>
        [HttpPost]
        [AuthButton(AuthFlag.Service)]
        public ActionResult EndRoomStatusService(RoomStatusServiceAndStopPara para)
        {
            return EndRoomStatusServiceOrStop(para);
        }
        /// <summary>
        /// 结束停用房状态
        /// </summary>
        [HttpPost]
        [AuthButton(AuthFlag.Stop)]
        public ActionResult EndRoomStatusStop(RoomStatusServiceAndStopPara para)
        {
            return EndRoomStatusServiceOrStop(para);
        }
        /// <summary>
        /// 结束房间（维修房或停用房）状态
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="type">类型（1：Service维修，2：Stop停用）</param>
        /// <returns></returns>
        [HttpPost]
        private ActionResult EndRoomStatusServiceOrStop(RoomStatusServiceAndStopPara para)
        {
            var businessDate = GetService<Gemstar.BSPMS.Hotel.Services.SystemManage.IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId);
            var result = GetService<IRoomStatusService>().EndRoomStatusServiceOrStop(CurrentInfo.HotelId, para.RoomId,para.Type, businessDate, CurrentInfo.UserName,para.Remark,para.ServiceUser, para);
            var id = para.RoomId.Replace(CurrentInfo.HotelId, "");
            var isService = para.Type == RoomStatusServiceAndStopFlag.Service;
            AddOperationLog(OpLogType.房态修改, string.Format("修改房态{2}房，{0}完成，原因：{1}，{3}",
                isService ? "维修房" : "停用房",para.Remark,id,isService? "维修人："+para.ServiceUser:""
               ),
               id);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 修改房间描述
        [AuthButton(AuthFlag.Details)]
        public ActionResult UpdateRoomRemark(string roomid)
        {
           var service=  GetService<IRoomService>();
           var data= service.Get(roomid);
            if (data != null)
                return Json(JsonResultData.Successed(data.Description));
            return Json(JsonResultData.Failure("不存在此房间"));
        }
        [AuthButton(AuthFlag.Details)]
        public ActionResult SaveRoomRemark(string roomid, string remark)
        {
            var service= GetService<IRoomService>();
            var data= service.UpdateRoom(CurrentInfo.HotelId, remark, roomid);
            if (data)
                return Json(JsonResultData.Successed());
            return Json(JsonResultData.Failure("保存失败"));
        }
        #endregion

        #region 设置提醒
        [AuthButton(AuthFlag.Add)]
        public ActionResult NotifySet(string roomid)
        {
            //房间号和（未通知或已通知）则为有效的
            var service = GetService<IWakeCallService>();
            var data = service.GetWakeCall(roomid, CurrentInfo.HotelId);
            return Json(JsonResultData.Successed(data));
        }
        [AuthButton(AuthFlag.Add)]
        public ActionResult NotifyAdd(string roomid, string notifyId, DateTime notifyTime, string notifyContent,string notifyRemarks)
        {
            Guid id;
            bool result;
            string resContent = "";
            if(!string.IsNullOrWhiteSpace(notifyId) && Guid.TryParse(notifyId,out id))
            {//修改
                var service = GetService<IWakeCallService>();
                WakeCall wk = service.Get(id);
                wk.Content = notifyContent;
                wk.CallTime = notifyTime;
                wk.Remark = notifyRemarks;
                result = service.AddNotify(wk,false);
                resContent = "修改成功";
            }
            else
            {//新增
                var service = GetService<IWakeCallService>();
                Guid wkId = Guid.NewGuid();
                WakeCall wk = new WakeCall()
                {
                    Hid = CurrentInfo.HotelId,
                    Id = wkId,
                    RoomId = roomid,
                    RoomNo = roomid.Replace(CurrentInfo.HotelId, ""),
                    Content = notifyContent,
                    CallTime = notifyTime,
                    Remark = notifyRemarks,
                    Creater = CurrentInfo.UserName,
                    CreateTime = DateTime.Now,
                    WakeCallTypeName = "房间提醒",
                    WakeMethod = 0,
                    Status = 1
                };
                result = service.AddNotify(wk);
                if(result)
                {
                    WakeCallDetil wkdetil = new WakeCallDetil()
                    {
                        Hid = CurrentInfo.HotelId,
                        Id = Guid.NewGuid(),
                        NotifyId = wkId,
                        Status = 0
                    };
                    result = service.AddNotifyDetil(wkdetil);
                }
                
                resContent = "新增成功";
            }            
            if (result)
                return Json(JsonResultData.Successed(resContent));
            return Json(JsonResultData.Failure("保存失败"));
        }
        #endregion

        public class RoomStausQty
        {
            public int NotRoomCount { get; set; }//假房总房数
            public int NotQty { get; set; }//不含假房总房数
            public int NoUsingQty { get; set; }//不含假房在住房
            public int NoContinueQty { get; set; }//不含假房续住房
            public int NoArrQty { get; set; }//不含假房预抵房
            public int NoDepQty { get; set; }//不含假房预离房
            public int NoServiceQty { get; set; }//不含假房维修房
            public int NoStopQty { get; set; }//不含假房停用房
        }
    }
}