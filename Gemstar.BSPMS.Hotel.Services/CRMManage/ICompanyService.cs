using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Hotel.Services.CRMManage;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface ICompanyService : ICRUDService<Company>
    {
        /// <summary>
        /// 批量更改状态（启用，禁用）
        /// </summary>
        /// <param name="ids">要更改的id，多项之间以逗号分隔</param>
        /// <param name="status">更新为当前状态</param>
        /// <returns>更改结果</returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status, OpLogType logType, ICurrentInfo currentInfo);

        /// <summary>
        /// 延期
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="date">延期时间</param>
        JsonResultData DelayValidDate(Guid id, DateTime date, OpLogType logType);
        /// <summary>
        /// 延期
        /// </summary>
        /// <param name="ids">以逗号分隔的主键ID</param>
        /// <param name="date">延期时间</param>
        JsonResultData BatchDelayValidDate(string ids, DateTime date, OpLogType logType, ICurrentInfo currentInfo);
        /// <summary>
        /// 批量更换业务员
        /// </summary>
        /// <param name="ids">以逗号分隔的主键ID</param>
        /// <param name="saleMan">新业务员</param>
        JsonResultData BatchUpdateSales(string ids, string saleMan, OpLogType logType, ICurrentInfo currentInfo);

        /// <summary>
        /// 获取合约单位键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="name">合约单位名称</param>
        /// <returns></returns>
        List<KeyValuePair<string, string>> List(string hid, string name);
        /// <summary>
        /// 获取合约单位键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="name">合约单位名称</param>
        /// <param name="notId">不包含合约单位ID</param>
        /// <returns></returns>
        List<KeyValuePair<string, string>> List(string hid, string name, Guid notId);
        /// <summary>
        /// 获取合约单位键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        List<Gemstar.BSPMS.Common.Tools.KeyValuePairModel<string, string>> GetCompanyInfoList(string hid, string keyword);
        /// <summary>
        /// 查询满足指定关键字的合约单位列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="keyword">关键字</param>
        /// <returns>包含指定关键字的合约单位列表</returns>
        List<CompanyPayListItem> Query(string hid, string keyword);
        /// <summary>
        /// 指定酒店的合约单位中是否已使用此类别
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="companyTypeId">合约单位类别ID</param>
        /// <returns></returns>
        bool IsExistsCompanyType(string hid, string companyTypeId);

        /// <summary>
        /// 根据主键ID获取合约单位信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">合约单位主键ID</param>
        /// <returns></returns>
        JsonResultData GetCompanyInfo(string hid, Guid id);

        /// <summary>
        /// 根据代码检查合约单位是否存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        bool IsCompany(string hid, string code);
        /// <summary>
        /// 检查合约单位名称是否存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsCompanyName(string hid, string name);
        /// <summary>
        /// 获取合约单位金额信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="companyId">合约单位ID</param>
        /// <returns></returns>
        CommpanyBlanceInfo GetCommpanyBlance(string hid, Guid companyId);

        /// <summary>
        /// 获取合约单位签约图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<CompanySinImg> getCompanySignImg(string hid,Guid id);

        /// <summary>
        /// 新增图片
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="company"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        JsonResultData AddCompanyImage(string hid,Guid company,string name, string url);

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        JsonResultData DelCompanyImage(int id);
        /// <summary>
        /// 发送合约单位营销短信
        /// </summary>
        /// <param name="ids">合约单位号</param>
        /// <param name="mobiles">手机号</param>
        /// <param name="content">发送内容</param>
        /// <returns></returns>
        JsonResultData SendMarketSms(string hid,string ids, string mobiles, string content);
    }
}
