using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using System;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Gemstar.BSPMS.Hotel.Web.Models;
using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Hotel.Web.Models.HelpFileModels;
using System.Collections.Generic;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Web.Controllers
{
    [NotAuth]
    public class HelpFilesController : BaseController
    {
        // GET: HelpFiles
        public ActionResult Index(HelpFileIndexModel model)
        {
            var service = GetService<IHelpFilesService>();
            var result = service.GetHelpFiles(model.menuId);
            if (CurrentInfo.HotelId != "000000")
                result = result.Where(w => w.CheckStatus).ToList();
            if (model.helpId.HasValue)
                model.helpFile = result.Where(w => w.Id == model.helpId.Value).FirstOrDefault();
            if (model.helpFile == null)
            {
                model.helpFile = result.FirstOrDefault();
            }

            model.allHelpsInMenu = result;
            model.HasAuthToAdd = CurrentInfo.HotelId == "000000";

            if (model.helpFile != null)
            {
                //这里的model.menuId是不变的，始终是第一次进来的menuid,所以不用赋值，始终不变

                //model.menuId = model.helpFile.MenuId;
                //model.menuName = model.helpFile.MenuName;
                //增加阅读次数
                service.AddReadQty(model.helpFile.Id);
            }
            //取出bin目录下面的程序最后编译时间
            var createTime = System.IO.File.GetLastWriteTime(Server.MapPath("~/bin/Gemstar.BSPMS.Hotel.Web.dll"));
            ViewBag.createTime = createTime.ToString("yyyy-MM-dd HH:mm");

            return View(model);
        }
        public ActionResult Add(HelpFileIndexModel model)
        {
            ViewBag.menuId = model.menuId;
            ViewBag.menuName = model.menuName;
            ViewBag.Domain = "http://res.gshis.com/";//这里为固定的  jxd-pmshelpfiles资源下的域名
            return View(model);
        }
        public ActionResult Edit(int helpId, string menuId, string menuName)
        {
            ViewBag.menuId = menuId;
            ViewBag.menuName = menuName;
            ViewBag.Domain = "http://res.gshis.com/";//这里为固定的  jxd-pmshelpfiles资源下的域名
            var service = GetService<IHelpFilesService>();
            var helpFile = service.Get(helpId);
            var model = new HelpFileIndexModel
            {
                helpId = helpId,
                allHelpsInMenu = new List<HelpFiles>(),
                HasAuthToAdd = CurrentInfo.HotelId == "000000",
                helpFile = helpFile,
                menuId = helpFile.MenuId,
                menuName = helpFile.MenuName
            };
            return View("Add", model);
        }
        [JsonException]
        public JsonResult Delete(int helpId)
        {
            var service = GetService<IHelpFilesService>();
            service.Delete(new HelpFiles { Id = helpId });
            service.Commit();
            return Json(JsonResultData.Successed(""));
        }
        [KendoGridDatasourceException]
        public ActionResult Read(string name, [DataSourceRequest] DataSourceRequest request)
        {
            var service = GetService<IHelpFilesService>();
            var resultList = service.GetHelpFilesImg(null, name);
            return Json(resultList.ToDataSourceResult(request));
        }
        [JsonException]
        public JsonResult AddImage(string name, string url, int? helpId)
        {
            var service = GetService<IHelpFilesService>();
            var result = service.AddHelpFileImage(name, url, helpId);
            return Json(result);
        }
        public JsonResult SaveData(HelpFiles help)
        {
            help.ReadNumber = 1;
            help.UpdateDate = DateTime.Now;
            help.UpdateUser = CurrentInfo.UserName;
            help.CheckUser = CurrentInfo.UserName;
            help.CheckStatus = false;
            help.AddDate = DateTime.Now;
            help.AddUser = CurrentInfo.UserName;
            var service = GetService<IHelpFilesService>();
            var result = service.SaveHelpFiles(help);
            return Json(result);
        }
        public JsonResult DeleteImage(string src, int id)
        {
            QiniuController qiniu = new QiniuController();
            var service = GetService<IHelpFilesService>();
           if(src.Contains("res.gshis.com"))
            {
                var result = qiniu.QiniuDelete(src, false);
            }
            else
            {
                var result = qiniu.QiniuDelete(src, true);
            }    
            var data = service.DeleteFileImage(id);
            if (data.Success)
                return Json(new JsonResultData { Success = true }, JsonRequestBehavior.DenyGet);
            else
                return Json(new JsonResultData { Success = false, Data = "删除失败" }, JsonRequestBehavior.DenyGet);
        }

    }
}