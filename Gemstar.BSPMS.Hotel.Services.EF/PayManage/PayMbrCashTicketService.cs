using System;
using System.Data.SqlClient;
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.MbrCardCenter;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 现金券付款服务
    /// </summary>
    public class PayMbrCashTicketService : PayBaseService
    {
        public PayMbrCashTicketService(DbHotelPmsContext pmsDb,string hid,string userName,string shiftId)
        {
            _pmsDb = pmsDb;
            _username = userName;
            _shiftId = shiftId;
            _hid = hid;
        }
        public override PayResult DoPayBeforeSaveFolio(string jsonStrPara)
        {
            var paraDic = GetParaDicFromJsonStr(jsonStrPara);
            var ticketNo = paraDic["ticketNo"].ToString();
            var amount = paraDic["amount"].ToString();
            var remark = paraDic["remark"].ToString();
            var regid = paraDic["regid"].ToString();
            var itemid = paraDic["itemid"].ToString();
            var entity = _pmsDb.ProfileCards.FirstOrDefault(p => p.TicketNo == ticketNo);
            if(entity == null)
            {
                throw new ApplicationException("票券号不存在");
            }
            var profileId = entity.Profileid;
            if (!IsHotelMbr(_pmsDb, profileId, _hid))
            {
                throw new ApplicationException("票券号不存在");
            }
            if (entity.Status != 1)
            {
                throw new ApplicationException("此票券已被使用");
            }
            if(entity.EndDate <= DateTime.Now)
            {
                throw new ApplicationException("此票券已过期");
            }            
            decimal? result = _pmsDb.Coupons.FirstOrDefault(w => w.Id == entity.TicketTypeid).CouponMoney;
            if (!string.IsNullOrWhiteSpace(amount))
            {
                if(decimal.Parse(amount) > result)
                {
                    throw new ApplicationException(string.Format("此代金券金额为{0}元,请修改原币金额",result));
                }
            }
            else
            {
                throw new ApplicationException("金额不能为空");
            }            
            var execResult = _pmsDb.Database.SqlQuery<upProfileCaInputResult>("EXEC up_profileCoupon @profileId=@profileId, @ticketNo = @ticketNo,@remarks=@remarks,@hid=@hid, @inputUser = @inputUser, @shiftId = @shiftId,@isUse=@isUse,@regid=@regid,@itemid=@itemid"
                , new SqlParameter("@profileId", profileId)
                , new SqlParameter("@ticketNo", ticketNo)
                , new SqlParameter("@remarks", "")
                , new SqlParameter("@hid", _hid)
                , new SqlParameter("@inputUser",_username??"")                
                , new SqlParameter("@shiftId", _shiftId)
                , new SqlParameter("@isUse", "2")
                , new SqlParameter("@regid", regid)
                , new SqlParameter("@itemid", itemid)
                ).Single();
            if (execResult.Success)
            {
                _profileCaId = execResult.Data.ToString();
                var profile = _pmsDb.MbrCards.FirstOrDefault(w => w.Id == profileId);
                
                return new PayResult { RefNo = execResult.Data, IsWaitPay = false,Remark=string.Format("[会员卡号:{0},姓名:{1},券票号:{2},券金额{3}]", profile.MbrCardNo,profile.GuestName,ticketNo, result) };
            }else
            {
                throw new ApplicationException(string.Format("现金券支付不成功，原因：{0}", execResult.Data));
            }
        }
        private DbHotelPmsContext _pmsDb;
        private string _username;
        private string _shiftId;
        private string _hid;
        private string _profileId;
        private string _profileCaId;
    }
}
