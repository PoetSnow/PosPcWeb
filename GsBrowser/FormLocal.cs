using System;
using System.Threading;
using System.Windows.Forms;

namespace GsBrowser
{
    public partial class FormLocal : Form
    {
        public FormLocal()
        {
            InitializeComponent();
        }

        private void FormLocal_Load(object sender, EventArgs e)
        {
            //全屏
            if (!string.IsNullOrWhiteSpace(ConfigHelp.GetItem("IsFullScreen")))
            {
                cbxIsFullScreen.Checked = Convert.ToBoolean(ConfigHelp.GetItem("IsFullScreen"));
            }
            else
            {
                ConfigHelp.AddItem("IsFullScreen");
            }

            //屏幕键盘
            if (!string.IsNullOrWhiteSpace(ConfigHelp.GetItem("EnableKeyboard")))
            {
                cbxEnableKeyboard.Checked = Convert.ToBoolean(ConfigHelp.GetItem("EnableKeyboard"));
            }
            else
            {
                ConfigHelp.AddItem("EnableKeyboard");
            }
            //屏幕键盘
            if (!string.IsNullOrWhiteSpace(ConfigHelp.GetItem("IsHandwrite")))
            {
                cbxIsHandwrite.Checked = Convert.ToBoolean(ConfigHelp.GetItem("IsHandwrite"));
            }
            else
            {
                ConfigHelp.AddItem("IsHandwrite");
            }
        }

        /// <summary>
        /// 更新启用全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxIsFullScreen_CheckedChanged(object sender, EventArgs e)
        {
            //保存修改到配置文件
            if (!string.IsNullOrWhiteSpace(ConfigHelp.GetItem("IsFullScreen")))
            {
                ConfigHelp.UpdateItem("IsFullScreen", cbxIsFullScreen.Checked.ToString());
            }
            else
            {
                ConfigHelp.AddItem("IsFullScreen", cbxIsFullScreen.Checked.ToString());
            }

            //更新窗口状态
            var form = Application.OpenForms["FormBrowser"];
            if (form == null)
            {
                form = new FormBrowser();
                if (cbxIsFullScreen.Checked)
                {
                    form.Hide();
                    Thread.Sleep(150);
                    form.FormBorderStyle = FormBorderStyle.None;
                    form.Show();
                }
                else
                {
                    form.FormBorderStyle = FormBorderStyle.FixedSingle;
                    form.Show();
                }
            }
            else
            {
                if (cbxIsFullScreen.Checked)
                {
                    if (form.FormBorderStyle != FormBorderStyle.None)
                    {
                        form.Hide();
                        Thread.Sleep(150);
                        form.FormBorderStyle = FormBorderStyle.None;
                        form.Show();
                    }
                }
                else
                {
                    if (form.FormBorderStyle != FormBorderStyle.FixedSingle)
                    {
                        form.FormBorderStyle = FormBorderStyle.FixedSingle;
                        form.Show();
                    }
                }
            }
            TopMost = true;
        }

        /// <summary>
        /// 更新启用屏幕键盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxEnableKeyboard_CheckedChanged(object sender, EventArgs e)
        {
            //保存修改到配置文件
            if (!string.IsNullOrWhiteSpace(ConfigHelp.GetItem("EnableKeyboard")))
            {
                ConfigHelp.UpdateItem("EnableKeyboard", cbxEnableKeyboard.Checked.ToString());
            }
            else
            {
                ConfigHelp.AddItem("EnableKeyboard", cbxEnableKeyboard.Checked.ToString());
            }

            //更新窗口状态
            var form = Application.OpenForms["FormBrowser"];
            if (form == null)
            {
                FormBrowser newForm = new FormBrowser();
                newForm.jsObject.EnableKeyboard = cbxEnableKeyboard.Checked;
            }
            else
            {
                var newForm = ((FormBrowser)Application.OpenForms["FormBrowser"]);
                newForm.jsObject.EnableKeyboard = cbxEnableKeyboard.Checked;
            }
        }

        private void cbxIsHandwrite_CheckedChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ConfigHelp.GetItem("IsHandwrite")))
            {
                ConfigHelp.UpdateItem("IsHandwrite", cbxIsHandwrite.Checked.ToString());
            }
            else
            {
                ConfigHelp.AddItem("IsHandwrite", cbxIsHandwrite.Checked.ToString());
            }

            //更新窗口状态
            var form = Application.OpenForms["FormBrowser"];
            if (form == null)
            {
                FormBrowser newForm = new FormBrowser();
                newForm.jsObject.IsHandwrite = cbxIsHandwrite.Checked;
            }
            else
            {
                var newForm = ((FormBrowser)Application.OpenForms["FormBrowser"]);
                newForm.jsObject.IsHandwrite = cbxIsHandwrite.Checked;
            }
        }

        private void FormLocal_Deactivate(object sender, EventArgs e)
        {
            Close();
        }
    }
}
