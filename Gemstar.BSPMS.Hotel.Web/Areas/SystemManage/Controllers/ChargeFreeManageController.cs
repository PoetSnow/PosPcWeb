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

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 充值赠送规则
    /// </summary>
    [AuthPage("99025004")]
    [AuthPage(ProductType.Member, "m61050004")]
    public class ChargeFreeManageController : BaseEditInWindowController<ChargeFree, IChargeFreeService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_chargeFree", "");
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new ChargeFreeAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(ChargeFreeAddViewModel chargeFreeViewModel)
        {
            if (chargeFreeViewModel.Rate == null)
            {
                chargeFreeViewModel.Rate = 0;
            }
            chargeFreeViewModel.Rate = chargeFreeViewModel.Rate / 100;

            return _Add(chargeFreeViewModel, new ChargeFree
            {
                Id = Guid.NewGuid(),
                Hid = CurrentInfo.HotelId,
                MbrCardTypeid = chargeFreeViewModel.MbrCardTypeid,
                Amount = chargeFreeViewModel.Amount,
                BeginAmount = chargeFreeViewModel.BeginAmount,
                EndAmount = chargeFreeViewModel.EndAmount,
                Rate = chargeFreeViewModel.Rate
            }, OpLogType.充值赠送规则增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(Guid.Parse(id), new ChargeFreeEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(ChargeFreeEditViewModel model)
        {
            if (model.Rate == null)
            {
                model.Rate = 0;
            }
            model.Rate = model.Rate / 100;
            return _Edit(model, new ChargeFree(), OpLogType.充值赠送规则修改);
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IChargeFreeService>(), OpLogType.充值赠送规则删除);
        }
        #endregion
    }
}