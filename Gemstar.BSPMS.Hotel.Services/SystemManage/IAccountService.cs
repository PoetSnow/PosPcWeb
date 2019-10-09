using Gemstar.BSPMS.Common.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Services
{
    /// <summary>
    /// 登录服务接口
    /// 此接口里面的方法要求只能从中央数据库获取数据，因为此时还没有登录，无法获取酒店业务数据库数据
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// 使用指定的酒店id，用户名和密码进行登录
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="username">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns>登录操作结果</returns>
        AccountInfo Login(string hid, string username, string pwd);
        AccountInfo AutoLogin(string hid, string username, string pwd);
        string CheckUserinfo(string hid, string code, string type, string value);

    }
}
