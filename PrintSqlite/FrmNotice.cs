using System;
using System.Windows.Forms;

namespace PrintSqlite
{
    public partial class FrmNotice : Form
    {
        public FrmNotice()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            stiDesignerControl1.TestReportSave();
            Hide();
        }
    }
}
