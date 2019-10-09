using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.ChargeFreeManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Extensions;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Gemstar.BSPMS.Hotel.Web.Models;
using Gemstar.BSPMS.Common.Services.Enums;
using System.IO;
using System.Web;
using Gemstar.BSPMS.Common.Tools;
using System.Linq;
using System.Data;
using NPOI.SS.UserModel;
using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 导入EXCEL资料
    /// </summary>
    [AuthPage("99120")]
    [AuthPage(ProductType.Member, "m99055")]
    [AuthPage(ProductType.Pos, "p99055")]
    public class ImportExcelController : BaseController
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            ViewBag.ExcelFileList = GetDownLoadFiles();
            ViewBag.DelList = GetDelList();
            return View();
        }
        #endregion

        #region 上传文件
        /// <summary>
        /// 保存EXCEL文件
        /// </summary>
        /// <param name="files">文件列表</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult Save(IEnumerable<HttpPostedFileBase> files, bool isDeleteOldData)
        {
            string message = "";
            try
            {
                bool isExcel = CheckFile(files, out message);
                if (isExcel)
                {
                    HandleFile(files, isDeleteOldData, out message);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Content(message);
        }

        /// <summary>
        /// 验证文件格式
        /// </summary>
        /// <param name="files">文件列表</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        private bool CheckFile(IEnumerable<HttpPostedFileBase> files, out string message)
        {
            message = "";
            if (files != null)
            {
                int count = files.Count();
                if (count <= 0)
                {
                    message = EnumExtension.GetDescription(ImportStatus.PlaseSelectFile);
                    return false;
                }
                if (count != 1)
                {
                    //控制只能选一个EXCEL文件
                    message = EnumExtension.GetDescription(ImportStatus.OnlySelectOneFile);
                    return false;
                }

                foreach (var file in files)
                {
                    if (file != null)
                    {
                        var extension = FileHelper.GetFileExtension(file.FileName, file.InputStream);
                        if (extension != FileExtension.XLS && extension != FileExtension.XLSX)
                        {
                            message = EnumExtension.GetDescription(ImportStatus.NotExcelFile);
                            break;
                        }
                    }
                    else
                    {
                        message = EnumExtension.GetDescription(ImportStatus.PlaseSelectFile);
                    }
                }
            }
            else
            {
                message = EnumExtension.GetDescription(ImportStatus.PlaseSelectFile);
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 处理文件
        /// </summary>
        /// <param name="files">文件列表</param>
        /// <param name="message">消息</param>
        private void HandleFile(IEnumerable<HttpPostedFileBase> files, bool isDeleteOldData, out string message)
        {
            message = "";

            var importTableList = GetService<IImportExcelService>().GetList();
            if (importTableList == null || importTableList.Count <= 0)
            {
                message = EnumExtension.GetDescription(ImportStatus.NotFunction);
                return;
            }

            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        IWorkbook workbook = null;
                        try
                        {
                            workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(file.InputStream);
                        }
                        catch
                        {
                            workbook = new NPOI.HSSF.UserModel.HSSFWorkbook(file.InputStream);
                        }
                        if (workbook != null && workbook.NumberOfSheets > 0)
                        {
                            if (workbook.NumberOfSheets != 1)
                            {
                                //控制在一个EXCEL文件中，只能有一个工作表
                                message = EnumExtension.GetDescription(ImportStatus.OnlyHasOneExcelTable);
                                if (workbook != null)
                                {
                                    workbook.Close();
                                }
                                return;
                            }
                            for (int i = 0; i < workbook.NumberOfSheets; i++)
                            {
                                HandleSheet(workbook.GetSheetAt(i), importTableList, isDeleteOldData, out message);
                            }
                        }
                        else
                        {
                            message = EnumExtension.GetDescription(ImportStatus.PlaseSelectFile);
                        }
                        if (workbook != null)
                        {
                            workbook.Close();
                        }
                    }
                    else
                    {
                        message = EnumExtension.GetDescription(ImportStatus.PlaseSelectFile);
                    }
                }
            }
            else
            {
                message = EnumExtension.GetDescription(ImportStatus.PlaseSelectFile);
            }
        }

        /// <summary>
        /// 处理单个文件
        /// </summary>
        /// <param name="sheet">表格文件</param>
        /// <param name="importTableList">可导入列表</param>
        private void HandleSheet(ISheet sheet, List<KeyValuePairModel<string, string>> importTableList, bool isDeleteOldData, out string message)
        {
            message = "";
            if (sheet == null)
            {
                message = EnumExtension.GetDescription(ImportStatus.PlaseSelectFile);
                return;
            }

            if (string.IsNullOrWhiteSpace(sheet.SheetName))
            {
                message = EnumExtension.GetDescription(ImportStatus.SheetNameIsNull);
                return;
            }

            if (sheet == null || sheet.LastRowNum < 1)
            {
                message = EnumExtension.GetDescription(ImportStatus.SheetNotValue);
                return;
            }

            //key表名，value存储过程名称
            var tableInfo = importTableList.Where(c => c.Key == sheet.SheetName).FirstOrDefault();
            if (tableInfo == null)
            {
                message = EnumExtension.GetDescription(ImportStatus.NotFunction) + "\n工作表名：" + sheet.SheetName;
                return;
            }

            string connectionString = MvcApplication.GetHotelDbConnStr();
            //表结构
            DataTable table = ADOHelper.ExecSql(string.Format("select top 0 * from {0}", tableInfo.Key), connectionString, null, true);
            if (table == null || table.Columns.Count <= 0)
            {
                message = EnumExtension.GetDescription(ImportStatus.NotFunction) + "\n工作表名：" + tableInfo.Key;
                return;
            }

            //excel标题行
            var titleRow = sheet.GetRow(0);
            if (titleRow == null || titleRow.Cells == null || titleRow.Cells.Count <= 0)
            {
                message = EnumExtension.GetDescription(ImportStatus.NotTitleRow);
                return;
            }

            if ((titleRow.Cells.Count + 4) != table.Columns.Count)
            {
                message = EnumExtension.GetDescription(ImportStatus.NotEqualColumnCount) + "\n正确的列数量：" + table.Columns.Count;
                return;
            }

            //key标题名，value 在excel表格中的位置第几列
            List<KeyValuePairModel<string, int>> sheetColumns = new List<KeyValuePairModel<string, int>>();
            foreach (DataColumn tableColumn in table.Columns)
            {
                if (tableColumn.ColumnName == "id" || tableColumn.ColumnName == "checkResult" || tableColumn.ColumnName == "hid" || tableColumn.ColumnName == "batch")
                {
                    continue;
                }
                foreach (var item in titleRow)
                {
                    if (tableColumn.ColumnName.ToLower() == item.StringCellValue.ToLower())
                    {
                        sheetColumns.Add(new KeyValuePairModel<string, int>(item.StringCellValue.ToLower(), item.ColumnIndex));
                        break;
                    }
                }
            }
            if ((sheetColumns.Count + 4) != table.Columns.Count)
            {
                message = EnumExtension.GetDescription(ImportStatus.ErrorColumnName);
                return;
            }

            //酒店ID，批次ID
            string hid = CurrentInfo.HotelId;
            Guid batchId = Guid.NewGuid();
            int rowCount = GetValidRowCount(sheet) - 1;




            int columCount = table.Columns.Count;
            for (int i = 1; i <= rowCount; i++)
            {
                DataRow row = table.NewRow();
                for (int j = 0; j < columCount; j++)
                {
                    object value = null;
                    #region 获取Value
                    string columnName = table.Columns[j].ColumnName.ToLower();
                    if (columnName == "id")
                    {
                        value = Guid.NewGuid().ToString().ToUpper();
                    }
                    else if (columnName == "checkresult")
                    {
                        value = null;
                    }
                    else if (columnName == "hid")
                    {
                        value = hid;
                    }
                    else if (columnName == "batch")
                    {
                        value = batchId.ToString().ToUpper();
                    }
                    else
                    {
                        var entity = sheetColumns.Where(c => c.Key == columnName).FirstOrDefault();
                        var cell = sheet.GetRow(i).GetCell(entity.Value);
                        try
                        {
                            switch (table.Columns[j].DataType.FullName)
                            {
                                case "System.Boolean":
                                    //布尔类型兼容处理
                                    if (cell.CellType == CellType.Numeric)
                                    {
                                        if (cell.NumericCellValue == 1)
                                            value = true;
                                        else if (cell.NumericCellValue == 0)
                                            value = false;
                                        else
                                            throw new Exception("布尔值请填写 0 或者 1 ");
                                    }
                                    else if (cell.CellType == CellType.Boolean)
                                    {
                                        value = cell.BooleanCellValue;
                                    }
                                    else if (cell.CellType == CellType.String)
                                    {
                                        if (cell.StringCellValue == "1")
                                            value = true;
                                        else if (cell.StringCellValue == "0")
                                            value = false;
                                        else
                                            throw new Exception("布尔值请填写 0 或者 1 ");
                                    }
                                    else
                                    {
                                        throw new Exception("单元格格式错误");
                                    }
                                    break;

                                case "System.DateTime":
                                    if (cell.DateCellValue == DateTime.MinValue || cell.DateCellValue == DateTime.MaxValue)
                                    {
                                        value = null;
                                    }
                                    else
                                    {
                                        value = cell.DateCellValue;
                                    }
                                    break;

                                case "System.Char":
                                case "System.String":
                                    value = string.IsNullOrWhiteSpace(cell.StringCellValue) ? null : cell.StringCellValue.Trim();
                                    break;

                                case "System.Byte":
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.SByte":
                                case "System.Single":
                                case "System.Decimal":
                                case "System.Double":
                                case "System.TimeSpan":
                                case "System.UInt16":
                                case "System.UInt32":
                                case "System.UInt64":
                                    value = cell.NumericCellValue;
                                    break;

                                default:
                                    value = string.IsNullOrWhiteSpace(cell.StringCellValue) ? null : cell.StringCellValue.Trim();
                                    break;
                            }
                        }
                        catch
                        {
                            value = (cell == null ? null : (string.IsNullOrWhiteSpace(cell.ToString()) ? null : cell.ToString().Trim()));
                        }
                    }
                    #endregion

                    if ((value == null || value == DBNull.Value) && (!table.Columns[j].AllowDBNull))
                    {
                        message = string.Format("\"{0}\"{1}", columnName, EnumExtension.GetDescription(ImportStatus.NotNullColumn));
                        return;
                    }
                    row[table.Columns[j].ColumnName] = (value == null ? DBNull.Value : value);
                }
                table.Rows.Add(row);
            }
            //执行
            if (table.Rows.Count > 0)
            {
                ADOHelper.BulkInsert(connectionString, tableInfo.Key, table);
                table.Dispose();
                var result = GetService<IImportExcelService>().Save(tableInfo.Value, CurrentInfo, batchId, isDeleteOldData);
                if (!result.Success)
                {
                    if (result.Data.ToLower() == batchId.ToString().ToLower())
                    {
                        message = string.Format("验证失败！<a target=\"_blank\" href=\"{0}?sheetName={1}&batch={2}\">详情...<a/>", Url.Action("Detail"), Server.UrlEncode(tableInfo.Key), result.Data);
                    }
                    else
                    {
                        message = result.Data;
                    }
                    return;
                }
            }

        }
        #endregion

        #region 删除文件
        [AuthButton(AuthFlag.Add)]
        public ActionResult Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"
            if (fileNames != null)
            {

            }
            // Return an empty string to signify success
            return Content("");
        }
        #endregion

        #region 状态消息
        public enum ImportStatus
        {
            /// <summary>
            /// 请选择要上传的EXCEL文件！
            /// </summary>
            [Description("请选择要上传的EXCEL文件！")]
            PlaseSelectFile,
            /// <summary>
            /// 只能选择一个EXCEL文件上传！
            /// </summary>
            [Description("只能选择一个EXCEL文件上传！")]
            OnlySelectOneFile,
            /// <summary>
            /// 此文件不是EXCEL格式的文件！
            /// </summary>
            [Description("此文件不是EXCEL格式的文件！")]
            NotExcelFile,
            /// <summary>
            /// 没有对应的功能来处理上传的EXCEL文件！
            /// </summary>
            [Description("没有对应的功能来处理上传的EXCEL文件！")]
            NotFunction,
            /// <summary>
            /// EXCEL文件中只能有一个工作表，请删除其他工作表！
            /// </summary>
            [Description("EXCEL文件中只能有一个工作表，请删除其他工作表！")]
            OnlyHasOneExcelTable,
            /// <summary>
            /// 工作表的名字不能为空！
            /// </summary>
            [Description("工作表的名字不能为空！")]
            SheetNameIsNull,
            /// <summary>
            /// 没有数据！
            /// </summary>
            [Description("没有数据！")]
            SheetNotValue,
            /// <summary>
            /// 没有标题行！
            /// </summary>
            [Description("没有标题行！")]
            NotTitleRow,
            /// <summary>
            /// 列数量不正确！
            /// </summary>
            [Description("列数量不正确！")]
            NotEqualColumnCount,
            /// <summary>
            /// 列名不正确！
            /// </summary>
            [Description("列名不正确！")]
            ErrorColumnName,
            /// <summary>
            /// 列的值不允许为空！
            /// </summary>
            [Description("列的值不允许为空！")]
            NotNullColumn,
        }
        #endregion

        #region excel模板
        /// <summary>
        /// 获取导入文件列表
        /// </summary>
        private List<string> GetDownLoadFiles()
        {
            List<string> fileList = new List<string>();
            try
            {
                var path = Server.MapPath("~/Content/Template/");
                var list = Directory.GetFiles(path, "导入快点*模板.xlsx", SearchOption.TopDirectoryOnly);
                if (list != null && list.Length > 0)
                {
                    var count = list.Length;
                    for (int i = 0; i < count; i++)
                    {
                        list[i] = list[i].Replace(path, "");
                    }
                    fileList = list.ToList();
                }
            }
            catch { }
            return fileList;
        }
        /// <summary>
        /// 下载导入文件
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult DownLoadFile(string fileName)
        {
            var list = GetDownLoadFiles();
            if (list != null && list.Contains(fileName))
            {
                var path = Server.MapPath("~/Content/Template/");
                return File((path + fileName), "application/octet-stream", Url.Encode(fileName));
            }
            return Content("文件不存在！");
        }
        #endregion

        #region 显示详细
        [AuthButton(AuthFlag.Query)]
        public ActionResult Detail(string sheetName, string batch)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sheetName) || string.IsNullOrWhiteSpace(batch))
                {
                    return Content("参数错误！");
                }
                var importTableList = GetService<IImportExcelService>().GetList();//key表名,value存储过程名
                if (importTableList == null || importTableList.Count <= 0)
                {
                    return Content(EnumExtension.GetDescription(ImportStatus.NotFunction));
                }
                //key表名，value存储过程名称
                var tableInfo = importTableList.Where(c => c.Key == sheetName).FirstOrDefault();
                if (tableInfo == null)
                {
                    return Content(EnumExtension.GetDescription(ImportStatus.NotFunction) + "\n工作表名：" + sheetName);
                }

                string sql = string.Format("select * from {0} where hid = '{1}' and batch = '{2}'", tableInfo.Key, CurrentInfo.HotelId, batch);
                var result = ADOHelper.ExecSql(sql, MvcApplication.GetHotelDbConnStr());
                if (result != null && result.Columns != null && result.Columns.Count > 0)
                {
                    if (result.Columns.Contains("id"))
                    {
                        result.Columns.Remove("id");
                    }
                    if (result.Columns.Contains("hid"))
                    {
                        result.Columns.Remove("hid");
                    }
                    if (result.Columns.Contains("batch"))
                    {
                        result.Columns.Remove("batch");
                    }
                    if (result.Columns.Contains("checkResult"))
                    {
                        result.Columns["checkResult"].SetOrdinal(0);
                        result.Columns["checkResult"].ColumnName = "检查结果";
                    }
                }
                byte[] resultByte = ExcelHelper.DataTableToExcel(result, tableInfo.Key);
                result.Dispose();
                return File(resultByte, "application/octet-stream", Url.Encode(tableInfo.Key + "资料模板-检查结果.xlsx"));
            }
            catch (Exception ex)
            {

            }
            return View();
        }
        #endregion

        #region 删除当天导入
        /// <summary>
        /// 获取可用删除的数据类别
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePairModel<string, string>> GetDelList()
        {
            var importTableList = GetService<IImportExcelService>().GetList();
            foreach (var item in importTableList)
            {
                item.Value = item.Value.Substring(("up_pos_import导入").Length);
            }
            importTableList.Insert(0, new KeyValuePairModel<string, string> { Key = "", Value = "请选择" });
            return importTableList;
        }
        /// <summary>
        /// 删除当天导入的数据
        /// </summary>
        /// <param name="id">数据ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Delete)]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(JsonResultData.Failure("请选择要删除的项目！"), JsonRequestBehavior.DenyGet);
            }
            var importTableList = GetService<IImportExcelService>().GetList();
            if (importTableList == null || importTableList.Count <= 0)
            {
                return Json(JsonResultData.Failure("要删除的项目不存在！"), JsonRequestBehavior.DenyGet);
            }
            var entity = importTableList.Where(c => c.Key == id).FirstOrDefault();
            if (entity == null || string.IsNullOrWhiteSpace(entity.Value))
            {
                return Json(JsonResultData.Failure("要删除的项目不存在！"), JsonRequestBehavior.DenyGet);
            }
            var result = GetService<IImportExcelService>().Delete(entity.Value, CurrentInfo);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 获取Excel有效行数

        /// <summary>
        /// 本行无内容，认为是最后一行的后一行
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public int GetValidRowCount(ISheet sheet)
        {
            var lastrow = sheet.LastRowNum;

            for (int i = 0; i < lastrow; i++)
            {
                var row = sheet.GetRow(i);

                var cells = row.Cells;
                var ishavevalue = false;
                for (int j = 0; j < cells.Count; j++)
                {
                    var value = (cells[j] == null ? null : (string.IsNullOrWhiteSpace(cells[j].ToString()) ? null : cells[j].ToString().Trim()));
                    if (!string.IsNullOrEmpty(value))
                    {
                        ishavevalue = true;
                        break;
                    }
                }

                if (!ishavevalue)
                {
                    return i;
                }
            }

            return lastrow;


        }





        #endregion


    }
}