using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gemstar.BSPMS.Hotel.Services.SystemManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models
{
    /// <summary>
    /// GS微信营销系统 参数类
    /// </summary>
    public class GsWxInterfaceModel
    {
        private bool _IsEnable = false;
        private string _CompanyId = null;
        private string _OpenidUrl = null;
        private string _TemplateMessageUrl = null;
        private string _CreatePayOrderUrl = null;
        private string _PayOrderUrl = null;

        public GsWxInterfaceModel() { }
        public GsWxInterfaceModel(Services.EF.DbHotelPmsContext db, string hid)
        {
            try
            {
                if (db != null && !string.IsNullOrWhiteSpace(hid))
                {
                    var codeList = new List<string> {
                      "isGsWxInterface"
                    , "GsWxComid"
                    , "GsWxOpenidUrl"
                    , "GsWxTemplateMessageUrl"
                    , "GsWxCreatePayOrderUrl"
                    , "GsWxPayOrderUrl"
                };

                    var paraList = db.PmsParas.Where(c => c.Hid == hid && codeList.Contains(c.Code)).ToList();
                    if (paraList != null && paraList.Count > 0)
                    {
                        if (GetValue(paraList, "isGsWxInterface") == "1")
                        {
                            _IsEnable = true;
                        }
                        _CompanyId = GetValue(paraList, "GsWxComid");
                        _OpenidUrl = GetValue(paraList, "GsWxOpenidUrl");
                        _TemplateMessageUrl = GetValue(paraList, "GsWxTemplateMessageUrl");
                        _CreatePayOrderUrl = GetValue(paraList, "GsWxCreatePayOrderUrl");
                        _PayOrderUrl = GetValue(paraList, "GsWxPayOrderUrl");
                    }
                }
            }
            catch
            {
                _IsEnable = false;
            }
        }
        public GsWxInterfaceModel(HotelWxParas hotelWxParas)
        {
            if (hotelWxParas?.IsGsWxInterface == "1")
            {
                _IsEnable = true;
            }
            _CompanyId = hotelWxParas?.GsWxComid;
            _OpenidUrl = hotelWxParas?.GsWxOpenidUrl;
            _TemplateMessageUrl = hotelWxParas?.GsWxTemplateMessageUrl;
            _CreatePayOrderUrl = hotelWxParas?.GsWxCreatePayOrderUrl;
            _PayOrderUrl = hotelWxParas?.GsWxPayOrderUrl;
        }
        private string GetValue(List<Services.Entities.PmsPara> paraList, string code)
        {
            var entity = paraList.FirstOrDefault(w => w.Code == code);
            if (entity != null)
            {
                if (string.IsNullOrWhiteSpace(entity.Value))
                {
                    return entity.DefaultValue;
                }
                else
                {
                    return entity.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get { return _IsEnable; } }

        /// <summary>
        /// 公司ID、酒店ID
        /// </summary>
        public string CompanyId { get { return _CompanyId; } }

        /// <summary>
        /// 获取Openid的URL地址
        /// </summary>
        public string OpenidUrl { get { return _OpenidUrl; } }

        /// <summary>
        /// 发送模板消息的URL地址
        /// </summary>
        public string TemplateMessageUrl { get { return _TemplateMessageUrl; } }

        /// <summary>
        /// 支付下单的URL地址
        /// </summary>
        public string CreatePayOrderUrl { get { return _CreatePayOrderUrl; } }

        /// <summary>
        /// 支付的URL地址
        /// </summary>
        public string PayOrderUrl { get { return _PayOrderUrl; } }
    }
    /// <summary>
    /// GS微信营销系统 结果类
    /// </summary>
    public class GsWxInterfaceResultModel
    {
        /// <summary>
        /// 状态 200成功
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string orderSn { get; set; }
    }
}