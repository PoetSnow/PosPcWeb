using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RoomManage;
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
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 房间资料管理
    /// </summary>
    [AuthPage("99006")]
    public class RoomManageController : BaseEditIncellController<Room, IRoomService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            string floorId = "0";
            SetCommonQueryValues("up_list_pmsRoom", "@h99floorid=" + floorId);
            var service = GetService<IRoomTypeService>();
            var datas = service.List(CurrentInfo.HotelId);
            var data = datas.Select(s => new SelectListItem { Value = s.Key, Text = s.Value }).ToList();
            ViewData["RoomTypeid_Data"] = new SelectList(data, "Value", "Text"); 
            var _roomTypeService = GetService<ICodeListService>();
            var lists = _roomTypeService.GetRoomFeatures(CurrentInfo.HotelId);
            var list = lists.Select(s => new SelectListItem { Value = s.Name, Text = s.Name }).ToList();
            ViewData["Feature_Data"] = new SelectList(list, "Value", "Text");
            var masterDb = GetService<DbCommonContext>();
            var m = GetService<IChannelService>();
            var c = m.getM_v_channelCodes(masterDb);
            ViewData["channelCode_Data"] = new SelectList(c, "code", "name"); 
            ViewBag.ishotel = !CurrentInfo.IsGroupInGroup;
            if (CurrentInfo.IsGroupInGroup)
            {
                var pmsHotelService = GetService<IPmsHotelService>();
                ViewBag.Hotels = pmsHotelService.GetHotelsInGroup(CurrentInfo.GroupId);
            }
            return View();
        }
        #endregion

        #region 增加
        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<Room> addVersions)
        {

            var floorId = Session["floorId"].ToString();
            var data = _Add(request, addVersions, roomViewModel =>
             {
                 roomViewModel.Floorid = floorId;
                 if (!CheckFloorId(roomViewModel.Floorid))
                 {
                     throw new Exception("楼层ID错误，请关闭后重试");
                 }
                 var floorEntity = GetService<ICodeListService>().GetCodeListByID(roomViewModel.Floorid);
                 if (floorEntity == null || floorEntity.Hid != CurrentInfo.HotelId || floorEntity.TypeCode != "06")
                 {
                     throw new Exception("楼层ID错误，请关闭后重试");
                 }
                 if (GetService<IRoomService>().IsExistsRoomNo(CurrentInfo.HotelId, roomViewModel.RoomNo, null))
                 {
                     throw new Exception("此房间号已存在，请修改[" + roomViewModel.RoomNo + "]");
                 }
                 RoomType roomTypeEntity = GetService<IRoomTypeService>().Get(roomViewModel.RoomTypeid);
                 if (roomTypeEntity == null || roomTypeEntity.Hid != CurrentInfo.HotelId)
                 {
                     throw new Exception("请选择房间类型");
                 }
                 roomViewModel.Id = CurrentInfo.HotelId + roomViewModel.RoomNo;
                 roomViewModel.Hid = CurrentInfo.HotelId;
                 roomViewModel.Status = EntityStatus.禁用;
                 roomViewModel.RoomTypeCode = roomTypeEntity.Code;
                 roomViewModel.RoomTypeName = roomTypeEntity.Name;
                 roomViewModel.RoomTypeShortName = roomTypeEntity.ShortName;
                 roomViewModel.FloorName = floorEntity.Name;
             }, OpLogType.房间增加);
            UpdateRoomStatusReset();
            return data;
            // return result;
        }
        #endregion

        #region 批量增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult BatchAdd(string floorId)
        {
            var codelist = GetService<ICodeListService>();
            CodeList cl = codelist.Get(int.Parse(floorId)); 
            ViewBag.floorname = cl.Name; ViewBag.floorid = cl.Id;
            return PartialView("_BatchAdd");
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult BatchAdd(List<RoomBatchAddViewModel> roomsadd, string log)
        {
            var service = GetService<IRoomService>();
            for (int i = 0; i < roomsadd.Count; i++)
            {
                var data = new Room
                {
                    Status = EntityStatus.禁用,
                    Hid = CurrentInfo.HotelId,
                    Id = CurrentInfo.HotelId + roomsadd[i].RoomNo,
                    RoomNo = roomsadd[i].RoomNo,
                    Floorid = roomsadd[i].Floorid,
                    Lockid = roomsadd[i].Lockid,
                    Tel = roomsadd[i].Tel,
                    RoomTypeid = roomsadd[i].RoomTypeid,
                    FloorName = roomsadd[i].FloorName,
                    RoomTypeName = roomsadd[i].RoomTypeName
                };
                service.Add(data);
            }
            service.Commit();
            service.BatchUpdateRoomTypeCode(CurrentInfo.HotelId);
            UpdateRoomStatusReset();
            AddOperationLog(OpLogType.房间批量增加, log);
            return Json(JsonResultData.Successed());
        }

        #endregion

        #region 修改
        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<Room> updatedVersions, [Bind(Prefix = "originModels")]IEnumerable<Room> originVersions)
        {
            var floorId = Session["floorId"].ToString();
            var data = _Update(request, updatedVersions, originVersions, (list, u) =>
            {
                var entity = list.SingleOrDefault(s => s.Id == u.Id && s.Hid == CurrentInfo.HotelId);
                if (entity == null || entity.Hid != CurrentInfo.HotelId)
                {
                    throw new Exception("错误信息，请关闭后重试");
                }
                if (GetService<IRoomService>().IsExistsRoomNo(CurrentInfo.HotelId, entity.RoomNo, entity.Id))
                {
                    throw new Exception("此房间号已存在，请修改 [" + entity.RoomNo + "]");
                }
                RoomType roomTypeEntity = GetService<IRoomTypeService>().Get(u.RoomTypeid);
                if (roomTypeEntity == null || roomTypeEntity.Hid != CurrentInfo.HotelId)
                {
                    throw new Exception("请选择房间类型");
                }
                u.RoomTypeName = roomTypeEntity.Name;
                u.RoomTypeShortName = roomTypeEntity.ShortName;
                u.RoomTypeCode = roomTypeEntity.Code;
                return entity;
            }, OpLogType.房间修改);
            UpdateRoomStatusReset();
            return data;
        }
        #endregion

        #region 启用禁用
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(JsonResultData.Failure("请指定要修改的记录id，多项之间以逗号分隔"));
            }
            var _roomService = GetService<IRoomService>();
            string log = _roomService.BatchUpdateStatus(id, EntityStatus.启用);
            var floorId = Session["floorId"].ToString();
            var codelist = GetService<ICodeListService>();
            if (log != "") { AddOperationLog(OpLogType.房间启用禁用, "楼层：" + codelist.GetFloorName(floorId) + "　房间号" + log.Trim('、') + "禁用改为启用"); };
            UpdateRoomStatusReset();
            return Json(JsonResultData.Successed(""));
        }
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(JsonResultData.Failure("请指定要修改的记录id，多项之间以逗号分隔"));
            }
            var _roomService = GetService<IRoomService>();
            string log = _roomService.BatchUpdateStatus(id, EntityStatus.禁用);
            var floorId = Session["floorId"].ToString();
            var codelist = GetService<ICodeListService>();
            if (log != "") { AddOperationLog(OpLogType.房间启用禁用, "楼层：" + codelist.GetFloorName(floorId) + "　房间号" + log.Trim('、') + "启用改为禁用"); };
            UpdateRoomStatusReset();
            return Json(JsonResultData.Successed(""));
        }

        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IRoomService>(), OpLogType.房间删除);
        }
        #endregion
        #region 其他
        /// <summary>
        /// 获取房间特色列表
        /// </summary>
        /// <returns></returns>
        private void FeatureList()
        {
            var _codeListService = GetService<ICodeListService>();
            List<CodeList> list = _codeListService.GetRoomFeatures(CurrentInfo.HotelId);

            Collection<String> featureSelectList = new Collection<String>();
            foreach (var item in list)
            {
                featureSelectList.Add(item.Name);
            }
            ViewBag.FeatureList = featureSelectList;
        }

        /// <summary>
        /// 在房间增删改后，调用
        /// </summary>
        /// <param name="hid">酒店ID</param>
        private void UpdateRoomStatusReset()
        {
            var _roomService = GetService<IRoomService>();
            _roomService.UpdateRoomStatusReset(CurrentInfo.HotelId);
        }

        protected override void AfterDeleteCommit()
        {
            UpdateRoomStatusReset();
        }
        /// <summary>
        /// 检查楼层ID
        /// </summary>
        /// <param name="floorId">楼层ID</param>
        /// <returns></returns>
        private bool CheckFloorId(string floorId)
        {
            if (string.IsNullOrWhiteSpace(floorId))
            {
                return false;
            }
            string typeCode = "06";//房间楼层
            if (!floorId.StartsWith(CurrentInfo.HotelId + typeCode))
            {
                return false;
            }
            if (!GetService<ICodeListService>().IsExists(CurrentInfo.HotelId, typeCode, floorId.Substring((CurrentInfo.HotelId + typeCode).Length)))
            {
                return false;
            }
            return true;
        }
        #endregion
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public void GetFloorId(string floorId)
        {
            Session["floorId"] = GetService<ICodeListService>().Get(int.Parse(floorId)).Id;
        }

        [HttpGet]
        [AuthButton(AuthFlag.None)]
        public ActionResult getChannelCodeName(string values)
        {
            var masterDb = GetService<DbCommonContext>();
            var m = GetService<IChannelService>();
            M_v_channelCode c = m.getM_v_channelCodes(masterDb).Where(w=>w.Code==values).FirstOrDefault();
            if (c != null)
            {
                return Json(JsonResultData.Successed(c.Name), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonResultData.Failure(""), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
