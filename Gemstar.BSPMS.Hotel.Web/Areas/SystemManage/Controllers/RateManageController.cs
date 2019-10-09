using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RateManage;
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
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Services.NotifyManage;
using Gemstar.BSPMS.Common.Services.Enums;
using System.Linq;
using Gemstar.BSPMS.Common.Services.BasicDataControls;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 价格体系设置 
    /// </summary>
    [AuthPage("61040")]
    [AuthBasicData(M_V_BasicDataType.BasicDataCodeRate)]
    public class RateManageController : BaseEditInWindowController<Rate, IRateService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(bool viewType = true,string hotelid="")
        {

            List<RoomType> rtlist = getRoomType();
            ViewBag.roomType = rtlist;
            ViewBag.roomtypecount = rtlist.Count;
            var rserv = GetService<IRateService>();
            List<Rate> ratelist = rserv.GetRate(CurrentInfo.HotelId);
            ViewBag.rateList = ratelist;
            var rdserv = GetService<IRateDetailService>();
            List<RateDetail> ratedetaillist = rdserv.GetRateDetail(CurrentInfo.HotelId);
            ViewBag.rateDetailList = ratedetaillist;
            ViewBag.viewType = viewType;
            if (CurrentInfo.IsGroupInGroup)
            {
                var pmsHotelService = GetService<IPmsHotelService>();
                ViewBag.Hotels = pmsHotelService.GetHotelsInGroup(CurrentInfo.GroupId);//当前集团的酒店列表
                ViewBag.hotelid = CurrentInfo.HotelId;
                if (viewType)
                {
                    return View("IndexGroup");
                }
                else
                {
                    return PartialView("IndexGroup");
                } 
            }
            else
            {
                if (viewType)
                {
                    return View();
                }
                else
                {
                    return PartialView();
                }
            }
           
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            ViewBag.IsPermanentRoom = IsPermanentRoom();//是否启用长租管理功能
            List<RoomType> alist = getRoomType();
            string[] listRoomTypes = new string[alist.Count];
            for (int i = 0; i < alist.Count; i++)
            {
                listRoomTypes[i] = alist[i].Id;
            }
            ViewBag.rtp = listRoomTypes;
            if (CurrentInfo.IsGroupInGroup)
            {
                return _AddGroup(new RateGroupAddViewModel(), M_V_BasicDataType.BasicDataCodeRate);
            }
            else
            {
                return _Add(new RateAddViewModel());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rateViewModel"></param>
        /// <param name="viewType">为了添加完价格代码后刷新主页面</param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(RateAddViewModel rateViewModel, bool viewType = true)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            if (rateViewModel.endDate != null)
            {
                if (rateViewModel.beginDate > rateViewModel.endDate)
                {
                    return Json(JsonResultData.Failure("生效时间不能大于失效时间！"), JsonRequestBehavior.AllowGet);
                }
                DateTime HotelBusinessDate = DateTime.Parse(GetService<Services.SystemManage.IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId).ToString("yyyy-MM-dd"));
                if (DateTime.Parse(rateViewModel.endDate.ToString()).AddDays(1) < HotelBusinessDate)
                {
                    return Json(JsonResultData.Failure("失效时间不能早于营业日！"), JsonRequestBehavior.AllowGet);
                }
            }

            var checkResult = CheckAdd(rateViewModel);
            if (checkResult.Success == false)
            {
                return Json(checkResult, JsonRequestBehavior.AllowGet);
            }
            var result = _Add(rateViewModel, new Rate
            {
                Hid = hid,
                Id = hid + rateViewModel.Code,
                Status = EntityStatus.禁用,
                DataSource = BasicDataDataSource.Added.Code
            }, OpLogType.价格码增加);
            if (!string.IsNullOrWhiteSpace(rateViewModel.RefRateid))
            {
                GetService<IRateService>().updateRateToRefcode(hid, hid + rateViewModel.Code, rateViewModel.RefRateid, rateViewModel.AddMode, rateViewModel.AddMode == true ? rateViewModel.AddRate : rateViewModel.AddAmount);
            }
            return result;
        }
        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        [JsonException]
        public ActionResult AddGroup(RateGroupAddViewModel rateViewModel)
        {
            ViewBag.IsPermanentRoom = IsPermanentRoom();//是否启用长租管理功能
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            if (rateViewModel.endDate != null)
            {
                if (rateViewModel.beginDate > rateViewModel.endDate)
                {
                    return Json(JsonResultData.Failure("生效时间不能大于失效时间！"), JsonRequestBehavior.AllowGet);
                }
                DateTime HotelBusinessDate = DateTime.Parse(GetService<Services.SystemManage.IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId).ToString("yyyy-MM-dd"));
                if (DateTime.Parse(rateViewModel.endDate.ToString()).AddDays(1) < HotelBusinessDate)
                {
                    return Json(JsonResultData.Failure("失效时间不能早于营业日！"), JsonRequestBehavior.AllowGet);
                }
            } 
            var checkResult = CheckAddGroup(rateViewModel);
            if (checkResult.Success == false)
            {
                return Json(checkResult, JsonRequestBehavior.AllowGet);
            }
            var result = _AddGroup(rateViewModel, new Rate
            {
                Hid = hid,
                Id = hid + rateViewModel.Code,
                Status = EntityStatus.禁用,
                DataSource = BasicDataDataSource.Added.Code

            }, OpLogType.价格码增加);
            if (!string.IsNullOrWhiteSpace(rateViewModel.RefRateid))//如果该价格代码有引用价格码的明细，则复制该引用的价格代码的价格明细到这个价格代码的明细中
            {
                GetService<IRateService>().updateRateToRefcode(hid, hid + rateViewModel.Code, rateViewModel.RefRateid, rateViewModel.AddMode, rateViewModel.AddMode == true ? rateViewModel.AddRate : rateViewModel.AddAmount);
            }
            return result;
        }

        protected override void AfterAddedAndSaved(Rate data)
        {
            NotifyOtaInfo(data);
        }
        private void NotifyOtaInfo(Rate data)
        {
            if (!string.IsNullOrWhiteSpace(data.Channelids))
            {
                var service = GetService<INotifyService>();
                var channels = data.Channelids.Split(',');
                foreach (var c in channels)
                {
                    service.NotifyOtaInfo(CurrentInfo.HotelId, MvcApplication.IsTestEnv, c);
                }
            }
        }
        /// <summary>
        /// 验证参数（集团）
        /// </summary>
        /// <param name="rateGroupViewModel"></param>
        /// <returns></returns>
        private JsonResultData CheckAddGroup(RateGroupAddViewModel rateViewModel)
        {
            //验证时间段内可用
            if (string.IsNullOrWhiteSpace(rateViewModel.StartTime) || string.IsNullOrWhiteSpace(rateViewModel.EndTime))
            {
                return JsonResultData.Failure("请输入时间段内可入住！");
            }

            DateTime nowDate = DateTime.Now;

            var startTime = Gemstar.BSPMS.Common.Extensions.TimeExtension.SetTime(nowDate, rateViewModel.StartTime);
            var endTime = Gemstar.BSPMS.Common.Extensions.TimeExtension.SetTime(nowDate, rateViewModel.EndTime);
            if (startTime == null || startTime == DateTime.MinValue
                ||
                endTime == null || endTime == DateTime.MinValue)
            {
                return JsonResultData.Failure("请输入时间段内可入住！");
            }

            if (startTime >= endTime)
            {
                return JsonResultData.Failure("时间段内可用，开始时间必须小于结束时间！");
            }
            //验证 钟点房、白日房、长包房
            if (rateViewModel.isHou == true && rateViewModel.isMonth != true && rateViewModel.IsDayRoom != true)
            {
                rateViewModel.DayRoomTime = null;
                rateViewModel.DayRoomAddMinute = null;
                rateViewModel.DayRoomAddPrice = null;

                if (rateViewModel.baseMinute == null || !rateViewModel.baseMinute.HasValue || rateViewModel.baseMinute <= 0)
                {
                    return JsonResultData.Failure("请输入基础时长！");
                }
                if (rateViewModel.addMinute == null || !rateViewModel.addMinute.HasValue || rateViewModel.addMinute <= 0)
                {
                    return JsonResultData.Failure("请输入超时分钟！");
                }
                if (rateViewModel.addPrice == null || !rateViewModel.addPrice.HasValue || rateViewModel.addPrice <= 0)
                {
                    return JsonResultData.Failure("请输入超时分钟价格！");
                }
            }
            else if (rateViewModel.isHou != true && rateViewModel.isMonth == true && rateViewModel.IsDayRoom != true)
            {
                rateViewModel.DayRoomTime = null;
                rateViewModel.DayRoomAddMinute = null;
                rateViewModel.DayRoomAddPrice = null;

                rateViewModel.baseMinute = null;
                rateViewModel.addMinute = null;
                rateViewModel.addPrice = null;
            }
            else if (rateViewModel.isHou != true && rateViewModel.isMonth != true && rateViewModel.IsDayRoom == true)
            {
                rateViewModel.baseMinute = null;
                rateViewModel.addMinute = null;
                rateViewModel.addPrice = null;

                if (string.IsNullOrWhiteSpace(rateViewModel.DayRoomTime))
                {
                    return JsonResultData.Failure("请输入白日房时间！");
                }
                var dayRoomTime = Gemstar.BSPMS.Common.Extensions.TimeExtension.SetTime(nowDate, rateViewModel.DayRoomTime);
                if (dayRoomTime == null || dayRoomTime == DateTime.MinValue)
                {
                    return JsonResultData.Failure("请输入白日房时间！");
                }
                if (rateViewModel.DayRoomAddMinute == null || !rateViewModel.DayRoomAddMinute.HasValue || rateViewModel.DayRoomAddMinute <= 0)
                {
                    return JsonResultData.Failure("请输入超时分钟！");
                }
                if (rateViewModel.DayRoomAddPrice == null || !rateViewModel.DayRoomAddPrice.HasValue || rateViewModel.DayRoomAddPrice <= 0)
                {
                    return JsonResultData.Failure("请输入超时分钟价格！");
                }
            }
            else
            {
                if (rateViewModel.isHou == true || rateViewModel.isMonth == true || rateViewModel.IsDayRoom == true)
                {
                    return JsonResultData.Failure("钟点房、白日房、长包房只能勾选一个！");
                }
            }
            if(rateViewModel.IsUseScore == true)//积分换房
            {
                if (rateViewModel.isMonth == true)//不支持长包房
                {
                    return JsonResultData.Failure("积分换房不适用于长包房！");
                }
                if (rateViewModel.NoPrintProfile != "2")//会员必须录入
                {
                    return JsonResultData.Failure("积分换房[需检查会员=必须录入]！");
                }
            }
            return JsonResultData.Successed();
        }
        /// <summary>
        /// 验证参数
        /// </summary>
        /// <param name="rateViewModel"></param>
        /// <returns></returns>
        private JsonResultData CheckAdd(RateAddViewModel rateViewModel)
        {
            //验证时间段内可用
            if (string.IsNullOrWhiteSpace(rateViewModel.StartTime) || string.IsNullOrWhiteSpace(rateViewModel.EndTime))
            {
                return JsonResultData.Failure("请输入时间段内可入住！");
            }

            DateTime nowDate = DateTime.Now;

            var startTime = Gemstar.BSPMS.Common.Extensions.TimeExtension.SetTime(nowDate, rateViewModel.StartTime);
            var endTime = Gemstar.BSPMS.Common.Extensions.TimeExtension.SetTime(nowDate, rateViewModel.EndTime);
            if (startTime == null || startTime == DateTime.MinValue
                ||
                endTime == null || endTime == DateTime.MinValue)
            {
                return JsonResultData.Failure("请输入时间段内可入住！");
            }

            if (startTime >= endTime)
            {
                return JsonResultData.Failure("时间段内可用，开始时间必须小于结束时间！");
            }
            //验证 钟点房、白日房、长包房
            if (rateViewModel.isHou == true && rateViewModel.isMonth != true && rateViewModel.IsDayRoom != true)
            {
                rateViewModel.DayRoomTime = null;
                rateViewModel.DayRoomAddMinute = null;
                rateViewModel.DayRoomAddPrice = null;

                if (rateViewModel.baseMinute == null || !rateViewModel.baseMinute.HasValue || rateViewModel.baseMinute <= 0)
                {
                    return JsonResultData.Failure("请输入基础时长！");
                }
                if (rateViewModel.addMinute == null || !rateViewModel.addMinute.HasValue || rateViewModel.addMinute <= 0)
                {
                    return JsonResultData.Failure("请输入超时分钟！");
                }
                if (rateViewModel.addPrice == null || !rateViewModel.addPrice.HasValue || rateViewModel.addPrice <= 0)
                {
                    return JsonResultData.Failure("请输入超时分钟价格！");
                }
            }
            else if (rateViewModel.isHou != true && rateViewModel.isMonth == true && rateViewModel.IsDayRoom != true)
            {
                rateViewModel.DayRoomTime = null;
                rateViewModel.DayRoomAddMinute = null;
                rateViewModel.DayRoomAddPrice = null;

                rateViewModel.baseMinute = null;
                rateViewModel.addMinute = null;
                rateViewModel.addPrice = null;
            }
            else if (rateViewModel.isHou != true && rateViewModel.isMonth != true && rateViewModel.IsDayRoom == true)
            {
                rateViewModel.baseMinute = null;
                rateViewModel.addMinute = null;
                rateViewModel.addPrice = null;

                if (string.IsNullOrWhiteSpace(rateViewModel.DayRoomTime))
                {
                    return JsonResultData.Failure("请输入白日房时间！");
                }
                var dayRoomTime = Gemstar.BSPMS.Common.Extensions.TimeExtension.SetTime(nowDate, rateViewModel.DayRoomTime);
                if (dayRoomTime == null || dayRoomTime == DateTime.MinValue)
                {
                    return JsonResultData.Failure("请输入白日房时间！");
                }
                if (rateViewModel.DayRoomAddMinute == null || !rateViewModel.DayRoomAddMinute.HasValue || rateViewModel.DayRoomAddMinute <= 0)
                {
                    return JsonResultData.Failure("请输入超时分钟！");
                }
                if (rateViewModel.DayRoomAddPrice == null || !rateViewModel.DayRoomAddPrice.HasValue || rateViewModel.DayRoomAddPrice <= 0)
                {
                    return JsonResultData.Failure("请输入超时分钟价格！");
                }
            }
            else
            {
                if (rateViewModel.isHou == true || rateViewModel.isMonth == true || rateViewModel.IsDayRoom == true)
                {
                    return JsonResultData.Failure("钟点房、白日房、长包房只能勾选一个！");
                }
            }
            if (rateViewModel.IsUseScore == true)//积分换房
            {
                if (rateViewModel.isMonth == true)//不支持长包房
                {
                    return JsonResultData.Failure("积分换房不适用于长包房！");
                }
                if (rateViewModel.NoPrintProfile != "2")//会员必须录入
                {
                    return JsonResultData.Failure("积分换房[需检查会员=必须录入]！");
                }
            }
            return JsonResultData.Successed();
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        { 
            var rateEntity= GetService<IRateService>().Get(id);
            var editResult = _CanEdit(rateEntity, M_V_BasicDataType.BasicDataCodeRate);
            if (editResult != null)
            {
                return editResult;
            } 
            ViewBag.IsPermanentRoom = IsPermanentRoom();//是否启用长租管理功能 
            RateEditViewModel model = new RateEditViewModel();
            ActionResult returnActionResult = _Edit(id, model);
            string[] listCompanyTypes = new string[] { };
            string[] listMbrCardTypes = new string[] { };
            string[] listChannelids = new string[] { };
            string[] listRoomTypeids = new string[] { };
            if (!string.IsNullOrWhiteSpace(model.CompanyTypes))
            {
                listCompanyTypes = model.CompanyTypes.Split(',');
            }
            if (!string.IsNullOrWhiteSpace(model.MbrCardTypes))
            {
                listMbrCardTypes = model.MbrCardTypes.Split(',');
            }
            if (!string.IsNullOrWhiteSpace(model.Channelids))
            {
                listChannelids = model.Channelids.Split(',');
            }
            if (!string.IsNullOrWhiteSpace(model.RoomTypeids))
            {
                listRoomTypeids = model.RoomTypeids.Split(',');
            }
            ViewBag.listCompanyTypes = listCompanyTypes;
            ViewBag.listMbrCardTypes = listMbrCardTypes;
            ViewBag.listChannelids = listChannelids;
            ViewBag.listRoomTypeids = listRoomTypeids;
            ViewBag.hid = GetService<ICurrentInfo>().HotelId;
            ViewBag.amode = model.AddMode;
            //单店的才允许修改价格代码code
            ViewBag.canEditCode = CurrentInfo.IsGroup ? false : true;
            if (CurrentInfo.IsGroupInGroup)
            {
                return _EditGroup(id, new RateGroupEditViewModel(), M_V_BasicDataType.BasicDataCodeRate);
            }
            else
            {
                return _Edit(id, new RateEditViewModel());
            }

        }

        /// <summary>
        /// 判断价格代码在预订和在住中是否存在，存在则不允许修改
        /// </summary>
        /// <param name="rateid"></param>
        /// <returns></returns>
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult checkRateExist(string rateid, bool isdel)
        {
            var rserv = GetService<IRateService>();
            string isexist = rserv.checkExistOthertb(CurrentInfo.HotelId, rateid, isdel);
            return Json(isexist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(RateEditViewModel model)
        {
            if (model.endDate != null)
            {
                if (model.beginDate > model.endDate)
                {
                    return Json(JsonResultData.Failure("生效时间不能大于失效时间！"), JsonRequestBehavior.AllowGet);
                }
                DateTime HotelBusinessDate = DateTime.Parse(GetService<Services.SystemManage.IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId).ToString("yyyy-MM-dd"));
                if (DateTime.Parse(model.endDate.ToString()).AddDays(1) < HotelBusinessDate)
                {
                    return Json(JsonResultData.Failure("失效时间不能早于营业日！"), JsonRequestBehavior.AllowGet);
                }
            }
            var checkResult = CheckEdit(model);
            if (checkResult.Success == false)
            {
                return Json(checkResult, JsonRequestBehavior.AllowGet);
            }
            if (!string.IsNullOrWhiteSpace(model.RefRateid) && model.PriceMode == "2")
            {
                GetService<IRateService>().updateRateToRefcode(CurrentInfo.HotelId, model.Id, model.RefRateid, model.AddMode, model.AddMode == true ? model.AddRate : model.AddAmount);
            }
            return _Edit(model, new Rate(), OpLogType.价格码修改);
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult EditGroup(RateGroupEditViewModel model)
        {
            ViewBag.IsPermanentRoom = IsPermanentRoom();//是否启用长租管理功能
            if (model.endDate != null)
            {
                if (model.beginDate > model.endDate)
                {
                    return Json(JsonResultData.Failure("生效时间不能大于失效时间！"), JsonRequestBehavior.AllowGet);
                }
                DateTime HotelBusinessDate = DateTime.Parse(GetService<Services.SystemManage.IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId).ToString("yyyy-MM-dd"));
                if (DateTime.Parse(model.endDate.ToString()).AddDays(1) < HotelBusinessDate)
                {
                    return Json(JsonResultData.Failure("失效时间不能早于营业日！"), JsonRequestBehavior.AllowGet);
                }
            }
            var checkResult = CheckGroupEdit(model);
            if (checkResult.Success == false)
            {
                return Json(checkResult, JsonRequestBehavior.AllowGet);
            }
            if (!string.IsNullOrWhiteSpace(model.RefRateid) && model.PriceMode == "2")
            {
                GetService<IRateService>().updateRateToRefcode(CurrentInfo.HotelId, model.Id, model.RefRateid, model.AddMode, model.AddMode == true ? model.AddRate : model.AddAmount);
            }
            return _EditGroup(model, new Rate() { Hid = CurrentInfo.HotelId }, OpLogType.价格码修改);
        }
        protected override void AfterEditedAndSaved(Rate data)
        {
            NotifyOtaInfo(data);
        }
        /// <summary>
        /// 参数验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private JsonResultData CheckGroupEdit(RateGroupEditViewModel model)
        {
            RateAddViewModel rateViewModel = new RateAddViewModel();
            Gemstar.BSPMS.Common.Tools.AutoSetValueHelper.SetValues(model, rateViewModel);
            return CheckAdd(rateViewModel);
        }
        /// <summary>
        /// 参数验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private JsonResultData CheckEdit(RateEditViewModel model)
        {
            RateAddViewModel rateViewModel = new RateAddViewModel();
            Gemstar.BSPMS.Common.Tools.AutoSetValueHelper.SetValues(model, rateViewModel);
            return CheckAdd(rateViewModel);
        }
        #endregion

        #region 明细信息
        [AuthButton(AuthFlag.Query)]
        public ActionResult Detail(string ratecode, string roomtype, string ratecodename, string roomtypename)
        {
            ViewData["rateid"] = ratecode;
            ViewData["roomtype"] = roomtype;
            ViewData["rateidname"] = ratecodename;
            ViewData["roomtypename"] = roomtypename;
            var hotelserver = GetService<IItemService>();
            ViewData["Cancelid"] = hotelserver.GetCodeListPubforSel("14");
            ViewData["Guaranteeid"] = hotelserver.GetCodeListPubforSel("13");
            ViewBag.isUseScore = GetService<IRateService>().Get(ratecode).IsUseScore ? "积分" : "价格";
            ViewBag.HotelBusinessDate = GetService<Services.SystemManage.IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId).ToString("yyyy-MM-dd");
            return PartialView("_Detail");
        }


        #region 查询所传年月的价格明细
        // GET: MarketingManage/RoomHold
        [AuthButton(AuthFlag.Query)]
        public ActionResult getData(string rateid, string rmtype, string year, string month)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var serv = GetService<IRateDetailService>();
            List<RateDetail> rh = serv.GetRateDetailbyRateid(currentInfo.HotelId, rateid, rmtype, year, month);
            return Json(rh, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 修改价格明细信息
        /// <summary>
        /// 修改价格明细
        /// </summary>
        /// <returns></returns>
        // GET: MarketingManage/RoomHold 修改价格权限
        [AuthButton(AuthFlag.UpdateCardStatus)]
        public ActionResult changeRateDetail(string begintime, string endtime, string strarr, string rateid, string roomtype, string ratename, string roomtypename)
        {
            DateTime HotelBusinessDate = DateTime.Parse(GetService<Services.SystemManage.IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId).ToString("yyyy-MM-dd"));
            if (DateTime.Parse(begintime).AddDays(1) < HotelBusinessDate || DateTime.Parse(endtime).AddDays(1) < HotelBusinessDate)
            {
                return Json("营业日之前的日期不能修改房价！", JsonRequestBehavior.AllowGet);
            }
            var currentInfo = GetService<ICurrentInfo>();
            var serv = GetService<IRateDetailService>();
            string rh = serv.UpdateRateDetail(begintime, endtime, strarr, rateid, roomtype, currentInfo.HotelId);
            NotifyRatePriceChange(rateid, begintime, endtime);
            //添加操作日志
            AddOperationLog(OpLogType.价格明细价格修改, string.Format("价格名称：{0}，房型：{1}，开始日期：{2}，结束日期：{3}，价格周一到周日分别为：{4}", ratename, roomtypename, begintime, endtime, strarr.Trim(',').Replace(",", "、")), rateid);
            return Json(rh, JsonRequestBehavior.AllowGet);
        }

        private void NotifyRatePriceChange(string rateid, string begintime, string endtime)
        {
            var rateService = GetService<IRateService>();
            var rateInfo = rateService.Get(rateid);
            if (rateInfo != null)
            {
                var notifyService = GetService<INotifyService>();
                if (rateInfo.Channelids != null)
                {
                    var channelIds = rateInfo.Channelids.Split(',');
                    foreach (var channel in channelIds)
                    {
                        notifyService.NotifyOtaRatePrice(rateInfo.Hid, MvcApplication.IsTestEnv, channel, rateid, begintime, endtime);
                    }
                }
            }
        }
        /// <summary>
        /// 修改政策和分时时间
        /// </summary>
        /// <returns></returns>
        // GET: MarketingManage/RoomHold 修改政策权限
        [AuthButton(AuthFlag.UpdateRecord)]
        public ActionResult changeRule(string begintime, string endtime, string strarr, string rateid, string roomtype, string cancelid, string cancelhours, string guaranteeid, string guaranteetime, string ratename, string roomtypename, string cancelname, string guaranteename, int? isClose)
        {
            DateTime HotelBusinessDate = DateTime.Parse(GetService<Services.SystemManage.IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId).ToString("yyyy-MM-dd"));
            if (DateTime.Parse(begintime).AddDays(1) < HotelBusinessDate || DateTime.Parse(endtime).AddDays(1) < HotelBusinessDate)
            {
                return Json("营业日之前的日期不能修改政策信息！", JsonRequestBehavior.AllowGet);
            }
            var currentInfo = GetService<ICurrentInfo>();
            var serv = GetService<IRateDetailService>();
            string rh = serv.UpdateRateDetail(begintime, endtime, rateid, roomtype, cancelid, cancelhours, guaranteeid, guaranteetime, currentInfo.HotelId, isClose ?? 0);
            NotifyRatePriceChange(rateid, begintime, endtime);
            //添加操作日志
            AddOperationLog(OpLogType.价格明细政策修改, string.Format("价格名称：{0}，房型：{1}，开始日期：{2}，结束日期：{3}，取消政策：{4}，担保政策：{5}，关闭：{6}", ratename, roomtypename, begintime, endtime, cancelname + (cancelhours == "" ? "" : "(" + cancelhours + ")"), guaranteename + (guaranteetime == "" ? "" : "(" + guaranteetime + ")"), (isClose == 0 ? "否" : "是")), rateid);
            return Json(rh, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region 设置价格明细
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult AddRD(string price, string Cancelid, string Guaranteeid, string ratecode, string roomtype)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            var rdserv = GetService<IRateDetailService>();
            int i = rdserv.AddRateDetail(hid, price, Cancelid, Guaranteeid, ratecode, roomtype);
            if (i > 0)
            {
                return Json("成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("失败", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 禁用还是启用
        /// <summary>
        /// 禁用价格代码
        /// </summary>
        /// <param name="rateid">价格代码</param>
        /// <param name="status">禁用还是启用状态</param>
        /// <returns></returns>
        [JsonException]
        [AuthButton(AuthFlag.Disable)]
        public ActionResult DisableRate(string rateid, EntityStatus status)
        { 
            return setStatus(rateid, status); 
        } 
        /// <summary>
        /// 启用价格代码
        /// </summary>
        /// <param name="rateid">价格代码</param>
        /// <param name="status">禁用还是启用状态</param>
        /// <returns></returns>
        [JsonException]
        [AuthButton(AuthFlag.Enable)]
        public ActionResult EnableRate(string rateid, EntityStatus status)
        {
            return setStatus(rateid, status); 
        }
        public ActionResult setStatus(string rateid, EntityStatus status)
        {
            var _RateService = GetService<IRateService>();
            var hid = CurrentInfo.HotelId;
            if (CurrentInfo.IsGroup)
            {
                var reval = _BatchBatchChangeStatusGroup(rateid, EntityKeyDataType.String, M_V_BasicDataType.BasicDataCodeRate, _RateService, status, OpLogType.价格码启用禁用);
                if (((Gemstar.BSPMS.Common.Services.JsonResultData)((System.Web.Mvc.JsonResult)reval).Data).Success)
                {
                    return Json("成功", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("失败", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                int i = _RateService.DisableRates(rateid, hid, status);
                if (i > 0)
                {
                    return Json("成功", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("失败", JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            if (CurrentInfo.IsGroup)
            {
                return _BatchDeleteGroup(id, EntityKeyDataType.String, GetService<IRateService>(), OpLogType.价格码删除);
            }
            else
            {
                return _BatchDelete(id, GetService<IRateService>(), OpLogType.价格码删除);
            } 
        }
        #endregion

        #region 下拉绑定
        /// <summary>
        /// 获取房间类型的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForRoomType()
        {

            List<RoomType> alist = getRoomType();
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
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForRoomTypeChangePrice()
        {
            List<RoomType> alist = getRoomType();
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            list.Add(new SelectListItem() { Value = "all", Text = "全部" });
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.None)]
        public JsonResult GetRateSelectList(string selectId = null)
        {
            var _rateService = GetService<IRateService>();
            return Json(_rateService.List(CurrentInfo.HotelId, selectId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取总库渠道的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForChannelcode()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var serv = GetService<IRoomHoldService>();
            SelectList sel = serv.GetChannel(currentInfo.HotelId);
            return Json(sel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取市场分类的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForMarketid()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            var hotelserver = GetService<IItemService>();
            List<CodeList> alist = hotelserver.GetCodeList("04", hid);//市场分类
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = "", Selected = true });
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取客人来源的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemsForSourceid()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            var hotelserver = GetService<IItemService>();
            List<CodeList> alist = hotelserver.GetCodeList("05", hid);//客人来源
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = "", Selected = true });
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取预订须知的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemsForBookingNotesid()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            var hotelserver = GetService<IBookingNotesService>();
            List<BookingNotes> alist = hotelserver.GetBookingNotes(hid);
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = "", Selected = true });
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取线上付款方式的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemsForPayTypeids()
        {
            var hotelserver = GetService<IItemService>();
            List<V_codeListPub> alist = hotelserver.GetCodeListPub("19");
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = "", Selected = true });
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.code.ToString(), Text = item.name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取价格方式的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemsForPriceMode()
        {
            var hotelserver = GetService<IItemService>();
            List<V_codeListPub> alist = hotelserver.GetCodeListPub("18");
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.code.ToString(), Text = item.name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取合约单位类型的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForCompanyTypes()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.GroupHotelId;
            var hotelserver = GetService<IItemService>();
            List<CodeList> alist = hotelserver.GetCodeList("11", hid);//合约单位类型
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取引用价格代码的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForRefRateid(string CurRateid)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            var hotelserver = GetService<IRateService>();
            List<Rate> alist = hotelserver.GetRateref(hid);
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = "", Selected = true });
            foreach (var item in alist)
            {
                if (CurRateid != item.Id)
                {
                    list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name.ToString() });
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 增减方式的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForAddMode()
        {
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            list.Add(new SelectListItem() { Value = false.ToString(), Text = "增减金额" });
            list.Add(new SelectListItem() { Value = true.ToString(), Text = "增减百分比" });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 是否适用散客
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForIsWalkIn()
        {
            Collection<SelectListItem> list = new Collection<SelectListItem>() {
                   new SelectListItem() { Value = true.ToString(), Text = "是"},
                   new SelectListItem() { Value = false.ToString(), Text = "否" }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 是否有早餐
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForBBF()
        {
            Collection<SelectListItem> list = new Collection<SelectListItem>() {
                   new SelectListItem() { Value = "1", Text = "是"},
                   new SelectListItem() { Value = "0", Text = "否" }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取携程价格代码的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemsForCtripCode()
        {
            var hotelserver = GetService<IItemService>();
            List<V_codeListPub> alist = hotelserver.GetCodeListPub("20");
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = "无" });
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.code.ToString(), Text = item.name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 是否有早餐
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForIsPrint()
        {
            Collection<SelectListItem> list = new Collection<SelectListItem>() {
                   new SelectListItem() { Value = "0", Text = "不检查", Selected = true },
                   new SelectListItem() { Value = "1", Text = "仅提示" },
                   new SelectListItem() { Value = "2", Text = "必须录入" }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion
        /// <summary>
        /// 变价
        /// </summary>
        /// <param name="ratecode"></param>
        /// <param name="ratecodename"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult ChangePrice(string ratecode, string ratecodename)
        {
            ViewData["rateid"] = ratecode;
            ViewData["rateidname"] = ratecodename;
            var hotelserver = GetService<IItemService>();
            ViewData["Cancelid"] = hotelserver.GetCodeListPubforSel("14");
            ViewData["Guaranteeid"] = hotelserver.GetCodeListPubforSel("13");
            ViewBag.isUseScore = GetService<IRateService>().Get(ratecode).IsUseScore?"积分":"价格";
            string[] arr = getRoomType().Where(w => w.Status == EntityStatus.禁用).Select(w => w.Id).ToArray();
            string disableRT = "";
            for (int i = 0; i < arr.Length; i++)
            {
                disableRT += arr[i] + ",";
            }
            ViewBag.rtDisable = disableRT;
            return PartialView("_ChangePrice");
        }
        public List<RoomType> getRoomType()
        {
            var serv = GetService<IChannelService>(); 
            List<RoomType> rtlist = serv.GetRoomType(CurrentInfo.HotelId, false);
            return rtlist;
        }
    }
}