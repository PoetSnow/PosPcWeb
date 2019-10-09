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
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Common.Services.BasicDataControls;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 付款方式维护
    /// </summary>
    [AuthPage("99060")]
    [AuthPage(ProductType.Member, "m99025")]
    [AuthBasicData(M_V_BasicDataType.BasicDataCodeItemPay)]
    [BusinessType("付款方式维护")]
    public class PayWayManageController : BaseEditInWindowController<Item, IItemService>
    {
        public string _dcFlag = "C";
        #region 查询
        // GET: SystemManage/PayWay

        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            if (CurrentInfo.IsGroupInGroup)
            {
                var pmsHotelService = GetService<IPmsHotelService>();
                ViewBag.Hotels = pmsHotelService.GetHotelsInGroup(CurrentInfo.GroupId);//当前集团的酒店列表
                ViewBag.hotelid = CurrentInfo.HotelId;
                SetCommonQueryValues("up_list_itemPay_group", "@s23分店=" + CurrentInfo.HotelId);
                return View("IndexGroup");
            }
            else
            {
                if (CurrentInfo.IsHotelInGroup)
                {
                    var resortControlService = GetService<IBasicDataResortControlService>();
                    var resortControl = resortControlService.GetResortControl(M_V_BasicDataType.BasicDataCodePayway, CurrentInfo.GroupId);
                    if (resortControl != null)
                    {
                        //ViewBag.isCanUpdate = resortControl.ResortCanUpdate;
                        ViewBag.isCanAdd = resortControl.ResortCanAdd;
                    }
                    else
                    {
                        // ViewBag.isCanUpdate = true;
                        ViewBag.isCanAdd = true;
                    }
                }
                else
                {
                    // ViewBag.isCanUpdate = true;
                    ViewBag.isCanAdd = true;
                }
                SetCommonQueryValues("up_list_itemPay", "");
                return View();
            }
        }
        #endregion

        #region 添加付款方式
        /// <summary>
        ///     
        /// </summary>
        /// <param name="pcid">付款类型编号</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(string pcid)
        {
            if (CurrentInfo.IsGroupInGroup)
            {
                return _AddGroup(new ItemGroupAddViewModel() { ItemTypeid = pcid, DcFlag = _dcFlag }, M_V_BasicDataType.BasicDataCodeItemPay);
            }
            else
            {
                return _Add(new ItemAddViewModel() { ItemTypeid = pcid, DcFlag = _dcFlag });
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
            string itemname = hotelserver.GetCodeListFornameBycode("03", currentInfo.HotelId, itemViewModel.ItemTypeid);//消费类别
            itemViewModel.ItemTypeName = itemname;
            return _Add(itemViewModel, new Item
            {
                Hid = currentInfo.HotelId,
                Id = currentInfo.HotelId + itemViewModel.Code,
                Status = EntityStatus.禁用,
                DataSource = BasicDataDataSource.Added.Code
            }, OpLogType.付款方式增加);
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
            string itemname = hotelserver.GetCodeListFornameBycode("03", hid, itemViewModel.ItemTypeid);//消费类别
            itemViewModel.ItemTypeName = itemname;
            return _AddGroup(itemViewModel, new Item
            {
                Hid = hid,
                Id = hid + itemViewModel.Code,
                Status = EntityStatus.禁用,
                DataSource = BasicDataDataSource.Added.Code
            }, OpLogType.付款方式增加);
        }

        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            if (id.Substring(0, 6) != CurrentInfo.HotelId)
            {
                return Json(JsonResultData.Failure("集团不可修改分店数据！"), JsonRequestBehavior.AllowGet);
            }
            var itemserv = GetService<IItemService>();
            var entity = itemserv.Get(id);
            var editResult = _CanEdit(entity, M_V_BasicDataType.BasicDataCodeItemPay);
            if (editResult != null)
            {
                return editResult;
            }
            //单店的才允许修改房型代码
            ViewBag.canEditCode = CurrentInfo.IsGroup ? false : true;
            List<V_itemReserv> val = itemserv.IsexistV_itemReserv(id.Replace(CurrentInfo.HotelId, ""), _dcFlag);
            if (val.Count > 0)
            {
                ViewBag.isexistItemReserv = false;
                V_codeListPub alist = GetService<IItemService>().GetCodeListPub("01", entity.Action)[0];
                ViewBag.actions = alist.name;
                ViewBag.IsRetuns = entity.IsRetun == true ? "是" : "否";
                ViewBag.IsCharges = entity.IsCharge == true ? "是" : "否";
            }
            else
            {
                ViewBag.isexistItemReserv = true;

            }

            if (CurrentInfo.IsGroupInGroup)
            {
                return _EditGroup(id, new ItemGroupEditViewModel(), M_V_BasicDataType.BasicDataCodeItemPay);
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
            if (model.Nights == null)
            {
                model.Nights = 0;
            }
            var hid = currentInfo.HotelId;
            var itemserv = GetService<IItemService>();
            string itemname = itemserv.GetCodeListFornameBycode("03", hid, model.ItemTypeid);//消费类别
            List<V_itemReserv> val = itemserv.IsexistV_itemReserv(model.Id.Replace(currentInfo.HotelId, ""), _dcFlag);
            if (val.Count > 0)
            {
                foreach (var item in val)
                {
                    if (model.Action != item.Action || model.DcFlag != item.Dcflag || model.Code != item.Itemcode || model.Name != item.Name || model.ItemTypeid != currentInfo.HotelId + "03" + item.Itemtypecode)
                    {
                        return Json(JsonResultData.Failure("系统保留项目中的属性不允许修改！"));
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
            model.ItemTypeName = itemname;
            //根据付款类型id查询付款类型名称
            return _Edit(model, new Item()
            {
                DcFlag = _dcFlag,
                ItemTypeName = itemname
            }, OpLogType.付款方式修改);
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult EditGroup(ItemGroupEditViewModel model)
        {
            var currentInfo = GetService<ICurrentInfo>();
            if (model.Nights == null)
            {
                model.Nights = 0;
            }
            var hid = currentInfo.HotelId;
            var itemserv = GetService<IItemService>();
            string itemname = itemserv.GetCodeListFornameBycode("03", hid, model.ItemTypeid);//消费类别
            List<V_itemReserv> val = itemserv.IsexistV_itemReserv(model.Id.Replace(currentInfo.HotelId, ""), _dcFlag);
            if (val.Count > 0)
            {
                foreach (var item in val)
                {
                    if (model.Action != item.Action || model.DcFlag != item.Dcflag || model.Code != item.Itemcode || model.Name != item.Name || model.ItemTypeid != currentInfo.HotelId + "03" + item.Itemtypecode)
                    {
                        return Json(JsonResultData.Failure("系统保留项目中的属性不允许修改！"));
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
            model.ItemTypeName = itemname;
            var entity = itemserv.Get(model.Id);
            if (entity == null)
            {
                return Json(JsonResultData.Failure("错误信息，请重试。"));
            }
            return _EditGroup(model, entity, OpLogType.付款方式修改);
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
        public JsonResult GetIsSelectLists()
        {
            Collection<SelectListItem> list = new Collection<SelectListItem>() {
                   new SelectListItem() { Value = "1", Text = "启用", Selected = true },
                   new SelectListItem() { Value = "21", Text = "禁用", Selected = false }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取处理方式的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetActionList()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            var MasterService = GetService<IMasterService>();
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            var ac = MasterService.GetHotelItemAction(hid);
            var hotelserver = GetService<IItemService>();
            List<V_codeListPub> alist = hotelserver.GetCodeListPub("01", "no");
            list.Add(new SelectListItem() { Value = alist[0].code.ToString(), Text = alist[0].name.ToString() });
            if (ac != null)
            {
                string[] actionstr = ac.Split(',');
                for (int i = 0; i < actionstr.Length; i++)
                {
                    if (actionstr[i] != "no")
                    {
                        alist = hotelserver.GetCodeListPub("01", actionstr[i]);
                        list.Add(new SelectListItem() { Value = alist[0].code.ToString(), Text = alist[0].name.ToString() });
                    }
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取类别的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForPayway()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hotelserver = GetService<IItemService>();
            List<CodeList> alist = hotelserver.GetCodeList("03", currentInfo.HotelId);//付款类别
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Code.ToString() + "   " + item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.None)]
        public JsonResult GetStatusSelectList()
        {
            var statusSelectList = EnumExtension.ToSelectList(typeof(EntityStatus), EnumValueType.Value, EnumValueType.Description);
            return Json(statusSelectList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 启用禁用
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            if (CurrentInfo.IsGroup)
            {
                var _roomTypeService = GetService<IItemService>();
                var reval = _BatchBatchChangeStatusGroup(id, EntityKeyDataType.String, M_V_BasicDataType.BasicDataCodeItemPay, _roomTypeService, EntityStatus.启用, OpLogType.付款方式启用禁用);
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
                var reval = _BatchBatchChangeStatusGroup(id, EntityKeyDataType.String, M_V_BasicDataType.BasicDataCodeItemConsume, _roomTypeService, EntityStatus.禁用, OpLogType.付款方式启用禁用);
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
                return _BatchDeleteGroup(id, EntityKeyDataType.String, GetService<IItemService>(), OpLogType.付款方式删除);
            }
            else
            {
                return _BatchDelete(id, GetService<IItemService>(), OpLogType.付款方式删除);
            }
        }
        #endregion
        #region 删除
        /// <summary>
        /// 判断是否是视图中的记录
        /// </summary>
        /// <param name="id">item表中的id</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Delete)]
        public ActionResult CheckForDelete(string[] id)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hotelserver = GetService<IItemService>();
            List<V_itemReserv> val = null;
            string str = "";
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
