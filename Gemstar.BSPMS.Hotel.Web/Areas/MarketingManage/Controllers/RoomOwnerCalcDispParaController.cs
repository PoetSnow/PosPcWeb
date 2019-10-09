using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.RoomOwnerCalcDispPara;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Controllers
{
    [AuthPage("61091003")]
    [BusinessType("分成展示项目定义")]
    public class RoomOwnerCalcDispParaController : BaseEditInWindowController<RoomOwnerCalcDispPara, IRoomOwnerCalcDispParaService>
    {
        /// <summary>
        /// 分成展示项目定义                      
        /// </summary>
        // GET: MarketingManage/RoomOwnerCalcDispPara 
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_RoomOwnerCalcDispPara", "");
            return View();
        }
        #region 增加
        /// <summary>
        /// 增加分成类型
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            setRoomOwnerCalcParas();

            return _Add(new RoomOwnerCalcDispParaAddViewModel() { isHidden = false });
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
        public ActionResult Add(RoomOwnerCalcDispParaAddViewModel RoomOwnerCalcDispParasViewModel)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            RoomOwnerCalcDispParasViewModel.Hid = hid;
            RoomOwnerCalcDispParasViewModel.TypeId = Guid.NewGuid();
            return _Add(RoomOwnerCalcDispParasViewModel, new RoomOwnerCalcDispPara { }, OpLogType.分成展示项目定义增加);
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

            return _Edit(Guid.Parse(id), new RoomOwnerCalcDispParaEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(RoomOwnerCalcDispParaEditViewModel model)
        {

            return _Edit(model, new RoomOwnerCalcDispPara() { }, OpLogType.分成展示项目定义修改);
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IRoomOwnerCalcDispParaService>(), OpLogType.分成展示项目定义删除);
        }
        #endregion


        /// <summary>
        /// 获取类别的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetdataTypeList()
        {  
            List<V_dataType> dt = GetService<IRoomOwnerCalcParaDefineService>().getDataType();
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in dt)
            {
                list.Add(new SelectListItem() { Value = item.Code.ToString(), Text = item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        } 
    }
}