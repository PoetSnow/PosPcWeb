using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.OnlineInterfaceManage;
using Gemstar.BSPMS.Hotel.Services.OnlineInterfaceManage.Invoice;

namespace Gemstar.BSPMS.Hotel.Services.EF.OnlineInterfaceManage
{
    public class InvoiceService : InvoiceBaseService<InvoiceModel>
    {


        public InvoiceService(IHotelInfoService hotelInfoService, DbHotelPmsContext pmsContext, ICurrentInfo currentInfo, InvoiceModel model)
             : base(hotelInfoService, pmsContext, currentInfo, InvoiceType.InvoicePosCY.ToString(), model)
        {
        }
        private string GetAccessToken()
        {
            var url = _model.RequestUrl + "/invoiceapi/GetToken";
            var content = "hid=" + HttpUtility.UrlEncode(_hid) + "&invoiceappid=" + HttpUtility.UrlEncode(_model.InvoiceAppId) + "&invoiceappsecret=" + HttpUtility.UrlEncode(_model.InvoiceAppSecret);
            var contentBytes = Encoding.UTF8.GetBytes(content);
            var responseData = "";
            var cDate = DateTime.Now;
            var isTrue = false;

            try
            {
                responseData = Request("post", url, contentBytes);
                var responseEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<InvoiceParameter.AccessToken.Response>(responseData);
                if (responseEntity != null && responseEntity.code == 1)
                {
                    isTrue = true;
                    return responseEntity.key; 
                }
            }
            catch (Exception ex)
            {
                responseData = responseData + "|ex:" + ex.ToString();
            }
            finally
            {
                if (!isTrue)
                {
                    AddLog(new List<OnlineInterfaceLog> { new OnlineInterfaceLog {
                        CDate = DateTime.Now
                        , Code = _code
                        , TypeCode = _typeCode
                        , Hid = _hid
                        , Url = url
                        , Regid = ""
                        , SendDate = cDate
                        , SendContent = content
                        , ReceiveDate = DateTime.Now
                        , ReceiveContent = responseData
                    } });
                }
            }

            return null;
        }


        /// <summary>
        /// 增加消费记录
        /// </summary>
        public override string ConsumInfo(string billid)
        {
            //up_pos_invoice_interface

            var entity = _pmsContext.Database.SqlQuery<InvoiceParameter.ConsumInfo.Request>("exec up_pos_invoice_interface @hid=@hid,@billid=@billid"
             , new System.Data.SqlClient.SqlParameter("@hid", _hid)
             , new System.Data.SqlClient.SqlParameter("@billid", billid)
             ).FirstOrDefault();
            if (entity == null) { return null; }

            string access_token = GetAccessToken(); if (string.IsNullOrWhiteSpace(access_token)) { return null; }
            string url = _model.RequestUrl + "/invoiceapi/ConsumInfo";

            var paras = new List<string>();
            paras.Add("hid=" + HttpUtility.UrlEncode(_hid));
            paras.Add("&key=" + HttpUtility.UrlEncode(access_token));
            paras.Add("&amount=" + HttpUtility.UrlEncode(entity.amount.ToString()));
            paras.Add("&billno=" + HttpUtility.UrlEncode(entity.billno));
            paras.Add("&cuser=" + HttpUtility.UrlEncode(entity.cuser));
            paras.Add("&guest=" + HttpUtility.UrlEncode(entity.guest));
            paras.Add("&keyno=" + HttpUtility.UrlEncode(entity.keyno));
            paras.Add("&outletcode=" + HttpUtility.UrlEncode(entity.outletcode));
            paras.Add("&outletname=" + HttpUtility.UrlEncode(entity.outletname));
            paras.Add("&remark=" + HttpUtility.UrlEncode(entity.remark));
            paras.Add("&roomno=" + HttpUtility.UrlEncode(entity.roomno));
            paras.Add("&searchtext=" + HttpUtility.UrlEncode(entity.searchtext));
            paras.Add("&shift=" + HttpUtility.UrlEncode(entity.shift));
            paras.Add("&transbsnsdate=" + HttpUtility.UrlEncode(entity.transbsnsdate));
            paras.Add("&transdate=" + HttpUtility.UrlEncode(entity.transdate));
            paras.Add("&transdetail=" + HttpUtility.UrlEncode(entity.transdetail));

            var content = string.Join("", paras);
            var contentBytes = Encoding.UTF8.GetBytes(content);
            var responseData = "";
            var cDate = DateTime.Now;
            var isTrue = false;

            try
            {
                responseData = Request("post", url, contentBytes);
                var responseEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<InvoiceParameter.ConsumInfo.Response>(responseData);
                if (responseEntity != null && responseEntity.code == 1)
                {
                    _pmsContext.Database.ExecuteSqlCommand("INSERT INTO [resFolioBillInvoice]([id],[hid],[billid],[cDate],[status],[invoiceType],[invoiceKey],[invoiceDate])VALUES(@id, @hid, @billid, @cDate, @status, @invoiceType, @invoiceKey, @invoiceDate)"
                        , new System.Data.SqlClient.SqlParameter("@id", Guid.NewGuid())
                        , new System.Data.SqlClient.SqlParameter("@hid", _hid)
                        , new System.Data.SqlClient.SqlParameter("@billid", billid)
                        , new System.Data.SqlClient.SqlParameter("@cDate", cDate)
                        , new System.Data.SqlClient.SqlParameter("@status", 1)
                        , new System.Data.SqlClient.SqlParameter("@invoiceType", _code)
                        , new System.Data.SqlClient.SqlParameter("@invoiceKey", responseEntity.obj)
                        , new System.Data.SqlClient.SqlParameter("@invoiceDate", DateTime.Now)
                        );
                    isTrue = true;
                    return "1§" + responseEntity.obj;
                }
            }
            catch (Exception ex)
            {
                responseData = responseData + "|ex:" + ex.ToString();
            }
            finally
            {
                if (!isTrue)
                {
                    AddLog(new List<OnlineInterfaceLog> { new OnlineInterfaceLog {
                        CDate = DateTime.Now
                        , Code = _code
                        , TypeCode = _typeCode
                        , Hid = _hid
                        , Url = url
                        , Regid = ""
                        , SendDate = cDate
                        , SendContent = content
                        , ReceiveDate = DateTime.Now
                        , ReceiveContent = responseData
                    } });
                }
            }

            return null;
        }

        /// <summary>
        /// 撤销消费记录
        /// </summary>
        public override void ConsumInfoRepeal(string billid)
        {
            var invoiceKey = _pmsContext.Database.SqlQuery<string>("select top 1 invoiceKey from resFolioBillInvoice where hid = @hid and billid = @billid and status = 1;"
               , new System.Data.SqlClient.SqlParameter("@hid", _hid)
               , new System.Data.SqlClient.SqlParameter("@billid", billid)
               ).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(invoiceKey)) { return; }

            string access_token = GetAccessToken(); if (string.IsNullOrWhiteSpace(access_token)) { return; }

            string url = _model.RequestUrl + "/invoiceapi/RepealConsum";
            var content = "hid=" + HttpUtility.UrlEncode(_hid) + "&key=" + HttpUtility.UrlEncode(access_token) + "&xf=" + HttpUtility.UrlEncode(invoiceKey);
            var contentBytes = Encoding.UTF8.GetBytes(content);
            string responseData = null;
            var cDate = DateTime.Now;
            var isTrue = false;
            try
            {
                responseData = Request("post", url, contentBytes);
                var responseEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<InvoiceParameter.ConsumInfoRepeal.Response>(responseData);
                if (responseEntity != null && responseEntity.code == 1)
                {
                    _pmsContext.Database.ExecuteSqlCommand("update resFolioBillInvoice set status = 51,invoiceRepealDate = @invoiceRepealDate where hid = @hid and billid = @billid and status = 1 and invoiceKey = @invoiceKey;"
                        , new System.Data.SqlClient.SqlParameter("@hid", _hid)
                        , new System.Data.SqlClient.SqlParameter("@billid", billid)
                        , new System.Data.SqlClient.SqlParameter("@invoiceKey", invoiceKey)
                        , new System.Data.SqlClient.SqlParameter("@invoiceRepealDate", DateTime.Now)
                        );
                    isTrue = true;
                }
            }
            catch (Exception ex)
            {
                responseData = responseData + "|ex:" + ex.ToString();
            }
            finally
            {
                if (!isTrue)
                {
                    AddLog(new List<OnlineInterfaceLog> { new OnlineInterfaceLog {
                        CDate = DateTime.Now
                        , Code = _code
                        , TypeCode = _typeCode
                        , Hid = _hid
                        , Url = url
                        , Regid = ""
                        , SendDate = cDate
                        , SendContent = content
                        , ReceiveDate = DateTime.Now
                        , ReceiveContent = responseData
                    } });
                }
            }

        }

        /// <summary>
        /// 获取商品代码列表
        /// </summary>
        public override InvoiceParameter.BaseResponse<List<InvoiceParameter.FindAllSP.Response>> FindAllSP(InvoiceParameter.FindAllSP.Request value)
        {
            var responseEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<InvoiceParameter.BaseResponse<List<InvoiceParameter.FindAllSP.Response>>>("");
            return responseEntity;
        }


        /// <summary>
        /// 获取商品代码
        /// </summary>
        public override InvoiceParameter.BaseResponse<InvoiceParameter.FindSP.Response> FindSP(InvoiceParameter.FindSP.Request value)
        {
            var responseEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<InvoiceParameter.BaseResponse<InvoiceParameter.FindSP.Response>>("");
            return responseEntity;
        }
    }
}
