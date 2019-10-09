using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos餐台资料
    /// </summary>
    [AuthPage(ProductType.Pos, "p99115001")]
    public class BasicDataPosMTabController : BaseEditInWindowController<PosTab, IPosTabService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult ListTabByHid([DataSourceRequest]DataSourceRequest request, string code = "", string name = "")
        {
            var domain = Request.Url.Host;
            var index = domain.IndexOf("pos");
            domain = domain.Substring(index);

            var service = GetService<IPosTabService>();
            var list = service.GetPosTabByHid(CurrentInfo.HotelId, code, name);
            var centerDb = GetService<DbCommonContext>();
            var smHotelList = centerDb.posSmMappingHids.ToList();
            var smHotel = smHotelList.Where(w => w.HId == CurrentInfo.HotelId).FirstOrDefault();
            var smHid = smHotel == null ? "" : smHotel.HotelCode ?? "";
            foreach (var temp in list)
            {

                //temp.OrderBarcode = "http://"+domain + temp.OrderBarcode;
                temp.OrderBarcode = "http://scanorder.gshis.com/ScanOrderTransfer/?Hid=" + smHid + "&tabid=" + temp.Id;
            }

            return Json(list.ToDataSourceResult(request));
        }
    }
}