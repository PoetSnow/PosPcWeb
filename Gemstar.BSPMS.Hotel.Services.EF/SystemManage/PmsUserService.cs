using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System.Data.SqlClient;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Common.Services;
using System.Data;
using Gemstar.BSPMS.Common.Services.Enums;
using System.Web.Mvc;
using System.Data.Entity;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class PmsUserService : CRUDService<PmsUser>, IPmsUserService
    {
        public PmsUserService(DbHotelPmsContext db) : base(db, db.PmsUsers)
        {
            _pmsContext = db;
        }
        #region 赋初始密码
        public override void Add(PmsUser obj)
        {
            obj.Pwd = GetDefaultPassword(obj);
            //加密密码
            obj.Pwd = PasswordHelper.GetEncryptedPassword(obj.Code, obj.Pwd);

            base.Add(obj);
        }
        #endregion

        #region 获取指定操作员对指定集团下的可操作分店列表
        /// <summary>
        /// 获取指定操作员对指定集团下的可操作分店列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="userid">操作员id</param>
        /// <returns>操作员对该集团下的可操作分店列表</returns>
        public List<UpQueryResortListForOperatorResult> GetResortListForOperator(string grpid, string userid)
        {
            return _pmsContext.Database.SqlQuery<UpQueryResortListForOperatorResult>(
                "exec up_queryResortListForOperator @userId = @userId,@grpid=@grpid"
                , new SqlParameter("@userId", userid)
                , new SqlParameter("@grpid", grpid)
                ).ToList();
        }

        protected override PmsUser GetTById(string id)
        {
            return new PmsUser { Id = Guid.Parse(id) };
        }
        #endregion


        #region 查询指定的操作员的密码是否是默认密码
        /// <summary>
        /// 查询指定的操作员的密码是否是默认密码
        /// </summary>
        /// <param name="userid">要检查的操作员id</param>
        /// <returns>true:是默认密码，false:不是，用户已经修改过密码了</returns>
        public bool IsUserPassowrdDefault(string userid)
        {
            var idValue = Guid.Parse(userid);
            var user = _pmsContext.PmsUsers.Single(w => w.Id == idValue);
            var password = GetDefaultPassword(user);

            var encryptedPassword = PasswordHelper.GetEncryptedPassword(user.Code, password);

            return encryptedPassword.Equals(user.Pwd);
        }
        #endregion

        #region 修改操作员密码
        /// <summary>
        /// 修改指定操作员的密码
        /// </summary>
        /// <param name="userId">操作员id</param>
        /// <param name="originPassword">原密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>修改结果</returns>
        public JsonResultData ChangePassword(string userId, string originPassword, string newPassword)
        {
            //判断数据有效性
            if (string.IsNullOrWhiteSpace(userId))
            {
                return JsonResultData.Failure("请指定要修改的操作员id");
            }
            if (string.IsNullOrWhiteSpace(originPassword))
            {
                return JsonResultData.Failure("为了保证安全，请指定操作员的原始密码");
            }
            try
            {
                _pmsContext.Configuration.AutoDetectChangesEnabled = true;
                var idValue = Guid.Parse(userId);
                var user = _pmsContext.PmsUsers.SingleOrDefault(w => w.Id == idValue);
                if (user == null)
                {
                    return JsonResultData.Failure("指定的操作员不存在");
                }
                var encrypted = PasswordHelper.GetEncryptedPassword(user.Code, originPassword);
                if (!encrypted.Equals(user.Pwd))
                {
                    return JsonResultData.Failure("原密码不正确");
                }
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    return JsonResultData.Failure("请指定新密码");
                }
                string err;
                if (!PasswordHelper.IsPasswordValid(newPassword, out err))
                {
                    return JsonResultData.Failure(err);
                }
                if (newPassword.Equals(originPassword))
                {
                    return JsonResultData.Failure("新密码与原密码相同，不需要修改");
                }
                encrypted = PasswordHelper.GetEncryptedPassword(user.Code, newPassword);
                user.Pwd = encrypted;
                _pmsContext.AddDataChangeLogs(OpLogType.修改密码);
                _pmsContext.SaveChanges();
                _pmsContext.Configuration.AutoDetectChangesEnabled = false;
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
            return JsonResultData.Successed("");
        }
        #endregion
        /// <summary>
        /// 获取操作员的默认密码
        /// 现在的默认规则是根据操作员的手机号来获取默认密码
        /// </summary>
        /// <param name="user">要获取默认密码的操作员实例</param>
        /// <returns>默认密码</returns>
        private string GetDefaultPassword(PmsUser user)
        {
            return PasswordHelper.GetDefaultPasswordFromMobile(user.Mobile);
        }

        /// <summary>
        /// 批量更改状态（启用，禁用）
        /// </summary>
        /// <param name="ids">要更改的id，多项之间以逗号分隔</param>
        /// <param name="status">更新为当前状态</param>
        /// <returns>更改结果</returns>
        public JsonResultData BatchUpdateStatus(string ids, EntityStatus status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ids))
                {
                    return JsonResultData.Failure("请指定要修改的记录id，多项之间以逗号分隔");
                }
                var idArray = ids.Split(',');
                foreach (var id in idArray)
                {
                    PmsUser update = _pmsContext.PmsUsers.Find(Guid.Parse(id));
                    if (update.Status != status)
                    {
                        update.Status = status;
                        _pmsContext.Entry(update).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                _pmsContext.AddDataChangeLogs(OpLogType.操作员启用禁用);
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        /// <summary>
        /// 是否注册用户
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        public bool IsRegUser(string userid)
        {
            string[] users = userid.Split(',');
            for (int i = 0; i < users.Length; i++)
            {
                var idValue = Guid.Parse(users[i]);
                var user = _pmsContext.PmsUsers.Single(w => w.Id == idValue);
                if (user.IsReg == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsExistsCode(string code, string hid)
        {
            List<PmsUser> pu = _pmsContext.PmsUsers.Where(w => w.Code == code && w.Grpid == hid).ToList();
            if (pu.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsExists(string hid, string code, string name, Guid? notId = null)
        {
            if (notId != null && notId.HasValue && notId.Value != Guid.Empty)
            {
                //edit
                var grpid = _pmsContext.PmsUsers.AsNoTracking().Where(c => c.Id == notId).Select(c => c.Grpid).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(grpid)) { grpid = hid; }
                if (_pmsContext.PmsUsers.Any(c => c.Grpid == grpid && c.Code == code && c.Id != notId))
                {
                    return true;
                }
                if (_pmsContext.PmsUsers.Any(c => c.Grpid == grpid && c.Name == name && c.Id != notId))
                {
                    return true;
                }
                return false;
            }
            else
            {
                //add
                if (_pmsContext.PmsUsers.Any(c => c.Grpid == hid && c.Code == code))
                {
                    return true;
                }
                if (_pmsContext.PmsUsers.Any(c => c.Grpid == hid && c.Name == name))
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsExists(string hid, string code, string name, string cardId, Guid? notId = null)
        {
            if (notId != null && notId.HasValue && notId.Value != Guid.Empty)
            {
                //edit
                var grpid = _pmsContext.PmsUsers.AsNoTracking().Where(c => c.Id == notId).Select(c => c.Grpid).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(grpid)) { grpid = hid; }
                if (_pmsContext.PmsUsers.Any(c => c.Grpid == grpid && c.Code == code && c.Id != notId))
                {
                    return true;
                }
                if (_pmsContext.PmsUsers.Any(c => c.Grpid == grpid && c.Name == name && c.Id != notId))
                {
                    return true;
                }
                if (_pmsContext.PmsUsers.Any(c => c.Grpid == grpid && c.CardId == cardId && c.Id != notId))
                {
                    return true;
                }
                return false;
            }
            else
            {
                //add
                if (_pmsContext.PmsUsers.Any(c => c.Grpid == hid && c.Code == code))
                {
                    return true;
                }
                if (_pmsContext.PmsUsers.Any(c => c.Grpid == hid && c.Name == name))
                {
                    return true;
                }
                if (_pmsContext.PmsUsers.Any(c => c.Grpid == hid && c.CardId == cardId))
                {
                    return true;
                }
                return false;
            }
        }

        public JsonResultData ResetPwds(string[] userid)
        {
            foreach (var id in userid)
            {
                PmsUser update = _pmsContext.PmsUsers.Find(Guid.Parse(id));
                if (update.Mobile == null || update.Mobile == "")
                {
                    return JsonResultData.Failure("重置失败,所选操作员手机号不能为空！");
                }
                update.Pwd = PasswordHelper.GetEncryptedPassword(update.Code, update.Mobile);
                _pmsContext.Entry(update).State = System.Data.Entity.EntityState.Modified;
            }
            _pmsContext.AddDataChangeLogs(OpLogType.操作员重置密码);
            _pmsContext.SaveChanges();
            return JsonResultData.Successed("重置成功！");
        }
        /// <summary>
        /// 获取指定酒店的注册用户的手机号
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的注册用户的手机号，如果不存在则返回null</returns>
        public string GetRegUserMobile(string hid)
        {
            var user = _pmsContext.PmsUsers.Where(w => w.Grpid == hid && w.IsReg == 1).FirstOrDefault();
            if (user != null)
            {
                return user.Mobile;
            }
            return null;
        }
        /// <summary>
        /// 获取指定酒店和卡号的操作员信息
        /// </summary>
        /// <param name="gid">集团ID</param>
        /// <param name="hid">酒店ID</param>
        /// <param name="cardId">卡号</param>
        /// <returns></returns>
        public PmsUser GetEntityByCardId(string gid, string hid, string cardId)
        {
            return _pmsContext.PmsUsers.Where(w => (w.Grpid == gid || w.Grpid == hid) && w.CardId == cardId).FirstOrDefault();
        }
        /// <summary>
        /// 获取用户姓名
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店的用户姓名 </returns>
        public string GetUserName(string hid, Guid userid)
        {
            var user = _pmsContext.PmsUsers.Where(w => w.Grpid == hid && w.Id == userid).FirstOrDefault();
            if (user != null)
            {
                return user.Name;
            }
            return null;
        }

        /// <summary>
        /// 获取指定集团下的所有操作员列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <returns>集团下所有操作员列表</returns>
        public List<PmsUser> UsersInGroup(string grpid)
        {
            return _pmsContext.PmsUsers.Where(w => w.Grpid == grpid).ToList();
        }
        /// <summary>
        /// 取消绑定指定操作员的微信信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="userId">操作员id</param>
        /// <returns>取消结果</returns>
        public JsonResultData UnbindWeixin(string hid, string userId, DbCommonContext centerDb)
        {
            try
            {
                var idValue = Guid.Parse(userId);
                PmsUser update = _pmsContext.PmsUsers.Where(w => w.Grpid == hid && w.Id == idValue).FirstOrDefault();
                if (update == null)
                {
                    return JsonResultData.Failure("没找到对应的操作员");
                }
                if (!string.IsNullOrWhiteSpace(update.WxOpenId))
                {
                    //删除绑定
                    var deleteList = centerDb.WeixinOperatorHotelMappings.Where(c => c.Hid == hid && c.OperatorId == update.Id && c.OperatorWxOpenId == update.WxOpenId);
                    centerDb.WeixinOperatorHotelMappings.RemoveRange(deleteList);
                    centerDb.SaveChanges();

                    update.WxOpenId = "";
                    _pmsContext.Entry(update).State = System.Data.Entity.EntityState.Modified;
                }
                _pmsContext.AddDataChangeLogs(OpLogType.操作员微信解绑);
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        public bool GroupControlAdd(string usercode, string Belonghotel, string grpid)
        {
            PmsUser user = _pmsContext.PmsUsers.Where(w => w.Grpid == grpid && w.Code == usercode).FirstOrDefault();
            string[] hotelids = Belonghotel.Split(',');
            foreach (var h in hotelids)
            {
                if (_pmsContext.PmsUsers.Where(w => w.Grpid == h && w.Code == usercode).Count() <= 0)
                {
                    PmsUser entity = new PmsUser();
                    entity = user;
                    entity.Id = Guid.NewGuid();
                    entity.Grpid = h;
                    entity.Belonghotel = "";
                    _pmsContext.PmsUsers.Add(entity);
                    _pmsContext.SaveChanges();
                }
            }
            return true;
        }

        public bool GroupControlEdit(string usercode, string Belonghotel, string grpid)
        {
            PmsUser user = _pmsContext.PmsUsers.AsNoTracking().Where(w => w.Grpid == grpid && w.Code == usercode).FirstOrDefault();
            string[] hotel = Belonghotel.Split(',');
            foreach (var h in hotel)
            {
                PmsUser users = _pmsContext.PmsUsers.AsNoTracking().Where(w => w.Grpid == h && w.Code == usercode).FirstOrDefault();
                if (users == null)
                {
                    PmsUser entity = new PmsUser();
                    entity = user;
                    entity.Id = Guid.NewGuid();
                    entity.Grpid = h;
                    entity.Belonghotel = "";
                    _pmsContext.PmsUsers.Add(entity); _pmsContext.SaveChanges();
                }
                else
                {
                    users.Name = user.Name;
                    users.Mobile = user.Mobile;
                    users.Email = user.Email;
                    users.Qq = user.Qq;
                    users.AnalysisUserCode = user.AnalysisUserCode;
                    _pmsContext.Entry(users).State = EntityState.Modified; _pmsContext.SaveChanges();
                }
            }

            return true;
        }
        public string getGrouphotelid(string id)
        {
            string[] arrid = id.Split(',');
            string str = id + ",";
            for (int j = 0; j < arrid.Length; j++)
            {
                Guid ids = Guid.Parse(arrid[j]);
                PmsUser s = _pmsContext.PmsUsers.AsNoTracking().Where(w => w.Id == ids).FirstOrDefault();
                Guid[] User = _pmsContext.PmsUsers.AsNoTracking().Where(w => w.Name == s.Name && s.Belonghotel.Contains(w.Grpid)).Select(w => w.Id).ToArray();
                for (int i = 0; i < User.Count(); i++)
                {
                    str += User[i].ToString() + ",";
                }
            }
            return str.Trim(',');
        }
        public PmsUser GetUserIDByCode(string hid, string code)
        {
            return _pmsContext.PmsUsers.Where(m => m.Grpid == hid && m.Code == code).FirstOrDefault();
        }

        private DbHotelPmsContext _pmsContext;



    }


}
