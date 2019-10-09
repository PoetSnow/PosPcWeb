using Gemstar.BSPMS.Common.Enumerator;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EF.SystemManage;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Services.WeixinManage;
using Gemstar.BSPMS.Hotel.Web.Models;
using Gemstar.BSPMS.Hotel.Web.Models.Account;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;

namespace Gemstar.BSPMS.Hotel.Web.Controllers
{
    /// <summary>
    /// 酒店统一登录入口，登录成功后根据酒店所在的web服务器组域名转到对应的web服务器上去
    /// </summary>
    [BusinessType("酒店统一登录")]
    [DogCheck]
    public class AccountController : Controller
    {
        public const string TryCookieName = "TryMobile";

        #region 登录

        public ActionResult Index(string env, string hid)
        {
            var model = new LoginViewModel();
            model.HotelId = Request["hid"];
            model.Username = Request["userName"];
            //取出运营系统参数中的试用参数信息
            var sysParaService = GetService<ISysParaService>();
            var tryHotelPara = sysParaService.Get("TryHotelIdForPos");
            var tryUserName = sysParaService.Get("TryUsernameForPos");
            var tryUserPass = sysParaService.Get("TryUserPassForPos");
            if (tryHotelPara != null)
            {
                model.TryHotelId = tryHotelPara.Value;
            }
            if (tryUserName != null)
            {
                model.TryUserName = tryUserName.Value;
            }
            if (tryUserPass != null)
            {
                model.TryUserPass = tryUserPass.Value;
            }
            ViewBag.domain = Request.Headers["Host"];
            ViewBag.product = GetProduct();
            //是否显示客户Logo
            if (!string.IsNullOrWhiteSpace(model.HotelId))
            {
                var currentInfo = GetService<ICurrentInfo>();
                currentInfo.HotelId = hid;
                currentInfo.SaveValues();
                var paraService = GetService<IPmsParaService>();
                var hotelInfoService = GetService<IHotelInfoService>();
                var hotelLogoAndNameInfo = paraService.GetHotelLogoAndNameInfo(model.HotelId, hotelInfoService);
                ViewBag.hotelLogoAndNameInfo = hotelLogoAndNameInfo;
                if (!string.IsNullOrEmpty(hotelLogoAndNameInfo.GSSysTitle))
                {
                    ViewBag.Title = hotelLogoAndNameInfo.GSSysTitle;
                }
            }
            return View(model);
        }

        /// <summary>
        /// 默认插入营业点等信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        private void CreatePosDate(string hid, string moduleCode)
        {
            //视图服务
            var codeListService = GetService<ICodeListService>();

            var posPosServiece = GetService<IPosPosService>();//收银点服务
            var posList = posPosServiece.GetPosByHid(hid);
            if (posList.Count <= 0)
            { //酒店类型
                var hotelService = GetService<IHotelInfoService>();
                var hotel = hotelService.ListValidHotels().Where(m => m.Hid == hid).FirstOrDefault();

                var CateringServicesType = hotel.CateringServicesTypeOrDefault;//
                var hotelTypeList = CateringServicesType.Split(',');

                var posMode = "A";  //云Pos模式
                var opneInfo = ""; //营业点开台属性

                var typeCode = "";//公用代码Code

                var posRefService = GetService<IPosRefeService>();  //营业点服务
                var posShiftService = GetService<IPosShiftService>();   //班次 服务
                var posShuffeService = GetService<IPosShuffleService>();    //市别服务
                foreach (var item in hotelTypeList)
                {
                    if (item == "B" || item == "C")   //快餐，零售模式(B:快餐，C：零售)
                    {
                        posMode = item;
                        opneInfo = item == "B" ? "J" : "I";
                        typeCode = item == "B" ? "74" : "75";
                    }
                    else
                    {
                        posMode = item;
                        opneInfo = item; //默认填人数
                        typeCode = "73";
                    }

                    var count = posPosServiece.GetPosByHid(hid).Where(m => m.PosMode == item).ToList().Count;
                    if (count <= 0)
                    {
                        var newPosCode = GetNewPosCode(hid, typeCode);
                        var newRefeCode = GetNewRefeCode(hid, typeCode);
                        var newShuffCode = GetNewShuffleCode(hid, typeCode);
                        var newShiftCode = GetNewShuffleCode(hid, typeCode);

                        var codeListPosPos = codeListService.List(typeCode).ToList();

                        #region 收银点赋值

                        var posPosModel = new PosPos
                        {
                            Id = hid + newPosCode,
                            Hid = hid,
                            Code = newPosCode,
                            Name = hotel.Name + newPosCode,
                            Seqid = 1,
                            ShiftId = hid + newPosCode,
                            Module = moduleCode,
                            Business = DateTime.Now,
                            PosMode = posMode,
                            IsBusinessend = PosBusinessEnd.当日结转,
                            CodeIn = moduleCode + newPosCode,
                            BusinessTime = "22:00"
                        };

                        #endregion 收银点赋值

                        #region 营业点赋值

                        var posRef = new PosRefe
                        {
                            Id = hid + newRefeCode,
                            Hid = hid,
                            Code = newRefeCode,
                            Cname = hotel.Name + codeListPosPos[1].name,
                            Module = moduleCode,
                            PosId = hid + newPosCode,
                            ShuffleId = hid + codeListPosPos[3].code,   //班次
                            OpenInfo = opneInfo,
                            RegType = 0,
                            IPrintBillss = 0,
                            IsOrderSameItem = 0,
                            IsTabProduce = 0,
                            ITagDecend = 0,
                            ITagPrintBill = 0,
                            IsTagLimitSrv = 0,
                            IsTagLimitDisc = 0,
                            IsTagSrvDisc = 0,
                            IsBuyZeroBill = 0,
                            IsTagPromptFoot = 0,
                            IsZeroPrintBill = 0,
                            IServicesTime = 0,
                            IsBusinessend = 0,
                            IsOnsaleTime = 0,
                            IsShowTableproperty = true  //字段无用，赋值防止系统报错
                        };

                        #endregion 营业点赋值

                        #region 市别赋值

                        var posShuffe = new PosShuffle
                        {
                            Id = hid + newShuffCode,
                            Hid = hid,
                            Code = newShuffCode,
                            Cname = codeListPosPos[2].name,
                            Refeid = hid + newRefeCode,
                            Stime = "00:00",
                            Etime = "23:59",
                            IsHideTab = 1,
                            Module = moduleCode
                        };

                        #endregion 市别赋值

                        #region 班次赋值

                        var posShift = new PosShift
                        {
                            Id = hid + newShiftCode,
                            Hid = hid,
                            Code = newShiftCode,
                            Name = codeListPosPos[3].name,
                            PosId = hid + newPosCode,
                            Module = moduleCode,
                            Stime = "00:00",
                            Etime = "23:59"
                        };

                        #endregion 班次赋值

                        posPosServiece.Add(posPosModel);
                        posPosServiece.AddDataChangeLog(OpLogType.Pos收银点增加);
                        posPosServiece.Commit();

                        posShiftService.Add(posShift);
                        posShiftService.AddDataChangeLog(OpLogType.Pos班次增加);
                        posShiftService.Commit();

                        posShuffeService.Add(posShuffe);
                        posShuffeService.AddDataChangeLog(OpLogType.Pos市别增加);
                        posShuffeService.Commit();

                        posRefService.Add(posRef);
                        posRefService.AddDataChangeLog(OpLogType.Pos营业点增加);
                        posRefService.Commit();
                    }
                }
            }
        }

        #region 获取市别，收银点，营业点，班次code

        /// <summary>
        /// 根据酒店ID 获取最新的收银点Code
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        private string GetNewPosCode(string hid, string typeCode)
        {
            var posPosServiece = GetService<IPosPosService>();
            var codeListService = GetService<ICodeListService>();
            var List = posPosServiece.GetPosByHid(hid).ToList();
            // posList.Add(new PosPos { Code = "dsfsdf" });    //测试数据
            var listcode = new List<PosPos>();
            if (List != null && List.Count > 0)
            {
                foreach (var pos in List)
                {
                    var code = pos.Code.TrimEnd();
                    if (Regex.IsMatch(code, "[0-9]+")) //验证是否是数字组成
                    {
                        listcode.Add(pos);
                    }
                }
            }

            var codeListPosPos = codeListService.List(typeCode).ToList();
            var newCode = codeListPosPos[0].code;
            //判断code 是否重复
            var isExists = posPosServiece.IsExists(hid, hid + newCode);

            if (isExists)
            {
                var code = listcode.Max(m => m.Code).TrimEnd();
                newCode = code.Substring(0, code.Length - 1) + (int.Parse(code.Substring(code.Length - 1, 1)) + 1);
            }

            return newCode;
        }

        /// <summary>
        /// 根据酒店ID获取营业点最新的code
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        private string GetNewRefeCode(string hid, string typeCode)
        {
            var Serviece = GetService<IPosRefeService>();
            var codeListService = GetService<ICodeListService>();
            var List = Serviece.GetRefe(hid).ToList();
            var listcode = new List<PosRefe>();
            if (List != null && List.Count > 0)
            {
                foreach (var refe in List)
                {
                    var code = refe.Code.TrimEnd();
                    if (Regex.IsMatch(code, "[0-9]+")) //验证是否是数字组成
                    {
                        listcode.Add(refe);
                    }
                }
            }
            var codeListPosPos = codeListService.List(typeCode).ToList();
            var newCode = codeListPosPos[1].code;
            //判断code 是否重复
            var isExists = Serviece.IsExists(hid, newCode);

            if (isExists)
            {
                var code = listcode.Max(m => m.Code).TrimEnd();
                newCode = code.Substring(0, code.Length - 1) + (int.Parse(code.Substring(code.Length - 1, 1)) + 1);
            }

            return newCode;
        }

        private string GetNewShuffleCode(string hid, string typeCode)
        {
            var Serviece = GetService<IPosShuffleService>();
            var codeListService = GetService<ICodeListService>();
            var List = Serviece.GetPosShuffle(hid).ToList();
            var listcode = new List<PosShuffle>();
            if (List != null && List.Count > 0)
            {
                foreach (var refe in List)
                {
                    var code = refe.Code.TrimEnd();
                    if (Regex.IsMatch(code, "[0-9]+")) //验证是否是数字组成
                    {
                        listcode.Add(refe);
                    }
                }
            }
            var codeListPosPos = codeListService.List(typeCode).ToList();
            var newCode = codeListPosPos[2].code;
            //判断code 是否重复
            var isExists = Serviece.IsExists(hid, newCode);

            if (isExists)
            {
                var code = listcode.Max(m => m.Code).TrimEnd();
                newCode = code.Substring(0, code.Length - 1) + (int.Parse(code.Substring(code.Length - 1, 1)) + 1);
            }

            return newCode;
        }

        private string GetNewShiftCode(string hid, string typeCode)
        {
            var Serviece = GetService<IPosShiftService>();
            var codeListService = GetService<ICodeListService>();
            var List = Serviece.GetShiftList(hid).ToList();
            var listcode = new List<PosShift>();
            if (List != null && List.Count > 0)
            {
                foreach (var refe in List)
                {
                    var code = refe.Code.TrimEnd();
                    if (Regex.IsMatch(code, "[0-9]+")) //验证是否是数字组成
                    {
                        listcode.Add(refe);
                    }
                }
            }
            var codeListPosPos = codeListService.List(typeCode).ToList();
            var newCode = codeListPosPos[3].code;
            //判断code 是否重复
            var isExists = Serviece.IsExists(hid, newCode);

            if (isExists)
            {
                var code = listcode.Max(m => m.Code).TrimEnd();
                newCode = code.Substring(0, code.Length - 1) + (int.Parse(code.Substring(code.Length - 1, 1)) + 1);
            }

            return newCode;
        }

        #endregion 获取市别，收银点，营业点，班次code

        /// <summary>
        /// 登录插入系统定义的高级功能（高级功能列表定义在v_codeListPub  typeCode=81）
        /// </summary>
        /// <param name="hid">酒店ID</param>
        private void CreatePosAdvanceFunc(string hid, string moduleCode)
        {
            //高级功能服务
            var PosAdvanceFuncService = GetService<IPosAdvanceFuncService>();

            //视图服务
            var codeListService = GetService<ICodeListService>();
            //获取有效状态的数据
            var codeList = codeListService.List("81").Where(m => m.status == 1).ToList();

            foreach (var code in codeList)
            {
                //循环判断高级功能是否在酒店中存在
                var PosAdvanceFunc = PosAdvanceFuncService.GetPosAdvanceFuncByFuncCode(hid, code.code);
                if (PosAdvanceFunc == null)
                {
                    //不存在的功能就添加
                    var PosAdvanceFuncModel = new PosAdvanceFunc()
                    {
                        Id = hid + code.code,
                        Hid = hid,
                        FuncCode = code.code,
                        Name1 = code.name,
                        IsUsed = true,
                        CreateDate = DateTime.Now,
                        Module = moduleCode
                    };
                    PosAdvanceFuncService.Add(PosAdvanceFuncModel);
                    PosAdvanceFuncService.AddDataChangeLog(OpLogType.Pos高级功能增加);
                    PosAdvanceFuncService.Commit();
                }
            }
        }

        [HttpPost, JsonException]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel loginModel, string hascheckcode)
        {
            try
            {
                if (hascheckcode == "1")
                {
                    var checkCodeInSession = Session[CheckCodeBuilder.CheckCodeKeyInSession];
                    if (checkCodeInSession == null || !loginModel.CheckCode.Equals(checkCodeInSession.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        return Json(JsonResultData.Failure("无效的验证码"));
                    }
                }
                var hotelinfo = GetService<IHotelInfoService>().GetHotelInfo(loginModel.HotelId);
                if (hotelinfo != null && !string.IsNullOrWhiteSpace(hotelinfo.Grpid) && hotelinfo.Grpid != hotelinfo.Hid)
                {
                    return Json(JsonResultData.Failure("酒店id不正确"));
                }
                //检查指定酒店是否已经开通当前域名对应的产品
                var productService = GetService<IProductService>();
                var hotelProducts = productService.GetHotelProducts(loginModel.HotelId);
                if (hotelProducts == null || hotelProducts.Count == 0)
                {
                    return Json(JsonResultData.Failure("指定酒店没有开通此产品，请与软件供应商联系"));
                }
                var product = GetProduct();
                var exists = hotelProducts.Any(w => w == product.Code);
                if (!exists)
                {
                    return Json(JsonResultData.Failure("指定酒店没有开通此产品，请与软件供应商联系"));
                }

                if (ModelState.IsValid || hascheckcode == "0")
                {
                    var _accountService = GetService<IAccountService>();
                    var loginResult = _accountService.Login(loginModel.HotelId, loginModel.Username, loginModel.Password);
                    if (!loginResult.LoginSuccess)
                    {
                        ModelState.AddModelError("Username", loginResult.ErrorMessage);
                        return Json(JsonResultData.Failure(ModelState.Values));
                    }
                    var _currentInfo = GetService<ICurrentInfo>();
                    _currentInfo.Clear();
                    FormsAuthentication.SetAuthCookie(loginResult.UserCode, false);
                    var sing = GetService<ISysParaService>().GetHotelFunctionses(loginResult.Hid);
                    var data = sing.Where(s => s.FuncCode == "FuncSignature").FirstOrDefault();
                    _currentInfo.GroupId = loginResult.Grpid;
                    _currentInfo.HotelId = loginResult.Hid;
                    _currentInfo.HotelName = loginResult.HotelName;
                    _currentInfo.DbServer = loginResult.DbServer;
                    _currentInfo.DbName = loginResult.DbName;
                    _currentInfo.DbUser = loginResult.DbUser;
                    _currentInfo.DbPwd = loginResult.DbPwd;
                    _currentInfo.UserId = loginResult.UserId;
                    _currentInfo.UserCode = loginResult.UserCode;
                    _currentInfo.UserName = loginResult.UserName;
                    _currentInfo.IsRegUser = loginResult.IsRegUser;
                    _currentInfo.Signature = data == null ? "0" : data.Isvalid == null ? "0" : data.Isvalid == true ? "1" : "0";
                    _currentInfo.LoginTimeTicks = DateTime.UtcNow.Ticks.ToString();
                    _currentInfo.VersionId = loginResult.VersionId;
                    var serverUrl = loginResult.WebServerUrl;
                    var accessDomain = GetAccessDomain();
                    //新的服务器地址不是完整的域名，需要添加上现在请求的域名来构成新的跳转域名
                    if (accessDomain.Contains(product.Domain))
                    {
                        serverUrl = string.Format("{0}.{1}", serverUrl, product.Domain);
                    }
                    else if (accessDomain.Contains(product.Domain2))
                    {
                        serverUrl = string.Format("{0}.{1}", serverUrl, product.Domain2);
                    }
                    else
                    {
                        serverUrl = string.Format("{0}.{1}", serverUrl, accessDomain);
                    }

                    _currentInfo.WebServerUrl = serverUrl;
                    _currentInfo.ModuleCode = GetModuleCodeByDomain(product.Domain);
                    _currentInfo.ProductType = ProductTypeHelper.GetProductType(product);
                    _currentInfo.SaveValues();

                    var pmsParaService = GetService<IPmsParaService>();

                    //系统参数中的酒店简称
                    var SimpleName = pmsParaService.GetValue(_currentInfo.HotelId, "PosGetHotelSimpleName");
                    if (!string.IsNullOrWhiteSpace(SimpleName))
                    {
                        _currentInfo.HotelName = SimpleName;
                    }
                    _currentInfo.SaveValues();

                    //判断是否是默认密码
                    var pmsUserService = GetService<IPmsUserService>();
                    if (pmsUserService.IsUserPassowrdDefault(loginResult.UserId))
                    {
                        //转到修改密码的界面中
                        return Json(JsonResultData.Successed(Url.Action("ChangePassword")));
                    }
                    //判断是否是试用用户
                    //取出运营系统参数中的试用参数信息
                    var sysParaService = GetService<ISysParaService>();
                    var tryHotelPara = sysParaService.Get("TryHotelIdForPos");
                    var tryUserName = sysParaService.Get("TryUsernameForPos");
                    var tryUserPass = sysParaService.Get("TryUserPassForPos");
                    var tryHotelId = tryHotelPara == null ? "" : tryHotelPara.Value;
                    var tryUser = tryUserName == null ? "" : tryUserName.Value;
                    var tryPass = tryUserPass == null ? "" : tryUserPass.Value;
                    if (loginModel.HotelId == tryHotelId && loginModel.Username == tryUser && loginModel.Password == tryPass)
                    {
                        //判断是否有提交体验cookie，没有则转到试用信息收集界面
                        var tryCookie = Request.Cookies[TryCookieName];
                        if (tryCookie == null)
                        {
                            return Json(JsonResultData.Successed(Url.Action("TryInfo")));
                        }
                    }

                    //默认添加一条数据.
                    var hotelService = GetService<IHotelInfoService>();
                    var hotel = hotelService.ListValidHotels().Where(m => m.Hid == loginModel.HotelId).FirstOrDefault();
                    if (hotel != null)
                    {
                        if (string.IsNullOrWhiteSpace(hotel.Grpid))
                        {
                            //单店
                            CreatePosDate(loginModel.HotelId, _currentInfo.ModuleCode);
                            //添加默认的高级功能
                            CreatePosAdvanceFunc(loginModel.HotelId, _currentInfo.ModuleCode);
                        }
                        else
                        {
                            //集团
                            var userService = GetService<IPmsUserService>();
                            //通过用户code 获取用户ID
                            var user = userService.GetUserIDByCode(loginModel.HotelId, loginModel.Username);
                            //获取分店列表
                            var ResortList = userService.GetResortListForOperator(loginModel.HotelId, user.Id.ToString());
                            foreach (var hotelModel in ResortList)
                            {
                                CreatePosDate(hotelModel.Hid, _currentInfo.ModuleCode);

                                CreatePosAdvanceFunc(hotelModel.Hid, _currentInfo.ModuleCode);
                            }
                        }
                        if (Request["hid"] != null && Request["mode"] != null)
                        {
                            var abc = serverUrl + Request["ReturnUrl"].ToString();
                            if (Request["hid"].ToString() == _currentInfo.HotelId && (Request["mode"].ToString() == "B" || Request["mode"].ToString() == "C"))
                            {
                                return Json(JsonResultData.Successed(serverUrl + Request["ReturnUrl"].ToString()));
                            }
                            else
                            {
                                return Json(JsonResultData.Successed(serverUrl + Request["ReturnUrl"].ToString()));
                            }
                        }
                        return Json(JsonResultData.Successed(serverUrl));
                    }
                    else
                    {
                        return Json(JsonResultData.Failure("使用期限已经过期，系统将暂停使用，请及时续费。"));
                    }
                }
                else
                {
                    return Json(JsonResultData.Failure(ModelState.Values));
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.ToString()));
            }           
        }

        #endregion 登录

        #region 验证码

        public ActionResult LoadCheckCode()
        {
            var checkCodeBuilder = new CheckCodeBuilder();
            var checkCode = checkCodeBuilder.GenerateRandomText();
            Session[CheckCodeBuilder.CheckCodeKeyInSession] = checkCode;
            using (var b = checkCodeBuilder.RenderImage(checkCode))
            {
                b.Save(Response.OutputStream, ImageFormat.Gif);
            }
            Response.Cache.SetNoStore();
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

            Response.ContentType = "image/gif";
            Response.StatusCode = 200;
            Response.StatusDescription = "OK";
            return new EmptyResult();
        }

        #endregion 验证码

        #region 注册

        public ActionResult Register()
        {
            var _settingInfo = GetService<ISettingProvider>();
            ViewBag.AgreementUrl = _settingInfo.SettingInfo.AgreementUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            //开始验证
            var check_result = RegisterCheck(model);
            if (!check_result.Success) return Json(check_result, JsonRequestBehavior.AllowGet);

            Guid serverId = Guid.Empty;
            Guid dbId = Guid.Empty;
            //获取注册时需要使用的服务器数据库信息
            var _sysParaService = GetService<ISysParaService>();
            var para = _sysParaService.GetDefaultRegisterConnPara();
            //验证配置参数有效性
            if (!para.ContainsKey("pubserverid") || !para.ContainsKey("pubdbid") || !Guid.TryParse(para["pubserverid"], out serverId) || !Guid.TryParse(para["pubdbid"], out dbId))
                return Json(JsonResultData.Failure("系统默认的配置参数异常"), JsonRequestBehavior.AllowGet);

            try
            {
                //组装酒店参数
                var hotel = new CenterHotel()
                {
                    Hid = MvcApplication.CreateHid(),
                    Grpid = string.IsNullOrEmpty(model.grpid) ? "" : model.grpid,
                    Name = model.name,
                    Provinces = model.provinces,
                    City = model.city,
                    Star = string.IsNullOrEmpty(model.star) ? "无星" : model.star,
                    Email = model.email,
                    Mobile = model.mobile,
                    Serverid = serverId,
                    Dbid = dbId,
                    ExpiryDate = DateTime.Now.AddYears(1),
                    Status = (byte)EntityStatus.启用
                };
                //开始注册逻辑
                var _beforeLoginService = GetService<IBeforeLoginService>();
                //先将酒店信息插入到中心数据库
                var insert_result = _beforeLoginService.InsertHotelToCenterDB(hotel);
                //然后将模板酒店信息复制到业务库
                var copy_result = _beforeLoginService.CopyModelHotel(hotel, model.loginCode, model.loginName, model.pwd, 1);
                //如果未复制成功，则将中心数据库的酒店信息删除，保持数据一致性
                if (!copy_result.Success)
                {
                    //若复制模板酒店数据失败，则删除中央数据库注册的酒店信息
                    _beforeLoginService.DeleteHotelFromCenterDB(hotel.Hid);
                    return Json(copy_result);
                }
                //系统设置初始化

                //跳转到注册成功的界面
                return Json(JsonResultData.Successed(Url.Action("Register_Success", new { hotelId = hotel.Hid, loginCode = model.loginCode })));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        public ActionResult Register_Success(string hotelId, string loginCode)
        {
            ViewBag.Hid = hotelId;
            ViewBag.LoginCode = loginCode;
            return View();
        }

        private JsonResultData DataValidCheck(RegisterViewModel model)
        {
            string msg = "";
            if (!PasswordHelper.IsPasswordValid(model.pwd, out msg)) return JsonResultData.Failure(msg);
            if (model.pwd != model.confirmPwd) return JsonResultData.Failure("密码与确认密码不一致");
            if (!RegexHelper.IsRightMobile(model.mobile)) return JsonResultData.Failure("手机号码格式不正确");
            if (!RegexHelper.IsRightEmail(model.email)) return JsonResultData.Failure("邮箱格式不正确");
            return JsonResultData.Successed();
        }

        private JsonResultData RegisterCheck(RegisterViewModel model)
        {
            //数据有效性验证
            var dataValidCheckResult = DataValidCheck(model);
            if (!dataValidCheckResult.Success) return dataValidCheckResult;

            //数据重复性验证
            var _hotelInfoService = GetService<IHotelInfoService>();
            var repeatCheckResult = _hotelInfoService.RepeatCheck(model.name, model.mobile);
            if (!repeatCheckResult.Success) return repeatCheckResult;
            //验证码验证
            return SMSSendHelper.MatchCheckCode(model.mobile, model.checkCode, CheckFunc.Register);
        }

        #endregion 注册

        #region 退出登录

        public ActionResult Logout()
        {
            var _currentInfo = GetService<ICurrentInfo>();
            var logservice = GetService<IoperationLog>();
            var hid = _currentInfo.HotelId;
            var userName = _currentInfo.UserName;
            var shiftName = _currentInfo.ShiftName;
            var hotelStatusService = GetService<IHotelStatusService>();
            var businessDay = hotelStatusService.GetBusinessDate(hid).ToString("yyyy-MM-dd");

            //记录退出的日志
            var text = string.Format("班次：{1}，登录营业日：{0}", businessDay, shiftName);
            logservice.AddOperationLog(hid, OpLogType.退出班次, text, userName, Common.Extensions.UrlHelperExtension.GetRemoteClientIPAddress());

            _currentInfo.Clear();
            _currentInfo.SaveValues();
            Session.Abandon();
            var accessDomain = Request.Headers["Host"];
            var domain = SharedSessionModule.GetLastThreeLevelDomain(accessDomain);
            var urlBuilder = new UriBuilder();
            urlBuilder.Host = domain;
            urlBuilder.Path = FormsAuthentication.LoginUrl;
            var url = urlBuilder.ToString();
     
            return Json(JsonResultData.Successed(url));
        }

        #endregion 退出登录

        #region 下拉绑定

        public JsonResult GetProvinceSelectList()
        {
            var _masterService = GetService<IMasterService>();
            var province = _masterService.GetProvince().Select(e => new SelectListItem { Value = e.Code, Text = e.Name });
            return Json(province, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCitySelectList(string key, object r)
        {
            var _masterService = GetService<IMasterService>();
            var city = _masterService.GetCity(key).Select(e => new SelectListItem { Value = e.Code, Text = e.Name });
            return Json(city, JsonRequestBehavior.AllowGet);
        }

        #endregion 下拉绑定

        #region 发送验证码

        /// <summary>
        /// 定向发送（找回密码）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public JsonResult SendCheckCode(string hid, string code, string value, string type, string func)
        {
            string retstr = "";
            if (CheckMobileAndEmail(hid, code, type, value, ref retstr))
            {
                if (type == "Mobile")
                {
                    return SendCheckCodeByMobile(value, func, hid);//手机验证
                }
                else
                {
                    return SendCheckCodeByEmail(value, func);//邮件验证
                }
            }
            else
            {
                return Json(JsonResultData.Failure(retstr + "请重新输入！"));
            }
        }

        /// <summary>
        /// 定向发送（注册用户）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public JsonResult SendCheckCodeReg(string value, string type, string func)
        {
            if (type == "Mobile")
            {
                //数据重复性验证
                var _hotelInfoService = GetService<IHotelInfoService>();
                var repeatCheckResult = _hotelInfoService.RepeatCheck("", value);
                if (!repeatCheckResult.Success)
                    return Json(repeatCheckResult, JsonRequestBehavior.AllowGet);
                else
                    return SendCheckCodeByMobile(value, func, "");//手机验证
            }
            else
            {
                return SendCheckCodeByEmail(value, func);//邮件验证
            }
        }

        /// <summary>
        /// 验证的手机号或邮箱要与系统数据库中该登录名资料一致时方可获取验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public bool CheckMobileAndEmail(string hid, string code, string type, string value, ref string retstr)
        {
            var pmsUserService = GetService<IAccountService>();
            //  var pmsUserService = DependencyResolver.Current.GetService<IPmsUserService>();
            retstr = pmsUserService.CheckUserinfo(hid, code, type, value);
            if (retstr == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 手机验证
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public JsonResult SendCheckCodeByMobile(string mobile, string func, string hid)
        {
            try
            {
                var username = "";
                var password = "";
                if (!string.IsNullOrWhiteSpace(hid))
                {
                    var hotelDb = GetHotelDb(hid);
                    var pmsParaService = new PmsParaService(hotelDb);
                    pmsParaService.IsSmsEnable(hid, out username, out password);
                }
                var sendPara = new SMSSendParaCheckCode
                {
                    Mobile = mobile,
                    Func = (CheckFunc)Enum.Parse(typeof(CheckFunc), func, true),
                    UserName = username,
                    Password = password
                };
                return Json(SMSSendHelper.SendCheckCode(sendPara), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SendCheckCodeByEmail(string email, string func)
        {
            try
            {
                EmailModel emailModel = new EmailModel() { FromName = "捷信达系统管理员", ToAddress = email, Subject = "重置密码验证【捷信达】", Remark = "这是一封系统邮件，请不要回复" };
                emailModel.BodyPara = new Dictionary<string, string>();
                return Json(EmailSendHelper.SendCheckCode(emailModel, func, EmailTemplate.ResetPassword), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion 发送验证码

        #region 更改密码

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.product = GetProduct();
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = GetService<ICurrentInfo>();
                var result = JsonResultData.Failure("未知状态");
                if (currentUser.IsServiceOperatorLogin())
                {
                    var serviceAuthorizeService = GetService<IServiceAuthorizeService>();
                    var userCode = currentUser.UserCode;
                    var codeInfos = userCode.Split(new string[] { "授权给" }, StringSplitOptions.RemoveEmptyEntries);
                    result = serviceAuthorizeService.ChangePassword(codeInfos[1], model.OriginPassword, model.NewPassword);
                }
                else
                {
                    var pmsUserService = GetService<IPmsUserService>();
                    result = pmsUserService.ChangePassword(currentUser.UserId, model.OriginPassword, model.NewPassword);
                }
                if (result.Success)
                {
                    //修改成功，则 退出登录 然后 转到登录界面中
                    currentUser.Clear();
                    Session.Abandon();
                    return Json(JsonResultData.Successed(FormsAuthentication.LoginUrl));
                }
                return Json(result);
            }
            return Json(JsonResultData.Failure(ModelState.Values));
        }

        #endregion 更改密码

        #region 重置密码

        public ActionResult ResetPassword()
        {
            ViewBag.product = GetProduct();
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            //数据校验
            var check_result = ResetPasswordCheck(model);
            if (!check_result.Success) return Json(check_result, JsonRequestBehavior.AllowGet);

            //重置密码
            var _beforeLoginService = GetService<IBeforeLoginService>();
            var reset_result = _beforeLoginService.ResetUserPassword(model.Hid, model.Account, model.NewPass);
            if (!reset_result.Success) return Json(reset_result, JsonRequestBehavior.AllowGet);
            //重置成功后返回跳转路径
            return Json(JsonResultData.Successed(Url.Action("Index", new { hid = model.Hid, userName = model.Account })));
        }

        /// <summary>
        /// 重置密码前的数据校验
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private JsonResultData ResetPasswordCheck(ResetPasswordViewModel model)
        {
            //数据有效性验证
            var valid = DataValidCheck(model);
            if (!valid.Success) return valid;

            //数据存在性验证
            var _beforeLoginService = GetService<IBeforeLoginService>();
            var check = _beforeLoginService.CheckHotelUser(model.Hid, model.Account);
            if (!check.Success) return check;

            //验证码验证
            if (model.GetMethod == "Mobile")
            {
                return SMSSendHelper.MatchCheckCode(model.GetMethodValue, model.CheckCode, CheckFunc.ResetPassword);
            }
            else
            {
                return EmailSendHelper.MatchCheckCode(model.GetMethodValue, model.CheckCode, CheckFunc.ResetPassword);
            }
        }

        /// <summary>
        /// 数据有效性校验
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private JsonResultData DataValidCheck(ResetPasswordViewModel model)
        {
            string msg = "";
            if (!PasswordHelper.IsPasswordValid(model.NewPass, out msg)) return JsonResultData.Failure(msg);
            if (model.NewPass != model.ConfirmNewPass) return JsonResultData.Failure("密码与确认密码不一致");
            if (model.GetMethod.IsNullOrEmpty()) return JsonResultData.Failure("请选择验证方式");
            if (model.GetMethod == "Mobile")
            {
                if (!RegexHelper.IsRightMobile(model.GetMethodValue)) return JsonResultData.Failure("手机号码格式不正确");
            }
            else
            {
                if (!RegexHelper.IsRightEmail(model.GetMethodValue)) return JsonResultData.Failure("邮箱格式不正确");
            }
            return JsonResultData.Successed();
        }

        #endregion 重置密码

        #region 获取服务接口

        /// <summary>
        /// 获取指定服务接口的实例
        /// </summary>
        /// <typeparam name="T">服务接口类型</typeparam>
        /// <returns>指定服务接口的实例</returns>
        protected T GetService<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }

        protected DbHotelPmsContext GetHotelDb(string hid)
        {
            var hotelInfoService = GetService<IHotelInfoService>();
            var hotelInfo = hotelInfoService.GetHotelInfo(hid);
            var isConnectViaInternet = hotelInfoService.IsConnectViaInternte();
            var connStr = ConnStrHelper.GetConnStr(hotelInfo.DbServer, hotelInfo.DbName, hotelInfo.Logid, hotelInfo.LogPwd, "GemstarBSPMS",hotelInfo.DbServerInternet,isConnectViaInternet);
            var hotelDb = new DbHotelPmsContext(connStr, hid, "", Request);
            return hotelDb;
        }

        #endregion 获取服务接口

        #region 获取广告

        public JsonResult GetBanner()
        {
            try
            {
                var masterService = GetService<IMasterService>();
                var list = masterService.GetAdSet("1").Select(c => new { c.Link, c.PicLink }).ToList();
                return Json(JsonResultData.Successed(list));
            }
            catch
            {
                return Json(JsonResultData.Successed(null));
            }
        }

        #endregion 获取广告

        #region 体验试用

        public ActionResult TryInfo()
        {
            //var domain = ".gshis.com";
            //if (IsBakDomainGshisNet())
            //{
            //    domain = ".gshis.net";
            //}
            ViewBag.product = GetProduct();
            ViewBag.domain = Request.Headers["Host"];
            return View();
        }

        [HttpPost]
        public ActionResult TryInfo(TryInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                //验证码验证
                var result = SMSSendHelper.MatchCheckCode(model.MobileNo, model.CheckCode, CheckFunc.TryUsePms);
                if (result.Success)
                {
                    //记录试用手机
                    var tryService = GetService<ITryInfoService>();
                    var tryInfo = new TryInfo
                    {
                        Mobile = model.MobileNo,
                        TryDate = DateTime.Now
                    };
                    tryService.Add(tryInfo);
                    tryService.Commit();
                    return Json(JsonResultData.Successed(Url.Action("Index", "Home")));
                }
                return Json(result);
            }
            return Json(JsonResultData.Failure(ModelState.Values));
        }

        #endregion 体验试用

        #region 授权他人登录

        public ActionResult AuthUser()
        {
            var currentUser = GetService<ICurrentInfo>();
            var model = new AuthUserViewModel
            {
                BeginDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                UserName = currentUser.UserName
            };
            return PartialView(model);
        }

        public ActionResult AuthSave(AuthUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!model.BeginDate.HasValue)
                    {
                        return Json(JsonResultData.Failure("请选择生效时间"));
                    }
                    if (!model.EndDate.HasValue)
                    {
                        return Json(JsonResultData.Failure("请选择失效时间"));
                    }
                    if (model.EndDate <= model.BeginDate)
                    {
                        return Json(JsonResultData.Failure("失效时间必须大于生效时间"));
                    }
                    if (model.EndDate <= DateTime.Now)
                    {
                        return Json(JsonResultData.Failure("失效时间必须大于当前时间"));
                    }
                    var service = GetService<IServiceAuthorizeService>();
                    var currentInfo = GetService<ICurrentInfo>();
                    string authCode;
                    var result = service.AddAuthorizeService(currentInfo.GroupId, currentInfo.HotelId, model.BeginDate.Value, model.EndDate.Value, currentInfo.UserId, currentInfo.UserCode, currentInfo.UserName, out authCode);
                    if (result.Success)
                    {
                        return Json(JsonResultData.Successed(authCode));
                    }
                    return Json(result);
                }
                else
                {
                    return Json(JsonResultData.Failure(ModelState.Values));
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        #endregion 授权他人登录

        #region 使用授权登录

        public ActionResult AuthLogin()
        {
            return View();
        }

        [HttpPost, JsonException]
        public ActionResult AuthLogin(AuthLoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var _accountService = GetService<IServiceAuthorizeService>();
                var loginResult = _accountService.AuthLogin(loginModel.AuthCode, loginModel.Username, loginModel.Password);
                if (!loginResult.LoginSuccess)
                {
                    ModelState.AddModelError("Username", loginResult.ErrorMessage);
                    return Json(JsonResultData.Failure(ModelState.Values));
                }
                var _currentInfo = GetService<ICurrentInfo>();
                _currentInfo.Clear();
                FormsAuthentication.SetAuthCookie(loginResult.UserCode, false);

                _currentInfo.GroupId = loginResult.Grpid;
                _currentInfo.HotelId = loginResult.Hid;
                _currentInfo.HotelName = loginResult.HotelName;
                _currentInfo.DbServer = loginResult.DbServer;
                _currentInfo.DbName = loginResult.DbName;
                _currentInfo.DbUser = loginResult.DbUser;
                _currentInfo.DbPwd = loginResult.DbPwd;
                _currentInfo.UserId = loginResult.UserId;
                _currentInfo.UserCode = loginResult.UserCode;
                _currentInfo.UserName = loginResult.UserName;
                _currentInfo.IsRegUser = loginResult.IsRegUser;
                _currentInfo.VersionId = loginResult.VersionId;

                var serverUrl = loginResult.WebServerUrl;
                //新的服务器地址不是完整的域名，需要添加上现在请求的域名来构成新的跳转域名
                var product = GetProduct();
                serverUrl = string.Format("{0}.{1}", serverUrl, product.Domain);

                _currentInfo.WebServerUrl = serverUrl;
                _currentInfo.ModuleCode = GetModuleCodeByDomain(product.Domain);
                _currentInfo.ProductType = ProductTypeHelper.GetProductType(product);
                _currentInfo.SaveValues();

                return Json(JsonResultData.Successed(serverUrl));
            }
            return Json(JsonResultData.Failure(ModelState.Values));
        }

        #endregion 使用授权登录

        #region 根据当前域名判断产品类型

        private M_v_products GetProduct()
        {
            string domain = GetAccessDomain();
            var productService = GetService<IProductService>();
            return productService.GetProductByDomain(domain);
        }

        private string GetAccessDomain()
        {
            var accessDomain = Request.Headers["Host"];
            var domain = SharedSessionModule.GetLastThreeLevelDomain(accessDomain);
            return domain;
        }

        #endregion 根据当前域名判断产品类型

        #region 绑定微信

        /// <summary>
        /// 进入绑定微信界面
        /// </summary>
        /// <returns></returns>
        public ActionResult BindWeixin()
        {
            var currentUser = GetService<ICurrentInfo>();
            var userService = GetService<IPmsUserService>();
            var userId = Guid.Parse(currentUser.UserId);
            var user = userService.Get(userId);
            var model = new BindWeixinViewModel
            {
                WxOpenId = user.WxOpenId
            };
            //如果还没有绑定，则自动生成好一个带参二维码，以便操作员进行绑定
            if (string.IsNullOrWhiteSpace(user.WxOpenId))
            {
                var qrCodeService = GetService<IQrCodeService>();
                var addQrResult = Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models.QrCodeHelper.CreateQrCode(qrCodeService, currentUser.GroupHotelId, QrCodeType.PmsUser, currentUser.UserId);
                if (addQrResult.Success)
                {
                    var qrCodeInfo = addQrResult.Data as WeixinQrcodes;
                    if (qrCodeInfo != null)
                    {
                        model.QrCodeImageUrl = QrCodeApi.GetShowQrCodeUrl(qrCodeInfo.Ticket);
                    }
                }
            }
            else
            {
                //显示已绑定的用户信息
                string nickname = ""; string headimgurl = "";
                var result = Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models.UserHelper.GetUserInfo(user.WxOpenId, out nickname, out headimgurl);
                if (result != null && result.Success)
                {
                    model.UserNickName = nickname;
                    model.UserHeadImgUrl = headimgurl;
                }
                else
                {
                    model.UserNickName = model.WxOpenId;
                }
            }
            return PartialView(model);
        }

        /// <summary>
        /// 删除之前的微信绑定信息
        /// </summary>
        /// <returns></returns>
        public ActionResult UnBindWeixin()
        {
            var currentUser = GetService<ICurrentInfo>();
            var userService = GetService<IPmsUserService>();
            var centerDb = GetService<Gemstar.BSPMS.Common.Services.EF.DbCommonContext>();
            var result = userService.UnbindWeixin(currentUser.GroupHotelId, currentUser.UserId, centerDb);
            return Json(result);
        }

        /// <summary>
        /// 重新生成二维码，用于操作员重新绑定
        /// </summary>
        /// <returns></returns>
        public ActionResult ReCreateQrCode()
        {
            var currentUser = GetService<ICurrentInfo>();
            var qrCodeService = GetService<IQrCodeService>();
            var addQrResult = Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models.QrCodeHelper.CreateQrCode(qrCodeService, currentUser.GroupHotelId, QrCodeType.PmsUser, currentUser.UserId);
            if (addQrResult.Success)
            {
                var qrCodeInfo = addQrResult.Data as WeixinQrcodes;
                if (qrCodeInfo != null)
                {
                    return Json(JsonResultData.Successed(QrCodeApi.GetShowQrCodeUrl(qrCodeInfo.Ticket)));
                }
            }
            return Json(JsonResultData.Failure(addQrResult.Data));
        }

        #endregion 绑定微信

        #region 二维码登录

        private const string domainUrl = "http://pos.gshis.com";

        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <returns>qrcodeSessionId二维码会话ID</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetQrcode(byte? loginType, string authCode)
        {
            int genQrcodeType = 1;
            string keyid = null;
            string qrcodeSessionId = GetService<IQrcodeLoginService>().GenerateQrcode(loginType ?? 1, authCode, out keyid);
            object result = null;
            if (!string.IsNullOrWhiteSpace(qrcodeSessionId))
            {
                string url = "";
                if (genQrcodeType == 0)
                {
                    url = domainUrl + Url.Action("ScanQrcode", "Account", new { s = keyid });
                }
                else
                {
                    url = domainUrl + Url.Action("GetQrcode", "Account", new { sign = qrcodeSessionId });
                }
                result = JsonResultData.Successed(new
                {
                    genQrcodeType = genQrcodeType, //二维码类型（0：jquery生成二维码；其他：服务器生成二维码；）
                    qrcodeSessionId = qrcodeSessionId,//二维码会话ID
                    qrcodeUrl = url//二维码URL
                });
            }
            else
            {
                result = JsonResultData.Failure("获取二维码失败！");
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="sign">qrcodeSessionId二维码会话ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetQrcode(string sign)
        {
            string keyid = GetService<IQrcodeLoginService>().GetQrcodeKeyIdById(sign);
            if (string.IsNullOrWhiteSpace(keyid)) { return File(new byte[] { }, "image/png"); }

            string url = domainUrl + Url.Action("ScanQrcode", "Account", new { s = keyid });
            var imageDate = Gemstar.BSPMS.Common.Tools.QrCodeHelper.GetQrCode(url);
            return File(imageDate, "image/png");
        }

        /// <summary>
        /// 扫描二维码
        /// </summary>
        /// <param name="s">keyid</param>
        /// <returns></returns>
        public ActionResult ScanQrcode(string s)
        {
            string id = null;
            string redirectUrl = GetService<IQrcodeLoginService>().GetQrcodeUrl(s, out id);
            string url = Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models.OAuthHelper.GetAuthorizeUrl(redirectUrl, id);
            return Redirect(url);
        }

        /// <summary>
        /// 获取二维码状态
        /// </summary>
        /// <param name="nowTime">当前时间戳</param>
        /// <param name="sign">qrcodeSessionId二维码会话ID</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        public ActionResult GetQrcodeState(long nowTime, string sign)
        {
            //1.参数验证
            var jsonErrorResult = Json(JsonResultData.Successed(new { errorcode = WeixinQrcodeLoginStatus.QRCODE_ERROR, jump = "", msg = "状态错误！" }), JsonRequestBehavior.DenyGet);
            if (nowTime <= 0 || string.IsNullOrWhiteSpace(sign))
            {
                return jsonErrorResult;
            }
            //2.获取状态
            string data = ""; WeixinQrcodeLogin entity = null;
            var status = GetService<IQrcodeLoginService>().GetQrcodeStatus(sign, out data, out entity);
            //3.返回结果
            var result = JsonResultData.Successed(new
            {
                errorcode = (int)status,
                jump = (status == WeixinQrcodeLoginStatus.QRCODE_LOGIN_SUCCESS ? data : ""),//登录成功后的值
                msg = (status == WeixinQrcodeLoginStatus.QRCODE_ERROR ? data : ""),//错误信息
            });
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 微信登录
        /// </summary>
        [ValidateAntiForgeryToken]
        public ActionResult LoginJumpForQrcode(string jump, string jsonPara)
        {
            bool IsCheckWxAuthLogin = false;
            var loginResult = GetService<IQrcodeLoginService>().Login(jump, out IsCheckWxAuthLogin);
            if (loginResult != null && loginResult.LoginSuccess)
            {
                var _currentInfo = GetService<ICurrentInfo>();
                _currentInfo.Clear();
                FormsAuthentication.SetAuthCookie(loginResult.UserCode, false);
                var sing = GetService<ISysParaService>().GetHotelFunctionses(loginResult.Hid);
                var data = sing.Where(s => s.FuncCode == "FuncSignature").FirstOrDefault();
                _currentInfo.GroupId = loginResult.Grpid;
                _currentInfo.HotelId = loginResult.Hid;
                _currentInfo.HotelName = loginResult.HotelName;
                _currentInfo.DbServer = loginResult.DbServer;
                _currentInfo.DbName = loginResult.DbName;
                _currentInfo.DbUser = loginResult.DbUser;
                _currentInfo.DbPwd = loginResult.DbPwd;
                _currentInfo.UserId = loginResult.UserId;
                _currentInfo.UserCode = loginResult.UserCode;
                _currentInfo.UserName = loginResult.UserName;
                _currentInfo.IsRegUser = loginResult.IsRegUser;
                _currentInfo.Signature = data == null ? "0" : data.Isvalid == null ? "0" : data.Isvalid == true ? "1" : "0";
                _currentInfo.LoginTimeTicks = DateTime.UtcNow.Ticks.ToString();
                _currentInfo.VersionId = loginResult.VersionId;
                var serverUrl = loginResult.WebServerUrl;
                //新的服务器地址不是完整的域名，需要添加上现在请求的域名来构成新的跳转域名
                var product = GetProduct();
                serverUrl = string.Format("{0}.{1}", serverUrl, product.Domain);

                _currentInfo.WebServerUrl = serverUrl;
                _currentInfo.ModuleCode = GetModuleCodeByDomain(product.Domain);
                _currentInfo.ProductType = ProductTypeHelper.GetProductType(product);
                _currentInfo.SaveValues();
                return Redirect(serverUrl);
            }

            return Redirect(domainUrl);
        }

        #endregion 二维码登录

        #region GsBrowser 登录接口

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost, JsonException]
        public ActionResult GsBrowserLogin(LoginViewModel loginModel)
        {
            var hotelinfo = GetService<IHotelInfoService>().GetHotelInfo(loginModel.HotelId);
            if (hotelinfo != null && !string.IsNullOrWhiteSpace(hotelinfo.Grpid) && hotelinfo.Grpid != hotelinfo.Hid)
            {
                return Json(JsonResultData.Failure("酒店id不正确"));
            }
            //检查指定酒店是否已经开通当前域名对应的产品
            var productService = GetService<IProductService>();
            var hotelProducts = productService.GetHotelProducts(loginModel.HotelId);
            if (hotelProducts == null || hotelProducts.Count == 0)
            {
                return Json(JsonResultData.Failure("指定酒店没有开通此产品，请与软件供应商联系"));
            }
            var product = GetProduct();
            var exists = hotelProducts.Any(w => w == product.Code);
            if (!exists)
            {
                return Json(JsonResultData.Failure("指定酒店没有开通此产品，请与软件供应商联系"));
            }

            if (ModelState.IsValid)
            {
                var _accountService = GetService<IAccountService>();
                var loginResult = _accountService.Login(loginModel.HotelId, loginModel.Username, loginModel.Password);
                if (!loginResult.LoginSuccess)
                {
                    ModelState.AddModelError("Username", loginResult.ErrorMessage);
                    return Json(JsonResultData.Failure(ModelState.Values));
                }
                var _currentInfo = GetService<ICurrentInfo>();
                _currentInfo.Clear();
                FormsAuthentication.SetAuthCookie(loginResult.UserCode, false);
                var sing = GetService<ISysParaService>().GetHotelFunctionses(loginResult.Hid);
                var data = sing.Where(s => s.FuncCode == "FuncSignature").FirstOrDefault();
                _currentInfo.GroupId = loginResult.Grpid;
                _currentInfo.HotelId = loginResult.Hid;
                _currentInfo.HotelName = loginResult.HotelName;
                _currentInfo.DbServer = loginResult.DbServer;
                _currentInfo.DbName = loginResult.DbName;
                _currentInfo.DbUser = loginResult.DbUser;
                _currentInfo.DbPwd = loginResult.DbPwd;
                _currentInfo.UserId = loginResult.UserId;
                _currentInfo.UserCode = loginResult.UserCode;
                _currentInfo.UserName = loginResult.UserName;
                _currentInfo.IsRegUser = loginResult.IsRegUser;
                _currentInfo.Signature = data == null ? "0" : data.Isvalid == null ? "0" : data.Isvalid == true ? "1" : "0";
                _currentInfo.LoginTimeTicks = DateTime.UtcNow.Ticks.ToString();
                _currentInfo.VersionId = loginResult.VersionId;
                var serverUrl = loginResult.WebServerUrl;
                //新的服务器地址不是完整的域名，需要添加上现在请求的域名来构成新的跳转域名
                serverUrl = string.Format("{0}.{1}", serverUrl, product.Domain);

                _currentInfo.WebServerUrl = serverUrl;
                _currentInfo.ModuleCode = GetModuleCodeByDomain(product.Domain);
                _currentInfo.ProductType = ProductTypeHelper.GetProductType(product);
                _currentInfo.SaveValues();

                var hotelService = GetService<IHotelInfoService>();
                var hotel = hotelService.ListValidHotels().Where(m => m.Hid == loginModel.HotelId).FirstOrDefault();
                var grpid = hotel.Grpid;

                var hids = "";
                var grpHotels = hotelService.ListValidHotels().Where(m => m.Grpid == grpid).ToList();
                foreach (var temp in grpHotels)
                {
                    hids += temp.Hid + ",";
                }

                if (string.IsNullOrWhiteSpace(loginResult.Grpid))
                {
                    hids = loginResult.Hid;
                }
                var posService = GetService<IPosPosService>();
                var poslist = posService.GetPosByHids(hids.Trim(','));

                var authService = GetService<IAuthListService>();
                var authlist = authService.GetUserAuthLists(hotel.Grpid, hotel.Hid, loginResult.UserId).Select(s => new { s.AuthCode }).ToList();

                var userService = GetService<IPmsUserService>();
                if (hotel.Hid == hotel.Grpid)
                {
                    var hotels = userService.GetResortListForOperator(hotel.Grpid, loginResult.UserId);
                    foreach (var item in hotels)
                    {
                        var centerHotel = hotelService.ListValidHotels().Where(m => m.Hid == item.Hid).FirstOrDefault();
                        item.CateringServicesModule = centerHotel.CateringServicesModule;
                    }

                    var cateringServicesTypes = hotelService.ListValidHotels().Where(m => m.Grpid == loginModel.HotelId).Select(w => new { w.Hid, w.CateringServicesType }).ToList();

                    return Json(JsonResultData.Successed(new { isGroup = true, hotel, _currentInfo, hotels, authlist, poslist, cateringServicesTypes }));
                }
                else



                {
                    var cateringServicesTypes = hotelService.ListValidHotels().Where(m => m.Hid == loginModel.HotelId).Select(w => new { w.Hid, w.CateringServicesType }).ToList();
                    return Json(JsonResultData.Successed(new { isGroup = false, hotel, authlist, poslist, _currentInfo }));
                }
            }
            return Json(JsonResultData.Failure(ModelState.Values));
        }

        /// <summary>
        /// 自动登录
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AutoLogin(string current)
        {
            //解密当前登录用户信息
            var decrypt = EncryptUtil.UnAesStr(current, "pos.gshis.com   ", "捷信达浏览器：GsBrowser");

            //转换Json对象
            JObject loginResult = (JObject)JsonConvert.DeserializeObject(decrypt);

            if (loginResult != null)
            {
                if (loginResult["Current"] != null)
                {
                    loginResult = loginResult["Current"] as JObject;
                }

                var product = GetProduct();
                var _currentInfo = GetService<ICurrentInfo>();
                _currentInfo.Clear();
                FormsAuthentication.SetAuthCookie(loginResult["UserCode"].ToString(), false);
                var sing = GetService<ISysParaService>().GetHotelFunctionses(loginResult["HotelId"].ToString());
                var data = sing.Where(s => s.FuncCode == "FuncSignature").FirstOrDefault();
                _currentInfo.GroupId = loginResult["GroupId"].ToString();
                _currentInfo.HotelId = loginResult["HotelId"].ToString();
                _currentInfo.HotelName = loginResult["HotelName"].ToString();
                _currentInfo.DbServer = loginResult["DbServer"].ToString();
                _currentInfo.DbName = loginResult["DbName"].ToString();
                _currentInfo.DbUser = loginResult["DbUser"].ToString();
                _currentInfo.DbPwd = loginResult["DbPwd"].ToString();
                _currentInfo.UserId = loginResult["UserId"].ToString();
                _currentInfo.UserCode = loginResult["UserCode"].ToString();
                _currentInfo.UserName = loginResult["UserName"].ToString();
                _currentInfo.IsRegUser = Convert.ToBoolean(loginResult["IsRegUser"]);
                _currentInfo.Signature = loginResult["Signature"].ToString();
                _currentInfo.LoginTimeTicks = DateTime.UtcNow.Ticks.ToString();
                _currentInfo.WebServerUrl = loginResult["WebServerUrl"].ToString();
                _currentInfo.ModuleCode = GetModuleCodeByDomain(product.Domain);
                _currentInfo.ProductType = ProductTypeHelper.GetProductType(product);
                _currentInfo.VersionId = loginResult["VersionId"] != null ? loginResult["VersionId"].ToString() : "";

                if (loginResult["PosId"] != null && loginResult["PosId"].ToString() != "")
                {
                    _currentInfo.PosId = loginResult["PosId"].ToString();
                    _currentInfo.PosName = loginResult["PosName"].ToString();
                }
                else
                {
                    //收银点信息
                    var pos = GetService<IPosPosService>().GetPosByHid(_currentInfo.HotelId).Where(m => (m.IStatus == (byte)EntityStatus.启用 || m.IStatus == null)).FirstOrDefault();
                    if (pos != null)
                    {
                        _currentInfo.PosId = pos.Id;
                        _currentInfo.PosName = pos.Name;
                    }
                }

                _currentInfo.SaveValues();

                var url = "";
                if (Request["ReturnUrl"] == null || string.IsNullOrWhiteSpace(Request["ReturnUrl"]))
                {
                    url = Request.Url.ToString();
                }
                else
                {
                    url = _currentInfo.WebServerUrl + Request["ReturnUrl"].ToString();
                }

                return Content(url);
            }

            return Content("");
        }

        public PartialViewResult _UpdateCurrent()
        {
            return PartialView("_UpdateCurrent");
        }

        #endregion GsBrowser 登录接口

        #region 捷云自动登录

        [HttpGet]
        public ActionResult AutoLogin(string hid, string username, string pwd)
        {
            var hotelinfo = GetService<IHotelInfoService>().GetHotelInfo(hid);
            if (hotelinfo != null && !string.IsNullOrWhiteSpace(hotelinfo.Grpid) && hotelinfo.Grpid != hotelinfo.Hid)
            {
                ViewBag.ErrorInfo = "酒店id不正确";
                return View("ErrorInfo");
            }
            //检查指定酒店是否已经开通当前域名对应的产品
            var productService = GetService<IProductService>();
            var hotelProducts = productService.GetHotelProducts(hid);
            if (hotelProducts == null || hotelProducts.Count == 0)
            {
                ViewBag.ErrorInfo = "指定酒店没有开通此产品，请与软件供应商联系";
                return View("ErrorInfo");
            }
            var product = GetProduct();
            var exists = hotelProducts.Any(w => w == product.Code);
            if (!exists)
            {
                ViewBag.ErrorInfo = "指定酒店没有开通此产品，请与软件供应商联系";
                return View("ErrorInfo");
            }

            if (ModelState.IsValid)
            {
                var _accountService = GetService<IAccountService>();
                var loginResult = _accountService.AutoLogin(hid, username, pwd);
                if (!loginResult.LoginSuccess)
                {
                    ModelState.AddModelError("Username", loginResult.ErrorMessage);
                    ViewBag.ErrorInfo = loginResult.ErrorMessage.ToString();
                    return View("ErrorInfo");
                }
                var _currentInfo = GetService<ICurrentInfo>();
                _currentInfo.Clear();
                FormsAuthentication.SetAuthCookie(loginResult.UserCode, false);
                var sing = GetService<ISysParaService>().GetHotelFunctionses(loginResult.Hid);
                var data = sing.Where(s => s.FuncCode == "FuncSignature").FirstOrDefault();
                _currentInfo.GroupId = loginResult.Grpid;
                _currentInfo.HotelId = loginResult.Hid;
                _currentInfo.HotelName = loginResult.HotelName;
                _currentInfo.DbServer = loginResult.DbServer;
                _currentInfo.DbName = loginResult.DbName;
                _currentInfo.DbUser = loginResult.DbUser;
                _currentInfo.DbPwd = loginResult.DbPwd;
                _currentInfo.UserId = loginResult.UserId;
                _currentInfo.UserCode = loginResult.UserCode;
                _currentInfo.UserName = loginResult.UserName;
                _currentInfo.IsRegUser = loginResult.IsRegUser;
                _currentInfo.Signature = data == null ? "0" : data.Isvalid == null ? "0" : data.Isvalid == true ? "1" : "0";
                _currentInfo.LoginTimeTicks = DateTime.UtcNow.Ticks.ToString();
                _currentInfo.VersionId = loginResult.VersionId;
                var serverUrl = loginResult.WebServerUrl;
                //新的服务器地址不是完整的域名，需要添加上现在请求的域名来构成新的跳转域名
                serverUrl = string.Format("{0}.{1}", serverUrl, product.Domain);

                _currentInfo.WebServerUrl = serverUrl;
                _currentInfo.ModuleCode = GetModuleCodeByDomain(product.Domain);
                _currentInfo.ProductType = ProductTypeHelper.GetProductType(product);
                _currentInfo.SaveValues();
                //判断是否是默认密码
                var pmsUserService = GetService<IPmsUserService>();
                if (pmsUserService.IsUserPassowrdDefault(loginResult.UserId))
                {
                    //转到修改密码的界面中
                    return Json(JsonResultData.Successed(Url.Action("ChangePassword")), JsonRequestBehavior.AllowGet);
                }
                //判断是否是试用用户
                //取出运营系统参数中的试用参数信息
                var sysParaService = GetService<ISysParaService>();
                var tryHotelPara = sysParaService.Get("TryHotelIdForPos");
                var tryUserName = sysParaService.Get("TryUsernameForPos");
                var tryUserPass = sysParaService.Get("TryUserPassForPos");
                var tryHotelId = tryHotelPara == null ? "" : tryHotelPara.Value;
                var tryUser = tryUserName == null ? "" : tryUserName.Value;
                var tryPass = tryUserPass == null ? "" : tryUserPass.Value;
                if (hid == tryHotelId && username == tryUser && pwd == tryPass)
                {
                    //判断是否有提交体验cookie，没有则转到试用信息收集界面
                    var tryCookie = Request.Cookies[TryCookieName];
                    if (tryCookie == null)
                    {
                        return Json(JsonResultData.Successed(Url.Action("TryInfo")), JsonRequestBehavior.AllowGet);
                    }
                }

                //默认添加一条数据.
                var hotelService = GetService<IHotelInfoService>();
                var hotel = hotelService.ListValidHotels().Where(m => m.Hid == hid).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(hotel.Grpid))
                {
                    //单店
                    CreatePosDate(hid, _currentInfo.ModuleCode);
                    //添加默认的高级功能
                    CreatePosAdvanceFunc(hid, _currentInfo.ModuleCode);
                }
                else
                {
                    //集团
                    var userService = GetService<IPmsUserService>();
                    //通过用户code 获取用户ID
                    var user = userService.GetUserIDByCode(hid, username);
                    //获取分店列表
                    var ResortList = userService.GetResortListForOperator(hid, user.Id.ToString());
                    foreach (var hotelModel in ResortList)
                    {
                        CreatePosDate(hotelModel.Hid, _currentInfo.ModuleCode);

                        CreatePosAdvanceFunc(hotelModel.Hid, _currentInfo.ModuleCode);
                    }
                }

                if (Request["hid"] != null && Request["mode"] != null)
                {
                    var abc = serverUrl + Request["ReturnUrl"].ToString();
                    if (Request["hid"].ToString() == _currentInfo.HotelId && (Request["mode"].ToString() == "B" || Request["mode"].ToString() == "C"))
                    {
                        return Redirect(serverUrl + Request["ReturnUrl"].ToString());
                    }
                    else
                    {
                        return Redirect(serverUrl + Request["ReturnUrl"].ToString());
                    }
                }

                return Redirect(serverUrl);
            }
            ViewBag.ErrorInfo = "此操作员没有权限！";
            return View("ErrorInfo");
        }

        #endregion 捷云自动登录

        /// <summary>
        /// 根据产品域名获取模块代码
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public string GetModuleCodeByDomain(string domain)
        {
            var moduleCode = "";
            if (domain.ToLower().IndexOf("pos") > -1 || domain.ToLower().IndexOf("postest") > -1)
            {
                moduleCode = Enum.GetName(typeof(ModuleCode), ModuleCode.CY);
            }

            return moduleCode;
        }
    }
}