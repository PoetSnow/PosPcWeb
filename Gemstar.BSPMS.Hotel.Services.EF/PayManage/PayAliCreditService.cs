using System;
using System.Data.SqlClient;
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Top.Api.Request;
using Top.Api;
using Gemstar.BSPMS.Common.Services.EF;
using System.Data.Entity;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Top.Api.Response;
using System.Collections.Generic;
using FastJSON;
using System.Transactions;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 阿里信用住付款
    /// </summary>
    public class PayAliCreditService : PayBaseService
    {
        public PayAliCreditService(DbCommonContext commDb,DbHotelPmsContext pmsDb,string hid,string userName,string shiftId,bool isTest)
        {
            _commDb = commDb;
            _pmsDb = pmsDb;
            _username = userName;
            _shiftId = shiftId;
            _hid = hid;
            _isTest = isTest;

        }
        public override PayResult DoPayBeforeSaveFolio(string jsonStrPara)
        {
            var paraDic = GetParaDicFromJsonStr(jsonStrPara);
            var amount =Convert.ToDecimal(paraDic["amount"]).ToString("#0.00");
            var regid = paraDic["regid"].ToString();
            var itemid = paraDic["itemid"].ToString();
            var ischeck = paraDic["ischeck"].ToString();
            if (ischeck != "1")
            {
                throw new ApplicationException("信用住支付只能用于结账");
            }
            var resfolio = _pmsDb.ResFolios.Where(d => d.Regid == regid && d.Dcflag == "D" && d.Status == 1);
            if(resfolio == null)
            {
                throw new ApplicationException("没有消费记录");
            }
              var sumAmount =   resfolio.Select(d => d.Amount).Sum().ToString();
            if(!amount.Equals(sumAmount))
            {
                throw new ApplicationException("信用住支付需一次性全部结账完，消费总金额为"+ sumAmount);
            }
            var resId = resfolio.FirstOrDefault().Resid;
            var room = resfolio.FirstOrDefault().RoomNo;
            var detail = _pmsDb.ResDetails.Where(w => w.Resid == resId && w.Hid == _hid).AsNoTracking().FirstOrDefault();
            var extType = (detail != null) ? (detail.ExtType ?? 0) : 0;
            if(extType != 2)
            {
                throw new ApplicationException("阿里信用住的账单才能使用信用住支付");
            }
            var topClient = TopClientInstance(_isTest);
            string sessionKey = SessionKey(_isTest),url = Url(_isTest),appkey= AppKey(_isTest), secret=AppSecret(_isTest);
            var client = new DefaultTopClient(url, appkey, secret);
            var req = new XhotelOrderAlipayfaceUpdateRequest();//线上更改订单状态接口
            //var req = new XhotelOrderAlipayfaceUpdateMockRequest();//沙箱更改订单状态接口
            req.OutId = resId;
            req.OptType = 4;
            if (!string.IsNullOrWhiteSpace(room))
            {
                req.OutRoomNumber = room;
            }
            req.CheckinDate = detail.ArrDate.ToDateString();
            req.CheckoutDate = DateTime.Now.ToDateString();
            var rsp = client.Execute(req, sessionKey);
            //开启新事务用于保存日志
            using (TransactionScope ts1 = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                var sendXml = AddSendXml(_hid, 0, "信用住订单状态变更上传", url, string.Format("上传酒店{0},订单：{1},订单状态{2},房间号:{3}，入住时间{4}，离店时间{5}", _hid, req.OutId, 4, req.OutRoomNumber, req.CheckinDate, req.CheckoutDate));
                UpdateReceiveXml(sendXml, rsp.Body);
                _pmsDb.SaveChanges();
                ts1.Complete();
            }
            if ("success".Equals(rsp.Result))
            {
                var resCredit = new XhotelOrderAlipayfaceSettleRequest();//线上信用住结账接口
                //var resCredit = new XhotelOrderAlipayfaceSettleMockRequest();//沙箱信用住结账接口
                resCredit.CheckOut = DateTime.Now;//离店时间
                resCredit.OutId = resId;//外部订单号
                resCredit.RoomNo = room;//房间号
                //总费用
                var consume = _pmsDb.Database.SqlQuery<Consume>("select (case when b.itemTypeName='房租' then b.itemTypeName else b.name end) as name,cast(a.amount*100 as int) as amount from resFolio a left join item b on a.itemid=b.id where a.regid=@regid and a.dcflag='D' and a.status = 1", new SqlParameter("@regid",regid),new SqlParameter()).ToList();
                resCredit.TotalRoomFee = consume.Where(d => d.Name == "房费").Sum(c => c.Amount);//房费
                var entity = consume.Where(d => d.Name != "房费").ToList();//杂费
                if (entity != null && entity.Count > 0)
                {
                    resCredit.OtherFee = entity.Sum(d => d.Amount);
                    JSON.Parameters.UseExtensions = false;
                    resCredit.OtherFeeDetail = JSON.ToJSON(entity);
                }
                else
                {
                    resCredit.OtherFee = 0;
                }
                var rspCredit = client.Execute(resCredit, sessionKey);
                //开启新事务用于保存日志
                using (TransactionScope ts2 = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    var sendCredit = AddSendXml(_hid, 0, "阿里信用住订单结账", url, string.Format("上传酒店{0},订单：{1},房间号:{2}，离店时间{3}，房费{4},杂费{5},杂费详情{6}", _hid, resCredit.OutId, resCredit.RoomNo, resCredit.CheckOut, resCredit.TotalRoomFee, resCredit.OtherFee, resCredit.OtherFeeDetail ?? ""));
                    UpdateReceiveXml(sendCredit, rspCredit.Body);
                    _pmsDb.SaveChanges();
                    ts2.Complete();
                }
                if (!"success".Equals(rspCredit.Result))
                {
                    throw new ApplicationException(rspCredit.SubErrMsg ?? rspCredit.SubErrCode ?? "网络异常，请重试");
                }
            }
             else
            {
                throw new ApplicationException(rsp.SubErrMsg ?? rsp.SubErrCode ?? "网络异常，请重试");
            }
            return new PayResult { RefNo = "阿里信用住付款", IsWaitPay = false};
        }
        private string GetParaValue(string paraCode, string defaultValue)
        {
            try
            {
                var _paras = _commDb.M_v_channelParas;
                if (_paras == null)
                {
                    return defaultValue;
                }
                var para = _paras.SingleOrDefault(w => w.Code == paraCode);
                if (para != null)
                {
                    return para.Value;
                }
                return defaultValue;
            }
            catch (Exception e)
            {
                return defaultValue;
            }
        }
        private ITopClient TopClientInstance(bool isEnvTest)
        {
            return new DefaultTopClient(Url(isEnvTest), AppKey(isEnvTest), AppSecret(isEnvTest), "json");
        }
        private string Url(bool isEnvTest)
        {            
            if (isEnvTest)
            {
                return GetParaValue("AlitripUrlTest", "http://gw.api.tbsandbox.com/router/rest");
            }
            return GetParaValue("AlitripUrl", "http://gw.api.taobao.com/router/rest");
        }
        private string AppKey(bool isEnvTest)
        {
            if (isEnvTest)
            {
                return GetParaValue("AlitripAppKeyTest", "1023274240");
            }
            return GetParaValue("AlitripAppKey", "23274240");
        }
        private string AppSecret(bool isEnvTest)
        {
            if (isEnvTest)
            {
                return GetParaValue("AlitripAppSecretTest", "sandboxbab90898b71a596362090be61");
            }
            return GetParaValue("AlitripAppSecret", "66d35b4bab90898b71a596362090be61");
        }

        private string Vendor
        {
            get { return GetParaValue("AlitripVendor", "jiexinda"); }
        }

        private string SessionKey(bool isEnvTest)
        {
            if (isEnvTest)
            {
                return GetParaValue("AlitripSessionKeyTest", "6101f24c31cee28550990c4cab1ebeae16a03968dd4afdd3688619053");
            }
            return GetParaValue("AlitripSessionKey", "6100d079b74db78159395e14e81eb4e91af3b6500e500e32730212118");
        }
        private SendXml AddSendXml(string hid, int notifyId, string type, string url, string content)
        {
            var sendXml = new SendXml
            {
                Hid = hid,
                NotifyId = notifyId,
                Cdate = DateTime.Now,
                SendType = type,
                Url = url,
                SendContent = content,
                SendDate = DateTime.Now
            };
            _pmsDb.SendXmls.Add(sendXml);
            return sendXml;
        }
        /// <summary>
        /// 更新接收到的内容
        /// </summary>
        /// <param name="sendXml">增加发送内容时返回的发送实例</param>
        /// <param name="content">接收到的内容</param>
        private void UpdateReceiveXml(SendXml sendXml, string content)
        {
            sendXml.ReceiveContent = content;
            sendXml.ReceiveDate = DateTime.Now;
            var obj = _pmsDb.Entry(sendXml);
            if (obj.State == EntityState.Unchanged)
            {
                obj.State = EntityState.Modified;
            }
        }
        /// <summary>
        /// 消费（名称，金额）
        /// </summary>
        private class Consume
        {
            public string Name { get; set; }
            public int Amount { get; set; }
        }

        private DbHotelPmsContext _pmsDb;
        private string _username;
        private string _shiftId;
        private string _hid;
        private bool _isTest;
        private DbCommonContext _commDb;
    }
}
