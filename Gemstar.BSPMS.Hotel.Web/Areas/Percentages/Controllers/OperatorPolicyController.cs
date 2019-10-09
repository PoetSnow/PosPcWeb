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
using Gemstar.BSPMS.Hotel.Web.Areas.Percentages.Models.OperatorPolicy;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Percentages.Controllers
{
    /// <summary>
    /// 操作员提成政策定义
    /// </summary>
    [AuthPage("99055002")]
    [AuthPage(ProductType.Member, "m61080002")]
    public class OperatorPolicyController : BaseEditInWindowController<PercentagesPolicyOperator, IOperatorPolicyService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_percentagesPolicyOperator", "");
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new OperatorPolicyAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(OperatorPolicyAddViewModel OperatorPolicyViewModel)
        {
            string hid = CurrentInfo.HotelId;
            if (OperatorPolicyViewModel.AmountBegin < 1 || OperatorPolicyViewModel.AmountEnd < 1)
            {
                return Json(JsonResultData.Failure("[开始值]和[结束值]必须大于等于1！"));
            }
            if (OperatorPolicyViewModel.AmountBegin > OperatorPolicyViewModel.AmountEnd)
            {
                return Json(JsonResultData.Failure("[开始值]要小于或等于[结束值]！"));
            }
            if (OperatorPolicyViewModel.AmountSumType == "single")
            {
                OperatorPolicyViewModel.IsInPlan = null;
            }
            else
            {
                if (OperatorPolicyViewModel.IsInPlan == null)
                {
                    return Json(JsonResultData.Failure("请选择[内容类型]！"));
                }
            }

            var OperatorPolicyService = GetService<IOperatorPolicyService>();
            if (OperatorPolicyService.ExixtsByAmountIsAll(hid, OperatorPolicyViewModel.AmountSource, OperatorPolicyViewModel.AmountSumType, OperatorPolicyViewModel.IsInPlan, OperatorPolicyViewModel.IsAllAmount))
            {
                return Json(JsonResultData.Failure("在[提成内容+计算类型+内容类型]相同的记录中，[是否全额]必须一致！"));
            }
            if (OperatorPolicyService.ExixtsByAmountRange(hid, OperatorPolicyViewModel.AmountSource, OperatorPolicyViewModel.AmountSumType, OperatorPolicyViewModel.IsInPlan, OperatorPolicyViewModel.AmountBegin, OperatorPolicyViewModel.AmountEnd))
            {
                return Json(JsonResultData.Failure("在[提成内容+计算类型+内容类型]相同的记录中，[开始值-结束值]的范围之间不能重叠！"));
            }

            return _Add(OperatorPolicyViewModel, new PercentagesPolicyOperator
            {
                Hid = hid,
                PolicyId = Guid.NewGuid(),

                AmountSource = OperatorPolicyViewModel.AmountSource,

                AmountBegin = OperatorPolicyViewModel.AmountBegin,
                AmountEnd = OperatorPolicyViewModel.AmountEnd,

                IsInPlan = OperatorPolicyViewModel.IsInPlan,
                IsAllAmount = OperatorPolicyViewModel.IsAllAmount,

                AmountSumType = OperatorPolicyViewModel.AmountSumType,
                CalcType = OperatorPolicyViewModel.CalcType,
                CalcValue = OperatorPolicyViewModel.CalcValue,

                PolicyDesciption = OperatorPolicyViewModel.PolicyDesciption,

            }, OpLogType.操作员提成政策增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(Guid.Parse(id), new OperatorPolicyEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(OperatorPolicyEditViewModel model)
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

            var OperatorPolicyService = GetService<IOperatorPolicyService>();
            if (OperatorPolicyService.ExixtsByAmountIsAll(hid, model.AmountSource, model.AmountSumType, model.IsInPlan, model.IsAllAmount, model.PolicyId))
            {
                return Json(JsonResultData.Failure("在[提成内容+计算类型+内容类型]相同的记录中，[是否全额]必须一致！"));
            }
            if (OperatorPolicyService.ExixtsByAmountRange(hid, model.AmountSource, model.AmountSumType, model.IsInPlan, model.AmountBegin, model.AmountEnd, model.PolicyId))
            {
                return Json(JsonResultData.Failure("在[提成内容+计算类型+内容类型]相同的记录中，[开始值-结束值]的范围之间不能重叠！"));
            }

            return _Edit(model, new PercentagesPolicyOperator(), OpLogType.操作员提成政策修改);
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
            GetService<IOperatorPolicyService>().EditIsAllAmount(CurrentInfo.HotelId, id);
            return Json(JsonResultData.Successed());
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IOperatorPolicyService>(), OpLogType.操作员提成政策删除);
        }
        #endregion

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