using Gemstar.BSPMS.Common.Services.Entities;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Common.Services
{
    public interface ISysParaService : ICRUDService<SysPara>
    {
        /// <summary>
        /// 获取七牛配置参数
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetQiniuPara();
        /// <summary>
        /// 获取后台添加酒店时默认的服务器数据库参数
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetDefaultConnPara();
        /// <summary>
        /// 获取用户注册酒店时默认的服务器数据库参数
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetDefaultRegisterConnPara();
        /// <summary>
        /// 获取发送短信配置参数
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetSMSSendPara();
        /// <summary>
        /// 获取发送邮件配置参数
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetEmailSendPara();

        /// <summary>
        /// 获取酒店功能设置
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        List<HotelFunctions> GetHotelFunctionses(string hid);
        /// <summary>
        /// 获取到期提醒的参数
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetExpiredPara(); 
    }
}
