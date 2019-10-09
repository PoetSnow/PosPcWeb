using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Kendo.Mvc.Extensions;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 通用代码设置
    /// </summary>
    [AuthPage("99070")]
    [AuthPage(ProductType.Member, "m99035")]
    [AuthPage(ProductType.Pos, "p99035")]
    public class CodeTypeController : BaseEditIncellController<CodeType, ICodeTypeService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            return View();
        }
    }
}