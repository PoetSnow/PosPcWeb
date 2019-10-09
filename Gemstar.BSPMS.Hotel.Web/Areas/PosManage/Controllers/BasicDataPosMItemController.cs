using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosMItem;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos扫码点餐Banner
    /// </summary>
    [AuthPage(ProductType.Pos, "p99115005")]
    public class BasicDataPosMItemController : BaseEditInWindowController<PosItem, IPosItemService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_PosMItem", "");
            return View();
        }

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            ViewBag.Domain = "http://res.gshis.com/";
            return _Edit(id, new PosMItemEditViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosMItemEditViewModel model)
        {
            var modelService = GetService<IPosItemService>();

            ActionResult result = _Edit(model, new PosItem(), OpLogType.Pos扫码点餐Banner修改);
            return result;
        }

        #endregion 修改

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListMItemByHid([DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosItemService>();
            var list = service.GetPosItemByShowSet(CurrentInfo.HotelId, PosShowSet.Mobile.ToString());
            return Json(list.ToDataSourceResult(request));
        }
    }
}