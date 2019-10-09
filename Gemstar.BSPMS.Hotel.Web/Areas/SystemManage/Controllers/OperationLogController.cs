using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    [AuthPage("40020")]
    [AuthPage(ProductType.Member, "m40020")]
    [AuthPage(ProductType.Pos, "p50020")]
    public class OperationLogController:BaseController
    {
        // GET: SystemManage/OperationLog
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            string SaveOpLogDays = GetService<IPmsParaService>().GetValue(CurrentInfo.HotelId, "SaveOpLogDays");
            ViewBag.SaveOpLogDays = SaveOpLogDays;
            ViewBag.currentbsday = System.DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }
        [AuthButton(AuthFlag.Query)]
        [KendoGridDatasourceException]
        public ActionResult IndexAjax([DataSourceRequest]DataSourceRequest request, ResLogQueryPara queryPara)
        {
            queryPara.HotelId = CurrentInfo.HotelId;
            var services = GetService<IoperationLog>();
            var orderDetails = services.GetOperationLog(queryPara);
            var result= Json(orderDetails.ToDataSourceResult(request));
            return result;
        }
    }
}