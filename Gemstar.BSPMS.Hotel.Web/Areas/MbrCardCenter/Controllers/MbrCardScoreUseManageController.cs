using Gemstar.BSPMS.Common.Enumerator;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MbrCardCenter;
using Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Controllers
{
    /// <summary>
    /// 会员积分兑换记录列表
    /// </summary>
    [AuthPage("30001")]
    public class MbrCardScoreUseManageController : BaseEditInWindowController<MbrCard, IMbrCardService>
    {
        [AuthButton(AuthFlag.IntegrarChRecord)]
        public ActionResult Index(string profileId)
        {
            ViewBag.IsGroup = CurrentInfo.IsGroup;
            SetCommonQueryValues("up_list_profileScoreUseList", "@h99profileid=" + profileId);
            return View();
        }
    }
}