using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosOperDiscount;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Kendo.Mvc.UI;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// 操作员折扣设置
    /// </summary>
    [AuthPage(ProductType.Pos, "p30005")]
    public class BasicDataPosOperDiscountController : BaseEditInWindowController<PosOperDiscount, IPosOperDiscountService>
    {

        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_PosOperDiscount", "");
            return View();
        }

        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosOperDiscountAddViewModel());
        }

        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosOperDiscountAddViewModel model)
        {
            var service = GetService<IPosOperDiscountService>();
            var boolResult = service.IsExists(CurrentInfo.HotelId, model.UserId, model.Module);
            var id = Guid.NewGuid();
            if (boolResult)
            {
                return Json(JsonResultData.Failure("操作员数据已经存在，请更换操作员！"));
            }
            var startTime = DateTime.Now.Date.ToString("yyyy-MM-dd") + " " + model.StartTime;
            var endTime = DateTime.Now.Date.ToString("yyyy-MM-dd") + " " + model.EndTime;
            try
            {
                Convert.ToDateTime(startTime);
                Convert.ToDateTime(endTime);
            }
            catch
            {

                return Json(JsonResultData.Failure("开始时间或者结束时间输入不合法！"));
            }

            ActionResult result = _Add(model, new PosOperDiscount { Id = id, Hid = CurrentInfo.HotelId, ModifiedDate = DateTime.Now }, OpLogType.Pos操作折扣设置添加);

            return result;
        }

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(Guid id)
        {
            return _Edit(id, new PosOperDiscountEditViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosOperDiscountEditViewModel model)
        {
            var startTime = DateTime.Now.Date.ToString("yyyy-MM-dd") + " " + model.StartTime;
            var endTime = DateTime.Now.Date.ToString("yyyy-MM-dd") + " " + model.EndTime;
            try
            {
                Convert.ToDateTime(startTime);
                Convert.ToDateTime(endTime);
            }
            catch
            {

                return Json(JsonResultData.Failure("开始时间或者结束时间输入不合法！"));
            }
            model.ModifiedDate = DateTime.Now;
            ActionResult result = _Edit(model, new PosOperDiscount(), OpLogType.Pos操作折扣设置修改);
            return result;
        }

        #region 下拉数据
        [AuthButton(AuthFlag.None)]
        public JsonResult UserList()
        {
            var service = GetService<IPosOperDiscountService>();
            var userList = service.GetPmsUserList(CurrentInfo.GroupHotelId);
            var a = userList.Select(w => new SelectListItem { Value = w.Id.ToString(), Text = w.Name }).ToList();
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        [AuthButton(AuthFlag.None)]
        public JsonResult iCountTypeList()
        {
            var codeListService = GetService<ICodeListService>();
            var datas = codeListService.List("92").ToList();

            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);

        }

        [AuthButton(AuthFlag.None)]
        public JsonResult iCmpTypeList()
        {
            var codeListService = GetService<ICodeListService>();
            var datas = codeListService.List("93").ToList();

            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        [AuthButton(AuthFlag.None)]
        public JsonResult iRateTypeList()
        {
            var codeListService = GetService<ICodeListService>();
            var datas = codeListService.List("94").ToList();

            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        #endregion



        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosOperDiscountService>(), OpLogType.Pos操作折扣设置删除);
        }

        #endregion 批量删除

    }
}