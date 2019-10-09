using System.Collections.Generic;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.MbrCardCenter
{
    /// <summary>
    /// 会员充值退款参数
    /// </summary>
    public class RechargeRefundPara
    {
        /// <summary>
        /// 计算增值金额服务实例
        /// </summary>
        public IChargeFreeService ChargeFreeService { get; set; }
        /// <summary>
        /// 当前酒店id
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 酒店名称
        /// </summary>
        public string HotelName { get; set; }
        /// <summary>
        /// 会员充值记录id
        /// </summary>
        public string ProfileCaId { get; set; }
        /// <summary>
        /// 通用的支付参数列表
        /// </summary>
        public List<M_v_payPara> CommonPayParas { get; set; }
        /// <summary>
        /// 酒店的支付参数列表
        /// </summary>
        public List<PmsPara> HotelPayParas { get; set; }
        /// <summary>
        /// 是否测试环境
        /// </summary>
        public bool IsEnvTest { get; set; }
        /// <summary>
        /// 支付日志记录实例
        /// </summary>
        public IPayLogService PayLogService { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
    }
}
