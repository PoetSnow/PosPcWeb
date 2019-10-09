using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using System.Text;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.POIFS;
using NPOI.Util;
using NPOI.SS.Util;
using System;

namespace Gemstar.BSPMS.Common.Tools
{

    /// <summary>
    /// Excel导入，导出操作类
    /// lcl add 2017-06-22
    /// </summary>
    public class ExcelHelp
    {
        #region Excel导出方法 ExportByWeb(dtSource,strHeaderText,strFileName)
        /// <summary>
        /// Excel导出方法 ExportByWeb()
        /// </summary>
        /// <param name="dtSource">DataTable数据源</param>
        /// <param name="strHeaderText">Excel表头文本（例如：车辆列表）</param>
        /// <param name="strFileName">Excel文件名（例如：车辆列表.xls）</param>
        public static void ExportByWeb(DataTable dtSource, string strHeaderText, string strFileName)
        {
            HttpContext curContext = HttpContext.Current;
            // 设置编码和附件格式
            curContext.Response.ContentType = "application/ms-excel";
            curContext.Response.ContentEncoding = Encoding.UTF8;
            curContext.Response.Charset = "";
            curContext.Response.AppendHeader("Content-Disposition",
                "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));
            //调用导出具体方法Export()
            curContext.Response.BinaryWrite(Export(dtSource, strHeaderText).GetBuffer());
            curContext.Response.End();
        }
        #endregion

        #region DataTable导出到Excel文件 Export(dtSource,strHeaderText,strFileName)
        /// <summary>
        /// DataTable导出到Excel文件 Export()
        /// </summary>
        /// <param name="dtSource">DataTable数据源</param>
        /// <param name="strHeaderText">Excel表头文本（例如：车辆列表）</param>
        /// <param name="strFileName">保存位置</param>
        public static void Export(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = Export(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }
        #endregion

        #region DataTable导出到Excel的MemoryStream Export(dtSource,strHeaderText)
        /// <summary>
        /// DataTable导出到Excel的MemoryStream Export()
        /// </summary>
        /// <param name="dtSource">DataTable数据源</param>
        /// <param name="strHeaderText">Excel表头文本（例如：车辆列表）</param>
        public static MemoryStream Export(DataTable dtSource, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();

            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "NPOI";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "文件作者信息"; //填加xls文件作者信息
                si.ApplicationName = "创建程序信息"; //填加xls文件创建程序信息
                si.LastAuthor = "最后保存者信息"; //填加xls文件最后保存者信息
                si.Comments = "作者信息"; //填加xls文件作者信息
                si.Title = "标题信息"; //填加xls文件标题信息
                si.Subject = "主题信息";//填加文件主题信息
                si.CreateDateTime = System.DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet();
                    }

                    #region 表头及样式
                    {
                        IRow headerRow = sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center; // ------------------
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1)); // ------------------
                    }
                    #endregion

                    #region 列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(1);
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center; // ------------------
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                        }
                    }
                    #endregion

                    rowIndex = 2;
                }
                #endregion

                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    ICell newCell = dataRow.CreateCell(column.Ordinal);
                    string drValue = row[column].ToString();
                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型
                            System.DateTime dateV;
                            System.DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);
                            newCell.CellStyle = dateStyle;//格式化显示
                            break;
                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }
                }
                #endregion
                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
               // sheet.Disponse();                
                return ms;
            }
        }
        #endregion

        #region 读取excel ,默认第一行为标头Import()
        /// <summary>
        /// 读取excel ,默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable Import(string strFileName)
        {
            DataTable dt = new DataTable();

            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }
        #endregion

        ///// <summary>
        /////  组装workbook.
        ///// </summary>
        ///// <param name="dt">dataTable资源</param>
        ///// <param name="columnHeader">表头</param>
        ///// <returns></returns>
        //public static HSSFWorkbook BuildWorkbook1(DataTable dt, string columnHeader = "")
        //{
        //    var workbook = new HSSFWorkbook();
        //    ISheet sheet = workbook.CreateSheet(string.IsNullOrWhiteSpace(dt.TableName) ? "Sheet1" : dt.TableName);

        //    #region 文件属性信息
        //    {
        //        var dsi = PropertySetFactory.CreateDocumentSummaryInformation();
        //        dsi.Company = "NPOI";
        //        workbook.DocumentSummaryInformation = dsi;

        //        SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
        //        si.Author = "文件作者信息";
        //        si.ApplicationName = "创建程序信息";
        //        si.LastAuthor = "最后保存者信息";
        //        si.Comments = "作者信息";
        //        si.Title = "标题信息";
        //        si.Subject = "主题信息";
        //        si.CreateDateTime = DateTime.Now;
        //        workbook.SummaryInformation = si;
        //    }
        //    #endregion

        //    var dateStyle = workbook.CreateCellStyle();
        //    var format = workbook.CreateDataFormat();
        //    dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

        //    //取得列宽
        //    var arrColWidth = new int[dt.Columns.Count];
        //    foreach (DataColumn item in dt.Columns)
        //    {
        //        arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
        //    }
        //    for (var i = 0; i < dt.Rows.Count; i++)
        //    {
        //        for (var j = 0; j < dt.Columns.Count; j++)
        //        {
        //            int intTemp = Encoding.GetEncoding(936).GetBytes(dt.Rows[i][j].ToString()).Length;
        //            if (intTemp > arrColWidth[j])
        //            {
        //                arrColWidth[j] = intTemp;
        //            }
        //        }
        //    }
        //    int rowIndex = 0;
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        #region 表头 列头
        //        if (rowIndex == 65535 || rowIndex == 0)
        //        {
        //            if (rowIndex != 0)
        //            {
        //                sheet = workbook.CreateSheet();
        //            }

        //            #region 表头及样式
        //            {
        //                var headerRow = sheet.CreateRow(0);
        //                headerRow.HeightInPoints = 25;
        //                headerRow.CreateCell(0).SetCellValue(columnHeader);
        //                //CellStyle
        //                ICellStyle headStyle = workbook.CreateCellStyle();
        //                headStyle.Alignment = HorizontalAlignment.Center;// 左右居中    
        //                headStyle.VerticalAlignment = VerticalAlignment.Center;// 上下居中 
        //                                                                       // 设置单元格的背景颜色（单元格的样式会覆盖列或行的样式）    
        //                headStyle.FillForegroundColor = (short)11;
        //                //定义font
        //                IFont font = workbook.CreateFont();
        //                font.FontHeightInPoints = 20;
        //                font.Boldweight = 700;
        //                headStyle.SetFont(font);
        //                headerRow.GetCell(0).CellStyle = headStyle;
        //                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dt.Columns.Count - 1));
        //            }
        //            #endregion


        //            #region 列头及样式
        //            {
        //                var headerRow = sheet.CreateRow(1);
        //                //CellStyle
        //                ICellStyle headStyle = workbook.CreateCellStyle();
        //                headStyle.Alignment = HorizontalAlignment.Center;// 左右居中    
        //                headStyle.VerticalAlignment = VerticalAlignment.Center;// 上下居中 
        //                                                                       //定义font
        //                IFont font = workbook.CreateFont();
        //                font.FontHeightInPoints = 10;
        //                font.Boldweight = 700;
        //                headStyle.SetFont(font);

        //                foreach (DataColumn column in dt.Columns)
        //                {
        //                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
        //                    headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
        //                    sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
        //                }
        //            }
        //            #endregion
        //            if (columnHeader != "")
        //            {
        //                //header row
        //                IRow row0 = sheet.CreateRow(0);
        //                for (int i = 0; i < dt.Columns.Count; i++)
        //                {
        //                    ICell cell = row0.CreateCell(i, CellType.String);
        //                    cell.SetCellValue(dt.Columns[i].ColumnName);
        //                }
        //            }

        //            rowIndex = 2;
        //        }
        //        #endregion


        //        #region 内容
        //        var dataRow = sheet.CreateRow(rowIndex);
        //        foreach (DataColumn column in dt.Columns)
        //        {
        //            var newCell = dataRow.CreateCell(column.Ordinal);

        //            string drValue = row[column].ToString();

        //            switch (column.DataType.ToString())
        //            {
        //                case "System.String"://字符串类型
        //                    newCell.SetCellValue(drValue);
        //                    break;
        //                case "System.DateTime"://日期类型
        //                    DateTime dateV;
        //                    DateTime.TryParse(drValue, out dateV);
        //                    newCell.SetCellValue(dateV);

        //                    newCell.CellStyle = dateStyle;//格式化显示
        //                    break;
        //                case "System.Boolean"://布尔型
        //                    bool boolV = false;
        //                    bool.TryParse(drValue, out boolV);
        //                    newCell.SetCellValue(boolV);
        //                    break;
        //                case "System.Int16"://整型
        //                case "System.Int32":
        //                case "System.Int64":
        //                case "System.Byte":
        //                    int intV = 0;
        //                    int.TryParse(drValue, out intV);
        //                    newCell.SetCellValue(intV);
        //                    break;
        //                case "System.Decimal"://浮点型
        //                case "System.Double":
        //                    double doubV = 0;
        //                    double.TryParse(drValue, out doubV);
        //                    newCell.SetCellValue(doubV);
        //                    break;
        //                case "System.DBNull"://空值处理
        //                    newCell.SetCellValue("");
        //                    break;
        //                default:
        //                    newCell.SetCellValue("");
        //                    break;
        //            }

        //        }
        //        #endregion

        //        rowIndex++;
        //    }
        //    //自动列宽
        //    for (int i = 0; i <= dt.Columns.Count; i++)
        //        sheet.AutoSizeColumn(i, true);

        //    return workbook;
        //}
        //public static void createExcel(DataTable dt)
        //{ 
        //    var newBook = BuildWorkbook1(dt);
        //    using (var fs = File.OpenWrite(@"d:/joye.net1.xls"))
        //    {
        //        newBook.Write(fs);
        //        Console.WriteLine("生成成功");
        //    }
        //}

    }
}