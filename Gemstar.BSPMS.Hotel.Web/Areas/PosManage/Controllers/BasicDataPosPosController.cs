using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosPos;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos收银点
    /// </summary>
    [AuthPage(ProductType.Pos, "p99021002")]
    public class BasicDataPosPosController : BaseEditInWindowController<PosPos, IPosPosService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_pos", "");
            return View();
        }

        #region 增加

        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            ViewBag.ModuleCode = CurrentInfo.ModuleCode;
            return _Add(new PosAddViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosAddViewModel addViewModel)
        {
            var hotelService = GetService<IHotelInfoService>();
            var hotel = hotelService.ListValidHotels().Where(m => m.Hid == CurrentInfo.HotelId).FirstOrDefault();
            var posCount = hotel.PosSettlePointCount; //运营系统定义的收银点数量

            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosPosService>();

            var posList = modelService.GetPosByHid(CurrentInfo.HotelId);
            if (posList.Count >= posCount) { return Json(JsonResultData.Failure("操作错误，收银点数量已达上限！")); }

            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Name);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误，重复代码 或 重复名称！")); }
            try
            {
                var date = System.DateTime.Now.Year + "/" + System.DateTime.Now.Month + "/" + System.DateTime.Now.Day;
                var addETime = System.DateTime.Parse(date + " " + addViewModel.BusinessTime);
            }
            catch (System.Exception)
            {
                return Json(JsonResultData.Failure("最早结转时间不合法！"));
            }
            ActionResult result = _Add(addViewModel, new PosPos { Id = id, Hid = CurrentInfo.HotelId }, OpLogType.Pos收银点增加);

            return result;
        }

        #endregion 增加

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new PosEditViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosEditViewModel model)
        {
            var modelService = GetService<IPosPosService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Name, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            try
            {
                var date = System.DateTime.Now.Year + "/" + System.DateTime.Now.Month + "/" + System.DateTime.Now.Day;
                var addETime = System.DateTime.Parse(date + " " + model.BusinessTime);
            }
            catch (System.Exception)
            {
                return Json(JsonResultData.Failure("最早结转时间不合法！"));
            }
            ActionResult result = _Edit(model, new PosPos(), OpLogType.Pos收银点修改);
            return result;
        }

        #endregion 修改

        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosPosService>(), OpLogType.Pos收银点删除);
        }

        #endregion 批量删除

        #region 下拉数据绑定

        /// <summary>
        /// 获取指定酒店下的收银点
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosPosByHid()
        {
            var service = GetService<IPosPosService>();
            var datas = service.GetPosByHid(CurrentInfo.HotelId);
            var listItems = datas.Where(m => m.PosMode == "A" && (m.IStatus == (byte)EntityStatus.启用 || m.IStatus == null)).Select(w => new SelectListItem { Value = w.Id, Text = w.Name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取指定酒店下的收银点
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosByHid(string hid, string hname)
        {
            CurrentInfo.HotelId = hid;
            CurrentInfo.HotelName = hname;
            CurrentInfo.SaveValues();

            var service = GetService<IPosPosService>();
            var datas = service.GetPosByHid(CurrentInfo.HotelId);
            var listItems = datas.Where(m => m.PosMode == "A" && (m.IStatus == (byte)EntityStatus.启用 || m.IStatus == null)).Select(w => new SelectListItem { Value = w.Id, Text = w.Name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取指定模块下的收银点
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosPosByModules()
        {
            var service = GetService<IPosPosService>();
            var datas = service.GetPosByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode).Where(m => (m.IStatus == (byte)EntityStatus.启用 || m.IStatus == null));
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 云Pos 属性列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosMode()
        {
            var service = GetService<ICodeListService>();
            var datas = service.List("72");
            var hotelService = GetService<IHotelInfoService>();
            var hotel = hotelService.ListValidHotels().Where(m => m.Hid == CurrentInfo.HotelId).FirstOrDefault();
            var hotleType = hotel.CateringServicesType.Split(',');

            var listItems = datas.Where(m => hotleType.Contains(m.code)).Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取指定酒店下的收银点
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListPosByHid()
        {
            var service = GetService<IPosPosService>();
            var datas = service.GetPosByHid(CurrentInfo.HotelId).Where(m => (m.IStatus == (byte)EntityStatus.启用 || m.IStatus == null)).ToList();
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        #endregion 下拉数据绑定

        #region 启用

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            var service = GetService<IPosPosService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.启用));
            return reval;
        }

        #endregion 启用

        #region 禁用

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            var service = GetService<IPosPosService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用));

            return reval;
        }

        #endregion 禁用
    }
}