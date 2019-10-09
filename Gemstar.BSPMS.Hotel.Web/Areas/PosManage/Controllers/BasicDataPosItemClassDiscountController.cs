using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemClass;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemClassDiscount;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    [AuthPage(ProductType.Pos, "p30004")]
    
    public class BasicDataPosItemClassDiscountController : BaseEditInWindowController<PosOnSale, IPosOnSaleService>
    {
        // GET: PosManage/BasicDataPosItemClassDiscount
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {           
            SetCommonQueryValues("up_pos_list_positemclassdiscount", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosItemClassDiscountAddViewModel() { IsDiscount = true});
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosItemClassDiscountAddViewModel addViewModel)
        {
            var id = Guid.NewGuid();
            var modelService = GetService<IPosItemClassDiscountService>();

            if (!CheckDate(addViewModel.StartTime, addViewModel.EndTime))
            {
                return Json(JsonResultData.Failure("操作错误,开始日期或者结束日期或输入不合法！"));
            }

            //项目ID和消费大类ID必填其一
            if (string.IsNullOrEmpty(addViewModel.Itemid) && string.IsNullOrEmpty(addViewModel.ItemClassID))
            {
                return Json(JsonResultData.Failure("请选择消费项目或者消费项目大类"));
            }

            if(!string.IsNullOrEmpty(addViewModel.Itemid) && string.IsNullOrEmpty(addViewModel.Unitid))
            {
                return Json(JsonResultData.Failure("请选择单位"));
            }

            if(string.IsNullOrEmpty(addViewModel.Itemid))
            {
                addViewModel.Itemid = "";
            }
            if (string.IsNullOrEmpty(addViewModel.Unitid))
            {
                addViewModel.Unitid = "";
            }


            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, "CY", addViewModel.Itemid,addViewModel.ItemClassID,addViewModel.CustomerTypeid,addViewModel.Refeid,addViewModel.TabTypeid,addViewModel.Unitid,addViewModel.StartTime,addViewModel.EndTime);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复消费项目、消费项目大类 或 时间段冲突！")); }


            ActionResult result = _Add(addViewModel, new PosOnSale { Id = id, Hid = CurrentInfo.HotelId, ModifiedDate = DateTime.Now,iType = 2 , Module = "CY" }, OpLogType.Pos大类折扣添加);

            return result;
        }

        #endregion 增加

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(Guid id)
        {
            return _Edit(id, new PosItemClassDiscountEditViewModel());
        }

        [HttpPost]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosItemClassDiscountEditViewModel model)
        {
            var modelService = GetService<IPosItemClassDiscountService>();

            if (!CheckDate(model.StartTime, model.EndTime))
            {
                return Json(JsonResultData.Failure("操作错误,开始日期或者结束日期或输入不合法！"));
            }

            //项目ID和消费大类ID必填其一
            if (string.IsNullOrEmpty(model.Itemid) && string.IsNullOrEmpty(model.ItemClassID))
            {
                return Json(JsonResultData.Failure("请选择消费项目或者消费项目大类"));
            }

            if (!string.IsNullOrEmpty(model.Itemid) && string.IsNullOrEmpty(model.Unitid))
            {
                return Json(JsonResultData.Failure("请选择单位"));
            }
            if (string.IsNullOrEmpty(model.Itemid))
            {
                model.Itemid = "";
            }
            if (string.IsNullOrEmpty(model.Unitid))
            {
                model.Unitid = "";
            }


            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, "CY", model.Itemid, model.ItemClassID, model.CustomerTypeid, model.Refeid, model.TabTypeid, model.Unitid, model.StartTime, model.EndTime,model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复消费项目、消费项目大类 或 时间段冲突！")); }
     

            ActionResult result = _Edit(model, new PosOnSale() { ModifiedDate = DateTime.Now ,Hid = CurrentInfo.HotelId, iType = 2,Module="CY" }, OpLogType.Pos大类折扣修改);
            return result;
        }

        #endregion 修改

        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id.Trim(','), GetService<IPosItemClassDiscountService >(), OpLogType.Pos大类折扣删除);
        }

        #endregion 批量删除

        #region 下拉列表

        //获取会员卡类型
        [AuthButton(AuthFlag.None)]
        public JsonResult ListMbrCardType()
        {
            var server = GetService<IPosItemClassDiscountService>();
            var list = server.GetMbrCardTypes(CurrentInfo.HotelId, u => true).Select(u=> { return new { Text = u.Name, Value = u.Id }; });

            return Json(list,JsonRequestBehavior.AllowGet);
        }

        #endregion

        private bool CheckDate(string startTime, string EndTime)
        {
            if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(EndTime))
            {
                return false;
            }
            var date = DateTime.Now.ToShortDateString().ToString();

            try
            {
                var s = Convert.ToDateTime(date + " " + startTime);
                var e = Convert.ToDateTime(date + " " + EndTime);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }





    }
}