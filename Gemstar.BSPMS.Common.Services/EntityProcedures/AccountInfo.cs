namespace Gemstar.BSPMS.Common.Services.EntityProcedures
{
    /// <summary>
    /// 登录结果
    /// </summary>
    public class AccountInfo
    {
        /// <summary>
        /// 登录是否成功
        /// </summary>
        public bool LoginSuccess { get; set; }
        /// <summary>
        /// 登录失败时的原因
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 当前集团id
        /// </summary>
        public string Grpid { get; set; }
        /// <summary>
        /// 当前酒店id
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 当前酒店名称
        /// </summary>
        public string HotelName { get; set; }
        /// <summary>
        /// 当前酒店所在的业务服务器地址
        /// </summary>
        public string WebServerUrl { get; set; }
        /// <summary>
        /// 当前酒店业务数据库所在的数据库服务器
        /// </summary>
        public string DbServer { get; set; }
        /// <summary>
        /// 当前酒店业务数据库的名称
        /// </summary>
        public string DbName { get; set; }
        /// <summary>
        /// 当前酒店业务数据库的连接用户名
        /// </summary>
        public string DbUser { get; set; }
        /// <summary>
        /// 当前酒店业务数据库的连接密码
        /// </summary>
        public string DbPwd { get; set; }
        /// <summary>
        /// 用户唯一主键
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户代码
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 是否注册用户
        /// </summary>
        public bool IsRegUser { get; set; }

        /// <summary>
        /// 版本Id
        /// </summary>
        public string VersionId { get; set; }
    }
}
