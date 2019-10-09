using Gemstar.BSPMS.Common.Services.Entities;
using System;
using System.Linq;


namespace Gemstar.BSPMS.Common.Services.EF
{
    public class SysCheckCodeService : CRUDService<SysCheckCode>, ISysCheckCodeService
    {
        private DbCommonContext _db;
        public SysCheckCodeService(DbCommonContext db)
            : base(db, db.SysCheckCodes)
        {
            _db = db;
        }

        protected override SysCheckCode GetTById(string id)
        {
            var checkCode = new SysCheckCode();
            checkCode.Id = int.Parse(id);
            return checkCode;
        }

        public JsonResultData AddCheckCode(SysCheckCode model)
        {
            Add(model);
            Commit();
            return JsonResultData.Successed(90);
        }

        public JsonResultData GetCheckCode(string method, string methodValue, string func, out int seconds)
        {
            seconds = 0;
            var checks = _db.SysCheckCodes.Where(e => e.EndDate >= DateTime.Now && e.GetMethod == method && e.GetMethodValue == methodValue && e.Func == func);
            if (checks.Any())
            {
                seconds = (int)(checks.First().EndDate.Value - DateTime.Now).TotalSeconds;
                return JsonResultData.Successed(checks.First().CheckCode);
            }
            else
            {
                return JsonResultData.Failure("验证码未发送或已过期，请重新发送验证码");
            }

        }

        public JsonResultData DeleteCheckCode(string method, string methodValue, string func)
        {
            try
            {
                var checks = _db.SysCheckCodes.Where(e => e.EndDate >= DateTime.Now && e.GetMethod == method && e.GetMethodValue == methodValue && e.Func == func);
                foreach (var item in checks)
                {
                    Delete(item);
                }
                Commit();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
    }
}
