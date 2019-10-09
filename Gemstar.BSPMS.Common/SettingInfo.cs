namespace Gemstar.BSPMS.Common
{
    public class SettingInfo
    {
        /// <summary>
        /// 应用程序名称
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// 根域名
        /// </summary>
        public string RootDomain { get; set; }
        /// <summary>
        /// 酒店注册时的服务协议页面地址
        /// </summary>
        public string AgreementUrl { get; set; }
        /// <summary>
        /// 测试环境配置值
        /// </summary>
        public EnvSettingInfo TestSettingInfo { get; set; }
        /// <summary>
        /// 生产环境配置值
        /// </summary>
        public EnvSettingInfo ProductSettingInfo { get; set; }

    }
    /// <summary>
    /// 环境配置项
    /// </summary>
    public class EnvSettingInfo
    {
        /// <summary>
        /// 通知程序的日志级别
        /// </summary>
        public LogLevel NotifyWebLogLevel { get; set; }
        /// <summary>
        /// redis服务器IP
        /// </summary>
        public string RedisServerIp { get; set; }
        /// <summary>
        /// redis服务器端口
        /// </summary>
        public string RedisServerPort { get; set; }
        /// <summary>
        /// redis服务器密码
        /// </summary>
        public string RedisServerPassword { get; set; }
        /// <summary>
        /// redis服务器2IP
        /// </summary>
        public string RedisServer2Ip { get; set; }
        /// <summary>
        /// redis服务器2端口
        /// </summary>
        public string RedisServer2Port { get; set; }
        /// <summary>
        /// redis服务器2密码
        /// </summary>
        public string RedisServer2Password { get; set; }
        /// <summary>
        /// 数据库服务器IP
        /// </summary>
        public string DatabaseServerIp { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DatabaseName { get; set; }
        /// <summary>
        /// 数据库用户名
        /// </summary>
        public string DatabaseUserName { get; set; }
        /// <summary>
        /// 数据库密码
        /// </summary>
        public string DatabasePassword { get; set; }
    }
}
