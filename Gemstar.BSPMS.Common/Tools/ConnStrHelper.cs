using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 连接字符串辅助类
    /// </summary>
    public static class ConnStrHelper
    {
        /// <summary>
        /// 将连接字符串的各配置项组装成连接字符串
        /// </summary>
        /// <param name="dbServerIp">数据库服务器ip</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="uid">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="appName">应用名称</param>
        /// <param name="dbServerInternet">数据库对应的外网连接地址</param>
        /// <param name="isConnectViaInternet">是否使用外网进行连接</param>
        /// <param name="isPwdEncrypted">是否密码已经加密,默认为true</param>
        /// <returns></returns>
        public static string GetConnStr(string dbServerIp,string dbName,string uid,string pwd,string appName,string dbServerInternet,bool isConnectViaInternet,bool isPwdEncrypted = true)
        {
            if(string.IsNullOrWhiteSpace(dbServerIp) || string.IsNullOrWhiteSpace(dbName) || string.IsNullOrWhiteSpace(uid))
            {
                throw new ApplicationException("登录信息已经失效，请重新登录!");
            }
            if(isPwdEncrypted && !string.IsNullOrWhiteSpace(pwd))
            {
                pwd = CryptHelper.DecryptDES(pwd);
            }
            if (isConnectViaInternet)
            {
                dbServerIp = dbServerInternet;
            }
            return string.Format("data source={0};initial catalog={1};user id={2};password={3};MultipleActiveResultSets=True;App={4}",dbServerIp,dbName,uid,pwd,appName);
        }
        /// <summary>
        /// 是否指定的连接字符串是使用外网地址
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <returns>true:外网地址；false:内网地址</returns>
        public static bool IsConnStrInternet(string connStr)
        {
            try
            {
                var conn = new SqlConnectionStringBuilder(connStr);
                var dataSource = conn.DataSource;
                return IsDatasourceInternet(dataSource);
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 判断指定的数据库连接地址是否使用外网地址
        /// </summary>
        /// <param name="dataSource">连接的数据库连接地址</param>
        /// <returns>true:外网地址；false:内网地址</returns>
        public static bool IsDatasourceInternet(string dataSource)
        {
            try
            {
                if (dataSource.StartsWith("10.") || dataSource.StartsWith("172.") || dataSource.StartsWith("192."))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
