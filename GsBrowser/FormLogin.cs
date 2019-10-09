using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SufeiUtil;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace GsBrowser
{
    public partial class FormLogin : Form
    {
        static HttpItem httpItem = new HttpItem();

        public FormLogin()
        {
            InitializeComponent();
        }

        private void SetForm_Load(object sender, EventArgs e)
        {
            initConfig();

            if (Application.OpenForms["FormBrowser"] == null)
            {
                FormBrowser form = new FormBrowser();
                form.WindowState = FormWindowState.Minimized;
                form.Show();
            }
        }

        /// <summary>
        /// 初始化基础配置文件，如果配置项被删除，则自动添加被删除的配置项，并赋默认值
        /// </summary>
        private void initConfig()
        {
            try
            {
                //集团代码
                if (!string.IsNullOrWhiteSpace(ConfigHelp.GetItem("Gid")))
                {
                    lblGroupId.Text = ConfigHelp.GetItem("Gid");
                }

                //酒店代码
                if (!string.IsNullOrWhiteSpace(ConfigHelp.GetItem("Hid")))
                {
                    txtHid.Text = ConfigHelp.GetItem("Hid");
                }

                //用户名
                if (!string.IsNullOrWhiteSpace(ConfigHelp.GetItem("UserName")))
                {
                    txtUserName.Text = ConfigHelp.GetItem("UserName");
                }

                //密码
                if (!string.IsNullOrWhiteSpace(ConfigHelp.GetItem("Password")))
                {
                    txtPassword.Text = EncryptUtil.UnAesStr(ConfigHelp.GetItem("Password"), "pos.gshis.com   ", "捷信达浏览器：GsBrowser");
                }

                //启动页
                if (!string.IsNullOrWhiteSpace(ConfigHelp.GetItem("StartPage")))
                {
                    if (rdbDefault.Text == ConfigHelp.GetItem("StartPage"))
                    {
                        rdbDefault.Checked = true;
                    }
                    else if (rdbFloor.Text == ConfigHelp.GetItem("StartPage"))
                    {
                        rdbFloor.Checked = true;
                    }
                    else if (rdbInSingle.Text == ConfigHelp.GetItem("StartPage"))
                    {
                        rdbInSingle.Checked = true;
                    }
                    else if (rdbCashier.Text == ConfigHelp.GetItem("StartPage"))
                    {
                        rdbCashier.Checked = true;
                    }
                }

                if (txtHid.Text != lblGroupId.Text && lblGroupId.Text != "GroupId")
                {
                    txtHid.Text = lblGroupId.Text;
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                //throw;
            }
        }

        /// <summary>
        /// 登录并保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHid.Text.Trim()))
            {
                MessageBox.Show("请输入酒店代码");
                return;
            }
            else if (string.IsNullOrWhiteSpace(txtUserName.Text.Trim()))
            {
                MessageBox.Show("请输入用户名");
                return;
            }
            else if (string.IsNullOrWhiteSpace(txtPassword.Text.Trim()))
            {
                MessageBox.Show("请输入密码");
                return;
            }
            else
            {
                try
                {
                    ConfigHelp.UpdateItem("Hid", txtHid.Text.Trim());
                    ConfigHelp.UpdateItem("UserName", txtUserName.Text.Trim());
                    ConfigHelp.UpdateItem("Password", EncryptUtil.AesStr(txtPassword.Text.Trim(), "pos.gshis.com   ", "捷信达浏览器：GsBrowser"));

                    // 请求接口数据
                    httpItem.Method = "post";
                    httpItem.URL = string.Format("http://pos.gshis.com/Account/GsBrowserLogin?HotelId={0}&Username={1}&Password={2}", txtHid.Text.Trim(), txtUserName.Text.Trim(), txtPassword.Text.Trim());

                    var isTest = Convert.ToBoolean(ConfigHelp.GetItem("IsTest"));
                    if (isTest)
                    {
                        httpItem.URL = string.Format("http://postest.gshis.com/Account/GsBrowserLogin?HotelId={0}&Username={1}&Password={2}", txtHid.Text.Trim(), txtUserName.Text.Trim(), txtPassword.Text.Trim());
                    }

                    var httpHelper = new HttpHelper();
                    var html = httpHelper.GetHtml(httpItem);

                    if (html.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //接口默认返回格式：{"Success": true,"Data": [],"ErrorCode": 0}
                        var json = JObject.Parse(html.Html);

                        if (Convert.ToBoolean(json["Success"]))
                        {
                            JavaScriptObject.HotelInfoJson = json;
                            JavaScriptObject.CurrentInfo = (JObject)json["Data"]["_currentInfo"];
                            if (Convert.ToBoolean(json["Data"]["isGroup"]))     //集团
                            {
                                ConfigHelp.UpdateItem("Gid", txtHid.Text.Trim());
                                DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(json["Data"]["hotels"].ToString());
                                if (dataTable != null && dataTable.Rows.Count > 0)
                                {
                                    cbxBranch.DataSource = dataTable;
                                    cbxBranch.DisplayMember = "Hname";
                                    cbxBranch.ValueMember = "Hid";

                                    if (cbxBranch.Items.Count > 0)
                                    {
                                        var hname = ConfigHelp.GetItem("Hname");
                                        for (int i = 0; i < cbxBranch.Items.Count; i++)
                                        {
                                            var name = ((DataRowView)cbxBranch.Items[i])["Hname"];
                                            if (name.Equals(hname))
                                            {
                                                cbxBranch.SelectedItem = cbxBranch.Items[i];
                                                break;
                                            }
                                        }
                                    }
                                }

                                //根据权限列表选择启动页
                                DataTable dtAuth = JsonConvert.DeserializeObject<DataTable>(json["Data"]["authlist"].ToString());
                                if (dtAuth != null && dtAuth.Rows.Count > 0)
                                {
                                    var dtList = dtAuth.AsEnumerable();
                                    if (dtList.Any(w => w["AuthCode"].ToString() == rdbFloor.Tag.ToString()))
                                    {
                                        rdbFloor.Checked = true;
                                    }
                                    else if (dtList.Any(w => w["AuthCode"].ToString() == rdbInSingle.Tag.ToString()))
                                    {
                                        rdbInSingle.Checked = true;
                                    }
                                    else if (dtList.Any(w => w["AuthCode"].ToString() == rdbCashier.Tag.ToString()))
                                    {
                                        rdbCashier.Checked = true;
                                    }
                                    else
                                    {
                                        rdbDefault.Checked = true;
                                    }
                                }

                                cbxBranch.Enabled = true;
                                panBranch.Enabled = true;
                            }
                            else    //单店
                            {
                                int i = 0;
                                var abc = "[" + json["Data"]["hotel"].ToString() + "]";
                                DataTable dataTable = JsonConvert.DeserializeObject<DataTable>("[" + json["Data"]["hotel"].ToString() + "]");

                                foreach (var temp in dataTable.Rows[0]["CateringServicesType"].ToString().Split(','))
                                {
                                    if (!string.IsNullOrEmpty(temp))
                                    {
                                        Button button = new Button();
                                        Enum cateringServicesType = (CateringServicesType)Enum.Parse(typeof(CateringServicesType), temp);
                                        var description = CommonHelp.GetDescription(cateringServicesType);
                                        button.Name = "btn" + temp;
                                        button.Text = description;
                                        button.Font = new Font("宋体", 12);
                                        button.Size = new Size(80, 30);
                                        button.Tag = temp;
                                        button.Location = new Point(10 + i * 90, 15);
                                        gbxMode.Controls.Add(button);
                                        button.Click += ModelJump_Click;
                                        i += 1;
                                    }
                                }
                                foreach (var temp in dataTable.Rows[0]["CateringServicesModule"].ToString().Split(','))
                                {
                                    if (!string.IsNullOrEmpty(temp))
                                    {
                                        if (temp == "A4")
                                        {
                                            Button button = new Button();
                                            Enum cateringServicesType = (CateringServicesType)Enum.Parse(typeof(CateringServicesType), temp);
                                            var description = CommonHelp.GetDescription(cateringServicesType);
                                            button.Name = "btn" + temp;
                                            button.Text = description;
                                            button.Font = new Font("宋体", 12);
                                            button.Size = new Size(80, 30);
                                            button.Tag = temp;
                                            button.Location = new Point(10 + i * 90, 15);
                                            gbxMode.Controls.Add(button);
                                            button.Click += ModelJump_Click;
                                            break;
                                        }


                                    }
                                }

                                DataTable dtPos = JsonConvert.DeserializeObject<DataTable>(json["Data"]["poslist"].ToString());
                                if (dtPos != null)
                                {
                                    gbxMode.Enabled = true;
                                    //var dt = dtPos.AsEnumerable().Where(w => w["PosMode"].ToString() == "A" && w["Hid"].ToString() == txtHid.Text).ToList();
                                    var dt = dtPos.AsEnumerable().Where(w => w["Hid"].ToString() == txtHid.Text).ToList();
                                    if (dt != null && dt.Count > 0)
                                    {
                                        cbxPos.DataSource = dt.CopyToDataTable();
                                        cbxPos.DisplayMember = "Name";
                                        cbxPos.ValueMember = "Id";

                                        var pname = ConfigHelp.GetItem("Pname");
                                        if (cbxPos.Items.Count > 0)
                                        {
                                            for (int j = 0; j < cbxPos.Items.Count; j++)
                                            {
                                                var name = ((DataRowView)cbxPos.Items[j])["Name"];
                                                if (name.Equals(pname))
                                                {
                                                    cbxPos.SelectedItem = cbxPos.Items[j];
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                //根据权限列表选择启动页
                                DataTable dtAuth = JsonConvert.DeserializeObject<DataTable>(json["Data"]["authlist"].ToString());
                                if (dtAuth != null && dtAuth.Rows.Count > 0)
                                {
                                    var dtList = dtAuth.AsEnumerable();
                                    if (dtList.Any(w => w["AuthCode"].ToString() == rdbFloor.Tag.ToString()))
                                    {
                                        rdbFloor.Checked = true; ConfigHelp.UpdateItem("AuthCode", rdbFloor.Tag.ToString());
                                    }
                                    else if (dtList.Any(w => w["AuthCode"].ToString() == rdbInSingle.Tag.ToString()))
                                    {
                                        rdbInSingle.Checked = true; ConfigHelp.UpdateItem("AuthCode", rdbInSingle.Tag.ToString());
                                    }
                                    else if (dtList.Any(w => w["AuthCode"].ToString() == rdbCashier.Tag.ToString()))
                                    {
                                        rdbCashier.Checked = true;
                                    }
                                    else
                                    {
                                        rdbDefault.Checked = true;
                                    }
                                }

                                cbxBranch.Enabled = false;
                                panBranch.Enabled = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show(json["Data"].ToString());
                            if (Application.OpenForms["FormLogin"] == null)
                            {
                                FormLogin form = new FormLogin();
                                form.Show();
                                form.Activate();
                                Hide();
                            }
                        }
                    }
                    else
                    {
                        if (html.StatusCode == 0)
                        {
                            MessageBox.Show(html.Html);
                        }
                        else
                        {
                            MessageBox.Show(html.StatusDescription);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelp.CreateLog(ex);
                    Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                    //throw;
                }
            }
        }

        /// <summary>
        /// 跳转不同模式页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModelJump_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbxBranch.Enabled && cbxBranch.SelectedIndex == -1)
                {
                    MessageBox.Show("请选择分店");
                }
                else if (cbxPos.SelectedIndex == -1)
                {
                    MessageBox.Show("请选择收银点");
                }
                else
                {
                    var url = "";

                    ConfigHelp.UpdateItem("Mode", ((Button)sender).Tag.ToString());
                    ConfigHelp.UpdateItem("ModeName", ((Button)sender).Text);
                    if (cbxBranch.Enabled)
                    {
                        ConfigHelp.UpdateItem("Hid", cbxBranch.SelectedValue.ToString());
                        ConfigHelp.UpdateItem("Hname", cbxBranch.Text.ToString());

                        JavaScriptObject.CurrentInfo["HotelId"] = cbxBranch.SelectedValue.ToString();
                        JavaScriptObject.CurrentInfo["HotelName"] = cbxBranch.Text.ToString();
                    }
                    else
                    {
                        ConfigHelp.UpdateItem("Hid", txtHid.Text);
                        JavaScriptObject.CurrentInfo["HotelId"] = txtHid.Text;
                    }
                    string a = cbxPos.SelectedValue.ToString();
                    ConfigHelp.UpdateItem("Pid", cbxPos.SelectedValue.ToString());
                    ConfigHelp.UpdateItem("Pname", cbxPos.Text.ToString());

                    JavaScriptObject.CurrentInfo["PosId"] = cbxPos.SelectedValue.ToString();
                    JavaScriptObject.CurrentInfo["PosName"] = cbxPos.Text.ToString();

                    if (Application.OpenForms["FormBrowser"] != null)
                    {
                        var form = ((FormBrowser)Application.OpenForms["FormBrowser"]);
                        form.jsObject.EnableKeyboard = Convert.ToBoolean(ConfigHelp.GetItem("EnableKeyboard"));
                        form.jsObject.Current = EncryptUtil.AesStr(JavaScriptObject.CurrentInfo.ToString(), "pos.gshis.com   ", "捷信达浏览器：GsBrowser");
                        ConfigHelp.UpdateItem("Current", EncryptUtil.AesStr(JavaScriptObject.CurrentInfo.ToString(), "pos.gshis.com   ", "捷信达浏览器：GsBrowser"));
                    }

                    string path = Application.StartupPath + "\\html\\Index.html";
                    if (File.Exists(path))
                    {
                        HtmlWeb htmlWeb = new HtmlWeb();
                        var hid = txtHid.Text.Trim();
                        var mode = ((Button)sender).Tag;

                        HtmlAgilityPack.HtmlDocument htmlDoc = htmlWeb.Load(path);
                        HtmlNode htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//a[@id='url']");

                        var returnUrl = "";
                        var isTest = Convert.ToBoolean(ConfigHelp.GetItem("IsTest"));
                        //默认
                        if (rdbDefault.Checked)  
                        {
                            ConfigHelp.UpdateItem("StartPage", rdbDefault.Text);
                            if (mode.ToString() == CateringServicesType.A.ToString())
                            {
                                returnUrl = HttpUtility.UrlEncode($"/PosManage/PosTabStatus/Index?mode={mode}", Encoding.UTF8);
                                url = "http://" + $"pos.gshis.com/Account/_UpdateCurrent?hid={hid}&mode={mode}&ReturnUrl={returnUrl}";

                                if (isTest)
                                {
                                    url = "http://" + $"postest.gshis.com/Account/_UpdateCurrent?hid={hid}&mode={mode}&ReturnUrl={returnUrl}";
                                }
                            }
                            else if (mode.ToString() == CateringServicesType.B.ToString() || mode.ToString() == CateringServicesType.C.ToString())
                            {
                                returnUrl = HttpUtility.UrlEncode($"/PosManage/PosInSingle/Index?mode={mode}", Encoding.UTF8);
                                url = "http://" + $"pos.gshis.com/Account/_UpdateCurrent?hid={hid}&mode={mode}&ReturnUrl={returnUrl}";

                                if (isTest)
                                {
                                    url = "http://" + $"postest.gshis.com/Account/_UpdateCurrent?hid={hid}&mode={mode}&ReturnUrl={returnUrl}";
                                }
                            }
                            else if (mode.ToString() == CateringServicesType.A4.ToString())
                            {
                                returnUrl = HttpUtility.UrlEncode($"/PosManage/PosSeafoodPool/Index?mode={mode}", Encoding.UTF8);
                                url = "http://" + $"pos.gshis.com/Account/_UpdateCurrent?hid={hid}&mode={mode}&ReturnUrl={returnUrl}";

                                if (isTest)
                                {
                                    url = "http://" + $"postest.gshis.com/Account/_UpdateCurrent?hid={hid}&mode={mode}&ReturnUrl={returnUrl}";
                                }
                            }
                        }
                        //楼面
                        else if (rdbFloor.Checked)  
                        {
                            ConfigHelp.UpdateItem("StartPage", rdbFloor.Text);
                            returnUrl = HttpUtility.UrlEncode($"/PosManage/PosTabStatus/Index?mode={mode}", Encoding.UTF8);
                            url = "http://" + $"pos.gshis.com/Account/_UpdateCurrent?hid={hid}&mode={mode}&ReturnUrl={returnUrl}";

                            if (isTest)
                            {
                                url = "http://" + $"postest.gshis.com/Account/_UpdateCurrent?hid={hid}&mode={mode}&ReturnUrl={returnUrl}";
                            }
                        }
                        //入单
                        else if (rdbInSingle.Checked)   
                        {
                            ConfigHelp.UpdateItem("StartPage", rdbInSingle.Text);
                            returnUrl = HttpUtility.UrlEncode($"/PosManage/PosInSingle/Index?mode={mode}", Encoding.UTF8);
                            url = "http://" + $"pos.gshis.com/Account/_UpdateCurrent?hid={hid}&mode={mode}&ReturnUrl={returnUrl}";

                            if (isTest)
                            {
                                url = "http://" + $"postest.gshis.com/Account/_UpdateCurrent?hid={hid}&mode={mode}&ReturnUrl={returnUrl}";
                            }
                        }
                        //收银
                        else if (rdbCashier.Checked)   
                        {
                            ConfigHelp.UpdateItem("StartPage", rdbInSingle.Text);
                            var authCode = ConfigHelp.GetItem("AuthCode");
                            returnUrl = HttpUtility.UrlEncode($"/Home/Main?authCode={authCode}", Encoding.UTF8);
                            url = "http://" + $"pos.gshis.com/Account/_UpdateCurrent?hid={hid}&mode={mode}&ReturnUrl={returnUrl}";

                            if (isTest)
                            {
                                url = "http://" + $"postest.gshis.com/Account/_UpdateCurrent?hid={hid}&mode={mode}&ReturnUrl={returnUrl}";
                            }
                        }

                        htmlNode.Attributes["href"].Value = url;
                        htmlDoc.Save(path, Encoding.UTF8);
                        ConfigHelp.UpdateItem("Url", url);

                        if (isTest)     //设置返回的URL
                        {
                            returnUrl = $@"http://postest.gshis.com{HttpUtility.UrlDecode(returnUrl)}";
                            ConfigHelp.UpdateItem("ReturnUrl", returnUrl);
                        }
                        else
                        {
                            returnUrl = $@"http://pos.gshis.com{HttpUtility.UrlDecode(returnUrl)}";
                            ConfigHelp.UpdateItem("ReturnUrl", returnUrl);
                        }

                        htmlDoc = htmlWeb.Load(path);
                        htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//a[@id='url']");
                        Console.WriteLine(url);

                        var isFullScreen = Convert.ToBoolean(ConfigHelp.GetItem("IsFullScreen"));
                        if (Application.OpenForms["FormBrowser"] != null)
                        {
                            FormBrowser form = (FormBrowser)Application.OpenForms["FormBrowser"];
                            form.browser.Load(path);
                            if (isFullScreen)
                            {
                                form.FormBorderStyle = FormBorderStyle.None;
                            }
                            else
                            {
                                form.FormBorderStyle = FormBorderStyle.FixedSingle;
                            }

                            TopMost = true;

                            form.WindowState = FormWindowState.Maximized;
                            form.Show();
                            form.Activate();
                        }
                        else
                        {
                            FormBrowser form = new FormBrowser();
                            form.browser.Load(path);

                            if (isFullScreen)
                            {
                                form.FormBorderStyle = FormBorderStyle.None;
                            }
                            else
                            {
                                form.FormBorderStyle = FormBorderStyle.FixedSingle;
                            }

                            form.Show();
                            form.Activate();
                        }

                        Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                //throw;
            }
        }

        private void cbxBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                gbxMode.Controls.Clear();
                var cbx = (ComboBox)sender;
                var json = JavaScriptObject.HotelInfoJson;
                if (json != null)
                {
                    DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(json["Data"]["cateringServicesTypes"].ToString());

                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        int i = 0;
                        foreach (DataRow row in dataTable.Rows)
                        {
                            if (cbx.SelectedValue.ToString() == row["Hid"].ToString())
                            {
                                json["Data"]["_currentInfo"]["HotelId"] = cbx.SelectedValue.ToString();
                                json["Data"]["_currentInfo"]["HotelName"] = cbx.Text.ToString();

                                var cst = row["CateringServicesType"].ToString().Split(',');
                                foreach (var temp in cst)
                                {
                                    if (!string.IsNullOrEmpty(temp))
                                    {
                                        Button button = new Button();
                                        Enum cateringServicesType = (CateringServicesType)Enum.Parse(typeof(CateringServicesType), temp);
                                        var description = CommonHelp.GetDescription(cateringServicesType);
                                        button.Name = "btn" + temp;
                                        button.Text = description;
                                        button.Font = new Font("宋体", 12);
                                        button.Size = new Size(80, 30);
                                        button.Tag = temp;
                                        button.Location = new Point(10 + i * 90, 15);
                                        gbxMode.Controls.Add(button);
                                        button.Click += ModelJump_Click;
                                        i += 1;
                                    }
                                }
                                var s = json["Data"].ToString();
                                DataTable hotelTable = JsonConvert.DeserializeObject<DataTable>(json["Data"]["hotels"].ToString());
                                hotelTable = hotelTable.AsEnumerable().Where(w => w["hid"].ToString() == cbx.SelectedValue.ToString()).CopyToDataTable();
                                foreach (var temp in hotelTable.Rows[0]["CateringServicesModule"].ToString().Split(','))
                                {
                                    if (!string.IsNullOrEmpty(temp))
                                    {
                                        if (temp == "A4")
                                        {
                                            Button button = new Button();
                                            Enum cateringServicesType = (CateringServicesType)Enum.Parse(typeof(CateringServicesType), temp);
                                            var description = CommonHelp.GetDescription(cateringServicesType);
                                            button.Name = "btn" + temp;
                                            button.Text = description;
                                            button.Font = new Font("宋体", 12);
                                            button.Size = new Size(80, 30);
                                            button.Tag = temp;
                                            button.Location = new Point(10 + i * 90, 15);
                                            gbxMode.Controls.Add(button);
                                            button.Click += ModelJump_Click;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }



                    DataTable dtPos = JsonConvert.DeserializeObject<DataTable>(json["Data"]["poslist"].ToString());
                    if (dtPos != null)
                    {
                        gbxMode.Enabled = true;
                        var dt = dtPos.AsEnumerable().Where(w => /*w["PosMode"].ToString() == "A" &&*/ w["Hid"].ToString() == cbxBranch.SelectedValue.ToString()).ToList();
                        if (dt != null && dt.Count > 0)
                        {
                            cbxPos.DataSource = dt.CopyToDataTable();
                            cbxPos.DisplayMember = "Name";
                            cbxPos.ValueMember = "Id";

                            var pname = ConfigHelp.GetItem("Pname");

                            if (cbxPos.Items != null && cbxPos.Items.Count > 0)
                            {
                                for (int i = 0; i < cbxPos.Items.Count; i++)
                                {
                                    var name = ((DataRowView)cbxPos.Items[i])["Name"];
                                    if (name.Equals(pname))
                                    {
                                        cbxPos.SelectedItem = cbxPos.Items[i];
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                //throw;
            }
        }

        private void cbxPos_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var cbx = (ComboBox)sender;
                var json = JavaScriptObject.HotelInfoJson;
                if (cbx.SelectedValue != null)
                {
                    json["Data"]["_currentInfo"]["PosId"] = cbx.SelectedValue.ToString();
                    json["Data"]["_currentInfo"]["PosName"] = cbx.Text.ToString();
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                //throw;
            }
        }

        /// <summary>
        /// 启用禁用屏幕键盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbKeyboardEnable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdbKeyboardEnable.Checked)
                {
                    if (Application.OpenForms["FormBrowser"] != null)
                    {
                        var form = ((FormBrowser)Application.OpenForms["FormBrowser"]);
                        form.jsObject.EnableKeyboard = true;
                    }
                    ConfigHelp.UpdateItem("EnableKeyboard", true.ToString());
                }
                else if (rdbKeyboardDisable.Checked)
                {
                    if (Application.OpenForms["FormBrowser"] != null)
                    {
                        var form = ((FormBrowser)Application.OpenForms["FormBrowser"]);
                        form.jsObject.EnableKeyboard = false;
                    }
                    ConfigHelp.UpdateItem("EnableKeyboard", false.ToString());
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                //throw;
            }
        }
    }
}
