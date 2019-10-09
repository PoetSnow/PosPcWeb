using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services
{
    /// <summary>
    /// 角色的服务接口
    /// </summary>
    public interface IRoleService:ICRUDService<Role>
    {
        /// <summary>
        /// 获取指定酒店的角色列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        IQueryable<Role> List(string hid);

        string getRoleName(Guid roleid);
    }
}
