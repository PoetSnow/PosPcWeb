using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.ScoreUseRuleManage;
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

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 积分兑换规则
    /// </summary>
    [AuthPage("99025003")]
    [AuthPage(ProductType.Member, "m61050003")]
    public class ScoreUseRuleManageController : BaseEditInWindowController<ScoreUseRule, IScoreUseRuleService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_scoreUseRule", "");
            ViewBag.isgrouphotel = CurrentInfo.IsHotelInGroup;
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new ScoreUseRuleAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(ScoreUseRuleAddViewModel scoreUseRuleViewModel)
        {
            ScoreUseRule scoreEntity;
            var mbrTypeids = scoreUseRuleViewModel.MbrCardTypeid.Split(',');
            foreach(var mbrTypeid in mbrTypeids)
            {
                scoreEntity = GetService<IScoreUseRuleService>().GetEntity(CurrentInfo.HotelId, mbrTypeid, scoreUseRuleViewModel.ItemScoreid);
                if (scoreEntity != null)
                {
                    return Json(JsonResultData.Failure("该兑换规则已经存在"));
                }
            }
            return _Add(scoreUseRuleViewModel, new ScoreUseRule
            {
                Id = Guid.NewGuid(),
                Hid = CurrentInfo.HotelId,
                AmountAndScore = scoreUseRuleViewModel.AmountAndScore,
                ItemScoreid = scoreUseRuleViewModel.ItemScoreid,
                MbrCardTypeid = scoreUseRuleViewModel.MbrCardTypeid,
                OnlyScore = scoreUseRuleViewModel.OnlyScore,
                ScoreAndAmount = scoreUseRuleViewModel.OnlyScore,
                Seqid = scoreUseRuleViewModel.Seqid
            }, OpLogType.积分兑换规则增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            string[] listMbrCardTypeids = new string[] { };
            var ScoreUseRuleService = GetService<IScoreUseRuleService>();
            var entity = ScoreUseRuleService.Get(Guid.Parse(id));
            if (!string.IsNullOrWhiteSpace(entity.MbrCardTypeid))
            {
                listMbrCardTypeids = entity.MbrCardTypeid.Split(',');
            }
            ViewBag.listMbrCardTypeids = listMbrCardTypeids;
            return _Edit(Guid.Parse(id), new ScoreUseRuleEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(ScoreUseRuleEditViewModel model)
        {
            var scoreEntity = GetService<IScoreUseRuleService>().GetEntity(CurrentInfo.HotelId, model.MbrCardTypeid, model.ItemScoreid);
            if (scoreEntity != null && scoreEntity.Id != model.Id)
                return Json(JsonResultData.Failure("该兑换规则已经存在"));
            return _Edit(model, new ScoreUseRule(), OpLogType.积分兑换规则修改);
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IScoreUseRuleService>(), OpLogType.积分兑换规则删除);
        }
        #endregion

    }
}
