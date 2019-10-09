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
using System.Web;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Controllers
{
    /// <summary>
    /// 会员消费记录列表
    /// </summary>
    [AuthPage("30001")]
    public class MbrMartketSmsManageController : BaseController
    {
        [AuthButton(AuthFlag.AddCardAuth)]
        public ActionResult Index(string para)
        {
            ViewBag.Para = para;
            return View();
        }
        [AuthButton(AuthFlag.AddCardAuth)]
        public JsonResult SendSms(string ids, string mobiles,string content)
        {
            var _mbrCardService = GetService<IMbrCardService>();
            var entity = _mbrCardService.SendMarketSms(ids,mobiles,content);
            if(entity.Success)
            {
                var count = ids.Split(',').Length;
                string opLog = "发送会员数：" + count + "，营销短信内容：" + content;
                AddOperationLog(OpLogType.会员营销短信, opLog);
            }
            return Json(entity);
        }
    }
}