using System;
using System.Collections.Generic;
using System.Linq;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.CRMManage;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.EF.CRMManage
{
    public class CompanySignerService : CRUDService<CompanySigner>, ICompanySignerService
    {
        public CompanySignerService(DbHotelPmsContext pmsContext) : base(pmsContext, pmsContext.CompanySigners)
        {
            _pmsContext = pmsContext;
        }

        protected override CompanySigner GetTById(string id)
        {
            return new CompanySigner
            {
                Id = Guid.Parse(id)
            };
        }

        /// <summary>
        /// 获取指定合约单位下的签单人列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="companyId">合约单位id</param>
        /// <returns>合约单位下的签单人列表</returns>
        public List<CompanySigner> GetSignerByCompany(string hid, Guid companyId)
        {
            return _pmsContext.CompanySigners.Where(w => w.Hid == hid && w.CompanyId == companyId).ToList();
        }
        private readonly DbHotelPmsContext _pmsContext;
    }
}
