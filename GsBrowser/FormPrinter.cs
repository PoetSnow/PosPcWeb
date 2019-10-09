using System;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace GsBrowser
{
    public partial class FormPrinter : Form
    {
        public FormPrinter()
        {
            InitializeComponent();
        }

        private void SetPrinter_Load(object sender, EventArgs e)
        {
            try
            {
                bool selected = true;
                var printer = ConfigHelp.GetItem("Printer");
                for (var i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
                {
                    cbxPrinter.Items.Add(PrinterSettings.InstalledPrinters[i]);
                    if (PrinterSettings.InstalledPrinters[i] == printer)
                    {
                        cbxPrinter.SelectedIndex = i;
                        selected = false;
                    }
                }

                if (selected && cbxPrinter.Items.Count > 0)
                {
                    cbxPrinter.SelectedIndex = 0;
                }

                stiDesignerControl1.OpenFile(Application.StartupPath + "\\report\\PosTheMenu.mrt");
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                //throw;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                stiDesignerControl1.TestReportSave();
                if (cbxPrinter.SelectedItem != null)
                {
                    var text = cbxPrinter.Text.ToString();
                    ConfigHelp.UpdateItem("Printer", text);
                }
                Hide();
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                //throw;
            }
        }

        private void SetPrinter_FormClosed(object sender, FormClosedEventArgs e)
        {
            btnSave_Click(null, null);
        }
    }
}
