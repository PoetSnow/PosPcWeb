using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosInvoiceItem;
using Gemstar.BSPMS.Hotel.Web.Controllers;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// 发票项目管理
    /// </summary>
    [AuthPage(ProductType.Pos, "p99035019")]
    public class BasicDataPosInvoiceItemController : BaseEditInWindowController<CodeList, ICodeListService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_InvoiceItem", "");
            return View();
        }


        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new InvoiceItemAddViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(InvoiceItemAddViewModel addViewModel)
        {


            var id = CurrentInfo.HotelId + addViewModel.TypeCode + addViewModel.Code;
            var modelService = GetService<ICodeListService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.TypeCode, addViewModel.Code);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码")); }

            if (!string.IsNullOrEmpty(Request["Remark"]))
            {
                addViewModel.Remark = Request["Remark"].ToString();
            }


            ActionResult result = _Add(addViewModel, new CodeList { Id = id, Hid = CurrentInfo.HotelId, Status = EntityStatus.启用 }, OpLogType.Pos发票项目添加);

            return result;
        }


        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(int id)
        {
            return _Edit(id, new InvoiceItemEditViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(InvoiceItemEditViewModel model)
        {

            var modelService = GetService<ICodeListService>();

            //bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.VDate, model.DaysName, model.Id);
            //if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }


            ActionResult result = _Edit(model, new CodeList(), OpLogType.Pos发票项目修改);
            return result;
        }


        [AuthButton(AuthFlag.None)]
        public JsonResult ListiItemSpecial()
        {
            //var datas = new List<Dictionary<string,string>>();


            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Value = "服务费", Text = "服务费" });
            listItems.Add(new SelectListItem { Value = "抹零", Text = "抹零" });
            listItems.Add(new SelectListItem { Value = "消费余额", Text = "消费余额" });
            //var listItems = datas.Select(w => new SelectListItem { Value = w., Text = w.Values }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
    }
}