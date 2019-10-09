using Gemstar.BSPMS.Common.Services.EntityProcedures;
using System.Collections.Generic;
using System.Data;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 通用查询服务
    /// </summary>
    public interface ICommonQueryService
    {
        /// <summary>
        /// 获取指定存储过程的所有参数集合
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <returns>指定存储过程的所有参数集合</returns>
        List<UpQueryProcedureParametersResult> GetProcedureParameters(string procedureName);
        /// <summary>
        /// 获取指定存储过程的所有参数集合
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="hid">当前登录信息 酒店ID</param>
        /// <param name="userName">当前登录信息 操作员名字</param>
        /// <param name="shiftId">当前登录信息 班次ID</param>
        /// <returns>指定存储过程的所有参数集合</returns>
        List<UpQueryProcedureParametersResult> GetProcedureParameters(string procedureName, string hid, string userName, string shiftId);
        /// <summary>
        /// 获取通用查询参数中指定下拉规则对应的下拉数据项
        /// </summary>
        /// <param name="dropdownCode">查询参数中的下拉规则码</param>
        /// <param name="currentUserCode">当前操作员代码</param>
        /// <returns>查询参数对应的下拉数据项</returns>
        List<UpGetDropDownCodeAndNameResult> GetDropDownCodeAndNames(string dropdownCode, string currentUserCode = "");
        /// <summary>
        /// 获取通用查询参数中指定下拉规则对应的下拉数据项
        /// </summary>
        /// <param name="dropdownCode">查询参数中的下拉规则码</param>
        /// <param name="currentUserCode">当前操作员代码</param>
        /// <param name="search">查询内容</param>
        /// <param name="hid">酒店ID</param>
        /// <returns>查询参数对应的下拉数据项</returns>
        List<UpGetDropDownCodeAndNameResult> GetDropDownCodeAndNames(string dropdownCode, string currentUserCode, string search, string hid, string paraName = "");
        /// <summary>
        /// 执行存储过程并且返回数据
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameterAndValues">参数名称及值</param>
        /// <returns>执行存储过程后返回的第一个结果集</returns>
        DataTable ExecuteQuery(string procedureName, Dictionary<string, string> parameterAndValues);
        /// <summary>
        /// 执行存储过程并且返回数据
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameterAndValues">存储过程参数值</param>
        /// <returns>结果类型列表</returns>
        IEnumerable<T> ExecuteQuery<T>(string procedureName, Dictionary<string, string> parameterAndValues);
    }
}
