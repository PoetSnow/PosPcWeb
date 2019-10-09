using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EntityProcedures;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Common;
using Gemstar.BSPMS.Hotel.Web.Models;
using Gemstar.BSPMS.Hotel.Web.Models.Query;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.Enums;
using System;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Web.Controllers
{
    [BusinessType("通用查询")]
    [Authorize]
    [NotAuth]
    public class QueryController : Controller
    {
        #region 生成查询条件界面
        /// <summary>
        /// 通用查询界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string procName, string procValues)
        {
            if (string.IsNullOrWhiteSpace(procName))
            {
                return Content("请指定查询用的存储过程");
            }
            var queryService = GetService<ICommonQueryService>();
            var parameters = new List<UpQueryProcedureParametersResult>();
            if (CurrentInfo != null && !string.IsNullOrWhiteSpace(CurrentInfo.HotelId) && !string.IsNullOrWhiteSpace(CurrentInfo.UserName) && !string.IsNullOrWhiteSpace(CurrentInfo.ShiftId))
            {
                parameters = queryService.GetProcedureParameters(procName, CurrentInfo.HotelId, CurrentInfo.UserName, CurrentInfo.ShiftId);
            }
            else
            {
                parameters = queryService.GetProcedureParameters(procName);
            }
            var model = new QueryModel
            {
                ProcedureParameters = parameters,
                ParameterValues = procValues
            };
            return PartialView("_Query", model);
        }

        public PartialViewResult Parameter(string parameterName, string procValues, string defaulValue, string displayParameterName, int index)
        {
            var model = new QueryViewModel
            {
                ParameterIndex = index,
                ParameterName = CommonQueryParameterHelper.GetDisplayName(parameterName)
            };
            if (!string.IsNullOrWhiteSpace(displayParameterName) && displayParameterName != model.ParameterName)
            {
                model.ParameterName = displayParameterName;//显示名称
            }
            //获取参数名称中的规则码和显示名称
            string codeValue;
            CommonQueryParameterHelper.GetCodeValue(parameterName, out codeValue);
            //获取上次查询时设置的值
            string parameterValue = "";
            if (!string.IsNullOrWhiteSpace(procValues))
            {
                var queryHelper = new CommonQueryHelper(procValues);
                parameterValue = queryHelper[parameterName];
            }

            string parameterText = "";
            if (!string.IsNullOrEmpty(parameterValue))
            {
                string[] valueAndText = parameterValue.Split('^');
                if (valueAndText.Length > 1)
                {
                    parameterValue = valueAndText[0];
                    parameterText = valueAndText[1];
                }
            }
            model.ParameterValue = parameterValue;
            model.ParameterValueText = parameterText;
            var viewName = "TextBox";
            if (string.IsNullOrWhiteSpace(procValues) && !string.IsNullOrWhiteSpace(defaulValue) && string.IsNullOrWhiteSpace(model.ParameterValue))
            {
                model.ParameterValue = defaulValue;//默认值
                parameterValue = model.ParameterValue;
            }

            var queryService = GetService<ICommonQueryService>();
            if (!(codeValue.StartsWith("d") || codeValue.StartsWith("e") || codeValue.StartsWith("m") || codeValue.StartsWith("n") || codeValue.StartsWith("k")))
            {
                List<SelectListItem> list = new List<SelectListItem>() { new SelectListItem() { Value = "", Text = "全部" } };
                switch (codeValue)
                {
                    case "h99":// 不显示:
                        viewName = "Hidden";
                        break;
                    case "p01"://日期选择
                        model.CustomViewPara = "yyyy-MM-dd";
                        viewName = "PickDate";
                        break;
                    case "p02"://日期时间选择
                        model.CustomViewPara = "yyyy-MM-dd HH:mm:ss";
                        viewName = "PickDatetime";
                        break;
                    case "p03"://日期时间选择
                        model.CustomViewPara = "MM-dd";
                        viewName = "PickDate";
                        break;
                    case "p05"://日期时间选择
                        model.CustomViewPara = "yyyy-MM";
                        viewName = "PickDate";
                        break;

                    case "s05"://下拉版本列表
                        var items = queryService.GetDropDownCodeAndNames(codeValue);
                        model.CustomViewPara = ToSelectListItems(items, parameterValue);
                        viewName = "DropDownList";
                        break;
                    //case 下拉物品类别:
                    //    model.CustomViewPara = AutoCompleteParameter.AutoCompleteParameterItemClassSingle(getControlInfo(index, model), AutoCompleteColSetting.AutoCompleteCol381);
                    //    viewName = "AutoComplete";
                    //    break;
                    case "c03"://复选框默认是
                        model.CustomViewPara = string.IsNullOrWhiteSpace(parameterValue) || parameterValue == "1";
                        viewName = "CheckBox";
                        break;
                    case "c04"://复选框默认否
                        model.CustomViewPara = parameterValue == "1";
                        viewName = "CheckBox";
                        break;
                    case "s08":
                        list.AddRange(EnumExtension.ToSelectList(typeof(EntityStatus), EnumValueType.Value, EnumValueType.Description));
                        model.CustomViewPara = list;
                        viewName = "DropDownList";
                        break;
                    case "s09"://付款类型
                        var itemsa = queryService.GetDropDownCodeAndNames(codeValue, "", "", CurrentInfo.HotelId);
                        model.CustomViewPara = ToSelectListItems(itemsa, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s17"://消费类型
                        var items17 = queryService.GetDropDownCodeAndNames(codeValue, "", "", CurrentInfo.HotelId);
                        model.CustomViewPara = ToSelectListItems(items17, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s10"://下拉版本列表
                        var itemsb = queryService.GetDropDownCodeAndNames(codeValue);
                        model.CustomViewPara = ToSelectListItems(itemsb, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s11"://积分项目键值对下列列表
                        var itemsc = GetService<IItemScoreService>().List(CurrentInfo.HotelId);
                        itemsc.Add(new KeyValuePair<string, string>("", "全部"));
                        model.CustomViewPara = ToSelectListItems(itemsc, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s12"://会员卡类型键值对下列列表
                        var itemsd = GetService<IMbrCardTypeService>().List(CurrentInfo.IsGroup ? CurrentInfo.GroupId : CurrentInfo.HotelId);
                        itemsd.Add(new KeyValuePair<string, string>("", "全部"));
                        model.CustomViewPara = ToSelectListItems(itemsd, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s13"://渠道编号
                        var itemse = queryService.GetDropDownCodeAndNames(codeValue);
                        model.CustomViewPara = ToSelectListItems(itemse, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s14"://会员账户类型
                        var itemsf = GetService<ICodeListService>().GetAccountType();
                        model.CustomViewPara = ToSelectListItems(itemsf, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s15"://房间类型
                        var roomtype = GetService<IRoomTypeService>().List(CurrentInfo.HotelId);
                        model.CustomViewPara = ToSelectListItems(roomtype, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s16"://班次
                        var shift = GetService<IShiftService>().List(CurrentInfo.HotelId);
                        model.CustomViewPara = ToSelectListItems(shift, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s18"://审核状态
                        var items18 = queryService.GetDropDownCodeAndNames(codeValue);
                        model.CustomViewPara = ToSelectListItems(items18, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s19"://业务员
                        var items19 = queryService.GetDropDownCodeAndNames(codeValue, "", "", CurrentInfo.HotelId);
                        model.CustomViewPara = ToSelectListItems(items19, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s20"://订单类型 
                        list.AddRange(EnumExtension.ToSelectList(typeof(ExtType), EnumValueType.Value, EnumValueType.Description));
                        model.CustomViewPara = list;
                        viewName = "DropDownList";
                        break;
                    case "s21"://发展来源
                        var items21 = queryService.GetDropDownCodeAndNames(codeValue);
                        model.CustomViewPara = ToSelectListItems(items21, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s22"://业主消费项目
                        var items22 = queryService.GetDropDownCodeAndNames(codeValue);
                        model.CustomViewPara = ToSelectListItems(items22, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s23"://有权限操作的分店下拉列表
                        var userService = GetService<IPmsUserService>();
                        var resorts = userService.GetResortListForOperator(CurrentInfo.GroupHotelId, CurrentInfo.UserId);
                        model.CustomViewPara = ToSelectListItems(resorts, parameterValue, showAll: false);
                        viewName = "DropDownList";
                        break;
                    case "a04"://云Pos 转并台报表
                        var items24 = queryService.GetDropDownCodeAndNames(codeValue);
                        model.CustomViewPara = ToSelectListItems(items24, parameterValue);
                        viewName = "MultiSelect";
                        break;
                    case "s24"://部门分类
                        var service = GetService<IPosDeptClassService>();
                        var datas = service.GetDeptClassByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
                        var items26 = datas.Select(w => new UpGetDropDownCodeAndNameResult { Code = w.Id, CodeName = w.Cname }).ToList();
                        items26.Insert(0, new UpGetDropDownCodeAndNameResult { Code = "", CodeName = "全部" });
                        model.CustomViewPara = ToSelectListItems(items26, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s25"://消费项目大类
                        var service1 = GetService<IPosItemClassService>();
                        var items25 = service1.GetPosItemClass(CurrentInfo.HotelId).Select(w => new UpGetDropDownCodeAndNameResult { Code = w.Id, CodeName = w.Cname }).ToList();
                        items25.Insert(0, new UpGetDropDownCodeAndNameResult { Code = "", CodeName = "全部" });
                        model.CustomViewPara = ToSelectListItems(items25, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s26"://消费项目状态
                        var codeservice = GetService<ICodeListService>();
                        var item26 = codeservice.GetPosStatus();
                        var listItems = item26.Select(w => new UpGetDropDownCodeAndNameResult { Code = w.code, CodeName = w.name }).ToList();
                        listItems.Insert(0, new UpGetDropDownCodeAndNameResult { Code = "", CodeName = "全部" });
                        model.CustomViewPara = ToSelectListItems(item26, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s27"://部门类别对应财务分类
                        var itemsS27 = queryService.GetDropDownCodeAndNames(codeValue, "", "", CurrentInfo.HotelId);
                        model.CustomViewPara = ToSelectListItems(itemsS27, parameterValue);
                        viewName = "MultiSelect";
                        break;
                    case "s28"://收入平衡表 统计分类
                        var itemS28 = queryService.GetDropDownCodeAndNames(codeValue);
                        model.CustomViewPara = ToSelectListItems(itemS28, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s29"://消费项目大类单选
                        var itemS29 = queryService.GetDropDownCodeAndNames(codeValue, "", "", CurrentInfo.HotelId);
                        model.CustomViewPara = ToSelectListItems(itemS29, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s30"://消费项目分类单选
                        var itemS30 = queryService.GetDropDownCodeAndNames(codeValue, "", "", CurrentInfo.HotelId);
                        model.CustomViewPara = ToSelectListItems(itemS30, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "s31"://消费项目分类多选
                        var itemsS31 = queryService.GetDropDownCodeAndNames(codeValue, "", "", CurrentInfo.HotelId);
                        model.CustomViewPara = ToSelectListItems(itemsS31, parameterValue);
                        viewName = "MultiSelect";
                        break;
                    case "s32"://操作日志操作类型
                        list.AddRange(EnumExtension.ToSelectList(typeof(OpLogType), EnumValueType.Text, EnumValueType.Text));
                        model.CustomViewPara = list.Where(s => s.Text.Contains("Pos")).ToList();
                        viewName = "DropDownList";
                        break;

                    case "r44"://云Pos 启用禁用
                        var items27 = queryService.GetDropDownCodeAndNames(codeValue);
                        model.CustomViewPara = ToSelectListItems(items27, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "r47"://云Pos 堂沽单报表类型
                        var items28 = queryService.GetDropDownCodeAndNames(codeValue);
                        model.CustomViewPara = ToSelectListItems(items28, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "r54"://云Pos 营业分析报表查询类型
                        var items29 = queryService.GetDropDownCodeAndNames(codeValue);
                        model.CustomViewPara = ToSelectListItems(items29, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "r61"://云Pos 推销员提成计算方式
                        var items30 = queryService.GetDropDownCodeAndNames(codeValue);
                        model.CustomViewPara = ToSelectListItems(items30, parameterValue);
                        viewName = "DropDownList";
                        break;
                    case "r62"://云Pos Pos 推销员提成报表排序方式
                        var items31 = queryService.GetDropDownCodeAndNames(codeValue);
                        model.CustomViewPara = ToSelectListItems(items31, parameterValue);
                        viewName = "DropDownList";
                        break;
                    default:
                        //普通文本框
                        viewName = "TextBox";
                        break;
                }
            }
            else
            {
                switch (codeValue.Substring(0, 1))
                {
                    case "d": //D:下拉
                        {
                            var itemse = queryService.GetDropDownCodeAndNames(codeValue.Substring(1), "", "", CurrentInfo.HotelId, parameterName);
                            model.CustomViewPara = ToSelectListItems(itemse, parameterValue);
                            viewName = "DropDownList";
                        }
                        break;
                    case "e"://E:带搜索条件的下拉
                        {
                            if (string.IsNullOrWhiteSpace(parameterValue))
                            {
                                model.ParameterValueText = "全部";
                            }
                            model.CustomViewPara = new List<SelectListItem>();
                            viewName = "DropDownListSearch";
                        }
                        break;
                    case "m"://M:下拉多选
                        {
                            var itemse = queryService.GetDropDownCodeAndNames(codeValue.Substring(1), "", "", CurrentInfo.HotelId, parameterName);
                            var allEntity = itemse.Where(c => (c.Code == "" && c.CodeName == "") || (c.Code == "" && c.CodeName == "全部") || (c.Code == "全部" && c.CodeName == "全部")).FirstOrDefault();
                            if (allEntity != null)
                            {
                                itemse.Remove(allEntity);
                            }
                            model.CustomViewPara = ToMultiSelectListItems(itemse, parameterValue);
                            viewName = "MultiSelect";
                        }
                        break;
                    case "n"://N:带搜索条件的下拉多选
                        {
                            if (string.IsNullOrWhiteSpace(parameterValue))
                            {
                                model.ParameterValueText = "全部";
                            }
                            System.Collections.IEnumerable selectedList = new List<string>();
                            selectedList = parameterValue.Split(',');
                            model.CustomViewPara = selectedList;
                            viewName = "MultiSelectSearch";
                        }
                        break;

                    case "k": // K 下拉多选，因为m的下拉多选已用完
                        var itemk = queryService.GetDropDownCodeAndNames(codeValue, "", "", CurrentInfo.HotelId, parameterName);
                        var allEntityk = itemk.Where(c => (c.Code == "" && c.CodeName == "") || (c.Code == "" && c.CodeName == "全部") || (c.Code == "全部" && c.CodeName == "全部")).FirstOrDefault();
                        if (allEntityk != null)
                        {
                            itemk.Remove(allEntityk);
                        }
                        model.CustomViewPara = ToMultiSelectListItems(itemk, parameterValue);
                        viewName = "MultiSelect";
                        break;
                    default:
                        viewName = "TextBox";
                        break;
                }
            }
            if (viewName == "PickDate")
            {
                var roleService = GetService<IUserRoleSingleService>();
                //var a=roleService.Get("");
                var currentInfo = GetService<ICurrentInfo>();
                var hid = currentInfo.HotelId;
                var history = roleService.isroleofhistory(hid, currentInfo.UserId);
                ViewBag.ishebid = history;
            }
            return PartialView("_" + viewName, model);
        }


        public JsonResult ParameterSearch(string parameterName, string parameterValue, string parameterValueText)
        {
            IEnumerable<SelectListItem> resultList = new List<SelectListItem>();
            var search = Request.QueryString.Get("filter[filters][0][value]");//查询参数
            if (string.IsNullOrWhiteSpace(search))
            {
                search = parameterValueText;
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                string codeValue;//获取参数名称中的规则码和显示名称
                CommonQueryParameterHelper.GetCodeValue(parameterName, out codeValue);
                if (!string.IsNullOrWhiteSpace(codeValue))
                {
                    if (codeValue.Substring(0, 1) == "e")
                    {
                        var itemse = GetService<ICommonQueryService>().GetDropDownCodeAndNames(codeValue.Substring(1), "", search, CurrentInfo.HotelId, parameterName);
                        resultList = ToSelectListItems(itemse, parameterValue);
                    }
                    else if (codeValue.Substring(0, 1) == "n")
                    {
                        string[] searchList = search.Split(',');
                        var queryService = GetService<ICommonQueryService>();
                        List<UpGetDropDownCodeAndNameResult> CodeAndNames = new List<UpGetDropDownCodeAndNameResult>();
                        foreach (var item in searchList)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                CodeAndNames.AddRange(queryService.GetDropDownCodeAndNames(codeValue.Substring(1), "", item, CurrentInfo.HotelId, parameterName));
                            }
                        }
                        for (int i = 1; i < CodeAndNames.Count; i++)
                        {
                            if (CodeAndNames[i].CodeName == "全部")
                            {
                                CodeAndNames.Remove(CodeAndNames[i]);
                                i--;
                            }
                        }
                        resultList = ToMultiSelectListItems(CodeAndNames, parameterValue);
                    }
                }
            }
            return Json(resultList, JsonRequestBehavior.AllowGet);
        }
        private static IEnumerable<SelectListItem> ToSelectListItems(List<UpGetDropDownCodeAndNameResult> items, string selectedValue, bool showAll = true)
        {
            var result = new List<SelectListItem>();
            foreach (var codeAndNameResult in items)
            {
                if (showAll == false && string.IsNullOrEmpty(codeAndNameResult.Code))
                {
                    continue;
                }
                var item = new SelectListItem { Value = codeAndNameResult.Code, Text = codeAndNameResult.CodeName };
                if (codeAndNameResult.Code.Equals(selectedValue))
                {
                    item.Selected = true;
                }
                result.Add(item);
            }
            return result;
        }
        private static IEnumerable<SelectListItem> ToSelectListItems(List<KeyValuePair<string, string>> items, string selectedValue, bool showAll = true)
        {
            var result = new List<SelectListItem>();
            foreach (var codeAndNameResult in items)
            {
                if (showAll == false && string.IsNullOrEmpty(codeAndNameResult.Key))
                {
                    continue;
                }
                var item = new SelectListItem { Value = codeAndNameResult.Key, Text = codeAndNameResult.Value };
                if (codeAndNameResult.Key.Equals(selectedValue))
                {
                    item.Selected = true;
                }
                result.Add(item);
            }
            return result;
        }
        private static IEnumerable<SelectListItem> ToSelectListItems(List<Services.Entities.V_codeListPub> items, string selectedValue, bool showAll = true)
        {
            var result = new List<SelectListItem>();
            foreach (var codeAndNameResult in items)
            {
                if (showAll == false && string.IsNullOrEmpty(codeAndNameResult.code))
                {
                    continue;
                }
                var item = new SelectListItem { Value = codeAndNameResult.code, Text = codeAndNameResult.name };
                if (codeAndNameResult.code.Equals(selectedValue))
                {
                    item.Selected = true;
                }
                result.Add(item);
            }
            return result;
        }
        private object ToSelectListItems(List<UpQueryResortListForOperatorResult> resorts, string selectedValue, bool showAll)
        {
            var result = new List<SelectListItem>();
            var item = new SelectListItem { Value = "", Text = "全部" };
            item.Selected = true;
            result.Add(item);
            foreach (var resort in resorts)
            {
                if (showAll == false && string.IsNullOrEmpty(resort.Hid))
                {
                    continue;
                }
                item = new SelectListItem { Value = resort.Hid, Text = resort.Hname };
                //if (resort.Hid.Equals(selectedValue))
                //{
                //    item.Selected = true;
                //}
                result.Add(item);
            }
            return result;
        }
        private static IEnumerable<SelectListItem> ToMultiSelectListItems(List<UpGetDropDownCodeAndNameResult> items, string selectedValue, bool showAll = true)
        {
            var result = new List<SelectListItem>();
            string[] selectedValueList = selectedValue.Split(',');
            foreach (var codeAndNameResult in items)
            {
                if (showAll == false && string.IsNullOrEmpty(codeAndNameResult.Code))
                {
                    continue;
                }
                var item = new SelectListItem { Value = codeAndNameResult.Code, Text = codeAndNameResult.CodeName };
                foreach (var value in selectedValueList)
                {
                    if (codeAndNameResult.Code.Equals(value))
                    {
                        item.Selected = true;
                    }
                }
                result.Add(item);
            }
            return result;
        }
        #endregion

        #region 执行ajax数据查询
        [KendoGridDatasourceException]
        public ActionResult AjaxQuery(CommonQueryModel query, [DataSourceRequest]DataSourceRequest request)
        {
            var queryService = GetService<ICommonQueryService>();
            var procedure = query.QueryProcedureName;
            var queryParameters = new CommonQueryHelper(query.QueryParameterValues);
            queryParameters.SetHiddleParaValuesFromCurrentInfo(CurrentInfo, queryService.GetProcedureParameters(query.QueryProcedureName));
            var paraValues = queryParameters.GetParameters();
            var dt = queryService.ExecuteQuery(procedure, paraValues);
            var data = Json(dt.ToDataSourceResult(request));
            data.MaxJsonLength = int.MaxValue;
            return data;
        }
        #endregion

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
        #endregion

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
        #endregion

    }
}