namespace GsBrowser
{
    partial class FormPrinter
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.cbxPrinter = new System.Windows.Forms.ComboBox();
            this.stiDesignerControl1 = new Stimulsoft.Report.Design.StiDesignerControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "点菜单打印机：";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(861, 726);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(135, 25);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "保存格式并关闭";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbxPrinter
            // 
            this.cbxPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPrinter.FormattingEnabled = true;
            this.cbxPrinter.Location = new System.Drawing.Point(108, 12);
            this.cbxPrinter.Name = "cbxPrinter";
            this.cbxPrinter.Size = new System.Drawing.Size(210, 20);
            this.cbxPrinter.TabIndex = 2;
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
            this.stiDesignerControl1.Size = new System.Drawing.Size(984, 682);
            this.stiDesignerControl1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.stiDesignerControl1);
            this.panel1.Location = new System.Drawing.Point(12, 38);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(984, 682);
            this.panel1.TabIndex = 4;
            // 
            // FormPrinter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 761);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cbxPrinter);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label1);
            this.Name = "FormPrinter";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "点菜单";
            this.Load += new System.EventHandler(this.SetPrinter_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cbxPrinter;
        internal Stimulsoft.Report.Design.StiDesignerControl stiDesignerControl1;
        private System.Windows.Forms.Panel panel1;
    }
}