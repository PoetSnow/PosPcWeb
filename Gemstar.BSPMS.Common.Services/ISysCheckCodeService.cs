using Gemstar.BSPMS.Common.Services.Entities;

namespace Gemstar.BSPMS.Common.Services
{
    public interface ISysCheckCodeService : ICRUDService<SysCheckCode>
    {
        /// <summary>
        /// 添加验证码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        JsonResultData AddCheckCode(SysCheckCode model);
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="method">验证方式</param>
        /// <param name="methodValue">验证方式的值：手机号或邮箱</param>
        /// <param name="func">验证针对的功能</param>
        /// <param name="seconds">距离下次可重发时间</param>
        /// <returns></returns>
        JsonResultData GetCheckCode(string method, string methodValue, string func, out int seconds);

        JsonResultData DeleteCheckCode(string method, string methodValue, string func);
    }
}
