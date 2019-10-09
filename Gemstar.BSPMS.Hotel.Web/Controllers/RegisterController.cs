using System;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Web.Models;
using Gemstar.BSPMS.Hotel.Web.Models.Register;
using GemStarSecurity;

namespace Gemstar.BSPMS.Hotel.Web.Controllers
{
    [NotAuth]
    public class RegisterController : Controller
    {
        public ActionResult RegAlter()
        {
            ViewData["errorMessage"] = ProgVersion.DogErrorMessage;
            ProgVersion.DogErrorMessage = "";
            return View();
        }

        public ActionResult RegisterAlter()
        {
            ViewData["errorMessage"] = ProgVersion.DogErrorMessage;
            return View();
        }
        public ActionResult Register()
        {
            var info = new RegInfoForLength20();
            var connStr = MvcApplication.GetCenterDBConnStr();
            var dataAccess = new Gemstar.Data.DataAccess(connStr, Gemstar.Data.DbType.SqlServer);
            var register = new CstRegister(dataAccess, ProgVersion.BeginGroup, GemstarSystems.Hmscy, NetDogAccessMethod.CloudCheckMacOnly);
            info.UserSeriesNo = register.GetUserSeriesNoViaServerNetAddress()[0]+ "HOTEL";
            dataAccess.ExecuteNonQuery("UPDATE cyuserinfo SET v_seriesno='"+info.UserSeriesNo+"'");
            return View(info);
        }
        
        [HttpPost]
        public ActionResult Register(RegInfoForLength20 info)
        {
            //检查值
            if (ModelState.IsValid)
            {
                var connStr = MvcApplication.GetCenterDBConnStr();
                var dataAccess = new Gemstar.Data.DataAccess(connStr, Gemstar.Data.DbType.SqlServer);
                try
                {
                    var regno = info.RegNo;
                    var register = new CstRegister(dataAccess, ProgVersion.BeginGroup, GemstarSystems.Hmscy, NetDogAccessMethod.CloudCheckMacOnly);
                    register.SetRegInfo(regno, false);
                    HttpContext.Application["checkStatus"] = null;
                    ProgVersion.CheckServerDog();
                    if (ProgVersion.IsDogOk)
                        return RedirectToAction("Index", "Account");
                    ModelState.AddModelError("no1", "注册成功，但验证注册信息失败，原因：" + ProgVersion.DogErrorMessage);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("no1", "注册失败！原因如下：" + ex.Message);
                }
            }
            return View(info);
        }
    }
}