using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.Entities;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 产品服务接口
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// 获取所有产品列表
        /// </summary>
        /// <returns>所有产品列表</returns>
        List<M_v_products> GetAllProducts();

        /// <summary>
        /// 获取指定酒店的产品代码列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的产品代码列表</returns>
        List<string> GetHotelProducts(string hid);

        /// <summary>
        /// 获取已开通指定产品的集团分店列表
        /// </summary>
        /// <param name="grpid">集团ID</param>
        /// <param name="productCode">产品代码</param>
        /// <returns></returns>
        List<string> GetHotels(string grpid, string productCode);

        /// <summary>
        /// 获取当前的访问域名来获取对应的产品实例
        /// </summary>
        /// <param name="domain">当前访问域名对应的产品实例</param>
        M_v_products GetProductByDomain(string domain);

        /// <summary>
        /// 获取指定代码产品实例
        /// </summary>
        /// <param name="code">产品代码</param>
        /// <returns>指定代码对应的产品实例</returns>
        M_v_products GetProductByCode(string code);

        /// <summary>
        /// 设置指定酒店的产品模块
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="productCodes">产品模块代码列表</param>
        /// <returns>设置结果</returns>
        JsonResultData SetHotelProducts(string hid, List<string> productCodes);
    }
}