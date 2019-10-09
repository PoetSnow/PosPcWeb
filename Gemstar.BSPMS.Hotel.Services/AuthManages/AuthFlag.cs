using System;

namespace Gemstar.BSPMS.Hotel.Services.AuthManages
{
    /// <summary>
    /// 权限项,权限项必须是2的次方。以后增加时就是在最大值的基础上乘以2进行赋值
    /// </summary>
    [Flags]
    public enum AuthFlag:Int64
    {
        /// <summary>
        /// 所有权限
        /// </summary>
        All = -1,
        /// <summary>
        /// 无权限
        /// </summary>
        None = 0,
        /// <summary>
        /// 查询
        /// </summary>
        Query = 1,
        /// <summary>
        /// 增加
        /// </summary>
        Add = 2,
        /// <summary>
        /// 修改
        /// </summary>
        Update = 4,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 8,
        /// <summary>
        /// 导出
        /// </summary>
        Export = 16,
        /// <summary>
        /// 打印
        /// </summary>
        Print = 32,
        /// <summary>
        /// 明细
        /// </summary>
        Details = 64,
        /// <summary>
        /// 成员
        /// </summary>
        Members = 128,
        /// <summary>
        /// 权限维护
        /// </summary>
        AuthManage = 256,
        /// <summary>
        /// 重置
        /// </summary>
        Reset = 512,
        /// <summary>
        /// 禁用
        /// </summary>
        Disable = 1024,
        /// <summary>
        /// 启用
        /// </summary>
        Enable = 2048,
        /// <summary>
        /// 打开
        /// </summary>
        Open = 4096,
        /// <summary>
        /// 消费
        /// </summary>
        Consume = 8192,
        /// <summary>
        /// 入住
        /// </summary>
        CheckIn = 16384,
        /// <summary>
        /// 帐务
        /// </summary>
        Accounting = 32768,
        /// <summary>
        /// 设置为脏房
        /// </summary>
        SetDirty = 65536,
        /// <summary>
        /// 设置为清洁房
        /// </summary>
        SetWaitClean = 131072,
        /// <summary>
        /// 设置为净房
        /// </summary>
        SetClean = 262144,
        /// <summary>
        /// 维修
        /// </summary>
        Service = 524288,
        /// <summary>
        /// 停用
        /// </summary>
        Stop = 1048576,
        /// <summary>
        /// 报表权限维护
        /// </summary>
        ReportAuthManage = 2097152,
        /// <summary>
        /// 关闭班次
        /// </summary>
        Close = 4194304,
        /// <summary>
        /// 更换业务员
        /// </summary>
        ReplaceSalesman = 8388608,
        /// <summary>
        /// 换卡号
        /// </summary>
        ChangeCardNum = 16777216,
        /// <summary>
        /// 升级卡类型
        /// </summary>
        UpgradeCard = 33554432,
        /// <summary>
        /// 变更卡状态
        /// </summary>
        UpdateCardStatus = 67108864,
        /// <summary>
        /// 审核
        /// </summary>
        Inspect = 134217728,
        /// <summary>
        /// 会员充值
        /// </summary>
        MemberRecharge = 268435456,
        /// <summary>
        /// 会员扣款
        /// </summary>
        MemberDebit = 536870912,
        /// <summary>
        /// 积分兑换
        /// </summary>
        IntegralExchange = 1073741824,
        /// <summary>
        /// 积分调整
        /// </summary>
        IntegralAdjustment = 2147483648,
        /// <summary>
        /// 变更记录
        /// </summary>
        UpdateRecord = 4294967296,
        /// <summary>
        /// 交易记录
        /// </summary>
        transactionRecord = 8589934592,
        /// <summary>
        /// 消费记录
        /// </summary>
        consumptionRecord = 17179869184,
        /// <summary>
        /// 积分兑换记录
        /// </summary>
        IntegrarChRecord = 34359738368,
        /// <summary>
        /// 批量延期
        /// </summary>
        BatchDelay = 68719476736,
        /// <summary>
        /// 取消预订
        /// </summary>
        CancelOrderDetailY= 137438953472,
        /// <summary>
        /// 取消入住
        /// </summary>
        CancelOrderDetailZ= 274877906944,
        /// <summary>
        /// 恢复入住
        /// </summary>
        RecoveryOrderDetailZ= 549755813888,
        /// <summary>
        /// 恢复预订
        /// </summary>
        RecoveryOrderDetailY= 1099511627776,
        /// <summary>
        /// 结账
        /// </summary>
        CheckoutCheck= 2199023255552,
        /// <summary>
        /// 迟付
        /// </summary>
        Out= 4398046511104,
        /// <summary>
        /// 清账
        /// </summary>
        ClearCheck= 8796093022208,
        /// <summary>
        /// 预授权
        /// </summary>
        AddCardAuth= 17592186044416,
        /// <summary>
        /// 转账
        /// </summary>
        Transfer= 35184372088832,
        /// <summary>
        /// 取消结账
        /// </summary>
        CancelCheckout= 70368744177664,
        /// <summary>
        /// 取消清账
        /// </summary>
        CancelClear= 140737488355328,
        /// <summary>
        /// 取消离店
        /// </summary>
        CancelOut= 281474976710656,
        /// <summary>
        /// 输入积分
        /// </summary>
        InputIntegra= 562949953421312,
        /// <summary>
        /// 账务作废与账务恢复
        /// </summary>
        CancelAndRecovery= 1125899906842624,

        /// <summary>
        /// 删除签名
        /// </summary>
        DeleteSinnature= 2251799813685248,
            
        /// <summary>
        /// 快速查询
        /// </summary>
        FastSelect= 4503599627370496,

        /// <summary>
        /// 查看历史报表
        /// </summary>
        ShowHistory = 9007199254740992,

        /// <summary>
        /// 调账
        /// </summary>
        AdjustFolio = 18014398509481984,

        /// <summary>
        /// 订单日志
        /// </summary>
        OrderLog = 36028797018963968,

        /// <summary>
        /// 批量发放优惠券
        /// </summary>
        GiveCoupon = 72057594037927936,

        /// <summary>
        /// 入账账务退款
        /// </summary>
        RefundFolio = 144115188075855872,

        /// <summary>
        /// 调价
        /// </summary>
        AdjustPrice = 288230376151711744,

        /// <summary>
        /// 冲销减免
        /// </summary>
        AbatementFolio = 576460752303423488,

        /// <summary>
        /// 消费录入权限
        /// </summary>
        ItemConsumeRoleAuth = 1152921504606846976,
    }
}
