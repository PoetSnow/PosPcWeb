using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosProdPrinter;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos出品打印机
    /// </summary>
    [AuthPage(ProductType.Pos, "p99110005")]
    public class BasicDataPosProdPrinterController : BaseEditInWindowController<PosProdPrinter, IPosProdPrinterService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_ProdPrinter", "");
            return View();
        }

        #region 增加

        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new ProdPrinterAddViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(ProdPrinterAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosProdPrinterService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            ActionResult result = _Add(addViewModel, new PosProdPrinter { Id = id, Hid = CurrentInfo.HotelId, }, OpLogType.Pos出品打印机增加);

            return result;
        }

        #endregion 增加

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new ProdPrinterEditViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(ProdPrinterEditViewModel model)
        {
            var modelService = GetService<IPosProdPrinterService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            ActionResult result = _Edit(model, new PosProdPrinter(), OpLogType.Pos出品打印机修改);
            return result;
        }

        #endregion 修改

        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosProdPrinterService>(), OpLogType.Pos出品打印机删除);
        }

        #endregion 批量删除

        #region 启用

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            var service = GetService<IPosProdPrinterService>();
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
            var service = GetService<IPosProdPrinterService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用));

            return reval;
        }

        #endregion 禁用

        /// <summary>
        /// 获取指定模块下的出品打印机列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForProdPrinterByModules()
        {
            var service = GetService<IPosProdPrinterService>();
            var datas = service.GetPosProdPrinterByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Code, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        #region 消费项目批量修改 传菜打印机以及出品打印机数据

        /// <summary>
        /// 出品打印机列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForProdPrinterA()
        {
            var service = GetService<IPosProdPrinterService>();
            var datas = service.GetPosProdPrinterByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode).Where(m => m.IsTabeachbreak == true);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Code, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 传菜打印机
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForProdPrinterB()
        {
            var service = GetService<IPosProdPrinterService>();
            var datas = service.GetPosProdPrinterByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode).Where(m => m.IsTabeachbreak == false);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Code, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}