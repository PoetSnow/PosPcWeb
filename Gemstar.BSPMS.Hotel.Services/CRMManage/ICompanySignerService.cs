using System;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.CRMManage
{
    /// <summary>
    /// 合约单位签单人服务接口
    /// </summary>
    public interface ICompanySignerService:ICRUDService<CompanySigner>
    {
        /// <summary>
        /// 获取指定合约单位下的签单人列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="companyId">合约单位id</param>
        /// <returns>合约单位下的签单人列表</returns>
        List<CompanySigner> GetSignerByCompany(string hid,Guid companyId);
    }
}
