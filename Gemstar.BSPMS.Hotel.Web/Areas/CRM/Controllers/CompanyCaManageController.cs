using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Web.Areas.CRM.Controllers
{
    /// <summary>
    /// 合约单位账务
    /// </summary>
    [AuthPage("60030002")]
    public class CompanyCaManageController : BaseEditInWindowController<CompanyCa, ICompanyCaService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(string id)
        {
            ViewBag.CompanyId = id;
            return PartialView();
        }
        #endregion        
    }
}