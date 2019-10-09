using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class RoleService : CRUDService<Role>, IRoleService
    {
        public RoleService(DbHotelPmsContext db) : base(db, db.Roles)
        {
            _pmsContext = db;
        }

        protected override Role GetTById(string id)
        {
            return new Role { Roleid = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 获取指定酒店的角色列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public IQueryable<Role> List(string hid)
        {
            return _pmsContext.Roles.Where(c => c.Hid == hid);
        }

        public string getRoleName(Guid roleid)
        {
          return _pmsContext.Roles.Where(c => c.Roleid == roleid).FirstOrDefault().Authname;
        }
    }
}
