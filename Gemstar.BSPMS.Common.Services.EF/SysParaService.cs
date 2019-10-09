
using Gemstar.BSPMS.Common.Services.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Gemstar.BSPMS.Common.Services.EF
{
    public class SysParaService : CRUDService<SysPara>, ISysParaService
    {
        private DbCommonContext _db;
        public SysParaService(DbCommonContext db)
            : base(db, db.SysParas)
        {
            _db = db;
        }

        protected override SysPara GetTById(string code)
        {
            var para = new SysPara();
            para.Code = code;
            return para;
        }

        public Dictionary<string, string> GetDefaultConnPara()
        {
            return GetPara(new Dictionary<string, string>() {
                { "gsserverid", ""},
                { "gsdbid","" }
             });
        }

        public Dictionary<string, string> GetDefaultRegisterConnPara()
        {
            return GetPara(new Dictionary<string, string>() {
                { "pubserverid", ""},
                { "pubdbid","" }
             });
        }

        public Dictionary<string, string> GetQiniuPara()
        {
            return GetPara(new Dictionary<string, string>() {
                { "domain", ""},
                { "bucket","" },
                { "access_key",""},
                { "secret_key",""}
             });
        }

        public Dictionary<string, string> GetSMSSendPara()
        {
            return GetPara(new Dictionary<string, string>() {
                { "smssendurl", ""},
                { "smssendusername","" },
                { "smssendpassword",""}
            });
        }

        public Dictionary<string, string> GetEmailSendPara()
        {
            return GetPara(new Dictionary<string, string>() {
                { "emailsendusername", ""},
                { "emailsendpassword","" },
                { "emailsendserver",""},
                { "emailsendport",""}
            });
        }

        private Dictionary<string, string> GetPara(Dictionary<string, string> para)
        {
            var keys = para.Keys;
            var sysPara = _db.SysParas.Where(e => keys.Contains(e.Code.ToLower())).ToList();
            foreach (var item in sysPara)
            {
                var lowerCode = item.Code.Trim().ToLower();
                para[lowerCode] = string.IsNullOrEmpty(item.Value) ? item.RefValue : item.Value;
            }
            return para;
        }
        public List<HotelFunctions> GetHotelFunctionses(string hid)
        {
            return _db.HotelFunctionses.Where(w => w.Hid == hid).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetExpiredPara()
        {
            return GetPara(new Dictionary<string, string>() {
                { "expiredlogindaycount", ""},
                { "expiredremindadvancecontent","" },
                { "expiredremindcontent",""},
                { "expirreminddaycount",""}
            });
        } 
    }
}
