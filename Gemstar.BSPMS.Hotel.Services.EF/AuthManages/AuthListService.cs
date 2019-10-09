using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;

namespace Gemstar.BSPMS.Hotel.Services.EF.AuthManages
{
    public class AuthListService : IAuthListService
    {
        public AuthListService(DbHotelPmsContext pmsContext)
        {
            _pmsContext = pmsContext;
        }
        public List<UpQueryAuthListForRoleResult> GetAllAuthLists(ProductType productType, AuthType authType, string hid, string roleId)
        {
            return _pmsContext.Database.SqlQuery<UpQueryAuthListForRoleResult>(
                "exec up_queryAuthListForRole @roleId=@roleId,@hid=@hid,@authType=@authType,@productType=@productType"
                , new SqlParameter("@roleId", roleId)
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@authType", (int)authType)
                , new SqlParameter("@productType", (int)productType)
                ).ToList();
        }
        /// <summary>
        /// 云POS高级功能按钮权限查询
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<UpQueryAuthListForRoleResult> GetAdvanceFuncLists(string hid, string roleId)
        {
            return _pmsContext.Database.SqlQuery<UpQueryAuthListForRoleResult>(
                "exec up_queryAdvanceFuncListForRole @roleId=@roleId,@hid=@hid"
                , new SqlParameter("@roleId", roleId)
                , new SqlParameter("@hid", hid)).ToList();
        }
        /// <summary>
        /// 重设指定角色在指定酒店内的所有权限
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="hid">酒店id</param>
        /// <param name="auths">角色拥有的当前所有权限</param>
        /// <returns>重设权限结果</returns>
        public JsonResultData ResetRoleAuths(ProductType productType, string roleId, string hid, List<AuthInfo> auths)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleId))
                {
                    return JsonResultData.Failure("请指定要重设权限的角色");
                }
                if (string.IsNullOrWhiteSpace(hid))
                {
                    return JsonResultData.Failure("请指定要重设权限的酒店id");
                }
                var roleIdValue = Guid.Parse(roleId);
                //删除的时候，也必须把数据取出来，再通过ef来删除，否则会报主键重复或者其他状态问题
                //var oldAuths = _pmsContext.RoleAuths.Where(w => w.RoleId == roleIdValue && w.Hid == hid);
                var oldAuths = _pmsContext.Database.SqlQuery<RoleAuth>("select r.*  FROM dbo.roleAuth AS r INNER JOIN dbo.AuthList AS a ON a.AuthCode = r.AuthCode WHERE hid = {0} AND roleid = {1} AND SUBSTRING(a.mask,{2},1)='1'", hid ?? "", roleId ?? "", (int)productType).ToList();
                foreach (var delAuth in oldAuths)
                {
                    var del = _pmsContext.RoleAuths.Single(w => w.Hid == delAuth.Hid && w.RoleId == delAuth.RoleId && w.AuthCode == delAuth.AuthCode);
                    _pmsContext.RoleAuths.Remove(del);
                }
                //_pmsContext.RoleAuths.RemoveRange(oldAuths);
                foreach (var auth in auths)
                {
                    var roleAuth = new RoleAuth
                    {
                        Hid = hid,
                        RoleId = roleIdValue,
                        AuthCode = auth.AuthCode,
                        AuthButtonValue = (Int64)auth.AuthValues
                    };
                    _pmsContext.RoleAuths.Add(roleAuth);
                }
                // _pmsContext.AddDataChangeLogs(OpLogType.角色操作权限修改);
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 获取指定角色的操作权限
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="roleid">角色id</param>
        /// <returns> </returns>
        public List<AuthInfo> Listrole(string hid, Guid roleid)
        {
            List<RoleAuth> roles = _pmsContext.RoleAuths.Where(w => w.RoleId == roleid && w.Hid == hid).ToList();
            List<AuthInfo> r = new List<AuthInfo>();
            AuthInfo ai = null;
            foreach (var auth in roles)
            {
                ai = new AuthInfo();
                ai.AuthCode = auth.AuthCode; ai.AuthValues = (AuthFlag)auth.AuthButtonValue;
                r.Add(ai);
            }
            return r;
        }
        public string GetAuthNamebycode(string code)
        {
            if (code == "" || _pmsContext.AuthLists.Where(w => w.AuthCode == code).Count() <= 0)
            {
                return "";
            }
            return _pmsContext.AuthLists.Where(w => w.AuthCode == code).FirstOrDefault().AuthName;

        }
        public bool isrootauth(string authcode)
        {
            return _pmsContext.AuthLists.Where(w => w.AuthCode == authcode && w.ParentCode == "1").ToList().Count > 0;
        }

        public List<UpQueryAuthListForRoleResult> GetAllAuthLists(ProductType productType, AuthType authType, string hid, string roleId, string QueryText)
        {
            return _pmsContext.Database.SqlQuery<UpQueryAuthListForRoleResult>(
                "exec up_queryAuthListForRole @roleId=@roleId,@hid=@hid,@authType=@authType,@productType=@productType,@queryText=@queryText"
                , new SqlParameter("@roleId", roleId)
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@authType", (int)authType)
                , new SqlParameter("@productType", (int)productType)
                , new SqlParameter("@queryText", QueryText)
                ).ToList();
        }

        /// <summary>
        /// 根据用户获取用户对应的权限列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="uid">用户ID</param>
        /// <returns></returns>
        public List<up_pos_list_roleAuthResult> GetUserAuthLists(string gid,string hid, string uid)
        {
            return _pmsContext.Database.SqlQuery<up_pos_list_roleAuthResult>(
                "exec up_pos_list_roleAuth @gid=@gid,@hid=@hid,@uid=@uid"
                , new SqlParameter("@gid", gid)
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@uid", uid)
                ).ToList();
        }

        private DbHotelPmsContext _pmsContext;
    }
}
