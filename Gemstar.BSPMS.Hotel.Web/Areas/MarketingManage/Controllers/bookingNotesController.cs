using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.BookingNotes;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Services.BasicDataControls;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Controllers
{
    /// <summary>
    /// 预订须知
    /// </summary>
    [AuthPage("61010")]
    [AuthBasicData(M_V_BasicDataType.BasicDataCodeBookingNotes)]
    [BusinessType("预订须知")]
    public class BookingNotesController : BaseEditInWindowController<BookingNotes, IBookingNotesService>
    {
        #region 查询
        // GET: MarketingManage/bookingNotes
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            if (CurrentInfo.IsGroupInGroup)
            {
                //集团分发型资料，列表需要不同，集团需要显示分店名称，并且查询条件中需要有分店可以选择
                SetCommonQueryValues("up_List_BookingNotes_group","");
                return View("IndexGroup");
            }
            else
            {
                SetCommonQueryValues("up_list_BookingNotes", "");
                return View();
            }
        }
        #endregion

        #region 增加
        /// <summary>
        /// 增加预订须知
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            if (CurrentInfo.IsGroupInGroup)
            {
                return _AddGroup(new BookingNotesGroupAddViewModel(), M_V_BasicDataType.BasicDataCodeBookingNotes);
            }
            else
            {
                return _Add(new BookingNotesAddViewModel());
            }
        }
        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        [JsonException]
        public ActionResult Add(BookingNotesAddViewModel bookingnotesViewModel)
        {
            var hid = CurrentInfo.HotelId;
            return _Add(bookingnotesViewModel, new BookingNotes { Hid = hid, Id = Guid.NewGuid(),DataSource=BasicDataDataSource.Added.Code },OpLogType.预订须知增加);
        }
        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        [JsonException]
        public ActionResult AddGroup(BookingNotesGroupAddViewModel bookingnotesViewModel)
        {
            var hid = CurrentInfo.GroupId;
            return _AddGroup(bookingnotesViewModel, new BookingNotes { Hid = hid, Id = Guid.NewGuid(), DataSource = BasicDataDataSource.Added.Code }, OpLogType.预订须知增加);
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
            //首先需要判断对应记录是否是自主增加的，如果是自主增加的则始终允许修改，如果是分发的，则根据集团设置的是否允许修改来判断
            var service = GetService<IBookingNotesService>();
            var user = service.Get(Guid.Parse(id));
            var editResult = _CanEdit(user, M_V_BasicDataType.BasicDataCodeBookingNotes);
            if(editResult != null)
            {
                return editResult;
            }
            //可以修改
            if (CurrentInfo.IsGroupInGroup)
            {
                return _EditGroup(Guid.Parse(id), new BookingNotesGroupEditViewModel(), M_V_BasicDataType.BasicDataCodeBookingNotes);
            }
            else
            {
                return _Edit(Guid.Parse(id), new BookingNotesEditViewModel());
            }
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult EditGroup(BookingNotesGroupEditViewModel model)
        {
            if (model.Remark == null)
            { 
                model.Remark = "";
            }
            return _EditGroup(model, new BookingNotes(), OpLogType.预订须知修改);
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(BookingNotesEditViewModel model)
        {
            if (model.Remark == null)
            {
                model.Remark = "";
            }
            return _Edit(model, new BookingNotes() { }, OpLogType.预订须知修改);
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            if (CurrentInfo.IsGroup) {
                return _BatchDeleteGroup(id,EntityKeyDataType.GUID, GetService<IBookingNotesService>(), OpLogType.预订须知删除);
            } else {
                return _BatchDelete(id, GetService<IBookingNotesService>(), OpLogType.预订须知删除);
            }
        }
        #endregion
    }
}