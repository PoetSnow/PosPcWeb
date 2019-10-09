using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.PermanentRoomManage;
using Gemstar.BSPMS.Hotel.Web.Models;
using Gemstar.BSPMS.Common.Tools;
using Kendo.Mvc.UI;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Common;
using Kendo.Mvc.Extensions;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Percentages.Controllers
{
    /// <summary>
    /// 房间打扫政策
    /// </summary>
    [AuthPage("99065")]
    public class CleanRoomPolicyController : BaseController
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(string roomTypeId)
        {
            if (!GetService<IPmsParaService>().IsDirtyLogToReportForm(CurrentInfo.HotelId))
            {
                return Content("请先开通[脏房转净房生成报表]");
            }
            SetCommonQueryValues("up_list_percentagesPolicyCleanRoom","");
            return View();
        }
        #endregion

        #region 修改
        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<Models.CleanRoomPolicy.CleanRoomPolicyEditViewModel> updatedVersions, [Bind(Prefix = "originModels")]IEnumerable<Models.CleanRoomPolicy.CleanRoomPolicyEditViewModel> originVersions)
        {
            string hid = CurrentInfo.HotelId;
            var values = updatedVersions.Select(c => new Gemstar.BSPMS.Hotel.Services.Entities.PercentagesPolicyCleanRoom { RoomTypeId = c.RoomTypeId,  ContinuedToLivePrice = c.ContinuedToLivePrice,  CheckOutPrice = c.CheckOutPrice, PolicyDesciption = c.PolicyDesciption }).ToList();
            GetService<Services.Percentages.ICleanRoomPolicyService>().AddOrUpdatePrice(hid, values);
            return Json(updatedVersions.ToDataSourceResult(request));
        }
        #endregion

        #region 执行ajax数据查询
        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.Query)]
        public ActionResult AjaxQuery(CommonQueryModel query, [DataSourceRequest]DataSourceRequest request)
        {
            var queryService = GetService<ICommonQueryService>();
            var procedure = query.QueryProcedureName;
            var queryParameters = new CommonQueryHelper(query.QueryParameterValues);
            queryParameters.SetHiddleParaValuesFromCurrentInfo(CurrentInfo, queryService.GetProcedureParameters(query.QueryProcedureName));
            var paraValues = queryParameters.GetParameters();
            var _queryService = GetService<ICommonQueryService>();
            var result = _queryService.ExecuteQuery<Gemstar.BSPMS.Hotel.Web.Areas.Percentages.Models.CleanRoomPolicy.CleanRoomPolicyListViewModel>(procedure, paraValues);
            return Json(result.ToDataSourceResult(request));
        }
        #endregion
    }
}