using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.OnlineInterfaceManage;
using Gemstar.BSPMS.Hotel.Services.OnlineInterfaceManage.Invoice;

namespace Gemstar.BSPMS.Hotel.Services.EF.OnlineInterfaceManage
{
    public abstract class InvoiceBaseService<T> : IInvoiceService where T : OnlineInterfaceModel
    {
        /// <summary>
        /// 酒店信息服务接口
        /// </summary>
        protected IHotelInfoService _hotelInfoService;
        /// <summary>
        /// 酒店业务库
        /// </summary>
        protected DbHotelPmsContext _pmsContext;
        /// <summary>
        /// 当前登录信息
        /// </summary>
        protected ICurrentInfo _currentInfo;
        /// <summary>
        /// 接口代码 接口的标识唯一
        /// </summary>
        protected string _code;
        /// <summary>
        /// 接口代码类型
        /// </summary>
        protected string _typeCode = "Invoice";
        /// <summary>
        /// 酒店ID
        /// </summary>
        protected string _hid;

        protected InvoiceModel _model;
        /// <summary>
        /// 构造函数
        /// </summary>
        protected InvoiceBaseService(IHotelInfoService hotelInfoService,DbHotelPmsContext pmsContext, ICurrentInfo currentInfo, string code, InvoiceModel model)
        {
            _hotelInfoService = hotelInfoService;
            _pmsContext = pmsContext;
            _currentInfo = currentInfo;
            _code = code;
            _hid = _currentInfo.HotelId;
            _model = model;
            if (string.IsNullOrWhiteSpace(_hid))
            {
                throw new Exception("当前登录信息无效");
            }
            if (string.IsNullOrWhiteSpace(_code))
            {
                throw new Exception("接口代码，接口的标识唯一不能为空");
            }
        }


        #region 抽象方法
        /// <summary>
        /// 增加消费记录
        /// </summary>
        public abstract string ConsumInfo(string billid);

        /// <summary>
        /// 撤销消费记录
        /// </summary>
        public abstract void ConsumInfoRepeal(string billid);

        /// <summary>
        /// 获取商品代码列表
        /// </summary>
        public abstract InvoiceParameter.BaseResponse<List<InvoiceParameter.FindAllSP.Response>> FindAllSP(InvoiceParameter.FindAllSP.Request value);

        /// <summary>
        /// 获取商品代码
        /// </summary>
        public abstract InvoiceParameter.BaseResponse<InvoiceParameter.FindSP.Response> FindSP(InvoiceParameter.FindSP.Request value);
        #endregion

        #region HTTP
        /// <summary>
        /// get或post发送数据，接收返回值
        /// </summary>
        /// <param name="type">提交类型:get,post</param>
        /// <param name="url">请求网址</param>
        /// <param name="data">请求数据</param>
        /// <returns></returns>
        public static string Request(string type, string url, byte[] data)
        {
            var request = WebRequest.CreateHttp(url);
            request.Method = type;
            request.ContentType = "application/x-www-form-urlencoded";
            if (type.ToLower() != "get")
            {
                request.ContentLength = data.Length;
                using (var input = request.GetRequestStream())
                {
                    input.Write(data, 0, data.Length);
                }
            }
            using (var response = request.GetResponse())
            {
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
        #endregion

        #region 日志记录
        protected void AddLog(List<OnlineInterfaceLog> logs)
        {
            if (logs == null || logs.Count <= 0)
            {
                return;
            }
            string connectionString = GetPmsConnStr();
            //表结构
            System.Data.DataTable table = BSPMS.Common.Tools.ADOHelper.ExecSql("select top 0 * from OnlineInterfaceLog", connectionString, null, true);
            foreach (var item in logs)
            {
                var row = table.NewRow();
                row["CDate"] = item.CDate;
                row["Code"] = item.Code ?? "";
                row["Hid"] = item.Hid ?? "";
                row["ReceiveContent"] = item.ReceiveContent ?? "";
                row["ReceiveDate"] = item.ReceiveDate != null && item.ReceiveDate.HasValue ? item.ReceiveDate.Value : DateTime.Now;
                row["Regid"] = item.Regid ?? "";
                row["SendContent"] = item.SendContent ?? "";
                row["SendDate"] = item.SendDate != null && item.SendDate.HasValue ? item.SendDate.Value : DateTime.Now;
                row["TypeCode"] = item.TypeCode ?? "";
                row["Url"] = item.Url ?? "";
                table.Rows.Add(row);
            }
            BSPMS.Common.Tools.ADOHelper.BulkInsert(connectionString, "OnlineInterfaceLog", table);
            table.Dispose();
        }

        private string GetPmsConnStr()
        {
            var hotelInfos = _hotelInfoService.GetHotelInfo(_hid);
            var isConnectViaInternet = _hotelInfoService.IsConnectViaInternte();
            string pmsConnStr = Gemstar.BSPMS.Common.Tools.ConnStrHelper.GetConnStr(hotelInfos.DbServer, hotelInfos.DbName, hotelInfos.Logid, hotelInfos.LogPwd, "GemstarBSPMS", hotelInfos.DbServerInternet, isConnectViaInternet);
            if (string.IsNullOrWhiteSpace(pmsConnStr))
            {
                throw new Exception("获取酒店业务数据库连接失败！");
            }
            return pmsConnStr;
        }
        #endregion
    }
}
