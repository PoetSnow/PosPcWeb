using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Gemstar.BSPMS.Hotel.Services.NotifyManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.NotifyManage
{
    /// <summary>
    /// 通知服务
    /// 用于通知通知处理程序，程序中的某些数据变化，需要上传到ota上
    /// </summary>
    public class NotifyService: INotifyService
    {
        public NotifyService(DbHotelPmsContext pmsContext)
        {
            _pmsContext = pmsContext;
        }
        /// <summary>
        /// 通知ota信息
        /// 在适用于ota的房型，价格码等信息变更时通知
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="isEnvTest">是否测试环境</param>
        /// <param name="channelId">渠道id</param>
        public void NotifyOtaInfo(string hid, bool isEnvTest, string channelId)
        {
            var notifyId = _pmsContext.Database.SqlQuery<decimal>("exec up_setNotify @hid=@hid,@isEnvTest=@isEnvTest,@typeCode='otaInfo',@para1=@para1"
                , new SqlParameter("@hid",hid??"")
                , new SqlParameter("@isEnvTest",isEnvTest ? 1 : 0)
                , new SqlParameter("@para1",channelId??"")
                ).Single();
            Notify(hid, notifyId,isEnvTest);
        }
        /// <summary>
        /// 通知ota价格
        /// 在适用于ota的价格码明细价格变更时通知
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="isEnvTest">是否测试环境</param>
        /// <param name="channelId">渠道id</param>
        /// <param name="rateId">价格id</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        public void NotifyOtaRatePrice(string hid, bool isEnvTest, string channelId, string rateId, string beginDate, string endDate)
        {
            var notifyId = _pmsContext.Database.SqlQuery<decimal>("exec up_setNotify @hid=@hid,@isEnvTest=@isEnvTest,@typeCode='otaRatePrice',@para1=@para1,@para2=@para2,@para3=@para3,@para4=@para4"
                , new SqlParameter("@hid", hid ?? "")
                , new SqlParameter("@isEnvTest",isEnvTest ? 1 : 0)
                , new SqlParameter("@para1", channelId ?? "")
                , new SqlParameter("@para2",rateId??"")
                , new SqlParameter("@para3",beginDate??"")
                , new SqlParameter("@para4",endDate??"")
                ).Single();
            Notify(hid, notifyId,isEnvTest);
        }
        /// <summary>
        /// 通知ota房间配额
        /// 在适用于ota的房型配额变更时通知
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="isEnvTest">是否测试环境</param>
        /// <param name="channelId">渠道id</param>
        /// <param name="roomTypeId">房型id</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        public void NotifyOtaRoomQty(string hid, bool isEnvTest, string channelId, string roomTypeId, string beginDate, string endDate)
        {
            var notifyId = _pmsContext.Database.SqlQuery<decimal>("exec up_setNotify @hid=@hid,@isEnvTest=@isEnvTest,@typeCode='otaRoomQty',@para1=@para1,@para2=@para2,@para3=@para3,@para4=@para4"
                , new SqlParameter("@hid", hid ?? "")
                , new SqlParameter("@isEnvTest",isEnvTest ? 1 : 0)
                , new SqlParameter("@para1", channelId ?? "")
                , new SqlParameter("@para2", roomTypeId ?? "")
                , new SqlParameter("@para3", beginDate ?? "")
                , new SqlParameter("@para4", endDate ?? "")
                ).Single();
            Notify(hid, notifyId,isEnvTest);
        }
        /// <summary>
        /// 通知ota订单状态变更
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="isEnvTest">是否测试环境</param>
        /// <param name="channelId">渠道id</param>
        /// <param name="resId">订单id</param>
        public void NotifyOtaResChanged(string hid, bool isEnvTest, string channelId, string resId)
        {
            var notifyId = _pmsContext.Database.SqlQuery<decimal>("exec up_setNotify @hid=@hid,@isEnvTest=@isEnvTest,@typeCode='otaResChanged',@para1=@para1,@para2=@para2"
                , new SqlParameter("@hid", hid ?? "")
                , new SqlParameter("@isEnvTest", isEnvTest ? 1 : 0)
                , new SqlParameter("@para1", channelId ?? "")
                , new SqlParameter("@para2", resId ?? "")
                ).Single();
            Notify(hid, notifyId,isEnvTest);
        }
        private void Notify(string hid,decimal notifyId, bool isEnvTest)
        {
            //从数据库中取出对应环境的通知地址
            var notifyUrl = "";
            if (isEnvTest)
            {
                notifyUrl = _pmsContext.Database.SqlQuery<string>("SELECT name2 FROM dbo.v_codeListPub WHERE typeCode = '21' AND code='NotifyAddressTest'").SingleOrDefault();
            }
            else
            {
                notifyUrl = _pmsContext.Database.SqlQuery<string>("SELECT name2 FROM dbo.v_codeListPub WHERE typeCode = '21' AND code='NotifyAddress'").SingleOrDefault();
            }
            var async = new NotifyAsync(_pmsContext, isEnvTest, hid, Convert.ToInt32(notifyId),notifyUrl);
            ThreadPool.QueueUserWorkItem(o => { ((NotifyAsync)o).DoNotifyAsync(); },async);
        }
        private DbHotelPmsContext _pmsContext;
    }
    public class NotifyAsync
    {
        private bool _isEnvTest;
        private DbHotelPmsContext _pmsContext;
        private string _hid;
        private decimal _notifyId;
        private string _notifyUrl;
        public NotifyAsync(DbHotelPmsContext pmsContext,bool isEnvTest,string hid,int notifyId,string notifyUrl)
        {
            _pmsContext = pmsContext;
            _isEnvTest = isEnvTest;
            _hid = hid;
            _notifyId = notifyId;
            _notifyUrl = notifyUrl;
        }
        public void DoNotifyAsync()
        {
            if (!string.IsNullOrWhiteSpace(_notifyUrl))
            {
                try
                {
                    var xmlStrBuilder = new StringBuilder();
                    xmlStrBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>")
                        .AppendLine("<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">")
                        .AppendLine("<soap12:Body>")
                        .AppendLine("<Notify xmlns=\"http://pmsnotify.gshis.com/\">")
                        .AppendLine(string.Format("<hid>{0}</hid>", _hid))
                        .AppendLine(string.Format("<notifyId>{0}</notifyId>", _notifyId))
                        .AppendLine(string.Format("<isEnvTest>{0}</isEnvTest>", _isEnvTest ? "true" : "false"))
                        .AppendLine("</Notify>")
                        .AppendLine("</soap12:Body>")
                        .AppendLine("</soap12:Envelope>");

                    var request = WebRequest.Create(_notifyUrl);
                    if (request != null)
                    {
                        //request.Credentials = CredentialCache.DefaultCredentials;
                        var sendContent = Encoding.UTF8.GetBytes(xmlStrBuilder.ToString());
                        request.ContentType = "application/soap+xml; charset=utf-8";
                        request.Headers.Add("charset", "utf-8");
                        request.ContentLength = sendContent.Length;
                        request.Method = "POST";
                        using (var requestStream = request.GetRequestStream())
                        {
                            requestStream.Write(sendContent, 0, sendContent.Length);
                            requestStream.Close();
                        }
                        using (var response = request.GetResponse())
                        {
                            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                            {
                                var s = reader.ReadToEnd();
                                reader.Close();
                            }

                        }
                    }
                }
                catch
                {
                    //通知失败处理，暂时不做任何处理
                }
            }
        }
    }
}
