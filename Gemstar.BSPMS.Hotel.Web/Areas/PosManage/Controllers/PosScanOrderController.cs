using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{

    /// <summary>
    /// Pos扫码点餐开关
    /// </summary>
    [AuthPage(ProductType.Pos, "p200012")]
    public class PosScanOrderController : BaseController
    {
        [AuthButton(AuthFlag.Update)]
        public ActionResult Index()
        {
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);
            if (pos != null)
            {
                if (pos.IsBrushOrder == true)
                {
                    pos.IsBrushOrder = false;
                }
                else
                {
                    pos.IsBrushOrder = true;
                }
                posService.Update(pos, new PosPos());
                posService.AddDataChangeLog(Common.Services.Enums.OpLogType.Pos收银点修改);
                posService.Commit();
                return Json(JsonResultData.Successed());
            }
            else
            {
                return Json(JsonResultData.Failure(""));
            }
        }
    }
}