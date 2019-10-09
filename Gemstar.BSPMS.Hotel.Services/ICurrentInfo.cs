using Gemstar.BSPMS.Hotel.Services.AuthManages;

namespace Gemstar.BSPMS.Hotel.Services
{
    /// <summary>
    /// 当前登录信息
    /// </summary>
    public interface ICurrentInfo
    {
        /// <summary>
        /// 清空之前的所有信息
        /// </summary>
        void Clear();
        /// <summary>
        /// 从存储中加载值
        /// </summary>
        void LoadValues();
        /// <summary>
        /// 将值保存到存储中
        /// </summary>
        void SaveValues();
        /// <summary>
        /// 当前所属的集团id
        /// </summary>
        string GroupId { get; set; }
        /// <summary>
        /// 当前操作的酒店id
        /// </summary>
        string HotelId { get; set; }
        /// <summary>
        /// 当前酒店名称
        /// </summary>
        string HotelName { get; set; }
        /// <summary>
        /// 当前登录人的id值，唯一主键值
        /// </summary>
        string UserId { get; set; }
        /// <summary>
        /// 当前登录人的代码
        /// </summary>
        string UserCode { get; set; }
        /// <summary>
        /// 当前登录人的姓名
        /// </summary>
        string UserName { get; set; }
        /// <summary>
        /// 是否注册用户
        /// </summary>
        bool IsRegUser { get; set; }
        /// <summary>
        /// 酒店对应的服务器地址
        /// </summary>
        string WebServerUrl { get; set; }
        /// <summary>
        /// 酒店的业务数据库服务器
        /// </summary>
        string DbServer { get; set; }
        /// <summary>
        /// 酒店的业务数据库名称
        /// </summary>
        string DbName { get; set; }
        /// <summary>
        /// 酒店的业务数据库登录用户名
        /// </summary>
        string DbUser { get; set; }
        /// <summary>
        /// 酒店的业务数据库登录密码
        /// </summary>
        string DbPwd { get; set; }
        /// <summary>
        /// 当前班次id
        /// </summary>
        string ShiftId { get; set; }
        /// <summary>
        /// 当前班次名称
        /// </summary>
        string ShiftName { get; set; }
        /// <summary>
        /// 当前是否集团，如果是集团则返回true，如果是单体酒店，则返回false
        /// </summary>
        bool IsGroup { get; }
        /// <summary>
        /// 当前是否集团下的集团管理公司，如果是则返回true，如果不是集团管理公司，则返回false
        /// </summary>
        bool IsGroupInGroup { get; }
        /// <summary>
        /// 当前是否集团下的分店，如果是则返回true，如果不是集团分店，则返回false
        /// </summary>
        bool IsHotelInGroup { get;}
        /// <summary>
        /// 集团或酒店id，如果当前是集团时则返回集团id，如果当前是单体酒店时，则返回酒店id
        /// </summary>
        string GroupHotelId { get; }
        /// <summary>
        /// 当前菜单类型
        /// </summary>
        AuthType AuthListType { get; }
        /// <summary>
        /// 当前产品类型
        /// </summary>
        ProductType ProductType { get; set; }
        /// <summary>
        /// 是否有电子签名功能 0:没有 1：有
        /// </summary>
        string Signature { get; set; }
        /// <summary>
        /// 登录时的时间戳
        /// </summary>
        string LoginTimeTicks { get; set; }
        /// <summary>
        /// 收银点id
        /// </summary>
        string PosId { get; set; }
        /// <summary>
        /// 收银点名称
        /// </summary>
        string PosName { get; set; }
        /// <summary>
        /// 模块(CY：餐饮；POS：POS点；CL：会所；SN：桑拿；WQ：温泉；YT：游艇)
        /// </summary>
        string ModuleCode { get; set; }
        /// <summary>
        /// 获取不包含酒店id的字符串，主要用于去掉像班次id，房型id等以酒店id开头的数据，记录日志时，不需要记录酒店id
        /// 只有在字符串的前几位与酒店id相同的才会去掉
        /// </summary>
        /// <param name="originStr">需要去掉酒店id的原始字符串</param>
        /// <param name="splitChar">分隔符</param>
        /// <param name="joinChar">合并时的连接字符</param>
        /// <returns>去掉酒店id之后的字符串</returns>
        string GetStringWithoutHotelId(string originStr, char splitChar = ',', string joinChar = ",");
        /// <summary>
        /// 是否售后服务工程师使用客人授权进行登录
        /// </summary>
        /// <returns>true:是，false:是酒店操作员自行登录</returns>
        bool IsServiceOperatorLogin();

        /// <summary>
        /// 版本Id
        /// </summary>
        string VersionId { get; set; }
    }
}
