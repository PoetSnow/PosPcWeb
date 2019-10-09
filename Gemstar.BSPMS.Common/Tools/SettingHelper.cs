using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 设置辅助类，用于从指定地址读取配置项后进行返回
    /// </summary>
    public static class SettingHelper
    {
        /// <summary>
        /// 从指定的域名中获取设置值
        /// </summary>
        /// <param name="settingUrl">要获取设置的地址</param>
        /// <param name="settingUrl2">要获取设置的地址2</param>
        /// <returns>如果设置的地址有正确返回值，则使用设置的值，否则返回默认值</returns>
        public static SettingInfo GetSetting(string settingUrl,string settingUrl2)
        {
            //处理参数默认值
            if (string.IsNullOrWhiteSpace(settingUrl))
            {
                settingUrl = "http://pmssetting.gshis.com/CurrentSetting";
            }
            if (string.IsNullOrWhiteSpace(settingUrl2))
            {
                settingUrl2 = "http://pmssetting.gshis.net/CurrentSetting";
            }
            //先从主设置地址中获取设置信息
            var result = GetSettingFromUrl(settingUrl);
            if(result == null)
            {
                //如果从主设置地址中没有正确获取到，则从备用地址中获取
                result = GetSettingFromUrl(settingUrl2);                   
            }
            //如果有正确获取到设置信息，则返回获取到的结果
            if(result != null)
            {
                return result;
            }
            //没有获取到正确的结果，返回默认对象
            return DefaultSettingInfo;
        }
        private static SettingInfo GetSettingFromUrl(string settingUrl)
        {
            try
            {
                var request = WebRequest.Create(settingUrl);
                string jsonStr = "";
                using (var response = request.GetResponse())
                {
                    var reader = new StreamReader(response.GetResponseStream());
                    jsonStr = reader.ReadToEnd();
                }
                if (!string.IsNullOrWhiteSpace(jsonStr))
                {
                    var serializer = new JavaScriptSerializer();
                    return serializer.Deserialize<SettingInfo>(jsonStr);
                }
            }
            catch
            {
            }
            return null;
        }
        //默认密码是jxd598，加密后的字符
        private static SettingInfo DefaultSettingInfo = new SettingInfo
        {
            ApplicationName = "GemstarBSPMS",
            RootDomain = ".gshis.com",
            AgreementUrl = "http://pmssetting.gshis.com:89/agreement.html",
            ProductSettingInfo = new EnvSettingInfo
            {
                RedisServerIp = "r-wz9851179e1f85a4.redis.rds.aliyuncs.com",
                RedisServerPort = "6379",
                RedisServerPassword = "C2ywn6DYgxr8kxDjbjgTPg==",
                DatabaseServerIp = "172.16.1.12,1544",
                DatabaseName = "pmsmaster",
                DatabaseUserName = "pms",
                DatabasePassword = "QQw5L+a0iRwF5LQjfFSUqQ==",
                NotifyWebLogLevel = LogLevel.ErrorOnly
            },
            TestSettingInfo = new EnvSettingInfo
            {
                RedisServerIp = "192.168.1.110",
                RedisServerPort = "6379",
                RedisServerPassword = "1gFvX4nmjhI=",
                DatabaseServerIp = "192.168.1.110",
                DatabaseName = "pmsmaster",
                DatabaseUserName = "jxd",
                DatabasePassword = "1gFvX4nmjhI=",
                NotifyWebLogLevel = LogLevel.ErrorAndInfoAndDebug
            }
        };
    }
}
