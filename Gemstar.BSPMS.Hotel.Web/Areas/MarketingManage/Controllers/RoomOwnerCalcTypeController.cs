using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.RoomOwnerCalcType;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Controllers
{  
    /// <summary>
    /// 分成类型定义
    /// </summary>
    [AuthPage("61091002")]
    [BusinessType("分成类型定义")]
    public class RoomOwnerCalcTypeController : BaseEditInWindowController<RoomOwnerCalcType, IRoomOwnerCalcTypeService>
    {
        #region 查询
        // GET: MarketingManage/bookingNotes
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_RoomOwnerCalcType", "");
            return View();
        }
        #endregion

        #region 增加
        /// <summary>
        /// 增加分成类型
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            setRoomOwnerCalcParas();

            return _Add(new RoomOwnerCalcTypeAddViewModel());
        }

        private void setRoomOwnerCalcParas()
        {
            string hid = CurrentInfo.HotelId;
            var calcParaService = GetService<IRoomOwnerCalcParaDefineService>();
            var roomownerFixed = calcParaService.ListbyParatype(hid, "fixed");//固定参数
            var roomownerRoom = calcParaService.ListbyParatype(hid, "room");//房间参数
            var roomownerRoomtype = calcParaService.ListbyParatype(hid, "roomtype");//房间类型参数
            var roomOwnerCalcFunctions = calcParaService.GetCalcFunctions();

            var itemService = GetService<IItemService>();
            var OwnerfeeItem = itemService.getOwnerfeeItem(hid);//房间费用

            ViewBag.roomownerFixed = roomownerFixed;
            ViewBag.roomownerRoom = roomownerRoom;
            ViewBag.roomownerRoomtype = roomownerRoomtype;
            ViewBag.roomOwnerCalcFunctions = roomOwnerCalcFunctions;
            ViewBag.OwnerfeeItem = OwnerfeeItem;
        }

        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        [JsonException]
        public ActionResult Add(RoomOwnerCalcTypeAddViewModel RoomOwnerCalcTypesViewModel)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            RoomOwnerCalcTypesViewModel.Hid = hid;
            RoomOwnerCalcTypesViewModel.TypeId = Guid.NewGuid();
            return _Add(RoomOwnerCalcTypesViewModel, new RoomOwnerCalcType { }, OpLogType.分成类型定义增加);
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
            setRoomOwnerCalcParas();

            return _Edit(Guid.Parse(id), new RoomOwnerCalcTypeEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(RoomOwnerCalcTypeEditViewModel model)
        {
         
            return _Edit(model, new RoomOwnerCalcType() { }, OpLogType.分成类型定义修改);
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            var roominfos = GetService<IRoomOwnerRoomInfosService>();
            var aa=roominfos.getOwnerRoomListbycalcTypeId(Guid.Parse(id));
            if (aa.Count > 0)
            {
                return Json(JsonResultData.Failure("此分成类型已使用，不允许删除！"));
            }
            return _BatchDelete(id, GetService<IRoomOwnerCalcTypeService>(), OpLogType.分成类型定义删除);
        }
        #endregion

    }
}