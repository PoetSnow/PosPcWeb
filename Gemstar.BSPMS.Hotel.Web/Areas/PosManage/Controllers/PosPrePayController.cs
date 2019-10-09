using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF.PayManage;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosReserve;
using Gemstar.BSPMS.Hotel.Web.Controllers;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{

    /// <summary>
    /// 收银--定金
    /// </summary>
    [AuthPage(ProductType.Pos, "p200018")]
    public class PosPrePayController : BaseEditInWindowController<YtPrepay, IYtPrepayService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            var service = GetService<IPosPosService>();
            var pos = service.Get(CurrentInfo.PosId);

            SetCommonQueryValues("up_pos_list_PrePay", "@h99hid=" + CurrentInfo.HotelId + "&@p0101_营业日 =" + pos.Business);
            return View();
        }

        #region 添加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var shiftService = GetService<IPosShiftService>();
            var shift = shiftService.Get(pos.ShiftId);


            var service = GetService<IYtPrepayService>();
            //重新生成定金的单号
            var billNo = service.GetBillNo(CurrentInfo.HotelId, Convert.ToDateTime(pos.Business), CurrentInfo.ModuleCode);

            var modle = new AddPrepayViewModel()
            {
                PosName = pos.Name,
                DBusiness = pos.Business,
                ShiftName = shift.Name,
                Shiftid = shift.Id,
                BillNo = billNo,
                PosNo = pos.Id.TrimEnd(),
                _DBusiness = Convert.ToDateTime(pos.Business).ToShortDateString(),
                OpenFlag = "B"
            };
            return PartialView("_AddPrepay", modle);
        }
        #endregion

        #region 删除
        //首先判断是否有勾选删除的权限。再判断如果删除的数据不是跨营业日       

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            var service = GetService<IYtPrepayService>();
            var model = service.Get(Guid.Parse(id));

            //判断是否存在定金买单或者
            var isPay = service.isExistsPay(CurrentInfo.HotelId, model.BillNo);
            if (isPay)
            {
                return Json(JsonResultData.Failure("存在押金付款或者押金退款的不能进行删除操作！"));
            }

            //取出当前营业日以及班次
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            //当前营业日，班次不等于操作的定金营业日与班次

            var isBussis = true;
            if (pos.Business != model.DBusiness || pos.ShiftId != model.Shiftid)
            {
                var authService = GetService<IAuthCheck>();
                //跨营业日修改
                isBussis = authService.HasAuth(CurrentInfo.UserId, "p200018", (long)AuthFlag.Export, CurrentInfo.HotelId);
            }
            // 判断该笔定金是否有使用(此功能等定金买单功能实现之后再来判断)

            //符合条件 进行删除
            if (isBussis)
            {
                //删除 首先处理退款，其次把定金状态改成取消状态
                var itemService = GetService<IPosItemService>();    //付款方式
                var item = itemService.Get(model.PayModeNo);


                if (string.IsNullOrWhiteSpace(item.PayType) || item.PayType == "no")
                {
                    UpdateYtModel(model);
                }
                else if (!string.IsNullOrWhiteSpace(item.PayType) && (item.PayType.Equals("AliBarcode", StringComparison.CurrentCultureIgnoreCase) || item.PayType.Equals("AliQrcode", StringComparison.CurrentCultureIgnoreCase) || item.PayType.Equals("WxBarcode", StringComparison.CurrentCultureIgnoreCase) || item.PayType.Equals("WxQrcode", StringComparison.CurrentCultureIgnoreCase)))
                {

                    //微信退款或者支付宝退款
                    var jsonResult = Refund(model, item);
                    var result = jsonResult.Data as JsonResultData;
                    if (result.Success == true)
                    {
                        UpdateYtModel(model);
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

                    }
                    else if (!string.IsNullOrEmpty(item.PayType) && item.PayType.Equals("mbrCard", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var jsonResult = CancelMbrCard(model);
                        var result = jsonResult.Data as JsonResultData;
                        if (result.Success == true)
                        {
                            UpdateYtModel(model);
                        }
                        else
                        {
                            return Json(JsonResultData.Failure("退款失败：" + result.Data));
                        }
                    }
                    else if (!string.IsNullOrEmpty(item.PayType) && item.PayType.Equals("mbrLargess", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var jsonResult = CancelMbrLargess(model);
                        var result = jsonResult.Data as JsonResultData;
                        if (result.Success == true)
                        {
                            UpdateYtModel(model);
                        }
                        else
                        {
                            return Json(JsonResultData.Failure("退款失败：" + result.Data));
                        }
                    }
                    else if (!string.IsNullOrEmpty(item.PayType) && item.PayType.Equals("mbrCardAndLargess", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var jsonResult = CancelMbrCardAndLargess(model);
                        var result = jsonResult.Data as JsonResultData;
                        if (result.Success == true)
                        {
                            UpdateYtModel(model);
                        }
                        else
                        {
                            return Json(JsonResultData.Failure("退款失败：" + result.Data));
                        }
                    }
                    else if (!string.IsNullOrEmpty(item.PayType) && item.PayType.Equals("corp", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var jsonResult = CancelContractUnit(model);
                        var result = jsonResult.Data as JsonResultData;
                        if (result.Success == true)
                        {
                            UpdateYtModel(model);
                        }
                        else
                        {
                            return Json(JsonResultData.Failure("退款失败：" + result.Data));
                        }
                    }
                }
                else
                {
                    UpdateYtModel(model);
                }
                return Json(JsonResultData.Successed());
            }
            else
            {
                return Json(JsonResultData.Failure("权限不足！"));
            }

        }

        private void UpdateYtModel(YtPrepay model)
        {
            var service = GetService<IYtPrepayService>();
            var oldModel = new YtPrepay() { IPrepay = model.IPrepay };

            model.IPrepay = (byte)PrePayStatus.取消;
            model.ModifiCator = CurrentInfo.UserName;
            model.ModifiedDate = DateTime.Now;

            AddOperationLog(OpLogType.Pos定金修改, "单号：" + model.Id + "，状态：" + oldModel.IPrepay + " -> " + model.IPrepay + "，操作员：" + CurrentInfo.UserName, model.BillNo);

            service.Update(model, oldModel);
            service.Commit();
        }

        /// <summary>
        /// 支付宝，微信退款
        /// </summary>
        /// <param name="model"></param>
        /// <param name="item"></param>
        /// <returns></returns>

        private JsonResult Refund(YtPrepay model, PosItem item)
        {
            if (model.Amount <= 0)
            {
                return Json(JsonResultData.Failure("退款金额必须大于0！"));
            }
            string folioPosItemAction = item.PayType;//付款账务，付款方式，处理方式
            string folioPosItemActionJsonPara = GetFolioPosItemActionJsonPara(folioPosItemAction, model);//组装JSON
            try
            {
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
                    FolioTransId = model.PayTransno,
                    Statu = PayStatu.Successed.ToString(),
                    Callback = "",
                    QrCodeUrl = "",
                    QueryTransId = "",
                    DCFlag = "C"
                };

                if (payService != null)
                {
                    folioPosItemActionJsonPara = GetFolioPosItemActionJsonPara(folioPosItemAction, model);//组装JSON
                    var afterPayResult = payService.DoPayAfterSaveFolio(PayProductType.PosPayment, model.PayTransno, folioPosItemActionJsonPara);
                    returnResult.Statu = afterPayResult.Statu.ToString();
                    returnResult.Callback = afterPayResult.Callback;
                    returnResult.QrCodeUrl = afterPayResult.QrCodeUrl;
                    returnResult.QueryTransId = afterPayResult.QueryTransId;
                }
                return Json(JsonResultData.Successed(returnResult));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
            return Json(JsonResultData.Successed());
        }


        /// <summary>
        /// 获取退款JSON字符串
        /// </summary>
        /// <param name="folioPosItemAction">处理方式</param>
        /// <param name="model">选中要退款的账务</param>
        /// <param name="amount">退款金额</param>
        /// <param name="newTransId">退款操作生成新的退款账务ID</param>
        /// <returns></returns>
        private string GetFolioPosItemActionJsonPara(string folioPosItemAction, YtPrepay model)
        {
            string folioPosItemActionJsonPara = "";
            if (folioPosItemAction == "AliBarcode" || folioPosItemAction == "AliQrcode")
            {
                var para = new
                {
                    originPayTransId = model.PayTransno,//选中要退款的账务的主键ID
                    refundId = model.PayTransno,//退款操作生成新的退款账务ID
                    refundAmount = model.Amount,//退款金额
                    refundReason = "Pos定金退款",//退款原因
                };
                folioPosItemActionJsonPara = Newtonsoft.Json.JsonConvert.SerializeObject(para);
            }
            else if (folioPosItemAction == "WxBarcode" || folioPosItemAction == "WxQrcode")
            {
                var para = new
                {
                    outTradeNo = model.PayTransno,//选中要退款的账务的主键ID
                    outRefundNo = model.PayTransno,//退款操作生成新的退款账务ID
                    totalFee = model.Amount,//原始总金额
                    refundFee = model.Amount,//退款金额
                    opUserId = CurrentInfo.UserName,//操作员
                };
                folioPosItemActionJsonPara = Newtonsoft.Json.JsonConvert.SerializeObject(para);
            }
            return folioPosItemActionJsonPara;
        }

        /// <summary>
        /// 取消会员储值支付
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult CancelMbrCard(YtPrepay model)
        {

            var refeService = GetService<IPosRefeService>();

            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var itemService = GetService<IPosItemService>();
            var item = itemService.Get(model.PayModeNo);

            var refe = refeService.GetRefeByPosAndModule(CurrentInfo.HotelId, pos.Id, CurrentInfo.ModuleCode).FirstOrDefault();

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
                + "<Amount>" + (0 - model.Amount) + "</Amount>"
                + "<RefNo>" + model.Id + "</RefNo>"
                + "<Remark>[POS客账号：" + model.BillNo + ";收银点：" + CurrentInfo.PosName + ";金额：" + model.Amount + ";酒店ID：" + CurrentInfo.HotelId + "类型：定金]</Remark>"
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
        /// 取消会员储值+增值支付
        /// </summary>
        /// <param name="posBillDetail"></param>
        /// <returns></returns>
        public JsonResult CancelMbrCardAndLargess(YtPrepay model)
        {
            var refeService = GetService<IPosRefeService>();

            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var itemService = GetService<IPosItemService>();
            var item = itemService.Get(model.PayModeNo);

            var refe = refeService.GetRefeByPosAndModule(CurrentInfo.HotelId, pos.Id, CurrentInfo.ModuleCode).FirstOrDefault();

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
                + "<Amount>" + (0 - model.Amount) + "</Amount>"
                + "<RefNo>" + model.Id + "</RefNo>"
                + "<Remark>[POS客账号：" + model.BillNo + ";收银点：" + CurrentInfo.PosName + ";金额：" + model.Amount + ";酒店ID：" + CurrentInfo.HotelId + ";类型：定金]</Remark>"
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
        /// 取消会员增值支付
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult CancelMbrLargess(YtPrepay model)
        {

            var refeService = GetService<IPosRefeService>();

            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var itemService = GetService<IPosItemService>();
            var item = itemService.Get(model.PayModeNo);

            var refe = refeService.GetRefeByPosAndModule(CurrentInfo.HotelId, pos.Id, CurrentInfo.ModuleCode).FirstOrDefault();

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
                + "<Amount>" + (0 - model.Amount) + "</Amount>"
                + "<RefNo>" + model.Id + "</RefNo>"
                + "<Remark>[POS客账号：" + model.BillNo + ";收银点：" + CurrentInfo.PosName + ";金额：" + model.Amount + ";酒店ID：" + CurrentInfo.HotelId + ";类型：定金]</Remark>"
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
        /// 取消合约单位挂账
        /// </summary>
        /// <param name="posBillDetail"></param>
        /// <returns></returns>
        public JsonResult CancelContractUnit(YtPrepay model)
        {
            var refeService = GetService<IPosRefeService>();

            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var itemService = GetService<IPosItemService>();
            var item = itemService.Get(model.PayModeNo);

            var refe = refeService.GetRefeByPosAndModule(CurrentInfo.HotelId, pos.Id, CurrentInfo.ModuleCode).FirstOrDefault();

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
                        + "<refNo>" + model.Id + "</refNo>"
                        + "<outletCode>" + posOutletCode + "</outletCode>"
                        + "<operator>" + CurrentInfo.UserName + "</operator>"
                        + "<remark>[POS客账号：" + model.BillNo + ";收银点：" + CurrentInfo.PosName + ";金额：" + model.Amount + ";酒店ID：" + CurrentInfo.HotelId + ";类型：定金]</remark>"
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

        #endregion


        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit()
        {
            //var posService = GetService<IPosPosService>();
            //var pos = posService.Get(CurrentInfo.PosId);

            //var shiftService = GetService<IPosShiftService>();
            //var shift = shiftService.Get(pos.ShiftId);


            //var service = GetService<IYtPrepayService>();
            ////重新生成定金的单号
            //var billNo = service.GetBillNo(CurrentInfo.HotelId, Convert.ToDateTime(pos.Business), CurrentInfo.ModuleCode);

            //var modle = new AddPrepayViewModel()
            //{
            //    PosName = pos.Name,
            //    DBusiness = pos.Business,
            //    ShiftName = shift.Name,
            //    Shiftid = shift.Id,
            //    BillNo = billNo,
            //    PosNo = pos.Id.TrimEnd(),
            //    _DBusiness = Convert.ToDateTime(pos.Business).ToShortDateString(),
            //    OpenFlag = "B"
            //};
            return PartialView("_EditPrepay");
        }
        #endregion

    }
}