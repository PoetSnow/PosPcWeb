using System;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using System.Data;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Common.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Services.ReportManage
{
    /// <summary>
    /// 报表服务
    /// </summary>
    public interface IReportService : ICRUDService<ReportFormat>
    {
        /// <summary>
        /// 获取报表中文名称
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns></returns>
        string GetReportName(string code, byte productmask);
        /// <summary>
        /// 获取指定报表的所有格式名称
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="templateName">报表号</param>
        /// <returns>报表号所有的格式名称</returns>
        List<string> GetStyleNames(string hid, string templateName);

        /// <summary>
        /// 是否存在自定义报表模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="templateName">模板名称</param>
        /// <param name="styleName">格式名称</param>
        /// <returns></returns>
        bool IsExistsTemplate(string hid, string templateName, string styleName);

        /// <summary>
        /// 获取自定义报表模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="templateName">模板名称</param>
        /// <param name="styleName">格式名称</param>
        /// <returns></returns>
        string GetTemplate(string hid, string templateName, string styleName);

        /// <summary>
        /// 获取自定义报表模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="templateName">模板名称</param>
        /// <returns></returns>
        ReportFormat GetReportFormat(string hid, string templateName);

        /// <summary>
        /// 保存自定义报表模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="templateName">模板名称</param>
        /// <param name="templateValue">模板值</param>
        /// <param name="styleName">格式名称</param>
        /// <returns></returns>
        bool SaveTemplate(string hid, string templateName, string templateValue, string styleName);

        /// <summary>
        /// 删除自定义报表模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="templateName">模板名称</param>
        /// <param name="styleName">格式名称</param>
        /// <returns></returns>
        bool DelTemplate(string hid, string templateName, string styleName);

        /// <summary>
        /// 添加查询参数临时数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        Guid? AddQueryParaTemp(string hid, string value);

        /// <summary>
        /// 移除查询参数临时数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        string GetQueryParaTemp(string hid, Guid id);

        /// <summary>
        /// 获取报表类型
        /// </summary>
        /// <returns></returns>
        List<string> GetReportType(ProductType productType);
        /// <summary>
        /// 判断是否有指定报表权限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="hid"></param>
        /// <param name="reportCode"></param>
        /// <returns></returns>
        bool IsReportauth(string uid, string hid, string reportCode);
        /// <summary>
        /// 判断是否有可以查看的报表
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="hid"></param>
        /// <returns></returns>
        bool IsReportauth(string uid, string hid);

        /// <summary>
        /// 获取报表中文名称
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns></returns>
        string GetFileName(string code, byte productmask);

        /// <summary>
        /// 添加电子签名
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool InsertSignature(SignatureLog data);

        /// <summary>
        /// 获取电子签名列表
        /// </summary>
        /// <param name="sType"></param>
        /// <returns></returns>
        IQueryable<SignatureLog> GetSignatureLogList(string sType, string hid);

        /// <summary>
        /// 删除电子签名
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteSignature(string id);

        /// <summary>
        /// 根据存储过程名称获取存储过程参数相关信息
        /// </summary>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        List<UpQueryProcedureParametersResult> GetProcedureParameters(string procedureName);

        List<ReportFormat> GetReportFormatList(string hid, string templateName);


        /// <summary>
        /// 设置用户收藏报表
        /// </summary>
        /// <param name="reportCode"></param>
        /// <param name="isCollect"></param>
        void setUserReportCollect(string reportCode, bool isCollect, Guid userId);
    }
}
