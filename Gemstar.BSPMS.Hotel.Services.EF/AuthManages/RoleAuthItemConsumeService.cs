using Gemstar.BSPMS.Hotel.Services.AuthManages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services.EF;
using System.Data.Entity;
using System.Data.SqlClient;
using Gemstar.BSPMS.Common.Tools;

namespace Gemstar.BSPMS.Hotel.Services.EF.AuthManages
{
    public class RoleAuthItemConsumeService : CRUDService<RoleAuthItemConsume>, IRoleAuthItemConsumeService
    {
        private DbHotelPmsContext _pmsContext;
        public RoleAuthItemConsumeService(DbHotelPmsContext db) : base(db, db.RoleAuthItemConsumes)
        {
            _pmsContext = db;
        }

        protected override RoleAuthItemConsume GetTById(string id)
        {
            var roleAuthItemConsume = new RoleAuthItemConsume { IsAllow = bool.Parse(id) };
            return roleAuthItemConsume;
        }

        public List<V_ReportList> GetV_roleItemConsumelist(string hid, string roleid)
        {
            return _pmsContext.Database.SqlQuery<V_ReportList>("exec up_v_roleItemConsumelist @hid={0},@roleid={1}", hid, roleid).ToList();
        }

        public string ChangeRoleAuthItemConsume(string hid, string roleid, string reportcodes)
        {
            reportcodes = reportcodes.Trim(',');
            int a = _pmsContext.Database.ExecuteSqlCommand("exec up_UpdateRoleauthItemConsume @hid={0},@roleid={1},@reportcode={2}", hid, roleid, reportcodes);
            return a > 0 ? "成功" : "失败";
        }

        public List<RoleAuthItemConsume> GetItemConsumeIsAllow(string hid, string roleid)
        {
            Guid roleId = Guid.Parse(roleid);
            return _pmsContext.RoleAuthItemConsumes.Where(w => w.Grpid == hid && w.RoleId == roleId && w.IsAllow == true).ToList();
        }

        /// <summary>
        /// 获取指定的用户的消费录入权限
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="userid">用户ID</param>
        /// <returns>key=消费项目ID，value=是否可操作</returns>
        public List<KeyValuePairModel<string, bool>> GetItemConsumeAuth(string hid, string userid)
        {
            if (!string.IsNullOrWhiteSpace(hid) && !string.IsNullOrWhiteSpace(userid))
            {
                try
                {
                    var procResults = _pmsContext.Database.SqlQuery<KeyValuePairModel<string,bool>>("exec up_queryAuthListForOperatorItemConsume @userid=@userid,@hid=@hid"
                    , new SqlParameter("@userid", userid)
                    , new SqlParameter("@hid", hid)
                    ).ToList();
                    return procResults;
                }
                catch { }
            }
            return new List<KeyValuePairModel<string, bool>>();
        }

    }
}
