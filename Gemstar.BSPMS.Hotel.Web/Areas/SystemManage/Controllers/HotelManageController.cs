using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.HotelManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 酒店资料
    /// </summary>
    [AuthPage("99110")]
    [AuthPage(ProductType.Member, "m99045")]
    [AuthPage(ProductType.Pos, "p99035013")]
    [BusinessType("消费项目维护")]
    public class HotelManageController : BaseEditInWindowController<PmsHotel, IPmsHotelService>
    {
        // GET: SystemManage/HotelManage

        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            if (CurrentInfo.IsGroup == true && !CurrentInfo.IsHotelInGroup)
            {
                SetCommonQueryValues("up_list_pmshotel", "");
                return View();
            }
            else
            {
                ViewBag.isGroup = CurrentInfo.IsGroup;
                ViewBag.grpname = (CurrentInfo.IsGroup == true ? GetService<IHotelInfoService>().GetHotelInfo(CurrentInfo.GroupId).Name : "");
                return _Edit(CurrentInfo.HotelId, new PmsHotelEditViewModel() { });
            }

        }
        #region 修改
        /// <summary>
        /// 修改会员卡类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            ViewBag.grpname = GetService<IHotelInfoService>().GetHotelInfo(CurrentInfo.GroupId).Name;
            ViewBag.isGroup = CurrentInfo.IsGroup;
            return _Edit(id, new PmsHotelEditViewModel(), "_EditGroup");
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PmsHotelEditViewModel model)
        {

            var curinfo = GetService<ICurrentInfo>();
            var result=_Edit(model, new PmsHotel() { Hid = model.Hid }, OpLogType.酒店资料修改);
            var dbCommon = GetService<DbCommonContext>();
            var channelParas = dbCommon.Hotels.Where(w => w.Hid == model.Hid).FirstOrDefault();
            dbCommon.Entry(channelParas).State = EntityState.Modified;             
            channelParas.Hotelshortname = model.Hotelshortname;
            channelParas.manageType = model.ManageType;
            channelParas.Star = model.Star;
            channelParas.City = model.City;
            channelParas.Provinces = model.Provinces;
            channelParas.Email = model.Email;  
            dbCommon.SaveChanges();
            return result;
        } 
        #endregion
            #region 下拉绑定
        [AuthButton(AuthFlag.None)]
        public JsonResult GetProvinceSelectList()
        {
            var _masterService = GetService<IMasterService>();
            var province = _masterService.GetProvince().Select(e => new SelectListItem { Value = e.Code, Text = e.Name });
            return Json(province, JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.None)]
        public JsonResult GetCitySelectList(string key, object r)
        {
            var _masterService = GetService<IMasterService>();
            var city = _masterService.GetCity(key).Select(e => new SelectListItem { Value = e.Code, Text = e.Name });
            return Json(city, JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.None)]
        public JsonResult getStarList()
        {
            var _masterService = GetService<IItemService>();
            var city = _masterService.GetCodeListPub("25").Select(e => new SelectListItem { Value = e.code, Text = e.name });
            return Json(city, JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.None)]
        public JsonResult getManageTypeList()
        {
            var _masterService = GetService<IItemService>();
            var city = _masterService.GetCodeListPub("26").Select(e => new SelectListItem { Value = e.code, Text = e.name });
            return Json(city, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}