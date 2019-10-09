using System;

namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 会员充值单打印结果集
    /// 存储过程up_print_profileRecharge的结果集
    /// </summary>
    public class UpPrintProfileRechargeResult
    {
        public Guid Id { get; set; }
        public string 付款方式 { get; set; }
        public string 会员卡类型 { get; set; }
        public string 操作类型 { get; set; }
        public string 客户名称 { get; set; }
        public string 会员卡号 { get; set; }
        public string 酒店名 { get; set; }
        public DateTime? 充值时间 { get; set; }
        public decimal? 充值金额 { get; set; }
        public decimal? 增值金额 { get; set; }
        public decimal? 充值后金额 { get; set; }
        public decimal? 增值后金额 { get; set; }
        public decimal? 充值前金额 { get; set; }
        public decimal? 增值前金额 { get; set; }
        public string 操作员 { get; set; }
        public string 营业点 { get; set; }
        public string 二维码内容 { get; set; }
    }
}
