using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

namespace Gemstar.BSPMS.Common.Tools
{
    public static class ExcelHelper
    {
        public static DataTable ExcelToDataTable(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            //String sConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Persist Security Info=True;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'";
            string sConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Persist Security Info=True;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1'";
            DataSet ds = new DataSet();
            OleDbDataAdapter oada = new OleDbDataAdapter("select * from [Sheet1$]", sConnectionString);
            try
            {
                oada.Fill(ds);
                oada.Dispose();
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }

        public static bool ExportToExcel(DataTable dt, string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return false;
                }
                String sConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Persist Security Info=True;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=2'";
                //String sConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + fileName + ";Extended Properties=Excel 12.0;HDR=YES";
                OleDbConnection cn = new OleDbConnection(sConnectionString);
                var columnCount = dt.Columns.Count;
                string sqlCreate =
                 @"CREATE TABLE [Sheet1] (";
                for (int i = 0; i < columnCount; i++)
                {
                    sqlCreate += "[" + dt.Columns[i].ColumnName + "] VarChar,";
                }
                sqlCreate = sqlCreate.TrimEnd(',');
                sqlCreate += ")";
                OleDbCommand cmd = new OleDbCommand(sqlCreate, cn);
                //创建Excel文件
                cn.Open();
                //创建表
                cmd.ExecuteNonQuery();

                var rowCount = dt.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    String queryString = @"INSERT INTO [Sheet1]  VALUES (";
                    for (int j = 0; j < columnCount; j++)
                    {
                        queryString += "'" + dt.Rows[i][j] + "',";
                    }
                    queryString = queryString.TrimEnd(',');
                    queryString += ")";
                    // 插入.
                    cmd.CommandText = queryString;
                    cmd.ExecuteNonQuery();
                }
                cmd.Dispose();
                cn.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// DataTable 返回 excel文件
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static byte[] DataTableToExcel(DataTable tb, string sheetName)
        {
            IWorkbook workbook = new XSSFWorkbook();
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                //创建表
                ISheet sheet = workbook.CreateSheet(sheetName);
                //标题行
                IRow rowTop = sheet.CreateRow(0);
                for (int t = 0; t < tb.Columns.Count; t++)
                {
                    rowTop.CreateCell(t).SetCellValue(tb.Columns[t].ColumnName);
                }
                //数据行
                for (int i = 0; i < tb.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    for (int j = 0; j < tb.Columns.Count; j++)
                    {
                        row.CreateCell(j).SetCellValue(tb.Rows[i][j].ToString());
                    }
                }
                //输出到内存
                workbook.Write(memoryStream);
                //返回
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                workbook.Close();
                memoryStream.Close();
                memoryStream.Dispose();
                tb.Dispose();
            }
            return null;
        }
    }

}
