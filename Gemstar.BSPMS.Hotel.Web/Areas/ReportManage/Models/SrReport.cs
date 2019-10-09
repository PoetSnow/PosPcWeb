using System;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Gemstar.BSPMS.Hotel.Services.ReportManage;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.ResManage;
using System.Web;
using System.Text;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models
{
    /// <summary>
    /// 报表实现类
    /// </summary>
    public class SrReport
    {
        #region private field
        /// <summary>
        /// 报表名称
        /// </summary>
        private readonly string _name;
        /// <summary>
        /// 报表对应的物理文件路径及名称
        /// </summary>
        private readonly string _file;
        /// <summary>
        /// 报表服务
        /// </summary>
        private readonly IReportService _reportService;
        /// <summary>
        /// 当前登录信息
        /// </summary>
        private readonly ICurrentInfo _currentInfo;
        /// <summary>
        /// 报表中文名称
        /// </summary>
        private readonly string _reportChineseName;
        /// <summary>
        /// 格式名称
        /// </summary>
        private readonly string _styleName;
        #endregion

        /// <summary>
        /// 创建stimulsoft report报表对象实例
        /// </summary>
        /// <param name="name">报表名称</param>
        /// <param name="file">报表对应的物理文件路径及名称</param>
        /// <param name="reportService">报表服务</param>
        /// <param name="currentInfo">当前登录信息</param>
        public SrReport(string name, string file, IReportService reportService, ICurrentInfo currentInfo, string reportChineseName = null, string styleName = null)
        {
            _name = name;
            _file = file;
            _reportService = reportService;
            _currentInfo = currentInfo;
            _reportChineseName = reportChineseName;
            _styleName = styleName;
            if (string.IsNullOrWhiteSpace(_name))
            {
                throw new Exception("报表名称为空");
            }
            if (string.IsNullOrWhiteSpace(_file))
            {
                throw new Exception("报表对应的物理文件路径及名称为空");
            }
            if (_reportService == null)
            {
                throw new Exception("报表服务为空");
            }
            if (_currentInfo == null)
            {
                throw new Exception("当前登录信息为空");
            }
        }


        /// <summary>
        /// 获取报表对象。如果有自定义则返回自定义对象，没有则返回标准对象。
        /// </summary>
        /// <param name="procedureParametersStr">存储过程参数字符串</param>
        /// <returns></returns>
        public StiReport GetReport(string procedureParametersStr, bool isSearchAll)
        {
            StiReport report = new StiReport();

            //2.报表格式
            string template = _reportService.GetTemplate(_currentInfo.HotelId, _name, _styleName);
            if (string.IsNullOrWhiteSpace(template))//1.检查数据库中是否有用户自定义语法
            {
                report.Load(_file);//1.2加载默认语法，从路径获得
            }
            else
            {
                report.LoadFromString(template);//1.1加载自定义语法，从数据库获得
            }
            //1.报表中文名称
            report.ReportName = _reportService.GetReportName(_name, (byte)_currentInfo.ProductType);
            if (string.IsNullOrWhiteSpace(report.ReportName))
            {
                if (string.IsNullOrWhiteSpace(_reportChineseName))
                {
                    throw new Exception("指定的报表不存在");
                }
                else
                {
                    report.ReportName = _reportChineseName;
                }
            }
            //3.重置数据库连接
            string databaseName = report.Dictionary.Databases.Count <= 0 ? "DatabasePms" : report.Dictionary.Databases[0].Name;
            string connectionString = MvcApplication.GetHotelDbConnStr();
            report.Dictionary.Databases.Clear();
            report.Dictionary.Databases.Add(new StiSqlDatabase(databaseName, connectionString));
            //4.根据查询条件中的值重新设置要执行的sql命令
            string weiXinQRCodeUrl = null;
            if (report.DataSources.Count > 0)
            {
                StiSqlSource sqlDataSource = report.DataSources[0] as StiSqlSource;
                sqlDataSource.NameInSource = databaseName;
                if ((!string.IsNullOrWhiteSpace(procedureParametersStr) && procedureParametersStr.Contains(",@")) //至少两个参数（hid参数和自定义参数）
                    ||
                    (!string.IsNullOrWhiteSpace(procedureParametersStr) && isSearchAll))//查询全部
                {
                    string procedureName = GetProcedureName();
                    sqlDataSource.SqlCommand = string.Format("exec {0} {1}", procedureName, procedureParametersStr);
                    try { weiXinQRCodeUrl = GetWeiXinQRCode(procedureName, procedureParametersStr); }
                    catch (Exception ex)
                    {
                        var logService = DependencyResolver.Current.GetService<ISysLogService>();
                        logService.AddSysLog(ex.Message, "SrReport", "/SrReport/GetReport", _currentInfo.UserName, _currentInfo.HotelId);
                    }
                }
                else
                {
                    sqlDataSource.SqlCommand = "return";
                }
            }
            //获取当前酒店logo地址
            Controllers.SRBillReportViewController s = new Controllers.SRBillReportViewController();
            var logoUrl = s.GetLogoUrl(_currentInfo.HotelId);
            var hotelInfo = s.GetHotelInfo(_currentInfo.HotelId);
            string shortname = s.GetHotelShortName(_currentInfo.HotelId);
            report.Dictionary.Variables.Add("HotelId", _currentInfo.HotelId);//酒店代码 
            report.Dictionary.Variables.Add("HotelName", shortname);//酒店名称
            report.Dictionary.Variables.Add("UserCode", _currentInfo.UserCode);//操作员代码
            report.Dictionary.Variables.Add("UserName", _currentInfo.UserName);//操作员
            report.Dictionary.Variables.Add("LogoUrl", logoUrl);//酒店logo
            report.Dictionary.Variables.Add("WeiXinQRCodeUrl", weiXinQRCodeUrl ?? "");//维修二维码开锁
            report.Dictionary.Variables.Add("ReportCode",  _name);//报表编号
            report.Dictionary.Variables.Add("HotelPhone", hotelInfo.Mobile);//酒店电话 
            report.Dictionary.Variables.Add("HotelAddress", hotelInfo.address ?? "");//酒店地址
            return report;
        }


        /// <summary>
        /// 保存报表对象。将报表对象保存为自定义格式到数据库中。
        /// </summary>
        /// <param name="report">报表对象</param>
        public bool SaveReport(StiReport report)
        {
            return _reportService.SaveTemplate(_currentInfo.HotelId, _name, report.SaveToString(), _styleName);
        }
        public bool SaveReport(StiReport report, string styleName)
        {
            return _reportService.SaveTemplate(_currentInfo.HotelId, _name, report.SaveToString(), styleName);
        }

        /// <summary>
        /// 删除报表对象。删除数据库中的自定义格式。
        /// </summary>
        public bool DelReport()
        {
            if (_reportService.IsExistsTemplate(_currentInfo.HotelId, _name, _styleName))
            {
                return _reportService.DelTemplate(_currentInfo.HotelId, _name, _styleName);
            }
            else
            {
                throw new Exception("自定义格式不存在！");
            }
        }


        #region 其他
        /// <summary>
        /// 获取报表的存储过程名称
        /// </summary>
        /// <returns>报表的存储过程名称</returns>
        public string GetProcedureName()
        {
            var report = new StiReport();
            report.Load(_file);
            string procedureName = report.ReportDescription;
            if (string.IsNullOrWhiteSpace(procedureName))
            {
                throw new Exception("报表的存储过程名称为空！");
            }
            return procedureName;
        }

        /// <summary>
        /// 获取微信二维码
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="procedureParameters">存储过程参数</param>
        /// <returns></returns>
        public string GetWeiXinQRCode(string procedureName, string procedureParameters)
        {
            if (!string.IsNullOrWhiteSpace(procedureName) && !string.IsNullOrWhiteSpace(procedureParameters))
            {
                //获取参数
                string hid = _currentInfo.HotelId;
                procedureName = procedureName.ToLower();
                procedureParameters = procedureParameters.ToLower();
                string[] procedureParameterList = procedureParameters.Split(',');
                if (procedureParameterList == null || procedureParameterList.Length <= 0) { return null; }
                if (procedureName != "up_print_deposit" && procedureName != "up_print_resrc") { return null; }
                //验证是否开启在线门锁
                string onlineLockType = null;
                string onlineLockUrl = null;
                var masterServiceObject = System.Web.Mvc.DependencyResolver.Current.GetService(typeof(Common.Services.IMasterService));
                var masterService = (masterServiceObject as Common.Services.IMasterService);
                if (masterService != null)
                {
                    onlineLockType = masterService.GetHotelOnlineLockType(hid);
                    onlineLockUrl = masterService.GetSysParaValue("onlineLockUrl");
                }
                if (onlineLockType != "jinweiLock")
                {
                    return null;
                }
                if (string.IsNullOrWhiteSpace(onlineLockUrl))
                {
                    return null;
                }
                var resServiceObject = System.Web.Mvc.DependencyResolver.Current.GetService(typeof(IResService));
                var resService = (resServiceObject as IResService);
                if (resService == null)
                {
                    return null;
                }
                //获取生成二维码的参数
                var onlineLockQueryInfo = new OnlineLockQueryResult { QuerySuccessed = false };
                if (procedureName == "up_print_deposit")//押金单
                {
                    foreach (var item in procedureParameterList)
                    {
                        if (!string.IsNullOrWhiteSpace(item) && item.Contains("@transid"))
                        {
                            string transid = item.Replace("@transid", "").Replace("=", "").Replace("'", "").Trim();
                            if (!string.IsNullOrWhiteSpace(transid))
                            {
                                var queryPara = new OnlineLockQueryPara { Hid = hid, Type = OnlineLockQueryParaType.QueryByTransId, QueryParaValue = transid };
                                onlineLockQueryInfo = resService.GetOnlineLockPara(queryPara);
                            }
                        }
                    }
                }
                else if (procedureName == "up_print_resrc")//RC单
                {
                    foreach (var item in procedureParameterList)
                    {
                        if (!string.IsNullOrWhiteSpace(item) && item.Contains("@regid"))
                        {
                            string regid = item.Replace("@regid", "").Replace("=", "").Replace("'", "").Trim();
                            if (!string.IsNullOrWhiteSpace(regid))
                            {

                                var queryPara = new OnlineLockQueryPara { Hid = hid, Type = OnlineLockQueryParaType.QueryByRegId, QueryParaValue = regid };
                                onlineLockQueryInfo = resService.GetOnlineLockPara(queryPara);
                            }
                        }
                    }
                }
                //返回结果
                if (onlineLockQueryInfo != null && onlineLockQueryInfo.QuerySuccessed)
                {
                    var guestName = onlineLockQueryInfo.Name;
                    try
                    {
                        guestName = HttpContext.Current.Server.UrlEncode(guestName);
                    }
                    catch { }
                    var url = new StringBuilder();
                    url.Append(onlineLockUrl).Append("?hotelCode=").Append(hid)
                        .Append("&regId=").Append(onlineLockQueryInfo.RegId.Substring(hid.Length))
                        .Append("&roomCode=").Append(onlineLockQueryInfo.RoomCode)
                        .Append("&mobile=").Append(onlineLockQueryInfo.Mobile)
                        .Append("&roomNo=").Append(onlineLockQueryInfo.RoomNo)
                        .Append("&idNo=").Append(onlineLockQueryInfo.CerNo)
                        .Append("&name=").Append(guestName)
                        ;

                    try
                    {
                        Common.Tools.HttpHelper.NewWebClient webClient = new Common.Tools.HttpHelper.NewWebClient(2 * 1000);
                        webClient.Encoding = Encoding.UTF8;
                        return webClient.DownloadString(url.ToString());
                    }
                    catch (Exception ex)
                    {
                        var logService = DependencyResolver.Current.GetService<ISysLogService>();
                        logService.AddSysLog(string.Format("请求二维码URL为{0},ex={1}", url.ToString(), ex.Message), "SrReport", "/SrReport/GetWeiXinQRCode", _currentInfo.UserName, _currentInfo.HotelId);
                    }
                }
            }
            return null;
        }
        #endregion
        /// <summary>
        /// 汇总账单
        /// </summary>
        public const string sumbill = "汇总账单";
        /// <summary>
        /// 汇总账单pos格式
        /// </summary>
        public const string sumbillpos = "汇总账单(pos格式)";
        /// <summary>
        /// 默认的单据格式名称，用于对应默认文件格式
        /// </summary>
        public const string DefaultBillFormatStyleName = "默认格式";
        /// <summary>
        /// 默认的单据pos格式名称，用于对应默认pos文件格式
        /// </summary>
        public const string DefaultBillPosFormatStyleName = "默认Pos格式";
        /// <summary>
        /// 默认的增加多格式前的用户自定义格式，用于对应用户添加格式之前的自定义格式
        /// </summary>
        public const string DefaultBillUserFormatStyleName = "自定义格式";
        /// <summary>
        /// 关联房RC单
        /// </summary>
        public const string resRelationRC = "关联房RC单";

        /// <summary>
        /// 菜式单位汇总（云pos）
        /// </summary>
        public const string SumPosDetaile = "菜式单位汇总";

        /// <summary>
        /// 账单明细（云pos）
        /// </summary>
        public const string billDetail = "账单明细";


        public const string billRefeDept = "营业点分组";
        /// <summary>
        /// 部门销售报表（部门分组）
        /// </summary>
        public const string billDept = "部门分组";

        /// <summary>
        /// 折扣报表（折扣人分组）
        /// </summary>
        public const string billDsicountUsers = "折扣人明细";
        /// <summary>
        /// 折扣报表（菜式）
        /// </summary>
        public const string billDsicountItem = "食品明细";

        /// <summary>
        /// 仓库物品汇总
        /// </summary>
        public const string WareHouseItemSummary = "仓库物品汇总";
    }
}