using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class UserRoleSingleService : IUserRoleSingleService
    {
        public UserRoleSingleService(DbHotelPmsContext pmsContext)
        {
            _pmsContext = pmsContext;
        }
        /// <summary>
        /// 指定酒店的所有操作员中，不属于指定角色的操作员列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <returns>指定酒店的所有操作员中，不属于指定角色的操作员列表</returns>
        public List<PmsUser> UsersInRole(string hid, string roleId)
        {
            return _pmsContext.Database.SqlQuery<PmsUser>("exec up_queryUsersInRole_single @hid=@hid,@roleId=@roleId", new SqlParameter("@hid", hid), new SqlParameter("@roleId", roleId)).ToList();
        }

        /// <summary>
        /// 指定酒店的所有操作员中，属于指定角色的操作员列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <returns>指定酒店的所有操作员中，属于指定角色的操作员列表</returns>
        public List<PmsUser> UsersNotInRole(string hid, string roleId)
        {
            return _pmsContext.Database.SqlQuery<PmsUser>("exec up_queryUsersNotInRole_single @hid=@hid,@roleId=@roleId", new SqlParameter("@hid", hid), new SqlParameter("@roleId", roleId)).ToList();
        }
        /// <summary>
        /// 重设指定酒店指定角色的所有成员
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <param name="userIds">角色下所有成员id</param>
        /// <returns>重设结果</returns>
        public JsonResultData ResetRoleMembers(string hid, string roleId, List<string> userIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hid))
                {
                    throw new ArgumentException("请指定要重设角色成员的酒店id", "hid");
                }
                if (string.IsNullOrWhiteSpace(roleId))
                {
                    throw new ArgumentException("请指定要重设角色成员的角色id", "roleId");
                }
                //删除指定酒店，指定角色下的所有成员
                var roleIdValue = Guid.Parse(roleId);
                var oldMembers = _pmsContext.UserRoles.Where(w => w.Hid == hid && w.Roleid == roleIdValue);
                _pmsContext.UserRoles.RemoveRange(oldMembers);
                //再依次增加 
                if (userIds != null)
                {
                    foreach (var userId in userIds)
                    {
                        var userRole = new UserRole
                        {
                            Grpid = "",
                            Hid = hid,
                            Id = Guid.NewGuid(),
                            Userid = Guid.Parse(userId),
                            Roleid = roleIdValue
                        };
                        _pmsContext.UserRoles.Add(userRole);
                    }
                }
                // _pmsContext.AddDataChangeLogs(OpLogType.角色成员修改);
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 重设指定酒店指定成员的所有角色
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="userId">成员id</param>
        /// <param name="roleIds">成员下的所有角色id</param>
        /// <returns>重设结果</returns>
        public JsonResultData ResetMemberRoles(string hid, Guid userId, List<Guid> roleIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hid))
                {
                    throw new ArgumentException("请指定要重设成员角色的酒店id", "hid");
                }
                //删除指定酒店，指定角色下的所有成员
                var oldMembers = _pmsContext.UserRoles.Where(w => w.Hid == hid && w.Userid == userId);
                _pmsContext.UserRoles.RemoveRange(oldMembers);
                //再依次增加
                foreach (var roleId in roleIds)
                {
                    var userRole = new UserRole
                    {
                        Grpid = "",
                        Hid = hid,
                        Id = Guid.NewGuid(),
                        Userid = userId,
                        Roleid = roleId
                    };
                    _pmsContext.UserRoles.Add(userRole);
                }
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 获取指定酒店指定用户的角色列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="userId">用户id</param>
        /// <returns>用户角色列表</returns>
        public IQueryable<UserRole> List(string hid, Guid userId)
        {

            return _pmsContext.UserRoles.Where(c => c.Hid == hid && c.Userid == userId && c.Grpid == "");
        }

        public IQueryable<UserRole> grpList(string hid, Guid userId, string grpId)
        {

            return _pmsContext.UserRoles.Where(c => c.Hid == hid && c.Userid == userId && c.Grpid == grpId);
        }

        /// <summary>
        /// 获取指定角色的用户
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="userId">角色id</param>
        /// <returns>用户角色列表</returns>
        public List<UserRole> Listrole(string hid, Guid roleid)
        {
            return _pmsContext.UserRoles.Where(c => c.Hid == hid && c.Roleid == roleid && c.Grpid == "").ToList();
        }

        public int isroleofhistory(string hid, string userid)
        {
            return _pmsContext.Database.SqlQuery<int>("exec up_roleofhistory @h99hid=@h99hid,@userid=@userid", new SqlParameter("@h99hid", hid), new SqlParameter("@userid", userid)).FirstOrDefault();

        }

        private DbHotelPmsContext _pmsContext;
    }
}
