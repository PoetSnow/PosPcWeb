using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using System.Text;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 付款处理动作调用接口的xml格式处理基类
    /// </summary>
    public class PayActionXmlBase
    {
        /// <summary>
        /// 检查指定接口是否能够正常调用
        /// </summary>
        /// <param name="para">接口参数</param>
        /// <param name="businessPara">其他业务参数</param>
        /// <returns>默认直接返回能够正常调用</returns>
        public virtual JsonResultData DoCheck(PaymentOperatePara para,PaymentOperateBusinessPara businessPara)
        {
            return JsonResultData.Successed();
        }
        /// <summary>
        /// 执行真实的接口调用
        /// </summary>
        /// <param name="para">接口参数</param>
        /// <param name="businessPara">其他业务参数</param>
        /// <returns>默认不做任何接口调用，直接返回成功，具体的接口调用由子类重写来实现</returns>
        public virtual JsonResultData DoOperate(PaymentOperatePara para,PaymentOperateBusinessPara businessPara)
        {
            return JsonResultData.Successed();
        }
        /// <summary>
        /// 当前处理动作需要返回的默认账务状态
        /// </summary>
        public virtual PosBillDetailStatus DefaultDetailStatus => PosBillDetailStatus.正常;
        /// <summary>
        /// 给xml增加业务信息之前的通用信息
        /// </summary>
        /// <param name="businessTitle">接口名称</param>
        /// <param name="xmlStrBuilder">xml生成器</param>
        protected void AppendBeforeBusinessXmlStr(string businessTitle,StringBuilder xmlStrBuilder)
        {
            xmlStrBuilder.Append("<?xml version='1.0' encoding='gbk' ?>")
                       .Append("<RealOperate>")
                       .Append("<XType>JxdBSPms</XType>")
                       .Append("<OpType>").Append(businessTitle).Append("</OpType>");
        }
        /// <summary>
        /// 给xml增加业务信息之后的通用信息
        /// </summary>
        /// <param name="xmlStrBuilder">xml生成器</param>
        protected void AppendAfterBusinessXmlStr(StringBuilder xmlStrBuilder)
        {
            xmlStrBuilder.Append("</RealOperate> ");
        }
    }
}
