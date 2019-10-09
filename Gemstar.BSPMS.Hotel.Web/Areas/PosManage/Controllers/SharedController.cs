using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models.Account;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Xml;
using System.Xml.Serialization;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos公共视图
    /// </summary>
    [AuthPage(ProductType.Pos, "p20001")]
    public class SharedController : BaseController
    {
        /// <summary>
        /// Pos刷卡分布视图
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PayByCard(PayByCardViewModel model)
        {
            return PartialView("_PayByCard", model);
        }

        /// <summary>
        /// Pos刷卡分布视图
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PayByCardForCancel(PayByCardViewModel model)
        {
            return PartialView("_PayByCardForCancel", model);
        }

        /// <summary>
        /// 检查卡号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult CheckCard(PayByCardViewModel model, string IsPrintBill = "0")
        {
            if (model != null && !string.IsNullOrWhiteSpace(model.CardId))
            {
                try
                {
                    var service = GetService<IPmsUserService>();
                    var entity = service.GetEntityByCardId(CurrentInfo.GroupId, CurrentInfo.HotelId, CryptHelper.EncryptDES(model.CardId));
                    if (entity != null)
                    {
                        var product = GetProduct();
                        FormsAuthentication.SetAuthCookie(entity.Code, false);
                        CurrentInfo.UserId = entity.Id.ToString();
                        CurrentInfo.UserCode = entity.Code;
                        CurrentInfo.UserName = entity.Name;
                        CurrentInfo.IsRegUser = entity.IsReg == 1 ? true : false;

                        //集团的获取集团的用户id,用于刷卡后权限判断
                        if (CurrentInfo.IsGroup)
                        {
                            var userId = GetService<IPmsUserService>().GetUserIDByCode(CurrentInfo.GroupHotelId, entity.Code).Id.ToString();
                            CurrentInfo.UserId = userId;
                        }
                        CurrentInfo.SaveValues();
                        CurrentInfo.LoadValues();

                        //卡号验证成功之后添加锁台记录(不是抹台)
                        if (string.IsNullOrEmpty(model.Billid) && (model.Flag != "B" && model.Flag != "C"))
                        {
                            var tabStatusService = GetService<IPosTabStatusService>();
                            var tabStatus = tabStatusService.Get(model.Tabid);

                            var tabLogService = GetService<IPosTabLogService>();
                            var tabLog = tabLogService.GetPosTabLogByTab(CurrentInfo.HotelId, tabStatus.Refeid, model.Tabid, model.TabNo);

                            var tabBillService = GetService<IPosBillService>();
                            var tabBill = tabBillService.GetPosBillByTabId(CurrentInfo.HotelId, tabStatus.Refeid, model.Tabid);

                            var refeService = GetService<IPosRefeService>();
                            var refe = refeService.Get(tabStatus.Refeid);
                            if (tabBill != null)
                            {
                                //这个添加锁台记录是 已经开台，退出之后删除锁台记录的账单，重新添加一条锁台记录
                                AddTabLog(model.Tabid, model.TabNo, tabBill.Billid, model.ComputerName, refe);
                            }
                        }
                        else if (!string.IsNullOrEmpty(model.Billid))
                        {
                            //存在账单ID根据账单ID去添加锁台记录
                            var tabBillService = GetService<IPosBillService>();
                            var tabBill = tabBillService.Get(model.Billid);
                            //不是预定单才需要添加锁台记录，预定账单再开台成功之后添加 锁台记录
                            if (tabBill.IsOrder == true && tabBill.Status != (byte)PosBillStatus.预订)
                            {

                                var refeService = GetService<IPosRefeService>();
                                var refe = refeService.Get(tabBill.Refeid);
                                if (tabBill != null)
                                {
                                    //这个添加锁台记录是 已经开台，退出之后删除锁台记录的账单，重新添加一条锁台记录
                                    AddTabLog(model.Tabid, model.TabNo, tabBill.Billid, model.ComputerName, refe);
                                }
                            }

                        }

                        return Json(JsonResultData.Successed());
                    }
                    return Json(JsonResultData.Failure("输入的卡号不存在，请修改后重试！"));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
            else
            {
                return Json(JsonResultData.Failure("请输入卡号！"));
            }
        }

        /// <summary>
        /// 检查卡号(取消消费项目)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult CheckCardForCancel(PayByCardViewModel model)
        {
            if (model != null && !string.IsNullOrWhiteSpace(model.CardId))
            {
                try
                {
                    var service = GetService<IPmsUserService>();
                    var entity = service.GetEntityByCardId(CurrentInfo.GroupId, CurrentInfo.HotelId, CryptHelper.EncryptDES(model.CardId));
                    if (entity != null)
                    {
                        Session["OldUserID"] = CurrentInfo.UserId;//当前登录人的用户id
                        CurrentInfo.UserId = entity.Id.ToString();//刷卡的用户id

                        if (CurrentInfo.IsGroup)//集团的获取集团的用户id,用于刷卡后权限判断
                        {
                            var userId = GetService<IPmsUserService>().GetUserIDByCode(CurrentInfo.GroupHotelId, entity.Code).Id.ToString();
                            CurrentInfo.UserId = userId;
                        }

                        CurrentInfo.SaveValues();
                        CurrentInfo.LoadValues();

                        return Json(JsonResultData.Successed());
                    }
                    return Json(JsonResultData.Failure("输入的卡号不存在，请修改后重试！"));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
            else
            {
                return Json(JsonResultData.Failure("请输入卡号！"));
            }
        }

        private void AddTabLog(string tabId, string tabNo, string billId, string computerName, PosRefe refe)
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

        /// <summary>
        /// 退出刷卡
        /// </summary>
        /// <param name="returnType">1：Json 2：Url</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ExitPayByCard(byte returnType)
        {
            if (!CurrentInfo.UserCode.Equals("posAutoLogin", StringComparison.CurrentCultureIgnoreCase) || returnType == 1)
            {
                return Json(JsonResultData.Successed());
            }

            var _currentInfo = GetService<ICurrentInfo>();
            var hid = _currentInfo.HotelId;
            var userName = _currentInfo.UserName;
            var shiftName = _currentInfo.ShiftName;
            var hotelStatusService = GetService<IHotelStatusService>();
            var businessDay = hotelStatusService.GetBusinessDate(hid).ToString("yyyy-MM-dd");
            _currentInfo.Clear();
            _currentInfo.SaveValues();
            Session.Abandon();
            var accessDomain = Request.Headers["Host"];
            var domain = SharedSessionModule.GetLastThreeLevelDomain(accessDomain);
            var urlBuilder = new UriBuilder();
            urlBuilder.Host = domain;
            urlBuilder.Path = FormsAuthentication.LoginUrl;
            var url = urlBuilder.ToString();

            //记录退出的日志
            var text = string.Format("班次：{1}，登录营业日：{0}", businessDay, shiftName);
            GetService<IoperationLog>().AddOperationLog(hid, OpLogType.退出班次, text, userName, Common.Extensions.UrlHelperExtension.GetRemoteClientIPAddress());
            return Json(JsonResultData.Failure(url));
        }

        /// <summary>
        /// 获取房账信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult GetRoomAccount(string roomNo)
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
                                            xmls.Add(node.Name, node.FirstChild.Value);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    string json = JsonConvert.SerializeObject(xmls);
                    return Json(JsonResultData.Successed(json));
                }

                return Json(JsonResultData.Failure("请输入房号"));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 获取会员卡信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult GetMbrCard(string mbrCardNo)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(mbrCardNo))
                {
                    var service = GetService<IPosBillDetailService>();
                    string xmlStr = "<?xml version='1.0' encoding='gbk' ?>"
                        + "<RealOperate>"
                        + "<XType>JxdBSPms</XType>"
                        + "<OpType>会员查询</OpType>"
                        + "<MbrQuery>"
                        + "<ProfileID></ProfileID>"
                        + "<NetName></NetName>"
                        + "<NetPwd></NetPwd>"
                        + "<OtherKeyWord></OtherKeyWord>"
                        + "<OtherName></OtherName>"
                        + "<Mobile>" + mbrCardNo + "</Mobile>"
                        + "<MbrCardNo>" + mbrCardNo + "</MbrCardNo>"
                        + "</MbrQuery>"
                        + "</RealOperate>";

                    var xmlInfo = service.RealOperate(CurrentInfo.HotelId, "", xmlStr);

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlInfo);
                    Dictionary<string, string> xmls = new Dictionary<string, string>();
                    if (doc != null)
                    {
                        if (doc["MbrQuery"] != null)
                        {
                            if (doc["MbrQuery"]["Rows"] != null)
                            {
                                if (doc["MbrQuery"]["Rows"]["Row"] != null)
                                {
                                    foreach (XmlNode node in doc["MbrQuery"]["Rows"]["Row"])
                                    {
                                        if (node != null && node.Name != null && node.FirstChild != null)
                                        {
                                            if (node.Name == "Status")
                                            {
                                                xmls.Add(node.Name, Convert.ToInt32(node.FirstChild.Value) < 51 ? "正常" : "无效");
                                            }
                                            else
                                            {
                                                xmls.Add(node.Name, node.FirstChild.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    string json = JsonConvert.SerializeObject(xmls);
                    return Json(JsonResultData.Successed(json));
                }

                return Json(JsonResultData.Failure("请输入会员卡号或手机号"));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 获取合约单位信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult GetContractUnit(string cttName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(cttName))
                {
                    var service = GetService<IPosBillDetailService>();
                    string xmlStr = "<?xml version='1.0' encoding='gbk' ?>"
                        + "<RealOperate>"
                        + "<XType>JxdBSPms</XType>"
                        + "<OpType>合约单位资料查询</OpType>"
                        + "<CompanyFolio>"
                        + "<hid>" + CurrentInfo.HotelId + "</hid>"
                        + "<cttName>" + cttName + "</cttName>"
                        + "</CompanyFolio>"
                        + "</RealOperate>";
                    var xmlInfo = service.RealOperate(CurrentInfo.HotelId, "", xmlStr);




                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlInfo);

                    List<XmlNodeResult> list = new List<XmlNodeResult>();

                    XmlNodeList topM = doc.SelectNodes("//Row");
                    foreach (XmlElement element in topM)
                    {
                        var s = new XmlNodeResult()
                        {
                            ProfileId = element.GetElementsByTagName("ProfileId")[0].InnerText,
                            CttName = element.GetElementsByTagName("CttName")[0].InnerText,
                            Contactor = element.GetElementsByTagName("Contactor")[0].InnerText,
                            Tel = element.GetElementsByTagName("Tel")[0].InnerText,
                            Cttno = element.GetElementsByTagName("Cttno")[0].InnerText,
                            Sales = element.GetElementsByTagName("Sales")[0].InnerText,
                            Mbrexpired = element.GetElementsByTagName("Mbrexpired")[0].InnerText,
                            Balance = element.GetElementsByTagName("Balance")[0].InnerText,
                            Creditlevel = element.GetElementsByTagName("Creditlevel")[0].InnerText,
                            ApprovalAmt = element.GetElementsByTagName("ApprovalAmt")[0].InnerText,
                            Remark = element.GetElementsByTagName("Remark")[0].InnerText,
                        };
                        list.Add(s);
                    }
                    var listb = list.Where(m => m.CttName == cttName).FirstOrDefault();
                    string json = JsonConvert.SerializeObject(listb);
                    return Json(JsonResultData.Successed(json));
                }

                return Json(JsonResultData.Failure("请输入合约单位"));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PayPrePay()
        {
            return PartialView("_PayPrePay");
        }

        /// <summary>
        /// 获取定金信息
        /// </summary>
        /// <param name="billNo">押金单号</param>
        /// <returns></returns>

        [AuthButton(AuthFlag.None)]
        public ActionResult GetPrePay(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                var service = GetService<IYtPrepayService>();
                var result = service.GetPrePayInfoById(CurrentInfo.HotelId, CurrentInfo.ModuleCode, Id);

                return Json(JsonResultData.Successed(result));
            }
            return Json(JsonResultData.Failure("押金信息查询失败！"));
        }

        #region 视图

        /// <summary>
        /// 获取买单信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.ChangeCardNum)]
        public PartialViewResult _Payment(PaymentViewModel model)
        {
            PosCommon common = new PosCommon();
            return common._Payment(model);

        }

        #region 保存数据

        /// <summary>
        ///  根据billid 保存数据到redis
        /// </summary>
        public void SetRedisBill(string Billid)
        {
            //账单主表
            var billService = GetService<IPosBillService>();
            var billSession = billService.Get(Billid);
            SaveValues("billSession_" + Billid, billSession);

            //   Session["billSession"] = billSession;

            //账单明细
            var billDetailservice = GetService<IPosBillDetailService>();
            //只针对消费项目
            var billDetailList = billDetailservice.GetBillDetailByDcFlagForPosInSing(CurrentInfo.HotelId, Billid, "D");
            SaveValues("billDetailSessionList_" + Billid, billDetailList);

            //作法
            var BillDetailActionService = GetService<IPosBillDetailActionService>();
            var BillDetailActionList = BillDetailActionService.GetPosBillDetailActionByModule(CurrentInfo.HotelId, Billid);
            SaveValues("BillDetailActionSessionList_" + Billid, BillDetailActionList);
        }

        /// <summary>
        /// 保存数据到本地
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
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
        /// 将Json格式的时间字符串替换为"yyyy-MM-dd HH:mm:ss"格式的字符串
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static string ReplaceJsonDateToDateString(string json)
        {
            return Regex.Replace(json, @"\\/Date\((\d+)\)\\/", match =>
            {
                DateTime dt = new DateTime(1970, 1, 1);
                dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                dt = dt.ToLocalTime();
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            });
        }

        #endregion 保存数据

        /// <summary>
        /// 获取指定酒店下付款方式列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PaymentMethod(PaymentViewModel model)
        {
            model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
            model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

            var service = GetService<IPosItemService>();
            var refeId = GetService<IPosBillService>().Get(model.Billid).Refeid;//获取账单营业点ID
            var posItemList = service.GetPosItemByDcFlag(CurrentInfo.HotelId, PosItemDcFlag.C.ToString(), model.PageIndex, model.PageSize, refeId);
            model.PayWayList = posItemList;


            ViewBag.Version = CurrentVersion;
            return PartialView("_PaymentMethod", model);
        }

        /// <summary>
        /// 付款方式列表视图
        /// </summary>
        [AuthButton(AuthFlag.Query)]
        public PartialViewResult _PaymentMethodList(PaymentViewModel model)
        {
            return PartialView("_PaymentMethodList", model);
        }

        #endregion 视图

        #region 自动更换市别

        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult ChangeShuffle()
        {
            //获取系统参数，根据参数判断是否自动切换市别
            var paraservice = GetService<IPmsParaService>();
            //是否自动切换市别（默认是自动的，）
            if (!paraservice.IsSelfChangeShuffle(CurrentInfo.HotelId))
            {
                return Json(JsonResultData.Successed());
            }

            var posPosService = GetService<IPosPosService>();   //收银点
            var posPosList = posPosService.GetPosByHid(CurrentInfo.HotelId).Where(m => (m.IStatus == (byte)EntityStatus.启用 || m.IStatus == null));

            var posRefeSercice = GetService<IPosRefeService>(); //营业点

            var ShuffleService = GetService<IPosShuffleService>(); //市别服务

            var NowTime = DateTime.Now; //当前时间
            foreach (var posPos in posPosList)
            {
                //获取收银点下面所有营业点
                var refeList = posRefeSercice.GetRefeByPos(CurrentInfo.HotelId, posPos.Id, CurrentInfo.ModuleCode);

                foreach (var refe in refeList)
                {
                    var newPosRefeEntity = new PosRefe();
                    AutoSetValueHelper.SetValues(refe, newPosRefeEntity);
                    //获取营业点下面所有市别
                    var shuffleList = ShuffleService.GetPosShuffleList(CurrentInfo.HotelId, refe.Id, CurrentInfo.ModuleCode);
                    foreach (var shuffle in shuffleList)
                    {
                        var startTime = Convert.ToDateTime(DateTime.Today.ToString("yyyy-MM-dd") + " " + shuffle.Stime);
                        var endTime = Convert.ToDateTime(DateTime.Today.ToString("yyyy-MM-dd") + " " + shuffle.Etime);
                        if (NowTime >= startTime && NowTime <= endTime)
                        {
                            if (refe.ShuffleId != shuffle.Id)
                            {
                                newPosRefeEntity.ShuffleId = shuffle.Id;
                                AddOperationLog(OpLogType.Pos换市别, "名称：" + refe.Cname + "，当前市别：" + refe.ShuffleId + " -> " + refe.ShuffleId, refe.Id);
                                posRefeSercice.Update(newPosRefeEntity, refe);
                                posRefeSercice.Commit();
                            }
                        }
                    }
                }
            }
            return Json(JsonResultData.Successed());
        }

        //获取当前收银点的市别
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult GetCurrentShuffle()
        {
            var posRefeSercice = GetService<IPosRefeService>(); //营业点
            var ShuffleService = GetService<IPosShuffleService>(); //市别服务
            var NowTime = DateTime.Now; //当前时间
            var shuffleName = "";
            var refeList = posRefeSercice.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
            if (refeList != null && refeList.Count > 0)
            {
                //营业点(默认第一条)
                string Refeid = refeList[0].Id;
                var shuffleService = GetService<IPosShuffleService>();
                var shuffle = shuffleService.GetEntity(CurrentInfo.HotelId, refeList[0].ShuffleId);
                shuffleName = shuffle == null ? "" : (string.IsNullOrEmpty(shuffle.Cname) ? "" : shuffle.Cname);
            }
            return Json(JsonResultData.Successed(shuffleName));
        }

        #endregion 自动更换市别
    }
    public class XmlNodeResult
    {
        public string ProfileId { get; set; }
        public string CttName { get; set; }
        public string Contactor { get; set; }
        public string Tel { get; set; }
        public string Cttno { get; set; }
        public string Sales { get; set; }
        public string Mbrexpired { get; set; }
        public string Balance { get; set; }
        public string Creditlevel { get; set; }
        public string ApprovalAmt { get; set; }
        public string Remark { get; set; }
    }
}