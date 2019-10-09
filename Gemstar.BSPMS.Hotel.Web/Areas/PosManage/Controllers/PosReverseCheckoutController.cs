using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF.PayManage;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Services.ReportManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosCashier;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosReverseCheckout;
using Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models;
using Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Models.ResOrderFolio;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos反结
    /// </summary>
    [AuthPage(ProductType.Pos, "p20005")]
    public class PosReverseCheckoutController : BaseController
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            ViewBag.Version = CurrentVersion;
            return View();
        }

        #region 业务逻辑

        /// <summary>
        /// 反结
        /// </summary>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Details)]
        public ActionResult ReverseCheckout(ReverseCheckoutViewModel model)
        {
            var billService = GetService<IPosBillService>();
            var bill = billService.Get(model.Billid);

            var service = GetService<IPosBillDetailService>();
            var billDetailList = service.GetBillDetailByDcFlag(CurrentInfo.HotelId, model.Billid);

            var itemService = GetService<IPosItemService>();

            if (billDetailList != null && billDetailList.Count > 0)
            {


                int count = 0;
                bool status = false;

                foreach (var billDetail in billDetailList.Where(w => w.DcFlag == PosItemDcFlag.C.ToString() && w.Status < 50).ToList())
                {
                    try
                    {
                        var posItem = itemService.Get(billDetail.Itemid);
                        if (!string.IsNullOrWhiteSpace(billDetail.SettleTransno))
                        {
                            count += 1;
                            var jsonResult = Refund(billDetail, posItem);
                            var result = jsonResult.Data as JsonResultData;
                            if (result.Success == true)
                            {
                                status = true;
                            }
                            else
                            {
                                return Json(result);
                            }
                        }
                        else if (!string.IsNullOrEmpty(posItem.PayType) && posItem.PayType.Equals("house", StringComparison.CurrentCultureIgnoreCase))
                        {
                            count += 1;

                            var jsonResult = CancelRoomAccount(billDetail);
                            var result = jsonResult.Data as JsonResultData;
                            if (result.Success == true)
                            {
                                status = true;
                            }
                            else
                            {
                                return Json(result);
                            }
                        }
                        else if (!string.IsNullOrEmpty(posItem.PayType) && posItem.PayType.Equals("mbrCard", StringComparison.CurrentCultureIgnoreCase))
                        {
                            count += 1;
                            var jsonResult = CancelMbrCard(billDetail);
                            var result = jsonResult.Data as JsonResultData;
                            if (result.Success == true)
                            {
                                status = true;
                            }
                            else
                            {
                                return Json(result);
                            }
                        }
                        else if (!string.IsNullOrEmpty(posItem.PayType) && posItem.PayType.Equals("mbrLargess", StringComparison.CurrentCultureIgnoreCase))
                        {
                            count += 1;
                            var jsonResult = CancelMbrLargess(billDetail);
                            var result = jsonResult.Data as JsonResultData;
                            if (result.Success == true)
                            {
                                status = true;
                            }
                            else
                            {
                                return Json(result);
                            }
                        }
                        else if (!string.IsNullOrEmpty(posItem.PayType) && posItem.PayType.Equals("mbrCardAndLargess", StringComparison.CurrentCultureIgnoreCase))
                        {
                            count += 1;
                            var jsonResult = CancelMbrCardAndLargess(billDetail);
                            var result = jsonResult.Data as JsonResultData;
                            if (result.Success == true)
                            {
                                status = true;
                            }
                            else
                            {
                                return Json(result);
                            }
                        }
                        else if (!string.IsNullOrEmpty(posItem.PayType) && posItem.PayType.Equals("corp", StringComparison.CurrentCultureIgnoreCase))
                        {
                            count += 1;
                            var jsonResult = CancelContractUnit(billDetail);
                            var result = jsonResult.Data as JsonResultData;
                            if (result.Success == true)
                            {
                                status = true;
                            }
                            else
                            {
                                return Json(result);
                            }
                        }
                        else if (!string.IsNullOrEmpty(posItem.PayType) && posItem.PayType.Equals("PrePay", StringComparison.CurrentCultureIgnoreCase))
                        {
                            count += 1;
                            try
                            {
                                //定金反结。修改押金付款的状态
                                var prePayService = GetService<IYtPrepayService>();
                                var preList = prePayService.GetModelByPaidNo(CurrentInfo.HotelId, billDetail.Id.ToString());
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
                                    if (_preModel!=null)
                                    {
                                        _preModel.IsClear = 0;  //修改成未结清
                                        prePayService.Update(_preModel, new YtPrepay());
                                        prePayService.Commit();
                                    }
                                    status = true;
                                }


                            }
                            catch (Exception ex)
                            {

                                return Json(JsonResultData.Failure(ex.Message.ToString()));
                            }



                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(JsonResultData.Failure(ex));
                    }
                }

                if (status == true || count == 0)
                {
                    var tabStatusService = GetService<IPosTabStatusService>();
                    var tabStatus = tabStatusService.Get(bill.Tabid);

                    #region 取消付款

                    if (string.IsNullOrWhiteSpace(model.Id))
                    {
                        foreach (var billDetail in billDetailList.Where(w => (w.DcFlag == PosItemDcFlag.C.ToString() || w.Isauto == (byte)PosBillDetailIsauto.抹零) && w.Status < 50).ToList())
                        {
                            try
                            {
                                var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, CanReason = billDetail.CanReason };

                                if (billDetail.Isauto == (byte)PosBillDetailIsauto.抹零)
                                {
                                    billDetail.Dueamount = 0;
                                    billDetail.Amount = 0;
                                }

                                billDetail.IsCheck = false;
                                billDetail.CanReason = model.CanReason;
                                billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                                billDetail.ModiUser = CurrentInfo.UserName;
                                billDetail.ModiDate = DateTime.Now;

                                AddOperationLog(OpLogType.Pos账单付款明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + oldBillDetail.ItemName + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, billDetail.Billid);

                                service.Update(billDetail, new PosBillDetail());
                                service.Commit();
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonResultData.Failure(ex));
                            }
                        }
                    }

                    #endregion 取消付款

                    #region 消费项目付款信息清空

                    foreach (var detail in billDetailList.Where(w => w.DcFlag == PosItemDcFlag.D.ToString() && w.Isauto != (byte)PosBillDetailIsauto.抹零).ToList())
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

                    #endregion 消费项目付款信息清空

                    #region 修改账单

                    var oldBill = new PosBill() { Status = bill.Status };

                    bill.Status = (byte)PosBillStatus.开台;
                    bill.CashMemo = (bill.CashMemo + "；" + model.CanReason).Trim('；');

                    //查询餐台是否被占用
                    var searList = billService.GetSmearListByClearTab(CurrentInfo.HotelId, bill.Tabid);
                    //反结修改餐台号
                    if (searList != null && searList.Count > 0 && tabStatus != null && tabStatus.TabNo == bill.TabNo)
                    {
                        bill.TabNo = bill.TabNo + "Z";
                    }

                    AddOperationLog(OpLogType.Pos账单修改, "客人姓名：" + oldBill.Name + "，状态：" + oldBill.Status + " -> " + bill.Status, bill.Billid);

                    billService.Update(bill, new PosBill());
                    billService.Commit();

                    #endregion 修改账单

                    #region 修改餐台状态

                    if (tabStatus != null && tabStatus.TabStatus == (byte)PosTabStatusEnum.空净)
                    {
                        var oldTabStatus = new PosTabStatus() { TabName = tabStatus.TabName, TabStatus = tabStatus.TabStatus };
                        tabStatus.TabStatus = (byte)PosTabStatusEnum.就座;
                        tabStatus.OpTabid = bill.Billid;
                        tabStatus.OpenRecord = bill.BillDate;
                        tabStatus.OpenGuest = bill.IGuest;
                        AddOperationLog(OpLogType.Pos餐台状态修改, "台号：" + tabStatus.Tabid + "，名称：" + oldTabStatus.TabName + "，状态：" + oldTabStatus.TabStatus + " -> " + tabStatus.TabStatus, tabStatus.TabNo);
                        tabStatusService.Update(tabStatus, new PosTabStatus());
                        tabStatusService.Commit();
                    }

                    #endregion 修改餐台状态

                    #region 修改抹零数据

                    foreach (var billDetail in billDetailList.Where(w => w.DcFlag == PosItemDcFlag.D.ToString() && w.Isauto == (byte)PosBillDetailIsauto.抹零 && w.Status < 50).ToList())
                    {
                        try
                        {
                            var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, CanReason = billDetail.CanReason };

                            billDetail.Dueamount = 0;
                            billDetail.Amount = 0;

                            billDetail.IsCheck = false;
                            billDetail.CanReason = model.CanReason;
                            billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                            billDetail.ModiUser = CurrentInfo.UserName;
                            billDetail.ModiDate = DateTime.Now;

                            AddOperationLog(OpLogType.Pos账单付款明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + oldBillDetail.ItemName + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, billDetail.Billid);

                            service.Update(billDetail, new PosBillDetail());
                            service.Commit();
                        }
                        catch (Exception ex)
                        {
                            return Json(JsonResultData.Failure(ex));
                        }
                    }

                    #endregion 修改抹零数据

                    PosCommon common = new PosCommon();

                    //调用电子发票接口 
                    common.RepealEInvoice(bill.Billid);


                    return Json(JsonResultData.Successed(""));
                }
            }
            return Json(JsonResultData.Failure("反结失败，请稍后重试！"));
        }

        /// <summary>
        /// 给指定的账务记录进行退款
        /// 首先检测是否已经退款，已经退款的则直接给出提示
        /// 再检测支付方式，如果是微信支付和支付宝支付的，则发起相应的退款申请，退款申请失败则直接给出提示
        /// 如果退款成功或者不是微信支付和支付宝支付的，则直接更改充值记录的退款标志，同时插入一笔对应金额的负数来冲减余额
        /// </summary>
        /// <param name="id">要进行退款的账务id</param>
        /// <returns>退款结果</returns>
        [HttpPost]
        [AuthButton(AuthFlag.RefundFolio)]
        public JsonResult Refund(PosBillDetail billDetail, PosItem item)
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
        /// 取消房账挂账
        /// </summary>
        /// <param name="posBillDetail"></param>
        /// <returns></returns>
        public JsonResult CancelRoomAccount(PosBillDetail posBillDetail)
        {
            var billService = GetService<IPosBillService>();
            var bill = billService.Get(posBillDetail.Billid);

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(bill.Refeid);

            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var itemService = GetService<IPosItemService>();
            var item = itemService.Get(posBillDetail.Itemid);

            var posOutletCode = string.IsNullOrWhiteSpace(pos.CodeIn) ? pos.Code : pos.CodeIn;
            if (!string.IsNullOrEmpty(item.CodeIn))
            {
                posOutletCode = item.CodeIn;
            }
            if (!string.IsNullOrEmpty(refe.CodeIn))
            {
                posOutletCode = refe.Code;
            }

            var service = GetService<IPosBillDetailService>();
            string xmlStr = "<?xml version='1.0' encoding='gbk' ?>"
                + "<RealOperate>"
                + "<XType>" + "JxdBSPms" + "</XType>"
                + "<OpType>" + "房账挂账取消" + "</OpType>"
                + "<RoomFolio>"
                + "<hid>" + CurrentInfo.HotelId + "</hid>"
                + "<refNo>" + posBillDetail.Id + "</refNo>"
                + "<outletCode>" + posOutletCode + "</outletCode>"
                + "<remark>[POS客账号：" + bill.BillNo + ";收银点：" + CurrentInfo.PosName + ";金额：" + posBillDetail.Amount + ";酒店ID：" + CurrentInfo.HotelId + "]</remark>"
                + "<operator>" + CurrentInfo.UserName + "</operator>"
                + "</RoomFolio>"
                + "</RealOperate>";

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
                                    if (node.Name == "RC")
                                    {
                                        if (Convert.ToInt32(node.FirstChild.Value) == 0)
                                        {
                                            return Json(JsonResultData.Successed(""));
                                        }
                                        else
                                        {
                                            return Json(JsonResultData.Failure(doc["RoomFolio"]["Rows"]["Row"]["ErrMsg"].InnerText));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Json(JsonResultData.Failure("取消房账挂账失败，请稍后重试！"));
        }

        /// <summary>
        /// 取消会员储值支付
        /// </summary>
        /// <param name="posBillDetail"></param>
        /// <returns></returns>
        public JsonResult CancelMbrCard(PosBillDetail posBillDetail)
        {
            var billService = GetService<IPosBillService>();
            var bill = billService.Get(posBillDetail.Billid);

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(bill.Refeid);

            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var itemService = GetService<IPosItemService>();
            var item = itemService.Get(posBillDetail.Itemid);

            var posOutletCode = string.IsNullOrWhiteSpace(pos.CodeIn) ? pos.Code : pos.CodeIn;
            if (!string.IsNullOrEmpty(item.CodeIn))
            {
                posOutletCode = item.CodeIn;
            }
            if (!string.IsNullOrEmpty(refe.CodeIn))
            {
                posOutletCode = refe.Code;
            }

            var service = GetService<IPosBillDetailService>();
            string xmlStr = "<?xml version='1.0' encoding='gbk'?>"
                + "<RealOperate>"
                + "<XType>" + "JxdBSPms" + "</XType>"
                + "<OpType>" + "会员交易取消" + "</OpType>"
                + "<ProfileCa>"
                    + "<OutletCode>" + posOutletCode + "</OutletCode>"
                    + "<HotelCode>" + CurrentInfo.HotelId + "</HotelCode>"
                + "<Amount>" + (0 - posBillDetail.Amount) + "</Amount>"
                + "<RefNo>" + posBillDetail.Id + "</RefNo>"
                + "<Remark>[POS客账号：" + bill.BillNo + ";收银点：" + CurrentInfo.PosName + ";金额：" + posBillDetail.Amount + ";酒店ID：" + CurrentInfo.HotelId + "]</Remark>"
                + "<Creator>" + CurrentInfo.UserName + "</Creator>"
                + "</ProfileCa>"
                + "</RealOperate>";
            var xmlInfo = service.RealOperate(CurrentInfo.HotelId, "", xmlStr);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlInfo);
            Dictionary<string, string> xmls = new Dictionary<string, string>();
            if (doc != null)
            {
                if (doc["ReturnMessage"] != null)
                {
                    foreach (XmlNode node in doc["ReturnMessage"])
                    {
                        if (node != null && node.Name != null && node.FirstChild != null)
                        {
                            if (node.Name == "MessageNo")
                            {
                                if (Convert.ToInt32(node.FirstChild.Value) == 1)
                                {
                                    return Json(JsonResultData.Successed(""));
                                }
                            }
                        }
                    }
                }
                else if (doc["ErrorMessage"] != null)
                {
                    if (doc["ErrorMessage"]["Message"] != null)
                    {
                        return Json(JsonResultData.Failure(doc["ErrorMessage"]["Message"].InnerText));
                    }
                }
            }
            return Json(JsonResultData.Failure("取消会员储值支付失败，请稍后重试！"));
        }

        /// <summary>
        /// 取消会员增值支付
        /// </summary>
        /// <param name="posBillDetail"></param>
        /// <returns></returns>
        public JsonResult CancelMbrLargess(PosBillDetail posBillDetail)
        {
            var billService = GetService<IPosBillService>();
            var bill = billService.Get(posBillDetail.Billid);

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(bill.Refeid);

            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var itemService = GetService<IPosItemService>();
            var item = itemService.Get(posBillDetail.Itemid);

            var posOutletCode = string.IsNullOrWhiteSpace(pos.CodeIn) ? pos.Code : pos.CodeIn;
            if (!string.IsNullOrEmpty(item.CodeIn))
            {
                posOutletCode = item.CodeIn;
            }
            if (!string.IsNullOrEmpty(refe.CodeIn))
            {
                posOutletCode = refe.Code;
            }

            var service = GetService<IPosBillDetailService>();
            string xmlStr = "<?xml version='1.0' encoding='gbk'?>"
                + "<RealOperate>"
                + "<XType>" + "JxdBSPms" + "</XType>"
                + "<OpType>" + "会员交易取消" + "</OpType>"
                + "<ProfileCa>"
                    + "<OutletCode>" + posOutletCode + "</OutletCode>"
                    + "<HotelCode>" + CurrentInfo.HotelId + "</HotelCode>"
                + "<Amount>" + (0 - posBillDetail.Amount) + "</Amount>"
                + "<RefNo>" + posBillDetail.Id + "</RefNo>"
                + "<Remark>[POS客账号：" + bill.BillNo + ";收银点：" + CurrentInfo.PosName + ";金额：" + posBillDetail.Amount + ";酒店ID：" + CurrentInfo.HotelId + "]</Remark>"
                + "<Creator>" + CurrentInfo.UserName + "</Creator>"
                + "</ProfileCa>"
                + "</RealOperate>";
            var xmlInfo = service.RealOperate(CurrentInfo.HotelId, "", xmlStr);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlInfo);
            Dictionary<string, string> xmls = new Dictionary<string, string>();
            if (doc != null)
            {
                if (doc["ReturnMessage"] != null)
                {
                    foreach (XmlNode node in doc["ReturnMessage"])
                    {
                        if (node != null && node.Name != null && node.FirstChild != null)
                        {
                            if (node.Name == "MessageNo")
                            {
                                if (Convert.ToInt32(node.FirstChild.Value) == 1)
                                {
                                    return Json(JsonResultData.Successed(""));
                                }
                            }
                        }
                    }
                }
                else if (doc["ErrorMessage"] != null)
                {
                    if (doc["ErrorMessage"]["Message"] != null)
                    {
                        return Json(JsonResultData.Failure(doc["ErrorMessage"]["Message"].InnerText));
                    }
                }
            }
            return Json(JsonResultData.Failure("取消会员增值支付失败，请稍后重试！"));
        }

        /// <summary>
        /// 取消会员储值+增值支付
        /// </summary>
        /// <param name="posBillDetail"></param>
        /// <returns></returns>
        public JsonResult CancelMbrCardAndLargess(PosBillDetail posBillDetail)
        {
            var billService = GetService<IPosBillService>();
            var bill = billService.Get(posBillDetail.Billid);

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(bill.Refeid);

            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var itemService = GetService<IPosItemService>();
            var item = itemService.Get(posBillDetail.Itemid);

            var posOutletCode = string.IsNullOrWhiteSpace(pos.CodeIn) ? pos.Code : pos.CodeIn;
            if (!string.IsNullOrEmpty(item.CodeIn))
            {
                posOutletCode = item.CodeIn;
            }
            if (!string.IsNullOrEmpty(refe.CodeIn))
            {
                posOutletCode = refe.Code;
            }

            var service = GetService<IPosBillDetailService>();
            string xmlStr = "<?xml version='1.0' encoding='gbk'?>"
                + "<RealOperate>"
                + "<XType>" + "JxdBSPms" + "</XType>"
                + "<OpType>" + "会员交易取消" + "</OpType>"
                + "<ProfileCa>"
                    + "<OutletCode>" + posOutletCode + "</OutletCode>"
                    + "<HotelCode>" + CurrentInfo.HotelId + "</HotelCode>"
                + "<Amount>" + (0 - posBillDetail.Amount) + "</Amount>"
                + "<RefNo>" + posBillDetail.Id + "</RefNo>"
                + "<Remark>[POS客账号：" + bill.BillNo + ";收银点：" + CurrentInfo.PosName + ";金额：" + posBillDetail.Amount + ";酒店ID：" + CurrentInfo.HotelId + "]</Remark>"
                + "<Creator>" + CurrentInfo.UserName + "</Creator>"
                + "</ProfileCa>"
                + "</RealOperate>";
            var xmlInfo = service.RealOperate(CurrentInfo.HotelId, "", xmlStr);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlInfo);
            Dictionary<string, string> xmls = new Dictionary<string, string>();
            if (doc != null)
            {
                if (doc["ReturnMessage"] != null)
                {
                    foreach (XmlNode node in doc["ReturnMessage"])
                    {
                        if (node != null && node.Name != null && node.FirstChild != null)
                        {
                            if (node.Name == "MessageNo")
                            {
                                if (Convert.ToInt32(node.FirstChild.Value) == 1)
                                {
                                    return Json(JsonResultData.Successed(""));
                                }
                            }
                        }
                    }
                }
                else if (doc["ErrorMessage"] != null)
                {
                    if (doc["ErrorMessage"]["Message"] != null)
                    {
                        return Json(JsonResultData.Failure(doc["ErrorMessage"]["Message"].InnerText));
                    }
                }
            }
            return Json(JsonResultData.Failure("取消会员储值+增值支付失败，请稍后重试！"));
        }

        /// <summary>
        /// 取消合约单位挂账
        /// </summary>
        /// <param name="posBillDetail"></param>
        /// <returns></returns>
        public JsonResult CancelContractUnit(PosBillDetail posBillDetail)
        {
            var billService = GetService<IPosBillService>();
            var bill = billService.Get(posBillDetail.Billid);

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(bill.Refeid);

            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var itemService = GetService<IPosItemService>();
            var item = itemService.Get(posBillDetail.Itemid);

            var posOutletCode = string.IsNullOrWhiteSpace(pos.CodeIn) ? pos.Code : pos.CodeIn;
            if (!string.IsNullOrEmpty(item.CodeIn))
            {
                posOutletCode = item.CodeIn;
            }
            if (!string.IsNullOrEmpty(refe.CodeIn))
            {
                posOutletCode = refe.Code;
            }

            var service = GetService<IPosBillDetailService>();
            string xmlStr = "<?xml version='1.0' encoding='gbk' ?>"
                + "<RealOperate>"
                    + "<XType>JxdBSPms</XType>"
                    + "<OpType>合约单位挂账取消</OpType>"
                    + "<CompanyFolio>"
                        + "<hid>" + CurrentInfo.HotelId + "</hid>"
                        + "<refNo>" + posBillDetail.Id + "</refNo>"
                        + "<outletCode>" + posOutletCode + "</outletCode>"
                        + "<operator>" + CurrentInfo.UserName + "</operator>"
                        + "<remark>[POS客账号：" + bill.BillNo + ";收银点：" + CurrentInfo.PosName + ";金额：" + posBillDetail.Amount + ";酒店ID：" + CurrentInfo.HotelId + "]</remark>"
                    + "</CompanyFolio>"
                + "</RealOperate>";
            var xmlInfo = service.RealOperate(CurrentInfo.HotelId, "", xmlStr);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlInfo);
            Dictionary<string, string> xmls = new Dictionary<string, string>();
            if (doc != null)
            {
                if (doc["CompanyFolio"] != null)
                {
                    if (doc["CompanyFolio"]["Rows"] != null)
                    {
                        if (doc["CompanyFolio"]["Rows"]["Row"] != null)
                        {
                            foreach (XmlNode node in doc["CompanyFolio"]["Rows"]["Row"])
                            {
                                if (node != null && node.Name != null && node.FirstChild != null)
                                {
                                    if (node.Name == "RC")
                                    {
                                        if (Convert.ToInt32(node.FirstChild.Value) == 0)
                                        {
                                            return Json(JsonResultData.Successed(""));
                                        }
                                        else
                                        {
                                            return Json(JsonResultData.Failure(doc["CompanyFolio"]["Rows"]["Row"]["ErrMsg"].InnerText));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Json(JsonResultData.Failure("取消合约单位挂账失败，请稍后重试！"));
        }

        /// <summary>
        /// 反结支付方式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.Print)]
        public ActionResult ReversePayment(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                try
                {
                    var service = GetService<IPosBillDetailService>();
                    var billDetail = service.GetEntity(CurrentInfo.HotelId, Convert.ToInt64(id));
                    if (billDetail != null)
                    {
                        var itemService = GetService<IPosItemService>();
                        var item = itemService.Get(billDetail.Itemid);


                        if (string.IsNullOrWhiteSpace(item.PayType) || item.PayType == "no")
                        {
                            var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, CanReason = billDetail.CanReason };

                            billDetail.IsCheck = false;
                            billDetail.CanReason = "反结付款方式";
                            billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                            billDetail.ModiUser = CurrentInfo.UserName;
                            billDetail.ModiDate = DateTime.Now;

                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + oldBillDetail.ItemName + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, billDetail.Billid);

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
                                billDetail.CanReason = "反结付款方式";
                                billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                                billDetail.ModiUser = CurrentInfo.UserName;
                                billDetail.ModiDate = DateTime.Now;

                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + oldBillDetail.ItemName + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, billDetail.Billid);

                                service.Update(billDetail, new PosBillDetail());
                                service.Commit();
                            }
                            else
                            {
                                return Json(JsonResultData.Failure("退款失败：" + result.Data));
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
                                    billDetail.CanReason = "反结付款方式";
                                    billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                                    billDetail.ModiUser = CurrentInfo.UserName;
                                    billDetail.ModiDate = DateTime.Now;

                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + oldBillDetail.ItemName + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, billDetail.Billid);

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
                                    billDetail.CanReason = "反结付款方式";
                                    billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                                    billDetail.ModiUser = CurrentInfo.UserName;
                                    billDetail.ModiDate = DateTime.Now;

                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + oldBillDetail.ItemName + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, billDetail.Billid);

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
                                    billDetail.CanReason = "反结付款方式";
                                    billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                                    billDetail.ModiUser = CurrentInfo.UserName;
                                    billDetail.ModiDate = DateTime.Now;

                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + oldBillDetail.ItemName + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, billDetail.Billid);

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
                                    billDetail.CanReason = "反结付款方式";
                                    billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                                    billDetail.ModiUser = CurrentInfo.UserName;
                                    billDetail.ModiDate = DateTime.Now;

                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + oldBillDetail.ItemName + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, billDetail.Billid);

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
                                    billDetail.CanReason = "反结付款方式";
                                    billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                                    billDetail.ModiUser = CurrentInfo.UserName;
                                    billDetail.ModiDate = DateTime.Now;

                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + oldBillDetail.ItemName + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, billDetail.Billid);

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

                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, billDetail.Billid);

                                service.Update(billDetail, new PosBillDetail());
                                service.Commit();

                            }
                        }
                        else
                        {
                            var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, CanReason = billDetail.CanReason };

                            billDetail.IsCheck = false;
                            billDetail.CanReason = "反结付款方式";
                            billDetail.Status = (byte)PosBillDetailStatus.反结取消;
                            billDetail.ModiUser = CurrentInfo.UserName;
                            billDetail.ModiDate = DateTime.Now;

                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + oldBillDetail.ItemName + "，状态：" + oldBillDetail.Status + " -> " + billDetail.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + billDetail.CanReason, billDetail.Billid);

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
                                    temp.CanReason = "反结付款方式";
                                    temp.Status = (byte)PosBillDetailStatus.反结取消;
                                    temp.ModiUser = CurrentInfo.UserName;
                                    temp.ModiDate = DateTime.Now;

                                    AddOperationLog(OpLogType.Pos账单付款明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + oldBillDetail.ItemName + "，状态：" + oldBillDetail.Status + " -> " + temp.Status + "，取消原因：" + oldBillDetail.CanReason + "->" + temp.CanReason, temp.Billid);

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

                        #endregion 消费项目付款信息清空

                        #region 修改账单

                        var billService = GetService<IPosBillService>();
                        var bill = billService.GetEntity(CurrentInfo.HotelId, billDetail.Billid);
                        var oldBill = new PosBill() { Status = bill.Status };

                        //查询餐台列表
                        var tabStatusService = GetService<IPosTabStatusService>();
                        var tabStatus = tabStatusService.Get(bill.Tabid);
                        //查询餐台是否被占用
                        var searList = billService.GetSmearListByClearTab(CurrentInfo.HotelId, bill.Tabid);

                        bill.Status = (byte)PosBillStatus.开台;
                        bill.CashMemo = (bill.CashMemo + "；" + "反结付款方式").Trim('；');
                        //反结在餐台后面添加一个Z
                        if (searList != null && searList.Count > 0 && tabStatus != null && tabStatus.TabNo == bill.TabNo)
                        {
                            bill.TabNo = bill.TabNo + "Z";
                        }

                        AddOperationLog(OpLogType.Pos账单修改, "客人姓名：" + oldBill.Name + "，状态：" + oldBill.Status + " -> " + bill.Status, bill.Billid);

                        billService.Update(bill, new PosBill());
                        billService.Commit();

                        #endregion 修改账单

                        #region 修改餐台状态

                        if (tabStatus != null && tabStatus.TabStatus == (byte)PosTabStatusEnum.空净)
                        {
                            var oldTabStatus = new PosTabStatus() { TabName = tabStatus.TabName, TabStatus = tabStatus.TabStatus };
                            tabStatus.TabStatus = (byte)PosTabStatusEnum.就座;
                            AddOperationLog(OpLogType.Pos餐台状态修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + oldTabStatus.TabName + "，状态：" + oldTabStatus.TabStatus + " -> " + tabStatus.TabStatus, tabStatus.TabNo);
                            tabStatusService.Update(tabStatus, new PosTabStatus());
                            tabStatusService.Commit();
                        }

                        #endregion 修改餐台状态

                        PosCommon common = new PosCommon();

                        //调用电子发票接口 
                        common.RepealEInvoice(bill.Billid);
                    }
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
            return Json(JsonResultData.Successed(""));
        }

        #endregion 业务逻辑

        #region 获取分布视图

        /// <summary>
        /// 反结账单列表视图
        /// </summary>
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
                list = service.GetPosBillReverseCheckout(CurrentInfo.HotelId, CurrentInfo.PosId, pos.Business, tabNo);
            }

            return PartialView("_PosBillList", list);
        }

        #endregion 获取分布视图

        #region 获取列表

        /// <summary>
        /// 获取账单明细付款列表
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListItemsForBillDetailByBill([DataSourceRequest]DataSourceRequest request, CashierViewModel model)
        {
            var service = GetService<IPosBillDetailService>();
            var list = service.GetBillDetailForPaymentByBillid(CurrentInfo.HotelId, model.Billid);
            return Json(list.ToDataSourceResult(request));
        }

        #endregion 获取列表

        #region 打印埋脚

        [AuthButton(AuthFlag.Print)]
        public ActionResult PrintBillPayMethod(ReportQueryModel model, string print)
        {
            PosCommon common = new PosCommon();
            var controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            return common.PrintBillPayMethod(model, print, controller);
        }

        #endregion 打印埋脚

        #region 打印账单

        [HttpPost]
        [AuthButton(AuthFlag.Export)]
        public ActionResult AddQueryParaTemp(ReportQueryModel model, string print, string Flag)
        {
            PosCommon common = new PosCommon();
            var controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            return common.AddQueryParaTemp(model, print, Flag, controller);
        }

        #endregion 打印账单

        #region 反结提示

        //反结验证权限
        [AuthButton(AuthFlag.Details)]
        public ActionResult ReverseCheck()
        {
            return Json(JsonResultData.Successed());
        }

        #endregion 反结提示

        #region 反结验证付款方式是否可以反结

        /// <summary>
        /// 付款方式是否可以反结
        /// </summary>
        /// <param name="billid">账单ID</param>
        /// <param name="id">付款方式ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult PayWayIsRepay(string billid, string id)
        {
            var itemService = GetService<IPosItemService>();

            if (!string.IsNullOrEmpty(billid))
            {
                var service = GetService<IPosBillDetailService>();
                var billDetailList = service.GetBillDetailByDcFlag(CurrentInfo.HotelId, billid);
                if (billDetailList != null && billDetailList.Count > 0)
                {
                    //账单反结
                    bool isrepay = true;    //是否可以反结
                    string payWayStr = "";  //付款方式
                                            //判断付款方式是否可以反结
                    foreach (var billDetail in billDetailList.Where(w => w.DcFlag == PosItemDcFlag.C.ToString() && w.Status < 50).ToList())
                    {
                        var posItem = itemService.Get(billDetail.Itemid);
                        if (posItem.IsRepay == null || posItem.IsRepay == false)
                        {
                            isrepay = false;
                            payWayStr += posItem.Cname + ",";

                        }
                    }
                    if (!isrepay)
                    {
                        return Json(JsonResultData.Failure("反结失败，" + payWayStr + "付款方式不能反结！"));
                    }
                }


            }
            else if (!string.IsNullOrEmpty(id))
            {
                //账单明细反结
                var service = GetService<IPosBillDetailService>();
                var billDetail = service.GetEntity(CurrentInfo.HotelId, Convert.ToInt64(id));
                if (billDetail != null)
                {
                    var item = itemService.Get(billDetail.Itemid);
                    if (item.IsRepay == null || item.IsRepay == false)
                    {
                        return Json(JsonResultData.Failure("反结失败：" + item.Cname + "不能进行反结"));
                    }
                }
            }

            return Json(JsonResultData.Successed());
        }
        #endregion
    }
}