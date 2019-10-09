using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Management;
using System.Windows.Forms;

namespace PrintSqlite
{
    /// <summary>
    /// 公共帮助类
    /// </summary>
    public static class CommonHelp
    {
        /// <summary>
        /// 绑定DataGridView数据到DataTable
        /// </summary>
        /// <param name="dgv">复制数据的DataGridView</param>
        /// <returns>返回的绑定数据后的DataTable</returns>
        public static DataTable GetDgvToTable(DataGridView dgv)
        {
            DataTable dt = new DataTable();
            // 列强制转换
            for (int count = 0; count < dgv.Columns.Count; count++)
            {
                DataColumn dc = new DataColumn(dgv.Columns[count].Name.ToString());
                dt.Columns.Add(dc);
            }
            // 循环行
            for (int count = 0; count < dgv.Rows.Count; count++)
            {
                DataRow dr = dt.NewRow();
                for (int countsub = 0; countsub < dgv.Columns.Count; countsub++)
                {
                    dr[countsub] = Convert.ToString(dgv.Rows[count].Cells[countsub].Value);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>  
        /// 按字符串长度切分成数组  
        /// </summary>  
        /// <param name="str">原字符串</param>  
        /// <param name="separatorCharNum">切分长度</param>  
        /// <returns>字符串数组</returns>  
        public static string[] SplitByLen(this string str, int separatorCharNum)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= separatorCharNum)
            {
                return new string[] { str };
            }
            string tempStr = str;
            List<string> strList = new List<string>();
            int iMax = Convert.ToInt32(Math.Ceiling(str.Length / (separatorCharNum * 1.0)));//获取循环次数  
            for (int i = 1; i <= iMax; i++)
            {
                string currMsg = tempStr.Substring(0, tempStr.Length > separatorCharNum ? separatorCharNum : tempStr.Length);
                strList.Add(currMsg);
                if (tempStr.Length > separatorCharNum)
                {
                    tempStr = tempStr.Substring(separatorCharNum, tempStr.Length - separatorCharNum);
                }
            }
            return strList.ToArray();
        }

        /// <summary>
        /// 获取本地打印机列表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetPrintDataTable()
        {
            DataTable dt = new DataTable();
            string data = Application.StartupPath + "\\data";
            string dataPath = data + "\\PrinterList.json";

            if (!Directory.Exists(data))
            {
                Directory.CreateDirectory(data);  //创建文件夹
                File.CreateText(dataPath).Dispose();  //创建文件
            }
            else
            {
                if (!File.Exists(dataPath))
                {
                    File.Create(dataPath).Dispose();  //创建文件
                }
                else
                {
                    var dataBase = File.ReadAllText(dataPath);  //读取文件
                    if (dataBase.IndexOf(":") > 0)
                    {
                        dt = JsonConvert.DeserializeObject<DataTable>(dataBase);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 获取本地打印机列表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetSQLitePrint()
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = string.Format("SELECT * FROM ProductPrinter;");
                dt = SQLiteHelper.GetDataTable(sql);
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                
                return null;
            }
            return dt;
        }

        /// <summary>
        /// 获取打印机任务队列
        /// </summary>
        /// <param name="printerName">打印机名称</param>
        /// <returns></returns>
        public static ManagementObjectCollection GetPrintJobsCollection()
        {
            try
            {
                string searchQuery = "SELECT * FROM Win32_PrintJob";
                ManagementObjectSearcher searchPrintJobs = new ManagementObjectSearcher(searchQuery);
                var jobs = searchPrintJobs.Get();

                //foreach (var job in jobs)     //循环打印队列
                //{
                //    string message = "";
                //    foreach (var property in job.Properties)    //循环队列属性
                //    {
                //        message += "\r\n" + property.Name + "（" + property.Type + "）：" + property.Value + "；";
                //    }
                //    LogHelp.CreateLog(new Exception() { Source = message });
                //}

                return jobs;
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                
                return null;
            }
        }

        /// <summary>
        /// 获取打印机状态
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        public static PropertyDataCollection GetPrintsProperties(string printerName)
        {
            try
            {
                string path = @"Win32_Printer.DeviceId='" + printerName + "'";
                ManagementObject printer = new ManagementObject(path);
                printer.Get();
                //string message = "打印机：" + printerName;
                //foreach (var property in printer.Properties)    //循环打印机属性
                //{
                //    message += property.Name + "（" + property.Type + "）：" + property.Value + "；";
                //}
                //LogHelp.CreateLog(new Exception() { Source = message });

                return printer.Properties;
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                
                return null;
            }
        }

        /// <summary>  
        /// 获取本机MAC地址  
        /// </summary>  
        /// <returns>本机MAC地址</returns>  
        public static string GetMacAddress()
        {
            try
            {
                string strMac = string.Empty;
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        strMac = mo["MacAddress"].ToString();
                    }
                }
                moc = null;
                mc = null;
                return strMac;
            }
            catch
            {
                return "unknown";
            }
        }
    }
}
