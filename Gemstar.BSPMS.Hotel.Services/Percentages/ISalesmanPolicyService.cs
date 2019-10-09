using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Percentages
{
    public interface ISalesmanPolicyService : ICRUDService<PercentagesPolicySalesman>
    {
        /// <summary>
        /// 验证是否存在 与[是否全额]相反的记录，如果存在，则为true，否则false。
        /// 规则：[酒店ID+提成内容+内容类型]中不能存在 既有全额，又有阶梯的记录。
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="amountSource">提成内容</param>
        /// <param name="amountSumType">金额计算类型：single:单次，month:按月累计</param>
        /// <param name="isInPlan">内容类型</param>
        /// <param name="isAllAmount">是否全额（true全额，false阶梯）</param>
        /// <param name="notId">不包括主键ID</param>
        /// <returns></returns>
        bool ExixtsByAmountIsAll(string hid, string amountSource, string amountSumType, bool? isInPlan, bool isAllAmount, Guid? notId = null);

        /// <summary>
        /// 验证是否存在 与[开始值-结束值]范围重合的记录，如果存在，则为true，否则false。
        /// 规则：[酒店ID+提成内容+内容类型]中不能存在 [开始值-结束值]区间重合 的记录。
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="amountSource">提成内容</param>
        /// <param name="amountSumType">金额计算类型：single:单次，month:按月累计</param>
        /// <param name="isInPlan">内容类型</param>
        /// <param name="amountBegin">开始值</param>
        /// <param name="amountEnd">结束值</param>
        /// <param name="notId">不包括主键ID</param>
        /// <returns></returns>
        bool ExixtsByAmountRange(string hid, string amountSource, string amountSumType, bool? isInPlan, decimal amountBegin, decimal amountEnd, Guid? notId = null);

        /// <summary>
        /// 修改[是否全额]
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">主键ID</param>
        void EditIsAllAmount(string hid, Guid id);
    }
}
