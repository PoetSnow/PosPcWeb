using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RoomTypeManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Extensions;
using System.Collections.ObjectModel;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Common.Services.Enums;
using System.Web;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Common.Tools;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 房间类型维护
    /// </summary>
    [AuthPage("99005")]
    [AuthBasicData(M_V_BasicDataType.BasicDataCodeRoomType)]
    public class RoomTypeManageController : BaseEditInWindowController<RoomType, IRoomTypeService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            if (CurrentInfo.IsGroupInGroup)
            {
                //集团分发型资料，列表需要不同，集团需要显示分店名称，并且查询条件中需要有分店可以选择
                SetCommonQueryValues("up_list_pmsRoomType_group", "@s23分店=" + CurrentInfo.HotelId);
                return View("IndexGroup");
            }
            else
            {
                SetCommonQueryValues("up_list_pmsRoomType", "");
                return View();
            }
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            var _sysParaService = GetService<ISysParaService>();
            var qiniuPara = _sysParaService.GetQiniuPara();
            ViewBag.Domain = qiniuPara.ContainsKey("domain") ? qiniuPara["domain"] : "http://res.gshis.com/";
            List<RoomOwnerCalcParaDefine> roomownerRoomtype = GetService<IRoomOwnerCalcParaDefineService>().ListbyParatype(CurrentInfo.HotelId, "roomtype");
            ViewBag.roomtypepara = roomownerRoomtype;

            if (CurrentInfo.IsGroupInGroup)
            {
                return _AddGroup(new RoomTypeGroupAddViewModel(), M_V_BasicDataType.BasicDataCodeRoomType);
            }
            else
            {
                return _Add(new RoomTypeAddViewModel());
            }
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(RoomTypeAddViewModel roomTypeViewModel)
        {
            var roomTypeService = GetService<IRoomTypeService>();
            var entity = roomTypeService.Get(CurrentInfo.HotelId + roomTypeViewModel.Code);
            if (entity != null)
            {
                return Json(JsonResultData.Failure("代码已存在，请重新输入。"));
            }
            //房型图片保存到房型图片表中
            roomTypeService.AddRoomtypePic(CurrentInfo.HotelId, CurrentInfo.HotelId + roomTypeViewModel.Code, roomTypeViewModel.PicAdd);
            return _Add(roomTypeViewModel, new RoomType
            {
                Hid = CurrentInfo.HotelId,
                Id = CurrentInfo.HotelId + roomTypeViewModel.Code,
                Code = roomTypeViewModel.Code,
                Name = roomTypeViewModel.Name,
                ShortName = roomTypeViewModel.ShortName,
                Price = roomTypeViewModel.Price,
                Count = roomTypeViewModel.Count,
                MaxCount = roomTypeViewModel.MaxCount,
                IsAdd = roomTypeViewModel.IsAdd,
                ChanelValid = roomTypeViewModel.ChanelValid,
                IsClose = roomTypeViewModel.IsClose,
                IsNotRoom = roomTypeViewModel.IsNotRoom,
                Status = EntityStatus.禁用,
                TotalRooms = 0,
                OverQauntity = roomTypeViewModel.OverQauntity,
                Seqid = roomTypeViewModel.Seqid,
                PicAdd = roomTypeViewModel.PicAdd,
                DataSource = BasicDataDataSource.Added.Code
            }, OpLogType.房型增加);
        }
        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        [JsonException]
        public ActionResult AddGroup(RoomTypeGroupAddViewModel roomTypeViewModel)
        {
            var hid = CurrentInfo.GroupId;
            var roomTypeService = GetService<IRoomTypeService>();

            var entity = roomTypeService.Get(hid + roomTypeViewModel.Code);
            if (entity != null)
            {
                return Json(JsonResultData.Failure("代码已存在，请重新输入。"));
            }
            //房型图片保存到房型图片表中
            roomTypeService.AddRoomtypePic(hid, hid + roomTypeViewModel.Code, roomTypeViewModel.PicAdd);
            return _AddGroup(roomTypeViewModel, new RoomType
            {
                Hid = hid,
                Id = hid + roomTypeViewModel.Code,
                Code = roomTypeViewModel.Code,
                Name = roomTypeViewModel.Name,
                ShortName = roomTypeViewModel.ShortName,
                Price = roomTypeViewModel.Price,
                Count = roomTypeViewModel.Count,
                MaxCount = roomTypeViewModel.MaxCount,
                IsAdd = roomTypeViewModel.IsAdd,
                ChanelValid = roomTypeViewModel.ChanelValid,
                IsClose = roomTypeViewModel.IsClose,
                IsNotRoom = roomTypeViewModel.IsNotRoom,
                Status = EntityStatus.禁用,
                TotalRooms = 0,
                OverQauntity = roomTypeViewModel.OverQauntity,
                Seqid = roomTypeViewModel.Seqid,
                PicAdd = roomTypeViewModel.PicAdd,
                DataSource = BasicDataDataSource.Added.Code
            }, OpLogType.房型增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            //首先需要判断对应记录是否是自主增加的，如果是自主增加的则始终允许修改，如果是分发的，则根据集团设置的是否允许修改来判断
            var entity = GetService<IRoomTypeService>().Get(id);
            var editResult = _CanEdit(entity, M_V_BasicDataType.BasicDataCodeRoomType);
            if (editResult != null)
            {
                return editResult;
            }
            //可以修改
            var _sysParaService = GetService<ISysParaService>();
            var qiniuPara = _sysParaService.GetQiniuPara();
            ViewBag.Domain = qiniuPara.ContainsKey("domain") ? qiniuPara["domain"] : "http://res.gshis.com/";
            List<RoomOwnerCalcParaDefine> roomownerRoomtype = GetService<IRoomOwnerCalcParaDefineService>().ListbyParatype(CurrentInfo.HotelId, "roomtype");
            ViewBag.roomtypepara = roomownerRoomtype;

            //单店的才允许修改房型代码
            ViewBag.canEditCode = CurrentInfo.IsGroup ? false : true;

            if (CurrentInfo.IsGroupInGroup)
            {
                return _EditGroup(id, new RoomTypeGroupEditViewModel(), M_V_BasicDataType.BasicDataCodeRoomType);
            }
            else
            {
                return _Edit(id, new RoomTypeEditViewModel());
            }
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(RoomTypeEditViewModel model)
        {
            var _roomTypeService = GetService<IRoomTypeService>();
            var entity = _roomTypeService.Get(model.Id);
            if (entity == null || entity.Hid != CurrentInfo.HotelId)
            {
                return Json(JsonResultData.Failure("错误信息，请重试。"));
            }
            if (model.Id != (CurrentInfo.HotelId + model.Code))
            {
                if (GetService<IRoomService>().IsExistsRoomType(CurrentInfo.HotelId, entity.Id))
                {
                    return Json(JsonResultData.Failure("不能修改当前房间代码，因为有房间属于此类型。"));
                }
                var newEntity = _roomTypeService.Get(CurrentInfo.HotelId + model.Code);
                if (newEntity != null)
                {
                    return Json(JsonResultData.Failure("代码已存在，请重新输入。"));
                }
            }
            ActionResult result = _Edit(model, entity, OpLogType.房型修改);
            EditCompleted(model, result);
            //设置保留房的可用性
            var serv = GetService<IChannelService>();
            serv.RoomTypeSet(CurrentInfo.HotelId);

            //房型图片保存到房型图片表中
            _roomTypeService.AddRoomtypePic(CurrentInfo.HotelId, model.Id, model.PicAdd);
            return result;
        }
        private void EditCompleted(RoomTypeEditViewModel model, ActionResult result)
        {
            var jsonResult = result as JsonResult;
            if (jsonResult != null)
            {
                var dataResult = jsonResult.Data as JsonResultData;
                if (dataResult != null && dataResult.Success)
                {
                    string newid = CurrentInfo.HotelId + model.Code;

                    var roomTypeService = GetService<IRoomTypeService>();
                    var roomType = roomTypeService.Get(newid);

                    var roomService = GetService<IRoomService>();
                    roomService.UpdateRoomType(model.Id, roomType);
                }
            }
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult EditGroup(RoomTypeGroupEditViewModel model)
        {

            var _roomTypeService = GetService<IRoomTypeService>();
            var entity = _roomTypeService.Get(model.Id);
            if (entity == null)
            {
                return Json(JsonResultData.Failure("错误信息，请重试。"));
            }
            //集团的一旦增加后不允许修改代码，只能删除后重新增加，否则由于修改代码引起的麻烦更多
            if (model.Id != (CurrentInfo.HotelId + model.Code))
            {
                return Json(JsonResultData.Failure("集团分发型资料不允许修改代码"));
            }
            ActionResult result = _EditGroup(model, entity, OpLogType.房型修改);
            //由于集团不再允许修改代码，所以不存在需要更改房间中的房型id的问题，不再执行下面的代码
            //EditCompleted(model, result);

            //房型图片保存到房型图片表中
            _roomTypeService.AddRoomtypePic(CurrentInfo.HotelId, model.Id, model.PicAdd);
            return result;
        }
        protected override void AfterEditedGroupAndSaved(List<RoomType> data)
        {
            //设置保留房的可用性
            var serv = GetService<IChannelService>();
            foreach (var r in data)
            {
                if (r != null)
                {
                    serv.RoomTypeSet(r.Hid);
                }
            }
        }
        #endregion

        #region 启用禁用
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            if (CurrentInfo.IsGroup)
            {
                var _roomTypeService = GetService<IRoomTypeService>();
                var reval = _BatchBatchChangeStatusGroup(id, EntityKeyDataType.String, M_V_BasicDataType.BasicDataCodeRoomType, _roomTypeService, EntityStatus.启用, OpLogType.房型启用禁用);
                return reval;
            }
            else
            {
                var _roomTypeService = GetService<IRoomTypeService>();
                var reval = Json(_roomTypeService.BatchUpdateStatus(id, EntityStatus.启用));
                //设置保留房的可用性
                var serv = GetService<IChannelService>();
                serv.RoomTypeSet(CurrentInfo.HotelId);
                return reval;
            }
        }
        protected override void AfterBatchChangeStatusGroupCommit(List<object> deletedModels)
        { //设置保留房的可用性
            var serv = GetService<IChannelService>();
            foreach (RoomType m in deletedModels)
            {
                serv.RoomTypeSet(m.Hid);
            }
        }
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            if (CurrentInfo.IsGroup)
            {
                var _roomTypeService = GetService<IRoomTypeService>();
                var reval = _BatchBatchChangeStatusGroup(id, EntityKeyDataType.String, M_V_BasicDataType.BasicDataCodeRoomType, _roomTypeService, EntityStatus.禁用, OpLogType.房型启用禁用);
                return reval;
            }
            else
            {
                var _roomTypeService = GetService<IRoomTypeService>();
                var reval = Json(_roomTypeService.BatchUpdateStatus(id, EntityStatus.禁用));
                //设置保留房的可用性
                var serv = GetService<IChannelService>();
                serv.RoomTypeSet(CurrentInfo.HotelId);
                return reval;
            }
        }

        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            //同时删除房型图片表中的数据
            var _roomTypeService = GetService<IRoomTypeService>();
            _roomTypeService.AddRoomtypePic(CurrentInfo.HotelId, id, "");

            if (CurrentInfo.IsGroup)
            {
                return _BatchDeleteGroup(id, EntityKeyDataType.String, GetService<IRoomTypeService>(), OpLogType.房型删除);
            }
            else
            {
                return _BatchDelete(id, GetService<IRoomTypeService>(), OpLogType.房型删除);
            }
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

        [AuthButton(AuthFlag.None)]
        public JsonResult GetStatusSelectList()
        {
            var statusSelectList = EnumExtension.ToSelectList(typeof(EntityStatus), EnumValueType.Value, EnumValueType.Text);
            return Json(statusSelectList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 房型下拉列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        [NotAuthAttribute]
        public JsonResult GetRoomTypeSelectList()
        {
            var _roomTypeService = GetService<IRoomTypeService>();
            return Json(_roomTypeService.List(CurrentInfo.HotelId), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 客房用品下拉列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetEquipmentSelectList()
        {
            var EquipmentSelectList = GetService<ICodeListService>().List(CurrentInfo.HotelId, "23");
            return Json(EquipmentSelectList, JsonRequestBehavior.AllowGet);
        }
        #endregion
        [AuthButton(AuthFlag.None)]
        [NotAuthAttribute]
        public JsonResult UploadImg(string id, string name, string type, string lastModifiedDate, int size, HttpPostedFileBase file)
        {

            return Json("", JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult SetEquipment(string roomtypeid)
        {
            var roomType = GetService<IRoomTypeService>();
            RoomType rt = roomType.Get(roomtypeid);
            ViewBag.roomtypename = rt.Name; ViewBag.roomtypeid = rt.Id;
            ViewBag.commonQueryModel = roomType.getroomtypeEquipment(CurrentInfo.HotelId, roomtypeid);
            return PartialView("_SetEquipment");
        }

        /// <summary>
        /// 保存系统参数
        /// </summary>
        /// <param name="id">酒店id</param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult SetEquipment(List<RtEqList> para, string id)
         {
            var roomTypeService = GetService<IRoomTypeService>();
            roomTypeService.setroomtypeEquipment(para, id);
            return Json(JsonResultData.Successed("修改成功！"));
        }
    }
}
