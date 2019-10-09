using CefSharp;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace GsBrowser
{
    /// <summary>
    /// 键盘控制台（快捷键处理）
    /// </summary>
    public class KeyBoardHandler : IKeyboardHandler
    {
        public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            return false;
        }

        public bool OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            if (modifiers == CefEventFlags.ShiftDown && windowsKeyCode == (int)Keys.Escape)   //组合键 Shift + Esc 退出程序
            {
                //DialogResult dr = MessageBox.Show("确认退出本程序吗？", "快点云Pos提示", MessageBoxButtons.OKCancel);
                //if (dr == DialogResult.OK)
                //{
                //    try
                //    {
                //        Application.ExitThread();
                //        Application.Exit();
                //        Process.GetCurrentProcess().Kill();
                //    }
                //    finally { browser.Dispose(); }
                //}
                Process current = Process.GetCurrentProcess();
                ProcessThreadCollection allThreads = current.Threads;
                current.CloseMainWindow();
                return true;
            }
            else if (windowsKeyCode == (int)Keys.F5)    //刷新页面
            {
                browser.Reload();
                return true;
            }
            else if (modifiers == CefEventFlags.ControlDown && windowsKeyCode == (int)Keys.F5)
            {
                browser.Reload(true);
                return true;
            }
            else if (modifiers == CefEventFlags.ControlDown && windowsKeyCode == (int)Keys.F11)   //设置界面
            {
                if (Application.OpenForms["FormSet"] == null)
                {
                    FormSet form = new FormSet();
                    form.Show();
                    form.Activate();
                }
                else
                {
                    Application.OpenForms["FormSet"].Show();
                    Application.OpenForms["FormSet"].Activate();
                }
                return true;
            }
#if DEBUG 
            #region 启用开发者工具（Ctrl + F12）
            else if (modifiers == CefEventFlags.ControlDown && windowsKeyCode == (int)Keys.F12)
            {
                foreach (Form temp in Application.OpenForms)
                {
                    if (temp.Name == "FormBrowser")
                    {
                        ((FormBrowser)temp).browser.ShowDevTools();
                        break;
                    }
                }
                return true;
            }
            #endregion
#endif

            return false;
        }
    }
}
