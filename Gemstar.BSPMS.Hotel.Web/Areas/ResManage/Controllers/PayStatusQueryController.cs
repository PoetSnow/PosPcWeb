using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Controllers
{
    /// <summary>
    /// 待支付记录列表
    /// </summary>
    [AuthPage("20030")]
    public class PayStatusQueryController : BaseController
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_ResFolioPayInfo", "@p02开始时间=" + DateTime.Now.AddHours(-2) + "&@p02结束时间=" + DateTime.Now);
            return View();
        }
    }
}