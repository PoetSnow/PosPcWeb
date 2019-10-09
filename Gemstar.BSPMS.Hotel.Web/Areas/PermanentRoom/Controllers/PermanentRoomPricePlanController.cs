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

namespace Gemstar.BSPMS.Hotel.Web.Areas.PermanentRoom.Controllers
{
    /// <summary>
    /// 长租房价
    /// </summary>
    [AuthPage("21050")]
    public class PermanentRoomPricePlanController : BaseController
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(string roomTypeId)
        {
            SetCommonQueryValues("up_list_PermanentRoomPricePlan", "@t00RoomTypeId=" + roomTypeId);
            return View();
        }
        #endregion

        #region 修改
        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<Models.PermanentRoomPricePlan> updatedVersions, [Bind(Prefix = "originModels")]IEnumerable<Gemstar.BSPMS.Hotel.Web.Areas.PermanentRoom.Models.PermanentRoomPricePlan> originVersions)
        {
            string hid = CurrentInfo.HotelId;
            var values = updatedVersions.Select(c => new PermanentRoomPricePlan { Hid = hid, Roomid = c.Roomid, RoomPriceByDay = c.RoomPriceByDay, RoomPriceByMonth = c.RoomPriceByMonth }).ToList();
            GetService<IPermanentRoomPricePlanService>().AddOrUpdatePrice(hid, values);
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
            var result = _queryService.ExecuteQuery<Models.PermanentRoomPricePlan>(procedure, paraValues);
            return Json(result.ToDataSourceResult(request));
        }
        #endregion



    }

    /// <summary>
    /// 长租房房价-房间类型列表
    /// </summary>
    [AuthPage("21050")]
    public class PermanentRoomTypeController : BaseEditIncellController<RoomType, IRoomTypeService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            return View();
        }
    }



}