/// <summary>
/// 编 码 人：苏飞
/// 联系方式：361983679  
/// 更新网站：http://www.sufeinet.com/thread-655-1-1.html
/// </summary>
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace PrintSqlite
{
    /// <summary>
    /// 数据访问基础类(基于SQLite)
    /// 可以用户可以修改满足自己项目的需要。
    /// </summary>
    public abstract class SQLiteHelper
    {
        //数据库连接字符串(web.config来配置)，可以动态更改connectionString支持多数据库.		
        public static string connectionString = ConfigurationManager.ConnectionStrings["SQLite"].ConnectionString;

        #region 公用方法
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static int GetMaxID(string fieldName, string tableName)
        {
            string strsql = "select max(" + fieldName + ")+1 from " + tableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns></returns>
        public static bool IsExists(string strSql)
        {
            object obj = GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 是否存在（基于SQLiteParameter）
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        public static bool IsExists(string strSql, params SQLiteParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 根据表名查询表的信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static DataTable GetTableInfo(string tableName)
        {
            string SQLString = "PRAGMA table_info([" + tableName + "])";
            return GetDataTable(SQLString);
        }
        #endregion

        #region  执行简单SQL语句
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SQLiteException ex)
                    {
                        connection.Close();
                        throw new Exception(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public static void ExecuteSqlTran(ArrayList SQLStringList)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = conn;
                SQLiteTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (SQLiteException ex)
                {
                    tx.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, string content)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand(SQLString, connection);
                SQLiteParameter myParameter = new SQLiteParameter("@content", DbType.String);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (SQLiteException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                SQLiteParameter myParameter = new SQLiteParameter("@fs", DbType.Binary);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (SQLiteException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SQLiteDataReader
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SQLiteDataReader</returns>
        public static SQLiteDataReader ExecuteReader(string strSQL)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
            try
            {
                connection.Open();
                SQLiteDataReader myReader = cmd.ExecuteReader();
                return myReader;
            }
            catch (SQLiteException e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet GetDataSet(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SQLiteDataAdapter command = new SQLiteDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (SQLiteException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataTable</returns>
        public static DataTable GetDataTable(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();
                    SQLiteDataAdapter command = new SQLiteDataAdapter(SQLString, connection);
                    DataSet ds = new DataSet();
                    command.Fill(ds, "dataSet");
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                    }
                }
                catch (SQLiteException ex)
                {
                    throw new Exception(ex.Message);
                }
                return dt;
            }
        }
        #endregion

        #region 执行带参数的SQL语句
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (SQLiteException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SQLiteParameter[]）</param>
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    SQLiteCommand cmd = new SQLiteCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SQLiteParameter[] cmdParms = (SQLiteParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            trans.Commit();
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (SQLiteException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SQLiteDataReader
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SQLiteDataReader</returns>
        public static SQLiteDataReader ExecuteReader(string SQLString, params SQLiteParameter[] cmdParms)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                SQLiteDataReader myReader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (SQLiteException e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet GetDataSet(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (SQLiteException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataTable</returns>
        public static DataTable GetDataTable(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    DataTable dt = new DataTable();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            dt = ds.Tables[0];
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return dt;
                }
            }
        }

        /// <summary>
        /// 执行命令前的准备
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="conn">连接</param>
        /// <param name="trans">事务</param>
        /// <param name="cmdText">语句</param>
        /// <param name="cmdParms">参数</param>
        private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, SQLiteTransaction trans, string cmdText, SQLiteParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (SQLiteParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        #endregion

        /// <summary>
        /// 批量导入 DataTable 到数据库
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="removeColumns">要移除的列名，多个请使用“,”分隔</param>
        /// <param name="dataTable">DataTable</param>
        public static bool BatchImportDataTable(string tableName, string removeColumns, DataTable dataTable)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(removeColumns))
                {
                    string[] names = removeColumns.Split(',');
                    for (int i = 0; i < names.Length; i++)
                    {
                        dataTable.Columns.Remove(names[i]);  //移除主键
                    }
                }

                //ArrayList sqlStringList = new ArrayList();
                //string SQLString = "PRAGMA table_info([" + tableName + "])";
                //var dt = GetDataTable(SQLString); //查询表信息
                //var s = dt.Columns.Count;
                InsertSqliteTable(dataTable, removeColumns, tableName);
                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    //循环拼接SQL语句
                //    foreach (DataRow row in dataTable.Rows)
                //    {
                //        string sql = "";
                //        var names = new StringBuilder();
                //        var values = new StringBuilder();
                //        foreach (DataRow dr in dt.Rows)
                //        {
                //            if (row.Table.Columns.Contains(dr["name"].ToString()))
                //            {
                //                //列名
                //                names.Append(dr["name"]);
                //                names.Append(",");
                //                //值
                //                values.Append("@" + dr["name"]);
                //                //  values.Append("'" + row[dr["name"].ToString()] + "'");
                //                values.Append(",");
                //                sql = string.Format("INSERT INTO {0}({1}) VALUES ({2});", tableName, names.ToString().TrimEnd(','), values.ToString().TrimEnd(','));
                //            }
                //        }

                //        sqlStringList.Add(sql);
                //    }

                //    ExecuteSqlTran(sqlStringList);  //执行事务
                //}

                return true;
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// 把获取到的数据插入到本地表
        /// </summary>
        /// <param name="dt2">拉取到的数据</param>
        /// <param name="localTableName">本地表名称</param>
        protected static void InsertSqliteTable(DataTable dataTable, string removeColumns, string localTableName)
        {

            int parameterCount;
            string[] columnNames;
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = conn;
                SQLiteTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    DataTable dt = GetReaderSchema(localTableName, removeColumns, conn);
                    cmd.CommandText = GetInsertSqliteSyntax(dt, localTableName, out parameterCount, out columnNames);
                    for (int i = 0; i < parameterCount; i++)
                    {
                        SQLiteParameter para = cmd.CreateParameter();
                        cmd.Parameters.Add(para);
                    }
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < parameterCount; j++)
                        {
                            var s = columnNames[j];
                            cmd.Parameters[j].ResetDbType();
                            cmd.Parameters[j].Value = dataTable.Rows[i][columnNames[j]];
                        }
                        cmd.ExecuteNonQuery();
                    }
                    tx.Commit();
                }
                catch (SQLiteException ex)
                {
                    tx.Rollback();
                    throw new Exception(ex.Message);
                }
            }

        }



        /// <summary>
        /// 根据表的字段加构信息生成插入数据到本地表的插入语句语法
        /// </summary>
        /// <param name="dtSchema">表的字段架构信息</param>
        /// <param name="localTableName">本地表名称</param>
        /// <param name="parameterCount">语法中包含的参数数量</param>
        public static string GetInsertSqliteSyntax(DataTable dtSchema, string localTableName, out int parameterCount, out string[] columnName)
        {
            StringBuilder insert = new StringBuilder();
            StringBuilder values = new StringBuilder();
            insert.Append("insert into ").Append(localTableName).Append(" (");
            values.Append(") values (");
            string splitChar = "";
            parameterCount = 0;
            columnName = new string[dtSchema.Rows.Count - 3];
            for (int i = 0; i < dtSchema.Rows.Count; i++)
            {
                DataRow dr = dtSchema.Rows[i];
                if (dr["ColumnName"].ToString() != "SerialNumber" && dr["ColumnName"].ToString() != "SendingTime" && dr["ColumnName"].ToString() != "CompletionTime")
                {
                    insert.Append(splitChar).Append(dr["ColumnName"].ToString());
                    values.Append(splitChar).Append("?");

                    columnName[parameterCount] = dr["ColumnName"].ToString();
                    parameterCount++;

                    splitChar = ",";
                }
            }
            insert.Append(values.ToString());
            insert.Append(")");
            return insert.ToString();
        }

        /// <summary>
        /// 获取表结构
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>

        private static DataTable GetReaderSchema(string tableName, string removeColumns, SQLiteConnection connection)
        {
            DataTable schemaTable = null;
            IDbCommand cmd = new SQLiteCommand();
            cmd.CommandText = string.Format("select * from [{0}]", tableName);
            cmd.Connection = connection;

            using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly))
            {
                schemaTable = reader.GetSchemaTable();
            }
            //if (!string.IsNullOrWhiteSpace(removeColumns))
            //{
            //    string[] names = removeColumns.Split(',');
            //    for (int i = 0; i < names.Length; i++)
            //    {
            //        schemaTable.Columns.Remove(names[i]);  //移除主键
            //    }
            //}
            return schemaTable;

        }

    }


}
