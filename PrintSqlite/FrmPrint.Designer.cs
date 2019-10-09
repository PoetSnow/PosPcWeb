namespace PrintSqlite
{
    partial class FrmPrint
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnSave = new System.Windows.Forms.Button();
            this.dgvPrint = new System.Windows.Forms.DataGridView();
            this.序号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.端口 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.名称 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.打印机 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.出品格式 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.备用端口 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.关联端口 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.逐条打印 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiPrintTest = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelPrint = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveSet = new System.Windows.Forms.Button();
            this.txtSecretKey = new System.Windows.Forms.TextBox();
            this.lblSecretKey = new System.Windows.Forms.Label();
            this.txtInterface = new System.Windows.Forms.TextBox();
            this.lblInterface = new System.Windows.Forms.Label();
            this.txtHid = new System.Windows.Forms.TextBox();
            this.lblHid = new System.Windows.Forms.Label();
            this.lblHidTip = new System.Windows.Forms.Label();
            this.lblSecretKeyTip = new System.Windows.Forms.Label();
            this.lblInterfaceTip = new System.Windows.Forms.Label();
            this.lblPollTime = new System.Windows.Forms.Label();
            this.txtPollTime = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPrintTime = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslTips = new System.Windows.Forms.ToolStripStatusLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSection = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrint)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(847, 656);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(150, 30);
            this.btnSave.TabIndex = 31;
            this.btnSave.Text = "保存端口并关闭";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dgvPrint
            // 
            this.dgvPrint.AllowUserToAddRows = false;
            this.dgvPrint.AllowUserToDeleteRows = false;
            this.dgvPrint.AllowUserToOrderColumns = true;
            this.dgvPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPrint.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrint.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.序号,
            this.Id,
            this.端口,
            this.名称,
            this.打印机,
            this.出品格式,
            this.备用端口,
            this.关联端口,
            this.逐条打印});
            this.dgvPrint.ContextMenuStrip = this.contextMenuStrip1;
            this.dgvPrint.Location = new System.Drawing.Point(12, 211);
            this.dgvPrint.Name = "dgvPrint";
            this.dgvPrint.RowTemplate.Height = 23;
            this.dgvPrint.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPrint.Size = new System.Drawing.Size(984, 437);
            this.dgvPrint.TabIndex = 30;
            this.dgvPrint.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvPrint_CellMouseDown);
            // 
            // 序号
            // 
            this.序号.HeaderText = "序号";
            this.序号.Name = "序号";
            this.序号.Width = 40;
            // 
            // Id
            // 
            this.Id.FillWeight = 48.86972F;
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.Width = 50;
            // 
            // 端口
            // 
            this.端口.FillWeight = 63.23319F;
            this.端口.HeaderText = "端口";
            this.端口.Name = "端口";
            this.端口.Width = 60;
            // 
            // 名称
            // 
            this.名称.FillWeight = 82.30656F;
            this.名称.HeaderText = "名称";
            this.名称.Name = "名称";
            this.名称.Width = 90;
            // 
            // 打印机
            // 
            this.打印机.FillWeight = 84.45042F;
            this.打印机.HeaderText = "打印机";
            this.打印机.Name = "打印机";
            this.打印机.Width = 190;
            // 
            // 出品格式
            // 
            this.出品格式.HeaderText = "出品格式";
            this.出品格式.Name = "出品格式";
            this.出品格式.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // 备用端口
            // 
            this.备用端口.FillWeight = 108.8126F;
            this.备用端口.HeaderText = "备用端口";
            this.备用端口.Name = "备用端口";
            this.备用端口.ToolTipText = "StandbyPort";
            this.备用端口.Width = 80;
            // 
            // 关联端口
            // 
            this.关联端口.FillWeight = 134.6627F;
            this.关联端口.HeaderText = "关联端口";
            this.关联端口.Name = "关联端口";
            this.关联端口.Width = 80;
            // 
            // 逐条打印
            // 
            this.逐条打印.FillWeight = 177.665F;
            this.逐条打印.HeaderText = "逐条打印";
            this.逐条打印.Name = "逐条打印";
            this.逐条打印.Width = 80;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiPrintTest,
            this.tsmiDelPrint});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(137, 48);
            // 
            // tsmiPrintTest
            // 
            this.tsmiPrintTest.Name = "tsmiPrintTest";
            this.tsmiPrintTest.Size = new System.Drawing.Size(136, 22);
            this.tsmiPrintTest.Text = "打印测试页";
            this.tsmiPrintTest.Click += new System.EventHandler(this.tsmiPrintTest_Click);
            // 
            // tsmiDelPrint
            // 
            this.tsmiDelPrint.Name = "tsmiDelPrint";
            this.tsmiDelPrint.Size = new System.Drawing.Size(136, 22);
            this.tsmiDelPrint.Text = "删除打印机";
            this.tsmiDelPrint.Click += new System.EventHandler(this.tsmiDelPrint_Click);
            // 
            // btnSaveSet
            // 
            this.btnSaveSet.Location = new System.Drawing.Point(82, 175);
            this.btnSaveSet.Name = "btnSaveSet";
            this.btnSaveSet.Size = new System.Drawing.Size(297, 30);
            this.btnSaveSet.TabIndex = 29;
            this.btnSaveSet.Text = "测试连接并刷新列表";
            this.btnSaveSet.UseVisualStyleBackColor = true;
            this.btnSaveSet.Click += new System.EventHandler(this.btnSaveSet_Click);
            // 
            // txtSecretKey
            // 
            this.txtSecretKey.Location = new System.Drawing.Point(82, 39);
            this.txtSecretKey.Name = "txtSecretKey";
            this.txtSecretKey.Size = new System.Drawing.Size(294, 21);
            this.txtSecretKey.TabIndex = 28;
            this.txtSecretKey.TextChanged += new System.EventHandler(this.txtSecretKey_TextChanged);
            // 
            // lblSecretKey
            // 
            this.lblSecretKey.AutoSize = true;
            this.lblSecretKey.Location = new System.Drawing.Point(11, 42);
            this.lblSecretKey.Name = "lblSecretKey";
            this.lblSecretKey.Size = new System.Drawing.Size(65, 12);
            this.lblSecretKey.TabIndex = 27;
            this.lblSecretKey.Text = "出品秘钥：";
            // 
            // txtInterface
            // 
            this.txtInterface.Location = new System.Drawing.Point(82, 66);
            this.txtInterface.Name = "txtInterface";
            this.txtInterface.ReadOnly = true;
            this.txtInterface.Size = new System.Drawing.Size(294, 21);
            this.txtInterface.TabIndex = 26;
            this.txtInterface.TextChanged += new System.EventHandler(this.txtInterface_TextChanged);
            this.txtInterface.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txtInterface_MouseDoubleClick);
            // 
            // lblInterface
            // 
            this.lblInterface.AutoSize = true;
            this.lblInterface.Location = new System.Drawing.Point(11, 70);
            this.lblInterface.Name = "lblInterface";
            this.lblInterface.Size = new System.Drawing.Size(65, 12);
            this.lblInterface.TabIndex = 25;
            this.lblInterface.Text = "接口地址：";
            // 
            // txtHid
            // 
            this.txtHid.Location = new System.Drawing.Point(82, 12);
            this.txtHid.Name = "txtHid";
            this.txtHid.Size = new System.Drawing.Size(100, 21);
            this.txtHid.TabIndex = 24;
            this.txtHid.TextChanged += new System.EventHandler(this.txtHid_TextChanged);
            // 
            // lblHid
            // 
            this.lblHid.AutoSize = true;
            this.lblHid.Location = new System.Drawing.Point(12, 15);
            this.lblHid.Name = "lblHid";
            this.lblHid.Size = new System.Drawing.Size(65, 12);
            this.lblHid.TabIndex = 23;
            this.lblHid.Text = "酒店代码：";
            // 
            // lblHidTip
            // 
            this.lblHidTip.AutoSize = true;
            this.lblHidTip.BackColor = System.Drawing.Color.Transparent;
            this.lblHidTip.ForeColor = System.Drawing.Color.Red;
            this.lblHidTip.Location = new System.Drawing.Point(188, 16);
            this.lblHidTip.Name = "lblHidTip";
            this.lblHidTip.Size = new System.Drawing.Size(101, 12);
            this.lblHidTip.TabIndex = 32;
            this.lblHidTip.Text = "* 请输入酒店代码";
            this.lblHidTip.Visible = false;
            // 
            // lblSecretKeyTip
            // 
            this.lblSecretKeyTip.AutoSize = true;
            this.lblSecretKeyTip.ForeColor = System.Drawing.Color.Red;
            this.lblSecretKeyTip.Location = new System.Drawing.Point(382, 43);
            this.lblSecretKeyTip.Name = "lblSecretKeyTip";
            this.lblSecretKeyTip.Size = new System.Drawing.Size(77, 12);
            this.lblSecretKeyTip.TabIndex = 33;
            this.lblSecretKeyTip.Text = "* 请输入秘钥";
            this.lblSecretKeyTip.Visible = false;
            // 
            // lblInterfaceTip
            // 
            this.lblInterfaceTip.AutoSize = true;
            this.lblInterfaceTip.ForeColor = System.Drawing.Color.Red;
            this.lblInterfaceTip.Location = new System.Drawing.Point(382, 70);
            this.lblInterfaceTip.Name = "lblInterfaceTip";
            this.lblInterfaceTip.Size = new System.Drawing.Size(101, 12);
            this.lblInterfaceTip.TabIndex = 34;
            this.lblInterfaceTip.Text = "* 请输入接口地址";
            this.lblInterfaceTip.Visible = false;
            // 
            // lblPollTime
            // 
            this.lblPollTime.AutoSize = true;
            this.lblPollTime.Location = new System.Drawing.Point(12, 125);
            this.lblPollTime.Name = "lblPollTime";
            this.lblPollTime.Size = new System.Drawing.Size(65, 12);
            this.lblPollTime.TabIndex = 35;
            this.lblPollTime.Text = "轮询间隔：";
            // 
            // txtPollTime
            // 
            this.txtPollTime.Location = new System.Drawing.Point(82, 121);
            this.txtPollTime.Name = "txtPollTime";
            this.txtPollTime.Size = new System.Drawing.Size(100, 21);
            this.txtPollTime.TabIndex = 36;
            this.txtPollTime.Text = "5";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(188, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 12);
            this.label1.TabIndex = 37;
            this.label1.Text = "请求出品数据的间隔时间，默认5秒";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(188, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(191, 12);
            this.label4.TabIndex = 43;
            this.label4.Text = "打印机最新数据间隔时间，默认3秒";
            // 
            // txtPrintTime
            // 
            this.txtPrintTime.Location = new System.Drawing.Point(82, 148);
            this.txtPrintTime.Name = "txtPrintTime";
            this.txtPrintTime.Size = new System.Drawing.Size(100, 21);
            this.txtPrintTime.TabIndex = 42;
            this.txtPrintTime.Text = "3";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 41;
            this.label5.Text = "打印间隔：";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslTips});
            this.statusStrip1.Location = new System.Drawing.Point(0, 691);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1008, 22);
            this.statusStrip1.TabIndex = 44;
            // 
            // tsslTips
            // 
            this.tsslTips.ForeColor = System.Drawing.Color.Red;
            this.tsslTips.Name = "tsslTips";
            this.tsslTips.Size = new System.Drawing.Size(32, 17);
            this.tsslTips.Text = "提示";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(188, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(185, 12);
            this.label2.TabIndex = 47;
            this.label2.Text = "选填（A-Z），多分区使用：A,B,C";
            // 
            // txtSection
            // 
            this.txtSection.Location = new System.Drawing.Point(82, 93);
            this.txtSection.Name = "txtSection";
            this.txtSection.Size = new System.Drawing.Size(100, 21);
            this.txtSection.TabIndex = 46;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 45;
            this.label3.Text = "出品分区：";
            // 
            // FrmPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 713);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSection);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPrintTime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPollTime);
            this.Controls.Add(this.lblPollTime);
            this.Controls.Add(this.lblInterfaceTip);
            this.Controls.Add(this.lblSecretKeyTip);
            this.Controls.Add(this.lblHidTip);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvPrint);
            this.Controls.Add(this.btnSaveSet);
            this.Controls.Add(this.txtSecretKey);
            this.Controls.Add(this.lblSecretKey);
            this.Controls.Add(this.txtInterface);
            this.Controls.Add(this.lblInterface);
            this.Controls.Add(this.txtHid);
            this.Controls.Add(this.lblHid);
            this.Name = "FrmPrint";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置打印参数";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPrint_FormClosing);
            this.Load += new System.EventHandler(this.PrintForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrint)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridView dgvPrint;
        private System.Windows.Forms.Button btnSaveSet;
        private System.Windows.Forms.TextBox txtSecretKey;
        private System.Windows.Forms.Label lblSecretKey;
        private System.Windows.Forms.TextBox txtInterface;
        private System.Windows.Forms.Label lblInterface;
        private System.Windows.Forms.TextBox txtHid;
        private System.Windows.Forms.Label lblHid;
        private System.Windows.Forms.Label lblHidTip;
        private System.Windows.Forms.Label lblSecretKeyTip;
        private System.Windows.Forms.Label lblInterfaceTip;
        private System.Windows.Forms.Label lblPollTime;
        private System.Windows.Forms.TextBox txtPollTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPrintTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripStatusLabel tsslTips;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelPrint;
        private System.Windows.Forms.ToolStripMenuItem tsmiPrintTest;
        private System.Windows.Forms.DataGridViewTextBoxColumn 序号;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn 端口;
        private System.Windows.Forms.DataGridViewTextBoxColumn 名称;
        private System.Windows.Forms.DataGridViewComboBoxColumn 打印机;
        private System.Windows.Forms.DataGridViewComboBoxColumn 出品格式;
        private System.Windows.Forms.DataGridViewTextBoxColumn 备用端口;
        private System.Windows.Forms.DataGridViewTextBoxColumn 关联端口;
        private System.Windows.Forms.DataGridViewCheckBoxColumn 逐条打印;
    }
}