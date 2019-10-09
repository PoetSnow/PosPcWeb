using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.ItemManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using System.Collections.ObjectModel;
using Gemstar.BSPMS.Common.Extensions;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Hotel.Services.SystemManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    // <summary>
    ///消费项目维护
    /// </summary>
    [AuthPage("99080")]
    [AuthPage(ProductType.Member, "m99020")]
    [AuthBasicData(M_V_BasicDataType.BasicDataCodeItemConsume)]
    [BusinessType("消费项目维护")]
    public class ConsumeManageController : BaseEditInWindowController<Item, IItemService>
    {
        public string _dcFlag = "D";
        #region 查询
        // GET: SystemManage/PayWay
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            var serv = GetService<IItemService>();
            ViewData["statypelist"] = serv.GetStatypesellist();
            bool isAllowOwner = GetService<IPmsParaService>().isAllowOwner(CurrentInfo.HotelId);
            ViewBag.isallowOwner = isAllowOwner;

            if (CurrentInfo.IsGroupInGroup)
            {
                var pmsHotelService = GetService<IPmsHotelService>();
                ViewBag.Hotels = pmsHotelService.GetHotelsInGroup(CurrentInfo.GroupId);//当前集团的酒店列表
                ViewBag.hotelid = CurrentInfo.HotelId;
                SetCommonQueryValues("up_list_itemConsume_Group", "@s23分店=" + CurrentInfo.HotelId);
                return View("IndexGroup");
            }
            else
            {
                if (CurrentInfo.IsHotelInGroup)
                {
                    var resortControlService = GetService<IBasicDataResortControlService>();
                    var resortControl = resortControlService.GetResortControl(M_V_BasicDataType.BasicDataCodeConsume, CurrentInfo.GroupId);
                    if (resortControl != null)
                    {
                        //ViewBag.isCanUpdate = resortControl.ResortCanUpdate;
                        ViewBag.isCanAdd = resortControl.ResortCanAdd;
                    }
                    else
                    {
                        //ViewBag.isCanUpdate = true;
                        ViewBag.isCanAdd = true;
                    }
                }
                else
                {
                    //ViewBag.isCanUpdate = true;
                    ViewBag.isCanAdd = true;
                }

                SetCommonQueryValues("up_list_itemConsume", "");
                return View();
            }
        }
        #endregion

        #region 添加消费方式
        /// <summary>
        ///     
        /// </summary>
        /// <param name="pcid">消费类型编号</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(string pcid)
        {
            bool isAllowOwner = GetService<IPmsParaService>().isAllowOwner(CurrentInfo.HotelId);
            ViewBag.isallowOwner = isAllowOwner;
            if (CurrentInfo.IsGroupInGroup)
            {
                return _AddGroup(new ItemGroupAddViewModel() { ItemTypeid = pcid, DcFlag = _dcFlag, StaType = "01", Notscore = false, }, M_V_BasicDataType.BasicDataCodeItemConsume);
            }
            else
            {
                return _Add(new ItemAddViewModel() { ItemTypeid = pcid, DcFlag = _dcFlag, StaType = "01", Notscore = false, });
            }
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(ItemAddViewModel itemViewModel)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hotelserver = GetService<IItemService>();
            if (itemViewModel.Nights == null)
            {
                itemViewModel.Nights = 0;
            }
            #region 判断是否存在视图中，不存在则限制code不能以0开头
            List<V_itemReserv> val = hotelserver.IsexistV_itemReserv(itemViewModel.Code, _dcFlag);
            if (val.Count <= 0)
            {
                if (itemViewModel.Code.Substring(itemViewModel.Code.Length - 1, 1) == "0")
                {
                    return Json(JsonResultData.Failure("自定义代码不能尾数为0，<br/>尾数为0是系统固定代码！"));
                }
            }
            #endregion
            string itemname = hotelserver.GetCodeListFornameBycode("02", currentInfo.HotelId, itemViewModel.ItemTypeid);//消费类别
            itemViewModel.ItemTypeName = itemname;
            if (float.Parse(itemViewModel.Action) < 6 && itemViewModel.Nights == 0)
            {
                return Json(JsonResultData.Failure("您所选的处理方式对应间夜数不能为0！"));
            }
            if (itemViewModel.OwnerProperty == "1")
            {
                itemViewModel.IsOwnerFee = true;
                itemViewModel.IsOwnerAmount = false;
            }
            else if (itemViewModel.OwnerProperty == "2")
            {
                itemViewModel.IsOwnerFee = false;
                itemViewModel.IsOwnerAmount = true;
            }
            else
            {
                itemViewModel.IsOwnerFee = false;
                itemViewModel.IsOwnerAmount = false;
            }
            return _Add(itemViewModel, new Item
            {
                Hid = currentInfo.HotelId,
                Id = currentInfo.HotelId + itemViewModel.Code,
                Status = EntityStatus.禁用,
                DataSource = BasicDataDataSource.Added.Code
            }, OpLogType.消费项目增加);
        }



        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        [JsonException]
        public ActionResult AddGroup(ItemGroupAddViewModel itemViewModel)
        {
            var hid = CurrentInfo.GroupId;
            var roomTypeService = GetService<IRoomTypeService>();

            var entity = roomTypeService.Get(hid + itemViewModel.Code);
            if (entity != null)
            {
                return Json(JsonResultData.Failure("代码已存在，请重新输入。"));
            }
            var currentInfo = GetService<ICurrentInfo>();
            var hotelserver = GetService<IItemService>();
            if (itemViewModel.Nights == null)
            {
                itemViewModel.Nights = 0;
            }
            #region 判断是否存在视图中，不存在则限制code不能以0开头
            List<V_itemReserv> val = hotelserver.IsexistV_itemReserv(itemViewModel.Code, _dcFlag);
            if (val.Count <= 0)
            {
                if (itemViewModel.Code.Substring(itemViewModel.Code.Length - 1, 1) == "0")
                {
                    return Json(JsonResultData.Failure("自定义代码不能尾数为0，<br/>尾数为0是系统固定代码！"));
                }
            }
            #endregion
            string itemname = hotelserver.GetCodeListFornameBycode("02", currentInfo.HotelId, itemViewModel.ItemTypeid);//消费类别
            itemViewModel.ItemTypeName = itemname;
            if (float.Parse(itemViewModel.Action) < 6 && itemViewModel.Nights == 0)
            {
                return Json(JsonResultData.Failure("您所选的处理方式对应间夜数不能为0！"));
            }
            if (itemViewModel.OwnerProperty == "1")
            {
                itemViewModel.IsOwnerFee = true;
                itemViewModel.IsOwnerAmount = false;
            }
            else if (itemViewModel.OwnerProperty == "2")
            {
                itemViewModel.IsOwnerFee = false;
                itemViewModel.IsOwnerAmount = true;
            }
            else
            {
                itemViewModel.IsOwnerFee = false;
                itemViewModel.IsOwnerAmount = false;
            }
            return _AddGroup(itemViewModel, new Item
            {
                Hid = hid,
                Id = hid + itemViewModel.Code,
                Code = itemViewModel.Code,
                Status = EntityStatus.禁用,
                DataSource = BasicDataDataSource.Added.Code
            }, OpLogType.消费项目增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            //首先需要判断对应记录是否是自主增加的，如果是自主增加的则始终允许修改，如果是分发的，则根据集团设置的是否允许修改来判断

            var Itemserver = GetService<IItemService>();
            var entity = Itemserver.Get(id);
            var editResult = _CanEdit(entity, M_V_BasicDataType.BasicDataCodeItemConsume);
            if (editResult != null)
            {
                return editResult;
            }
            bool isAllowOwner = GetService<IPmsParaService>().isAllowOwner(CurrentInfo.HotelId);
            ViewBag.isallowOwner = isAllowOwner;
            //return _Edit(id, new ItemEditViewModel() { });
            //单店的才允许修改消费项目代码
            ViewBag.canEditCode = CurrentInfo.IsGroup ? false : true; 
            List<V_itemReserv> val = Itemserver.IsexistV_itemReserv(entity.Code, _dcFlag);//是不是保留项目
            if (val.Count > 0)
            {
                ViewBag.isexistItemReserv = false;
                V_codeListPub alist = GetService<IItemService>().GetCodeListPub("00", entity.Action)[0];
                ViewBag.actions = alist.name;
                //ViewBag.IsRetuns = entity.IsRetun == true ? "是" : "否";
                //ViewBag.IsCharges = entity.IsCharge == true ? "是" : "否";
            }
            else
            {
                ViewBag.isexistItemReserv = true;
            }
            if (CurrentInfo.IsGroupInGroup)
            {
                return _EditGroup(id, new ItemGroupEditViewModel(), M_V_BasicDataType.BasicDataCodeItemConsume);
            }
            else
            {
                return _Edit(id, new ItemEditViewModel());
            }
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(ItemEditViewModel model)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var Itemserver = GetService<IItemService>();
            if (model.Nights == null)
            {
                model.Nights = 0;
            }
            string itemname = Itemserver.GetCodeListFornameBycode("02", currentInfo.HotelId, model.ItemTypeid);//消费类别 
            List<V_itemReserv> val = Itemserver.IsexistV_itemReserv(model.Id.Replace(currentInfo.HotelId, ""), _dcFlag);
            if (val.Count > 0)
            {
                foreach (var item in val)
                {
                    if (model.Action != item.Action || model.DcFlag != item.Dcflag || model.Code != item.Itemcode || model.Name != item.Name || model.StaType != item.StaType || model.ItemTypeid != currentInfo.HotelId + "02" + item.Itemtypecode)
                    {
                        return Json(JsonResultData.Failure("系统保留消费项目中的属性不允许修改！"));
                    }
                }
            }
            else
            {
                if (model.Code.Substring(model.Code.Length - 1, 1) == "0")
                {
                    return Json(JsonResultData.Failure("自定义代码不能尾数为0，<br/>尾数为0是系统固定代码！"));
                }
            }
            //根据消费类型id查询消费类型名称
            model.ItemTypeName = itemname;
            if (model.OwnerProperty == "1")
            {
                model.IsOwnerFee = true;
                model.IsOwnerAmount = false;
            }
            else if (model.OwnerProperty == "2")
            {
                model.IsOwnerFee = false;
                model.IsOwnerAmount = true;
            }
            else
            {
                model.IsOwnerFee = false;
                model.IsOwnerAmount = false;
            }
            return _Edit(model, new Item()
            {
                DcFlag = _dcFlag,
                ItemTypeName = itemname
            }, OpLogType.消费项目修改);
        }


        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult EditGroup(ItemGroupEditViewModel model)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var itemserver = GetService<IItemService>();
            string itemname = itemserver.GetCodeListFornameBycode("02", currentInfo.HotelId, model.ItemTypeid);//消费类别 
            List<V_itemReserv> val = itemserver.IsexistV_itemReserv(model.Id.Replace(currentInfo.HotelId, ""), _dcFlag);
            if (val.Count > 0)
            {
                foreach (var item in val)
                {
                    if (model.Action != item.Action || model.DcFlag != item.Dcflag || model.Code != item.Itemcode || model.Name != item.Name || model.StaType != item.StaType || model.ItemTypeid != currentInfo.HotelId + "02" + item.Itemtypecode)
                    {
                        return Json(JsonResultData.Failure("系统保留消费项目中的属性不允许修改！"));
                    }
                }
            }
            else
            {
                if (model.Code.Substring(model.Code.Length - 1, 1) == "0")
                {
                    return Json(JsonResultData.Failure("自定义代码不能尾数为0，<br/>尾数为0是系统固定代码！"));
                }
            }
            //根据消费类型id查询消费类型名称
            model.ItemTypeName = itemname;
            if (model.OwnerProperty == "1")
            {
                model.IsOwnerFee = true;
                model.IsOwnerAmount = false;
            }
            else if (model.OwnerProperty == "2")
            {
                model.IsOwnerFee = false;
                model.IsOwnerAmount = true;
            }
            else
            {
                model.IsOwnerFee = false;
                model.IsOwnerAmount = false;
            }
            var entity = itemserver.Get(model.Id);
            if (entity == null)
            {
                return Json(JsonResultData.Failure("错误信息，请重试。"));
            }
            return _EditGroup(model, entity, OpLogType.消费项目修改);
            //return _Edit(model, new Item()
            //{
            //    DcFlag = _dcFlag,
            //    ItemTypeName = itemname
            //}, OpLogType.消费项目修改);
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
        /// 获取类别的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForConsume()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hotelserver = GetService<IItemService>();
            List<CodeList> alist = hotelserver.GetCodeList("02", currentInfo.HotelId);//消费类别
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Code.ToString() + "    " + item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取发票项目的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForInvoiceItemid()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hotelserver = GetService<IItemService>();
            List<CodeList> alist = hotelserver.GetCodeList("13", currentInfo.HotelId);//发票项目
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = "", Selected = true });
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.None)]
        public JsonResult GetIsSelectLists()
        {
            Collection<SelectListItem> list = new Collection<SelectListItem>() {
                   new SelectListItem() { Value = "1", Text = "是", Selected = true },
                   new SelectListItem() { Value = "21", Text = "否", Selected = false }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.None)]
        public JsonResult GetStatusSelectList()
        {
            var statusSelectList = EnumExtension.ToSelectList(typeof(EntityStatus), EnumValueType.Value, EnumValueType.Description);
            return Json(statusSelectList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取处理方式的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetActionList()
        {
            var hotelserver = GetService<IItemService>();
            List<V_codeListPub> alist = hotelserver.GetCodeListPub("00");
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = "" + item.code.ToString(), Text = item.name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取统计分类的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetStatypeList(string act)
        {
            var hotelserver = GetService<IItemService>();
            List<V_codeListPub> alist = hotelserver.GetCodeListPub("00", act);
            var statype = "";
            foreach (var item in alist)
            {
                List<V_codeListPub> blist = hotelserver.GetCodeListPub("07", item.name2);
                foreach (var itemb in blist)
                {
                    statype = itemb.code + "," + itemb.name;
                }
            }
            return Json(statype, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取统计分类的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetInvoiceItemid(string invoice)
        {
            var hotelserver = GetService<IItemService>();
            string invo = hotelserver.GetCodeList("13", CurrentInfo.HotelId, invoice);
            return Json(invo, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 业主属性选择
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetOwnerProperty()
        {
            Collection<SelectListItem> list = new Collection<SelectListItem>() {
                   new SelectListItem() { Value = "0", Text = "无", Selected = true },
                   new SelectListItem() { Value = "1", Text = "业主费用", Selected = false },
                   new SelectListItem() { Value = "2", Text = "业主房租", Selected = false }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 判断有没有存在处理方式
        [AuthButton(AuthFlag.Add)]
        public ActionResult checkAction(string act, string id = "")
        {

            string str = "";
            var currentInfo = GetService<ICurrentInfo>();
            var hotelserver = GetService<IItemService>();
            List<Item> alist = hotelserver.GetItembyAction(currentInfo.HotelId, act, "D", id);//消费类别
            if (alist.Count > 0)
            {
                str = "处理方式已存在";
            }
            return Json(str, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 启用禁用
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            if (CurrentInfo.IsGroup)
            {
                var _roomTypeService = GetService<IItemService>();
                var reval = _BatchBatchChangeStatusGroup(id, EntityKeyDataType.String, M_V_BasicDataType.BasicDataCodeItemConsume, _roomTypeService, EntityStatus.启用, OpLogType.消费项目启用禁用);
                return reval;
            }
            else
            {
                var _hotelService = GetService<IItemService>();
                return Json(_hotelService.Enable(id, _dcFlag));

            }
        }

        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            if (CurrentInfo.IsGroup)
            {
                var _roomTypeService = GetService<IItemService>();
                var reval = _BatchBatchChangeStatusGroup(id, EntityKeyDataType.String, M_V_BasicDataType.BasicDataCodeItemConsume, _roomTypeService, EntityStatus.禁用, OpLogType.消费项目启用禁用);
                return reval;
            }
            else
            {
                var _hotelService = GetService<IItemService>();
                return Json(_hotelService.Disable(id, _dcFlag));

            }
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            if (CurrentInfo.IsGroup)
            {
                return _BatchDeleteGroup(id, EntityKeyDataType.String, GetService<IItemService>(), OpLogType.消费项目删除);
            }
            else
            {
                return _BatchDelete(id, GetService<IItemService>(), OpLogType.消费项目删除);
            }
        }
        #endregion
        #region 删除
        /// <summary>
        /// 判断是否是视图中的记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Delete)]
        public ActionResult CheckForDelete(string[] id)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hotelserver = GetService<IItemService>();
            List<V_itemReserv> val = null; string str = "";
            var arr = id[0].Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                val = hotelserver.IsexistV_itemReserv(arr[i].Replace(currentInfo.HotelId, ""), _dcFlag);
                if (val.Count > 0)
                {
                    return Json(JsonResultData.Failure("尾数为0是系统固定项目，不允许删除！"));
                }
                str = hotelserver.IsexistItemId(currentInfo.HotelId, arr[i]);
                if (str != "")
                {
                    return Json(JsonResultData.Failure("已使用消费项目不允许删除！(" + str.Trim('、') + ")"));
                }
            }

            return Json(JsonResultData.Successed("可以删除！"));
        }

        #endregion
    }
}
