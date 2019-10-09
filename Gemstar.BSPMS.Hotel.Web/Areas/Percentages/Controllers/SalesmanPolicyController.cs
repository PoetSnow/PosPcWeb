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
using Gemstar.BSPMS.Hotel.Web.Areas.Percentages.Models.SalesmanPolicy;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Percentages.Controllers
{
    /// <summary>
    /// 业务员提成政策定义
    /// </summary>
    [AuthPage("99045002")]
    [AuthPage(ProductType.Member, "m61075002")]
    public class SalesmanPolicyController : BaseEditInWindowController<PercentagesPolicySalesman, ISalesmanPolicyService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_percentagesPolicySalesman", "");
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new SalesmanPolicyAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(SalesmanPolicyAddViewModel salesmanPolicyViewModel)
        {
            string hid = CurrentInfo.HotelId;
            if(salesmanPolicyViewModel.AmountBegin < 1 || salesmanPolicyViewModel.AmountEnd < 1)
            {
                return Json(JsonResultData.Failure("[开始值]和[结束值]必须大于等于1！"));
            }
            if(salesmanPolicyViewModel.AmountBegin > salesmanPolicyViewModel.AmountEnd)
            {
                return Json(JsonResultData.Failure("[开始值]要小于或等于[结束值]！"));
            }
            if(salesmanPolicyViewModel.AmountSumType == "single")
            {
                salesmanPolicyViewModel.IsInPlan = null;
            }
            else
            {
                if(salesmanPolicyViewModel.IsInPlan == null)
                {
                    return Json(JsonResultData.Failure("请选择[内容类型]！"));
                }
            }

            var salesmanPolicyService = GetService< ISalesmanPolicyService>();
            if(salesmanPolicyService.ExixtsByAmountIsAll(hid, salesmanPolicyViewModel.AmountSource, salesmanPolicyViewModel.AmountSumType, salesmanPolicyViewModel.IsInPlan, salesmanPolicyViewModel.IsAllAmount))
            {
                return Json(JsonResultData.Failure("在[提成内容+计算类型+内容类型]相同的记录中，[是否全额]必须一致！"));
            }
            if (salesmanPolicyService.ExixtsByAmountRange(hid, salesmanPolicyViewModel.AmountSource, salesmanPolicyViewModel.AmountSumType, salesmanPolicyViewModel.IsInPlan, salesmanPolicyViewModel.AmountBegin, salesmanPolicyViewModel.AmountEnd))
            {
                return Json(JsonResultData.Failure("在[提成内容+计算类型+内容类型]相同的记录中，[开始值-结束值]的范围之间不能重叠！"));
            }

            return _Add(salesmanPolicyViewModel, new PercentagesPolicySalesman
            {
                Hid = hid,
                PolicyId = Guid.NewGuid(),

                AmountSource = salesmanPolicyViewModel.AmountSource,

                AmountBegin = salesmanPolicyViewModel.AmountBegin,
                AmountEnd = salesmanPolicyViewModel.AmountEnd,

                IsInPlan = salesmanPolicyViewModel.IsInPlan,
                IsAllAmount = salesmanPolicyViewModel.IsAllAmount,

                AmountSumType = salesmanPolicyViewModel.AmountSumType,
                CalcType = salesmanPolicyViewModel.CalcType,
                CalcValue = salesmanPolicyViewModel.CalcValue,
                
                PolicyDesciption = salesmanPolicyViewModel.PolicyDesciption,
                
            }, OpLogType.业务员提成政策增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(Guid.Parse(id), new SalesmanPolicyEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(SalesmanPolicyEditViewModel model)
        {
            string hid = CurrentInfo.HotelId;
            if (model.AmountBegin < 0 || model.AmountEnd < 0)
            {
                return Json(JsonResultData.Failure("[开始值]和[结束值]必须大于等于零！"));
            }
            if (model.AmountBegin > model.AmountEnd)
            {
                return Json(JsonResultData.Failure("[开始值]要小于或等于[结束值]！"));
            }
            if (model.AmountSumType == "single")
            {
                model.IsInPlan = null;
            }
            else
            {
                if (model.IsInPlan == null)
                {
                    return Json(JsonResultData.Failure("请选择[内容类型]！"));
                }
            }

            var salesmanPolicyService = GetService<ISalesmanPolicyService>();
            if (salesmanPolicyService.ExixtsByAmountIsAll(hid, model.AmountSource, model.AmountSumType, model.IsInPlan, model.IsAllAmount, model.PolicyId))
            {
                return Json(JsonResultData.Failure("在[提成内容+计算类型+内容类型]相同的记录中，[是否全额]必须一致！"));
            }
            if (salesmanPolicyService.ExixtsByAmountRange(hid, model.AmountSource, model.AmountSumType, model.IsInPlan, model.AmountBegin, model.AmountEnd, model.PolicyId))
            {
                return Json(JsonResultData.Failure("在[提成内容+计算类型+内容类型]相同的记录中，[开始值-结束值]的范围之间不能重叠！"));
            }

            return _Edit(model, new PercentagesPolicySalesman(), OpLogType.业务员提成政策修改);
        }
        /// <summary>
        /// 修改[是否全额]
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.Update)]
        public ActionResult EditIsAllAmount(Guid id)
        {
            GetService<ISalesmanPolicyService>().EditIsAllAmount(CurrentInfo.HotelId, id);
            return Json(JsonResultData.Successed());
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<ISalesmanPolicyService>(), OpLogType.业务员提成政策删除);
        }
        #endregion

        #region 下拉列表
        /// <summary>
        /// 业务员提成取值来源
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForSource()
        {
            var queryService = GetService<ICodeListService>();
            var datas = queryService.GetPercentagesSalesmanType();
            var list = datas.Select(c => new SelectListItem { Text = c.name, Value = c.code });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 金额计算类型：single:单次，month:按月累计
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForAmountSumType()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "按月累计", Value = "month" });
            list.Add(new SelectListItem { Text = "单次", Value = "single" });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 提成计算类型：percent:比例，price:单价，amount:固定金额
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForCalcType()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "比例", Value = "percent" });
            list.Add(new SelectListItem { Text = "单价", Value = "price" });
            list.Add(new SelectListItem { Text = "固定金额", Value = "amount" });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}