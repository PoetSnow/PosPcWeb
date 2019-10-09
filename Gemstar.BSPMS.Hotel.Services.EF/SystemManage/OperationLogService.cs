using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.SystemManage
{
    public class OperationLogService : IoperationLog
    {
        private DbHotelPmsContext _pmsContext;
        private ICurrentInfo _currentInfo;
        public OperationLogService(DbHotelPmsContext pmsContext, ICurrentInfo currentInfo)
        {
            _pmsContext = pmsContext;
            _currentInfo = currentInfo;
        }
        public List<UpQueryOperationLog> GetOperationLog(ResLogQueryPara para)
        {
            var maskTxt = GetMaskForProduct();

            return _pmsContext.Database.SqlQuery<UpQueryOperationLog>("exec up_list_operationLog @hid=@hid,@operationDateBegin=@operationDateBegin,@operationDateEnd=@operationDateEnd,@operationContent=@operationContent,@operator=@operator,@ip=@ip,@opeartionType=@opeartionType,@opeartionNo=@opeartionNo,@keys=@keys,@mask=@mask"
                   , new SqlParameter("@hid", para.HotelId ?? "")
                   , new SqlParameter("@operationDateBegin", para.operationDateBegin ?? "")
                   , new SqlParameter("@operationDateEnd", para.operationDateEnd ?? "")
                   , new SqlParameter("@operationContent", para.operationContent ?? "")
                   , new SqlParameter("@operator", para.operators ?? "")
                   , new SqlParameter("@ip", para.ip ?? "")
                   , new SqlParameter("@opeartionType", para.opeartionType ?? "")
                   , new SqlParameter("@opeartionNo", para.opeartionNo ?? "")
                   , new SqlParameter("@mask", maskTxt == "0000000000" ? null : maskTxt)
                   , new SqlParameter("@keys", para.billNo ?? "")
                  ).ToList();
        }


        /// <summary>
        /// 获取产品掩码
        /// </summary>
        /// <returns>10位掩码</returns>
        private string GetMaskForProduct()
        {
            var maskTxt = "0000000000";
            var maskNum = 0;
            if (_currentInfo == null)
            {
                maskTxt = "1000000000";
            }
            else
            {
                maskNum = (int)_currentInfo.ProductType;
                if (maskNum > 0 && maskNum <= 10)
                {
                    maskNum--;
                    maskTxt = maskTxt.Remove(maskNum, 1);
                    maskTxt = maskTxt.Insert(maskNum, "1");
                }
            }
            return maskTxt;
        }


        /// <summary>
        /// 获取单个客单日志
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="type">类型（1=主单ID，2=子单ID）</param>
        /// <param name="resid">主单ID 或 子单ID</param>
        /// <param name="keywords">内容关键字</param>
        /// <returns></returns>
        public List<OpLog> GetCustomerOrderLog(string hid, int type, string id, string keywords)
        {
            List<string> regids = new List<string>();

            if (type == 1)
            {
                regids = _pmsContext.ResDetails.AsNoTracking().Where(c => c.Hid == hid && c.Resid == id).Select(c => c.Regid).ToList();
                if (regids != null && regids.Count <= 0)
                {
                    return new List<OpLog>();
                }
            }
            else
            {
                regids.Add(id);
            }

            List<OpLog> list = _pmsContext.OpLogs.AsNoTracking().Where(c => c.Hid == hid && regids.Contains(c.Keys) && c.CText.Contains(keywords)).OrderByDescending(c => c.CDate).ToList();
            if (type == 1)
            {//主单ID，要获取客单设置
                List<OpLog> resSettingLogs = _pmsContext.OpLogs.AsNoTracking().Where(c => c.Hid == hid && c.Keys == id && c.CText.Contains(keywords)).ToList();
                if (resSettingLogs != null && resSettingLogs.Count > 0)
                {
                    list.AddRange(resSettingLogs);
                }
                list = list.OrderByDescending(c => c.CDate).ToList();
            }
            return list;
        }

        public List<PmsUser> GetPmsUser(string hid)
        {
            return _pmsContext.PmsUsers.Where(w => w.Grpid == hid).ToList();
        }

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="type">操作类型</param>
        /// <param name="text">操作内容</param>
        /// <param name="user">操作员</param>
        /// <param name="ip">操作IP</param>
        /// <param name="keys">关键字</param>
        public void AddOperationLog(string hid, OpLogType type, string text, string user, string ip, string keys = "")
        {
            var maskTxt = GetMaskForProduct();
            _pmsContext.Database.ExecuteSqlCommandAsync("INSERT INTO [opLog]([hid],[cDate],[cUser],[ip],[xType],[cText],[keys],[mask])VALUES(@hid,@cDate,@cUser,@ip,@xType,@cText,@keys,@mask)",
                new SqlParameter("@hid", hid),
                new SqlParameter("@cDate", DateTime.Now),
                new SqlParameter("@cUser", user),
                new SqlParameter("@ip", ip),
                new SqlParameter("@xType", type.ToString()),
                new SqlParameter("@cText", (text.Length > 4000 ? text.Substring(0, 4000) : text)),
                new SqlParameter("@keys", keys),
                new SqlParameter("@mask", maskTxt == "0000000000" ? null : maskTxt)
                ).Wait();
        }

        /// <summary>
        /// 添加订单操作日志
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="user">操作员</param>
        /// <param name="ip">操作IP</param>
        /// <param name="regids">账号,多个之间用逗号隔开</param>
        /// <param name="type">类型（0换房，1调价，2迟付）</param>
        /// <param name="value1">旧值</param>
        /// <param name="value2">新值</param>
        /// <param name="other1">其他内容 旧值</param>
        /// <param name="other2">其他内容 新值</param>
        /// <param name="remark">手动备注</param>
        /// <param name="describle">描述</param>
        public Guid AddResLog(string hid, string user, string ip, string regids, int type, string value1, string value2, string other1, string other2, string remark, string describle)
        {
            if (string.IsNullOrWhiteSpace(regids)) { return Guid.Empty; }
            if (!regids.Contains(","))
            {
                Guid id = Guid.NewGuid();
                _pmsContext.Database.ExecuteSqlCommandAsync("INSERT INTO [resLog]([id],[hid],[cDate],[cUser],[ip],[regid],[xType],[value1],[value2],[other1],[other2],[remark],[describle])VALUES(@id,@hid,@cDate,@cUser,@ip,@regid,@xType,@value1,@value2,@other1,@other2,@remark,@describle)",
                    new SqlParameter("@id", id),
                    new SqlParameter("@hid", hid),
                    new SqlParameter("@cDate", DateTime.Now),
                    new SqlParameter("@cUser", user),
                    new SqlParameter("@ip", ip),
                    new SqlParameter("@regid", regids),
                    new SqlParameter("@xType", type),
                    new SqlParameter("@value1", value1),
                    new SqlParameter("@value2", value2),
                    new SqlParameter("@other1", string.IsNullOrWhiteSpace(other1) ? "" : other1),
                    new SqlParameter("@other2", string.IsNullOrWhiteSpace(other2) ? "" : other2),
                    new SqlParameter("@remark", string.IsNullOrWhiteSpace(remark) ? "" : remark),
                    new SqlParameter("@describle", string.IsNullOrWhiteSpace(describle) ? "" : describle)
                    ).Wait();
                return id;
            }
            else
            {
                string[] list = regids.Split(',');
                foreach (var itemRegid in list)
                {
                    _pmsContext.ResLogs.Add(new ResLog
                    {
                        Id = Guid.NewGuid(),
                        Hid = hid,
                        CDate = DateTime.Now,
                        CUser = user,
                        Ip = ip,
                        Regid = itemRegid,
                        XType = (byte)type,
                        Value1 = value1,
                        Value2 = value2,
                        Other1 = string.IsNullOrWhiteSpace(other1) ? "" : other1,
                        Other2 = string.IsNullOrWhiteSpace(other2) ? "" : other2,
                        Remark = string.IsNullOrWhiteSpace(remark) ? "" : remark,
                        Describle = string.IsNullOrWhiteSpace(describle) ? "" : describle,
                    });
                }
                _pmsContext.SaveChanges();
                return Guid.NewGuid();
            }
        }

    }
}