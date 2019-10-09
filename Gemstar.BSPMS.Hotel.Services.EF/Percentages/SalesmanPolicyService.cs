using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.Percentages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EF.Percentages
{
    public class SalesmanPolicyService : CRUDService<PercentagesPolicySalesman>, ISalesmanPolicyService
    {
        public SalesmanPolicyService(DbHotelPmsContext db) : base(db, db.PercentagesPolicySalesmans)
        {
            _pmsContext = db;
        }
        protected override PercentagesPolicySalesman GetTById(string id)
        {
            return new PercentagesPolicySalesman {  PolicyId = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;


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
        public bool ExixtsByAmountIsAll(string hid, string amountSource, string amountSumType, bool? isInPlan, bool isAllAmount, Guid? notId = null)
        {
            Expression<Func<PercentagesPolicySalesman, bool>> predicate = c => c.Hid == hid && c.AmountSource == amountSource && c.AmountSumType == amountSumType && c.IsInPlan == isInPlan
            && c.IsAllAmount == !isAllAmount
            && (notId == null ? true : c.PolicyId != notId);

            return _pmsContext.PercentagesPolicySalesmans.Any(predicate);
        }

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
        public bool ExixtsByAmountRange(string hid, string amountSource, string amountSumType, bool? isInPlan, decimal amountBegin, decimal amountEnd, Guid? notId = null)
        {
            Expression<Func<PercentagesPolicySalesman, bool>> predicate = c => c.Hid == hid && c.AmountSource == amountSource && c.AmountSumType == amountSumType && c.IsInPlan == isInPlan
            && 
            (
                (c.AmountBegin <= amountBegin && c.AmountEnd >= amountBegin)
                ||
                (c.AmountBegin <= amountEnd && c.AmountEnd >= amountEnd)
            )
            && (notId == null ? true : c.PolicyId != notId);

            return _pmsContext.PercentagesPolicySalesmans.Any(predicate);
        }

        /// <summary>
        /// 修改[是否全额]
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">主键ID</param>
        public void EditIsAllAmount(string hid, Guid id)
        {
            try
            {
                var entity = _pmsContext.PercentagesPolicySalesmans.AsNoTracking().FirstOrDefault(c => c.Hid == hid && c.PolicyId == id);
                if (entity != null)
                {
                    string sql = @"update PercentagesPolicySalesman set IsAllAmount = @isAllAmount_set
                               where hid=@hid and AmountSource=@amountSource and AmountSumType=@amountSumType and IsAllAmount = @isAllAmount";

                    List<SqlParameter> parameters = new List<SqlParameter>() {
                           new SqlParameter("@hid", entity.Hid)
                         , new SqlParameter("@amountSource", entity.AmountSource)
                         , new SqlParameter("@amountSumType", entity.AmountSumType)
                         , new SqlParameter("@isAllAmount", entity.IsAllAmount)
                         , new SqlParameter("@isAllAmount_set", !entity.IsAllAmount)
                    };
                    if (entity.IsInPlan != null && entity.IsInPlan.HasValue)
                    {
                        sql = sql + " and IsInPlan=@isInPlan";
                        parameters.Add(new SqlParameter("@isInPlan", entity.IsInPlan.Value));
                    }
                    else
                    {
                        sql = sql + " and IsInPlan is null";
                    }

                    _pmsContext.Database.ExecuteSqlCommand(sql, parameters.ToArray());
                }
            }
            catch { }
        }
    }
}
