using System;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using Gemstar.BSPMS.Hotel.Web.Areas.CRM.Models.GuestManage;
using System.Collections.Generic;
using Gemstar.BSPMS.Hotel.Services.MbrCardCenter;
using Gemstar.BSPMS.Common.Services.Enums;
using System.Linq;
using Gemstar.BSPMS.Common.Tools;

namespace Gemstar.BSPMS.Hotel.Web.Areas.CRM.Controllers
{
    /// <summary>
    /// 客历管理
    /// </summary>
    [AuthPage("60020")]
    [BusinessType("客历列表维护")]
    public class GuestManageController : BaseEditInWindowController<Guest, IGuestService>
    {
        #region 查询
        // GET: SystemManage/PayWay
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_Guest","");
            return View();
        }
        #endregion

        #region  添加客户信息
        /// <summary>
        ///     添加客户信息
        /// </summary> 
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new GuestAddViewModel() { });
        }
        [HttpPost]
        [System.ComponentModel.DataAnnotations.JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(GuestAddViewModel itemViewModel)
        {
            var currentInfo = GetService<ICurrentInfo>();
            if (itemViewModel.GuestName == null)
            {
                itemViewModel.GuestName = "";
            }
            if (itemViewModel.Cerid == null)
            {
                itemViewModel.Cerid = "";
            }
            if (itemViewModel.Mobile == null)
            {
                itemViewModel.Mobile = "";
            }
            if (IsGuestManage(itemViewModel.GuestName, itemViewModel.Cerid))
                return Json(new JsonResultData { Success = false, Data = "此客历已经存在" });
            return _Add(itemViewModel, new Guest
            {
                Hid = currentInfo.HotelId,
                Id = Guid.NewGuid() ,
            },OpLogType.客历增加);
        }
        #endregion

        #region 客历详细信息的显示列表
        /// <summary>
        /// 客历详细信息的显示列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Details)]
        public ActionResult Detail(string id, bool hasButton = true)
        {
            var guestserv = GetService<IGuestService>();
            var Icompany = GetService<ICompanyService>();
            Guest s= guestserv.Get(Guid.Parse(id));
            var mbrserv = GetService<IItemService>();
            var certp= mbrserv.GetCodeListPub("09",s.CerType);
            if (certp != null)
                ViewBag.certype = certp[0].name;
            else
                ViewBag.certype = "";
            ViewData["id"] = id;
            var result= guestserv.GetGuestTrans(CurrentInfo.HotelId, id);
            var first= result.OrderByDescending(w => w.depDate).FirstOrDefault();
            var amountSum = result.Select(w=> w.amount).Sum();
            var nightSum = result.Select(w => w.nights).Sum();
            var viewModel = new GuestEditViewModel();
            AutoSetValueHelper.SetValues(s, viewModel);
            viewModel.Amount = amountSum==0?"":amountSum.ToString();
            viewModel.Nigths =nightSum==0?"": nightSum.ToString("#0.0");
            viewModel.DepDate = first==null?"":first.settleBsnsdate;
            viewModel.RoomNo = first==null?"":first.roomNo;
            Guid cId;
            if(Guid.TryParse(viewModel.CompanyName,out cId))
            {
                var company = Icompany.Get(cId);
                viewModel.CompanyName = company == null ? "" : company.Name;
            }            
            ViewBag.HasButton = hasButton;
            return PartialView("_Detail", viewModel);
        }
        [AuthButton(AuthFlag.Details)]
        public ActionResult DetailByCerId(string cerType, string cerId, bool hasButton = true)
        {
            var entity = GetService<IGuestService>().GetGuestByCerId(CurrentInfo.HotelId, cerType, cerId);
            if(entity == null)
            {
                return Content("找不到相同证件类型和证件号的客历！");
            }
            return Detail(entity.Id.ToString(), hasButton);
        }
        #endregion

            #region 修改
            /// <summary>
            /// 修改客历信息
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            ViewData["id"] = id;
            return _Edit(Guid.Parse(id), new GuestEditViewModel() { });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(GuestEditViewModel model)
        {
            var curinfo = GetService<ICurrentInfo>();
            if (IsGuestManage(model.GuestName, model.Cerid,model.Id.ToString()))
                return Json(new JsonResultData { Success = false, Data = "此客历已经存在" });
            return _Edit(model, new Guest() { Hid = curinfo.HotelId },OpLogType.客历修改);
        }
          
        #endregion
 
        #region 下拉绑定
        /// <summary>
        /// 性别选择
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetIsSelectList()
        {
            Collection<SelectListItem> list = new Collection<SelectListItem>() {
                   new SelectListItem() { Value = "M", Text = "男", Selected = true },
                   new SelectListItem() { Value = "F", Text = "女", Selected = false }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        } 
        /// <summary>
        /// 获取证件类型的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetCerTypeList()
        {
            var hotelserver = GetService<IItemService>();
            List<V_codeListPub> alist = hotelserver.GetCodeListPub("09");
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.code.ToString(), Text = item.name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion
         
        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IGuestService>(), OpLogType.客历删除);
        }
        #endregion

        #region 验证客历唯一
        public bool IsGuestManage(string name, string cerid,string id=null)
        {
            var guestserv = GetService<IGuestService>();
            var data=  guestserv.GetGuest(CurrentInfo.HotelId);
            if (string.IsNullOrWhiteSpace(id))
            {
                return data.Any(a => a.GuestName == name && a.Cerid == cerid);
            }
            else
            { 
                var newid = Guid.Parse(id);
                return data.Any(a => a.GuestName == name && a.Cerid == cerid && a.Id!=newid);
            }
        }
        #endregion
    }
}