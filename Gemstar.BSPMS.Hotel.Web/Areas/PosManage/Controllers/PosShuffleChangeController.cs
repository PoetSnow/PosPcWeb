using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosShuffleChange;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos换市别
    /// </summary>
    [AuthPage(ProductType.Pos, "p20009")]
    public class PosShuffleChangeController : BaseController
    {
        [AuthButton(AuthFlag.None)]
        public PartialViewResult Index()
        {
            var model = new ShuffleChangeViewModel();
            ViewBag.Version = CurrentVersion;
            return PartialView(model);
        }

        /// <summary>
        /// 修改市别
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult UpdateShuffle(PosRefe model)
        {
            var service = GetService<IPosRefeService>();
            var entity = service.Get(model.Id);

            if (entity != null)
            {
                try
                {
                    var oldEntity = new PosRefe { Id = entity.Id, ShuffleId = entity.ShuffleId };
                    entity.ShuffleId = model.ShuffleId;
                    AddOperationLog(OpLogType.Pos换市别, "名称：" + entity.Cname + "，当前市别：" + oldEntity.ShuffleId + " -> " + entity.ShuffleId, entity.Id);

                    service.Update(entity, new PosRefe());
                    service.Commit();

                    return Json(JsonResultData.Successed(""));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
            return Json(JsonResultData.Failure(""));
        }

        /// <summary>
        /// 根据营业点获取当前营业日和市别等信息
        /// </summary>
        /// <param name="refeid"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult GetShuffleChange(string refeid)
        {
            try
            {
                var server = GetService<IPosShuffleService>();
                var result = server.GetShuffleChange(CurrentInfo.HotelId, refeid);
                return Json(JsonResultData.Successed(result));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
    }
}