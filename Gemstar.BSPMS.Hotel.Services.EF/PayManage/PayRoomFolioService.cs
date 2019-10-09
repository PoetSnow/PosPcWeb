using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.SMSSendManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 会员卡费转房账
    /// </summary>
    public class PayRoomFolioService : PayBaseService
    {
        public PayRoomFolioService(DbHotelPmsContext pmsDb,string hid,string userName,string shiftId)
        {
            _pmsDb = pmsDb;
            _hid = hid;
            _username = userName;
            _shiftId = shiftId;
        }
        public override PayResult DoPayBeforeSaveFolio(string jsonStrPara)
        {
            var paraDic = GetParaDicFromJsonStr(jsonStrPara);
            var roomNo = paraDic["roomNo"].ToString();
            var amount = Convert.ToDecimal(paraDic["amount"]);
            var invno = paraDic["invno"].ToString();
            var remark = paraDic["remark"].ToString();
            var id = paraDic["Id"].ToString();
            var itemid = paraDic["itemid"].ToString();

            var sql = _pmsDb.Rooms.Where(c => c.Hid == _hid && c.RoomNo == roomNo);
            if (!sql.Any())
            {
                throw new ApplicationException("酒店不存在此房间号");
            }
            var resdetial = _pmsDb.ResDetails.Where(c => c.Hid == _hid && c.RoomNo == roomNo && c.Status == "I").FirstOrDefault();
            if (resdetial == null)
            {
                throw new ApplicationException("此房间号没有客人在住");
            }
            Guid profileid;
            if(!Guid.TryParse(id, out profileid))
            {
                throw new ApplicationException("此付款方式只用于会员卡费转房账");
            }
            var entity = GetProfileById(_pmsDb, _hid, profileid);
            remark = remark + string.Format("{0}[转房账会员卡号:{1};会员姓名:{2}]", remark,
                   entity.MbrCardNo ?? "",
                   entity.GuestName ?? "");
            var execResult = _pmsDb.Database.SqlQuery<upProfileCaInputResult>("exec up_resFolio_input @hid=@hid,@regid=@regid,@itemid=@itemid,@quantity=@quantity,@amount=@amount,@invNo=@invNo,@inputUser=@inputUser,@remark=@remark,@transShift=@transShift"
                , new SqlParameter("@hid", _hid)
                , new SqlParameter("@regid", resdetial.Regid)
                , new SqlParameter("@itemid", _hid + "0810")
                , new SqlParameter("@quantity", 1)
                , new SqlParameter("@amount", amount)
                , new SqlParameter("@invNo", invno)
                , new SqlParameter("@inputUser", _username)
                , new SqlParameter("@remark", remark)
                , new SqlParameter("@transShift", _shiftId)
                ).FirstOrDefault();
            if (execResult.Success)
            {
                remark = string.Format("[会员卡费转房账房间号:{0};主单号:{1}]",
                    roomNo,
                    resdetial.Resno??"");
                return new PayResult { RefNo = resdetial.Regid, IsWaitPay = false, Remark = remark };
            }
            else
            {
                throw new ApplicationException(string.Format("会员卡费转房账失败，原因：{0}", execResult.Data));
            }
        }
        private DbHotelPmsContext _pmsDb;
        private string _username;
        private string _shiftId;
        private string _hid;
    }
}
