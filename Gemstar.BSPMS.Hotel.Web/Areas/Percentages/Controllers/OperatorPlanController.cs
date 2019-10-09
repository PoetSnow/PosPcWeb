using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.ChargeFreeManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Extensions;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Gemstar.BSPMS.Hotel.Web.Models;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.Percentages;
using Gemstar.BSPMS.Hotel.Web.Areas.Percentages.Models.OperatorPlan;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Percentages.Controllers
{
    /// <summary>
    /// 操作员提成任务定义
    /// </summary>
    [AuthPage("99055001")]
    [AuthPage(ProductType.Member, "m61080001")]
    public class OperatorPlanController : BaseEditIncellController<percentagesPlan, IpercentagesPlanService>  
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_percentagesPlanOperators", "");
            return View();
        }
        #endregion

      
        #region 修改
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<percentagesPlan> updateClass, [Bind(Prefix = "originModels")]IEnumerable<percentagesPlan> originClass)
        {
            var serv = GetService<IpercentagesPlanService>();
            foreach (var model in updateClass)
            {
                decimal?[] parr = new decimal?[12];
                parr[0] = model.one;
                parr[1] = model.two;
                parr[2] = model.three;
                parr[3] = model.four;
                parr[4] = model.five;
                parr[5] = model.six;
                parr[6] = model.seven;
                parr[7] = model.eight;
                parr[8] = model.nine;
                parr[9] = model.ten;
                parr[10] = model.eleven;
                parr[11] = model.twelve;
                serv.setPercentagesPlanOperators(model.Hid, model.SalesmanId, model.curYear, model.PlanSource, parr);
            }
            return Json("");
        }
        #endregion
        //#region 修改
        //[AuthButton(AuthFlag.Update)]
        //public ActionResult Edit(string id)
        //{
        //    return _Edit(Guid.Parse(id), new OperatorPlanEditViewModel());
        //}
        //[HttpPost]
        //[JsonException]
        //[AuthButton(AuthFlag.Update)]
        //public ActionResult Edit(OperatorPlanEditViewModel model)
        //{
        //    model.PlanDate = DateTime.Parse(model.PlanDate.ToString("yyyy-MM-01"));
        //    if (GetService<IOperatorPlanService>().Exists(CurrentInfo.HotelId, new List<Guid> { model.OperatorId }, model.PlanDate, model.PlanSource, model.PlanId))
        //    {
        //        return Json(JsonResultData.Failure("此计划任务已存在，不能重复！"));
        //    }
        //    return _Edit(model, new PercentagesPlanOperator(), OpLogType.操作员提成任务修改);
        //}
        //#endregion
        

        #region 下拉列表
    
        /// <summary>
        /// 操作员提成取值来源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForSource()
        {
            var queryService = GetService<ICodeListService>();
            var datas = queryService.GetPercentagesOperatorType();
            var list = datas.Select(c => new SelectListItem { Text = c.name, Value = c.code });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}