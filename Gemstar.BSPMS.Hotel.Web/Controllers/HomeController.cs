using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Web.Models.Home;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using System;
using Gemstar.BSPMS.Hotel.Services.RoomStatusManage;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosShuffleChange;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;

namespace Gemstar.BSPMS.Hotel.Web.Controllers
{
    /// <summary>
    /// 由于此controller是默认controller，所以在程序启动时，用户还没有登录时也会首先创建此controller实例，然后判断是否登录再转到登录界面去
    /// 所以此controller的构造函数中不能包含需要酒店业务数据库实例的那些服务接口，在方法中需要那些服务接口时，再使用dependency来创建
    /// </summary>
    [NotAuth]
    public class HomeController : BaseController
    {
        #region 分店班次选择

        /// <summary>
        /// 分店班次选择
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var sessionInvalidResult = CheckSession();
            if (sessionInvalidResult != null)
            {
                return sessionInvalidResult;
            }
            if (AddTryLog(false))//体验者未输入手机号
            {
                return Redirect(Url.Action("TryInfo", "Account"));
            }
            var defaultHotelId = CurrentInfo.HotelId;
            var defaultHotelName = CurrentInfo.HotelName;
            var shiftService = GetService<IShiftService>();
            ViewBag.product = GetProduct();

            var paraService = GetService<IPmsParaService>();
            //是否显示客户Logo
            var hotelInfoService = GetService<IHotelInfoService>();
            var hotelLogoAndNameInfo = paraService.GetHotelLogoAndNameInfo(defaultHotelId, hotelInfoService);
            ViewBag.hotelLogoAndNameInfo = hotelLogoAndNameInfo;
            if (!string.IsNullOrEmpty(hotelLogoAndNameInfo.GSSysTitle))
            {
                ViewBag.Title = hotelLogoAndNameInfo.GSSysTitle;
            }

            if (CurrentInfo.IsGroup)
            {
                var userService = GetService<IPmsUserService>();
                var productService = GetService<IProductService>();

                var hotels = userService.GetResortListForOperator(CurrentInfo.GroupId, CurrentInfo.UserId);
                var posHotels = productService.GetHotels(CurrentInfo.GroupId, ProductType.Pos.ToString());

                hotels = hotels.Where(w => posHotels.Contains(w.Hid)).ToList();
                if (hotels.Count > 0)
                {
                    //如果操作员有权限的酒店中，不包含默认的酒店代码，则取有权限的第一家酒店
                    var defaultHotelExists = hotels.Count(w => w.Hid == defaultHotelId) > 0;
                    if (!defaultHotelExists)
                    {
                        defaultHotelName = hotels[0].Hname;
                        defaultHotelId = hotels[0].Hid;
                    }
                }

                var hotelAndPosModel = new SelectHotelAndPosViewModel
                {
                    ResortList = hotels,
                    CurrentHotelId = defaultHotelId,
                    CurrentHotelName = defaultHotelName,
                    CurrentPosId = CurrentInfo.PosId,
                    CurrentPosName = CurrentInfo.PosName,
                };

                ViewBag.Title = defaultHotelName;
                ViewBag.HotelId = defaultHotelId;
                return View("SelectHotelAndPos", hotelAndPosModel);
            }
            var posModel = new SelectPosViewModel
            {
                CurrentPosId = CurrentInfo.PosId,
                CurrentPosName = CurrentInfo.PosName,
            };

            ViewBag.Title = defaultHotelName;
            ViewBag.HotelId = defaultHotelId;

            var hotelService = GetService<IHotelInfoService>();
            var hotel = hotelService.ListValidHotels().Where(m => m.Hid == CurrentInfo.HotelId).FirstOrDefault();
            var hotelType = hotel.CateringServicesTypeOrDefault; //酒店类型（）
            ViewData["hotelType"] = hotelType;//酒店类型
            ViewData["hotelModule"] = hotel.CateringServicesModuleOrDefault;//酒店模块

            //是否显示系统
            var authListService = GetService<IAuthListService>();
            var isSystemSetup = authListService.GetUserAuthLists(CurrentInfo.GroupId, CurrentInfo.HotelId, CurrentInfo.UserId).Any(a => a.AuthCode.Contains("p99"));
            ViewBag.IsSystemSetup = CurrentInfo.IsRegUser ? true : isSystemSetup;

            if (hotelType == "B" || hotelType == "C")
            {
                var service = GetService<IPosPosService>();
                var posPosModel = service.GetPosByHid(CurrentInfo.HotelId).Where(m => (m.IStatus == (byte)EntityStatus.启用 || m.IStatus == null)).FirstOrDefault();

                var _currentInfo = GetService<ICurrentInfo>();
                _currentInfo.PosId = posPosModel.Id;
                _currentInfo.PosName = posPosModel.Name;
                _currentInfo.SaveValues();
                return Redirect("PosManage/PosInSingle");
                //return View("PosInSingle",posModel);
            }
            else
            {
                return View("SelectPos", posModel);
            }
        }

        [JsonException]
        public ActionResult SetHotelAndShift(string hid, string hname, string shiftId, string shiftName, int checkShiftStatu = 1)
        {
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(hname))
            {
                return Json(JsonResultData.Failure("请选择分店"));
            }
            if (CurrentInfo.IsGroup)
            {
                var userService = GetService<IPmsUserService>();
                var ResortList = userService.GetResortListForOperator(CurrentInfo.GroupId, CurrentInfo.UserId);
                if (!ResortList.Exists(c => c.Hid == hid && c.Hname == hname))
                {
                    return Json(JsonResultData.Failure("请选择分店"));
                }
            }

            CurrentInfo.HotelId = hid;
            CurrentInfo.HotelName = hname;
            ViewBag.product = GetProduct();
            var sing = GetService<ISysParaService>().GetHotelFunctionses(hid);
            var data = sing.Where(s => s.FuncCode == "FuncSignature").FirstOrDefault();
            CurrentInfo.Signature = data == null ? "0" : data.Isvalid == null ? "0" : data.Isvalid == true ? "1" : "0";
            return SetShift(hid, hname, shiftId, shiftName, checkShiftStatu);
        }

        [JsonException]
        public ActionResult SetShift(string hid, string hname, string shiftId, string shiftName, int checkShiftStatu = 1)
        {
            var shiftService = GetService<IShiftService>();
            var shifts = shiftService.GetShiftsAvailable(CurrentInfo.HotelId);
            //取出当前酒店的营业日，以便同时记录到日志中
            var hotelStatusService = GetService<IHotelStatusService>();
            var businessDay = hotelStatusService.GetBusinessDate(CurrentInfo.HotelId).ToString("yyyy-MM-dd");
            if (string.IsNullOrWhiteSpace(shiftId) || string.IsNullOrWhiteSpace(shiftName))
            {
                if (shifts.Count <= 0)
                {
                    CurrentInfo.ShiftId = "";
                    CurrentInfo.ShiftName = "";
                    CurrentInfo.SaveValues();
                    AddOperationLog(OpLogType.登录班次, string.Format("班次：{0}，登录营业日：{1}", shiftName, businessDay));
                    AddTryLog(true);
                    return Json(JsonResultData.Successed(new SetShiftResultModel { ResultType = ShiftResultType.Redirect, Message = Url.Action("Main", new { rnd = DateTime.Now.Ticks, hid, hname, shiftId, shiftName }) }));
                }
            }
            if (checkShiftStatu == 1)
            {
                //检查班次状态，如果是已经关闭状态则返回提示是否继续
                var shift = shiftService.Get(shiftId);
                if (shift.LoginStatus == Services.Enums.ShiftLoginStatus.已关闭)
                {
                    //判断是否有重开班次的权限
                    var pageId = "99030";
                    var authCheck = GetService<IAuthCheck>();
                    var hasAuth = authCheck.HasAuth(CurrentInfo.UserId, pageId, (int)AuthFlag.Open, CurrentInfo.HotelId);
                    if (!hasAuth)
                    {
                        return Json(JsonResultData.Failure("你没有重开已关闭班次的权限"));
                    }
                    return Json(JsonResultData.Successed(new SetShiftResultModel { ResultType = ShiftResultType.Promot, Message = "所选的班次已经关闭，是否重新开启该班次？" }));
                }
                else if (shift.LoginStatus == Services.Enums.ShiftLoginStatus.未开)
                {
                    return Json(JsonResultData.Successed(new SetShiftResultModel { ResultType = ShiftResultType.Promot, Message = "所选的班次未开，是否现在开启该班次？" }));
                }
            }
            var result = shiftService.OpenShift(CurrentInfo.HotelId, shiftId);
            if (result.Success)
            {
                if ((string.IsNullOrWhiteSpace(CurrentInfo.ShiftId) && string.IsNullOrWhiteSpace(CurrentInfo.ShiftName)) || (CurrentInfo.ShiftId == shiftId && CurrentInfo.ShiftName == shiftName))
                    AddOperationLog(OpLogType.登录班次, string.Format("班次：{0}，登录营业日：{1}", shiftName, businessDay));
                else
                    AddOperationLog(OpLogType.登录班次, string.Format("班次：{1}=>{2}，登录营业日：{0}", businessDay, CurrentInfo.ShiftName, shiftName));

                CurrentInfo.ShiftId = shiftId;
                CurrentInfo.ShiftName = shiftName;
                CurrentInfo.SaveValues();
                AddTryLog(true);
                return Json(JsonResultData.Successed(new SetShiftResultModel { ResultType = ShiftResultType.Redirect, Message = Url.Action("Main", new { rnd = DateTime.Now.Ticks, hid, hname, shiftId, shiftName }) }));
            }
            return Json(result);
        }

        public ActionResult SelectPos()
        {
            ViewBag.Title = CurrentInfo.HotelName;
            ViewBag.HotelId = CurrentInfo.HotelId;
            var hotelService = GetService<IHotelInfoService>();
            var hotel = hotelService.ListValidHotels().Where(m => m.Hid == CurrentInfo.HotelId).FirstOrDefault();
            var hotelType = hotel == null ? "" : hotel.CateringServicesType;
            ViewData["hotelType"] = hotelType;
            //  ViewBag.Title1 = CurrentInfo.HotelName + "Test";
            return View(new SelectPosViewModel());
        }

        public JsonResult SaveLoginPos(SelectPosViewModel model)
        {
            var _currentInfo = GetService<ICurrentInfo>();
            _currentInfo.PosId = model.CurrentPosId;
            _currentInfo.PosName = model.CurrentPosName;

            //2018年11月27日15:07:38 snow  选择收银点的同时把班次保存到缓存中
            var service = GetService<IPosPosService>();
            if (!string.IsNullOrEmpty(model.CurrentPosId))
            {
                var posModel = service.Get(model.CurrentPosId);
                if (posModel != null)
                {
                    var shiftService = GetService<IPosShiftService>();
                    var shiftModle = shiftService.Get(posModel.ShiftId);
                    _currentInfo.ShiftId = shiftModle == null ? "" : shiftModle.Id;
                    _currentInfo.ShiftName = shiftModle == null ? "" : shiftModle.Name;
                }
            }

            _currentInfo.SaveValues();
            return Json(JsonResultData.Successed(""));
        }

        private bool AddTryLog(bool isLog)
        {
            bool result = false;
            //判断是否是试用用户，是则试用者手机号信息到日志中
            //取出运营系统参数中的试用参数信息
            var sysParaService = GetService<ISysParaService>();
            var tryHotelPara = sysParaService.Get("TryHotelId");
            var tryUserName = sysParaService.Get("TryUsername");
            var tryHotelId = tryHotelPara == null ? "" : tryHotelPara.Value;
            var tryUser = tryUserName == null ? "" : tryUserName.Value;
            if (CurrentInfo.HotelId.Equals(tryHotelId, System.StringComparison.OrdinalIgnoreCase) && CurrentInfo.UserCode.Equals(tryUser, System.StringComparison.OrdinalIgnoreCase))
            {
                //判断是否有提交体验cookie，没有则转到试用信息收集界面
                var tryCookie = Request.Cookies[AccountController.TryCookieName];
                if (tryCookie != null)
                {
                    if (isLog)
                    {
                        var mobile = tryCookie.Value;
                        if (mobile.Length >= 7)
                        {
                            mobile = mobile.Substring(0, 3) + "****" + mobile.Substring(7, mobile.Length - 7);
                        }

                        AddOperationLog(OpLogType.登录试用体验, string.Format("体验者手机号：{0}", mobile));
                    }
                }
                else
                {
                    result = true;
                    //Response.Redirect(Url.Action("TryInfo","Account"));
                }
            }
            return result;
        }

        [HttpPost]
        public JsonResult GetShiftsAvailable(string hid)
        {
            if (CurrentInfo.IsGroup)
            {
                var userService = GetService<IPmsUserService>();
                var ResortList = userService.GetResortListForOperator(CurrentInfo.GroupId, CurrentInfo.UserId);
                if (ResortList.Exists(c => c.Hid == hid))
                {
                    var shiftService = GetService<IShiftService>();
                    var shifts = shiftService.GetShiftsAvailable(hid);
                    return Json(JsonResultData.Successed(shifts));
                }
            }
            return Json(JsonResultData.Failure("请选择分店"));
        }

        /// <summary>
        /// 酒店模块
        /// </summary>
        /// <param name="id">收银点id</param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult HotelModule(string id)
        {
            var posService = GetService<IPosPosService>();
            var pos = posService.GetPosByHid(CurrentInfo.HotelId, id);
            if (pos != null)
            {
                CurrentInfo.PosId = pos.Id;
                CurrentInfo.PosName = pos.Name;
                CurrentInfo.SaveValues();
            }

            var service = GetService<IHotelInfoService>();
            var hotel = service.ListValidHotels().Where(m => m.Hid == CurrentInfo.HotelId).FirstOrDefault();

            //是否显示系统
            var authListService = GetService<IAuthListService>();
            var isSystemSetup = authListService.GetUserAuthLists(CurrentInfo.GroupId, CurrentInfo.HotelId, CurrentInfo.UserId).Any(a => a.AuthCode.Contains("p99"));
            ViewBag.IsSystemSetup = CurrentInfo.IsRegUser ? true : isSystemSetup;

            if (hotel != null)
            {
                return PartialView("HotelModule", hotel.CateringServicesModuleOrDefault);
            }
            return PartialView("HotelModule", null);
        }

        /// <summary>
        /// 获取当前酒店
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCurrentInfo()
        {
            return Json( new { CurrentInfo.HotelId } );
        }

        #endregion 分店班次选择

        #region 主界面

        public ActionResult Main(string hid, string hname, string shiftId, string shiftName, string authCode)
        {
            var sessionInvalidResult = CheckSession();
            if (sessionInvalidResult != null)
            {
                return sessionInvalidResult;
            }
            if (AddTryLog(false))//体验者未输入手机号
            {
                return Redirect(Url.Action("TryInfo", "Account"));
            }
            //ViewBag.notify = IsHasAuth("10001", 4096);//接收提醒权限(云POS暂无提醒功能)
            //系统参数中的酒店简称
            var pmsParaService = GetService<IPmsParaService>();
            var SimpleName = pmsParaService.GetValue(CurrentInfo.HotelId, "PosGetHotelSimpleName");
            if (!string.IsNullOrWhiteSpace(SimpleName))
            {
                CurrentInfo.HotelName = SimpleName;
                CurrentInfo.SaveValues();
            }
            //已经修复了session取值问题，所以不再从参数中获取，直接从session中取值
            hid = CurrentInfo.HotelId;
            hname = CurrentInfo.HotelName;
            shiftId = CurrentInfo.ShiftId;
            shiftName = CurrentInfo.ShiftName;

            ViewBag.Title = hname;
            ViewBag.hotelId = hid;
            ViewBag.hotelName = hname;
            ViewBag.shiftName = shiftName;
            ViewBag.username = CurrentInfo.UserName;
            ViewBag.IsGroup = CurrentInfo.IsGroup;
            ViewBag.HotelBusinessDate = GetService<IHotelStatusService>().GetBusinessDate(hid).ToString("yyyy-MM-dd");
            ViewBag.AdSet = GetService<IMasterService>().GetAdSet("2").ToList();
            ViewBag.isServiceOperator = CurrentInfo.IsServiceOperatorLogin();
            ViewBag.product = GetProduct();

            ViewBag.posName = CurrentInfo.PosName;//收银点
            if (!string.IsNullOrEmpty(CurrentInfo.PosId))
            {
                var posService = GetService<IPosPosService>();
                var pos = posService.GetCleaningMachine(CurrentInfo.HotelId, CurrentInfo.PosId);
                ViewBag.business = pos == null ? "" : (string.IsNullOrEmpty(pos.Business) ? "" : pos.Business);
                //班次
                var server = GetService<IPosPosService>();
                var model = server.GetShiftChange(CurrentInfo.HotelId, CurrentInfo.PosId);
                ViewBag.shiftName = model == null ? "" : (string.IsNullOrEmpty(model.ShiftName) ? "" : model.ShiftName);

                //市别
                var refeService = GetService<IPosRefeService>();
                var refeList = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
                if (refeList != null && refeList.Count > 0)
                {
                    //营业点(默认第一条)
                    string Refeid = refeList[0].Id;
                    var shuffleService = GetService<IPosShuffleService>();
                    var refe = refeService.GetEntity(CurrentInfo.HotelId, Refeid);
                    var shuffle = shuffleService.GetEntity(CurrentInfo.HotelId, refe.ShuffleId);
                    ViewBag.shuffleName = shuffle == null ? "" : (string.IsNullOrEmpty(shuffle.Cname) ? "" : shuffle.Cname);
                }
                else
                {
                    ViewBag.shuffleName = "";
                }
            }
            else
            {
                ViewBag.shuffleName = "";
                ViewBag.shiftName = "";
                ViewBag.business = "";
            }

            //是否显示客户Logo
            var paraService = GetService<IPmsParaService>();
            var hotelInfoService = GetService<IHotelInfoService>();
            var hotelLogoAndNameInfo = paraService.GetHotelLogoAndNameInfo(hid, hotelInfoService);
            ViewBag.hotelLogoAndNameInfo = hotelLogoAndNameInfo;
            if (!string.IsNullOrEmpty(hotelLogoAndNameInfo.GSSysTitle))
            {
                ViewBag.Title = hotelLogoAndNameInfo.GSSysTitle;
            }

            ViewBag.AuthCode = authCode;
            return View();
        }

        #endregion 主界面

        #region 没有权限界面

        public ActionResult Deny()
        {
            return View();
        }

        #endregion 没有权限界面

        #region 显示帮助文档

        [HttpPost]
        public ActionResult GetHelpFiles(string id, string name)
        {
            var service = GetService<IHelpFilesService>();
            var result = service.GetHelpFiles(id);
            if (CurrentInfo.HotelId != "000000")
                result = result.Where(w => w.CheckStatus).ToList();
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #endregion 显示帮助文档

        #region 加载公告

        public JsonResult GetNoticeData()
        {
            var service = GetService<INoticeService>();
            var data = service.GetNoticeFirst(DateTime.Now, CurrentInfo.VersionId);
            if (data != null)
                return Json(new JsonResultData() { Success = true, Data = data }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResultData() { Success = false, Data = "没有公告" }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Notice(Guid id)
        {
            var service = GetService<INoticeService>();
            var data = service.Get(id);
            ViewBag.title = data.Title;
            ViewBag.content = data.Content;
            return View();
        }

        #endregion 加载公告

        #region 加载提醒

        public JsonResult GetNotify()
        {
            /*var service = GetService<IWakeCallService>();
            var time = service.NotifyTimeBef(CurrentInfo.HotelId);
            var data = service.GetNotify(CurrentInfo.HotelId, "", "", "", DateTime.Now.AddMinutes(time).ToString("yyyy-MM-dd HH:mm:ss"), 1);
            if (data != null && data.Count > 0)
            {
                var entity = service.GetNotify(CurrentInfo.HotelId, "", CurrentInfo.UserId, DateTime.Now.AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.AddMinutes(time).ToString("yyyy-MM-dd HH:mm:ss"), 0);
                if (entity != null && entity.Count > 0)
                {
                    UpQueryNotifyResult noti = entity.First();
                    noti.Count = data.Count;
                    return Json(JsonResultData.Successed(noti));
                }
                else
                {
                    return Json(JsonResultData.Failure(data.Count));
                }
            }*/
            return Json(JsonResultData.Failure("0"));
        }

        #endregion 加载提醒


        #region 加载到期提前提醒
        /// <summary>
        /// 如果当前日期+[到期提前提醒天数]>=酒店到期日期，并且当前小时=14，则显示[到期提前提醒内容]  
        /// </summary>   
        /// <returns></returns> 
        public JsonResult GetExpirRemind()
        {
            Dictionary<string, string> ExpirePara = GetService<ISysParaService>().GetExpiredPara();
            string ExpiredRemindAdvanceContent = ExpirePara["expiredremindadvancecontent"];
            int ExpirRemindDayCount = int.Parse(ExpirePara["expirreminddaycount"]);
            string ExpiredRemindContent = ExpirePara["expiredremindcontent"];
            DateTime dt = DateTime.Now.AddDays(ExpirRemindDayCount);
            UpQueryHotelInfoByIdResult hotel = GetService<IHotelInfoService>().GetHotelInfo(CurrentInfo.HotelId);
            if (DateTime.Now >= hotel.ExpiryDate && DateTime.Now.Hour == 18)//到期提醒内容
            {
                return Json(JsonResultData.Successed(string.Format(ExpiredRemindContent, hotel.ExpiryDate.Value.ToString("yyyy年MM月dd日"), hotel.CustomerStatus)));
            }
            if (dt >= hotel.ExpiryDate && DateTime.Now.Hour == 18)//到期提前提醒内容
            {
                return Json(JsonResultData.Successed(string.Format(ExpiredRemindAdvanceContent, hotel.ExpiryDate.Value.ToString("yyyy年MM月dd日"), hotel.CustomerStatus)));
            }
            else
            {
                return Json(JsonResultData.Failure("0"));
            }
        }
        #endregion

        #region 判断收银点是否清机

        public JsonResult CheckBussisByHotelAndPos(string hid, string posId)
        {
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(posId);
            var currentMachineTime = System.DateTime.Now;   //结转时间

            if (pos != null)
            {
                //根据当前营业点的营业日以及结转设置得到最早清机时间
                if (pos.IsBusinessend == (byte)PosBusinessEnd.当日结转)
                {
                    var date = Convert.ToDateTime(pos.Business.ToString());
                    currentMachineTime = Convert.ToDateTime(date.ToShortDateString() + " " + pos.BusinessTime);
                }
                else
                {
                    var date = Convert.ToDateTime(pos.Business.ToString());
                    currentMachineTime = Convert.ToDateTime(date.AddDays(1).ToShortDateString() + " " + pos.BusinessTime);
                }
                if (DateTime.Now > currentMachineTime)
                {
                    return Json(JsonResultData.Failure("收银点未清机，请及时清机"));
                }
                else
                {
                    return Json(JsonResultData.Successed());
                }
            }
            else
            {
                return Json(JsonResultData.Successed());
            }
        }

        #endregion 判断收银点是否清机

        /// <summary>
        /// 根据收银点获取营业日
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetBusinessByPos(string id)
        {
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(id);

            if (pos != null && pos.Business != null)
            {
                string business = pos.Business.Value.Date.ToString("yyyy-MM-dd");
                return Json(JsonResultData.Successed(business));
            }

            return Json(JsonResultData.Failure(""));
        }
    }
}