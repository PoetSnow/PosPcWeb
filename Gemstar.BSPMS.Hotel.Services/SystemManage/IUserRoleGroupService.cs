using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Linq;
using System;

namespace Gemstar.BSPMS.Hotel.Services
{
    /// <summary>
    /// 集团酒店的角色成员服务接口
    /// </summary>
    public interface IUserRoleGroupService
    {
        /// <summary>
        /// 指定酒店的所有操作员中，不属于指定角色的操作员列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <returns>指定酒店的所有操作员中，不属于指定角色的操作员列表</returns>
        List<PmsUser> UsersNotInRole(string grpid, string hid, string roleId);
        /// <summary>
        /// 指定酒店的所有操作员中，属于指定角色的操作员列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <returns>指定酒店的所有操作员中，属于指定角色的操作员列表</returns>
        List<PmsUser> UsersInRole(string grpid, string hid, string roleId);
        /// <summary>
        /// 重设指定酒店指定角色的所有成员
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <param name="userIds">角色下所有成员id</param>
        /// <returns>重设结果</returns>
        JsonResultData ResetRoleMembers(string grpid, string hid, string roleId, List<string> userIds);
        /// <summary>
        /// 重设指定酒店指定成员的所有角色
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="hid">酒店id</param>
        /// <param name="userId">成员id</param>
        /// <param name="roleIds">成员下所有角色id</param>
        /// <returns>重设结果</returns>
        JsonResultData ResetMemberRoles(string grpid, string hid, Guid userId, List<Guid> roleIds);
        /// <summary>
        /// 重设指定成员在集团内所有分店的所有角色
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="userId">成员id</param>
        /// <param name="hotelAndRoleIds">成员所属的酒店和角色id，以|分隔</param>
        /// <returns>重设结果</returns>
        JsonResultData ResetMemberHotelRoles(string grpid, Guid userId, string[] hotelAndRoleIds);
        /// <summary>
        /// 重设指定角色在集团内所有分店的成员
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="roleId">角色id</param>
        /// <param name="hotelAndMemberIds">成员所属的酒店和用户id，以｜分隔</param>
        /// <returns>重设结果</returns>
        JsonResultData ResetRoleHotelMembers(string grpid, Guid roleId, string[] hotelAndMemberIds);
        /// <summary>
        /// 获取指定酒店的用户角色列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="hid">酒店id</param>
        /// <param name="userId">用户id</param>
        /// <returns>用户角色列表</returns>
        IQueryable<UserRole> List(string grpid, string hid, Guid userId);
        /// <summary>
        /// 获取指定用户在集团下的所有酒店的角色列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="userId">用户id</param>
        /// <returns>用户在集团下的所有酒店的角色列表</returns>
        List<UserRole> ListHotelRoles(string grpid, Guid userId);
        /// <summary>
        /// 获取指定角色在集团下的所有成员列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="roleId">角色id</param>
        /// <returns>指定角色在集团下的所有成员列表</returns>
        List<UserRole> ListHotelMembersInRole(string grpid, string roleId);
        /// <summary>
        /// 获取指定角色的用户
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="roleid">角色id</param>
        /// <returns>用户角色列表</returns>
        List<UserRole> Listrole(string hid, Guid roleid, string grpid);

    }
}
