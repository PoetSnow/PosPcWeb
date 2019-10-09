using System;
using System.IO;
using System.Windows.Forms;

namespace GsBrowser
{
    public partial class FormSet : Form
    {
        public FormSet()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCache_Click(object sender, EventArgs e)
        {
            try
            {
                //缓存路径
                var cache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.Combine("GsBrowser", "cache"));
                //删除缓存文件
                CommonHelp.DeleteFolderAndContent(cache);
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }

        /// <summary>
        /// 打印机设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrinter_Click(object sender, EventArgs e)
        {
            try
            {
                if (Application.OpenForms["FormPrinter"] == null)
                {
                    FormPrinter form = new FormPrinter();
                    form.Show();
                    form.Activate();
                }
                else
                {
                    Application.OpenForms["FormPrinter"].Show();
                    Application.OpenForms["FormPrinter"].Activate();
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }

        }

        /// <summary>
        /// 登录信息设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (Application.OpenForms["FormLogin"] == null)
                {
                    FormLogin form = new FormLogin();
                    form.Show();
                    form.Activate();
                }
                else
                {
                    Application.OpenForms["FormLogin"].Show();
                    Application.OpenForms["FormLogin"].Activate();
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }

        }

        /// <summary>
        /// 本地参数设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLocal_Click(object sender, EventArgs e)
        {
            try
            {
                if (Application.OpenForms["FormLocal"] == null)
                {
                    FormLocal form = new FormLocal();
                    form.Show();
                    form.Activate();
                }
                else
                {
                    Application.OpenForms["FormLocal"].Show();
                    Application.OpenForms["FormLocal"].Activate();
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }

        }
    }
}