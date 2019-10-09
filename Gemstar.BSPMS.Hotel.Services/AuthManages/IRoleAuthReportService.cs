using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.AuthManages
{
    /// <summary>
    /// 报表权限
    /// </summary>
    public interface IRoleAuthReportService
    {
        /// <summary>
        /// 获取指定酒店下的报表角色权限
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店下的报表角色权限</returns>
        List<RoleAuthReport> GetAllRoleAuthReports(string hid,string roleid);

        /// <summary>
        /// 获取指定酒店下的可查看的报表角色权限
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店下的报表角色权限</returns>
        List<RoleAuthReport> GetReportsIsAllow(string hid, string roleid);
      
        /// <summary>
        /// 获取所有报表
        /// </summary>  
        List<V_ReportList> GetV_ReportLists(ProductType productType,string hid,string roleid);

        /// <summary>
        /// 设置角色报表权限
        /// </summary>
        /// <param name="roleid">角色编号</param>
        /// <param name="reportcodes">角色编号</param>
        /// <returns></returns>
        string ChangeRoleAuthReport(ProductType productType,string hid,string roleid, string reportcodes);
    }
}
