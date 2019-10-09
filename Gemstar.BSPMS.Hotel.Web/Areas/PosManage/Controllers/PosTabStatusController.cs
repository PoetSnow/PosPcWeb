using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosTabStatus;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos餐台状态
    /// </summary>
    [AuthPage(ProductType.Pos, "p10001")]
    public class PosTabStatusController : BaseEditInWindowController<PosBill, IPosBillService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(string mode = "")
        {
            TabStatusViewModel model = new TabStatusViewModel();
            var tabStatusService = GetService<IPosTabStatusService>();
            model.PageTotal = tabStatusService.GetPosTabStatusResultTotal(CurrentInfo.HotelId, model.Code, model.Refeid, model.Tabtype, null);

            //收银点的当前营业日
            var pos = new PosPos();
            var posService = GetService<IPosPosService>();
            if (!string.IsNullOrEmpty(mode))
            {
                pos = posService.GetPosByHid(CurrentInfo.HotelId).Where(w => w.PosMode == mode).FirstOrDefault();
                if (pos != null)
                {
                    CurrentInfo.PosId = pos.Id;
                    CurrentInfo.PosName = pos.Name;
                }
            }
            else if (string.IsNullOrEmpty(CurrentInfo.PosId))
            {
                pos = posService.GetPosByHid(CurrentInfo.HotelId).FirstOrDefault();
                if (pos != null)
                {
                    CurrentInfo.PosId = pos.Id;
                    CurrentInfo.PosName = pos.Name;
                }
            }
            else
            {
                pos = posService.Get(CurrentInfo.PosId);
            }
            model.Business = pos.Business;

            var refeService = GetService<IPosRefeService>();
            var list = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
            if (list != null && list.Count > 0)
            {
                //model.Business = list[0].Business;
                model.IsOpenBrush = list[0].IsOpenBrush;
            }

            //楼面自动刷新间隔时间
            var pmsParaService = GetService<IPmsParaService>();
            ViewBag.TabStatusRefreshInterval = pmsParaService.GetValue(CurrentInfo.HotelId, "tabStatusRefreshInterval");
            //提示信息显示时间
            ViewBag.TipsTime = pmsParaService.GetValue(CurrentInfo.HotelId, "tipsTime");
            //是否是通过快捷方式进行自动登录的用户
            ViewBag.IsAutoLogin = CurrentInfo.UserCode.Equals("posAutoLogin", StringComparison.CurrentCultureIgnoreCase) || CurrentInfo.UserName.Equals("Pos自动登录", StringComparison.CurrentCultureIgnoreCase) ? true : false;

            ViewBag.Version = CurrentVersion;
            return View(model);
        }

        [AuthButton(AuthFlag.None)]
        public List<up_pos_list_TabStatusResult> GetTabStatusList(TabStatusViewModel model)
        {
            model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
            model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

            var service = GetService<IPosTabStatusService>();

            var list = service.GetPosTabStatusResult(CurrentInfo.HotelId, model.Code, model.Refeid, model.Tabtype, model.TabStatus.ToString(), model.PageIndex, model.PageSize);


            return list;
        }

        /// <summary>
        /// 根据模型返回查询总数
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ContentResult GetTabStatusTotal(TabStatusViewModel model)
        {
            model.Code = model.Code ?? "";
            model.Refeid = model.Refeid ?? "";
            model.Tabtype = model.Tabtype ?? "";

            model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
            model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

            var refe = new PosRefe();
            var refeService = GetService<IPosRefeService>();
            if (string.IsNullOrWhiteSpace(model.Refeid))
            {
                var refeList = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
                if (refeList != null && refeList.Count > 0)
                {
                    foreach (var temp in refeList)
                    {
                        model.Refeid += ',' + temp.Id;
                    }
                    model.Refeid = model.Refeid.Trim(',');
                }
            }
            else
            {
                refe = refeService.Get(model.Refeid);
            }

            var service = GetService<IPosTabStatusService>();
            return Content(service.GetPosTabStatusResultTotal(CurrentInfo.HotelId, model.Code, model.Refeid, model.Tabtype, null).ToString());
        }

        #region 开台登记

        /// <summary>
        /// 锁台
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult AddLockTab(PosTabLogAddViewModel model)
        {
            var result = CheckTabLog(model);
            return result;
        }

        /// <summary>
        /// 开台登记
        /// </summary>
        /// <param name="addViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult AddOpenTab(OpenTabAddViewModel addViewModel)
        {
            if (!string.IsNullOrEmpty(addViewModel.BillId))
            {
                //预订开台
                var result = OpenOrderBill(addViewModel);
                return Json(result);

            }

            if (addViewModel != null && addViewModel.Tabid != null)
            {
                //验证是否被占用

                PosTabLogAddViewModel posTabLogAddViewModel = new PosTabLogAddViewModel();
                posTabLogAddViewModel.Tabid = addViewModel.Tabid;
                posTabLogAddViewModel.TabNo = addViewModel.TabNo;

                JsonResult result = CheckTabLog(posTabLogAddViewModel);

                var serializer = new JavaScriptSerializer();
                var valueStr = ReplaceJsonDateToDateString(serializer.Serialize(result.Data));

                var JsonresultData = serializer.Deserialize<JsonResultData>(valueStr);

                if (!JsonresultData.Success)
                {
                    return Json(JsonresultData);
                }
                //调用通用方法进行开台
                var resultData = addPosBillForOpenTab(addViewModel);
                return Json(resultData);
            }


            return Json(JsonResultData.Failure(""));
        }

        /// <summary>
        /// 抹台登记
        /// </summary>
        /// <param name="addViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult AddSmearTab(PosBillAddViewModel addViewModel)
        {
            if (addViewModel != null && addViewModel.Tabid != null)
            {
                var tabService = GetService<IPosTabService>();
                var tab = tabService.Get(addViewModel.Tabid);

                var refeService = GetService<IPosRefeService>();
                var refe = refeService.Get(tab.Refeid);

                var posService = GetService<IPosPosService>();
                var pos = posService.Get(CurrentInfo.PosId);

                var billService = GetService<IPosBillService>();
                var posBill = billService.GetLastBillId(CurrentInfo.HotelId, tab.Refeid, pos.Business);

                bool isexsit = billService.IsExists(CurrentInfo.HotelId, posBill.Billid, posBill.BillNo);
                if (isexsit)
                {
                    posBill = billService.GetLastBillId(CurrentInfo.HotelId, tab.Refeid, pos.Business);
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        DateTime openTime = DateTime.Now;
                        var posTabServiceService = GetService<IPosTabServiceService>();
                        var posTabService = posTabServiceService.GetPosTabService(CurrentInfo.HotelId, CurrentInfo.ModuleCode, tab.Refeid, tab.TabTypeid, null, (byte)PosITagperiod.随时, openTime);

                        var service = GetService<IPosBillService>();
                        PosBill bill = new PosBill()
                        {
                            Hid = CurrentInfo.HotelId,
                            Billid = posBill.Billid,
                            BillNo = posBill.BillNo,
                            MBillid = posBill.Billid,
                            InputUser = CurrentInfo.UserName,
                            BillDate = openTime,
                            IsService = true,
                            IsLimit = true,

                            ServiceRate = posTabService.Servicerate,
                            Limit = posTabService.NLimit,
                            IsByPerson = posTabService.IsByPerson == 0 ? false : true,
                            IHour = posTabService.LimitTime,
                            Discount = posTabService.Discount,

                            TabFlag = (byte)PosBillTabFlag.物理台,
                            Status = (byte)PosBillStatus.开台,
                            Shiftid = pos.ShiftId,
                            Shuffleid = refe.ShuffleId,
                        };

                        addViewModel.Refeid = refe.Id;
                        addViewModel.BillBsnsDate = pos.Business;
                        AutoSetValueHelper.SetValues(addViewModel, bill);
                        //抹台的餐台 在餐台后面添加一个字母进行区分

                        var searList = service.GetSmearList(CurrentInfo.HotelId, bill.Tabid, pos.Business.Value.Date);
                        if (searList != null && searList.Count > 0)
                        {
                            List<string> tabNoList = new List<string>();
                            foreach (var tabNo in searList)
                            {
                                var Letter = tabNo.TabNo.Substring(tabNo.TabNo.Length - 1, 1);
                                //不是数字添加进数组
                                if (!CheckLetter(Letter))
                                {
                                    tabNoList.Add(Letter);
                                }
                            }
                            string max;
                            if (tabNoList.Count <= 0)
                            {
                                max = "A";
                            }
                            else
                            {
                                max = tabNoList.Max();
                                max = Convert.ToChar(Convert.ToInt16(max.ToCharArray()[0]) + 1).ToString();
                                if (max == "Z")
                                {
                                    //如果为Z暂补考虑
                                }
                            }
                            bill.TabNo = bill.TabNo + max;
                        }

                        service.Add(bill);
                        service.AddDataChangeLog(OpLogType.Pos账单增加);
                        service.Commit();

                        //修改餐台状态
                        var tabStatusService = GetService<IPosTabStatusService>();

                        //抹台数据不修改餐台
                        //PosTabStatus tabStatus = new PosTabStatus() { GuestName = addViewModel.Name, Tabid = addViewModel.Tabid, TabStatus = (byte)PosTabStatusEnum.就座, OpTabid = bill.Billid, OpenRecord = DateTime.Now, OpenGuest = bill.IGuest };
                        //updateTabStatus(tabStatus);

                        tabStatusService.SetTabStatus(CurrentInfo.HotelId, addViewModel.Refeid, (byte)PosTabStatusOpType.初始化, "", "", "");

                        //添加开台记录表
                        PosCommon common = new PosCommon();
                        common.AddTabLog(addViewModel.Tabid, addViewModel.TabNo, posBill.Billid, addViewModel.ComputerName, refe);
                        if (addViewModel.ReturnType == 1)    //1：返回URL 2：返回Json
                        {
                            var url = Url.Action("Index", "PosInSingle", new { rnd = new Random().NextDouble() }) + "&refeid=" + System.Web.HttpUtility.UrlEncode(refe.Id) + "&tabid=" + System.Web.HttpUtility.UrlEncode(bill.Tabid) + "&billid=" + bill.Billid + "&tabFlag=" + bill.TabFlag + "&openFlag=A";
                            // return Redirect(url);
                            return Json(JsonResultData.Successed(url));
                        }
                        else
                        {
                            return Json(JsonResultData.Successed(posBill.Billid));
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(JsonResultData.Failure(ex));
                    }
                }
            }

            return Json(JsonResultData.Failure(""));
        }

        /// <summary>
        /// 判断字符串是否为数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool CheckLetter(string str)
        {
            try
            {
                int.Parse(str);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

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


                    //过滤迟付账单
                    if (tabBill != null && tabBill.IsOver != null && tabBill.IsOver == true)
                    {
                        tabBill = null;
                    }
                    //如果是预订账单
                    if (tabBill != null && tabBill.IsOrder == true && tabBill.Status == (byte)PosBillStatus.预订)
                    {
                        tabBill = null;
                    }

                    //如果已经锁台并且成功开台
                    if (tabLog != null && tabBill != null)
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
        /// 更新餐台状态信息
        /// </summary>
        /// <param name="refeid"></param>
        /// <param name="tabid"></param>
        /// <param name="status"></param>
        [AuthButton(AuthFlag.None)]
        public void updateTabStatus(PosTabStatus posTabStatus)
        {
            var tabStatusService = GetService<IPosTabStatusService>();
            var tabStatus = tabStatusService.Get(posTabStatus.Tabid);
            if (tabStatus != null)
            {
                tabStatus.TabStatus = posTabStatus.TabStatus;
                tabStatus.OpTabid = posTabStatus.OpTabid;
                tabStatus.OpenRecord = posTabStatus.OpenRecord;
                tabStatus.GuestName = posTabStatus.GuestName;
                tabStatus.OpenGuest = posTabStatus.OpenGuest == null ? 1 : posTabStatus.OpenGuest;
                tabStatusService.Update(tabStatus, new PosTabStatus());
                tabStatusService.AddDataChangeLog(OpLogType.Pos餐台状态修改);
                tabStatusService.Commit();
            }
        }

        #endregion 开台登记

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult EditOpenTab(OpenTabEditViewModel model)
        {
            var modelService = GetService<IPosBillService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Refeid, model.Billid, model.Billid);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

            PosBill bill = modelService.Get(model.Billid);

            PosTabStatus tabStatus = new PosTabStatus() { GuestName = model.Name, Tabid = bill.Tabid, TabStatus = (byte)PosTabStatusEnum.就座, OpTabid = model.Billid, OpenGuest = model.IGuest };
            updateTabStatus(tabStatus);

            var tabStatusService = GetService<IPosTabStatusService>();
            tabStatusService.SetTabStatus(CurrentInfo.HotelId, model.Refeid, (byte)PosTabStatusOpType.初始化, "", "", "");

            ActionResult result = _Edit(model, bill, OpLogType.Pos账单修改);

            return result;
        }

        #region 获取视图

        /// <summary>
        /// 根据模型返回餐台状态视图
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _AddOpenTab(OpenTabAddViewModel model)
        {
            var tabService = GetService<IPosTabService>();
            var tab = tabService.Get(model.Tabid);

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(tab.Refeid);

            model.OpenInfo = refe.OpenInfo;

            //查询预订账单数据
            if (!string.IsNullOrEmpty(model.BillId))
            {
                var service = GetService<IPosBillService>();
                var bill = service.Get(model.BillId);
                if (bill != null)
                {
                    model.IGuest = bill.IGuest;
                    model.Mobile = bill.Mobile;
                    model.Sale = bill.Sale;
                    model.Name = bill.Name;
                    model.CustomerTypeid = bill.CustomerTypeid;
                    model.CardNo = bill.CardNo;
                    model.BillId = bill.Billid;
                }
            }
            return PartialView("_AddOpenTab", model);
        }

        /// <summary>
        /// 根据模型返回餐台状态视图
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _AddSmearTab(OpenTabAddViewModel model)
        {
            var tabService = GetService<IPosTabService>();
            var tab = tabService.Get(model.Tabid);

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(tab.Refeid);

            model.OpenInfo = refe.OpenInfo;

            return PartialView("_AddSmearTab", model);
        }

        /// <summary>
        /// 编辑开台信息视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult _EditOpenTab(OpenTabEditViewModel model)
        {
            var service = GetService<IPosBillService>();
            var entity = service.Get(model.Billid);

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(entity.Refeid);

            if (refe == null || string.IsNullOrEmpty(refe.OpenInfo))
            {
                return Json(JsonResultData.Failure("当前营业点没有设置开台属性！"));
            }
            model.OpenInfo = refe.OpenInfo;
            var serializer = new JavaScriptSerializer();
            AutoSetValueHelper.SetValues(entity, model);
            model.OriginJsonData = ReplaceJsonDateToDateString(serializer.Serialize(entity));
            return PartialView("_EditOpenTab", model);
        }

        /// <summary>
        /// 选择营业点视图
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _PosRefeList()
        {
            var service = GetService<IPosRefeService>();
            var model = service.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
            return PartialView("_PosRefeList", model);
        }

        /// <summary>
        /// 选择餐台类型视图
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _PosTabTypeList(string refeId)
        {
            // CurrentInfo.PosId
            var service = GetService<IPosTabtypeService>();
            //  var model = service.GetTabtypeByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            var model = service.GetTabtypeByModuleOrRefe(CurrentInfo.HotelId, CurrentInfo.ModuleCode, refeId, CurrentInfo.PosId);
            return PartialView("_PosTabTypeList", model);
        }

        /// <summary>
        /// 选择餐台类型视图
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _LetterInput(LetterInputViewModel model)
        {
            ViewBag.Version = CurrentVersion;
            return PartialView("_LetterInput", model);
        }

        /// <summary>
        /// 选择餐台类型视图
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _PosSmearList(string model)
        {
            var s = ReplaceJsonDateToDateString(model);
            return PartialView("_PosSmearList", JsonConvert.DeserializeObject<List<PosBill>>(s));
        }

        /// <summary>
        /// 根据模型返回餐台状态视图
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PosTabStatusList(TabStatusViewModel model)
        {
            model.Code = model.Code ?? "";
            model.Refeid = model.Refeid ?? "";
            model.Tabtype = model.Tabtype ?? "";
            model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
            model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

            var pos = new PosPos();
            var posService = GetService<IPosPosService>();
            if (!string.IsNullOrEmpty(model.Mode))
            {
                pos = posService.GetPosByHid(CurrentInfo.HotelId).Where(w => w.PosMode == model.Mode).FirstOrDefault();
                if (pos != null)
                {
                    CurrentInfo.PosId = pos.Id;
                    CurrentInfo.PosName = pos.Name;
                }
            }
            else if (string.IsNullOrEmpty(CurrentInfo.PosId))
            {
                pos = posService.GetPosByHid(CurrentInfo.HotelId).FirstOrDefault();
                if (pos != null)
                {
                    CurrentInfo.PosId = pos.Id;
                    CurrentInfo.PosName = pos.Name;
                }
            }
            else
            {
                pos = posService.Get(CurrentInfo.PosId);
            }

            var refe = new PosRefe();
            var refeService = GetService<IPosRefeService>();
            var refeList = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
            if (string.IsNullOrWhiteSpace(model.Refeid))
            {

                if (refeList != null && refeList.Count > 0)
                {
                    foreach (var temp in refeList)
                    {
                        model.Refeid += ',' + temp.Id;
                    }
                    model.Refeid = model.Refeid.Trim(',');
                }
            }
            else
            {
                refe = refeService.Get(model.Refeid);
            }

            ViewBag.Business = pos.Business;

            var service = GetService<IPosTabStatusService>();
            service.SetTabStatus(CurrentInfo.HotelId, model.Refeid, (byte)PosTabStatusOpType.初始化, "", "", "");
            //设置餐台预定状态
            service.SetTabReserveStatus(CurrentInfo.HotelId);

            var list = service.GetPosTabStatusResult(CurrentInfo.HotelId, model.Code, model.Refeid, model.Tabtype, model.TabStatus.ToString(), model.PageIndex, model.PageSize);



            return PartialView("_PosTabStatusList", list);
        }

        #endregion 获取视图

        #region 获取列表

        /// <summary>
        /// 获取指定模块下的餐台列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListItemsForPosTabByModules([DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosTabService>();
            var list = service.GetPosTabByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 获取指定模块下的营业点
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListItemsForPosRefeByModules([DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosRefeService>();
            var list = service.GetRefeByPosAndModule(CurrentInfo.HotelId, CurrentInfo.PosId, CurrentInfo.ModuleCode);
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 获取指定模块下的餐台类型
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListItemsForPosTypeByModules([DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosTabtypeService>();
            var list = service.GetTabtypeByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 根据模型返回餐台状态视图
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ActionResult GetTabStatusStatistics(TabStatusViewModel model)
        {
            var refe = new PosRefe();
            var refeService = GetService<IPosRefeService>();
            if (string.IsNullOrWhiteSpace(model.Refeid))
            {
                var refeList = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
                if (refeList != null && refeList.Count > 0)
                {
                    foreach (var temp in refeList)
                    {
                        model.Refeid += ',' + temp.Id;
                    }
                    model.Refeid = model.Refeid.Trim(',');
                }
            }
            else
            {
                refe = refeService.Get(model.Refeid);
            }

            var service = GetService<IPosTabStatusService>();
            var result = service.GetPosTabStatusStatistics(CurrentInfo.HotelId, CurrentInfo.PosId, model.Refeid ?? "", model.Tabtype ?? "");
            return Json(JsonResultData.Successed(result));
        }

        /// <summary>
        /// 补打所选消费项目的点菜单
        /// </summary>
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult GetSmearList(string refeid, string tabid)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(tabid))
                {
                    var posService = GetService<IPosPosService>();
                    var pos = posService.GetPosByHid(CurrentInfo.HotelId, CurrentInfo.PosId);

                    var service = GetService<IPosBillService>();
                    var searList = service.GetSmearList(CurrentInfo.HotelId, tabid, pos.Business.Value.Date);
                    if (searList != null && searList.Count <= 1)
                    {
                        return Json(JsonResultData.Successed());
                    }
                    else if (searList != null && searList.Count > 1)
                    {
                        return Json(JsonResultData.Successed(searList));
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
            return Json(JsonResultData.Failure(""));
        }

        #endregion 获取列表

        #region 开台判断营业点是否有设置开台属性

        /// <summary>
        /// 验证营业点的开台属性
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult CheckRefeOpenInfo(OpenTabAddViewModel model)
        {
            var tabService = GetService<IPosTabService>();
            var tab = tabService.Get(model.Tabid);

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(tab.Refeid); //获取餐台信息

            if (string.IsNullOrWhiteSpace(refe.OpenInfo))
            {
                JsonResultData result = null;
                if (string.IsNullOrEmpty(model.BillId))
                {
                    result = addPosBillForOpenTab(model);
                }
                else
                {
                    //预订开台
                    result = OpenOrderBill(model);
                }


                if (result.Success)
                {
                    return Json(JsonResultData.Successed(result.Data));
                }
                else
                {
                    return Json(JsonResultData.Failure(result.Data));
                }
            }
            else
            {
                return Json(JsonResultData.Successed("0"));
            }
        }

        /// <summary>
        /// 通用开台
        /// </summary>
        /// <param name="model"></param>
        private JsonResultData addPosBillForOpenTab(OpenTabAddViewModel model)
        {
            if (model != null && model.Tabid != null)
            {
                var tabService = GetService<IPosTabService>();
                var tab = tabService.Get(model.Tabid);

                var refeService = GetService<IPosRefeService>();
                var refe = refeService.Get(tab.Refeid);

                var posService = GetService<IPosPosService>();
                var pos = posService.Get(CurrentInfo.PosId);

                var billService = GetService<IPosBillService>();
                var posBill = billService.GetLastBillId(CurrentInfo.HotelId, tab.Refeid, pos.Business);

                bool isexsit = billService.IsExists(CurrentInfo.HotelId, posBill.Billid, posBill.BillNo);
                if (isexsit)
                {
                    posBill = billService.GetLastBillId(CurrentInfo.HotelId, tab.Refeid, pos.Business);
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        DateTime openTime = DateTime.Now;
                        var posTabServiceService = GetService<IPosTabServiceService>();
                        var posTabService = posTabServiceService.GetPosTabService(CurrentInfo.HotelId, CurrentInfo.ModuleCode, tab.Refeid, tab.TabTypeid, null, (byte)PosITagperiod.随时, openTime);

                        var service = GetService<IPosBillService>();
                        PosBill bill = new PosBill()
                        {
                            Hid = CurrentInfo.HotelId,
                            Billid = posBill.Billid,
                            BillNo = posBill.BillNo,
                            MBillid = posBill.Billid,
                            InputUser = CurrentInfo.UserName,
                            BillDate = openTime,
                            IsService = true,
                            IsLimit = true,

                            ServiceRate = posTabService.Servicerate,
                            Limit = posTabService.NLimit,
                            IsByPerson = posTabService.IsByPerson == 0 ? false : true,
                            IHour = posTabService.LimitTime,
                            Discount = posTabService.Discount,

                            TabFlag = (byte)PosBillTabFlag.物理台,
                            Status = (byte)PosBillStatus.开台,
                            Shiftid = pos.ShiftId,
                            Shuffleid = refe.ShuffleId,

                            Refeid = refe.Id,
                            BillBsnsDate = pos.Business,
                            IGuest = model.IGuest == null ? 1 : model.IGuest,//默认为1

                            Name = model.Name,
                            Mobile = model.Mobile,
                            Tabid = tab.Id,
                            TabNo = tab.TabNo,
                            Sale = model.Sale,
                            CardNo = model.CardNo,
                            CustomerTypeid = model.CustomerTypeid,
                            OpenMemo = model.OpenMemo
                        };


                        //AutoSetValueHelper.SetValues(model, bill);
                        service.Add(bill);
                        service.AddDataChangeLog(OpLogType.Pos账单增加);
                        service.Commit();

                        PosTabStatus tabStatus = new PosTabStatus() { GuestName = model.Name, Tabid = model.Tabid, TabStatus = (byte)PosTabStatusEnum.就座, OpTabid = bill.Billid, OpenRecord = DateTime.Now, OpenGuest = bill.IGuest };
                        updateTabStatus(tabStatus);

                        var tabStatusService = GetService<IPosTabStatusService>();
                        tabStatusService.SetTabStatus(CurrentInfo.HotelId, refe.Id, (byte)PosTabStatusOpType.初始化, "", "", "");

                        //添加锁台记录
                        PosCommon common = new PosCommon();
                        common.AddTabLog(tab.Id, tab.TabNo, bill.Billid, model.ComputerName, refe);

                        if (model.ReturnType == 1)    //1：返回URL 2：返回Json
                        {
                            var url = Url.Action("Index", "PosInSingle", new { rnd = new Random().NextDouble() }) + "&refeid=" + System.Web.HttpUtility.UrlEncode(refe.Id) + "&tabid=" + System.Web.HttpUtility.UrlEncode(bill.Tabid) + "&billid=" + bill.Billid + "&tabFlag=" + bill.TabFlag + "&openFlag=A";
                            // return Redirect(url);
                            var _return = new { Flag = "1", Url = url };
                            return JsonResultData.Successed(_return);
                        }
                        else
                        {
                            var _return = new { Flag = "2", Url = bill.Billid };
                            return JsonResultData.Successed(_return);
                        }
                    }
                    catch (Exception ex)
                    {
                        return JsonResultData.Failure(ex);
                    }
                }
            }
            return JsonResultData.Failure("");
        }

        #endregion 开台判断营业点是否有设置开台属性

        #region 沽清

        [AuthButton(AuthFlag.Details)]
        public ActionResult _SellOut(string refeId)
        {
            var classServer = GetService<IPosItemClassService>();

            var refeids = ""; //收银点下的营业点集合

            var RefeService = GetService<IPosRefeService>();
            var refeList = RefeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
            if (string.IsNullOrEmpty(refeId))
            {
                foreach (var refe in refeList)
                {
                    refeids += refe.Id + ",";
                }
                refeids.Substring(0, refeids.Length - 1);
            }
            else
            {
                refeids = refeId;
            }


            ViewBag.PageTotal = classServer.GetPosItemClassTotal(CurrentInfo.HotelId, refeids, CurrentInfo.PosId);
            ViewBag.refeId = refeId;
            return PartialView("_SellOut");
        }

        /// <summary>
        ///沽清项目列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListPosSellOut([DataSourceRequest]DataSourceRequest request, string refeId)
        {
            var service = GetService<IPosSellOutService>();
            var refeids = ""; //收银点下的营业点集合

            var RefeService = GetService<IPosRefeService>();
            var refeList = RefeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
            if (string.IsNullOrEmpty(refeId))
            {
                foreach (var refe in refeList)
                {
                    refeids += refe.Id + ",";
                }
            }
            refeids.Substring(0, refeids.Length - 1);
            var list = service.GetItemListBySellOut(CurrentInfo.HotelId, CurrentInfo.ModuleCode, refeids ?? "");
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 添加沽清项目
        /// </summary>
        /// <param name="AddViewModel"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult AddPosSellOut(PosSellOutAddViewModel AddViewModel)
        {
            var service = GetService<IPosSellOutService>();
            //验证数据
            var boolResult = service.IsExists(CurrentInfo.HotelId, AddViewModel.itemId);
            if (boolResult)
            {
                //有数据就修改
                var oldEntity = service.GetPosSelloutByItemId(CurrentInfo.HotelId, AddViewModel.itemId);
                if (oldEntity != null)
                {
                    var newEntity = new PosSellout();
                    //赋值
                    AutoSetValueHelper.SetValues(oldEntity, newEntity);
                    //营业点不为空并且营业点不存在沽清表中
                    if (string.IsNullOrWhiteSpace(newEntity.RefeId) && !string.IsNullOrWhiteSpace(AddViewModel.refeId))
                    {
                        newEntity.RefeId = AddViewModel.refeId;
                    }
                    else if (!string.IsNullOrWhiteSpace(newEntity.RefeId) && !string.IsNullOrWhiteSpace(AddViewModel.refeId))
                    {
                        if (!newEntity.RefeId.Contains(AddViewModel.refeId))
                        {
                            newEntity.RefeId += "," + AddViewModel.refeId;
                        }
                    }
                    //  newEntity.UnitId = "";
                    //如果单位不为空则判断是否添加单位还是删除单位
                    if (!string.IsNullOrWhiteSpace(AddViewModel.unitId))
                    {
                        var UnitId = newEntity.UnitId ?? "";
                        if (UnitId.Contains(AddViewModel.unitId))
                        {
                            List<string> newUnitList = new List<string>();
                            var unitArr = newEntity.UnitId.Split(',').ToList();
                            foreach (var unit in unitArr)
                            {
                                //表中不等于本次选择的单位添加到新的集合中
                                if (unit != AddViewModel.unitId)
                                {
                                    newUnitList.Add(unit);
                                }
                            }
                            newEntity.UnitId = string.Join(",", newUnitList.ToArray());
                        }
                        else
                        {
                            //如果单位不存在，则新增
                            if (string.IsNullOrWhiteSpace(newEntity.UnitId))
                            {
                                newEntity.UnitId = AddViewModel.unitId;
                            }
                            else
                            {
                                newEntity.UnitId += "," + AddViewModel.unitId;
                            }
                        }
                    }
                    newEntity.SellStatus = 0;
                    newEntity.ModiDate = DateTime.Now;
                    newEntity.ModiUser = CurrentInfo.UserName;
                    service.Update(newEntity, oldEntity);
                    service.AddDataChangeLog(OpLogType.Pos沽清项目修改);
                    service.Commit();
                }
            }
            else
            {
                //没有数据就添加
                var model = new PosSellout()
                {
                    Id = Guid.NewGuid(),
                    Hid = CurrentInfo.HotelId,
                    ItemId = AddViewModel.itemId,
                    ItemCode = AddViewModel.itemCode,
                    ItemName = AddViewModel.itemName,
                    SellStatus = 0,
                    Module = CurrentInfo.ModuleCode,
                    RefeId = AddViewModel.refeId,
                    UnitId = AddViewModel.unitId,
                    CreateDate = DateTime.Now,
                    TransUser = CurrentInfo.UserName
                };
                service.Add(model);
                service.AddDataChangeLog(OpLogType.Pos沽清项目增加);
                service.Commit();
            }
            return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 修改沽清表状态
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult UpdatePosSellout(string Id)
        {
            var service = GetService<IPosSellOutService>();
            try
            {
                var oldEntity = service.Get(new Guid(Id));
                if (oldEntity != null)
                {
                    var newEntity = new PosSellout();
                    //赋值
                    AutoSetValueHelper.SetValues(oldEntity, newEntity);
                    newEntity.SellStatus = 1;
                    newEntity.ModiDate = DateTime.Now;
                    newEntity.ModiUser = CurrentInfo.UserName;

                    service.Update(newEntity, oldEntity);
                    service.AddDataChangeLog(OpLogType.Pos沽清项目修改);
                    service.Commit();
                    return Json(JsonResultData.Successed());
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }

            return Json(JsonResultData.Failure("数据不存在"));
        }

        #endregion 沽清

        #region 复位

        /// <summary>
        /// 复位列表视图
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Members)]
        public ActionResult _Restoration(string refeId)
        {
            //餐台日志记录
            var service = GetService<IPosTabLogService>();
            ViewBag.refeID = refeId;    //营业点
            return PartialView("_Restoration");
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult ListPosTabLog([DataSourceRequest]DataSourceRequest request, string refeId)
        {
            var service = GetService<IPosTabLogService>();
            var list = service.GetPosTabLogListByRefeId(CurrentInfo.HotelId, refeId ?? "");
            return Json(list.ToDataSourceResult(request));
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult UpdateRestoration(string ids)
        {
            var idList = ids.Split('|');
            var service = GetService<IPosTabLogService>();
            foreach (var item in idList)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var model = service.Get(new Guid(item));
                    if (model != null)
                    {
                        service.Delete(model);
                        service.AddDataChangeLog(OpLogType.Pos锁台删除);
                        service.Commit();
                    }
                }
            }
            return Json(JsonResultData.Successed());
        }

        #endregion 复位

        #region 添加锁台记录

        /// <summary>
        /// 进入正在操作的餐台，添加锁台记录
        /// </summary>
        /// <param name="addViewModel"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult AddTabLog(PosTabLogAddViewModel addViewModel)
        {
            var tabLogService = GetService<IPosTabLogService>();    //餐台记录
            PosCommon common = new PosCommon();
            var tabService = GetService<IPosTabService>();
            var tab = tabService.Get(addViewModel.Tabid);

            var billService = GetService<IPosBillService>();
            var bill = billService.GetPosBillByTabId(CurrentInfo.HotelId, tab.Refeid, addViewModel.Tabid, addViewModel.Billid ?? "");

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(tab.Refeid);


            if (bill != null)
            {
                //清楚锁台记录
                string computer = addViewModel.ComputerName ?? "";
                var tabLogList = tabLogService.GetPosTabLogListByTab(CurrentInfo.HotelId, refe.Id, addViewModel.Tabid, addViewModel.TabNo);
                if (tabLogList != null && tabLogList.Count > 0)
                {
                    foreach (var tabLog in tabLogList)
                    {
                        if (tabLog.Billid == bill.Billid && tabLog.TransUser == CurrentInfo.UserName)
                        {
                            tabLogService.Delete(tabLog);
                            tabLogService.AddDataChangeLog(OpLogType.Pos锁台删除);
                            tabLogService.Commit();
                        }
                    }
                }


                PosTabLog posTabLog = new PosTabLog()
                {
                    Id = Guid.NewGuid(),
                    Hid = CurrentInfo.HotelId,
                    Msg = string.Format($"{addViewModel.TabNo}餐台被{computer} => {CurrentInfo.UserName}在操作"),
                    Status = (byte)PosTabLogStatus.开台自动锁台,
                    Computer = computer,
                    ConnectDate = DateTime.Now,
                    Module = refe.Module,
                    TransUser = CurrentInfo.UserName,
                    CreateDate = DateTime.Now
                };
                posTabLog.Refeid = refe.Id;
                posTabLog.Billid = bill.Billid;
                posTabLog.Tabid = addViewModel.Tabid;
                posTabLog.TabNo = addViewModel.TabNo;
                posTabLog.Refeid = refe.Id;
                //   AutoSetValueHelper.SetValues(model, posTabLog);
                tabLogService.Add(posTabLog);
                tabLogService.AddDataChangeLog(OpLogType.Pos锁台增加);
                tabLogService.Commit();
            }
            return Json(JsonResultData.Successed());
        }

        #endregion 添加锁台记录

        /// <summary>
        /// 获取故障的出品打印机
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult GetFaultPrinter()
        {
            var prodPrinterService = GetService<IPosProdPrinterService>();
            var prodPrinters = prodPrinterService.GetPosProdPrinterByFault(CurrentInfo.HotelId, ModuleCode.CY.ToString());

            if (prodPrinters != null && prodPrinters.Count > 0)
            {
                var message = "";
                foreach (var temp in prodPrinters)
                {
                    message += string.Format("<span>　{0}-{1}：<font color='red'>{2}</font></span>", temp.Code, temp.Cname, temp.Remark);
                }
                return Json(JsonResultData.Successed(message));
            }

            return Json(JsonResultData.Successed(""));
        }

        #region 复制餐台菜式
        [AuthButton(AuthFlag.None)]
        public ActionResult _CopyTabList(string tabid, string mode = "")
        {
            try
            {
                TabStatusViewModel model = new TabStatusViewModel();
                var tabStatusService = GetService<IPosTabStatusService>();
                model.PageTotal = tabStatusService.GetPosTabStatusResultTotal(CurrentInfo.HotelId, model.Code, model.Refeid, model.Tabtype, null);

                //收银点的当前营业日
                var pos = new PosPos();
                var posService = GetService<IPosPosService>();
                if (!string.IsNullOrEmpty(mode))
                {
                    pos = posService.GetPosByHid(CurrentInfo.HotelId).Where(w => w.PosMode == mode).FirstOrDefault();
                    if (pos != null)
                    {
                        CurrentInfo.PosId = pos.Id;
                        CurrentInfo.PosName = pos.Name;
                    }
                }
                else if (string.IsNullOrEmpty(CurrentInfo.PosId))
                {
                    pos = posService.GetPosByHid(CurrentInfo.HotelId).FirstOrDefault();
                    if (pos != null)
                    {
                        CurrentInfo.PosId = pos.Id;
                        CurrentInfo.PosName = pos.Name;
                    }
                }
                else
                {
                    pos = posService.Get(CurrentInfo.PosId);
                }
                model.Business = pos.Business;

                var refeService = GetService<IPosRefeService>();
                var list = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
                if (list != null && list.Count > 0)
                {
                    model.IsOpenBrush = list[0].IsOpenBrush;
                }

                //楼面自动刷新间隔时间
                var pmsParaService = GetService<IPmsParaService>();
                ViewBag.TabStatusRefreshInterval = pmsParaService.GetValue(CurrentInfo.HotelId, "tabStatusRefreshInterval");
                //提示信息显示时间
                ViewBag.TipsTime = pmsParaService.GetValue(CurrentInfo.HotelId, "tipsTime");
                //是否是通过快捷方式进行自动登录的用户
                ViewBag.IsAutoLogin = CurrentInfo.UserCode.Equals("posAutoLogin", StringComparison.CurrentCultureIgnoreCase) || CurrentInfo.UserName.Equals("Pos自动登录", StringComparison.CurrentCultureIgnoreCase) ? true : false;

                ViewBag.Version = CurrentVersion;
                model.Tabid = tabid;
                return PartialView("_CopyTabList", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 弹出所有未开台餐台选择界面
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _CopyTabView(TabStatusViewModel model)
        {
            try
            {
                model.Code = model.Code ?? "";
                model.Refeid = model.Refeid ?? "";
                model.Tabtype = model.Tabtype ?? "";
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;
                var pos = new PosPos();
                var posService = GetService<IPosPosService>();
                if (!string.IsNullOrEmpty(model.Mode))
                {
                    pos = posService.GetPosByHid(CurrentInfo.HotelId).Where(w => w.PosMode == model.Mode).FirstOrDefault();
                    if (pos != null)
                    {
                        CurrentInfo.PosId = pos.Id;
                        CurrentInfo.PosName = pos.Name;
                    }
                }
                else if (string.IsNullOrEmpty(CurrentInfo.PosId))
                {
                    pos = posService.GetPosByHid(CurrentInfo.HotelId).FirstOrDefault();
                    if (pos != null)
                    {
                        CurrentInfo.PosId = pos.Id;
                        CurrentInfo.PosName = pos.Name;
                    }
                }
                else
                {
                    pos = posService.Get(CurrentInfo.PosId);
                }

                var refe = new PosRefe();
                var refeService = GetService<IPosRefeService>();
                if (string.IsNullOrWhiteSpace(model.Refeid))
                {
                    var refeList = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
                    if (refeList != null && refeList.Count > 0)
                    {
                        foreach (var temp in refeList)
                        {
                            model.Refeid += ',' + temp.Id;
                        }
                        model.Refeid = model.Refeid.Trim(',');
                    }
                }
                else
                {
                    refe = refeService.Get(model.Refeid);
                }

                ViewBag.Business = pos.Business;

                var service = GetService<IPosTabStatusService>();
                service.SetTabStatus(CurrentInfo.HotelId, model.Refeid, (byte)PosTabStatusOpType.初始化, "", "", "");

                var list = service.GetPosTabStatusResult(CurrentInfo.HotelId, model.Code, model.Refeid, model.Tabtype, model.TabStatus.ToString(), model.PageIndex, model.PageSize).Where(t => t.TabStatus == 5 || t.TabStatus == 7).ToList();
                return PartialView("_CopyTabView", list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 复制餐台菜式
        /// </summary>
        /// <param name="sourcebillid">原单据id</param>
        /// <param name="targettabid">目标餐台</param>
        /// <param name="CustomerTypeid">新台客户类型</param>
        /// <param name="iGuest">新台人数</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult CopyTab(string sourcebillid, string targettabid, string CustomerTypeid, int iGuest)
        {
            try
            {
                var tabService = GetService<IPosTabService>();
                var tab = tabService.Get(targettabid);
                var refeService = GetService<IPosRefeService>();
                var refe = refeService.Get(tab.Refeid);
                var posService = GetService<IPosPosService>();
                var pos = posService.Get(CurrentInfo.PosId);
                var billService = GetService<IPosBillService>();
                var posBill = billService.GetLastBillId(CurrentInfo.HotelId, tab.Refeid, pos.Business);
                bool isexsit = billService.IsExists(CurrentInfo.HotelId, posBill.Billid, posBill.BillNo);
                if (isexsit)
                {
                    posBill = billService.GetLastBillId(CurrentInfo.HotelId, tab.Refeid, pos.Business);
                }
                DateTime openTime = DateTime.Now;
                var posTabServiceService = GetService<IPosTabServiceService>();
                var posTabService = posTabServiceService.GetPosTabService(CurrentInfo.HotelId, CurrentInfo.ModuleCode, tab.Refeid, tab.TabTypeid, null, (byte)PosITagperiod.随时, openTime);
                var service = GetService<IPosBillService>();
                //根据原餐台id查找当前所要复制的主单据
                var oldbill = service.GetPosBillByBillid(CurrentInfo.HotelId, sourcebillid);
                //新单
                PosBill bill = new PosBill();
                AutoSetValueHelper.SetValues(oldbill, bill);
                bill.InputUser = CurrentInfo.UserName;
                bill.BillDate = openTime;
                bill.Billid = posBill.Billid;
                bill.BillNo = posBill.BillNo;
                bill.MBillid = posBill.Billid;
                bill.Refeid = tab.Refeid;
                bill.Tabid = tab.Id;
                bill.TabNo = tab.TabNo;
                bill.CustomerTypeid = CustomerTypeid;
                bill.IGuest = iGuest;
                bill.ServiceRate = posTabService.Servicerate;
                bill.Limit = posTabService.NLimit;
                bill.IsByPerson = posTabService.IsByPerson == 0 ? false : true;
                bill.IHour = posTabService.LimitTime;
                bill.Discount = posTabService.Discount;
                bill.TabFlag = (byte)PosBillTabFlag.物理台;
                bill.Status = (byte)PosBillStatus.开台;
                bill.Shiftid = pos.ShiftId;
                bill.Shuffleid = refe.ShuffleId;
                bill.BillBsnsDate = pos.Business;
                service.Add(bill);
                service.AddDataChangeLog(OpLogType.Pos账单增加);
                service.Commit();
                //查找旧单明细，将明细复制给新单
                var detailservice = GetService<IPosBillDetailService>();
                var olddetaillist = detailservice.GetPosBillDetailsByTabid(CurrentInfo.HotelId, sourcebillid, "D");
                foreach (var item in olddetaillist)
                {
                    PosBillDetail newdetail = new PosBillDetail();
                    AutoSetValueHelper.SetValues(item, newdetail);
                    newdetail.Billid = bill.Billid;
                    newdetail.MBillid = bill.Billid;
                    newdetail.Tabid = targettabid;
                    detailservice.Add(newdetail);
                    detailservice.Commit();
                }
                PosTabStatus tabStatus = new PosTabStatus() { GuestName = "", Tabid = targettabid, TabStatus = (byte)PosTabStatusEnum.就座, OpTabid = bill.Billid, OpenRecord = DateTime.Now, OpenGuest = bill.IGuest };
                updateTabStatus(tabStatus);
                //添加锁台记录
                PosCommon common = new PosCommon();
                common.AddTabLog(targettabid, tab.TabNo, bill.Billid, "", refe);

                return Json(JsonResultData.Successed("复制成功"));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.ToString()));
            }
        }
        #endregion

        #region 预订开台
        [AuthButton(AuthFlag.Add)]
        public PartialViewResult _OrderReserver(string tabId, string tabno, string billid, string status)
        {
            //查询当前日期下所有的预定信息
            //var service = GetService<IPosReserveService>();
            //var list = service.GetOrderBillByTabId(CurrentInfo.HotelId, tabId, DateTime.Now.Date);
            ViewBag.TabId = tabId;
            ViewBag.Tabno = tabno;
            ViewBag.Billid = billid;
            ViewBag.Status = status;
            return PartialView("_OrderReserver");
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult ListOrderReserver([DataSourceRequest]DataSourceRequest request, string tabId)
        {
            var service = GetService<IPosReserveService>();
            var list = service.GetOrderBillByTabId(CurrentInfo.HotelId, tabId, DateTime.Now.Date);
            return Json(list.ToDataSourceResult(request));
        }
        #endregion


        /// <summary>
        /// 通用的预订账单开台
        /// </summary>
        /// <returns></returns>
        private JsonResultData OpenOrderBill(OpenTabAddViewModel addViewModel)
        {
            var billId = addViewModel.BillId;
            //预订开台
            var service = GetService<IPosBillService>();
            try
            {
                var bill = service.Get(billId);
                if (bill == null)
                {
                    return JsonResultData.Failure("未找到预订账单！");
                }


                var tabService = GetService<IPosTabService>();
                var tab = tabService.Get(addViewModel.Tabid);

                var refeService = GetService<IPosRefeService>();
                var refe = refeService.Get(tab.Refeid);

                var posService = GetService<IPosPosService>();
                var pos = posService.Get(refe.PosId);

                var posBill = service.GetLastBillId(CurrentInfo.HotelId, refe.Id, pos.Business);
                if (posBill != null)
                {
                    bill.BillNo = posBill.BillNo;

                }
                DateTime openTime = DateTime.Now;
                var posTabServiceService = GetService<IPosTabServiceService>();
                var posTabService = posTabServiceService.GetPosTabService(CurrentInfo.HotelId, CurrentInfo.ModuleCode, tab.Refeid, tab.TabTypeid, null, (byte)PosITagperiod.随时, openTime);

                bill.CustomerTypeid = addViewModel.CustomerTypeid;
                bill.IGuest = addViewModel.IGuest;
                bill.Sale = addViewModel.Sale;
                bill.Status = (byte)PosBillStatus.开台;
                bill.InputUser = CurrentInfo.UserName;
                bill.BillDate = openTime;
                bill.IsService = true;
                bill.IsLimit = true;
                bill.BillBsnsDate = pos.Business;
                bill.ServiceRate = posTabService != null ? (posTabService.Servicerate == null ? 0 : posTabService.Servicerate) : 0;
                bill.Limit = posTabService != null ? (posTabService.NLimit == null ? 0 : posTabService.NLimit) : 0;
                bill.IsByPerson = posTabService.IsByPerson == 0 ? false : true;
                bill.IHour = posTabService.LimitTime;
                bill.Discount = posTabService != null ? (posTabService.Discount == null ? 1 : posTabService.Discount) : 1;

                bill.TabFlag = (byte)PosBillTabFlag.物理台;
                bill.Status = (byte)PosBillStatus.开台;
                bill.Shiftid = pos.ShiftId;
                bill.Shuffleid = refe.ShuffleId;

                service.Update(bill, new PosBill());
                AddOperationLog(OpLogType.Pos账单增加, "预订账单开台,操作员：" + CurrentInfo.UserName, bill.BillNo);
                service.Commit();

                PosTabStatus tabStatus = new PosTabStatus() { GuestName = addViewModel.Name, Tabid = addViewModel.Tabid, TabStatus = (byte)PosTabStatusEnum.就座, OpTabid = bill.Billid, OpenRecord = DateTime.Now, OpenGuest = bill.IGuest };
                updateTabStatus(tabStatus);

                var tabStatusService = GetService<IPosTabStatusService>();
                tabStatusService.SetTabStatus(CurrentInfo.HotelId, refe.Id, (byte)PosTabStatusOpType.初始化, "", "", "");

                //添加开台记录表
                var common = GetService<PosCommon>();
                common.AddTabLog(addViewModel.Tabid, addViewModel.TabNo, bill.Billid, addViewModel.ComputerName, refe);

                //修改餐台最早预抵时间
                common.SetTabServerArrDate(CurrentInfo.HotelId, bill.Tabid);


                var url = Url.Action("Index", "PosInSingle", new { rnd = new Random().NextDouble() }) + "&refeid=" + System.Web.HttpUtility.UrlEncode(refe.Id) + "&tabid=" + System.Web.HttpUtility.UrlEncode(bill.Tabid) + "&billid=" + bill.Billid + "&tabFlag=" + bill.TabFlag + "&openFlag=A";
              
                // return Redirect(url);
                var _return = new { Flag = "1", Url = url };

                return JsonResultData.Successed(_return);
            }
            catch (Exception ex)
            {

                return JsonResultData.Failure(ex.Message.ToString());
            }
        }
    }
}