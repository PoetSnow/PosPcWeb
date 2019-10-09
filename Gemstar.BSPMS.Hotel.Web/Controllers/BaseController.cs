using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.ResFolioManage;
using Kendo.Mvc.UI;
using Gemstar.BSPMS.Hotel.Services.ResManage;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.Extensions;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Common.Services.Enums;
using System.Collections.Generic;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.ObjectModel;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Common.DataAnnotations;
using System.Text.RegularExpressions;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;

namespace Gemstar.BSPMS.Hotel.Web.Controllers
{
    /// <summary>
    /// 通用的父controller，提供一些通用的实现，除account以外的所有controller都继承此controller
    /// 要求所有的controller都不要提供实例的服务接口字段，服务接口实例在action中需要时再实例化，而不是在类的构造函数中实例化，以避免不必要的创建。因为有些服务接口只是此类的某一个action才需要的
    /// </summary>
    [Authorize]
    [Auth]
    [DogCheck]
    [ExecuteTimeLog]
    public class BaseController : Controller
    {

        /// <summary>
        /// 当前版本号，引用外部文件时调用，防止修改文件后，本地文件缓存不能及时更新，例如：*.js、*.css...等文件
        /// </summary>
        protected string CurrentVersion
        {
            get { return BundleConfig.GetVersion(); }
        }

        #region 检查会话信息

        public ActionResult CheckSession()
        {
            if (string.IsNullOrWhiteSpace(CurrentInfo.HotelId))
            {
                //登录信息已经丢失，直接跳回到登录界面
                return Redirect(FormsAuthentication.LoginUrl);
            }
            return null;
        }

        #endregion 检查会话信息

        #region 视图名称，如果是集团模式，则在原视图名称的后面加上Group

        /// <summary>
        /// 视图名称，如果是集团模式，则在原视图名称的后面加上Group
        /// </summary>
        /// <param name="originViewName">原视图名称</param>
        /// <returns>当前模式对应的正确的视图名称</returns>
        public string GetViewName(string originViewName)
        {
            if (CurrentInfo.IsGroup)
            {
                return string.Format("{0}Group", originViewName);
            }
            return originViewName;
        }

        #endregion 视图名称，如果是集团模式，则在原视图名称的后面加上Group

        #region 当前登录信息

        private ICurrentInfo _currentInfo;

        protected ICurrentInfo CurrentInfo
        {
            get
            {
                if (_currentInfo == null)
                {
                    _currentInfo = GetService<ICurrentInfo>();
                }
                return _currentInfo;
            }
        }

        #endregion 当前登录信息

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
            var connStr = ConnStrHelper.GetConnStr(hotelInfo.DbServer, hotelInfo.DbName, hotelInfo.Logid, hotelInfo.LogPwd, "GemstarBSPMS", hotelInfo.DbServerInternet, isConnectViaInternet);
            var hotelDb = new DbHotelPmsContext(connStr, hid, "", Request);
            return hotelDb;
        }

        #endregion 获取服务接口

        #region 通用查询

        /// <summary>
        /// 设置通用查询用的参数值
        /// </summary>
        /// <param name="procedureName">查询用的存储过程名称</param>
        /// <param name="procedureParameterAndValues">存储过程的参数值，第一次时传递空值</param>
        public void SetCommonQueryValues(string procedureName, string procedureParameterAndValues, string gridControlId = "grid")
        {
            ViewBag.commonQueryModel = new CommonQueryModel { QueryProcedureName = procedureName, QueryParameterValues = procedureParameterAndValues, GridControlId = gridControlId };
        }

        [AuthButton(AuthFlag.Query)]
        [KendoGridDatasourceException]
        public ActionResult AjaxQueryResDetails(ResDetailsForCommonPara para, [DataSourceRequest]DataSourceRequest request)
        {
            para.Hid = CurrentInfo.HotelId;
            if (!para.IsSettle.HasValue) { para.IsSettle = 0; }

            var service = GetService<IResService>();
            var results = service.QueryResDetails(para);

            return Json(results.ToDataSourceResult(request));
        }

        #endregion 通用查询

        #region 下拉列表数据绑定

        /// <summary>
        /// 当前酒店房间类型下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForRoomType()
        {
            var service = GetService<IRoomTypeService>();
            var datas = service.List(CurrentInfo.HotelId);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Key, Text = w.Value }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取酒店客人来源下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForCustomerSource()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetCustomerSource(CurrentInfo.HotelId);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取酒店市场分类下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForMarketCategory()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetMarketCategory(CurrentInfo.HotelId);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取酒店证件类型下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForCerType()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetCerType();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取酒店价格代码下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForRates()
        {
            var service = GetService<IRateService>();
            var datas = service.List(CurrentInfo.HotelId);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Key, Text = w.Value }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取酒店合约单位下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForCompanys()
        {
            var service = GetService<ICompanyService>();
            var datas = service.List(CurrentInfo.HotelId, "");
            var listItems = datas.Select(w => new SelectListItem { Value = w.Key, Text = w.Value }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取酒店会员列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForProfiles(string nameOrMobile)
        {
            if (!string.IsNullOrWhiteSpace(nameOrMobile))
            {
                var service = GetService<Services.MbrCardCenter.IMbrCardService>();
                var datas = service.List(CurrentInfo.HotelId, nameOrMobile);
                var listItems = datas.Select(w => new { w.Id, w.MbrCardNo, w.GuestName, w.Mobile }).ToList();
                return Json(listItems, JsonRequestBehavior.AllowGet);
            }
            return Json(new System.Collections.Generic.List<Services.Entities.MbrCard>(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取当前酒店的消费项目类型和付款项目类型下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemForFolioItemType()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetFolioItemTypes(CurrentInfo.HotelId);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取指定处理方式的消费项目下拉数据源
        /// </summary>
        /// <param name="type">d/c</param>
        /// <param name="itemAction">处理方式</param>
        /// <returns>满足条件的项目列表</returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListItemsForSpecialActionItem(string type, string itemAction, bool returnAll = false)
        {
            var services = GetService<IItemService>();
            var items = services.GetItembyAction(CurrentInfo.HotelId, itemAction, type, "");
            if (returnAll)
            {
                var listItems = items.Select(w => new { Value = w.Id, Text = w.Name, Rate = w.Rate }).ToList();
                return Json(listItems, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var listItems = items.Select(w => new SelectListItem { Value = w.Id, Text = w.Name }).ToList();
                return Json(listItems, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取当前酒店的指定订单下的客单数据
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemFoResDetailGuest(string resId)
        {
            var services = GetService<IResFolioService>();
            var datas = services.QueryResDetailForFolioAdd(CurrentInfo.HotelId, resId);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Regid, Text = w.RegName }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取当前酒店的发票开票项目类型下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemForInvoiceProjectType()
        {
            var service = GetService<ICodeListService>();

            CodeType codeTypeEntity = GetService<ICodeListService>().GetCodeType("13");
            var datas = service.GetInvoiceProjectType(CurrentInfo.HotelId);
            List<SelectListItem> listItems = new List<SelectListItem>();
            if (codeTypeEntity != null)
            {
                string splitStr = "&|&";
                string labelName = "税率";
                if (codeTypeEntity.lable12 == labelName)
                {
                    listItems = datas.Select(w => new SelectListItem { Value = (w.Id + splitStr + w.Name2), Text = w.Name }).ToList();
                }
                else if (codeTypeEntity.label3 == labelName)
                {
                    listItems = datas.Select(w => new SelectListItem { Value = (w.Id + splitStr + w.Name3), Text = w.Name }).ToList();
                }
                else if (codeTypeEntity.label4 == labelName)
                {
                    listItems = datas.Select(w => new SelectListItem { Value = (w.Id + splitStr + w.Name4), Text = w.Name }).ToList();
                }
                else
                {
                    listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Name }).ToList();
                }
            }
            else
            {
                listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Name }).ToList();
            }
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取当前酒店的用户
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemForPmsUser()
        {
            var service = GetService<IoperationLog>();
            var datas = service.GetPmsUser(CurrentInfo.HotelId);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Code, Text = w.Name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        #region 帮助文档下拉框

        /// <summary>
        /// 获取一级菜单
        /// </summary>
        /// <returns></returns>
        public JsonResult ListItemAuth()
        {
            var service = GetService<IAuthCheck>();
            var datas = service.GetChildAuths("1", CurrentInfo.UserId, CurrentInfo.HotelId, CurrentInfo.ProductType, CurrentInfo.AuthListType);
            var listItems = datas.Select(w => new SelectListItem { Value = w.AuthCode, Text = w.AuthName });
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListItemAuthChild(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
            var service = GetService<IAuthCheck>();
            var datas = service.GetChildAuths(id, CurrentInfo.UserId, CurrentInfo.HotelId, CurrentInfo.ProductType, CurrentInfo.AuthListType);
            var listItems = datas.Select(w => new SelectListItem { Value = w.AuthCode, Text = w.AuthName });
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListItembntChild(string id, string name)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
            var service = GetService<IAuthCheck>();
            var datas = service.GetBntAuths(id);
            datas.Insert(0, new SelectListItem() { Text = name, Value = id });
            if (id == "30001")
            {
                datas.Insert(1, new SelectListItem() { Text = "会员详情", Value = "30001_64" });
            }
            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        #endregion 帮助文档下拉框

        /// <summary>
        /// 获取国籍列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public JsonResult ListItemsForNation(string text)
        {
            try
            {
                var service = GetService<IResService>();
                var datas = service.GetNationList(text);
                return Json(datas, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 业务员,下拉框
        /// </summary>
        /// <param name="nameOrMobile">名称或手机号</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetSalesnameSelectLists()
        {
            var _salesService = GetService<ISalesService>();
            List<Sales> alist = _salesService.List(CurrentInfo.IsGroup ? CurrentInfo.GroupId : CurrentInfo.HotelId);
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Name.ToString(), Text = item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 价格代码
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetRateSelectList()
        {
            var _rateService = GetService<IRateService>();
            return Json(_rateService.List(CurrentInfo.HotelId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 服务员列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsWaiter()
        {
            var service = GetService<IItemService>();
            var data = service.GetCodeList("18", CurrentInfo.HotelId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据身份证号码获取籍贯
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetCity(string idCard)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(idCard) && idCard.Length >= 4)
                {
                    string provinceCode = idCard.Substring(0, 2) + "0000";
                    string cityCode = idCard.Substring(0, 4) + "00";
                    var list = GetService<IMasterService>().GetCity(provinceCode, cityCode);
                    if (list != null && list.Count > 0 && !string.IsNullOrWhiteSpace(list[0].Name))
                    {
                        string result = list[0].Name;
                        if (!result.StartsWith(list[0].ProvinceName))
                        {
                            result = list[0].ProvinceName + result;
                        }
                        return Json(JsonResultData.Successed(result), JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        var provinceEntity = GetService<IMasterService>().GetProvince(provinceCode);
                        if (provinceEntity != null && !string.IsNullOrWhiteSpace(provinceEntity.Name))
                        {
                            return Json(JsonResultData.Successed(provinceEntity.Name), JsonRequestBehavior.DenyGet);
                        }
                    }
                }
                return Json(JsonResultData.Failure(""), JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex), JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 获取微信授权人列表
        /// </summary>
        /// <param name="authType">授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权）</param>
        /// <returns>Key：授权人ID；Value：授权人姓名；</returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetWeiXinAuthorizationUsersToList(byte authType)
        {
            var result = GetService<IAuthorizationService>().GetWeiXinAuthorizationUsersToList(CurrentInfo, authType);
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 撤销申请
        /// </summary>
        /// <param name="id">主键ID</param>
        [AuthButton(AuthFlag.None)]
        public JsonResult RevokeAuthorization(Guid id)
        {
            GetService<IAuthorizationService>().RevokeAuthorization(CurrentInfo, id);
            return Json(JsonResultData.Successed(), JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 押金类型,下拉框
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetPermanentRoomDepositTypeSelectLists()
        {
            var service = GetService<ICodeListService>();
            var datas = service.List(CurrentInfo.HotelId, "20");
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取付款方式
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetPayMethodList()
        {
            var service = GetService<IPosItemService>();
            //取出是否支持预订的付款方式
            var datas = service.GetPosItem(CurrentInfo.HotelId, PosItemDcFlag.C.ToString()).Where(w => w.IsSubscription == true).ToList();
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id + "_" + w.PayType, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        #endregion 下拉列表数据绑定

        #region pos下拉数据绑定

        /// <summary>
        /// 获取Pos模块列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosModules()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosModules();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos类别列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosIstagclasss()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosIstagclasss();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos出品方式列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosProduceTypes()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosProduceTypes();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos微信点餐支付方式列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosWxPaytypes()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosWxPaytypes();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos财务分类列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosAcTypes()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosAcClasss();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// 获取Pos临时台标志列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosIstagtemp()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosIstagtemps();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos餐台状态列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosStatno()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosStatnos();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos财务类型列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosAcClass()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosAcClasss();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos币种列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosMontypeno()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosMontypeno();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos处理方式列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosPayType()
        {
            var service = GetService<ICodeListService>();
            var currentInfo = GetService<ICurrentInfo>();
            //取出当前酒店运营后台中设置的已开通的付款处理动作
            var hid = currentInfo.HotelId;
            var MasterService = GetService<IMasterService>();
            var ac = MasterService.GetHotelItemAction(hid);
            var actionList = new List<string>();
            actionList.Add("no");
            actionList.Add("PrePay");   //不修改运营平台 手动在后台添加一个定金的处理方式 snow 2019年7月18日
            if (ac != null)
            {
                actionList.AddRange(ac.Split(','));
            }
            //取出当前产品适用的所有付款处理动作
            var datas = service.GetPosPayType(currentInfo.ProductType);
            //合并取出当前酒店已经开通的付款处理动作
            var listItems = datas.Where(w => actionList.Contains(w.code)).Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos要求操作列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosiTagOperator()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosiTagOperator();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos联单打印列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosIsCombine()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosIsCombine();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos要求属性列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosIsTagProperty()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosIsTagProperty();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos出品状态列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosIsProduce()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosIsProduce();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos显示临时台列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosIsHideTab()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosIsHideTab();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos出品名称列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosIsProdName()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosIsProdName();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos状态列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosStatus()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosStatus();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos点作法选项列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosIsOrderAction()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosIsOrderAction();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos减库存列表下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosIsStock()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosIsStock();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos日期类型下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosITagperiod()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosITagperiod();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos最低消费记法下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosIsByPerson()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosIsByPerson();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos数量方式下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosQuanMode()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosQuanMode();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos收费状态下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosIsCharge()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosIsCharge();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos串口号下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosComno()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosComno();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos开台录入信息下拉数据源
        /// </summary>
        /// <param name="posId">收银点ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosOpenInfo(string posId)
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosOpenInfo();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            var posService = GetService<IPosPosService>();
            var Pos = posService.Get(posId);//当前收银点模式、、
            if (Pos != null)
            {
                if (Pos.PosMode == "B")//快餐
                {
                    listItems = datas.Where(m => m.code == "J").Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
                }
                else if (Pos.PosMode == "C")//零售
                {
                    listItems = datas.Where(m => m.code == "I").Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
                }
                else
                {
                    //餐饮
                    listItems = datas.Where(m => m.code != "I" && m.code != "J").Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
                }
                return Json(listItems, JsonRequestBehavior.AllowGet);
            }
            else
            {
                listItems = datas.Where(m => m.code != "I" && m.code != "J").Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
                return Json(listItems, JsonRequestBehavior.AllowGet);
            }

            //var service = GetService<ICodeListService>();
            //var datas = service.GetPosOpenInfo();
            //var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            //return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos折扣类型下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosIsForce()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosIsForce();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos金额折类型下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosDaType()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosDaType();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos账单状态下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosBillStatus()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosBillStatus();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取PosKTV开台类型下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosIKtvStatus()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosIKtvStatus();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos自动标志下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosIsauto()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosIsauto();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos账单明细状态下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosBillDetailStatus()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosBillDetailStatus();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos账单明细出品状态下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosBillDetailIsProduce()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosBillDetailIsProduce();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos客人类型状态下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosCustomerTypeStatus()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosCustomerTypeStatus();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Pos客人类型状态下拉数据源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosReasonIstagType()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetPosReasonIstagType();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 二级仓库列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListDepot()
        {
            var service = GetService<IPosDepotService>();
            var datas = service.GetPosDepotList(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取营业经理
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListSaleUser()
        {
            var service = GetService<IPmsUserService>();
            var datas = service.UsersInGroup(CurrentInfo.GroupHotelId).Where(w => !string.IsNullOrEmpty(w.OperatorStatus) && w.OperatorStatus.Contains("0"));
            var listItems = datas.Select(w => new SelectListItem { Value = w.Code, Text = w.Name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取酒店定金列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPrePay()
        {
            var service = GetService<IYtPrepayService>();
            var datas = service.GetYtPrepayList(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id.ToString(), Text = w.BillNo }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet); 


        }

        #endregion pos下拉数据绑定

        #region 自动完成数据绑定

        /// <summary>
        /// 自动完成指定类别下的项目列表
        /// </summary>
        /// <param name="type">项目类型</param>
        /// <param name="keyword">要查找的关键字</param>
        /// <returns>满足条件的项目列表</returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult AutoCompleteItem(string type, string keyword)
        {
            var services = GetService<IItemService>();
            var items = services.Query(CurrentInfo.HotelId, type, keyword);
            var newItems = items.Select(s => new Services.Entities.Item
            {
                CodeName = s.Code + "-" + s.Name,
                Code = s.Code,
                Name = s.Name,
                Seqid = s.Seqid,
                Status = s.Status,
                Taxrate = s.Taxrate,
                Action = s.Action,
                Alias = s.Alias,
                DcFlag = s.DcFlag,
                Hid = s.Hid,
                StaType = s.StaType,
                Id = s.Id,
                InvoiceItemid = s.InvoiceItemid,
                IsCharge = s.IsCharge,
                IsInput = s.IsInput,
                IsQuantity = s.IsQuantity,
                IsRetun = s.IsRetun,
                ItemTypeid = s.ItemTypeid,
                ItemTypeName = s.ItemTypeName,
                Nights = s.Nights,
                Price = s.Price,
                Rate = s.Rate <= 0 ? 1 : s.Rate
            }).ToList();
            if (newItems != null && newItems.Count > 0)
            {
                //消费入账权限控制
                var itemids = GetService<IRoleAuthItemConsumeService>().GetItemConsumeAuth(CurrentInfo.HotelId, CurrentInfo.UserId);
                if (itemids != null && itemids.Count > 0)
                {
                    var removeItemids = itemids.Where(c => c.Value == false).Select(c => c.Key).ToList();
                    if (removeItemids != null && removeItemids.Count > 0)
                    {
                        newItems.RemoveAll(c => removeItemids.Contains(c.Id));
                    }
                }
            }
            return Json(newItems, JsonRequestBehavior.AllowGet);
        }

        #endregion 自动完成数据绑定

        #region 指定的id是否是页面权限项的值

        /// <summary>
        /// 指定的id是否是页面权限项的值
        /// </summary>
        /// <param name="controller">子控制实例</param>
        /// <param name="id">要检查的参数值</param>
        /// <returns>true:是页面的权限值，false:不是权限值，是正常的参数值</returns>
        protected bool IsPageAuth(BaseController controller, string id)
        {
            var type = controller.GetType();
            var authPage = type.GetCustomAttributes(typeof(AuthPageAttribute), false);
            if (authPage.Length > 0)
            {
                return ((AuthPageAttribute)authPage[0]).AuthCode == id;
            }
            return false;
        }

        #endregion 指定的id是否是页面权限项的值

        #region 检查某个功能是否有权限(针对前端的控制)

        /// <summary>
        /// 检查某个功能是否有权限(针对前端的控制)
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="buttonValue"></param>
        /// <returns></returns>
        public bool IsHasAuth(string authCode, long buttonValue)
        {
            var service = GetService<IAuthCheck>();
            var result = service.HasAuth(CurrentInfo.UserId, authCode, buttonValue, CurrentInfo.HotelId);
            return result;
        }

        /// <summary>
        /// 是否启用长租管理功能
        /// </summary>
        /// <returns></returns>
        public bool IsPermanentRoom(string hid = null)
        {
            return GetService<Gemstar.BSPMS.Hotel.Services.IPmsParaService>().IsPermanentRoom(string.IsNullOrWhiteSpace(hid) ? CurrentInfo.HotelId : hid);//是否启用长租管理功能
        }

        #endregion 检查某个功能是否有权限(针对前端的控制)

        #region 批量启用禁用

        [AuthButton(AuthFlag.Enable)]
        public ActionResult _BatchChangeStatus<T>(string id, ICRUDService<T> service, EntityStatus status, OpLogType opType) where T : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(JsonResultData.Failure("请选择要删除的数据！"));
                }
                if (id == "0")
                {
                    return Json(JsonResultData.Failure("要删除的数据不存在！"));
                }
                var result = service.BatchUpdateStatus(id, status, opType);
                if (result.Success)
                {
                    AfterBatchChangeStatus();
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        [AuthButton(AuthFlag.Enable)]
        public ActionResult _BatchBatchChangeStatusGroup<T>(string id, EntityKeyDataType keyType, string basicDataCode, ICRUDService<T> service, EntityStatus status, OpLogType opType) where T : class, IBasicDataCopyEntity
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(JsonResultData.Failure("请选择要启用禁用的数据！"));
                }
                if (id == "0")
                {
                    return Json(JsonResultData.Failure("要启用禁用的数据不存在！"));
                }
                var hid = CurrentInfo.HotelId;
                bool isGroupInGroup = CurrentInfo.IsGroupInGroup;
                bool isHotelnGroup = CurrentInfo.IsHotelInGroup;
                bool hotelCanDisable = true;
                if (isHotelnGroup)
                {
                    //取出集团设置的基础资料分店控制设置
                    var resortControlService = GetService<IBasicDataResortControlService>();
                    var resortControl = resortControlService.GetResortControl(basicDataCode, CurrentInfo.GroupId);
                    hotelCanDisable = resortControl == null ? true : resortControl.ResortCanDisable;
                }
                var idArray = id.Split(',');
                var result = new List<object>();
                foreach (var idStr in idArray)
                {
                    if (!string.IsNullOrWhiteSpace(idStr))
                    {
                        var key = EntityKeyHelper.GetKeyValue(idStr, keyType);
                        var delete = service.Get(key);
                        //判断是否可以启用禁用
                        //集团只能启用禁用集团创建的记录，并且进行分发
                        if (isGroupInGroup)
                        {
                            if (delete.Hid != hid || (delete.DataSource != BasicDataDataSource.Added.Code && !string.IsNullOrWhiteSpace(delete.DataSource)))
                            {
                                return Json(JsonResultData.Failure("集团只能启用禁用集团创建的记录"));
                            }
                        }
                        //集团分店自己创建的数据可以启用禁用，集团分店受集团设置的是否可禁用权限来控制
                        else if (isHotelnGroup)
                        {
                            //如果是其他分店的，则不允许启用禁用
                            if (delete.Hid != hid)
                            {
                                return Json(JsonResultData.Failure("分店不能启用禁用其他分店自主的记录"));
                            }
                            else if (delete.DataSource == BasicDataDataSource.Copyed.Code && !hotelCanDisable)
                            {
                                return Json(JsonResultData.Failure("集团设置为不能禁用启用"));
                            }
                        }
                        //单店不受限制，都可以启用禁用
                        if (isGroupInGroup)
                        {
                            var deletedModels = service.ChangeStatusAndCopy(delete, status);
                            result.AddRange(deletedModels);
                        }
                        else
                        {
                            service.ChangeStatus(delete, status);
                            result.Add(delete);
                        }
                    }
                }
                service.AddDataChangeLog(opType);
                service.Commit();
                AfterBatchChangeStatusGroupCommit(result);
                return Json(JsonResultData.Successed(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 在批量删除完成，并且提交成功后调用，由子类重写来实现在删除后的一些业务处理
        /// </summary>
        protected virtual void AfterBatchChangeStatus() { }

        protected virtual void AfterBatchChangeStatusGroupCommit(List<object> deletedModels)
        {
        }

        #endregion 批量启用禁用

        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult _BatchDelete<T>(string id, ICRUDService<T> service, OpLogType opType) where T : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(JsonResultData.Failure("请选择要删除的数据！"));
                }
                if (id == "0")
                {
                    return Json(JsonResultData.Failure("要删除的数据不存在！"));
                }
                var result = service.BatchDelete(id, opType);
                if (result.Success)
                {
                    AfterDeleteCommit();
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        [AuthButton(AuthFlag.Delete)]
        public ActionResult _BatchDeleteGroup<T>(string id, EntityKeyDataType keyType, ICRUDService<T> service, OpLogType opType) where T : class, IBasicDataCopyEntity
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(JsonResultData.Failure("请选择要删除的数据！"));
                }
                if (id == "0")
                {
                    return Json(JsonResultData.Failure("要删除的数据不存在！"));
                }
                bool isGroupInGroup = CurrentInfo.IsGroupInGroup;
                var idArray = id.Split(',');
                var result = new List<object>();
                foreach (var idStr in idArray)
                {
                    if (!string.IsNullOrWhiteSpace(idStr))
                    {
                        var key = EntityKeyHelper.GetKeyValue(idStr, keyType);
                        var delete = service.Get(key);
                        //判断是否可以删除，都是只能删除本酒店自己创建的记录,不能删除的直接返回错误
                        if (delete.Hid == CurrentInfo.HotelId && (delete.DataSource == BasicDataDataSource.Added.Code || string.IsNullOrWhiteSpace(delete.DataSource)))
                        {
                            if (isGroupInGroup)
                            {
                                var deletedModels = service.DeleteGroupAndHotelCopied(delete);
                                result.AddRange(deletedModels);
                            }
                            else
                            {
                                service.Delete(delete);
                                result.Add(delete);
                            }
                        }
                        else
                        {
                            return Json(JsonResultData.Failure("只能删除本酒店自主增加的记录"));
                        }
                    }
                }
                service.AddDataChangeLog(opType);
                service.Commit();
                AfterDeleteGroupCommit(result);
                return Json(JsonResultData.Successed(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 在批量删除完成，并且提交成功后调用，由子类重写来实现在删除后的一些业务处理
        /// </summary>
        protected virtual void AfterDeleteCommit() { }

        protected virtual void AfterDeleteGroupCommit(List<object> deletedModels)
        {
        }

        #endregion 批量删除

        #region 操作记录

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="type">操作类型</param>
        /// <param name="text">操作内容</param>
        /// <param name="keys">关键字</param>
        public void AddOperationLog(OpLogType type, string text, string keys = "")
        {
            GetService<IoperationLog>().AddOperationLog(CurrentInfo.HotelId, type, text, CurrentInfo.UserName, Common.Extensions.UrlHelperExtension.GetRemoteClientIPAddress(), keys);
        }

        #endregion 操作记录

        #region 授权转换处理

        /// <summary>
        /// 授权信息转换为Html代码
        /// </summary>
        /// <param name="result">结果</param>
        /// <param name="viewName">视图名</param>
        public void AuthorizationInfoConvertToHtml(JsonResultData result, string viewName)
        {
            if (result != null && result.Success == false && result.ErrorCode == 4)//4为授权信息显示
            {
                result.Data = this.ControllerContext.RenderViewToString(viewName, result.Data);
            }
        }

        #endregion 授权转换处理

        #region 根据当前域名判断产品类型

        protected M_v_products GetProduct()
        {
            var accessDomain = Request.Headers["Host"];
            var domain = SharedSessionModule.GetLastThreeLevelDomain(accessDomain);
            var productService = GetService<IProductService>();
            return productService.GetProductByDomain(domain);
        }

        #endregion 根据当前域名判断产品类型

        #region 获取当前酒店类型（餐饮，快餐，零售）

        public string GetHotelServicesType()
        {
            var hotelService = GetService<IHotelInfoService>();
            var hotel = hotelService.ListValidHotels().Where(m => m.Hid == CurrentInfo.HotelId).FirstOrDefault();
            return hotel.CateringServicesType;
        }

        #endregion 获取当前酒店类型（餐饮，快餐，零售）

        #region Json序列化时间格式转换

        /// <summary>
        /// 将Json序列化后时间戳转换为"yyyy-MM-dd HH:mm:ss"格式
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string ReplaceJsonDateToDateString(string json)
        {
            if (json.IndexOf("\\/Date") > -1)
            {
                return Regex.Replace(json, @"\\/Date\((\d+)\)\\/", match =>
                {
                    DateTime dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                    dt = dt.ToLocalTime();
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
                });
            }
            else
            {
                return Regex.Replace(json, @"\/Date\((\d+)\)\/", match =>
                {
                    DateTime dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                    dt = dt.ToLocalTime();
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
                });
            }
        }

        #endregion Json序列化时间格式转换

        #region 获取当前操作对应的收银点以及银业点

        /// <summary>
        /// 获取当前操作员对应的收银点
        /// </summary>
        /// <returns></returns>
        public List<PosPos> GetUserPostId()
        {
            var service = GetService<IPmsUserService>();

            var user = service.Get(CurrentInfo.UserId);
            List<PosPos> result = null;
            if (user != null)
            {
                //判断操作员是否对应了收银点
                if (!string.IsNullOrEmpty(user.PosId))
                {
                    var posService = GetService<IPosPosService>();
                    var posArr = user.PosId.Split(',');
                    //循环设置的收银点取出收银点信息添加到list中
                    foreach (var posId in posArr)
                    {
                        var pos = posService.Get(posId);
                        result.Add(pos);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取当前操作员
        /// </summary>
        /// <param name="posId"></param>
        /// <returns></returns>
        public List<PosRefe> GetUserRefeId(string posId)
        {
            var service = GetService<IPmsUserService>();
            var user = service.Get(CurrentInfo.UserId);

            List<PosRefe> result = null;
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.RefeId))
                {
                    var refeService = GetService<IPosRefeService>();
                    var refeArr = user.RefeId.Split(',');
                    var refeList = refeService.GetRefeByPos(CurrentInfo.HotelId, posId, CurrentInfo.ModuleCode).ToList();
                    foreach (var id in refeArr)
                    {
                        var refe = refeList.Where(w => w.Id == id).FirstOrDefault();
                        if (refe != null)
                        {
                            result.Add(refe);
                        }
                    }
                }
            }
            return result;
        }
        #endregion
    }
}