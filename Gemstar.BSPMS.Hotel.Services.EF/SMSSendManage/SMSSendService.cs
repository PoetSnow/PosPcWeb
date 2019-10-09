using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.SMSSendManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.SMSSendManage
{
    public class SMSSendService:ISMSSendService
    {
        public SMSSendService(DbHotelPmsContext hotelDb)
        {
            _hotelDb = hotelDb;
        }

        /// <summary>
        /// 根据指定会员充值记录发送充值提示短信
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="profileId">会员id</param>
        /// <param name="profileCaId">会员充值记录id</param>
        /// <param name="smsLogService">记录短信发送情况服务实例</param>
        /// <returns>发送结果</returns>
        public JsonResultData SendMbrRecharge(string hid, Guid profileId, string profileCaId,Dictionary<string,string> smsCenterParas,ISmsLogService smsLogService)
        {
            if (!IsSendMbrMsg("TransactionMsg", profileId.ToString(), hid))
            {
                return JsonResultData.Failure("此会员不发送短信");
            }
            if (string.IsNullOrWhiteSpace(profileCaId))
            {
                return JsonResultData.Failure("会员充值记录id不能为空");
            }
            //检测酒店是否有启用短信模块，没有则直接返回
            var smsInfo = _hotelDb.Database.SqlQuery<UpGetSmsInfoResult>("exec up_GetSmsInfo @hid=@hid", new SqlParameter("@hid", hid ?? "")).SingleOrDefault();
            if(smsInfo == null || smsInfo.Enable != "1")
            {
                return JsonResultData.Failure("酒店没有启用短信模块");
            }
            //取出会员的手机号
            var mobile = _hotelDb.Database.SqlQuery<string>("SELECT mobile FROM profile WHERE id = @profileId"
                , new SqlParameter("@profileId",profileId)
                //, new SqlParameter("@hid",hid??"")
                ).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(mobile) || !RegexHelper.IsRightMobile(mobile))
            {
                return JsonResultData.Failure("会员未设置正确的手机号");
            }
            //取出会员充值记录的详细情况
            var rechargeInfo = _hotelDb.Database.SqlQuery<UpPrintProfileRechargeResult>("exec up_print_profileRecharge @h99hid=@h99hid,@t00id=@t00id"
                , new SqlParameter("@h99hid",hid??"")
                , new SqlParameter("@t00id",profileCaId??"")
                ).ToList();
            if(rechargeInfo.Count == 0)
            {
                return JsonResultData.Failure("指定的充值记录id不正确");
            }
            //异步发送短信
            var sendPara = new SMSSendParaHotelMbrRecharge
            {
                Mobile = mobile,
                UserName = smsInfo.UserName,
                Password = smsInfo.Password,
                GuestName = rechargeInfo[0].客户名称,
                Balance = rechargeInfo[0].充值后金额 ?? 0,
                HotelName = rechargeInfo[0].酒店名,
                CurrentLargess = rechargeInfo[0].增值后金额 ?? 0,
                RechargeTime = rechargeInfo[0].充值时间 ?? DateTime.Now,
                LargessAmount = rechargeInfo[0].增值金额 ?? 0,
                RechargeAmount = rechargeInfo[0].充值金额 ?? 0
            };
            var async = new SMSSendAsync(sendPara,smsCenterParas,smsLogService);
            ThreadPool.QueueUserWorkItem(o => { ((SMSSendAsync)o).DoSend(); }, async);
            return JsonResultData.Successed("已经异步发送");
        }
        /// <summary>
        /// 发送会员消费短信提示
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="profileId">会员id</param>
        /// <param name="profileCaId">会员扣款记录id</param>
        /// <param name="smsCenterParas">中央库中的短信参数信息</param>
        /// <param name="smsLogService">短信发送日志记录实例</param>
        /// <returns>发送状态</returns>
        public JsonResultData SendMbrConsume(string hid, string profileId, string profileCaId, Dictionary<string, string> smsCenterParas, ISmsLogService smsLogService)
        {
            if (!IsSendMbrMsg("TransactionMsg", profileId, hid))
            {
                return JsonResultData.Failure("此会员不发送短信");
            }
            if (string.IsNullOrWhiteSpace(profileCaId))
            {
                return JsonResultData.Failure("会员扣款记录id不能为空");
            }
            //检测酒店是否有启用短信模块，没有则直接返回
            var smsInfo = _hotelDb.Database.SqlQuery<UpGetSmsInfoResult>("exec up_GetSmsInfo @hid=@hid", new SqlParameter("@hid", hid ?? "")).SingleOrDefault();
            if (smsInfo == null || smsInfo.Enable != "1")
            {
                return JsonResultData.Failure("酒店没有启用短信模块");
            }
            //取出会员的手机号
            var mobile = _hotelDb.Database.SqlQuery<string>("SELECT mobile FROM profile WHERE id = @profileId"
                , new SqlParameter("@profileId", profileId)
                //, new SqlParameter("@hid", hid ?? "")
                ).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(mobile) || !RegexHelper.IsRightMobile(mobile))
            {
                return JsonResultData.Failure("会员未设置正确的手机号");
            }
            //取出会员扣款记录的详细情况
            var rechargeInfo = _hotelDb.Database.SqlQuery<UpPrintProfileRechargeResult>("exec up_print_profileRecharge @h99hid=@h99hid,@t00id=@t00id"
                , new SqlParameter("@h99hid", hid ?? "")
                , new SqlParameter("@t00id", profileCaId ?? "")
                ).ToList();
            if (rechargeInfo.Count == 0)
            {
                return JsonResultData.Failure("指定的扣款记录id不正确");
            }
            //异步发送短信
            var sendPara = new SMSSendParaHotelMbrConsume
            {
                Mobile = mobile,
                UserName = smsInfo.UserName,
                Password = smsInfo.Password,
                GuestName = rechargeInfo[0].客户名称,
                HotelName = rechargeInfo[0].酒店名,
                AmountCharge = rechargeInfo[0].充值金额??0,
                ConsumeTime = rechargeInfo[0].充值时间??DateTime.Now,
                BalanceCharge = rechargeInfo[0].充值后金额??0,
            };
            if (rechargeInfo[0].增值金额.HasValue && (rechargeInfo[0].增值金额 != 0))
            {
                sendPara.AmountLargess = rechargeInfo[0].增值金额 ?? 0;
                sendPara.BalanceLargess = rechargeInfo[0].增值后金额 ?? 0;
            }
            var async = new SMSSendAsync(sendPara, smsCenterParas, smsLogService);
            ThreadPool.QueueUserWorkItem(o => { ((SMSSendAsync)o).DoSend(); }, async);
            return JsonResultData.Successed("已经异步发送");
        }
        /// <summary>
        /// 发送会员消费同时扣除会员储值和增值时的短信提示
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="profileId">会员id</param>
        /// <param name="profileCaIds">会员扣款记录id</param>
        /// <param name="smsCenterParas">中央库中的短信参数信息</param>
        /// <param name="smsLogService">短信发送日志记录实例</param>
        /// <returns>发送状态</returns>
        public JsonResultData SendMbrConsumeViaChargeAndLargess(string hid, string profileId, string profileCaIds, Dictionary<string, string> smsCenterParas, ISmsLogService smsLogService)
        {
            if (!IsSendMbrMsg("TransactionMsg", profileId, hid))
            {
                return JsonResultData.Failure("此会员不发送短信");
            }
            if (string.IsNullOrWhiteSpace(profileCaIds))
            {
                return JsonResultData.Failure("会员扣款记录id不能为空");
            }
            //检测酒店是否有启用短信模块，没有则直接返回
            var smsInfo = _hotelDb.Database.SqlQuery<UpGetSmsInfoResult>("exec up_GetSmsInfo @hid=@hid", new SqlParameter("@hid", hid ?? "")).SingleOrDefault();
            if (smsInfo == null || smsInfo.Enable != "1")
            {
                return JsonResultData.Failure("酒店没有启用短信模块");
            }
            //取出会员的手机号
            var mobile = _hotelDb.Database.SqlQuery<string>("SELECT mobile FROM profile WHERE id = @profileId AND hid = @hid"
                , new SqlParameter("@profileId", profileId)
                , new SqlParameter("@hid", hid ?? "")
                ).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(mobile) || !RegexHelper.IsRightMobile(mobile))
            {
                return JsonResultData.Failure("会员未设置正确的手机号");
            }
            //取出会员扣款记录的详细情况
            var rechargeInfo = _hotelDb.Database.SqlQuery<UpProfilecaConsumeSendSMSResult>("up_profileca_consumeSendSMS @h99hid=@h99hid,@t00profileid=@t00profileid,@t00ids=@t00ids"
                , new SqlParameter("@h99hid", hid ?? "")
                ,new SqlParameter("@t00profileid",profileId??"")
                , new SqlParameter("@t00ids", profileCaIds ?? "")
                ).ToList();
            if (rechargeInfo.Count == 0)
            {
                return JsonResultData.Failure("指定的扣款记录id不正确");
            }
            //异步发送短信
            var sendPara = new SMSSendParaHotelMbrConsume
            {
                Mobile = mobile,
                UserName = smsInfo.UserName,
                Password = smsInfo.Password,
                GuestName = rechargeInfo[0].GuestName,
                HotelName = rechargeInfo[0].HotelName,
                AmountCharge = rechargeInfo[0].ConsumeChargeAmount ?? 0,
                ConsumeTime = rechargeInfo[0].TransDate ?? DateTime.Now,
                BalanceCharge = rechargeInfo[0].BalanceCurrentCharge ?? 0,
                AmountLargess = rechargeInfo[0].ConsumeLargessAmount ?? 0,
                BalanceLargess = rechargeInfo[0].BalanceCurrentLargess ?? 0
            };
            var async = new SMSSendAsync(sendPara, smsCenterParas, smsLogService);
            ThreadPool.QueueUserWorkItem(o => { ((SMSSendAsync)o).DoSend(); }, async);
            return JsonResultData.Successed("已经异步发送");
        }
        //查询短信余额
        public int QueryBalance(string hid,Dictionary<string, string> smsCenterParas)
        {
            var smsInfo = _hotelDb.Database.SqlQuery<UpGetSmsInfoResult>("exec up_GetSmsInfo @hid=@hid", new SqlParameter("@hid", hid ?? "")).SingleOrDefault();
            if (smsInfo == null || smsInfo.Enable != "1")
            {
                return -1;
            }
            var senpara = new Dictionary<string, string>();
            senpara.Add("username", smsInfo.UserName);
            senpara.Add("password", smsInfo.Password);
            return SMSSendHelper.QueryBalance(senpara, smsCenterParas);
        }
        /// <summary>
        /// 发送密码锁的密码到客户手机
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="profileId"></param>
        /// <param name="profileCaIds"></param>
        /// <param name="smsCenterParas"></param>
        /// <param name="smsLogService"></param>
        /// <returns></returns>
        public JsonResultData SendHotelLockPassword(string hid, SMSSendParaHotelLockPwd sendPara, UpGetSmsInfoResult smsInfo,Dictionary<string, string> smsCenterParas, ISmsLogService smsLogService)
        {            
            sendPara.UserName = smsInfo.UserName;
            sendPara.Password = smsInfo.Password;
            var async = new SMSSendAsync(sendPara, smsCenterParas, smsLogService);
            ThreadPool.QueueUserWorkItem(o => { ((SMSSendAsync)o).DoSend(); }, async);
            return JsonResultData.Successed("已经异步发送");
        }
        /// <summary>
        /// 通用发送短信内容
        /// </summary>
        /// <param name="sendPara"></param>
        /// <param name="smsCenterParas"></param>
        /// <param name="smsLogService"></param>
        /// <returns></returns>
        public JsonResultData CommonSendSms(SMSSendParaCommonSms sendPara, Dictionary<string, string> smsCenterParas, ISmsLogService smsLogService)
        {
            var async = new SMSSendAsync(sendPara, smsCenterParas, smsLogService);
            ThreadPool.QueueUserWorkItem(o => { ((SMSSendAsync)o).DoSend(); }, async);
            return JsonResultData.Successed("已经异步发送");
        }
        /// <summary>
        /// 是否发送短信
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public bool IsSendMbrMsg(string msgType, string profileId,string hid)
        {
            var pid = Guid.Parse(profileId);
            //var result=_hotelDb.MbrCards.Where(w => w.Hid == hid && w.Id == pid);
            var result = _hotelDb.MbrCards.Where(w => w.Id == pid);
            var isSendMsg = true;
            if (msgType == "AdvertisementMsg")
                isSendMsg = result.Select(s => s.IsAdvertisementMsg).FirstOrDefault();
            else if (msgType == "TransactionMsg")
                isSendMsg = result.Select(s => s.IsTransactionMsg).FirstOrDefault();

            return isSendMsg;
        }
        private DbHotelPmsContext _hotelDb;
    }
    public class SMSSendAsync
    {
        private SMSSendParaHotel _sendPara;
        private Dictionary<string, string> _smsCenterParas;
        private ISmsLogService _smsLogService;
        public SMSSendAsync(SMSSendParaHotel sendPara,Dictionary<string,string> smsCenterParas,ISmsLogService smsLogService)
        {
            _sendPara = sendPara;
            _smsCenterParas = smsCenterParas;
            _smsLogService = smsLogService;
        }
        public void DoSend()
        {
            try
            {
                SMSSendHelper.SendHotelMessage(_sendPara,_smsCenterParas,_smsLogService);
            }
            catch
            {
                //先不做处理
            }
        }
    }
}
