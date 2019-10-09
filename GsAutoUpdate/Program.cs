using GsAutoUpdate.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace GsAutoUpdate
{
    internal static class Program
    {
        private static bool f;
        private static Process pCurrent = Process.GetCurrentProcess();
        private static Mutex m = new Mutex(true, pCurrent.MainModule.FileName.Replace(":", "").Replace(@"\", "") + "GsAutoUpdate", out f);//互斥，

        /// <summary>
        /// 程序主入口
        /// </summary>
        /// <param name="args">[0]程序名称，[1]静默更新 0：否 1：是</param>
        [STAThread]
        private static void Main(string[] args)
        {

            //异常处理
            BindExceptionHandler();

            LogHelp.AddLog($"----------程序启动----------{f}");
            for (int i = 0; i < args.Length; i++)
            {
                LogHelp.AddLog($"---------{i}-{args[i]}---------");
            }

            if (f)
            {
                try
                {
                    if (String.IsNullOrEmpty(args[0]) == false)
                    {
                        //加载待更新程序的信息
                        UpdateWork.updateAppInfo = new UpdateAppInfo()
                        {
                            AppName = args[0],                           
                            IsPopup = Convert.ToInt32(args[1]),
                            WatUpdateDirectory = args[2],
                            Hid = args[3]
                        };

                        LogHelp.AddLog($"---------UpdateWork.WatUpdateDirectory- {UpdateWork.updateAppInfo.WatUpdateDirectory}---------");

                        UpdateWork updateWork = new UpdateWork(args[0]);
                       
                        if (!Directory.Exists(UpdateWork.updateAppInfo.WatUpdateDirectory))
                        {
                            LogHelp.AddLog("---------待更新程序目录不存在----------");
                            Application.ExitThread();
                            Application.Exit();
                        }

                        if (updateWork.UpdateVerList != null && updateWork.UpdateVerList.Count > 0 )
                        {
                            /* 当前用户是管理员的时候，直接启动应用程序
                             * 如果不是管理员，则使用启动对象启动程序，以确保使用管理员身份运行
                             */
                            //获得当前登录的Windows用户标示
                            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                            //创建Windows用户主题
                            Application.EnableVisualStyles();
                            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);

                            //检测最新版本更新是否是必须更新，
                            var ismust = updateWork.UpdateVerList.Last().IsMust;

                            //判断当前登录用户是否为管理员
                            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
                            {
                                
                                //必须更新
                                if (ismust)
                                {
                                    if (args[1] == "0")  //弹出更新窗体
                                    {
                                        UpdateForm updateForm = new UpdateForm(updateWork);
                                        if (updateForm.ShowDialog() == DialogResult.OK)
                                        {
                                            Application.Exit();
                                        }
                                    }
                                    else //静默更新
                                    {
                                        updateWork.Do();
                                    }
                                }
                                else  if (args[1] == "1")  //直接更新
                                {
                                    updateWork.Do();
                                }
                                else
                                {
                                    Application.EnableVisualStyles();
                                    Application.SetCompatibleTextRenderingDefault(false);
                                    LogHelp.AddLog("---------管理员权限启动更新程序----------");
                                    Application.Run(new MainForm(updateWork));
                                }
                            }
                            else
                            {
                                String result = Environment.GetEnvironmentVariable("systemdrive");
                                if (AppDomain.CurrentDomain.BaseDirectory.Contains(result))
                                {
                                    //创建启动对象
                                    ProcessStartInfo startInfo = new ProcessStartInfo
                                    {
                                        //设置运行文件
                                        FileName = System.Windows.Forms.Application.ExecutablePath,
                                        //设置启动动作,确保以管理员身份运行
                                        Verb = "runas",
                                        Arguments = $"{args[0]} {args[1]} {args[2]} {args[3]}"
                                    };
                                   
                                    LogHelp.AddLog("----------启动UAC启动更新程序----------");

                                    //如果不是管理员，则启动UAC
                                    System.Diagnostics.Process.Start(startInfo);
                                }
                                else
                                {

                                    //必须更新
                                    if (ismust)
                                    {
                                        if (args[1] == "0")  //弹出更新窗体
                                        {
                                            UpdateForm updateForm = new UpdateForm(updateWork);
                                            if (updateForm.ShowDialog() == DialogResult.OK)
                                            {
                                                Application.Exit();
                                            }
                                        }
                                        else //静默更新
                                        {
                                            updateWork.Do();
                                        }
                                    }
                                    else if (args[1] == "1")  //直接更新
                                    {
                                        updateWork.Do();
                                    }
                                    else
                                    {
                                        Application.EnableVisualStyles();
                                        Application.SetCompatibleTextRenderingDefault(false);
                                        LogHelp.AddLog("----------非管理员权限启动更新程序----------");
                                        Application.Run(new MainForm(updateWork));
                                    }
                                }
                            }
                        }
                    }




                }
                catch (Exception ex)
                {
                    LogHelp.AddException(ex);
                    //MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 绑定程序中的异常处理
        /// </summary>
        private static void BindExceptionHandler()
        {
            //设置应用程序处理异常方式：ThreadException处理
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            //处理未捕获的异常
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
        /// <summary>
        /// 处理UI线程异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogHelp.AddException( e.Exception as Exception);
            MessageBox.Show("程序出现错误：" + e.Exception.Message, "捷信达更新程序");
            //Environment.Exit(0);//退出程序
        }
        /// <summary>
        /// 处理未捕获的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogHelp.AddException(e.ExceptionObject as Exception);
            MessageBox.Show("程序出现错误：" + (e.ExceptionObject as Exception).Message, "捷信达更新程序");
           // Environment.Exit(0);//退出程序
        }



    }
}