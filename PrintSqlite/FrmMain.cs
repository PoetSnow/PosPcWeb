using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stimulsoft.Report;
using SufeiUtil;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrintSqlite
{
    public partial class FrmMain : Form
    {
        private static string hid = ConfigHelp.GetItem("Hid");  //酒店代码
        private static string mac = ConfigHelp.GetItem("Mac");  //Mac地址
        private static string secretKey = ConfigHelp.GetItem("SecretKey");  //秘钥
        private static string section = ConfigHelp.GetItem("Section");  //分区
        private static string interfaceUrl = ConfigHelp.GetItem("Interface");   //接口地址
        private static bool isEnvTest = Convert.ToBoolean(ConfigHelp.GetItem("isEnvTest"));     //是否是测试版本

        private static string hotelName = ConfigHelp.GetItem("HotelName");  //酒店名称

        private static int interval = 0;  //轮询请求间隔时间
        private static int selectIndex = 0;     //当前选中的打印机索引

        private static List<int> taskIds = new List<int>();   //当前线程列表
        private static List<CancellationTokenSource> taskCancelTokens = new List<CancellationTokenSource>();

        private static DataTable prints = CommonHelp.GetSQLitePrint();

        private object locking = new object();//用来加锁，保证更新所有记录时是单线程操作

        public FrmMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void PrintFrom_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text += "（" + hid + "_" + hotelName + "）";

                #region Mac地址不一致清空已有数据

                var macAddress = CommonHelp.GetMacAddress();
                if (string.IsNullOrWhiteSpace(mac))
                {
                    ConfigHelp.UpdateItem("Mac", macAddress);
                }

                if (!macAddress.Equals(mac, StringComparison.CurrentCultureIgnoreCase))
                {
                    ConfigHelp.UpdateItem("Hid", "");
                    ConfigHelp.UpdateItem("SecretKey", "");
                    ConfigHelp.UpdateItem("Section", "");
                    ConfigHelp.UpdateItem("HotelName", "");
                    ConfigHelp.UpdateItem("Mac", macAddress);

                    SQLiteHelper.ExecuteSql("DELETE FROM ProductRecord");
                    SQLiteHelper.ExecuteSql("DELETE FROM ProductPrinter");
                }

                #endregion Mac地址不一致清空已有数据

                var path = string.Format(@"{0}\format", Application.StartupPath);
                DirectoryInfo root = new DirectoryInfo(path);
                FileInfo[] files = root.GetFiles();
                foreach (var file in files)
                {
                    if (file.Extension == ".mrt")
                    {
                        ToolStripMenuItem menuItem = new ToolStripMenuItem();
                        menuItem.Name = "tsmi" + file.Name;
                        menuItem.Size = new System.Drawing.Size(124, 22);
                        menuItem.Text = file.Name.Replace(file.Extension, "");
                        menuItem.Click += new EventHandler(tsmiFormat_Click);
                        tmsiNoticeForm.DropDownItems.Add(menuItem);
                    }
                }

                if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(interfaceUrl))
                {
                    tsmiSetForm_Click(null, null);
                }
                else
                {

                    var updatework = new UpdateWork("",hid);
                    //检查是否有更新
                    if (updatework.UpdateVerList.Count > 0)
                    {
                        //启动更新程序
                        updatework.UpdateApp(hid);
                    }


                    tsmiRefresh_Click(null, null);
                    StartTasks();
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                getDataStatusLabel.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        /// <summary>
        /// 启动多线程任务
        /// </summary>
        private void StartTasks()
        {
            //foreach (var id in taskIds)
            //{
            //    oldTaskIds.Add(id); //把当前线程增加到需要结束的线程列表
            //}
            foreach (var token in taskCancelTokens)
            {
                token.Cancel();
            }

            var getToken = new CancellationTokenSource();
            Task taskGetList = Task.Factory.StartNew(() => TimeGetList(getToken.Token), getToken.Token);
            taskCancelTokens.Add(getToken);

            var printToken = new CancellationTokenSource();
            Task taskPrint = Task.Factory.StartNew(() => TimePrint(printToken.Token), printToken.Token);
            taskCancelTokens.Add(printToken);

            //增加最新的线程当前线程列表
            taskIds.Clear();
            taskIds.Add(taskGetList.Id);
            taskIds.Add(taskPrint.Id);
        }

        /// <summary>
        /// 获取接口出品数据
        /// </summary>
        /// <param name="dataPath"></param>
        private void GetProdPrintInSqlite(string dataPath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(hid) && !string.IsNullOrWhiteSpace(secretKey))
                {
                    interval = -1;
                    var start = DateTime.Now;

                    LogHelp.WriteLog("开始请求出品数据");

                    // 请求接口数据
                    var httpItem = new HttpItem();
                    httpItem.Method = "post";
                    httpItem.PostEncoding = Encoding.UTF8;
                    httpItem.URL = $"{interfaceUrl}/GetProdPrinterResult";
                    httpItem.ContentType = "application/x-www-form-urlencoded";
                    httpItem.Postdata = $"hid={hid}&secretKey={secretKey}&section={section}&isEnvTest={isEnvTest}";

                    var httpHelper = new HttpHelper();
                    var html = httpHelper.GetHtml(httpItem);
                    TimeSpan timeSpan = DateTime.Now - start;


                    LogHelp.WriteLog($"成功获得出品数据，耗时 {timeSpan.TotalMilliseconds} 毫秒");

                    if (html.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //接口默认返回格式：{"Success": true,"Data": [],"ErrorCode": 0}
                        var json = JObject.Parse(html.Html);
                        if (Convert.ToBoolean(json["Success"]))
                        {
                            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(json["Data"].ToString());
                            if (dataTable != null && dataTable.Rows.Count > 0)
                            {
                                //记录本次拉取到的数据
                                var requestID = "";
                                foreach (DataRow dr in dataTable.Rows)
                                {
                                    requestID += dr["流水号"].ToString() + "，";

                                }
                                LogHelp.WriteLog($"本次拉取数据：" + requestID);
                                var delIds = "";
                                var tableName = "ProductRecord";

                                DataTable dtColumn = SQLiteHelper.GetDataTable("PRAGMA table_info([" + tableName + "]);");
                                DataTable dsInsert = SQLiteHelper.GetDataTable("SELECT * FROM " + tableName + " ORDER BY SerialNumber DESC LIMIT 0;");
                                if (dtColumn != null && dtColumn.Rows.Count > 0)
                                {
                                    DataTable dtInsert = dsInsert.Clone();
                                    if (prints != null && prints.Rows.Count > 0)
                                    {
                                        foreach (DataRow dr in prints.Rows)
                                        {
                                            var list = dataTable.AsEnumerable().Where(w => w["出品端口"].ToString().Contains(dr["Port"].ToString())).Distinct().ToList();
                                            if (list != null && list.Count > 0)
                                            {
                                                foreach (DataRow row in list)
                                                {
                                                    #region 赋值

                                                    delIds += row["流水号"].ToString() + ",";
                                                    DataRow newDataRow = dtInsert.NewRow();

                                                    newDataRow["ProducePort"] = dr["Port"];
                                                    newDataRow["Printer"] = dr["Printer"];
                                                    newDataRow["PrinterName"] = dr["Name"];

                                                    newDataRow["HotelCode"] = row["酒店代码"];
                                                    newDataRow["OddNumbers"] = row["单号"];
                                                    newDataRow["ConsumptionID"] = row["消费Id"];
                                                    newDataRow["ConsumptionState"] = row["消费状态"];
                                                    newDataRow["BusinessPointCode"] = row["营业点代码"];
                                                    newDataRow["BusinessPointName"] = row["营业点名称"];
                                                    newDataRow["TableNumber"] = row["台号"];
                                                    newDataRow["TableName"] = row["台名"];
                                                    newDataRow["GuestName"] = row["客人姓名"];
                                                    newDataRow["NumberOfPeople"] = row["人数"];
                                                    newDataRow["OrderPeople"] = row["点菜人"];
                                                    newDataRow["OrderTime"] = Convert.ToDateTime(row["点菜时间"]).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                                    newDataRow["BanquetCode"] = row["酒席编码"];
                                                    newDataRow["BanquetName"] = row["酒席名称"];
                                                    newDataRow["BanquetNumber"] = row["酒席数量"];
                                                    newDataRow["BanquetUnit"] = row["酒席单位"];
                                                    newDataRow["BanquetPrice"] = row["酒席单价"];
                                                    newDataRow["BanquetAmount"] = row["酒席金额"];
                                                    newDataRow["ProjectCode"] = row["项目编码"];
                                                    newDataRow["ProjectName"] = row["项目名称"];
                                                    newDataRow["ProjectEnglishName"] = row["项目英文名"];
                                                    newDataRow["ProjectOtherName"] = row["项目其它名"];
                                                    newDataRow["Unit"] = row["单位"];
                                                    newDataRow["UnitEnglishName"] = row["单位英文名"];
                                                    newDataRow["Number"] = row["数量"];
                                                    newDataRow["Only"] = row["条只"];
                                                    newDataRow["Price"] = row["单价"];
                                                    newDataRow["Amount"] = row["金额"];
                                                    newDataRow["ProduceBarcode"] = row["出品条码"];
                                                    newDataRow["ProduceStatus"] = row["出品状态"];
                                                    newDataRow["ProduceNumber"] = row["出品次数"];
                                                    newDataRow["Practice"] = row["作法"].ToString().Replace("/", "\r\n");
                                                    newDataRow["Requirement"] = row["要求"];
                                                    newDataRow["GuestPosition"] = row["客位"];
                                                    newDataRow["Cook"] = row["厨师"];
                                                    newDataRow["Salesman"] = row["推销员"];
                                                    newDataRow["DepartmentCategory"] = row["部门类别"];
                                                    newDataRow["WineCardNumber"] = row["酒卡号"];
                                                    newDataRow["Remarks"] = row["备注"];
                                                    newDataRow["FlowNumber"] = row["流水号"];

                                                    newDataRow["PrintState"] = ProdPrinterStatus.等待打印;

                                                    dtInsert.Rows.Add(newDataRow);

                                                    #endregion 赋值
                                                }
                                            }
                                        }
                                        delIds = delIds.TrimEnd(',');
                                        var removeColumns = "SerialNumber,SendingTime,CompletionTime";
                                        var isSuccess = SQLiteHelper.BatchImportDataTable(tableName, removeColumns, dtInsert);
                                        if (isSuccess && !string.IsNullOrWhiteSpace(delIds))
                                        {
                                            LogHelp.WriteLog($"请求删除接口数据：" + delIds);
                                            #region 删除保存到 SQLite 的数据
                                            httpItem.URL = $"{interfaceUrl}/DelProducelist";
                                            httpItem.ContentType = "application/x-www-form-urlencoded";
                                            httpItem.Postdata = $"hid={hid}&secretKey={secretKey}&ids={delIds}&isEnvTest={isEnvTest}";
                                            httpHelper = new HttpHelper();
                                            var delHtml = httpHelper.GetHtml(httpItem);
                                            if (delHtml.StatusCode == System.Net.HttpStatusCode.OK)
                                            {
                                                //接口默认返回格式：{"Success": true,"Data": [],"ErrorCode": 0}
                                                var delJson = JObject.Parse(delHtml.Html);
                                                if (Convert.ToBoolean(delJson["Success"]))
                                                {
                                                    LogHelp.WriteLog($"删除{delIds.Trim(',').Split(',').Count()}条出品数据成功");
                                                }
                                                else
                                                {
                                                    LogHelp.WriteLog($"{delHtml.Html}，删除{delIds.Trim(',').Split(',').Count()}条出品数据失败：{delIds.Trim(',')}");
                                                }
                                            }
                                            else
                                            {
                                                if (delHtml.StatusCode == 0)
                                                {
                                                    LogHelp.WriteLog($"{delHtml.Html}，删除{delIds.Trim(',').Split(',').Count()}条出品数据失败：{delIds.Trim(',')}");
                                                }
                                                else
                                                {
                                                    LogHelp.WriteLog($"{delHtml.StatusDescription}，删除{delIds.Trim(',').Split(',').Count()}条出品数据失败：{delIds.Trim(',')}");
                                                }
                                            }

                                            #endregion 删除保存到 SQLite 的数据
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //oldTaskIds.Add(Task.CurrentId.Value);
                            getDataStatusLabel.Text = $"提示({DateTime.Now})：{json["Data"].ToString()}";
                            LogHelp.WriteLog(json["Data"].ToString());
                        }
                        timeSpan = DateTime.Now - start;
                        interval = timeSpan.Seconds;
                        LogHelp.WriteLog($"请求接口成功，耗时 {interval} 秒");
                    }
                    else
                    {
                        getDataStatusLabel.ForeColor = System.Drawing.Color.Red;
                        //oldTaskIds.Add(Task.CurrentId.Value);
                        if (html.StatusCode == 0)
                        {
                            getDataStatusLabel.Text = $"提示({DateTime.Now})：{html.Html}";
                            LogHelp.WriteLog(html.Html);
                        }
                        else
                        {
                            getDataStatusLabel.Text = $"提示({DateTime.Now})：{html.StatusDescription}";
                            LogHelp.WriteLog(html.StatusDescription);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                //oldTaskIds.Add(Task.CurrentId.Value);
                getDataStatusLabel.ForeColor = System.Drawing.Color.Red;
                getDataStatusLabel.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        /// <summary>
        /// 开始打印
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private void StartPrinting(object obj)
        {
            var print = (obj as PrintListModel).Print;
            var dtWait = (obj as PrintListModel).WaitDataTable;

            var documentName = "";
            var port = print["Port"].ToString();
            var printer = print["Printer"].ToString();

            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                if (!string.IsNullOrWhiteSpace(printer))
                {
                    if (dtWait != null && dtWait.Rows.Count > 0)
                    {
                        var isOne = Convert.ToBoolean(print["PrintOneByOne"]);
                        if (isOne)
                        {
                            documentName = port + dtWait.Rows[0]["FlowNumber"].ToString();
                        }
                        else
                        {
                            documentName = port + dtWait.Rows[0]["OddNumbers"].ToString();
                        }

                        var sql = "";
                        int resultCount = 0;
                        var path = string.Format(@"{0}\format\{1}.mrt", Application.StartupPath, print["ProduceFormat"].ToString());

                        try
                        {
                            StiReport format = new StiReport();
                            var cachepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReportCache");
                            if (!Directory.Exists(cachepath))
                            {
                                Directory.CreateDirectory(cachepath);
                            }
                            format.ReportCachePath = cachepath;                           
                            format.Load(path);  //加载打印格式文件
                            format.RegData("ProductRecord", dtWait);    //设置打印报表文件的数据源，Print为数据源名称
                            format.ReportName = documentName;
                            format.Render(false);

                            PrinterSettings printerSettings = new PrinterSettings();
                            printerSettings.PrintFileName = format.ReportName;
                            printerSettings.PrinterName = printer;
                            printerSettings.Copies = 1;

                            format.Print(false, printerSettings);


                            sql = string.Format("UPDATE ProductRecord SET CompletionTime = '{0}',PrintState = '{1}' WHERE DocumentName = '{2}' AND PrintState = '{3}' AND ProducePort = '{4}';", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ProdPrinterStatus.打印成功, documentName, ProdPrinterStatus.正在打印, port);
                            resultCount = SQLiteHelper.ExecuteSql(sql);

                            if (resultCount > 0)
                            {
                                if (isOne)
                                {
                                    LogHelp.WriteLog("打印成功(逐条)：" + dtWait.Rows[0]["Name"].ToString() + "；" + dtWait.Rows[0]["TableName"].ToString() + "；" + dtWait.Rows[0]["TableName"].ToString() + "；" + dtWait.Rows[0]["ProjectName"].ToString());
                                }
                                else
                                {
                                    LogHelp.WriteLog("打印成功(整单)：" + dtWait.Rows[0]["Name"].ToString() + "；" + dtWait.Rows[0]["TableName"].ToString());
                                }
                            }

                            SetStateIco();
                        }
                        catch (Exception ex)
                        {
                            sql = string.Format("UPDATE ProductRecord SET CompletionTime = '{0}',PrintState = '{1}' WHERE DocumentName = '{2}' AND PrintState = '{3}' AND ProducePort = '{4}';", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ProdPrinterStatus.打印失败, documentName, ProdPrinterStatus.正在打印, port);
                            resultCount = SQLiteHelper.ExecuteSql(sql);

                            if (resultCount > 0)
                            {
                                if (isOne)
                                {
                                    LogHelp.WriteLog("打印失败(逐条)：" + dtWait.Rows[0]["Name"].ToString() + "；" + dtWait.Rows[0]["TableName"].ToString() + "；" + dtWait.Rows[0]["ProjectName"].ToString());
                                    LogHelp.CreateLog(ex);
                                }
                                else
                                {
                                    LogHelp.WriteLog("打印失败(整单)：" + dtWait.Rows[0]["Name"].ToString() + "；" + dtWait.Rows[0]["TableName"].ToString());
                                    LogHelp.CreateLog(ex);
                                }
                            }

                            getDataStatusLabel.Text = $"提示({DateTime.Now})：{ex.Message}";
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                getDataStatusLabel.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                //oldTaskIds.Add(Task.CurrentId.Value);
            }
        }

        /// <summary>
        /// 轮询获取出品数据列表
        /// </summary>
        internal void TimeGetList(CancellationToken cancellationToken)
        {
            try
            {
                Thread.CurrentThread.IsBackground = true;
                LogHelp.WriteLog($"开始轮询({DateTime.Now})：{Task.CurrentId}");

                //轮询间隔时间
                var pollTime = Convert.ToInt32(ConfigHelp.GetItem("pollTime")) * 1000;
                string json = "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
                string log = Application.StartupPath + "\\data";
                string dataPath = log + json;

                while (true)
                {

                    cancellationToken.ThrowIfCancellationRequested();
                    getDataStatusLabel.ForeColor = System.Drawing.Color.Green;
                    getDataStatusLabel.Text = $"获取状态:{DateTime.Now}开始获取出品数据";

                    //if (oldTaskIds.Contains(Task.CurrentId.Value))
                    //{
                    //    LogHelp.WriteLog($"结束轮询({DateTime.Now})：{Task.CurrentId}");
                    //    oldTaskIds.Remove(Task.CurrentId.Value);
                    //    break;
                    //}

                    ////如果上次请求时间大于等于轮询间隔，跳过本次执行
                    //if (interval == -1 || interval >= pollTime)
                    //{
                    //    Thread.Sleep(pollTime);
                    //    continue;
                    //}

                    GetProdPrintInSqlite(dataPath);
                    Thread.Sleep(pollTime);
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                getDataStatusLabel.ForeColor = System.Drawing.Color.Red;
                getDataStatusLabel.Text = $"获取状态：({DateTime.Now})：{ex.Message}";
                //oldTaskIds.Add(Task.CurrentId.Value);
            }
        }

        /// <summary>
        /// 设置打印参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiSetForm_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["FrmPrint"] != null)
            {
                var form = (FrmPrint)Application.OpenForms["FrmPrint"];
                form.Show();
                form.Activate();
            }
            else
            {
                FrmPrint form = new FrmPrint();
                form.Show();
                form.Activate();
            }
        }

        /// <summary>
        /// 逐条打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiFormat_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["FrmNotice"] != null)
            {
                var form = (FrmNotice)Application.OpenForms["FrmNotice"];
                form.stiDesignerControl1.OpenFile(Application.StartupPath + "\\format\\" + ((ToolStripMenuItem)sender).Text + ".mrt");
                form.Show();
                form.Activate();
            }
            else
            {
                FrmNotice form = new FrmNotice();
                form.stiDesignerControl1.OpenFile(Application.StartupPath + "\\format\\" + ((ToolStripMenuItem)sender).Text + ".mrt");
                form.Show();
                form.Activate();
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)//当用户点击窗体右上角X按钮或(Alt + F4)时 发生
            {
                e.Cancel = true;
                Hide();
            }
        }

        /// <summary>
        /// 鼠标单击任务栏图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                cmsRightMenu.Show();
            }
            if (e.Button == MouseButtons.Left)
            {
                Show();
                Activate();
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiExit_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确认退出本程序吗？", "快点云Pos提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                //oldTaskIds.AddRange(taskIds);
                Application.ExitThread();
                Application.Exit();
                Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        /// 选择打印机列表时刷新出品数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvPrints_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ListView lv = (ListView)sender;
                if (lv.SelectedItems.Count > 0)
                {
                    selectIndex = lv.SelectedItems[0].Index;
                    var printName = lv.Items[selectIndex].ToolTipText;
                    RefreshWaitPrintList(printName);
                    selectIndex = lv.SelectedItems[0].Index;
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                getDataStatusLabel.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        /// <summary>
        /// 刷新等待打印的列表
        /// </summary>
        /// <param name="waitList">等待打印的列表数据</param>
        private void RefreshWaitPrintList(string printName)
        {
            try
            {
                var sql = string.Format("SELECT * FROM ProductRecord WHERE PrintState = '{0}' AND ProductRecord.Printer = '{1}';", ProdPrinterStatus.等待打印.ToString(), printName);
                var dtWait = SQLiteHelper.GetDataTable(sql);
                dgvPrintWait.DataSource = dtWait;
                dgvPrintWait.Columns["HotelCode"].Visible = false;
                dgvPrintWait.Columns["FlowNumber"].Visible = false;
                dgvPrintWait.Columns["OddNumbers"].Visible = false;
                dgvPrintWait.Columns["ConsumptionID"].Visible = false;
                dgvPrintWait.Columns["BusinessPointCode"].Visible = false;
                for (int i = 0; i < dgvPrintWait.Rows.Count; i++)
                {
                    dgvPrintWait.Rows[i].Cells["SerialNumber"].Value = i + 1;
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                getDataStatusLabel.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        /// <summary>
        /// 鼠标双击任务栏图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(interfaceUrl))
            {
                tsmiSetForm_Click(null, null);
            }
            else
            {
                Show();
                Activate();
            }
        }

        /// <summary>
        /// 显示主界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiShow_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(interfaceUrl))
            {
                tsmiSetForm_Click(null, null);
            }
            else
            {
                Show();
                Activate();
            }
        }

        /// <summary>
        /// 刷新打印机列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(hid) && !string.IsNullOrWhiteSpace(secretKey))
                {
                    #region 获取接口出品打印机列表

                    var httpItem = new HttpItem();
                    httpItem.Method = "post";
                    httpItem.PostEncoding = Encoding.UTF8;
                    httpItem.URL = $"{interfaceUrl}/GetProdPrinter";
                    httpItem.ContentType = "application/x-www-form-urlencoded";
                    httpItem.Postdata = $"hid={hid}&secretKey={secretKey}&section={section}&isEnvTest={isEnvTest}";
                    var httpHelper = new HttpHelper();
                    var html = httpHelper.GetHtml(httpItem);
                    if (html.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //接口默认返回格式：{"Success": true,"Data": [],"ErrorCode": 0}
                        var json = JObject.Parse(html.Html);
                        if (Convert.ToBoolean(json["Success"]))
                        {
                            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json["Data"].ToString());
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                lvPrints.Items.Clear();
                                for (var i = 0; i < dt.Rows.Count; i++)
                                {
                                    lvPrints.Items.Add(dt.Rows[i]["Code"].ToString());
                                    lvPrints.Items[i].Name = dt.Rows[i]["Code"].ToString();
                                    lvPrints.Items[i].Tag = dt.Rows[i]["Cname"].ToString();
                                    lvPrints.Items[i].Text = "（0）\r\n" + dt.Rows[i]["Cname"].ToString();
                                    lvPrints.Items[i].ToolTipText = "";
                                    lvPrints.Items[i].ImageIndex = 0;
                                }
                            }
                        }
                        else
                        {
                            getDataStatusLabel.Text = $"提示({DateTime.Now})：{json["Data"].ToString()}";
                            LogHelp.WriteLog(json["Data"].ToString());
                        }
                    }
                    else
                    {
                        if (html.StatusCode == 0)
                        {
                            getDataStatusLabel.Text = $"提示({DateTime.Now})：{html.Html}";
                            LogHelp.WriteLog(html.Html);
                        }
                        else
                        {
                            getDataStatusLabel.Text = $"提示({DateTime.Now})：{html.StatusDescription}";
                            LogHelp.WriteLog(html.StatusDescription);
                        }
                    }

                    #endregion 获取接口出品打印机列表
                }

                SetLocalPrinterConfig();    //设置本地配置

                SetStateIco();  //设置状态图标
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                getDataStatusLabel.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        /// <summary>
        /// 设置本地打印机配置
        /// </summary>
        private void SetLocalPrinterConfig()
        {
            var message = "";
            var list = CommonHelp.GetSQLitePrint();
            foreach (ListViewItem print in lvPrints.Items)
            {
                var temp = list.AsEnumerable().Where(w => w["Port"].ToString() == print.Name).FirstOrDefault();
                if (temp != null)
                {
                    var isExist = false;
                    //获取所有打印机名称
                    for (var i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
                    {
                        if (temp["Printer"].ToString() == PrinterSettings.InstalledPrinters[i])
                        {
                            isExist = true;
                            break;
                        }
                    }

                    if (isExist)
                    {
                        print.ToolTipText = temp["Printer"].ToString();
                    }
                    else
                    {
                        message += "〔" + print.Tag + "〕";
                        print.ToolTipText = "";
                        print.ImageIndex = 1;
                    }
                }
                else
                {
                    message += "〔" + print.Tag + "〕";
                    print.ImageIndex = 1;
                }
            }

            //设置当前选中的打印机
            if (lvPrints.Items != null && lvPrints.Items.Count > 0)
            {
                if (selectIndex < lvPrints.Items.Count)
                {
                    lvPrints.Items[selectIndex].Checked = true;
                }
                else
                {
                    selectIndex = 0;
                    lvPrints.Items[selectIndex].Checked = true;
                }
            }

            if (list == null || list.Rows.Count == 0)
            {
                foreach (ListViewItem print in lvPrints.Items)
                {
                    message += "〔" + print.Tag + "〕";
                    print.ImageIndex = 1;
                }
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                getDataStatusLabel.Text = $"提示({DateTime.Now})：{message}未设置打印机";
            }
        }

        /// <summary>
        /// 设置打印机状态图标
        /// </summary>
        private void SetStateIco()
        {
            Task.Factory.StartNew(() =>
            {
                #region 设置打印机状态图标

                lock (locking)
                {
                    Thread.CurrentThread.IsBackground = true;
                    foreach (ListViewItem lvi in lvPrints.Items)
                    {

                        if (string.IsNullOrWhiteSpace(lvi.ToolTipText))
                        {
                            lvi.ImageIndex = 1;
                        }
                        else
                        {
                            var properties = CommonHelp.GetPrintsProperties(lvi.ToolTipText);
                            var printerState = properties["PrinterState"].Value != null ? Enum.GetName(typeof(PrinterState), properties["PrinterState"].Value) : "";
                            var printerStatus = properties["PrinterStatus"].Value != null ? Enum.GetName(typeof(PrinterStatus), properties["PrinterStatus"].Value) : "";
                            var workOffline = Convert.ToBoolean(properties["WorkOffline"].Value.ToString());
                            if (!string.IsNullOrEmpty(printerState))
                            {
                                var state = (PrinterState)Enum.Parse(typeof(PrinterState), printerState);
                                if (state == PrinterState.卡纸 || state == PrinterState.没有碳粉 || state == PrinterState.输出箱满 || state == PrinterState.输出箱满 || state == PrinterState.门打开了 || state == PrinterState.需要用户干预 || state == PrinterState.已暂停 || state == PrinterState.错误)
                                {
                                    lvi.ImageIndex = 1;

                                    UpdateProdPrinter(lvi.ToolTipText, (byte)ProdPrinterIStatus.故障, Enum.GetName(typeof(PrinterState), state));
                                }
                                else if (state == PrinterState.离线 || state == PrinterState.未知服务 || state == PrinterState.无法使用 || workOffline)
                                {
                                    lvi.ImageIndex = 2;
                                    if (workOffline)
                                    {
                                        UpdateProdPrinter(lvi.ToolTipText, (byte)ProdPrinterIStatus.故障, "离线");
                                    }
                                    else
                                    {
                                        UpdateProdPrinter(lvi.ToolTipText, (byte)ProdPrinterIStatus.故障, Enum.GetName(typeof(PrinterState), state));
                                    }
                                }
                                else
                                {
                                    lvi.ImageIndex = 0;

                                    UpdateProdPrinter(lvi.ToolTipText, (byte)ProdPrinterIStatus.正常, "");
                                }
                            }
                            else if (!string.IsNullOrEmpty(printerStatus))
                            {
                                var status = (PrinterStatus)Enum.Parse(typeof(PrinterStatus), printerStatus);
                                if (status == PrinterStatus.停止打印 || status == PrinterStatus.其他)
                                {
                                    lvi.ImageIndex = 1;

                                    UpdateProdPrinter(lvi.ToolTipText, (byte)ProdPrinterIStatus.故障, Enum.GetName(typeof(PrinterStatus), status));
                                }
                                else if (status == PrinterStatus.离线 || status == PrinterStatus.未知 || workOffline)
                                {
                                    lvi.ImageIndex = 2;

                                    UpdateProdPrinter(lvi.ToolTipText, (byte)ProdPrinterIStatus.故障, Enum.GetName(typeof(PrinterStatus), status));
                                }
                                else
                                {
                                    lvi.ImageIndex = 0;

                                    UpdateProdPrinter(lvi.ToolTipText, (byte)ProdPrinterIStatus.正常, "");
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(lvi.ToolTipText))
                        {
                            var sql = string.Format("SELECT PrintOneByOne FROM ProductPrinter where Printer = '{0}'", lvi.ToolTipText);
                            var PrintOneByOne = SQLiteHelper.GetSingle(sql);

                            //逐条与非逐条
                            if (Convert.ToBoolean(PrintOneByOne))
                            {
                                sql = string.Format("SELECT count(*) FROM (SELECT FlowNumber FROM ProductRecord WHERE PrintState = '{0}' AND ProductRecord.Printer = '{1}' GROUP BY FlowNumber);", ProdPrinterStatus.等待打印.ToString(), lvi.ToolTipText);
                            }
                            else
                            {
                                sql = string.Format("SELECT count(*) FROM (SELECT OddNumbers FROM ProductRecord WHERE PrintState = '{0}' AND ProductRecord.Printer = '{1}' GROUP BY OddNumbers);", ProdPrinterStatus.等待打印.ToString(), lvi.ToolTipText);
                            }

                            var count = SQLiteHelper.GetSingle(sql) != null ? Convert.ToInt32(SQLiteHelper.GetSingle(sql)) : 0;
                            var text = "（" + count + "）\r\n" + lvi.Tag;

                            if (lvi.Text != text)
                            {
                                lvi.Text = text;
                            }
                        }
                    }
                }

                #endregion 设置打印机状态图标
            });
        }

        /// <summary>
        /// 更新出品打印机状态
        /// </summary>
        /// <param name="printer"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        private bool UpdateProdPrinter(string printer, byte status, string remark)
        {
            var sql = string.Format("SELECT * FROM ProductPrinter where Printer = '{0}';", printer);
            var dtPrinter = SQLiteHelper.GetDataTable(sql);

            if (dtPrinter != null && dtPrinter.Rows.Count > 0)
            {
                var count = 0;

                var httpItem = new HttpItem();
                httpItem.Method = "post";
                httpItem.PostEncoding = Encoding.UTF8;
                httpItem.URL = $"{interfaceUrl}/UpdateProdPrinter";
                httpItem.ContentType = "application/x-www-form-urlencoded";

                foreach (DataRow row in dtPrinter.Rows)
                {
                    var code = row["Port"].ToString();

                    httpItem.Postdata = string.Format("hid={0}&secretKey={1}&code={2}&status={3}&remark={4}&isEnvTest={5}", hid, secretKey, code, status, remark, isEnvTest);
                    var httpHelper = new HttpHelper();
                    var html = httpHelper.GetHtml(httpItem);
                    if (html.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //接口默认返回格式：{"Success": true,"Data": [],"ErrorCode": 0}
                        var json = JObject.Parse(html.Html);
                        if (Convert.ToBoolean(json["Success"]))
                        {
                            count++;
                        }
                    }
                }

                if (count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 刷新打印机列表
        /// </summary>
        internal void refreshPrintList()
        {
            try
            {
                prints = CommonHelp.GetSQLitePrint();
                tsmiRefresh_Click(tsmiRefresh, null);
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                getDataStatusLabel.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        /// <summary>
        /// 首次加载时如果没有填写酒店代码、秘钥、接口地址，弹窗设置参数窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromMain_Shown(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(interfaceUrl))
            {
                Hide();
            }
            if (notifyIcon.Visible == false)
            {
                notifyIcon.Visible = true;
            }
        }

        /// <summary>
        /// 刷新出品列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiRefreshList_Click(object sender, EventArgs e)
        {
            //foreach (var id in taskIds)
            //{
            //    oldTaskIds.Add(id); //把当前线程增加到需要结束的线程列表
            //}

            StartTasks();
        }

        /// <summary>
        /// 查询出品数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiQuery_Click(object sender, EventArgs e)
        {
            FrmQuery form = new FrmQuery();
            form.Show();
            form.Activate();
        }

        /// <summary>
        /// 打印测试页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiPrintTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvPrints.SelectedItems.Count > 0)
                {
                    //调用Windows默认打印测试页
                    //string path = @"Win32_Printer.DeviceId='" + lvPrints.SelectedItems[0].ToolTipText + "'";
                    //ManagementObject printer = new ManagementObject(path);
                    //var parameters = printer.GetMethodParameters("PrintTestPage");
                    //printer.InvokeMethod("PrintTestPage", parameters, null);

                    StiReport format = new StiReport();
                    var path = $@"{Application.StartupPath}\format\\测试页.mrt";
                    format.Load(path);  //加载打印格式文件
                    format.Compile();
                    format["Port"] = lvPrints.SelectedItems[0].Name;
                    format["PrintName"] = lvPrints.SelectedItems[0].Tag;
                    format.ReportName = "快点云Pos出品测试页";
                    format.Render(false);
                    PrinterSettings printerSettings = new PrinterSettings();
                    printerSettings.PrintFileName = format.ReportName;
                    printerSettings.PrinterName = lvPrints.SelectedItems[0].ToolTipText;
                    printerSettings.Copies = 1;
                    format.Print(false, printerSettings);
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                getDataStatusLabel.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        /// <summary>
        /// 暂停打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiPausePrinting_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvPrints.SelectedItems.Count > 0)
                {
                    string path = @"Win32_Printer.DeviceId='" + lvPrints.SelectedItems[0].ToolTipText + "'";
                    ManagementObject printer = new ManagementObject(path);
                    var parameters = printer.GetMethodParameters("Pause");
                    printer.InvokeMethod("Pause", parameters, null);
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                getDataStatusLabel.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        /// <summary>
        /// 恢复打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiRestorePrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvPrints.SelectedItems.Count > 0)
                {
                    string path = @"Win32_Printer.DeviceId='" + lvPrints.SelectedItems[0].ToolTipText + "'";
                    ManagementObject printer = new ManagementObject(path);
                    var parameters = printer.GetMethodParameters("Resume");
                    printer.InvokeMethod("Resume", parameters, null);
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                getDataStatusLabel.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        /// <summary>
        /// 取消打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiCancelPrinting_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvPrints.SelectedItems.Count > 0)
                {
                    string path = @"Win32_Printer.DeviceId='" + lvPrints.SelectedItems[0].ToolTipText + "'";
                    ManagementObject printer = new ManagementObject(path);
                    var parameters = printer.GetMethodParameters("CancelAllJobs");
                    printer.InvokeMethod("CancelAllJobs", parameters, null);
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                getDataStatusLabel.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        /// <summary>
        /// 定时打印数据
        /// </summary>
        private void TimePrint(CancellationToken cancellationToken)
        {
            try
            {
                Thread.CurrentThread.IsBackground = true;
                LogHelp.WriteLog($"开始定时打印({DateTime.Now})：{Task.CurrentId}");
                //打印间隔时间
                var printTime = Convert.ToInt32(ConfigHelp.GetItem("printTime")) * 1000;
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    printStatusLabel.ForeColor = System.Drawing.Color.Green;
                    printStatusLabel.Text = $"打印状态：({DateTime.Now})开始执行打印";

                    //if (oldTaskIds.Contains(Task.CurrentId.Value))
                    //{
                    //    LogHelp.WriteLog($"结束定时打印({DateTime.Now})：{Task.CurrentId}");
                    //    oldTaskIds.Remove(Task.CurrentId.Value);
                    //    break;
                    //}

                    //填充等待打印的数据到列表
                    //var jobs = CommonHelp.GetPrintJobsCollection();
                    //foreach (DataRow print in prints.Rows) //取打印机
                    if (prints != null && prints.Rows.Count > 0)
                    {
                        for (int i = 0; i < prints.Rows.Count; i++)
                        {
                            #region 打印等待打印的数据

                            var print = prints.Rows[i];
                            var port = print["Port"].ToString();    //打印机端口（A06）
                            var printer = print["Printer"].ToString();  //打印机名称

                            if (!string.IsNullOrWhiteSpace(printer))
                            {
                                var dtWait = new DataTable();
                                var printOneByOne = print["printOneByOne"] != null ? Convert.ToBoolean(print["printOneByOne"]) : true; //是否逐条打印

                                var sql = string.Format("SELECT * FROM ProductRecord INNER JOIN ProductPrinter ON ProductRecord.Printer = ProductPrinter.Printer WHERE ProductRecord.PrintState = '{0}' COLLATE NOCASE AND ProductRecord.Printer = '{1}' COLLATE NOCASE AND ProductPrinter.PrintOneByOne = '{2}' COLLATE NOCASE AND ProductRecord.ProducePort = '{3}' COLLATE NOCASE LIMIT 1;", ProdPrinterStatus.等待打印, printer, printOneByOne.ToString(), port);
                                dtWait = SQLiteHelper.GetDataTable(sql);

                                if (dtWait != null && dtWait.Rows.Count > 0)
                                {
                                    #region 获取打印机状态、任务队列

                                    var properties = CommonHelp.GetPrintsProperties(printer);
                                    var jobCount = properties["JobCountSinceLastReset"] != null ? Convert.ToInt32(properties["JobCountSinceLastReset"].Value) : 0;
                                    var printerState = properties["PrinterState"].Value != null ? Enum.GetName(typeof(PrinterState), properties["PrinterState"].Value) : "";
                                    var printerStatus = properties["PrinterStatus"].Value != null ? Enum.GetName(typeof(PrinterStatus), properties["PrinterStatus"].Value) : "";
                                    var state = PrinterState.离线;
                                    var status = PrinterStatus.离线;
                                    if (!string.IsNullOrEmpty(printerState))
                                    {
                                        state = (PrinterState)Enum.Parse(typeof(PrinterState), printerState);
                                    }
                                    else if (!string.IsNullOrEmpty(printerStatus))
                                    {
                                        status = (PrinterStatus)Enum.Parse(typeof(PrinterStatus), printerStatus);
                                    }

                                    #endregion 获取打印机状态、任务队列

                                    if (jobCount == 0 && (state == PrinterState.空闲 || status == PrinterStatus.空闲))
                                    {
                                        #region 获取逐条/整单等待打印的数据

                                        if (printOneByOne)  //逐条
                                        {
                                            var documentName = print["Port"].ToString() + dtWait.Rows[0]["FlowNumber"].ToString();
                                            sql = string.Format("UPDATE ProductRecord SET DocumentName = '{0}', SendingTime = '{1}', PrintState = '{2}' WHERE FlowNumber = '{3}' AND Printer = '{4}' AND PrintState = '{5}' AND ProducePort = '{6}' COLLATE NOCASE;", documentName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ProdPrinterStatus.正在打印.ToString(), dtWait.Rows[0]["FlowNumber"].ToString(), printer, ProdPrinterStatus.等待打印.ToString(), print["Port"].ToString());
                                            var count = SQLiteHelper.ExecuteSql(sql);

                                            if (count > 0)
                                            {
                                                LogHelp.WriteLog("开始打印(逐条)：" + dtWait.Rows[0]["Name"].ToString() + "；" + dtWait.Rows[0]["TableName"].ToString() + "；" + dtWait.Rows[0]["ProjectName"].ToString());

                                                //开始多线程打印
                                                Task.Factory.StartNew(() => StartPrinting(new PrintListModel { WaitDataTable = dtWait, Print = print }));
                                            }
                                        }
                                        else    //非逐条
                                        {
                                            sql = string.Format("SELECT * FROM ProductRecord INNER JOIN ProductPrinter ON ProductRecord.Printer = ProductPrinter.Printer WHERE ProductRecord.PrintState = '{0}' COLLATE NOCASE AND ProductRecord.Printer = '{1}' COLLATE NOCASE AND ProductRecord.OddNumbers = '{2}' COLLATE NOCASE AND ProductPrinter.PrintOneByOne = '{3}' COLLATE NOCASE AND ProductRecord.ProducePort = '{4}' COLLATE NOCASE; ", ProdPrinterStatus.等待打印.ToString(), printer, dtWait.Rows[0]["OddNumbers"], printOneByOne.ToString(), print["Port"].ToString());
                                            dtWait = SQLiteHelper.GetDataTable(sql);

                                            if (dtWait != null && dtWait.Rows.Count > 0)
                                            {
                                                var documentName = print["Port"].ToString() + dtWait.Rows[0]["OddNumbers"].ToString();
                                                sql = string.Format("UPDATE ProductRecord SET DocumentName = '{0}', SendingTime = '{1}', PrintState = '{2}' WHERE OddNumbers = '{3}' AND Printer = '{4}' AND PrintState = '{5}' AND ProducePort = '{6}' COLLATE NOCASE;", documentName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ProdPrinterStatus.正在打印.ToString(), dtWait.Rows[0]["OddNumbers"].ToString(), printer, ProdPrinterStatus.等待打印.ToString(), print["Port"].ToString());
                                                var count = SQLiteHelper.ExecuteSql(sql);

                                                if (count > 0)
                                                {
                                                    LogHelp.WriteLog("开始打印(整单)：" + dtWait.Rows[0]["Name"].ToString() + "；" + dtWait.Rows[0]["TableName"].ToString());

                                                    //开始多线程打印
                                                    Task.Factory.StartNew(() => StartPrinting(new PrintListModel { WaitDataTable = dtWait, Print = print }));
                                                }
                                            }
                                        }

                                        #endregion 获取逐条/整单等待打印的数据

                                    }
                                }
                            }

                            #endregion 打印等待打印的数据
                        }
                    }

                    Thread.Sleep(printTime);
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                //oldTaskIds.Add(Task.CurrentId.Value);
                printStatusLabel.ForeColor = System.Drawing.Color.Red;
                printStatusLabel.Text = $"打印状态：({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        private void FrmMain_Activated(object sender, EventArgs e)
        {
            if (notifyIcon.Visible == false)
            {
                notifyIcon.Visible = true;
            }
        }

        private void tsmiReboot_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
            Application.Exit();
            Application.Restart();
            Process.GetCurrentProcess().Kill();
        }

        ////获取客户标识
        ///// <summary>
        ///// 获取客户标识
        ///// </summary>
        ///// <param name="Hid"></param>
        ///// <param name="InterfaceUrl"></param>
        ///// <param name="secretKey"></param>
        ///// <param name="isEnvTest"></param>
        ///// <returns>返回客户标识，为空字符串不更新程序</returns>
        //private string GetAppServerAddress(string Hid, string InterfaceUrl, string secretKey, string isEnvTest)
        //{
        //    var _serveraddress = "";  //1  vip1  ，2 --- vip2
        //    //查询酒店标识
        //    var httpItem = new HttpItem();
        //    httpItem.Method = "post";
        //    httpItem.URL = $"{InterfaceUrl.Trim()}/GetServerAddRess";
        //    httpItem.ContentType = "application/x-www-form-urlencoded";
        //    httpItem.Postdata = $"hid={Hid}&secretKey={secretKey}&isEnvTest={isEnvTest}";
        //    var httpHelper = new HttpHelper();
        //    var html = httpHelper.GetHtml(httpItem);
        //    if (html.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        //接口默认返回格式：{"Success": true,"Data": [],"ErrorCode": 0}
        //        var json = JObject.Parse(html.Html);
        //        if (Convert.ToBoolean(json["Success"]))
        //        {
        //            _serveraddress = json["Data"].ToString();
        //        }
        //        else
        //        {
                   
        //            LogHelp.WriteLog(json["Data"].ToString());
        //        }
        //    }
        //    else
        //    {
        //        if (html.StatusCode == 0)
        //        {                    
        //            LogHelp.WriteLog(html.Html);
        //        }
        //        else
        //        {                 
        //            LogHelp.WriteLog(html.StatusDescription);
        //        }
        //    }
        //    return _serveraddress;
        //}


    }
}