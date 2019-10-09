using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosTabStatus;
using Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using Stimulsoft.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos入单
    /// </summary> 
    [AuthPage(ProductType.Pos, "p60")]
    public class PosInSingleController : BaseEditInWindowController<PosBill, IPosBillService>
    {
        [AuthButton(AuthFlag.None)]
        public ActionResult Index(string tabid, string refeid, string billid, string tabFlag, string openFlag = "", string mode = "")
        {
            try
            {
                var flag = Request["flag"]; //餐饮模式直接进入入单界面，传一个flag 进来，用来控制界面按钮事件
                ViewBag.Flag = flag;
                ViewBag.openFlag = openFlag;    //该值用来控制退出到哪个界面（A：楼面，B：收银 C: 显示台号列表，退出到收银 D：预订界面）
                ViewBag.UserName = CurrentInfo.UserName;    //当前操作员
                ViewBag.HotelName = CurrentInfo.HotelName;    //当前酒店名称（本地报表打印使用）

                //2019-05-31 zk 添加自定义套餐是否显示添加提示框
                var pmsParaService = GetService<IPmsParaService>();
                ViewBag.IsShowSetMealMessagebox = pmsParaService.GetValue(CurrentInfo.HotelId, "PosIsShowSetMealMessagebox");

                //键盘点菜剩余1道菜时是否自动点菜
                var PosKeyAddItem = pmsParaService.GetValue(CurrentInfo.HotelId, "PosKeyAddItem");
                ViewBag.PosKeyAddItem = PosKeyAddItem;

                ViewBag.EnbaledAdvanceFunc = false;
                var auth = GetService<IAuthCheck>();
                if (auth.HasAuth(CurrentInfo.UserId, "p60", 2097152, CurrentInfo.HotelId))//判断是否拥有高级功能权限
                {
                    ViewBag.EnbaledAdvanceFunc = true;
                }


                InSingleViewModel model = new InSingleViewModel();

                var tabService = GetService<IPosTabService>();
                var tab = tabService.Get(tabid);

                var refeService = GetService<IPosRefeService>();
                var sessionRefe = Session["PosRefe"] as PosRefe;
                if (!string.IsNullOrWhiteSpace(refeid))
                {
                    var refe = refeService.Get(refeid);
                    if (sessionRefe != null && refe != null && sessionRefe != refe)
                    {
                        model.IsOpenBrush = refe.IsOpenBrush;
                        ViewBag.isCanItemPrint = refe.isCanItemPrint;//取消项目是否本地打印取消单
                        Session["PosRefe"] = refe;
                    }
                }
                else if (!string.IsNullOrEmpty(tabid))
                {
                    var refe = refeService.Get(tab.Refeid);
                    if (sessionRefe == null)
                    {
                        ViewBag.isCanItemPrint = refe.isCanItemPrint;//取消项目是否本地打印取消单
                        model.IsOpenBrush = refe.IsOpenBrush;
                        Session["PosRefe"] = refe;
                    }
                    else if (sessionRefe != null && refe != null && sessionRefe != refe)
                    {
                        ViewBag.isCanItemPrint = refe.isCanItemPrint;//取消项目是否本地打印取消单
                        model.IsOpenBrush = refe.IsOpenBrush;
                        Session["PosRefe"] = refe;
                    }
                }
                var pos = new PosPos();
                var posService = GetService<IPosPosService>();
                if (!string.IsNullOrEmpty(mode))
                {
                    pos = posService.GetPosByHid(CurrentInfo.HotelId).Where(w => w.PosMode == mode).FirstOrDefault();
                    if (pos != null)
                    {
                        CurrentInfo.PosId = pos.Id;
                        CurrentInfo.PosName = pos.Name;
                    }
                }
                else if (string.IsNullOrEmpty(CurrentInfo.PosId))
                {
                    pos = posService.GetPosByHid(CurrentInfo.HotelId).FirstOrDefault();
                    if (pos != null)
                    {
                        CurrentInfo.PosId = pos.Id;
                        CurrentInfo.PosName = pos.Name;
                    }
                }
                else
                {
                    pos = posService.Get(CurrentInfo.PosId);
                }

                if (!string.IsNullOrWhiteSpace(tabid) && Convert.ToByte(tabFlag) == (byte)PosBillTabFlag.物理台)
                {
                    #region 物理台

                    model = new InSingleViewModel() { Tabid = tabid, Refeid = tab.Refeid };
                    var billService = GetService<IPosBillService>();
                    var bill = billService.GetPosBillByTabIdForNotDelayed(CurrentInfo.HotelId, tab.Refeid, tabid, billid ?? "");
                    if (bill != null)
                    {
                        bill.ServiceRate = bill.ServiceRate >= 1 && bill.ServiceRate <= 100 ? bill.ServiceRate / 100 : bill.ServiceRate;
                        bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount;

                        var classServer = GetService<IPosItemClassService>();
                        ViewBag.PageTotal = classServer.GetPosItemClassTotal(CurrentInfo.HotelId, tab.Refeid, CurrentInfo.PosId);

                        var openItemService = GetService<IPosTabOpenItemService>();
                        var openItemList = openItemService.GetPosTabOpenItemByTabType(CurrentInfo.HotelId, tab.Module, tab.Refeid, tab.TabTypeid, "", (byte)PosITagperiod.随时, DateTime.Now);

                        if (bill != null)
                        {
                            var service = GetService<IPosBillDetailService>();
                            var itemService = GetService<IPosItemService>();
                            var billDetails = service.GetUpBillDetailByBillid(CurrentInfo.HotelId, bill.Billid, PosItemDcFlag.D.ToString());
                            if ((billDetails == null || billDetails.Count == 0) && openItemList != null && openItemList.Count > 0)
                            {
                                #region 增加开台项目

                                foreach (var temp in openItemList)
                                {
                                    if (temp.QuanMode == 1) //数量按人计
                                    {
                                        temp.Quantity = temp.Quantity * Convert.ToDecimal(bill.IGuest);
                                    }

                                    var item = itemService.GetEntity(CurrentInfo.HotelId, temp.Itemid);
                                    var amount = item.IsDiscount == true ? (temp.Price * temp.Quantity) * bill.Discount : (temp.Price * temp.Quantity);
                                    var server = item.IsService == true ? (temp.Price * temp.Quantity * bill.ServiceRate) : 0;

                                    //判断开台项目的收费状态
                                    var status = (byte)PosBillDetailStatus.保存;
                                    if (temp.IsCharge == 0)
                                    {
                                        status = (byte)PosBillDetailStatus.赠送;
                                    }
                                    else if (temp.IsCharge == 1)
                                    {
                                        status = (byte)PosBillDetailStatus.例送;
                                    }
                                    else
                                    {
                                        status = (byte)PosBillDetailStatus.保存;
                                    }

                                    #region 消费项目明细赋值

                                    PosBillDetail billDetail = new PosBillDetail()
                                    {
                                        MBillid = bill.MBillid,
                                        Billid = bill.Billid,
                                        Price = temp.Price,
                                        Quantity = temp.Quantity,
                                        Tabid = tab.Id,

                                        Hid = CurrentInfo.HotelId,
                                        Itemid = temp.Itemid,
                                        ItemCode = temp.itemCode,
                                        ItemName = temp.itemName,
                                        Unitid = temp.Unitid,
                                        UnitCode = temp.unitCode,
                                        UnitName = temp.unitName,
                                        DcFlag = PosItemDcFlag.D.ToString(),

                                        IsCheck = false,
                                        Isauto = (byte)PosBillDetailIsauto.开台项目,
                                        Status = status,
                                        //     IsProduce = (byte)PosBillDetailIsProduce.未出品,
                                        Dueamount = temp.Price * temp.Quantity,
                                        DiscAmount = 0,
                                        Discount = item.IsDiscount == true ? bill.Discount : 100,
                                        Amount = amount,
                                        //   Service = server,

                                        SP = false,
                                        SD = false,
                                        TransUser = CurrentInfo.UserName,
                                        TransBsnsDate = bill.BillBsnsDate,
                                        TransShiftid = bill.Shiftid,
                                        TransShuffleid = bill.Shuffleid,
                                        TransDate = DateTime.Now,

                                        PriceOri = temp.Price,
                                        PriceClub = temp.Price

                                    };

                                    #endregion 消费项目明细赋值

                                    billDetail.OrderType = "PC";

                                    service.Add(billDetail);
                                    // service.AddDataChangeLog(OpLogType.Pos账单消费明细增加);
                                    service.Commit();
                                    AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + billDetail.Id + ",名称：" + item.Cname + ",数量：" + billDetail.Quantity.ToString() + ",单位：" + billDetail.UnitName + ",金额：" + billDetail.Dueamount + ",状态：" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status), bill.BillNo);

                                }

                                #endregion 增加开台项目

                                billDetails = service.GetUpBillDetailByBillid(CurrentInfo.HotelId, bill.Billid, PosItemDcFlag.D.ToString());
                            }

                            #region 选中的消费项目

                            string itemIds = "";
                            if (billDetails != null && billDetails.Count > 0)
                            {
                                foreach (var item in billDetails)
                                {
                                    itemIds += item.Itemid + ",";
                                }
                            }

                            ViewBag.BillItemIds = itemIds.TrimEnd(',');
                            ViewBag.PosBill = bill;

                            #endregion 选中的消费项目
                        }

                        if (sessionRefe == null)
                        {
                            var refe = refeService.Get(tab.Refeid);
                            model.IsOpenBrush = refe.IsOpenBrush;
                            Session["PosRefe"] = refe;
                        }
                        else
                        {
                            model.IsOpenBrush = sessionRefe.IsOpenBrush;
                        }
                        PosCommon common = new PosCommon();
                        common.SetRedisBill(bill.Billid);  //保存数据到redis
                        ViewBag.billStatus = bill.Status;
                    }
                    else
                    {
                        //更新餐台状态
                        var tabStatusService = GetService<IPosTabStatusService>();
                        var tabStatus = tabStatusService.GetPosTabStatus(CurrentInfo.HotelId, tab.Refeid, tabid);
                        if (tabStatus != null)
                        {
                            tabStatus.TabStatus = (byte)PosTabStatusEnum.空净;
                            tabStatusService.Update(tabStatus, new PosTabStatus());
                            tabStatusService.AddDataChangeLog(OpLogType.Pos餐台状态修改);
                            tabStatusService.Commit();
                        }
                        return RedirectToAction("Index", "PosTabStatus");
                    }

                    #endregion 物理台
                }
                else if (!string.IsNullOrWhiteSpace(billid) && Convert.ToByte(tabFlag) == (byte)PosBillTabFlag.快餐台)
                {
                    #region 快餐台

                    var billService = GetService<IPosBillService>();
                    var posBill = billService.Get(billid);
                    var bill = billService.GetPosBillByTabIdForNotDelayed(CurrentInfo.HotelId, posBill.Refeid, posBill.Tabid);
                    model = new InSingleViewModel() { Tabid = bill.Tabid, Refeid = bill.Refeid, Billid = bill.Billid };

                    bill.ServiceRate = bill.ServiceRate >= 1 && bill.ServiceRate <= 100 ? bill.ServiceRate / 100 : bill.ServiceRate;
                    bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount;

                    var classServer = GetService<IPosItemClassService>();
                    ViewBag.PageTotal = classServer.GetPosItemClassTotal(CurrentInfo.HotelId, bill.Refeid, CurrentInfo.PosId);

                    if (bill != null)
                    {
                        var service = GetService<IPosBillDetailService>();
                        var billDetails = service.GetUpBillDetailByBillid(CurrentInfo.HotelId, bill.Billid, PosItemDcFlag.D.ToString());

                        #region 选中的消费项目

                        string itemIds = "";
                        if (billDetails != null && billDetails.Count > 0)
                        {
                            foreach (var item in billDetails)
                            {
                                itemIds += item.Itemid + ",";
                            }
                        }

                        ViewBag.BillItemIds = itemIds.TrimEnd(',');
                        ViewBag.PosBill = bill;

                        #endregion 选中的消费项目
                    }

                    if (sessionRefe == null && refeid != null)
                    {
                        var refe = refeService.Get(bill.Refeid);
                        model.IsOpenBrush = refe.IsOpenBrush;
                        Session["PosRefe"] = refe;
                    }

                    #endregion 快餐台
                }
                else
                {
                    //快餐模式
                    if (mode == "B")
                    {
                        if (string.IsNullOrWhiteSpace(billid))
                        {
                            #region 封装程序 无餐台
                            var billService = GetService<IPosBillService>();
                            var refeList = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
                            if (refeList != null && refeList.Count > 0)
                            {
                                model = new InSingleViewModel { Tabid = "", Refeid = refeList[0].Id };
                                var classServer = GetService<IPosItemClassService>();
                                ViewBag.PageTotal = classServer.GetPosItemClassTotal(CurrentInfo.HotelId, refeList[0].ShuffleId, CurrentInfo.PosId);
                                var bill = billService.GetBillListForPosRefe(CurrentInfo.HotelId, refeList[0].Id, pos.Business).FirstOrDefault();
                                var upbill = new up_pos_list_billByRefeidAndTabidResult();
                                if (bill == null)
                                {
                                    //根据营业日生成虚拟台id 从1开始
                                    string xntabid = string.Empty;
                                    //最新虚拟台id
                                    var zxxntabid = billService.GetAllBillListForPosRefe(CurrentInfo.HotelId, refeList[0].Id, pos.Business).Select(t => t.Tabid).Max();
                                    if (string.IsNullOrWhiteSpace(zxxntabid))
                                    {
                                        xntabid = "1";
                                    }
                                    else
                                    {
                                        int lastNumber = int.Parse(zxxntabid);
                                        lastNumber++;
                                        xntabid = lastNumber.ToString();
                                    }

                                    var posBill = billService.GetLastBillId(CurrentInfo.HotelId, refeList[0].Id, pos.Business);
                                    PosBill newbill = new PosBill()
                                    {
                                        Hid = CurrentInfo.HotelId,
                                        Billid = posBill.Billid,
                                        BillNo = posBill.BillNo,
                                        MBillid = posBill.Billid,
                                        InputUser = CurrentInfo.UserName,
                                        BillDate = DateTime.Now,
                                        IsService = true,
                                        IsLimit = true,
                                        Shiftid = pos.ShiftId,
                                        Shuffleid = refeList[0].ShuffleId,
                                        Refeid = refeList[0].Id,
                                        BillBsnsDate = pos.Business,
                                        TabFlag = (byte)PosBillTabFlag.快餐台,
                                        Status = (byte)PosBillStatus.开台,
                                        Discount = 100,
                                        ServiceRate = 0,
                                        Tabid = xntabid,
                                        TabNo = xntabid
                                    };
                                    billService.Add(newbill);
                                    billService.Commit();

                                    var shuffleService = GetService<IPosShuffleService>();
                                    var shiftService = GetService<IPosShiftService>();

                                    var refe = refeService.GetEntity(CurrentInfo.HotelId, newbill.Refeid);
                                    var shuffle = shuffleService.GetEntity(CurrentInfo.HotelId, newbill.Shuffleid);
                                    var shift = shiftService.GetEntity(CurrentInfo.HotelId, pos.ShiftId);
                                    upbill.Billid = newbill.Billid;
                                    upbill.BillNo = newbill.BillNo;
                                    upbill.MBillid = newbill.MBillid;
                                    upbill.Refeid = refe.Id;
                                    upbill.RefeName = refe.Cname;
                                    upbill.Shuffleid = refe.ShuffleId;
                                    upbill.BillBsnsDate = pos.Business;
                                    upbill.ShuffleName = shuffle.Cname;
                                    upbill.Shiftid = shift.Id;
                                    upbill.ShiftName = shift.Name;
                                    upbill.InputUser = CurrentInfo.UserName;
                                    upbill.Discount = 100;
                                    upbill.ServiceRate = 0;
                                    upbill.Tabid = xntabid;
                                    upbill.TabNo = xntabid;
                                }
                                else
                                {
                                    var shuffleService = GetService<IPosShuffleService>();
                                    var shiftService = GetService<IPosShiftService>();
                                    AutoSetValueHelper.SetValues(bill, upbill);
                                    var refe = refeService.GetEntity(CurrentInfo.HotelId, bill.Refeid);
                                    var shuffle = shuffleService.GetEntity(CurrentInfo.HotelId, bill.Shuffleid);
                                    var shift = shiftService.GetEntity(CurrentInfo.HotelId, pos.ShiftId);
                                    upbill.MBillid = bill.MBillid;
                                    upbill.Refeid = refe.Id;
                                    upbill.RefeName = refe.Cname;
                                    upbill.Shuffleid = refe.ShuffleId;
                                    upbill.BillBsnsDate = pos.Business;
                                    upbill.ShuffleName = shuffle.Cname;
                                    upbill.Shiftid = shift.Id;
                                    upbill.ShiftName = shift.Name;
                                    upbill.Tabid = bill.Tabid;
                                    upbill.TabNo = bill.TabNo;
                                    upbill.ServiceRate = upbill.ServiceRate >= 1 && upbill.ServiceRate <= 100 ? upbill.ServiceRate / 100 : upbill.ServiceRate;
                                    upbill.Discount = upbill.Discount >= 1 && upbill.Discount <= 100 ? upbill.Discount / 100 : upbill.Discount;
                                }
                                ViewBag.PosBill = upbill;
                                if (string.IsNullOrWhiteSpace(refeid) || refeid != "undefined" || (refeList[0].Id != null && refeid != refeList[0].Id))
                                {
                                    var refe = refeService.Get(refeList[0].Id);
                                    Session["PosRefe"] = refe;
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            var billService = GetService<IPosBillService>();
                            var refeList = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
                            var upbill = new up_pos_list_billByRefeidAndTabidResult();
                            if (refeList != null && refeList.Count > 0)
                            {
                                model = new InSingleViewModel { Tabid = "", Refeid = refeList[0].Id };
                                var bill = billService.GetBillListForPosRefe(CurrentInfo.HotelId, refeList[0].Id, pos.Business).Where(t => t.Billid == billid).FirstOrDefault();
                                var shuffleService = GetService<IPosShuffleService>();
                                var shiftService = GetService<IPosShiftService>();
                                AutoSetValueHelper.SetValues(bill, upbill);
                                var refe = refeService.GetEntity(CurrentInfo.HotelId, bill.Refeid);
                                var shuffle = shuffleService.GetEntity(CurrentInfo.HotelId, bill.Shuffleid);
                                var shift = shiftService.GetEntity(CurrentInfo.HotelId, pos.ShiftId);
                                upbill.MBillid = bill.MBillid;
                                upbill.Refeid = refe.Id;
                                upbill.RefeName = refe.Cname;
                                upbill.Shuffleid = refe.ShuffleId;
                                upbill.BillBsnsDate = pos.Business;
                                upbill.ShuffleName = shuffle.Cname;
                                upbill.Shiftid = shift.Id;
                                upbill.ShiftName = shift.Name;
                                upbill.Tabid = bill.Tabid;
                                upbill.TabNo = bill.TabNo;
                                upbill.ServiceRate = upbill.ServiceRate >= 1 && upbill.ServiceRate <= 100 ? upbill.ServiceRate / 100 : upbill.ServiceRate;
                                upbill.Discount = upbill.Discount >= 1 && upbill.Discount <= 100 ? upbill.Discount / 100 : upbill.Discount;
                            }
                            ViewBag.PosBill = upbill;
                            if (string.IsNullOrWhiteSpace(refeid) || refeid != "undefined" || (refeList[0].Id != null && refeid != refeList[0].Id))
                            {
                                var refe = refeService.Get(refeList[0].Id);
                                Session["PosRefe"] = refe;
                            }
                        }
                    }
                    else
                    {
                        #region 未选择餐台
                        var refeList = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
                        if (refeList != null && refeList.Count > 0)
                        {
                            model = new InSingleViewModel { Tabid = "", Refeid = refeList[0].Id };
                            var classServer = GetService<IPosItemClassService>();
                            ViewBag.PageTotal = classServer.GetPosItemClassTotal(CurrentInfo.HotelId, refeList[0].ShuffleId, CurrentInfo.PosId);

                            var billService = GetService<IPosBillService>();
                            var bill = billService.GetPosBillByTabIdForNotDelayed(CurrentInfo.HotelId, model.Refeid, model.Tabid);
                            if (bill == null)
                            {
                                bill = new up_pos_list_billByRefeidAndTabidResult();

                                var shuffleService = GetService<IPosShuffleService>();
                                var shiftService = GetService<IPosShiftService>();

                                var refe = refeService.GetEntity(CurrentInfo.HotelId, model.Refeid);
                                var shuffle = shuffleService.GetEntity(CurrentInfo.HotelId, refe.ShuffleId);
                                var shift = shiftService.GetEntity(CurrentInfo.HotelId, CurrentInfo.PosId);

                                bill.Refeid = refe.Id;
                                bill.RefeName = refe.Cname;
                                bill.Shuffleid = refe.ShuffleId;
                                bill.BillBsnsDate = pos.Business;
                                bill.ShuffleName = shuffle.Cname;
                                bill.Shiftid = shift.Id;
                                bill.ShiftName = shift.Name;
                                bill.InputUser = CurrentInfo.UserName;
                                bill.Discount = 100;
                                bill.ServiceRate = 0;
                            }
                            else
                            {
                                bill.ServiceRate = bill.ServiceRate >= 1 && bill.ServiceRate <= 100 ? bill.ServiceRate / 100 : bill.ServiceRate;
                                bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount;
                            }
                            ViewBag.PosBill = bill;
                            if (string.IsNullOrWhiteSpace(refeid) || refeid != "undefined" || (refeList[0].Id != null && refeid != refeList[0].Id))
                            {
                                var refe = refeService.Get(refeList[0].Id);
                                Session["PosRefe"] = refe;
                            }
                        }
                        #endregion 未选择餐台
                    }
                }

                //提示信息显示时间

                ViewBag.TipsTime = pmsParaService.GetValue(CurrentInfo.HotelId, "tipsTime");

                //是否是通过快捷方式进行自动登录的用户
                ViewBag.IsAutoLogin = CurrentInfo.UserCode.Equals("posAutoLogin", StringComparison.CurrentCultureIgnoreCase) || CurrentInfo.UserName.Equals("Pos自动登录", StringComparison.CurrentCultureIgnoreCase) ? true : false;

                ViewBag.Pos = pos;
                ViewBag.Version = CurrentVersion;
                //特价菜日期类型
                var PosItemClassService = GetService<IPosItemClassService>();
                var PosItemClass = PosItemClassService.GetPosItemClassByRefe(CurrentInfo.HotelId, refeid ?? "", 1, 99999, CurrentInfo.PosId).Where(w => w.Code == "TJC" && w.Id == "0").FirstOrDefault();
                if (PosItemClass != null)
                {
                    ViewBag.iTagperiod = PosItemClass.iTagperiod;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region
        /// <summary>
        /// 营业点
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult _PosRefeList()
        {
            return PartialView("_PosRefeList");
        }

        /// <summary>
        /// 增加消费项目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult AddBillDetail(PosBillDetailAddViewModel model)
        {
            var billService = GetService<IPosBillService>();
            var service = GetService<IPosBillDetailService>();

            var PosOnSaleService = GetService<IPosOnSaleService>();//特价菜

            //消费项目
            var itemService = GetService<IPosItemService>();
            var item = itemService.GetEntity(CurrentInfo.HotelId, model.Itemid);

            //单位
            var unitService = GetService<IPosUnitService>();
            var unit = unitService.GetEntity(CurrentInfo.HotelId, model.Unitid);

            //消费项目价格
            var itemPriceService = GetService<IPosItemPriceService>();
            var itemPrice = itemPriceService.GetPosItemPriceByUnitid(CurrentInfo.HotelId, model.Itemid, model.Unitid);

            //账单已经存在。添加消费项目明细
            if (model != null && model.Billid != null && model.Itemid != null)
            {
                try
                {
                    //判断选择的项目是否沽清
                    var SelloutService = GetService<IPosSellOutService>();
                    var PosSellout = SelloutService.GetPosSelloutByItemId(CurrentInfo.HotelId, model.Itemid);
                    if (PosSellout != null && PosSellout.SellStatus == 0)
                    {
                        if (string.IsNullOrWhiteSpace(PosSellout.UnitId))
                        {
                            //单位全部沽清
                            return Json(JsonResultData.Failure(item.Cname + "已沽清"));
                        }
                        else
                        {
                            if (PosSellout.UnitId.Contains(model.Unitid))
                            {
                                return Json(JsonResultData.Failure(item.Cname + "_" + unit.Cname + "已沽清"));
                            }
                        }
                    }

                    //验证是否连接云仓库，并且进行操作
                    PosCmpStock cmpStock = new PosCmpStock();
                    var res = cmpStock.IsPosReduceStock(model, item, unit);

                    return Json(res.Data);
                    //  return Json(JsonResultData.Successed());
                    //   service.StatisticsBillDetail(CurrentInfo.HotelId, bill.Billid, bill.MBillid);

                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex.Message.ToString()));
                }

                //  return Json(JsonResultData.Successed());
            }
            else
            {
                try
                {
                    #region 增加账单

                    var refe = Session["PosRefe"] as PosRefe;

                    var posService = GetService<IPosPosService>();
                    var pos = posService.Get(CurrentInfo.PosId);

                    var tabid = billService.GetTakeoutForTabid(CurrentInfo.HotelId, refe.Id, pos.Business, (byte)PosBillTabFlag.快餐台);
                    var posBill = billService.GetLastBillId(CurrentInfo.HotelId, refe.Id, pos.Business);

                    model.Tabid = string.IsNullOrWhiteSpace(model.Tabid) ? CurrentInfo.HotelId + tabid : model.Tabid;
                    model.Billid = string.IsNullOrWhiteSpace(model.Billid) ? posBill.Billid : model.Billid;
                    model.MBillid = string.IsNullOrWhiteSpace(model.MBillid) ? posBill.Billid : model.MBillid;

                    #region 账单赋值

                    PosBill bill = new PosBill()
                    {
                        Hid = CurrentInfo.HotelId,
                        Refeid = refe.Id,
                        Billid = posBill.Billid,
                        BillNo = posBill.BillNo,
                        MBillid = posBill.Billid,
                        InputUser = CurrentInfo.UserName,

                        IGuest = 1,
                        TabNo = tabid,
                        BillBsnsDate = pos.Business,
                        BillDate = DateTime.Now,
                        IsService = true,
                        IsLimit = true,
                        Status = (byte)PosBillStatus.开台,
                        TabFlag = (byte)PosBillTabFlag.快餐台,
                        Shiftid = pos.ShiftId,
                        Shuffleid = refe.ShuffleId,
                        Discount = 100,
                        ServiceRate = 0
                    };

                    bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount ?? 0;
                    bill.ServiceRate = bill.ServiceRate >= 1 && bill.ServiceRate <= 100 ? bill.ServiceRate / 100 : bill.ServiceRate ?? 0;

                    #endregion 账单赋值

                    AutoSetValueHelper.SetValues(model, bill);
                    billService.Add(bill);
                    billService.Commit();
                    AddOperationLog(OpLogType.Pos账单增加, "营业点：" + refe.Cname + ",营业日：" + pos.Business + " ,餐台号：" + tabid, bill.BillNo);

                    #endregion 增加账单

                    bill = billService.Get(bill.Billid);

                    var billDetailService = GetService<IPosBillDetailService>();
                    bool isExists = service.IsExists(CurrentInfo.HotelId, bill.Billid, model.Itemid);

                    if (isExists)
                    {
                        var billDetail = billDetailService.GetBillDetailByBillid(CurrentInfo.HotelId, bill.Billid, model.Itemid);
                        var oldBillDetail = new PosBillDetail();
                        AutoSetValueHelper.SetValues(billDetail, oldBillDetail);
                        #region 账单明细赋值

                        if (billDetail.Unitid != model.Unitid)
                        {
                            billDetail.Unitid = model.Unitid;
                            billDetail.UnitCode = unit.Code;
                            billDetail.UnitName = unit.Cname;
                            billDetail.Price = model.Price;
                            billDetail.IsProduce = (byte)PosBillDetailIsProduce.修改;
                            billDetail.Dueamount = model.Price * billDetail.Quantity;
                        }
                        else
                        {
                            billDetail.Quantity = billDetail.Quantity + model.Quantity;
                            billDetail.Dueamount = model.Price * billDetail.Quantity;
                        }

                        billDetail.Discount = billDetail.Discount >= 1 && billDetail.Discount <= 100 ? billDetail.Discount / 100 : billDetail.Discount;
                        var discAmount = item.IsDiscount == true ? billDetail.Dueamount - billDetail.Dueamount * bill.Discount : 0;
                        var serviceAmount = item.IsService == true ? billDetail.Price * billDetail.Quantity * bill.ServiceRate : 0;
                        var amount = item.IsDiscount == true ? (billDetail.Price * billDetail.Quantity - billDetail.DiscAmount) * billDetail.Discount
                            : (billDetail.Price * billDetail.Quantity);

                        billDetail.Amount = amount;
                        billDetail.DiscAmount = discAmount;
                        billDetail.Service = serviceAmount;



                        #endregion 账单明细赋值

                        billDetailService.Update(billDetail, new PosBillDetail());
                        billDetailService.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + ",台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，数量：" + oldBillDetail.Quantity + "->" + billDetail.Quantity + "，金额：" + oldBillDetail.Dueamount + "->" + billDetail.Dueamount + ",原价：" + billDetail.PriceOri.ToString() + ",会员价：" + billDetail.PriceClub.ToString() + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status), bill.BillNo);
                    }
                    else
                    {
                        var dueamount = model.Price * model.Quantity;
                        var discAmount = item.IsDiscount == true ? dueamount - (dueamount * bill.Discount) : 0;
                        var amount = item.IsDiscount == true ? dueamount * bill.Discount : dueamount;
                        var serviceAmount = item.IsService == true ? dueamount * bill.ServiceRate : 0;

                        #region 账单明细赋值

                        PosBillDetail billDetail = new PosBillDetail()
                        {
                            Hid = CurrentInfo.HotelId,
                            Billid = bill.Billid,
                            MBillid = bill.MBillid,
                            ItemCode = item.Code,
                            ItemName = item.Cname,
                            UnitCode = unit.Code,
                            UnitName = unit.Cname,
                            Price = model.Price,
                            DcFlag = PosItemDcFlag.D.ToString(),
                            IsCheck = false,
                            Isauto = (byte)PosBillDetailIsauto.录入项目,
                            Status = (byte)PosBillDetailStatus.保存,
                            IsProduce = (byte)PosBillDetailIsProduce.未出品,
                            Dueamount = dueamount,
                            DiscAmount = discAmount,
                            Discount = bill.Discount,
                            Amount = amount,
                            Service = serviceAmount,
                            SP = false,
                            SD = false,
                            TransUser = CurrentInfo.UserName,
                            TransBsnsDate = bill.BillBsnsDate,
                            TransShiftid = bill.Shiftid,
                            TransShuffleid = bill.Shuffleid,
                            TransDate = DateTime.Now,
                        };

                        #endregion 账单明细赋值

                        AutoSetValueHelper.SetValues(model, billDetail);
                        billDetail.OrderType = "PC";
                        billDetail.IsWeight = false;    //添加海鲜消费项目默认是未称重
                        billDetailService.Add(billDetail);
                        //billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细增加);
                        billDetailService.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + billDetail.Id + ",名称：" + billDetail.ItemName + ",数量：" + billDetail.Quantity.ToString() + ",单位：" + billDetail.UnitName + ",金额：" + billDetail.Dueamount.ToString() + ",原价：" + billDetail.PriceOri.ToString() + ",会员价：" + billDetail.PriceClub.ToString() + ",状态：" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status), bill.BillNo);


                    }

                    //   billDetailService.StatisticsBillDetail(CurrentInfo.HotelId, bill.Billid, bill.MBillid);

                    return Json(JsonResultData.Successed(bill));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
        }

        /// <summary>
        /// 添加消费项目库存提示进行再次添加的操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult AddBillDetailB(PosBillDetailAddViewModel model)
        {
            //消费项目
            var itemService = GetService<IPosItemService>();
            var item = itemService.GetEntity(CurrentInfo.HotelId, model.Itemid);

            //单位
            var unitService = GetService<IPosUnitService>();
            var unit = unitService.GetEntity(CurrentInfo.HotelId, model.Unitid);
            PosCmpStock cmpStock = new PosCmpStock();
            string errMsg = "";
            var result = cmpStock.AddPosBillCost(item, unit, model, out errMsg, "1");
            if (!result)
            {
                return Json(JsonResultData.Failure(errMsg));
            }
            return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 增加手写消费项目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult AddHandWrite(PosBillDetailAddViewModel model)
        {
            var billService = GetService<IPosBillService>();
            var service = GetService<IPosBillDetailService>();

            var itemService = GetService<IPosItemService>();
            var item = itemService.GetEntity(CurrentInfo.HotelId, model.Itemid);

            var unitService = GetService<IPosUnitService>();
            var unit = unitService.GetEntity(CurrentInfo.HotelId, model.Unitid);

            var itemPriceService = GetService<IPosItemPriceService>();
            var itemPrice = itemPriceService.GetPosItemPriceByUnitid(CurrentInfo.HotelId, model.Itemid, model.Unitid);

            #region 会员价查询以及设置
            //验证消费项目

            //设置会员价
            model.PriceClub = model.Price;
            model.PriceOri = model.Price;
            #endregion



            if (model != null && model.Billid != null && model.Itemid != null)
            {
                try
                {
                    #region 增加消费明细
                    var bill = billService.Get(model.Billid);

                    bill.ServiceRate = bill.ServiceRate >= 1 && bill.ServiceRate <= 100 ? bill.ServiceRate / 100 : bill.ServiceRate;
                    bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount;

                    bool isExists = service.IsExistsForLD(CurrentInfo.HotelId, bill.Billid, item.Id, item.Cname);
                    if (isExists)
                    {
                        var billDetail = service.GetBillDetailByBillidForLD(CurrentInfo.HotelId, bill.Billid, item.Id, item.Cname);
                        var oldBillDetail = new PosBillDetail();
                        AutoSetValueHelper.SetValues(billDetail, oldBillDetail);
                        #region 赋值
                        if (billDetail.Unitid != model.Unitid)
                        {
                            billDetail.Quantity = model.Quantity;
                            billDetail.Unitid = model.Unitid;
                            billDetail.UnitCode = unit.Code;
                            billDetail.UnitName = unit.Cname;
                            billDetail.Price = model.Price;
                            billDetail.Dueamount = model.Price * billDetail.Quantity;
                            billDetail.PriceClub = model.PriceClub;  //设置会员价                            
                        }
                        else
                        {
                            billDetail.Quantity = model.Quantity;
                            billDetail.Dueamount = model.Price * billDetail.Quantity;
                        }
                        //如果是套餐头
                        if (billDetail.SP == true && billDetail.SD == false)
                        {
                            //查找明细并更新数量，单价不变
                            var SDlist = service.GetBillDetailByUpid(CurrentInfo.HotelId, billDetail.Billid, billDetail.Upid).Where(t => t.Itemid != billDetail.Itemid).ToList();
                            if (SDlist.Count > 0)
                            {
                                foreach (var sditem in SDlist)
                                {
                                    sditem.Quantity = sditem.Quantity * model.Quantity;
                                    service.Update(sditem, new PosBillDetail());
                                    service.Commit();
                                }
                            }
                        }
                        billDetail.ItemName = model.ItemName;
                        billDetail.Discount = billDetail.Discount >= 1 && billDetail.Discount <= 100 ? billDetail.Discount / 100 : billDetail.Discount;
                        var serviceAmount = item.IsService == true ? billDetail.Price * billDetail.Quantity * bill.ServiceRate : 0;
                        var amount = item.IsDiscount == true ? (billDetail.Price * billDetail.Quantity - billDetail.DiscAmount) * billDetail.Discount
                            : (billDetail.Price * billDetail.Quantity);

                        billDetail.Amount = amount;
                        billDetail.Service = serviceAmount;

                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + ",台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，数量：" + oldBillDetail.Quantity + "->" + billDetail.Quantity + "，金额：" + oldBillDetail.Dueamount + "->" + billDetail.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status), bill.BillNo);

                        #endregion 赋值

                        service.Update(billDetail, new PosBillDetail());
                        service.Commit();
                    }
                    else
                    {
                        var dueamount = model.Price * model.Quantity;
                        //   var discAmount = model.IsDiscount == true ? dueamount - (dueamount * bill.Discount) : 0;
                        var amount = item.IsDiscount == true ? dueamount * bill.Discount : dueamount;
                        var serviceAmount = item.IsService == true ? dueamount * bill.ServiceRate : 0;

                        #region 赋值

                        PosBillDetail billDetail = new PosBillDetail()
                        {
                            Hid = CurrentInfo.HotelId,
                            ItemCode = item.Code,
                            ItemName = item.Cname,
                            UnitCode = model.Unitid,
                            UnitName = unit.Cname,
                            Price = model.Price,
                            DcFlag = PosItemDcFlag.D.ToString(),
                            IsCheck = false,
                            Isauto = (byte)PosBillDetailIsauto.录入项目,
                            Status = (byte)PosBillDetailStatus.保存,
                            //    IsProduce = (byte)PosBillDetailIsProduce.未出品,
                            Dueamount = dueamount,
                            //  DiscAmount = discAmount,
                            Discount = bill.Discount,
                            Amount = amount,
                            Service = serviceAmount,
                            SP = false,
                            SD = false,
                            TransUser = CurrentInfo.UserName,
                            TransBsnsDate = bill.BillBsnsDate,
                            TransShiftid = bill.Shiftid,
                            TransShuffleid = bill.Shuffleid,
                            TransDate = DateTime.Now,
                        };

                        #endregion 赋值

                        AutoSetValueHelper.SetValues(model, billDetail);
                        service.Add(billDetail);

                        service.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + billDetail.Id + ",名称：" + billDetail.ItemName + ",数量：" + billDetail.Quantity.ToString() + ",单位：" + billDetail.UnitName + ",金额：" + billDetail.Dueamount.ToString() + ",状态：" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status), bill.BillNo);

                    }

                    #endregion 增加消费明细

                    //   service.StatisticsBillDetail(CurrentInfo.HotelId, bill.Billid, bill.MBillid);
                    return Json(JsonResultData.Successed(bill));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
            else
            {
                try
                {
                    #region 增加账单

                    var refe = Session["PosRefe"] as PosRefe;

                    var posService = GetService<IPosPosService>();
                    var pos = posService.Get(CurrentInfo.PosId);

                    var tabid = billService.GetTakeoutForTabid(CurrentInfo.HotelId, refe.Id, pos.Business, (byte)PosBillTabFlag.快餐台);
                    var posBill = billService.GetLastBillId(CurrentInfo.HotelId, refe.Id, pos.Business);

                    model.Tabid = string.IsNullOrWhiteSpace(model.Tabid) ? CurrentInfo.HotelId + tabid : model.Tabid;
                    model.Billid = string.IsNullOrWhiteSpace(model.Billid) ? posBill.Billid : model.Billid;
                    model.MBillid = string.IsNullOrWhiteSpace(model.MBillid) ? posBill.Billid : model.MBillid;

                    #region 账单赋值

                    PosBill bill = new PosBill()
                    {
                        Hid = CurrentInfo.HotelId,
                        Refeid = refe.Id,
                        Billid = posBill.Billid,
                        BillNo = posBill.BillNo,
                        MBillid = posBill.Billid,
                        InputUser = CurrentInfo.UserName,

                        IGuest = 1,
                        TabNo = tabid,
                        BillBsnsDate = pos.Business,
                        BillDate = DateTime.Now,
                        IsService = true,
                        IsLimit = true,
                        Status = (byte)PosBillStatus.开台,
                        TabFlag = (byte)PosBillTabFlag.快餐台,
                        Shiftid = pos.ShiftId,
                        Shuffleid = refe.ShuffleId,
                        Discount = 100,
                        ServiceRate = 0
                    };

                    bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount ?? 0;
                    bill.ServiceRate = bill.ServiceRate >= 1 && bill.ServiceRate <= 100 ? bill.ServiceRate / 100 : bill.ServiceRate ?? 0;

                    #endregion 账单赋值

                    AutoSetValueHelper.SetValues(model, bill);
                    billService.Add(bill);
                    billService.Commit();
                    AddOperationLog(OpLogType.Pos账单增加, "营业点：" + refe.Cname + ",营业日：" + pos.Business + " ,餐台号：" + tabid, bill.BillNo);

                    #endregion 增加账单

                    bill = billService.Get(bill.Billid);

                    var billDetailService = GetService<IPosBillDetailService>();
                    bool isExists = service.IsExistsForLD(CurrentInfo.HotelId, bill.Billid, model.Itemid, item.Cname);
                    if (isExists)
                    {
                        var billDetail = billDetailService.GetBillDetailByBillid(CurrentInfo.HotelId, bill.Billid, model.Itemid, model.ItemName);
                        var oldBillDetail = new PosBillDetail();
                        AutoSetValueHelper.SetValues(billDetail, oldBillDetail);

                        #region 账单明细赋值

                        if (billDetail.Unitid != model.Unitid)
                        {
                            billDetail.Unitid = model.Unitid;
                            billDetail.UnitCode = unit.Code;
                            billDetail.UnitName = unit.Cname;
                            billDetail.Price = model.Price;
                            //billDetail.IsProduce = (byte)PosBillDetailIsProduce.修改;
                            billDetail.Dueamount = model.Price * billDetail.Quantity;
                        }
                        else
                        {
                            billDetail.Quantity = billDetail.Quantity + model.Quantity;
                            billDetail.Dueamount = model.Price * billDetail.Quantity;
                        }

                        billDetail.Discount = billDetail.Discount >= 1 && billDetail.Discount <= 100 ? billDetail.Discount / 100 : billDetail.Discount;
                        var discAmount = item.IsDiscount == true ? billDetail.Dueamount - billDetail.Dueamount * bill.Discount : 0;
                        var serviceAmount = item.IsService == true ? billDetail.Price * billDetail.Quantity * bill.ServiceRate : 0;
                        var amount = item.IsDiscount == true ? (billDetail.Price * billDetail.Quantity - billDetail.DiscAmount) * billDetail.Discount
                            : (billDetail.Price * billDetail.Quantity);

                        billDetail.Amount = amount;
                        billDetail.DiscAmount = discAmount;
                        billDetail.Service = serviceAmount;

                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，数量：" + oldBillDetail.Quantity + "->" + billDetail.Quantity + "，金额：" + oldBillDetail.Amount + "->" + billDetail.Amount + "，服务费：" + oldBillDetail.Service + "->" + billDetail.Service, billDetail.Billid);

                        #endregion 账单明细赋值

                        billDetailService.Update(billDetail, new PosBillDetail());

                        billDetailService.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + ",名称：" + billDetail.ItemName + ",数量：" + oldBillDetail.Quantity + "-->" + billDetail.Quantity.ToString() + ",单位：" + oldBillDetail.UnitName + "-->" + billDetail.UnitName + ",金额：" + oldBillDetail.Quantity * oldBillDetail.Price + "-->" + billDetail.Dueamount + ",状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status), bill.BillNo);

                    }
                    else
                    {
                        var dueamount = model.Price * model.Quantity;
                        var discAmount = item.IsDiscount == true ? dueamount - (dueamount * bill.Discount) : 0;
                        var amount = item.IsDiscount == true ? dueamount * bill.Discount : dueamount;
                        var serviceAmount = item.IsService == true ? dueamount * bill.ServiceRate : 0;

                        #region 账单明细赋值

                        PosBillDetail billDetail = new PosBillDetail()
                        {
                            Hid = CurrentInfo.HotelId,
                            Billid = bill.Billid,
                            MBillid = bill.MBillid,
                            ItemCode = item.Code,
                            ItemName = item.Cname,
                            UnitCode = unit.Code,
                            UnitName = unit.Cname,
                            Price = model.Price,
                            DcFlag = PosItemDcFlag.D.ToString(),
                            IsCheck = false,
                            Isauto = (byte)PosBillDetailIsauto.录入项目,
                            Status = (byte)PosBillDetailStatus.保存,
                            //      IsProduce = (byte)PosBillDetailIsProduce.未出品,
                            Dueamount = dueamount,
                            DiscAmount = discAmount,
                            Discount = bill.Discount,
                            Amount = amount,
                            Service = serviceAmount,
                            SP = false,
                            SD = false,
                            TransUser = CurrentInfo.UserName,
                            TransBsnsDate = bill.BillBsnsDate,
                            TransShiftid = bill.Shiftid,
                            TransShuffleid = bill.Shuffleid,
                            TransDate = DateTime.Now,
                        };

                        #endregion 账单明细赋值

                        AutoSetValueHelper.SetValues(model, billDetail);
                        billDetailService.Add(billDetail);

                        billDetailService.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + billDetail.Id + ",名称：" + billDetail.ItemName + ",数量：" + billDetail.Quantity.ToString() + ",单位：" + billDetail.UnitName + ",金额：" + billDetail.Dueamount.ToString() + ",状态：" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status), bill.BillNo);

                    }

                    //   billDetailService.StatisticsBillDetail(CurrentInfo.HotelId, bill.Billid, bill.MBillid);

                    return Json(JsonResultData.Successed(bill));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
        }

        /// <summary>
        /// 取消锁台
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.ChangeCardNum)]
        public ActionResult CancelLockTab(PosTabLogAddViewModel model)
        {
            PosCommon Common = new PosCommon();



            if (!string.IsNullOrEmpty(model.Billid) && !string.IsNullOrEmpty(model.Tabid) && !string.IsNullOrEmpty(model.Refeid))
            {
                try
                {
                    var orderList = Common.GetOrderItem(model.Billid, "");//获取点菜单需要的数据


                    var billDetailService = GetService<IPosBillDetailService>(); //账单明细
                    var posBillService = GetService<IPosBillService>(); //账单主表

                    //重算一遍
                    billDetailService.StatisticsBillDetail(CurrentInfo.HotelId, model.Billid, model.Billid);

                    //判断是否有点菜
                    var billDetailCount = billDetailService.GetBillDetailByDcFlag(CurrentInfo.HotelId, model.Billid).ToList().Count;
                    if (billDetailCount > 0)
                    {
                        //退出验证数据是否修改
                        var check = Common.CheckPosBillData(model.Billid);

                        if (check)
                        {
                            var posPosService = GetService<IPosPosService>();
                            var pospos = posPosService.Get(CurrentInfo.PosId);
                            if (pospos != null) //判断收银点类型
                            {
                                if (pospos.PosMode == "B" || pospos.PosMode == "C")
                                {

                                    return Json(JsonResultData.Successed("1"));
                                }
                            }

                            PosBill billModel = posBillService.Get(model.Billid);
                            //结账状态或者清机状态清除餐台记录表数据
                            //退出清除餐台锁台记录
                            //if (billModel != null && (billModel.Status == 2 || billModel.Status == 3))
                            //{
                            var service = GetService<IPosTabLogService>();
                            var tabLogList = service.GetPosTabLogListByTab(CurrentInfo.HotelId, model.Refeid, model.Tabid, model.TabNo);
                            if (tabLogList != null && tabLogList.Count > 0)
                            {
                                foreach (var tabLog in tabLogList)
                                {
                                    if (tabLog.Billid == billModel.Billid && tabLog.TransUser == CurrentInfo.UserName)
                                    {
                                        service.Delete(tabLog);
                                        service.AddDataChangeLog(OpLogType.Pos锁台删除);
                                        service.Commit();
                                    }
                                }
                            }
                            //}

                            return Json(JsonResultData.Successed(""));
                        }
                        else
                        {

                            PosBill billModel = posBillService.Get(model.Billid);
                            var service = GetService<IPosTabLogService>();
                            var tabLogList = service.GetPosTabLogListByTab(CurrentInfo.HotelId, model.Refeid, model.Tabid, model.TabNo);
                            if (tabLogList != null && tabLogList.Count > 0)
                            {
                                foreach (var tabLog in tabLogList)
                                {
                                    if (tabLog.Billid == billModel.Billid && tabLog.TransUser == CurrentInfo.UserName)
                                    {
                                        service.Delete(tabLog);
                                        service.AddDataChangeLog(OpLogType.Pos锁台删除);
                                        service.Commit();
                                    }
                                }
                            }
                            var returnJson = new
                            {
                                msg = "是否要落单？",
                                jsonOrder = orderList
                            };
                            return Json(JsonResultData.Failure(returnJson, 2));
                        }
                    }
                    else
                    {
                        posBillService.DeletePosBillByCCancelLockTab(CurrentInfo.HotelId, model.Billid, model.Tabid);
                        return Json(JsonResultData.Successed());
                    }


                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex.Message.ToString()));
                }
            }
            else
            {
                return Json(JsonResultData.Successed("账单信息未传入"));
            }

            // return Json(JsonResultData.Successed("清理锁台记录失败，请稍后重试！"));
        }


        /// <summary>
        /// 更新数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.AuthManage)]
        public ActionResult UpdateQuantity(PosBillDetail model, string Flag = "0")
        {
            try
            {
                // model.Quantity = model.Quantity == null ? 0 : model.Quantity;

                if (model.Quantity == null)
                {
                    return Json(JsonResultData.Successed());
                }

                var service = GetService<IPosBillDetailService>();
                var billDetail = service.Get(model.Id);

                var billService = GetService<IPosBillService>();
                var suitService = GetService<IPosItemSuitService>();

                var bill = billService.Get(billDetail.Billid);
                bill.ServiceRate = bill.ServiceRate >= 1 && bill.ServiceRate <= 100 ? bill.ServiceRate / 100 : bill.ServiceRate;

                //获取关联的作法信息
                var billDeetailActionService = GetService<IPosBillDetailActionService>();
                var billDeetailActionList = billDeetailActionService.GetPosBillDetailActionByModule(CurrentInfo.HotelId, bill.Billid).Where(m => m.Mid == billDetail.Id).ToList();

                if (billDetail != null)
                {
                    billDetail.Discount = billDetail.Discount >= 1 && billDetail.Discount <= 100 ? billDetail.Discount / 100 : billDetail.Discount;
                    var oldBillDetail = new PosBillDetail();
                    AutoSetValueHelper.SetValues(billDetail, oldBillDetail);


                    var pmsParaService = GetService<IPmsParaService>();
                    //是否连接云仓库
                    var PosIsConnectWareHouse = pmsParaService.GetValue(CurrentInfo.HotelId, "PosIsConnectWareHouse");

                    //连接云仓库库存控制
                    var PosConnectWareHouseValue = pmsParaService.GetValue(CurrentInfo.HotelId, "PosConnectWareHouseValue");
                    if (PosIsConnectWareHouse != "0")
                    {
                        PosCmpStock cmpStock = new PosCmpStock();
                        //修改仓库耗用表数据
                        var billCostService = GetService<IPosBillCostService>();
                        var billCostList = billCostService.GetBillCostList(CurrentInfo.HotelId, CurrentInfo.ModuleCode, billDetail.Id);

                        var costService = GetService<IPosCostItemService>();
                        if (billCostList != null && billCostList.Count > 0)
                        {
                            foreach (var billCost in billCostList)
                            {
                                if (Flag == "0")
                                {
                                    //调用接口查询数量
                                    var result = cmpStock.PostStockByItemCode(billCost.OutCodeNo, CurrentInfo.HotelId, Regex.Replace(billCost.WhCode, CurrentInfo.HotelId, ""));
                                    if (result != null)
                                    {
                                        if (result.ErrorNo == "0")
                                        {
                                    
                                            var posCost = costService.GetListPostCostByItemId(CurrentInfo.HotelId, billDetail.Itemid, billDetail.Unitid).Where(w => w.CostItemid == billCost.CostItemid).FirstOrDefault();
                                            //已消耗数量
                                            var sumCostQuantity = billCostService.GetBillCostSumQuantity(CurrentInfo.HotelId, CurrentInfo.ModuleCode, bill.BillBsnsDate, billCost.WhCode, billCost.CostItemid, billDetail.Id) ?? 0;
                                            var quantityVal = Convert.ToDecimal(result.Data.FirstOrDefault().StockQuantity) - sumCostQuantity < model.Quantity * posCost.OriQuan;
                                            if (quantityVal|| Convert.ToDecimal(result.Data.FirstOrDefault().StockQuantity) <= 0)
                                            {
                                                if (PosConnectWareHouseValue == "1")
                                                {
                                                    return Json(JsonResultData.Failure("库存不足,是否继续添加？", 1));
                                                }
                                                else if (PosConnectWareHouseValue == "3")
                                                {
                                                    return Json(JsonResultData.Failure("添加失败，库存不足", 3));
                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        return Json(JsonResultData.Failure("物品不存在"));
                                    }
                                }
                                
                                if (model.Quantity != 0)
                                {
                                    billCost.Quantity = billCost.OriQuan * model.Quantity;
                                    billCost.Amount = billCost.OriQuan * model.Quantity * billCost.Price;
                                    billCostService.Update(billCost, new PosBillCost());
                                }
                                else
                                {
                                    billCostService.Delete(billCost);
                                }
                                billService.Commit();
                            }
                        }
                    }

                    //获取当前操作的主套餐
                    var mainItemSuit = service.GetBillDetailByUpid(CurrentInfo.HotelId, billDetail.Billid, billDetail.Upid).Where(w => w.SD == false && w.SP == true).FirstOrDefault();

                    var suitList = new List<up_pos_list_itemSuitByItemIdResult>();
                    if (mainItemSuit == null)
                    {
                        suitList = suitService.GetPosItemSuitListByItemId(CurrentInfo.HotelId, billDetail.Itemid);
                    }
                    else
                    {
                        suitList = suitService.GetPosItemSuitListByItemId(CurrentInfo.HotelId, mainItemSuit.Itemid);
                    }

                    //var suitList = suitService.GetPosItemSuitListByItemId(CurrentInfo.HotelId, mainItemSuit.Itemid);
                    var itemList = service.GetBillDetailByUpid(CurrentInfo.HotelId, billDetail.Billid, billDetail.Upid).Where(w => w.SD == true).ToList();
                    if (model.Quantity != 0)
                    {
                        #region 修改套餐

                        billDetail.Quantity = model.Quantity;
                        billDetail.Dueamount = billDetail.Price * model.Quantity;



                        decimal? actionAmount = 0;//作法价格
                        var actionService = GetService<IPosActionService>();
                        foreach (var billDeetailAction in billDeetailActionList)
                        {

                            // var action = actionService.GetActionByCode(CurrentInfo.HotelId, billDeetailAction.ActionNo, billDeetailAction.ActionName);
                            if (billDeetailAction.AddPrice != null && billDeetailAction.AddPrice > 0)
                            {
                                if (billDeetailAction.IByQuan != null && billDeetailAction.IByQuan.Value && billDeetailAction.IByGuest != null && billDeetailAction.IByGuest.Value)
                                {
                                    actionAmount += Convert.ToDecimal(billDeetailAction.AddPrice) * Convert.ToDecimal(billDetail.Quantity) * Convert.ToDecimal(bill.IGuest);
                                }
                                else if (billDeetailAction.IByQuan != null && billDeetailAction.IByQuan.Value)
                                {
                                    actionAmount += Convert.ToDecimal(billDeetailAction.AddPrice) * billDetail.Quantity;
                                }
                                else if (billDeetailAction.IByGuest != null && billDeetailAction.IByGuest.Value)
                                {
                                    actionAmount += Convert.ToDecimal(billDeetailAction.AddPrice) * Convert.ToDecimal(bill.IGuest);
                                }
                                else
                                {
                                    actionAmount += Convert.ToDecimal(billDeetailAction.AddPrice);
                                }
                            }
                        }
                        billDetail.AddPrice = actionAmount;
                        billDetail.Dueamount += actionAmount;//把作法加价的金额重新计算

                        billDetail.Amount = (billDetail.Price * billDetail.Quantity - billDetail.DiscAmount) * billDetail.Discount;
                        billDetail.Service = billDetail.Price * billDetail.Quantity * bill.ServiceRate;

                        //会员价检测 主动IsFore = PosBillIsForce.会员价
                        if (bill.IsForce == (byte)PosBillIsForce.会员价)
                        {
                            billDetail.Amount = (billDetail.PriceClub * billDetail.Quantity - billDetail.DiscAmount) * billDetail.Discount;
                            billDetail.Service = billDetail.PriceClub * billDetail.Quantity * bill.ServiceRate;
                        }

                        service.Update(billDetail, oldBillDetail);
                        service.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + ",台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，数量：" + oldBillDetail.Quantity + "->" + billDetail.Quantity + "，金额：" + oldBillDetail.Dueamount + "->" + billDetail.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status), bill.BillNo);
                        #endregion 修改套餐

                        #region 修改套餐明细

                        //当前操作的是套餐明细。直接修改套餐明细数量
                        if (billDetail.SP == false && billDetail.SD == true)
                        {

                        }
                        else
                        {
                            foreach (var item in itemList)
                            {
                                var suit = suitList.Where(w => w.ItemId2 == item.Itemid).FirstOrDefault();

                                var oldSuitDetail = new PosBillDetail();
                                AutoSetValueHelper.SetValues(item, oldSuitDetail);

                                if (suit != null)
                                {
                                    item.Quantity = suit.Quantity * billDetail.Quantity;
                                    item.Dueamount = suit.Amount * item.Quantity;
                                }
                                else
                                {
                                    //直接修改自定义套餐明细数量
                                    item.Quantity = model.Quantity;
                                    item.Dueamount = item.Price * model.Quantity;
                                }
                                var suitDeetailActionList = billDeetailActionService.GetPosBillDetailActionByModule(CurrentInfo.HotelId, bill.Billid).Where(m => m.Mid == item.Id).ToList();
                                foreach (var suitDeetailAction in suitDeetailActionList)
                                {
                                    var action = actionService.GetActionByCode(CurrentInfo.HotelId, suitDeetailAction.ActionNo, suitDeetailAction.ActionName);
                                    if (action.AddPrice != null && action.AddPrice > 0)
                                    {
                                        if (action.IsByQuan != null && action.IsByQuan.Value && action.IsByGuest != null && action.IsByGuest.Value)
                                        {
                                            actionAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(item.Quantity) * Convert.ToDecimal(bill.IGuest);
                                        }
                                        else if (action.IsByQuan != null && action.IsByQuan.Value)
                                        {
                                            actionAmount += Convert.ToDecimal(action.AddPrice) * item.Quantity;
                                        }
                                        else if (action.IsByGuest != null && action.IsByGuest.Value)
                                        {
                                            actionAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(bill.IGuest);
                                        }
                                        else
                                        {
                                            actionAmount += Convert.ToDecimal(action.AddPrice);
                                        }
                                    }
                                }

                                billDetail.AddPrice = actionAmount;
                                billDetail.Dueamount += actionAmount;//把作法加价的金额重新计算
                                item.Discount = item.Discount >= 1 && item.Discount <= 100 ? item.Discount / 100 : item.Discount;
                                item.Amount = bill.IsService == true ? item.Price * item.Quantity * bill.ServiceRate : 0;

                                service.Update(item, oldSuitDetail);
                                service.Commit();
                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + item.Id + ",台号：" + billDetail.Tabid + "，名称：" + item.ItemName + "，数量：" + oldSuitDetail.Quantity + "->" + item.Quantity + "，金额：" + oldSuitDetail.Dueamount + "->" + billDetail.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldSuitDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), item.Status), bill.BillNo);


                            }
                        }



                        #endregion 修改套餐明细

                        service.Commit();
                        return Json(JsonResultData.Successed(""));
                    }
                    else
                    {

                        #region 删除套餐明细

                        if (billDetail.SP == true)
                        {
                            foreach (var item in itemList)
                            {
                                var suitDeetailActionList = billDeetailActionService.GetPosBillDetailActionByModule(CurrentInfo.HotelId, bill.Billid).Where(m => m.Mid == item.Id).ToList();

                                //删除套餐明细对应作法
                                foreach (var suitDeetailAction in suitDeetailActionList)
                                {
                                    AddOperationLog(OpLogType.Pos账单作法明细删除, "单号：" + suitDeetailAction.Id + ",名称：" + suitDeetailAction.ActionName + ",数量：" + suitDeetailAction.Quan.ToString() + ",金额：" + suitDeetailAction.Amount.ToString(), bill.BillNo);
                                    billDeetailActionService.Delete(suitDeetailAction);

                                }

                                billDeetailActionService.Commit();

                                //删除套餐明细
                                service.Delete(item);
                                AddOperationLog(OpLogType.Pos账单消费明细删除, "单号：" + item.Id + ",名称：" + item.ItemName + ",数量：" + item.Quantity.ToString() + ",金额：" + item.Dueamount.ToString(), bill.BillNo);
                                service.Commit();
                            }
                        }

                        #endregion 删除套餐明细

                        #region 删除套餐

                        //删除套餐对应作法
                        foreach (var billDeetailAction in billDeetailActionList)
                        {
                            billDeetailActionService.Delete(billDeetailAction);
                            AddOperationLog(OpLogType.Pos账单作法明细删除, "单号：" + billDeetailAction.Id + ",名称：" + billDeetailAction.ActionName + ",数量：" + billDeetailAction.Quan.ToString() + ",金额：" + billDeetailAction.Amount.ToString(), bill.BillNo);
                        }

                        //删除套餐
                        service.Delete(billDetail);
                        AddOperationLog(OpLogType.Pos账单消费明细删除, "单号：" + billDetail.Id + ",名称：" + billDetail.ItemName + ",数量：" + billDetail.Quantity.ToString() + ",金额：" + billDetail.Dueamount.ToString(), bill.BillNo);

                        #endregion 删除套餐

                        service.Commit();
                        return Json(JsonResultData.Successed(""));
                    }


                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
            return Json(JsonResultData.Failure("修改数量失败，请稍后重试！"));
        }

        /// <summary>
        /// 取消消费项目
        /// </summary>
        /// <param name="model"></param>
        /// <param name="istagtype"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult CancelItem(string id, string status, string canReason, string istagtype, string isreuse)
        {
            try
            {
                var service = GetService<IPosBillDetailService>();
                var suitService = GetService<IPosItemSuitService>();


                var billDetail = service.Get(Convert.ToInt64(id));

                if (billDetail != null)
                {
                    //赠送
                    if (Convert.ToByte(istagtype) == 1)
                    {
                        return CancelItemA(id, status, canReason, istagtype, isreuse);
                    }
                    else
                    {
                        return CancelItemB(id, status, canReason, istagtype, isreuse);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
            return Json(JsonResultData.Failure("消费项目取消失败，请稍后重试！"));
        }

        /// <summary>
        /// 赠送
        /// </summary>
        /// <param name="billDetail"></param>
        /// <param name="canReason"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.RecoveryOrderDetailZ)]
        public ActionResult CancelItemA(string id, string status, string canReason, string istagtype, string isreuse)
        {


            var service = GetService<IPosBillDetailService>();
            var suitService = GetService<IPosItemSuitService>();


            var billDetail = service.Get(Convert.ToInt64(id));

            var oldbillDetail = new PosBillDetail();
            AutoSetValueHelper.SetValues(billDetail, oldbillDetail);

            var itemList = service.GetBillDetailByUpid(CurrentInfo.HotelId, billDetail.Billid, billDetail.Upid).Where(w => w.SD == true).ToList();

            var billService = GetService<IPosBillService>();
            var bill = billService.Get(billDetail.Billid);

            if (billDetail.CanReason == canReason)//选择的原因是否是赠送
            {
                // 是赠送的并且原因不为空则取消赠送
                if (billDetail.Status == (byte)PosBillDetailStatus.赠送 && !string.IsNullOrWhiteSpace(canReason))
                {
                    if (billDetail.SP == true)
                    {
                        //取消赠送套餐明细
                        foreach (var item in itemList)
                        {
                            var oldItem = new PosBillDetail();
                            AutoSetValueHelper.SetValues(item, oldItem);
                            item.Status = (byte)PosBillDetailStatus.正常;
                            item.CanReason = string.Empty;
                            item.ModiUser = CurrentInfo.UserName;
                            item.ModiDate = DateTime.Now;
                            service.Update(item, new PosBillDetail());
                            service.AddDataChangeLog(OpLogType.Pos账单消费明细修改);

                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + item.Id + ",台号：" + item.Tabid + "，名称：" + item.ItemName + "，数量：" + oldItem.Quantity + "->" + item.Quantity + "，金额：" + oldItem.Dueamount + "->" + item.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldItem.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), item.Status) + "，原因：" + oldItem.CanReason + "-->" + item.CanReason, bill.BillNo);
                            service.Commit();
                        }
                    }

                    //取消赠送套餐
                    billDetail.Status = (byte)PosBillDetailStatus.正常;
                    billDetail.CanReason = string.Empty;
                    billDetail.ModiDate = DateTime.Now;
                    billDetail.ModiUser = CurrentInfo.UserName;
                    service.Update(billDetail, new PosBillDetail());

                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + ",台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，数量：" + oldbillDetail.Quantity + "->" + billDetail.Quantity + "，金额：" + oldbillDetail.Dueamount + "->" + billDetail.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldbillDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status) + "，原因：" + oldbillDetail.CanReason + "-->" + billDetail.CanReason, bill.BillNo);
                    service.Commit();

                    return Json(JsonResultData.Successed(""));
                }
            }
            if (billDetail.SP == true)
            {
                //赠送套餐明细
                foreach (var item in itemList)
                {
                    var oldSuitDetail = new PosBillDetail();
                    AutoSetValueHelper.SetValues(item, oldSuitDetail);

                    if (item != null)
                    {
                        item.Status = (byte)PosBillDetailStatus.赠送;
                        item.CanReason = canReason;
                        item.ModiUser = CurrentInfo.UserName;
                        item.ModiDate = DateTime.Now;

                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + item.Id + ",台号：" + item.Tabid + "，名称：" + item.ItemName + "，数量：" + oldSuitDetail.Quantity + "->" + item.Quantity + "，金额：" + oldSuitDetail.Dueamount + "->" + item.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldSuitDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), item.Status) + "，原因：" + oldSuitDetail.CanReason + "-->" + item.CanReason, bill.BillNo);

                        service.Update(item, new PosBillDetail());
                        service.Commit();
                    }
                }
            }

            billDetail.Status = (byte)PosBillDetailStatus.赠送;
            billDetail.CanReason = canReason;
            billDetail.ModiUser = CurrentInfo.UserName;
            billDetail.ModiDate = DateTime.Now;

            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + ",台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，数量：" + oldbillDetail.Quantity + "->" + billDetail.Quantity + "，金额：" + oldbillDetail.Dueamount + "->" + billDetail.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldbillDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status) + "，原因：" + oldbillDetail.CanReason + "-->" + billDetail.CanReason, bill.BillNo);
            service.Update(billDetail, new PosBillDetail());
            service.Commit();

            return Json(JsonResultData.Successed(""));


        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="billDetail"></param>
        /// <param name="canReason"></param>
        /// <param name="status"></param>
        /// <param name="isreuse"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Reset)]
        public ActionResult CancelItemB(string id, string status, string canReason, string istagtype, string isreuse)
        {
            var service = GetService<IPosBillDetailService>();
            var billDetail = service.Get(Convert.ToInt64(id));

            var oldbillDetail = new PosBillDetail();
            AutoSetValueHelper.SetValues(billDetail, oldbillDetail);

            var itemList = service.GetBillDetailByUpid(CurrentInfo.HotelId, billDetail.Billid, billDetail.Upid).Where(w => w.SD == true).ToList();

            if (Session["OldUserID"] != null && !string.IsNullOrEmpty(Session["OldUserID"].ToString()))
            {
                CurrentInfo.UserId = Session["OldUserID"].ToString();//刷卡前的用户id

                if (CurrentInfo.IsGroup)//集团的获取集团的用户id,用于刷卡后权限判断
                {
                    var userId = GetService<IPmsUserService>().GetUserIDByCode(CurrentInfo.GroupHotelId, CurrentInfo.UserCode).Id.ToString();
                    CurrentInfo.UserId = userId;
                }

                CurrentInfo.SaveValues();
                CurrentInfo.LoadValues();

                Session["OldUserID"] = null;
            }

            var billService = GetService<IPosBillService>();
            var bill = billService.Get(billDetail.Billid);
            //保存，赠送状态并且没有落单的项目直接删掉
            if (Convert.ToByte(status) == (byte)PosBillDetailStatus.保存)
            {

                if (billDetail.SP == true)
                {
                    //删除套餐明细
                    foreach (var item in itemList)
                    {
                        service.Delete(item);
                        AddOperationLog(OpLogType.Pos账单消费明细删除, "单号：" + item.Id + ",名称：" + item.ItemName + ",数量：" + item.Quantity.ToString() + ",金额：" + item.Dueamount.ToString(), bill.BillNo);
                        service.Commit();
                    }
                }
                //删除仓库耗用表数据
                var billCostService = GetService<IPosBillCostService>();
                var billCostList = billCostService.GetBillCostList(CurrentInfo.HotelId, CurrentInfo.ModuleCode, billDetail.Id);
                if (billCostList != null && billCostList.Count > 0)
                {
                    foreach (var billCost in billCostList)
                    {
                        billCostService.Delete(billCost);
                        billCostService.AddDataChangeLog(OpLogType.Pos消费项目仓库耗用表删除);
                        billCostService.Commit();
                    }
                }

                //删除套餐
                service.Delete(billDetail);
                AddOperationLog(OpLogType.Pos账单消费明细删除, "单号：" + billDetail.Id + ",名称：" + billDetail.ItemName + ",数量：" + billDetail.Quantity.ToString() + ",金额：" + billDetail.Dueamount.ToString(), bill.BillNo);
                service.Commit();



                return Json(JsonResultData.Successed(""));
            }
            else if (Convert.ToBoolean(isreuse))
            {
                if (billDetail.SP == true)
                {
                    //取消套餐明细
                    foreach (var item in itemList)
                    {
                        var oldSuitDetail = new PosBillDetail();
                        AutoSetValueHelper.SetValues(item, oldSuitDetail);
                        item.Status = (byte)PosBillDetailStatus.加回库存取消;
                        item.CanReason = canReason;
                        item.ModiUser = CurrentInfo.UserName;
                        item.ModiDate = DateTime.Now;

                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + item.Id + ",台号：" + item.Tabid + "，名称：" + item.ItemName + "，数量：" + oldSuitDetail.Quantity + "->" + item.Quantity + "，金额：" + oldSuitDetail.Dueamount + "->" + item.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldSuitDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), item.Status) + "，原因：" + oldSuitDetail.CanReason + "-->" + item.CanReason, bill.BillNo);
                        service.Update(item, oldSuitDetail);
                        service.Commit();
                    }
                }

                billDetail.Status = (byte)PosBillDetailStatus.加回库存取消;
            }
            else if (Convert.ToBoolean(isreuse) == false)
            {
                if (billDetail.SP == true)
                {
                    //取消套餐明细
                    foreach (var item in itemList)
                    {
                        var oldSuitDetail = new PosBillDetail();
                        AutoSetValueHelper.SetValues(item, oldSuitDetail);

                        item.Status = (byte)PosBillDetailStatus.不加回库取消;
                        item.CanReason = canReason;
                        item.ModiUser = CurrentInfo.UserName;
                        item.ModiDate = DateTime.Now;
                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + item.Id + ",台号：" + item.Tabid + "，名称：" + item.ItemName + "，数量：" + oldSuitDetail.Quantity + "->" + item.Quantity + "，金额：" + oldSuitDetail.Dueamount + "->" + item.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldSuitDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), item.Status) + "，原因：" + oldSuitDetail.CanReason + "-->" + item.CanReason, bill.BillNo);
                        service.Update(item, new PosBillDetail());
                        service.Commit();
                    }
                }

                billDetail.Status = (byte)PosBillDetailStatus.不加回库取消;
            }

            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + ",台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，数量：" + oldbillDetail.Quantity + "->" + billDetail.Quantity + "，金额：" + oldbillDetail.Dueamount + "->" + billDetail.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldbillDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status) + "，原因：" + oldbillDetail.CanReason + "-->" + billDetail.CanReason, bill.BillNo);
            billDetail.ModiDate = DateTime.Now;
            billDetail.ModiUser = CurrentInfo.UserName;
            service.Update(billDetail, new PosBillDetail());
            service.Commit();

            return Json(JsonResultData.Successed(""));
        }


        /// <summary>
        /// 获取账单明细
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult GetBillDetail(string billDetailId)
        {
            var service = GetService<IPosBillDetailService>();
            var billDetail = service.Get(int.Parse(billDetailId));
            if (billDetail != null)
            {
                return Json(JsonResultData.Successed(billDetail));

            }
            else
            {
                return Json(JsonResultData.Failure(""));
            }

        }

        /// <summary>
        /// 更新要求
        /// </summary>
        /// <param name="model"></param>
        /// <param name="requestid"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Enable)]
        public ActionResult UpdateRequest(PosBillDetail model, string requestid)
        {
            try
            {
                var billService = GetService<IPosBillService>();//账单服务
                var bill = new PosBill();
                var service = GetService<IPosBillDetailService>();
                var requestService = GetService<IPosRequestService>();
                var request = requestService.Get(requestid);
                if (request.ITagOperator == (byte)PosRequestOperator.全单)
                {
                    var billDetails = service.GetBillDetailByDcFlag(CurrentInfo.HotelId, model.Billid, PosItemDcFlag.D.ToString());
                    if (billDetails != null && billDetails.Count > 0)
                    {
                        foreach (var billDetail in billDetails)
                        {
                            bill = billService.Get(billDetail.Billid);//获取账单信息
                            var oldBillDetail = new PosBillDetail();
                            AutoSetValueHelper.SetValues(billDetail, oldBillDetail);

                            billDetail.Request = model.Request;


                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + ",台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，金额：" + oldBillDetail.Dueamount + "->" + billDetail.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status) + "，要求：" + oldBillDetail.Request + "-->" + billDetail.Request, bill.BillNo);

                            service.Update(billDetail, oldBillDetail);
                            service.Commit();
                        }
                    }
                }
                if (request.ITagOperator == (byte)PosRequestOperator.单道)
                {
                    var billDetail = service.Get(model.Id);

                    bill = billService.Get(billDetail.Billid);//获取账单信息

                    var oldBillDetail = new PosBillDetail();
                    AutoSetValueHelper.SetValues(billDetail, oldBillDetail);

                    billDetail.Request = model.Request;
                    //billDetail.IsProduce = Convert.ToByte(request.IsProduce);

                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + ",台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，金额：" + oldBillDetail.Dueamount + "->" + billDetail.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status) + "，要求：" + oldBillDetail.Request + "-->" + billDetail.Request, bill.BillNo);

                    service.Update(billDetail, oldBillDetail);
                    service.Commit();
                }
                if (request.ITagOperator == (byte)PosRequestOperator.本单)
                {
                    var billDetails = service.GetBillDetailByDcFlag(CurrentInfo.HotelId, model.Billid, PosItemDcFlag.D.ToString()).Where(w => w.Status == (byte)PosBillDetailStatus.保存).ToList();
                    if (billDetails != null && billDetails.Count > 0)
                    {
                        foreach (var billDetail in billDetails)
                        {
                            bill = billService.Get(billDetail.Billid);//获取账单信息

                            var oldBillDetail = new PosBillDetail();
                            AutoSetValueHelper.SetValues(billDetail, oldBillDetail);

                            billDetail.Request = model.Request;
                            //billDetail.IsProduce = Convert.ToByte(request.IsProduce);

                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + ",台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，金额：" + oldBillDetail.Dueamount + "->" + billDetail.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status) + "，要求：" + oldBillDetail.Request + "-->" + billDetail.Request, bill.BillNo);

                            service.Update(billDetail, new PosBillDetail());
                            service.Commit();
                        }
                    }
                }
                return Json(JsonResultData.Successed(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 更新作法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult UpdateBillDetailAction(PosBillDetailActionAddViewModel model)
        {
            try
            {
                var billDetailService = GetService<IPosBillDetailService>();    //账单明细
                var actionService = GetService<IPosActionService>();
                var service = GetService<IPosBillDetailActionService>();    //作法

                var billService = GetService<IPosBillService>();//账单

                var deleteFlag = false;
                var bill = new PosBill();
                #region 手写作法删除

                if (model.ActionType == "-1")   //手写作法
                {
                    var IdList = model.ActionId.Split(',');
                    foreach (var id in IdList)
                    {
                        var BillDetailAction = service.Get(int.Parse(id));
                        if (BillDetailAction != null)
                        {
                            bill = billService.Get(BillDetailAction.MBillid);
                            AddOperationLog(OpLogType.Pos账单作法明细删除, "单号：" + id + ",名称：" + BillDetailAction.ActionName + "，金额：" + BillDetailAction.Amount, bill.BillNo);
                            //  deleteFlag = true;
                            service.Delete(BillDetailAction);
                            service.Commit();


                        }
                    }
                    var billDetailModel = billDetailService.Get(model.Mid);   //账单
                    string HandactionString = "";
                    decimal handAddprice = 0;
                    var BillDetailList = service.GetBillDetailActionByMid(CurrentInfo.HotelId, model.MBillid, model.Mid);

                    if (BillDetailList != null && BillDetailList.Count > 0)
                    {
                        int? igroupid = null;
                        foreach (var temp in BillDetailList)
                        {
                            handAddprice += temp.Amount ?? 0;
                            if (igroupid == null || temp.Igroupid == igroupid)
                            {
                                HandactionString += "," + temp.ActionName;
                            }
                            else
                            {
                                HandactionString += "/" + temp.ActionName;
                            }
                            igroupid = temp.Igroupid;
                        }
                    }
                    var newbillDetailModel = new PosBillDetail();

                    //获取主账单数据
                    bill = billService.Get(billDetailModel.Billid);
                    AutoSetValueHelper.SetValues(billDetailModel, newbillDetailModel);
                    newbillDetailModel.Action = HandactionString.Trim(',');
                    newbillDetailModel.AddPrice = handAddprice;
                    newbillDetailModel.Dueamount = newbillDetailModel.Price * newbillDetailModel.Quantity + handAddprice;

                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newbillDetailModel.Id + ",名称：" + newbillDetailModel.ItemName + "，作法：" + billDetailModel.Action + "->" + newbillDetailModel.Action, bill.BillNo);
                    //修改账单明细
                    billDetailService.Update(newbillDetailModel, billDetailModel);
                    //billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细修改);

                    billDetailService.Commit();
                    return Json(JsonResultData.Successed(model.Igroupid));
                }

                #endregion 手写作法删除

                var action = actionService.Get(model.ActionId);
                var detailAction = service.GetBillDetailActionByMid(CurrentInfo.HotelId, model.MBillid, model.Mid, action.Code, "1");
                if (detailAction != null && detailAction.Id > 0)
                {
                    bill = billService.Get(detailAction.MBillid);
                    AddOperationLog(OpLogType.Pos账单作法明细删除, "单号：" + detailAction.Id + ",名称：" + detailAction.ActionName + "，金额：" + detailAction.Amount, bill.BillNo);

                    deleteFlag = true;
                    service.Delete(detailAction);
                    // service.AddDataChangeLog(OpLogType.Pos账单作法明细删除);
                    service.Commit();
                }
                else
                {
                    if (model.Igroupid == null)
                    {
                        model.Igroupid = service.GetIgroupidByMid(CurrentInfo.HotelId, model.MBillid, model.Mid);
                    }

                    decimal amount = 0;
                    if (action.AddPrice != null && action.AddPrice > 0)
                    {
                        if (action.IsByQuan != null && action.IsByQuan.Value && action.IsByGuest != null && action.IsByGuest.Value)
                        {
                            amount = Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(model.Quan) * Convert.ToDecimal(model.IGuest);
                        }
                        else if (action.IsByQuan != null && action.IsByQuan.Value)
                        {
                            amount = Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(model.Quan);
                        }
                        else if (action.IsByGuest != null && action.IsByGuest.Value)
                        {
                            amount = Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(model.IGuest);
                        }
                        else
                        {
                            amount = Convert.ToDecimal(action.AddPrice);
                        }
                    }

                    PosBillDetailAction billDetailAction = new PosBillDetailAction()
                    {
                        Hid = CurrentInfo.HotelId,
                        ActionNo = action.Code,
                        ActionName = action.Cname,
                        AddPrice = action.AddPrice,
                        Nmultiple = action.Multiple,
                        IByQuan = action.IsByQuan,
                        IByGuest = action.IsByGuest,
                        Amount = amount,
                        ModiUser = CurrentInfo.UserName,
                        ModiDate = DateTime.Now,
                        ActionType = "1"        //代表是通用作法
                    };
                    bill = billService.Get(model.MBillid);
                    model.AddPrice = action.AddPrice;
                    model.IByGuest = action.IsByGuest;
                    model.IByQuan = action.IsByQuan;

                    AutoSetValueHelper.SetValues(model, billDetailAction);
                    billDetailAction.ActionType = "1";
                    service.Add(billDetailAction);
                    //  service.AddDataChangeLog(OpLogType.Pos账单作法明细增加);
                    service.Commit();

                    bill = billService.Get(billDetailAction.MBillid);

                    AddOperationLog(OpLogType.Pos账单作法明细增加, "单号：" + billDetailAction.Id + ",名称：" + billDetailAction.ActionName + "，金额：" + billDetailAction.Amount, bill.BillNo);
                }

                var billDetail = billDetailService.Get(model.Mid);

                decimal addprice = 0;
                string actionString = "";
                var list = service.GetBillDetailActionByMid(CurrentInfo.HotelId, model.MBillid, model.Mid);
                if (list != null && list.Count > 0)
                {
                    int? igroupid = null;
                    foreach (var temp in list)
                    {
                        addprice += temp.Amount ?? 0;
                        if (igroupid == null || temp.Igroupid == igroupid)
                        {
                            actionString += "," + temp.ActionName;
                        }
                        else
                        {
                            actionString += "/" + temp.ActionName;
                        }
                        igroupid = temp.Igroupid;
                    }
                }

                var oldBillDetail = new PosBillDetail();
                AutoSetValueHelper.SetValues(billDetail, oldBillDetail);

                billDetail.AddPrice = addprice;
                billDetail.Action = actionString.Trim(',');
                if (deleteFlag)
                {
                    billDetail.Dueamount = billDetail.Price * billDetail.Quantity + addprice;
                    billDetail.Amount = billDetail.Price * billDetail.Quantity * billDetail.Discount;
                }
                else
                {
                    billDetail.Dueamount = billDetail.Price * billDetail.Quantity + addprice;
                    billDetail.Amount = billDetail.Price * billDetail.Quantity * billDetail.Discount;
                }
                bill = billService.Get(billDetail.Billid);

                //  AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，作法：" + oldBillDetail.Action + " -> " + billDetail.Action + "，作法加价：" + oldBillDetail.AddPrice + " -> " + billDetail.AddPrice + "，折前金额：" + oldBillDetail.Dueamount + " -> " + billDetail.Dueamount + "，折后金额：" + oldBillDetail.Amount + " -> " + billDetail.Amount, billDetail.Billid);
                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + ",台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，数量：" + oldBillDetail.Quantity + "->" + billDetail.Quantity + "，作法加价：" + oldBillDetail.AddPrice + " -> " + billDetail.AddPrice + "，金额：" + oldBillDetail.Dueamount + "->" + billDetail.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status), bill.BillNo);


                billDetailService.Update(billDetail, oldBillDetail);
                billDetailService.Commit();

                //  billDetailService.StatisticsBillDetail(CurrentInfo.HotelId, billDetail.Billid, billDetail.MBillid);
                return Json(JsonResultData.Successed(model.Igroupid));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 添加菜式做法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult UpdateBillActionForItem(PosBillDetailActionAddViewModel model)
        {
            var itemActionService = GetService<IPosItemActionService>();
            var itemActionModel = itemActionService.Get(new Guid(model.ActionId));    //取菜式作法对应参数添加做法

            var actionService = GetService<IPosActionService>();
            var action = actionService.Get(itemActionModel.Actionid);   //基础作法

            var service = GetService<IPosBillDetailActionService>();

            var billService = GetService<IPosBillService>();

            var bill = new PosBill();

            var deleteFlag = false;
            var detailAction = service.GetBillDetailActionByMid(CurrentInfo.HotelId, model.MBillid, model.Mid, action.Code, "2");
            if (detailAction != null && detailAction.Id > 0)
            {
                bill = billService.Get(detailAction.MBillid);
                AddOperationLog(OpLogType.Pos账单作法明细删除, "单号：" + detailAction.Id + ",名称：" + detailAction.ActionName + "，金额：" + detailAction.Amount, bill.BillNo);

                deleteFlag = true;
                service.Delete(detailAction);
                // service.AddDataChangeLog(OpLogType.Pos账单作法明细删除);
                service.Commit();
            }
            else
            {
                //添加做法
                if (model.Igroupid == null)
                {
                    model.Igroupid = service.GetIgroupidByMid(CurrentInfo.HotelId, model.MBillid, model.Mid);
                }

                decimal amount = 0;
                if (itemActionModel.AddPrice != null && itemActionModel.AddPrice > 0)
                {
                    if (itemActionModel.IsByQuan != null && itemActionModel.IsByQuan.Value && itemActionModel.IsByGuest != null && itemActionModel.IsByGuest.Value)
                    {
                        amount = Convert.ToDecimal(itemActionModel.AddPrice) * Convert.ToDecimal(model.Quan) * Convert.ToDecimal(model.IGuest);
                    }
                    else if (itemActionModel.IsByQuan != null && itemActionModel.IsByQuan.Value)
                    {
                        amount = Convert.ToDecimal(itemActionModel.AddPrice) * Convert.ToDecimal(model.Quan);
                    }
                    else if (itemActionModel.IsByGuest != null && itemActionModel.IsByGuest.Value)
                    {
                        amount = Convert.ToDecimal(itemActionModel.AddPrice) * Convert.ToDecimal(model.IGuest);
                    }
                    else
                    {
                        amount = Convert.ToDecimal(itemActionModel.AddPrice);
                    }
                }
                PosBillDetailAction billDetailAction = new PosBillDetailAction()
                {
                    Hid = CurrentInfo.HotelId,
                    ActionNo = action.Code,
                    ActionName = action.Cname,
                    AddPrice = itemActionModel.AddPrice,
                    Nmultiple = itemActionModel.Multiple,
                    IByQuan = itemActionModel.IsByQuan,
                    IByGuest = itemActionModel.IsByGuest,
                    Amount = amount,
                    ModiUser = CurrentInfo.UserName,
                    ModiDate = DateTime.Now,
                    ActionType = "2"        //菜式做法
                };
                bill = billService.Get(model.MBillid);
                model.AddPrice = itemActionModel.AddPrice;
                model.IByGuest = itemActionModel.IsByGuest;
                model.IByQuan = itemActionModel.IsByQuan;

                AutoSetValueHelper.SetValues(model, billDetailAction);

                billDetailAction.ActionType = "2";
                service.Add(billDetailAction);
                //  service.AddDataChangeLog(OpLogType.Pos账单作法明细增加);
                service.Commit();

                bill = billService.Get(billDetailAction.MBillid);

                AddOperationLog(OpLogType.Pos账单作法明细增加, "单号：" + billDetailAction.Id + ",名称：" + billDetailAction.ActionName + "，金额：" + billDetailAction.Amount, bill.BillNo);
            }
            var billDetailService = GetService<IPosBillDetailService>();
            var billDetail = billDetailService.Get(model.Mid);

            decimal addprice = 0;
            string actionString = "";
            var list = service.GetBillDetailActionByMid(CurrentInfo.HotelId, model.MBillid, model.Mid);
            if (list != null && list.Count > 0)
            {
                int? igroupid = null;
                foreach (var temp in list)
                {
                    addprice += temp.Amount ?? 0;
                    if (igroupid == null || temp.Igroupid == igroupid)
                    {
                        actionString += "," + temp.ActionName;
                    }
                    else
                    {
                        actionString += "/" + temp.ActionName;
                    }
                    igroupid = temp.Igroupid;
                }
            }

            var oldBillDetail = new PosBillDetail();
            AutoSetValueHelper.SetValues(billDetail, oldBillDetail);

            billDetail.AddPrice = addprice;
            billDetail.Action = actionString.Trim(',');
            if (deleteFlag)
            {
                billDetail.Dueamount = billDetail.Price * billDetail.Quantity + addprice;
                billDetail.Amount = billDetail.Price * billDetail.Quantity * billDetail.Discount;
            }
            else
            {
                billDetail.Dueamount = billDetail.Price * billDetail.Quantity + addprice;
                billDetail.Amount = billDetail.Price * billDetail.Quantity * billDetail.Discount;
            }
            bill = billService.Get(billDetail.Billid);

            //  AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，作法：" + oldBillDetail.Action + " -> " + billDetail.Action + "，作法加价：" + oldBillDetail.AddPrice + " -> " + billDetail.AddPrice + "，折前金额：" + oldBillDetail.Dueamount + " -> " + billDetail.Dueamount + "，折后金额：" + oldBillDetail.Amount + " -> " + billDetail.Amount, billDetail.Billid);
            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + ",台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，数量：" + oldBillDetail.Quantity + "->" + billDetail.Quantity + "，作法加价：" + oldBillDetail.AddPrice + " -> " + billDetail.AddPrice + "，金额：" + oldBillDetail.Dueamount + "->" + billDetail.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status), bill.BillNo);


            billDetailService.Update(billDetail, oldBillDetail);
            billDetailService.Commit();

            //  billDetailService.StatisticsBillDetail(CurrentInfo.HotelId, billDetail.Billid, billDetail.MBillid);
            return Json(JsonResultData.Successed(model.Igroupid));


            //  return Json(JsonResultData.Successed(model));
        }

        /// <summary>
        /// 获取作法所在页码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult GetActionPageIndex(PosBillDetailActionViewModel model)
        {
            try
            {
                int pageIndex = 0;
                var server = GetService<IPosActionService>();
                var detailActionService = GetService<IPosBillDetailActionService>();

                var igroupid = 0;
                if (string.IsNullOrWhiteSpace(model.ActionId))
                {
                    var list = detailActionService.GetBillDetailActionByMid(CurrentInfo.HotelId, model.MBillid, model.Mid);
                    var action = new PosAction();
                    if (list != null && list.Count > 0)
                    {
                        igroupid = Convert.ToInt32(list[0].Igroupid);
                        //var action = server.GetActionByCode(CurrentInfo.HotelId, list[0].ActionNo, list[0].ActionName);
                        for (int i = 0; i < list.Count; i++)
                        {
                            action = server.GetActionByCode(CurrentInfo.HotelId, list[i].ActionNo, list[i].ActionName);
                            if (action != null)
                            {
                                break;
                            }
                        }
                        pageIndex = action == null ? 0 : server.GetActionPageIndex(CurrentInfo.HotelId, CurrentInfo.ModuleCode, action.ActionTypeID, action.Id, model.PageSize);
                        model.ActionTypeId = action == null ? "0" : action.ActionTypeID;
                        model.igroupid = igroupid;
                    }
                }
                else
                {
                    var idList = model.ActionId.Split(',');
                    foreach (var id in idList)
                    {
                        var detailActionModel = detailActionService.Get(int.Parse(id));
                        igroupid = Convert.ToInt32(detailActionModel.Igroupid);
                        model.igroupid = igroupid;
                        if (detailActionModel.ActionNo == "Hand")
                        {
                            continue;
                        }
                        var action = server.GetActionByCode(CurrentInfo.HotelId, detailActionModel.ActionNo, detailActionModel.ActionName);
                        pageIndex = server.GetActionPageIndex(CurrentInfo.HotelId, CurrentInfo.ModuleCode, action.ActionTypeID, action.Id, model.PageSize);
                        model.ActionTypeId = action.ActionTypeID;

                        if (action != null)
                        {
                            break;
                        }
                    }
                    // detailActionService.Get()
                    //    string id = model.ActionId.Split(',')[0];
                    //var action = server.Get(id);
                    //pageIndex = server.GetActionPageIndex(CurrentInfo.HotelId, CurrentInfo.ModuleCode, action.ActionTypeID, action.Id, model.PageSize);
                    //model.ActionTypeId = action.ActionTypeID;
                }

                model.PageIndex = pageIndex;
                return Json(JsonResultData.Successed(model));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 落单以及先落

        /// <summary>
        /// 落单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.ReplaceSalesman)]
        public ActionResult beAlone(string billid, string ids)
        {
            var refe = Session["PosRefe"] as PosRefe;
            var controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            PosCommon common = new PosCommon();
            return common.Cmpbelone(billid, ids, refe, controller);
        }

        /// <summary>
        /// 先落
        /// </summary>
        /// <param name="billid"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.RecoveryOrderDetailY)]
        public ActionResult beAloneB(string billid, string ids)
        {
            var refe = Session["PosRefe"] as PosRefe;
            var controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            PosCommon common = new PosCommon();
            return common.Cmpbelone(billid, ids, refe, controller);

        }

        #endregion

        #region 
        /// <summary>
        /// 获取消费列表统计数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult GetStatistics(string billid, string mBillid, int isave)
        {
            try
            {
                var service = GetService<IPosBillDetailService>();
                var statistics = service.GetBillDetailStatistics(CurrentInfo.HotelId, billid, mBillid, isave);
                return Json(JsonResultData.Successed(statistics));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 获取外卖台账单
        /// </summary>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult GetTakeoutBill(PosBillAddViewModel model)
        {
            try
            {
                if (model != null)
                {
                    var refe = Session["PosRefe"] as PosRefe;

                    var posService = GetService<IPosPosService>();
                    var pos = posService.Get(CurrentInfo.PosId);

                    var service = GetService<IPosBillService>();
                    var tabid = service.GetTakeoutForTabid(CurrentInfo.HotelId, model.Refeid, pos.Business, (byte)PosBillTabFlag.快餐台);
                    var posBill = service.GetLastBillId(CurrentInfo.HotelId, refe.Id, pos.Business);

                    bool isexsit = service.IsExists(CurrentInfo.HotelId, posBill.Billid, posBill.BillNo);
                    if (isexsit)
                    {
                        posBill = service.GetLastBillId(CurrentInfo.HotelId, refe.Id, pos.Business);
                    }

                    PosBill bill = new PosBill()
                    {
                        Hid = CurrentInfo.HotelId,
                        Billid = posBill.Billid,
                        BillNo = posBill.BillNo,
                        MBillid = posBill.Billid,
                        InputUser = CurrentInfo.UserName,

                        BillDate = DateTime.Now,
                        IsService = true,
                        IsLimit = true,
                        Status = (byte)PosBillStatus.开台,
                        TabFlag = (byte)PosBillTabFlag.快餐台,
                        Shiftid = pos.ShiftId,
                        Shuffleid = refe.ShuffleId,
                    };

                    model.IGuest = string.IsNullOrWhiteSpace(refe.OpenInfo) ? 1 : (refe.OpenInfo.IndexOf("I") > -1 ? model.IGuest : 1);
                    model.Tabid = CurrentInfo.HotelId + tabid;
                    model.TabNo = string.IsNullOrWhiteSpace(refe.OpenInfo) ? "" : (refe.OpenInfo.IndexOf("J") > -1 ? "" : tabid);
                    model.BillBsnsDate = pos.Business.Value.Date;

                    AutoSetValueHelper.SetValues(model, bill);
                    service.Add(bill);
                    service.Commit();
                    AddOperationLog(OpLogType.Pos账单增加, "营业点：" + refe.Cname + ",营业日：" + pos.Business + " ,餐台号：" + tabid, bill.BillNo);

                    return Json(JsonResultData.Successed(bill));
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
            return Json(JsonResultData.Failure(""));
        }

        /// <summary>
        /// 外卖台开台
        /// </summary>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult AddTakeoutBill(PosTakeoutBillAddViewModel model)
        {
            try
            {
                if (model != null)
                {
                    var service = GetService<IPosBillService>();
                    var bill = service.Get(model.Billid);
                    AutoSetValueHelper.SetValues(model, bill);
                    service.Update(bill, new PosBill());
                    //  service.AddDataChangeLog(OpLogType.Pos账单修改);
                    service.Commit();

                    AddOperationLog(OpLogType.Pos账单修改, "", bill.BillNo);


                    return Json(JsonResultData.Successed(bill));
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
            return Json(JsonResultData.Failure(""));
        }

        /// <summary>
        /// 根据模型返回餐台状态视图
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _AddOpenTab(OpenTabAddViewModel model)
        {
            var tabService = GetService<IPosTabService>();
            var tab = tabService.Get(model.Tabid);

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(tab.Refeid);

            model.OpenInfo = refe.OpenInfo;

            return PartialView("_AddOpenTab", model);
        }

        /// <summary>
        /// 编辑开台信息视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.CheckIn)]
        public ActionResult _EditOpenTab(OpenTabEditViewModel model)
        {
            var service = GetService<IPosBillService>();
            var entity = service.Get(model.Billid);

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.Get(entity.Refeid);

            if (refe == null || string.IsNullOrEmpty(refe.OpenInfo))
            {
                return Json(JsonResultData.Failure("当前营业点没有设置开台属性！"));
            }
            model.OpenInfo = refe.OpenInfo;
            var serializer = new JavaScriptSerializer();
            AutoSetValueHelper.SetValues(entity, model);
            model.OriginJsonData = ReplaceJsonDateToDateString(serializer.Serialize(entity));
            return PartialView("_EditOpenTab", model);
        }

        /// <summary>
        /// 根据营业点获取项目大类列表
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _ItemClassList(PosItemClassViewModel model)
        {
            //if (model != null && !string.IsNullOrWhiteSpace(model.Refeid))
            //{
            var itemService = GetService<IPosItemService>();
            //var isItemSuit = itemService.isItemSuit(CurrentInfo.HotelId, model.Refeid);
            model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
            model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;// - (isItemSuit ? 1 : 0);


            var service = GetService<IPosItemClassService>();
            // var list = service.GetPosItemClassByRefe(CurrentInfo.HotelId, model.Refeid ?? "", model.PageIndex, model.PageSize);
            var list = service.GetPosItemClassByRefe(CurrentInfo.HotelId, model.Refeid ?? "", model.PageIndex, model.PageSize, CurrentInfo.PosId);

            //if (isItemSuit)
            //{
            //    up_pos_list_ItemClassBySingleResult itemClass = new up_pos_list_ItemClassBySingleResult();
            //    itemClass.Id = "suite";
            //    itemClass.Cname = "套餐";
            //    list.Add(itemClass);
            //}

            return PartialView("_ItemClassList", list);
            // }
            //  return PartialView("_ItemClassList");
        }

        /// <summary>
        /// 根据消费大类获取消费项目列表
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _ItemList(PosItemViewModel model)
        {
            var itemService = GetService<IPosItemService>();
            if (model != null && !string.IsNullOrWhiteSpace(model.Refeid))
            {
                model.Itemid = model.Itemid ?? "";
                PosRefe refe = Session["PosRefe"] as PosRefe;
                var list = itemService.GetPosItemByItemClassid(CurrentInfo.HotelId, model.Refeid, model.Itemid, refe == null ? "" : refe.ShuffleId, model.PageIndex, model.PageSize, model.Keyword ?? "");

                return PartialView("_ItemList", list);
            }
            else
            {
                //沽清用到
                //PosRefe refe = Session["PosRefe"] as PosRefe;
                var list = itemService.GetPosItemByItemClassid(CurrentInfo.HotelId, model.Refeid ?? "", model.Itemid, "", model.PageIndex, model.PageSize, model.Keyword ?? "");

                return PartialView("_ItemList", list);
            }
        }

        /// <summary>
        /// 根据消费大类获取消费项目列表
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _SuitItemList(PosItemViewModel model)
        {
            var itemService = GetService<IPosItemService>();
            var list = itemService.GetSuitItemByRefeid(CurrentInfo.HotelId, model.Refeid ?? "", model.PageIndex, model.PageSize, model.Keyword ?? "");
            return PartialView("_SuitItemList", list);
        }

        /// <summary>
        /// 账单信息视图，入单页左侧头部使用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _BillInfo(InSingleViewModel model)
        {
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);
            if (model != null && !string.IsNullOrWhiteSpace(model.Tabid) && !string.IsNullOrWhiteSpace(model.Refeid))
            {
                var billService = GetService<IPosBillService>();
                var bill = billService.GetPosBillByTabId(CurrentInfo.HotelId, model.Refeid, model.Tabid);

                if (bill == null)
                {
                    bill = new up_pos_list_billByRefeidAndTabidResult();

                    var refeService = GetService<IPosRefeService>();
                    var shuffleService = GetService<IPosShuffleService>();
                    var shiftService = GetService<IPosShiftService>();


                    var refe = refeService.GetEntity(CurrentInfo.HotelId, model.Refeid);
                    var shuffle = shuffleService.GetEntity(CurrentInfo.HotelId, refe.ShuffleId);
                    var shift = shiftService.GetEntity(CurrentInfo.HotelId, CurrentInfo.PosId);


                    bill.Refeid = refe.Id;
                    bill.RefeName = refe.Cname;
                    bill.Shuffleid = refe.ShuffleId;
                    bill.BillBsnsDate = pos.Business;
                    bill.ShuffleName = shuffle.Cname;
                    bill.Shiftid = shift.Id;
                    bill.ShiftName = shift.Name;



                }
                else
                {
                    bill.ServiceRate = bill.ServiceRate >= 1 && bill.ServiceRate <= 100 ? bill.ServiceRate / 100 : bill.ServiceRate;
                    bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount;
                }

                return PartialView("_BillInfo", bill);
            }
            return PartialView("_BillInfo", new up_pos_list_billByRefeidAndTabidResult());
        }
        #endregion

        #region 取消，赠送原因视图

        /// <summary>
        /// 取消原因视图
        /// </summary>
        [AuthButton(AuthFlag.Reset)]
        public PartialViewResult _ReasonList(PosReasonViewModel model)
        {
            if (model != null)
            {
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

                var service = GetService<IPosReasonService>();
                var list = service.GetPosReasonByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode, model.Istagtype, model.PageIndex, model.PageSize);

                return PartialView("_ReasonList", list);
            }

            return PartialView("_ReasonList", new List<PosReason>());
        }


        /// <summary>
        /// 赠送原因视图
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.RecoveryOrderDetailZ)]
        public PartialViewResult _GiveReasonList(PosReasonViewModel model)
        {
            if (model != null)
            {
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

                var service = GetService<IPosReasonService>();
                var list = service.GetPosReasonByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode, model.Istagtype, model.PageIndex, model.PageSize);

                return PartialView("_ReasonList", list);
            }
            return PartialView("_ReasonList", new List<PosReason>());
        }

        #endregion

        #region
        /// <summary>
        /// 要求视图
        /// </summary>
        [AuthButton(AuthFlag.Enable)]
        public ActionResult _RequestList(InSingleViewModel model)
        {
            var refeService = GetService<IPosRefeService>();//营业点
            var refe = refeService.Get(model.Refeid);
            if (refe != null && refe.IsRequest == true)
            {
                //营业点设置的是否显示要求
                if (model != null)
                {
                    model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                    model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

                    var service = GetService<IPosRequestService>();
                    var list = service.GetPosRequestByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode, model.PageIndex, model.PageSize);
                    return PartialView("_RequestList", list);
                }
                return PartialView("_RequestList", new List<PosReason>());
            }
            else
            {
                return Json(JsonResultData.Failure("当前营业点设置不显示要求"));
                //PartialView("_RequestList", new List<PosReason>());
            }
        }

        /// <summary>
        /// 作法类型视图
        /// </summary>
        [AuthButton(AuthFlag.Open)]
        public ActionResult _ActionTypeList(InSingleViewModel model)
        {
            var refeService = GetService<IPosRefeService>();//营业点
            var refe = refeService.Get(model.Refeid);
            if (refe != null && refe.IsAction == true)
            {
                if (model != null)
                {
                    model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                    model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

                    var service = GetService<IPosActionTypeService>();
                    var list = service.GetPosActionTypeByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode, model.PageIndex, model.PageSize);

                    var itemActionService = GetService<IPosItemActionService>();
                    ViewBag.isCuisine = itemActionService.GetPosItemActionTotal(CurrentInfo.HotelId, model.Itemid) > 0 ? true : false;
                    ViewBag.Itemid = model.Itemid;

                    return PartialView("_ActionTypeList", list);
                }
                return PartialView("_ActionTypeList", new List<PosActionType>());
            }
            else
            {
                return Json(JsonResultData.Failure("当前营业点设置不显示作法"));
            }
        }

        /// <summary>
        /// 作法视图
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _ActionList(PosActionViewModel model)
        {
            if (model != null)
            {
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

                if (string.IsNullOrWhiteSpace(model.mBillid))
                {
                    var service = GetService<IPosActionService>();
                    var list = service.GetActionByModuleAndType(CurrentInfo.HotelId, CurrentInfo.ModuleCode, model.ActionTypeId, model.PageIndex, model.PageSize);
                    ViewBag.ActionTypeId = model.ActionTypeId;
                    return PartialView("_ActionList", list);
                }
                else
                {
                    var service = GetService<IPosBillDetailActionService>();
                    var ActionTypeId = string.IsNullOrWhiteSpace(model.ActionTypeId) ? "" : model.ActionTypeId;
                    var list = service.GetBillDetailAction(CurrentInfo.HotelId, model.mBillid, model.mId, model.igroupid, ActionTypeId, model.PageIndex, model.PageSize);
                    ViewBag.ActionTypeId = model.ActionTypeId;
                    return PartialView("_ActionList", list);
                }
            }
            return PartialView("_ActionList", new List<PosAction>());
        }

        /// <summary>
        /// 作法分组视图
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _ActionGroupList(string mBillid, long? mid)
        {
            if (mBillid != null && mid != null)
            {
                var service = GetService<IPosBillDetailActionService>();
                var list = service.GetBillDetailActionGroupByMid(CurrentInfo.HotelId, mBillid, mid);
                if (list != null && list.Count > 0)
                {
                    ViewBag.BillItemIds = list[0].Ids.Replace(", ", ",");
                }

                return PartialView("_ActionGroupList", list);
            }
            return PartialView("_ActionGroupList", new List<up_pos_list_BillDetailActionGroupResult>());
        }

        /// <summary>
        /// 折扣类型视图
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _DiscTypeList(InSingleViewModel model)
        {
            if (model != null)
            {
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

                var service = GetService<IPosDiscTypeService>();
                var list = service.GetPosDiscTypeByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode, model.discType, model.PageIndex, model.PageSize);

                //var codeListService = GetService<ICodeListService>();
                //var codeList = codeListService.List("76").ToList().Where(m => m.code != "0");
                //ViewBag.DiscTypeList= codeList;
                return PartialView("_DiscTypeList", list);
            }
            return PartialView("_DiscTypeList", new List<up_pos_discTypeList>());
        }

        /// <summary>
        /// 餐台状态视图
        /// </summary>
        [AuthButton(AuthFlag.SetClean)]
        public PartialViewResult _TabStatusList(PosTabStatusViewModel model)
        {
            ViewBag.Isoutsell = false;
            ViewBag.IsVirtual = false;
            ViewBag.IsManual = false;

            if (model != null)
            {
                model.Refeid = model.Refeid ?? "";
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

                if (string.IsNullOrWhiteSpace(model.Refeid))
                {
                    var refeService = GetService<IPosRefeService>();
                    var refeList = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
                    if (refeList != null && refeList.Count > 0)
                    {
                        ViewBag.Isoutsell = refeList[0].Isoutsell;
                        foreach (var refe in refeList)
                        {
                            model.Refeid += ',' + refe.Id;
                        }

                        string[] openInfos = string.IsNullOrWhiteSpace(refeList[0].OpenInfo) ? new string[] { } : refeList[0].OpenInfo.Split(',');
                        foreach (var openInfo in openInfos)
                        {
                            if (openInfo == "I")
                            {
                                ViewBag.IsVirtual = true;
                                break;
                            }
                            else if (openInfo == "J")
                            {
                                ViewBag.IsManual = true;
                                break;
                            }
                        }
                    }

                    model.Refeid.Trim(',');
                }
                else
                {
                    var refeService = GetService<IPosRefeService>();
                    var refe = refeService.Get(model.Refeid);
                    ViewBag.Isoutsell = refe.Isoutsell;

                    string[] openInfos = string.IsNullOrWhiteSpace(refe.OpenInfo) ? new string[] { } : refe.OpenInfo.Split(',');
                    foreach (var openInfo in openInfos)
                    {
                        if (openInfo == "I")
                        {
                            ViewBag.IsVirtual = true;
                            break;
                        }
                        else if (openInfo == "J")
                        {
                            ViewBag.IsManual = true;
                            break;
                        }
                    }
                }

                var service = GetService<IPosTabStatusService>();
                var list = service.GetPosTabStatusResult(CurrentInfo.HotelId, model.Tabid, model.Refeid, "", null, model.PageIndex, model.PageSize);

                return PartialView("_TabStatusList", list);
            }
            return PartialView("_TabStatusList", new List<up_pos_list_TabStatusResult>());
        }

        /// <summary>
        /// 根据消费项目获取对应价格列表
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _ItemPriceList(PosItemPriceViewModel model)
        {
            if (model != null)
            {
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

                var service = GetService<IPosItemPriceService>();

                var sellOutService = GetService<IPosSellOutService>();  //沽清
                var list = service.GetPosItemPriceByItemId(CurrentInfo.HotelId, model.Itemid, model.PageIndex, model.PageSize);
                //循环列表
                foreach (var itemPrice in list)
                {
                    //根据项目ID与酒店ID取沽清表数据
                    var posSelloutModel = sellOutService.GetPosSelloutByItemId(CurrentInfo.HotelId, itemPrice.itemId);
                    if (posSelloutModel != null)
                    {
                        //判断沽清表中单位
                        if (string.IsNullOrWhiteSpace(posSelloutModel.UnitId) || posSelloutModel.UnitId.Contains(itemPrice.Unitid))
                        {
                            itemPrice.SelloutStatus = "0";  //代表全部
                        }
                    }
                }
                return PartialView("_ItemPriceList", list);
            }
            return PartialView("_ItemPriceList", new List<up_pos_list_ItemPriceByItemidResult>());
        }

        /// <summary>
        /// 根据消费项目获取对应价格列表
        /// </summary>
        [AuthButton(AuthFlag.RecoveryOrderDetailY)]
        public PartialViewResult _ItemListSelect(InSingleViewModel model)
        {
            return PartialView("_ItemListSelect", model);
        }

        /// <summary>
        /// 输入窗口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _LetterInput(LetterInputViewModel model)
        {
            return PartialView("_LetterInput", model);
        }

        /// <summary>
        /// 数字窗口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.AuthManage)]   //数量权限
        public PartialViewResult _NumberInput(LetterInputViewModel model)
        {
            //ViewBag.Version = CurrentVersion;
            return PartialView("_NumberInput", model);
        }

        /// <summary>
        /// 图例
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _Cutline()
        {
            return PartialView("_Cutline");
        }

        /// <summary>
        /// 快餐台开台
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _AddTakeoutTab(OpenTakeoutTabAddViewModel model)
        {
            PosRefe refe = Session["PosRefe"] as PosRefe;
            model.OpenInfo = refe.OpenInfo;
            return PartialView("_AddTakeoutTab", model);
        }

        /// <summary>
        /// 手写单视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult _AddHandWrite(PosBillDetailHandAddViewModel model)
        {
            var service = GetService<IPosItemService>();
            var priceService = GetService<IPosItemPriceService>();

            var item = service.Get(model.Itemid);
            var unit = new PosItemPrice();

            if (item.Unitid != null)
            {
                unit = priceService.GetPosItemPriceByUnitid(CurrentInfo.HotelId, item.Id, item.Unitid);
            }
            else
            {
                unit = priceService.GetPosItemDefaultPriceByUnitid(CurrentInfo.HotelId, item.Id);
            }

            if (unit != null)
            {
                model.Unitid = unit.Unitid;
                model.UnitCode = unit.UnitCode;
                model.UnitName = unit.Unit;
            }

            model.ItemName = "";
            model.Price = 1;

            return PartialView("_AddHandWrite", model);
        }

        #endregion

        #region 获取列表

        /// <summary>
        /// 获取指定账单下的账单明细列表
        /// </summary>
        /// <param name="request"></param> 
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListUpBillDetailByBillid([DataSourceRequest]DataSourceRequest request, string billid)
        {
            var billServer = GetService<IPosBillDetailService>();
            var list = billServer.GetUpBillDetailByBillid(CurrentInfo.HotelId, billid, PosItemDcFlag.D.ToString()).ToList();
            long count = 0;
            //foreach (var temp in list)
            //{
            //    temp.Row = count += 1;
            //    count = temp.Row;

            //    if (temp.SD == true)
            //    {
            //        temp.ItemName = "　" + temp.ItemName;
            //        temp.Dueamount = null;
            //        temp.Amount = null;
            //        temp.Discount = null;
            //        temp.DiscAmount = null;
            //        temp.Service = null;
            //    }
            //}
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 获取指定账单下的账单明细列表（先落弹窗列表）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListUpBillDetailByBillidAndStatus([DataSourceRequest]DataSourceRequest request, string billid)
        {
            var billServer = GetService<IPosBillDetailService>();
            var list = billServer.GetUpBillDetailByBillid(CurrentInfo.HotelId, billid, PosItemDcFlag.D.ToString()).Where(w => w.Status == (byte)PosBillDetailStatus.保存).ToList();
            foreach (var temp in list)
            {
                if (temp.SD == true)
                {
                    temp.ItemName = "　" + temp.ItemName;
                    temp.Dueamount = null;
                    temp.Amount = null;
                    temp.Discount = null;
                    temp.DiscAmount = null;
                    temp.Service = null;
                }
            }
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 获取指定账单下的账单明细列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListItemsForPosTabByModules([DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosTabService>();
            var list = service.GetPosTabByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 获取指定账单下的营业点
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListItemsForPosRefeByModules([DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosRefeService>();
            var list = service.GetRefeByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 获取指定账单下的账单明细类型
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListItemsForPosTypeByModules([DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosTabtypeService>();
            var list = service.GetTabtypeByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 根据消费大类获取消费项目总数
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ContentResult GetItemTotal(PosItemViewModel model)
        {
            if (model != null)
            {
                model.Keyword = model.Keyword ?? "";

                PosRefe refe = Session["PosRefe"] as PosRefe;
                var service = GetService<IPosItemService>();
                if (string.IsNullOrWhiteSpace(model.Refeid))
                {
                    return Content(service.GetPosItemTotal(CurrentInfo.HotelId, model.Refeid ?? "", model.Itemid ?? "", "", model.Keyword).ToString());
                }
                else if (!string.IsNullOrWhiteSpace(model.Itemid))
                {
                    return Content(service.GetPosItemTotal(CurrentInfo.HotelId, model.Refeid ?? "", model.Itemid ?? "", refe == null ? "" : refe.ShuffleId, model.Keyword).ToString());
                }
                else if (string.IsNullOrWhiteSpace(model.Itemid))
                {
                    return Content(service.GetPosItemTotal(CurrentInfo.HotelId, model.Refeid ?? "", model.Itemid ?? "", refe == null ? "" : refe.ShuffleId, model.Keyword).ToString());
                }
            }
            return Content("0");
        }

        /// <summary>
        /// 根据消费项目查询对应价格总数
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ContentResult GetItemPriceTotal(PosItemPriceViewModel model)
        {
            if (string.IsNullOrEmpty(model.itagperiod))
            {
                var service = GetService<IPosItemPriceService>();
                return Content(service.GetPosItemPriceTotal(CurrentInfo.HotelId, model.Itemid).ToString());
            }
            else
            {
                var billService = GetService<IPosBillService>();
                var bill = billService.Get(model.BillId);

                var tabService = GetService<IPosTabService>();
                var tab = tabService.Get(bill.Tabid);

                var service = GetService<IPosOnSaleService>();
                return Content(service.GetUnitByPosOnSaleItemTotal(CurrentInfo.HotelId, model.Itemid, bill.Refeid ?? "", model.itagperiod ?? "", bill.CustomerTypeid ?? "", tab.TabTypeid ?? "").ToString());
            }

        }

        /// <summary>
        /// 查询原因总数
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ContentResult GetReasonTotal(PosReasonViewModel model)
        {
            var service = GetService<IPosReasonService>();
            return Content(service.GetPosReasonByModuleTotal(CurrentInfo.HotelId, CurrentInfo.ModuleCode, model.Istagtype).ToString());
        }

        /// <summary>
        /// 查询要求总数
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ContentResult GetRequestTotal()
        {
            var service = GetService<IPosRequestService>();
            return Content(service.GetPosRequestByModuleTotal(CurrentInfo.HotelId, CurrentInfo.ModuleCode).ToString());
        }

        /// <summary>
        /// 查询作法类型总数
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ContentResult GetActionTypeTotal()
        {
            var service = GetService<IPosActionTypeService>();
            return Content(service.GetPosActionTypeByModuleTotal(CurrentInfo.HotelId, CurrentInfo.ModuleCode).ToString());
        }

        /// <summary>
        /// 查询作法总数
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ContentResult GetActionTotal(PosActionViewModel model)
        {
            var service = GetService<IPosActionService>();
            return Content(service.GetActionByModuleTotal(CurrentInfo.HotelId, CurrentInfo.ModuleCode, model.ActionTypeId).ToString());
        }

        /// <summary>
        /// 查询折扣类型总数
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ContentResult GetDiscTypeTotal(InSingleViewModel model)
        {
            var service = GetService<IPosDiscTypeService>();
            return Content(service.GetPosDiscTypeByModuleTotal(CurrentInfo.HotelId, CurrentInfo.ModuleCode, model.discType).ToString());
        }

        /// <summary>
        /// 查询餐台状态总数
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ContentResult GetTabStatusTotal(PosTabStatusViewModel model)
        {
            if (model != null)
            {

                model.Refeid = model.Refeid ?? "";
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;
                if (string.IsNullOrWhiteSpace(model.Refeid))
                {
                    var refeService = GetService<IPosRefeService>();
                    var refeList = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
                    if (refeList != null && refeList.Count > 0)
                    {
                        ViewBag.Isoutsell = refeList[0].Isoutsell;
                        foreach (var refe in refeList)
                        {
                            model.Refeid += ',' + refe.Id;
                        }
                    }
                    model.Refeid.Trim(',');
                }
                var service = GetService<IPosTabStatusService>();
                return Content(service.GetPosTabStatusResultTotal(CurrentInfo.HotelId, model.Tabid, model.Refeid, "", null).ToString());
            }
            return Content("0");
        }

        #endregion 获取列表

        #region 详细

        /// <summary>
        /// 账单明细
        /// </summary>
        /// <param name="posBillId">主账单ID</param>
        /// <param name="openFlag">详细打开Or客位打开（A：详细，B：客位）</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.SetWaitClean)]
        public ActionResult _PosBillDetailed(string posBillId, string openFlag)
        {
            ViewBag.PosBillId = posBillId;
            ViewBag.openFlag = openFlag;
            if (openFlag == "B")
            {
                return PartialView("_PosBillDetailed_B");
            }
            else
            {
                return PartialView("_PosBillDetailed");
            }
        }

        #endregion 详细

        #region 根据酒店ID，账单ID分组查询账单明细

        /// <summary>
        /// 根据酒店ID，账单ID分组查询账单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="billid">账单ID</param>
        /// <param name="Flag">类别 A：部门类别 B：大类 C：项目分类</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListUpBillDetailByGroupItem([DataSourceRequest]DataSourceRequest request, string billid, string Flag)
        {
            var billServer = GetService<IPosBillDetailService>();
            var result = billServer.GetBillDetailGroupItemClass(CurrentInfo.HotelId, billid, Flag, PosItemDcFlag.D.ToString());
            var s = result.ToDataSourceResult(request);
            return Json(result.ToDataSourceResult(request));
        }

        #endregion 根据酒店ID，账单ID分组查询账单明细

        #region 修改客位信息

        /// <summary>
        /// 入单界面修改客位信息
        /// </summary>
        /// <param name="billDetailId"></param>
        /// <param name="place"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult BillDetailPlace(string billDetailIds, string place)
        {
            UpdateBillDetailPlace(billDetailIds, place);
            return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 客位视图
        /// </summary>
        /// <param name="billDetailIds"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult BillDetailPlaceView(string billDetailIds)
        {
            BillDetailEditPlaceModel model = new BillDetailEditPlaceModel();
            model.Ids = billDetailIds;
            return PartialView("_BillDetailPlace", model);
        }

        /// <summary>
        /// 修改入单明细的客位信息
        /// </summary>
        /// <param name="billDetailIds">ID集合</param>
        /// <param name="place">客位值</param>
        private void UpdateBillDetailPlace(string billDetailIds, string place)
        {
            var billDetailIdList = billDetailIds.Split(',');
            var service = GetService<IPosBillDetailService>();
            var billService = GetService<IPosBillService>();
            foreach (var billDetailId in billDetailIdList)
            {
                if (!string.IsNullOrEmpty(billDetailId))
                {
                    var entity = new PosBillDetail();

                    var oldEntity = service.Get(int.Parse(billDetailId));
                    AutoSetValueHelper.SetValues(oldEntity, entity);
                    var bill = billService.Get(oldEntity.Billid);
                    if (place == "第一道" || place == "第二道" || place == "第三道")
                    {
                        oldEntity.Place = place;
                    }
                    else
                    {
                        oldEntity.Place += "_" + place;
                    }
                    service.Update(oldEntity, new PosBillDetail());
                    service.Commit();
                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + oldEntity.Id + "，名称：" + oldEntity.ItemName + "，客位：" + entity.Place + "-->" + oldEntity.Place, bill.BillNo);
                }
            }
        }

        #endregion 修改客位信息

        #region 通过账单主ID获取账单明细

        [AuthButton(AuthFlag.None)]
        public ActionResult GetBillDetailLisrForBillID(string BillId)
        {
            var service = GetService<IPosBillDetailService>();
            var list = service.GetBillDetailByDcFlagForPosInSing(CurrentInfo.HotelId, BillId, PosItemDcFlag.D.ToString());
            var result = "";
            foreach (var item in list)
            {
                result += item.Itemid + ",";
            }

            return Json(JsonResultData.Successed(result));
        }

        #endregion 通过账单主ID获取账单明细

        #region 判断营业点打单之后是否可以修改账单

        /// <summary>
        /// 验证营业点打单之后是否可以修改账单
        /// </summary>
        /// <param name="BillId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult CheckEditForBillRefe(string BillId)
        {
            var billService = GetService<IPosBillService>();
            var model = billService.Get(BillId);
            if (model != null)
            {
                var posRefeService = GetService<IPosRefeService>();

                var posRefe = posRefeService.Get(model.Refeid);//获取账单营业点ID

                if (posRefe.IsPrintEdit == false && (model.IPrint != null && model.IPrint > 0))
                {
                    return Json(JsonResultData.Failure("打单之后不可以修改！"));
                }

                //获取系统参数，
                var paraservice = GetService<IPmsParaService>();
                var IsPayOrderAgain = paraservice.IsPayOrderAgain(CurrentInfo.HotelId);
                if (!IsPayOrderAgain)
                {
                    if (model.Status == (byte)PosBillStatus.结账 || model.Status == (byte)PosBillStatus.清台)
                    {
                        return Json(JsonResultData.Failure("不能修改已经买单的账单！"));
                    }
                }

                //有取消权限直接跳过刷卡
                if (IsHasAuth("p60", (int)AuthFlag.Reset) || CurrentInfo.IsRegUser == true)
                {
                    return Json(JsonResultData.Successed("1"));
                }

                return Json(JsonResultData.Successed(""));
            }
            else
            {
                return Json(JsonResultData.Failure("账单不存在，请重试"));
            }
        }

        #endregion 判断营业点打单之后是否可以修改账单

        #region 换台

        /// <summary>
        /// 更换台号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.SetDirty)]
        public PartialViewResult _ChangeTable(InSingleViewModel model)
        {
            var billService = GetService<IPosBillService>();
            var bill = billService.GetPosBillByBillid(CurrentInfo.HotelId, model.Billid);

            ViewBag.oldModel = bill;    //传参到界面
            ViewBag.openFlag = model.openFlag;    //传参到界面
            return PartialView("_ChangeTable");
        }

        /// <summary>
        /// 转台
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult ChangeTable(ChangeTableModel model)
        {
            if (model != null)
            {
                var service = GetService<IPosBillService>();
                var billDetailService = GetService<IPosBillDetailService>();
                service.ChangeTab(CurrentInfo.HotelId, model.oldBillId, model.oldTabId, model.newServiceRate, model.newLimit, model.ServiceRateFlag, model.ItemFlag, model.newTabId);

                var billService = GetService<IPosBillService>();
                var bill = billService.Get(model.oldBillId);



                bill.InputUser = CurrentInfo.UserName;
                bill.BillDate = DateTime.Now;
                billService.Update(bill, new PosBill());
                //billService.AddDataChangeLog(OpLogType.Pos账单修改);
                AddOperationLog(OpLogType.Pos账单修改, "单号：" + bill.Billid + ",台号：" + model.oldTabId + "-->" + model.newTabId, bill.BillNo);
                billService.Commit();

                decimal amount = 0;
                var billRow = "";
                //账单操作表
                var billDetailList = billDetailService.GetBillDetailByDcFlag(CurrentInfo.HotelId, model.oldBillId, PosItemDcFlag.D.ToString());
                if (billDetailList != null && billDetailList.Count > 0)
                {
                    foreach (var item in billDetailList)
                    {
                        if (item.Status == (byte)PosBillDetailStatus.正常 || item.Status == (byte)PosBillDetailStatus.保存)
                        {
                            //计算本次转的金额
                            amount += item.Amount == null ? 0 : Convert.ToDecimal(item.Amount);
                        }
                        billRow += item.Id + ",";
                    }
                }

                var billChange = new PosBillChange
                {
                    Id = Guid.NewGuid(),
                    Hid = CurrentInfo.HotelId,
                    BillBsnsDate = bill.BillBsnsDate,
                    Refeid = bill.Refeid,
                    Tabid = model.oldTabId,
                    MBillid = bill.MBillid,
                    NmBillid = bill.MBillid,
                    Nrefeid = bill.Refeid,
                    Ntabid = model.newTabId,
                    iStatus = (byte)PosBillChangeStatus.全单转台,
                    Amount = amount,
                    BillRow = billRow,
                    Module = "CY",
                    TransUser = CurrentInfo.UserName,
                    CreateDate = DateTime.Now
                };

                var billChangeService = GetService<IPosBillChangeService>();
                billChangeService.Add(billChange);
                billChangeService.AddDataChangeLog(OpLogType.Pos账单操作记录增加);
                billChangeService.Commit();

                //锁台记录处理
                var tabLogService = GetService<IPosTabLogService>();
                var tabLogModel = tabLogService.GetPosTabLogByBillId(CurrentInfo.HotelId, model.oldBillId);

                if (tabLogModel != null)
                {
                    tabLogModel.Tabid = model.newTabId;
                    tabLogModel.TabNo = model.newTabNo;
                    tabLogModel.Msg = string.Format($"{model.newTabNo}餐台被{tabLogModel.Computer} => {CurrentInfo.UserName}在操作");

                    tabLogService.Update(tabLogModel, new PosTabLog());
                    tabLogService.AddDataChangeLog(OpLogType.Pos锁台修改);
                    tabLogService.Commit();
                }

                //转台之后的数据（用于打印转台通知单）
                var ListDetail = billDetailService.GetDataSetByChangeTab(CurrentInfo.HotelId, model.oldTabId, model.oldBillId).ToList();

                var serializer = new JavaScriptSerializer();
                var valueStr = ReplaceJsonDateToDateString(serializer.Serialize(ListDetail));

                var result = new
                {
                    url = Url.Action("Index", "PosInSingle", new { rnd = new Random().NextDouble() }) + "&refeid=" + model.newRefeId + "&tabid=" + HttpUtility.UrlEncode(model.newTabId, Encoding.UTF8) + "&billid=" + model.oldBillId + "&tabFlag=" + bill.TabFlag + "&openFlag=" + model.openFlag,
                    ListDetail = valueStr
                };
                var url = Url.Action("Index", "PosInSingle", new { rnd = new Random().NextDouble() }) + "&refeid=" + model.newRefeId + "&tabid=" + HttpUtility.UrlEncode(model.newTabId, Encoding.UTF8) + "&billid=" + model.oldBillId + "&tabFlag=" + bill.TabFlag + "&openFlag=" + model.openFlag;

                return Json(JsonResultData.Successed(result));
            }
            return Json(JsonResultData.Failure(""));
        }

        /// <summary>
        /// 下拉列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListTabFlag()
        {
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Value="1",Text="原台"},
                 new SelectListItem { Value="2",Text="新台"}
            };

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 选择餐台界面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _SelectPosTab(TabStatusViewModel model)
        {
            ViewBag.Isoutsell = false;
            ViewBag.IsVirtual = false;
            ViewBag.IsManual = false;

            if (model != null)
            {
                model.Refeid = model.Refeid ?? "";
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

                //if (string.IsNullOrWhiteSpace(model.Refeid))
                //{
                //    var refeService = GetService<IPosRefeService>();
                //    var refeList = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);
                //    if (refeList != null && refeList.Count > 0)
                //    {
                //        ViewBag.Isoutsell = refeList[0].Isoutsell;
                //        foreach (var refe in refeList)
                //        {
                //            model.Refeid += ',' + refe.Id;
                //        }

                //        string[] openInfos = refeList[0].OpenInfo.Split(',');
                //        foreach (var openInfo in openInfos)
                //        {
                //            if (openInfo == "I")
                //            {
                //                ViewBag.IsVirtual = true;
                //                break;
                //            }
                //            else if (openInfo == "J")
                //            {
                //                ViewBag.IsManual = true;
                //                break;
                //            }
                //        }
                //    }

                //    model.Refeid.Trim(',');
                //}
                //else
                //{
                var refeService = GetService<IPosRefeService>();
                var refe = refeService.Get(model.Refeid);
                ViewBag.Isoutsell = refe.Isoutsell;

                string[] openInfos = string.IsNullOrWhiteSpace(refe.OpenInfo) ? new string[] { } : refe.OpenInfo.Split(',');
                foreach (var openInfo in openInfos)
                {
                    if (openInfo == "I")
                    {
                        ViewBag.IsVirtual = true;
                        break;
                    }
                    else if (openInfo == "J")
                    {
                        ViewBag.IsManual = true;
                        break;
                    }
                }
                //}
                //if (refe.Isoutsell==true&& model.PageSize==22)   //有外卖台并且pagesize为22的时候
                //{
                //    model.PageSize -= 1;
                //}

                var service = GetService<IPosTabStatusService>();
                var list = service.GetPosTabStatusResult(CurrentInfo.HotelId, model.Tabid, model.Refeid, "", null, model.PageIndex, model.PageSize);

                var count = service.GetPosTabStatusResultTotal(CurrentInfo.HotelId, model.Tabid, model.Refeid, "", null);
                ViewBag.refeId = model.Refeid;
                ViewBag.HotelId = CurrentInfo.HotelId;
                ViewBag.PageIndex = model.PageIndex;
                ViewBag.PageSize = model.PageSize;
                ViewBag.PageTotal = count;
                return PartialView("_SelectPosTab", list);
            }
            return PartialView("_SelectPosTab", new List<up_pos_list_TabStatusResult>());
            //   return PartialView("_SelectPosTab", list);
        }

        /// <summary>
        /// 根据餐台，营业点获取餐台服务费率与最低消费
        /// </summary>
        /// <param name="tabId"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult _GetPosTabInfo(string tabId, string RefeId)
        {
            try
            {
                var posTabService = GetService<IPosTabService>();
                var posTab = posTabService.GetEntity(CurrentInfo.HotelId, tabId);
                DateTime openTime = DateTime.Now;
                var posTabServiceService = GetService<IPosTabServiceService>();
                var posTabServiceModel = posTabServiceService.GetPosTabService(CurrentInfo.HotelId, CurrentInfo.ModuleCode, RefeId, posTab.TabTypeid, null, (byte)PosITagperiod.随时, openTime);
                if (posTabServiceModel != null)
                {
                    var result = new
                    {
                        newServiceRate = posTabServiceModel.Servicerate == null ? 0 : posTabServiceModel.Servicerate, //服务费率
                        newLimit = posTabServiceModel.NLimit == null ? 0 : posTabServiceModel.NLimit   //最低消费
                    };
                    return Json(JsonResultData.Successed(result));
                }
                return Json(JsonResultData.Failure(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
        }

        #endregion 换台

        #region 补打功能
        /// <summary>
        /// 补打全部点菜单
        /// </summary>
        [HttpPost]
        [AuthButton(AuthFlag.UpgradeCard)]
        public ActionResult SupplyPrintAll(string billid)
        {
            if (!string.IsNullOrWhiteSpace(billid))
            {
                var service = GetService<IPosBillDetailService>();
                var billService = GetService<IPosBillService>();

                var refe = Session["PosRefe"] as PosRefe;
                if (refe.IPrintBillss == PosPrintBillss.打印)
                {
                    var orderlist = service.GetBillOrderByPrint(CurrentInfo.HotelId, billid, billid).ToList();

                    foreach (var item in orderlist)
                    {
                        var detail = service.Get(item.id);
                        if (detail != null)
                        {
                            var bill = billService.Get(detail.Billid);
                            var newDetail = new PosBillDetail();
                            AutoSetValueHelper.SetValues(detail, newDetail);
                            newDetail.iOrderPrint = Convert.ToByte((newDetail.iOrderPrint ?? 0) + 1);
                            service.Update(newDetail, detail);
                            service.Commit();
                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newDetail.Id + ",点菜单次数：" + detail.iOrderPrint + "-->+" + newDetail.iOrderPrint, bill.BillNo);
                        }
                    }
                    //var list = service.GetBillDetailByBillid(CurrentInfo.HotelId, billid, PosItemDcFlag.D.ToString(), (byte)PosBillDetailStatus.正常).ToList();
                    //foreach (var temp in list)
                    //{
                    //    var oldtemp = new PosBillDetail();
                    //    AutoSetValueHelper.SetValues(temp, oldtemp);
                    //    foreach (var order in orderlist)
                    //    {
                    //        if (temp.Id == order.id)
                    //        {
                    //            var bill = billService.Get(temp.Billid);
                    //            byte iOrderPrint = temp.iOrderPrint ?? 0;
                    //            temp.iOrderPrint = Convert.ToByte(iOrderPrint + 1);
                    //            service.Update(temp, new PosBillDetail());
                    //            // service.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                    //            service.Commit();
                    //            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + temp.Id + ",点菜单次数：" + oldtemp.iOrderPrint + "-->+" + temp.iOrderPrint, bill.BillNo);
                    //            break;
                    //        }
                    //    }
                    //}

                    return Json(JsonResultData.Successed(orderlist));
                }
            }

            return Json(JsonResultData.Failure("打印失败"));
        }

        /// <summary>
        /// 补打所选批次点菜单
        /// </summary>
        [HttpPost]
        [AuthButton(AuthFlag.UpgradeCard)]
        public ActionResult SupplyPrintSelBatch(string billid, string batchTime)
        {
            if (!string.IsNullOrWhiteSpace(billid) && !string.IsNullOrWhiteSpace(batchTime))
            {
                var service = GetService<IPosBillDetailService>();
                var billService = GetService<IPosBillService>();

                var refe = Session["PosRefe"] as PosRefe;
                if (refe.IPrintBillss == PosPrintBillss.打印)
                {
                    var orderlist = service.GetBillOrderByPrint(CurrentInfo.HotelId, billid, billid).Where(w => Convert.ToInt32(w.批次) == Convert.ToInt32(batchTime)).ToList();

                    foreach (var item in orderlist)
                    {
                        var detail = service.Get(item.id);
                        if (detail != null)
                        {
                            var bill = billService.Get(detail.Billid);
                            var newDetail = new PosBillDetail();
                            AutoSetValueHelper.SetValues(detail, newDetail);
                            newDetail.iOrderPrint = Convert.ToByte((newDetail.iOrderPrint ?? 0) + 1);
                            service.Update(newDetail, detail);
                            service.Commit();
                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newDetail.Id + ",点菜单次数：" + detail.iOrderPrint + "-->+" + newDetail.iOrderPrint, bill.BillNo);
                        }
                    }
                    //var list = service.GetBillDetailByBillid(CurrentInfo.HotelId, billid, PosItemDcFlag.D.ToString(), (byte)PosBillDetailStatus.正常).Where(w => Convert.ToByte(w.iOrderPrint) == 0).ToList();
                    //foreach (var temp in list)
                    //{
                    //    var oldtemp = new PosBillDetail();
                    //    AutoSetValueHelper.SetValues(temp, oldtemp);
                    //    foreach (var order in orderlist)
                    //    {
                    //        if (temp.Id == order.id)
                    //        {
                    //            var bill = billService.Get(temp.Billid);        //获取账单信息
                    //            byte iOrderPrint = temp.iOrderPrint ?? 0;
                    //            temp.iOrderPrint = Convert.ToByte(iOrderPrint + 1);
                    //            service.Update(temp, new PosBillDetail());
                    //            // service.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                    //            service.Commit();
                    //            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + temp.Id + ",点菜单次数：" + oldtemp.iOrderPrint + "-->+" + temp.iOrderPrint, bill.BillNo);
                    //            break;
                    //        }
                    //    }
                    //}

                    return Json(JsonResultData.Successed(orderlist));
                }
            }

            return Json(JsonResultData.Failure("打印失败"));
        }

        /// <summary>
        /// 补打所选消费项目的点菜单
        /// </summary>
        [HttpPost]
        [AuthButton(AuthFlag.UpgradeCard)]
        public ActionResult SupplyPrintSelItem(string billid, string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var service = GetService<IPosBillDetailService>();
                var billService = GetService<IPosBillService>();

                var refe = Session["PosRefe"] as PosRefe;
                if (refe.IPrintBillss == PosPrintBillss.打印)
                {
                    var orderlist = service.GetBillOrderByPrint(CurrentInfo.HotelId, billid, billid).Where(w => w.id == Convert.ToInt64(id)).ToList();

                    foreach (var item in orderlist)
                    {
                        var detail = service.Get(item.id);
                        if (detail != null)
                        {
                            var bill = billService.Get(detail.Billid);
                            var newDetail = new PosBillDetail();
                            AutoSetValueHelper.SetValues(detail, newDetail);
                            newDetail.iOrderPrint = Convert.ToByte((newDetail.iOrderPrint ?? 0) + 1);
                            service.Update(newDetail, detail);
                            service.Commit();
                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newDetail.Id + ",点菜单次数：" + detail.iOrderPrint + "-->+" + newDetail.iOrderPrint, bill.BillNo);
                        }
                    }
                    //var entity = service.GetBillDetailByBillid(CurrentInfo.HotelId, billid, PosItemDcFlag.D.ToString(), (byte)PosBillDetailStatus.正常).Where(w => w.Id == Convert.ToInt64(id)).FirstOrDefault();
                    //var oldEntity = new PosBillDetail();
                    //AutoSetValueHelper.SetValues(entity, oldEntity);
                    //var bill = billService.Get(entity.Billid);
                    //foreach (var temp in orderlist)
                    //{

                    //    if (entity.Id == temp.id)
                    //    {
                    //        byte iOrderPrint = entity.iOrderPrint ?? 0;
                    //        entity.iOrderPrint = Convert.ToByte(iOrderPrint + 1);
                    //        service.Update(entity, new PosBillDetail());
                    //        // service.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                    //        service.Commit();
                    //        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + entity.Id + ",点菜单次数：" + oldEntity.iOrderPrint + "-->+" + entity.iOrderPrint, bill.BillNo);
                    //        break;
                    //    }
                    //}

                    return Json(JsonResultData.Successed(orderlist));
                }
            }

            return Json(JsonResultData.Failure("打印失败"));
        }
        #endregion

        #region 统一输入折扣界面



        /// <summary>
        /// 修改折扣界面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult UpdatePosDiscType(BillDiscountModel model)
        {
            var service = GetService<IPosBillService>();
            var BillDetailservice = GetService<IPosBillDetailService>();
            var oldEntity = service.Get(model.Id);//获取实体

            //获取系统参数，
            var paraservice = GetService<IPmsParaService>();
            var IsPayOrderAgain = paraservice.IsPayOrderAgain(CurrentInfo.HotelId);
            if (!IsPayOrderAgain)   //不让二次消费，不允许买单后账单修改
            {
                if (oldEntity.Status == (byte)PosBillStatus.清台 || oldEntity.Status == (byte)PosBillStatus.结账)
                {
                    return Json(JsonResultData.Failure("已经买单，清台的账单不能修改折扣"));
                }
            }

            PosBill newModle = new PosBill();
            AutoSetValueHelper.SetValues(oldEntity, newModle);

            //使用折扣功能前 先取消所有折扣
            if (!string.IsNullOrEmpty(newModle.Profileid) && !string.IsNullOrEmpty(newModle.CardNo))
            {
                newModle.Profileid = null;
                newModle.CardNo = null;
                this.PriCancelDiscount(oldEntity.Billid);
            }
            PosCommon common = new PosCommon();

            //  0：全单折；1：照单全折；2：全单金额折；3：照单金额折；4：单道折扣；5：单道金额折
            if (model.discType == "-1" || model.discType == "0")//全单折扣(收银折扣界面传递进来的是0.入单界面传递进来的是-1)
            {
                newModle.Discount = model.disCount > 1 ? model.disCount / 100 : model.disCount;
                var result = common.CheckOperDiscount(newModle.Discount, null, oldEntity.Refeid);
                if (!result.Success)
                {
                    return Json(result);
                }

                newModle.IsForce = 1;
                newModle.Approver = CurrentInfo.UserName;   //折扣人
                service.Update(newModle, oldEntity);
                //service.AddDataChangeLog(OpLogType.Pos账单修改);
                service.Commit();
                AddOperationLog(OpLogType.Pos账单修改, "全单折扣：" + oldEntity.Discount + "-->" + newModle.Discount + ",折扣类型：" + oldEntity.IsForce + "-->" + newModle.IsForce, newModle.BillNo);

                BillDetailservice.UpdateBillDetailDisc(CurrentInfo.HotelId, oldEntity.Billid, oldEntity.MBillid);

                //    BillDetailservice.StatisticsBillDetail(CurrentInfo.HotelId, oldEntity.Billid, oldEntity.MBillid);
            }
            else if (model.discType == "1")//照单全折
            {
                newModle.Discount = model.disCount > 1 ? model.disCount / 100 : model.disCount;
                var result = common.CheckOperDiscount(newModle.Discount, null, oldEntity.Refeid);
                if (!result.Success)
                {
                    return Json(result);
                }
                newModle.IsForce = 2;
                newModle.Approver = CurrentInfo.UserName;   //折扣人
                service.Update(newModle, oldEntity);
                // service.AddDataChangeLog(OpLogType.Pos账单修改);
                service.Commit();
                AddOperationLog(OpLogType.Pos账单修改, "照单全折：" + oldEntity.Discount + "-->" + newModle.Discount + ",折扣类型：" + oldEntity.IsForce + "-->" + newModle.IsForce, newModle.BillNo);

                BillDetailservice.UpdateBillDetailDisc(CurrentInfo.HotelId, oldEntity.Billid, oldEntity.MBillid);
                // BillDetailservice.StatisticsBillDetail(CurrentInfo.HotelId, oldEntity.Billid, oldEntity.MBillid);
            }
            else if (model.discType == "2")//全单金额折
            {
                newModle.DiscAmount = model.disCount;
                var result = common.CheckOperDiscount(null, newModle.DiscAmount, oldEntity.Refeid);
                if (!result.Success)
                {
                    return Json(result);
                }
                //输入的金额小于等于0的时候，既不是全单金额折也不是照单金额折
                if (model.disCount <= 0)
                {
                    newModle.DaType = null;
                }
                else { newModle.DaType = 0; }


                //与会员折扣兼容，不会相互替换
                if (newModle.IsForce != 3)
                    newModle.IsForce = 0;

                newModle.Approver = CurrentInfo.UserName;   //折扣人
                service.Update(newModle, oldEntity);
                //service.AddDataChangeLog(OpLogType.Pos账单修改);

                service.Commit();

                AddOperationLog(OpLogType.Pos账单修改, "全单金额折：" + oldEntity.DaType + "-->" + newModle.DaType + ",折扣类型：" + oldEntity.IsForce + "-->" + newModle.IsForce, newModle.BillNo);
                BillDetailservice.UpdateBillDetailDisc(CurrentInfo.HotelId, oldEntity.Billid, oldEntity.MBillid);
                // BillDetailservice.StatisticsBillDetail(CurrentInfo.HotelId, oldEntity.Billid, oldEntity.MBillid);
            }
            else if (model.discType == "3")//照单金额折
            {
                newModle.DiscAmount = model.disCount;

                var result = common.CheckOperDiscount(null, newModle.DiscAmount, oldEntity.Refeid);
                if (!result.Success)
                {
                    return Json(result);
                }
                //输入的金额小于等于0的时候，既不是全单金额折也不是照单金额折
                if (model.disCount <= 0)
                {
                    newModle.DaType = null;
                }
                else { newModle.DaType = 1; }

                //与会员折扣兼容，不会相互替换
                if (newModle.IsForce != 3)
                    newModle.IsForce = 0;

                newModle.Approver = CurrentInfo.UserName;   //折扣人
                service.Update(newModle, oldEntity);
                // service.AddDataChangeLog(OpLogType.Pos账单修改);
                service.Commit();

                AddOperationLog(OpLogType.Pos账单修改, "照单金额折：" + oldEntity.DaType + "-->" + newModle.DaType + ",折扣类型：" + oldEntity.IsForce + "-->" + newModle.IsForce, newModle.BillNo);

                BillDetailservice.UpdateBillDetailDisc(CurrentInfo.HotelId, oldEntity.Billid, oldEntity.MBillid);
                //  BillDetailservice.StatisticsBillDetail(CurrentInfo.HotelId, oldEntity.Billid, oldEntity.MBillid);
            }

            PosBillDetail newBillDetail = new PosBillDetail();
            if (model.discType == "4" || model.discType == "5") //单道折扣 or 单道金额折
            {
                //分割传递进来的ID
                var detailIDListArr = model.detailIdList.Split(',');
                foreach (var detailID in detailIDListArr)
                {
                    if (!string.IsNullOrWhiteSpace(detailID))
                    {
                        var oldBillDetail = BillDetailservice.Get(int.Parse(detailID));
                        AutoSetValueHelper.SetValues(oldBillDetail, newBillDetail);
                        if (oldBillDetail != null)
                        {
                            if (model.discType == "4")    //单道折扣
                            {
                                newBillDetail.IsDishDisc = true;
                                newBillDetail.Discount = model.disCount > 1 ? model.disCount / 100 : model.disCount;
                                var result = common.CheckOperDiscount(newBillDetail.Discount, null, oldEntity.Refeid);
                                if (!result.Success)
                                {
                                    return Json(result);
                                }
                            }
                            if (model.discType == "5")//单道金额折
                            {
                                newBillDetail.IsDishDisc = false;
                                newBillDetail.DiscAmount = model.disCount;

                                var result = common.CheckOperDiscount(null, newBillDetail.DiscAmount, oldEntity.Refeid);
                                if (!result.Success)
                                {
                                    return Json(result);
                                }
                            }
                            newBillDetail.Approver = CurrentInfo.UserName;   //折扣人
                            BillDetailservice.Update(newBillDetail, oldBillDetail);
                            //  BillDetailservice.AddDataChangeLog(OpLogType.Pos账单付款明细修改);
                            BillDetailservice.Commit();
                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newBillDetail.Id + "，是否单道折扣：" + oldBillDetail.IsDishDisc + "->" + newBillDetail.IsDishDisc + "，折扣率：" + oldBillDetail.Discount + "->" + newBillDetail.Discount, newModle.BillNo);

                            BillDetailservice.UpdateBillDetailDisc(CurrentInfo.HotelId, oldEntity.Billid, oldEntity.MBillid);
                            //   BillDetailservice.StatisticsBillDetail(CurrentInfo.HotelId, oldBillDetail.Billid, oldBillDetail.MBillid);
                        }
                    }
                }
            }

            return Json(JsonResultData.Successed());
        }

        #endregion 统一输入折扣界面

        #region 添加手写作法

        /// <summary>
        /// 更细手写作法
        /// </summary>
        /// <param name="billDetailId">账单明细ID</param>
        /// <param name="val">作法内容</param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult AddPracticeItem(string Id, string Practice)
        {
            var billService = GetService<IPosBillService>();

            var service = GetService<IPosBillDetailService>();
            var oldEntity = service.Get(Convert.ToInt32(Id));
            if (oldEntity != null)
            {
                var bill = billService.Get(oldEntity.Billid);
                PosBillDetail newEntity = new PosBillDetail();
                //数据复制
                AutoSetValueHelper.SetValues(oldEntity, newEntity);
                newEntity.Action += "," + Practice;
                service.Update(newEntity, oldEntity);
                //  service.AddDataChangeLog(OpLogType.Pos账单作法明细修改);
                service.Commit();
                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newEntity.Id + "，作法：" + oldEntity.Action + "-->" + newEntity.Action, bill.BillNo);

                return Json(JsonResultData.Successed(""));
            }
            else
            {
                return Json(JsonResultData.Failure("要添加作法的项目不存在！"));
            }
        }

        #endregion 添加手写作法

        #region 获取作法明细

        /// <summary>
        /// 获取作法明细
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="billId"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult BilldetailActionList(string Id, string billId)
        {
            var service = GetService<IPosBillDetailActionService>();
            var actionService = GetService<IPosActionService>();
            var actionList = service.GetBillDetailActionGroupByMid(CurrentInfo.HotelId, billId, Convert.ToInt32(Id));
            var arrId = "";
            foreach (var action in actionList)
            {
                // arrId += action.Ids + ",";
                var ids = action.Ids.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    var model = service.Get(Convert.ToInt32(ids[i]));
                    var actionModel = actionService.GetActionByCode(CurrentInfo.HotelId, model.ActionNo, model.ActionName);
                    arrId += actionModel == null ? "" : actionModel.Id + ",";
                }
            }
            return Json(JsonResultData.Successed(arrId));
        }

        #endregion 获取作法明细

        #region 更新手写作法

        /// 更新手写作法
        [AuthButton(AuthFlag.None)]
        public ActionResult UpdateBillDetailHandAction(PosBillDetailActionAddViewModel model)
        {
            var billService = GetService<IPosBillService>();

            try
            {
                var actionService = GetService<IPosActionService>();
                var service = GetService<IPosBillDetailActionService>();
                if (model.Igroupid == null)
                {
                    model.Igroupid = service.GetIgroupidByMid(CurrentInfo.HotelId, model.MBillid, model.Mid);
                }
                decimal amount = 0;
                if (model.AddPrice != null && model.AddPrice > 0)
                {
                    if (model.IByQuan != null && model.IByQuan.Value && model.IByGuest != null && model.IByGuest.Value)
                    {
                        amount = Convert.ToDecimal(model.AddPrice) * Convert.ToDecimal(model.Quan) * Convert.ToDecimal(model.IGuest);
                    }
                    else if (model.IByQuan != null && model.IByQuan.Value)
                    {
                        amount = Convert.ToDecimal(model.AddPrice) * Convert.ToDecimal(model.Quan);
                    }
                    else if (model.IByGuest != null && model.IByGuest.Value)
                    {
                        amount = Convert.ToDecimal(model.AddPrice) * Convert.ToDecimal(model.IGuest);
                    }
                    else
                    {
                        amount = Convert.ToDecimal(model.AddPrice);
                    }
                }

                PosBillDetailAction billDetailAction = new PosBillDetailAction()
                {
                    Hid = CurrentInfo.HotelId,
                    ActionNo = "Hand",
                    ActionName = model.HandActionName,
                    AddPrice = 0,
                    Nmultiple = 0,
                    IByQuan = false,
                    IByGuest = false,
                    Amount = 0,
                    ModiUser = CurrentInfo.UserName,
                    ModiDate = DateTime.Now,
                    Igroupid = model.Igroupid,

                };
                AutoSetValueHelper.SetValues(model, billDetailAction);
                billDetailAction.Amount = amount;
                service.Add(billDetailAction);
                //  service.AddDataChangeLog(OpLogType.Pos账单作法明细增加);
                service.Commit();

                var bill = billService.Get(model.MBillid);  //获取账单信息
                AddOperationLog(OpLogType.Pos账单作法明细增加, "单号：" + billDetailAction.Id + "，作法：" + model.HandActionName + "，金额：" + billDetailAction.Amount, bill.BillNo);

                var billDetailService = GetService<IPosBillDetailService>();
                var billDetail = billDetailService.Get(model.Mid);

                decimal addprice = 0;
                string actionString = "";
                var list = service.GetBillDetailActionByMid(CurrentInfo.HotelId, model.MBillid, model.Mid);
                if (list != null && list.Count > 0)
                {
                    int? igroupid = null;
                    foreach (var temp in list)
                    {
                        addprice += temp.Amount ?? 0;
                        if (igroupid == null || temp.Igroupid == igroupid)
                        {
                            actionString += "," + temp.ActionName;
                        }
                        else
                        {
                            actionString += "/" + temp.ActionName;
                        }
                        igroupid = temp.Igroupid;
                    }
                }

                var oldBillDetail = new PosBillDetail() { AddPrice = billDetail.AddPrice, Action = billDetail.Action, Dueamount = billDetail.Dueamount, Amount = billDetail.Amount };

                billDetail.AddPrice = addprice;
                billDetail.Action = actionString.Trim(',');
                billDetail.Dueamount += addprice;

                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，作法：" + oldBillDetail.Action + "-->" + billDetail.Action + "，加价：" + oldBillDetail.AddPrice + "-->" + billDetail.AddPrice, bill.BillNo);
                billDetailService.Update(billDetail, new PosBillDetail());
                billDetailService.Commit();


                return Json(JsonResultData.Successed(model.Igroupid));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        #endregion 更新手写作法

        #region 更新手写要求

        /// <summary>
        /// 更新手写要求
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.Enable)]
        public ActionResult UpdateBillDetailHandRequest(PosBillDetail model)
        {
            try
            {
                var service = GetService<IPosBillDetailService>();
                var oldEntity = service.Get(model.Id);
                var newEntity = new PosBillDetail();

                AutoSetValueHelper.SetValues(oldEntity, newEntity);
                newEntity.Request = model.Request;

                var billService = GetService<IPosBillService>();
                var bill = billService.Get(newEntity.Billid);
                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newEntity.Id + "，要求：" + oldEntity.Request + "-->" + newEntity.Request, bill.BillNo);
                service.Update(newEntity, oldEntity);
                //service.AddDataChangeLog(OpLogType.Pos账单付款明细修改);
                service.Commit();
                return Json(JsonResultData.Successed(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
            // return Json(JsonResultData.Successed());
        }

        #endregion 更新手写要求

        #region 高级功能

        /// <summary>
        /// 高级功能列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _AdvanceFuncList(PosAdvanceFuncViewModel model)
        {
            var service = GetService<IPosAdvanceFuncService>();
            var PosAdvanceFuncList = service.GetPosAdvanceFuncList(CurrentInfo.HotelId, model.refeId, CurrentInfo.ModuleCode, model.PageIndex, model.PageSize);

            //排除掉系统管理员的权限控制
            if (CurrentInfo.IsRegUser == false)
            {
                var userRoleServices = GetService<IUserRoleSingleService>();
                var role = userRoleServices.List(CurrentInfo.HotelId, Guid.Parse(CurrentInfo.UserId)).ToList(); //查询单店用户的角色权限
                //判断是否是集团
                if (CurrentInfo.IsGroup)
                {
                    var userService = GetService<IPmsUserService>();
                    var UserID = userService.GetUserIDByCode(CurrentInfo.GroupId, CurrentInfo.UserCode).Id;//查询集团的用户id
                    role = userRoleServices.grpList(CurrentInfo.HotelId, UserID, CurrentInfo.GroupId).ToList(); //查询集团用户的角色权限
                }

                List<string> advance = new List<string>();
                var authManageService = GetService<IAuthListService>();
                role.ForEach(r =>
                {
                    var authLists = authManageService.GetAdvanceFuncLists(CurrentInfo.HotelId, r.Roleid.ToString());
                    authLists.Where(w => w.Checked == true).Select(s => s.AuthName).ToList().ForEach(a =>
                    {
                        advance.Add(a);
                    });
                });

                advance = advance.Distinct().ToList();//去掉重复的

                PosAdvanceFuncList = PosAdvanceFuncList.Where(w => advance.Contains(w.Name1)).ToList();
            }

            return PartialView("_AdvanceFuncList", PosAdvanceFuncList);
        }

        /// <summary>
        /// 获取高级功能总数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ContentResult GetAdvanceFuncTotal(PosAdvanceFuncViewModel model)
        {
            var service = GetService<IPosAdvanceFuncService>();
            var count = service.GetPosAdvanceFuncCount(CurrentInfo.HotelId, model.refeId, CurrentInfo.ModuleCode);
            return Content(count.ToString());
        }

        #region 修改单价

        /// <summary>
        /// 修改单价界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Inspect)]
        public ActionResult _PriceNumber(string id)
        {
            ViewBag.Id = id;
            return PartialView("_PriceNumber");
        }

        /// <summary>
        /// 修改单价
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult UpdatePriceNumber(string Id, decimal Price)
        {
            try
            {
                var billDetailService = GetService<IPosBillDetailService>();
                var oldEntity = billDetailService.Get(int.Parse(Id));
                if (oldEntity == null)
                {
                    return Json(JsonResultData.Failure("要修改的消费项目不存在，请重新操作"));
                }
                var billService = GetService<IPosBillService>();
                var bill = billService.Get(oldEntity.Billid);


                var newEntity = new PosBillDetail();
                AutoSetValueHelper.SetValues(oldEntity, newEntity);

                newEntity.Price = Price;

                //修改单价要修改原价
                newEntity.PriceOri = Price;

                newEntity.Dueamount = Price * newEntity.Quantity;
                billDetailService.Update(newEntity, oldEntity);
                billDetailService.AddDataChangeLog(OpLogType.Pos账单修改);
                billDetailService.Commit();
                AddOperationLog(OpLogType.Pos账单消费明细修改, "修改单价：" + oldEntity.Price + "-->" + newEntity.Price + "，金额：" + oldEntity.Dueamount + "-->" + newEntity.Dueamount, bill.BillNo);

                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Successed(ex.Message.ToString()));
            }
        }

        #endregion 

        #region 取消打单

        [AuthButton(AuthFlag.UpdateCardStatus)]
        public ActionResult CancelPrint(string billId)
        {
            var service = GetService<IPosBillService>();
            var oldEntity = service.Get(billId);
            if (oldEntity == null)
            {
                return Json(JsonResultData.Failure("要操作的数据不存在！"));
            }
            var newEntity = new PosBill();
            AutoSetValueHelper.SetValues(oldEntity, newEntity);
            newEntity.IPrint = 0;   //打单次数修改成0
            service.Update(newEntity, oldEntity);
            service.AddDataChangeLog(OpLogType.Pos账单修改);
            AddOperationLog(OpLogType.Pos账单修改, "取消打单：" + oldEntity.IPrint + "-->" + newEntity.IPrint, newEntity.BillNo);
            service.Commit();

            return Json(JsonResultData.Successed());
        }

        #endregion 取消打单

        #region 手工清台

        [AuthButton(AuthFlag.MemberRecharge)]
        public ActionResult HandClearTable(string billId)
        {
            var service = GetService<IPosBillService>();
            var oldEntity = service.Get(billId);
            if (oldEntity == null)
            {
                return Json(JsonResultData.Failure("要操作的数据不存在"));
            }
            var billDetailService = GetService<IPosBillDetailService>();

            //获取付款金额与已付金额
            var billDetailList = billDetailService.GetBillDetailForPaymentTotalByBillid(CurrentInfo.HotelId, billId);
            if (billDetailList.Paid != billDetailList.Total)
            {
                return Json(JsonResultData.Failure("账单未全部买单！"));
            }

            //获取收银点
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);
            //源数据赋值到新的实体
            var newEntity = new PosBill();
            AutoSetValueHelper.SetValues(oldEntity, newEntity);



            //修改账单状态
            if (oldEntity.TabFlag == (byte)PosBillTabFlag.物理台)
            {
                newEntity.DepBsnsDate = pos.Business;
                newEntity.MoveUser = CurrentInfo.UserName;
                newEntity.DepDate = DateTime.Now;
                newEntity.Status = (byte)PosBillStatus.清台;

                service.Update(newEntity, oldEntity);
                // service.AddDataChangeLog(OpLogType.Pos账单修改);
                service.Commit();
                AddOperationLog(OpLogType.Pos账单修改, "手工清台：" + oldEntity.Status + "-->" + newEntity.Status, newEntity.BillNo);
            }

            var statusService = GetService<IPosTabStatusService>();
            //修改餐台状态
            PosCommon common = new PosCommon();
            common.SetTabStatus(oldEntity);


            //清理锁台记录
            var tabLogService = GetService<IPosTabLogService>();
            var tabLogList = tabLogService.GetPosTabLogListByTab(CurrentInfo.HotelId, newEntity.Refeid, newEntity.Tabid, newEntity.TabNo);
            //if (tabLogList != null && tabLogList.Count > 0)
            //{
            foreach (var tabLog in tabLogList)
            {
                if (tabLog.Billid == oldEntity.Billid)
                {
                    tabLogService.Delete(tabLog);
                    tabLogService.AddDataChangeLog(OpLogType.Pos锁台删除);
                    tabLogService.Commit();
                }
            }
            //}
            return Json(JsonResultData.Successed());
        }

        #endregion 手工清台

        #region 取消服务费

        /// <summary>
        /// 取消服务费
        /// </summary>
        /// <param name="billId">账单ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.IntegralExchange)]
        public ActionResult CancelServiceRate(string billId)
        {
            var service = GetService<IPosBillService>();
            var bill = service.Get(billId); //获取对象
                                            // bill.ServiceRate = 0;

            var oldBill = new PosBill();
            AutoSetValueHelper.SetValues(bill, oldBill);
            bill.IsService = false;
            bill.ServiceRate = 0;
            service.Update(bill, new PosBill());
            service.AddDataChangeLog(OpLogType.Pos账单修改);
            service.Commit();
            AddOperationLog(OpLogType.Pos账单修改, "免服务费：" + oldBill.IsService + "-->" + bill.IsService, bill.BillNo);

            return Json(JsonResultData.Successed());
        }

        #endregion 取消服务费

        #region 取消折扣

        /// <summary>
        /// 取消折扣
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult CancelDiscount(string billId)
        {
            PosCommon posCommon = new PosCommon();
            var res = posCommon.CancelDisCount(billId);
            return Json(res);
        }
        private ActionResult PriCancelDiscount(string billId)
        {
            try
            {
                var billService = GetService<IPosBillService>();
                var bill = billService.GetEntity(CurrentInfo.HotelId, billId);
                if (bill.Status == (byte)PosBillStatus.结账 || bill.Status == (byte)PosBillStatus.清台)
                {
                    return Json(JsonResultData.Failure("已经买单，清台的账单不能修改折扣"));
                }


                var oldDiscount = bill.Discount;
                var oldDiscAmount = bill.DiscAmount;

                bill.DiscAmount = 0;    //折扣金额
                bill.Discount = 1;  //折扣率
                bill.DaType = null;
                bill.IsForce = null;
                bill.Profileid = null;
                bill.CardNo = null;
                billService.Update(bill, null);
                billService.AddDataChangeLog(OpLogType.Pos账单修改);
                billService.Commit();

                AddOperationLog(OpLogType.Pos账单修改, "折扣率：" + oldDiscount + "-->" + bill.Discount + "，折扣金额：" + oldDiscAmount + "-->" + bill.DiscAmount, bill.BillNo);

                var billDetailService = GetService<IPosBillDetailService>();
                var billDetailList = billDetailService.GetBillDetailByDcFlag(CurrentInfo.HotelId, billId, "D");
                foreach (var billDetail in billDetailList)
                {
                    var newBillDetail = new PosBillDetail();
                    AutoSetValueHelper.SetValues(billDetail, newBillDetail);
                    newBillDetail.Price = billDetail.PriceOri != null ? billDetail.PriceOri : billDetail.Price;  //价格恢复为原价
                    newBillDetail.Discount = 1;    //折扣率
                    newBillDetail.DiscAmount = 0;  //折扣金额
                    newBillDetail.IsDishDisc = null;
                    billDetailService.Update(newBillDetail, billDetail);
                    //    billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                    billDetailService.Commit();
                    AddOperationLog(OpLogType.Pos账单修改, "单号：" + newBillDetail.Id + "，折扣率：" + billDetail.Discount + "-->" + newBillDetail.Discount + "，折扣金额：" + billDetail.DiscAmount + "-->" + newBillDetail.DiscAmount, bill.BillNo);

                }
                billDetailService.UpdateBillDetailDisc(CurrentInfo.HotelId, billId, billId);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
            return Json(JsonResultData.Successed());
        }




        #endregion 取消折扣

        #region 修改服务费

        /// <summary>
        /// 视图
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.IntegralAdjustment)]
        public PartialViewResult _NumberInputByServiceRate(LetterInputViewModel model)
        {
            ViewBag.Version = CurrentVersion;
            return PartialView("_NumberInput", model);
        }

        /// <summary>
        /// 修改服务费率
        /// </summary>
        /// <param name="billId">账单ID</param>
        /// <param name="ServiceRate">服务费率</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult UpdateServiceRate(string billId, string ServiceRate)
        {
            var service = GetService<IPosBillService>();
            var bill = service.Get(billId);
            if (bill != null)
            {
                if (bill.Status == (byte)PosBillStatus.结账 || bill.Status == (byte)PosBillStatus.清台)
                {
                    return Json(JsonResultData.Failure("结账，清台的账单不能修改服务费"));
                }

                else
                {
                    try
                    {
                        var newBillModel = new PosBill();
                        AutoSetValueHelper.SetValues(bill, newBillModel);
                        decimal serviceRate = decimal.Parse(ServiceRate);
                        //统一转换百分比的小数
                        serviceRate = serviceRate > 1 ? serviceRate / 100 : serviceRate;
                        newBillModel.ServiceRate = serviceRate;
                        newBillModel.IsService = true;
                        service.Update(newBillModel, bill);
                        //service.AddDataChangeLog(OpLogType.Pos账单修改);
                        service.Commit();
                        AddOperationLog(OpLogType.Pos账单修改, "服务费率：" + bill.ServiceRate + "-->" + newBillModel.ServiceRate, bill.BillNo);

                        return Json(JsonResultData.Successed());
                    }
                    catch (Exception ex)
                    {
                        return Json(JsonResultData.Failure(ex.Message.ToString()));
                    }
                }
            }

            return Json(JsonResultData.Failure("操作出错"));
        }

        #endregion 修改服务费

        #region 并台

        [AuthButton(AuthFlag.UpdateRecord)]
        public ActionResult _MergeTab(string Flag, string billDetailId)
        {
            int PageIndex = 1;
            int PageSize = 22;
            var service = GetService<IPosTabStatusService>();

            ViewBag.PageIndex = PageIndex;
            ViewBag.PageSize = PageSize;

            ViewBag.Flag = Flag;
            ViewBag.billDetailId = billDetailId;

            var Service = GetService<IPosRefeService>();
            var refeList = Service.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);

            return PartialView("_MergeTab", refeList);
        }

        /// <summary>
        /// 餐台列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _TabStatusListByMergeTab(TabStatusViewModel model)
        {
            if (model != null)
            {
                model.Refeid = model.Refeid ?? "";
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

                var service = GetService<IPosTabStatusService>();
                var list = service.GetPosTabStatusResult(CurrentInfo.HotelId, model.Tabid, model.Refeid, "", null, model.PageIndex, model.PageSize);

                return PartialView("_TabStatusListByMergeTab", list);
            }
            return PartialView("_TabStatusListByMergeTab");
        }

        /// <summary>
        /// 选择餐台类型视图
        /// </summary>
        /// 账单json 字符串
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _PosSmearList(string model)
        {
            return PartialView("_PosSmearList", JsonConvert.DeserializeObject<List<PosBill>>(model));
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult MergeTab(string billId, string newBillId, string newTabId)
        {
            var tabService = GetService<IPosTabService>();

            var billService = GetService<IPosBillService>();

            var billDetailService = GetService<IPosBillDetailService>();

            //源数据
            var billdetailList = billDetailService.GetBillDetailByDcFlag(CurrentInfo.HotelId, billId).Where(w => (w.Isauto != 4 && w.Isauto != 5 && w.Isauto != 6)).ToList();
            var oldBill = billService.Get(billId);
            //目标数据
            PosBill bill = null;
            if (billdetailList == null || billdetailList.Count <= 0)
            {
                return Json(JsonResultData.Failure("没有要操作的数据！"));
            }

            if (string.IsNullOrEmpty(newBillId))
            {
                //没有抹台数据，用台号查询账单并台
                var tab = tabService.Get(newTabId);

                //to并台的数据
                var billResult = billService.GetPosBillByTabId(CurrentInfo.HotelId, tab.Refeid, newTabId);

                bill = billService.Get(billResult.Billid);
                // AutoSetValueHelper.SetValues(billResult, bill);
            }
            else
            {
                //有抹台数据用账单ID 并台
                bill = billService.Get(newBillId);
            }
            try
            {
                decimal amount = 0;
                var billRow = "";
                //循环修改账单明细数据主账单ID
                foreach (var billDetail in billdetailList)
                {
                    if (billDetail.Status == (byte)PosBillDetailStatus.正常 || billDetail.Status == (byte)PosBillDetailStatus.保存)
                    {
                        //计算本次转的金额
                        amount += billDetail.Amount == null ? 0 : Convert.ToDecimal(billDetail.Amount);
                    }
                    billRow += billDetail.Id + ",";
                    //源数据的账单ID 改成目标数据的账单ID
                    billDetail.Billid = bill.Billid;
                    billDetail.MBillid = bill.Billid;
                    billDetail.Memo = CurrentInfo.UserName + "并台";
                    billDetailService.Update(billDetail, new PosBillDetail());
                    billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                    billDetailService.Commit();

                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，账单ID：" + oldBill.Billid + "-->" + bill.Billid, oldBill.BillNo);
                    AddOperationLog(OpLogType.Pos账单消费明细增加, "并台：" + billRow, bill.BillNo);
                }

                #region 操作账单记录

                var billChange = new PosBillChange
                {
                    Id = Guid.NewGuid(),
                    Hid = CurrentInfo.HotelId,
                    BillBsnsDate = bill.BillBsnsDate,
                    Refeid = bill.Refeid,
                    Tabid = oldBill.Tabid,
                    MBillid = oldBill.MBillid,
                    NmBillid = bill.MBillid,
                    Nrefeid = bill.Refeid,
                    Ntabid = bill.Tabid,
                    iStatus = (byte)PosBillChangeStatus.并台,
                    Amount = amount,
                    BillRow = billRow,
                    Module = "CY",
                    TransUser = CurrentInfo.UserName,
                    CreateDate = DateTime.Now
                };

                var billChangeService = GetService<IPosBillChangeService>();
                billChangeService.Add(billChange);
                billChangeService.AddDataChangeLog(OpLogType.Pos账单操作记录增加);
                billChangeService.Commit();

                #endregion 操作账单记录

                //修改主账单状态
                var oldStatus = oldBill.Status;

                oldBill.Status = (byte)PosBillStatus.取消;
                oldBill.MoveUser = CurrentInfo.UserName;

                billService.Update(oldBill, new PosBill());
                //billService.AddDataChangeLog(OpLogType.Pos账单修改);
                billService.Commit();
                AddOperationLog(OpLogType.Pos账单修改, "并台状态：" + oldStatus + "-->" + oldBill.Status, oldBill.BillNo);

                var iguest = bill.IGuest;
                bill.IGuest += oldBill.IGuest;
                billService.Update(bill, new PosBill());
                billService.AddDataChangeLog(OpLogType.Pos账单修改);
                billService.Commit();
                AddOperationLog(OpLogType.Pos账单修改, "并台人数：" + iguest + "-->" + bill.IGuest, bill.BillNo);

                var statusService = GetService<IPosTabStatusService>();
                var tab = statusService.Get(bill.Tabid);
                tab.OpenGuest += oldBill.IGuest;
                statusService.Update(tab, new PosTabStatus());
                statusService.AddDataChangeLog(OpLogType.Pos餐台状态修改);
                statusService.Commit();
                //修改餐台状态
                if (oldBill.Tabid != newTabId && !string.IsNullOrEmpty(newTabId))
                {
                    PosCommon common = new PosCommon();
                    common.SetTabStatus(oldBill);
                }

                //清理锁台记录
                var tabLogService = GetService<IPosTabLogService>();
                var tabLogList = tabLogService.GetPosTabLogListByTab(CurrentInfo.HotelId, oldBill.Refeid, oldBill.Tabid, oldBill.TabNo);
                if (tabLogList != null && tabLogList.Count > 0)
                {
                    foreach (var tabLog in tabLogList)
                    {
                        if (tabLog.Billid == oldBill.Billid)
                        {
                            tabLogService.Delete(tabLog);
                            tabLogService.AddDataChangeLog(OpLogType.Pos锁台删除);
                            tabLogService.Commit();
                        }
                    }
                }
                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
        }

        #endregion 并台

        #region 整单取消

        /// <summary>
        /// 整单取消
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.transactionRecord)]
        public PartialViewResult _ReasonListByAll(PosReasonViewModel model)
        {
            if (model != null)
            {
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

                var service = GetService<IPosReasonService>();
                var list = service.GetPosReasonByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode, model.Istagtype, model.PageIndex, model.PageSize);

                ViewBag.pageIndex = model.PageIndex;
                return PartialView("_ReasonListByAll", list);
            }
            return PartialView("_ReasonListByAll", new List<PosReason>());
        }

        /// <summary>
        /// 整单取消
        /// </summary>
        /// <param name="billId">账单ID</param>
        /// <param name="canReason">取消原因</param>
        /// <param name="isreuse">是否加回库存</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult CancelAllItemByNum(string billId, string canReason, string isreuse)
        {
            //账单服务
            var billService = GetService<IPosBillService>();

            //账单明细服务
            var billDetailService = GetService<IPosBillDetailService>();

            //账单
            var bill = billService.Get(billId);
            var oldBill = new PosBill();
            AutoSetValueHelper.SetValues(bill, oldBill);

            //判断是否有付款信息
            var billDetailList = billDetailService.GetBillDetailByDcFlag(CurrentInfo.HotelId, billId);
            if (billDetailList != null || billDetailList.Count > 0)
            {
                //判断是否有付款信息
                var billDetailListByC = billDetailList.Where(m => m.DcFlag == "C").ToList();
                if (billDetailListByC.Count > 0)
                {
                    return Json(JsonResultData.Failure("已存在付款信息，不能整单取消"));
                }
            }
            try
            {
                //修改账单明细状态
                foreach (var billDetail in billDetailList)
                {
                    var oldbillDetail = new PosBillDetail();
                    AutoSetValueHelper.SetValues(billDetail, oldbillDetail);
                    if (billDetail.Status == (byte)PosBillDetailStatus.保存)
                    {
                        billDetail.Status = (byte)PosBillDetailStatus.未落单取消;
                    }
                    else
                    {
                        if (Convert.ToBoolean(isreuse))
                        {
                            billDetail.Status = (byte)PosBillDetailStatus.加回库存取消;
                        }
                        else if (Convert.ToBoolean(isreuse) == false)
                        {
                            billDetail.Status = (byte)PosBillDetailStatus.不加回库取消;
                        }
                    }

                    billDetailService.Update(billDetail, new PosBillDetail());
                    // billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                    billDetailService.Commit();
                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，状态：" + oldbillDetail.Status + "-->" + billDetail.Status, bill.BillNo);
                }
                //修改账单状态
                bill.Memo = canReason;
                bill.Status = (byte)PosBillStatus.取消;
                billService.Update(bill, new PosBill());
                //billService.AddDataChangeLog(OpLogType.Pos账单修改);
                billService.Commit();
                AddOperationLog(OpLogType.Pos账单修改, "状态：" + oldBill.Status + "-->" + bill.Status, bill.BillNo);

                var statusService = GetService<IPosTabStatusService>();
                //修改餐台状态
                PosCommon common = new PosCommon();
                common.SetTabStatus(bill);

                //清理锁台记录
                var tabLogService = GetService<IPosTabLogService>();
                var tabLogList = tabLogService.GetPosTabLogListByTab(CurrentInfo.HotelId, bill.Refeid, bill.Tabid, bill.TabNo);
                if (tabLogList != null && tabLogList.Count > 0)
                {
                    foreach (var tabLog in tabLogList)
                    {
                        if (tabLog.Billid == bill.Billid)
                        {
                            tabLogService.Delete(tabLog);
                            tabLogService.AddDataChangeLog(OpLogType.Pos锁台删除);
                            tabLogService.Commit();
                        }
                    }
                }

                //打印取消单
                var orderlist = new List<up_pos_print_billOrderResult>();
                var refe = Session["PosRefe"] as PosRefe;
                if (refe.IPrintBillss == PosPrintBillss.打印)
                {
                    orderlist = billDetailService.GetBillOrderByPrint(CurrentInfo.HotelId, bill.Billid, bill.MBillid).Where(w => w.计费状态 > 50 && w.计费状态 < (byte)PosBillDetailStatus.未落单取消 && Convert.ToInt32(w.点菜单打单次数) == 1).ToList();
                    var list = billDetailService.GetBillDetailByBillidAndStatus(CurrentInfo.HotelId, bill.Billid, PosItemDcFlag.D.ToString(), (byte)PosBillDetailStatus.未落单取消);
                    foreach (var temp in list)
                    {
                        foreach (var order in orderlist)
                        {
                            if (temp.Id == order.id)
                            {
                                var billDetailNew = new PosBillDetail();
                                AutoSetValueHelper.SetValues(temp, billDetailNew);
                                byte iOrderPrint = temp.iOrderPrint ?? 0;
                                billDetailNew.iOrderPrint = Convert.ToByte(iOrderPrint + 1);
                                billDetailService.Update(billDetailNew, new PosBillDetail());
                                billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                                billDetailService.Commit();
                                break;
                            }
                        }
                    }
                }

                return Json(JsonResultData.Successed(orderlist));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
        }

        #endregion 整单取消

        #region 转菜

        [AuthButton(AuthFlag.consumptionRecord)]
        public ActionResult _MergeTabB(string Flag, string billDetailId)
        {
            int PageIndex = 1;
            int PageSize = 22;
            var service = GetService<IPosTabStatusService>();

            ViewBag.PageIndex = PageIndex;
            ViewBag.PageSize = PageSize;

            ViewBag.Flag = Flag;
            ViewBag.billDetailId = billDetailId;

            var Service = GetService<IPosRefeService>();
            var refeList = Service.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);

            return PartialView("_MergeTab", refeList);
        }

        /// <summary>
        /// 服务费率等餐台资料
        /// </summary>
        /// <param name="newTabId">新餐台ID</param>
        /// <param name="billId">当前账单ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _TabService(string newTabId, string billId)
        {
            var billService = GetService<IPosBillService>();
            var bill = billService.GetPosBillByBillid(CurrentInfo.HotelId, billId);

            ViewBag.oldModel = bill;    //传参到界面

            ChangeTableModel result = new ChangeTableModel();
            //餐台信息
            try
            {
                var posTabService = GetService<IPosTabService>();
                var posTab = posTabService.GetEntity(CurrentInfo.HotelId, newTabId);

                DateTime openTime = DateTime.Now;

                var posTabServiceService = GetService<IPosTabServiceService>();
                var posTabServiceModel = posTabServiceService.GetPosTabService(CurrentInfo.HotelId, CurrentInfo.ModuleCode, posTab.Refeid, posTab.TabTypeid, null, (byte)PosITagperiod.随时, openTime);
                if (posTabServiceModel != null)
                {
                    result = new ChangeTableModel()
                    {
                        newServiceRate = posTabServiceModel.Servicerate == null ? 0 : posTabServiceModel.Servicerate,
                        newLimit = posTabServiceModel.NLimit == null ? 0 : posTabServiceModel.NLimit,
                        newTabName = posTab.Cname,
                        newTabId = newTabId,
                        newTabNo = posTab.TabNo,
                        newRefeId = posTab.Refeid
                    };
                    //   ViewBag.newTab = result;
                }
            }
            catch (Exception ex)
            {
                return PartialView("_TabService", ex.Message.ToString());
                //  throw;
            }
            return PartialView("_TabService", result);
        }

        /// <summary>
        /// 转菜界面
        /// </summary>
        /// <param name="newTabId">新餐台ID</param>
        /// <param name="billId">当前账单ID</param>
        /// <param name="newBillId">新账单ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PosBillDetailListByMerge(string newTabId, string billId, string newBillId)
        {
            var billService = GetService<IPosBillService>();
            var bill = billService.GetPosBillByBillid(CurrentInfo.HotelId, billId);

            ViewBag.oldModel = bill;    //传参到界面

            ChangeTableModel result = new ChangeTableModel();
            //餐台信息
            try
            {
                var posTabService = GetService<IPosTabService>();
                var posTab = posTabService.GetEntity(CurrentInfo.HotelId, newTabId);

                DateTime openTime = DateTime.Now;
                var posTabServiceService = GetService<IPosTabServiceService>();
                var posTabServiceModel = posTabServiceService.GetPosTabService(CurrentInfo.HotelId, CurrentInfo.ModuleCode, posTab.Refeid, posTab.TabTypeid, null, (byte)PosITagperiod.随时, openTime);

                var Newbill = billService.GetPosBillByTabId(CurrentInfo.HotelId, posTab.Refeid, newTabId, newBillId ?? "");// Get(newBillId);
                if (Newbill != null)//已经有账单的餐台
                {
                    result = new ChangeTableModel()
                    {
                        newServiceRate = posTabServiceModel.Servicerate == null ? 0 : posTabServiceModel.Servicerate,
                        newLimit = posTabServiceModel.NLimit == null ? 0 : posTabServiceModel.NLimit,
                        newTabName = posTab.Cname,
                        newTabId = newTabId,
                        newTabNo = posTab.TabNo,
                        newRefeId = posTab.Refeid,
                        oldBillId = billId,
                        newBillId = Newbill.Billid
                    };
                }
                else
                {
                    if (posTabServiceModel != null)
                    {
                        result = new ChangeTableModel()
                        {
                            newServiceRate = posTabServiceModel.Servicerate == null ? 0 : posTabServiceModel.Servicerate,
                            newLimit = posTabServiceModel.NLimit == null ? 0 : posTabServiceModel.NLimit,
                            newTabName = posTab.Cname,
                            newTabId = newTabId,
                            newTabNo = posTab.TabNo,
                            newRefeId = posTab.Refeid,
                            oldBillId = billId
                        };
                        //   ViewBag.newTab = result;
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("_PosBillDetailListByMerge", ex.Message.ToString());
                //  throw;
            }
            return PartialView("_PosBillDetailListByMerge", result);
        }

        /// <summary>
        /// 抹台列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _PosSmearListByChangeItem(string model)
        {
            return PartialView("_PosSmearListByChangeItem", JsonConvert.DeserializeObject<List<PosBill>>(model));
        }

        /// <summary>
        /// 转菜
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ChangeItemByMerge(ChangeTableModel model)
        {
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var billService = GetService<IPosBillService>();
            var oldBill = billService.Get(model.oldBillId);//当前账单信息

            var posTabServiceService = GetService<IPosTabServiceService>();

            var posTabService = GetService<IPosTabService>();//餐台服务
            var tab = posTabService.Get(model.newTabId);

            var refeSrvice = GetService<IPosRefeService>();
            var refe = refeSrvice.Get(model.newRefeId);

            var billDetailService = GetService<IPosBillDetailService>();

            string billDetailIdList = "";    //本次更改的数据ID 集合
            decimal outAmount = 0;//本次更改的金额
            if (string.IsNullOrEmpty(model.newBillId))
            {
                //获取新的账单ID
                var posBill = billService.GetLastBillId(CurrentInfo.HotelId, model.newRefeId, pos.Business);

                bool isexsit = billService.IsExists(CurrentInfo.HotelId, posBill.Billid, posBill.BillNo);
                if (isexsit)
                {
                    posBill = billService.GetLastBillId(CurrentInfo.HotelId, model.newRefeId, pos.Business);
                }

                #region 需要转的消费项目

                try
                {
                    #region 新账单处理

                    var posTabServiceModel = posTabServiceService.GetPosTabService(CurrentInfo.HotelId, CurrentInfo.ModuleCode, model.newRefeId, tab.TabTypeid, null, (byte)PosITagperiod.随时, DateTime.Now);
                    PosBill bill = new PosBill()
                    {
                        Hid = CurrentInfo.HotelId,
                        Billid = posBill.Billid,
                        BillNo = posBill.BillNo,
                        MBillid = posBill.Billid,
                        InputUser = CurrentInfo.UserName,
                        BillDate = DateTime.Now,
                        IsService = true,
                        IsLimit = true,

                        ServiceRate = model.ServiceRateFlag == "1" ? oldBill.ServiceRate : model.newServiceRate,
                        Limit = model.ServiceRateFlag == "1" ? oldBill.Limit : model.newLimit,

                        IsByPerson = posTabServiceModel.IsByPerson == 0 ? false : true,
                        IHour = posTabServiceModel.LimitTime,
                        Discount = posTabServiceModel.Discount,

                        TabFlag = (byte)PosBillTabFlag.物理台,
                        Status = (byte)PosBillStatus.开台,
                        Shiftid = pos.ShiftId,
                        Shuffleid = refe.ShuffleId,
                        Refeid = model.newRefeId,
                        BillBsnsDate = pos.Business,
                        IGuest = oldBill.IGuest,
                        Name = oldBill.Name,
                        Tabid = model.newTabId,
                        TabNo = tab.TabNo
                    };

                    billService.Add(bill);
                    billService.AddDataChangeLog(OpLogType.Pos账单增加);
                    billService.Commit();
                    AddOperationLog(OpLogType.Pos账单增加, "转菜开台--营业点：" + bill.Tabid + "，餐台：" + bill.Tabid, bill.BillNo);

                    PosTabStatus tabStatus = new PosTabStatus() { Tabid = model.newTabId, TabStatus = (byte)PosTabStatusEnum.就座, OpTabid = bill.Billid, OpenRecord = DateTime.Now, OpenGuest = bill.IGuest };
                    updateTabStatus(tabStatus);

                    //AddTabLog(model.newTabId, tab.TabNo, bill.Billid, model.ComputerName, refe);

                    #endregion 新账单处理

                    #endregion 需要转的消费项目

                    #region 新账单开台项目处理

                    if (model.ItemFlag == "2")    //新台的开台项目
                    {
                        var itemService = GetService<IPosItemService>();

                        var openItemService = GetService<IPosTabOpenItemService>();
                        var openItemList = openItemService.GetPosTabOpenItemByTabType(CurrentInfo.HotelId, tab.Module, tab.Refeid, tab.TabTypeid, "", (byte)PosITagperiod.随时, DateTime.Now);
                        foreach (var temp in openItemList)
                        {
                            if (temp.QuanMode == 1) //数量按人计
                            {
                                temp.Quantity = temp.Quantity * Convert.ToDecimal(bill.IGuest);
                            }

                            var item = itemService.GetEntity(CurrentInfo.HotelId, temp.Itemid);
                            var amount = item.IsDiscount == true ? (temp.Price * temp.Quantity) * bill.Discount : (temp.Price * temp.Quantity);
                            var server = item.IsService == true ? (temp.Price * temp.Quantity * bill.ServiceRate) : 0;

                            //判断开台项目的收费状态
                            var status = (byte)PosBillDetailStatus.保存;
                            if (temp.IsCharge == 0)
                            {
                                status = (byte)PosBillDetailStatus.赠送;
                            }
                            else if (temp.IsCharge == 1)
                            {
                                status = (byte)PosBillDetailStatus.例送;
                            }
                            else
                            {
                                status = (byte)PosBillDetailStatus.保存;
                            }

                            #region 消费项目明细赋值

                            PosBillDetail billDetail = new PosBillDetail()
                            {
                                MBillid = bill.MBillid,
                                Billid = bill.Billid,
                                Price = temp.Price,
                                Quantity = temp.Quantity,
                                Tabid = tab.Id,

                                Hid = CurrentInfo.HotelId,
                                Itemid = temp.Itemid,
                                ItemCode = temp.itemCode,
                                ItemName = temp.itemName,
                                Unitid = temp.Unitid,
                                UnitCode = temp.unitCode,
                                UnitName = temp.unitName,
                                DcFlag = PosItemDcFlag.D.ToString(),

                                IsCheck = false,
                                Isauto = (byte)PosBillDetailIsauto.开台项目,
                                Status = status,
                                //     IsProduce = (byte)PosBillDetailIsProduce.未出品,
                                Dueamount = temp.Price * temp.Quantity,
                                DiscAmount = 0,
                                Discount = item.IsDiscount == true ? bill.Discount : 100,
                                Amount = amount,
                                //   Service = server,

                                SP = false,
                                SD = false,
                                TransUser = CurrentInfo.UserName,
                                TransBsnsDate = bill.BillBsnsDate,
                                TransShiftid = bill.Shiftid,
                                TransShuffleid = bill.Shuffleid,
                                TransDate = DateTime.Now,
                            };

                            billDetailService.Add(billDetail);
                            // billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细增加);
                            billDetailService.Commit();
                            AddOperationLog(OpLogType.Pos账单消费明细增加, "转菜--名称：" + billDetail.ItemName + "，数量：" + billDetail.Quantity + "，金额" + billDetail.Dueamount + "，类型：" + billDetail.Isauto + "，状态：" + billDetail.Status, bill.BillNo);
                        }

                        #endregion 消费项目明细赋值
                    }

                    #endregion 新账单开台项目处理

                    bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount;

                    ChangeItemNum(model.ChangeItemJson, bill, oldBill, out billDetailIdList, out outAmount);

                    #region 账单操作记录表

                    var billChange = new PosBillChange
                    {
                        Id = Guid.NewGuid(),
                        Hid = CurrentInfo.HotelId,
                        BillBsnsDate = oldBill.BillBsnsDate,
                        Refeid = oldBill.Refeid,
                        Tabid = oldBill.Tabid,
                        MBillid = oldBill.MBillid,
                        NmBillid = bill.MBillid,
                        Nrefeid = bill.Refeid,
                        Ntabid = bill.Tabid,
                        iStatus = (byte)PosBillChangeStatus.转菜,
                        Amount = outAmount,
                        BillRow = billDetailIdList,
                        Module = "CY",
                        TransUser = CurrentInfo.UserName,
                        CreateDate = DateTime.Now
                    };

                    var billChangeService = GetService<IPosBillChangeService>();
                    billChangeService.Add(billChange);
                    billChangeService.AddDataChangeLog(OpLogType.Pos账单操作记录增加);
                    billChangeService.Commit();

                    #endregion 账单操作记录表

                    var ListDetail = billDetailService.GetDataSetByChangeItem(CurrentInfo.HotelId, oldBill.Tabid, billDetailIdList).ToList();

                    var serializer = new JavaScriptSerializer();
                    var valueStr = ReplaceJsonDateToDateString(serializer.Serialize(ListDetail));
                    return Json(JsonResultData.Successed(valueStr));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex.Message.ToString()));
                }


            }
            else
            {
                //已存在账单的 转菜
                var newbill = billService.Get(model.newBillId);
                try
                {
                    newbill.Discount = newbill.Discount >= 1 && newbill.Discount <= 100 ? newbill.Discount / 100 : newbill.Discount;

                    ChangeItemNum(model.ChangeItemJson, newbill, oldBill, out billDetailIdList, out outAmount);

                    #region 账单操作记录表

                    var billChange = new PosBillChange
                    {
                        Id = Guid.NewGuid(),
                        Hid = CurrentInfo.HotelId,
                        BillBsnsDate = oldBill.BillBsnsDate,
                        Refeid = oldBill.Refeid,
                        Tabid = oldBill.Tabid,
                        MBillid = oldBill.MBillid,
                        NmBillid = newbill.MBillid,
                        Nrefeid = newbill.Refeid,
                        Ntabid = newbill.Tabid,
                        iStatus = (byte)PosBillChangeStatus.转菜,
                        Amount = outAmount,
                        BillRow = billDetailIdList,
                        Module = "CY",
                        TransUser = CurrentInfo.UserName,
                        CreateDate = DateTime.Now
                    };

                    var billChangeService = GetService<IPosBillChangeService>();
                    billChangeService.Add(billChange);
                    billChangeService.AddDataChangeLog(OpLogType.Pos账单操作记录增加);
                    billChangeService.Commit();

                    #endregion 账单操作记录表

                    var ListDetail = billDetailService.GetDataSetByChangeItem(CurrentInfo.HotelId, oldBill.Tabid, billDetailIdList).ToList();
                    var serializer = new JavaScriptSerializer();
                    var valueStr = ReplaceJsonDateToDateString(serializer.Serialize(ListDetail));
                    return Json(JsonResultData.Successed(valueStr));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex.Message.ToString()));
                }
            }

            //  return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 处理消费明细
        /// </summary>
        /// <param name="itemJsonStr"></param>
        private void ChangeItemNum(string itemJsonStr, PosBill bill, PosBill oldBill, out string billDetailIdList, out decimal outAmount)
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();

            List<ChangeItem> BillDetailList = Serializer.Deserialize<List<ChangeItem>>(itemJsonStr);

            var service = GetService<IPosBillDetailService>();  //账单明细

            var suitService = GetService<IPosItemSuitService>();    //套餐

            var itemService = GetService<IPosItemService>();    //消费项目

            var billDetailActionService = GetService<IPosBillDetailActionService>();//账单明细作法

            var actionService = GetService<IPosActionService>();    //作法

            billDetailIdList = "";
            outAmount = 0;
            foreach (var item in BillDetailList)
            {
                //处理新增数据
                var model = service.Get(int.Parse(item.Id));
                var newModel = new PosBillDetail();
                AutoSetValueHelper.SetValues(model, newModel);
                var quantity = item.Num == null ? model.Quantity : item.Num;
                if (quantity == model.Quantity)//如果数量相同 则修改账单明细的BillId
                {
                    if (model.SP == true && model.SD == false)//套餐
                    {
                        var tcList = service.GetBillDetailByDcFlag(CurrentInfo.HotelId, model.Billid, PosItemDcFlag.D.ToString()).Where(m => m.Upid == model.Upid);
                        foreach (var tcBillDetail in tcList)
                        {
                            if (tcBillDetail.Id != model.Id)
                            {
                                var newTcBillDetail = new PosBillDetail();
                                AutoSetValueHelper.SetValues(tcBillDetail, newTcBillDetail);
                                newTcBillDetail.Billid = bill.Billid;
                                newTcBillDetail.MBillid = bill.Billid;
                                newTcBillDetail.TransUser = CurrentInfo.UserName;
                                newTcBillDetail.TransBsnsDate = bill.BillBsnsDate;
                                newTcBillDetail.TransShiftid = bill.Shiftid;
                                newTcBillDetail.TransShuffleid = bill.Shuffleid;
                                newTcBillDetail.TransDate = DateTime.Now;

                                service.Update(newTcBillDetail, tcBillDetail);
                                service.Commit();
                                AddOperationLog(OpLogType.Pos账单消费明细增加, "转菜--单号：" + newTcBillDetail.Id + "，账单：" + oldBill.Billid + "-->" + newTcBillDetail.Billid, oldBill.BillNo);
                                billDetailIdList += newTcBillDetail.Id + ",";
                            }
                        }
                    }
                    newModel.Billid = bill.Billid;
                    newModel.MBillid = bill.Billid;
                    newModel.TransUser = CurrentInfo.UserName;
                    newModel.TransBsnsDate = bill.BillBsnsDate;
                    newModel.TransShiftid = bill.Shiftid;
                    newModel.TransShuffleid = bill.Shuffleid;
                    newModel.TransDate = DateTime.Now;
                    service.Update(newModel, model);
                    // service.AddDataChangeLog(OpLogType.Pos消费项目修改);
                    service.Commit();
                    AddOperationLog(OpLogType.Pos账单消费明细增加, "转菜--单号：" + newModel.Id + "，账单：" + oldBill.Billid + "-->" + newModel.Billid, oldBill.BillNo);
                    billDetailIdList += newModel.Id + ",";
                    outAmount += Convert.ToDecimal(newModel.Amount);

                }
                else
                {
                    var guid = new Guid();
                    var dueamount = quantity * model.Price;
                    var posItem = itemService.Get(model.Itemid);

                    var amount = posItem.IsDiscount == true ? dueamount * bill.Discount : dueamount;

                    #region 账单明细赋值

                    newModel.Quantity = quantity;
                    newModel.Billid = bill.Billid;
                    newModel.MBillid = bill.Billid;
                    newModel.Dueamount = dueamount;
                    newModel.TransUser = CurrentInfo.UserName;
                    newModel.TransBsnsDate = bill.BillBsnsDate;
                    newModel.TransShiftid = bill.Shiftid;
                    newModel.TransShuffleid = bill.Shuffleid;
                    newModel.TransDate = DateTime.Now;
                    newModel.Discount = bill.Discount;
                    newModel.Amount = amount;
                    if (model.SP == true && model.SD == false)
                    {
                        newModel.Upid = guid;
                    }

                    service.Add(newModel);
                    //service.AddDataChangeLog(OpLogType.Pos消费项目增加);
                    service.Commit();
                    AddOperationLog(OpLogType.Pos账单消费明细增加, "转菜--名称：" + newModel.ItemName + "，数量：" + newModel.Quantity, bill.BillNo);

                    #endregion 账单明细赋值

                    //复制作法
                    var actionList = billDetailActionService.GetBillDetailActionByMid(CurrentInfo.HotelId, model.MBillid, model.Id);
                    foreach (var billDetailAction in actionList)
                    {
                        var newAction = new PosBillDetailAction();
                        AutoSetValueHelper.SetValues(billDetailAction, newAction);
                        newAction.MBillid = bill.Billid;
                        newAction.Mid = newModel.Id;
                        billDetailActionService.Add(newAction);
                        // billDetailActionService.AddDataChangeLog(OpLogType.Pos账单作法明细增加);
                        billDetailActionService.Commit();
                        AddOperationLog(OpLogType.Pos账单作法明细增加, "转菜--名称：" + newAction.ActionName + "，金额：" + newAction.Amount, bill.BillNo);
                    }
                    var oldQuantity = model.Quantity;//用于记录转菜前的数量
                    var olddueanmount = model.Dueamount;//用于记录转菜前的金额
                    model.Quantity -= quantity;
                    model.Dueamount = model.Quantity * model.Price;
                    //service.Update(model, new PosBillDetail());
                    ////  service.AddDataChangeLog(OpLogType.Pos消费项目修改);
                    //service.Commit();


                    #region 处理作法金额

                    //当前操作的账单
                    actionList = billDetailActionService.GetBillDetailActionByMid(CurrentInfo.HotelId, model.MBillid, model.Id);
                    decimal actionAmount = 0;
                    foreach (var billDetailAction in actionList)
                    {
                        var action = actionService.GetActionByCode(CurrentInfo.HotelId, billDetailAction.ActionNo, billDetailAction.ActionName);

                        if (action != null && action.AddPrice != null && action.AddPrice > 0)
                        {
                            if (action.IsByQuan != null && action.IsByQuan.Value && action.IsByGuest != null && action.IsByGuest.Value)
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(model.Quantity) * Convert.ToDecimal(oldBill.IGuest);
                            }
                            else if (action.IsByQuan != null && action.IsByQuan.Value)
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(model.Quantity);
                            }
                            else if (action.IsByGuest != null && action.IsByGuest.Value)
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(oldBill.IGuest);
                            }
                            else
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice);
                            }
                        }
                    }

                    model.AddPrice = actionAmount;
                    model.Dueamount = model.Price * model.Quantity + actionAmount;
                    service.Update(model, new PosBillDetail());
                    service.Commit();
                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + model.Id + "，数量" + oldQuantity + "-->" + model.Quantity + "，金额：" + dueamount + "-->" + model.Dueamount, oldBill.BillNo);

                    //  新账单作法金额计算
                    actionList = billDetailActionService.GetBillDetailActionByMid(CurrentInfo.HotelId, newModel.MBillid, newModel.Id);
                    actionAmount = 0;
                    foreach (var billDetailAction in actionList)
                    {
                        var action = actionService.GetActionByCode(CurrentInfo.HotelId, billDetailAction.ActionNo, billDetailAction.ActionName);

                        if (action != null && action.AddPrice != null && action.AddPrice > 0)
                        {
                            if (action.IsByQuan != null && action.IsByQuan.Value && action.IsByGuest != null && action.IsByGuest.Value)
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(newModel.Quantity) * Convert.ToDecimal(bill.IGuest);
                            }
                            else if (action.IsByQuan != null && action.IsByQuan.Value)
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(newModel.Quantity);
                            }
                            else if (action.IsByGuest != null && action.IsByGuest.Value)
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(bill.IGuest);
                            }
                            else
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice);
                            }
                        }
                    }

                    model.AddPrice = actionAmount;
                    newModel.Dueamount = newModel.Price * newModel.Quantity + actionAmount;
                    service.Update(newModel, new PosBillDetail());
                    service.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                    service.Commit();

                    #endregion 处理作法金额

                    //如果是套餐
                    if (model.SP == true && model.SD == false)
                    {
                        // newModel.Upid = new Guid();
                        //套餐明细
                        var tcList = service.GetBillDetailByDcFlag(CurrentInfo.HotelId, model.Billid, PosItemDcFlag.D.ToString()).Where(m => m.Upid == model.Upid);

                        foreach (var tcBillDetail in tcList)
                        {
                            if (tcBillDetail.Id != model.Id)
                            {
                                #region 账单赋值

                                //新账单数据
                                var newTcBillDetail = new PosBillDetail();
                                AutoSetValueHelper.SetValues(tcBillDetail, newTcBillDetail);
                                var suitList = suitService.GetPosItemSuitListByItemId(CurrentInfo.HotelId, model.Itemid).Where(m => m.ItemId2 == tcBillDetail.Itemid).FirstOrDefault();

                                newTcBillDetail.Quantity = suitList.Quantity * quantity;
                                newTcBillDetail.Dueamount = tcBillDetail.Quantity * tcBillDetail.Price;

                                newTcBillDetail.ModiDate = DateTime.Now;
                                newTcBillDetail.Amount = newTcBillDetail.Dueamount;
                                newTcBillDetail.Billid = bill.Billid;
                                newTcBillDetail.MBillid = bill.MBillid;
                                newTcBillDetail.Upid = guid;
                                service.Add(newTcBillDetail);
                                //service.AddDataChangeLog(OpLogType.Pos消费项目增加);
                                service.Commit();
                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newTcBillDetail.Id + "，数量：" + newTcBillDetail.Quantity + "，金额：" + newTcBillDetail.Dueamount, bill.BillNo);

                                //旧账单数据
                                var tcoldQuantity = tcBillDetail.Quantity;
                                var tcDueamount = tcBillDetail.Dueamount;

                                tcBillDetail.Quantity -= suitList.Quantity * quantity;
                                tcBillDetail.Dueamount = tcBillDetail.Quantity * tcBillDetail.Price;

                                tcBillDetail.ModiDate = DateTime.Now;
                                tcBillDetail.Amount = newTcBillDetail.Dueamount;

                                service.Update(tcBillDetail, new PosBillDetail());
                                //    service.AddDataChangeLog(OpLogType.Pos消费项目修改);
                                service.Commit();
                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + tcBillDetail.Id + "，数量" + tcoldQuantity + "-->" + tcBillDetail.Quantity + "，金额：" + tcDueamount + "-->" + tcBillDetail.Dueamount, oldBill.BillNo);
                                billDetailIdList += newTcBillDetail.Id + ",";

                                #endregion 账单赋值
                            }
                        }
                    }
                    billDetailIdList += newModel.Id + ",";
                    outAmount += Convert.ToDecimal(newModel.Amount);
                }
            }
        }

        private void updateTabStatus(PosTabStatus posTabStatus)
        {
            var tabStatusService = GetService<IPosTabStatusService>();
            var tabStatus = tabStatusService.Get(posTabStatus.Tabid);
            tabStatus.TabStatus = posTabStatus.TabStatus;
            tabStatus.OpTabid = posTabStatus.OpTabid;
            tabStatus.OpenRecord = posTabStatus.OpenRecord;
            tabStatus.GuestName = posTabStatus.GuestName;
            tabStatus.OpenGuest = posTabStatus.OpenGuest == null ? 1 : posTabStatus.OpenGuest;
            tabStatusService.Update(tabStatus, new PosTabStatus());
            tabStatusService.AddDataChangeLog(OpLogType.Pos餐台状态修改);
            tabStatusService.Commit();
        }

        /// <summary>
        /// 添加餐台记录
        /// </summary>
        /// <param name="tabId">餐台ID</param>
        /// <param name="tabNo">餐台代码</param>
        /// <param name="billId">账单ID</param>
        /// <param name="refe">营业点</param>
        private void AddTabLog(string tabId, string tabNo, string billId, string computerName, PosRefe refe)
        {
            #region 清理当前操作员之前的锁台记录

            var tabLogService = GetService<IPosTabLogService>();    //餐台记录

            string computer = computerName ?? "";
            var tabLogList = tabLogService.GetPosTabLogListByTab(CurrentInfo.HotelId, refe.Id, tabId, tabNo);
            if (tabLogList != null && tabLogList.Count > 0)
            {
                foreach (var tabLog in tabLogList)
                {
                    if (tabLog.Billid == billId)
                    {
                        tabLogService.Delete(tabLog);
                        tabLogService.AddDataChangeLog(OpLogType.Pos锁台删除);
                        tabLogService.Commit();
                    }
                }
            }

            #endregion 清理当前操作员之前的锁台记录

            #region 添加当前锁台记录

            PosTabLog posTabLog = new PosTabLog()
            {
                Id = Guid.NewGuid(),
                Hid = CurrentInfo.HotelId,
                Msg = string.Format($"{tabNo}餐台被{computer} => {CurrentInfo.UserName}在操作"),
                Status = (byte)PosTabLogStatus.开台自动锁台,
                Computer = computer,
                ConnectDate = DateTime.Now,
                Module = refe.Module,
                TransUser = CurrentInfo.UserName,
                CreateDate = DateTime.Now
            };
            posTabLog.Refeid = refe.Id;
            posTabLog.Billid = billId;
            posTabLog.Tabid = tabId;
            posTabLog.TabNo = tabNo;
            posTabLog.Refeid = refe.Id;
            //   AutoSetValueHelper.SetValues(model, posTabLog);
            tabLogService.Add(posTabLog);
            tabLogService.AddDataChangeLog(OpLogType.Pos锁台增加);
            tabLogService.Commit();

            #endregion 添加当前锁台记录
        }

        #endregion


        #region 取消最低消费
        [AuthButton(AuthFlag.CancelOrderDetailZ)]
        public ActionResult CancelMinConsume(string billid)
        {
            try
            {
                var billService = GetService<IPosBillService>();
                var bill = billService.Get(billid);
                if (bill.Status == (byte)PosBillStatus.结账 || bill.Status == (byte)PosBillStatus.清台)
                {
                    return Json(JsonResultData.Failure("已经买单，清台的账单不能修改"));
                }
                var newEntity = new PosBill();
                AutoSetValueHelper.SetValues(bill, newEntity);

                newEntity.IsLimit = false;

                billService.Update(newEntity, bill);
                billService.AddDataChangeLog(OpLogType.Pos账单修改);
                billService.Commit();

                AddOperationLog(OpLogType.Pos账单修改, "是否计最低消费：" + bill.IsLimit + "-->" + newEntity.IsLimit, bill.BillNo);

            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
            return Json(JsonResultData.Successed());
        }

        #endregion

        #endregion

        #region 买单直接打印

        /// <summary>
        /// 买单成功打印账单
        /// </summary>
        /// <param name="billId"></param>
        /// <param name="isPrintBill"></param>
        /// <param name="reportName"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult PrintByPay(string billId, string isPrintBill, string reportName = "PosBillPrint")
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "SRReports\\PosBillPrintByPay.mrt";

            var service = GetService<IPosBillDetailService>();
            string sql = "Exec up_pos_print_billDetail";
            SqlParameter[] para = {
                new SqlParameter("@h99hid",CurrentInfo.HotelId),
                new SqlParameter("@h99mBillid",billId),
                new SqlParameter("@h99billid",billId),
            };
            DataSet ds = service.GetDataSetByPayPrint(sql, para);

            var billService = GetService<IPosBillService>();

            var bill = billService.Get(billId);//获取账单信息
            var iprint = bill.IPrint;

            var refeService = GetService<IPosRefeService>();    //营业点

            PosRefe refe = refeService.Get(bill == null ? "0" : bill.Refeid);   //获取营业点
            if (refe.ITagPrintBill == PosTagPrintBill.提示是否买单 || (refe.ITagPrintBill == PosTagPrintBill.立即打印账单)
                || (refe.ITagPrintBill == PosTagPrintBill.提示是否打印账单 && isPrintBill == "1"))
            {
                bool result = PrintReport(filePath, ds.Tables[0], false, false);
                if (result)
                {
                    bill.IPrint = bill.IPrint == null ? 1 : bill.IPrint + 1;
                    billService.Update(bill, new PosBill());

                    AddOperationLog(OpLogType.Pos账单修改, "单号：" + bill.Billid + "，打单次数：" + iprint + "-->" + bill.IPrint, bill.BillNo);

                    billService.AddDataChangeLog(OpLogType.Pos账单修改);
                    billService.Commit();
                    return Json(JsonResultData.Successed());
                }
                else
                {
                    return Json(JsonResultData.Failure("账单打印失败！"));
                }
            }

            return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 打印本地报表
        /// </summary>
        /// <param name="FileName">报表文件绝对路径</param>
        /// <param name="dsSource">数据源</param>
        /// <param name="isPreview">是否预览，True 预览， false 不预览</param>
        /// <returns></returns>
        public bool PrintReport(string FileName, DataTable dt, bool showPrintDialog = false, bool isSupplement = false)
        {
            try
            {
                var s = dt.Rows.Count;
                //对数据集重命名
                StiReport report1 = new StiReport();
                report1.Load(FileName);
                report1.Compile();
                report1["UserName"] = CurrentInfo.UserName;
                report1["HotelName"] = CurrentInfo.HotelName;
                report1.ReportName = DateTime.Now.ToString("yyyyMMddHHmmssms");
                report1.RegData("up_pos_print_billDetail", dt);
                if (showPrintDialog == false) report1.PrinterSettings.ShowDialog = false;
                report1.Render(false);
                if (showPrintDialog == false)
                {
                    report1.Print();
                }
                else
                {
                    report1.Show();
                }
            }
            catch (Exception er)
            {
                return false;
            }
            return true;
        }

        #endregion 买单直接打印

        #region 取消，赠送可以填写份数

        [AuthButton(AuthFlag.None)]
        public ActionResult _NumberInputByPosIn(CancelItemQuantity model)
        {
            ViewBag.Version = CurrentVersion;
            return PartialView("_NumberInputByPosIn", model);
        }

        /// <summary>
        /// 根据输入的数字取消项目
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult CancelItemByNumber(CancelItemQuantity model)
        {
            try
            {
                //账单明细服务
                var billDetailService = GetService<IPosBillDetailService>();

                //获取当前操作的对象
                var billDetail = billDetailService.Get(int.Parse(model.Id));

                var oldbillDetail = new PosBillDetail();
                AutoSetValueHelper.SetValues(billDetail, oldbillDetail);

                var billService = GetService<IPosBillService>();
                var bill = billService.Get(billDetail.Billid);

                //套餐服务
                var suitService = GetService<IPosItemSuitService>();

                var itemList = billDetailService.GetBillDetailByUpid(CurrentInfo.HotelId, billDetail.Billid, billDetail.Upid).Where(w => w.SD == true).ToList();

                //物品组成
                var service = GetService<IPosCostItemService>();
                var costList = service.GetListPostCostByItemId(CurrentInfo.HotelId, billDetail.Itemid, billDetail.Unitid);

                //获取仓库消耗数据
                var billCostService = GetService<IPosBillCostService>();
                var billCostList = billCostService.GetBillCostList(CurrentInfo.HotelId, CurrentInfo.ModuleCode, billDetail.Id);


                #region 如果输入的数量与账单中的数量一致，则直接修改状态

                if (billDetail.Quantity == model.CancelNum)
                {
                    if (Convert.ToByte(model.istagtype) == 0)   //取消
                    {
                        if (Convert.ToBoolean(model.isreuse))
                        {
                            if (billDetail.SP == true)
                            {
                                #region 取消套餐明细

                                foreach (var item in itemList)
                                {
                                    var oldSuitDetail = new PosBillDetail() { Status = item.Status, CanReason = item.CanReason };
                                    item.Status = (byte)PosBillDetailStatus.加回库存取消;
                                    item.CanReason = model.canReason;

                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + item.Id + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldSuitDetail.Status) + "->" + Enum.GetName(typeof(PosBillDetailStatus), item.Status) + "，原因：" + oldSuitDetail.CanReason + "->" + item.CanReason, bill.BillNo);

                                    billDetailService.Update(item, new PosBillDetail());
                                    billDetailService.Commit();
                                }

                                #endregion 取消套餐明细
                            }

                            billDetail.Status = (byte)PosBillDetailStatus.加回库存取消;
                            //删除耗用表数据
                            foreach (var billCost in billCostList)
                            {
                                billCostService.Delete(billCost);
                                billCostService.Commit();
                                billCostService.AddDataChangeLog(OpLogType.Pos消费项目仓库耗用表删除);
                                billCostService.Commit();
                            }
                        }
                        else if (Convert.ToBoolean(model.isreuse) == false)
                        {
                            if (billDetail.SP == true)
                            {
                                #region 取消套餐明细

                                foreach (var item in itemList)
                                {
                                    var oldSuitDetail = new PosBillDetail() { Status = item.Status, CanReason = item.CanReason };
                                    item.Status = (byte)PosBillDetailStatus.不加回库取消;
                                    item.CanReason = model.canReason;

                                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + item.Id + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldSuitDetail.Status) + "->" + Enum.GetName(typeof(PosBillDetailStatus), item.Status) + "，原因：" + oldSuitDetail.CanReason + "->" + item.CanReason, bill.BillNo);

                                    billDetailService.Update(item, new PosBillDetail());
                                    billDetailService.Commit();
                                }

                                #endregion 取消套餐明细
                            }

                            billDetail.Status = (byte)PosBillDetailStatus.不加回库取消;
                        }
                    }
                    else
                    {
                        if (billDetail.SP == true)
                        {
                            #region 赠送套餐明细

                            foreach (var item in itemList)
                            {
                                var oldSuitDetail = new PosBillDetail() { Status = item.Status, CanReason = item.CanReason };
                                item.Status = (byte)PosBillDetailStatus.赠送;
                                item.CanReason = model.canReason;

                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + item.Id + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldSuitDetail.Status) + "->" + Enum.GetName(typeof(PosBillDetailStatus), item.Status) + "，原因：" + oldSuitDetail.CanReason + "->" + item.CanReason, bill.BillNo);

                                billDetailService.Update(item, new PosBillDetail());
                                billDetailService.Commit();
                            }

                            #endregion 赠送套餐明细
                        }

                        billDetail.Status = (byte)PosBillDetailStatus.赠送;
                    }

                    billDetail.CanReason = model.canReason;
                    billDetail.TransUser = CurrentInfo.UserName;  //取消的人
                    billDetail.ModiDate = DateTime.Now;

                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldbillDetail.Status) + "->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status) + "，原因：" + oldbillDetail.CanReason + "->" + billDetail.CanReason, bill.BillNo);

                    billDetailService.Update(billDetail, new PosBillDetail());

                    billDetailService.Commit();

                    return Json(JsonResultData.Successed());
                }

                #endregion 如果输入的数量与账单中的数量一致，则直接修改状态

                var newModel = new PosBillDetail();

                var maxAcbillNo = billDetailService.GetMaxAcBillNo(CurrentInfo.HotelId, billDetail.Billid);

                //复制一个对象
                AutoSetValueHelper.SetValues(billDetail, newModel);
                newModel.Upid = Guid.NewGuid();
                newModel.Acbillno = maxAcbillNo;


                //新增加的数据是取消的
                newModel.Quantity = model.CancelNum;    //数量
                newModel.Dueamount = model.CancelNum * newModel.Price;//价格
                var suitList = suitService.GetPosItemSuitListByItemId(CurrentInfo.HotelId, billDetail.Itemid);

                if (Convert.ToByte(model.istagtype) == 0)   //取消
                {
                    if (Convert.ToBoolean(model.isreuse))
                    {
                        foreach (var billCost in billCostList)
                        {
                            billCost.Quantity -= newModel.Quantity * billCost.OriQuan;
                            billCost.Amount -= newModel.Quantity * billCost.OriQuan * billCost.Price;
                            billCost.ModiUser = CurrentInfo.UserName;
                            billCost.Modifieddate = DateTime.Now;
                            billCostService.Update(billCost, new PosBillCost());
                            billCostService.AddDataChangeLog(OpLogType.Pos消费项目仓库耗用表修改);
                            billCostService.Commit();
                        }


                        newModel.Status = (byte)PosBillDetailStatus.加回库存取消;
                        newModel.CanReason = model.canReason;
                        newModel.TransUser = CurrentInfo.UserName;  //取消的人
                        newModel.ModiDate = DateTime.Now;
                        newModel.Amount = newModel.Dueamount;
                        billDetailService.Add(newModel);
                        billDetailService.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newModel.Id + "，原因：" + model.canReason + " ，取消数量：" + model.CancelNum, bill.BillNo);
                        //   billDetailService.AddDataChangeLog(OpLogType.Pos消费项目增加);
                        if (newModel.SP == true)
                        {
                            #region 取消套餐明细

                            foreach (var item in itemList)
                            {
                                var newSuitDetail = new PosBillDetail();
                                AutoSetValueHelper.SetValues(item, newSuitDetail);

                                var suit = suitList.Where(w => w.ItemId2 == item.Itemid).FirstOrDefault();

                                newSuitDetail.Upid = newModel.Upid;
                                newSuitDetail.Quantity = suit.Quantity * model.CancelNum;    //数量
                                newSuitDetail.Dueamount = suit.Amount * newSuitDetail.Quantity;     //价格
                                newSuitDetail.Status = (byte)PosBillDetailStatus.加回库存取消;

                                newSuitDetail.CanReason = model.canReason;
                                newSuitDetail.TransUser = CurrentInfo.UserName;  //取消的人
                                newSuitDetail.ModiDate = DateTime.Now;
                                newSuitDetail.Amount = newModel.Dueamount;

                                billDetailService.Add(newSuitDetail);
                                billDetailService.Commit();
                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newSuitDetail.Id + "，原因：" + model.canReason + " ，取消数量：" + newSuitDetail.Quantity, bill.BillNo);
                            }

                            #endregion 取消套餐明细
                        }
                    }
                    else if (Convert.ToBoolean(model.isreuse) == false)
                    {
                        newModel.Status = (byte)PosBillDetailStatus.不加回库取消;
                        newModel.CanReason = model.canReason;
                        newModel.TransUser = CurrentInfo.UserName;  //取消的人
                        newModel.ModiDate = DateTime.Now;
                        newModel.Amount = newModel.Dueamount;
                        billDetailService.Add(newModel);
                        billDetailService.AddDataChangeLog(OpLogType.Pos消费项目增加);
                        if (newModel.SP == true)
                        {
                            #region 取消套餐明细

                            foreach (var item in itemList)
                            {
                                var newSuitDetail = new PosBillDetail();
                                AutoSetValueHelper.SetValues(item, newSuitDetail);
                                var suit = suitList.Where(w => w.ItemId2 == item.Itemid).FirstOrDefault();

                                newSuitDetail.Upid = newModel.Upid;
                                newSuitDetail.Quantity = suit.Quantity * model.CancelNum;    //数量
                                newSuitDetail.Dueamount = suit.Amount * newSuitDetail.Quantity;     //价格
                                newSuitDetail.Status = (byte)PosBillDetailStatus.不加回库取消;

                                newSuitDetail.CanReason = model.canReason;
                                newSuitDetail.TransUser = CurrentInfo.UserName;  //取消的人
                                newSuitDetail.ModiDate = DateTime.Now;
                                newSuitDetail.Amount = newModel.Dueamount;

                                billDetailService.Add(newSuitDetail);
                                //  billDetailService.AddDataChangeLog(OpLogType.Pos消费项目增加);
                                billDetailService.Commit();
                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newSuitDetail.Id + "，原因：" + model.canReason + " ，取消数量：" + newSuitDetail.Quantity, bill.BillNo);
                            }

                            #endregion 取消套餐明细
                        }
                    }
                }
                else
                {
                    newModel.Status = (byte)PosBillDetailStatus.赠送;
                    newModel.CanReason = model.canReason;
                    newModel.TransUser = CurrentInfo.UserName;  //取消的人
                    newModel.ModiDate = DateTime.Now;
                    newModel.Amount = newModel.Dueamount;
                    billDetailService.Add(newModel);
                    billDetailService.AddDataChangeLog(OpLogType.Pos消费项目增加);
                    if (newModel.SP == true)
                    {
                        newModel.Status = (byte)PosBillDetailStatus.赠送;
                        newModel.CanReason = model.canReason;
                        newModel.TransUser = CurrentInfo.UserName;  //取消的人
                        newModel.ModiDate = DateTime.Now;
                        newModel.Amount = newModel.Dueamount;
                        billDetailService.Add(newModel);
                        billDetailService.AddDataChangeLog(OpLogType.Pos消费项目增加);

                        #region 赠送套餐明细

                        foreach (var item in itemList)
                        {
                            var newSuitDetail = new PosBillDetail();
                            AutoSetValueHelper.SetValues(item, newSuitDetail);
                            var suit = suitList.Where(w => w.ItemId2 == item.Itemid).FirstOrDefault();

                            newSuitDetail.Upid = newModel.Upid;
                            newSuitDetail.Quantity = suit.Quantity * model.CancelNum;    //数量
                            newSuitDetail.Dueamount = suit.Amount * newSuitDetail.Quantity;     //价格
                            newSuitDetail.Status = (byte)PosBillDetailStatus.赠送;

                            newSuitDetail.CanReason = model.canReason;
                            newSuitDetail.TransUser = CurrentInfo.UserName;  //取消的人
                            newSuitDetail.ModiDate = DateTime.Now;
                            newSuitDetail.Amount = newModel.Dueamount;

                            billDetailService.Add(newSuitDetail);
                            //  billDetailService.AddDataChangeLog(OpLogType.Pos消费项目增加);
                            billDetailService.Commit();
                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newSuitDetail.Id + "，原因：" + model.canReason + " ，取消数量：" + newSuitDetail.Quantity, bill.BillNo);
                        }

                        #endregion 赠送套餐明细
                    }
                }

                //newModel.Status = (byte)PosBillDetailStatus.加回库存取消;
                //billDetailService.Update(newModel, new PosBillDetail());
                //billDetailService.AddDataChangeLog(OpLogType.Pos消费项目修改);

                #region 修改套餐明细

                foreach (var item in itemList)
                {
                    var suit = suitList.Where(w => w.ItemId2 == item.Itemid).FirstOrDefault();
                    var oldItem = new PosBillDetail();
                    AutoSetValueHelper.SetValues(item, oldItem);

                    item.Quantity -= suit.Quantity * model.CancelNum;
                    item.Dueamount = suit.Amount * item.Quantity;

                    billDetailService.Update(item, new PosBillDetail());
                    // billDetailService.AddDataChangeLog(OpLogType.Pos消费项目修改);

                    billDetailService.Commit();
                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + item.Id + "，原因：" + model.canReason + " ，数量：" + oldItem.Quantity + "-->" + item.Quantity + "，金额：" + oldItem.Dueamount + "-->" + item.Dueamount, bill.BillNo);
                }

                #endregion 修改套餐明细

                billDetail.Quantity -= model.CancelNum;
                billDetail.Dueamount = billDetail.Quantity * billDetail.Price;

                billDetailService.Update(billDetail, new PosBillDetail());
                //  billDetailService.AddDataChangeLog(OpLogType.Pos消费项目修改);

                billDetailService.Commit();
                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，原因：" + model.canReason + " ，数量：" + oldbillDetail.Quantity + "-->" + billDetail.Quantity + "，金额：" + oldbillDetail.Dueamount + "-->" + billDetail.Dueamount, bill.BillNo);

                billDetailService.StatisticsBillDetail(CurrentInfo.HotelId, billDetail.Billid, billDetail.Billid);
                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
                //  throw;
            }
        }

        #endregion 取消，赠送可以填写份数

        /// <summary>
        /// 增加套餐
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult AddSuiteDetail(PosBillDetailAddViewModel model)
        {
            var billService = GetService<IPosBillService>();
            var service = GetService<IPosBillDetailService>();
            var suitService = GetService<IPosItemSuitService>();

            var itemService = GetService<IPosItemService>();
            var posItem = itemService.GetEntity(CurrentInfo.HotelId, model.Itemid);

            var unitService = GetService<IPosUnitService>();
            var unit = unitService.GetEntity(CurrentInfo.HotelId, model.Unitid);

            var itemPriceService = GetService<IPosItemPriceService>();
            var itemPrice = itemPriceService.GetPosItemPriceByUnitid(CurrentInfo.HotelId, model.Itemid, model.Unitid);

            #region 会员价查询以及设置
            //获取会员价
            var memberprice = itemPrice.Price;
            if (itemPrice.MemberPrice != null)
            {
                memberprice = (decimal)itemPrice.MemberPrice;
            }
            //else if (CurPosItemPrice.Price != null)
            //{
            //    memberprice = CurPosItemPrice.Price;
            //}

            //设置会员价
            model.PriceClub = memberprice;
            model.PriceOri = model.Price ?? 0;// itemPrice.Price;
            #endregion







            if (model != null && model.Billid != null && model.Itemid != null)  //物理台
            {
                try
                {
                    //判断套餐是否有设置套餐明细
                    if (!CheckSuitItem(model.Itemid))
                    {
                        return Json(JsonResultData.Failure("非自定义套餐必须设置套餐明细！"));
                    }

                    #region 判断选择的项目是否沽清

                    var SelloutService = GetService<IPosSellOutService>();
                    var PosSellout = SelloutService.GetPosSelloutByItemId(CurrentInfo.HotelId, model.Itemid);
                    if (PosSellout != null && PosSellout.SellStatus == 0)
                    {
                        if (string.IsNullOrWhiteSpace(PosSellout.UnitId))
                        {
                            //单位全部沽清
                            return Json(JsonResultData.Failure(posItem.Cname + "已沽清"));
                        }
                        else
                        {
                            if (PosSellout.UnitId.Contains(model.Unitid))
                            {
                                return Json(JsonResultData.Failure(posItem.Cname + "_" + unit.Cname + "已沽清"));
                            }
                        }
                    }

                    #endregion 判断选择的项目是否沽清

                    #region 增加或修改套餐

                    var bill = billService.Get(model.Billid);

                    bill.ServiceRate = bill.ServiceRate >= 1 && bill.ServiceRate <= 100 ? bill.ServiceRate / 100 : bill.ServiceRate;
                    bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount;

                    bool isExists = service.IsExistsForLD(CurrentInfo.HotelId, bill.Billid, model.Itemid);
                    if (isExists)
                    {
                        #region 修改套餐

                        var billDetail = service.GetBillDetailByBillidForLD(CurrentInfo.HotelId, bill.Billid, model.Itemid);
                        var oldBillDetail = new PosBillDetail();//
                        AutoSetValueHelper.SetValues(billDetail, oldBillDetail);

                        if (billDetail.Unitid != model.Unitid)
                        {
                            billDetail.Unitid = model.Unitid;
                            billDetail.UnitCode = unit.Code;
                            billDetail.UnitName = unit.Cname;
                            billDetail.Price = itemPrice.Price;
                            billDetail.Dueamount = itemPrice.Price * billDetail.Quantity;
                            billDetail.PriceOri = model.PriceOri;
                            billDetail.PriceClub = model.PriceClub;
                        }
                        else
                        {
                            billDetail.Quantity = billDetail.Quantity + model.Quantity;
                            billDetail.Dueamount = itemPrice.Price * billDetail.Quantity;
                        }

                        billDetail.Discount = billDetail.Discount >= 1 && billDetail.Discount <= 100 ? billDetail.Discount / 100 : billDetail.Discount;
                        var serviceAmount = posItem.IsService == true ? billDetail.Price * billDetail.Quantity * bill.ServiceRate : 0;
                        var amount = posItem.IsDiscount == true ? (billDetail.Price * billDetail.Quantity - billDetail.DiscAmount) * billDetail.Discount
                            : (billDetail.Price * billDetail.Quantity);

                        billDetail.Amount = amount;
                        billDetail.Service = serviceAmount;


                        service.Update(billDetail, new PosBillDetail());
                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，数量：" + oldBillDetail.Quantity + "->" + billDetail.Quantity + "，金额：" + oldBillDetail.Dueamount + "->" + billDetail.Dueamount + "，服务费：" + oldBillDetail.Service + "->" + billDetail.Service, bill.BillNo);

                        #endregion 修改套餐

                        #region 修改套餐明细

                        var suitList = suitService.GetPosItemSuitListByItemId(CurrentInfo.HotelId, billDetail.Itemid);
                        var itemList = service.GetBillDetailByUpid(CurrentInfo.HotelId, billDetail.Billid, billDetail.Upid).Where(w => w.SD == true).ToList();
                        foreach (var item in itemList)
                        {
                            var suit = suitList.Where(w => w.ItemId2 == item.Itemid).FirstOrDefault();
                            var oldSuitDetail = new PosBillDetail();//
                            AutoSetValueHelper.SetValues(item, oldSuitDetail);

                            item.Quantity = suit.Quantity * billDetail.Quantity;
                            item.Dueamount = suit.Amount * item.Quantity;
                            item.Discount = item.Discount >= 1 && item.Discount <= 100 ? item.Discount / 100 : item.Discount;
                            item.Amount = posItem.IsService == true ? item.Price * item.Quantity * bill.ServiceRate : 0;
                            item.Service = posItem.IsDiscount == true ? (item.Price * item.Quantity - item.DiscAmount) * item.Discount
                            : (item.Price * item.Quantity);



                            service.Update(item, new PosBillDetail());
                            service.Commit();
                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + item.Id + "，数量：" + oldSuitDetail.Quantity + "->" + item.Quantity + "，金额：" + oldSuitDetail.Dueamount + "->" + item.Dueamount + "，服务费：" + oldSuitDetail.Service + "->" + item.Service, bill.BillNo);
                        }

                        #endregion 修改套餐明细

                        service.Commit();
                    }
                    else
                    {
                        #region 增加套餐

                        var dueamount = itemPrice.Price * model.Quantity;
                        var amount = posItem.IsDiscount == true ? dueamount * bill.Discount : dueamount;
                        var serviceAmount = posItem.IsService == true ? dueamount * bill.ServiceRate : 0;
                        model.Upid = Guid.NewGuid();

                        PosBillDetail billDetail = new PosBillDetail()
                        {
                            Hid = CurrentInfo.HotelId,
                            DcFlag = PosItemDcFlag.D.ToString(),
                            IsCheck = false,
                            ItemCode = posItem.Code,
                            ItemName = posItem.Cname,
                            UnitCode = unit.Code,
                            UnitName = unit.Cname,
                            Price = itemPrice.Price,
                            Isauto = (byte)PosBillDetailIsauto.录入项目,
                            Status = (byte)PosBillDetailStatus.保存,
                            Dueamount = dueamount,
                            Discount = 1,
                            Amount = amount,
                            Service = serviceAmount,
                            SP = true,
                            SD = false,
                            TransUser = CurrentInfo.UserName,
                            TransBsnsDate = bill.BillBsnsDate,
                            TransShiftid = bill.Shiftid,
                            TransShuffleid = bill.Shuffleid,
                            TransDate = DateTime.Now,
                        };

                        AutoSetValueHelper.SetValues(model, billDetail);
                        billDetail.OrderType = "PC";
                        service.Add(billDetail);
                        service.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + billDetail.Id + "，名称：" + billDetail.ItemName + "，数量：" + billDetail.Quantity + "，金额：" + billDetail.Dueamount, bill.BillNo);
                        //service.AddDataChangeLog(OpLogType.Pos账单消费明细增加);

                        #endregion 增加套餐

                        #region 增加套餐明细

                        var itemList = suitService.GetPosItemSuitListByItemId(CurrentInfo.HotelId, billDetail.Itemid);
                        foreach (var item in itemList.Where(w => w.IsAuto == true).OrderBy(o => o.IGrade).ToList())
                        {
                            PosBillDetail suitBillDetail = new PosBillDetail()
                            {
                                Hid = CurrentInfo.HotelId,
                                Billid = bill.Billid,
                                MBillid = bill.MBillid,
                                Itemid = item.ItemId2,
                                ItemCode = item.ItemCode2,
                                ItemName = item.ItemName,
                                Unitid = item.Unitid,
                                UnitCode = item.UnitCode,
                                UnitName = item.UnitName,
                                DcFlag = PosItemDcFlag.D.ToString(),
                                IsCheck = false,
                                Isauto = (byte)PosBillDetailIsauto.录入项目,
                                Status = (byte)PosBillDetailStatus.保存,
                                Dueamount = item.Amount * model.Quantity,
                                Discount = 1,
                                Price = item.Price,
                                PriceOri = item.Price,
                                PriceClub = 0,
                                Amount = item.Amount * model.Quantity,
                                OverSuite = item.AddPrice,
                                Service = posItem.IsService == true ? item.Amount * bill.ServiceRate : 0,
                                SP = false,
                                SD = true,
                                Upid = model.Upid,
                                Tabid = model.Tabid,
                                Quantity = item.Quantity * model.Quantity,
                                TransUser = CurrentInfo.UserName,
                                TransBsnsDate = bill.BillBsnsDate,
                                TransShiftid = bill.Shiftid,
                                TransShuffleid = bill.Shuffleid,
                                TransDate = DateTime.Now,
                                OrderType = "PC"
                            };

                            service.Add(suitBillDetail);
                            //service.AddDataChangeLog(OpLogType.Pos账单消费明细增加);
                            service.Commit();
                            AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + suitBillDetail.Id + "，名称：" + suitBillDetail.ItemName + "，数量：" + suitBillDetail.Quantity + "，金额：" + suitBillDetail.Dueamount, bill.BillNo);
                        }

                        #endregion 增加套餐明细

                        service.Commit();
                    }

                    #endregion 增加或修改套餐

                    return Json(JsonResultData.Successed());
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
            else    //虚拟台
            {
                try
                {
                    //判断套餐是否有设置套餐明细
                    if (!CheckSuitItem(model.Itemid))
                    {
                        return Json(JsonResultData.Failure("非自定义套餐必须设置套餐明细！"));
                    }

                    #region 增加账单

                    var refe = Session["PosRefe"] as PosRefe;

                    var posService = GetService<IPosPosService>();
                    var pos = posService.Get(CurrentInfo.PosId);

                    var tabid = billService.GetTakeoutForTabid(CurrentInfo.HotelId, refe.Id, pos.Business, (byte)PosBillTabFlag.快餐台);
                    var posBill = billService.GetLastBillId(CurrentInfo.HotelId, refe.Id, pos.Business);

                    model.Tabid = string.IsNullOrWhiteSpace(model.Tabid) ? CurrentInfo.HotelId + tabid : model.Tabid;
                    model.Billid = string.IsNullOrWhiteSpace(model.Billid) ? posBill.Billid : model.Billid;
                    model.MBillid = string.IsNullOrWhiteSpace(model.MBillid) ? posBill.Billid : model.MBillid;

                    PosBill bill = new PosBill()
                    {
                        Hid = CurrentInfo.HotelId,
                        Refeid = refe.Id,
                        Billid = posBill.Billid,
                        BillNo = posBill.BillNo,
                        MBillid = posBill.Billid,
                        InputUser = CurrentInfo.UserName,

                        IGuest = 1,
                        TabNo = tabid,
                        BillBsnsDate = pos.Business,
                        BillDate = DateTime.Now,
                        IsService = true,
                        IsLimit = true,
                        Status = (byte)PosBillStatus.开台,
                        TabFlag = (byte)PosBillTabFlag.快餐台,
                        Shiftid = pos.ShiftId,
                        Shuffleid = refe.ShuffleId,
                        Discount = 100,
                        ServiceRate = 0
                    };

                    bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount ?? 0;
                    bill.ServiceRate = bill.ServiceRate >= 1 && bill.ServiceRate <= 100 ? bill.ServiceRate / 100 : bill.ServiceRate ?? 0;

                    AutoSetValueHelper.SetValues(model, bill);
                    billService.Add(bill);
                    // billService.AddDataChangeLog(OpLogType.Pos账单增加);
                    billService.Commit();
                    AddOperationLog(OpLogType.Pos账单增加, "单号：" + bill.Billid + "，餐台：" + bill.Tabid + "，营业点：" + bill.Refeid, bill.BillNo);

                    #endregion 增加账单

                    bill = billService.Get(bill.Billid);    //获取账单
                    bill.ServiceRate = bill.ServiceRate >= 1 && bill.ServiceRate <= 100 ? bill.ServiceRate / 100 : bill.ServiceRate;
                    bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount;

                    bool isExists = service.IsExistsForLD(CurrentInfo.HotelId, bill.Billid, model.Itemid);
                    if (isExists)   //
                    {
                        #region 修改套餐

                        var billDetail = service.GetBillDetailByBillidForLD(CurrentInfo.HotelId, bill.Billid, model.Itemid);

                        var oldBillDetail = new PosBillDetail();//
                        AutoSetValueHelper.SetValues(billDetail, oldBillDetail);

                        if (billDetail.Unitid != model.Unitid)  //修改单位
                        {
                            billDetail.Unitid = model.Unitid;
                            billDetail.UnitCode = unit.Code;
                            billDetail.UnitName = unit.Cname;
                            billDetail.Price = itemPrice.Price;
                            billDetail.Dueamount = itemPrice.Price * billDetail.Quantity;
                        }
                        else    //修改数量
                        {
                            billDetail.Quantity = billDetail.Quantity + model.Quantity;
                            billDetail.Dueamount = itemPrice.Price * billDetail.Quantity;
                        }

                        billDetail.Discount = billDetail.Discount >= 1 && billDetail.Discount <= 100 ? billDetail.Discount / 100 : billDetail.Discount;
                        var serviceAmount = posItem.IsService == true ? billDetail.Price * billDetail.Quantity * bill.ServiceRate : 0;
                        var amount = posItem.IsDiscount == true ? (billDetail.Price * billDetail.Quantity - billDetail.DiscAmount) * billDetail.Discount
                            : (billDetail.Price * billDetail.Quantity);

                        billDetail.Amount = amount;
                        billDetail.Service = serviceAmount;
                        // AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + billDetail.Id + "，数量：" + suitBillDetail.Quantity + "，金额：" + suitBillDetail.Dueamount + "，操作员：" + CurrentInfo.UserName, bill.BillNo);


                        service.Update(billDetail, oldBillDetail);
                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，数量：" + oldBillDetail.Quantity + "->" + billDetail.Quantity + "，金额：" + oldBillDetail.Amount + "->" + billDetail.Amount, bill.BillNo);

                        #endregion 修改套餐

                        #region 修改套餐明细

                        var suitList = suitService.GetPosItemSuitListByItemId(CurrentInfo.HotelId, billDetail.Itemid);
                        var itemList = service.GetBillDetailByUpid(CurrentInfo.HotelId, billDetail.Billid, billDetail.Upid).Where(w => w.SD == true).ToList();
                        foreach (var item in itemList)
                        {
                            var suit = suitList.Where(w => w.ItemId2 == item.Itemid).FirstOrDefault();
                            var oldSuitDetail = new PosBillDetail();
                            AutoSetValueHelper.SetValues(item, oldSuitDetail);

                            item.Quantity = suit.Quantity * billDetail.Quantity;
                            item.Dueamount = suit.Amount * item.Quantity;
                            item.Discount = item.Discount >= 1 && item.Discount <= 100 ? item.Discount / 100 : item.Discount;
                            item.Amount = posItem.IsService == true ? item.Price * item.Quantity * bill.ServiceRate : 0;
                            item.Service = posItem.IsDiscount == true ? (item.Price * item.Quantity - item.DiscAmount) * item.Discount
                            : (item.Price * item.Quantity);



                            service.Update(item, new PosBillDetail());
                            service.Commit();
                            AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Billid + "，数量：" + oldSuitDetail.Quantity + "->" + item.Quantity + "，金额：" + oldSuitDetail.Dueamount + "->" + item.Dueamount, bill.BillNo);
                        }

                        #endregion 修改套餐明细

                        service.Commit();
                    }
                    else
                    {
                        #region 增加套餐

                        model.Upid = Guid.NewGuid();
                        var dueamount = itemPrice.Price * model.Quantity;
                        var amount = posItem.IsDiscount == true ? dueamount * bill.Discount : dueamount;
                        var serviceAmount = posItem.IsService == true ? dueamount * bill.ServiceRate : 0;

                        PosBillDetail billDetail = new PosBillDetail()
                        {
                            Hid = CurrentInfo.HotelId,
                            DcFlag = PosItemDcFlag.D.ToString(),
                            IsCheck = false,
                            ItemCode = posItem.Code,
                            ItemName = posItem.Cname,
                            UnitCode = unit.Code,
                            UnitName = unit.Cname,
                            Price = itemPrice.Price,
                            Isauto = (byte)PosBillDetailIsauto.录入项目,
                            Status = (byte)PosBillDetailStatus.保存,
                            Dueamount = dueamount,
                            Discount = 1,
                            Amount = amount,
                            Service = serviceAmount,
                            SP = true,
                            SD = false,
                            Upid = model.Upid,
                            TransUser = CurrentInfo.UserName,
                            TransBsnsDate = bill.BillBsnsDate,
                            TransShiftid = bill.Shiftid,
                            TransShuffleid = bill.Shuffleid,
                            TransDate = DateTime.Now,
                        };

                        AutoSetValueHelper.SetValues(model, billDetail);

                        billDetail.OrderType = "PC";
                        service.Add(billDetail);
                        AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + billDetail.Billid + "，名称：" + billDetail.ItemName + "，数量：" + billDetail.Quantity + "，金额：" + billDetail.Dueamount, bill.BillNo);
                        //  service.AddDataChangeLog(OpLogType.Pos账单消费明细增加);

                        #endregion 增加套餐

                        #region 增加套餐明细

                        var itemList = suitService.GetPosItemSuitListByItemId(CurrentInfo.HotelId, billDetail.Itemid);
                        foreach (var item in itemList.Where(w => w.IsAuto == true).OrderBy(o => o.IGrade).ToList())
                        {
                            PosBillDetail suitBillDetail = new PosBillDetail()
                            {
                                Hid = CurrentInfo.HotelId,
                                Billid = bill.Billid,
                                MBillid = bill.MBillid,
                                Itemid = item.ItemId2,
                                ItemCode = item.ItemCode2,
                                ItemName = item.ItemName,
                                Unitid = item.Unitid,
                                UnitCode = item.UnitCode,
                                UnitName = item.UnitName,
                                DcFlag = PosItemDcFlag.D.ToString(),
                                IsCheck = false,
                                Isauto = (byte)PosBillDetailIsauto.录入项目,
                                Status = (byte)PosBillDetailStatus.保存,
                                Dueamount = item.Amount * model.Quantity,
                                Discount = 1,
                                Price = item.Price,
                                PriceOri = item.Price,
                                Amount = item.Amount * model.Quantity,
                                OverSuite = item.AddPrice,
                                Service = posItem.IsService == true ? item.Amount * bill.ServiceRate : 0,
                                SP = false,
                                SD = true,
                                Upid = model.Upid,
                                Tabid = model.Tabid,
                                Quantity = item.Quantity * model.Quantity,
                                TransUser = CurrentInfo.UserName,
                                TransBsnsDate = bill.BillBsnsDate,
                                TransShiftid = bill.Shiftid,
                                TransShuffleid = bill.Shuffleid,
                                TransDate = DateTime.Now,
                                OrderType = "PC"
                            };


                            service.Add(suitBillDetail);
                            // service.AddDataChangeLog(OpLogType.Pos账单消费明细增加);
                            service.Commit();
                            AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + suitBillDetail.Billid + "，名称：" + suitBillDetail.ItemName + "，数量：" + suitBillDetail.Quantity + "，金额：" + suitBillDetail.Dueamount, bill.BillNo);
                        }

                        #endregion 增加套餐明细

                        service.Commit();
                    }

                    return Json(JsonResultData.Successed(bill));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
        }

        /// <summary>
        /// 验证套餐是否是自定义套餐
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private bool CheckSuitItem(string itemId)
        {
            var itemService = GetService<IPosItemService>();
            var item = itemService.Get(itemId);
            if (item != null)
            {
                //不是自定义套餐 必须定义套餐明细
                if (item.IsUserDefined == false)
                {
                    var service = GetService<IPosItemSuitService>();
                    var suitList = service.GetPosItemSuitListByItemId(CurrentInfo.HotelId, itemId);
                    if (suitList == null || suitList.Count <= 0)
                    {
                        return false;
                    }
                    else return true;
                }
                else
                {
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// 获取套餐明细列表
        /// </summary>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult _SuitList(PosItemSuitViewModel model)
        {
            try
            {
                if (model != null && model.Upid != null)
                {
                    List<up_pos_list_itemSuitByItemIdResult> suitList = new List<up_pos_list_itemSuitByItemIdResult>();
                    var billDetailService = GetService<IPosBillDetailService>();
                    var billDetailList = billDetailService.GetBillDetailByUpid(CurrentInfo.HotelId, model.Billid, Guid.Parse(model.Upid));
                    if (billDetailList != null && billDetailList.Count > 0)
                    {
                        var billdetail1 = billDetailList.Where(w => w.SP == true).FirstOrDefault();
                        var billdetail2 = billDetailList.Where(w => w.Itemid == model.ItemId2).FirstOrDefault();
                        if (billdetail1 != null && billdetail2 != null)
                        {
                            ViewBag.BillDetailId = model.BillDetailId;

                            var suitService = GetService<IPosItemSuitService>();
                            var suit = suitService.GetPosItemSuitListByItemId(CurrentInfo.HotelId, billdetail1.Itemid.ToString()).Where(w => w.ItemId2 == billdetail2.Itemid).FirstOrDefault();
                            if (suit != null)
                            {
                                suitList = suitService.GetPosItemSuitListByItemId(CurrentInfo.HotelId, billdetail1.Itemid, suit.IGrade, model.PageIndex, model.PageSize).ToList();
                            }
                        }
                        return PartialView("_SuitList", suitList);
                    }
                }
                return Json(JsonResultData.Failure(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 更新套餐
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult UpdateSuit(PosItemSuitViewModel model)
        {
            try
            {
                if (model != null && !string.IsNullOrEmpty(model.BillDetailId))
                {
                    var billDetailService = GetService<IPosBillDetailService>();
                    var billDetail = billDetailService.GetEntity(CurrentInfo.HotelId, Convert.ToInt64(model.BillDetailId));

                    //获取账单明细
                    var billService = GetService<IPosBillService>();
                    var bill = billService.Get(billDetail.Billid);

                    var oldbillDetail = new PosBillDetail();
                    AutoSetValueHelper.SetValues(billDetail, oldbillDetail);

                    if (billDetail.SD == true)
                    {
                        billDetail.Itemid = model.ItemId2;
                        billDetail.ItemCode = model.ItemCode2;
                        billDetail.ItemName = model.ItemName;
                        billDetail.Unitid = model.Unitid;
                        billDetail.UnitCode = model.UnitCode;
                        billDetail.UnitName = model.UnitName;
                        billDetail.Price = model.Price;
                        billDetail.Quantity = model.Quantity;
                        billDetail.Amount = model.Amount;
                        billDetail.OverSuite = model.AddPrice;

                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，名称：" + oldbillDetail.ItemName + "-->" + billDetail.ItemName + "，数量：" + oldbillDetail.Quantity + "-->" + billDetail.Quantity + "，金额：" + oldbillDetail.Dueamount + "-->" + billDetail.Dueamount, bill.BillNo);

                        billDetailService.Update(billDetail, new PosBillDetail());
                        billDetailService.Commit();

                        return Json(JsonResultData.Successed(""));
                    }
                    return Json(JsonResultData.Failure("您选择的项目不是套餐明细，无法修改！"));
                }

                return Json(JsonResultData.Failure(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        #region 时价菜添加项目

        [AuthButton(AuthFlag.Add)]
        public ActionResult _PriceNumberByAddItem(PosBillDetailAddViewModel model)
        {
            return PartialView("_PriceNumberByAddItem", model);
        }

        #endregion 时价菜添加项目

        #region 打单

        [AuthButton(AuthFlag.Service)]
        public ActionResult AddQueryParaTemp(ReportQueryModel model, string print, string Flag)
        {
            PosCommon common = new PosCommon();
            var controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            return common.AddQueryParaTemp(model, print, Flag, controller);

        }

        #endregion 打单

        #region 预览

        [AuthButton(AuthFlag.Stop)]
        public ActionResult AddQueryParaTempA(ReportQueryModel model, string print, string Flag)
        {
            PosCommon common = new PosCommon();
            var controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            return common.AddQueryParaTemp(model, print, Flag, controller);

        }

        #endregion 预览

        #region 买单视图

        [AuthButton(AuthFlag.Close)]
        public PartialViewResult _Payment(PaymentViewModel model)
        {
            PosCommon common = new PosCommon();
            return common._Payment(model);
        }

        #endregion 买单视图

        #region 买单成功打单

        [HttpPost]
        [AuthButton(AuthFlag.Service)]
        public ActionResult AddQueryParaTempByPaySusses(ReportQueryModel model, string print, string Flag, string isPrintBill)
        {
            PosCommon common = new PosCommon();
            return common.AddQueryParaTempByPaySusses(model, print, Flag, isPrintBill);
        }

        #endregion 买单成功打单

        #region 打单之前判断收银点设置是否打单

        /// <summary>
        /// 买单之前验证营业点设置的 买单未打单条件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult CheckRefeITagPrintBill(PaymentViewModel model)
        {
            PosCommon common = new PosCommon();
            return common.CheckRefeITagPrintBill(model);
        }

        #endregion 打单之前判断收银点设置是否打单

        #region 特价菜功能

        /// <summary>
        /// 特价菜消费项目列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _ItemListTJC(PosItemViewModel model)
        {
            var billService = GetService<IPosBillService>();
            var bill = billService.Get(model.billId);
            var customerTypeid = "";
            var tabTypeid = "";
            //获取客人类型与餐台类型
            if (bill != null)
            {

                var tabService = GetService<IPosTabService>();
                var tab = tabService.Get(bill.Tabid);
                tabTypeid = tab.TabTypeid ?? "";
                customerTypeid = bill.CustomerTypeid ?? "";
            }

            var service = GetService<IPosOnSaleService>();
            //获取特价菜列表
            var list = service.GetItemListByPosOnSale(CurrentInfo.HotelId, model.Refeid ?? "", model.itagperiod ?? "", customerTypeid, tabTypeid, model.PageIndex, model.PageSize);
            return PartialView("_ItemListTJC", list);
        }

        /// <summary>
        /// 获取特价菜数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult GetPosOnSaleItemTotal(PosItemViewModel model)
        {
            var billService = GetService<IPosBillService>();
            var bill = billService.Get(model.billId);
            var customerTypeid = "";
            var tabTypeid = "";
            //获取客人类型与餐台类型
            if (bill != null)
            {

                var tabService = GetService<IPosTabService>();
                var tab = tabService.Get(bill.Tabid);
                tabTypeid = tab.TabTypeid ?? "";
                customerTypeid = bill.CustomerTypeid ?? "";
            }

            var service = GetService<IPosOnSaleService>();
            //获取特价菜列表
            var result = service.GetPosOnSaleTotal(CurrentInfo.HotelId, model.Refeid ?? "", model.itagperiod ?? "", customerTypeid, tabTypeid);
            return Content(result.ToString());
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult AddPosOnSaleItem(Guid Id, string BillId)
        {
            var service = GetService<IPosOnSaleService>();

            var model = service.Get(Id);
            if (model != null)
            {
                var itemService = GetService<IPosItemService>();//消费项目接口
                var item = itemService.Get(model.Itemid);

                var unitService = GetService<IPosUnitService>();//单位接口
                var unit = unitService.Get(model.Unitid);

                var billService = GetService<IPosBillService>();    //账单主表
                var bill = billService.Get(BillId);

                var billDetailService = GetService<IPosBillDetailService>();//账单明细

                PosBillDetail billDetail = new PosBillDetail()
                {
                    MBillid = bill.MBillid,
                    Billid = bill.Billid,
                    Price = model.Price,
                    Quantity = 1,
                    Tabid = bill.Tabid,

                    Hid = CurrentInfo.HotelId,
                    Itemid = item.Id,
                    ItemCode = item.Code,
                    ItemName = item.Cname,
                    Unitid = unit.Id,
                    UnitCode = unit.Code,
                    UnitName = unit.Cname,
                    DcFlag = PosItemDcFlag.D.ToString(),

                    IsCheck = false,
                    Isauto = (byte)PosBillDetailIsauto.特价菜,
                    Status = (byte)PosBillDetailStatus.保存,
                    Dueamount = model.Price * 1,
                    DiscAmount = 0,
                    Discount = model.IsDiscount == true ? bill.Discount : 100,
                    Amount = model.Price * 1,

                    SP = false,
                    SD = false,
                    TransUser = CurrentInfo.UserName,
                    TransBsnsDate = bill.BillBsnsDate,
                    TransShiftid = bill.Shiftid,
                    TransShuffleid = bill.Shuffleid,
                    TransDate = DateTime.Now,
                };
                billDetailService.Add(billDetail);
                //  billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细增加);
                billDetailService.Commit();
                AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + billDetail.Id + "，名称：" + billDetail.ItemName + "，数量：" + billDetail.Quantity + "，金额：" + billDetail.Dueamount, bill.BillNo);
            }

            return Json(JsonResultData.Successed());
        }

        [AuthButton(AuthFlag.None)]

        public ActionResult _PosOnSaleItemUint(PosItemPriceViewModel model)
        {
            if (model != null)
            {
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

                var service = GetService<IPosOnSaleService>();

                var billService = GetService<IPosBillService>();
                var bill = billService.Get(model.BillId);
                var customerTypeid = "";
                var tabTypeid = "";
                var refeId = "";
                //获取客人类型与餐台类型
                if (bill != null)
                {

                    var tabService = GetService<IPosTabService>();
                    var tab = tabService.Get(bill.Tabid);
                    tabTypeid = tab.TabTypeid ?? "";
                    customerTypeid = bill.CustomerTypeid ?? "";
                    refeId = bill.Refeid ?? "";
                }

                var sellOutService = GetService<IPosSellOutService>();  //沽清
                var list = service.GetUnitByPosOnSaleItem(CurrentInfo.HotelId, model.Itemid, refeId, model.itagperiod, customerTypeid, tabTypeid, model.PageIndex, model.PageSize);
                //循环列表
                foreach (var itemPrice in list)
                {
                    //根据项目ID与酒店ID取沽清表数据
                    var posSelloutModel = sellOutService.GetPosSelloutByItemId(CurrentInfo.HotelId, itemPrice.itemId);
                    if (posSelloutModel != null)
                    {
                        //判断沽清表中单位
                        if (string.IsNullOrWhiteSpace(posSelloutModel.UnitId) || posSelloutModel.UnitId.Contains(itemPrice.Unitid))
                        {
                            itemPrice.SelloutStatus = "0";  //代表全部
                        }
                    }
                }
                return PartialView("_ItemPriceList", list);
            }
            return PartialView("_ItemPriceList", new List<up_pos_list_ItemPriceByItemidResult>());

        }
        #endregion 特价菜功能

        /// <summary>
        /// 根据消费项目获取消费项目对应作法列表
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ActionResult _ItemActionList(PosItemViewModel model)
        {
            var service = GetService<IPosItemActionService>();
            var list = service.GetPosItemActionListByItemId(CurrentInfo.HotelId, model.Itemid, model.PageIndex, model.PageSize);
            if (list != null && list.Count > 0)
            {
                ViewBag.Itemid = model.Itemid;
                ViewBag.Itype = "Item";
                return PartialView("_ItemActionList", list);
            }
            else
            {
                return Content("");
            }
        }

        /// <summary>
        /// 查询消费项目作法总数
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ContentResult GetItemActionTotal(PosItemActionViewModel model)
        {
            var service = GetService<IPosItemActionService>();
            return Content(service.GetPosItemActionTotal(CurrentInfo.HotelId, model.Itemid).ToString());
        }

        /// <summary>
        /// 根据作法查询同组作法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _ActionMultisubList(PosActionMultisubViewModel model)
        {
            var service = GetService<IPosActionMultisubService>();
            var list = service.GetPosActionMultisubByactionid(CurrentInfo.HotelId, model.Actionid, model.PageIndex, model.PageSize);
            if (list != null && list.Count > 0)
            {
                ViewBag.ActionId = model.Actionid;
                return PartialView("_ActionMultisubList", list);
            }
            else
            {
                return Content("");
            }
        }

        /// <summary>
        /// 增加套餐明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult AddCustomSuiteDetail(PosBillDetailAddViewModel model)
        {
            try
            {
                var billService = GetService<IPosBillService>();
                var service = GetService<IPosBillDetailService>();

                var itemService = GetService<IPosItemService>();
                var item = itemService.GetEntity(CurrentInfo.HotelId, model.Itemid);

                var unitService = GetService<IPosUnitService>();
                var unit = unitService.GetEntity(CurrentInfo.HotelId, model.Unitid);

                var itemPriceService = GetService<IPosItemPriceService>();
                var itemPrice = itemPriceService.GetPosItemPriceByUnitid(CurrentInfo.HotelId, model.Itemid, model.Unitid);

                if (model != null && model.Billid != null && model.Itemid != null)
                {
                    try
                    {
                        //判断选择的项目是否沽清
                        var SelloutService = GetService<IPosSellOutService>();
                        var PosSellout = SelloutService.GetPosSelloutByItemId(CurrentInfo.HotelId, model.Itemid);
                        if (PosSellout != null && PosSellout.SellStatus == 0)
                        {
                            if (string.IsNullOrWhiteSpace(PosSellout.UnitId))
                            {
                                //单位全部沽清
                                return Json(JsonResultData.Failure(item.Cname + "已沽清"));
                            }
                            else
                            {
                                if (PosSellout.UnitId.Contains(model.Unitid))
                                {
                                    return Json(JsonResultData.Failure(item.Cname + "_" + unit.Cname + "已沽清"));
                                }
                            }
                        }

                        #region 增加消费明细

                        var bill = billService.Get(model.Billid);

                        bill.ServiceRate = bill.ServiceRate >= 1 && bill.ServiceRate <= 100 ? bill.ServiceRate / 100 : bill.ServiceRate;
                        bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount;

                        bool isExists = service.IsExistsSuiteItem(CurrentInfo.HotelId, bill.Billid, model.Upid, model.Itemid);
                        if (isExists)
                        {
                            return Json(JsonResultData.Failure("当前套餐已包含此项"));
                        }
                        else
                        {
                            var dueamount = itemPrice.Price * model.Quantity;
                            var amount = item.IsDiscount == true ? dueamount * bill.Discount : dueamount;
                            var serviceAmount = item.IsService == true ? dueamount * bill.ServiceRate : 0;

                            #region 赋值

                            PosBillDetail billDetail = new PosBillDetail()
                            {
                                Hid = CurrentInfo.HotelId,
                                DcFlag = PosItemDcFlag.D.ToString(),
                                IsCheck = false,
                                ItemCode = item.Code,
                                ItemName = item.Cname,
                                UnitCode = unit.Code,
                                UnitName = unit.Cname,
                                Price = itemPrice.Price,
                                Isauto = (byte)PosBillDetailIsauto.录入项目,
                                Status = (byte)PosBillDetailStatus.保存,
                                IsProduce = (byte)PosBillDetailIsProduce.未出品,
                                Dueamount = dueamount,
                                Discount = bill.Discount,
                                Amount = amount,
                                Service = serviceAmount,
                                SP = false,
                                SD = true,
                                TransUser = CurrentInfo.UserName,
                                TransBsnsDate = bill.BillBsnsDate,
                                TransShiftid = bill.Shiftid,
                                TransShuffleid = bill.Shuffleid,
                                TransDate = DateTime.Now,
                                Upid = model.Upid
                            };

                            #endregion 赋值

                            AutoSetValueHelper.SetValues(model, billDetail);
                            service.Add(billDetail);
                            //service.AddDataChangeLog(OpLogType.Pos账单消费明细增加);
                            service.Commit();
                            AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + billDetail.Id + "，名称：" + billDetail.ItemName + "，数量：" + billDetail.Quantity + "，金额：" + billDetail.Dueamount, bill.BillNo);
                        }

                        #endregion 增加消费明细

                        return Json(JsonResultData.Successed());
                    }
                    catch (Exception ex)
                    {
                        return Json(JsonResultData.Failure(ex));
                    }
                }

                return Json(JsonResultData.Failure(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 更新套餐明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult UpdateCustomSuiteDetail(PosBillDetailEditViewModel model)
        {
            try
            {
                var billService = GetService<IPosBillService>();
                var service = GetService<IPosBillDetailService>();

                var bill = billService.Get(model.Billid);

                bill.ServiceRate = bill.ServiceRate >= 1 && bill.ServiceRate <= 100 ? bill.ServiceRate / 100 : bill.ServiceRate;
                bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount;

                if (model != null && model.Billid != null && model.Itemid != null)
                {
                    //判断选择的项目是否沽清
                    var SelloutService = GetService<IPosSellOutService>();
                    var PosSellout = SelloutService.GetPosSelloutByItemId(CurrentInfo.HotelId, model.Itemid);
                    if (PosSellout != null && PosSellout.SellStatus == 0)
                    {
                        if (string.IsNullOrWhiteSpace(PosSellout.UnitId))
                        {
                            //单位全部沽清
                            return Json(JsonResultData.Failure(model.ItemName + "已沽清"));
                        }
                        else
                        {
                            if (PosSellout.UnitId.Contains(model.Unitid))
                            {
                                return Json(JsonResultData.Failure(model.ItemName + "_" + model.UnitName + "已沽清"));
                            }
                        }
                    }

                    var billDetail = service.GetEntity(CurrentInfo.HotelId, model.Id);

                    var oldBillDetail = new PosBillDetail();
                    AutoSetValueHelper.SetValues(billDetail, oldBillDetail);

                    var dueamount = model.Price * model.Quantity;
                    var amount = model.IsDiscount == true ? dueamount * bill.Discount : dueamount;
                    var serviceAmount = model.IsService == true ? dueamount * bill.ServiceRate : 0;

                    if (billDetail != null)
                    {
                        #region 赋值

                        billDetail.Hid = CurrentInfo.HotelId;
                        billDetail.DcFlag = PosItemDcFlag.D.ToString();
                        billDetail.IsCheck = false;
                        billDetail.Itemid = model.Itemid;
                        billDetail.ItemCode = model.ItemCode;
                        billDetail.ItemName = model.ItemName;
                        billDetail.Unitid = model.Unitid;
                        billDetail.UnitCode = model.UnitCode;
                        billDetail.UnitName = model.UnitName;
                        billDetail.Isauto = (byte)PosBillDetailIsauto.录入项目;
                        billDetail.Status = (byte)PosBillDetailStatus.保存;
                        billDetail.IsProduce = (byte)PosBillDetailIsProduce.未出品;
                        billDetail.Dueamount = dueamount;
                        billDetail.Discount = bill.Discount;
                        billDetail.Amount = amount;
                        billDetail.Service = serviceAmount;
                        billDetail.TransUser = CurrentInfo.UserName;
                        billDetail.TransDate = DateTime.Now;

                        #endregion 赋值

                        service.Update(billDetail, new PosBillDetail());
                        //service.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                        service.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，项目：" + oldBillDetail.ItemName + "-->" + billDetail.ItemName, bill.BillNo);

                        return Json(JsonResultData.Successed());
                    }
                }

                return Json(JsonResultData.Failure(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 称重界面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public PartialViewResult _WeighInput(PosBillDetailAddViewModel model)
        {

            return PartialView("_WeighInput", model);
        }

        /// <summary>
        /// 手写作法界面
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public PartialViewResult _HandAction(PosHandActionViewModel model)
        {
            return PartialView("_HandAction", model);
        }

        /// <summary>
        /// 是否已上菜（传菜）
        /// </summary>
        /// <param name="detailid"></param>
        /// <param name="sentstatus">（0：未传菜；1：已传菜）</param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult UpdatePosBillDetailStatus(string detailid, byte sentstatus)
        {
            try
            {
                var billdetailservice = GetService<IPosBillDetailService>();
                billdetailservice.UpdatePosBillDetailStatus(detailid, sentstatus);
                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }


        #region 快餐模式
        /// <summary>
        /// 查询餐台状态总数
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ContentResult GetBModeBillListCount(string refeid)
        {
            var billService = GetService<IPosBillService>();
            var pos = new PosPos();
            var posService = GetService<IPosPosService>();
            pos = posService.Get(CurrentInfo.PosId);
            return Content(billService.GetBillListForPosRefe(CurrentInfo.HotelId, refeid, pos.Business).Count().ToString());
        }
        /// <summary>
        /// 快餐模式，未买单的单据列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _BModeBillList(string refeid, int pageIndex, int pageSize)
        {
            var billService = GetService<IPosBillService>();
            var pos = new PosPos();
            var posService = GetService<IPosPosService>();
            pos = posService.Get(CurrentInfo.PosId);
            List<PosBill> list = billService.GetBillListForPosRefe(CurrentInfo.HotelId, refeid, pos.Business).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return PartialView("_BModeBillList", list);
        }
        /// <summary>
        /// + 号添加新的单据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult AddNewBModeBill(string refeid)
        {
            try
            {
                var refeService = GetService<IPosRefeService>();
                var billService = GetService<IPosBillService>();
                var pos = new PosPos();
                var posService = GetService<IPosPosService>();
                pos = posService.Get(CurrentInfo.PosId);
                var refeobj = refeService.GetEntity(CurrentInfo.HotelId, refeid);
                //根据营业日生成虚拟台id 从1开始
                string xntabid = string.Empty;
                //最新虚拟台id
                var zxxntabid = billService.GetAllBillListForPosRefe(CurrentInfo.HotelId, refeid, pos.Business).Select(t => t.Tabid).Max();
                if (string.IsNullOrWhiteSpace(zxxntabid))
                {
                    xntabid = "1";
                }
                else
                {
                    int lastNumber = int.Parse(zxxntabid);
                    lastNumber++;
                    xntabid = lastNumber.ToString();
                }
                var posBill = billService.GetLastBillId(CurrentInfo.HotelId, refeobj.Id, pos.Business);
                PosBill newbill = new PosBill()
                {
                    Hid = CurrentInfo.HotelId,
                    Billid = posBill.Billid,
                    BillNo = posBill.BillNo,
                    MBillid = posBill.Billid,
                    InputUser = CurrentInfo.UserName,
                    BillDate = DateTime.Now,
                    IsService = true,
                    IsLimit = true,
                    Shiftid = pos.ShiftId,
                    Shuffleid = refeobj.ShuffleId,
                    Refeid = refeobj.Id,
                    BillBsnsDate = pos.Business,
                    TabFlag = (byte)PosBillTabFlag.物理台,
                    Status = (byte)PosBillStatus.开台,
                    Discount = 100,
                    ServiceRate = 0,
                    Tabid = xntabid,
                    TabNo = xntabid
                };
                billService.Add(newbill);
                billService.Commit();
                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 买单实收功能
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.None)]
        public ActionResult UpdateReceivedPayment(BillDiscountModel model)
        {
            try
            {
                var service = GetService<IPosBillService>();
                var BillDetailservice = GetService<IPosBillDetailService>();
                var oldEntity = service.Get(model.Id);//获取实体
                //获取系统参数，
                var paraservice = GetService<IPmsParaService>();
                var IsPayOrderAgain = paraservice.IsPayOrderAgain(CurrentInfo.HotelId);
                if (!IsPayOrderAgain)   //不让二次消费，不允许买单后账单修改
                {
                    if (oldEntity.Status == (byte)PosBillStatus.清台 || oldEntity.Status == (byte)PosBillStatus.结账)
                    {
                        return Json(JsonResultData.Failure("已经买单，清台的账单不能修改折扣"));
                    }
                }
                PosBill newModle = new PosBill();
                PosCommon common = new PosCommon();
                AutoSetValueHelper.SetValues(oldEntity, newModle);
                newModle.DiscAmount = (newModle.DiscAmount == null ? 0 : newModle.DiscAmount) + model.disCount;
                var result = common.CheckOperDiscount(null, model.disCount, oldEntity.Refeid);
                if (!result.Success)
                {
                    return Json(result);
                }
                //输入的金额小于等于0的时候，既不是全单金额折也不是照单金额折
                if (model.disCount <= 0)
                {
                    newModle.DaType = null;
                }
                else { newModle.DaType = 1; }

                //与会员折扣兼容，不会相互替换
                if (newModle.IsForce != 3)
                    newModle.IsForce = 0;

                newModle.Approver = CurrentInfo.UserName;   //折扣人
                service.Update(newModle, oldEntity);
                service.Commit();
                AddOperationLog(OpLogType.Pos账单修改, "照单金额折：" + oldEntity.DaType + "-->" + newModle.DaType + ",折扣类型：" + oldEntity.IsForce + "-->" + newModle.IsForce, newModle.BillNo);
                BillDetailservice.UpdateBillDetailDisc(CurrentInfo.HotelId, oldEntity.Billid, oldEntity.MBillid);
                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.ToString()));
            }
        }
        #endregion

    }
}