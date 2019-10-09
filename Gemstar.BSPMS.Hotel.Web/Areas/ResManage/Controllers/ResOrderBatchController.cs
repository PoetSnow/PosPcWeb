using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.ResManage;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Models.ResOrderBatch;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Controllers
{
    /// <summary>
    /// 批量预订选房、批量入住功能
    /// </summary>
    [AuthPage("20020")]
    public class ResOrderBatchController : BaseController
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(int? isCheckIn, string roomTypeId, string roomId)
        {
            ViewBag.isCheckIn = isCheckIn??0;

            #region 设置默认值
            var pmsParaService = GetService<IPmsParaService>();
            ViewBag.defaultArrTime = pmsParaService.GetValue(CurrentInfo.HotelId, "arrTime");//默认抵店时间
            ViewBag.defaultHoldTime = pmsParaService.GetValue(CurrentInfo.HotelId, "holdTime");//默认保留时间    
            ViewBag.BusinessDate = GetService<IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId).ToString("yyyy-MM-dd");
            ViewBag.roomTypeId = roomTypeId;
            ViewBag.roomId = roomId;
            ViewBag.HotelId = CurrentInfo.HotelId;
            ViewBag.GroupDefaultRateCode = pmsParaService.GroupDefaultRateCode(CurrentInfo.HotelId);//团体默认价格代码
            #endregion

            return View();
        }

        #region 执行ajax数据查询
        [AuthButton(AuthFlag.Query)]
        [KendoGridDatasourceException]
        public ActionResult AjaxQueryRoomTypes(RoomTypeQueryPara para, [DataSourceRequest]DataSourceRequest request)
        {
            //设置查询参数默认值
            if (!para.ArrDate.HasValue)
            {
                para.ArrDate = DateTime.Now;
            }
            if (!para.DepDate.HasValue)
            {
                para.DepDate = DateTime.Now.AddDays(1);
            }
            //执行查询
            var hid = CurrentInfo.HotelId;
            var service = GetService<IResService>();
            var roomTypes = service.GetRoomTypeForRate(hid, para.ArrDate.Value, para.DepDate.Value, para.RateId, (para.IsCheckIn == 1 ? ResDetailStatus.I : ResDetailStatus.R));
            var gridModels = new List<ResOrderBatchGridModel>();
            foreach(var r in roomTypes)
            {
                gridModels.Add(new ResOrderBatchGridModel
                {
                    bbf = r.bbf,
                    id = r.id,
                    name = r.name,
                    rate = r.rate,
                    roomqty = r.roomqty,
                    selectedQty = 0,
                    seqid = r.seqid
                });
            }

            return Json(gridModels.ToDataSourceResult(request));
        }
        [AuthButton(AuthFlag.Query)]
        [KendoGridDatasourceException]
        public ActionResult AjaxQueryRooms(RoomQueryPara para, [DataSourceRequest]DataSourceRequest request)
        {
            //设置查询参数默认值
            if (!para.arrDate.HasValue)
            {
                para.arrDate = DateTime.Now;
            }
            if (!para.depDate.HasValue)
            {
                para.depDate = DateTime.Now.AddDays(1);
            }
            //执行查询
            var hid = CurrentInfo.HotelId;
            var service = GetService<IResService>();
            var rooms = service.GetRoomFor(hid,para.isCheckIn == 1 ? ResDetailStatus.I:ResDetailStatus.R,para.roomTypeId, para.arrDate.Value, para.depDate.Value, null);

            return Json(rooms.ToDataSourceResult(request));
        }
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
        #endregion
        /// <summary>
        /// 保存批量入住，批量预订
        /// </summary>
        /// <param name="addModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.Update)]
        [JsonException]
        public ActionResult Save(ResBatchAddModel addModel)
        {
            var hotelService = GetService<IHotelStatusService>();
            var businessDate = hotelService.GetBusinessDate(CurrentInfo.HotelId);
            var service = GetService<IResService>();
            var result = service.BatchAddRes(addModel, CurrentInfo, businessDate);
            if (result.Success)
            {
                //将regid转换为跳转的链接地址
                var firstRegId = result.Data.ToString();
                var url = Url.Action("Index", "ResOrderAdd", new { area = "ResManage", type = addModel.IsCheckIn == 1 ? "I" : "R", id = firstRegId, parameters = (addModel.IsCheckIn == 1 ? Url.Encode("IsBatchCheckIn=true") : "") });
                result = JsonResultData.Successed(url);
            }
            AuthorizationInfoConvertToHtml(result, "AuthorizationTemplates/AdjustPrice");
            return Json(result);
        }

        #region 批量修改订单状态
        [AuthButton(AuthFlag.Query)]
        [HttpGet]
        public ActionResult ChangeOrderStatus(string resId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(resId))
                {
                    return Json(JsonResultData.Failure("参数错误"));
                }
                ResMainInfo model = GetService<IResService>().GetResMainInfoByResId(CurrentInfo, resId);
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        /// <summary>
        /// 批量修改订单状态提交
        /// </summary>
        /// <param name="resId">主单ID</param>
        /// <param name="type">类型（CancelR：取消预订，CancelI：取消入住，RecoveryR：恢复预订，RecoveryI：恢复入住）</param>
        /// <param name="regIds">子单ID列表</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        [HttpPost]
        public ActionResult ChangeOrderStatus(string resId, string type, List<string> regIds)
        {
            if (string.IsNullOrWhiteSpace(resId) || string.IsNullOrWhiteSpace(type) || regIds == null || regIds.Count <= 0)
            {
                return Json(JsonResultData.Failure("参数不正确！"), JsonRequestBehavior.DenyGet);
            }
            if(type != "CancelR" && type != "CancelI" && type != "RecoveryR" && type != "RecoveryI")
            {
                return Json(JsonResultData.Failure("参数不正确！"), JsonRequestBehavior.DenyGet);
            }

            #region 验证权限
            string authCode = "2002010";
            long authButtonValue = -1;
            switch (type)
            {
                case "CancelR"://取消预订
                    authButtonValue = (Int64)AuthFlag.CancelOrderDetailY;
                    break;
                case "CancelI"://取消入住
                    authButtonValue = (Int64)AuthFlag.CancelOrderDetailZ;
                    break;
                case "RecoveryR"://恢复预订
                    authButtonValue = (Int64)AuthFlag.RecoveryOrderDetailY;
                    break;
                case "RecoveryI"://恢复入住
                    authButtonValue = (Int64)AuthFlag.RecoveryOrderDetailZ;
                    break;
                default:
                    return Json(JsonResultData.Failure("参数不正确！"), JsonRequestBehavior.DenyGet);
            }
            bool isAuth = GetService<Services.AuthManages.IAuthCheck>().HasAuth(CurrentInfo.UserId, authCode, authButtonValue, CurrentInfo.HotelId);
            if (!isAuth)
            {
                return Json(JsonResultData.Failure("没有操作权限！"), JsonRequestBehavior.DenyGet);
            }
            #endregion

            var result = GetService<IResService>().BatchCancelAndRecoveryOrderDetail(CurrentInfo, resId, type, regIds, MvcApplication.IsTestEnv);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

    }
}