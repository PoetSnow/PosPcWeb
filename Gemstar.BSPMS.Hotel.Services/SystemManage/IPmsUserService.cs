using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services
{
    /// <summary>
    /// 酒店操作员服务接口
    /// </summary>
    public interface IPmsUserService : ICRUDService<PmsUser>
    {
        /// <summary>
        /// 获取指定操作员对指定集团下的可操作分店列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="userid">操作员id</param>
        /// <returns>操作员对该集团下的可操作分店列表</returns>
        List<UpQueryResortListForOperatorResult> GetResortListForOperator(string grpid, string userid);
        /// <summary>
        /// 查询指定的操作员的密码是否是默认密码
        /// </summary>
        /// <param name="userid">要检查的操作员id</param>
        /// <returns>true:是默认密码，false:不是，用户已经修改过密码了</returns>
        bool IsUserPassowrdDefault(string userid);
        /// <summary>
        /// 修改指定操作员的密码
        /// </summary>
        /// <param name="userId">操作员id</param>
        /// <param name="originPassword">原密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>修改结果</returns>
        JsonResultData ChangePassword(string userId, string originPassword, string newPassword);
        /// <summary>
        /// 批量更改状态（启用，禁用）
        /// </summary>
        /// <param name="ids">要更改的id，多项之间以逗号分隔</param>
        /// <param name="status">更新为当前状态</param>
        /// <returns>更改结果</returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status);

        /// <summary>
        /// 查询指定的操作员是否是注册用户（就是新建或注册酒店时那个用户，这个用户有所有权限）
        /// </summary>
        /// <param name="userid">要检查的操作员id</param>
        /// <returns>true:是默认密码，false:不是，用户已经修改过密码了</returns>
        bool IsRegUser(string userid);

        /// <summary>
        /// 判断登录名是否存在
        /// </summary>
        /// <param name="userid">要检查的code</param>
        /// <returns>true:存在，false:不存在</returns>
        bool IsExistsCode(string code, string hid);

        bool IsExists(string hid, string code, string name, Guid? notId = null);

        bool IsExists(string hid, string code, string name, string cardId, Guid? notId = null);
        /// <summary>
        /// 重置操作员密码
        /// </summary>
        /// <param name="userid">要检查的操作员id</param>
        /// <returns>重置结果</returns>
        JsonResultData ResetPwds(string[] userid);
        /// <summary>
        /// 获取指定酒店的注册用户的手机号
        /// </summary>
        /// <param name="grpid">集团id，如果是单体酒店就直接传酒店id</param>
        /// <returns>指定酒店的注册用户的手机号，如果不存在则返回null</returns>
        string GetRegUserMobile(string grpid);
        /// <summary>
        /// 获取指定酒店和卡号的操作员信息
        /// </summary>
        /// <param name="gid">集团ID</param>
        /// <param name="hid">酒店ID</param>
        /// <param name="cardId">卡号</param>
        /// <returns></returns>
        PmsUser GetEntityByCardId(string gid, string hid, string cardId);
        /// <summary>
        /// 获取用户姓名
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的用户姓名 </returns>
        string GetUserName(string hid, Guid userid);
        /// <summary>
        /// 获取指定集团下的所有操作员列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <returns>集团下所有操作员列表</returns>
        List<PmsUser> UsersInGroup(string grpid);
        /// <summary>
        /// 取消绑定指定操作员的微信信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="userId">操作员id</param>
        /// <returns>取消结果</returns>
        JsonResultData UnbindWeixin(string hid, string userId, Gemstar.BSPMS.Common.Services.EF.DbCommonContext centerDb);

        /// <summary>
        /// 集团增加下发分店数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="grpid"></param>
        /// <returns></returns>
        bool GroupControlAdd(string usercode, string Belonghotel, string grpid);
        /// <summary>
        /// 集团修改下发分店数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="grpid"></param>
        /// <returns></returns>
        bool GroupControlEdit(string usercode, string Belonghotel, string grpid);
        /// <summary>
        /// 获取分店同操作人的id
        /// </summary> 
        /// <returns></returns>
        string getGrouphotelid(string id);

        /// <summary>
        /// 根据酒店ID，代码，名称获取用户ID
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        PmsUser GetUserIDByCode(string hid, string code);
    }
}
