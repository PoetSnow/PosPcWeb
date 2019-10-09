using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services;
using System;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Web.Areas.CRM.Controllers
{
    /// <summary>
    /// 客户消费明细查询
    /// </summary>
    [AuthPage("60020")]
    [BusinessType("客户消费明细查询")]
    public class GuestTransManageController : BaseEditInWindowController<GuestTrans, IGuestTransService>
    {
        #region 查询
        // GET: SystemManage/PayWay
        [AuthButton(AuthFlag.Consume)]
        public ActionResult Index(string id, string begintime, string endtime)  
        {
            ViewData["khid"] = id;
            ViewData["begintime"] = begintime;
            ViewData["endtime"] = endtime;
            SetCommonQueryValues("up_list_GuestTrans", "@t00客户id=" + Guid.Parse(id) + "&@p01开始时间=" + DateTime.Parse(begintime) + "&@p01结束时间=" + DateTime.Parse(endtime));

            return View();


         }
        #endregion               
    }
}