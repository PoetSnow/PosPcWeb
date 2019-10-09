using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.ItemScoreManage;
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
    /// 积分项目
    /// </summary>
    [AuthPage("99025002")]
    [AuthPage(ProductType.Member, "m61050002")]
    public class ItemScoreManageController : BaseEditInWindowController<ItemScore, IItemScoreService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_itemScore", "@h99hid="+CurrentInfo.GroupHotelId);
            ViewBag.isgrouphotel = CurrentInfo.IsHotelInGroup;
            ViewBag.isgroupingroup = CurrentInfo.IsGroupInGroup;
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            var _sysParaService = GetService<ISysParaService>();
            var qiniuPara = _sysParaService.GetQiniuPara();
            ViewBag.Domain = qiniuPara.ContainsKey("domain") ? qiniuPara["domain"] : "http://res.gshis.com/";
            ViewBag.isgroupingroup = CurrentInfo.IsGroupInGroup;
            var pmsHotelService = GetService<IPmsHotelService>();
            ViewBag.Hotels = pmsHotelService.GetHotelsInGroup(CurrentInfo.GroupId);//当前集团的酒店列表
            return _Add(new ItemScoreAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(ItemScoreAddViewModel itemScoreViewModel)
        {
            return _Add(itemScoreViewModel, new ItemScore()
            {
                Code = itemScoreViewModel.Code,
                Hid = CurrentInfo.HotelId,
                Id = CurrentInfo.HotelId + itemScoreViewModel.Code,
                Name = itemScoreViewModel.Name,
                PicAdd = itemScoreViewModel.PicAdd,
                Remark = itemScoreViewModel.Remark,
                Seqid = itemScoreViewModel.Seqid,
                Status = EntityStatus.启用
            }, OpLogType.积分项目增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            var _sysParaService = GetService<ISysParaService>();
            var qiniuPara = _sysParaService.GetQiniuPara();
            ViewBag.Domain = qiniuPara.ContainsKey("domain") ? qiniuPara["domain"] : "http://res.gshis.com/";
            string[] listBelonghotels = new string[] { };
            var ItemScoreService = GetService<IItemScoreService>();
            var entity = ItemScoreService.Get(id);
            if (!string.IsNullOrWhiteSpace(entity.BelongHotel))
            {
                listBelonghotels = entity.BelongHotel.Split(',');
            }
            ViewBag.listBelonghotels = listBelonghotels;
            ViewBag.isgroupingroup = CurrentInfo.IsGroupInGroup;
            var pmsHotelService = GetService<IPmsHotelService>();
            ViewBag.Hotels = pmsHotelService.GetHotelsInGroup(CurrentInfo.GroupId);//当前集团的酒店列表
            return _Edit(id, new ItemScoreEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(ItemScoreEditViewModel model)
        {
            return _Edit(model, new ItemScore(), OpLogType.积分项目修改);
        }
        #endregion

        #region 启用禁用
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            var _itemScoreService = GetService<IItemScoreService>();
            return Json(_itemScoreService.BatchUpdateStatus(id, EntityStatus.启用));
        }
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            var _itemScoreService = GetService<IItemScoreService>();
            return Json(_itemScoreService.BatchUpdateStatus(id, EntityStatus.禁用));
        }

        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IItemScoreService>(), OpLogType.积分项目删除);
        }
        #endregion
        #region 其他

        [AuthButton(AuthFlag.None)]
        public ActionResult Detail(string picLink, object r)
        {
            return Content("<img src='" + picLink + "' alt='链接不正确，图片加载失败' />");
        }

        [AuthButton(AuthFlag.None)]
        public JsonResult GetItemScoreSelectList()
        {
            var _itemScoreService = GetService<IItemScoreService>();
            return Json(_itemScoreService.List(CurrentInfo.HotelId), JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}