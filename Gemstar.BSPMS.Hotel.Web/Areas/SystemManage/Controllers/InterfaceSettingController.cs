using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Common.Tools;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 接口设置
    /// </summary>
    [AuthPage("99090")]
    [AuthPage(ProductType.Member, "m99060")]
    [AuthPage(ProductType.Pos, "p99060")]
    public class InterfaceSettingController : BaseController
    {
        [AuthButton(AuthFlag.Update)]
        public ActionResult Index()
        {
            var hotelServices = GetService<IHotelInfoService>();
            var hotelInterfaces = hotelServices.GetHotelInterface(CurrentInfo.HotelId); 
            return View(hotelInterfaces);
        }
        /// <summary>
        /// 由于以tab方式加载内容后，所有的内容都是合并在一起的，这样不行，需要使用iframe来区分开，否则之前做好封装的那些通用查询等全部都使用不了
        /// </summary>
        /// <param name="typeCode">硬件类型代码</param>
        /// <param name="code">硬件型号代码</param>
        /// <param name="name">硬件型号名称</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult TabstripContent(string typeCode,string code,string name)
        {
            ViewBag.typeCode = typeCode;
            ViewBag.code = code;
            ViewBag.name = name; 
            return PartialView("_TabstripContent");
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult Errowpage()
        { 
            return PartialView("Errowpage");
        }
        /// <summary>
        /// 下载硬件接口版本
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public string DownloadFile(string  versionId)
        {

            if (string.IsNullOrWhiteSpace(versionId))
                versionId = "interface";
            var hotelServices = GetService<IHotelInfoService>();
            var result = hotelServices.GetDownloadFile(versionId);
            if (result!=null) {
                var piclink = result.PicLink;
                return  piclink;
            }
            return null;
       
        }
    }
}