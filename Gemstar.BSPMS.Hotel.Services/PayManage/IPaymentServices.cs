using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using System;

namespace Gemstar.BSPMS.Hotel.Services.PayManage
{
    /// <summary>
    /// 买单账务处理接口
    /// </summary>
    public interface IPaymentServices
    {


        /// <summary>
        /// 获取指定账单参数对应的账单信息，以用于买单
        /// </summary>
        /// <param name="para">账单参数</param>
        /// <returns>账单信息</returns>
        PaymentBillInfo GetPaymentBillInfo(PaymentBillPara para);

        /// <summary>
        /// 修改收银备注
        /// </summary>
        /// <param name="para">修改收银备注</param>
        void ChangeCashMemo(PaymentOperatePara para);

        /// <summary>
        /// 增加账务信息
        /// </summary>
        /// <param name="para"></param>
        /// <param name="businessPara"></param>
        /// <param name="detailStatu"></param>
        /// <returns></returns>
        AddBillResult AddBill(PaymentOperatePara para, PaymentOperateBusinessPara businessPara, PosBillDetailStatus detailStatu);

        /// <summary>
        /// 接口处理成功后更改账务状态
        /// </summary>
        /// <param name="para">接口参数</param>
        /// <param name="businessPara">其他业务参数</param>
        void ChangeBillStatuWhenSuccess(PaymentOperatePara para, PaymentOperateBusinessPara businessPara);

        /// <summary>
        /// 增加完账务后，再次检查是否已经全部支付完成
        /// </summary>
        /// <param name="para"></param>
        /// <param name="businessPara"></param>
        /// <returns></returns>
        bool IsAllPaid(PaymentOperatePara para, PaymentOperateBusinessPara businessPara);

        /// <summary>
        /// 添加结账信息
        /// </summary>
        /// <param name="para"></param>
        /// <param name="businessPara"></param>
        void Settle(PaymentOperatePara para, PaymentOperateBusinessPara businessPara);

        /// <summary>
        /// 处理尾数和抹零
        /// </summary>
        /// <param name="para"></param>
        /// <param name="businessPara"></param>
        void TailAmount(PaymentOperatePara para, PaymentOperateBusinessPara businessPara);

        /// <summary>
        /// 找赎
        /// </summary>
        /// <param name="para"></param>
        /// <param name="businessPara"></param>
        void SmallChange(PaymentOperatePara para, PaymentOperateBusinessPara businessPara);

        /// <summary>
        /// 清台
        /// </summary>
        /// <param name="para"></param>
        /// <param name="businessPara"></param>
        void ClearTable(PaymentOperatePara para, PaymentOperateBusinessPara businessPara);
    }

    /// <summary>
    /// 买单账务处理时的账单参数
    /// </summary>
    public class PaymentBillPara
    {
        /// <summary>
        /// 当前酒店id
        /// </summary>
        public string Hid { get; set; }

        /// <summary>
        /// 要买单的账单ID
        /// </summary>
        public string BillId { get; set; }

        /// <summary>
        /// 要买单的账单的主单ID
        /// </summary>
        public string MainBillId { get; set; }
    }

    /// <summary>
    /// 买单账务处理时的账单信息
    /// </summary>
    public class PaymentBillInfo
    {
        /// <summary>
        /// 保留位数
        /// </summary>
        public byte? IDecPlace { get; set; }

        /// <summary>
        /// 开台备注
        /// </summary>
        public string OpenMemo { get; set; }

        /// <summary>
        /// 收银备注
        /// </summary>
        public string CashMemo { get; set; }

        /// <summary>
        /// 未付金额
        /// </summary>
        public decimal? UnPaid { get; set; }

        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 尾数处理差额
        /// </summary>
        public decimal? TailDifference { get; set; }
    }

    /// <summary>
    /// 账务处理参数
    /// </summary>
    public class PaymentOperatePara
    {
        /// <summary>
        /// 主单号
        /// </summary>
        public string MBillid { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string Billid { get; set; }

        /// <summary>
        /// 手工单号
        /// </summary>
        public string Acbillno { get; set; }

        /// <summary>
        /// 捷云房号
        /// </summary>
        public string RoomNo { get; set; }

        /// <summary>
        /// 捷云合约单位签单人
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 捷云会员ID
        /// </summary>
        public string ProfileId { get; set; }

        /// <summary>
        /// 捷云会员账户类型
        /// </summary>
        public string BalanceType { get; set; }

        /// <summary>
        /// 捷云会员交易类型
        /// </summary>
        public string PaymentDesc { get; set; }

        /// <summary>
        /// 付款项目
        /// </summary>
        public string Itemid { get; set; }

        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 尾数处理差额
        /// </summary>
        public decimal? TailDifference { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string FolioItemAction { get; set; }

        /// <summary>
        /// 付款方式参数
        /// </summary>
        public string FolioItemActionJsonPara { get; set; }

        /// <summary>
        /// 收银备注
        /// </summary>
        public string CashMemo { get; set; }


        /// <summary>
        /// 挂账公司名称
        /// </summary>
        public string CttName { get; set; }

        /// <summary>
        /// 押金买单是否退款（1：退款，0：不退款）
        /// </summary>
        public string IsRefund { get; set; }

        /// <summary>
        /// 押金单Id
        /// </summary>
        public string PrePayId { get; set; }
    }

    /// <summary>
    /// 账务处理的业务参数
    /// </summary>
    public class PaymentOperateBusinessPara
    {
        /// <summary>
        /// 当前酒店id
        /// </summary>
        public string Hid { get; set; }

        /// <summary>
        /// 收银点id
        /// </summary>
        public string PosId { get; set; }

        /// <summary>
        /// 收银点名称
        /// </summary>
        public string PosName { get; set; }

        /// <summary>
        /// 营业点代码
        /// </summary>
        public string PosOutlteCode { get; set; }

        /// <summary>
        /// 当前用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 结账批次id
        /// </summary>
        public Guid? SettleId { get; set; }

        /// <summary>
        /// 结账交易号，用于线上交易对账
        /// </summary>
        public string SettleTransNo { get; set; }

        /// <summary>
        /// 是否测试环境
        /// </summary>
        public bool IsTestEnv { get; set; }

        /// <summary>
        /// 账单id
        /// </summary>
        public string Billid { get; set; }

        /// <summary>
        /// 营业点id
        /// </summary>
        public string Refeid { get; set; }

        /// <summary>
        /// 餐台id
        /// </summary>
        public string Tabid { get; set; }

        /// <summary>
        /// 是否已付
        /// </summary>
        public byte IsCheck { get; set; }

        /// <summary>
        /// 结账营业日
        /// </summary>
        public DateTime? SettleBsnsDate { get; set; }

        /// <summary>
        /// 结账市别
        /// </summary>
        public string SettleShuffleid { get; set; }

        /// <summary>
        /// 结账班次
        /// </summary>
        public string SettleShiftId { get; set; }

        /// <summary>
        /// 结账用户
        /// </summary>
        public string SettleUser { get; set; }

        /// <summary>
        /// 结账交易名称
        /// </summary>
        public string SettleTransName { get; set; }

        /// <summary>
        /// 餐台状态
        /// </summary>
        public byte? TabStatus { get; set; }

        /// <summary>
        /// 当前开台id
        /// </summary>
        public string OpTabid { get; set; }

        /// <summary>
        /// 离店营业日
        /// </summary>
        public DateTime? DepBsnsDate { get; set; }

        /// <summary>
        /// 离店操作员
        /// </summary>
        public string MoveUser { get; set; }

        /// <summary>
        /// 账单状态
        /// </summary>
        public byte? Status { get; set; }

        /// <summary>
        /// 等待状态
        /// </summary>
        public byte? WaitStatus { get; set; }

        /// <summary>
        /// 计费状态
        /// </summary>
        public byte? BillDetailStatus { get; set; }

        /// <summary>
        /// 自动标志
        /// </summary>
        public byte? Isauto { get; set; }

        /// <summary>
        /// 增加账务后的结果
        /// </summary>
        public AddBillResult AddedBillResult { get; set; }

        /// <summary>
        /// 账务增加后，重新计算的金额信息
        /// </summary>
        public AmountInfoAfterBillAdded AmountInfoAfterBillAdded { get; set; }

        public TailInfoAfterTailAmount TailInfoAfterTailAmount { get; set; }
    }

    /// <summary>
    /// 增加账务结果
    /// </summary>
    public class AddBillResult
    {
        /// <summary>
        /// 单号
        /// </summary>
        public string BillNo { get; set; }

        /// <summary>
        /// 参考号
        /// </summary>
        public string BillRefeId { get; set; }

        /// <summary>
        /// 营业日
        /// </summary>
        public DateTime? BillBsnsDate { get; set; }

        /// <summary>
        /// 班次id
        /// </summary>
        public string BillShiftid { get; set; }

        /// <summary>
        /// 市别id
        /// </summary>
        public string BillShuffleid { get; set; }

        /// <summary>
        /// 餐台id
        /// </summary>
        public string BillTabId { get; set; }

        /// <summary>
        /// 账务明细Id
        /// </summary>
        public string DetailId { get; set; }

        /// <summary>
        /// 原币金额
        /// </summary>
        public decimal? DueAmount { get; set; }

        /// <summary>
        /// 未付金额
        /// </summary>
        public decimal? UnpaidAmount { get; set; }

        /// <summary>
        /// 收银点对应的当前营业日
        /// </summary>
        public DateTime? PosBusinessDate { get; set; }

        /// <summary>
        /// 支付方式名称
        /// </summary>
        public string ItemName { get; set; }
    }

    public class AmountInfoAfterBillAdded
    {
        /// <summary>
        /// 总付款金额
        /// </summary>
        public decimal? TotalPaid { get; set; }

        /// <summary>
        /// 总消费金额
        /// </summary>
        public decimal? TotalConsume { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal? Total { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal RateUnPaid { get; set; }
    }

    /// <summary>
    /// 尾数抹零处理后的相关信息，以便后续进行处理找赎等
    /// </summary>
    public class TailInfoAfterTailAmount
    {
        /// <summary>
        /// 尾数抹零处理后的差额
        /// </summary>
        public decimal TailDifference { get; set; }
    }
}