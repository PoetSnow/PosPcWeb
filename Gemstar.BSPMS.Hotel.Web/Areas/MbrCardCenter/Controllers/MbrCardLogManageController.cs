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
    /// 会员变更记录列表
    /// </summary>
    [AuthPage("30001")]
    public class MbrCardLogManageController : BaseEditInWindowController<MbrCard, IMbrCardService>
    {
        [AuthButton(AuthFlag.UpdateRecord)]
        public ActionResult Index(string profileId)
        {
            SetCommonQueryValues("up_list_profileLogList", "@h99profileid=" + profileId + "&@t00变更类型=换卡号");
            return View();
        }
    }
}