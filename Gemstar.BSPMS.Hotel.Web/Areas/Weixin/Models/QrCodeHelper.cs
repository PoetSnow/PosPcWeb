using System;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.WeixinManage;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models
{
    /// <summary>
    /// 带参数二维码辅助类
    /// </summary>
    public class QrCodeHelper
    {
        /// <summary>
        /// 创建带参数二维码
        /// </summary>
        /// <param name="qrCodeService">创建服务</param>
        /// <param name="hid">酒店id</param>
        /// <param name="idType">参数类型</param>
        /// <param name="idValue">参数值</param>
        /// <param name="expireSeconds">有效时长秒数</param>
        /// <returns>创建是否成功，成功时数据是二维码对象实例</returns>
        public static JsonResultData CreateQrCode(IQrCodeService qrCodeService,string hid,QrCodeType idType,string idValue, int expireSeconds = 300)
        {
            try
            {
                var sceneId = qrCodeService.AddQrCode(hid, idType, idValue, expireSeconds);
                var addResult = QrCodeApi.Create(null, expireSeconds, sceneId, QrCode_ActionName.QR_SCENE);
                if (addResult.errcode == ReturnCode.请求成功)
                {
                    qrCodeService.UpdateQrCode(sceneId, addResult.ticket, addResult.url);
                    return JsonResultData.Successed(new WeixinQrcodes { SceneId = sceneId, Cdate = DateTime.Now, EndDate = DateTime.Now.AddSeconds(expireSeconds), Hid = hid, IdType = idType.ToString(), IdValue = idValue,Ticket = addResult.ticket,QrcodeContent = addResult.url });
                }
                else
                {
                    return JsonResultData.Failure(addResult.errmsg);
                }
            }
            catch(Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
    }
}