using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stimulsoft.Report;
using SufeiUtil;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PrintSqlite
{
    public partial class FrmPrint : Form
    {
        public FrmPrint()
        {
            InitializeComponent();
        }
        private void PrintForm_Load(object sender, EventArgs e)
        {
            try
            {
                initConfig();
                if (string.IsNullOrWhiteSpace(txtHid.Text))
                {
                    lblHidTip.Visible = true;
                }
                if (string.IsNullOrWhiteSpace(txtSecretKey.Text))
                {
                    lblSecretKeyTip.Visible = true;
                }
                if (string.IsNullOrWhiteSpace(txtInterface.Text))
                {
                    lblInterfaceTip.Visible = true;
                }
                var hid = txtHid.Text.Trim();
                var secretKey = txtSecretKey.Text.Trim();
                var section = txtSection.Text.Trim();
                var isEnvTest = ConfigHelp.GetItem("isEnvTest");

                if (!string.IsNullOrWhiteSpace(hid) && !string.IsNullOrWhiteSpace(secretKey))
                {

                    //根据酒店ID获取客户类型
                    var updatework = new UpdateWork("",hid);
                    //检查是否有更新
                    if (updatework.UpdateVerList.Count > 0)
                    {
                        //启动更新程序
                        updatework.UpdateApp( hid);
                    }

                    GetPrinterListByInterface(hid, secretKey, section, isEnvTest);
                }

                GetPrinterListByLocal();

            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                tsslTips.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        /// <summary>
        /// 初始化基础配置文件，如果配置项被删除，则自动添加被删除的配置项，并赋默认值
        /// </summary>
        private void initConfig()
        {
            try
            {
                //酒店Id
                if (ConfigHelp.IsExist("Hid"))
                {
                    txtHid.Text = ConfigHelp.GetItem("Hid");
                }
                else
                {
                    ConfigHelp.AddItem("Hid", "");
                }
                //秘钥
                if (ConfigHelp.IsExist("SecretKey"))
                {
                    txtSecretKey.Text = ConfigHelp.GetItem("SecretKey");
                }
                else
                {
                    ConfigHelp.AddItem("SecretKey", "");
                }
                //分区
                if (ConfigHelp.IsExist("Section"))
                {
                    txtSection.Text = ConfigHelp.GetItem("Section");
                }
                else
                {
                    ConfigHelp.AddItem("Section", "");
                }
                //出品打印接口地址
                if (ConfigHelp.IsExist("Interface"))
                {
                    txtInterface.Text = ConfigHelp.GetItem("Interface");
                }
                else
                {
                    ConfigHelp.AddItem("Interface", "http://pmsnotify.gshis.com/ProducePrint");
                }
                //轮询间隔时间
                if (ConfigHelp.IsExist("pollTime"))
                {
                    txtPollTime.Text = ConfigHelp.GetItem("pollTime");
                }
                else
                {
                    ConfigHelp.AddItem("pollTime", "5");
                }
                //轮询间隔时间
                if (ConfigHelp.IsExist("printTime"))
                {
                    txtPrintTime.Text = ConfigHelp.GetItem("printTime");
                }
                else
                {
                    ConfigHelp.AddItem("printTime", "3");
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                tsslTips.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");

            }
        }

        /// <summary>
        /// 获取本地打印机列表
        /// </summary>
        private void GetPrinterListByLocal()
        {
            //获取本地保存的格式文件
            var path = Application.StartupPath + "\\format";
            var info = new DirectoryInfo(path);
            var files = info.GetFiles("*.mrt");

            var prints = CommonHelp.GetSQLitePrint();
            if (prints != null && prints.Rows.Count > 0)
            {
                dgvPrint.Rows.Clear();
                for (int i = 0; i < prints.Rows.Count; i++)
                {
                    //添加列
                    dgvPrint.Rows.Add((i + 1).ToString());
                    ((DataGridViewTextBoxCell)dgvPrint.Rows[i].Cells["Id"]).Value = prints.Rows[i]["Id"];
                    ((DataGridViewTextBoxCell)dgvPrint.Rows[i].Cells["端口"]).Value = prints.Rows[i]["Port"];
                    ((DataGridViewTextBoxCell)dgvPrint.Rows[i].Cells["名称"]).Value = prints.Rows[i]["Name"];
                    ((DataGridViewTextBoxCell)dgvPrint.Rows[i].Cells["备用端口"]).Value = prints.Rows[i]["StandbyPort"].ToString();
                    ((DataGridViewTextBoxCell)dgvPrint.Rows[i].Cells["关联端口"]).Value = prints.Rows[i]["AssociatedPort"].ToString();
                    ((DataGridViewCheckBoxCell)dgvPrint.Rows[i].Cells["逐条打印"]).Value = prints.Rows[i]["PrintOneByOne"].ToString();

                    //读取打印机列表到 ComboBox
                    ((DataGridViewComboBoxCell)dgvPrint.Rows[i].Cells["打印机"]).Items.Add("");
                    for (var j = 0; j < PrinterSettings.InstalledPrinters.Count; j++)
                    {
                        ((DataGridViewComboBoxCell)dgvPrint.Rows[i].Cells["打印机"]).Items.Add(PrinterSettings.InstalledPrinters[j]);
                    }

                    //读取出品格式列表到 ComboBox
                    foreach (var file in files)
                    {
                        if (file.Name.IndexOf("测试") == -1)
                        {
                            ((DataGridViewComboBoxCell)dgvPrint.Rows[i].Cells["出品格式"]).Items.Add(file.Name.Replace(file.Extension, ""));
                        }
                    }

                    //设置本地存储的端口、打印机对应关系
                    foreach (DataRow temp in prints.Rows)
                    {
                        if (prints.Rows[i]["Id"].ToString() == temp["Id"].ToString())
                        {
                            //根据本地配置给打印机下拉框赋值
                            if (((DataGridViewComboBoxCell)dgvPrint.Rows[i].Cells["打印机"]).Items.IndexOf(temp["Printer"]) > -1)
                            {
                                ((DataGridViewComboBoxCell)dgvPrint.Rows[i].Cells["打印机"]).Value = temp["Printer"];
                            }

                            //根据本地配置给出品格式下拉框赋值
                            if (((DataGridViewComboBoxCell)dgvPrint.Rows[i].Cells["出品格式"]).Items.IndexOf(temp["ProduceFormat"]) > -1)
                            {
                                ((DataGridViewComboBoxCell)dgvPrint.Rows[i].Cells["出品格式"]).Value = temp["ProduceFormat"];
                            }
                        }
                    }

                    //如果没有设置出品格式，默认选择第一个
                    var format = ((DataGridViewComboBoxCell)dgvPrint.Rows[i].Cells["出品格式"]).Value ?? "";
                    if (string.IsNullOrEmpty(format.ToString()) && files.Length > 0)
                    {
                        var temp = files.Where(w => !w.Name.Contains("测试")).ToList();
                        if (temp != null && temp.Count > 0 && temp[0].Name.IndexOf("测试") == -1)
                        {
                            ((DataGridViewComboBoxCell)dgvPrint.Rows[i].Cells["出品格式"]).Value = temp[0].Name.Replace(files[0].Extension, "");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取接口打印机列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="secretKey">秘钥</param>
        /// <param name="section">分区</param>
        /// <param name="isEnvTest">是否测试版</param>
        private void GetPrinterListByInterface(string hid, string secretKey, string section, string isEnvTest)
        {
            //发送请求
            var httpItem = new HttpItem();
            httpItem.Method = "post";
            httpItem.URL = $"{txtInterface.Text.Trim()}/GetProdPrinter";
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
                    //加载列表数据到 DataGridView
                    PrintDocument print = new PrintDocument();
                    string sDefault = print.PrinterSettings.PrinterName;//默认打印机名
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(json["Data"].ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            var id = dt.Rows[i]["Id"];
                            var port = dt.Rows[i]["Code"];
                            var name = dt.Rows[i]["Cname"];
                            var printOneByOne = dt.Rows[i]["IsTabeachbreak"];
                            var standbyPort = dt.Rows[i]["Section1"].ToString() + dt.Rows[i]["Comno1"].ToString();
                            var associatedPort = dt.Rows[i]["Section2"].ToString() + dt.Rows[i]["Comno2"].ToString();

                            var sql = string.Format("SELECT * FROM ProductPrinter WHERE Port = '{0}'", dt.Rows[i]["Code"]);
                            var exists = SQLiteHelper.IsExists(sql);
                            if (exists)
                            {
                                var update = string.Format("UPDATE ProductPrinter SET Port = '{0}', Name = '{1}', StandbyPort = '{2}', AssociatedPort = '{3}', PrintOneByOne = '{4}' WHERE Id = '{5}'", port, name, standbyPort, associatedPort, printOneByOne, id);

                                SQLiteHelper.ExecuteSql(update);
                            }
                            else
                            {
                                var insert = string.Format("INSERT INTO ProductPrinter (Id,Port,Name,StandbyPort,AssociatedPort,PrintOneByOne)VALUES('{0}','{1}','{2}','{3}','{4}','{5}');", id, port, name, standbyPort, associatedPort, printOneByOne);

                                SQLiteHelper.ExecuteSql(insert);
                            }
                        }
                    }
                }
                else
                {
                    tsslTips.Text = $"提示({DateTime.Now})：{json["Data"].ToString()}";
                    LogHelp.WriteLog(json["Data"].ToString());
                }
            }
            else
            {
                if (html.StatusCode == 0)
                {
                    tsslTips.Text = $"提示({DateTime.Now})：{html.Html}";
                    LogHelp.WriteLog(html.Html);
                }
                else
                {
                    tsslTips.Text = $"提示({DateTime.Now})：{html.StatusDescription}";
                    LogHelp.WriteLog(html.StatusDescription);
                }
            }
        }

        //保存设置并刷新端口列表
        private void btnSaveSet_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtHid.Text))
                {
                    lblHidTip.Visible = true;
                }
                else if (string.IsNullOrWhiteSpace(txtSecretKey.Text))
                {
                    lblHidTip.Visible = true;
                }
                else if (string.IsNullOrWhiteSpace(txtInterface.Text))
                {
                    lblHidTip.Visible = true;
                }
                else
                {
                    var hid = txtHid.Text.Trim();
                    var secretKey = txtSecretKey.Text.Trim();
                    var section = txtSection.Text.Trim();
                    var isEnvTest = ConfigHelp.GetItem("isEnvTest");

                    ConfigHelp.UpdateItem("Hid", hid);
                    ConfigHelp.UpdateItem("SecretKey", secretKey);
                    ConfigHelp.UpdateItem("Section", txtSection.Text.Trim());
                    ConfigHelp.UpdateItem("Interface", txtInterface.Text.Trim());

                    #region 获取酒店名称

                    var httpItem = new HttpItem();
                    httpItem.Method = "post";
                    httpItem.PostEncoding = Encoding.UTF8;
                    httpItem.URL = $"{txtInterface.Text.Trim()}/GetHotelInfo";
                    httpItem.ContentType = "application/x-www-form-urlencoded";
                    httpItem.Postdata = $"hid={hid}&secretKey={secretKey}&isEnvTest={isEnvTest}";
                    var httpHelper = new HttpHelper();
                    var html = httpHelper.GetHtml(httpItem);
                    if (html.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //接口默认返回格式：{"Success": true,"Data": [],"ErrorCode": 0}
                        var json = JObject.Parse(html.Html);
                        if (Convert.ToBoolean(json["Success"]))
                            ConfigHelp.UpdateItem("HotelName", json["Data"].ToString());
                    }
                    #endregion

                    if (string.IsNullOrWhiteSpace(txtPollTime.Text))
                    {
                        txtPollTime.Text = "5";
                    }
                    ConfigHelp.UpdateItem("pollTime", txtPollTime.Text.Trim());

                    if (string.IsNullOrWhiteSpace(txtPrintTime.Text))
                    {
                        txtPrintTime.Text = "3";
                    }
                    ConfigHelp.UpdateItem("printTime", txtPrintTime.Text.Trim());

                    if (!string.IsNullOrWhiteSpace(hid) && !string.IsNullOrWhiteSpace(secretKey))
                    {
                        {
                            var updatework = new UpdateWork("",hid);
                            //检查是否有更新
                            if (updatework.UpdateVerList.Count > 0)
                            {
                                //启动更新程序
                                updatework.UpdateApp( hid);
                            }

                            GetPrinterListByInterface(hid, secretKey, section, isEnvTest);
                        }

                        GetPrinterListByLocal();
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                tsslTips.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        /// <summary>
        /// 保存配置列表到数据文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ArrayList sqlStringList = new ArrayList();
                DataTable dt = CommonHelp.GetDgvToTable(dgvPrint);
                if (dt != null && dt.Rows.Count > 0)
                {
                    //拼接值
                    foreach (DataRow row in dt.Rows)
                    {
                        var id = row["Id"];
                        var port = row["端口"];
                        var name = row["名称"];
                        var printer = row["打印机"];
                        var produceFormat = row["出品格式"];
                        var standbyPort = row["备用端口"];
                        var associatedPort = row["关联端口"];
                        var printOneByOne = row["逐条打印"];

                        var sql = string.Format("UPDATE ProductPrinter SET Port = '{0}', Name = '{1}', Printer = '{2}', ProduceFormat = '{3}', StandbyPort = '{4}', AssociatedPort = '{5}', PrintOneByOne = '{6}' WHERE Id = '{7}'", port, name, printer, produceFormat, standbyPort, associatedPort, printOneByOne, id);

                        sqlStringList.Add(sql);
                    }
                    SQLiteHelper.ExecuteSqlTran(sqlStringList);
                }

                if (Application.OpenForms["FrmMain"] == null)
                {
                    FrmMain form = new FrmMain();
                    form.PrintFrom_Load(form, null);
                    form.refreshPrintList();
                    form.Show();
                    form.Activate();
                }
                else
                {
                    var form = (FrmMain)Application.OpenForms["FrmMain"];
                    form.refreshPrintList();
                    form.Show();
                    form.Activate();
                }

                Hide();
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                tsslTips.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }
        private void FormPrint_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtHid.Text) || string.IsNullOrWhiteSpace(txtHid.Text) || string.IsNullOrWhiteSpace(txtHid.Text))
                {
                    if (MessageBox.Show("确认退出本程序吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        Application.ExitThread();
                        Application.Exit();
                        Process.GetCurrentProcess().Kill();
                    }
                }
                else
                {
                    if (Application.OpenForms["FrmMain"] == null)
                    {
                        FrmMain form = new FrmMain();
                        form.PrintFrom_Load(form, null);
                        form.refreshPrintList();
                        form.Show();
                        form.Activate();
                    }
                    else
                    {
                        var form = (FrmMain)Application.OpenForms["FrmMain"];
                        form.refreshPrintList();
                        form.Show();
                        form.Activate();
                    }
                    btnSaveSet_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                tsslTips.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");

            }
        }

        /// <summary>
        /// 双击接口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInterface_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (txtInterface.ReadOnly)
            {
                txtInterface.ReadOnly = false;
            }
            else
            {
                txtInterface.ReadOnly = true;
            }
        }

        /// <summary>
        /// 酒店代码提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtHid_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHid.Text))
            {
                lblHidTip.Visible = true;
            }
            else
            {
                lblHidTip.Visible = false;
            }
        }

        /// <summary>
        /// 秘钥提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSecretKey_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSecretKey.Text))
            {
                lblSecretKeyTip.Visible = true;
            }
            else
            {
                lblSecretKeyTip.Visible = false;
            }
        }

        /// <summary>
        /// 接口提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInterface_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInterface.Text))
            {
                lblInterfaceTip.Visible = true;
            }
            else
            {
                lblInterfaceTip.Visible = false;
            }
        }

        /// <summary>
        /// 鼠标右键选中行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvPrint_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    dgvPrint.ClearSelection();
                    dgvPrint.Rows[e.RowIndex].Selected = true;
                    dgvPrint.CurrentCell = dgvPrint.Rows[e.RowIndex].Cells[e.ColumnIndex];
                }
            }
        }

        /// <summary>
        /// 删除选中打印机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiDelPrint_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要删除数据吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                var dr = dgvPrint.SelectedRows[0];
                var id = dr.Cells["Id"].Value;

                var delete = $"DELETE FROM ProductPrinter WHERE Id ='{id}'";
                SQLiteHelper.ExecuteSql(delete);

                tsslTips.Text = $"提示({DateTime.Now})：{dr.Cells["名称"].Value}删除成功";

                btnSaveSet_Click(btnSaveSet, null);
            }
        }

        private void tsmiPrintTest_Click(object sender, EventArgs e)
        {
            try
            {
                var dr = dgvPrint.SelectedRows[0];

                StiReport format = new StiReport();
                var path = $@"{Application.StartupPath}\format\\测试页.mrt";
                format.Load(path);  //加载打印格式文件
                format.Compile();
                format["Port"] = dr.Cells["端口"].Value;
                format["PrintName"] = dr.Cells["名称"].Value;
                format.ReportName = "快点云Pos出品测试页";
                format.Render(false);
                PrinterSettings printerSettings = new PrinterSettings();
                printerSettings.PrintFileName = format.ReportName;
                printerSettings.PrinterName = ((DataGridViewComboBoxCell)dr.Cells["打印机"]).Value.ToString();
                printerSettings.Copies = 1;
                format.Print(false, printerSettings);
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                tsslTips.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        //获取客户标识
        /// <summary>
        /// 获取客户标识
        /// </summary>
        /// <param name="Hid"></param>
        /// <param name="InterfaceUrl"></param>
        /// <param name="secretKey"></param>
        /// <param name="isEnvTest"></param>
        /// <returns>返回客户标识，为空字符串不更新程序</returns>
        private string GetAppServerAddress(string Hid, string InterfaceUrl, string secretKey, string isEnvTest)
        {
            var _serveraddress = "";  //1  vip1  ，2 --- vip2
            //查询酒店标识
            var httpItem = new HttpItem();
            httpItem.Method = "post";
            httpItem.URL = $"{InterfaceUrl.Trim()}/GetServerAddRess";
            httpItem.ContentType = "application/x-www-form-urlencoded";
            httpItem.Postdata = $"hid={Hid}&secretKey={secretKey}&isEnvTest={isEnvTest}";
            var httpHelper = new HttpHelper();
            var html = httpHelper.GetHtml(httpItem);
            if (html.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //接口默认返回格式：{"Success": true,"Data": [],"ErrorCode": 0}
                var json = JObject.Parse(html.Html);
                if (Convert.ToBoolean(json["Success"]))
                {
                    _serveraddress = json["Data"].ToString();
                }
                else
                {
                    tsslTips.Text = $"提示({DateTime.Now})：{json["Data"].ToString()}";
                    LogHelp.WriteLog(json["Data"].ToString());
                }
            }
            else
            {
                if (html.StatusCode == 0)
                {
                    tsslTips.Text = $"提示({DateTime.Now})：{html.Html}";
                    LogHelp.WriteLog(html.Html);
                }
                else
                {
                    tsslTips.Text = $"提示({DateTime.Now})：{html.StatusDescription}";
                    LogHelp.WriteLog(html.StatusDescription);
                }
            }
            return _serveraddress;
        }



    }
}
