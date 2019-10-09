using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Services.ReportManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosGuestQuery;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosTabStatus;
using Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos迟付结账
    /// </summary>
    [AuthPage(ProductType.Pos, "p200013")]
    public class PosDelayedPaymentController : BaseController
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            ViewBag.Version = CurrentVersion;
            return View();
        }

        /// <summary>
        /// 收银账单列表视图
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PosBillList(string tabNo)
        {
            var refeService = GetService<IPosRefeService>();
            var refe = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
            var list = new List<up_pos_list_billByPosidResult>();
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);
            if (refe != null && refe.Count > 0)
            {
                var service = GetService<IPosBillService>();
                list = service.GetPosBillDelayedPayment(CurrentInfo.HotelId, CurrentInfo.PosId, null, tabNo);
            }

            return PartialView("_PosBillList", list);
        }

        /// <summary>
        /// 查询窗口
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _QueryBillHtml()
        {
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var model = new Models.PosGuestQuery.QueryBillModel();
            model.PosId = CurrentInfo.PosId;
            model.BillBsnsDate = pos.Business;
            return PartialView("_QueryBillHtml", model);
        }

        /// <summary>
        /// 收银账单列表视图
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult GetPosBillList(QueryDelayedModel model)
        {
            var list = new List<up_pos_list_billByPosidResult>();
            if (model != null && !string.IsNullOrWhiteSpace(model.BillBsnsDate.ToString()))
            {
                var service = GetService<IPosBillService>();
                list = service.GetPosBillDelayedPayment(CurrentInfo.HotelId, model.PosId, model.BillBsnsDate, model.tabNo ?? "");
            }
            return PartialView("_PosBillList", list);
        }

        #region 打印账单

        [HttpPost]
        [AuthButton(AuthFlag.Export)]
        public ActionResult AddQueryParaTemp(ReportQueryModel model, string print, string Flag)
        {
            PosCommon common = new PosCommon();
            var controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            return common.AddQueryParaTemp(model, print, Flag,controller);
        }

        #endregion 打印账单

        #region 取消打单

        [AuthButton(AuthFlag.Print)]
        public ActionResult CancelPrint(string billId)
        {
            var service = GetService<IPosBillService>();
            var oldEntity = service.Get(billId);
            if (oldEntity == null)
            {
                return Json(JsonResultData.Failure("要操作的数据不存在！"));
            }
            var newEntity = new PosBill();
            AutoSetValueHelper.SetValues(oldEntity, newEntity);
            newEntity.IPrint = 0;   //打单次数修改成0
            service.Update(newEntity, oldEntity);
            service.AddDataChangeLog(OpLogType.Pos账单修改);

            service.Commit();

            return Json(JsonResultData.Successed());
        }

        #endregion 取消打单

        #region 账单预览

        [HttpPost]
        [AuthButton(AuthFlag.Details)]
        public ActionResult AddQueryParaTempA(ReportQueryModel model, string print, string Flag)
        {
            PosCommon common = new PosCommon();
            var controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            return common.AddQueryParaTemp(model, print, Flag,controller);
        }

        #endregion 账单预览

        #region 买单验证

        /// <summary>
        /// 买单之前验证餐台是否被操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Members)]
        public ActionResult PayCheck(PosTabLogAddViewModel model)
        {
            var result = CheckTabLog(model);
            return result;
        }

        /// <summary>
        /// 通用方法验证餐台是否被占用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private JsonResult CheckTabLog(PosTabLogAddViewModel model)
        {
            try
            {
                string computer = model.ComputerName ?? "";
                if (!string.IsNullOrWhiteSpace(model.Tabid) && string.IsNullOrWhiteSpace(model.Billid))
                {
                    var tabStatusService = GetService<IPosTabStatusService>();
                    var tabStatus = tabStatusService.Get(model.Tabid);

                    var tabLogService = GetService<IPosTabLogService>();
                    var tabLog = tabLogService.GetPosTabLogByTab(CurrentInfo.HotelId, tabStatus.Refeid, model.Tabid, model.TabNo);

                    var tabBillService = GetService<IPosBillService>();
                    var tabBill = tabBillService.GetPosBillByTabId(CurrentInfo.HotelId, tabStatus.Refeid, model.Tabid);

                    //如果已经锁台并且成功开台
                    if (tabLog != null && tabBill != null && tabStatus.TabStatus == (byte)PosTabStatusEnum.就座)
                    {
                        var pmsParaService = GetService<IPmsParaService>();
                        //是否可以多人操作餐台
                        var isMultiPalyer = pmsParaService.GetValue(CurrentInfo.HotelId, "tabMultiPlayer");
                        if (isMultiPalyer == "1") //可以多人操作
                        {
                            return Json(JsonResultData.Failure(tabLog.Msg + "，是否继续？", 2));
                        }
                        else
                        {
                            return Json(JsonResultData.Failure(model.TabNo + "餐台有人在操作，请您稍后再操作！", 3));
                        }
                    }

                    return Json(JsonResultData.Successed(tabBill));
                }
                else if (!string.IsNullOrWhiteSpace(model.Billid))
                {
                    var tabBillService = GetService<IPosBillService>();
                    var tabBill = tabBillService.Get(model.Billid);

                    var tabLogService = GetService<IPosTabLogService>();
                    //根据账单ID获取锁台记录
                    var tabLog = tabLogService.GetPosTabLogByBillId(CurrentInfo.HotelId, model.Billid);

                    //如果已经锁台并且成功开台
                    if (tabLog != null && tabBill != null && tabBill.Status == (byte)PosBillStatus.开台)
                    {
                        var pmsParaService = GetService<IPmsParaService>();
                        //s是否可以多人操作餐台
                        var isMultiPalyer = pmsParaService.GetValue(CurrentInfo.HotelId, "tabMultiPlayer");
                        if (isMultiPalyer == "1") //可以多人操作
                        {
                            return Json(JsonResultData.Failure(tabLog.Msg + "，是否继续？", 2));
                        }
                        else
                        {
                            return Json(JsonResultData.Failure(model.TabNo + "餐台有人在操作，请您稍后再操作！", 3));
                        }
                    }

                    return Json(JsonResultData.Successed(tabBill));
                }

                return Json(JsonResultData.Failure(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        #endregion 买单验证

        #region 折扣权限验证

        [AuthButton(AuthFlag.None)]
        public ActionResult _PosBillDetailed(string posBillId, string openFlag)
        {
            ViewBag.PosBillId = posBillId;
            ViewBag.openFlag = openFlag;
            if (openFlag == "B")
            {
                return PartialView("_PosBillDetailed_B");
            }
            else
            {
                return PartialView("_PosBillDetailed");
            }
        }

        #endregion 折扣权限验证

        #region 取消折扣验证

        [AuthButton(AuthFlag.Reset)]
        public ActionResult CheckEditForBillRefe(string BillId)
        {
            var billService = GetService<IPosBillService>();
            var model = billService.Get(BillId);
            if (model != null)
            {
                var posRefeService = GetService<IPosRefeService>();

                var posRefe = posRefeService.Get(model.Refeid);//获取账单营业点ID

                if (posRefe.IsPrintEdit == false && (model.IPrint != null && model.IPrint > 0))
                {
                    return Json(JsonResultData.Failure("打单之后不可以修改！"));
                }

                return Json(JsonResultData.Successed(""));
            }
            else
            {
                return Json(JsonResultData.Failure("账单不存在，请重试"));
            }
        }
        #endregion

        /// <summary>
        /// 买单之前验证营业点设置的 买单未打单条件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult CheckRefeITagPrintBill(PaymentViewModel model)
        {
            PosCommon common = new PosCommon();
            return common.CheckRefeITagPrintBill(model);
        }

        /// <summary>
        /// 获取买单信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [AuthButton(AuthFlag.None)]
        public PartialViewResult _Payment(PaymentViewModel model)
        {
            PosCommon common = new PosCommon();
            return common._Payment(model);
        }


        /// <summary>
        /// 支付宝、微信支付成功
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public void OtherPaymentSuccess(string billid)
        {

            var billService = GetService<IPosBillService>();
            var bill = billService.Get(billid);
            if (bill != null && bill.Status == (byte)PosBillStatus.开台)
            {
                var refeService = GetService<IPosRefeService>();
                var refe = refeService.Get(bill.Refeid);

                var posService = GetService<IPosPosService>();
                var pos = posService.Get(CurrentInfo.PosId);

                if (refe.Isclrtab == true)
                {
                    //修改账单状态
                    bill.DepBsnsDate = pos.Business;
                    bill.MoveUser = CurrentInfo.UserName;
                    bill.DepDate = DateTime.Now;
                    bill.Status = (byte)PosBillStatus.清台;

                    billService.Update(bill, new PosBill());
                    billService.AddDataChangeLog(OpLogType.Pos账单修改);
                    billService.Commit();
                }
                else if (refe.Isclrtab == false)
                {
                    //修改账单状态
                    bill.DepBsnsDate = pos.Business;
                    bill.MoveUser = CurrentInfo.UserName;
                    bill.DepDate = DateTime.Now;
                    if (bill.TabFlag == (byte)PosBillTabFlag.物理台)
                    {
                        bill.Status = (byte)PosBillStatus.结账;
                    }
                    else
                    {
                        bill.Status = (byte)PosBillStatus.清台;
                    }
                    billService.Update(bill, new PosBill());
                    billService.AddDataChangeLog(OpLogType.Pos账单修改);
                    billService.Commit();
                }

                //修改餐台状态
                PosCommon common = new PosCommon();
                common.SetTabStatus(bill);

                //调用电子发票接口 
                common.AddEInvoice(bill.Billid);

                //清理锁台记录
                var tabLogService = GetService<IPosTabLogService>();
                var tabLogList = tabLogService.GetPosTabLogListByTab(CurrentInfo.HotelId, bill.Refeid, bill.Tabid, bill.TabNo);
                if (tabLogList != null && tabLogList.Count > 0)
                {
                    foreach (var tabLog in tabLogList)
                    {
                        if (tabLog.Billid == bill.Billid)
                        {
                            tabLogService.Delete(tabLog);
                            tabLogService.AddDataChangeLog(OpLogType.Pos锁台删除);
                            tabLogService.Commit();
                        }
                    }
                }
            }
        }


        [AuthButton(AuthFlag.None)]
        public ActionResult AddQueryParaTempByPaySusses(ReportQueryModel model, string print, string Flag, string isPrintBill)
        {
            try
            {
                if (model != null && !string.IsNullOrWhiteSpace(model.ReportCode))
                {
                    if (Flag == "A")//区分打单跟打印预览
                    {
                        var ParameterValuesList = model.ParameterValues.Split('@');
                        var billid = "0";
                        foreach (var billIdArr in ParameterValuesList)
                        {
                            //字符串分割得到账单ID
                            if (!string.IsNullOrWhiteSpace(billIdArr) && billIdArr.IndexOf("&") == -1)
                            {
                                billid = billIdArr.Split('=')[1];
                                break;
                            }
                        }
                        var BillService = GetService<IPosBillService>();
                        var billModel = BillService.Get(billid);
                        if (billModel != null)
                        {
                            var refeService = GetService<IPosRefeService>();    //营业点

                            PosRefe refe = refeService.Get(billModel.Refeid);   //获取营业点
                            if (refe.ITagPrintBill == PosTagPrintBill.提示是否买单 || (refe.ITagPrintBill == PosTagPrintBill.立即打印账单)
        || (refe.ITagPrintBill == PosTagPrintBill.提示是否打印账单 && isPrintBill == "1"))
                            {
                                billModel.IPrint = billModel.IPrint == null ? 1 : billModel.IPrint + 1;
                                BillService.Update(billModel, new PosBill());
                                BillService.AddDataChangeLog(OpLogType.Pos账单修改);
                                BillService.Commit();

                                if (model.ParameterValues.IndexOf("@hid") == -1)
                                {
                                    model.ParameterValues = "@h99hid=" + CurrentInfo.HotelId + "&" + model.ParameterValues;
                                }

                                var serializer = new JavaScriptSerializer();
                                string value = ReplaceJsonDateToDateString(serializer.Serialize(model));
                                Guid? id = GetService<IReportService>().AddQueryParaTemp(CurrentInfo.HotelId, value);
                                if (id != null)
                                {
                                    var url = new StringBuilder();
                                    url.Append("http://").Append(Request.Url.Host).Append("/ReportManage");
                                    url.Append("/SRBillReportView/Index")
                                        .Append("?ReportCode=").Append(Server.UrlEncode(model.ReportCode))
                                        .Append("&ParameterValues=").Append(Server.UrlEncode(id.Value.ToString()))
                                        .Append("&ChineseName=").Append(Server.UrlEncode(model.ChineseName));
                                    if (!string.IsNullOrWhiteSpace(print))
                                    {
                                        url.Append("&print=").Append(Server.UrlEncode(print));
                                    }
                                    //账单明细数据
                                    var billDetailService = GetService<IPosBillDetailService>();
                                    var ListDetail = billDetailService.GetBillDetailByPrint(CurrentInfo.HotelId, billModel.Billid, billModel.MBillid);

                                    var valueStr = ReplaceJsonDateToDateString(serializer.Serialize(ListDetail));

                                    var result = new
                                    {
                                        url = url.ToString(),
                                        ListDetail = valueStr
                                    };
                                    return Json(JsonResultData.Successed(result));
                                }
                                else
                                {
                                    return Json(JsonResultData.Failure("账单打印失败！"));
                                }
                            }
                            else
                            {
                                return Json(JsonResultData.Failure("账单打印失败！"));
                            }
                        }
                        else
                        {
                            return Json(JsonResultData.Failure("账单打印失败！"));
                        }
                    }
                    else
                    {
                        return Json(JsonResultData.Failure("账单打印失败！"));
                    }
                }
                else
                {
                    return Json(JsonResultData.Failure("账单打印失败！"));
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

    }
}
