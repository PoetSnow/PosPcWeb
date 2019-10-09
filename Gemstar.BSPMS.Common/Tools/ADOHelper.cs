using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Tools
{
    public static class ADOHelper
    {
        public class BatchSqlClass
        {
            public string TableName { get; set; }
            public string Sql { get; set; }
            public List<SqlParameter> Param { get; set; }
        }
        /// <summary>
        /// 批量执行sql，减少数据库连接
        /// </summary>
        /// <param name="batchSql"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static DataSet ExecBatchSql(List<BatchSqlClass> batchSql, string connStr)
        {
            DataSet ds = new DataSet();
            if (batchSql == null || batchSql.Count() == 0) return ds;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;

                foreach (var item in batchSql)
                {
                    DataTable dt = new DataTable();
                    dt.TableName = item.TableName;
                    cmd.CommandText = item.Sql;
                    cmd.Parameters.Clear();
                    if (item.Param != null && item.Param.Count > 0)
                    {
                        for (int i = 0, j = item.Param.Count; i < j; i++)
                        {
                            var p = (SqlParameter)((ICloneable)item.Param[i]).Clone();
                            if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && p.Value == null)
                            {
                                p.Value = DBNull.Value;
                            }
                            cmd.Parameters.Add(p);
                        }
                    }
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(dt);
                    ds.Tables.Add(dt);
                }
                conn.Close();
            }
            return ds;
        }

        /// <summary>
        /// 执行SQL语句 返回结果集
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="connStr">连接字符串</param>
        /// <param name="param">参数列表</param>
        /// <returns>结果集DataTable</returns>
        public static DataTable ExecSql(string sql, string connStr, List<SqlParameter> param = null, bool isSchema = false)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                if (param != null)
                {
                    foreach (SqlParameter p in param)
                    {
                        if (p.Value == null)
                        {
                            p.Value = DBNull.Value;
                        }
                        cmd.Parameters.Add(p);
                    }
                }
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                if (isSchema)
                {
                    sda.FillSchema(ds, SchemaType.Source);
                }
                sda.Fill(ds);
            }
            return ds.Tables[0];
        }


        /// <summary>
        /// 返回单个值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connStr">连接字符串</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object ExecScalar(string sql, string connStr, List<SqlParameter> param = null)
        {
            object result = null;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                if (param != null)
                {
                    foreach (SqlParameter p in param)
                    {
                        if (p.Value == null)
                        {
                            p.Value = DBNull.Value;
                        }
                        cmd.Parameters.Add(p);
                    }
                }

                con.Open();
                result = cmd.ExecuteScalar();
                con.Close();

            }
            return result;
        }

        /// <summary>
        /// 执行SQL语句(Update,Delete,Insert不包括查询 )
        /// </summary>
        /// <param name="sql">SQL语句</param>
        ///  <param name="connStr">连接字符串</param>
        /// <param name="param">参数列表</param>
        /// <returns></returns>
        public static int ExecNonQuery(string sql, string connStr, List<SqlParameter> param = null)
        {
            //声明数据库连接
            SqlConnection con = new SqlConnection();
            //设置数据库连接串 从配置文件(Web.config)获取
            con.ConnectionString = connStr;
            //打开数据库
            con.Open();
            //创建命令对象
            SqlCommand cmd = new SqlCommand();
            //与数据库连接关联
            cmd.Connection = con;
            //设置命令对象要执行的SQL语句
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;

            //先清空参数列表
            cmd.Parameters.Clear();
            if (param != null)
            {
                foreach (SqlParameter p in param)
                {
                    if (p.Value == null)
                    {
                        p.Value = DBNull.Value;
                    }
                    //重新添加参数
                    cmd.Parameters.Add(p);
                }
            }
            //执行SQL语句，返回受影响的行数
            int rows = cmd.ExecuteNonQuery();
            //关闭数据库连接
            con.Close();
            return rows;
        }
        /// <summary>
        /// 批量执行SQL语句(Update,Delete,Insert不包括查询 )
        /// </summary>
        /// <param name="sql">SQL语句数组</param>
        ///  <param name="connStr">连接字符串</param> 
        /// <returns></returns>
        public static int ExecBatchNonQuery(string[] sqls, string connStr)
        {
            //声明数据库连接
            SqlConnection con = new SqlConnection();
            //设置数据库连接串 从配置文件(Web.config)获取
            con.ConnectionString = connStr;
            //打开数据库
            con.Open();
            //创建命令对象
            SqlCommand cmd = new SqlCommand();
            //与数据库连接关联
            cmd.Connection = con;
            //设置命令对象要执行的SQL语句
            int rows = 0;
            for (int i = 0; i < sqls.Length; i++)
            {
                cmd.CommandText = sqls[i];
                cmd.CommandType = CommandType.Text;  
                //执行SQL语句，返回受影响的行数
                rows += cmd.ExecuteNonQuery();
            }
            //关闭数据库连接
            con.Close();
            return rows;
        }

        #region 批量插入SqlBulk
        public static void BulkInsert(string connStr, string tableName, DataTable table)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    try
                    {
                        conn.Open();
                        bulkCopy.DestinationTableName = tableName;
                        if (table != null && table.Rows.Count > 0)
                        {
                            bulkCopy.BatchSize = table.Rows.Count;
                            bulkCopy.WriteToServer(table);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        bulkCopy.Close();
                        conn.Dispose();
                        conn.Close();
                    }
                }
            }
        }
        #endregion

        #region 数据类型转换
        /// <summary>
        /// 数据类型转换
        /// </summary>
        /// <param name="sqlTypeString">slq类型 字符串</param>
        /// <returns></returns>
        public static SqlDbType SqlTypeStringToSqlType(string sqlTypeString)
        {
            SqlDbType dbType = SqlDbType.Variant;//默认为Object

            switch (sqlTypeString)
            {
                case "int":
                    dbType = SqlDbType.Int;
                    break;
                case "varchar":
                    dbType = SqlDbType.VarChar;
                    break;
                case "bit":
                    dbType = SqlDbType.Bit;
                    break;
                case "datetime":
                    dbType = SqlDbType.DateTime;
                    break;
                case "decimal":
                    dbType = SqlDbType.Decimal;
                    break;
                case "float":
                    dbType = SqlDbType.Float;
                    break;
                case "image":
                    dbType = SqlDbType.Image;
                    break;
                case "money":
                    dbType = SqlDbType.Money;
                    break;
                case "ntext":
                    dbType = SqlDbType.NText;
                    break;
                case "nvarchar":
                    dbType = SqlDbType.NVarChar;
                    break;
                case "smalldatetime":
                    dbType = SqlDbType.SmallDateTime;
                    break;
                case "smallint":
                    dbType = SqlDbType.SmallInt;
                    break;
                case "text":
                    dbType = SqlDbType.Text;
                    break;
                case "bigint":
                    dbType = SqlDbType.BigInt;
                    break;
                case "binary":
                    dbType = SqlDbType.Binary;
                    break;
                case "char":
                    dbType = SqlDbType.Char;
                    break;
                case "nchar":
                    dbType = SqlDbType.NChar;
                    break;
                case "numeric":
                    dbType = SqlDbType.Decimal;
                    break;
                case "real":
                    dbType = SqlDbType.Real;
                    break;
                case "smallmoney":
                    dbType = SqlDbType.SmallMoney;
                    break;
                case "sql_variant":
                    dbType = SqlDbType.Variant;
                    break;
                case "timestamp":
                    dbType = SqlDbType.Timestamp;
                    break;
                case "tinyint":
                    dbType = SqlDbType.TinyInt;
                    break;
                case "uniqueidentifier":
                    dbType = SqlDbType.UniqueIdentifier;
                    break;
                case "varbinary":
                    dbType = SqlDbType.VarBinary;
                    break;
                case "xml":
                    dbType = SqlDbType.Xml;
                    break;
            }
            return dbType;
        }
        /// <summary>
        /// 数据类型转换
        /// </summary>
        /// <param name="sqlType">sql类型</param>
        /// <returns></returns>
        public static Type SqlTypeToCsharpType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(Int64);
                case SqlDbType.Binary:
                    return typeof(Object);
                case SqlDbType.Bit:
                    return typeof(Boolean);
                case SqlDbType.Char:
                    return typeof(String);
                case SqlDbType.DateTime:
                    return typeof(DateTime);
                case SqlDbType.Decimal:
                    return typeof(Decimal);
                case SqlDbType.Float:
                    return typeof(Double);
                case SqlDbType.Image:
                    return typeof(Object);
                case SqlDbType.Int:
                    return typeof(Int32);
                case SqlDbType.Money:
                    return typeof(Decimal);
                case SqlDbType.NChar:
                    return typeof(String);
                case SqlDbType.NText:
                    return typeof(String);
                case SqlDbType.NVarChar:
                    return typeof(String);
                case SqlDbType.Real:
                    return typeof(Single);
                case SqlDbType.SmallDateTime:
                    return typeof(DateTime);
                case SqlDbType.SmallInt:
                    return typeof(Int16);
                case SqlDbType.SmallMoney:
                    return typeof(Decimal);
                case SqlDbType.Text:
                    return typeof(String);
                case SqlDbType.Timestamp:
                    return typeof(Object);
                case SqlDbType.TinyInt:
                    return typeof(Byte);
                case SqlDbType.Udt://自定义的数据类型
                    return typeof(Object);
                case SqlDbType.UniqueIdentifier:
                    return typeof(Object);
                case SqlDbType.VarBinary:
                    return typeof(Object);
                case SqlDbType.VarChar:
                    return typeof(String);
                case SqlDbType.Variant:
                    return typeof(Object);
                case SqlDbType.Xml:
                    return typeof(Object);
                default:
                    return null;
            }
        }
        #endregion

    }
}
