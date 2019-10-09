using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Web.Script.Serialization;
using System.Data.Entity;
using System.Web;
using Gemstar.BSPMS.Common.Tools;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.SMSSendManage;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.EF.PwdLock
{
    public interface ILock
    {
        ///写卡
        string WriteLock(IMasterService masterService, DbHotelPmsContext _pmsContext, ResDetail resDetailEntity, UpGetSmsInfoResult smsInfo, string lockid, string hid, string regId, string seqId, string beginTime);
        //注销卡
        void CancelLock(LockLog lockEntity, DbHotelPmsContext _pmsContext,string hid,string cardNo);
    }
}
