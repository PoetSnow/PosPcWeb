using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Services.ResFolioManage;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Models.ResOrderFolio;
using Gemstar.BSPMS.Common.Services;
using System;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using System.Transactions;
using System.Collections.Generic;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services;
using System.Linq;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.ResManage;
using System.Text;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Hotel.Services.RoomStatusManage;
using Gemstar.BSPMS.Hotel.Services.NotifyManage;
using Gemstar.BSPMS.Hotel.Services.EF.PayManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Controllers
{
    /// <summary>
    /// 客账
    /// </summary>
    [AuthPage("20020")]
    public class ResOrderFolioController : BaseController
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(string id)
        {
            var folioInfo = new ResFolioMainInfo();
            if(!string.IsNullOrWhiteSpace(id) && !IsPageAuth(this, id))
            {
                //修改
                //var service = GetService<IResFolioService>();
                //folioInfo = service.GetResFolioMainInfoByRegId(CurrentInfo.HotelId,id);
            }
            folioInfo.CurrentUserName = CurrentInfo.UserName;
            ViewBag.HotelId = CurrentInfo.HotelId;
            ViewBag.isPayPrintDeposit = GetService<IPmsParaService>().IsPayPrintDeposit(CurrentInfo.HotelId);
            return PartialView(folioInfo);
        }

        #region 执行ajax数据查询
        [AuthButton(AuthFlag.Query)]
        [KendoGridDatasourceException]
        public ActionResult AjaxQueryFolios(ResFolioQueryPara para, [DataSourceRequest]DataSourceRequest request)
        {
            para.Hid = CurrentInfo.HotelId;
            var service = GetService<IResFolioService>();
            var folios = service.QueryResFolioFolioInfos(para);

            return Json(folios.ToDataSourceResult(request));
        }
        [AuthButton(AuthFlag.Query)]
        [KendoGridDatasourceException]
        public ActionResult AjaxFolioGuest(string resId, [DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IResFolioService>();
            var detailInfos = service.GetResFolioDetailInfoByResId(CurrentInfo.HotelId, resId);
            return Json(detailInfos.ToDataSourceResult(request));
        }
        /// <summary>
        /// 查询可取消结账的结账批次号信息
        /// </summary>
        /// <param name="resId">订单id</param>
        /// <returns>查询结果</returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult AjaxCheckoutBatchNos(string resId)
        {
            try
            {
                var services = GetService<IResFolioService>();
                var batchNos = services.QueryResFolioCheckoutBatchNos(CurrentInfo.HotelId, resId);
                return Json(JsonResultData.Successed(batchNos));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        /// <summary>
        /// 查询预授权信息
        /// </summary>
        /// <param name="resId">订单id</param>
        /// <returns>查询结果</returns>
        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.Query)]
        public ActionResult AjaxCardAuths(string resId, [DataSourceRequest]DataSourceRequest request)
        {
            var services = GetService<IResFolioService>();
            var cardAuths = services.QueryCardAuths(CurrentInfo.HotelId, resId);
            return Json(cardAuths.ToDataSourceResult(request));
        }
        /// <summary>
        /// 根据账务ID获取账务信息
        /// </summary>
        /// <param name="transid">账务ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetFolioByTransid(string transid)
        {
            Guid id = Guid.Empty;
            if(Guid.TryParse(transid,out id))
            {
                if(id!=null && id != Guid.Empty)
                {
                    var result = GetService<IResFolioService>().GetFolioByTransid(CurrentInfo.HotelId, id);
                    return Json(result, JsonRequestBehavior.DenyGet);
                }
            }
            return Json(JsonResultData.Failure(""), JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 入账
        [AuthButton(AuthFlag.None)]
        public ActionResult AddFolilIdentical(string resId, string selectedRegId, int isCheckout,string checkoutRegIds="")
        {
            ViewBag.isCheckout = isCheckout;
            var services = GetService<IResFolioService>();
            var regIds = services.QueryResDetailForFolioAdd(CurrentInfo.HotelId, resId);
            //如果是结账并且有传递结账时选中的客账，则结账弹出的入账时，只能选择之前选择好的客账明细，而不是所有客账明细
            if(isCheckout == 1 && !string.IsNullOrWhiteSpace(checkoutRegIds))
            {
                var checkoutRegIdArray = checkoutRegIds.Split(',');
                regIds = regIds.Where(w => checkoutRegIdArray.Contains(w.Regid)).ToList();
            }
            var viewModel = new ResFolioAddViewModel
            {
                RegIds = regIds,
                FolioRegId = selectedRegId
            };
            return PartialView("_AddFolio", viewModel);
        }
        [AuthButton(AuthFlag.Add)]
        public ActionResult AddFolio(string resId,string selectedRegId,int isCheckout=0)
        {
            return AddFolilIdentical( resId,  selectedRegId,  isCheckout);
        } 
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult AddFolio(ResFolioAddViewModel model)
        {
            try
            {
                var outAuthResult = OutOrderOperateFolioAuth(1, model.FolioRegId); if (outAuthResult != null) { return outAuthResult; }
                if (ModelState.IsValid)
                {
                    var service = GetService<IResFolioService>();
                    var payServiceBuilder = GetService<IPayServiceBuilder>();
                    var commonDb = GetService<DbCommonContext>();
                    var pmsParaService = GetService<IPmsParaService>();

                    var commonPayParas = commonDb.M_v_payParas.ToList();
                    var hotelPayParas = pmsParaService.GetPmsParas(CurrentInfo.HotelId);

                    upProfileCaInputResult addResult=null;
                    List<string> multipleAddedFolioTransIds = null;
                    IPayService payService = null;
                    string authUserName = null;string reason = "";
                    using (var tc = new TransactionScope())
                    {
                        if (model.FolioDCFlag.Equals("d", StringComparison.OrdinalIgnoreCase))
                        {
                            var resFolioDebitPara = new ResFolioDebitPara
                            {
                                Hid = CurrentInfo.HotelId,
                                RegId = model.FolioRegId,
                                ItemId = model.FolioItemId,
                                Quantity = model.FolioItemQty,
                                Amount = model.FolioAmount.Value,
                                InvNo = model.FolioInvoNo,
                                Remark = model.FolioRemark,
                                UserName = CurrentInfo.UserName,
                                TransShift = CurrentInfo.ShiftId
                            };
                            bool isCheckAuth = false;
                            var checkResult = CheckAuthAbatement(resFolioDebitPara);
                            if (!checkResult.Success)
                            {
                                if (!CheckAuthAbatement(model.AuthorizationSaveContinue, "", out authUserName,out reason))
                                {
                                    return Json(checkResult, JsonRequestBehavior.DenyGet);
                                }
                                else
                                {
                                    isCheckAuth = true;
                                }
                            }
                            addResult = service.AddFolioDebit(resFolioDebitPara);
                            if (isCheckAuth && addResult.Success)
                            {
                                CheckAuthAbatement(model.AuthorizationSaveContinue, addResult.Data, out authUserName,out reason);
                            }
                        }
                        else
                        {
                            //如果是付款，则获取支付服务实例，进行支付，并且将支付成功后返回的交易号保存到refno中
                            payService = payServiceBuilder.GetPayService(model.FolioItemAction, commonPayParas, hotelPayParas,MvcApplication.IsTestEnv);
                            var payResult = new PayResult { RefNo = "", IsWaitPay = false };
                            if (payService != null)
                            {
                                if (string.IsNullOrWhiteSpace(model.FolioItemActionJsonPara))
                                {
                                    return Json(JsonResultData.Failure("参数不能为空"));
                                }
                                payResult = payService.DoPayBeforeSaveFolio(model.FolioItemActionJsonPara);
                            }
                            if (payResult.IsMultiple)
                            {
                                multipleAddedFolioTransIds = new List<string>();
                                var inputDate = DateTime.Now;
                                foreach (var itemInfo in payResult.MultipleItemInfos)
                                {
                                    addResult = service.AddFolioCredit(new ResFolioCreditPara
                                    {
                                        Hid = CurrentInfo.HotelId,
                                        RegId = model.FolioRegId,
                                        ItemId = itemInfo.ItemId,
                                        OriAmount = itemInfo.Amount,
                                        Amount = itemInfo.Amount,
                                        InvNo = model.FolioInvoNo,
                                        Remark = (model.FolioRemark??"")+(itemInfo.Remark??""),
                                        UserName = CurrentInfo.UserName,
                                        RefNo = itemInfo.RefNo,
                                        IsWaitPay = payResult.IsWaitPay,
                                        TransShift = CurrentInfo.ShiftId,
                                        Paymentdesc = model.IsCheckout == 1 ? model.FolioAmount.Value > 0 ? "C" : "D" : "",
                                        InputDate = inputDate
                                    });
                                    multipleAddedFolioTransIds.Add(addResult.Data.ToString());
                                }
                            }
                            else
                            {
                                addResult = service.AddFolioCredit(new ResFolioCreditPara
                                {
                                    Hid = CurrentInfo.HotelId,
                                    RegId = model.FolioRegId,
                                    ItemId = model.FolioItemId,
                                    OriAmount = model.FolioOriAmount.Value,
                                    Amount = model.FolioAmount.Value,
                                    InvNo = model.FolioInvoNo,
                                    Remark = (model.FolioRemark??"")+(payResult.Remark??""),
                                    UserName = CurrentInfo.UserName,
                                    RefNo = payResult.RefNo,
                                    IsWaitPay = payResult.IsWaitPay,
                                    TransShift = CurrentInfo.ShiftId,
                                    Paymentdesc = model.IsCheckout == 1 ? model.FolioAmount.Value > 0 ? "C" : "D" : "",
                                    FolioDepositType = model.FolioDepositType
                                });
                            }
                        }
                        tc.Complete();
                    }
                    //记录日志
                    var items = GetService<IItemService>().Query(CurrentInfo.HotelId,model.FolioDCFlag,null);
                    var item=items.Where(s => s.Id == model.FolioItemId).FirstOrDefault();
                    var reg = GetService<IResService>().GetResDetailRegid(CurrentInfo.HotelId, model.FolioRegId);
                    var logType = model.FolioDCFlag.Equals("d", StringComparison.OrdinalIgnoreCase);
                    AddOperationLog(OpLogType.入账,
                        string.Format("{9}　账号：{0}，房号：{8}，{7}：{1}，数量：{2}，金额：{3:F2}，单号：{4}，备注：{5}，班次：{6}{10}",
                        model.FolioRegId.Replace(CurrentInfo.HotelId, ""),
                        item==null?"":item.Name,
                        model.FolioItemQty,
                        model.FolioAmount.Value,
                        model.FolioInvoNo,
                        model.FolioRemark,
                        CurrentInfo.ShiftName,
                        logType ? "消费项目" : "付款方式",
                        reg==null?"":reg.RoomNo,
                        logType?"入账-消费":"入账-付款",
                        (!string.IsNullOrWhiteSpace(authUserName) ? ("，"+ reason + "，授权人：" + authUserName) : "")
                        ),
                         model.FolioRegId);
                    var returnResult = new ResFolioAddResult
                    {
                        FolioTransId = addResult.Data,
                        Statu = PayStatu.Successed.ToString(),
                        Callback = "",
                        QrCodeUrl = "",
                        QueryTransId = "",
                        DCFlag = model.FolioDCFlag
                    };
                    //如果有多笔明细入账，则需要将每笔记录id以逗号分隔后进行返回
                    if(multipleAddedFolioTransIds != null)
                    {
                        returnResult.FolioTransId = string.Join(",", multipleAddedFolioTransIds);
                    }
                    if (model.FolioDCFlag.Equals("c", StringComparison.OrdinalIgnoreCase))
                    {
                        if (payService != null)
                        {
                            //转换一下folio的transid格式，以保证长度为32位
                            var transId = Guid.Parse(addResult.Data).ToString("N");
                            var afterPayResult = payService.DoPayAfterSaveFolio(PayProductType.ResFolio,transId, model.FolioItemActionJsonPara);
                            returnResult.Statu = afterPayResult.Statu.ToString();
                            returnResult.Callback = afterPayResult.Callback;
                            returnResult.QrCodeUrl = afterPayResult.QrCodeUrl;
                            returnResult.QueryTransId = afterPayResult.QueryTransId;
                        }
                    }
                       
                    return Json(JsonResultData.Successed(returnResult));
                }
                else
                {
                    return Json(JsonResultData.Failure(ModelState.Values));
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
                throw;
            }
        }
        #endregion

        #region 日租半日租
        [AuthButton(AuthFlag.CheckoutCheck)]
        public ActionResult CheckDayCharge(string regIds)
        {
            try
            {
                var outAuthResult = OutOrderOperateFolioAuth(1, regIds); if (outAuthResult != null) { return outAuthResult; }
                var services = GetService<IResFolioService>();
                var chargeInfos = services.DayChargeCheck(CurrentInfo.HotelId, regIds);
                return Json(JsonResultData.Successed(chargeInfos));
            }catch(Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.CheckoutCheck)]
        public ActionResult AddDayCharge(List<upResFolioDayChargeCheckResult> chargeInfos, string authorizationSaveContinue, bool isTemp = false)
        {
            try
            {
                bool isCheckAuth = false;
                string authUserName = null;string reason = "";
                var checkResult = CheckAuthDayCharge(chargeInfos, isTemp);
                if (!checkResult.Success)
                {
                    if (!CheckAuthDayCharge(authorizationSaveContinue, "", out authUserName,out reason))
                    {
                        return Json(checkResult, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        isCheckAuth = true;
                    }
                }
                var services = GetService<IResFolioService>();
                var transIds = services.DayChargeAdd(CurrentInfo.HotelId, CurrentInfo.UserName, CurrentInfo.ShiftId, chargeInfos, isTemp);
                if (!string.IsNullOrWhiteSpace(transIds) && !isTemp)
                {
                    if (isCheckAuth)
                    {
                        CheckAuthDayCharge(authorizationSaveContinue, transIds, out authUserName,out reason);
                    }
                    var log = new StringBuilder();
                    log.Append("收取 ");
                    foreach(var info in chargeInfos)
                    {
                        log.AppendFormat("账号{0}房号{1}{2}{3}，"
                            ,CurrentInfo.GetStringWithoutHotelId(info.RegId)
                            ,info.RoomNo
                            ,info.Type
                            ,info.Amount
                            );
                    }
                    log.AppendFormat("班次：{0}",CurrentInfo.ShiftName);
                    if (!string.IsNullOrWhiteSpace(authUserName))
                    {
                        log.AppendFormat("，{0}", reason);
                        log.AppendFormat("，授权人：{0}", authUserName);
                    }
                    AddOperationLog(OpLogType.日租半日租收取, log.ToString(), (chargeInfos != null && chargeInfos.Count > 0 ? chargeInfos[0].RegId : ""));
                }
                return Json(JsonResultData.Successed(transIds));
            }catch(Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.CheckoutCheck)]
        public ActionResult FreeDayCharge(string regIds,string freeReason, string authorizationSaveContinue)
        {
            try
            {
                bool isCheckAuth = false;
                string authUserName = null;string reason = "";
                var checkResult = CheckAuthDayChargeByFree(regIds);
                if (!checkResult.Success)
                {
                    if (!CheckAuthDayCharge(authorizationSaveContinue, "", out authUserName,out reason))
                    {
                        return Json(checkResult, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        isCheckAuth = true;
                    }
                }
                var services = GetService<IResFolioService>();
                services.DayChargeFree(CurrentInfo.HotelId, regIds,freeReason);
                if (!string.IsNullOrWhiteSpace(regIds))
                {
                    if (isCheckAuth)
                    {
                        CheckAuthDayCharge(authorizationSaveContinue, regIds, out authUserName,out reason);
                    }
                    string opLog = string.Format("操作员：{0}，班次：{1}。免收详情：", CurrentInfo.UserName, CurrentInfo.ShiftId.Replace(CurrentInfo.HotelId, ""));
                    string splitstr = ",";
                    if (regIds.Contains(splitstr))
                    {
                        opLog += string.Format("账号：{0}。", ((splitstr + regIds).Replace(splitstr + CurrentInfo.HotelId, splitstr)).Substring(1));
                    }
                    else
                    {
                        opLog += string.Format("账号：{0}。", regIds.Replace(CurrentInfo.HotelId, ""));
                    }
                    opLog += "免收原因：" + freeReason;
                    if (!string.IsNullOrWhiteSpace(authUserName))
                    {
                        opLog += string.Format("，{0}", reason);
                        opLog += string.Format("，授权人：{0}", authUserName);
                    }
                    AddOperationLog(OpLogType.日租半日租免收, opLog, (regIds.Split(',')[0]));
                }
                return Json(JsonResultData.Successed(""));
            }catch(Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        /// <summary>
        /// 移除之前加收的日租，半日租
        /// 在回收成功后，结账不成功，或者是没有进行结账操作，直接关闭结账窗口时触发
        /// </summary>
        /// <param name="regIds">之前加收日租半日租的regids，以逗号分隔</param>
        /// <returns>移除是否成功</returns>
        [AuthButton(AuthFlag.CheckoutCheck)]
        public ActionResult RemoveDayCharge(string regIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regIds))
                {
                    return Json(JsonResultData.Failure("请指定之前加收日租半日租时的客账"));
                }
                var services = GetService<IResFolioService>();
                //查询出需要作废的日租半日租记录
                var dayCharges = services.DayChargeQuery(CurrentInfo.HotelId, regIds);
                services.DayChargeRemove(CurrentInfo.HotelId, regIds, CurrentInfo.UserCode, CurrentInfo.ShiftId);
                //根据之前查询出来的记录进行记日志
                if (dayCharges.Count > 0)
                {
                    var opLog = new StringBuilder();
                    opLog.Append("作废 ");
                    foreach(var charge in dayCharges)
                    {
                        opLog.AppendFormat("账号{0}房号{1}{2}{3}，"
                            ,charge.regidWithoutHid
                            ,charge.roomNo
                            ,charge.name
                            ,charge.amount
                            );
                    }
                    opLog.AppendFormat("班次：{0}",
                        CurrentInfo.ShiftName
                        );
                    AddOperationLog(OpLogType.日租半日租作废, opLog.ToString(),(regIds.Split(',')[0]));
                }
                return Json(JsonResultData.Successed(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 结账
        /// <summary>
        /// 结账检查
        /// </summary>
        /// <param name="regIds">以逗号分隔的客情id</param>
        /// <param name="transIds">以逗号分隔的客账id</param>
        /// <returns>检查结果</returns>
        [AuthButton(AuthFlag.CheckoutCheck)]
        public ActionResult CheckoutCheck(string regIds, string transIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regIds))
                {
                    return Json(JsonResultData.Failure("请选择要结账的房间"));
                }
                var services = GetService<IResFolioService>();
                var result = services.ResFolioOpCheck(new ResFolioOpPara
                {
                    Hid = CurrentInfo.HotelId,
                    RegIds = regIds,
                    TransIds = transIds,
                    OpType = "checkout_c",
                    InputUser = CurrentInfo.UserName,
                    Shiftid = CurrentInfo.ShiftId
                });
                return Json(JsonResultData.Successed(result));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        /// <summary>
        /// 结账
        /// </summary>
        /// <param name="regIds">以逗号分隔的客情id</param>
        /// <param name="transIds">以逗号分隔的客账id</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.CheckoutCheck)]
        public ActionResult Checkout(string regIds, string transIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regIds))
                {
                    return Json(JsonResultData.Failure("请选择要结账的房间"));
                }
                var outAuthResult = OutOrderOperateFolioAuth(1, regIds); if (outAuthResult != null) { return outAuthResult; }
                var services = GetService<IResFolioService>();
                var result = services.ResFolioOp(new ResFolioOpPara
                {
                    Hid = CurrentInfo.HotelId,
                    RegIds = regIds,
                    TransIds = transIds,
                    OpType = "checkout",
                    InputUser = CurrentInfo.UserName,
                    Shiftid = CurrentInfo.ShiftId
                });
                if (result != null)
                {                    
                    if (!string.IsNullOrWhiteSpace(result.roomno))
                    {
                        result.roomno = result.roomno.Trim();
                        if (result.roomno.EndsWith(","))
                        {
                            result.roomno = result.roomno.Substring(0, result.roomno.Length - 1);
                        }
                        #region 记录房态
                        var rooms = result.roomno.Split(',');
                        var regid = regIds.Split(',');
                        var service = GetService<IRoomStatusService>();
                        var hid = CurrentInfo.HotelId;
                        for (int i = 0; i < rooms.Length; i++)
                        {
                            var roomNo = rooms[i];
                            var roomid = hid + roomNo;
                            var roomstatus= service.GetRoomStatus(hid, roomid);
                            if (roomstatus != null&&regid.Length==rooms.Length) {
                                var dirty = roomstatus.IsDirty == 0 ? "净房" : roomstatus.IsDirty == 1 ? "脏房" : "清洁房";
                                var oldValue = "在住，" + dirty;
                                var newValue = "空房，" + dirty;
                                service.SetRoomStatusLog(roomid, roomNo, "checkOut", oldValue, newValue, regid[i], "");
                            }
                        }
                        #endregion

                        #region 通知OTA离店变更
                        if (regid.Length > 0)
                        {
                            var ResDetail = GetService<IResService>().GetResDetailRegid(CurrentInfo.HotelId, regid[0]);
                            if(ResDetail != null)
                            {
                                var notify = GetService<INotifyService>();
                                notify.NotifyOtaResChanged(CurrentInfo.HotelId, MvcApplication.IsTestEnv, ResDetail.Sourceid,ResDetail.Resid);
                            }
                        }
                        #endregion

                    }
                    AddOperationLog(OpLogType.结账, string.Format("结账 账号：{0}，房号：{3}，消费金额：{1}，付款金额：{2}", (("," + regIds).Replace("," + CurrentInfo.HotelId, ",")).Substring(1), result.amount_d, result.amount_c,result.roomno),(regIds.Split(',')[0]));
                    
                }
                return Json(JsonResultData.Successed(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        /// <summary>
        /// 结账下的入账窗口
        /// </summary>
        /// <param name="resId"></param>
        /// <param name="selectedRegId"></param>
        /// <param name="isCheckout"></param>
        [AuthButton(AuthFlag.CheckoutCheck)]
        public ActionResult AddFolioCheckOut(string resId, string selectedRegId, int isCheckout,string checkoutRegIds)
        {
            return AddFolilIdentical(resId, selectedRegId, isCheckout, checkoutRegIds);
        }
        #endregion

        #region 取消结账
        /// <summary>
        /// 取消结账
        /// </summary>
        /// <param name="billId">取取消结账的结账批次id</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.CancelCheckout)]
        public ActionResult CancelCheckout(string billId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(billId))
                {
                    return Json(JsonResultData.Failure("请选择要取消结账的已结批次"));
                }
                var services = GetService<IResFolioService>();
                var result = services.ResFolioReOp(new ResFolioOpPara
                {
                    Hid = CurrentInfo.HotelId,
                    RegIds = "",
                    TransIds = billId,
                    OpType = "Recheckout",
                    InputUser = CurrentInfo.UserName,
                    Shiftid = CurrentInfo.ShiftId
                });
                if (result != null)
                {  
                    if (!string.IsNullOrWhiteSpace(result.regids))
                    {
                        result.regids = result.regids.Trim();
                        if (result.regids.EndsWith(","))
                        {
                            result.regids = result.regids.Substring(0, result.regids.Length - 1);
                        }
                    }
                    AddOperationLog(OpLogType.取消结账, string.Format("账号：{0}，房号：{3}，消费金额：{1}，付款金额：{2}", result.regids, result.amount_d, result.amount_c,result.roomno),(!string.IsNullOrWhiteSpace(result.regids) ? CurrentInfo.HotelId + result.regids.Split(',')[0] : ""));
                }
                return Json(JsonResultData.Successed(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 取消清账
        /// <summary>
        /// 取消清账
        /// </summary>
        /// <param name="billId">取消清账的结账批次id</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.CancelClear)]
        public ActionResult CancelClear(string billId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(billId))
                {
                    return Json(JsonResultData.Failure("请选择要取消清账的已结批次"));
                }
                var services = GetService<IResFolioService>();
                var result = services.ResFolioReOp(new ResFolioOpPara
                {
                    Hid = CurrentInfo.HotelId,
                    RegIds = "",
                    TransIds = billId,
                    OpType = "reClear",
                    InputUser = CurrentInfo.UserName,
                    Shiftid = CurrentInfo.ShiftId
                });
                if (result != null)
                {
                    var log = new StringBuilder();
                    log.AppendFormat("取消清账");
                    log.AppendFormat(" 账号：{0}", CurrentInfo.GetStringWithoutHotelId(result.regids, joinChar: "/"))
                        .AppendFormat("，房号：{0}", CurrentInfo.GetStringWithoutHotelId(result.roomno, joinChar: "/"))
                        .AppendFormat("，消费金额：{0}", result.amount_d)
                        .AppendFormat("，支付金额：{0}", result.amount_c);
                    var amountClear = result.amount_d - result.amount_c;
                    if (amountClear != 0)
                    {
                        log.AppendFormat("，作废清账金额：{0}", amountClear);
                    }
                    AddOperationLog(OpLogType.取消清账, log.ToString(), (!string.IsNullOrWhiteSpace(result.regids) ? CurrentInfo.HotelId + result.regids.Split(',')[0] : ""));
                }
                return Json(JsonResultData.Successed(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 取消离店
        /// <summary>
        /// 取消离店
        /// </summary>
        /// <param name="regIds">要取消离店的regId，以逗号分隔</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.CancelOut)]
        public ActionResult CancelOut(string regIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regIds))
                {
                    return Json(JsonResultData.Failure("请选择要取消离店的房间"));
                }
                var services = GetService<IResFolioService>();
                var result = services.ResFolioReOp(new ResFolioOpPara
                {
                    Hid = CurrentInfo.HotelId,
                    RegIds = regIds,
                    TransIds = "",
                    OpType = "reout",
                    InputUser = CurrentInfo.UserName,
                    Shiftid = CurrentInfo.ShiftId
                });
                if (result != null)
                {
                    if (!string.IsNullOrWhiteSpace(result.regids))
                    {
                        result.regids = result.regids.Trim();
                        if (result.regids.EndsWith(","))
                        {
                            result.regids = result.regids.Substring(0, result.regids.Length - 1);
                        }
                    }
                    AddOperationLog(OpLogType.取消离店, string.Format("账号：{0}，消费金额：{1}，付款金额：{2}", result.regids, result.amount_d, result.amount_c),(regIds.Split(',')[0]));
                }
                return Json(JsonResultData.Successed(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 迟付
        /// <summary>
        /// 迟付
        /// </summary>
        /// <param name="regIds">以逗号分隔的客情id</param>
        /// <param name="outReason">迟付原因</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Out)]
        public ActionResult Out(string regIds,string outReason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regIds))
                {
                    return Json(JsonResultData.Failure("请选择要迟付的房间"));
                }
                if (string.IsNullOrWhiteSpace(outReason))
                {
                    return Json(JsonResultData.Failure("请输入迟付原因"));
                }
                var services = GetService<IResFolioService>();
                var result = services.ResFolioOp(new ResFolioOpPara
                {
                    Hid = CurrentInfo.HotelId,
                    RegIds = regIds,
                    TransIds = "",
                    OpType = "out",
                    InputUser = CurrentInfo.UserName,
                    Shiftid = CurrentInfo.ShiftId,
                    OutReason = outReason
                });
                if (result != null)
                {
                    AddOperationLog(OpLogType.迟付, string.Format("账号：{0}，消费金额：{1}，付款金额：{2}，迟付原因：{3}", 
                        (("," + regIds).Replace("," + CurrentInfo.HotelId, ",")).Substring(1), 
                        result.amount_d, 
                        result.amount_c,
                        outReason), (regIds.Split(',')[0]));
                    GetService<IoperationLog>().AddResLog(CurrentInfo.HotelId, CurrentInfo.UserName, Common.Extensions.UrlHelperExtension.GetRemoteClientIPAddress(), regIds, 2, "", "", "", "", outReason, CurrentInfo.ShiftId);
                }
                return Json(JsonResultData.Successed(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 清账
        /// <summary>
        /// 清账检查
        /// </summary>
        /// <param name="transIds">以逗号分隔的客账id</param>
        /// <returns>检查结果</returns>
        [AuthButton(AuthFlag.ClearCheck)]
        public ActionResult ClearCheck(string transIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(transIds))
                {
                    return Json(JsonResultData.Failure("请选择要清账的明细账"));
                }
                var outAuthResult = OutOrderOperateFolioAuth(2, transIds); if (outAuthResult != null) { return outAuthResult; }
                var services = GetService<IResFolioService>();
                var result = services.ResFolioOpCheck(new ResFolioOpPara
                {
                    Hid = CurrentInfo.HotelId,
                    RegIds = "",
                    TransIds = transIds,
                    OpType = "clear_c",
                    InputUser = CurrentInfo.UserName,
                    Shiftid = CurrentInfo.ShiftId
                });
                return Json(JsonResultData.Successed(result));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        /// <summary>
        /// 清账
        /// </summary>
        /// <param name="transIds">以逗号分隔的客账明细id</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.ClearCheck)]
        public ActionResult Clear(string transIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(transIds))
                {
                    return Json(JsonResultData.Failure("请选择要清账的客账明细"));
                }
                var outAuthResult = OutOrderOperateFolioAuth(2, transIds); if (outAuthResult != null) { return outAuthResult; }
                var services = GetService<IResFolioService>();
                var result = services.ResFolioOp(new ResFolioOpPara
                {
                    Hid = CurrentInfo.HotelId,
                    RegIds = "",
                    TransIds = transIds,
                    OpType = "clear",
                    InputUser = CurrentInfo.UserName,
                    Shiftid = CurrentInfo.ShiftId
                });
                if (result != null)
                {
                    var log = new StringBuilder();
                    log.Append("清账");
                    log.AppendFormat(" 账号：{0}", CurrentInfo.GetStringWithoutHotelId(result.regids, joinChar: "/"))
                        .AppendFormat("，房号：{0}", CurrentInfo.GetStringWithoutHotelId(result.roomno, joinChar: "/"))
                        .AppendFormat("，消费金额：{0}", result.amount_d)
                        .AppendFormat("，付款金额：{0}", result.amount_c);
                    var amountClear = result.amount_d - result.amount_c;
                    if(amountClear != 0)
                    {
                        log.AppendFormat("，清账金额：{0}", amountClear);
                    }
                    AddOperationLog(OpLogType.清账, log.ToString(), (!string.IsNullOrWhiteSpace(result.regids) ? CurrentInfo.HotelId + result.regids.Split(',')[0] : ""));
                }
                return Json(JsonResultData.Successed(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        /// <summary>
        /// 清账下的入账窗口
        /// </summary>
        /// <param name="resId"></param>
        /// <param name="selectedRegId"></param>
        /// <param name="isCheckout"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.ClearCheck)]
        public ActionResult AddFolioClear(string resId, string selectedRegId, int isCheckout)
        {
            return AddFolilIdentical(resId, selectedRegId, isCheckout);
        }
        #endregion

        #region 预授权
        [AuthButton(AuthFlag.AddCardAuth)]
        public ActionResult AddCardAuth(ResFolioCardAuthAddPara para)
        {
            try
            {
                para.Hid = CurrentInfo.HotelId;
                para.CreateUser = CurrentInfo.UserName;
                var service = GetService<IResFolioService>();
                service.AddCardAuth(para);
                var itemEntity = GetService<IItemService>().Get(para.ItemId);
                AddOperationLog(OpLogType.预授权增加, 
                    string.Format("账号：{0}，付款方式：{1}，卡号：{2}，有效期：{3}，授权码：{4}，授权金额：{5}，授权人：{6}，备注：{7}", 
                    para.RegId.Replace(CurrentInfo.HotelId, ""), 
                    (itemEntity != null) ? itemEntity.Name : para.ItemId.Replace(CurrentInfo.HotelId, ""),
                    para.CardNo,
                    para.ValidDate,
                    para.AuthNo,
                    para.AuthAmount.Value,
                    para.CreateUser,
                    para.Remark), para.RegId);
                return Json(JsonResultData.Successed(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.AddCardAuth)]
        public ActionResult UpdateCardAuth(string originJsonStr,CardAuth update, int isCheckout = 0)
        {
            try
            {
                var outAuthResult = OutOrderOperateFolioAuth(1, update.Regid); if (outAuthResult != null) { return outAuthResult; }
                if(update.Status != 1)
                {
                    //是取消或完成操作
                    update.CompleteUse = CurrentInfo.UserName;
                    update.CompleteDate = DateTime.Now;
                    var hotelService = GetService<IHotelStatusService>();
                    update.CompleteBsnsDate = hotelService.GetBusinessDate(CurrentInfo.HotelId);
                    if(update.Status == 2)
                    {
                        if(!update.BillAmount.HasValue || update.BillAmount <= 0)
                        {
                            return Json(JsonResultData.Failure("完成时必须输入大于0的扣款金额，否则请使用取消功能"));
                        }
                    }
                }
                update.Hid = CurrentInfo.HotelId;
                
                var service = GetService<IResFolioService>();
                var result = service.UpdateCardAuth(originJsonStr, update,CurrentInfo.ShiftId, isCheckout);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        [AuthButton(AuthFlag.AddCardAuth)]
        public ActionResult GetCardAuthIds(string regids)
        {
            List<Guid> cardAuthIds = new List<Guid>();
            if (!string.IsNullOrWhiteSpace(regids))
            {
                List<string> regidList = new List<string>();
                if (regids.Contains(","))
                {
                    regidList = regids.Split(',').ToList();
                }
                else
                {
                    regidList.Add(regids);
                }
                if(regidList.Count > 0)
                {
                    cardAuthIds = GetService<IResFolioService>().GetCardAuthIds(CurrentInfo.HotelId, regidList);
                }
            }
            return Json((cardAuthIds != null && cardAuthIds.Count > 0) ? JsonResultData.Successed(cardAuthIds) : JsonResultData.Failure(""));
        }
        #endregion

        #region 转账
        [AuthButton(AuthFlag.Transfer)]
        public ActionResult Transfer(string transIds,string regId)
        {
            var outAuthResult = OutOrderOperateFolioAuth(2, transIds); if (outAuthResult != null) { return outAuthResult; }
            var services = GetService<IResFolioService>();
            var result = services.Transfer(CurrentInfo, transIds, regId);
            if(result != null && result.Success)
            {
                var entity = result.Data as UpResFolioOpResult;
                if (entity != null)
                {
                    var service = GetService<IResService>();
                    var targetResDetail = service.GetResDetailRegid(CurrentInfo.HotelId, regId);
                    var log = new StringBuilder();
                    log.AppendFormat("转账")
                        .AppendFormat(" 账号：{0}", CurrentInfo.GetStringWithoutHotelId(entity.regids, joinChar: "/"))
                        .AppendFormat("，房号：{0}", CurrentInfo.GetStringWithoutHotelId(entity.roomno, joinChar: "/"))
                        .AppendFormat("，消费：{0}，付款：{1}", entity.amount_d, entity.amount_c)
                        .AppendFormat("转到目标账号：{0}", CurrentInfo.GetStringWithoutHotelId(regId));
                    if(targetResDetail != null)
                    {
                        log.AppendFormat("，目标房号：{0}", targetResDetail.RoomNo);
                    }
                    log.AppendFormat("，班次：{0}", CurrentInfo.ShiftName);
                    AddOperationLog(OpLogType.转账,log.ToString(), regId);
                }
                result.Data = null;
            }
            return Json(result);
        }
        #endregion

        #region 商品录入
        [AuthButton(AuthFlag.Add)]
        public ActionResult AddItems()
        {
            return PartialView("_Additem");
        }
        [AuthButton(AuthFlag.Query)]
        [KendoGridDatasourceException]
        public ActionResult GetItemsParent([DataSourceRequest]DataSourceRequest request)
        {
           var data= GetService<ICodeListService>().GetItemsProducts(CurrentInfo.HotelId);
            return Json(data.ToDataSourceResult(request));
        }

        [AuthButton(AuthFlag.Query)]
        [KendoGridDatasourceException]
        public ActionResult GetItemInfo([DataSourceRequest]DataSourceRequest request,string itemid="all")
        {
            var data = new List<Item>();
            var service = GetService<IItemService>();
            if (itemid == "all")
                data = service.GetItem(CurrentInfo.HotelId, "D");
            else
                data = service.GetCodeListbyitemTypeid(CurrentInfo.HotelId, itemid, "D");
           var result= data.Where(w=>w.IsQuantity==true && (byte)w.Status < (byte)EntityStatus.禁用).OrderBy(b=>b.Seqid).Select(s => new ResFolioAddItemsViewModel {ItemId=s.Id, Id = s.Code, ItemName = s.Name, ItemPrice = s.Price==null?0:s.Price, ItemQty = 0 }).ToList();

            if(result != null && result.Count > 0)
            {
                //消费入账权限控制
                var itemids = GetService<IRoleAuthItemConsumeService>().GetItemConsumeAuth(CurrentInfo.HotelId, CurrentInfo.UserId);
                if(itemids != null && itemids.Count > 0)
                {
                    var removeItemids = itemids.Where(c => c.Value == false).Select(c => c.Key).ToList();
                    if(removeItemids != null && removeItemids.Count > 0)
                    {
                        result.RemoveAll(c => removeItemids.Contains(c.ItemId));
                    }
                }
            }
            return Json(result.ToDataSourceResult(request));
        }
        [AuthButton(AuthFlag.Add)]
        public ActionResult SaveItemsInfo(string itemList, string FolioRegId)
        {
            var outAuthResult = OutOrderOperateFolioAuth(1, FolioRegId); if (outAuthResult != null) { return outAuthResult; }
            var ser = new JavaScriptSerializer();
            var data= ser.Deserialize<List<ResFolioAddItemsViewModel>>(itemList);
            var service = GetService<IResFolioService>();
            var reg = GetService<IResService>().GetResDetailRegid(CurrentInfo.HotelId, FolioRegId);
            StringBuilder bu = new StringBuilder();
            var shiftname = CurrentInfo.ShiftName;
            var shifId = CurrentInfo.ShiftId;
            var userName = CurrentInfo.UserName;

            #region 录入负数权限
            if(data == null || data.Count <= 0) { return Json(JsonResultData.Failure("请录入商品信息！")); }
            if (data.Where(c => c.ItemSumPrice < 0).Count() > 0)
            {
                #region 是否减免冲销权限
                string hid = CurrentInfo.HotelId;
                string userid = CurrentInfo.UserId;
                var authCheckService = GetService<Services.AuthManages.IAuthCheck>();
                string authCode = "";
                long authButtonValue = -1;
                bool isAuthTrue = AuthorizationInfo.GetAuthority(2, out authCode, out authButtonValue);
                bool isAuthAbatement = isAuthTrue ? authCheckService.HasAuth(userid, authCode, authButtonValue, hid) : false;
                if (!isAuthAbatement)
                {
                    return Json(JsonResultData.Failure("没有冲减账务权限，不允许录入负数金额！"));
                }
                #endregion
            }
            #endregion

            foreach (var model in data)
            {
                try
                {

                    var addResult = service.AddFolioDebit(new ResFolioDebitPara
                    {
                        Hid = CurrentInfo.HotelId,
                        RegId = FolioRegId,
                        ItemId = model.ItemId,
                        Quantity = model.ItemQty,
                        Amount = model.ItemSumPrice,
                        UserName = userName,
                        TransShift = shifId
                    });
                    AddOperationLog(OpLogType.商品录入, string.Format("账号：{0}，房号：{1}，商品名：{2}，数量：{3}，金额：{4:F2}，班次：{5}",
                    FolioRegId.Replace(CurrentInfo.HotelId, ""),
                    reg == null ? "" : reg.RoomNo,
                    model.ItemName,
                    model.ItemQty,
                    model.ItemSumPrice,
                    shiftname
                   ), FolioRegId);
                }
                catch (Exception ex)
                {
                    bu.Append(string.Format("{0}：商品录入失败，原因：{1}", model.ItemName, ex.Message));
                }
            }
            if (bu.Length > 0)
                return Json(JsonResultData.Failure(bu.ToString()));
            
            return Json(JsonResultData.Successed());
        }
        #endregion

        #region 账务作废与账务恢复
        /// <summary>
        /// 账务作废与账务恢复
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="transid">账务ID</param>
        /// <param name="shiftid">当前班次ID</param>
        /// <param name="isCheck">是否检查，true只检查，不执行操作。false检查且执行</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.CancelAndRecovery)]
        public ActionResult CancelAndRecoveryFolio(string transid, string reason, bool isCheck)
        {
            //验证参数
            Guid transidG = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(transid))
            {
                Guid.TryParse(transid, out transidG);
            }
            if(transidG == null || transidG == Guid.Empty)
            {
                return Json(JsonResultData.Failure("请勾选账务！"));
            }
            var outAuthResult = OutOrderOperateFolioAuth(2, transid); if (outAuthResult != null) { return outAuthResult; }
            //执行
            string hid = CurrentInfo.HotelId;
            string shiftId = CurrentInfo.ShiftId;
            string shiftName = CurrentInfo.ShiftName;
            var resFolioService = GetService<IResFolioService>();

            var oldEntity = resFolioService.GetEntity(hid, transidG);
            string oldStatus = (oldEntity == null || oldEntity.Status  == null ? "" : oldEntity.Status.ToString());

            var result = resFolioService.CancelAndRecoveryFolio(hid, transid, shiftId, isCheck);
            if (isCheck)
            {
                return Json(result);
            }
            if (result != null && result.Success)
            {
                var newEntity = resFolioService.GetEntity(hid, transidG);
                string newStatus =  (newEntity == null || newEntity.Status == null ? "" : newEntity.Status.ToString());
                resFolioService.AddResFolioLog(hid, CurrentInfo.UserName, transidG, 1, oldStatus, newStatus, shiftName, null, reason, (result.Data == null ? "" : result.Data.ToString()));

                AddOperationLog(OpLogType.账务作废与恢复, string.Format("账务ID：{0}，当前班次：{1}，{2}", transid, shiftName, result.Data), newEntity.Regid);
            }
            return Json(result);
        }
        #endregion

        #region 调账
        [AuthButton(AuthFlag.Query)]
        public ActionResult ListItemForResBillId(string resId)
        {
            if (string.IsNullOrWhiteSpace(resId))
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
            var list = GetService<IResBillSettingService>().ListItemForResBillId(CurrentInfo.HotelId, resId);
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            if(list != null && list.Count > 0)
            {
                foreach(var item in list)
                {
                    result.Add(new KeyValuePair<string, string>(item, item + "账单"));
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.AdjustFolio)]
        public ActionResult AdjustFolio(string resId, Guid[] folioIds, string toResBillCode)
        {
            if (string.IsNullOrWhiteSpace(resId) || folioIds == null || folioIds.Length <= 0 ||  string.IsNullOrWhiteSpace(toResBillCode))
            {
                return Json(JsonResultData.Failure("参数错误！"), JsonRequestBehavior.AllowGet);
            }
            var result = GetService<IResBillSettingService>().AdjustFolio(CurrentInfo.HotelId, resId, folioIds, toResBillCode, CurrentInfo.UserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 退款
        /// <summary>
        /// 获取可退款的账务
        /// </summary>
        /// <param name="regids"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public JsonResult GetRefundFolios(string regids)
        {
            try
            {
                var result = GetService<IResFolioService>().GetRefundFolios(CurrentInfo.HotelId, regids);
                return Json(JsonResultData.Successed(result), JsonRequestBehavior.DenyGet);
            }
            catch(Exception ex)
            {
                return Json(JsonResultData.Failure(ex), JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 给指定的账务记录进行退款
        /// 首先检测是否已经退款，已经退款的则直接给出提示
        /// 再检测支付方式，如果是微信支付和支付宝支付的，则发起相应的退款申请，退款申请失败则直接给出提示
        /// 如果退款成功或者不是微信支付和支付宝支付的，则直接更改充值记录的退款标志，同时插入一笔对应金额的负数来冲减余额
        /// </summary>
        /// <param name="id">要进行退款的账务id</param>
        /// <returns>退款结果</returns>
        [AuthButton(AuthFlag.RefundFolio)]
        [HttpPost]
        public JsonResult Refund(Guid transId, decimal amount)
        {
            //检查参数
            if (transId == null || transId == Guid.Empty)
            {
                return Json(JsonResultData.Failure("请指定要退款的账务！"));
            }
            var outAuthResult = OutOrderOperateFolioAuth(2, transId.ToString()); if (outAuthResult != null) { return outAuthResult; }
            if (amount <= 0)
            {
                return Json(JsonResultData.Failure("请填写要退款的金额！"));
            }
            //检查账务
            ResFolio resFolioEntity = null;
            Item itemEntity = null;
            ResDetail resDetailEntity = null;
            var checkResult = RefundCheck(transId, out resFolioEntity, out itemEntity, out resDetailEntity);
            if (!checkResult.Success)
            {
                return Json(checkResult);
            }
            //检查金额
            if (amount > resFolioEntity.Amount)
            {
                return Json(JsonResultData.Failure("退款金额不能大于入账金额！"));
            }

            string folioItemAction = itemEntity.Action;//付款账务，付款方式，处理方式
            string folioItemActionJsonPara = GetFolioItemActionJsonPara(folioItemAction, resFolioEntity, amount, null);//组装JSON
            try
            {
                var service = GetService<IResFolioService>();
                var payServiceBuilder = GetService<IPayServiceBuilder>();
                var commonDb = GetService<DbCommonContext>();
                var pmsParaService = GetService<IPmsParaService>();

                var commonPayParas = commonDb.M_v_payParas.ToList();
                var hotelPayParas = pmsParaService.GetPmsParas(CurrentInfo.HotelId);

                upProfileCaInputResult addResult;
                IPayService payService = null;
                using (var tc = new TransactionScope())
                {
                    payService = payServiceBuilder.GetPayRefundService(folioItemAction, commonPayParas, hotelPayParas, MvcApplication.IsTestEnv);
                    var payResult = new PayResult { RefNo = "", IsWaitPay = false };
                    if (payService != null)
                    {
                        if (string.IsNullOrWhiteSpace(folioItemActionJsonPara))
                        {
                            return Json(JsonResultData.Failure("参数不能为空"));
                        }
                        payResult = payService.DoPayBeforeSaveFolio(folioItemActionJsonPara);
                    }
                    addResult = service.AddFolioCredit(new ResFolioCreditPara
                    {
                        Hid = CurrentInfo.HotelId,
                        RegId = resFolioEntity.Regid,
                        ItemId = resFolioEntity.Itemid,
                        OriAmount = -amount,
                        Amount = -amount,
                        InvNo = resFolioEntity.InvNo,
                        Remark = resFolioEntity.Remark,
                        UserName = CurrentInfo.UserName,
                        RefNo = payResult.RefNo,
                        IsWaitPay = payResult.IsWaitPay,
                        TransShift = CurrentInfo.ShiftId,
                        Paymentdesc = "",
                        Invalid = true,
                    });
                    tc.Complete();
                }

                var returnResult = new ResFolioAddResult
                {
                    FolioTransId = addResult.Data,
                    Statu = PayStatu.Successed.ToString(),
                    Callback = "",
                    QrCodeUrl = "",
                    QueryTransId = "",
                    DCFlag = resFolioEntity.Dcflag
                };

                var newTransId = Guid.Parse(addResult.Data);

                string logStr = string.Format("退款账务ID：{0}，退款记录账务ID：{1}，退款金额：{2}。", resFolioEntity.Transid.ToString(), newTransId.ToString(), amount);
                AddOperationLog(OpLogType.入账退款, logStr, resFolioEntity.Regid);

                if (payService != null)
                {
                    folioItemActionJsonPara = GetFolioItemActionJsonPara(folioItemAction, resFolioEntity, amount, newTransId);//组装JSON
                    var afterPayResult = payService.DoPayAfterSaveFolio(PayProductType.ResFolio, newTransId.ToString("N"), folioItemActionJsonPara);
                    returnResult.Statu = afterPayResult.Statu.ToString();
                    returnResult.Callback = afterPayResult.Callback;
                    returnResult.QrCodeUrl = afterPayResult.QrCodeUrl;
                    returnResult.QueryTransId = afterPayResult.QueryTransId;
                }

                return Json(JsonResultData.Successed(returnResult));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
                throw;
            }
        }

        /// <summary>
        /// 退款检查
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.RefundFolio)]
        [HttpPost]
        public JsonResult RefundCheck(Guid transId)
        {
            var outAuthResult = OutOrderOperateFolioAuth(2, transId.ToString()); if (outAuthResult != null) { return outAuthResult; }
            //检查账务
            ResFolio resFolioEntity = null;
            Item itemEntity = null;
            ResDetail resDetailEntity = null;
            var checkResult = RefundCheck(transId, out resFolioEntity, out itemEntity, out resDetailEntity);
            return Json(checkResult);
        }

        /// <summary>
        /// 退款检查
        /// </summary>
        /// <param name="id"></param>
        private JsonResultData RefundCheck(Guid transId, out ResFolio resFolioEntity, out Item itemEntity, out ResDetail resDetailEntity)
        {
            //[类型：付款]，[状态：有效未结]，[金额：>0]，[付款方式-处理方式：微信，支付宝]，，，[参考号：必须有值]，[订单状态：未结账]
            resFolioEntity = null;
            itemEntity = null;
            resDetailEntity = null;

            if (transId == null || transId == Guid.Empty)
            {
                return JsonResultData.Failure("请指定要退款的账务！");
            }

            string hid = CurrentInfo.HotelId;

            //1.检查账务 付款类型=C，状态=未结，金额>0
            resFolioEntity = GetService<IResFolioService>().GetEntity(hid, transId);
            if (resFolioEntity == null || string.IsNullOrWhiteSpace(resFolioEntity.Regid))
            {
                return JsonResultData.Failure("退款的账务不存在！");
            }
            if (string.IsNullOrWhiteSpace(resFolioEntity.Dcflag) || !resFolioEntity.Dcflag.Equals("c", StringComparison.OrdinalIgnoreCase))
            {
                return JsonResultData.Failure("退款的账务不属于付款！");
            }
            if (resFolioEntity.Status != 1)
            {
                return JsonResultData.Failure("退款的账务必须是未结状态！");
            }
            if (resFolioEntity.Amount <= 0)
            {
                return JsonResultData.Failure("退款的账务金额必须大于零！");
            }
            if (string.IsNullOrWhiteSpace(resFolioEntity.Itemid))
            {
                return JsonResultData.Failure("退款的账务付款方式不能为空！");
            }

            //2.检查付款方式 类型=C，状态=启用，处理方式=（支付宝刷卡支付、支付宝扫码支付、微信刷卡支付、微信扫码支付）
            itemEntity = GetService<IItemService>().Get(resFolioEntity.Itemid);
            if (itemEntity == null || itemEntity.Hid != hid || string.IsNullOrWhiteSpace(itemEntity.DcFlag) || !itemEntity.DcFlag.Equals("c", StringComparison.OrdinalIgnoreCase) || itemEntity.Status != EntityStatus.启用)
            {
                return JsonResultData.Failure("退款的账务付款方式不存在！");
            }
            if (string.IsNullOrWhiteSpace(itemEntity.Action) || (!itemEntity.Action.Equals("AliBarcode", StringComparison.OrdinalIgnoreCase) && !itemEntity.Action.Equals("AliQrcode", StringComparison.OrdinalIgnoreCase) && !itemEntity.Action.Equals("WxBarcode", StringComparison.OrdinalIgnoreCase) && !itemEntity.Action.Equals("WxQrcode", StringComparison.OrdinalIgnoreCase)))
            {
                return JsonResultData.Failure("退款的账务付款方式处理方式只支持（支付宝刷卡支付、支付宝扫码支付、微信刷卡支付、微信扫码支付）！");
            }
            //3.参考号
            if (string.IsNullOrWhiteSpace(resFolioEntity.RefNo))
            {
                return JsonResultData.Failure("退款的账务参考号不能为空！");
            }
            //4.订单
            resDetailEntity = GetService<IResService>().GetResDetail(hid, resFolioEntity.Regid);
            if (resDetailEntity == null)
            {
                return JsonResultData.Failure("退款的账务所在的订单不存在！");
            }
            if (resDetailEntity.IsSettle == true)
            {
                return JsonResultData.Failure("退款的账务所在的订单已结账！");
            }
            return JsonResultData.Successed();
        }

        /// <summary>
        /// 获取退款JSON字符串
        /// </summary>
        /// <param name="folioItemAction">处理方式</param>
        /// <param name="resFolioEntity">选中要退款的账务</param>
        /// <param name="amount">退款金额</param>
        /// <param name="newTransId">退款操作生成新的退款账务ID</param>
        /// <returns></returns>
        private string GetFolioItemActionJsonPara(string folioItemAction, ResFolio resFolioEntity,  decimal amount, Guid? newTransId)
        {
            string folioItemActionJsonPara = "";
            if (folioItemAction == "AliBarcode" || folioItemAction == "AliQrcode")
            {
                var para = new
                {
                    originPayTransId = resFolioEntity.Transid.ToString("N"),//选中要退款的账务的主键ID
                    refundId = (newTransId != null && newTransId.HasValue) ? newTransId.Value.ToString() : "",//退款操作生成新的退款账务ID
                    refundAmount = amount,//退款金额
                    refundReason = "入账退款",//退款原因
                };
                folioItemActionJsonPara = Newtonsoft.Json.JsonConvert.SerializeObject(para);
            }
            else if (folioItemAction == "WxBarcode" || folioItemAction == "WxQrcode")
            {
                var para = new
                {
                    outTradeNo = resFolioEntity.Transid.ToString("N"),//选中要退款的账务的主键ID
                    outRefundNo = (newTransId != null && newTransId.HasValue) ? newTransId.Value.ToString() : "",//退款操作生成新的退款账务ID
                    totalFee = resFolioEntity.Amount,//原始总金额
                    refundFee = amount,//退款金额
                    opUserId = CurrentInfo.UserName,//操作员
                };
                folioItemActionJsonPara = Newtonsoft.Json.JsonConvert.SerializeObject(para);
            }
            return folioItemActionJsonPara;
        }
        #endregion

        #region 客账减免授权，客账冲销授权-Abatement
        /// <summary>
        /// 提交授权
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult SubmitAuthByAbatement(Gemstar.BSPMS.Hotel.Services.AuthManages.AuthorizationInfo.SubmitAuthInfo submitAuthInfo)
        {
            if (submitAuthInfo.AuthType != 2 && submitAuthInfo.AuthType != 3)//客账减免授权，直接负数入账；客账冲销授权，直接负数入账 并且 作废选中的账务
            {
                return Json(JsonResultData.Failure("参数错误，只接受客账冲销减免授权！"), JsonRequestBehavior.DenyGet);
            }
            var result = GetService<IAuthorizationService>().SubmitAuthorization(CurrentInfo, submitAuthInfo, Weixin.Models.TemplateMessageHelper.SendAuthTemplateMessage);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 获取授权结果
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult GetAuthResultByAbatement(Guid id)
        {
            var result = GetService<IAuthorizationService>().GetAuthorization(CurrentInfo, id);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 减免冲销授权
        /// <summary>
        /// 验证减免冲销授权
        /// </summary>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="adjustPriceOrderList">调价订单信息</param>
        /// <param name="operationSource">操作来源</param>
        /// <returns></returns>
        public JsonResultData CheckAuthAbatement(ResFolioDebitPara model)
        {
            if(model == null || string.IsNullOrWhiteSpace(model.RegId) || string.IsNullOrWhiteSpace(model.ItemId)) { return JsonResultData.Successed(); }
            if(model.Amount >= 0) { return JsonResultData.Successed(); }
            #region 是否减免冲销权限
            string hid = CurrentInfo.HotelId;
            string userid = CurrentInfo.UserId;
            var authCheckService = GetService<Services.AuthManages.IAuthCheck>();
            string authCode = "";
            long authButtonValue = -1;
            bool isAuthTrue = AuthorizationInfo.GetAuthority(2, out authCode, out authButtonValue);
            bool isAuthAbatement = isAuthTrue ? authCheckService.HasAuth(userid, authCode, authButtonValue, hid) : false;
            if (isAuthAbatement)
            {
                return JsonResultData.Successed("有减免冲销权限");
            }
            #endregion
            //转换
            var itemEntity = GetService<IItemService>().Get(model.ItemId);
            var resDetailEntity = GetService<IResService>().GetResDetail(model.Hid, model.RegId);
            if(itemEntity == null || resDetailEntity == null) { return JsonResultData.Successed(); }
            if (!DependencyResolver.Current.GetService<Gemstar.BSPMS.Hotel.Services.IPmsParaService>().isAllowAuthAbatementFolio(hid))
            {
                return JsonResultData.Failure("没有冲减账务权限，不允许录入负数金额！");
            }
            var result = JsonResultData.Failure(new Gemstar.BSPMS.Hotel.Services.ResFolioManage.ResFolioAbatementInfo
            {
                Amount = model.Amount,
                Hid = model.Hid,
                InvNo = model.InvNo,
                ItemId = model.ItemId,
                Quantity = itemEntity.IsQuantity == true ? model.Quantity : null,
                RegId = model.RegId,
                Remark = model.Remark,
                CustomerName = resDetailEntity.Guestname,
                RoomNo = resDetailEntity.RoomNo,
                ItemName = itemEntity.Name,
                ItemCode = itemEntity.Code,
            }, 4);
            AuthorizationInfoConvertToHtml(result, "AuthorizationTemplates/AbatementFolio");
            return result;
        }
        /// <summary>
        /// 获取授权结果，用于确认结果
        /// </summary>
        /// <param name="authIdAndTicks">主键ID+授权时间</param>
        /// <param name="keys">外部关联ID</param>
        /// <returns></returns>
        private bool CheckAuthAbatement(string authIdAndTicks, string keys, out string authUserName,out string reason)
        {
            authUserName = null;reason = "";
            var authCheckService = GetService<Services.AuthManages.IAuthorizationService>();
            var result = authCheckService.CheckAndUpdateAuthorization(CurrentInfo, authIdAndTicks, keys,out reason);
            if(result != null)
            {
                authUserName = result.Data;
                return result.Success;
            }
            return false;
        }
        #endregion

        #region 房租加收修改授权
        /// <summary>
        /// 提交授权
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult SubmitAuthByDayCharge(Gemstar.BSPMS.Hotel.Services.AuthManages.AuthorizationInfo.SubmitAuthInfo submitAuthInfo)
        {
            if (submitAuthInfo.AuthType != 4)
            {
                return Json(JsonResultData.Failure("参数错误，只接受房租加收修改授权！"), JsonRequestBehavior.DenyGet);
            }
            var result = GetService<IAuthorizationService>().SubmitAuthorization(CurrentInfo, submitAuthInfo, Weixin.Models.TemplateMessageHelper.SendAuthTemplateMessage);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 获取授权结果
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult GetAuthResultByDayCharge(Guid id)
        {
            var result = GetService<IAuthorizationService>().GetAuthorization(CurrentInfo, id);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 房租加收修改授权
        /// <summary>
        /// 验证房租加收修改授权
        /// </summary>
        /// <returns></returns>
        public JsonResultData CheckAuthDayCharge(List<upResFolioDayChargeCheckResult> chargeInfos, bool isTemp = false)
        {
            if (isTemp == true) { return JsonResultData.Successed(); }
            if (chargeInfos == null || chargeInfos.Count <= 0) { return JsonResultData.Successed(); }

            #region 是否房租加收修改授权权限
            string hid = CurrentInfo.HotelId;
            string userid = CurrentInfo.UserId;
            var authCheckService = GetService<Services.AuthManages.IAuthCheck>();
            string authCode = "";
            long authButtonValue = -1;
            bool isAuthTrue = AuthorizationInfo.GetAuthority(4, out authCode, out authButtonValue);
            bool isAuthAbatement = isAuthTrue ? authCheckService.HasAuth(userid, authCode, authButtonValue, hid) : false;
            if (isAuthAbatement)
            {
                return JsonResultData.Successed("有房租加收修改权限");
            }
            #endregion
            //转换
            var services = GetService<IResFolioService>();
            var oldChargeInfos = services.DayChargeCheck(CurrentInfo.HotelId, string.Join(",", chargeInfos.Select(c => c.RegId).Distinct()));

            List<ResFolioDayChargeInfo> list = new List<ResFolioDayChargeInfo>();
            foreach (var item in chargeInfos)
            {
                var entity = oldChargeInfos.Where(c => c.RegId == item.RegId).FirstOrDefault();
                if (entity != null)
                {
                    if (entity.Amount != item.Amount || entity.Type != item.Type)
                    {
                        list.Add(new ResFolioDayChargeInfo
                        {
                            RegId = entity.RegId,
                            RoomNo = entity.RoomNo,
                            RoomTypeName = entity.RoomTypeName,
                            GuestName = entity.GuestName,
                            Type = item.Type,
                            Amount = item.Amount,
                            OriginType = entity.Type,
                            OriginAmount = entity.Amount,
                        });
                    }
                }
            }

            if (list != null && list.Count > 0)
            {
                if (!DependencyResolver.Current.GetService<Gemstar.BSPMS.Hotel.Services.IPmsParaService>().isAllowAuthDayChargeFolio(hid))
                {
                    return JsonResultData.Failure("没有房租加收修改权限，不允许修改金额！");
                }
                var result = JsonResultData.Failure(list, 4);
                AuthorizationInfoConvertToHtml(result, "AuthorizationTemplates/DayChargeFolio");
                return result;
            }
            return JsonResultData.Successed();
        }
        /// <summary>
        /// 验证房租加收修改授权
        /// </summary>
        /// <param name="regids"></param>
        /// <returns></returns>
        public JsonResultData CheckAuthDayChargeByFree(string regids)
        {
            #region 是否房租加收修改授权权限
            string hid = CurrentInfo.HotelId;
            string userid = CurrentInfo.UserId;
            var authCheckService = GetService<Services.AuthManages.IAuthCheck>();
            string authCode = "";
            long authButtonValue = -1;
            bool isAuthTrue = AuthorizationInfo.GetAuthority(4, out authCode, out authButtonValue);
            bool isAuthAbatement = isAuthTrue ? authCheckService.HasAuth(userid, authCode, authButtonValue, hid) : false;
            if (isAuthAbatement)
            {
                return JsonResultData.Successed("有房租加收修改权限");
            }
            #endregion

            List<ResFolioDayChargeInfo> list = new List<ResFolioDayChargeInfo>();

            var services = GetService<IResFolioService>();
            var oldChargeInfos = services.DayChargeCheck(CurrentInfo.HotelId, regids);
            if(oldChargeInfos != null && oldChargeInfos.Count > 0)
            {
                foreach(var entity in oldChargeInfos)
                {
                    list.Add(new ResFolioDayChargeInfo
                    {
                        RegId = entity.RegId,
                        RoomNo = entity.RoomNo,
                        RoomTypeName = entity.RoomTypeName,
                        GuestName = entity.GuestName,
                        Type = "免收",
                        Amount = 0,
                        OriginType = entity.Type,
                        OriginAmount = entity.Amount,
                    });
                }
            }

            if (list != null && list.Count > 0)
            {
                if (!DependencyResolver.Current.GetService<Gemstar.BSPMS.Hotel.Services.IPmsParaService>().isAllowAuthDayChargeFolio(hid))
                {
                    return JsonResultData.Failure("没有房租加收修改权限，不允许修改金额！");
                }
                var result = JsonResultData.Failure(list, 4);
                AuthorizationInfoConvertToHtml(result, "AuthorizationTemplates/DayChargeFolio");
                return result;
            }
            return JsonResultData.Successed();

        }

        /// <summary>
        /// 获取授权结果，用于确认结果
        /// </summary>
        /// <param name="authIdAndTicks">主键ID+授权时间</param>
        /// <param name="keys">外部关联ID</param>
        /// <returns></returns>
        private bool CheckAuthDayCharge(string authIdAndTicks, string keys, out string authUserName,out string reason)
        {
            authUserName = null;reason = "";
            var authCheckService = GetService<Services.AuthManages.IAuthorizationService>();
            var result = authCheckService.CheckAndUpdateAuthorization(CurrentInfo, authIdAndTicks, keys,out reason);
            if (result != null)
            {
                authUserName = result.Data;
                return result.Success;
            }
            return false;
        }
        #endregion

        #region 长包房功能
        /// <summary>
        /// 获取预结日期
        /// </summary>
        /// <param name="regIds"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult InAdvanceCheckout_GetEndDate(string regIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regIds))
                {
                    return Json(JsonResultData.Failure("请选择要预结的房间"));
                }
                
                var services = GetService<IResFolioService>();
                var result = services.InAdvanceCheckout_GetEndDate(CurrentInfo, regIds);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 预结
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult InAdvanceCheckout(string regIds, DateTime endDate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regIds))
                {
                    return Json(JsonResultData.Failure("请选择要预结的房间"));
                }
                if (endDate == null || endDate == DateTime.MinValue || endDate == DateTime.MaxValue)
                {
                    return Json(JsonResultData.Failure("请选择预结日期"));
                }
                var services = GetService<IResFolioService>();
                var result = services.InAdvanceCheckout(CurrentInfo, regIds, endDate);
                if (result != null && result.Success)
                {
                    AddOperationLog(OpLogType.入账, string.Format("账号：{0}，预结日期：{1}",
                        (("," + regIds).Replace("," + CurrentInfo.HotelId, ",")).Substring(1),
                        endDate.ToString("yyyy-MM-dd")), (regIds.Split(',')[0]));
                }
                return Json(JsonResultData.Successed(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 水电费入账
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult WaterAndElectricity_AddFolioDebit(List<PermanentRoomFolioPara.WaterAndElectricityFolioPara> list)
        {
            var service = GetService<Services.ResFolioManage.IResFolioService>();
            var checkResult = service.WaterAndElectricity_AddFolioDebit_Check(CurrentInfo, list);
            if (checkResult.Success == false)
            {
                return Json(checkResult, JsonRequestBehavior.DenyGet);
            }

            //保存
            bool isSuccess = true;
            string resultData = "";
            list = list.OrderBy(c => c.RoomNo).ToList();
            foreach (var item in list)
            {
                var addResult = service.AddFolioDebit(new Services.ResFolioManage.ResFolioDebitPara
                {
                    Hid = CurrentInfo.HotelId,
                    RegId = item.Regid,
                    ItemId = item.ItemId,
                    Quantity = item.Quantity,
                    Amount = item.AmountD.Value,
                    InvNo = item.InvNo,
                    Remark = item.Remark + string.Format("[读数：{0}-{1}]", Convert.ToString(item.LastTimeMeterReading), Convert.ToString(item.ThisTimeMeterReading)),
                    UserName = CurrentInfo.UserName,
                    TransShift = CurrentInfo.ShiftId
                });
                if (addResult.Success)
                {
                    resultData += string.Format("账号：{0}，房号：{1}，成功\n", item.Regid.Substring(CurrentInfo.HotelId.Length), item.RoomNo);
                    service.AddResFolioLog(CurrentInfo.HotelId, CurrentInfo.UserName, Guid.Parse(addResult.Data), 2, Convert.ToString(item.LastTimeMeterReading), Convert.ToString(item.ThisTimeMeterReading), item.Regid, item.RoomNo, "", CurrentInfo.ShiftId);
                    AddOperationLog(Gemstar.BSPMS.Common.Services.Enums.OpLogType.入账,
                        string.Format("{9}　账号：{0}，房号：{8}，{7}：{1}，数量：{2}，金额：{3:F2}，单号：{4}，备注：{5}，班次：{6}",
                        item.Regid.Replace(CurrentInfo.HotelId, ""),
                        item.ItemName,
                        item.Quantity,
                        item.AmountD,
                        item.InvNo,
                        item.Remark,
                        CurrentInfo.ShiftName,
                        "消费项目",
                        item.RoomNo,
                        "入账-消费"), item.Regid);
                }
                else
                {
                    isSuccess = false;
                    resultData += string.Format("账号：{0}，房号：{1}，失败，原因：{2}\n", item.Regid.Substring(CurrentInfo.HotelId.Length), item.RoomNo, addResult.Data);
                }
            }

            return Json((isSuccess ? JsonResultData.Successed(resultData) : JsonResultData.Failure(resultData)), JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 拆账
        /// <summary>
        /// 拆账
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Details)]
        public ActionResult SplitFolio(ResFolioSplitInfo.Para para)
        {
            try
            {
                var result = GetService<IResFolioService>().SplitFolio(CurrentInfo, para);
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {
                return Json(JsonResultData.Failure("账务已被修改，请获取账务最新内容！"), JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex), JsonRequestBehavior.DenyGet);
            }
        }
        #endregion

        #region 迟付账务处理权限
        /// <summary>
        /// 是否有操作 订单[迟付状态的订单]内账务的权限
        /// </summary>
        /// <param name="type">参数类型（1：regId 或 regIds，2：transId 或 transIds）</param>
        /// <param name="value">参数值</param>
        private JsonResult OutOrderOperateFolioAuth(int type, string value)
        {
            try
            {
                if (type != 1 && type != 2) { return null; }
                if (string.IsNullOrWhiteSpace(value)) { return null; }
                bool isOut = GetService<IResFolioService>().IsOutStatusOrder(CurrentInfo.HotelId, type, value);
                if (isOut)
                {
                    string authCode = "2002020";
                    long authButtonValue = (Int64)AuthFlag.Inspect;
                    bool isAuth = GetService<Services.AuthManages.IAuthCheck>().HasAuth(CurrentInfo.UserId, authCode, authButtonValue, CurrentInfo.HotelId);
                    if (!isAuth)
                    {
                        return Json(JsonResultData.Failure("没有迟付订单的操作权限！"));
                    }
                }
            }
            catch { }
            return null;
        }
        #endregion
    }
}