using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EF.OnlineInterfaceManage;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.OnlineInterfaceManage;
using Gemstar.BSPMS.Hotel.Services.OnlineInterfaceManage.Invoice;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Services.ReportManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models;
using Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage
{
    /// <summary>
    /// 云Pos的一些公共方法
    /// </summary>
    public class PosCommon : BaseController
    {
        #region  pos打单。预览
        /// <summary>
        /// 打单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="print"></param>
        /// <param name="Flag">区分打单跟打印预览（A:打单）</param>
        /// <returns></returns>
        public JsonResult AddQueryParaTemp(ReportQueryModel model, string print, string Flag, string controller = "")
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
                            var oldBill = new PosBill();
                            AutoSetValueHelper.SetValues(billModel, oldBill);
                            //if (billModel.Status==2)
                            //{
                            //打单次数加1
                            billModel.IPrint = billModel.IPrint == null ? 1 : billModel.IPrint + 1;
                            billModel.PrintRecord = DateTime.Now;   //打单时间
                            BillService.Update(billModel, new PosBill());
                            //  BillService.AddDataChangeLog(OpLogType.Pos账单修改);
                            BillService.Commit();
                            AddOperationLog(OpLogType.Pos账单修改, "单号：" + billModel.Billid + "，打单次数：" + oldBill.IPrint + "-->" + billModel.IPrint, billModel.BillNo);
                            // }
                        }
                        //添加账单打印记录
                        AddPosPrintBill(billid, PosPrintBillStatus.账单打印, controller);
                    }
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
                        url.Append("http://").Append(System.Web.HttpContext.Current.Request.Url.Host).Append("/ReportManage");
                        url.Append("/SRBillReportView/Index")
                            .Append("?ReportCode=").Append(System.Web.HttpContext.Current.Server.UrlEncode(model.ReportCode))
                            .Append("&ParameterValues=").Append(System.Web.HttpContext.Current.Server.UrlEncode(id.Value.ToString()))
                            .Append("&ChineseName=").Append(System.Web.HttpContext.Current.Server.UrlEncode(model.ChineseName));
                        if (!string.IsNullOrWhiteSpace(print))
                        {
                            url.Append("&print=").Append(System.Web.HttpContext.Current.Server.UrlEncode(print));
                        }

                        return Json(JsonResultData.Successed(url.ToString()));
                    }
                    else
                    {
                        return Json(JsonResultData.Failure("添加失败！"));
                    }
                }
                else
                {
                    return Json(JsonResultData.Failure("参数错误！"));
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 退出验证数据是否修改
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys">键值</param>
        /// <returns></returns>
        private T LoadValues<T>(string keys)
        {
            T _values = default(T);
            var sessionKey = keys;
            if (!string.IsNullOrWhiteSpace(sessionKey))
            {
                try
                {
                    var serializer = new JavaScriptSerializer();
                    var redisConnection = MvcApplication.RedisConnection;
                    var db = redisConnection.GetDatabase();
                    var task = db.StringGetAsync(sessionKey);
                    task.Wait();
                    var valueStr = task.Result;
                    if (!string.IsNullOrWhiteSpace(valueStr))
                    {
                        _values = serializer.Deserialize<T>(valueStr);
                    }
                }
                catch (Exception ex)
                {
                    MvcApplication.WriteRedisException(ex);
                }
            }
            if (_values == null)
            {
                _values = default(T);
            }
            return _values;
        }

        /// <summary>
        /// 比较--两个类型一样的实体类对象的值
        /// </summary>
        /// <param name="oneT"></param>
        /// <returns></returns>
        private bool CheckTypeValue<T>(T oneT, T twoT)
        {
            #region
            bool result = true;//两个类型作比较时使用,如果有不一样的就false

            if (oneT == null || twoT == null) { return false; }//为空直接返回false
            Type typeOne = oneT.GetType();
            Type typeTwo = twoT.GetType();
            //如果两个T类型不一样  就不作比较
            if (!typeOne.Equals(typeTwo)) { return false; }
            PropertyInfo[] pisOne = typeOne.GetProperties(); //获取所有公共属性(Public)
            PropertyInfo[] pisTwo = typeTwo.GetProperties();
            //如果长度为0返回false
            if (pisOne.Length <= 0 || pisTwo.Length <= 0) { return false; }

            //如果长度不一样，返回false
            if (!(pisOne.Length.Equals(pisTwo.Length))) { return false; }

            //遍历两个T类型，遍历属性，并作比较
            for (int i = 0; i < pisOne.Length; i++)
            {
                //获取属性名
                string oneName = pisOne[i].Name;
                string twoName = pisTwo[i].Name;
                //获取属性的值
                object oneValue = pisOne[i].GetValue(oneT, null);
                object twoValue = pisTwo[i].GetValue(twoT, null);
                if (oneName == "IPrint") continue;

                //比较,只比较值类型
                if ((pisOne[i].PropertyType.IsValueType || pisOne[i].PropertyType.Name.StartsWith("String"))
                    && (pisTwo[i].PropertyType.IsValueType || pisTwo[i].PropertyType.Name.StartsWith("String")))
                {
                    if (oneValue == null && twoValue != null) { return false; }

                    if (twoValue == null && oneValue != null) { return false; }

                    if (oneValue == null && twoValue == null) { continue; }

                    if (oneValue.GetType() == typeof(DateTime))
                    {
                        //值类型,直接判断是否相等
                        if ((DateTime.Parse(oneValue.ToString()) - DateTime.Parse(twoValue.ToString())).Ticks != 0)
                            return false;
                    }
                    else
                    {
                        //值类型,直接判断是否相等
                        if (!oneValue.Equals(twoValue))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //如果对象中的属性是实体类对象，递归遍历比较
                    bool b = CheckTypeValue(oneValue, twoValue);
                    if (!b) { result = b; break; }
                }
            }
            return result;
            #endregion 退出验证数据是否修改
        }


        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys">键值</param>
        /// <param name="t"></param>
        private void SaveValues<T>(string keys, T t)
        {
            var sessionKey = keys;
            if (!string.IsNullOrWhiteSpace(sessionKey))
            {
                var serializer = new JavaScriptSerializer();
                var valueStr = ReplaceJsonDateToDateString(serializer.Serialize(t));

                var redisConnection = MvcApplication.RedisConnection;
                var db = redisConnection.GetDatabase();
                try
                {
                    db.KeyDelete(sessionKey);
                    db.StringSet(sessionKey, valueStr);
                }
                catch
                {
                    db.KeyDelete(sessionKey);
                    db.StringSet(sessionKey, valueStr);
                }
            }
        }

        /// <summary>
        ///  根据billid 保存数据到redis
        /// </summary>
        public void SetRedisBill(string Billid)
        {
            //账单主表
            var billService = GetService<IPosBillService>();
            var billSession = billService.GetBillByCancalLockTab(Billid, CurrentInfo.HotelId);
            SaveValues("billSession_" + Billid, billSession);

            //   Session["billSession"] = billSession;

            //账单明细
            var billDetailservice = GetService<IPosBillDetailService>();
            //只针对消费项目
            var billDetailList = billDetailservice.GetBillDetailByDcFlagForPosInSing(CurrentInfo.HotelId, Billid, "D");
            SaveValues("billDetailSessionList_" + Billid, billDetailList);

            //做法
            var BillDetailActionService = GetService<IPosBillDetailActionService>();
            var BillDetailActionList = BillDetailActionService.GetPosBillDetailActionByModule(CurrentInfo.HotelId, Billid);
            SaveValues("BillDetailActionSessionList_" + Billid, BillDetailActionList);
        }


        /// <summary>
        /// 验证入单界面关联的数据是否修改
        /// </summary>
        /// <param name="Billid"></param>
        /// <returns></returns>
        public bool CheckPosBillData(string Billid)
        {
            var billservice = GetService<IPosBillService>();
            var bill = billservice.Get(Billid); //得到数据库最新的数据


            var serializer = new JavaScriptSerializer();
            var valueStr = ReplaceJsonDateToDateString(serializer.Serialize(bill));


            var billSession = LoadValues<PosBill>("billSession_" + Billid);//取本地存储的数据

            var billDetailservice = GetService<IPosBillDetailService>();
            //数据库里账单明细最新数据
            var billDetailList = billDetailservice.GetBillDetailByDcFlagForPosInSing(CurrentInfo.HotelId, bill.Billid, "D");
            valueStr = ReplaceJsonDateToDateString(serializer.Serialize(billDetailList));



            //加载redis 里存储的数据
            var billDetailSessionList = LoadValues<List<up_pos_BillDetailResult>>("billDetailSessionList_" + Billid);

            var BillDetailActionService = GetService<IPosBillDetailActionService>();
            var BillDetailActionList = BillDetailActionService.GetPosBillDetailActionByModule(CurrentInfo.HotelId, bill.Billid);
            var BillDetailActionSessionList = LoadValues<List<PosBillDetailAction>>("BillDetailActionSessionList_" + Billid);

            var billFlag = true;    //主表判断状态

            //判断主表是否有差异
            if (!CheckTypeValue(bill, billSession))
            {
                billFlag = false;
            }

            //判断明细表数量是否一致
            if (billDetailList.Count != billDetailSessionList.Count)
            {
                billFlag = false;
            }
            else
            {
                /*
                 * 循环判断账单明细表的数据与session 里的数据是否有差异
                 * 有差异的返回false 。并且退出循环
                */
                // var serializer = new JavaScriptSerializer();
                for (int i = 0; i < billDetailSessionList.Count; i++)
                {
                    if (!CheckTypeValue(billDetailList[i], billDetailSessionList[i]))
                    {
                        billFlag = false;
                        break;
                    }
                }
            }


            return billFlag;
        }
        #endregion


        #region 买单视图

        /// <summary>
        /// 买单分布视图
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PartialViewResult _Payment(PaymentViewModel model)
        {
            ViewBag.Version = CurrentVersion;
            if (model != null && !string.IsNullOrWhiteSpace(model.Billid))
            {
                var paymentService = GetService<IPaymentServices>();
                var billPara = new PaymentBillPara
                {
                    BillId = model.Billid,
                    MainBillId = model.MBillid,
                    Hid = CurrentInfo.HotelId
                };
                var billInfo = paymentService.GetPaymentBillInfo(billPara);

                ViewBag.IDecPlace = billInfo.IDecPlace;     //保留位数
                ViewBag.OpenMemo = billInfo.OpenMemo;       //开台备注
                ViewBag.CashMemo = billInfo.CashMemo; //收银备注
                ViewBag.ToBePaid = billInfo.UnPaid; //未付金额

                var posService = GetService<IPosPosService>();
                var pos = posService.Get(CurrentInfo.PosId);
                ViewBag.PosName = pos.Name;  //收银点


                var pmsParaService = GetService<IPmsParaService>();
                //挂账合约单位超限额处理
                var PosCompanyQuota = pmsParaService.GetValue(CurrentInfo.HotelId, "PosCompanyQuota");
                ViewBag.PosCompanyQuota = PosCompanyQuota; //挂账合约单位超限额处理

                model.Amount = billInfo.Amount;
                model.TailDifference = billInfo.TailDifference;
                model.PageIndex = 1;
                model.PageSize = 6;

                PosCommon common = new PosCommon();
                common.SetRedisBill(model.Billid);  //保存数据到redis

                return PartialView("_Payment", model);
            }

            return PartialView("_Payment");
        }
        #endregion

        #region 买单成功打单

        /// <summary>
        /// 买单成功打单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="print"></param>
        /// <param name="Flag"></param>
        /// <param name="isPrintBill"></param>
        /// <returns></returns>
        public JsonResult AddQueryParaTempByPaySusses(ReportQueryModel model, string print, string Flag, string isPrintBill)
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

                            var oldBill = new PosBill();
                            AutoSetValueHelper.SetValues(billModel, oldBill);

                            var refeService = GetService<IPosRefeService>();    //营业点

                            PosRefe refe = refeService.Get(billModel.Refeid);   //获取营业点
                            if (refe.ITagPrintBill == PosTagPrintBill.提示是否买单 || (refe.ITagPrintBill == PosTagPrintBill.立即打印账单) || (refe.ITagPrintBill == PosTagPrintBill.提示是否打印账单 && isPrintBill == "1"))
                            {
                                billModel.IPrint = billModel.IPrint == null ? 1 : billModel.IPrint + 1;
                                billModel.PrintRecord = DateTime.Now;
                                BillService.Update(billModel, new PosBill());
                                //BillService.AddDataChangeLog(OpLogType.Pos账单修改);
                                BillService.Commit();
                                AddOperationLog(OpLogType.Pos账单修改, "单号：" + billModel.Billid + "，打单次数：" + oldBill.IPrint + "-->" + billModel.IPrint, billModel.BillNo);

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
                                    url.Append("http://").Append(System.Web.HttpContext.Current.Request.Url.Host).Append("/ReportManage");
                                    url.Append("/SRBillReportView/Index")
                                        .Append("?ReportCode=").Append(System.Web.HttpContext.Current.Server.UrlEncode(model.ReportCode))
                                        .Append("&ParameterValues=").Append(System.Web.HttpContext.Current.Server.UrlEncode(id.Value.ToString()))
                                        .Append("&ChineseName=").Append(System.Web.HttpContext.Current.Server.UrlEncode(model.ChineseName));
                                    if (!string.IsNullOrWhiteSpace(print))
                                    {
                                        url.Append("&print=").Append(System.Web.HttpContext.Current.Server.UrlEncode(print));
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
        #endregion

        #region 买单之前验证营业点设置的
        /// <summary>
        /// 买单之前验证营业点设置的 买单未打单条件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult CheckRefeITagPrintBill(PaymentViewModel model)
        {
            var billService = GetService<IPosBillService>();

            var bill = billService.Get(model.Billid);//获取账单信息

            var refeService = GetService<IPosRefeService>();    //营业点

            PosRefe refe = refeService.Get(bill == null ? "0" : bill.Refeid);   //获取营业点

            if (refe != null)
            {
                if (refe.ITagPrintBill == PosTagPrintBill.提示是否买单 && (bill.IPrint == null ? 0 : bill.IPrint) <= 0)
                {
                    //没有打单的账单提示（该账单未打单，是否继续买单）
                    return Json(JsonResultData.Failure("该账单未打单，是否继续买单?", 1));
                }
                else if (refe.ITagPrintBill == PosTagPrintBill.提示是否打印账单)
                {
                    //提示是否打印账单
                    return Json(JsonResultData.Failure("是否打印账单?", 2));
                }
                else if (refe.ITagPrintBill == PosTagPrintBill.必须打单才买单)
                {
                    //提示必须打单才能买单
                    return Json(JsonResultData.Failure("必须打单才能买单", 3));
                }
                else if (refe.ITagPrintBill == PosTagPrintBill.立即打印账单)
                {
                    return Json(JsonResultData.Successed("1"));
                }
                else
                {
                    return Json(JsonResultData.Successed());
                }
            }
            return Json(JsonResultData.Failure("该账单不存在"));
        }
        #endregion

        #region 账单打印记录

        /// <summary>
        /// 账单打印记录
        /// </summary>
        /// <param name="billId">账单ID</param>
        /// <param name="status">状态</param>
        /// <param name="controller">控制器名称</param>
        public void AddPosPrintBill(string billId, PosPrintBillStatus status, string controller)
        {

            var billService = GetService<IPosBillService>();//账单
            var bill = billService.Get(billId);
            decimal amount = 0;//金额
            var billRow = "";//账单明细

            var billDetailService = GetService<IPosBillDetailService>();
            if (status == PosPrintBillStatus.账单打印)
            {

                var billDetailList = billDetailService.GetBillDetailByDcFlag(CurrentInfo.HotelId, billId, PosItemDcFlag.D.ToString());

                if (billDetailList != null && billDetailList.Count > 0)
                {
                    foreach (var item in billDetailList)
                    {

                        if ((item.Status == (byte)PosBillDetailStatus.正常 || item.Status == (byte)PosBillDetailStatus.赠送 || item.Status == (byte)PosBillDetailStatus.例送) && item.Isauto < (byte)PosBillDetailIsauto.服务费)
                        {
                            //计算本次转的金额
                            amount += item.Amount == null ? 0 : Convert.ToDecimal(item.Amount);
                            billRow += item.Id + ",";
                        }
                    }
                }
            }
            else if (status == PosPrintBillStatus.埋脚打印)
            {
                var billDetailList = billDetailService.GetBillDetailByDcFlag(CurrentInfo.HotelId, billId, PosItemDcFlag.C.ToString());


                if (billDetailList != null && billDetailList.Count > 0)
                {
                    foreach (var item in billDetailList)
                    {

                        if (item.Status == (byte)PosBillDetailStatus.正常 && item.Isauto == (byte)PosBillDetailIsauto.付款)
                        {
                            //计算本次转的金额
                            amount += item.Amount == null ? 0 : Convert.ToDecimal(item.Amount);
                            billRow += item.Id + ",";
                        }
                    }
                }
            }
            var posPrintBill = new PosPrintBill
            {
                Id = Guid.NewGuid(),
                Hid = CurrentInfo.HotelId,
                BillBsnsDate = bill.BillBsnsDate,
                BillId = billId,
                PosId = CurrentInfo.PosId,
                RefeId = bill.Refeid,
                TabId = bill.Tabid,
                iStatus = (byte)status,
                Amount = amount,
                BillRow = billRow,
                ShiftId = bill.Shiftid,
                ShuffleId = bill.Shuffleid,
                Remark = controller,
                TransUser = CurrentInfo.UserName,
                CreateDate = DateTime.Now,
                Module = "CY"

            };
            var printBillService = GetService<IPosPrintBillService>();
            printBillService.Add(posPrintBill);
            printBillService.AddDataChangeLog(OpLogType.Pos账单打印表添加);
            printBillService.Commit();
        }


        #endregion

        #region 打印埋脚
        /// <summary>
        /// 打印埋脚
        /// </summary>
        /// <param name="model"></param>
        /// <param name="print"></param>
        /// <returns></returns>
        public JsonResult PrintBillPayMethod(ReportQueryModel model, string print, string controller = "")
        {
            if (model != null && !string.IsNullOrWhiteSpace(model.ReportCode))
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
                    billModel.IPaidPrint = billModel.IPaidPrint == null ? 1 : billModel.IPaidPrint + 1;
                    BillService.Update(billModel, new PosBill());
                    BillService.AddDataChangeLog(OpLogType.Pos账单修改);
                    BillService.Commit();

                    //添加打印埋脚记录
                    AddPosPrintBill(billid, PosPrintBillStatus.埋脚打印, controller);
                }

                if (model.ParameterValues.IndexOf("@hid") == -1)
                {
                    model.ParameterValues = "@h99hid=" + CurrentInfo.HotelId + "&" + model.ParameterValues;
                }
                string value = new JavaScriptSerializer().Serialize(model);
                Guid? id = GetService<IReportService>().AddQueryParaTemp(CurrentInfo.HotelId, value);
                if (id != null)
                {
                    var url = new StringBuilder();
                    url.Append("http://").Append(System.Web.HttpContext.Current.Request.Url.Host).Append("/ReportManage");
                    url.Append("/SRBillReportView/Index")
                        .Append("?ReportCode=").Append(System.Web.HttpContext.Current.Server.UrlEncode(model.ReportCode))
                        .Append("&ParameterValues=").Append(System.Web.HttpContext.Current.Server.UrlEncode(id.Value.ToString()))
                        .Append("&ChineseName=").Append(System.Web.HttpContext.Current.Server.UrlEncode(model.ChineseName));
                    if (!string.IsNullOrWhiteSpace(print))
                    {
                        url.Append("&print=").Append(System.Web.HttpContext.Current.Server.UrlEncode(print));
                    }

                    return Json(JsonResultData.Successed(url.ToString()));
                }
                else
                {
                    return Json(JsonResultData.Failure("添加失败！"));
                }
            }
            else
            {
                return Json(JsonResultData.Failure("参数错误！"));
            }
        }
        #endregion

        #region 餐台状态
        /// <summary>
        /// 设置餐台状态
        /// </summary>
        /// <param name="bill">账单</param>
        public void SetTabStatus(PosBill bill)
        {
            var statusService = GetService<IPosTabStatusService>();

            var billService = GetService<IPosBillService>();
            //修改餐台状态
            var searList = billService.GetSmearListByClearTab(CurrentInfo.HotelId, bill.Tabid);
            if (searList != null && searList.Count < 1)
            {
                if (bill.TabFlag == (byte)PosBillTabFlag.物理台)
                {
                    var tabStatus = statusService.Get(bill.Tabid);

                    if (tabStatus != null)
                    {
                        var newtabStatus = new PosTabStatus();
                        AutoSetValueHelper.SetValues(tabStatus, newtabStatus);
                        if (bill.Status == (byte)PosBillStatus.清台 || bill.Status == (byte)PosBillStatus.取消 || bill.Status == (byte)PosBillStatus.迟付)
                        {
                            newtabStatus.TabStatus = (byte)PosTabStatusEnum.空净;
                            newtabStatus.OpTabid = null;
                            newtabStatus.OpenGuest = null;
                            newtabStatus.GuestName = null;
                            newtabStatus.OpenRecord = null;
                        }
                        else if (bill.Status == (byte)PosBillStatus.结账)
                        {
                            newtabStatus.TabStatus = (byte)PosTabStatusEnum.已买单未离座;
                        }

                        statusService.Update(newtabStatus, tabStatus);
                        statusService.AddDataChangeLog(OpLogType.Pos餐台状态修改);
                        statusService.Commit();
                    }
                }
            }
            else
            {
                //判断
                var tabStatus = statusService.Get(bill.Tabid);


                if (tabStatus != null && tabStatus.OpTabid == bill.Billid)
                {
                    var newtabStatus = new PosTabStatus();
                    AutoSetValueHelper.SetValues(tabStatus, newtabStatus);
                    //if (bill.Status == (byte)PosBillStatus.清台 || bill.Status == (byte)PosBillStatus.取消)
                    //{
                    //  newtabStatus.TabStatus = (byte)PosTabStatusEnum.空净;
                    newtabStatus.OpTabid = null;
                    newtabStatus.OpenGuest = null;
                    newtabStatus.GuestName = null;
                    newtabStatus.OpenRecord = null;
                    //}
                    //else if (bill.Status == (byte)PosBillStatus.结账)
                    //{
                    //    newtabStatus.TabStatus = (byte)PosTabStatusEnum.已买单未离座;
                    //}

                    statusService.Update(newtabStatus, tabStatus);
                    statusService.AddDataChangeLog(OpLogType.Pos餐台状态修改);
                    statusService.Commit();
                }


            }
        }
        #endregion


        #region 锁台记录

        /// <summary>
        /// 添加餐台记录
        /// </summary>
        /// <param name="tabId">餐台ID</param>
        /// <param name="tabNo">餐台代码</param>
        /// <param name="billId">账单ID</param>
        /// <param name="refe">营业点</param>
        public void AddTabLog(string tabId, string tabNo, string billId, string computerName, PosRefe refe)
        {
            #region 清理当前操作员之前的锁台记录

            var tabLogService = GetService<IPosTabLogService>();    //餐台记录

            string computer = computerName ?? "";
            var tabLogList = tabLogService.GetPosTabLogListByTab(CurrentInfo.HotelId, refe.Id, tabId, tabNo);
            if (tabLogList != null && tabLogList.Count > 0)
            {
                foreach (var tabLog in tabLogList)
                {
                    if (tabLog.Billid == billId)
                    {
                        tabLogService.Delete(tabLog);
                        tabLogService.AddDataChangeLog(OpLogType.Pos锁台删除);
                        tabLogService.Commit();
                    }
                }
            }

            #endregion 清理当前操作员之前的锁台记录

            #region 添加当前锁台记录

            PosTabLog posTabLog = new PosTabLog()
            {
                Id = Guid.NewGuid(),
                Hid = CurrentInfo.HotelId,
                Msg = string.Format($"{tabNo}餐台被{computer} => {CurrentInfo.UserName}在操作"),
                Status = (byte)PosTabLogStatus.开台自动锁台,
                Computer = computer,
                ConnectDate = DateTime.Now,
                Module = refe.Module,
                TransUser = CurrentInfo.UserName,
                CreateDate = DateTime.Now
            };
            posTabLog.Refeid = refe.Id;
            posTabLog.Billid = billId;
            posTabLog.Tabid = tabId;
            posTabLog.TabNo = tabNo;
            posTabLog.Refeid = refe.Id;
            //   AutoSetValueHelper.SetValues(model, posTabLog);
            tabLogService.Add(posTabLog);
            tabLogService.AddDataChangeLog(OpLogType.Pos锁台增加);
            tabLogService.Commit();

            #endregion 添加当前锁台记录
        }
        #endregion

        #region 打印会员结账单

        /// <summary>
        /// 打印会员结账单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="print"></param>
        /// <param name="Flag">区分打单跟打印预览（A:打单）</param>
        /// <returns></returns>
        public JsonResult AddMardQueryParaTemp(ReportQueryModel model, string print, string Flag, string controller = "")
        {
            try
            {
                if (model != null && !string.IsNullOrWhiteSpace(model.ReportCode))
                {
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
                        url.Append("http://").Append(System.Web.HttpContext.Current.Request.Url.Host).Append("/ReportManage");
                        url.Append("/SRBillReportView/Index")
                            .Append("?ReportCode=").Append(System.Web.HttpContext.Current.Server.UrlEncode(model.ReportCode))
                            .Append("&ParameterValues=").Append(System.Web.HttpContext.Current.Server.UrlEncode(id.Value.ToString()))
                            .Append("&ChineseName=").Append(System.Web.HttpContext.Current.Server.UrlEncode(model.ChineseName));
                        if (!string.IsNullOrWhiteSpace(print))
                        {
                            url.Append("&print=").Append(System.Web.HttpContext.Current.Server.UrlEncode(print));
                        }

                        return Json(JsonResultData.Successed(url.ToString()));
                    }
                    else
                    {
                        return Json(JsonResultData.Failure("添加失败！"));
                    }
                }
                else
                {
                    return Json(JsonResultData.Failure("参数错误！"));
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }


        #endregion

        #region 保存菜式打印点菜单

        /// <summary>
        ///  获取保存菜式用于打印点菜单
        /// </summary>
        /// <param name="billId">账单ID</param>
        /// <param name="ids">选择的消费项目，格式（1001|1002）</param>
        /// <returns></returns>
        public List<up_pos_print_billOrderResult> GetOrderItem(string billId, string ids)
        {
            var pmsParaService = GetService<IPmsParaService>();

            var billService = GetService<IPosBillService>();

            var service = GetService<IPosBillDetailService>();



            //保存的菜式是否打印点菜单
            var PosCompanyQuota = pmsParaService.GetValue(CurrentInfo.HotelId, "PosIsSaveItemPrint");

            if (string.IsNullOrEmpty(billId) || PosCompanyQuota == "0")
            {
                return null;
            }
            else
            {
                var idArray = ids.Trim('|').Split('|');

                var bill = billService.Get(billId);        //获取账单信息

                var orderlist = service.GetBillOrderByPrint(CurrentInfo.HotelId, billId, billId).Where(w => w.计费状态 == (byte)PosBillDetailStatus.保存).ToList();

                var newOrderList = new List<up_pos_print_billOrderResult>();
                if (!string.IsNullOrEmpty(ids)) //打印部分菜式
                {
                    foreach (var order in orderlist)
                    {
                        foreach (var detailId in idArray)
                        {
                            if (Convert.ToInt64(detailId) == order.id)
                            {
                                var billDetail = service.Get(Convert.ToInt64(detailId));
                                var newbillDetail = new PosBillDetail();
                                AutoSetValueHelper.SetValues(billDetail, newbillDetail);

                                newbillDetail.iOrderPrint = (newbillDetail.iOrderPrint ?? 0 + 1);
                                newbillDetail.BatchTime = "00";
                                service.Update(newbillDetail, billDetail);
                                service.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newbillDetail.Id + ",点菜单次数：" + billDetail.iOrderPrint + "-->+" + newbillDetail.iOrderPrint, bill.BillNo);
                                newOrderList.Add(order);
                            }

                        }
                    }
                }
                else
                {
                    //打印全部菜式（需要排除已经打印过的菜式）
                    foreach (var order in orderlist)
                    {
                        if (order.点菜单打单次数 <= 0)
                        {
                            var billDetail = service.Get(Convert.ToInt64(order.id));
                            if (billDetail != null)
                            {
                                var newbillDetail = new PosBillDetail();
                                AutoSetValueHelper.SetValues(billDetail, newbillDetail);

                                newbillDetail.iOrderPrint = (newbillDetail.iOrderPrint ?? 0 + 1);
                                newbillDetail.BatchTime = "00";
                                service.Update(newbillDetail, billDetail);
                                service.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newbillDetail.Id + ",点菜单次数：" + billDetail.iOrderPrint + "-->+" + newbillDetail.iOrderPrint, bill.BillNo);

                                newOrderList.Add(order);
                            }

                        }
                    }
                }
                return newOrderList;
            }

        }
        #endregion


        #region 先落以及落单的业务处理代码
        /// <summary>
        /// 先落以及落单业务处理代码
        /// </summary>
        /// <param name="billid">账单ID</param>
        /// <param name="ids">先落账单明细ID</param>
        /// <param name="refe">营业点</param>
        /// <param name="controller">控制器名称</param>
        /// <returns></returns>
        public JsonResult Cmpbelone(string billid, string ids, PosRefe refe, string controller)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(billid))
                {
                    return Json(JsonResultData.Failure("单号不能为空"));
                }

                var billService = GetService<IPosBillService>();
                var bill = billService.Get(billid);

                var operDiscountService = GetService<IPosOperDiscountService>();
                try
                {
                    operDiscountService.cmpOperDiscount(CurrentInfo.HotelId, billid, CurrentInfo.ModuleCode, refe.Id);
                }
                catch (Exception ex)
                {

                    return Json(JsonResultData.Failure(ex.Message.ToString()));
                }

                var service = GetService<IPosBillDetailService>();
                var batchTime = service.GetNewBatchTimeByBillid(CurrentInfo.HotelId, billid, PosItemDcFlag.D.ToString());
                var billDetailList = service.GetBillDetailByBillid(CurrentInfo.HotelId, billid, PosItemDcFlag.D.ToString(), (byte)PosBillDetailStatus.保存);
                if (billDetailList != null && billDetailList.Count > 0)
                {
                    if (string.IsNullOrWhiteSpace(ids)) //落单
                    {
                        foreach (var billDetail in billDetailList)
                        {
                            var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, IsProduce = billDetail.IsProduce, BatchTime = billDetail.BatchTime };

                            billDetail.Status = (byte)PosBillDetailStatus.正常;
                            //   billDetail.IsProduce = (byte)PosBillDetailIsProduce.未出品;
                            billDetail.BatchTime = batchTime;
                            //会员价
                            //billDetail.PriceClub = billDetail.Price;
                            //billDetail.PriceOri = billDetail.Price;
                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + ",原价：" + billDetail.PriceOri.ToString() + ",会员价：" + billDetail.PriceClub.ToString() + "，计费状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + " -> " + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status) + "，批次：" + oldBillDetail.BatchTime + " -> " + batchTime, bill.BillNo);

                            service.Update(billDetail, null);
                            service.Commit();
                        }
                    }
                    else
                    {
                        var idArray = ids.Trim('|').Split('|');
                        foreach (var billDetail in billDetailList)
                        {
                            for (int i = 0; i < idArray.Count(); i++)
                            {
                                var abc = idArray[i];
                                if (billDetail.Id == Convert.ToInt64(idArray[i]))
                                {
                                    var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, IsProduce = billDetail.IsProduce, BatchTime = billDetail.BatchTime };

                                    billDetail.Status = (byte)PosBillDetailStatus.正常;
                                    //    billDetail.IsProduce = (byte)PosBillDetailIsProduce.未出品;
                                    billDetail.BatchTime = batchTime;

                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + ",原价：" + billDetail.PriceOri.ToString() + ",会员价：" + billDetail.PriceClub.ToString() + "，计费状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + " -> " + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status)
                                        + "，批次：" + oldBillDetail.BatchTime + " -> " + batchTime, bill.BillNo);

                                    service.Update(billDetail, new PosBillDetail());
                                    service.Commit();
                                    break;
                                }
                            }
                        }
                    }
                    //  service.StatisticsBillDetail(CurrentInfo.HotelId, billid, billDetailList[0].MBillid);
                    // SetRedisBill(billid);//落单之后保存数据到redis
                }

                //赠送落单
                var billDetailListZS = service.GetBillDetailByBillid(CurrentInfo.HotelId, billid, PosItemDcFlag.D.ToString(), (byte)PosBillDetailStatus.赠送).Where(m => (m.IsProduce == 0 || m.IsProduce == null)).ToList();

                if (billDetailListZS != null && billDetailListZS.Count > 0)
                {
                    if (string.IsNullOrWhiteSpace(ids))
                    {
                        foreach (var billDetail in billDetailListZS)
                        {
                            var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, IsProduce = billDetail.IsProduce, BatchTime = billDetail.BatchTime };

                            billDetail.Status = (byte)PosBillDetailStatus.赠送;
                            //     billDetail.IsProduce = (byte)PosBillDetailIsProduce.未出品;
                            billDetail.BatchTime = batchTime;

                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + ",原价：" + billDetail.PriceOri.ToString() + ",会员价：" + billDetail.PriceClub.ToString() + "，计费状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + " -> " + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status) + "，批次：" + oldBillDetail.BatchTime + " -> " + batchTime, bill.BillNo);

                            service.Update(billDetail, new PosBillDetail());
                            service.Commit();
                        }
                    }
                    else
                    {
                        var idArray = ids.Trim('|').Split('|');
                        foreach (var billDetail in billDetailListZS)
                        {
                            for (int i = 0; i < idArray.Count(); i++)
                            {
                                var abc = idArray[i];
                                if (billDetail.Id == Convert.ToInt64(idArray[i]))
                                {
                                    var oldBillDetail = new PosBillDetail() { Status = billDetail.Status, IsProduce = billDetail.IsProduce, BatchTime = billDetail.BatchTime };

                                    billDetail.Status = (byte)PosBillDetailStatus.赠送;
                                    //    billDetail.IsProduce = (byte)PosBillDetailIsProduce.未出品;
                                    billDetail.BatchTime = batchTime;

                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + ",原价：" + billDetail.PriceOri.ToString() + ",会员价：" + billDetail.PriceClub.ToString() + "，计费状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + " -> " + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status)
                                        + "，批次：" + oldBillDetail.BatchTime + " -> " + batchTime, bill.BillNo);

                                    service.Update(billDetail, new PosBillDetail());
                                    service.Commit();

                                    break;
                                }
                            }
                        }
                    }
                    // SetRedisBill(billid);//落单之后保存数据到redis
                }

                //修改落单时间

                if (bill != null)
                {
                    var newBill = new PosBill();
                    AutoSetValueHelper.SetValues(bill, newBill);
                    newBill.LastRecord = DateTime.Now;

                    billService.Update(newBill, bill);
                    billService.Commit();
                }


                //账单明细
                service.StatisticsBillDetail(CurrentInfo.HotelId, billid, billid);


                //打印点菜单
                var orderlist = new List<up_pos_print_billOrderResult>();
                if (string.IsNullOrWhiteSpace(ids)) //落单
                {
                    //var refe = Session["PosRefe"] as PosRefe;
                    if (refe.IPrintBillss == PosPrintBillss.打印)
                    {
                        //点菜单打印记录

                        decimal amount = 0;//金额
                        var billRow = "";//账单明细
                        orderlist = service.GetBillOrderByPrint(CurrentInfo.HotelId, billid, billid).Where(w => Convert.ToInt32(w.点菜单打单次数) == 0 || (w.计费状态 > 50 && w.计费状态 < (byte)PosBillDetailStatus.未落单取消 && Convert.ToInt32(w.点菜单打单次数) == 1) && w.是否打点菜单 == true).ToList();
                        var list = service.GetBillDetailByBillidAndStatus(CurrentInfo.HotelId, billid, PosItemDcFlag.D.ToString(), (byte)PosBillDetailStatus.未落单取消);
                        foreach (var temp in list)
                        {
                            foreach (var order in orderlist)
                            {
                                if (temp.Id == order.id)
                                {
                                    var billDetailNew = new PosBillDetail();
                                    AutoSetValueHelper.SetValues(temp, billDetailNew);
                                    byte iOrderPrint = temp.iOrderPrint ?? 0;
                                    billDetailNew.iOrderPrint = Convert.ToByte(iOrderPrint + 1);
                                    service.Update(billDetailNew, new PosBillDetail());
                                    service.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                                    service.Commit();
                                    //记录本次打印的数据
                                    amount += Convert.ToDecimal(temp.Amount);
                                    billRow += temp.Id + ",";


                                    break;
                                }
                            }
                        }

                        #region 插入数据PosPrintBill
                        var posPrintBill = new PosPrintBill
                        {
                            Id = Guid.NewGuid(),
                            Hid = CurrentInfo.HotelId,
                            BillBsnsDate = bill.BillBsnsDate,
                            BillId = billid,
                            PosId = CurrentInfo.PosId,
                            RefeId = bill.Refeid,
                            TabId = bill.Tabid,
                            iStatus = (byte)PosPrintBillStatus.点菜单打印,
                            Amount = amount,
                            BillRow = billRow,
                            ShiftId = bill.Shiftid,
                            ShuffleId = bill.Shuffleid,
                            Remark = controller,
                            TransUser = CurrentInfo.UserName,
                            CreateDate = DateTime.Now,
                            Module = "CY"

                        };
                        var printBillService = GetService<IPosPrintBillService>();
                        printBillService.Add(posPrintBill);
                        printBillService.AddDataChangeLog(OpLogType.Pos账单打印表添加);
                        printBillService.Commit();
                        #endregion
                        //service.StatisticsBillDetail(CurrentInfo.HotelId, billid, billid);
                        //SetRedisBill(billid);//落单之后保存数据到redis
                    }
                }





                SetRedisBill(billid);  //保存数据到redis

                var IsCommitQuit = "0";//不退出


                if (string.IsNullOrEmpty(ids))  //落单判断是否退出
                {
                    //var refe = Session["PosRefe"] as PosRefe;
                    var refeService = GetService<IPosRefeService>();
                    var refeModel = refeService.Get(refe.Id);
                    if (refeModel != null)
                    {
                        if (refeModel.IsCommitQuit == null || refeModel.IsCommitQuit == false)
                        {
                            IsCommitQuit = "0";

                        }
                        else
                        {
                            IsCommitQuit = "1";//退出当前界面
                            //删除锁台记录
                            var TabLogServiceService = GetService<IPosTabLogService>();
                            var tabLogList = TabLogServiceService.GetPosTabLogListByTab(CurrentInfo.HotelId, refeModel.Id, bill.Tabid, bill.TabNo);
                            if (tabLogList != null && tabLogList.Count > 0)
                            {
                                foreach (var tabLog in tabLogList)
                                {
                                    if (tabLog.Billid == bill.Billid && tabLog.TransUser == CurrentInfo.UserName)
                                    {
                                        TabLogServiceService.Delete(tabLog);
                                        TabLogServiceService.AddDataChangeLog(OpLogType.Pos锁台删除);
                                        TabLogServiceService.Commit();
                                    }
                                }
                            }

                        }
                    }

                }
                var result = new
                {
                    order = orderlist,
                    CommitQuit = IsCommitQuit
                };
                return Json(JsonResultData.Successed(result));

            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
        }
        #endregion


        #region 取消折扣


        /// <summary>
        /// 取消折扣
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public JsonResultData CancelDisCount(string billId)
        {
            try
            {
                var billService = GetService<IPosBillService>();
                var bill = billService.Get(billId);

                //获取系统参数，
                var paraservice = GetService<IPmsParaService>();
                var IsPayOrderAgain = paraservice.IsPayOrderAgain(CurrentInfo.HotelId);
                if (!IsPayOrderAgain)   //不让二次消费，不允许买单后账单修改
                {
                    if (bill.Status == (byte)PosBillStatus.结账 || bill.Status == (byte)PosBillStatus.清台)
                    {
                        return JsonResultData.Failure("已经买单，清台的账单不能修改折扣");
                    }
                }



                var newEntity = new PosBill();
                AutoSetValueHelper.SetValues(bill, newEntity);

                newEntity.DiscAmount = 0;    //折扣金额
                newEntity.Discount = 1;  //折扣率
                newEntity.DaType = null;
                newEntity.IsForce = null;
                newEntity.Profileid = null;
                newEntity.CardNo = null;
                billService.Update(newEntity, bill);
                billService.AddDataChangeLog(OpLogType.Pos账单修改);
                billService.Commit();

                AddOperationLog(OpLogType.Pos账单修改, "折扣率：" + bill.Discount + "-->" + newEntity.Discount + "，折扣金额：" + bill.DiscAmount + "-->" + newEntity.DiscAmount, bill.BillNo);

                var billDetailService = GetService<IPosBillDetailService>();
                var billDetailList = billDetailService.GetBillDetailByDcFlag(CurrentInfo.HotelId, billId, "D");
                foreach (var billDetail in billDetailList)
                {
                    var newBillDetail = new PosBillDetail();
                    AutoSetValueHelper.SetValues(billDetail, newBillDetail);
                    newBillDetail.Price = billDetail.PriceOri != null ? billDetail.PriceOri : billDetail.Price;  //价格恢复为原价
                    newBillDetail.Discount = 1;    //折扣率
                    newBillDetail.DiscAmount = 0;  //折扣金额
                    newBillDetail.IsDishDisc = null;
                    billDetailService.Update(newBillDetail, billDetail);
                    //    billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                    billDetailService.Commit();
                    AddOperationLog(OpLogType.Pos账单修改, "单号：" + newBillDetail.Id + "，折扣率：" + billDetail.Discount + "-->" + newBillDetail.Discount + "，折扣金额：" + billDetail.DiscAmount + "-->" + newBillDetail.DiscAmount, bill.BillNo);

                }
                billDetailService.UpdateBillDetailDisc(CurrentInfo.HotelId, billId, billId);
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex.Message.ToString());
            }
            return JsonResultData.Successed();
        }
        #endregion

        #region 判断用户的折扣权限

        public JsonResultData CheckOperDiscount(decimal? discType, decimal? discAmount, string refeId)
        {
            var service = GetService<IPosOperDiscountService>();

            var model = service.GetOperDiscountByUserId(CurrentInfo.HotelId, CurrentInfo.UserId, refeId, CurrentInfo.ModuleCode);
            if (model != null)
            {
                if (model.Discount != null && model.Discount > 0)
                {
                    var Discount = model.Discount > 1 ? model.Discount / 100 : model.Discount;
                    if (discType != null && discType < Discount)
                    {
                        return JsonResultData.Failure("修改折扣失败：" + CurrentInfo.UserName + "最低折扣为：" + Discount);
                    }
                }
                if (model.DiscAmount != null && model.DiscAmount > 0)
                {
                    if (discAmount != null && discAmount > model.DiscAmount)
                    {
                        return JsonResultData.Failure("修改折扣失败：" + CurrentInfo.UserName + "金额折：" + model.DiscAmount);
                    }
                }
            }
            return JsonResultData.Successed();
        }
        #endregion

        #region 设置餐台最早预定时间

        #endregion

        #region 设置餐台最早预定时间

        /// <summary>
        /// 设置餐台最早预抵时间
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="tabId">餐台Id</param>
        public void SetTabServerArrDate(string hid, string tabId)
        {
            var service = GetService<IPosReserveService>();

            //获取餐台所有预定信息最早的预抵时间
            var listBill = service.GetOrderBillByTabId(CurrentInfo.HotelId, tabId);
            var minTime = listBill.ConvertAll(w => w.OrderDate).Min();

            //当前操作的时间最小的话 把这个时间更新进去

            var tabService = GetService<IPosTabStatusService>();
            var tab = tabService.Get(tabId);
            if (tab != null)
            {
                var date = tab.ArrDate;
                var oldTab = new PosTabStatus();
                AutoSetValueHelper.SetValues(tab, oldTab);
                tab.ArrDate = minTime;
                tabService.Update(tab, oldTab);
                AddOperationLog(OpLogType.Pos餐台状态修改, "修改餐台最早预定时间：" + date + "-->" + minTime, tabId);
                tabService.Commit();

            }

        }
        #endregion

        #region 电子发票
        /// <summary>
        /// 消费传送到发票系统，以便开电子发票
        /// </summary>
        /// <param name="billid"></param>
        /// <param name="id"></param>
        public void AddEInvoice(string billid)
        {
            if (!string.IsNullOrWhiteSpace(billid))
            {
                var hid = CurrentInfo.HotelId;
                // var businessPointId = CurrentInfo.BusinessPointId;
                string host = System.Web.HttpContext.Current.Request.Url.Host;
                System.Threading.ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        var para = new
                        {
                            hid = hid,
                            billid = billid
                        };
                        var data = Encoding.UTF8.GetBytes(Common.Tools.JsonHelper.SerializeObject(para));

                        var request = System.Net.WebRequest.CreateHttp("http://" + host + "/PosManage/Invoice/InvoiceConsum");
                        request.Method = "post";
                        request.ContentType = "application/json";
                        request.Timeout = Int32.MaxValue;
                        request.ContentLength = data.Length;
                        using (var input = request.GetRequestStream())
                        {
                            input.Write(data, 0, data.Length);
                        }
                        using (var response = request.GetResponse())
                        {

                        }
                    }
                    catch { }
                });
            }
        }
        /// <summary>
        /// 从发票系统中撤销消费记录，使其不能开电子发票
        /// </summary>
        /// <param name="billid"></param>
        public void RepealEInvoice(string billid)
        {
            if (!string.IsNullOrWhiteSpace(billid))
            {
                string hid = CurrentInfo.HotelId;
                string host = System.Web.HttpContext.Current.Request.Url.Host;
                System.Threading.ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        var para = new
                        {
                            hid = hid,
                            billid = billid,
                        };
                        var data = Encoding.UTF8.GetBytes(Common.Tools.JsonHelper.SerializeObject(para));

                        var request = System.Net.WebRequest.CreateHttp("http://" + host + "/PosManage/Invoice/InvoiceConsumRepeal");
                        request.Method = "post";
                        request.ContentType = "application/json";
                        request.Timeout = Int32.MaxValue;
                        request.ContentLength = data.Length;
                        using (var input = request.GetRequestStream())
                        {
                            input.Write(data, 0, data.Length);
                        }
                        using (var response = request.GetResponse())
                        {

                        }
                    }
                    catch { }
                });
            }
        }
        #endregion



    }
}