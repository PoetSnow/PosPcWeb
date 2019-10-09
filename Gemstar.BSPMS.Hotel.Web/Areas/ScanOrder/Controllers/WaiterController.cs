using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Controllers
{
    /// <summary>
    /// 服务员端控制器
    /// </summary>
    public class WaiterController : Controller
    {

        /// <summary>
        /// 用户登录界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
           
            return View();
        }

        public ActionResult Login(LoginViewModel model)
        {
            //登录成功要判断
            return View();
        }

        /// <summary>
        /// 餐台列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "餐台列表";
            return View();
        }


        public ActionResult OrderList()
        {
            ViewBag.Title = "订单列表";


            var refeId = "000388001";
            return View();
        }


    }
}