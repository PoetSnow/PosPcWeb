using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosMBanner;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos扫码点餐Banner
    /// </summary>
    [AuthPage(ProductType.Pos, "p99115003")]
    public class BasicDataPosMBannerController : BaseEditInWindowController<PosMBanner, IPosMBannerService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            return View();
        }

        #region 增加

        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            ViewBag.Domain = "http://res.gshis.com/";
            return _Add(new PosMBannerAddViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosMBannerAddViewModel addViewModel)
        {
            var modelService = GetService<IPosMBannerService>();
            ActionResult result = _Add(addViewModel, new PosMBanner { Id = Guid.NewGuid(), Hid = CurrentInfo.HotelId, Creator = CurrentInfo.UserName, Createdate = DateTime.Now }, OpLogType.Pos扫码点餐Banner添加);
            return result;
        }

        #endregion 增加

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(Guid id)
        {
            ViewBag.Domain = "http://res.gshis.com/";
            return _Edit(id, new PosMBannerEditViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosMBannerEditViewModel model)
        {
            var modelService = GetService<IPosMBannerService>();

            ActionResult result = _Edit(model, new PosMBanner(), OpLogType.Pos扫码点餐Banner修改);
            return result;
        }

        #endregion 修改

        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosMBannerService>(), OpLogType.Pos扫码点餐Banner删除);
        }

        #endregion 批量删除

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListMBannerByHid([DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosMBannerService>();
            var list = service.GetMBannerList(CurrentInfo.HotelId);
            return Json(list.ToDataSourceResult(request));
        }
    }
}