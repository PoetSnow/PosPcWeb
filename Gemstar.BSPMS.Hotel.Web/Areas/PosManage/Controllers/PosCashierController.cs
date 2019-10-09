using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF.PayManage;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Services.ResFolioManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosCashier;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosTabStatus;
using Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos单位定义
    /// </summary>
    [AuthPage(ProductType.Pos, "p20001")]
    public class PosCashierController : BaseController
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            //提示信息显示时间
            var pmsParaService = GetService<IPmsParaService>();
            ViewBag.TipsTime = pmsParaService.GetValue(CurrentInfo.HotelId, "tipsTime");
            ViewBag.UserName = CurrentInfo.UserName;    //当前操作员
            ViewBag.HotelName = CurrentInfo.HotelName;    //当前酒店名称（本地报表打印使用）
            //收银自动刷新间隔时间
            ViewBag.PosCashierRefreshInterval = pmsParaService.GetValue(CurrentInfo.HotelId, "posCashierRefreshInterval");
            ViewBag.Version = CurrentVersion;

            //获取系统参数，
            var paraservice = GetService<IPmsParaService>();
            ViewBag.IsPayOrderAgain = paraservice.IsPayOrderAgain(CurrentInfo.HotelId) ? 1 : 0;

            return View();
        }

        /// <summary>
        /// 添加付款项
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult AddPayment(PaymentViewModel model)
        {
            if (model != null && model.Billid != null && model.Itemid != null)
            {
                try
                {
                    var billService = GetService<IPosBillService>();
                    var bill = billService.Get(model.Billid);

                    var refeService = GetService<IPosRefeService>();
                    var refe = refeService.Get(bill.Refeid);

                    var posService = GetService<IPosPosService>();
                    var pos = posService.Get(CurrentInfo.PosId);

                    var itemService = GetService<IPosItemService>();
                    var item = itemService.GetEntity(CurrentInfo.HotelId, model.Itemid);

                    //当前支付方式，支付方式为不计入收入，则按各个菜式占总金额比例分摊各个菜式的公司账金额                    
                    if (item.IsInCome == false)
                    {
                        var PosBillDetailService = GetService<IPosBillDetailService>();
                        PosBillDetailService.AverageOutComeAmount(CurrentInfo.HotelId, model.Billid, decimal.Parse(model.Amount.ToString()), decimal.Parse(item.Rate.ToString()));
                    }

                    //修改收银备注
                    var paymentService = GetService<IPaymentServices>();
                    paymentService.ChangeCashMemo(model);

                    //处理各种付款处理动作
                    var payActionXmlBuilder = GetService<PayActionXmlBuilder>();
                    var payActionXmlHandler = payActionXmlBuilder.Build(model.FolioItemAction);

                    //获取当前付款方式信息
                    var payment = itemService.GetPosPayType(item.PayType);
                    if (payment == null)
                    {
                        return Json(JsonResultData.Failure("付款方式未找到"));
                    }

                    //根据付款方式获取支付信息记录
                    var otherpayinfo = JsonHelper.SerializeObject<OtherSysPayInfo>(model.CttName);
                    model.CttName = GetPayInfoByPayMethod(otherpayinfo, model, model.FolioItemAction);

                    //是否打印会员结账单
                    var paraservice = GetService<IPmsParaService>();
                    //付款方式为会员付款记录会员支付前会员金额
                    var IsMacrdPay = IsMcardPay(model.FolioItemAction) && paraservice.IsPrintMemberBill(CurrentInfo.HotelId);
                    MbrCardInfoModel oldmbrCardInfo = null;
                    if (IsMacrdPay)
                    {
                        var res = GetService<IPosItemClassDiscountService>().GetMbrCardInfoByCardID(CurrentInfo.HotelId, otherpayinfo.MbrCardNo, "");
                        if (!res.Success)
                        {

                            return Json(JsonResultData.Failure("会员信息查询失败"));
                        }
                        oldmbrCardInfo = (MbrCardInfoModel)res.Data;
                    }

                    //开始增加账务
                    Guid? settleid = null;
                    if (Session["settleid"] == null)
                    {
                        if (!string.IsNullOrWhiteSpace(model.ProfileId))
                        {
                            settleid = Guid.Parse(model.ProfileId);
                        }
                        else
                        {
                            settleid = Guid.NewGuid();
                            Session["settleid"] = settleid;
                        }
                    }
                    else
                    {
                        settleid = Guid.Parse(Session["settleid"].ToString());
                    }

                    string settleTransno = null;
                    if (item.PayType.Equals("AliBarcode", StringComparison.OrdinalIgnoreCase) || item.PayType.Equals("AliQrcode", StringComparison.OrdinalIgnoreCase) || item.PayType.Equals("WxBarcode", StringComparison.OrdinalIgnoreCase) || item.PayType.Equals("WxQrcode", StringComparison.OrdinalIgnoreCase))
                    {
                        settleTransno = Guid.NewGuid().ToString("N");
                    }

                    //获取收银点对应的捷云营业点代码
                    var posOutletCode = string.IsNullOrWhiteSpace(pos.CodeIn) ? pos.Code : pos.CodeIn;
                    if (!string.IsNullOrWhiteSpace(item.CodeIn))
                    {
                        posOutletCode = item.CodeIn;
                    }
                    else if (!string.IsNullOrWhiteSpace(refe.CodeIn))
                    {
                        posOutletCode = refe.CodeIn;
                    }

                    var businessPara = new PaymentOperateBusinessPara
                    {
                        Hid = CurrentInfo.HotelId,
                        UserName = CurrentInfo.UserName,
                        PosId = CurrentInfo.PosId,
                        PosName = CurrentInfo.PosName,
                        Billid = model.Billid,
                        Refeid = bill.Refeid,
                        Tabid = bill.Tabid,

                        //账单明细
                        IsCheck = 1,
                        SettleId = settleid,
                        SettleBsnsDate = pos.Business,
                        SettleShuffleid = refe.ShuffleId,
                        SettleShiftId = pos.ShiftId,
                        SettleUser = CurrentInfo.UserName,
                        SettleTransNo = settleTransno,
                        SettleTransName = model.CttName,

                        //餐台状态
                        TabStatus = refe.Isclrtab == true ? (byte)PosTabStatusEnum.空净 : (byte)PosTabStatusEnum.已买单未离座,
                        OpTabid = "",

                        //账单
                        DepBsnsDate = pos.Business,
                        MoveUser = CurrentInfo.UserName,
                        Status = refe.Isclrtab == true ? (byte)PosBillStatus.清台 : (byte)PosBillStatus.结账,
                        WaitStatus = 1,
                        BillDetailStatus = (byte)PosBillDetailStatus.正常,
                        Isauto = (byte)PosBillDetailIsauto.付款,

                        //挂账
                        PosOutlteCode = posOutletCode
                    };

                    var actionCheckResult = payActionXmlHandler.DoCheck(model, businessPara);
                    //如果检查没有通过，则表示不能处理账务，直接返回错误
                    if (!actionCheckResult.Success)
                    {
                        return Json(actionCheckResult);
                    }
                    businessPara.SettleId = settleid.Value;
                    var detailStatu = payActionXmlHandler.DefaultDetailStatus;
                    var addBillResult = paymentService.AddBill(model, businessPara, detailStatu);
                    businessPara.AddedBillResult = addBillResult;
                    //处理付款处理动作的实际处理
                    var actionResult = payActionXmlHandler.DoOperate(model, businessPara);
                    if (!actionResult.Success)
                    {
                        return Json(actionResult);
                    }
                    //接口调用成功，如果默认状态不是正常状态的，则需要处理账务状态
                    if (detailStatu != PosBillDetailStatus.正常)
                    {
                        paymentService.ChangeBillStatuWhenSuccess(model, businessPara);
                    }
                    //检查是否买单完成，完成则处理结账信息，差额，尾数，找赎,清台等
                    if (paymentService.IsAllPaid(model, businessPara))
                    {
                        //添加消费项目的结账信息
                        paymentService.Settle(model, businessPara);
                        //添加尾数抹零处理
                        paymentService.TailAmount(model, businessPara);
                        //找赎
                        paymentService.SmallChange(model, businessPara);
                        //清台
                        paymentService.ClearTable(model, businessPara);
                        //TODO:请胡工审核一下代码，并且测试功能，测试通过后请移除不需要的以前的那些处理代码
                    }

                    Session["settleid"] = null;
                    //保存账单数据到redis
                    var common = GetService<PosCommon>();
                    common.SetRedisBill(model.Billid);

                    //付款方式为会员付款记录会员支付后会员金额
                    MbrCardInfoModel newmbrCardInfo;
                    if (IsMacrdPay)
                    {
                        var res = GetService<IPosItemClassDiscountService>().GetMbrCardInfoByCardID(CurrentInfo.HotelId, otherpayinfo.MbrCardNo, "");
                        if (!res.Success)
                        {

                            return Json(JsonResultData.Failure("会员信息查询失败"));
                        }
                        newmbrCardInfo = (MbrCardInfoModel)res.Data;

                        //写入Cookie ，供本地查询使用
                        //"@@mbillid=" + data["mbillid"] + "&@@detailid=" + data["detailid"] + "@@mcardtype=" + data["mcardtype"] + "@@oldBaseAmtBalance=" + data["oldBaseAmtBalance"] + "@@oldIncamount=" + data["oldIncamount"] + "@@BaseAmtBalance=" + data["BaseAmtBalance"] + "@@Incamount=" + data["Incamount"] + "",
                        var _paydatta = new { mbillid = model.MBillid, detailid = addBillResult.DetailId, mcardtype = newmbrCardInfo.MbrCardTypeName, oldBaseAmtBalance = oldmbrCardInfo.BaseAmtBalance, oldIncamount = oldmbrCardInfo.Incamount, BaseAmtBalance = newmbrCardInfo.BaseAmtBalance, Incamount = newmbrCardInfo.Incamount };

                        var cookie = new HttpCookie($"McardPayInfo" + model.MBillid + model.SendIndex, JsonHelper.SerializeObject(_paydatta));
                        cookie.Expires = DateTime.Now.AddMinutes(1);
                        Response.Cookies.Add(cookie);
                    }


                    return Json(actionResult);
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }

            return Json(JsonResultData.Failure(""));
        }


        //根据付款方式记录其他系统支付信息（ 会员卡记录 卡号;名称;余额; 房账：客人名;房号 合约单位:单位名;签单人  ）
        private string GetPayInfoByPayMethod(OtherSysPayInfo PayInfo, PaymentOperatePara model, string action)
        {
            var payinfostr = "";

            if (action.Equals("house", StringComparison.OrdinalIgnoreCase))
            {
                payinfostr = $"{PayInfo.labelRoom};{PayInfo.roomNo}";
            }
            if (action.Equals("mbrCard", StringComparison.OrdinalIgnoreCase))
            {
                var _posItemService = GetService<IPosItemService>();
                var item = _posItemService.Get(model.Itemid);
                model.Amount = model.Amount * Convert.ToDecimal(item.Rate ?? 1);
                payinfostr = $"{PayInfo.MbrCardNo};{PayInfo.GuestCName};{ string.Format("{0:N2}", PayInfo.lblBalance - model.Amount)}";
            }
            if (action.Equals("mbrLargess", StringComparison.OrdinalIgnoreCase))
            {
                var _posItemService = GetService<IPosItemService>();
                var item = _posItemService.Get(model.Itemid);
                model.Amount = model.Amount * Convert.ToDecimal(item.Rate ?? 1);
                payinfostr = $"{PayInfo.MbrCardNo};{PayInfo.GuestCName};{string.Format("{0:N2}", PayInfo.lblBalance - model.Amount)}";
            }
            if (action.Equals("mbrCardAndLargess", StringComparison.OrdinalIgnoreCase))
            {
                var _posItemService = GetService<IPosItemService>();
                var item = _posItemService.Get(model.Itemid);
                model.Amount = model.Amount * Convert.ToDecimal(item.Rate ?? 1);
                payinfostr = $"{PayInfo.MbrCardNo};{PayInfo.GuestCName};{string.Format("{0:N2}", PayInfo.lblBalance - model.Amount)}";
            }
            if (action.Equals("corp", StringComparison.OrdinalIgnoreCase))
            {
                payinfostr = $"{PayInfo.CorpAutoComplete};{PayInfo.CorpSignPerson}";
            }
            return payinfostr;

        }

        //验证是否会员付款方式
        private bool IsMcardPay(string action)
        {
            if (action.Equals("mbrCard", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (action.Equals("mbrLargess", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (action.Equals("mbrCardAndLargess", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查房号是否允许挂房帐
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public bool CheckRoomAccount(string roomNo)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(roomNo))
                {
                    var service = GetService<IPosBillDetailService>();
                    string xmlStr = "<?xml version='1.0' encoding='gbk' ?>"
                        + "<RealOperate>"
                            + "<XType>" + "JxdBSPms" + "</XType>"
                            + "<OpType>" + "房账客人资料查询" + "</OpType>"
                            + "<RoomFolio>"
                                + "<hid>" + CurrentInfo.HotelId + "</hid>"
                                + "<roomNo>" + roomNo + "</roomNo>"
                                + "<guestCName></guestCName>"
                                + "<Regid></Regid>"
                                + "<Outlet></Outlet>"
                            + "</RoomFolio>"
                        + "</RealOperate> ";
                    var xmlInfo = service.RealOperate(CurrentInfo.HotelId, "", xmlStr);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlInfo);
                    Dictionary<string, string> xmls = new Dictionary<string, string>();
                    if (doc != null)
                    {
                        if (doc["RoomFolio"] != null)
                        {
                            if (doc["RoomFolio"]["Rows"] != null)
                            {
                                if (doc["RoomFolio"]["Rows"]["Row"] != null)
                                {
                                    foreach (XmlNode node in doc["RoomFolio"]["Rows"]["Row"])
                                    {
                                        if (node != null && node.Name != null && node.FirstChild != null)
                                        {
                                            if (node != null && node.Name != null && node.FirstChild != null)
                                            {
                                                if (node.Name.Equals("isTransfer", StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    if (Convert.ToInt32(doc["RoomFolio"]["Rows"]["Row"]["isTransfer"].FirstChild.Value) == 1)
                                                    {
                                                        return true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
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

            var oldbill = new PosBill();
            AutoSetValueHelper.SetValues(bill, oldbill);

            //判断已买单金额是否跟总金额平账
            var billDetailService = GetService<IPosBillDetailService>();
            var sumAmount = billDetailService.GetBillDetailAmountSumByBillid(CurrentInfo.HotelId, billid);  //应付金额

            var isPayAmount = billDetailService.GetIsPayAmount(CurrentInfo.HotelId, billid);  //已付金额
            if (sumAmount - isPayAmount <= 0)
            {
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
                        //  billService.AddDataChangeLog(OpLogType.Pos账单修改);
                        billService.Commit();
                        AddOperationLog(OpLogType.Pos账单修改, "买单：" + Enum.GetName(typeof(PosBillStatus), oldbill.Status) + "-->" + Enum.GetName(typeof(PosBillStatus), bill.Status), bill.BillNo);
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
                        //   billService.AddDataChangeLog(OpLogType.Pos账单修改);
                        billService.Commit();
                        AddOperationLog(OpLogType.Pos账单修改, "买单：" + Enum.GetName(typeof(PosBillStatus), oldbill.Status) + "-->" + Enum.GetName(typeof(PosBillStatus), bill.Status), bill.BillNo);
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

        }

        /// <summary>
        /// 获取账单付款金额合计
        /// </summary>
        /// <param name="billid"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public JsonResult GetPaymentTotal(PaymentViewModel model)
        {
            try
            {
                var service = GetService<IPosBillDetailService>();
                var paymentTotal = service.GetBillDetailForPaymentTotalByBillid(CurrentInfo.HotelId, model.Billid);
                return Json(JsonResultData.Successed(paymentTotal));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 删除支付列表项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult DeletePayment(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                try
                {
                    var service = GetService<IPosBillDetailService>();
                    var billDetail = service.GetEntity(CurrentInfo.HotelId, Convert.ToInt64(id));



                    if (billDetail != null)
                    {
                        var billService = GetService<IPosBillService>();
                        var bill = billService.Get(billDetail.Billid);

                        var itemService = GetService<IPosItemService>();
                        var item = itemService.Get(billDetail.Itemid);
                        if (string.IsNullOrWhiteSpace(item.PayType) || item.PayType == "no")
                        {
                            var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, CanReason = billDetail.CanReason };

                            billDetail.IsCheck = false;
                            billDetail.CanReason = "付款列表删除";
                            billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                            billDetail.ModiUser = CurrentInfo.UserName;
                            billDetail.ModiDate = DateTime.Now;

                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason + "，操作员：" + CurrentInfo.UserName, bill.BillNo);


                            service.Update(billDetail, new PosBillDetail());
                            service.Commit();
                        }
                        else if (!string.IsNullOrWhiteSpace(billDetail.SettleTransno))
                        {
                            var posItem = itemService.Get(billDetail.Itemid);
                            var jsonResult = Refund(billDetail, posItem);
                            var result = jsonResult.Data as JsonResultData;
                            if (result.Success == true)
                            {
                                var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, CanReason = billDetail.CanReason };

                                billDetail.IsCheck = false;
                                billDetail.CanReason = "付款列表删除";
                                billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                                billDetail.ModiUser = CurrentInfo.UserName;
                                billDetail.ModiDate = DateTime.Now;

                                //    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + oldBillDetail.ItemName + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, billDetail.Billid);
                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, bill.BillNo);
                                service.Update(billDetail, new PosBillDetail());
                                service.Commit();
                            }
                            else
                            {
                                return Json(JsonResultData.Failure(result.Data));
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(item.PayType) && item.PayType != "no")
                        {
                            if (!string.IsNullOrEmpty(item.PayType) && item.PayType.Equals("house", StringComparison.CurrentCultureIgnoreCase))
                            {
                                var jsonResult = new PosReverseCheckoutController().CancelRoomAccount(billDetail);
                                var result = jsonResult.Data as JsonResultData;
                                if (result.Success == true)
                                {
                                    var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, CanReason = billDetail.CanReason };

                                    billDetail.IsCheck = false;
                                    billDetail.CanReason = "付款列表删除";
                                    billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                                    billDetail.ModiUser = CurrentInfo.UserName;
                                    billDetail.ModiDate = DateTime.Now;

                                    //  AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + oldBillDetail.ItemName + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, billDetail.Billid);
                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, bill.BillNo);
                                    service.Update(billDetail, new PosBillDetail());
                                    service.Commit();
                                }
                                else
                                {
                                    return Json(JsonResultData.Failure("退款失败：" + result.Data));
                                }
                            }
                            else if (!string.IsNullOrEmpty(item.PayType) && item.PayType.Equals("mbrCard", StringComparison.CurrentCultureIgnoreCase))
                            {
                                var jsonResult = new PosReverseCheckoutController().CancelMbrCard(billDetail);
                                var result = jsonResult.Data as JsonResultData;
                                if (result.Success == true)
                                {
                                    var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, CanReason = billDetail.CanReason };

                                    billDetail.IsCheck = false;
                                    billDetail.CanReason = "付款列表删除";
                                    billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                                    billDetail.ModiUser = CurrentInfo.UserName;
                                    billDetail.ModiDate = DateTime.Now;

                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, bill.BillNo);

                                    service.Update(billDetail, new PosBillDetail());
                                    service.Commit();
                                }
                                else
                                {
                                    return Json(JsonResultData.Failure("退款失败：" + result.Data));
                                }
                            }
                            else if (!string.IsNullOrEmpty(item.PayType) && item.PayType.Equals("mbrLargess", StringComparison.CurrentCultureIgnoreCase))
                            {
                                var jsonResult = new PosReverseCheckoutController().CancelMbrLargess(billDetail);
                                var result = jsonResult.Data as JsonResultData;
                                if (result.Success == true)
                                {
                                    var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, CanReason = billDetail.CanReason };

                                    billDetail.IsCheck = false;
                                    billDetail.CanReason = "付款列表删除";
                                    billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                                    billDetail.ModiUser = CurrentInfo.UserName;
                                    billDetail.ModiDate = DateTime.Now;

                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, bill.BillNo);

                                    service.Update(billDetail, new PosBillDetail());
                                    service.Commit();
                                }
                                else
                                {
                                    return Json(JsonResultData.Failure("退款失败：" + result.Data));
                                }
                            }
                            else if (!string.IsNullOrEmpty(item.PayType) && item.PayType.Equals("mbrCardAndLargess", StringComparison.CurrentCultureIgnoreCase))
                            {
                                var jsonResult = new PosReverseCheckoutController().CancelMbrCardAndLargess(billDetail);
                                var result = jsonResult.Data as JsonResultData;
                                if (result.Success == true)
                                {
                                    var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, CanReason = billDetail.CanReason };

                                    billDetail.IsCheck = false;
                                    billDetail.CanReason = "付款列表删除";
                                    billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                                    billDetail.ModiUser = CurrentInfo.UserName;
                                    billDetail.ModiDate = DateTime.Now;

                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, bill.BillNo);

                                    service.Update(billDetail, new PosBillDetail());
                                    service.Commit();
                                }
                                else
                                {
                                    return Json(JsonResultData.Failure("退款失败：" + result.Data));
                                }
                            }
                            else if (!string.IsNullOrEmpty(item.PayType) && item.PayType.Equals("corp", StringComparison.CurrentCultureIgnoreCase))
                            {
                                var jsonResult = new PosReverseCheckoutController().CancelContractUnit(billDetail);
                                var result = jsonResult.Data as JsonResultData;
                                if (result.Success == true)
                                {
                                    var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, CanReason = billDetail.CanReason };

                                    billDetail.IsCheck = false;
                                    billDetail.CanReason = "付款列表删除";
                                    billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                                    billDetail.ModiUser = CurrentInfo.UserName;
                                    billDetail.ModiDate = DateTime.Now;

                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, bill.BillNo);

                                    service.Update(billDetail, new PosBillDetail());
                                    service.Commit();
                                }
                                else
                                {
                                    return Json(JsonResultData.Failure("退款失败：" + result.Data));
                                }
                            }
                            else if (!string.IsNullOrEmpty(item.PayType) && item.PayType.Equals("PrePay", StringComparison.CurrentCultureIgnoreCase))
                            {
                                //定金反结。修改押金付款的状态
                                var prePayService = GetService<IYtPrepayService>();
                                var preList = prePayService.GetModelByPaidNo(CurrentInfo.HotelId, id);
                                string billNo = "";
                                foreach (var preModel in preList)
                                {
                                    billNo = preModel.BillNo;
                                    preModel.IPrepay = (byte)PrePayStatus.取消;
                                    prePayService.Update(preModel, new YtPrepay());
                                    prePayService.Commit();
                                }
                                if (!string.IsNullOrEmpty(billNo))
                                {
                                    var _preModel = prePayService.GetPreModel(CurrentInfo.HotelId, billNo, CurrentInfo.ModuleCode, PrePayStatus.交押金);
                                    if (_preModel != null)
                                    {
                                        _preModel.IsClear = 0;  //修改成未结清
                                        prePayService.Update(_preModel, new YtPrepay());
                                        prePayService.Commit();
                                    }
                                }

                                var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, CanReason = billDetail.CanReason };

                                billDetail.IsCheck = false;
                                billDetail.CanReason = "付款列表删除";
                                billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                                billDetail.ModiUser = CurrentInfo.UserName;
                                billDetail.ModiDate = DateTime.Now;

                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, bill.BillNo);

                                service.Update(billDetail, new PosBillDetail());
                                service.Commit();

                            }
                        }
                        else
                        {
                            var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, CanReason = billDetail.CanReason };

                            billDetail.IsCheck = false;
                            billDetail.CanReason = "付款列表删除";
                            billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                            billDetail.ModiUser = CurrentInfo.UserName;
                            billDetail.ModiDate = DateTime.Now;

                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, bill.BillNo);

                            service.Update(billDetail, new PosBillDetail());
                            service.Commit();
                        }

                        var billDetailList = service.GetBillDetailByDcFlag(CurrentInfo.HotelId, billDetail.Billid);
                        foreach (var temp in billDetailList)
                        {
                            if (temp.Status == (byte)PosBillDetailStatus.找赎 || temp.Isauto == (byte)PosBillDetailIsauto.抹零)
                            {
                                try
                                {
                                    if (temp.Isauto == (byte)PosBillDetailIsauto.抹零)
                                    {
                                        temp.Dueamount = 0;
                                        temp.Amount = 0;
                                    }

                                    var oldBillDetail = new PosBillDetail() { Status = temp.Status, CanReason = temp.CanReason };
                                    temp.IsCheck = false;
                                    temp.CanReason = "付款列表删除";
                                    temp.Status = (byte)PosBillDetailStatus.反结取消;
                                    temp.ModiUser = CurrentInfo.UserName;
                                    temp.ModiDate = DateTime.Now;

                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, bill.BillNo);

                                    service.Update(temp, new PosBillDetail());
                                    service.Commit();
                                }
                                catch (Exception ex)
                                {
                                    return Json(JsonResultData.Failure(ex));
                                }
                            }
                        }

                        #region 消费项目付款信息清空


                        foreach (var detail in billDetailList.Where(w => w.DcFlag == PosItemDcFlag.D.ToString() && w.Settleid == billDetail.Settleid && w.Isauto != (byte)PosBillDetailIsauto.抹零).ToList())
                        {
                            detail.IsCheck = false;
                            detail.Settleid = null;
                            detail.SettleDate = null;
                            detail.SettleShiftId = null;
                            detail.SettleBsnsDate = null;
                            detail.SettleShuffleid = null;
                            detail.SettleUser = null;

                            service.Update(detail, new PosBillDetail());
                            service.Commit();
                        }
                        PosCommon common = new PosCommon();
                        common.RepealEInvoice(billDetail.Billid);

                        #endregion 消费项目付款信息清空
                    }
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
            return Json(JsonResultData.Successed(""));
        }

        /// <summary>
        /// 迟付
        /// </summary>
        /// <param name="billid"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult DelayedPayment(string billid, string memo)
        {
            try
            {
                var billService = GetService<IPosBillService>();
                var bill = billService.Get(billid);
                if (bill != null && bill.Status == (byte)PosBillStatus.开台)
                {
                    var oldBill = new PosBill();
                    AutoSetValueHelper.SetValues(bill, oldBill);
                    var refeService = GetService<IPosRefeService>();
                    var refe = refeService.Get(bill.Refeid);

                    var posService = GetService<IPosPosService>();
                    var pos = posService.Get(CurrentInfo.PosId);

                    //if (refe.Isclrtab == true)
                    //{
                    //修改账单状态
                    bill.MoveUser = CurrentInfo.UserName;
                    bill.Status = (byte)PosBillStatus.迟付;
                    bill.IsOver = true;
                    bill.CashMemo = memo;   //迟付备注
                    billService.Update(bill, new PosBill());
                    billService.AddDataChangeLog(OpLogType.Pos账单修改);
                    billService.Commit();
                    AddOperationLog(OpLogType.Pos账单修改, "状态：" + oldBill.Status + " -> " + bill.Status, bill.BillNo);
                    //}
                }

                //修改餐台状态
                PosCommon common = new PosCommon();
                common.SetTabStatus(bill);

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

                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
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
        private JsonResult Refund(PosBillDetail billDetail, PosItem item)
        {
            //检查参数
            if (billDetail.Settleid == null || billDetail.Settleid == Guid.Empty)
            {
                return Json(JsonResultData.Failure("请指定要退款的账务！"));
            }
            if (billDetail.Amount <= 0)
            {
                return Json(JsonResultData.Failure("请填写要退款的金额！"));
            }
            //检查账务
            var checkResult = RefundCheck(billDetail, item);
            if (!checkResult.Success)
            {
                return Json(checkResult);
            }

            string folioPosItemAction = item.PayType;//付款账务，付款方式，处理方式
            string folioPosItemActionJsonPara = GetFolioPosItemActionJsonPara(folioPosItemAction, billDetail);//组装JSON
            try
            {
                var service = GetService<IPosBillDetailService>();
                var payServiceBuilder = GetService<IPayServiceBuilder>();
                var commonDb = GetService<DbCommonContext>();
                var pmsParaService = GetService<IPmsParaService>();

                var commonPayParas = commonDb.M_v_payParas.ToList();
                var hotelPayParas = pmsParaService.GetPmsParas(CurrentInfo.HotelId);

                IPayService payService = null;
                using (var tc = new TransactionScope())
                {
                    payService = payServiceBuilder.GetPayRefundService(folioPosItemAction, commonPayParas, hotelPayParas, MvcApplication.IsTestEnv);
                    var payResult = new PayResult { RefNo = "", IsWaitPay = false };
                    if (payService != null)
                    {
                        if (string.IsNullOrWhiteSpace(folioPosItemActionJsonPara))
                        {
                            return Json(JsonResultData.Failure("参数不能为空"));
                        }
                        payResult = payService.DoPayBeforeSaveFolio(folioPosItemActionJsonPara);
                    }

                    tc.Complete();
                }

                var returnResult = new ResFolioAddResult
                {
                    FolioTransId = billDetail.SettleTransno,
                    Statu = PayStatu.Successed.ToString(),
                    Callback = "",
                    QrCodeUrl = "",
                    QueryTransId = "",
                    DCFlag = "C"
                };

                var newTransId = billDetail.SettleTransno;

                string logStr = string.Format("退款账务ID：{0}，退款记录账务ID：{1}，退款金额：{2}。", billDetail.Id.ToString(), newTransId, billDetail.Amount);
                AddOperationLog(OpLogType.Pos退款, logStr, billDetail.Billid);

                if (payService != null)
                {
                    folioPosItemActionJsonPara = GetFolioPosItemActionJsonPara(folioPosItemAction, billDetail);//组装JSON
                    var afterPayResult = payService.DoPayAfterSaveFolio(PayProductType.PosPayment, newTransId, folioPosItemActionJsonPara);
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
        private JsonResultData RefundCheck(PosBillDetail billDetail, PosItem item)
        {
            //检查付款明细
            if (billDetail.Settleid == null || billDetail.Settleid == Guid.Empty)
            {
                return JsonResultData.Failure("请指定要退款的账务！");
            }
            if (billDetail == null || string.IsNullOrWhiteSpace(billDetail.Billid))
            {
                return JsonResultData.Failure("退款的账务不存在！");
            }
            if (string.IsNullOrWhiteSpace(billDetail.DcFlag) || !billDetail.DcFlag.Equals("C", StringComparison.OrdinalIgnoreCase))
            {
                return JsonResultData.Failure("退款的账务不属于付款！");
            }
            if (billDetail.IsCheck != true)
            {
                return JsonResultData.Failure("退款的账务必须是已付状态！");
            }
            if (billDetail.Amount <= 0)
            {
                return JsonResultData.Failure("退款的账务金额必须大于零！");
            }
            if (string.IsNullOrWhiteSpace(billDetail.Itemid))
            {
                return JsonResultData.Failure("退款的账务付款方式不能为空！");
            }

            //2.检查付款方式 类型=C，状态=启用，处理方式=（支付宝刷卡支付、支付宝扫码支付、微信刷卡支付、微信扫码支付）
            if (string.IsNullOrWhiteSpace(item.PayType) || (!item.PayType.Equals("AliBarcode", StringComparison.OrdinalIgnoreCase) && !item.PayType.Equals("AliQrcode", StringComparison.OrdinalIgnoreCase) && !item.PayType.Equals("WxBarcode", StringComparison.OrdinalIgnoreCase) && !item.PayType.Equals("WxQrcode", StringComparison.OrdinalIgnoreCase)))
            {
                return JsonResultData.Failure("退款的账务付款方式处理方式只支持（支付宝刷卡支付、支付宝扫码支付、微信刷卡支付、微信扫码支付）！");
            }
            return JsonResultData.Successed();
        }

        /// <summary>
        /// 获取退款JSON字符串
        /// </summary>
        /// <param name="folioPosItemAction">处理方式</param>
        /// <param name="posBillDetail">选中要退款的账务</param>
        /// <param name="amount">退款金额</param>
        /// <param name="newTransId">退款操作生成新的退款账务ID</param>
        /// <returns></returns>
        private string GetFolioPosItemActionJsonPara(string folioPosItemAction, PosBillDetail posBillDetail)
        {
            string folioPosItemActionJsonPara = "";
            if (folioPosItemAction == "AliBarcode" || folioPosItemAction == "AliQrcode")
            {
                var para = new
                {
                    originPayTransId = posBillDetail.SettleTransno,//选中要退款的账务的主键ID
                    refundId = posBillDetail.SettleTransno,//退款操作生成新的退款账务ID
                    refundAmount = posBillDetail.Amount,//退款金额
                    refundReason = "Pos退款",//退款原因
                };
                folioPosItemActionJsonPara = Newtonsoft.Json.JsonConvert.SerializeObject(para);
            }
            else if (folioPosItemAction == "WxBarcode" || folioPosItemAction == "WxQrcode")
            {
                var para = new
                {
                    outTradeNo = posBillDetail.SettleTransno,//选中要退款的账务的主键ID
                    outRefundNo = posBillDetail.SettleTransno,//退款操作生成新的退款账务ID
                    totalFee = posBillDetail.Amount,//原始总金额
                    refundFee = posBillDetail.Amount,//退款金额
                    opUserId = CurrentInfo.UserName,//操作员
                };
                folioPosItemActionJsonPara = Newtonsoft.Json.JsonConvert.SerializeObject(para);
            }
            return folioPosItemActionJsonPara;
        }

        /// <summary>
        /// 手工清台
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.Disable)]
        public ActionResult ManualClearing(string billid)
        {
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);
            if (!string.IsNullOrWhiteSpace(billid))
            {
                var service = GetService<IPosBillDetailService>();
                var paymentTotal = service.GetBillDetailForPaymentTotalByBillid(CurrentInfo.HotelId, billid);

                if (paymentTotal != null)
                {
                    if (paymentTotal.Consume == 0)
                    {
                        try
                        {
                            var billService = GetService<IPosBillService>();
                            var bill = billService.Get(billid);

                            var oldBill = new PosBill();
                            AutoSetValueHelper.SetValues(bill, oldBill);
                            #region 修改账单状态

                            var refeService = GetService<IPosRefeService>();
                            var refe = refeService.Get(bill.Refeid);

                            bill.DepBsnsDate = pos.Business;
                            bill.MoveUser = CurrentInfo.UserName;
                            bill.DepDate = DateTime.Now;
                            bill.Status = (byte)PosBillStatus.清台;

                            billService.Update(bill, new PosBill());
                            //  billService.AddDataChangeLog(OpLogType.Pos账单修改);
                            billService.Commit();
                            AddOperationLog(OpLogType.Pos账单修改, "状态：" + oldBill.Status + " -> " + bill.Status, bill.BillNo);

                            #endregion 修改账单状态

                            #region 修改餐台状态

                            //修改餐台状态
                            PosCommon common = new PosCommon();
                            common.SetTabStatus(bill);

                            #endregion 修改餐台状态

                            #region 清理锁台记录

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

                            #endregion 清理锁台记录

                            return Json(JsonResultData.Successed(""));
                        }
                        catch (Exception ex)
                        {
                            return Json(JsonResultData.Failure(ex));
                        }
                    }
                    if (paymentTotal.Consume != 0 && paymentTotal.Paid != 0)
                    {
                        if (paymentTotal.Paid >= paymentTotal.Total || paymentTotal.Consume == paymentTotal.Paid)
                        {
                            try
                            {
                                var billService = GetService<IPosBillService>();
                                var bill = billService.Get(billid);
                                var oldBill = new PosBill();
                                AutoSetValueHelper.SetValues(bill, oldBill);

                                #region 修改账单状态

                                var refeService = GetService<IPosRefeService>();
                                var refe = refeService.Get(bill.Refeid);

                                bill.DepBsnsDate = pos.Business;
                                bill.MoveUser = CurrentInfo.UserName;
                                bill.DepDate = DateTime.Now;
                                bill.Status = (byte)PosBillStatus.清台;

                                billService.Update(bill, new PosBill());
                                //billService.AddDataChangeLog(OpLogType.Pos账单修改);
                                billService.Commit();
                                AddOperationLog(OpLogType.Pos账单修改, "状态：" + oldBill.Status + " -> " + bill.Status, bill.BillNo);
                                #endregion 修改账单状态

                                #region 修改餐台状态

                                //修改餐台状态
                                PosCommon common = new PosCommon();
                                common.SetTabStatus(bill);

                                #endregion 修改餐台状态

                                #region 清理锁台记录

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

                                #endregion 清理锁台记录

                                return Json(JsonResultData.Successed(""));
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonResultData.Failure(ex));
                            }
                        }
                        else
                        {
                            return Json(JsonResultData.Failure("只有已经结账的账单才能手工清台"));
                        }
                    }
                    else
                    {
                        return Json(JsonResultData.Failure("只有已经结账的账单才能手工清台"));
                    }
                }
                else
                {
                    return Json(JsonResultData.Failure("只有已经结账的账单才能手工清台"));
                }
            }
            else
            {
                return Json(JsonResultData.Failure("此账单不存在或已删除"));
            }
        }

        /// <summary>
        /// 获取尾数处理金额
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult GetTailProcessing(TailProcessingViewModel model)
        {
            try
            {
                //获取账单
                var billService = GetService<IPosBillService>();
                var bill = billService.Get(model.Billid);

                //获取付款方式
                var itemService = GetService<IPosItemService>();
                var item = itemService.GetEntity(CurrentInfo.HotelId, model.Itemid);

                //根据付款方式的汇率换算金额后再进行尾数处理
                var service = GetService<IPosBillDetailService>();
                decimal exchangeRate = model.Amount / item.Rate ?? 0;
                model.Amount = service.GetAmountByBillTailProcessing(CurrentInfo.HotelId, bill.Refeid, exchangeRate);

                return Json(model);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 根据人民币面值获取相近的付款金额
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult GetExpectedPaymentg(decimal amount)
        {
            try
            {
                decimal bit = amount % 10;
                decimal ten = amount % 100 - bit;
                int max = Convert.ToInt32(amount - amount % 100 + 100);
                int[] nums = new int[] { 0, 0, 0, 0 };
                if (amount > 0)
                {
                    nums[0] = Convert.ToInt32(amount + 10 - bit);
                    nums[1] = Convert.ToInt32(amount + 20 - bit);
                    nums[2] = Convert.ToInt32(amount + 50 - bit);
                    nums[3] = Convert.ToInt32(amount + 100 - bit);

                    nums[0] = nums[0] > max || nums[0] % 10 > 5 ? max : nums[0];
                    nums[1] = nums[1] > max || nums[1] % 20 > 10 ? max : nums[1];
                    nums[2] = nums[2] > max || nums[2] % 50 > 20 ? max : nums[2];
                    nums[3] = nums[3] > max || nums[3] % 100 > 50 ? max : nums[3];
                }

                return Json(JsonResultData.Successed(nums));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        #region 获取分布视图

        /// <summary>
        /// 获取账单信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PosBill(string billid)
        {
            var service = GetService<IPosBillService>();
            var model = service.GetPosBillByBillid(CurrentInfo.HotelId, billid);
            return PartialView("_PosBill", model);
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
                list = service.GetPosBillByPosid(CurrentInfo.HotelId, CurrentInfo.PosId, pos.Business, tabNo);
            }

            return PartialView("_PosBillList", list);
        }

        /// <summary>
        /// 收银账单明细列表视图
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PosBillDetailList(CashierViewModel model)
        {
            return PartialView("_PosBillDetailList", model);
        }

        /// <summary>
        /// 添加查询参数临时数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns>返回url地址的参数部分,例如?ReportCode={0}&ParameterValues={1}&ChineseName={2}</returns>
        [HttpPost]
        [AuthButton(AuthFlag.Export)]
        public ActionResult AddQueryParaTemp(ReportQueryModel model, string print, string Flag)
        {
            PosCommon common = new PosCommon();
            var controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            return common.AddQueryParaTemp(model, print, Flag, controller);

        }

        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult AddQueryParaTempByPaySusses(ReportQueryModel model, string print, string Flag, string isPrintBill)
        {
            PosCommon common = new PosCommon();
            return common.AddQueryParaTempByPaySusses(model, print, Flag, isPrintBill);
        }

        #endregion 获取分布视图

        #region 打单之前判断收银点设置是否打单

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

        #endregion 打单之前判断收银点设置是否打单

        #region 获取列表

        /// <summary>
        /// 获取指定账单下的账单明细列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListUpBillDetailByBillid([DataSourceRequest]DataSourceRequest request, string billid)
        {
            var billServer = GetService<IPosBillDetailService>();
            var list = billServer.GetUpBillDetailByCashier(CurrentInfo.HotelId, billid, PosItemDcFlag.D.ToString()).ToList();
            byte[] status = new byte[] { 1, 2, 3, 51, 52 };
            foreach (var temp in list)
            {
                if (Array.IndexOf(status, temp.Status) != -1)
                {
                    temp.Dueamount = 0;
                    temp.Amount = 0;
                    temp.Discount = 0;
                    temp.DiscAmount = 0;
                }
                if (temp.SD == true)
                {
                    temp.Row = "　" + temp.Row;
                    temp.ItemName = "　" + temp.ItemName;
                    temp.Dueamount = null;
                    temp.Amount = null;
                    temp.Discount = null;
                    temp.DiscAmount = null;
                    temp.Service = null;
                }
            }

            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 获取指定模块下的消费项目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListItemsForItemByDcFlag([DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosItemService>();
            var datas = service.GetPosItem(CurrentInfo.HotelId, PosItemDcFlag.C.ToString());
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取指定酒店下付款方式总数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ContentResult GetPaymentMethodTotal(CashierViewModel model)
        {
            var service = GetService<IPosItemService>();
            var total = service.GetPosItemTotalByDcFlag(CurrentInfo.HotelId, PosItemDcFlag.C.ToString());
            return Content(total.ToString());
        }

        /// <summary>
        /// 获取指定账单下付款明细列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult PaymentDetailByBill([DataSourceRequest]DataSourceRequest request, CashierViewModel model)
        {
            var billServer = GetService<IPosBillDetailService>();
            var list = billServer.GetBillDetailForPaymentByBillid(CurrentInfo.HotelId, model.Billid);
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 获取付款处理方式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult GetPayTypeByPayMethod(string id)
        {
            var service = GetService<IPosItemService>();
            var payType = service.GetEntity(CurrentInfo.HotelId, id);
            return Json(payType, JsonRequestBehavior.AllowGet);
        }

        #endregion 获取列表

        /// <summary>
        /// 报表穿透视图
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _BillDetailByReport(string billId)
        {
            var service = GetService<IPosBillService>();
            var model = service.GetPosBillByBillid(CurrentInfo.HotelId, billId);
            return View("_BillDetailByReport", model);
        }

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
            AddOperationLog(OpLogType.Pos账单修改, "取消打单：" + oldEntity.IPrint + " -> " + newEntity.IPrint, newEntity.BillNo);

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
            return common.AddQueryParaTemp(model, print, Flag, controller);
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
        [AuthButton(AuthFlag.ChangeCardNum)]
        public ActionResult PayCheck(PosTabLogAddViewModel model)
        {
            var result = CheckTabLog(model);
            return result;
        }

        #endregion 买单验证

        #region 迟付验证

        [AuthButton(AuthFlag.Out)]
        public ActionResult payDelayedCheck(PosTabLogAddViewModel model)
        {
            var result = CheckTabLog(model);
            return result;
        }

        #endregion 迟付验证

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



        #region 入单验证

        [AuthButton(AuthFlag.Reset)]
        public ActionResult InSingleCheck(PosTabLogAddViewModel model)
        {
            var result = CheckTabLog(model);
            return result;
        }

        #endregion 入单验证

        #region 会员折扣
        [AuthButton(AuthFlag.AdjustPrice)]
        public ActionResult MemberDiscount(string posBillId)
        {
            ViewBag.BillID = posBillId;
            return PartialView("_MemberDiscount");
        }


        //获取会员信息
        [AuthButton(AuthFlag.AdjustPrice)]
        public JsonResult GetMbrCardInfo(string CardID)
        {
            var posservice = GetService<IPosPosService>();
            var Pospos = posservice.Get(CurrentInfo.PosId);
            var service = GetService<IPosItemClassDiscountService>();
            var result = service.GetMbrCardInfoByCardID(CurrentInfo.HotelId, CardID, (Pospos.CodeIn == null ? "" : Pospos.CodeIn.Trim()));
            return Json(result);
        }

        //获取账单大类的折扣
        /// <summary>
        /// 会员卡类型
        /// </summary>
        /// <param name="posBillId">账单ID</param>
        /// <param name="MType">会员卡类型</param>
        /// <param name="IsUserMember">是否使用会员</param>
        /// , string MType,string IsUserMember
        /// <returns></returns>
        [AuthButton(AuthFlag.AdjustPrice)]
        public JsonResult GetItemClassDisCountByBill([DataSourceRequest]DataSourceRequest request, string billid, string membercard = "", bool IsUseMenber = false)
        {
            try
            {
                ///TODO : 是否做餐台客人类型验证，只有会员才可以使用会员折扣
                var MemberDisCount = 1M;
                MbrCardInfoModel MbrCardInfo = null;
                if (IsUseMenber)
                {
                    //获取会员信息
                    var posservice = GetService<IPosPosService>();
                    var Pospos = posservice.Get(CurrentInfo.PosId);
                    var service = GetService<IPosItemClassDiscountService>();
                    var result = service.GetMbrCardInfoByCardID(CurrentInfo.HotelId, membercard, (Pospos.CodeIn == null ? "" : Pospos.CodeIn.Trim()));
                    if (!result.Success)
                    {
                        var emptylist = new List<MemberDiscountViewModel>();
                        return new System.Web.Mvc.JsonResult()
                        {
                            Data = emptylist.ToDataSourceResult(request),
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            MaxJsonLength = Int32.MaxValue
                        };
                    }
                    MbrCardInfo = (MbrCardInfoModel)result.Data;

                    //没有折扣率使用默认折扣率
                    if (!MbrCardInfo.IsHasDiscount)
                    {
                        MemberDisCount = 1;
                    }
                    MemberDisCount = MbrCardInfo.DiscountRate;
                }

                var posbillservice = GetService<IPosBillService>();
                //获取账单
                var posbill = posbillservice.GetEntity(CurrentInfo.HotelId, billid);
                //获取账单的消费项目详情
                var poisitemlist = posbillservice.GetPosItemByPosBill(CurrentInfo.HotelId, posbill);

                var _list = new List<string>();
                foreach (var item in poisitemlist)
                {
                    _list.Add(item.ItemClassid);
                }
                _list = _list.Distinct().ToList();

                //获取消费项目的消费大类列表
                var ptclassservice = GetService<IPosItemClassService>();
                var positemclasslist = ptclassservice.GetPosItemClass(CurrentInfo.HotelId, u => _list.Contains(u.Id)).ToList();

                //会员大类折扣筛选
                Func<PosOnSale, bool> mwherefunc = b =>
                {
                    var res = b.iType == 2 && _list.Contains(b.ItemClassid) && (b.Itemid == null || b.Itemid == "");
                    if (MbrCardInfo != null)  //使用会员
                    {
                        res = res && (b.TabTypeid == MbrCardInfo.MbrCardType || b.TabTypeid == null || b.TabTypeid == "");
                    }
                    else
                    {
                        res = res && (b.TabTypeid == null || b.TabTypeid == "");
                    }
                    return res;
                };
                //获取对应的大类折扣
                var mdservice = GetService<IPosItemClassDiscountService>();
                var mdiscountlist = mdservice.GetItemClassDisCount(CurrentInfo.HotelId, mwherefunc);
                //会员大类折扣时间筛选
                mdiscountlist = mdservice.MemberDisCountTimeFileter(mdiscountlist);

                //获取对应的项目折扣
                _list.Clear();
                foreach (var item in poisitemlist)
                {
                    _list.Add(item.ItemID);
                }
                _list = _list.Distinct().ToList();

                //会员大类 消费项目 折扣筛选
                Func<PosOnSale, bool> mitemwherefunc = b =>
                {
                    var res = b.iType == 2 && _list.Contains(b.Itemid);
                    if (MbrCardInfo != null)  //使用会员
                    {
                        res = res && (b.TabTypeid == MbrCardInfo.MbrCardType || b.TabTypeid == null || b.TabTypeid == "");
                    }
                    else
                    {
                        res = res && (b.TabTypeid == null || b.TabTypeid == "");
                    }
                    return res;
                };
                var mitemdiscountlist = mdservice.GetItemClassDisCount(CurrentInfo.HotelId, mitemwherefunc);
                //会员大类折扣时间筛选
                mitemdiscountlist = mdservice.MemberDisCountTimeFileter(mitemdiscountlist);


                //遍历 账单的消费项目大类折扣，查找对应的大类折扣
                var list = new List<MemberDiscountViewModel>();

                foreach (var itemclass in positemclasslist)
                {
                    //查找大类折扣
                    var mdiscount = mdiscountlist.Where(u => u.ItemClassid == itemclass.Id).FirstOrDefault();

                    //是否有项目大类折扣
                    var IsHasItemClassDisCount = mdiscount != null;
                    //项目大类折扣
                    var ItemClassDisCount = IsHasItemClassDisCount ? Convert.ToDecimal(mdiscount.Discount) : 1;

                    if (mdiscount == null)  //没有设置大类折扣
                    {
                        list.Add(new MemberDiscountViewModel()
                        {
                            SortID = 1,
                            ParentID = "",
                            ItemID = itemclass.Id,
                            ItemName = itemclass.Cname,
                            IsCanEdit = IsUseMenber ? false : true,   //没有设置大类折扣且没有使用会员折扣， 可以编辑
                            DisCount = mdservice.CalculateMemberItemClassDisCount(MbrCardInfo, ItemClassDisCount, 1, IsHasItemClassDisCount),
                            IsHasDisCount = false,
                            IsPosItemClass = true,
                            Batch = ""
                        });
                    }
                    else
                    {
                        list.Add(new MemberDiscountViewModel()
                        {
                            SortID = 1,
                            ParentID = "",
                            ItemID = itemclass.Id,
                            ItemName = itemclass.Cname,
                            IsCanEdit = false,   //设置大类折扣 不了编辑
                            DisCount = mdservice.CalculateMemberItemClassDisCount(MbrCardInfo, ItemClassDisCount, 1, IsHasItemClassDisCount),
                            IsHasDisCount = true,
                            IsPosItemClass = true,
                            Batch = ""
                        });
                    }

                    //查找大类下的消费消费项目
                    var curitemlist = poisitemlist.Where(u => u.ItemClassid == itemclass.Id).ToList();
                    foreach (var item in curitemlist)
                    {
                        var mitemdiscount = mitemdiscountlist.Where(u => u.Itemid == item.ItemID && u.Unitid == item.UnitID).FirstOrDefault();
                        if (mitemdiscount == null)  //没有设置项目折扣
                        {
                            //list.Add(new MemberDiscountViewModel()
                            //{
                            //    SortID = 1,
                            //    ParentID = itemclass.Id,
                            //    ItemID = item.ItemID,
                            //    ItemName = item.Cname,
                            //    IsCanEdit = true,   //没有设置项目折扣 可以编辑
                            //    DisCount = mdiscount != null ? mdiscount.Discount : (IsUseMenber ? MemberDisCount : item.Discount),   //没有设置项目折扣使用大类折扣，没有大类折扣使用会员折扣，没有会员折扣使用账单原来的折扣
                            //    IsHasDisCount = false,
                            //    IsPosItemClass = false,
                            //    Batch = item.Batch,
                            //    UnitID = item.UnitID,
                            //    UnitName = item.UnitName,
                            //    Amount = item.Amount,
                            //    Price = item.Price,
                            //    Count = item.Quantity
                            //});
                        }
                        else
                        {
                            var a = mdservice.CalculateMemberItemDisCount(MbrCardInfo, ItemClassDisCount, Convert.ToDecimal(mitemdiscount.Discount), 1, IsHasItemClassDisCount, true, item.ItemIsCanDiscount);

                            list.Add(new MemberDiscountViewModel()
                            {
                                SortID = 1,
                                ParentID = "",
                                ItemID = item.ItemID,
                                ItemName = item.Cname,
                                IsCanEdit = false,   //设置项目折扣不可编辑
                                DisCount = mdservice.CalculateMemberItemDisCount(MbrCardInfo, ItemClassDisCount, Convert.ToDecimal(mitemdiscount.Discount), 1, IsHasItemClassDisCount, true, item.ItemIsCanDiscount),
                                IsHasDisCount = true,
                                IsPosItemClass = false,
                                Batch = item.Batch,
                                UnitID = item.UnitID,
                                UnitName = item.UnitName,
                                Amount = item.Amount,
                                Price = item.Price,
                                Count = item.Quantity
                            });
                        }
                    }
                }

                //排序
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].SortID = i + 1;
                }

                return new System.Web.Mvc.JsonResult()
                {
                    Data = list.ToDataSourceResult(request),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.ToString()));
            }
        }

        //输入折扣键盘
        [AuthButton(AuthFlag.None)]
        public ActionResult MemBerDiscountNumber(string discType = "0", string BillId = "")
        {
            ViewBag.discType = discType;    //折扣类型
            ViewBag.BillId = BillId;        //主账单ID
            return PartialView("MemBerDiscountNumber");
        }


        //设置会员大类折扣
        [AuthButton(AuthFlag.AdjustPrice)]
        [JsonException]
        public JsonResult UpdateMemBerDiscount(string BillId = "", string MemberCard = "", string ChangeValue = "")
        {
            var IsUseMenber = !string.IsNullOrEmpty(MemberCard);
            //查询订单，获取订单详情的大类，
            var MemberDisCount = 1M;
            var mdservice = GetService<IPosItemClassDiscountService>();
            MbrCardInfoModel MbrCardInfo = null;
            if (IsUseMenber)
            {
                //获取会员信息
                var posservice = GetService<IPosPosService>();
                var Pospos = posservice.Get(CurrentInfo.PosId);
                var service = GetService<IPosItemClassDiscountService>();
                var result = service.GetMbrCardInfoByCardID(CurrentInfo.HotelId, MemberCard, (Pospos.CodeIn == null ? "" : Pospos.CodeIn.Trim()));
                MbrCardInfo = (MbrCardInfoModel)result.Data;

                //没有折扣率使用默认折扣率
                if (!MbrCardInfo.IsHasDiscount)
                {
                    return Json(JsonResultData.Failure("会员折扣率不存在"));
                }
                MemberDisCount = MbrCardInfo.DiscountRate;

                //验证会员折扣方式
                if (mdservice.GetMemberDisCountType(MbrCardInfo.DiscountMode).FirstOrDefault() == null)
                {
                    return Json(JsonResultData.Failure("暂时不支持此种会员折扣方式"));
                }

            }
            var posbillservice = GetService<IPosBillService>();
            //获取账单
            var posbill = posbillservice.GetEntity(CurrentInfo.HotelId, BillId);
            //获取账单的消费项目详情
            var poisitemlist = posbillservice.GetPosItemByPosBill(CurrentInfo.HotelId, posbill);

            var _list = new List<string>();
            foreach (var item in poisitemlist)
            {
                _list.Add(item.ItemClassid);
            }
            _list = _list.Distinct().ToList();

            //获取消费项目的消费大类列表
            var ptclassservice = GetService<IPosItemClassService>();
            var positemclasslist = ptclassservice.GetPosItemClass(CurrentInfo.HotelId, u => _list.Contains(u.Id)).ToList();

            //会员大类折扣筛选
            Func<PosOnSale, bool> mwherefunc = b =>
            {
                var res = b.iType == 2 && _list.Contains(b.ItemClassid) && (b.Itemid == null || b.Itemid == "");
                if (MbrCardInfo != null)  //使用会员
                {
                    res = res && (b.TabTypeid == MbrCardInfo.MbrCardType || b.TabTypeid == null || b.TabTypeid == "");
                }
                else
                {
                    res = res && (b.TabTypeid == null || b.TabTypeid == "");
                }
                return res;
            };
            //获取对应的大类折扣

            var mdiscountlist = mdservice.GetItemClassDisCount(CurrentInfo.HotelId, mwherefunc);
            //会员大类折扣时间筛选
            mdiscountlist = mdservice.MemberDisCountTimeFileter(mdiscountlist);

            //获取对应的项目折扣
            _list.Clear();
            foreach (var item in poisitemlist)
            {
                _list.Add(item.ItemID);
            }
            _list = _list.Distinct().ToList();

            //会员大类 消费项目 折扣筛选
            Func<PosOnSale, bool> mitemwherefunc = b =>
            {
                var res = b.iType == 2 && _list.Contains(b.Itemid);
                if (MbrCardInfo != null)  //使用会员
                {
                    res = res && (b.TabTypeid == MbrCardInfo.MbrCardType || b.TabTypeid == null || b.TabTypeid == "");
                }
                else
                {
                    res = res && (b.TabTypeid == null || b.TabTypeid == "");
                }
                return res;
            };
            var mitemdiscountlist = mdservice.GetItemClassDisCount(CurrentInfo.HotelId, mitemwherefunc);
            //会员大类折扣时间筛选
            mitemdiscountlist = mdservice.MemberDisCountTimeFileter(mitemdiscountlist);

            var list = new List<MemberDiscountViewModel>();
            //先 按照默认数据进行折扣设置，
            foreach (var itemclass in positemclasslist)
            {
                //查找大类折扣
                var mdiscount = mdiscountlist.Where(u => u.ItemClassid == itemclass.Id).FirstOrDefault();

                //是否有项目大类折扣
                var IsHasItemClassDisCount = mdiscount != null;
                //项目大类折扣
                var ItemClassDisCount = IsHasItemClassDisCount ? Convert.ToDecimal(mdiscount.Discount) : 1;

                //查找大类下的消费消费项目，修改为对应折扣
                var curitemlist = poisitemlist.Where(u => u.ItemClassid == itemclass.Id).ToList();
                foreach (var item in curitemlist)
                {
                    var mitemdiscount = mitemdiscountlist.Where(u => u.Itemid == item.ItemID && u.Unitid == item.UnitID).FirstOrDefault();
                    if (mitemdiscount == null)  //没有设置项目折扣
                    {
                        list.Add(new MemberDiscountViewModel()
                        {
                            SortID = 1,
                            ParentID = itemclass.Id,
                            ItemID = item.ItemID,
                            ItemName = item.Cname,
                            IsCanEdit = IsUseMenber ? false : (item.ItemIsCanDiscount ? true : false),   //没有设置项目折扣且没有使用会员折扣，可以编辑
                            DisCount = mdservice.CalculateMemberItemDisCount(MbrCardInfo, ItemClassDisCount, 1, 1, IsHasItemClassDisCount, false, item.ItemIsCanDiscount), //计算折扣
                            IsHasDisCount = false,
                            IsPosItemClass = false,
                            Batch = item.Batch,
                            UnitID = item.UnitID,
                            UnitName = item.UnitName,
                            Amount = item.Amount,
                            Price = item.Price,
                            Count = item.Quantity
                        });
                    }
                    else
                    {
                        list.Add(new MemberDiscountViewModel()
                        {
                            SortID = 1,
                            ParentID = "",
                            ItemID = item.ItemID,
                            ItemName = item.Cname,
                            IsCanEdit = false,   //设置项目折扣 不可编辑
                            DisCount = mdservice.CalculateMemberItemDisCount(MbrCardInfo, ItemClassDisCount, Convert.ToDecimal(mitemdiscount.Discount), 1, IsHasItemClassDisCount, true, item.ItemIsCanDiscount), //计算折扣   
                            IsHasDisCount = true,
                            IsPosItemClass = false,
                            Batch = item.Batch,
                            UnitID = item.UnitID,
                            UnitName = item.UnitName,
                            Amount = item.Amount,
                            Price = item.Price,
                            Count = item.Quantity
                        });
                    }
                }
            }

            //没有使用会员，才可以修改折扣
            if (!IsUseMenber)
            {
                //按照修改的折扣进行修改，注意查询是否可以更改 ，TODO : 当前只修改大类折扣
                if (!string.IsNullOrEmpty(ChangeValue))
                {
                    //读取修改数据
                    try
                    {
                        //查找修改了的大类折扣，修改该项目大类下可修改的消费项目，按照折扣方式更新折扣
                        var changelist = JsonHelper.SerializeObject<List<MemberDiscountChangeModel>>(ChangeValue);
                        foreach (var item in changelist)
                        {
                            if (item.IsItemClass)
                            {
                                //查找到对应的大类折扣 ，修改该大类折扣下可以修改的消费项目折扣记录
                                var caneditlist = list.Where(u => u.ParentID == item.ItemID && !u.IsPosItemClass && u.IsCanEdit).ToList();
                                for (int i = 0; i < caneditlist.Count; i++)
                                {
                                    //直接修改，因为没有任何折扣方式，就是直接设置折扣
                                    caneditlist[i].DisCount = item.DisCount;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            //获取账单详情记录
            var pbilldetailService = GetService<IPosBillDetailService>();
            var posbilldetaillist = pbilldetailService.GetBillDetailByDcFlag(CurrentInfo.HotelId, posbill.Billid, "D");
            //修改所有消费项目详情状态为 单道折
            for (int i = 0; i < posbilldetaillist.Count; i++)
            {
                posbilldetaillist[i].IsDishDisc = true;
            }
            //修改对应的单道折扣
            StringBuilder sb = new StringBuilder($"单号：{ posbill.Billid }，台号：{ posbill.Tabid }，名称：{posbill.Name }, {(IsUseMenber ? $"使用会员折扣，折扣方式: {MbrCardInfo.DiscountMode }，折扣率: {MbrCardInfo.DiscountRate}。 " : "")} "); //日志记录
            foreach (var item in list)
            {
                if (!item.IsPosItemClass)
                {
                    var posbilldetail = posbilldetaillist.Where(u => u.Itemid == item.ItemID && u.BatchTime == item.Batch && u.Unitid == item.UnitID).FirstOrDefault();
                    if (posbilldetail != null)
                    {
                        sb.Append($"消费详情ID ：{posbilldetail.Id},折扣： {posbilldetail.Discount} > ");
                        posbilldetail.Discount = item.DisCount;
                        sb.Append($"{item.DisCount},");
                        if (IsUseMenber && MbrCardInfo.DiscountMode == "3") //会员价记录会员折扣
                        {
                            sb.Append($"会员折扣：{posbilldetail.DiscountClub} > {item.DisCount} ");
                            posbilldetail.DiscountClub = item.DisCount;
                        }

                        sb.Append(";");
                    }
                }
            }

            for (int i = 0; i < posbilldetaillist.Count; i++)
            {
                pbilldetailService.Update(posbilldetaillist[i], null);
            }
            //修改账单折扣方式
            if (IsUseMenber && MbrCardInfo.DiscountMode == "3")  //会员价 
            {
                sb.Append($"主单折扣类型 ： { posbill.IsForce} > 3");
                posbill.IsForce = 3;  //折扣方式会员价
            }
            else
            {
                sb.Append($"主单折扣类型 ： { posbill.IsForce} > 0");
                posbill.IsForce = 0; //一般折扣
            }

            //会员折扣记录会员卡号以及会员ID
            if (IsUseMenber)
            {
                posbill.Profileid = MbrCardInfo.ProFileID;
                posbill.CardNo = MbrCardInfo.MbrCardNo;
            }
            //单道折扣时，如果账单折扣为NUll 会把所有账单折扣设置为1  ，为了避免这种情况，检测一下账单折扣 ，为null则设置为 1
            if (posbill.Discount == null)
                posbill.Discount = 1M;

            posbillservice.Update(posbill, null);

            AddOperationLog(OpLogType.Pos账单修改, sb.ToString());

            //保存
            pbilldetailService.Commit();

            //调用重算方法
            pbilldetailService.UpdateBillDetailDisc(CurrentInfo.HotelId, posbill.Billid, posbill.MBillid);
            //返回结果
            return Json(JsonResultData.Successed(""));
        }

        #endregion

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

        /// <summary>
        /// 待支付账单列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult _WaitPayment()
        {
            SetCommonQueryValues("up_pos_list_ResFolioPayInfo", "@p02开始时间=" + DateTime.Now.AddHours(-2) + "&@p02结束时间=" + DateTime.Now, "_WaitGrid");
            return View();
        }

        #region  打印会员结账单
        [AuthButton(AuthFlag.None)]
        public ActionResult AddMardQueryParaTemp(ReportQueryModel model, string print, string Flag, string controller = "")
        {
            var common = new PosCommon();
            return common.AddMardQueryParaTemp(model, print, Flag);

        }

        #endregion


        #region 买单成功调用接口生成发票数据

        [AuthButton(AuthFlag.None)]
        public ActionResult AddConsumInfo(string billId)
        {
            PosCommon common = new PosCommon();
            common.AddEInvoice(billId);
            return Json(JsonResultData.Successed());
        }
        #endregion



        #region 迟付输入原因窗口

        [AuthButton(AuthFlag.None)]
        public ActionResult _DelayPayMemo(string billId)
        {
            ViewBag.BillId = billId;
            return PartialView("_DelayPayMemo");
        }

        #endregion
    }
}