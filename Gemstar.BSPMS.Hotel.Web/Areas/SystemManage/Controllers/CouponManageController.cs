using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.CouponManage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Common.Services.Enums;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 优惠券设置
    /// </summary>
    [AuthPage("99025005")]
    [AuthPage(ProductType.Member, "m61050005")]
    [AuthBasicData("20")]
    [BusinessType("优惠券设置")]
    public class CouponManageController : BaseEditInWindowController<Coupon, ICouponService>
    {
        #region 查询
        // GET: SystemManage/PayWay

        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_itemCoupon", "");
            return View();
        }
        #endregion

        #region 新增
        /// <summary>
        /// 新增的视图    
        /// </summary>
        /// <param name="pcid">消费类型编号</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(string cid)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var serv = GetService<IChannelService>();
            List<RoomType> alist = serv.GetRoomType(currentInfo.HotelId,true);
            string[] listCompanyTypes = new string[alist.Count];
            for (int i = 0; i < alist.Count; i++)
            {
                listCompanyTypes[i] = alist[i].Id;
            }
            ViewBag.rtp = listCompanyTypes;
            return _Add(new CouponAddViewModel() { ItemTypeid = cid});
        }
        /// <summary>
        /// 新增功能
        /// </summary>
        /// <param name="couponViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(CouponAddViewModel model)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hotelserver = GetService<IItemService>();
            var server = GetService<ICouponService>();
            string itemname = hotelserver.GetCodeListFornameBycode("28", currentInfo.HotelId, model.ItemTypeid);//优惠券类别
            model.ItemTypeName = itemname;
            return _Add(model, new Coupon
            {
                Hid = currentInfo.HotelId,
                Id = currentInfo.HotelId + model.Code,
                Status = EntityStatus.禁用,

            }, OpLogType.优惠券添加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {            
            CouponEditViewModel model = new CouponEditViewModel();
            ActionResult returnActionResult = _Edit(id, model);
            string[] listRoomTypeids = new string[] { };
            if (!string.IsNullOrWhiteSpace(model.RoomTypeids))
            {
                listRoomTypeids = model.RoomTypeids.Split(',');
            }
            ViewBag.listRoomTypeids = listRoomTypeids;
            return _Edit(id, new CouponEditViewModel() { });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(CouponEditViewModel model)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hotelserver = GetService<IItemService>();
            string itemname = hotelserver.GetCodeListFornameBycode("28", currentInfo.HotelId, model.ItemTypeid);//优惠券类别
            model.ItemTypeName = itemname;
            //根据付款类型id查询付款类型名称
            return _Edit(model, new Coupon(){
                ItemTypeName = itemname
            }, OpLogType.优惠券修改);
        }
        #endregion

        #region 启用禁用删除
        //启用
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            var _hotelService = GetService<ICouponService>();
            return Json(_hotelService.Enable(id));
        }
        //禁用
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            var _hotelService = GetService<ICouponService>();
            if (!_hotelService.isExistTicket(id))
                return Json(JsonResultData.Failure("发放过优惠券的不可禁用"));
            return Json(_hotelService.Disable(id));
        }
        //删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            var _hotelService = GetService<ICouponService>();
            if (!_hotelService.isExistTicket(id))
                return Json(JsonResultData.Failure("发放过优惠券的不可删除"));
            return _BatchDelete(id, GetService<ICouponService>(), OpLogType.优惠券删除);
        }
        #endregion
        
        #region 下拉列表
        /// <summary>
        /// 获取优惠券类型的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForCoupon()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hotelserver = GetService<IItemService>();
            List<CodeList> alist = hotelserver.GetCodeList("28", currentInfo.IsGroup? currentInfo.GroupId:currentInfo.HotelId);//优惠券类型
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Code.ToString() + "    " + item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }        
        /// <summary>
        /// 获取房间类型的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public JsonResult listItemForRoomType()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var serv = GetService<IChannelService>();
            List<RoomType> alist = serv.GetRoomTypeforChanelValid(currentInfo.HotelId);
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}