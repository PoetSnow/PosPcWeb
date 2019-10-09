using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.AuthManages
{
    public interface IRoleAuthItemConsumeService
    {
        /// <summary>
        /// 获取指定酒店下的可查看的消费入账角色权限
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店下的报表角色权限</returns>
        List<RoleAuthItemConsume> GetItemConsumeIsAllow(string hid, string roleid);

        /// <summary>
        /// 获取角色的消费项目入账权限
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        List<V_ReportList> GetV_roleItemConsumelist(string hid, string roleid);

        /// <summary>
        /// 设置角色消费项目入账权限
        /// </summary>
        /// <param name="roleid">角色编号</param>
        /// <param name="reportcodes">角色编号</param>
        /// <returns></returns>
        string ChangeRoleAuthItemConsume(string hid, string roleid, string reportcodes);

        /// <summary>
        /// 获取指定的用户的消费录入权限
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="userid">用户ID</param>
        /// <returns>key=消费项目ID，value=是否可操作</returns>
        List<BSPMS.Common.Tools.KeyValuePairModel<string, bool>> GetItemConsumeAuth(string hid, string userid);
    }
}
