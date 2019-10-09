using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PrintSqlite
{
    internal static class Program
    {
        #region Setting from ini document

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string Section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string Section, string key, string def, /*StringBuilder reVal原来用StringBuilder*/byte[] buffer/*现在使用byte串*/, int Usize, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        #endregion Setting from ini document

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main(string[] Args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
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

                bool canRun = canRunWithSingleInstance();
                if (canRun)
                {
                    //更新后还原用户参数
                    UpdateWork.RestoreAppPara();
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FrmMain());
                } else
                {
                    MessageBox.Show(null, "当前程序已运行，请不要重复运行本程序，本次运行即将退出。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Application.ExitThread();
                    Application.Exit();
                    Process.GetCurrentProcess().Kill();
                }
            }
            else // 用管理员用户运行 
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Application.ExecutablePath;
                startInfo.Arguments = string.Join(" ", Args);
                startInfo.Verb = "runas";
                Process.Start(startInfo);
                Application.ExitThread();
                Application.Exit();
                Process.GetCurrentProcess().Kill();
            }
        }

        private static bool canRunWithSingleInstance()
        {
            bool canRun = true;
            var allProcesses = Process.GetProcesses();
            foreach(var process in allProcesses)
            {
              
                if(process.ProcessName == Application.ProductName && process.Id != Process.GetCurrentProcess().Id)
                {
                    canRun = false;
                }
            }
            return canRun;
        }
    }
}