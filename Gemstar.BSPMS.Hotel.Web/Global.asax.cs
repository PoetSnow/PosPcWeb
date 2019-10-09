using Gemstar.BSPMS.Common;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Web.App_Start;
using RedisSessionProvider.Config;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using System.IO;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Web.Models;
using Senparc.Weixin.MP.Containers;

namespace Gemstar.BSPMS.Hotel.Web
{
    public class MvcApplication : System.Web.HttpApplication, ISettingProvider
    {
        private static SettingInfo _settingInfo;
        private static ConfigurationOptions _redisConfigOpts;
        private static ConnectionMultiplexer _redisConnection;
        /*
         * 陈前良，2017-4-20 14:54:29
         由于微信相关的值一般不会变动，而如果设置到数据库中，每次还要从数据库中读取，没有必要
         所以此处先直接设置为常量，以加快速度，等将来确定需要经常变动时，再更改为从数据库中读取
             */
        /// <summary>
        /// 第三方URL对应的Token
        /// </summary>
        public const string WeixinToken = "szsjxd";
        /// <summary>
        /// 第三方URL对应的消息加解密密钥
        /// </summary>
        public const string WeixinEncodingAESKey = "9vQB3ADB0taWjvykRZKglF6DDMNjZPmGc1kvPNHRy1C";
        /// <summary>
        /// 微信AppId
        /// </summary>
        public const string WeixinAppId = "wx5a9d341a4bc0ff50";
        /// <summary>
        /// 微信AppSecret
        /// </summary>
        public const string WeixinAppSecret = "241a55526236bd13599f0f35821834ac";
        /// <summary>
        /// 业主第三方URL对应的Token
        /// </summary>
        public const string OwnerWeixinToken = "jxdOwner";
        /// <summary>
        /// 业主第三方URL对应的消息加解密密钥
        /// </summary>
        public const string OwnerWeixinEncodingAESKey = "HJP7ca6FZPmQiiwkBEANOC8NU5zPsTliHJ4ncueJwNC";
        /// <summary>
        /// 业主微信AppId
        /// </summary>
        public const string OwnerWeixinAppId = "wx21cce2dcf20a61f5";
        /// <summary>
        /// 业主微信AppSecret
        /// </summary>
        public const string OwnerWeixinAppSecret = "4806ab0e4db6d7fee292de32c6403c6d";

        /// <summary>
        /// 是否测试环境
        /// </summary>
        public static bool IsTestEnv
        {
            get
            {
#if envTest
                return true;
#else
                return false;            
#endif
            }
        }

        public SettingInfo SettingInfo
        {
            get
            {
                return _settingInfo;
            }
        }

        protected void Application_Start()
        {
            GetSettingInfoFromRemoteServer();
            SetRedisConfig();

            AutofacConfig.Config(this);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = true;
            //注册微信相关值
            AccessTokenContainer.Register(WeixinAppId, WeixinAppSecret);
        }
        void Session_Start(object sender, EventArgs e)
        {
            if (ProgVersion.IsServerDogEnabled)
            {
                ProgVersion.CheckServerDog();
            }
        }
        /// <summary>
        /// 从配置文件中的设置url地址处读取配置信息
        /// 在程序第一次启动或数据库连接不成功时调用此方法来重新读取配置信息
        /// 使用此方法可以避免在此程序部署多个实例后，修改配置信息需要修改好多台服务器的问题
        /// </summary>
        public static void GetSettingInfoFromRemoteServer()
        {
            var settingUrl = ConfigurationManager.AppSettings["pmsSettingUrl"];
            var setting2Url = ConfigurationManager.AppSettings["pmsSetting2Url"];
            _settingInfo = SettingHelper.GetSetting(settingUrl, setting2Url);
        }
        /// <summary>
        /// 设置redis服务器配置信息，在应用程序启动时进行调用
        /// 在动态设置后，将设置的值反写回配置文件后，也需要调用一次此方法，以便重新读取一次配置文件中的值
        /// </summary>
        public static void SetRedisConfig()
        {
            RedisSessionConfig.SessionTimeout = TimeSpan.FromHours(4);
            var redisServerIp = _settingInfo.ProductSettingInfo.RedisServerIp;
            var redisServerPort = _settingInfo.ProductSettingInfo.RedisServerPort;
            var redisPassword = _settingInfo.ProductSettingInfo.RedisServerPassword;

            var redisServer2Ip = _settingInfo.ProductSettingInfo.RedisServer2Ip;
            var redisServer2Port = _settingInfo.ProductSettingInfo.RedisServer2Port;
            var redis2Password = _settingInfo.ProductSettingInfo.RedisServer2Password;
            if (IsTestEnv)
            {
                redisServerIp = _settingInfo.TestSettingInfo.RedisServerIp;
                redisServerPort = _settingInfo.TestSettingInfo.RedisServerPort;
                redisPassword = _settingInfo.TestSettingInfo.RedisServerPassword;

                redisServer2Ip = _settingInfo.TestSettingInfo.RedisServer2Ip;
                redisServer2Port = _settingInfo.TestSettingInfo.RedisServer2Port;
                redis2Password = _settingInfo.TestSettingInfo.RedisServer2Password;
            }
            redisPassword = CryptHelper.DecryptDES(redisPassword);
            redis2Password = CryptHelper.DecryptDES(redis2Password);

            //测试主redis是否能连接成功，可以则使用主的，否则使用备用的
            try
            {
                _redisConfigOpts = ConfigurationOptions.Parse(string.Format("{0}:{1}", redisServerIp, redisServerPort));
                _redisConfigOpts.Password = redisPassword;
                _redisConnection = ConnectionMultiplexer.Connect(_redisConfigOpts);
                var db = _redisConnection.GetDatabase();
                db.StringSet("testConnect", "testConnect");
                db.KeyDelete("testConnect");
            }
            catch
            {
                //主redis无效，直接使用备用的
                redisServerIp = redisServer2Ip;
                redisServerPort = redisServer2Port;
                redisPassword = redis2Password;
                _redisConfigOpts = ConfigurationOptions.Parse(string.Format("{0}:{1}", redisServerIp, redisServerPort));
                _redisConfigOpts.Password = redisPassword;
                _redisConnection = ConnectionMultiplexer.Connect(_redisConfigOpts);
            }

            RedisConnectionConfig.GetSERedisServerConfig = (HttpContextBase context) =>
            {
                return new KeyValuePair<string, ConfigurationOptions>(
                    "DefaultConnection",
                    _redisConfigOpts);
            };
            RedisSessionConfig.SessionExceptionLoggingDel = (Exception e) =>
            {
                WriteRedisException(e);
            };
        }
        public static ConnectionMultiplexer RedisConnection
        {
            get
            {
                return _redisConnection;
            }
        }
        public static void WriteRedisException(Exception ex)
        {
            try
            {
                var file = HttpContext.Current.Server.MapPath("~/Logs");
                if (!Directory.Exists(file))
                {
                    Directory.CreateDirectory(file);
                }
                file = Path.Combine(file, string.Format("{0}.log", DateTime.Today.ToString("yyyyMMdd")));
                using (var fs = File.AppendText(file))
                {
                    fs.WriteLine("--------------------------");
                    fs.WriteLine(string.Format("{0} 错误信息:{1}", DateTime.Now.ToString(), ex.Message));
                    fs.WriteLine(string.Format("调用堆栈:{0}", ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        var inner = ex.InnerException;
                        while (inner.InnerException != null)
                        {
                            inner = inner.InnerException;
                        }
                        fs.WriteLine(string.Format("内部异常错误信息：{0}", inner.Message));
                    }
                    fs.Close();
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// 获取中央数据库的连接字符串
        /// </summary>
        /// <returns>设置中的项构成的中央数据库的连接字符串</returns>
        public static string GetCenterDBConnStr()
        {
            try
            {
                var dbServerIp = _settingInfo.ProductSettingInfo.DatabaseServerIp;
                var dbName = _settingInfo.ProductSettingInfo.DatabaseName;
                var dbUser = _settingInfo.ProductSettingInfo.DatabaseUserName;
                var pwd = _settingInfo.ProductSettingInfo.DatabasePassword;
                if (IsTestEnv)
                {
                    dbServerIp = _settingInfo.TestSettingInfo.DatabaseServerIp;
                    dbName = _settingInfo.TestSettingInfo.DatabaseName;
                    dbUser = _settingInfo.TestSettingInfo.DatabaseUserName;
                    pwd = _settingInfo.TestSettingInfo.DatabasePassword;
                    //要求数据库名称必须包含test
                    if (dbName.IndexOf("test", StringComparison.OrdinalIgnoreCase) < 0 && dbName.IndexOf("dev", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        throw new ApplicationException("测试环境下连接的数据库名称中必须包含test或者dev字样");
                    }
                }

                return ConnStrHelper.GetConnStr(dbServerIp, dbName, dbUser, pwd, _settingInfo.ApplicationName, dbServerIp, false);
            }
            catch (NullReferenceException)
            {
                //出现异常时，重新从远端服务器上取一次配置信息，因为是配置信息中的数据库信息丢失了
                GetSettingInfoFromRemoteServer();
                return GetCenterDBConnStr();
            }
        }
        /// <summary>
        /// 获取当前酒店业务数据库的连接字符串
        /// </summary>
        /// <returns>从当前登录的session会话中读取到的当前酒店业务数据库连接字符串</returns>
        public static string GetHotelDbConnStr()
        {
            var currentInfo = DependencyResolver.Current.GetService<ICurrentInfo>();
            if (currentInfo == null || string.IsNullOrWhiteSpace(currentInfo.HotelId))
            {
                throw new ApplicationException("当前登录信息已经超时过期，请重新登录!");
            }
            var centerDb = DependencyResolver.Current.GetService<DbCommonContext>();
            return GetHotelDbConnStr(centerDb, currentInfo.HotelId);
        }
        /// <summary>
        /// 获取当前酒店业务数据库的连接字符串
        /// </summary>
        /// <param name="centerDb">中央主数据库实例</param>
        /// <param name="hid">酒店id</param>
        /// <param name="isEnvTest">是否测试环境</param>
        ///  <param name="isForReadonly">是否只读数据库</param>
        /// <returns>从当前登录的session会话中读取到的当前酒店业务数据库连接字符串</returns>
        public static string GetHotelDbConnStr(DbCommonContext centerDb, string hid, bool isForReadonly = false, int times = 0)
        {
            try
            {
                if (_settingInfo == null)
                {
                    GetSettingInfoFromRemoteServer();
                }
                var hotelInfoServices = new HotelInfoService(centerDb);
                var hotelInfos = hotelInfoServices.GetHotelInfo(hid);
                var dbName = hotelInfos.DbName;
                var dbServer = hotelInfos.DbServer;
                var dbUid = hotelInfos.Logid;
                var dbPwd = hotelInfos.LogPwd;
                if (IsTestEnv)
                {
                    //要求数据库名称必须包含test
                    if (hotelInfos.DbName.IndexOf("test", StringComparison.OrdinalIgnoreCase) < 0 && hotelInfos.DbName.IndexOf("dev", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        throw new ApplicationException("测试环境下连接的数据库名称中必须包含test或dev字样");
                    }
                }
                return ConnStrHelper.GetConnStr(hotelInfos.DbServer, hotelInfos.DbName, hotelInfos.Logid, hotelInfos.LogPwd, _settingInfo.ApplicationName, hotelInfos.DbServerInternet, centerDb.IsConnectViaInternetIp());

            }
            catch (Exception e)
            {
                //出现异常时，重新从远端服务器上取一次配置信息，因为是配置信息中的数据库信息丢失了
                //GetSettingInfoFromRemoteServer();
                //return GetHotelDbConnStr(centerDb, hid);

                if (times >= 2)
                {
                    if (string.IsNullOrWhiteSpace(hid))
                    {
                        throw new ApplicationException("当前酒店id为空，无法获取对应的酒店业务数据库连接", e);
                    }
                    if (centerDb == null)
                    {
                        throw new ApplicationException("当前中央主数据库实例为null，无法连接", e);
                    }
                    throw new ApplicationException($"已经尝试了多次获取酒店id为{hid}业务数据库连接，始终不成功。对应的主数据库连接：{centerDb?.Database?.Connection?.ConnectionString}", e);
                }
                //出现异常时，重新从远端服务器上取一次配置信息，因为是配置信息中的数据库信息丢失了
                GetSettingInfoFromRemoteServer();
                return GetHotelDbConnStr(centerDb, hid, isForReadonly, times + 1);

            }
        }

        private static bool IsInternetIpConnectMaster(DbCommonContext centerDb)
        {
            return centerDb.Database.Connection.DataSource.Contains("47.107.172.67");
        }

        public static string CreateHid(string baseName = "hotel", int no47 = 1, int len = 6)
        {
            return ADOHelper.ExecScalar(string.Format("up_getsysBaseNo @as_baseName='{0}',@no47={1},@len={2}", baseName, no47, len), GetCenterDBConnStr()).ToString();
        }
    }
}
