using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Linq;
using System;

namespace Gemstar.BSPMS.Hotel.Services
{
    /// <summary>
    /// 单体酒店的角色成员服务接口
    /// </summary>
    public interface IUserRoleSingleService
    {
        /// <summary>
        /// 指定酒店的所有操作员中，不属于指定角色的操作员列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <returns>指定酒店的所有操作员中，不属于指定角色的操作员列表</returns>
        List<PmsUser> UsersNotInRole(string hid, string roleId);
        /// <summary>
        /// 指定酒店的所有操作员中，属于指定角色的操作员列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <returns>指定酒店的所有操作员中，属于指定角色的操作员列表</returns>
        List<PmsUser> UsersInRole(string hid, string roleId);
        /// <summary>
        /// 重设指定酒店指定角色的所有成员
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <param name="userIds">角色下所有成员id</param>
        /// <returns>重设结果</returns>
        JsonResultData ResetRoleMembers(string hid, string roleId, List<string> userIds);
        /// <summary>
        /// 重设指定酒店指定成员的所有角色
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="userId">成员id</param>
        /// <param name="roleIds">成员下的所有角色id</param>
        /// <returns>重设结果</returns>
        JsonResultData ResetMemberRoles(string hid, Guid userId, List<Guid> roleIds);
        /// <summary>
        /// 获取指定酒店指定用户的角色列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="userId">用户id</param>
        /// <returns>用户角色列表</returns>
        IQueryable<UserRole> List(string hid, Guid userId);

        IQueryable<UserRole> grpList(string hid, Guid userId, string grpId);

        /// <summary>
        /// 获取指定角色的用户
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="userId">角色id</param>
        /// <returns>用户角色列表</returns>
        List<UserRole> Listrole(string hid, Guid roleid);

        int isroleofhistory(string hid, string userid);
    }
}
