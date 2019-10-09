using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// POS买单账务处理接口实现
    /// </summary>
    public class PaymentServicesPos : IPaymentServices
    {
        private IPosBillService _posBillService;
        private IPosRefeService _posRefeService;
        private IPosPosService _posPosService;
        private IPosBillDetailService _posBillDetailService;
        private IPosItemService _posItemService;
        private IPosTabStatusService _posTabStatusService;
        private IPosTabLogService _posTabLogService;

        public PaymentServicesPos(IPosBillService posBillService, IPosRefeService posRefeService, IPosBillDetailService posBillDetailService, IPosPosService posPosService, IPosItemService posItemService, IPosTabStatusService posTabStatusService, IPosTabLogService posTabLogService)
        {
            _posBillService = posBillService;
            _posRefeService = posRefeService;
            _posBillDetailService = posBillDetailService;
            _posPosService = posPosService;
            _posItemService = posItemService;
            _posTabStatusService = posTabStatusService;
            _posTabLogService = posTabLogService;
        }

        
        /// <summary>
        /// 获取指定账单参数对应的账单信息，以用于买单
        /// </summary>
        /// <param name="para">账单参数</param>
        /// <returns>账单信息</returns>
        public PaymentBillInfo GetPaymentBillInfo(PaymentBillPara para)
        {
            var bill = _posBillService.Get(para.BillId);

            var refe = _posRefeService.Get(bill.Refeid);

            var amount = _posBillDetailService.GetBillDetailAmountSumByBillid(para.Hid, para.BillId);
            //重新计算金额
            _posBillDetailService.StatisticsBillDetail(para.Hid, para.BillId, para.MainBillId);

            var paymentTotal = _posBillDetailService.GetBillDetailForPaymentTotalByBillid(para.Hid, para.BillId);
            var amountTail = _posBillDetailService.GetAmountByBillTailProcessing(para.Hid, bill.Refeid, Convert.ToDecimal(paymentTotal.Total));

            return new PaymentBillInfo
            {
                IDecPlace = refe.IDecPlace,
                OpenMemo = bill.OpenMemo,
                CashMemo = bill.CashMemo,
                UnPaid = paymentTotal.UnPaid,
                Amount = amountTail,
                TailDifference = amountTail - amount
            };
        }
        /// <summary>
        /// 修改收银备注
        /// </summary>
        /// <param name="para">修改收银备注</param>
        public void ChangeCashMemo(PaymentOperatePara para)
        {
            var bill = _posBillService.Get(para.Billid);
            if (bill != null)
            {
                var newEntity = new PosBill();
                AutoSetValueHelper.SetValues(bill, newEntity);
                newEntity.CashMemo = para.CashMemo;    //收银备注
                _posBillService.Update(newEntity, bill);
                _posBillService.AddDataChangeLog(OpLogType.Pos账单修改);
                _posBillService.Commit();
            }
        }
        /// <summary>
        /// 增加账务
        /// </summary>
        /// <param name="para">接口参数</param>
        /// <param name="businessPara">其他业务参数</param>
        /// <param name="detailStatu">默认账务状态</param>
        /// <returns>增加账务结果信息</returns>
        public AddBillResult AddBill(PaymentOperatePara para, PaymentOperateBusinessPara businessPara, PosBillDetailStatus detailStatu)
        {
            var bill = _posBillService.Get(para.Billid);

            var refe = _posRefeService.Get(bill.Refeid);

            var pos = _posPosService.Get(businessPara.PosId);

            var item = _posItemService.Get(para.Itemid);

            var paymentTotal = _posBillDetailService.GetBillDetailForPaymentTotalByBillid(businessPara.Hid, para.Billid);
            PosBillDetail billDetail = new PosBillDetail()
            {
                Hid = businessPara.Hid,
                Tabid = bill.Tabid,
                ItemCode = item.Code,
                ItemName = item.Cname,
                DcFlag = PosItemDcFlag.C.ToString(),
                Status = (byte)detailStatu,
                Isauto = (byte)PosBillDetailIsauto.付款,
                IsCheck = true,

                Settleid = businessPara.SettleId,
                SettleDate = DateTime.Now,
                SettleShiftId = pos.ShiftId,
                SettleBsnsDate = pos.Business,
                SettleShuffleid = refe.ShuffleId,
                SettleUser = businessPara.UserName,
                SettleTransno = businessPara.SettleTransNo,
                SettleTransName= businessPara.SettleTransName
            };

            //增加付款明细
            billDetail.Dueamount = para.Amount;
            para.Amount = para.Amount * Convert.ToDecimal(item.Rate ?? 1);
            AutoSetValueHelper.SetValues(para, billDetail);
            _posBillDetailService.Add(billDetail);
            _posBillDetailService.AddDataChangeLog(OpLogType.Pos账单付款明细增加);
            _posBillDetailService.Commit();

            return new AddBillResult
            {
                BillNo = bill.BillNo,
                BillRefeId = bill.Refeid,
                DetailId = billDetail.Id.ToString(),
                DueAmount = billDetail.Dueamount,
                UnpaidAmount = paymentTotal.UnPaid,
                BillBsnsDate = bill.BillBsnsDate,
                BillShiftid = bill.Shiftid,
                BillShuffleid = bill.Shuffleid,
                BillTabId = bill.Tabid,
                PosBusinessDate = pos.Business,
                ItemName = item?.Cname
            };
        }
        /// <summary>
        /// 接口处理成功后更改账务状态
        /// </summary>
        /// <param name="para">接口参数</param>
        /// <param name="businessPara">其他业务参数</param>
        public void ChangeBillStatuWhenSuccess(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            var billDetailId = Convert.ToInt64(businessPara.AddedBillResult.DetailId);
            var billDetail = _posBillDetailService.Get(billDetailId);
            billDetail.Status = (byte)PosBillDetailStatus.正常;
            _posBillDetailService.Update(billDetail, new PosBillDetail());
            _posBillDetailService.AddDataChangeLog(OpLogType.Pos账单付款明细修改);
            _posBillDetailService.Commit();
        }
        /// <summary>
        /// 是否已经全部买单
        /// </summary>
        /// <param name="para">接口参数</param>
        /// <param name="businessPara">其他业务参数</param>
        /// <returns>true:已经全部买单，false:还没有全部买完</returns>
        public bool IsAllPaid(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            var item = _posItemService.Get(para.Itemid);
            var paymentTotalNew = _posBillDetailService.GetBillDetailForPaymentTotalByBillid(businessPara.Hid, para.Billid);
            var rateUnPaid = _posBillDetailService.GetAmountByBillTailProcessing(businessPara.Hid, businessPara.AddedBillResult.BillRefeId, Convert.ToDecimal(businessPara.AddedBillResult.UnpaidAmount / item.Rate));
            businessPara.AmountInfoAfterBillAdded = new AmountInfoAfterBillAdded
            {
                RateUnPaid = rateUnPaid,
                TotalConsume = paymentTotalNew.Consume,
                TotalPaid = paymentTotalNew.Paid,
                Total = paymentTotalNew.Total
            };
            return (paymentTotalNew.Paid >= paymentTotalNew.Total || businessPara.AddedBillResult.DueAmount == rateUnPaid);
        }
        /// <summary>
        /// 添加结账信息
        /// </summary>
        /// <param name="para"></param>
        /// <param name="businessPara"></param>
        public void Settle(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            var pos = _posPosService.Get(businessPara.PosId);
            var refe = _posRefeService.Get(businessPara.AddedBillResult.BillRefeId);
            var posBillDetails = _posBillDetailService.GetBillDetailByDcFlag(businessPara.Hid, para.Billid, PosItemDcFlag.D.ToString()).Where(w => w.Settleid == null).ToList();
            if (posBillDetails != null && posBillDetails.Count > 0)
            {
                foreach (var detail in posBillDetails)
                {
                    detail.IsCheck = true;
                    detail.Settleid = businessPara.SettleId;
                    detail.SettleDate = DateTime.Now;
                    detail.SettleShiftId = pos.ShiftId;
                    detail.SettleBsnsDate = pos.Business;
                    detail.SettleShuffleid = refe.ShuffleId;
                    detail.SettleUser = businessPara.UserName;
                    _posBillDetailService.Update(detail, new PosBillDetail());
                }
                _posBillDetailService.Commit();
            }
        }
        /// <summary>
        /// 处理尾数和抹零
        /// </summary>
        /// <param name="para"></param>
        /// <param name="businessPara"></param>
        public void TailAmount(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            var pos = _posPosService.Get(businessPara.PosId);
            var refe = _posRefeService.Get(businessPara.AddedBillResult.BillRefeId);
            PosBillDetail billDetailWs = new PosBillDetail()
            {
                Hid = businessPara.Hid,
                Billid = para.Billid,
                MBillid = para.MBillid,
                ItemCode = "抹零",
                ItemName = "抹零",
                IsCheck = true,
                DcFlag = PosItemDcFlag.D.ToString(),
                Isauto = (byte)PosBillDetailIsauto.抹零,
                Status = (byte)PosBillDetailStatus.正常,
                IsProduce = (byte)PosBillDetailIsProduce.已出品,

                TransUser = businessPara.UserName,
                TransBsnsDate = businessPara.AddedBillResult.BillBsnsDate,
                TransShiftid = businessPara.AddedBillResult.BillShiftid,
                TransShuffleid = businessPara.AddedBillResult.BillShuffleid,
                TransDate = DateTime.Now,

                Settleid = businessPara.SettleId,
                SettleDate = DateTime.Now,
                SettleShiftId = pos.ShiftId,
                SettleBsnsDate = pos.Business,
                SettleShuffleid = refe.ShuffleId,
                SettleUser = businessPara.UserName,
            };

            //汇率换算导致的差额
            decimal tailDifference = 0;
            if (businessPara.AddedBillResult.DueAmount == businessPara.AmountInfoAfterBillAdded.RateUnPaid)
            {
                tailDifference = Convert.ToDecimal(businessPara.AmountInfoAfterBillAdded.TotalPaid - businessPara.AmountInfoAfterBillAdded.TotalConsume);
                billDetailWs.Dueamount = tailDifference;
                billDetailWs.Amount = tailDifference;
            }
            else if (para.TailDifference != null && para.TailDifference != 0)
            {
                billDetailWs.Dueamount = para.TailDifference;
                billDetailWs.Amount = para.TailDifference;
            }
            businessPara.TailInfoAfterTailAmount = new TailInfoAfterTailAmount { TailDifference = tailDifference };

            //有抹零则修改，无抹零则增加
            var temp = _posBillDetailService.GetEntity(businessPara.Hid, para.Billid, (byte)PosBillDetailIsauto.抹零);
            if (temp == null)
            {
                _posBillDetailService.Add(billDetailWs);
                _posBillDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细增加);
                _posBillDetailService.Commit();
            }
            else
            {
                temp.CanReason = "";
                temp.Dueamount = billDetailWs.Dueamount;
                temp.Amount = billDetailWs.Amount + temp.Amount;
                temp.Status = (byte)PosBillDetailStatus.正常;
                _posBillDetailService.Update(temp, new PosBillDetail());
                _posBillDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                _posBillDetailService.Commit();
            }
        }
        /// <summary>
        /// 找赎
        /// </summary>
        /// <param name="para"></param>
        /// <param name="businessPara"></param>
        public void SmallChange(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            if (businessPara.AmountInfoAfterBillAdded.TotalPaid - businessPara.TailInfoAfterTailAmount.TailDifference > businessPara.AmountInfoAfterBillAdded.Total)
            {
                var smallChange = businessPara.AmountInfoAfterBillAdded.Total - businessPara.AmountInfoAfterBillAdded.TotalPaid;
                para.Amount = smallChange;
                if (para.Amount < 0)
                {
                    var bill = _posBillService.Get(para.Billid);

                    var refe = _posRefeService.Get(bill.Refeid);

                    var pos = _posPosService.Get(businessPara.PosId);

                    var item = _posItemService.Get(para.Itemid);
                    PosBillDetail billDetail = new PosBillDetail()
                    {
                        Hid = businessPara.Hid,
                        Tabid = bill.Tabid,
                        ItemCode = item.Code,
                        ItemName = item.Cname,
                        DcFlag = PosItemDcFlag.C.ToString(),
                        Status = (byte)PosBillDetailStatus.找赎,
                        Isauto = (byte)PosBillDetailIsauto.找赎,
                        Dueamount = smallChange,
                        IsCheck = true,

                        Settleid = businessPara.SettleId,
                        SettleDate = DateTime.Now,
                        SettleShiftId = pos.ShiftId,
                        SettleBsnsDate = pos.Business,
                        SettleShuffleid = refe.ShuffleId,
                        SettleUser = businessPara.UserName,
                    };
                    decimal exchangeRate = smallChange / item.Rate ?? 0;

                    billDetail.Dueamount = _posBillDetailService.GetAmountByBillTailProcessing(businessPara.Hid, bill.Refeid, exchangeRate);

                    AutoSetValueHelper.SetValues(para, billDetail);
                    _posBillDetailService.Add(billDetail);
                    _posBillDetailService.AddDataChangeLog(OpLogType.Pos账单付款明细增加);
                    _posBillDetailService.Commit();
                }
            }
        }
        /// <summary>
        /// 清台
        /// </summary>
        /// <param name="para"></param>
        /// <param name="businessPara"></param>
        public void ClearTable(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            var bill = _posBillService.Get(para.Billid);
            var refe = _posRefeService.Get(bill.Refeid);
            var pos = _posPosService.Get(businessPara.PosId);

            if (refe.Isclrtab == true)
            {
                //修改账单状态
                bill.DepBsnsDate = pos.Business;
                bill.MoveUser = businessPara.UserName;
                bill.DepDate = DateTime.Now;
                bill.Status = (byte)PosBillStatus.清台;

                _posBillService.Update(bill, new PosBill());
                _posBillService.AddDataChangeLog(OpLogType.Pos账单修改);
                _posBillService.Commit();

            }
            else if (refe.Isclrtab == false)
            {
                //修改账单状态
                bill.DepBsnsDate = pos.Business;
                bill.MoveUser = businessPara.UserName;
                bill.DepDate = DateTime.Now;
                if (bill.TabFlag == (byte)PosBillTabFlag.物理台)
                {
                    bill.Status = (byte)PosBillStatus.结账;
                }
                else
                {
                    bill.Status = (byte)PosBillStatus.清台;
                }
                _posBillService.Update(bill, new PosBill());
                _posBillService.AddDataChangeLog(OpLogType.Pos账单修改);
                _posBillService.Commit();
            }
            //修改餐台状态
            var searList = _posBillService.GetSmearListByClearTab(businessPara.Hid, bill.Tabid);
            if (searList != null && searList.Count < 1)
            {
                if (bill.TabFlag == (byte)PosBillTabFlag.物理台)
                {
                    var tabStatus = _posTabStatusService.Get(bill.Tabid);

                    if (tabStatus != null)
                    {
                        var newtabStatus = new PosTabStatus();
                        AutoSetValueHelper.SetValues(tabStatus, newtabStatus);
                        if (bill.Status == (byte)PosBillStatus.清台 || bill.Status == (byte)PosBillStatus.取消 || bill.Status == (byte)PosBillStatus.迟付)
                        {
                            newtabStatus.TabStatus = (byte)PosTabStatusEnum.空净;
                            newtabStatus.OpTabid = null;
                            newtabStatus.OpenGuest = null;
                            newtabStatus.GuestName = null;
                            newtabStatus.OpenRecord = null;
                        }
                        else if (bill.Status == (byte)PosBillStatus.结账)
                        {
                            newtabStatus.TabStatus = (byte)PosTabStatusEnum.已买单未离座;
                        }

                        _posTabStatusService.Update(newtabStatus, tabStatus);
                        _posTabStatusService.AddDataChangeLog(OpLogType.Pos餐台状态修改);
                        _posTabStatusService.Commit();
                    }
                }
            }
            else
            {
                //判断
                var tabStatus = _posTabStatusService.Get(bill.Tabid);
                if (tabStatus != null && tabStatus.OpTabid == bill.Billid)
                {
                    var newtabStatus = new PosTabStatus();
                    AutoSetValueHelper.SetValues(tabStatus, newtabStatus);
                    //if (bill.Status == (byte)PosBillStatus.清台 || bill.Status == (byte)PosBillStatus.取消)
                    //{
                    // newtabStatus.TabStatus = (byte)PosTabStatusEnum.空净;
                    newtabStatus.OpTabid = null;
                    newtabStatus.OpenGuest = null;
                    newtabStatus.GuestName = null;
                    newtabStatus.OpenRecord = null;
                    //} else if (bill.Status == (byte)PosBillStatus.结账)
                    //{
                    //    newtabStatus.TabStatus = (byte)PosTabStatusEnum.已买单未离座;
                    //}

                    _posTabStatusService.Update(newtabStatus, tabStatus);
                    _posTabStatusService.AddDataChangeLog(OpLogType.Pos餐台状态修改);
                    _posTabStatusService.Commit();
                }
            }

            //清理锁台记录
            var tabLogList = _posTabLogService.GetPosTabLogListByTab(businessPara.Hid, bill.Refeid, bill.Tabid, bill.TabNo);
            if (tabLogList != null && tabLogList.Count > 0)
            {
                foreach (var tabLog in tabLogList)
                {
                    if (tabLog.Billid == bill.Billid)
                    {
                        _posTabLogService.Delete(tabLog);
                        _posTabLogService.AddDataChangeLog(OpLogType.Pos锁台删除);
                        _posTabLogService.Commit();
                    }
                }
            }
        }
    }
}
