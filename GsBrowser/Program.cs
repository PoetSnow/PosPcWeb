using CefSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GsBrowser
{
    static class Program
    {
        #region Setting from ini document 
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string Section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string Section, string key, string def, /*StringBuilder reVal原来用StringBuilder*/byte[] buffer/*现在使用byte串*/, int Usize, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        #endregion

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] Args)
        {
            //获取当前登录的Windows用户的标识 
            System.Security.Principal.WindowsIdentity wid = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(wid);

            // 判断当前用户是否是管理员 
            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                byte[] bufferContent = new byte[255];
                string LanguageInfoPath = Application.StartupPath + "\\DayaSystemInfo.ini";

                FileInfo fileInfo = new FileInfo(LanguageInfoPath);
                string section = "Setting";
                string key = "Language";

                int returnNum = 0;
                /* 读取显示语言及处理 */
                if (fileInfo.Exists)
                    returnNum = GetPrivateProfileString(section, key, "", bufferContent, (int)bufferContent.Length, fileInfo.ToString().Trim());

                string lanResult = "";
                int languageIndex = 0;
                if (returnNum != 0)
                {
                    lanResult = Encoding.Default.GetString(bufferContent, 0, 1);
                    if ((lanResult.Trim().Equals("1")) || (lanResult.Trim().Equals("0")))
                    {
                        languageIndex = Convert.ToInt32(lanResult);
                    }
                }

                bool ret;
                System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out ret);
                if (ret)
                {
                    #region 配置 CefSharp 属性
                    var settings = new CefSettings();
                    settings.Locale = "zh-CN";

                    //缓存路径
                    var cache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.Combine("GsBrowser", "cache"));
                    if (Directory.Exists(cache))
                    {
                        try
                        {
                            CommonHelp.DeleteFolderAndContent(cache);   //清除缓存
                        }
                        catch (Exception ex)
                        {
                            LogHelp.WriteLog("清除缓存错误：" + ex.Message);
                            LogHelp.CreateLog(ex);                           
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(cache);
                    }
                    settings.CachePath = cache;

                    //日志文件
                    settings.PersistSessionCookies = true;
                    settings.LogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.Combine("GsBrowser", "log"));
                    settings.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
                    settings.UserDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.Combine("GsBrowser", "userData"));

                    //禁用 GPU 硬件加速
                    settings.CefCommandLineArgs.Add("disable-gpu", "1");
                    settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
                    settings.CefCommandLineArgs.Add("enable-begin-frame-scheduling", "1");
                    settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1"); //Disable Vsync

                    //禁用Windows DirectWrite字体渲染系统。
                    settings.CefCommandLineArgs.Add("disable-direct-write", "1");

                    Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
                    #endregion

                    if (string.IsNullOrWhiteSpace(ConfigHelp.GetItem("UserName")) || string.IsNullOrWhiteSpace(ConfigHelp.GetItem("Password")))
                    {
                        

                        if (string.IsNullOrWhiteSpace(ConfigHelp.GetItem("UserName")) || string.IsNullOrWhiteSpace(ConfigHelp.GetItem("Password")))
                        {
                            String path = AppDomain.CurrentDomain.BaseDirectory + "GsAutoUpdate.exe";    //程序路径
                            //同时启动自动更新程序
                            if (File.Exists(path))  //判断是否存在更新程序
                            {
                                ProcessStartInfo processStartInfo = new ProcessStartInfo()
                                {
                                    UseShellExecute = false,
                                    FileName = "GsAutoUpdate.exe",
                                    Arguments = "快点云Pos系统 0",   //参数信息，程序名称 0：弹窗更新 1：静默更新
                                };
                                Process proc = Process.Start(processStartInfo);
                                if (proc != null)
                                {
                                    proc.WaitForExit();
                                }
                            }
                        }
                        else
                        {
                            String path = AppDomain.CurrentDomain.BaseDirectory + "GsAutoUpdate.exe";    //程序路径
                            //同时启动自动更新程序
                            if (File.Exists(path))  //判断是否存在更新程序
                            {
                                ProcessStartInfo processStartInfo = new ProcessStartInfo()
                                {
                                    UseShellExecute = false,
                                    FileName = "GsAutoUpdate.exe",
                                    Arguments = Assembly.GetExecutingAssembly().GetName().Name + " 0",   //参数信息，程序名称 0：弹窗更新 1：静默更新
                                };
                                Process proc = Process.Start(processStartInfo);
                                if (proc != null)
                                {
                                    proc.WaitForExit();
                                }
                            }
                        }

                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new FormLogin());
                    }
                    else
                    {
                        String path = AppDomain.CurrentDomain.BaseDirectory + "GsAutoUpdate.exe";    //程序路径
                        if (string.IsNullOrWhiteSpace(ConfigHelp.GetItem("UserName")) || string.IsNullOrWhiteSpace(ConfigHelp.GetItem("Password")))
                        {                                                               //同时启动自动更新程序
                            if (File.Exists(path))  //判断是否存在更新程序
                            {
                                ProcessStartInfo processStartInfo = new ProcessStartInfo()
                                {
                                    UseShellExecute = false,
                                    FileName = "GsAutoUpdate.exe",
                                    Arguments = Assembly.GetExecutingAssembly().GetName().Name + " 0",   //参数信息，程序名称 0：弹窗更新 1：静默更新
                                };
                                Process proc = Process.Start(processStartInfo);
                                if (proc != null)
                                {
                                    proc.WaitForExit();
                                }
                            }
                        }
                        else
                        {                                                                    //同时启动自动更新程序
                            if (File.Exists(path))  //判断是否存在更新程序
                            {
                                ProcessStartInfo processStartInfo = new ProcessStartInfo()
                                {
                                    UseShellExecute = false,
                                    FileName = "GsAutoUpdate.exe",
                                    Arguments = "快点云Pos系统 1",   //参数信息，程序名称 0：弹窗更新 1：静默更新
                                };
                                Process proc = Process.Start(processStartInfo);
                                if (proc != null)
                                {
                                    proc.WaitForExit();
                                }
                            }
                        }

                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new FormBrowser());
                    }
                }
                else
                {
                    MessageBox.Show(null, "当前程序已运行，请不要重复运行本程序，本次运行即将退出。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Environment.Exit(0);//退出程序
                }
            }
            else // 用管理员用户运行 
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Application.ExecutablePath;
                startInfo.Arguments = string.Join(" ", Args);
                startInfo.Verb = "runas";
                Process.Start(startInfo);
                Environment.Exit(0);
            }
        }
    }
}
