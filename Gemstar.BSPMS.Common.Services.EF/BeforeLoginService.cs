using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Common.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Services.EF
{
    public class BeforeLoginService : IBeforeLoginService
    {
        private string _centerDbConnectionStr { get; set; }
        private string _appName { get; set; }
        public BeforeLoginService(string centerDbConnectionString, string appName)
        {
            _centerDbConnectionStr = centerDbConnectionString;
            _appName = appName;
        }
        /// <summary>
        /// 根据酒店代码获取数据库连接，此方法仅限于酒店已存在时使用
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="centerDbConn"></param>
        /// <param name="appName"></param>
        /// <returns></returns>
        public JsonResultData GetConnectionString(string hid)
        {
            try
            {
                var sql = string.Format(@"select d.* from hotel h 
                                                 left join dbList d
                                                 on h.dbid=d.id
                                                 where h.hid='{0}'", hid);
                var db = ADOHelper.ExecSql(sql, _centerDbConnectionStr);
                if (db == null || db.Rows.Count <= 0)
                {
                    return JsonResultData.Failure("获取到数据库基本信息失败");
                }
                var dbServer = db.Rows[0]["dbServer"].ToString();
                var dbServerInternet = db.Rows[0]["intIp"].ToString();
                var dbName = db.Rows[0]["dbName"].ToString();
                var logId = db.Rows[0]["logid"].ToString();
                var logPwd = db.Rows[0]["logPwd"].ToString();
                if (string.IsNullOrEmpty(dbServer) || string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(logId) || string.IsNullOrEmpty(logPwd))
                    return JsonResultData.Failure("获取数据库详细信息失败");
                return JsonResultData.Successed(ConnStrHelper.GetConnStr(dbServer, dbName, logId, logPwd, _appName,dbServerInternet,ConnStrHelper.IsConnStrInternet(_centerDbConnectionStr)));
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        public JsonResultData DeleteHotelFromCenterDB(string hid)
        {
            try
            {
                var sql = "delete hotel where hid='" + hid + "'";
                var result = ADOHelper.ExecNonQuery(sql, _centerDbConnectionStr);
                return JsonResultData.Successed();
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        public JsonResultData InsertHotelToCenterDB(CenterHotel hotel)
        {
            try
            {
                var sql = "insert into hotel(grpid,hid,name,provinces,city,star,email,mobile,serverid,dbid,expirydate,status) values(@GrpId,@Hid,@Name,@Province,@City,@Star,@Email,@Mobile,@ServerId,@DbId,@ExpireDate,@Status)";
                List<SqlParameter> paras = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName="@GrpId",             Value=hotel.Grpid,            SqlDbType=SqlDbType.Char },
                    new SqlParameter() {ParameterName="@Hid",                Value=hotel.Hid,                SqlDbType=SqlDbType.Char },
                    new SqlParameter() {ParameterName="@Name",             Value=hotel.Name,             SqlDbType=SqlDbType.VarChar },
                    new SqlParameter() {ParameterName="@Province",         Value=hotel.Provinces,         SqlDbType=SqlDbType.VarChar },
                    new SqlParameter() {ParameterName="@City",                Value=hotel.City,                SqlDbType=SqlDbType.VarChar },
                    new SqlParameter() {ParameterName="@Star",                Value=hotel.Star,                 SqlDbType=SqlDbType.VarChar },
                    new SqlParameter() {ParameterName="@Email",              Value=hotel.Email,              SqlDbType=SqlDbType.VarChar },
                    new SqlParameter() {ParameterName="@Mobile",            Value=hotel.Mobile,            SqlDbType=SqlDbType.VarChar },
                    new SqlParameter() {ParameterName="@ServerId",            Value=hotel.Serverid,         SqlDbType=SqlDbType.UniqueIdentifier },
                     new SqlParameter() {ParameterName="@DbId",            Value=hotel.Dbid,            SqlDbType=SqlDbType.UniqueIdentifier },
                    new SqlParameter() {ParameterName="@ExpireDate",      Value=hotel.ExpiryDate,        SqlDbType=SqlDbType.Date },
                    new SqlParameter() {ParameterName="@Status",              Value=hotel.Status,                SqlDbType=SqlDbType.TinyInt }
                };
                var result = ADOHelper.ExecNonQuery(sql, _centerDbConnectionStr, paras);
                return JsonResultData.Successed();
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        public JsonResultData CheckHotelUser(string hid, string account)
        {
            var hotel_conn = GetConnectionString(hid);
            if (!hotel_conn.Success)
            {
                return hotel_conn;
            }
            var result = ADOHelper.ExecScalar("select count(0) from pmsUser where code='" + account + "'", hotel_conn.Data.ToString());
            if (int.Parse(result.ToString()) > 0)
            {
                return JsonResultData.Successed("用户账号已存在");
            }
            else
            {
                return JsonResultData.Failure("用户账号不存在");
            }
        }

        public JsonResultData ResetUserPassword(string hid, string account, string pwd)
        {
            try
            {
                var hotelConn = GetConnectionString(hid);
                var sql = "update pmsUser set pwd=@Pwd where grpid=@hid and code=@Account";
                List<SqlParameter> paras = new List<SqlParameter>();
                paras.Add(new SqlParameter() { ParameterName = "@Pwd", SqlDbType = SqlDbType.VarChar, Value = PasswordHelper.GetEncryptedPassword(account, pwd) });
                paras.Add(new SqlParameter() { ParameterName = "@hid", SqlDbType = SqlDbType.VarChar, Value = hid });
                paras.Add(new SqlParameter() { ParameterName = "@Account", SqlDbType = SqlDbType.VarChar, Value = account });
                ADOHelper.ExecNonQuery(sql, hotelConn.Data.ToString(), paras);
                return JsonResultData.Successed();
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }

        }

        private string GetModelHid(string hid)
        {
            var sql = "select v.modelHid from hotel h left join  serverList s on h.serverid=s.id left join versionList v on s.versionid=v.id where h.hid='" + hid + "'";
            return ADOHelper.ExecScalar(sql, _centerDbConnectionStr).ToString();
        }

        private void CreateBaseHotel(CenterHotel hotel, StringBuilder sb, List<SqlParameter> sqlParams, string loginCode, string loginName, string pwd, int isReg)
        {
            //生成酒店主体信息插入sql（ADO参数示例）
            sb.AppendLine(string.Format("insert into pmsHotel(GrpId,Hid,Name,Provinces,City,Star,Email,Mobile,Status,Hotelshortname,manageType) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')", hotel.Grpid, hotel.Hid, hotel.Name, hotel.Provinces, hotel.City, hotel.Star, hotel.Email, hotel.Mobile, hotel.Status, hotel.Hotelshortname, hotel.manageType));

            //生成默认账号
            if (string.IsNullOrEmpty(loginCode))
            {
                loginCode = "admin";
            }
            if (string.IsNullOrEmpty(loginName))
            {
                loginName = "系统管理员";
            }
            if (string.IsNullOrEmpty(pwd))
            {
                pwd = PasswordHelper.GetEncryptedPassword(loginCode, PasswordHelper.GetDefaultPasswordFromMobile(hotel.Mobile));
            }
            else
            {
                pwd = PasswordHelper.GetEncryptedPassword(loginCode, pwd);
            }
            if (string.IsNullOrWhiteSpace(hotel.Grpid) || hotel.Grpid == hotel.Hid)//集团分店不增加操作员
            {
                sb.AppendLine(string.Format("insert into pmsUser(grpid,id,Code,Name,email,qq,mobile,pwd,isReg,status,loginDate) values('{0}',newid(),'{1}','{2}','{3}','','{4}','{5}','{6}',1,null)", hotel.Hid, loginCode, loginName, hotel.Email, hotel.Mobile, pwd, isReg));
            }
        }

        /// <summary>
        /// 从模板酒店复制初始数据
        /// </summary>
        /// <param name="hotel"></param>
        /// <param name="appName"></param>
        public JsonResultData CopyModelHotel(CenterHotel hotel, string loginCode = "admin", string loginName = "系统管理员", string pwd = "", int isReg = 1)
        {
            try
            {
                //开始创建批量插入模板酒店信息的sql列表，生成插入数据的sql后，进行批量执行，无需重复打开数据库连接
                StringBuilder sb = new StringBuilder();
                List<SqlParameter> sqlParams = new List<SqlParameter>();
                //创建酒店主体信息的插入sql
                CreateBaseHotel(hotel, sb, sqlParams, loginCode, loginName, pwd, isReg);


                //开始创建批量获取模板酒店信息的sql列表，生成获取要复制的数据的sql后，进行批量执行，无需重复打开数据库连接
                List<ADOHelper.BatchSqlClass> batchSql = new List<ADOHelper.BatchSqlClass>();
                //获取模板酒店代码
                var modelHid = GetModelHid(hotel.Hid);
                //生成要创建新酒店的数据库连接
                var target_conn = GetConnectionString(hotel.Hid);
                if (!target_conn.Success)
                {
                    return JsonResultData.Failure("目标数据库连接获取失败");
                }
                //生成模板酒店的数据库连接
                var source_conn = GetConnectionString(modelHid);
                //如果存在模板酒店代码，如果连接存在
                if (!string.IsNullOrWhiteSpace(modelHid) && source_conn.Success)
                {
                    //创建模板酒店代码参数
                    SqlParameter _modelHid = new SqlParameter() { ParameterName = "@modelHid", SqlValue = modelHid, SqlDbType = SqlDbType.VarChar };
                    //根据模板酒店的数据库连接  在指定的数据库 验证模板酒店是否存在
                    var ModelHotelCount = ADOHelper.ExecScalar("select count(0) from pmsHotel where hid=@modelHid", source_conn.Data.ToString(), new List<SqlParameter>() { _modelHid });
                    //如果不存在，则返回
                    if (ModelHotelCount != null && int.Parse(ModelHotelCount.ToString()) != 0)
                    {
                        //将要执行的（这里指要获取的模板酒店信息的查询sql来获取源数据）sql语句添加了批量执行sql的列表
                        AddSqlToBatchSqlList(batchSql, _modelHid, hotel.Grpid);
                    }
                }
                //执行批量查询，并得到查询的结果集DataSet
                var ds = ADOHelper.ExecBatchSql(batchSql, source_conn.Data.ToString());

                //创建批量插入sql
                CreateBatchInsertSql(ds, sb, modelHid, hotel.Hid, hotel.Grpid);

                var sql = sb.ToString();
                //执行批量插入
                var result = ADOHelper.ExecNonQuery(sql, target_conn.Data.ToString(), sqlParams);

                return JsonResultData.Successed();
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        /// <summary>
        /// 根据源数据生成批量插入的sql
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="sb"></param>
        /// <param name="modelHid"></param>
        /// <param name="hid"></param>
        private void CreateBatchInsertSql(DataSet ds, StringBuilder sb, string modelHid, string hid, string grpid)
        {
            //其他初始化数据处理
            sb.AppendLine("exec up_hotelNew @hid='" + hid + "'");

            if (ds == null || ds.Tables.Count == 0)
            {
                return;
            }
            Dictionary<string, string> idMapping = new Dictionary<string, string>();
            var table_count = ds.Tables.Count;
            //循环源数据集合DataSet
            for (int i = 0; i < table_count; i++)
            {
                //单个表
                var table = ds.Tables[i];
                //表名
                var tableName = table.TableName;
                //循环单表数据行，每行生成一条插入sql
                for (int j = 0; j < table.Rows.Count; j++)
                {
                    var row = table.Rows[j];
                    var str = "insert into " + tableName + " values(";
                    //循环列拼接插入的sql
                    for (int k = 0; k < table.Columns.Count; k++)
                    {
                        //列数据类型
                        var dataType = table.Columns[k].DataType;
                        //列名
                        var columnName = table.Columns[k].ColumnName;
                        //列值
                        var columnValue = row[columnName].ToString();
                        //处理酒店hid列，直接用新酒店hid
                        if (columnName.Equals("hid", StringComparison.OrdinalIgnoreCase))
                        {
                            str += "'" + hid + "',";
                        }
                        //处理id列,角色表的id列的名称是roleid,而不是id，所以需要特殊判断
                        else if (columnName.Equals("id", StringComparison.OrdinalIgnoreCase) || (tableName.Equals("role", StringComparison.OrdinalIgnoreCase) && columnName.Equals("roleid", StringComparison.OrdinalIgnoreCase)))
                        {
                            //若id列为varchar类型，假如id由hid及code组合而成，则将hid部分由新的酒店hid替换，规定hid为前6位，所以替换前6位就可以了
                            if (dataType.Name == "String" && columnValue.StartsWith(modelHid))
                            {
                                str += "'" + (hid + columnValue.Substring(6)) + "',";
                            }
                            //若id列为uniqueidentifier类型，则生成新的id并将新旧id添加到idMapping中，以供其他表的外键id在关联此表id时可以根据模板id找到新id
                            //如果是集团分店则显示集团的市场分类和客人来源
                            //else if (tableName == "codeList" && (table.Rows[j]["typecode"].ToString() == "04" || table.Rows[j]["typecode"].ToString() == "05") && !string.IsNullOrWhiteSpace(grpid))   改成了分店自己的hid拼接后这段代码不需要
                            //{
                            //    str += "'" + columnValue + "',";
                            //}
                            else
                            {
                                var newid = Guid.NewGuid().ToString();
                                idMapping.Add(columnValue, newid);
                                str += "'" + newid + "',";
                            }
                        }
                        //处理外键id或组合id列
                        else if (columnName.EndsWith("id"))
                        {
                            //如果外键id列为varchar类型，假如外键id有hid及code组合而成，则将外键id的hid部分由新的酒店hid替换
                            if (dataType.Name == "String" && (columnValue.StartsWith(modelHid)))
                            {
                                str += "'" + (hid + columnValue.Substring(6)) + "',";
                            }
                            else if (dataType.Name == "Guid")
                            {
                                str += (columnValue == "" ? "null," : "'" + GetSqlValue(columnValue) + "',");
                            }
                            //如果外键id列为uniqueidentifier类型，则在idMapping中寻找其新的替换id，如若没有找到，则表示外键id列未生成新的id，不做处理
                            else
                            {
                                str += "'" + (idMapping.Keys.Contains(columnValue) ? idMapping[columnValue] : GetSqlValue(columnValue)) + "',";
                            }
                        }
                        //处理数值列，sql语句中无需添加单引号
                        else if (dataType.Name == "Int" || dataType.Name == "Byte")
                        {
                            str += (columnValue == "" ? "0" : columnValue) + ",";
                        }
                        else if (dataType.Name == "Decimal" && string.IsNullOrWhiteSpace(columnValue))
                        {
                            str += "'0.00',";
                        }
                        //其他非特殊列
                        else
                        {
                            if (!("codeList").Equals(tableName, StringComparison.OrdinalIgnoreCase) || !("pk").Equals(columnName))
                                str += "'" + GetSqlValue(columnValue) + "',";
                        }
                    }
                    str = str.TrimEnd(',') + ")";
                    sb.AppendLine(str);
                }
            }
            //为了以防在查询需要复制的数据源时，有外键关联表的顺序添加错误，导致外键id列未被替换，可在此处继续处理新旧id替换操作
            foreach (var item in idMapping)
            {
                if (sb.ToString().Contains(item.Key))
                    sb = sb.Replace(item.Key, item.Value);
            }
        }
        /// <summary>
        /// 将值更改为sql友好的字符串值，主要是将单引号替换为两个单引号，否则会引起sql语法错误
        /// </summary>
        /// <param name="value">要保存到数据库中的值</param>
        /// <returns>sql友好的值</returns>
        private string GetSqlValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            return value.Replace("'", "''");
        }

        /// <summary>
        /// 创建批量查询sql
        /// 存在外键id列的表，必须放在外键所关联的表的后面进行添加，否则将无法更新外键id列
        /// 比如表rateDetail中的列rateId是表rate的主键，那么必须先添加rate表，后添加rateDetail表
        /// 比如表hotelDayDetail中的列hotelDayId是表hotelDay的主键，那么必须先添加hotelDay表，后添加hotelDayDetail表
        /// </summary>
        /// <param name="batchSql"></param>
        /// <param name="modelHid"></param>
        private void AddSqlToBatchSqlList(List<ADOHelper.BatchSqlClass> batchSql, SqlParameter modelHid, string hid)
        {

            //获取模板酒店公用代码
            if (string.IsNullOrWhiteSpace(hid))
            {
                batchSql.Add(new ADOHelper.BatchSqlClass()
                {
                    TableName = "codeList",
                    Sql = "select * from codeList where hid= @modelHid",
                    Param = new List<SqlParameter>() { modelHid }
                });
            }
            else//集团分店，取集团市场分类和客人来源
            {
                batchSql.Add(new ADOHelper.BatchSqlClass()
                {
                    TableName = "codeList",
                    Sql = "select * from codeList where hid= '" + hid + "' and typecode in('04','05') union all select * from codeList where hid= @modelHid and typecode not in('04','05')",
                    Param = new List<SqlParameter>() { modelHid }
                });
            }
            //班次
            batchSql.Add(new ADOHelper.BatchSqlClass()
            {
                TableName = "shift",
                Sql = "SELECT * FROM shift WHERE hid = @modelHid",
                Param = new List<SqlParameter>() { modelHid }
            });

            //获取模板酒店房间类型
            batchSql.Add(new ADOHelper.BatchSqlClass()
            {
                TableName = "roomType",
                Sql = "select * from roomType where hid= @modelHid",
                Param = new List<SqlParameter>() { modelHid }
            });
            //角色
            batchSql.Add(new ADOHelper.BatchSqlClass()
            {
                TableName = "role",
                Sql = "SELECT * FROM role WHERE hid = @modelHid",
                Param = new List<SqlParameter>() { modelHid }
            });
            batchSql.Add(new ADOHelper.BatchSqlClass()
            {
                TableName = "roleAuth",
                Sql = "SELECT * FROM roleAuth WHERE hid = @modelHid",
                Param = new List<SqlParameter>() { modelHid }
            });
            batchSql.Add(new ADOHelper.BatchSqlClass()
            {
                TableName = "roleAuthReport",
                Sql = "SELECT * FROM roleAuthReport WHERE grpid = @modelHid",
                Param = new List<SqlParameter>() { modelHid }
            });

            //获取模板酒店会员卡类型
            batchSql.Add(new ADOHelper.BatchSqlClass()
            {
                TableName = "mbrCardType",
                Sql = "select * from mbrCardType where hid= @modelHid",
                Param = new List<SqlParameter>() { modelHid }
            });
            //获取模板酒店合约单位类型
            batchSql.Add(new ADOHelper.BatchSqlClass()
            {
                TableName = "companyType",
                Sql = "select * from companyType where hid= @modelHid",
                Param = new List<SqlParameter>() { modelHid }
            });
            //获取模板酒店消费项目及付款方式
            batchSql.Add(new ADOHelper.BatchSqlClass()
            {
                TableName = "item",
                Sql = "select * from item where hid= @modelHid",
                Param = new List<SqlParameter>() { modelHid }
            });
            //获取价格码
            batchSql.Add(new ADOHelper.BatchSqlClass()
            {
                TableName = "rate",
                Sql = "SELECT * FROM rate WHERE hid = @modelHid",
                Param = new List<SqlParameter>() { modelHid }
            });
            //自定义报表模板
            //  CenterHotel isgrouphotel = _db.Hotels.Where(c => c.Grpid != hid && c.Hid == hc.Hid && c.Grpid != null && c.Grpid != "").FirstOrDefault(); 

            batchSql.Add(new ADOHelper.BatchSqlClass()
            {
                TableName = "reportFormat",
                Sql = "SELECT * FROM reportFormat WHERE hid = @modelHid",
                Param = new List<SqlParameter>() { modelHid }
            });

        }
    }
}
