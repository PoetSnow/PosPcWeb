using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.PlanTask;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Controllers
{ /// <summary>
  /// 付款分类管理
  /// </summary>
    [AuthPage("61090")]
    [AuthPage(ProductType.Member, "m61065")]
    public class PlanTaskController :   BaseEditInWindowController<PlanTask, IPlanTaskService>
    {

        // GET: MarketingManage/PlanTask

        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(bool viewType = true)
        {
            SetCommonQueryValues("up_list_PlanTask", "");
            ViewBag.viewType = viewType;
            if (viewType)
            {
                return View();
            }
            else
            {
                return PartialView();
            }
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            
            return _Add(new PlanTaskAddViewModel());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rateViewModel"></param>
        /// <param name="viewType"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(string begintime,string endtime,string arr)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            var PlanTaskService = GetService<IPlanTaskService>();
            var hc = PlanTaskService.setPlanTask(hid, "0", begintime, endtime,arr);
            return Json(hc, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 修改
        /// <summary>
        ///   
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(Guid  id) 
        {
            PlanTaskEditViewModel model = new PlanTaskEditViewModel();
            model.Hid = CurrentInfo.HotelId;
            return _Edit(id, model);
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PlanTaskEditViewModel planTaskViewModel)
        {

            return _Edit(planTaskViewModel, new PlanTask() { }, OpLogType.计划任务修改);
        }
        #endregion
        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {  
            return _BatchDelete(id, GetService<IPlanTaskService>(), OpLogType.计划任务删除);
        }
        #endregion
    }
}