using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosHoliday;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos节假日基础资料
    /// </summary>
    [AuthPage(ProductType.Pos, "p99035016")]
    public class BasicDataPosHolidayController : BaseEditInWindowController<PosHoliday, IPosHolidayService>
    {
        // GET: PosManage/BasicDataPosHoliday
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_Holiday", "");
            return View();
        }

        #region 增加

        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new HolidayAddViewModel());
        }

        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(HolidayAddViewModel addViewModel)
        {
            var id = Guid.NewGuid();


            try
            {
                var date = System.DateTime.Now.Year + "/" + addViewModel.VDate.Split('-')[0] + "/" + addViewModel.VDate.Split('-')[1];
                var addETime = System.DateTime.Parse(date);
            }
            catch (System.Exception)
            {
                return Json(JsonResultData.Failure("日期输入不合法"));
            }
            var modelService = GetService<IPosHolidayService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.VDate, addViewModel.DaysName, id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }


            ActionResult result = _Add(addViewModel, new PosHoliday { Id = id, Hid = CurrentInfo.HotelId, Modified = DateTime.Now }, OpLogType.Pos节假日添加);

            return result;
        }

        #endregion 增加

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(Guid id)
        {
            //ViewBag.id = id;
            return _Edit(id, new HolidayEditViewModel());
        }

        [HttpPost]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(HolidayEditViewModel model)
        {
            var modelService = GetService<IPosHolidayService>();
            try
            {
                var date = System.DateTime.Now.Year + "/" + model.VDate.Split('-')[0] + "/" + model.VDate.Split('-')[1];
                var addETime = System.DateTime.Parse(date);
            }
            catch (System.Exception)
            {
                return Json(JsonResultData.Failure("日期输入不合法"));
            }
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.VDate, model.DaysName, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

            model.Modified = DateTime.Now;
            ActionResult result = _Edit(model, new PosHoliday(), OpLogType.Pos节假日修改);
            return result;
        }

        #endregion 修改

        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosHolidayService>(), OpLogType.Pos节假日删除);
        }

        #endregion 批量删除
    }
}