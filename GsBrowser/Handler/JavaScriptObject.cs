using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stimulsoft.Report;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GsBrowser
{
    /// <summary>
    /// JavaScript 对象
    /// </summary>
    public class JavaScriptObject
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="browser">需要注册的 JavaScript 对象的浏览器</param>
        public JavaScriptObject(ChromiumWebBrowser browser)
        {
            CefSharpSettings.LegacyJavascriptBindingEnabled = true; //启用传统的 JavaScript 绑定，调用C#函数必须启用
            browser.RegisterJsObject("jsObject", this, new BindingOptions { CamelCaseJavascriptNames = false });  //注册 JavaScript 对象

            HotelInfoJson = new JObject();
            CurrentInfo = new JObject();
        }

        #region 内部属性
        /// <summary>
        /// 集团代码
        /// </summary>
        internal string Gid { get; set; }

        /// <summary>
        /// 酒店代码
        /// </summary>
        internal string Hid { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        internal string Password { get; set; }

        /// <summary>
        /// 跳转的链接
        /// </summary>
        internal string Url { get; set; }

        /// <summary>
        /// 当前登录的酒店返回的JObject对象
        /// </summary>
        internal static JObject HotelInfoJson { get; set; }

        internal static JObject CurrentInfo { get; set; }

        /// <summary>
        /// 酒店名
        /// </summary>
        public string HotelName { get; set; }
        #endregion

        #region 公共属性
        /// <summary>
        /// 返回的链接
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// 当前登录用户信息
        /// </summary>
        public string Current { get; set; }
        /// <summary>
        /// 是否启用屏幕键盘
        /// </summary>
        public bool EnableKeyboard { get; set; }
        /// <summary>
        /// 计算机名称
        /// </summary>
        public string ComputerName { get; set; }

        /// <summary>
        /// 取消项目是否本地打印取消单
        /// </summary>
        public string isCanItemPrint { get; set; }

        /// <summary>
        /// 是否启用手写
        /// </summary>
        public bool IsHandwrite { get; set; }
        #endregion

        #region JavaScript 可直接调用的函数
        /// <summary>
        /// 测试函数，JavaScript 调用方式：if (jsObject) { jsObject.Hello(); }
        /// </summary>
        public void Hello()
        {
            MessageBox.Show("您好！欢迎使用本程序！");
        }

        /// <summary>
        /// 打印报表
        /// </summary>
        /// <param name="reportName">报表文件名</param>
        /// <param name="DataSourceName">数据源名称</param>
        /// <param name="data">数据源</param>
        /// <param name="showPrintDialog">是否显示打印对话框</param>
        public void PrintReport(string reportName, string dataSourceName, string data, bool showPrintDialog = false, bool isSupplement = false)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                var printer = ConfigHelp.GetItem("Printer");
                var dt = JsonConvert.DeserializeObject<DataTable>(data);

                if (string.IsNullOrWhiteSpace(printer))
                {
                    MessageBox.Show("请按 Ctrl + F11 设置打印机");
                }

                try
                {
                    var orders = dt.AsEnumerable().Where(w => Convert.ToInt32(w["计费状态"]) < 50);
                    if (orders != null)
                    {
                        while (true)
                        {
                            int count = 0;
                            var jobs = CommonHelp.GetPrintJobsCollection();
                            foreach (var job in jobs)     //循环打印队列
                            {
                                if (job.Properties["Name"].Value.ToString().IndexOf(printer) > -1)
                                {
                                    count += 1;
                                }
                            }

                            var properties = CommonHelp.GetPrintsProperties(printer);
                            var jobCount = properties["JobCountSinceLastReset"] != null ? Convert.ToInt32(properties["JobCountSinceLastReset"].Value) : 0;

                            if (count == 0 && jobCount == 0)
                            {
                                #region 打印点菜单
                                var dataTable = orders.CopyToDataTable();
                                if (dataTable != null && dataTable.Rows.Count > 0)
                                {
                                    StiReport report = new StiReport();
                                    var path = string.Format(Application.StartupPath + $@"\report\{reportName}.mrt");
                                    report.Load(path);  //加载打印格式文件
                                    report.Compile();
                                    report["UserName"] = UserName;
                                    report["IsOrder"] = "点菜单";
                                    report["Explain"] = isSupplement ? "(补打)" : "";
                                    report.RegData(dataSourceName, dataTable);    //设置打印报表文件的数据源，Print为数据源名称
                                    report.ReportName = DateTime.Now.ToString("yyyyMMddHHmmssms");
                                    report.Render(false);

                                    PrinterSettings printerSettings = new PrinterSettings();
                                    printerSettings.PrintFileName = report.ReportName;
                                    printerSettings.Copies = 1;
                                    if (string.IsNullOrEmpty(printer))
                                    {
                                        PrintDocument document = new PrintDocument();
                                        printerSettings.PrinterName = document.PrinterSettings.PrinterName;
                                    }
                                    else
                                    {
                                        printerSettings.PrinterName = printer;
                                    }

                                    report.Print(showPrintDialog, printerSettings);
                                }
                                #endregion
                                break;  //结束循环
                            }

                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelp.CreateLog(ex);
                    //throw;
                }

                try
                {
                    if (isCanItemPrint == "1")
                    {
                        #region 打印取消单
                        var cancels = dt.AsEnumerable().Where(w => Convert.ToInt32(w["计费状态"]) > 50);
                        if (cancels != null)
                        {
                            while (true)
                            {
                                int count = 0;
                                var jobs = CommonHelp.GetPrintJobsCollection();
                                foreach (var job in jobs)     //循环打印队列
                                {
                                    if (job.Properties["Name"].Value.ToString().IndexOf(printer) > -1)
                                    {
                                        count += 1;
                                    }
                                }

                                var properties = CommonHelp.GetPrintsProperties(printer);
                                var jobCount = properties["JobCountSinceLastReset"] != null ? Convert.ToInt32(properties["JobCountSinceLastReset"].Value) : 0;

                                if (count == 0 && jobCount == 0)
                                {
                                    var dataTable = cancels.CopyToDataTable();
                                    if (dataTable != null && dataTable.Rows.Count > 0)
                                    {
                                        StiReport report = new StiReport();
                                        var path = string.Format(Application.StartupPath + $@"\report\{reportName}.mrt");
                                        report.Load(path);  //加载打印格式文件
                                        report.Compile();
                                        report["UserName"] = UserName;
                                        report["IsOrder"] = "取消单";
                                        report["Explain"] = isSupplement ? "(补打)" : "";
                                        report.RegData(dataSourceName, dataTable);    //设置打印报表文件的数据源，Print为数据源名称
                                        report.ReportName = DateTime.Now.ToString("yyyyMMddHHmmssms");
                                        report.Render(false);

                                        PrinterSettings printerSettings = new PrinterSettings();
                                        printerSettings.PrintFileName = report.ReportName;
                                        printerSettings.Copies = 1;
                                        if (string.IsNullOrEmpty(printer))
                                        {
                                            PrintDocument document = new PrintDocument();
                                            printerSettings.PrinterName = document.PrinterSettings.PrinterName;
                                        }
                                        else
                                        {
                                            printerSettings.PrinterName = printer;
                                        }

                                        report.Print(showPrintDialog, printerSettings);
                                    }
                                    break;  //结束循环
                                }
                                Thread.Sleep(1000);
                            }

                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    LogHelp.CreateLog(ex);
                    //throw;
                }
            });
        }


        /// <summary>
        /// 打印转菜报表
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="dataSourceName"></param>
        /// <param name="data"></param>
        /// <param name="showPrintDialog"></param>
        /// <param name="isSupplement"></param>
        public void PrintReportByChangeItem(string reportName, string dataSourceName, string data, bool showPrintDialog = false, bool isSupplement = false)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                var printer = ConfigHelp.GetItem("Printer");

                var dt = JsonConvert.DeserializeObject<DataTable>(data);
                try
                {
                    while (true)
                    {
                        int count = 0;
                        var jobs = CommonHelp.GetPrintJobsCollection();
                        foreach (var job in jobs)     //循环打印队列
                        {
                            if (job.Properties["Name"].Value.ToString().IndexOf(printer) > -1)
                            {
                                count += 1;
                            }
                        }

                        var properties = CommonHelp.GetPrintsProperties(printer);
                        var jobCount = properties["JobCountSinceLastReset"] != null ? Convert.ToInt32(properties["JobCountSinceLastReset"].Value) : 0;

                        if (count == 0 && jobCount == 0)
                        {
                            #region 打印转菜单
                            var orders = dt.AsEnumerable().CopyToDataTable();
                            if (orders != null && orders.Rows.Count > 0)
                            {
                                StiReport report = new StiReport();
                                var path = string.Format(Application.StartupPath + $@"\report\{reportName}.mrt");
                                report.Load(path);  //加载打印格式文件
                                report.Compile();


                                report["原台号"] = orders.Rows[0]["原餐台"].ToString();
                                report["新台号"] = orders.Rows[0]["新餐台"].ToString();
                                report["转菜人"] = orders.Rows[0]["转菜人"].ToString();
                                var s = orders.Rows[0]["转菜时间"];
                                report["转菜时间"] = Convert.ToDateTime(orders.Rows[0]["转菜时间"]);
                                report.RegData(dataSourceName, orders);    //设置打印报表文件的数据源，Print为数据源名称
                                report.ReportName = DateTime.Now.ToString("yyyyMMddHHmmssms");
                                report.Render(false);

                                PrinterSettings printerSettings = new PrinterSettings();
                                printerSettings.PrintFileName = report.ReportName;
                                printerSettings.Copies = 1;
                                if (string.IsNullOrEmpty(printer))
                                {
                                    PrintDocument document = new PrintDocument();
                                    printerSettings.PrinterName = document.PrinterSettings.PrinterName;
                                }
                                else
                                {
                                    printerSettings.PrinterName = printer;
                                }

                                report.Print(showPrintDialog, printerSettings);
                            }
                            #endregion
                            break;  //结束循环
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    LogHelp.CreateLog(ex);
                    //throw;
                }
            });
        }


        /// <summary>
        /// 打印换台报表
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="dataSourceName"></param>
        /// <param name="data"></param>
        /// <param name="showPrintDialog"></param>
        /// <param name="isSupplement"></param>
        public void PrintReportByChangeTab(string reportName, string dataSourceName, string data, bool showPrintDialog = false, bool isSupplement = false)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                var printer = ConfigHelp.GetItem("Printer");

                var dt = JsonConvert.DeserializeObject<DataTable>(data);
                try
                {
                    while (true)
                    {
                        int count = 0;
                        var jobs = CommonHelp.GetPrintJobsCollection();
                        foreach (var job in jobs)     //循环打印队列
                        {
                            if (job.Properties["Name"].Value.ToString().IndexOf(printer) > -1)
                            {
                                count += 1;
                            }
                        }

                        var properties = CommonHelp.GetPrintsProperties(printer);
                        var jobCount = properties["JobCountSinceLastReset"] != null ? Convert.ToInt32(properties["JobCountSinceLastReset"].Value) : 0;

                        if (count == 0 && jobCount == 0)
                        {
                            #region 打印转台单
                            var orders = dt.AsEnumerable().CopyToDataTable();
                            if (orders != null && orders.Rows.Count > 0)
                            {
                                StiReport report = new StiReport();
                                var path = string.Format(Application.StartupPath + $@"\report\{reportName}.mrt");
                                report.Load(path);  //加载打印格式文件
                                report.Compile();


                                report["原餐台"] = orders.Rows[0]["原餐台"].ToString();
                                report["新餐台"] = orders.Rows[0]["新餐台"].ToString();
                                report["转台人"] = orders.Rows[0]["转台人"].ToString();
                                var s = orders.Rows[0]["转台时间"];
                                report["转台时间"] = Convert.ToDateTime(orders.Rows[0]["转台时间"]);
                                report.RegData(dataSourceName, orders);    //设置打印报表文件的数据源，Print为数据源名称
                                report.ReportName = DateTime.Now.ToString("yyyyMMddHHmmssms");
                                report.Render(false);

                                PrinterSettings printerSettings = new PrinterSettings();
                                printerSettings.PrintFileName = report.ReportName;
                                printerSettings.Copies = 1;
                                if (string.IsNullOrEmpty(printer))
                                {
                                    PrintDocument document = new PrintDocument();
                                    printerSettings.PrinterName = document.PrinterSettings.PrinterName;
                                }
                                else
                                {
                                    printerSettings.PrinterName = printer;
                                }

                                report.Print(showPrintDialog, printerSettings);
                            }
                            #endregion
                            break;  //结束循环
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    LogHelp.CreateLog(ex);
                    //throw;
                }
            });
        }

        /// <summary>
        /// 买单成功打印账单
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="dataSourceName"></param>
        /// <param name="data"></param>
        /// <param name="showPrintDialog"></param>
        /// <param name="isSupplement"></param>
        public void PrintReportByPaySusses(string reportName, string dataSourceName, string data, bool showPrintDialog = false, bool isSupplement = false)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                var printer = ConfigHelp.GetItem("Printer");

                var dt = JsonConvert.DeserializeObject<DataTable>(data);
                try
                {
                    while (true)
                    {
                        int count = 0;
                        var jobs = CommonHelp.GetPrintJobsCollection();
                        foreach (var job in jobs)     //循环打印队列
                        {
                            if (job.Properties["Name"].Value.ToString().IndexOf(printer) > -1)
                            {
                                count += 1;
                            }
                        }

                        var properties = CommonHelp.GetPrintsProperties(printer);
                        var jobCount = properties["JobCountSinceLastReset"] != null ? Convert.ToInt32(properties["JobCountSinceLastReset"].Value) : 0;

                        if (count == 0 && jobCount == 0)
                        {
                            #region 打印账单
                            var orders = dt.AsEnumerable().CopyToDataTable();
                            if (orders != null && orders.Rows.Count > 0)
                            {
                                StiReport report = new StiReport();
                                var path = string.Format(Application.StartupPath + $@"\report\{reportName}.mrt");
                                report.Load(path);  //加载打印格式文件
                                report.Compile();

                                report["UserName"] = UserName;
                                report["HotelName"] = HotelName;
                                report.RegData(dataSourceName, orders);    //设置打印报表文件的数据源，Print为数据源名称
                                report.ReportName = DateTime.Now.ToString("yyyyMMddHHmmssms");
                                report.Render(false);

                                PrinterSettings printerSettings = new PrinterSettings();
                                printerSettings.PrintFileName = report.ReportName;
                                printerSettings.Copies = 1;
                                if (string.IsNullOrEmpty(printer))
                                {
                                    PrintDocument document = new PrintDocument();
                                    printerSettings.PrinterName = document.PrinterSettings.PrinterName;
                                }
                                else
                                {
                                    printerSettings.PrinterName = printer;
                                }

                                report.Print(showPrintDialog, printerSettings);
                            }
                            #endregion
                            break;  //结束循环
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    LogHelp.CreateLog(ex);
                    //throw;
                }
            });
        }

        /// <summary>
        /// 打开屏幕键盘
        /// </summary>
        public void ScreenKeyboard()
        {
            try
            {
                string windir = Environment.GetEnvironmentVariable("WINDIR");
                var osk = Path.Combine(Path.Combine(windir, "system32"), "osk.exe");
                if (!File.Exists(osk))
                {
                    if (Environment.Is64BitOperatingSystem || Environment.Is64BitProcess)
                    {
                        osk = Application.StartupPath + @"\keyboard\OSKLauncher64.exe";
                    }
                    else
                    {
                        osk = Application.StartupPath + @"\keyboard\OSKLauncher.exe";
                    }
                }
                Process.Start(osk);
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
            }
        }

        /// <summary>
        /// 关闭屏幕键盘
        /// </summary>
        public void ScreenKeyboardClose()
        {
            try
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process p in processes)
                {
                    if (p.ProcessName.Equals("osk", StringComparison.CurrentCultureIgnoreCase) || p.ProcessName.Equals("OSKLauncher", StringComparison.CurrentCultureIgnoreCase) || p.ProcessName.Equals("OSKLauncher64", StringComparison.CurrentCultureIgnoreCase))
                    {
                        p.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
            }
        }

        /// <summary>
        /// 打开手写板
        /// </summary>
        /// <param name="id">窗体</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        public void OpenHandwrite(string id, int x = 100, int y = 100)
        {
            try
            {
                if (Application.OpenForms["FormBrowser"] == null)
                {
                    FormBrowser form = new FormBrowser();
                    if (form.InvokeRequired)
                    {
                        form.Invoke(new MethodInvoker(delegate { OpenHandwrite(id, x, y); }));
                        return;
                    }

                    //存在手写板直接显示，不存在则创建
                    if (form.browser.Controls["handwrite1"] != null)
                    {
                        form.browser.Controls["handwrite1"].Show();
                        form.Id = id;
                    }
                    else
                    {
                        Handwrite.Handwrite handwrite1 = new Handwrite.Handwrite();
                        handwrite1.Location = new System.Drawing.Point(x, y);    //窗体的起始位置
                        handwrite1.Size = new System.Drawing.Size(501, 350);
                        handwrite1.SelectTextEvent += new Handwrite.Handwrite.SelectTextDelegate(form.handwrite1_SelectTextEvent);
                        handwrite1.CloseEvent += new Handwrite.Handwrite.CloseDelegate(form.handwrite1_CloseEvent);
                        handwrite1.DeleteInput += new Handwrite.Handwrite.DeleteInputDelegate(form.handwrite1_DeleteEvent);
                        handwrite1.Name = "handwrite1";
                        form.Id = id;
                        form.browser.Controls.Add(handwrite1);
                    }

                }
                else
                {
                    var form = (FormBrowser)Application.OpenForms["FormBrowser"];
                    if (form.InvokeRequired)
                    {
                        form.Invoke(new MethodInvoker(delegate { OpenHandwrite(id, x, y); }));
                        return;
                    }

                    //存在手写板直接显示，不存在则创建
                    if (form.browser.Controls["handwrite1"] != null)
                    {
                        form.browser.Controls["handwrite1"].Show();
                        form.Id = id;
                    }
                    else
                    {
                        Handwrite.Handwrite handwrite1 = new Handwrite.Handwrite();
                        handwrite1.Location = new System.Drawing.Point(x, y);    //窗体的起始位置
                        handwrite1.Size = new System.Drawing.Size(501, 350);
                        handwrite1.SelectTextEvent += new Handwrite.Handwrite.SelectTextDelegate(form.handwrite1_SelectTextEvent);
                        handwrite1.CloseEvent += new Handwrite.Handwrite.CloseDelegate(form.handwrite1_CloseEvent);
                        handwrite1.DeleteInput += new Handwrite.Handwrite.DeleteInputDelegate(form.handwrite1_DeleteEvent);
                        handwrite1.Name = "handwrite1";
                        form.Id = id;
                        form.browser.Controls.Add(handwrite1);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelp.WriteLog(ex.Message.ToString());
               // LogHelp.CreateLog(ex);
                // throw ex;
            }

        }

        /// <summary>
        /// 设置本地参数
        /// </summary>
        public void SetLocalParameters()
        {
            if (Application.OpenForms["FormBrowser"] != null)
            {
                Application.OpenForms["FormBrowser"].Activate();
                //模拟按下Ctrl
                CommonHelp.keybd_event(CommonHelp.KeyControl, 0, 0, 0);
                //模拟按下F11
                CommonHelp.keybd_event(CommonHelp.KeyF11, 0, 0, 0);
                //松开按键Ctrl
                CommonHelp.keybd_event(CommonHelp.KeyControl, 0, 2, 0);
                //松开按键F11
                CommonHelp.keybd_event(CommonHelp.KeyF11, 0, 2, 0);
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logContent"></param>
        public void WriteLog(string logContent)
        {
            LogHelp.WriteLog(logContent);
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        public void Exit()
        {
            //try
            //{
            //    Application.ExitThread();
            //    Application.Exit();
            //    Process.GetCurrentProcess().Kill();
            //}
            //catch (Exception ex)
            //{
            //    LogHelp.CreateLog(ex);
            //    //throw;
            //}
            
            Process current = Process.GetCurrentProcess();
            ProcessThreadCollection allThreads = current.Threads;
            current.CloseMainWindow();
        }


        #endregion
    }
}
