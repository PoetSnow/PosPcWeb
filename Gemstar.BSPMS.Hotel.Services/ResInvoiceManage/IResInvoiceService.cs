using System.Collections.Generic;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.ResInvoiceManage
{
    /// <summary>
    /// 发票
    /// </summary>
    public interface IResInvoiceService
    {
        /// <summary>
        /// 根据酒店Id和订单Id返回对应的发票所需要的所有信息
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="resId">订单Id</param>
        /// <returns>对应的发票所需要的所有信息</returns>
        ResInvoiceMainInfo GetResInvoiceMainInfoByResId(string hid, string resId);

        /// <summary>
        /// 根据酒店Id和会员账务Id返回对应的发票所需要的所有信息
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="profileCaId">会员账务Id</param>
        /// <returns>对应的发票所需要的所有信息</returns>
        ResInvoiceMainInfo GetResInvoiceMainInfoByProfileCaId(string hid, Guid profileCaId);

        /// <summary>
        /// 根据酒店Id和合约单位账务Id返回对应的发票所需要的所有信息
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="companyCaId">合约单位账务Id</param>
        /// <returns>对应的发票所需要的所有信息</returns>
        ResInvoiceMainInfo GetResInvoiceMainInfoByCompanyCaId(string hid, Guid companyCaId);

        /// <summary>
        /// 删除发票信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="Id">发票ID</param>
        /// <returns></returns>
        JsonResultData DelResInvoice(string hid, Guid Id);

        /// <summary>
        /// 增加或修改发票信息
        /// </summary>
        /// <param name="resInvoiceInfo">发票信息实例</param>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="businessDate">当前酒店营业日</param>
        /// <returns></returns>
        JsonResultData AddOrUpdateResInvoice(ResInvoiceInfo resInvoiceInfo, ICurrentInfo currentInfo, DateTime businessDate);

        /// <summary>
        /// 获取 发票来源的 发票信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="reftype">发票的关联类型。 0:和订单号相关  1:和会员账务相关  2:和合约单位账务相关</param>
        /// <param name="id">reftype=0订单Id，reftype=1会员账务Id，reftype=2合约单位账务Id</param>
        /// <returns></returns>
        JsonResultData GetInvoiceSourceInfo(string hid, byte reftype, string id);

        /// <summary>
        /// 获取开票信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="taxType">0:普通发票  1：增值税专用发票</param>
        /// <param name="taxName">模糊查询字符串</param>
        /// <returns></returns>
        List<Entities.InvoiceInfo> GetInvoicePartInfo(string hid, byte taxType, string taxName);

        /// <summary>
        /// 获取订单中指定合约单位的发票信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">订单ID</param>
        /// <returns></returns>
        ResInvoiceSimple GetCompanyInvoiceInfo(string hid, string resid);

        /// <summary>
        /// 生成发票明细消息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="reftype">发票的关联类型。 0:和订单号相关  1:和会员账务相关  2:和合约单位账务相关</param>
        /// <param name="id">reftype=0订单Id，reftype=1会员账务Id，reftype=2合约单位账务Id</param>
        /// <returns></returns>
        List<Gemstar.BSPMS.Common.Tools.KeyValuePairModel<string, decimal?>> GenerateInvoiceDetails(string hid, byte reftype, string id);
    }
}
