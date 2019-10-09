using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF.MbrCardCenter;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Hotel.Services.MbrCardCenter;
using Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.RoomOwnerRoomInfos;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Controllers
{
    /// <summary>
    ///  业主房间委托
    /// </summary>
    [AuthPage("30002")]
    [BusinessType("业主房间委托")]
    public class RoomOwnerRoomInfosController : BaseEditInWindowController<RoomOwnerRoomInfos, IRoomOwnerRoomInfosService>
    {
        // GET: MarketingManage/RoomOwnerRoomInfos
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_RoomOwnerRoomInfos", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            ViewBag.roompara = getRoompara();
            ViewBag.desc = getdesc("");
            ViewBag.feature = getFeature("");
            ViewBag.rtname = getrtname(""); 
            return _Add(new RoomOwnerRoomInfosAddViewModel() { Hid = CurrentInfo.HotelId, RoomInfoId = Guid.NewGuid() });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(RoomOwnerRoomInfosAddViewModel companyViewModel)
        {
            var service = GetService<IRoomService>();
            companyViewModel.RoomId = (companyViewModel.RoomId == null ? companyViewModel.RoomNo : companyViewModel.RoomId);
            companyViewModel.RoomNo = service.Get(companyViewModel.RoomId).RoomNo;

            return _Add(companyViewModel, new RoomOwnerRoomInfos
            {
            }, OpLogType.业主房间委托增加);
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改业主房间委托
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns> 
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            var serv = GetService<IRoomOwnerRoomInfosService>();
            var entity = serv.Get(Guid.Parse(id));
            ViewBag.roompara = getRoompara();
            ViewBag.roomlist = getOwnerRoomNo(entity.ProfileId);
            ViewBag.desc = getdesc(entity.CalcTypeId.ToString());
            ViewBag.feature = getFeature(entity.RoomId.ToString());
            return _Edit(Guid.Parse(id), new RoomOwnerRoomInfosEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(RoomOwnerRoomInfosEditViewModel model)
        {
            var service = GetService<IRoomService>();
            model.RoomId = (model.RoomId == null ? model.RoomNo : model.RoomId);
            if (service.Get(model.RoomId) == null)
            {
                model.RoomId = model.RoomNo;
            }
            model.RoomNo = service.Get(model.RoomId).RoomNo;
            //修改房号同时要修改费用记录中的房号 
            return _Edit(model, new RoomOwnerRoomInfos() { }, OpLogType.业主房间委托修改);
        }
        #endregion

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Editnew(string id)
        {
            var serv = GetService<IRoomOwnerRoomInfosService>();
            var entity = serv.Get(Guid.Parse(id));
            ViewBag.roompara = getRoompara();
            ViewBag.roomlist = getOwnerRoomNo(entity.ProfileId);
            return _Edit(Guid.Parse(id), new RoomOwnerRoomInfosEditViewModel());
        }

        #region 获取业主姓名列表
        [AuthButton(AuthFlag.None)]
        public JsonResult GetProfileIsOwnerSelectList(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                text = Request.QueryString.Get("filter[filters][0][value]");
            }
            var m = GetService<IMbrCardService>();
            List<MbrCard> c = m.ListforOwner(CurrentInfo.IsGroup?CurrentInfo.GroupId:CurrentInfo.HotelId, text);
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            for (int i = 0; i < c.Count(); i++)
            {
                list.Add(new SelectListItem() { Value = c[i].Id.ToString(), Text = c[i].GuestName });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 获取分成类型列表
        [AuthButton(AuthFlag.None)]
        public JsonResult GetRoomOwnerCalcTypeList()
        {
            var m = GetService<IRoomOwnerCalcTypeService>();
            List<RoomOwnerCalcType> c = m.List(CurrentInfo.HotelId).OrderBy(w => w.SeqId).ToList();
            Collection<SelectListItem> list = new Collection<SelectListItem>();

            for (int i = 0; i < c.Count(); i++)
            {
                list.Add(new SelectListItem() { Value = c[i].TypeId.ToString(), Text = c[i].TypeName });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 获取所选分成类型的说明
        [AuthButton(AuthFlag.None)]
        public ActionResult getCalctypeDescrib(string typeid)
        {
            var m = GetService<IRoomOwnerCalcTypeService>();
            string c = m.Get(Guid.Parse(typeid)).TypeDesc;
            return Json(JsonResultData.Successed(c));
        }
        #endregion

        #region 获取没有委托出去的房间编号的列表
        [AuthButton(AuthFlag.None)]
        public ActionResult getRoomlist(string text, string CurRoomid)
        {
            var m = GetService<IRoomService>();
            var roomlist = m.roomforisOwner(CurrentInfo.HotelId, CurRoomid, text);
            Collection<SelectListItem> list = new Collection<SelectListItem>();

            if (CurRoomid != null)
            {
                Room r = roomlist.Where(w => w.Id == CurRoomid).FirstOrDefault();
                if (r != null)
                {
                    list.Add(new SelectListItem() { Value = CurRoomid, Text = r.RoomNo });
                }
            }
            for (int i = 0; i < roomlist.Count(); i++)
            {
                if (roomlist[i].Id.ToString() != CurRoomid)
                {
                    list.Add(new SelectListItem() { Value = roomlist[i].Id.ToString(), Text = roomlist[i].RoomNo });
                }

            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 获取房间参数
        /// </summary>
        /// <returns></returns>
        public List<RoomOwnerCalcParaDefine> getRoompara()
        {
            var m = GetService<IRoomOwnerCalcParaDefineService>();
            List<RoomOwnerCalcParaDefine> roompara = m.ListbyParatype(CurrentInfo.HotelId, "room");
            return roompara;
        }
        /// <summary>
        /// 获取当前业主的委托房间列表
        /// </summary>
        /// <param name="profileid"></param>
        /// <returns></returns>
        public List<RoomOwnerRoomInfos> getOwnerRoomNo(Guid profileid)
        {
            var serv = GetService<IRoomOwnerRoomInfosService>();
            List<RoomOwnerRoomInfos> roominfo = serv.getOwnerRoomList(CurrentInfo.HotelId, profileid);
            return roominfo;
        }
        /// <summary>
        /// 修改委托房间列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult setRoomlist(string profileid)
        {
            List<RoomOwnerRoomInfos> roomnolist = getOwnerRoomNo(Guid.Parse(profileid));
            return Json(JsonResultData.Successed(roomnolist));
        }
        /// <summary>
        /// 默认的第一个分成说明
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public string getdesc(string id)
        {
           
            var m = GetService<IRoomOwnerCalcTypeService>();
            List<RoomOwnerCalcType> c = null;
            if (id == "")
            {
                c = m.List(CurrentInfo.HotelId);
                if (c.Count() > 0)
                {
                    return c[0].TypeDesc;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                RoomOwnerCalcType rc = m.Get(Guid.Parse(id));
                
                return m.Get(Guid.Parse(id)).TypeDesc;
            }
        }
        /// <summary>
        /// 默认的第一个房号的特色
        /// </summary>
        /// <returns></returns> 
        public string getFeature(string id)
        {
            var m = GetService<IRoomService>();
            List<Room> c = null;
            if (id == "")
            {
                c = m.roomforisOwner(CurrentInfo.HotelId, id, "");
                if (c.Count() > 0)
                {
                    return c[0].Feature;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return m.Get(id).Feature;
            }
            
        }
        /// <summary>
        /// 默认的第一个房号的房型
        /// </summary>
        /// <returns></returns> 
        public string getrtname(string id)
        {
            var m = GetService<IRoomService>();
            List<Room> c = null;
            if (id == "")
            {
                c = m.roomforisOwner(CurrentInfo.HotelId, id, "");
                if (c.Count() > 0)
                {
                    return c[0].RoomTypeName;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return m.Get(id).RoomTypeName;
            }

        }
        #region 获取所选分成类型的说明
        [AuthButton(AuthFlag.None)]
        public ActionResult getRoomFeature(string id)
        {
            var m = GetService<IRoomService>();
            Room rm = m.Get(id);
             
            return Json(JsonResultData.Successed(rm.Feature+"_"+rm.RoomTypeName));
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IRoomOwnerRoomInfosService>(), OpLogType.业主房间委托删除);
        }
        #endregion
    }
}