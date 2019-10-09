using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace PrintSqlite
{
    public partial class FrmQuery : Form
    {

        public FrmQuery()
        {
            InitializeComponent();
        }

        private void FrmQuery_Load(object sender, EventArgs e)
        {
            dtpDate.MaxDate = DateTime.Now.Date;

            btnQuery_Click(null, null);
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                //获取本地配置的打印机信息
                DataTable prints = CommonHelp.GetPrintDataTable();

                #region 获取本地的出品列表
                var sql = string.Format("SELECT * FROM ProductRecord WHERE OrderTime >= '{0}' AND OrderTime < '{1}';", Convert.ToDateTime(dtpDate.Text).Date.ToString("yyyy-MM-dd"), Convert.ToDateTime(dtpDate.Text).AddDays(1).ToString("yyyy-MM-dd"));
                var dt = SQLiteHelper.GetDataTable(sql);
                Console.Write(dt.Rows.Count);
                #endregion

                var refe = txtRefe.Text.Trim();
                var tab = txtTab.Text.Trim();
                var name = txtName.Text.Trim();
                var bill = txtBill.Text.Trim();
                var port = txtPort.Text.Trim();

                var list = (from t in dt.AsEnumerable()//查询
                            where ((t.Field<string>("BusinessPointName").Contains(refe) || string.IsNullOrEmpty(refe))
                               && (t.Field<string>("TableNumber").Contains(tab) || string.IsNullOrEmpty(tab))
                              && (t.Field<string>("ProjectName").Contains(name) || string.IsNullOrEmpty(name))
                              && (t.Field<string>("OddNumbers").Contains(bill) || string.IsNullOrEmpty(bill))
                              && (t.Field<string>("ProducePort").Contains(port) || string.IsNullOrEmpty(port)))
                            select t).ToList();

                dgvPrintQuery.DataSource = dt;
                if (list != null && list.Count > 0)
                {
                    dgvPrintQuery.DataSource = list.CopyToDataTable();
                    dgvPrintQuery.Columns["HotelCode"].Visible = false;
                    dgvPrintQuery.Columns["FlowNumber"].Visible = false;
                    dgvPrintQuery.Columns["OddNumbers"].Visible = false;
                    dgvPrintQuery.Columns["ConsumptionID"].Visible = false;
                    dgvPrintQuery.Columns["BusinessPointCode"].Visible = false;

                    for (int i = 0; i < dgvPrintQuery.Rows.Count; i++)
                    {
                        dgvPrintQuery.Rows[i].Cells["SerialNumber"].Value = i + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                tsslTips.Text = $"提示({DateTime.Now})：{ex.Message}";
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
            }
        }
    }
}
