using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using System.Web;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System;

namespace Gemstar.BSPMS.Hotel.Web.Models
{
    /// <summary>
    /// 当前登录信息的session实现
    /// </summary>
    public class CurrentInfoSession : ICurrentInfo
    {
        public const string SessionKeyUserId = "UserId";
        public const string SessionKeyUserCode = "UserCode";
        public const string SessionKeyUserName = "UserName";
        public const string SessionKeyIsRegUser = "IsRegUser";
        public const string SessionKeyGroupId = "GroupId";
        public const string SessionKeyHotelId = "HotelId";
        public const string SessionKeySignature = "Signature";
        public const string SessionKeyHotelName = "HotelName";
        public const string SessionKeyProductType = "ProductType";
        public const string SessionKeyWebServerUrl = "WebServerUrl";
        public const string SessionKeyDbServer = "DbServer";
        public const string SessionKeyDbName = "DbName";
        public const string SessionKeyDbUser = "DbUser";
        public const string SessionKeyDbPwd = "DbPwd";
        public const string SessionKeyShiftId = "ShiftId";
        public const string SessionKeyShiftName = "ShiftName";
        public const string SessionKeyLoginTimeTicks = "LoginTimeTicks";
        public const string SessionKeyPosId = "PosId";
        public const string SessionKeyPosName = "PosName";
        public const string SessionKeyMobile = "Mobile";
        public const string SessionKeyAddress = "Address";
        public const string SessionKeyModuleCode = "ModuleCode";
        public const string SessionKeyVersionId = "VersionId";
        private Dictionary<string, string> _values;

        /// <summary>
        /// 当前登录人的id值，唯一主键值
        /// </summary>
        public string UserId
        {
            get { return getValue(SessionKeyUserId); }
            set { setValue(SessionKeyUserId, value); }
        }

        public string UserCode
        {
            get { return getValue(SessionKeyUserCode); }
            set { setValue(SessionKeyUserCode, value); }
        }

        public string UserName
        {
            get { return getValue(SessionKeyUserName); }
            set { setValue(SessionKeyUserName, value); }
        }

        /// <summary>
        /// 是否注册用户
        /// </summary>
        public bool IsRegUser
        {
            get { return getValue(SessionKeyIsRegUser) == "1"; }
            set { setValue(SessionKeyIsRegUser, value ? "1" : "0"); }
        }

        public string GroupId
        {
            get { return getValue(SessionKeyGroupId); }
            set { setValue(SessionKeyGroupId, value); }
        }

        public string HotelId
        {
            get { return getValue(SessionKeyHotelId); }
            set { setValue(SessionKeyHotelId, value); }
        }

        public string Signature
        {
            get { return getValue(SessionKeySignature); }
            set { setValue(SessionKeySignature, value); }
        }

        /// <summary>
        /// 当前是否集团，如果是集团则返回true，如果是单体酒店，则返回false
        /// </summary>
        public bool IsGroup { get { return !string.IsNullOrWhiteSpace(GroupId); } }

        /// <summary>
        /// 当前是否集团下的集团管理公司，如果是则返回true，如果不是集团管理公司，则返回false
        /// </summary>
        public bool IsGroupInGroup { get { return GroupId.Equals(HotelId); } }

        /// <summary>
        /// 当前是否集团下的分店，如果是则返回true，如果不是集团分店，则返回false
        /// </summary>
        public bool IsHotelInGroup { get { return !string.IsNullOrWhiteSpace(GroupId) && !GroupId.Equals(HotelId); } }

        /// <summary>
        /// 集团或酒店id，如果当前是集团时则返回集团id，如果当前是单体酒店时，则返回酒店id
        /// </summary>
        public string GroupHotelId { get { return string.IsNullOrWhiteSpace(GroupId) ? HotelId : GroupId; } }

        /// <summary>
        /// 当前菜单类型
        /// </summary>
        public AuthType AuthListType
        {
            get
            {
                if (IsGroup)
                {
                    if (IsGroupInGroup)
                    {
                        return AuthType.Group;
                    }
                    return AuthType.GroupHotel;
                }
                else
                {
                    return AuthType.SingleHotel;
                }
            }
        }

        /// <summary>
        /// 当前产品类型
        /// </summary>
        public ProductType ProductType
        {
            get { return (ProductType)Enum.Parse(typeof(ProductType), getValue(SessionKeyProductType)); }
            set { setValue(SessionKeyProductType, value.ToString()); }
        }

        /// <summary>
        /// 当前酒店名称
        /// </summary>
        public string HotelName
        {
            get { return getValue(SessionKeyHotelName); }
            set { setValue(SessionKeyHotelName, value); }
        }

        public string WebServerUrl
        {
            get { return getValue(SessionKeyWebServerUrl); }
            set { setValue(SessionKeyWebServerUrl, value); }
        }

        public string DbServer
        {
            get { return getValue(SessionKeyDbServer); }
            set { setValue(SessionKeyDbServer, value); }
        }

        public string DbName
        {
            get { return getValue(SessionKeyDbName); }
            set { setValue(SessionKeyDbName, value); }
        }

        public string DbUser
        {
            get { return getValue(SessionKeyDbUser); }
            set { setValue(SessionKeyDbUser, value); }
        }

        public string DbPwd
        {
            get { return getValue(SessionKeyDbPwd); }
            set { setValue(SessionKeyDbPwd, value); }
        }

        /// <summary>
        /// 当前班次id
        /// </summary>
        public string ShiftId
        {
            get { return getValue(SessionKeyShiftId); }
            set { setValue(SessionKeyShiftId, value); }
        }

        /// <summary>
        /// 当前班次名称
        /// </summary>
        public string ShiftName
        {
            get { return getValue(SessionKeyShiftName); }
            set { setValue(SessionKeyShiftName, value); }
        }

        /// <summary>
        /// 登录时的时间戳
        /// </summary>
        public string LoginTimeTicks
        {
            get { return getValue(SessionKeyLoginTimeTicks); }
            set { setValue(SessionKeyLoginTimeTicks, value); }
        }

        /// <summary>
        /// 收银点Id
        /// </summary>
        public string PosId
        {
            get { return getValue(SessionKeyPosId); }
            set { setValue(SessionKeyPosId, value); }
        }

        /// <summary>
        /// 收银点
        /// </summary>
        public string PosName
        {
            get { return getValue(SessionKeyPosName); }
            set { setValue(SessionKeyPosName, value); }
        }

        /// <summary>
        /// 模块(CY：餐饮；POS：POS点；CL：会所；SN：桑拿；WQ：温泉；YT：游艇)
        /// </summary>
        public string ModuleCode
        {
            get { return getValue(SessionKeyModuleCode); }
            set { setValue(SessionKeyModuleCode, value); }
        }

        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile
        {
            get { return getValue(SessionKeyMobile); }
            set { setValue(SessionKeyMobile, value); }
        }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get { return getValue(SessionKeyAddress); }
            set { setValue(SessionKeyAddress, value); }
        }

        private void setValue(string key, string value)
        {
            _values[key] = value;
        }

        private string getValue(string key)
        {
            if (_values.ContainsKey(key))
            {
                return _values[key];
            }
            return string.Empty;
        }

        public void Clear()
        {
            //先将软件狗的值取出来，放在变量在
            var isDogOk = ProgVersion.IsDogOk;
            var dogError = ProgVersion.DogErrorMessage;
            //清空会话中的其他信息
            _values.Clear();
            var _session = HttpContext.Current.Session;
            if (_session != null)
            {
                _session.Clear();
            }
            //再次恢复软件狗的检测值
            ProgVersion.IsDogOk = isDogOk;
            ProgVersion.DogErrorMessage = dogError;
        }

        private string GetValueKey()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                return string.Format("Session{0}ValueKey", HttpContext.Current.Session.SessionID);
            }
            return string.Empty;
        }

        /// <summary>
        /// 从存储中加载值
        /// </summary>
        public void LoadValues()
        {
            _values = null;
            var sessionKey = GetValueKey();
            if (!string.IsNullOrWhiteSpace(sessionKey))
            {
                try
                {
                    var serializer = new JavaScriptSerializer();
                    var redisConnection = MvcApplication.RedisConnection;
                    var db = redisConnection.GetDatabase();
                    var task = db.StringGetAsync(sessionKey);
                    task.Wait();
                    var valueStr = task.Result;
                    if (!string.IsNullOrWhiteSpace(valueStr))
                    {
                        _values = serializer.Deserialize<Dictionary<string, string>>(valueStr);
                    }
                }
                catch (Exception ex)
                {
                    MvcApplication.WriteRedisException(ex);
                }
            }
            if (_values == null)
            {
                _values = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// 将值保存到存储中
        /// </summary>
        public void SaveValues()
        {
            var sessionKey = GetValueKey();
            if (!string.IsNullOrWhiteSpace(sessionKey))
            {
                var serializer = new JavaScriptSerializer();
                var valueStr = serializer.Serialize(_values);
                var redisConnection = MvcApplication.RedisConnection;
                var db = redisConnection.GetDatabase();
                try
                {
                    db.StringSet(sessionKey, valueStr);
                }
                catch
                {
                    db.KeyDelete(sessionKey);
                    db.StringSet(sessionKey, valueStr);
                }
            }
        }

        /// <summary>
        /// 获取不包含酒店id的字符串，主要用于去掉像班次id，房型id等以酒店id开头的数据，记录日志时，不需要记录酒店id
        /// 只有在字符串的前几位与酒店id相同的才会去掉
        /// </summary>
        /// <param name="originStr">需要去掉酒店id的原始字符串</param>
        /// <param name="splitChar">分隔符</param>
        /// <param name="joinChar">合并时的连接字符</param>
        /// <returns>去掉酒店id之后的字符串</returns>
        public string GetStringWithoutHotelId(string originStr, char splitChar = ',', string joinChar = ",")
        {
            if (string.IsNullOrWhiteSpace(originStr))
            {
                return originStr;
            }
            var strs = originStr.Split(splitChar);
            var hid = HotelId;

            var replacedStrs = new List<string>();
            foreach (var str in strs)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    if (str.StartsWith(hid))
                    {
                        replacedStrs.Add(str.Substring(hid.Length));
                    }
                    else
                    {
                        replacedStrs.Add(str);
                    }
                }
            }
            return string.Join(joinChar, replacedStrs.ToArray());
        }

        /// <summary>
        /// 是否售后服务工程师使用客人授权进行登录
        /// </summary>
        /// <returns>true:是，false:是酒店操作员自行登录</returns>
        public bool IsServiceOperatorLogin()
        {
            //先直接根据用户代码中，是否包含授权给三个字来判断，因为售后服务工程师使用客人授权进行登录时，用户代码会加上此三个字，并且正常情况下，真实的作员不会有这三个字
            return UserCode.Contains("授权给");
        }

        /// <summary>
        /// 版本Id
        /// </summary>
        public string VersionId
        {
            get { return getValue(SessionKeyVersionId); }
            set { setValue(SessionKeyVersionId, value); }
        }
    }
}