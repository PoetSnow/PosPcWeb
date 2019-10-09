using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models.Home;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos换收银点
    /// </summary>
    [AuthPage(ProductType.Pos, "p20015")]
    public class PosPosChangeController : BaseController
    {
        [AuthButton(AuthFlag.None)]
        public PartialViewResult Index()
        {
            var model = new SelectPosViewModel();
            //给model赋值
            model.CurrentPosId = CurrentInfo.PosId;
            model.CurrentPosName = CurrentInfo.PosName;
            ViewBag.Version = CurrentVersion;
            return PartialView(model);
        }

        /// <summary>
        /// 修改班次
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult UpdatePos(SelectPosViewModel model)
        {
            var _currentInfo = GetService<ICurrentInfo>();
            _currentInfo.PosId = model.CurrentPosId;
            _currentInfo.PosName = model.CurrentPosName;
            _currentInfo.SaveValues();
            return Json(JsonResultData.Successed(""));
        }
    }
}