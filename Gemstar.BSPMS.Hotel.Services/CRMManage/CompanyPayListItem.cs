using System.Collections.Generic;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.CRMManage
{
    /// <summary>
    /// 合约单位挂账时，合约单位自动完成下拉选项
    /// 由于合约单位挂账时，需要根据合约单位是否有签单人来进行不同的操作，所以除了需要的代码和名称外，同时需要返回所有的签单人信息
    /// </summary>
    public class CompanyPayListItem
    {
        /// <summary>
        /// 合约单位id
        /// </summary>
        public string CompanyId { get; set; }
        /// <summary>
        /// 合约单位名称
        /// </summary>
        public string Name { get; set; }
        public List<CompanySigner> Signers { get; set; }
    }
}
