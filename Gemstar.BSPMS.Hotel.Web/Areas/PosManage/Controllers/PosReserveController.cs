using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF.PayManage;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosReserve;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// pos 预定
    /// </summary>
    [AuthPage(ProductType.Pos, "p40")]
    public class PosReserveController : BaseController
    {
        [AuthButton(AuthFlag.None)]
        public ActionResult Index(string posId)
        {
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(posId);
            //营业点列表
            var refeList = GetRefeList(posId);

            //默认取排序第一的营业点
            var refe = refeList.OrderBy(w => w.Seqid).FirstOrDefault();
            if (refe != null)
            {
                ViewBag.RefeId = refe.Id;
                ViewBag.RefeName = refe.Cname;
            }


            //餐台类型列表
            var tabTypeList = GetTabTypeList();



            ViewBag.RefeList = refeList;
            ViewBag.TabTypeList = tabTypeList;
            ViewBag.PosId = posId;
            ViewBag.Business = pos.Business;
            return View();
        }

        /// <summary>
        /// 获取营业点下所有市别的时间跨度
        /// </summary>
        /// <param name="refeId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _TimeList(string refeId)
        {
            var list = GetTimeList(refeId);
            return PartialView("_Time", list);
        }

        /// <summary>
        /// 获取餐台列表
        /// </summary>
        /// <param name="refeId">营业点Id</param>
        /// <param name="business">营业日</param>
        /// <param name="tabTypeId">餐台类型</param>
        /// <returns></returns>

        [AuthButton(AuthFlag.None)]
        public PartialViewResult _TabList(string refeId, string business, string tabTypeId, string flag)
        {

            var timeList = GetTimeList(refeId);
            var time = string.Join(",", timeList.ConvertAll(w => w.Time).ToArray());    //把时间串拼接成

            var service = GetService<IPosReserveService>();
            var tabList = service.GetReserveTabStatus(CurrentInfo.HotelId, refeId, tabTypeId == "0" ? "" : tabTypeId, time, Convert.ToDateTime(business), flag);
            return PartialView("_Tab", tabList);
        }

        /// <summary>
        /// 获取餐台状态列表
        /// </summary>
        /// <param name="refeId"></param>
        /// <param name="business"></param>
        /// <param name="tabTypeId"></param>
        /// <returns></returns>

        [AuthButton(AuthFlag.None)]
        public PartialViewResult _TabStatusList(string refeId, string business, string tabTypeId, string flag)
        {
            var timeList = GetTimeList(refeId);
            var time = string.Join(",", timeList.ConvertAll(w => w.Time).ToArray());

            var service = GetService<IPosReserveService>();
            var tabList = service.GetReserveTabStatus(CurrentInfo.HotelId, refeId, tabTypeId == "0" ? "" : tabTypeId, time, Convert.ToDateTime(business), flag);

            TabStatusList s = new TabStatusList()
            {
                TimeLists = timeList,
                TabStatusLists = tabList
            };
            return PartialView("_TabStatus", s);
        }

        #region 添加预定信息

        /// <summary>
        /// 添加预定信息视图
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _AddReserve(string tabId, string refeId, string time, string orderData, string flag)
        {
            var service = GetService<IPosTabService>();

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(refeId);

            var posService = GetService<IPosPosService>();
            var pos = posService.Get(refe.PosId);

            //根据时间点击的时间段 获取当前时间段最早的那笔预订账单
            var orderService = GetService<IPosReserveService>();
            // orderService.GetOrderBillByTabId(CurrentInfo.HotelId, tabId).ToList().Where(w=>w.OrderDate);

            var tab = service.Get(tabId);

            ViewBag.tabTypeId = tab.TabTypeid;
            ViewBag.refeId = refeId;

            var thisDate = DateTime.Now;    //当前日期

            var _date = Convert.ToDateTime(orderData).ToString("yyyy-MM-dd");
            var _clickData = Convert.ToDateTime(_date + " " + time + ":00");    //点击餐台的时间

            //如果点击的时间小于等于当前时间 则用当前时间，大于等于 则用点击的时间

            ViewBag.OrderDate = _clickData <= thisDate ? thisDate : _clickData;

            var bussDate = pos.Business.Value.ToShortDateString().ToString();
            ViewBag.StartTime = bussDate + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute;

            return PartialView("_AddReserve");
        }

        /// <summary>
        /// 加载餐台类型情况
        /// </summary>
        /// <param name="refeId"></param>
        /// <param name="ReserveDate"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult AjaxTabTypeInfo(string refeId, string ReserveDate, [DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosReserveService>();

            var list = service.GetReserveTabTypeInfo(CurrentInfo.HotelId, CurrentInfo.ModuleCode, refeId, Convert.ToDateTime(ReserveDate)).ToList();

            return Json(list.ToDataSourceResult(request));
        }


        [AuthButton(AuthFlag.None)]
        public ActionResult AjaxTabList(string refeId, string ReserveDate, string tabTypeId, [DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosReserveService>();

            var list = service.GetTabListByTabTypeId(CurrentInfo.HotelId, CurrentInfo.ModuleCode, refeId, Convert.ToDateTime(ReserveDate), tabTypeId).ToList();

            return Json(list.ToDataSourceResult(request));
        }

        #endregion

        #region 数据加载

        /// <summary>
        /// 获取营业点
        /// </summary>
        /// <param name="posId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosRefe(string posId)
        {
            var datas = GetRefeList(posId);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();

            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 餐台类型列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosTabType()
        {
            var datas = GetTabTypeList();
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Value = "0", Text = "全部餐台类型" });
            listItems.AddRange(datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList());
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForShuffle(string refeId)
        {
            var service = GetService<IPosShuffleService>();
            var datas = service.GetPosShuffleList(CurrentInfo.HotelId, refeId, CurrentInfo.ModuleCode);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();

            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForCustomerType()
        {
            var service = GetService<IPosCustomerTypeService>();
            var datas = service.GetPosCustomerTypeByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();

            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 营业经理
        /// </summary>
        /// <returns></returns>

        //[AuthButton(AuthFlag.None)]
        //public JsonResult ListItemForSale()
        //{
        //    var service = GetService<IPmsUserService>();
        //    var datas = service.UsersInGroup(CurrentInfo.GroupHotelId).Where(w=>);
        //}


        [AuthButton(AuthFlag.None)]
        public JsonResult ResListItemsForCompanys(string text, bool isNullOrWhiteSpace = true)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                text = Request.QueryString.Get("filter[filters][0][value]");
            }
            if (!string.IsNullOrWhiteSpace(text) || isNullOrWhiteSpace)
            {
                var service = GetService<ICompanyService>();
                var datas = service.GetCompanyInfoList(CurrentInfo.HotelId, text);
                return Json(datas, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<KeyValuePairModel<string, string>>(), JsonRequestBehavior.AllowGet);
        }

        #endregion




        /// <summary>
        /// 获取营业点列表
        /// </summary>
        /// <param name="posId"></param>
        /// <returns></returns>
        private List<PosRefe> GetRefeList(string posId)
        {
            var service = GetService<IPosRefeService>();
            var refeList = service.GetRefeByPos(CurrentInfo.HotelId, posId, CurrentInfo.ModuleCode);
            return refeList;
        }

        /// <summary>
        /// 获取餐台类型列表
        /// </summary>
        /// <returns></returns>
        private List<PosTabtype> GetTabTypeList()
        {
            var service = GetService<IPosTabtypeService>();
            var tabTypeList = service.GetTabtypeByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            return tabTypeList;
        }


        [AuthButton(AuthFlag.None)]
        public ActionResult GetTabList([DataSourceRequest]DataSourceRequest request, string posId, string refeId)
        {
            var service = GetService<IPosTabStatusService>();
            if (string.IsNullOrEmpty(refeId))   //为空 取出所有营业点的数据
            {
                var refeList = GetUserRefeId(posId);
                refeId = string.Join(",", refeList.ConvertAll(w => w.Id).ToArray());
            }


            //获取营业点关联的餐台列表
            var tabCount = service.GetPosTabStatusResultTotal(CurrentInfo.HotelId, "", refeId, "", "");

            var tabList = service.GetPosTabStatusResult(CurrentInfo.HotelId, "", refeId, "", "", 1, tabCount);
            return Json(tabList.ToDataSourceResult(request));
        }

        /// <summary>
        /// 根据营业点获取时间列表（用于预定时间）  
        /// </summary>

        private List<TimeList> GetTimeList(string refeId)
        {

            var service = GetService<IPosShuffleService>();//市别服务
            var shuffleList = service.GetPosShuffleList(CurrentInfo.HotelId, refeId, CurrentInfo.ModuleCode).Where(w => !string.IsNullOrEmpty(w.Stime) && !string.IsNullOrEmpty(w.Etime)).ToList();

            //获取最小的起止时间
            var stime = shuffleList.ConvertAll(w => Convert.ToInt32(w.Stime.Split(':')[0])).Min();

            //获取最大的结束时间
            var etime = shuffleList.ConvertAll(w => Convert.ToInt32(w.Etime.Split(':')[0])).Max();

            List<TimeList> list = new List<TimeList>();
            for (int i = stime; i <= etime; i++)
            {
                var s = new TimeList() { Time = i };
                list.Add(s);
            }
            return list;
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult AddPosReserve(AddReserveModel model)
        {
            var service = GetService<IPosBillService>();    //预定账单服务


            //判断预抵日期
            var dateResult = CheckDate(model.OrderDate);
            if (dateResult.Success == false)
            {
                return Json(dateResult);
            }
            //判断合约单位，会员
            if (!string.IsNullOrEmpty(model.ProfileId))
            {
                var profileService = GetService<Services.MbrCardCenter.IMbrCardService>();
                var profileModel = profileService.Get(Guid.Parse(model.ProfileId));
                if (profileModel == null)
                {
                    return Json(JsonResultData.Failure("会员输入有误！"));
                }
                model.ProfileNo = profileModel.MbrCardNo;   //会员卡号
            }
            if (!string.IsNullOrEmpty(model.CttId))
            {
                var CttService = GetService<ICompanyService>();
                var cttModel = CttService.Get(model.CttId);
                if (cttModel == null)
                {
                    return Json(JsonResultData.Failure("合约单位输入有误！"));
                }
                model.CttName = cttModel.Name;  //合约单位
            }

            //循环餐台 生产预定信息
            if (string.IsNullOrEmpty(model.TabId))
            {
                return Json(JsonResultData.Failure("请选择餐台！"));
            }
            var tabList = model.TabId.Split(',');
            var tabService = GetService<IPosTabService>();

            //string errMsg = "";

            ////判断当前时间，餐台不能重复预订（根据预订时长以及营业点设置的时间进行判断是否有重读预订）
            //var result = CheckOrderTab(tabList, Convert.ToDateTime(model.OrderDate), model.ReservedTime, out errMsg);
            //if (result)
            //{
            //    return Json(JsonResultData.Failure(errMsg));
            //}

            var mBillId = "";   //用来存放同一笔预订信息的主单号
            foreach (var tabId in tabList)
            {
                if (string.IsNullOrEmpty(tabId))
                {
                    continue;
                }
                var tabModel = tabService.Get(tabId);
                //用营业日的前一天生成 billId以及BillNo
                var posBill = service.GetLastBillId(CurrentInfo.HotelId, model.RefeId, Convert.ToDateTime(model.Business).AddDays(-1));
                if (string.IsNullOrEmpty(mBillId))
                {
                    mBillId = posBill.Billid;       //默认取第一笔订单ID为主单号
                }

                PosBill bill = new PosBill
                {
                    Hid = CurrentInfo.HotelId,
                    Billid = posBill.Billid,
                    MBillid = mBillId,
                    BillNo = posBill.Billid,
                    Name = model.Name,                //客人姓名
                    Mobile = model.MobilePhone,       //手机号码
                    Refeid = model.RefeId,
                    Tabid = tabId,
                    TabNo = tabModel.TabNo,
                    Sale = model.Sale,
                    Profileid = model.ProfileId,
                    CardNo = model.ProfileNo,
                    Cttid = model.CttId,
                    CttName = model.CttName,
                    Consumer = "",    //合约单位消费人
                    Status = (byte)PosBillStatus.预订,
                    IsOrder = true,
                    CustomerTypeid = model.GuestType,
                    TabFlag = (byte)PosBillTabFlag.物理台,
                    OrderDate = model.OrderDate,
                    OrderOperDate = DateTime.Now,
                    OrderUser = CurrentInfo.UserName,
                    OrderExpired = model.EndeDate,
                    Shuffleid = model.Shuffle
                };
                service.Add(bill);
                service.AddDataChangeLog(OpLogType.Pos预定账单添加);
                service.Commit();

                SaveOrderInfo(model, posBill.Billid);

                //修改餐台的最早预定时间
                SetTabStatusArrDate(tabId, model.OrderDate);
            }
            return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 判断当前操作餐台是否有重复预订订单
        /// </summary>
        /// <param name="tabList">本次预订的餐台列表</param>
        /// <param name="errMsg">错误信息</param>
        /// <param name="orderDate">预抵日期</param>
        /// <param name="ReservedTime">预定保留时长</param>
        /// <returns>true:有重复。false:没有重读订单</returns>
        private bool CheckOrderTab(string[] tabList, DateTime orderDate, int? ReservedTime, out string errMsg)
        {
            errMsg = "";
            var tabService = GetService<IPosTabService>();
            var refeService = GetService<IPosRefeService>();
            //如果传入的预订时长大于0，则用预订保留时长判断，没有则用营业点设置的预订保留时长

            DateTime _addSTime; //当前餐台的最早提示时间
            DateTime _addETime; //当前餐台保留时间
            foreach (var id in tabList)
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                var tab = tabService.Get(id);
                if (tab == null)
                {
                    errMsg += "未找到餐台，请重新选择餐台！";
                    continue;
                }
                var refe = refeService.Get(tab.Refeid);
                if (refe == null)
                {
                    errMsg += tab.TabNo + "没有设置营业点！";
                    continue;
                }
                _addSTime = orderDate.AddMinutes(-(int)refe.IHoldAlert);
                var _iorderkeep = ReservedTime > 0 ? (int)ReservedTime : (int)refe.IOrderKeep;
                _addETime = orderDate.AddMinutes(_iorderkeep);

                var result = IsExistsTab(tab.Id, _addSTime, _addETime);
                if (result)
                {
                    errMsg += tab.TabNo + "该时间段已经存在预订账单了！";
                    continue;
                }
            }
            if (!string.IsNullOrEmpty(errMsg))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 判断餐台是否有重复预订
        /// </summary>
        /// <param name="tabId">餐台Id</param>
        /// <param name="_thisSTime">当前账单开始时间</param>
        /// <param name="_thisETime">当前账单结束时间</param>
        /// <returns>true：有重读订单。false：没有重复订单</returns>
        private bool IsExistsTab(string tabId, DateTime _thisSTime, DateTime _thisETime)
        {
            var service = GetService<IPosReserveService>();
            var billOrderService = GetService<IPosBillOrderService>();
            var refeService = GetService<IPosRefeService>();

            var orderList = service.GetOrderBillByTabId(CurrentInfo.HotelId, tabId);
            if (orderList == null || orderList.Count <= 0)
            {
                return false;
            }
            var _iholdalert = 0;    //预订提示时间
            var _iorderkeep = 0;    //预订保留是时间
            foreach (var bill in orderList)
            {
                var refe = refeService.Get(bill.Refeid);
                _iholdalert = (int)refe.IHoldAlert;
                //  _iorderkeep = (int)refe.IOrderKeep;

                var billOrder = billOrderService.GetBillOrder(CurrentInfo.HotelId, bill.Billid, "ReservedTime", "PosBill");
                if (billOrder != null)
                {
                    _iorderkeep = string.IsNullOrEmpty(billOrder.ColumnValue) ? (int)refe.IOrderKeep : Convert.ToInt32(billOrder.ColumnValue);
                }
                var sTime = Convert.ToDateTime(bill.OrderDate).AddMinutes(-_iholdalert);
                var eTime = Convert.ToDateTime(bill.OrderDate).AddMinutes(_iorderkeep);
                if ((_thisSTime >= sTime && _thisSTime <= eTime) || (_thisETime >= sTime && _thisETime <= eTime))
                {
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// 验证预抵日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private JsonResultData CheckDate(DateTime? date)
        {
            try
            {
                var thisDate = Convert.ToDateTime(date);
                if (thisDate <= DateTime.Now)
                {
                    return JsonResultData.Failure("预抵时间已经超出当前时间");
                }

            }
            catch
            {
                return JsonResultData.Failure("预抵时间输入不合法");

            }
            return JsonResultData.Successed();
        }


        /// <summary>
        /// 保存预定信息
        /// </summary>
        private void SaveOrderInfo(AddReserveModel model, string billId)
        {
            var billOrderService = GetService<IPosBillOrderService>();  //预定信息接口

            Type type = model.GetType();
            PropertyInfo[] pis = type.GetProperties(); //获取所有公共属性(Public)
            for (int i = 0; i < pis.Length; i++)
            {
                var name = pis[i].Name;
                object value = pis[i].GetValue(model, null);
                if (value == null)
                {
                    continue;
                }

                var billOrder = new PosBillOrder()
                {
                    Id = Guid.NewGuid(),
                    Hid = CurrentInfo.HotelId,
                    Billid = billId,
                    TableName = "PosBill",
                    ColumnName = name,
                    ColumnValue = value.ToString(),
                    TransUser = CurrentInfo.UserName,
                    CreateDate = DateTime.Now
                };
                billOrderService.Add(billOrder);
                billOrderService.AddDataChangeLog(OpLogType.Pos预定信息添加);
                billOrderService.Commit();

            }
        }

        /// <summary>
        /// 修改餐台最早预定时间
        /// </summary>
        /// <param name="tabId">餐台Id</param>
        private void SetTabStatusArrDate(string tabId, DateTime? OrderDate)
        {
            var service = GetService<IPosReserveService>();

            //获取餐台所有预定信息最早的预抵时间
            var listBill = service.GetOrderBillByTabId(CurrentInfo.HotelId, tabId);
            var minTime = listBill.ConvertAll(w => w.OrderDate).Min();

            //当前操作的时间最小的话 把这个时间更新进去
            if (Convert.ToDateTime(OrderDate) <= minTime || minTime == null)
            {
                var tabService = GetService<IPosTabStatusService>();
                var tab = tabService.Get(tabId);
                if (tab != null)
                {
                    var date = tab.ArrDate;
                    tab.ArrDate = OrderDate;
                    tabService.Update(tab, new PosTabStatus());
                    AddOperationLog(OpLogType.Pos餐台状态修改, "修改餐台最早预定时间：" + date + "-->" + OrderDate, tabId);
                    tabService.Commit();

                }
            }


        }

        #region  预订账单列表

        /// <summary>
        /// 获取日期内预定账单列表
        /// </summary>
        /// <param name="refeId">营业点ID</param>
        /// <param name="business">日期</param>
        /// <param name="tabTypeId">餐台类型</param>
        /// <param name="Flag">日期类型（1：预订日期，2：预抵日期）</param>
        /// <param name="status">预订账单状态</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _BillOrderList(string refeId, string business, string tabTypeId, string Flag, string status)
        {
            ViewBag.RefeId = refeId;
            ViewBag.Business = business;
            ViewBag.TabTypeId = tabTypeId;
            ViewBag.Flag = Flag;
            ViewBag.Status = status;
            return PartialView("_OrderList");
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult AjaxBillOrderList(string refeId, string ReserveDate, string TabtypeId, string Flag, string status, [DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosReserveService>();

            var list = service.GetOrderBillList(CurrentInfo.HotelId, refeId, Convert.ToDateTime(ReserveDate), "", Flag, "").ToList();

            return Json(list.ToDataSourceResult(request));
        }

        #endregion


        #region 修改预订账单状态
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _UpdateOrderBill(string billId, string time, string refeId, string date, string tabId)
        {
            ViewBag.RefeId = refeId;
            var service = GetService<IPosReserveService>(); //预订服务
            var billService = GetService<IPosBillService>();    //预订账单服务
            if (string.IsNullOrEmpty(billId))   //账单Id为空 则是从预订主界面进入
            {
                var _thisDate = Convert.ToDateTime(date).ToString("yyyy-MM-dd");
                //根据时间，日期等信息查找时间段最早的预订信息
                var _sDate = Convert.ToDateTime(_thisDate + " " + (Convert.ToInt32(time) - 1).ToString() + ":59");
                var _eDate = Convert.ToDateTime(_thisDate + " " + time.ToString() + ":59");

                var orderBill = service.GetBillOrder(CurrentInfo.HotelId, tabId, _sDate, _eDate, refeId);

                var model = GetReserveModel(orderBill.Billid);


                var tabService = GetService<IPosTabService>();
                var tab = tabService.Get(orderBill.Tabid);
                model.BillId = orderBill.Billid;
                model.TabNo = tab.TabNo;
                return PartialView("_UpdateReserve", model);
            }
            else
            {
                //var orderBill = service.GetBillOrder(CurrentInfo.HotelId, billId);

                //不为空 则根据账单ID 获取预订信息进行编辑
                var model = GetReserveModel(billId);


                var bill = billService.Get(billId);

                var tabService = GetService<IPosTabService>();
                var tab = tabService.Get(bill.Tabid);
                model.BillId = billId;
                model.TabNo = tab.TabNo;
                return PartialView("_UpdateReserve", model);
            }
        }

        /// <summary>
        /// 根据账单ID 查询预订账单信息
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        private EditReserveModel GetReserveModel(string billId)
        {
            var service = GetService<IPosBillOrderService>();
            EditReserveModel model = new EditReserveModel();

            Type type = model.GetType();

            PropertyInfo[] pis = type.GetProperties(); //获取所有公共属性(Public)
            for (int i = 0; i < pis.Length; i++)
            {
                var name = pis[i].Name;
                var _model = service.GetBillOrder(CurrentInfo.HotelId, billId, name, "PosBill");
                if (_model != null)
                {
                    //var proName = pis[i].PropertyType.Name;
                    //var s = typeof(DateTime).Name;
                    if (name == "OrderDate" || name == "EndeDate" || name == "Business")
                    {
                        //EndeDate,Business
                        pis[i].SetValue(model, Convert.ToDateTime(_model.ColumnValue), null);
                    }
                    else if (name == "FYAmount" || name == "EarnestMoney" || name == "ReservedTime" || name == "HYFYAmount" || name == "CYFYAmount")
                    {
                        pis[i].SetValue(model, Convert.ToDecimal(_model.ColumnValue), null);
                    }
                    else if (name == "IGuest")
                    {
                        pis[i].SetValue(model, Convert.ToInt32(_model.ColumnValue), null);
                    }
                    else
                    {
                        pis[i].SetValue(model, _model.ColumnValue, null);
                    }
                }
            }
            return model;
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult UpdatePosReserve(EditReserveModel model, string billId)
        {
            try
            {
                //判断预抵日期
                var dateResult = CheckDate(model.OrderDate);
                if (dateResult.Success == false)
                {
                    return Json(dateResult);
                }
                //判断合约单位，会员
                if (!string.IsNullOrEmpty(model.ProfileId))
                {
                    var profileService = GetService<Services.MbrCardCenter.IMbrCardService>();
                    var profileModel = profileService.Get(Guid.Parse(model.ProfileId));
                    if (profileModel == null)
                    {
                        return Json(JsonResultData.Failure("会员输入有误！"));
                    }
                    model.ProfileNo = profileModel.MbrCardNo;   //会员卡号
                }
                if (!string.IsNullOrEmpty(model.CttId))
                {
                    var CttService = GetService<ICompanyService>();
                    var cttModel = CttService.Get(model.CttId);
                    if (cttModel == null)
                    {
                        return Json(JsonResultData.Failure("合约单位输入有误！"));
                    }
                    model.CttName = cttModel.Name;  //合约单位
                }

                //修改posbill 数据
                var service = GetService<IPosBillService>();
                var bill = service.Get(billId);
                if (bill == null)
                {
                    return Json(JsonResultData.Failure("预订账单不存在！"));
                }
                var oldBill = new PosBill();
                AutoSetValueHelper.SetValues(bill, oldBill);

                bill.Name = model.Name;
                bill.Mobile = model.MobilePhone;
                bill.Sale = model.Sale;
                bill.Profileid = model.ProfileId;
                bill.CardNo = model.ProfileNo;
                bill.Cttid = model.CttId;
                bill.CttName = model.CttName;
                bill.CustomerTypeid = model.GuestType;
                bill.OrderDate = model.OrderDate;
                bill.OrderExpired = model.EndeDate;
                bill.Shuffleid = model.Shuffle;
                bill.IGuest = model.IGuest;
                //bill.OrderUser = CurrentInfo.UserName;

                service.Update(bill, oldBill);
                service.AddDataChangeLog(OpLogType.Pos预定账单修改);
                service.Commit();

                //修改预订账单信息
                updateOrderBill(model, billId);
            }
            catch (Exception ex)
            {

                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }


            return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 修改预订账单信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="billId"></param>
        private void updateOrderBill(EditReserveModel model, string billId)
        {
            var billOrderService = GetService<IPosBillOrderService>();  //预定信息接口

            Type type = model.GetType();
            PropertyInfo[] pis = type.GetProperties(); //获取所有公共属性(Public)
            for (int i = 0; i < pis.Length; i++)
            {
                var name = pis[i].Name;
                object value = pis[i].GetValue(model, null);
                if (value == null)
                {
                    continue;
                }

                var orderModel = billOrderService.GetBillOrder(CurrentInfo.HotelId, billId, name, "PosBill");
                if (orderModel == null) //增加
                {
                    var billOrder = new PosBillOrder()
                    {
                        Id = Guid.NewGuid(),
                        Hid = CurrentInfo.HotelId,
                        Billid = billId,
                        TableName = "PosBill",
                        ColumnName = name,
                        ColumnValue = value.ToString(),
                        TransUser = CurrentInfo.UserName,
                        CreateDate = DateTime.Now
                    };
                    billOrderService.Add(billOrder);
                    billOrderService.AddDataChangeLog(OpLogType.Pos预定信息添加);
                    billOrderService.Commit();
                }
                else
                {
                    //修改
                    orderModel.ColumnValue = value.ToString();
                    orderModel.TransUser = CurrentInfo.UserName;

                    billOrderService.Update(orderModel, new PosBillOrder());
                    billOrderService.AddDataChangeLog(OpLogType.Pos预定信息修改);
                    billOrderService.Commit();
                }


            }
        }


        /// <summary>
        /// 取消预订单
        /// </summary>
        /// <param name="billId">账单ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult DeleteBillOrder(string billId)
        {
            var service = GetService<IPosBillService>();
            var bill = service.Get(billId);
            if (bill == null)
            {
                return Json(JsonResultData.Failure("未找到预订账单！"));
            }
            var oldStatus = bill.Status;

            bill.Status = (byte)PosBillStatus.取消;
            try
            {
                service.Update(bill, new PosBill());
                AddOperationLog(OpLogType.Pos预定账单修改, "取消预订账单状态从" + oldStatus + "-->" + bill.Status.ToString(), billId);
                service.Commit();

                //修改餐台最早预定时间
                var orderService = GetService<PosCommon>();
                orderService.SetTabServerArrDate(CurrentInfo.HotelId, bill.Tabid);

                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {

                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
        }
        #endregion

        /// <summary>
        /// 根据选择的时间切换市别
        /// </summary>
        /// <param name="refeId">营业点ID</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ContentResult SetShuffleId(string refeId, string time)
        {
            var shuffleService = GetService<IPosShuffleService>();
            var list = shuffleService.GetPosShuffleList(CurrentInfo.HotelId, refeId, CurrentInfo.ModuleCode);

            var _thisDate = DateTime.Now.ToString("yyyy-MM-dd");
            var _thisTime = Convert.ToDateTime(_thisDate + " " + (Convert.ToDateTime(time).Hour + ":" + Convert.ToDateTime(time).Minute));
            string shuffleId = "";
            if (list != null)
            {
                foreach (var shuffle in list)
                {
                    var stime = Convert.ToDateTime(_thisDate + " " + shuffle.Stime);
                    var etime = Convert.ToDateTime(_thisDate + " " + shuffle.Etime);
                    if (stime > etime)
                    {
                        if (_thisTime > stime || _thisTime < etime)
                        {
                            shuffleId = shuffle.Id;
                            break;
                        }
                    }
                    else
                    {
                        if (_thisTime > stime && _thisTime < etime)
                        {
                            shuffleId = shuffle.Id;
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(shuffleId))
                {
                    shuffleId = list.FirstOrDefault().Id;
                }
            }
            return Content(shuffleId);
        }

        /// <summary>
        /// 切换市别同时更改时间
        /// </summary>
        /// <param name="shuffleId">市别 Id</param>
        /// <param name="time">当前设置的时间</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ContentResult SetDateTimeByShuffleId(string shuffleId, string time)
        {
            var shuffleService = GetService<IPosShuffleService>();
            var shuffle = shuffleService.Get(shuffleId);

            var _date = Convert.ToDateTime(time).ToString("yyyy-MM-dd");

            // var stime = shuffle.Stime;

            var _stime = Convert.ToDateTime(_date + " " + shuffle.Stime);
            if (_stime < DateTime.Now)
            {
                return Content(DateTime.Now.ToString());

            }
            else
            {
                return Content(_stime.ToString());
            }
        }

        #region 添加定金功能
        [AuthButton(AuthFlag.Members)]
        public PartialViewResult _AddPrepay(string billId)
        {
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var shiftService = GetService<IPosShiftService>();
            var shift = shiftService.Get(pos.ShiftId);


            var service = GetService<IYtPrepayService>();
            //重新生成定金的单号
            var billNo = service.GetBillNo(CurrentInfo.HotelId, Convert.ToDateTime(pos.Business), CurrentInfo.ModuleCode);

            var modle = new AddPrepayViewModel()
            {
                PosName = pos.Name,
                DBusiness = pos.Business,
                ShiftName = shift.Name,
                Shiftid = shift.Id,
                OriBillNo = billId,
                BillNo = billNo,
                PosNo = pos.Id.TrimEnd(),
                _DBusiness = Convert.ToDateTime(pos.Business).ToShortDateString(),
                OpenFlag = "A",
            };
            return PartialView("_AddPrepay", modle);
        }

        /// <summary>
        /// 买单视图
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult AddPrepay(AddPrepayViewModel model)
        {
            var service = GetService<IYtPrepayService>();
            var itemService = GetService<IPosItemService>();

            //验证单号
            var isExistsVal = service.isExists(CurrentInfo.HotelId, model.BillNo);
            if (isExistsVal)
            {
                return Json(JsonResultData.Failure("单号已经存在，请重新打开窗口进行添加！"));
            }

            var PayModeNo = model.PayModeNo;
            var FolioItemAction = PayModeNo.Split('_')[1];      //付款处理动作
            var payId = PayModeNo.Split('_')[0];

            var item = itemService.GetEntity(CurrentInfo.HotelId, payId);

            //处理各种付款处理动作
            var payActionXmlBuilder = GetService<PayActionXmlBuilder>();
            var payActionXmlHandler = payActionXmlBuilder.Build(FolioItemAction);

            //获取当前付款方式信息
            var payment = itemService.GetPosPayType(item.PayType);
            if (payment == null)
            {
                return Json(JsonResultData.Failure("付款方式未找到"));
            }

            //获取会员信息
            MbrCardInfoModel oldmbrCardInfo = null;
            if (!string.IsNullOrEmpty(model.CardId))
            {
                var res = GetService<IPosItemClassDiscountService>().GetMbrCardInfoByCardID(CurrentInfo.HotelId, model.folioMbrCardNo, "");
                if (!res.Success)
                {
                    return Json(JsonResultData.Failure("会员信息查询失败"));
                }
                oldmbrCardInfo = (MbrCardInfoModel)res.Data;
            }


            //微信，支付宝交易号
            string settleTransno = null;
            if (FolioItemAction.Equals("AliBarcode", StringComparison.OrdinalIgnoreCase) || FolioItemAction.Equals("AliQrcode", StringComparison.OrdinalIgnoreCase) || FolioItemAction.Equals("WxBarcode", StringComparison.OrdinalIgnoreCase) || FolioItemAction.Equals("WxQrcode", StringComparison.OrdinalIgnoreCase))
            {
                settleTransno = Guid.NewGuid().ToString("N");
            }

            //取收银点默认第一个营业点
            var refeService = GetService<IPosRefeService>();
            var refe = refeService.GetRefeByPosAndModule(CurrentInfo.HotelId, model.PosNo, CurrentInfo.ModuleCode).FirstOrDefault();

            //根据付款方式的汇率换算金额后再进行尾数处理
            var detailService = GetService<IPosBillDetailService>();
            decimal exchangeRate = model.Amount * item.Rate ?? 0;
            model.Amountbwb = detailService.GetAmountByBillTailProcessing(CurrentInfo.HotelId, refe.Id, exchangeRate);

            //获取收银点对应的捷云营业点代码
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(model.PosNo);
            var posOutletCode = string.IsNullOrWhiteSpace(pos.CodeIn) ? pos.Code : pos.CodeIn;
            if (!string.IsNullOrWhiteSpace(item.CodeIn))
            {
                posOutletCode = item.CodeIn;
            }
            else if (!string.IsNullOrWhiteSpace(refe.CodeIn))
            {
                posOutletCode = refe.CodeIn;
            }

            var addModel = new YtPrepay()
            {
                Id = Guid.NewGuid(),
                Hid = CurrentInfo.HotelId,
                VGuest = model.VGuest,
                OriBillNo = model.OriBillNo,
                BillNo = model.BillNo,
                IType = (byte)PrePayIType.餐饮订金,
                IsClear = 0,
                PayModeNo = payId,
                Amount = model.Amount,
                Amountbwb = model.Amountbwb,
                Remark = model.Remark,
                Creator = CurrentInfo.UserName,
                CreateDate = DateTime.Now,
                IPrepay = (byte)PrePayStatus.待支付,
                Module = CurrentInfo.ModuleCode,
                DBusiness = model.DBusiness,
                Shiftid = model.Shiftid,
                Mobile = model.Mobile,
                HandBillno = model.HandBillno,
                UsedDate = model.UsedDate,
                IsMsg = model.IsMsg,
                UsedDesc = model.UsedDesc,
                PayTransno = settleTransno   //微信，支付宝交易记录号
            };
            if (!string.IsNullOrEmpty(model.CardId))
            {
                addModel.Cardid = Guid.Parse(model.CardId);
                addModel.CardNo = oldmbrCardInfo.MbrCardNo;
            }

            service.Add(addModel);
            service.AddDataChangeLog(OpLogType.Pos定金添加);
            service.Commit();


            var businessPara = new PaymentOperateBusinessPara
            {
                Hid = CurrentInfo.HotelId,
                UserName = CurrentInfo.UserName,
                PosId = CurrentInfo.PosId,
                PosName = CurrentInfo.PosName,
                Billid = model.BillNo,
                Refeid = refe.Id,
                PosOutlteCode = posOutletCode,
                SettleTransNo = settleTransno
            };

            var addBillResult = new AddBillResult
            {
                BillNo = model.BillNo,
                BillRefeId = refe.Id,
                DetailId = addModel.Id.ToString(),
                DueAmount = addModel.Amount,
                UnpaidAmount = addModel.Amount,
                BillBsnsDate = addModel.DBusiness,
                BillShiftid = addModel.Shiftid,
                PosBusinessDate = pos.Business,
                ItemName = item?.Cname
            };
            businessPara.AddedBillResult = addBillResult;

            //财务处理参数
            var _model = new PaymentOperatePara()
            {
                MBillid = model.BillNo,
                Billid = model.BillNo,
                ProfileId = model.CardId,
                Itemid = payId,
                Amount = model.Amount,
                FolioItemAction = FolioItemAction,
                FolioItemActionJsonPara = model.FolioItemActionJsonPara,

            };

            var actionResult = payActionXmlHandler.DoOperate(_model, businessPara);
            if (!actionResult.Success)
            {
                return Json(actionResult);
            }

            addModel.IPrepay = (byte)PrePayStatus.交押金;
            service.Update(addModel, new YtPrepay());
            service.AddDataChangeLog(OpLogType.Pos定金修改);
            service.Commit();




            return Json(JsonResultData.Successed());
        }



        #endregion

        #region 菜谱

        [AuthButton(AuthFlag.AuthManage)]
        public ActionResult _PosBill(string billId)
        {
            var service = GetService<IPosBillService>();
            var bill = service.Get(billId);
            if (bill==null)
            {
                return Json(JsonResultData.Failure("预订账单不存在，请重新选择！"));
            }
            var url = Url.Action("Index", "PosInSingle", new { rnd = new Random().NextDouble() }) + "&refeid=" + System.Web.HttpUtility.UrlEncode(bill.Refeid) + "&tabid=" + System.Web.HttpUtility.UrlEncode(bill.Tabid) + "&billid=" + bill.Billid + "&tabFlag=" + bill.TabFlag + "&openFlag=D";
            return Json(JsonResultData.Successed(url));
        }
        #endregion

    }

    public class CustomComparer : System.Collections.IComparer

    {
        public int Compare(object x, object y)
        {
            string s1 = (string)x;
            string s2 = (string)y;
            if (s1.Length > s2.Length) return 1;
            if (s1.Length < s2.Length) return -1;
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i] > s2[i]) return 1;
                if (s1[i] < s2[i]) return -1;
            }
            return 0;
        }
    }
}