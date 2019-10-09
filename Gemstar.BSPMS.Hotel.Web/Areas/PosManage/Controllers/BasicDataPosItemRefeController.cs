using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemRefe;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos消费项目对应营业点
    /// </summary>
    [AuthPage(ProductType.Pos, "p99020011")]
    public class BasicDataPosItemRefeController : BaseEditInWindowController<PosItemRefe, IPosItemRefeService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_ItemRefe", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosItemRefeAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosItemRefeAddViewModel addViewModel)
        {
            var id = Guid.NewGuid();
            var modelService = GetService<IPosItemRefeService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Itemid, addViewModel.Refeid);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

            if (!string.IsNullOrEmpty(Request["Shuffleid"]))
            {
                addViewModel.Shuffleid = Request["Shuffleid"].ToString();
            }
            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(addViewModel.Refeid);
            if (refe == null)
            {
                return Json(JsonResultData.Failure("营业点不能为空"));
            }

            if (Request["ProdPrinter"] != null)
            {
                addViewModel.ProdPrinter = Request["ProdPrinter"].ToString().Replace(",","");
            }

            if (Request["SentPrtNo"] != null)
            {
                addViewModel.SentPrtNo = Request["SentPrtNo"].ToString().Replace(",","");
            }

            ActionResult result = _Add(addViewModel, new PosItemRefe { Id = id, Hid = CurrentInfo.HotelId, RefeName = refe.Cname, Modified = DateTime.Now }, OpLogType.Pos消费项目对应营业点增加);
            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(Guid id)
        {
            return _Edit(id, new PosItemRefeEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosItemRefeEditViewModel model)
        {
            var modelService = GetService<IPosItemRefeService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Itemid, model.Refeid, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(model.Refeid);

            if (refe == null)
            {
                return Json(JsonResultData.Failure("营业点不能为空"));
            }
            //开台信息、楼面台号显示内容为空时追加表单提交信息
            Type type = Request.Form.GetType();
            type.GetMethod("MakeReadWrite", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Request.Form, null);
            if (Request["Shuffleid"] == null)
            {
                Request.Form.Add("Shuffleid", "");
            }


            if (!string.IsNullOrEmpty(Request["Shuffleid"]))
            {
                model.Shuffleid = Request["Shuffleid"].ToString();
            }
            model.RefeName = refe.Cname;
            model.Modified = DateTime.Now;

            if (Request["ProdPrinter"] != null)
            {
                model.ProdPrinter = Request["ProdPrinter"].ToString().Replace(",","");
            }
            else
            {
                Request.Form.Add("ProdPrinter", "");
            }

            if (Request["SentPrtNo"] != null)
            {
                model.SentPrtNo = Request["SentPrtNo"].ToString().Replace(",","");
            }
            else
            {
                Request.Form.Add("SentPrtNo", "");
            }

            ActionResult result = _Edit(model, new PosItemRefe(), OpLogType.Pos消费项目对应营业点修改);
            return result;
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosItemRefeService>(), OpLogType.Pos消费项目对应营业点删除);
        }
        #endregion
    }
}
