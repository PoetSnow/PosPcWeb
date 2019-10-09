using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.CRM.Controllers
{
    [AuthPage("60030002001")]
    public class CompanyImgManageController : BaseController
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Add(string id)
        {
            ViewBag.Domain = "http://res.gshis.com/";//这里为固定的  jxd-pmshelpfiles资源下的域名
            ViewBag.isadd = IsHasAuth("60030002001", 2) ? 0 : 1;//添加图片权限
            ViewBag.companyid = id;
            return PartialView();
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="companyid"></param>
        /// <param name="request"></param>
        /// <returns></returns>        
        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.Query)]
        public ActionResult Read(string companyid, [DataSourceRequest] DataSourceRequest request)
        {
            var cinfo = GetService<ICurrentInfo>();
            var service = GetService<ICompanyService>();
            var resultList = service.getCompanySignImg(cinfo.HotelId, Guid.Parse(companyid));
            return Json(resultList.ToDataSourceResult(request));
        }
        /// <summary>
        /// 添加图片
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="companyid"></param>
        /// <returns></returns>
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public JsonResult AddImage(string name, string url,string companyid)
        {
            var cinfo = GetService<ICurrentInfo>();
            var service = GetService<ICompanyService>();
            var result = service.AddCompanyImage(cinfo.HotelId, Guid.Parse(companyid),name,url);
            return Json(result);
        }
        /// <summary>
        /// 查看大图
        /// </summary>
        /// <param name="picLink"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult Detail(string picLink, object r)
        {
            return Content("<img src='" + picLink + "' alt='链接不正确，图片加载失败' />");
        }
        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="src"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Delete)]
        public JsonResult DeleteImage(string src, int id)
        {
            QiniuController qiniu = new QiniuController();
            var result = qiniu.QiniuDelete(src, false);
            var service = GetService<ICompanyService>();
            var data = service.DelCompanyImage(id);
            if (data.Success)
                return Json(new JsonResultData { Success = true }, JsonRequestBehavior.DenyGet);
            else
                return Json(new JsonResultData { Success = false, Data = "删除失败" }, JsonRequestBehavior.DenyGet);
        }
    }
}