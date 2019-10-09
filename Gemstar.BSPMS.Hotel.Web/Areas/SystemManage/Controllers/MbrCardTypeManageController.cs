using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.MbrCardTypeManage;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using Gemstar.BSPMS.Common.Extensions;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 会员卡类型维护
    /// </summary>
    [AuthPage("99025001")]
    [AuthPage(ProductType.Member, "m61050001")]
    [AuthBasicData("mbrCardType")]
    [BusinessType("角色管理")]
    public class MbrCardTypeManageController : BaseEditInWindowController<MbrCardType, IMbrCardTypeService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        // GET: SystemManage/MbrCardTypeManage
        public ActionResult Index()
        {
            //会员卡类型属于集团管控型属性，集团和分店都可以查询，并且都是按grpid来查询
            SetCommonQueryValues("up_list_MbrCardType", "");
            return View();
        }
        #endregion     

        #region  添加
        /// <summary>
        /// 添加会员卡类型
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new MbrCardTypeAddViewModel() { CardFee = 0});
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(MbrCardTypeAddViewModel mbrcdtpViewModel)
        {
            var currentInfo = GetService<ICurrentInfo>();

            var hid = currentInfo.HotelId;
            if (IsMbrCardSeqit(hid, mbrcdtpViewModel.Seqid, null))
            {
                return Json(JsonResultData.Failure("会员等级已经存在"));
            }
            return _Add(mbrcdtpViewModel, new MbrCardType { Hid = hid, Id = hid + mbrcdtpViewModel.Code }, OpLogType.会员卡类型增加);
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改会员卡类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new MbrCardTypeEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(MbrCardTypeEditViewModel model)
        {
            if (IsMbrCardSeqit(CurrentInfo.HotelId, model.Seqid, model.Id))
                return Json(JsonResultData.Failure("会员等级已经存在"));
            return _Edit(model, new MbrCardType(), OpLogType.会员卡类型修改);
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IMbrCardTypeService>(), OpLogType.会员卡类型删除);
        }
        #endregion
        #region 下拉绑定
        [AuthButton(AuthFlag.None)]
        public JsonResult GetIsSelectList()
        {
            Collection<SelectListItem> list = new Collection<SelectListItem>() {
                   new SelectListItem() { Value = true.ToString(), Text = "是", Selected = true },
                   new SelectListItem() { Value = false.ToString(), Text = "否", Selected = false }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 绑定状态下拉框
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetStatusSelectList()
        {
            var statusList = EnumExtension.ToSelectList(typeof(EntityStatus), EnumValueType.Value, EnumValueType.Text);
            return Json(statusList, JsonRequestBehavior.AllowGet);
        }

        [AuthButton(AuthFlag.None)]
        public JsonResult GetMbrCardTypeSelectList()
        {
            var _mbrCardTypeService = GetService<IMbrCardTypeService>();
            return Json(_mbrCardTypeService.List(CurrentInfo.IsGroup ? CurrentInfo.GroupId : CurrentInfo.HotelId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取价格代码的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetRates()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var rserv = GetService<IRateService>();
            List<Rate> ratelist = rserv.GetRateref(currentInfo.HotelId);
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in ratelist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 会员等级是否存在
        public bool IsMbrCardSeqit(string hid, int seqit, string id)
        {
            return GetService<IMbrCardTypeService>().IsMbrCardTypeSeqit(hid, seqit, id);
        }
        #endregion
    }
}