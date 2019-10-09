using System;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Services.SMSSendManage
{
    /// <summary>
    /// 短信发送服务
    /// </summary>
    public interface ISMSSendService
    {
        /// <summary>
        /// 根据指定会员充值记录发送充值提示短信
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="profileId">会员id</param>
        /// <param name="profileCaId">会员充值记录id</param>
        /// <param name="smsCenterParas">中央库中的短信配置参数</param>
        /// <param name="smsLogService">记录短信发送情况服务实例</param>
        /// <returns>发送结果</returns>
        JsonResultData SendMbrRecharge(string hid,Guid profileId,string profileCaId, Dictionary<string, string> smsCenterParas, ISmsLogService smsLogService);

        /// <summary>
        /// 发送会员消费短信提示
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="profileId">会员id</param>
        /// <param name="profileCaId">会员扣款记录id</param>
        /// <param name="smsCenterParas">中央库中的短信参数信息</param>
        /// <param name="smsLogService">短信发送日志记录实例</param>
        /// <returns>发送状态</returns>
        JsonResultData SendMbrConsume(string hid, string profileId, string profileCaId, Dictionary<string, string> smsCenterParas, ISmsLogService smsLogService);

        /// <summary>
        /// 发送会员消费同时扣除会员储值和增值时的短信提示
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="profileId">会员id</param>
        /// <param name="profileCaIds">会员扣款记录id</param>
        /// <param name="smsCenterParas">中央库中的短信参数信息</param>
        /// <param name="smsLogService">短信发送日志记录实例</param>
        /// <returns>发送状态</returns>
        JsonResultData SendMbrConsumeViaChargeAndLargess(string hid, string profileId, string profileCaIds, Dictionary<string, string> smsCenterParas, ISmsLogService smsLogService);
        /// <summary>
        /// 查询短信余额
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="smsCenterParas">中央库中的短信参数信息</param>
        /// <returns></returns>
        int QueryBalance(string hid, Dictionary<string, string> smsCenterParas);
        /// <summary>
        /// 发送密码锁短信
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="mobile"></param>
        /// <param name="guestName"></param>
        /// <param name="hotelName"></param>
        /// <param name="smsCenterParas"></param>
        /// <param name="smsLogService"></param>
        /// <returns></returns>
        JsonResultData SendHotelLockPassword(string hid, SMSSendParaHotelLockPwd sendPara, UpGetSmsInfoResult smsInfo, Dictionary<string, string> smsCenterParas, ISmsLogService smsLogService);
        /// <summary>
        /// 通用短信模板内容
        /// </summary>
        /// <param name="sendPara"></param>
        /// <param name="smsCenterParas"></param>
        /// <param name="smsLogService"></param>
        /// <returns></returns>
        JsonResultData CommonSendSms(SMSSendParaCommonSms sendPara, Dictionary<string, string> smsCenterParas, ISmsLogService smsLogService);
    }
}
