using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Gemstar.BSPMS.Hotel.Web.Models;
using Gemstar.BSPMS.Hotel.Services.RoomStatusManage;
using Kendo.Mvc.Extensions;

namespace Gemstar.BSPMS.Hotel.Web.Areas.RoomState.Controllers
{
    /// <summary>
    /// 房态表
    /// </summary>
    [AuthPage("10003")]
    public class TableController : BaseController
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            return View();
        } 
        [AuthButton(AuthFlag.Query)]
        [KendoGridDatasourceException]
        public ActionResult AjaxQuery([DataSourceRequest]DataSourceRequest request)
        {
            var services = GetService<IRoomStatusService>();
            var roomStatus = services.GetCurrentRoomStatusGroupByRoomType(CurrentInfo.HotelId);
            return Json(roomStatus.ToDataSourceResult(request));
        }
        #endregion
    }
}