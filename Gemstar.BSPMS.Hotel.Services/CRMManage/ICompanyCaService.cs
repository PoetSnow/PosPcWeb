using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface ICompanyCaService : ICRUDService<CompanyCa>
    {

        List<CompanyCa> GetCompanCaList(List<string> ids);
        /// <summary>
        /// 合约单位往来录入
        /// </summary>
        /// <param name="companyid">合约单位ID</param>
        /// <param name="type">交易类型</param>
        /// <param name="amount">金额</param>
        /// <param name="itemid">付款方式</param>
        /// <param name="invno">手工单号</param>
        /// <param name="remark">备注</param>
        JsonResultData AddProc(Guid companyid, string type, decimal amount, string itemid, string invno, string remark,string outletcode,string refno = null);

        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="toCompanyId">转到合约单位ID</param>
        void TransferAccounts(List<string> id, Guid toCompanyId, string remark,string name);

        /// <summary>
        /// 拆账
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="amount">拆账金额</param>
        void SplitAccounts(Guid id, decimal amount, string remark);

        /// <summary>
        /// 核销
        /// </summary>
        /// <param name="ids">主键ID列表（逗号隔开）</param>
        /// <returns></returns>
        JsonResultData CancelAfterVerification(string ids);
        /// <summary>
        /// 核销记录
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="companyid"></param>
        /// <param name="checkDateBegin"></param>
        /// <param name="checkDateEnd"></param>
        /// <returns></returns>
        List<UpQueryCancelRecord> CancelRecord(string hid, string companyid, string checkDateBegin, string checkDateEnd);
        /// <summary>
        /// 取消核销
        /// </summary>
        /// <param name="ids"></param>
        JsonResultData CancelRecordLog(string ids);

        /// <summary>
        /// 根据核验号查询核销记录ID按逗号拼接
        /// </summary>
        /// <param name="checkNo"></param>
        /// <returns></returns>
        string GetCancelRecordByCheckNo(Guid checkNo);
    }
}
