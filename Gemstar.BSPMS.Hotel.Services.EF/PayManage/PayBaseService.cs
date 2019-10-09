using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Script.Serialization;
using Aop.Api;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.PayManage.AliProviderPay;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 支付服务类基类，用于实现通用的一些实现
    /// </summary>
    public class PayBaseService:IPayService
    {
        /// <summary>
        /// 将json格式的字符串转换为字典对象
        /// </summary>
        /// <param name="jsonStrPara">json格式的参数字符串</param>
        /// <returns>字典对象</returns>
        public static Dictionary<string,object> GetParaDicFromJsonStr(string jsonStrPara)
        {
            var jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<Dictionary<string, object>>(jsonStrPara);
        }
        /// <summary>
        /// 根据会员卡号获取对应的会员id
        /// </summary>
        /// <param name="pmsDb">数据库实例</param>
        /// <param name="hid">酒店id</param>
        /// <param name="cardNo">会员卡卡号</param>
        /// <returns>对应的会员id，如果不存在则返回空字符</returns>
        public static string GetProfileIdByCardNo(DbHotelPmsContext pmsDb,string hid,string cardNo)
        {
            var entity = pmsDb.Database.SqlQuery<Entities.MbrCard>(@"EXEC dbo.up_query_mbrCard @hid = @hid,@cardNo = @cardNo"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@cardNo", cardNo)).SingleOrDefault();
            if(entity == null)
            {
                return null;
            }
            else
            {
                return entity.Id.ToString();
            }
            //return pmsDb.Database.SqlQuery<string>("SELECT convert(varchar(100),id) FROM dbo.profile WHERE hid=@hid and mbrCardNo = @cardNo AND status = 1"
            //    , new SqlParameter("@hid",hid??"")
            //    , new SqlParameter("@cardNo",cardNo??"")
            //    ).SingleOrDefault();
        }
        /// <summary>
        /// 根据会员卡ID查询卡信息
        /// </summary>
        /// <param name="pmsDb"></param>
        /// <param name="hid"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public static Entities.MbrCard GetProfileById(DbHotelPmsContext pmsDb, string hid, Guid profileId)
        {
            return pmsDb.Database.SqlQuery<Entities.MbrCard>(@"EXEC dbo.up_query_mbrCard @hid = @hid,@profileId = @profileId"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@profileId", profileId)).SingleOrDefault();
        }
        public static bool IsHotelMbr(DbHotelPmsContext pmsDb, Guid profileId,string hid)
        {
            var entity = pmsDb.Database.SqlQuery<Entities.MbrCard>(@"EXEC dbo.up_query_mbrCard @hid = @hid,@profileId = @profileId"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@profileId", profileId)).SingleOrDefault();
            return (entity != null);
        }
        #region 获取会员余额
        /// <summary>
        /// 获取会员余额
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public static Entities.MbrCardBalance GetProfileBalance(DbHotelPmsContext pmsDb,string hid,string profileId)
        {
            //取出会员的储值和增值余额，判断余额是否足够
            var profileIdValue = Guid.Parse(profileId);
            var profileBalance = pmsDb.MbrCardBalances.AsNoTracking().SingleOrDefault(w => w.profileId == profileIdValue);
            if (profileBalance == null)
            {
                throw new ApplicationException("会员余额不足");
            }

            return profileBalance;
        }
        #endregion
        /// <summary>
        /// 安全的从字典对象中获取值
        /// 如果对应键值存在，则返回对应的值
        /// 如果对应键值不存在，则返回指定的默认值
        /// </summary>
        /// <param name="dic">字典对象</param>
        /// <param name="key">键值</param>
        /// <param name="defaultValue">键值不存在时的默认值</param>
        /// <returns>对象的值</returns>
        public static string GetValueSafely(Dictionary<string,object> dic,string key,string defaultValue = "")
        {
            if (dic.ContainsKey(key))
            {
                var obj = dic[key];
                if(obj == null)
                {
                    return "";
                }
                return obj.ToString();
            }
            return defaultValue;
        }
        public static string GetAlipayOutTradeNo(string hid, PayProductType productType,string folioTransId)
        {
            return string.Format("{0}_{1}_{2}", hid, (int)productType, folioTransId);
        }

        public virtual PayAfterResult DoPayAfterSaveFolio(PayProductType productType,string payTransId, string jsonStrPara)
        {
            return new PayAfterResult {Statu = PayStatu.Successed,Callback = "",QueryTransId = "",QrCodeUrl = "" };
        }

        public virtual PayResult DoPayBeforeSaveFolio(string jsonStrPara)
        {
            return new PayResult { RefNo = "", IsWaitPay = false };
        }
        protected IAopClient GetClient(IPayLogService payLogService,string hid,AliPayConfigPara paraInfo)
        {

            payLogService.Debug(hid, "AlipayBarcodePay", "支付开始");

            //var serviceClient = new AlipayTradeImpl(paraInfo.ServerUrl, paraInfo.AppId, paraInfo.PrivateKey, "json", paraInfo.Version,paraInfo.SignType, paraInfo.AlipayPublicKey, paraInfo.Charset);
            var serviceClient = new DefaultAopClient(paraInfo.ServerUrl, paraInfo.AppId, paraInfo.PrivateKey, "json", paraInfo.Version, paraInfo.SignType, paraInfo.AlipayPublicKey, paraInfo.Charset,false);
            payLogService.Debug(hid, "AlipayBarcodePay", "传入的配置参数：[ServerUrl:" + paraInfo.ServerUrl + "][AppId:" + paraInfo.AppId + "][PrivateKey:" + paraInfo.PrivateKey + "][Version:" + paraInfo.Version + "][SignType:" + paraInfo.SignType + "][AlipayPublicKey:" + paraInfo.AlipayPublicKey + "][Charset:" + paraInfo.Charset + "]");
            return serviceClient;
        }
        /// <summary>
        /// 判断返回值是否正确
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        protected bool IsSuccessCode(AopResponse response)
        {
            return response != null && response.Code == ResultCode.SUCCESS;
        }
        /// <summary>
        /// 返回响应的处理失败消息
        /// </summary>
        /// <param name="response">响应对象</param>
        /// <returns>处理失败消息</returns>
        protected string FailResult(AopResponse response)
        {
            if (response == null || string.IsNullOrWhiteSpace(response.SubMsg))
            {
                return "未知错误，可能是网络不通或者密钥不正确导致无法解析结果";
            }
            return response.SubMsg;
        }
    }
}
