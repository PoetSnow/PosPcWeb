using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.Channel;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.NotifyManage;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Controllers
{
    /// <summary>
    /// 渠道设置
    /// </summary>
    [AuthPage("61030")]
    [BusinessType("渠道设置")]
    public class ChannelController : BaseEditInWindowController<Channel, IChannelService>
    {
        #region 查询
        // GET: MarketingManage/Channel         
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            //	--同步分库中的渠道信息（channel）
            var channel = GetService<IChannelService>();
            var masterDb = GetService<DbCommonContext>();
            channel.resetChannel(CurrentInfo.HotelId, masterDb);
            SetCommonQueryValues("up_list_channel", "");
            return View();
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改渠道
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            ChannelEditViewModel model = new ChannelEditViewModel();
            ActionResult returnActionResult = _Edit(id, model);
            string[] list = new string[] { };
            if (!string.IsNullOrWhiteSpace(model.Roomtypeid))
            {
                list = model.Roomtypeid.Split(',');
            }
            ViewBag.SelectRoomtypeid = list;
            if (!string.IsNullOrWhiteSpace(model.Customerservice))
            {
                list = model.Customerservice.Split(',');
            }
            else
            {
                list = "".Split(',');
            }
            ViewBag.SelectCustomerservice = list;
            //判断是否是众荟渠道的，是则取出众荟渠道参数，以便前端显示自助匹配链接地址让酒店用户操作
            var dbCommon = GetService<DbCommonContext>();
            var channelParas = dbCommon.M_v_channelParas.ToList();
            var channelService = GetService<IChannelService>();
            var channelUrl = channelService.GetZHMappingUrl(CurrentInfo.HotelId, id, CurrentInfo.UserCode, MvcApplication.IsTestEnv, channelParas);

            ViewBag.channelUrl = channelUrl;
            return returnActionResult;
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(ChannelEditViewModel model)
        {
            var MasterService = GetService<IMasterService>();
            int i = MasterService.updateHotelChannel(model.Hid, model.Code, model.Refno);
            var reval = _Edit(model, new Channel() { }, OpLogType.渠道修改);
            //设置保留房的可用性
            var serv = GetService<IChannelService>();
            serv.RoomTypeSet(model.Hid);
            //重置房型房量
            var _roomService = GetService<IRoomService>();
            _roomService.UpdateRoomStatusReset(model.Hid);
            return reval;
        }
        protected override void AfterEditedAndSaved(Channel data)
        {
            var notifyService = GetService<INotifyService>();
            notifyService.NotifyOtaInfo(data.Hid, MvcApplication.IsTestEnv, data.Id);
        }
        #endregion

        #region 下拉框绑定

        /// <summary>
        /// 获取市场分类的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public JsonResult listItemForMarketid()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            var hotelserver = GetService<IItemService>();
            List<CodeList> alist = hotelserver.GetCodeList("04", hid);//市场分类
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取担保和预付记账代码的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public JsonResult listItemForPayItemid()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            var hotelserver = GetService<IItemService>();
            List<Item> alist = hotelserver.GetItem(hid, "C");//付款方式
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name.ToString() });
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
        /// <summary>
        /// 获取客服操作员的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public JsonResult listItemForCustomerservice()
        {
            var userService = GetService<IPmsUserService>();
            List<PmsUser> alist = userService.UsersInGroup(CurrentInfo.HotelId);
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Name.ToString(), Text = item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IChannelService>(), OpLogType.渠道删除);
        }
        #endregion
    }
}