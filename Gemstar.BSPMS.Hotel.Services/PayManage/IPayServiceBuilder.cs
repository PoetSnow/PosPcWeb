using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.PayManage
{
    /// <summary>
    /// 支付服务实例创建接口
    /// </summary>
    public interface IPayServiceBuilder
    {
        /// <summary>
        /// 根据付款处理方式代码获取对应的支付服务实例
        /// </summary>
        /// <param name="action">支付处理方式</param>
        /// <param name="commonPayParas">通用的支付参数列表</param>
        /// <param name="hotelPayParas">酒店的支付参数列表</param>
        /// <param name="isEnvTest">是否测试环境</param>
        /// <returns>对应的支付服务实例</returns>
        IPayService GetPayService(string action,List<M_v_payPara> commonPayParas,List<PmsPara> hotelPayParas,bool isEnvTest);
        /// <summary>
        /// 根据付款处理方式代码获取对应的支付退款服务实例
        /// </summary>
        /// <param name="action">支付处理方式</param>
        /// <param name="commonPayParas">通用的支付参数列表</param>
        /// <param name="hotelPayParas">酒店的支付参数列表</param>
        /// <param name="isEnvTest">是否测试环境</param>
        /// <returns>对应的支付服务实例</returns>
        IPayService GetPayRefundService(string action, List<M_v_payPara> commonPayParas, List<PmsPara> hotelPayParas, bool isEnvTest);
    }
}
