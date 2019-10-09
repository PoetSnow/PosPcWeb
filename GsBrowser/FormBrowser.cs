using CefSharp.WinForms;
using HtmlAgilityPack;
using SufeiUtil;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GsBrowser
{

    public partial class FormBrowser : Form
    {
        private HttpItem httpItem = new HttpItem();

        public readonly ChromiumWebBrowser browser;
        public JavaScriptObject jsObject;

        public FormBrowser()
        {
            InitializeComponent();
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\html\\Index.html";  //获取文件的物理路径
                path = "file://" + path.Replace("\\", "/"); //转换为File协议路径
                browser = new ChromiumWebBrowser(path)
                {
                    Dock = DockStyle.Fill,
                    KeyboardHandler = new KeyBoardHandler(),
                    JsDialogHandler = new JsDialogHandler(),
                };
                Controls.Add(browser);

                jsObject = new JavaScriptObject(browser);   //注册js对象
                //初始化手写参数
                if (!string.IsNullOrWhiteSpace(ConfigHelp.GetItem("IsHandwrite")))
                {
                    jsObject.IsHandwrite = Convert.ToBoolean(ConfigHelp.GetItem("IsHandwrite"));
                }
                else
                {
                    ConfigHelp.AddItem("IsHandwrite", true.ToString());
                    jsObject.IsHandwrite = Convert.ToBoolean(ConfigHelp.GetItem("IsHandwrite"));
                }

                //初始化键盘参数
                if (!string.IsNullOrWhiteSpace(ConfigHelp.GetItem("EnableKeyboard")))
                {
                    jsObject.EnableKeyboard = Convert.ToBoolean(ConfigHelp.GetItem("EnableKeyboard"));
                }
                else
                {
                    ConfigHelp.AddItem("EnableKeyboard", true.ToString());
                    jsObject.EnableKeyboard = Convert.ToBoolean(ConfigHelp.GetItem("EnableKeyboard"));
                }

                browser.AddressChanged += Browser_AddressChanged;

                CheckForIllegalCrossThreadCalls = false;    //不检查非法跨线程调用
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                //throw;
            }
        }

        private void GsBrowser_Load(object sender, EventArgs e)
        {
            try
            {
                var isFullScreen = Convert.ToBoolean(ConfigHelp.GetItem("IsFullScreen"));
                if (isFullScreen)
                {
                    FormBorderStyle = FormBorderStyle.None;
                }
                else
                {
                    FormBorderStyle = FormBorderStyle.FixedSingle;
                }

                //消除大小改变时的闪烁现象
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.DoubleBuffer, true);
                SetStyle(ControlStyles.UserPaint, true);

                //访问主界面
                string userName = ConfigHelp.GetItem("UserName");
                string password = ConfigHelp.GetItem("Password");
                jsObject.ComputerName = Environment.MachineName;
                jsObject.ReturnUrl = ConfigHelp.GetItem("ReturnUrl");
                Console.WriteLine(jsObject.ComputerName);
                if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
                {
                    #region 页面跳转
                    jsObject.Current = ConfigHelp.GetItem("Current");   //当前用户信息
                    string path = Application.StartupPath + "\\html\\Index.html";
                    if (File.Exists(path))
                    {
                        var isTest = Convert.ToBoolean(ConfigHelp.GetItem("IsTest"));
                        if (isTest)
                        {
                            HtmlWeb htmlWeb = new HtmlWeb();
                            HtmlAgilityPack.HtmlDocument htmlDoc = htmlWeb.Load(path);
                            HtmlNode htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//a[@id='url']");
                            if (htmlNode.Attributes["href"].Value.IndexOf("postest") == -1)
                            {
                                htmlNode.Attributes["href"].Value = "http://postest.gshis.com";
                                htmlDoc.Save(path, Encoding.UTF8);
                                ConfigHelp.UpdateItem("Url", htmlNode.Attributes["href"].Value);
                            }
                        }

                        browser.Load(path);
                    }
                    #endregion
                }
                else
                {
                    if (Application.OpenForms["FormLogin"] == null)
                    {
                        FormLogin form = new FormLogin();
                        form.Show();
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

        /// <summary>
        /// 浏览器地址改变时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browser_AddressChanged(object sender, CefSharp.AddressChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(e.Address) || e.Address.IndexOf("javascript:") > -1)
                {
                    string path = Application.StartupPath + "\\html\\Index.html";
                    if (File.Exists(path))
                    {
                        HtmlWeb htmlWeb = new HtmlWeb();

                        HtmlAgilityPack.HtmlDocument htmlDoc = htmlWeb.Load(path);
                        HtmlNode htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//a[@id='url']");

                        var isTest = Convert.ToBoolean(ConfigHelp.GetItem("IsTest"));
                        if (isTest)
                        {
                            if (htmlNode.Attributes["href"].Value.IndexOf("postest") == -1)
                            {
                                htmlNode.Attributes["href"].Value = "http://postest.gshis.com";
                                htmlDoc.Save(path, Encoding.UTF8);
                            }
                        }

                        browser.Load(path);
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

        delegate void MessageBoxShowEventHandler(string msg, FormClosingEventArgs e);
        private void BrowserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Invoke(new MessageBoxShowEventHandler(MessageBoxShowClosingEvent), new object[] { "确认退出本程序吗？",e });
        }

        private void MessageBoxShowClosingEvent(string msg, FormClosingEventArgs e)
        {            
            DialogResult dr = MessageBox.Show(msg, "快点云Pos提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (dr == DialogResult.OK)
            {
                try
                {
                    Application.ExitThread();
                    Application.Exit();
                    Process.GetCurrentProcess().Kill();
                }
                finally
                {
                    browser.Dispose();
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        #region 手写板

        public string Id = "";  //获取手写值的Html标签ID

        /// <summary>
        /// 绑定手写事件
        /// </summary>
        /// <param name="value"></param>
        internal void handwrite1_SelectTextEvent(object sender, EventArgs e, string value)
        {
            var val = "GetHandwrittenValue('" + Id + "','" + value + "');";
            browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(val);
           // Handwrite.Handwrite s = new Handwrite.Handwrite();
           //s.ic
           // browser.Controls["handwrite1"].Hide();
        }

        /// <summary>
        /// 关闭手写板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void handwrite1_CloseEvent(object sender, EventArgs e)
        {
            if (browser.Controls["handwrite1"] != null)
            {
                browser.Controls["handwrite1"].Dispose();
            }
        }

        /// <summary>
        /// 关闭手写板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void handwrite1_DeleteEvent(object sender, EventArgs e)
        { 
            var val = "DeleteInput('" + Id + "');";
            browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(val);
            //browser.Controls["handwrite1"].Hide();
        }
        /// <summary>
        /// 关闭手写板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void FormHandwrite_Deactivate(object sender, EventArgs e)
        {
            if (browser.Controls["handwrite1"] != null)
            {
                browser.Controls["handwrite1"].Hide();
            }
        }
        #endregion
    }
}
