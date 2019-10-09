using Gemstar.BSPMS.Common.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Gemstar.BSPMS.Common.Services.EF
{
    public class SmsLogService : ISmsLogService
    {
        private string _hid { get; set; }
        private string _centerDbConnectionString { get; set; }
        private string _createUser { get; set; }

        public SmsLogService(string hid, string conn, string createUser)
        {
            _hid = hid;
            _centerDbConnectionString = conn;
            _createUser = createUser;
        }



        public void AddLog(string mobile, string msg, string returnMsg, DateTime? sendDate = null)
        {
            if (sendDate == null)
            {
                sendDate = DateTime.Now;
            }
            var sql = "insert into smsLog(hid,moble,createtDate,sendDate,msg,msgReturn,createUse) values(@Hid,@Mobile,@CreateDate,@SendDate,@Msg,@ReturnMsg,@CreateUser)";
            List<SqlParameter> paras = new List<SqlParameter>();
            paras.Add(new SqlParameter() { ParameterName = "@Hid", SqlDbType = SqlDbType.VarChar, Value = _hid });
            paras.Add(new SqlParameter() { ParameterName = "@Mobile", SqlDbType = SqlDbType.VarChar, Value = mobile });
            paras.Add(new SqlParameter() { ParameterName = "@CreateDate", SqlDbType = SqlDbType.DateTime, Value = DateTime.Now });
            paras.Add(new SqlParameter() { ParameterName = "@SendDate", SqlDbType = SqlDbType.DateTime, Value = sendDate });
            paras.Add(new SqlParameter() { ParameterName = "@Msg", SqlDbType = SqlDbType.VarChar, Value = msg });
            paras.Add(new SqlParameter() { ParameterName = "@ReturnMsg", SqlDbType = SqlDbType.VarChar, Value = returnMsg });
            paras.Add(new SqlParameter() { ParameterName = "@CreateUser", SqlDbType = SqlDbType.VarChar, Value = _createUser });
            ADOHelper.ExecNonQuery(sql, _centerDbConnectionString, paras);
        }
    }
}
