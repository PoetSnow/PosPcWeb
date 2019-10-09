using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.ResManage;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.RoomStatusManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PermanentRoom.Controllers
{
    /// <summary>
    /// 客情
    /// </summary>
    [AuthPage("21020")]
    public class ResOrderCustomerController : BaseController
    {
        /// <summary>
        /// 新客情 或 客情维护
        /// </summary>
        /// <param name="type">类型（R预订，I入住）</param>
        /// <param name="id">子单ID（为空，新订单。否则，维护订单</param>
        /// <param name="parameters">参数 例如：id=000000123&roomid=000000124 </param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(string type, string id, string parameters)
        {
            #region 类型
            if (string.IsNullOrWhiteSpace(type))
            {
                return Content("参数不正确");
            }
            type = type.ToUpper().Trim();
            if (type != "R" && type != "I")
            {
                return Content("参数不正确");
            }
            ViewBag.Type = type;
            #endregion

            #region 参数
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                List<KeyValuePairModel<string, string>> list = new List<KeyValuePairModel<string, string>>();

                parameters = Server.UrlDecode(parameters);
                string[] paraList = parameters.Split('&');
                if (paraList != null && paraList.Length > 0)
                {
                    foreach (var item in paraList)
                    {
                        string[] temp = item.Split('=');
                        if (temp != null && temp.Length == 2)
                        {
                            if (!string.IsNullOrWhiteSpace(temp[0]) && !string.IsNullOrWhiteSpace(temp[1]))
                            {
                                list.Add(new KeyValuePairModel<string, string>(temp[0].ToLower(), temp[1]));
                            }
                        }
                    }
                }
                if (list != null && list.Count > 0)
                {
                    var roomService = GetService<IRoomService>();
                    //房间类型ID
                    var RoomTypeIdEntity = list.FirstOrDefault(c => c.Key == "roomtypeid");
                    if (RoomTypeIdEntity != null && !string.IsNullOrWhiteSpace(RoomTypeIdEntity.Value))
                    {
                        ViewBag.RoomTypeId = RoomTypeIdEntity.Value;
                    }
                    //房间ID
                    var RoomIdEntity = list.FirstOrDefault(c => c.Key == "roomid");
                    if (RoomIdEntity != null && !string.IsNullOrWhiteSpace(RoomIdEntity.Value))
                    {
                        ViewBag.RoomId = RoomIdEntity.Value;
                        //if (!GetService<IRoomService>().IsExistsRoomId(CurrentInfo.HotelId, ViewBag.RoomId))
                        //{
                        //    ViewBag.RoomId = "";
                        //}
                        var roomEntity = roomService.GetEntity(CurrentInfo.HotelId, ViewBag.RoomId);
                        if(roomEntity == null)
                        {
                            ViewBag.RoomId = "";
                        }
                        else
                        {
                            ViewBag.RoomNo = roomEntity.RoomNo;
                        }
                        string roomTypeId = roomService.GetRoomType(CurrentInfo.HotelId, ViewBag.RoomId);
                        if (!string.IsNullOrWhiteSpace(roomTypeId))
                        {
                            ViewBag.RoomTypeId = roomTypeId;
                        }
                        else
                        {
                            ViewBag.RoomTypeId = "";
                        }
                        //id = GetService<IRoomStatusService>().GetRegId(CurrentInfo.HotelId, ViewBag.RoomId);
                    }
                    //抵店时间
                    //var ArrDateEntity = list.FirstOrDefault(c => c.Key == "arrdate");
                    //if (ArrDateEntity != null && !string.IsNullOrWhiteSpace(ArrDateEntity.Value))
                    //{
                    //    DateTime arrDate;
                    //    if (DateTime.TryParse(ArrDateEntity.Value, out arrDate))
                    //    {
                    //        ViewBag.ArrDate = arrDate.ToString(DateTimeExtension.DateFormatStr);
                    //    }
                    //}
                    //是否批量入住
                    //var IsBatchCheckIn = list.FirstOrDefault(c => c.Key == "isbatchcheckin");
                    //if (IsBatchCheckIn != null && !string.IsNullOrWhiteSpace(IsBatchCheckIn.Value))
                    //{
                    //    ViewBag.IsBatchCheckIn = (IsBatchCheckIn.Value == "true" ? true : false);
                    //}
                    //是否拖动换房
                    var changeRoom = list.FirstOrDefault(c => c.Key == "changeroomid");
                    if (changeRoom != null && !string.IsNullOrWhiteSpace(changeRoom.Value))
                    {
                        if (roomService.IsExistsRoomId(CurrentInfo.HotelId, changeRoom.Value))
                        {
                            string roomTypeId = roomService.GetRoomType(CurrentInfo.HotelId, changeRoom.Value);
                            if (!string.IsNullOrWhiteSpace(roomTypeId))
                            {
                                ViewBag.ChangeRoomId = changeRoom.Value;
                                ViewBag.ChangeRoomTypeId = roomTypeId;
                            }
                        }
                    }
                }
            }
            #endregion

            #region 设置默认值
            var pmsParaService = GetService<IPmsParaService>();
            ViewBag.defaultArrTime = pmsParaService.GetValue(CurrentInfo.HotelId, "arrTime");//默认抵店时间
            ViewBag.defaultHoldTime = pmsParaService.GetValue(CurrentInfo.HotelId, "holdTime");//默认保留时间

            var hotelInterfaces = GetService<IHotelInfoService>().GetHotelInterface(CurrentInfo.HotelId);
            var lockInterface = hotelInterfaces.SingleOrDefault(w => w.TypeCode == "01");//只取出设置的门锁读卡器接口信息
            var idInterface = hotelInterfaces.SingleOrDefault(w => w.TypeCode == "02");//只取出设置的身份证读卡器接口信息
            ViewBag.lockType = lockInterface == null ? "" : lockInterface.TypeCode;
            ViewBag.lockCode = lockInterface == null ? "" : lockInterface.Code;
            ViewBag.lockEditionName = lockInterface == null ? "" : lockInterface.EditionName;
            ViewBag.idType = idInterface == null ? "" : idInterface.TypeCode;
            ViewBag.idCode = idInterface == null ? "" : idInterface.Code;
            ViewBag.idEditionName = idInterface == null ? "" : idInterface.EditionName;
            ViewBag.BusinessDate = GetService<IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId).ToString("yyyy-MM-dd");
            ViewBag.IsSignature = CurrentInfo.Signature;
            ViewBag.CurrentHotelId = CurrentInfo.HotelId;
            ViewBag.IsScanSavePhoto = GetService<IPmsParaService>().IsScanSavePhoto(CurrentInfo.HotelId);//是否启用扫描身份证保存照片功能
            #endregion

            #region 返回
            ResMainInfo addModel = new ResMainInfo();
            if (!string.IsNullOrWhiteSpace(id) && !IsPageAuth(this, id))
            {
                //addModel = GetService<IResService>().GetResMainInfoByRegId(CurrentInfo, id);
                addModel.Resid = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetResId(id);
                addModel.SelectRegId = id;
                //addModel.ExtType = EnumExtension.GetDescription(typeof(ExtType), "0");
            }
            else
            {
                addModel.ResTime = DateTime.Now.ToString(DateTimeExtension.DateTimeWithoutSecondFormatStr);
            }
            #endregion

            return PartialView(addModel);
        }

        /// <summary>
        /// 根据子单ID获取订单信息
        /// </summary>
        /// <param name="regId">子单ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetResMainInfoByRegId(string regId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regId))
                {
                    return Json(JsonResultData.Failure("参数错误"));
                }
                Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.ResMainInfo model = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetResMainInfoByRegId(CurrentInfo, regId);
                return Json(JsonResultData.Successed(model));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        #region 填写子单内容需要获取的数据
        /// <summary>
        /// 根据价格码获取价格信息
        /// </summary>
        /// <param name="rateId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetRate(string rateId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rateId))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var entity = GetService<IRateService>().Get(rateId);
                if (entity != null && entity.Hid == CurrentInfo.HotelId)
                {
                    return Json(JsonResultData.Successed(entity));
                }
                else
                {
                    return Json(JsonResultData.Successed(null));
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        /// <summary>
        /// 根据房间类型获取房间标准人数作为早餐份数
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetBreakFastQty(string roomTypeId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomTypeId))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var entity = GetService<IRoomTypeService>().Get(roomTypeId);
                if (entity != null && entity.Hid == CurrentInfo.HotelId)
                {
                    return Json(JsonResultData.Successed(entity.Count));
                }
                else
                {
                    return Json(JsonResultData.Successed(0));
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        /// <summary>
        /// 根据参数获取可入住的房号列表，新入住时使用此方法
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <param name="arrDate"></param>
        /// <param name="depDate"></param>
        /// <param name="regId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetRoomFor(string roomTypeId, DateTime? arrDate, DateTime? depDate, string regId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomTypeId) || !arrDate.HasValue || !depDate.HasValue)
                {
                    return Json(new List<string>(), JsonRequestBehavior.AllowGet);
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetRoomFor(CurrentInfo.HotelId, ResDetailStatus.I, roomTypeId, arrDate.Value, depDate.Value, regId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 获取当前酒店熟客信息
        /// </summary>
        /// <param name="guestName"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult ListItemsForGuests(string guestName)
        {
            if (string.IsNullOrWhiteSpace(guestName))
            {
                guestName = Request.QueryString.Get("filter[filters][0][value]");
            }
            if (!string.IsNullOrWhiteSpace(guestName))
            {
                var service = GetService<IGuestService>();
                var datas = service.GetGuest(CurrentInfo.HotelId, guestName);
                return Json(datas, JsonRequestBehavior.AllowGet);
            }
            return Json(new System.Collections.Generic.List<Services.Entities.Guest>(), JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetGuestByCerId(string cerType, string cerId)
        {
            if (string.IsNullOrWhiteSpace(cerType) || string.IsNullOrWhiteSpace(cerId))
            {
                return Json(JsonResultData.Failure(""), JsonRequestBehavior.DenyGet);
            }
            var guestEntity = GetService<IGuestService>().GetGuestByCerId(CurrentInfo.HotelId, cerType, cerId);
            if (guestEntity != null)
            {
                return Json(JsonResultData.Successed(guestEntity), JsonRequestBehavior.DenyGet);
            }
            return Json(JsonResultData.Failure(""), JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 获取房间类型
        /// </summary>
        /// <param name="arrDate"></param>
        /// <param name="depDate"></param>
        /// <param name="regId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetRoomType(DateTime? arrDate, DateTime? depDate, string regId, string type)
        {
            try
            {
                if (!arrDate.HasValue || !depDate.HasValue)
                {
                    return Json(new List<string>(), JsonRequestBehavior.AllowGet);
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetRoomType(CurrentInfo.HotelId, arrDate.Value, depDate.Value, regId, (type == "I" ? ResDetailStatus.I : ResDetailStatus.R));
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="resInfo"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult Save(Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.ResMainInfo resInfo)
        {
            try
            {
                bool isAddLog = (resInfo != null && resInfo.ResDetailInfos != null && resInfo.ResDetailInfos.Count > 0 && resInfo.ResDetailInfos[0] != null && !string.IsNullOrWhiteSpace(resInfo.ResDetailInfos[0].Regid)) ? false : true;
                var hotelService = GetService<IHotelStatusService>();
                var businessDate = hotelService.GetBusinessDate(CurrentInfo.HotelId);
                var resService = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>();
                var result = resService.AddOrUpdateRes(resInfo, CurrentInfo, businessDate);
                if (result.Success)
                {
                    #region 新入住记录房态
                    if (resInfo.ResDetailInfos[0].Status == "I" && isAddLog)
                    {
                        var roomservice = GetService<IRoomStatusService>();
                        var roomid = resInfo.ResDetailInfos[0].Roomid;
                        var regid = resInfo.ResDetailInfos[0].Regid;
                        var remark = resInfo.ResDetailInfos[0].Remark;
                        var roomstatus = roomservice.GetRoomStatus(CurrentInfo.HotelId, roomid);
                        if (roomstatus != null)
                        {
                            var dirty = roomstatus.IsDirty == 0 ? "净房" : roomstatus.IsDirty == 1 ? "脏房" : "清洁房";
                            var oldValue = "空房，" + dirty;
                            var newValue = "在住房，" + dirty;
                            roomservice.SetRoomStatusLog(roomid, roomid.Replace(CurrentInfo.HotelId, ""), "checkIn", oldValue, newValue, regid, remark);
                        }
                    }
                    #endregion
                }
                AuthorizationInfoConvertToHtml(result, "AuthorizationTemplates/AdjustPrice");
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        #region（取消、恢复）子单
        /// <summary>
        /// 取消预订
        /// </summary>
        /// <param name="regId"></param>
        /// <param name="saveContinue"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.CancelOrderDetailY)]
        public ActionResult CancelOrderDetailY(string regId, string saveContinue)
        {
            return CancelOrderDetail(regId, saveContinue, 1);
        }
        /// <summary>
        /// 取消入住
        /// </summary>
        /// <param name="regId"></param>
        /// <param name="saveContinue"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.CancelOrderDetailZ)]
        public ActionResult CancelOrderDetailZ(string regId, string saveContinue)
        {
            return CancelOrderDetail(regId, saveContinue, 2);
        }
        /// <summary>
        /// 取消子单
        /// </summary>
        /// <param name="regId">子单ID</param>
        /// <param name="saveContinue"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult CancelOrderDetail(string regId, string saveContinue, int type = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regId) || (saveContinue != "0" && saveContinue != "1"))
                {
                    return Json(JsonResultData.Failure("参数错误"));
                }
                string opLog = null;
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().CancelOrderDetail(CurrentInfo.HotelId, MvcApplication.IsTestEnv, regId, saveContinue, out opLog);
                if (result.Success)
                {
                    if (type == 0)
                    {

                    }
                    else if (type == 1)
                    {
                        AddOperationLog(OpLogType.取消预订, opLog, regId);
                    }
                    else if (type == 2)
                    {
                        AddOperationLog(OpLogType.取消入住, opLog, regId);
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        /// <summary>
        /// 恢复入住
        /// </summary>
        /// <param name="regId"></param>
        /// <param name="saveContinue"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.RecoveryOrderDetailZ)]
        public ActionResult RecoveryOrderDetailZ(string regId, string saveContinue)
        {
            return RecoveryOrderDetail(regId, saveContinue, 2);
        }
        /// <summary>
        /// 恢复预订
        /// </summary>
        /// <param name="regId"></param>
        /// <param name="saveContinue"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.RecoveryOrderDetailY)]
        public ActionResult RecoveryOrderDetailY(string regId, string saveContinue)
        {
            return RecoveryOrderDetail(regId, saveContinue, 1);
        }
        /// <summary>
        /// 恢复子单
        /// </summary>
        /// <param name="regId">子单ID</param>
        /// <param name="saveContinue"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult RecoveryOrderDetail(string regId, string saveContinue, int type = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regId) || (saveContinue != "0" && saveContinue != "1"))
                {
                    return Json(JsonResultData.Failure("参数错误"));
                }
                string opLog = null;
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().RecoveryOrderDetail(CurrentInfo.HotelId, MvcApplication.IsTestEnv, regId, saveContinue, out opLog);
                if (result.Success)
                {
                    if (type == 0)
                    {

                    }
                    else if (type == 1)
                    {
                        AddOperationLog(OpLogType.恢复预订, opLog, regId);
                    }
                    else if (type == 2)
                    {
                        AddOperationLog(OpLogType.恢复入住, opLog, regId);
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 门卡
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetLockInfo(string resId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(resId))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetLockInfo(CurrentInfo.HotelId, resId);
                return Json(JsonResultData.Successed(result));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetLockWriteCardPara(string regId, string cardNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regId))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetLockWriteCardPara(CurrentInfo.HotelId, CurrentInfo.UserName, regId, cardNo);
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult WriteLock(string regId, string cardNo, string seqId, string seqNo, string onlockType = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regId) || string.IsNullOrWhiteSpace(cardNo))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().WriteLock(CurrentInfo.HotelId, regId, cardNo, seqId, CurrentInfo.UserName, seqNo, onlockType);
                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult CancelLock(string cardNo, int status, string seqId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cardNo))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().CancelLock(CurrentInfo.HotelId, cardNo, status, seqId);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetResIdByLockInfo(string cardNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cardNo))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetResIdByLockInfo(CurrentInfo.HotelId, cardNo);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [NotAuth]
        public ActionResult Lock()
        {
            try
            {
                string hid = CurrentInfo.HotelId;
                if (!string.IsNullOrWhiteSpace(hid))
                {
                    if (!GetService<IAuthCheck>().HasAuth(CurrentInfo.UserId, "20020", (Int64)AuthFlag.Update, hid))
                    {
                        return View();
                    }
                    var hotelInterfaces = GetService<IHotelInfoService>().GetHotelInterface(hid);
                    var lockInterface = hotelInterfaces.SingleOrDefault(w => w.TypeCode == "01");//只取出设置的门锁读卡器接口信息
                    ViewBag.lockType = lockInterface == null ? "" : lockInterface.TypeCode;
                    ViewBag.lockCode = lockInterface == null ? "" : lockInterface.Code;
                    ViewBag.lockEditionName = lockInterface == null ? "" : lockInterface.EditionName;
                }
            }
            catch
            {
                ViewBag.lockType = "";
                ViewBag.lockCode = "";
                ViewBag.lockEditionName = "";
            }
            return View();
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult CheckLockInfoByRegIds(string regids)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regids))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().CheckLockInfoByRegIds(CurrentInfo.HotelId, regids);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 延期
        [AuthButton(AuthFlag.Update)]
        public ActionResult DelayDepDate(List<KeyValuePairModel<string, DateTime>> data, string saveContinue, string delayContinue)
        {
            try
            {
                if (data == null || data.Count <= 0)
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                List<KeyValuePairModel<string, string>> logList = null;
                string msg = null;
                var hotelService = GetService<IHotelStatusService>();
                var businessDate = hotelService.GetBusinessDate(CurrentInfo.HotelId);
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().DelayDepDate(CurrentInfo.HotelId, data, saveContinue, delayContinue, businessDate, out logList, out msg);
                var newResult = new { Success = result.Success, Data = result.Data, ErrorCode = result.ErrorCode, Msg = msg };
                if (result != null && result.Success && logList != null && logList.Count > 0)
                {
                    foreach (var item in logList)
                    {
                        AddOperationLog(OpLogType.延期, item.Value, item.Key);
                    }
                }
                return Json(newResult, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 换房
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetRoomForRoomType(string regId, string roomTypeId, string floors = null, string features = null, string roomno = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regId) || string.IsNullOrWhiteSpace(roomTypeId))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                if (!string.IsNullOrWhiteSpace(floors) || !string.IsNullOrWhiteSpace(features) || !string.IsNullOrWhiteSpace(roomno))
                {
                    var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetRoomForRoomType(CurrentInfo.HotelId, regId, roomTypeId, floors, features, roomno);
                    return Json(JsonResultData.Successed(result));
                }
                else
                {
                    var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetRoomForRoomType(CurrentInfo.HotelId, regId, roomTypeId);
                    return Json(JsonResultData.Successed(result));
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetRateDetailPrices(string rateId, string roomTypeId, DateTime? beginDate, DateTime? endDate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rateId) || string.IsNullOrWhiteSpace(roomTypeId) || !beginDate.HasValue || !endDate.HasValue)
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                if (beginDate == endDate)
                {
                    endDate = endDate.Value.AddDays(1);
                }
                //var detailPrices = GetService<IRateDetailService>().GetRateDetail(CurrentInfo.HotelId, rateId, roomTypeId, beginDate.Value, endDate.Value);
                //var result = detailPrices.Select(w => new { Ratedate = w.RateDate.ToDateString(), Price = w.Rate }).ToList();
                List<KeyValuePairModel<string, decimal>> list = new List<KeyValuePairModel<string, decimal>>();
                while (beginDate < endDate)
                {
                    list.Add(new KeyValuePairModel<string, decimal> { Key = beginDate.ToDateString(), Value = 0 });
                    beginDate = beginDate.Value.AddDays(1);
                }
                var result = list.Select(w => new { Ratedate = w.Key, Price = w.Value }).ToList();
                return Json(JsonResultData.Successed(result));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult ChangeRoom(string regId, string roomId, List<ResDetailPlanInfo> orderDetailPlans, string remark, string saveContinue, string authorizationSaveContinue, decimal roomPrice, int? water, int? electric, int? gas)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regId) || string.IsNullOrWhiteSpace(roomId) || orderDetailPlans == null || orderDetailPlans.Count <= 0 || roomPrice < 0)
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                string hid = CurrentInfo.HotelId;
                var resService = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>();
                var currentBusinessDate = GetService<IHotelStatusService>().GetBusinessDate(hid);
                //获取换房之前数据
                var resDetailEntity = resService.GetResDetail(hid, regId);
                var price = resService.GetCurrentResDetailPrice(regId, currentBusinessDate);
                //换房
                var result = resService.ChangeRoom(CurrentInfo, regId, roomId, orderDetailPlans, remark, saveContinue, currentBusinessDate, authorizationSaveContinue, roomPrice, water, electric, gas);
                AuthorizationInfoConvertToHtml(result, "AuthorizationTemplates/AdjustPrice");
                var newResult = new { Success = result.Success, Data = result.Data, ErrorCode = result.ErrorCode, ResLogId = "" };
                if (result != null && result.Success)
                {
                    //1.换房成功后，增加换房日志
                    var newPrice = resService.GetCurrentResDetailPrice(regId, currentBusinessDate);
                    var resLogId = GetService<IoperationLog>().AddResLog(hid, CurrentInfo.UserName, UrlHelperExtension.GetRemoteClientIPAddress(), regId, 0, resDetailEntity.Roomid, roomId, (price == null ? "" : price.Value.ToString()), (newPrice == null ? "" : newPrice.Value.ToString()), remark, null);
                    newResult = new { Success = result.Success, Data = result.Data, ErrorCode = result.ErrorCode, ResLogId = resLogId.ToString() };
                    //2.换房成功后，修改房态将原房间转脏房，并记录操作日志
                    var setRoomStatusResult = GetService<Services.RoomStatusManage.IRoomStatusService>().SetRoomStatusDirty(resDetailEntity.Roomid, RoomStatusDirtyFlag.Dirty, true);
                    if (setRoomStatusResult != null && setRoomStatusResult.Success)
                    {
                        AddOperationLog(OpLogType.房态修改, string.Format("修改房态{2}房：{0}=>{1}", "换房完成后自动设置为", "脏房", resDetailEntity.Roomid.Replace(hid, "")), resDetailEntity.Roomid);
                    }
                }
                return Json(newResult, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 关联房
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetRelationList(string notResId, string roomNo, string guestName, string guestMobile, string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(notResId))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetRelationList(CurrentInfo.HotelId, notResId, roomNo, guestName, guestMobile, status, true);
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult AddRelation(string addRegId, string toResId, bool isRes = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(addRegId) || string.IsNullOrWhiteSpace(toResId))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                JsonResultData result = JsonResultData.Failure("参数不正确");
                if (isRes)
                {
                    result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().AddRelation(CurrentInfo.HotelId, addRegId, toResId);
                }
                else
                {
                    result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().AddRelation(CurrentInfo.HotelId, new string[] { addRegId }, toResId);
                }
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult RemoveRelation(string regId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regId))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().RemoveRelation(CurrentInfo.HotelId, regId);
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetRelationResInfo(string regId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regId))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetRelationResInfo(CurrentInfo.HotelId, regId);
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 分房、入住
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetRoomAutoChoose(string resId, bool isAuto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(resId))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetRoomAutoChoose(CurrentInfo.HotelId, resId, isAuto);
                return Json(JsonResultData.Successed(result));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult SaveRooms(string resId, List<KeyValuePairModel<string, List<KeyValuePairModel<string, string>>>> data, string saveContinue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(resId))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().SaveRooms(CurrentInfo.HotelId, resId, data, saveContinue);
                if (result.Success)
                {
                    var temp = result.Data as Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.ResMainInfo;
                    foreach (var item in temp.ResDetailInfos)
                    {
                        if (item.Status == "R" && !string.IsNullOrWhiteSpace(item.Roomid) && !string.IsNullOrWhiteSpace(item.RoomNo) && item.RoomQty == 1)
                        {
                            string regid = item.Regid.Replace(CurrentInfo.HotelId, "");
                            AddOperationLog(OpLogType.预订分房入住, string.Format("主单号：{0}，账号：{1}，房号：{2}，保存留房", resId.Replace(CurrentInfo.HotelId, ""), regid, item.RoomNo), item.Regid);
                        }
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult SaveRoomsAndCheckIn(string resId, List<KeyValuePairModel<string, List<KeyValuePairModel<string, string>>>> data, string saveContinue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(resId) || data == null || data.Count <= 0)
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var businessDate = GetService<IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId);
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().SaveRoomsAndCheckIn(CurrentInfo.HotelId,CurrentInfo.UserName, resId, data, saveContinue, businessDate, MvcApplication.IsTestEnv);
                if (result.Success)
                {
                    List<string> roomidList = new List<string>();
                    foreach (var item in data)
                    {
                        foreach (var child in item.Value)
                        {
                            roomidList.Add(child.Key);
                        }
                    }
                    var temp = result.Data as Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.ResMainInfo;
                    foreach (var item in temp.ResDetailInfos)
                    {
                        if (item.Status == "I" && item.ResStatus == "R" && !string.IsNullOrWhiteSpace(item.Roomid) && !string.IsNullOrWhiteSpace(item.RoomNo) && item.RoomQty == 1 && roomidList.Contains(item.Roomid))
                        {
                            string regid = item.Regid.Replace(CurrentInfo.HotelId, "");
                            AddOperationLog(OpLogType.预订分房入住, string.Format("主单号：{0}，账号：{1}，房号：{2}，分房入住", resId.Replace(CurrentInfo.HotelId, ""), regid, item.RoomNo), item.Regid);
                        }
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult ClearRooms(string resId, List<KeyValuePairModel<string, List<KeyValuePairModel<string, string>>>> data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(resId))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().ClearRooms(CurrentInfo.HotelId, resId, data);
                if (result.Success)
                {
                    foreach (var item in data)
                    {
                        foreach (var child in item.Value)
                        {
                            string regid = item.Key.Replace(CurrentInfo.HotelId, "");
                            AddOperationLog(OpLogType.预订分房入住, string.Format("主单号：{0}，账号：{1}，房号：{2}，清除留房", resId.Replace(CurrentInfo.HotelId, ""), regid, child.Value), item.Key);
                        }
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 其他
        /// <summary>
        /// 根据会员主键ID获取会员信息
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetMbrCardInfo(Guid id)
        {
            try
            {
                return Json(GetService<Gemstar.BSPMS.Hotel.Services.MbrCardCenter.IMbrCardService>().GetMbrCardInfo(CurrentInfo.HotelId, id));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        /// <summary>
        /// 根据合约主键ID获取合约单位信息
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetCompanyInfo(Guid id)
        {
            try
            {
                return Json(GetService<Gemstar.BSPMS.Hotel.Services.ICompanyService>().GetCompanyInfo(CurrentInfo.HotelId, id));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetMbrCardBlance(Guid? id)
        {
            try
            {
                if (id != null && id.HasValue)
                {
                    var sevice = GetService<Gemstar.BSPMS.Hotel.Services.MbrCardCenter.IMbrCardService>();
                    var entity = sevice.GetCardBalance(id.Value);
                    if (sevice.GetGrpid(entity.Hid) == CurrentInfo.GroupHotelId)
                    {
                        ViewBag.MbrCardTypeName = sevice.GetMbrCardTypeNameById(id.Value);
                        return PartialView("_ResMbrCards", entity);
                    }
                }
            }
            catch { }
            return PartialView("_ResMbrCards", null);
        }
        /// <summary>
        /// 获取订单内的特殊要求
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetSpecialRequirements(string resid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(resid))
                {
                    return Json(JsonResultData.Failure(""));
                }
                return Json(GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetSpecialRequirements(CurrentInfo.HotelId, resid));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.None)]
        public JsonResult ResListItemsForCompanys(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                text = Request.QueryString.Get("filter[filters][0][value]");
            }
            var service = GetService<ICompanyService>();
            var datas = service.GetCompanyInfoList(CurrentInfo.HotelId, text);
            return Json(datas, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取酒店价格代码下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ResListItemsForRates()
        {
            var service = GetService<IRateService>();
            var datas = service.PermanentRoomListAll(CurrentInfo.HotelId);
            return Json(datas, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据主单ID获取子单列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public JsonResult GetResDetailsByResId(string resid)
        {
            var list = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetResDetailsByResId(CurrentInfo.HotelId, resid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AuthButton(AuthFlag.Update)]
        public ActionResult GetCommpanyBlance(Guid? id)
        {
            try
            {
                if (id != null && id.HasValue)
                {
                    var entity = GetService<ICompanyService>().GetCommpanyBlance(CurrentInfo.HotelId, id.Value);
                    return PartialView("_ResCompanys", entity);
                }
            }
            catch { }
            return PartialView("_ResCompanys", null);
        }
        /// <summary>
        /// 根据证件类型和证件号码获取会员信息
        /// </summary>
        /// <param name="cerType"></param>
        /// <param name="cerId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetProfileInfoByCerId(string cerType, string cerId)
        {
            if (string.IsNullOrWhiteSpace(cerType) || string.IsNullOrWhiteSpace(cerId))
            {
                return Json(JsonResultData.Failure("参数不能为空！"), JsonRequestBehavior.DenyGet);
            }
            var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetProfileInfoByCerId(CurrentInfo.IsGroup ? CurrentInfo.GroupId : CurrentInfo.HotelId, cerType, cerId, CurrentInfo.IsGroup);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 客人信息批量修改
        /// <summary>
        /// 根据主单ID获取主单内所有客人信息列表
        /// </summary>
        /// <param name="resId">主单ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetCustomerInfoByResId(string resId)
        {
            var datas = GetService<ICodeListService>().GetCerType();
            ViewBag.CerTypeList = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();

            var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetCustomerInfoByResId(CurrentInfo.HotelId, resId);
            return PartialView("_CustomerInfos", result);
        }
        /// <summary>
        /// 保存客人信息
        /// </summary>
        /// <param name="data">客人信息列表</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult SaveCustomerInfos(List<ResDetailCustomerInfos> data)
        {
            return Json(GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().SaveCustomerInfos(CurrentInfo.HotelId, data, CurrentInfo.UserName), JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 更新备注
        [AuthButton(AuthFlag.Update)]
        public ActionResult UpdateRemark(string regid, string remark)
        {
            return Json(GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().UpdateRemark(CurrentInfo, regid, remark), JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 客情调价授权-AdjustPrice
        /// <summary>
        /// 提交授权
        /// </summary>
        [AuthButton(AuthFlag.Update)]
        public ActionResult SubmitAuthByAdjustPrice(Gemstar.BSPMS.Hotel.Services.AuthManages.AuthorizationInfo.SubmitAuthInfo submitAuthInfo)
        {
            if (submitAuthInfo.AuthType != 1)
            {
                return Json(JsonResultData.Failure("参数错误，只接受客情调价授权！"), JsonRequestBehavior.DenyGet);
            }
            var result = GetService<IAuthorizationService>().SubmitAuthorization(CurrentInfo, submitAuthInfo, Weixin.Models.TemplateMessageHelper.SendAuthTemplateMessage);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 获取授权结果
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetAuthResultByAdjustPrice(Guid id)
        {
            var result = GetService<IAuthorizationService>().GetAuthorization(CurrentInfo, id);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 预订选房
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetPermanentRoomForRoomType(string regId, string roomTypeId, string arrDate, string depDate, string type, string floors = null, string features = null, string roomno = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomTypeId) || string.IsNullOrWhiteSpace(arrDate) || string.IsNullOrWhiteSpace(depDate))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetPermanentRoomForRoomType(CurrentInfo.HotelId, regId, roomTypeId, floors, features, roomno, arrDate, depDate, type);
                return Json(JsonResultData.Successed(result));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 根据房号获取房间信息
        /// </summary>
        /// <param name="roomNo"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetPermanentRoomForRoomNo(string roomNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomNo))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetPermanentRoomForRoomNo(CurrentInfo.HotelId, roomNo);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 获取长租房价
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="roomid"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetPermanentRoomPriceForRoomId(string roomid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomid))
                {
                    return Json(JsonResultData.Failure("参数不正确"));
                }
                var result = GetService<Gemstar.BSPMS.Hotel.Services.PermanentRoomManage.IResService>().GetPermanentRoomPriceForRoomId(CurrentInfo.HotelId, roomid);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion
    }
}