using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosDepart;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos出品部门
    /// </summary>
    [AuthPage(ProductType.Pos, "p99110003")]
    public class BasicDataPosDepartController : BaseEditInWindowController<PosDepart, IPosDepartService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_Depart", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosDepartAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosDepartAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosDepartService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

            if (Request["ProdPrinter"] != null)
            {
                addViewModel.ProdPrinter = Request["ProdPrinter"].ToString().Replace(",","");
            }

            ActionResult result = _Add(addViewModel, new PosDepart { Id = id, Hid = CurrentInfo.HotelId }, OpLogType.Pos出品部门增加);

            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new PosDepartEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosDepartEditViewModel model)
        {
            var modelService = GetService<IPosDepartService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

            //出品打印机为空时追加表单提交信息
            Type type = Request.Form.GetType();
            type.GetMethod("MakeReadWrite", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Request.Form, null);
            if (Request["ProdPrinter"] != null)
            {
                model.ProdPrinter = Request["ProdPrinter"].ToString().Replace(",","");
            }
            else
            {
                Request.Form.Add("ProdPrinter", "");
            }

            ActionResult result = _Edit(model, new PosDepart(), OpLogType.Pos出品部门修改);
            return result;
        }
        #endregion
        
        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosDepartService>(), OpLogType.Pos出品部门删除);
        }
        #endregion


        #region 启用
        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {

            var service = GetService<IPosDepartService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.启用));
            return reval;
        }
        #endregion

        #region 禁用
        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {

            var service = GetService<IPosDepartService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用));

            return reval;

        }
        #endregion
    }
}