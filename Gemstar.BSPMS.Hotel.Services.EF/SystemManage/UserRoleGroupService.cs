using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class UserRoleGroupService : IUserRoleGroupService
    {
        public UserRoleGroupService(DbHotelPmsContext pmsContext)
        {
            _pmsContext = pmsContext;
        }
        /// <summary>
        /// 指定酒店的所有操作员中，不属于指定角色的操作员列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <returns>指定酒店的所有操作员中，不属于指定角色的操作员列表</returns>
        public List<PmsUser> UsersInRole(string grpid, string hid, string roleId)
        {
            return _pmsContext.Database.SqlQuery<PmsUser>("exec up_queryUsersInRole_group @grpid=@grpid,@hid=@hid,@roleId=@roleId",new SqlParameter("@grpid",grpid), new SqlParameter("@hid",hid),new SqlParameter("@roleId",roleId)).ToList();
        }

        /// <summary>
        /// 指定酒店的所有操作员中，不属于指定角色的操作员列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <returns>指定酒店的所有操作员中，不属于指定角色的操作员列表</returns>
        public List<PmsUser> UsersNotInRole(string grpid, string hid, string roleId)
        {
            return _pmsContext.Database.SqlQuery<PmsUser>("exec up_queryUsersNotInRole_group @grpid=@grpid,@hid=@hid,@roleId=@roleId",new SqlParameter("@grpid", grpid), new SqlParameter("@hid", hid), new SqlParameter("@roleId", roleId)).ToList();
        }
        /// <summary>
        /// 重设指定酒店指定角色的所有成员
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <param name="userIds">角色下所有成员id</param>
        /// <returns>重设结果</returns>
        public JsonResultData ResetRoleMembers(string grpid, string hid, string roleId, List<string> userIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(grpid))
                {
                    throw new ArgumentException("请指定要重设角色成员的集团id","grpid");
                }
                if (string.IsNullOrWhiteSpace(hid))
                {
                    throw new ArgumentException("请指定要重设角色成员的酒店id","hid");
                }
                if (string.IsNullOrWhiteSpace(roleId))
                {
                    throw new ArgumentException("请指定要重设角色成员的角色id","roleId");
                }
                //删除指定酒店，指定角色下的所有成员
                var roleIdValue = Guid.Parse(roleId);
                var oldMembers = _pmsContext.UserRoles.Where(w => w.Hid == hid && w.Roleid == roleIdValue && w.Grpid == grpid);
                _pmsContext.UserRoles.RemoveRange(oldMembers);
                //再依次增加
                foreach(var userId in userIds)
                {
                    var userRole = new UserRole
                    {
                        Grpid = grpid,
                        Hid = hid,
                        Id = Guid.NewGuid(),
                        Userid = Guid.Parse(userId),
                        Roleid = roleIdValue
                    };
                    _pmsContext.UserRoles.Add(userRole);
                }
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }catch(Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 重设指定酒店指定成员的所有角色
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="hid">酒店id</param>
        /// <param name="userId">成员id</param>
        /// <param name="roleIds">成员下所有角色id</param>
        /// <returns>重设结果</returns>
        public JsonResultData ResetMemberRoles(string grpid, string hid, Guid userId, List<Guid> roleIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(grpid))
                {
                    throw new ArgumentException("请指定要重设成员角色的集团id", "grpid");
                }
                if (string.IsNullOrWhiteSpace(hid))
                {
                    throw new ArgumentException("请指定要重设成员角色的酒店id", "hid");
                }
                //删除指定酒店，指定角色下的所有成员
                var oldMembers = _pmsContext.UserRoles.Where(w => w.Hid == hid && w.Userid == userId && w.Grpid == grpid);
                _pmsContext.UserRoles.RemoveRange(oldMembers);
                //再依次增加
                foreach (var roleId in roleIds)
                {
                    var userRole = new UserRole
                    {
                        Grpid = grpid,
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
        /// 重设指定成员在集团内所有分店的所有角色
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="userId">成员id</param>
        /// <param name="hotelAndRoleIds">成员所属的酒店和角色id，以|分隔</param>
        /// <returns>重设结果</returns>
        public JsonResultData ResetMemberHotelRoles(string grpid, Guid userId, string[] hotelAndRoleIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(grpid))
                {
                    throw new ArgumentException("请指定要重设成员角色的集团id", "grpid");
                }
                //删除原所属分店和角色
                var oldMembers = _pmsContext.UserRoles.Where(w => w.Userid == userId && w.Grpid == grpid);
                _pmsContext.UserRoles.RemoveRange(oldMembers);
                //再依次增加
                foreach (var hotelAndRoleId in hotelAndRoleIds)
                {
                    var idsInfo = hotelAndRoleId.Split('|');
                    if (idsInfo.Length > 1)
                    {
                        var userRole = new UserRole
                        {
                            Grpid = grpid,
                            Hid = idsInfo[0],
                            Id = Guid.NewGuid(),
                            Userid = userId,
                            Roleid = Guid.Parse(idsInfo[1])
                        }; _pmsContext.UserRoles.Add(userRole);
                    }
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
        /// 重设指定角色在集团内所有分店的成员
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="roleId">角色id</param>
        /// <param name="hotelAndMemberIds">成员所属的酒店和用户id，以｜分隔</param>
        /// <returns>重设结果</returns>
        public JsonResultData ResetRoleHotelMembers(string grpid, Guid roleId, string[] hotelAndMemberIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(grpid))
                {
                    throw new ArgumentException("请指定要重设成员角色的集团id", "grpid");
                }
                //删除原所属分店和角色
                var oldMembers = _pmsContext.UserRoles.Where(w => w.Roleid == roleId && w.Grpid == grpid);
                _pmsContext.UserRoles.RemoveRange(oldMembers);
                //再依次增加
                foreach (var hotelAndRoleId in hotelAndMemberIds)
                {
                    var idsInfo = hotelAndRoleId.Split('|');
                    if (idsInfo.Length > 1)
                    {
                        var userRole = new UserRole
                        {
                            Grpid = grpid,
                            Hid = idsInfo[0],
                            Id = Guid.NewGuid(),
                            Userid = Guid.Parse(idsInfo[1]),
                            Roleid = roleId
                        }; _pmsContext.UserRoles.Add(userRole);
                    }
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
        /// 获取指定酒店的用户角色列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="hid">酒店id</param>
        /// <param name="userId">用户id</param>
        /// <returns>用户角色列表</returns>
        public IQueryable<UserRole> List(string grpid, string hid, Guid userId)
        {
            return _pmsContext.UserRoles.Where(c => c.Grpid == grpid && c.Hid == hid && c.Userid == userId);
        }

        /// <summary>
        /// 获取指定用户在集团下的所有酒店的角色列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="userId">用户id</param>
        /// <returns>用户在集团下的所有酒店的角色列表</returns>
        public List<UserRole> ListHotelRoles(string grpid, Guid userId)
        {
            return _pmsContext.UserRoles.Where(c => c.Grpid == grpid && c.Userid == userId).OrderBy(c=>c.Hid).ToList();
        }

        /// <summary>
        /// 获取指定角色在集团下的所有成员列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="roleId">角色id</param>
        /// <returns>指定角色在集团下的所有成员列表</returns>
        public List<UserRole> ListHotelMembersInRole(string grpid, string roleId)
        {
            var roleIdValue = Guid.Parse(roleId);
            return _pmsContext.UserRoles.Where(c => c.Grpid == grpid && c.Roleid == roleIdValue).OrderBy(c=>c.Hid).ToList();
        }
        /// <summary>
        /// 获取指定角色的用户
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="roleid">角色id</param>
        /// <returns>用户角色列表</returns>
        public List<UserRole> Listrole(string hid, Guid roleid,string grpid)
        {
            return _pmsContext.UserRoles.Where(c => c.Hid == hid && c.Roleid == roleid && c.Grpid == grpid).ToList();
        }
        private DbHotelPmsContext _pmsContext;
    }
}
