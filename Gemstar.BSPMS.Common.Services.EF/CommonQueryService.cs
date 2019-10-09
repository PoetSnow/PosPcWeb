﻿using Gemstar.BSPMS.Common.Services.EntityProcedures;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Gemstar.BSPMS.Common.Services.EF
{
    /// <summary>
    /// 通用查询服务的EF实现
    /// </summary>
    public class CommonQueryService : ICommonQueryService
    {
        private DbContext _dbContext;
        public CommonQueryService(DbContext dbContenxt)
        {
            _dbContext = dbContenxt;
        }
        /// <summary>
        /// 获取通用查询参数中指定下拉规则对应的下拉数据项
        /// </summary>
        /// <param name="dropdownCode">查询参数中的下拉规则码</param>
        /// <param name="currentUserCode">当前操作员代码</param>
        /// <returns>查询参数对应的下拉数据项</returns>
        public List<UpGetDropDownCodeAndNameResult> GetDropDownCodeAndNames(string dropdownCode, string currentUserCode = "")
        {
            return _dbContext.Database.SqlQuery<UpGetDropDownCodeAndNameResult>(
                  "exec up_GetDropDownCodeAndName @ruleCode=@ruleCode,@seqid=@seqid",
                  new SqlParameter("ruleCode", dropdownCode),
                  new SqlParameter("seqid", currentUserCode)).ToList();
        }

        /// <summary>
        /// 获取通用查询参数中指定下拉规则对应的下拉数据项
        /// </summary>
        /// <param name="dropdownCode">查询参数中的下拉规则码</param>
        /// <param name="currentUserCode">当前操作员代码</param>
        /// <param name="search">查询内容</param>
        /// <param name="hid">酒店ID</param>
        /// <returns>查询参数对应的下拉数据项</returns>
        public List<UpGetDropDownCodeAndNameResult> GetDropDownCodeAndNames(string dropdownCode, string currentUserCode, string search, string hid, string paraName = "")
        {
            return _dbContext.Database.SqlQuery<UpGetDropDownCodeAndNameResult>(
                  "exec up_GetDropDownCodeAndName @ruleCode=@ruleCode,@seqid=@seqid,@search=@search,@hid=@hid,@paraName=@paraName",
                  new SqlParameter("ruleCode", dropdownCode),
                  new SqlParameter("seqid", currentUserCode),
                  new SqlParameter("search", search),
                  new SqlParameter("hid", hid),
                  new SqlParameter("paraName", paraName)
                  ).ToList();
        }

        /// <summary>
        /// 获取指定存储过程的所有参数集合
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <returns>指定存储过程的所有参数集合</returns>
        public List<UpQueryProcedureParametersResult> GetProcedureParameters(string procedureName)
        {
            return _dbContext.Database.SqlQuery<UpQueryProcedureParametersResult>(
                   "exec up_QueryProcedureParameters @procedureName=@procedureName",
                   new SqlParameter("procedureName", procedureName)).ToList();
        }
        /// <summary>
        /// 获取指定存储过程的所有参数集合
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="hid">当前登录信息 酒店ID</param>
        /// <param name="userName">当前登录信息 操作员名字</param>
        /// <param name="shiftId">当前登录信息 班次ID</param>
        /// <returns>指定存储过程的所有参数集合</returns>
        public List<UpQueryProcedureParametersResult> GetProcedureParameters(string procedureName, string hid, string userName, string shiftId)
        {
            return _dbContext.Database.SqlQuery<UpQueryProcedureParametersResult>(
                   "exec up_QueryProcedureParameters @procedureName=@procedureName,@hid=@hid,@username=@username,@shiftid=@shiftid",
                   new SqlParameter("procedureName", procedureName),
                   new SqlParameter("hid", hid),
                   new SqlParameter("username", userName),
                   new SqlParameter("shiftid", shiftId)
                   ).ToList();
        }
        /// <summary>
        /// 执行存储过程并且返回数据
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameterAndValues">参数名称及值</param>
        /// <returns>执行存储过程后返回的第一个结果集</returns>
        public DataTable ExecuteQuery(string procedureName, Dictionary<string, string> parameterAndValues)
        {
            var cmd = _dbContext.Database.Connection.CreateCommand();
            var dt = new DataTable();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procedureName;
            foreach (var name in parameterAndValues.Keys)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = name;
                p.Value = parameterAndValues[name];
                cmd.Parameters.Add(p);
            }
            try
            {
                _dbContext.Database.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                    reader.Close();
                }
            }
            finally
            {
                _dbContext.Database.Connection.Close();
            }
            return dt;
        }
        /// <summary>
        /// 执行存储过程并且返回数据
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameterAndValues">存储过程参数值</param>
        /// <returns>结果类型列表</returns>
        public IEnumerable<T> ExecuteQuery<T>(string procedureName, Dictionary<string, string> parameterAndValues)
        {
            var sql = new StringBuilder();
            var paras = new object[parameterAndValues.Count];
            sql.AppendFormat("exec {0} ", procedureName);
            var i = 0;
            var split = "";
            foreach (var key in parameterAndValues.Keys)
            {
                sql.AppendFormat("{0}{1}={1}", split, key);
                split = ",";
                paras[i] = new SqlParameter(key, parameterAndValues[key]);
                i++;
            }

            return _dbContext.Database.SqlQuery<T>(sql.ToString(), paras).ToList();
        }
    }
}