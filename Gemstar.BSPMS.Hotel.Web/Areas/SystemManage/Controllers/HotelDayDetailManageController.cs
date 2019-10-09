using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.HotelDayDetailManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Extensions;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 酒店日历详细
    /// </summary>
    [AuthPage("99045")]
    public class HotelDayDetailManageController : BaseEditInWindowController<HotelDayDetail, IHotelDayDetailService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_hotelDayDetail", "@h99hotelDayid=6F9619FF-8B86-D011-B42D-00C04FC964FF");
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(string hotelDayId)
        {
            GetSelectList();
            return _Add(new HotelDayDetailAddViewModel() { HotelDayid = Guid.Parse(hotelDayId) });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(HotelDayDetailAddViewModel hotelDayDetailViewModel)
        {
            if (hotelDayDetailViewModel.Week == "1234567") { hotelDayDetailViewModel.Week = "0"; }
            return _Add(hotelDayDetailViewModel, new HotelDayDetail
            {
                Hid = CurrentInfo.HotelId,
                Id = Guid.NewGuid(),
                BeginDay = hotelDayDetailViewModel.BeginDay,
                EndDay = hotelDayDetailViewModel.EndDay,
                HotelDayid = hotelDayDetailViewModel.HotelDayid,
                Name = hotelDayDetailViewModel.Name,
                Week = hotelDayDetailViewModel.Week
            }, OpLogType.酒店日历明细增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            GetSelectList();
            return _Edit(Guid.Parse(id), new HotelDayDetailEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(HotelDayDetailEditViewModel model)
        {
            if (model.Week == "1234567") { model.Week = "0"; }
            return _Edit(model, new HotelDayDetail(), OpLogType.酒店日历明细增加);
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IHotelDayDetailService>(), OpLogType.酒店日历明细删除);
        }
        #endregion
        private void GetSelectList()
        {
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            list.Add(new SelectListItem() { Text = "星期一", Value = "1" });
            list.Add(new SelectListItem() { Text = "星期二", Value = "2" });
            list.Add(new SelectListItem() { Text = "星期三", Value = "3" });
            list.Add(new SelectListItem() { Text = "星期四", Value = "4" });
            list.Add(new SelectListItem() { Text = "星期五", Value = "5" });
            list.Add(new SelectListItem() { Text = "星期六", Value = "6" });
            list.Add(new SelectListItem() { Text = "星期日", Value = "7" });
            ViewBag.WeekList = list;
        }



    }
}