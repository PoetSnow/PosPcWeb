using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface ICompanyLogService : ICRUDService<CompanyLog>
    {
        /// <summary>
        /// 简单增加对象
        /// </summary>
        /// <param name="companyid">合约单位ID</param>
        /// <param name="type">更改类型</param>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        void AddSimple(Guid companyid, string type, string oldValue, string newValue);
    }
}
