using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using Gemstar.BSPMS.Hotel.Services.ReportManage;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Common;
using Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using System.Web.Script.Serialization;
using System.Text;
using Gemstar.BSPMS.Hotel.Services.ResFolioManage;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using StackExchange.Redis;
using RedisSessionProvider.Config;
using System.Web.Caching;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models
{
    /// <summary>
    /// 报表基类
    /// </summary>
    public class SRReportBaseController : BaseController
    {
        /// <summary>
        /// 获取报表对象。如果有自定义则返回自定义对象，没有则返回标准对象。
        /// </summary>
        protected StiReport GetReport(ReportQueryModel model)
        {
            try
            {
                var rpt = new SrReport(model.ReportCode, GetReportFilePath(model.ReportCode,model.StyleName), GetService<IReportService>(), CurrentInfo, model.ChineseName,model.StyleName);
                string procedureParametersStr = null;
                string queryCondition = "";

                var parameters = GetService<ICommonQueryService>().GetProcedureParameters(rpt.GetProcedureName());
                var queryHelper = new CommonQueryHelper(model.ParameterValues);
                queryHelper.SetHiddleParaValuesFromCurrentInfo(CurrentInfo, parameters);
                queryHelper.SetParaValue("@hid", CurrentInfo.HotelId, parameters);
                bool isHasHid = false;//是否有酒店ID
                bool isSearchAll = false;//是否有查询全部选项
                foreach (var item in parameters)
                {
                    var DisplayName = CommonQueryParameterHelper.GetDisplayName(item.ParameterName);
                    if(!string.IsNullOrWhiteSpace(item.DisplayParameterName) && item.DisplayParameterName != DisplayName)
                    {
                        DisplayName = item.DisplayParameterName;
                    }
                    var ParameterValueText = "";
                    var ParameterValue = queryHelper[item.ParameterName];
                    if (!string.IsNullOrWhiteSpace(ParameterValue))
                    {
                        string[] valueAndText = ParameterValue.Split('^');
                        if (valueAndText.Length > 1)
                        {
                            ParameterValue = valueAndText[0];
                            ParameterValueText = valueAndText[1];
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(ParameterValue))
                    {
                        switch (item.DataType)
                        {
                            case "date":
                                {
                                    DateTime date = new DateTime();
                                    if (!DateTime.TryParse(ParameterValue, out date))
                                    {
                                        throw new Exception(string.Format("查询条件[{0}]错误。", DisplayName));
                                    }
                                }
                                break;
                            case "datetime":
                                {
                                    DateTime datetime = new DateTime();
                                    if (!DateTime.TryParse(ParameterValue, out datetime))
                                    {
                                        throw new Exception(string.Format("查询条件[{0}]错误。", DisplayName));
                                    }
                                }
                                break;
                            case "tinyint":
                                {
                                    int tinyint = 0;
                                    if (!Int32.TryParse(ParameterValue, out tinyint) && tinyint >= 0 && tinyint <= 255)
                                    {
                                        throw new Exception(string.Format("查询条件[{0}]错误。", DisplayName));
                                    }
                                }
                                break;
                        }
                        procedureParametersStr += string.Format("{0}='{1}',", item.ParameterName, ParameterValue);
                        if (item.ParameterName != "@h99grpid" && item.ParameterName != "@h99hid" && item.ParameterName != "@hid")
                        {
                            queryCondition += string.Format("{0}：{1}，", DisplayName, ParameterValueText);
                        }
                        else
                        {
                            isHasHid = true;
                        }
                    }
                    else
                    {
                        if (ParameterValueText == "全部")
                        {
                            isSearchAll = true;
                        }
                    }
                }
                if (!isHasHid)
                {
                    throw new Exception("请指定酒店ID。");
                }
                if (!string.IsNullOrWhiteSpace(procedureParametersStr))
                {
                    procedureParametersStr = procedureParametersStr.Substring(0, procedureParametersStr.Length - 1);
                }
                var srReport = rpt.GetReport(procedureParametersStr, isSearchAll);
                if (!string.IsNullOrWhiteSpace(queryCondition))
                {
                    srReport.Dictionary.Variables.Add("QueryCondition", queryCondition.Substring(0, queryCondition.Length-1));//查询条件
                }
                //从会话信息中取指定报表的缓存键值，每个会话信息的缓存键值都不一样，以避免多人看同一报表时互相影响
                var reportGuid = Session[model.ReportCode] as string;
                if (string.IsNullOrWhiteSpace(reportGuid))
                {
                    reportGuid = Guid.NewGuid().ToString("N");
                    Session[model.ReportCode] = reportGuid;
                }
                srReport.ReportGuid = reportGuid;
                return srReport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 保存报表对象。将报表对象保存为自定义格式到数据库中。
        /// </summary>
        /// <param name="report">报表对象</param>
        protected bool SaveReport(string rptCode, StiReport report,string styleName)
        {
            var rpt = new SrReport(rptCode, GetReportFilePath(rptCode,styleName), GetService<IReportService>(), CurrentInfo,null,styleName);
            return rpt.SaveReport(report);
        }

        /// <summary>
        /// 删除报表对象。删除数据库中的自定义格式。
        /// </summary>
        protected bool DelReport(string rptCode,string styleName)
        {
            var rpt = new SrReport(rptCode, GetReportFilePath(rptCode,styleName), GetService<IReportService>(), CurrentInfo,null,styleName);
            return rpt.DelReport();
        }

        /// <summary>
        /// 获取默认格式报表路径。 获取默认格式报表物理文件路径。
        /// </summary>
        /// <param name="rptCode">报表代码</param>
        /// <param name="styleName">格式名称</param>
        protected string GetReportFilePath(string rptCode,string styleName)
        {
            string fileName = GetService<IReportService>().GetFileName(rptCode,(byte)CurrentInfo.ProductType);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = rptCode;
            }
            if(styleName == SrReport.DefaultBillPosFormatStyleName)
            {
                fileName = GetPosRptCode(rptCode);
            }
            if (styleName == SrReport.sumbill)
            {
                fileName = "up_print_bill（summary）";
            }
            if (styleName == SrReport.sumbillpos)
            {
                fileName = "up_print_bill（summary）(pos)";
            }
            if (styleName == SrReport.resRelationRC)
            {
                fileName = "resRelationRC";
            }
            if (styleName==SrReport.billDetail)
            {
                fileName = "posBillDetailForB";
            }
            if (styleName == SrReport.billDept)
            {
                fileName = "PosBillDept";
            }
            if (styleName == SrReport.billDsicountUsers)//折扣报表（折扣人分组）
            {
                fileName = "PosBillDiscountByUser";
            }
            if (styleName == SrReport.billDsicountItem)//折扣报表（食品明细）
            {
                fileName = "PosBillDiscountItem";
            }
            if (styleName == SrReport.WareHouseItemSummary)//仓库物品汇总
            {
                fileName = "PosWareHouseItemSummary";
            }
            return Server.MapPath(string.Format("~/SRReports/{0}.mrt", fileName));
        }
        /// <summary>
        /// 获取单据打印默认pos格式文件名称
        /// 规则就是在单据打印名称的后面加上(pos)
        /// </summary>
        /// <param name="rptCode">单据打印格式名称</param>
        /// <returns>对应的pos格式名称</returns>
        protected string GetPosRptCode(string rptCode)
        {
            return string.Format("{0}(pos)", rptCode);
        } 
        /// <summary>
        /// 是否存在默认pos格式文件
        /// </summary>
        /// <param name="rptCode">单据打印名称</param>
        /// <returns>true:存在，false:不存在</returns>
        protected bool HasPosFile(string rptCode)
        {
            var posRptCode = GetPosRptCode(rptCode);
            var posFilePath = Server.MapPath(string.Format("~/SRReports/{0}.mrt", posRptCode));
            return System.IO.File.Exists(posFilePath);
        }

        #region
        /// <summary>
        /// 添加查询参数临时数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns>返回url地址的参数部分,例如?ReportCode={0}&ParameterValues={1}&ChineseName={2}</returns>
        [HttpPost]
        [AuthButton(AuthFlag.Query)]
        public ActionResult AddQueryParaTemp(ReportQueryModel model,string print)
        {
            try
            {
                if (model != null && !string.IsNullOrWhiteSpace(model.ReportCode))
                {
                    string value = new JavaScriptSerializer().Serialize(model);
                    Guid? id = GetService<IReportService>().AddQueryParaTemp(CurrentInfo.HotelId, value);
                    if (id != null)
                    {
                        var url = new StringBuilder();
                        url.Append("http://").Append(Request.Url.Host).Append("/ReportManage");
                        if (Request.Url.ToString().Contains("SRBillReportView"))
                        {
                            url.Append("/SRBillReportView");
                        }
                        else
                        {
                            url.Append("/SRReportView");
                        }
                        url.Append("/Index")
                            .Append("?ReportCode=").Append(Server.UrlEncode(model.ReportCode))
                            .Append("&ParameterValues=").Append(Server.UrlEncode(id.Value.ToString()))
                            .Append("&ChineseName=").Append(Server.UrlEncode(model.ChineseName));
                        if (!string.IsNullOrWhiteSpace(print))
                        {
                            url.Append("&print=").Append(Server.UrlEncode(print));
                        }

                        return Json(JsonResultData.Successed(url.ToString()));
                    }
                    else
                    {
                        return Json(JsonResultData.Failure("添加失败！"));
                    }
                }
                else
                {
                    return Json(JsonResultData.Failure("参数错误！"));
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        /// <summary>
        /// 添加查询参数临时数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns>返回url地址的参数部分,例如?ReportCode={0}&ParameterValues={1}&ChineseName={2}</returns>
        [HttpPost]
        [AuthButton(AuthFlag.Query)]
        public ActionResult AddWaitPayQueryParaTemp(string id, string print)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(JsonResultData.Failure("请指定待支付id"));
                }
                var folioPayService = GetService<IResFolioPayInfoService>();
                var folioPay = folioPayService.GetHotelPayInfo(Convert.ToInt32(id),CurrentInfo.HotelId);
                if(folioPay == null)
                {
                    return Json(JsonResultData.Failure("指定的待支付id不存在"));
                }
                ReportQueryModel model = null;
                if(folioPay.ProductType == PayProductType.MbrRecharge.ToString())
                {
                    var waitPayService = GetService<IWaitPayListService>();
                    var waitPay = waitPayService.Get(Guid.Parse(folioPay.ProductTransId));
                    if(waitPay == null)
                    {
                        return Json(JsonResultData.Failure("没有待支付的会员充值记录"));
                    }
                    model = new ReportQueryModel
                    {
                        ChineseName = "充值单据",
                        ProcedureName = "up_print_profileRecharge",
                        ReportCode = "up_print_profileRecharge"                        
                    };
                    var serializer = new JavaScriptSerializer();
                    var waitPayParas = serializer.DeserializeObject(waitPay.BusinessPara) as Dictionary<string, object>;
                    var paraValues = new StringBuilder();
                    paraValues.Append("@t00profileid=").Append(waitPayParas["profileId"])
                        .Append("&@t00ptype=c")
                        .Append("&@t00waitPayId=").Append(waitPay.WaitPayId.ToString())
                        .Append("&@t00largessAmount=").Append(waitPayParas["freeAmount"])
                        .Append("&@inputUser=").Append(CurrentInfo.UserName);
                    model.ParameterValues = paraValues.ToString();
                }
                return AddQueryParaTemp(model, print);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 获取并移除查询参数临时数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ReportQueryModel GetQueryParaTemp(Guid id)
        {
            try
            {
                string value = GetService<IReportService>().GetQueryParaTemp(CurrentInfo.HotelId, id);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    ReportQueryModel model = new JavaScriptSerializer().Deserialize<ReportQueryModel>(value);
                    if (model != null && !string.IsNullOrWhiteSpace(model.ReportCode))
                    {
                        return model;
                    }
                }
            }
            catch{}
            return null;
        }

        /// <summary>
        /// 检查参数
        /// </summary>
        /// <param name="model"></param>
        /// <remarks>检查model中ParameterValues是否GUID，如果是，则从表中获取相对应的结果并修改model，返回修改后的model。否则，返回未修改的model。未从表中找到GUID，则返回NULL。</remarks>
        /// <returns></returns>
        public ReportQueryModel CheckParameterValues(ReportQueryModel model)
        {
            //判断GUID并获取查询参数
            if (!string.IsNullOrWhiteSpace(model.ParameterValues) && model.ParameterValues.Length == 36)
            {
                Guid id = new Guid();
                if (Guid.TryParse(model.ParameterValues, out id))
                {
                    var entity = GetQueryParaTemp(id);
                    if (entity != null && entity.ReportCode == model.ReportCode && entity.ChineseName == model.ChineseName)
                    {
                        model = entity;
                    }
                    else
                    {
                        model = null;//此地址已失效，请从系统中打开报表
                    }
                }
            }
            return model;
        }
        #endregion
    }
}