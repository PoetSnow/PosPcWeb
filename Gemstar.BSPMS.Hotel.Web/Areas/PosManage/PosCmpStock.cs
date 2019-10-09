using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage
{
    /// <summary>
    /// 云Pos 连接云仓库处理
    /// </summary>
    public class PosCmpStock : BaseController
    {
        public string errNo = "1";
        /// <summary>
        /// 添加账单明细数据
        /// </summary>
        /// <param name="model"></param>
        public void AddBillDetail(Models.PosInSingle.PosBillDetailAddViewModel model, out PosBillDetail billDetail)
        {


            var billService = GetService<IPosBillService>();
            var service = GetService<IPosBillDetailService>();
            PosBillDetail AddbillDetail = new PosBillDetail();

            //消费项目信息
            var itemService = GetService<IPosItemService>();
            var item = itemService.GetEntity(CurrentInfo.HotelId, model.Itemid);

            //获取单位信息
            var unitService = GetService<IPosUnitService>();
            var unit = unitService.GetEntity(CurrentInfo.HotelId, model.Unitid);

            //消费项目价格
            var itemPriceService = GetService<IPosItemPriceService>();
            var itemPrice = itemPriceService.GetPosItemPriceByUnitid(CurrentInfo.HotelId, model.Itemid, model.Unitid);

            var PosOnSaleService = GetService<IPosOnSaleService>();//特价菜

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

            //时价菜的会员价取 填写的价格
            if (model.IsCurrent != null && model.IsCurrent == true)
            {
                memberprice = model.Price;
            }

            //设置会员价
            model.PriceClub = memberprice;
            model.PriceOri = model.Price ?? 0;// itemPrice.Price;
            #endregion

            var bill = billService.Get(model.Billid);

            var tabService = GetService<IPosTabService>();//餐台服务
            var tab = tabService.Get(bill.Tabid);

            bill.ServiceRate = bill.ServiceRate >= 1 && bill.ServiceRate <= 100 ? bill.ServiceRate / 100 : bill.ServiceRate;
            bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount;



            //验证本次添加的菜式是否存在
            bool isExists = service.IsExistsForLD(CurrentInfo.HotelId, bill.Billid, model.Itemid);
            if (isExists)
            {
                if (string.IsNullOrEmpty(model.itagperiod))//本次添加的项目项目不是特价菜
                {
                    billDetail = service.GetBillDetailByBillidForLDByTJC(CurrentInfo.HotelId, bill.Billid, model.Itemid, PosBillDetailIsauto.特价菜);
                    if (billDetail != null)
                    {

                        var oldBillDetail = new PosBillDetail();
                        AutoSetValueHelper.SetValues(billDetail, oldBillDetail);


                        #region 赋值
                        if (billDetail.Unitid != model.Unitid)
                        {
                            billDetail.Unitid = model.Unitid;
                            billDetail.UnitCode = unit.Code;
                            billDetail.UnitName = unit.Cname;
                            billDetail.Price = model.Price;// itemPrice.Price;
                            billDetail.IsProduce = (byte)PosBillDetailIsProduce.修改;
                            billDetail.Dueamount = model.Price * billDetail.Quantity;
                            billDetail.Price = model.Price;
                            billDetail.PriceClub = memberprice; //设置会员价
                            billDetail.PriceOri = model.Price; //保存原价
                            billDetail.Piece = model.Price;
                        }
                        else
                        {
                            //海鲜称重 的数量
                            model.Quantity = model.OriQuan == null ? model.Quantity : model.OriQuan;
                            billDetail.OriQuan += model.OriQuan;
                            billDetail.Piece += model.Piece;
                            billDetail.Price = model.Price;
                            billDetail.PriceClub = memberprice; //设置会员价
                            billDetail.PriceOri = model.Price; //保存原价
                            billDetail.Quantity = billDetail.Quantity + model.Quantity;
                            billDetail.Dueamount = model.Price * billDetail.Quantity;
                        }

                        //根据特价菜计算金额
                        billDetail.Discount = billDetail.Discount >= 1 && billDetail.Discount <= 100 ? billDetail.Discount / 100 : billDetail.Discount;
                        var serviceAmount = item.IsService == true ? billDetail.Price * billDetail.Quantity * bill.ServiceRate : 0;
                        var amount = item.IsDiscount == true ? (billDetail.Price * billDetail.Quantity - billDetail.DiscAmount) * billDetail.Discount
                            : (billDetail.Price * billDetail.Quantity);

                        billDetail.Amount = amount;
                        billDetail.Service = serviceAmount;



                        #endregion 赋值

                        service.Update(billDetail, oldBillDetail);
                        service.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，名称：" + billDetail.ItemName + ",数量：" + oldBillDetail.Quantity + "-->" + billDetail.Quantity.ToString() + ",单位：" + oldBillDetail.UnitName + "-->" + billDetail.UnitName + ",金额：" + oldBillDetail.Quantity * oldBillDetail.Price + "-->" + billDetail.Dueamount + ",状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status), bill.BillNo);

                    }
                    else
                    {
                        var dueamount = model.Price * model.Quantity;
                        var amount = item.IsDiscount == true ? dueamount * bill.Discount : dueamount;
                        var serviceAmount = item.IsService == true ? dueamount * bill.ServiceRate : 0;
                        #region 赋值
                        AddbillDetail = new PosBillDetail()
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
                            IsProduce = (byte)PosBillDetailIsProduce.未出品,
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
                            IsWeight = false
                        };
                        model.Quantity = model.OriQuan != null ? model.OriQuan : model.Quantity;
                        #endregion 赋值
                        AutoSetValueHelper.SetValues(model, AddbillDetail);

                        AddbillDetail.OrderType = "PC";
                        AddbillDetail.IsWeight = false;
                        service.Add(AddbillDetail);
                        //  service.AddDataChangeLog(OpLogType.Pos账单消费明细增加);
                        service.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + billDetail.Id + ",名称：" + AddbillDetail.ItemName + ",数量：" + model.Quantity + "-->" + AddbillDetail.Quantity.ToString() + ",单位：" + model.UnitName + "-->" + AddbillDetail.UnitName + ",金额：" + model.Price * model.Quantity + "-->" + AddbillDetail.Dueamount.ToString() + ",状态：" + Enum.GetName(typeof(PosBillDetailStatus), AddbillDetail.Status), bill.BillNo);

                    }
                }
                else
                {
                    //本次添加的是特价菜
                    billDetail = service.GetBillDetailByBillidForLD(CurrentInfo.HotelId, bill.Billid, model.Itemid, PosBillDetailIsauto.特价菜);
                    if (billDetail != null)
                    {
                        //是特价菜 修改特价菜数据
                        var oldBillDetail = new PosBillDetail();
                        AutoSetValueHelper.SetValues(billDetail, oldBillDetail);
                        //var oldBillDetail = new PosBillDetail() { Amount = billDetail.Amount, Status = billDetail.Status, Quantity = model.Quantity, Service = billDetail.Service, Unitid = billDetail.Unitid, UnitCode = billDetail.UnitCode, UnitName = billDetail.UnitName };

                        #region 赋值
                        if (billDetail.Unitid != model.Unitid)
                        {
                            billDetail.Unitid = model.Unitid;
                            billDetail.UnitCode = unit.Code;
                            billDetail.UnitName = unit.Cname;
                            billDetail.Price = model.Price;
                            billDetail.IsProduce = (byte)PosBillDetailIsProduce.修改;
                            billDetail.Dueamount = model.Price * billDetail.Quantity;
                            billDetail.Price = model.Price;
                            billDetail.PriceClub = model.Price; //特价菜会员价设置为输入价格
                            billDetail.PriceOri = model.Price; //保存原价
                            billDetail.Piece = model.Price;
                        }
                        else
                        {
                            billDetail.Quantity = billDetail.Quantity + model.Quantity;
                            billDetail.Dueamount = model.Price * billDetail.Quantity;
                        }

                        //获取所有符合条件的特价菜
                        var PosOnSaleModel = PosOnSaleService.GetPosOnSaleByItemId(CurrentInfo.HotelId, model.Itemid, bill.Refeid, model.itagperiod, bill.CustomerTypeid, tab.TabTypeid, model.Unitid);

                        //根据特价菜计算金额
                        billDetail.Discount = PosOnSaleModel.Discount = PosOnSaleModel.Discount == null ? 1 : PosOnSaleModel.Discount >= 1 && PosOnSaleModel.Discount <= 100 ? PosOnSaleModel.Discount / 100 : PosOnSaleModel.Discount;

                        var serviceAmount = item.IsService == true ? billDetail.Price * billDetail.Quantity * bill.ServiceRate : 0;
                        var amount = PosOnSaleModel.IsDiscount == true ? (billDetail.Price * billDetail.Quantity - billDetail.DiscAmount) * billDetail.Discount
                            : (billDetail.Price * billDetail.Quantity);

                        billDetail.Amount = amount;
                        billDetail.Service = serviceAmount;


                        #endregion 赋值

                        service.Update(billDetail, new PosBillDetail());
                        service.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + ",名称：" + billDetail.ItemName + ",数量：" + oldBillDetail.Quantity + "-->" + billDetail.Quantity.ToString() + ",单位：" + oldBillDetail.UnitName + "-->" + billDetail.UnitName + ",金额：" + oldBillDetail.Dueamount + "-->" + billDetail.Dueamount + ",状态：" + Enum.GetName(typeof(PosBillDetailStatus), oldBillDetail.Status) + "-->" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status), bill.BillNo);

                    }
                    else
                    {
                        //获取所有符合条件的特价菜
                        var PosOnSaleModel = PosOnSaleService.GetPosOnSaleByItemId(CurrentInfo.HotelId, model.Itemid, bill.Refeid, model.itagperiod, bill.CustomerTypeid, tab.TabTypeid, model.Unitid);

                        PosOnSaleModel.Discount = PosOnSaleModel.Discount == null ? 1 : PosOnSaleModel.Discount >= 1 && PosOnSaleModel.Discount <= 100 ? PosOnSaleModel.Discount / 100 : PosOnSaleModel.Discount;

                        var dueamount = model.Price * model.Quantity;
                        var amount = PosOnSaleModel.IsDiscount == true ? dueamount * PosOnSaleModel.Discount : dueamount;
                        var serviceAmount = PosOnSaleModel.IsService == true ? dueamount * bill.ServiceRate : 0;


                        #region 赋值
                        AddbillDetail = new PosBillDetail()
                        {
                            Hid = CurrentInfo.HotelId,
                            ItemCode = item.Code,
                            ItemName = item.Cname,
                            UnitCode = unit.Code,
                            UnitName = unit.Cname,
                            Price = model.Price,
                            DcFlag = PosItemDcFlag.D.ToString(),
                            IsCheck = false,
                            Isauto = (byte)PosBillDetailIsauto.特价菜,
                            Status = (byte)PosBillDetailStatus.保存,
                            IsProduce = (byte)PosBillDetailIsProduce.未出品,
                            Dueamount = dueamount,
                            //  DiscAmount = discAmount,
                            Discount = PosOnSaleModel.Discount,
                            Amount = amount,
                            Service = serviceAmount,
                            SP = false,
                            SD = false,
                            TransUser = CurrentInfo.UserName,
                            TransBsnsDate = bill.BillBsnsDate,
                            TransShiftid = bill.Shiftid,
                            TransShuffleid = bill.Shuffleid,
                            TransDate = DateTime.Now,
                            IsWeight = false
                        };
                        #endregion 赋值
                        AutoSetValueHelper.SetValues(model, AddbillDetail);
                        AddbillDetail.OrderType = "PC";

                        AddbillDetail.IsWeight = false;    //添加海鲜消费项目默认是未称重
                        service.Add(AddbillDetail);
                        // service.AddDataChangeLog(OpLogType.Pos账单消费明细增加);
                        service.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + billDetail.Id + ",名称：" + AddbillDetail.ItemName + ",数量：" + AddbillDetail.Quantity.ToString() + ",单位：" + AddbillDetail.UnitName + ",金额：" + AddbillDetail.Dueamount.ToString() + ",状态：" + Enum.GetName(typeof(PosBillDetailStatus), AddbillDetail.Status), bill.BillNo);

                    }
                }
            }
            else
            {
                model.Quantity = model.OriQuan == null ? model.Quantity : model.OriQuan;
                decimal? dueamount = model.Price * model.Quantity;
                decimal? amount;
                decimal? serviceAmount;
                byte? Isauto;
                if (string.IsNullOrEmpty(model.itagperiod))
                {

                    //   var discAmount = model.IsDiscount == true ? dueamount - (dueamount * bill.Discount) : 0;
                    amount = item.IsDiscount == true ? dueamount * bill.Discount : dueamount;
                    serviceAmount = item.IsService == true ? dueamount * bill.ServiceRate : 0;
                    Isauto = (byte)PosBillDetailIsauto.录入项目;
                }
                else
                {
                    var PosOnSaleModel = PosOnSaleService.GetPosOnSaleByItemId(CurrentInfo.HotelId, model.Itemid, bill.Refeid, model.itagperiod, bill.CustomerTypeid, tab.TabTypeid, model.Unitid);
                    amount = PosOnSaleModel.IsDiscount == true ? dueamount * PosOnSaleModel.Discount : dueamount;
                    serviceAmount = PosOnSaleModel.IsService == true ? dueamount * bill.ServiceRate : 0;
                    Isauto = (byte)PosBillDetailIsauto.特价菜;
                }


                #region 赋值
                billDetail = new PosBillDetail()
                {
                    Hid = CurrentInfo.HotelId,
                    ItemCode = item.Code,
                    ItemName = item.Cname,
                    UnitCode = model.Unitid,
                    UnitName = unit.Cname,
                    Price = model.Price,
                    DcFlag = PosItemDcFlag.D.ToString(),
                    IsCheck = false,
                    Isauto = Isauto,
                    Status = (byte)PosBillDetailStatus.保存,
                    IsProduce = (byte)PosBillDetailIsProduce.未出品,
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
                    IsWeight = false
                };

                #endregion 赋值

                AutoSetValueHelper.SetValues(model, billDetail);
                billDetail.OrderType = "PC";
                billDetail.IsWeight = false;    //添加海鲜消费项目默认是未称重
                service.Add(billDetail);
                //  service.AddDataChangeLog(OpLogType.Pos账单消费明细增加);
                service.Commit();
                AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + billDetail.Id + ",名称：" + billDetail.ItemName + ",数量：" + billDetail.Quantity.ToString() + ",单位：" + billDetail.UnitName + ",金额：" + billDetail.Dueamount.ToString() + ",状态：" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status), bill.BillNo);

            }


        }


        /// <summary>
        /// 验证是否连击云仓库
        /// </summary>
        /// <param name="model"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public JsonResult IsPosReduceStock(Models.PosInSingle.PosBillDetailAddViewModel model, PosItem item, PosUnit unit)
        {


            var pmsParaService = GetService<IPmsParaService>();
            //是否连接云仓库
            var PosIsConnectWareHouse = pmsParaService.GetValue(CurrentInfo.HotelId, "PosIsConnectWareHouse");

            //连接云仓库库存控制
            var PosConnectWareHouseValue = pmsParaService.GetValue(CurrentInfo.HotelId, "PosConnectWareHouseValue");

            PosBillDetail billDetail = new PosBillDetail();
            string errMsg = "";
            if (PosIsConnectWareHouse == "0")
            {
                //不连接云仓库
                try
                {
                    AddBillDetail(model, out billDetail);

                    return Json(JsonResultData.Successed());
                }
                catch
                {
                    return Json(JsonResultData.Failure("添加消费项目失败！"));
                }
            }
            else
            {
                if (item.IsStock == (byte)PosItemIsStock.减库存)
                {

                    if (PosConnectWareHouseValue == "0")
                    {
                        //不判断库存
                        AddPosBillCost(item, unit, model, out errMsg);
                        if (!string.IsNullOrWhiteSpace(errMsg))
                        {
                            return Json(JsonResultData.Failure(errMsg));
                        }
                        return Json(JsonResultData.Successed());
                    }
                    else if (PosConnectWareHouseValue == "1")
                    {

                        AddPosBillCost(item, unit, model, out errMsg);
                        if (!string.IsNullOrWhiteSpace(errMsg) && errNo == "1")
                        {
                            return Json(JsonResultData.Failure(errMsg, 3));
                        }
                        else
                        {
                            if (errNo != "1")
                            {
                                return Json(JsonResultData.Failure("库存不足，是否继续添加？", 1));
                            }

                        }


                    }
                    else if (PosConnectWareHouseValue == "2")
                    {

                        AddPosBillCost(item, unit, model, out errMsg);
                        if (!string.IsNullOrWhiteSpace(errMsg))
                        {
                            return Json(JsonResultData.Failure(errMsg));
                        }
                        return Json(JsonResultData.Successed());
                    }
                    else if (PosConnectWareHouseValue == "3")
                    {
                        return Json(JsonResultData.Failure("添加失败，库存不足", 3));
                    }
                }
                else
                {
                    //不减库存 直接添加消费项目
                    AddBillDetail(model, out billDetail);
                    return Json(JsonResultData.Successed());
                }

            }
            return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 添加消费项目仓库耗用表
        /// </summary>
        /// <param name="item"></param>
        /// <param name="unit"></param>
        public bool AddPosBillCost(PosItem item, PosUnit unit, Models.PosInSingle.PosBillDetailAddViewModel model, out string errMsg, string Flag = "0")
        {
            errMsg = "";
            //物品组成
            var service = GetService<IPosCostItemService>();

            PosDepart depart = new PosDepart(); //出品部门

            PosRefe refe = new PosRefe();  //营业点

            PosDepot depot = new PosDepot(); //二级仓库

            var depotService = GetService<IPosDepotService>();

            var posBillCostService = GetService<IPosBillCostService>();

            //修改单位的情况下 先删除现有的库存数据
            var detailService = GetService<IPosBillDetailService>();

            PosBillDetail detail = null;
            if (string.IsNullOrEmpty(model.itagperiod))//本次添加的项目项目不是特价菜
            {
                detail = detailService.GetBillDetailByBillidForLDByTJC(CurrentInfo.HotelId, model.Billid, model.Itemid, PosBillDetailIsauto.特价菜);

            }
            else
            {
                detail = detailService.GetBillDetailByBillidForLD(CurrentInfo.HotelId, model.Billid, model.Itemid, PosBillDetailIsauto.特价菜);
            }
            if (detail != null)
            {
                if (detail.Unitid != model.Unitid)
                {
                    var billCostList = posBillCostService.GetBillCostList(CurrentInfo.HotelId, CurrentInfo.ModuleCode, detail.Id).Where(w => w.Unitid == detail.Unitid).ToList();
                    foreach (var billcost in billCostList)
                    {
                        posBillCostService.Delete(billcost);

                        posBillCostService.Commit();
                    }

                }

            }


            //获取物品列表
            var costList = service.GetListPostCostByItemId(CurrentInfo.HotelId, item.Id, unit.Id);
            if (costList != null && costList.Count > 0)
            {
                //获取账单信息
                var billService = GetService<IPosBillService>();
                var bill = billService.Get(model.Billid);

                #region 判断物理台是否有设置出品部门，快餐台营业点是否设置出品部门
                var tabFlag = bill.TabFlag; //餐台标识：（0：物理台，1：快餐台，2：外卖台）
                if (tabFlag == (byte)PosBillTabFlag.物理台)
                {
                    //获取餐台上选择的出品部门
                    var tabService = GetService<IPosTabService>();
                    var tab = tabService.Get(model.Tabid);

                    if (string.IsNullOrWhiteSpace(tab.DeptDepart))
                    {
                        errMsg = "餐台未设置出品部门";
                        return false;
                    }
                    else
                    {
                        var deptDepart = tab.DeptDepart;   //出品部门
                        //获取出品部门
                        var departService = GetService<IPosDepartService>();
                        depart = departService.GetDepartByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode).Where(w => w.DeptClassID == item.DeptClassid && deptDepart.Contains(w.Id)).FirstOrDefault();
                        if (depart == null)
                        {
                            errMsg = "消费项目未设置出品部门";
                            return false;
                        }
                    }
                }
                else
                {
                    //外卖台，快餐台取营业点上的二级仓库
                    var refeService = GetService<IPosRefeService>();
                    refe = refeService.Get(bill.Refeid);
                    if (string.IsNullOrWhiteSpace(refe.DepartNo))
                    {
                        errMsg = "营业点未设置二级仓库";
                        return false;
                    }
                }
                #endregion


                //二级仓库
                if (tabFlag == 0)
                {
                    depot = depotService.Get(depart.WhCode);
                }
                else
                {
                    depot = depotService.Get(refe.DepartNo);
                }

                //判断库存是否足够
                if (Flag == "0")
                {
                    var checkVal = CheckStockByItemCode(costList, depot.Code, model.Quantity, bill.BillBsnsDate, out errMsg);
                    if (!checkVal)
                    {
                        errNo = "-1";
                        return false;
                    }
                }


                //消费项目以及物品
                var itemService = GetService<IPosItemService>();


                PosBillDetail billDetail = new PosBillDetail();

                //添加消费项目明细数据以及 消费项目仓库耗用数据
                AddBillDetail(model, out billDetail);
                foreach (var cost in costList)
                {
                    //根据项目ID 以及单位ID 判断是否存在耗用数据
                    var billCost = posBillCostService.GetBillCost(CurrentInfo.HotelId, CurrentInfo.ModuleCode, billDetail.Id, cost.CostItemid);

                    var costItem = itemService.Get(cost.CostItemid);
                    if (billCost != null)
                    {
                        //存在数据，修改数据 
                        billCost.Quantity = cost.OriQuan * billDetail.Quantity;
                        billCost.Amount = cost.OriQuan * billDetail.Quantity * cost.Price;
                        billCost.Unitid = model.Unitid;
                        billCost.ModiUser = CurrentInfo.UserName;
                        billCost.Modifieddate = DateTime.Now;
                        posBillCostService.Update(billCost, new PosBillCost());
                        posBillCostService.AddDataChangeLog(OpLogType.Pos消费项目仓库耗用表修改);
                        posBillCostService.Commit();
                    }
                    else
                    {



                        //不存在 则新增数据
                        #region 消费项目仓库耗用表赋值

                        var billDate = Convert.ToDateTime(billDetail.TransBsnsDate);
                        var billMonth = billDate.Month.ToString().Length == 1 ? "0" + billDate.Month.ToString() : billDate.Month.ToString();
                        var whNo = billDate.Year.ToString() + billMonth + billDate.Day.ToString();
                        billCost = new PosBillCost
                        {
                            Id = Guid.NewGuid(),
                            Hid = CurrentInfo.HotelId,
                            PostSysName = CurrentInfo.ModuleCode,
                            BillID = billDetail.Id,
                            BillBsnsDate = billDetail.TransBsnsDate,
                            Itemid = item.Id,
                            Unitid = unit.Id,
                            Status = billDetail.Status,
                            WhBillNo = whNo + depot.Code + costItem.Code,    //出品部门二级仓库加物品编码
                            WhCode = depart.WhCode,
                            CostItemid = cost.CostItemid,
                            CostItemUnitid = cost.CostItemUnitid,
                            OriQuan = cost.OriQuan,
                            Quantity = cost.OriQuan * billDetail.Quantity,       //实际数量=组成数量*账单明细数量
                            XRate = 1,
                            Price = cost.Price,                             //添加的时候默认为物品的单价
                            Amount = cost.OriQuan * billDetail.Quantity * cost.Price,
                            IsBuildBill = false,
                            IsCheckPrice = false,
                            TransUser = CurrentInfo.UserName,
                            TransDate = DateTime.Now,


                            Remark = "",
                            OutCodeNo = costItem.OutCodeNo      //外部代码
                        };

                        #endregion

                        posBillCostService.Add(billCost);
                        posBillCostService.AddDataChangeLog(OpLogType.Pos消费项目仓库耗用表添加);
                        posBillCostService.Commit();
                    }


                }
                return true;
            }
            else
            {
                //没有物品组成的直接添加消费项目
                PosBillDetail billDetail = new PosBillDetail();
                AddBillDetail(model, out billDetail);
                return true;
            }

        }

        /// <summary>
        /// 判断库存数据是否足够
        /// </summary>
        /// <param name="costList">消费项目物品组成列表</param>
        /// <param name="depotNo">二级仓库编码</param>
        /// <returns></returns>
        public bool CheckStockByItemCode(List<PostCost> costList, string depotNo, decimal? quantity, DateTime? billBsnsDate, out string errMsg)
        {
            errMsg = "";
            var itemService = GetService<IPosItemService>();    //物品详细信息接口

            var billCostService = GetService<IPosBillCostService>();

            foreach (var cost in costList)
            {
                var costItem = itemService.Get(cost.CostItemid);
                var result = PostStockByItemCode(costItem.OutCodeNo, CurrentInfo.HotelId, depotNo);
                if (result != null)
                {
                    if (result.ErrorNo == "0")
                    {
                        //库存小于等于0直接抛出错误
                        if (Convert.ToDecimal(result.Data.FirstOrDefault().StockQuantity) <= 0)
                        {
                            errMsg = "点菜失败" + costItem.Cname + "库存不足";
                            return false;
                        }
                        else
                        {
                            var billCostQuantity = billCostService.GetBillCostSumQuantity(CurrentInfo.HotelId, CurrentInfo.ModuleCode, billBsnsDate, CurrentInfo.HotelId + depotNo, cost.CostItemid);
                            var quantityVal = Convert.ToDecimal(result.Data.FirstOrDefault().StockQuantity) - billCostQuantity < quantity * cost.OriQuan;
                            if (quantityVal)
                            {
                                errMsg = "点菜失败" + costItem.Cname + "库存不足";
                                return false;
                            }
                        }

                    }
                    else
                    {
                        errMsg = result.Msg;
                        return false;
                    }
                }
                else
                {
                    errMsg = "物品数据不存在";
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 调用接口查询物品数据
        /// </summary>
        /// <param name="itemCode">物品编码</param>
        /// <param name="resortId">酒店ID</param>
        /// <param name="depotNo">部门编码</param>
        public StockByItemCodeResult PostStockByItemCode(string itemCode, string resortId, string depotNo)
        {
            var pmsParaService = GetService<IPmsParaService>();
            var postUrl = pmsParaService.GetValue(CurrentInfo.HotelId, "PosStockByItemCodeUrl");

            var httpItem = new ScanOrder.Models.HttpItem();
            httpItem.Method = "post";
            httpItem.PostEncoding = Encoding.UTF8;
            httpItem.URL = postUrl;
            httpItem.ContentType = "application/x-www-form-urlencoded";
            httpItem.Postdata = $"itemCode={itemCode}&resortId={resortId}&depotNo={depotNo}";

            var httpHelper = new ScanOrder.Models.HttpHelper();
            var html = httpHelper.GetHtml(httpItem);
            StockByItemCodeResult result = null;
            if (html.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<StockByItemCodeResult>(html.Html);
            }
            return result;
        }

        public bool PosCostItemAnsyc()
        {
            //获取云仓库接口地址
            var pmsParaService = GetService<IPmsParaService>();
            var postUrl = pmsParaService.GetValue(CurrentInfo.HotelId, "PosCostItemsInfoUrl");

            var httpItem = new ScanOrder.Models.HttpItem();
            httpItem.Method = "post";
            httpItem.PostEncoding = Encoding.UTF8;
            httpItem.URL = postUrl;
            httpItem.ContentType = "application/x-www-form-urlencoded";
            httpItem.Postdata = $"";

            var httpHelper = new ScanOrder.Models.HttpHelper();
            var html = httpHelper.GetHtml(httpItem);
            StockByItemCodeResult result = null;
            if (html.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<StockByItemCodeResult>(html.Html);
            }

            return true;
        }

    }

    /// <summary>
    /// 接口返回结果
    /// </summary>
    public class StockByItemCodeResult
    {
        /// <summary>
        /// 请求结果 0：成功 ，1：失败
        /// </summary>
        public string ErrorNo { get; set; }

        /// <summary>
        /// 请求消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 请求成功返回的结果
        /// </summary>
        public List<StockByItemCodeData> Data { get; set; }
    }

    public class StockByItemCodeData
    {
        /// <summary>
        /// 物品编码
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 物品名称
        /// </summary>
        public string ItemCname { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string DisplayUnit { get; set; }

        /// <summary>
        /// 库存数
        /// </summary>
        public string StockQuantity { get; set; }

        /// <summary>
        /// 含税价
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// 库存价
        /// </summary>
        public string WiPrice { get; set; }
    }




}