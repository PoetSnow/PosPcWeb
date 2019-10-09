using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos换班次
    /// </summary>
    [AuthPage(ProductType.Pos, "p20007")]
    public class PosShiftChangeController : BaseController
    {
        [AuthButton(AuthFlag.None)]
        public PartialViewResult Index()
        {
            var server = GetService<IPosPosService>();
            var model = server.GetShiftChange(CurrentInfo.HotelId, CurrentInfo.PosId);
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
        public ActionResult UpdateShift(PosPos model)
        {
            var service = GetService<IPosPosService>();
            var entity = service.Get(model.Id);

            if (entity != null)
            {
                try
                {
                    var oldEntity = new PosPos { Id = entity.Id, ShiftId = entity.ShiftId };
                    entity.ShiftId = model.ShiftId;
                    AddOperationLog(OpLogType.Pos换班次, "名称：" + entity.Name + "，当前班次：" + oldEntity.ShiftId + " -> " + entity.ShiftId, entity.Id);

                    service.Update(entity, new PosPos());
                    service.Commit();
                    //更换班次把班次信息存储到缓存中 snow 2018年11月27日15:10:48
                    var _currentInfo = GetService<ICurrentInfo>();
                    _currentInfo.ShiftId = entity.Id;
                    _currentInfo.ShiftName = entity.Name;
                    _currentInfo.SaveValues();
                    return Json(JsonResultData.Successed(""));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
            return Json(JsonResultData.Failure(""));
        }
    }
}