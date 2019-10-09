namespace PrintSqlite
{
    partial class FrmNotice
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
            this.stiDesignerControl1 = new Stimulsoft.Report.Design.StiDesignerControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // stiDesignerControl1
            // 
            this.stiDesignerControl1.BackColor = System.Drawing.Color.White;
            this.stiDesignerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stiDesignerControl1.Location = new System.Drawing.Point(0, 0);
            this.stiDesignerControl1.Name = "stiDesignerControl1";
            this.stiDesignerControl1.ShowMainMenu = false;
            this.stiDesignerControl1.ShowToolbarBorders = false;
            this.stiDesignerControl1.ShowToolbarLayout = false;
            this.stiDesignerControl1.ShowToolbarStandard = false;
            this.stiDesignerControl1.ShowToolbarStyle = false;
            this.stiDesignerControl1.Size = new System.Drawing.Size(760, 506);
            this.stiDesignerControl1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.stiDesignerControl1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(760, 506);
            this.panel1.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(637, 524);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(135, 25);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "保存格式并关闭";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FrmNotice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.panel1);
            this.Name = "FrmNotice";
            this.ShowInTaskbar = false;
            this.Text = "设置票据格式";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public Stimulsoft.Report.Design.StiDesignerControl stiDesignerControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSave;
    }
}