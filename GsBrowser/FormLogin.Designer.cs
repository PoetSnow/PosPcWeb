namespace GsBrowser
{
    partial class FormLogin
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.rdbKeyboardEnable = new System.Windows.Forms.RadioButton();
            this.rdbKeyboardDisable = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.lblGroupId = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtHid = new System.Windows.Forms.TextBox();
            this.gbxMode = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.rdbFloor = new System.Windows.Forms.RadioButton();
            this.cbxBranch = new System.Windows.Forms.ComboBox();
            this.rdbInSingle = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.cbxPos = new System.Windows.Forms.ComboBox();
            this.rdbDefault = new System.Windows.Forms.RadioButton();
            this.panBranch = new System.Windows.Forms.Panel();
            this.rdbCashier = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.panBranch.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.rdbKeyboardEnable);
            this.panel1.Controls.Add(this.rdbKeyboardDisable);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.lblGroupId);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnLogin);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtPassword);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtUserName);
            this.panel1.Controls.Add(this.txtHid);
            this.panel1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel1.Location = new System.Drawing.Point(13, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(320, 154);
            this.panel1.TabIndex = 7;
            // 
            // rdbKeyboardEnable
            // 
            this.rdbKeyboardEnable.AutoSize = true;
            this.rdbKeyboardEnable.Location = new System.Drawing.Point(86, 94);
            this.rdbKeyboardEnable.Name = "rdbKeyboardEnable";
            this.rdbKeyboardEnable.Size = new System.Drawing.Size(47, 16);
            this.rdbKeyboardEnable.TabIndex = 18;
            this.rdbKeyboardEnable.Text = "启用";
            this.rdbKeyboardEnable.UseVisualStyleBackColor = true;
            this.rdbKeyboardEnable.CheckedChanged += new System.EventHandler(this.rdbKeyboardEnable_CheckedChanged);
            // 
            // rdbKeyboardDisable
            // 
            this.rdbKeyboardDisable.AutoSize = true;
            this.rdbKeyboardDisable.Checked = true;
            this.rdbKeyboardDisable.Location = new System.Drawing.Point(139, 94);
            this.rdbKeyboardDisable.Name = "rdbKeyboardDisable";
            this.rdbKeyboardDisable.Size = new System.Drawing.Size(47, 16);
            this.rdbKeyboardDisable.TabIndex = 17;
            this.rdbKeyboardDisable.TabStop = true;
            this.rdbKeyboardDisable.Text = "禁用";
            this.rdbKeyboardDisable.UseVisualStyleBackColor = true;
            this.rdbKeyboardDisable.CheckedChanged += new System.EventHandler(this.rdbKeyboardEnable_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 16;
            this.label7.Text = "屏幕键盘：";
            // 
            // lblGroupId
            // 
            this.lblGroupId.AutoSize = true;
            this.lblGroupId.Location = new System.Drawing.Point(29, 123);
            this.lblGroupId.Name = "lblGroupId";
            this.lblGroupId.Size = new System.Drawing.Size(47, 12);
            this.lblGroupId.TabIndex = 7;
            this.lblGroupId.Text = "GroupId";
            this.lblGroupId.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(11, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "酒店代码：";
            // 
            // btnLogin
            // 
            this.btnLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogin.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLogin.Location = new System.Drawing.Point(84, 114);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(224, 30);
            this.btnLogin.TabIndex = 6;
            this.btnLogin.Text = "获取餐饮类型";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(23, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "用户名：";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPassword.Location = new System.Drawing.Point(84, 65);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(224, 21);
            this.txtPassword.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(35, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "密码：";
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtUserName.Location = new System.Drawing.Point(84, 38);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(224, 21);
            this.txtUserName.TabIndex = 4;
            // 
            // txtHid
            // 
            this.txtHid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHid.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtHid.Location = new System.Drawing.Point(84, 11);
            this.txtHid.Name = "txtHid";
            this.txtHid.Size = new System.Drawing.Size(224, 21);
            this.txtHid.TabIndex = 3;
            // 
            // gbxMode
            // 
            this.gbxMode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxMode.AutoSize = true;
            this.gbxMode.Enabled = false;
            this.gbxMode.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gbxMode.Location = new System.Drawing.Point(13, 266);
            this.gbxMode.Name = "gbxMode";
            this.gbxMode.Size = new System.Drawing.Size(320, 59);
            this.gbxMode.TabIndex = 9;
            this.gbxMode.TabStop = false;
            this.gbxMode.Text = "模式";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(35, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "分店：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "启动页：";
            // 
            // rdbFloor
            // 
            this.rdbFloor.AutoSize = true;
            this.rdbFloor.Location = new System.Drawing.Point(139, 60);
            this.rdbFloor.Name = "rdbFloor";
            this.rdbFloor.Size = new System.Drawing.Size(47, 16);
            this.rdbFloor.TabIndex = 14;
            this.rdbFloor.Tag = "p10";
            this.rdbFloor.Text = "楼面";
            this.rdbFloor.UseVisualStyleBackColor = true;
            // 
            // cbxBranch
            // 
            this.cbxBranch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBranch.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbxBranch.FormattingEnabled = true;
            this.cbxBranch.Location = new System.Drawing.Point(86, 6);
            this.cbxBranch.Name = "cbxBranch";
            this.cbxBranch.Size = new System.Drawing.Size(222, 20);
            this.cbxBranch.TabIndex = 11;
            this.cbxBranch.SelectedIndexChanged += new System.EventHandler(this.cbxBranch_SelectedIndexChanged);
            // 
            // rdbInSingle
            // 
            this.rdbInSingle.AutoSize = true;
            this.rdbInSingle.Location = new System.Drawing.Point(192, 60);
            this.rdbInSingle.Name = "rdbInSingle";
            this.rdbInSingle.Size = new System.Drawing.Size(47, 16);
            this.rdbInSingle.TabIndex = 15;
            this.rdbInSingle.Tag = "p60";
            this.rdbInSingle.Text = "入单";
            this.rdbInSingle.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(23, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 16;
            this.label6.Text = "收银点：";
            // 
            // cbxPos
            // 
            this.cbxPos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPos.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbxPos.FormattingEnabled = true;
            this.cbxPos.Location = new System.Drawing.Point(86, 32);
            this.cbxPos.Name = "cbxPos";
            this.cbxPos.Size = new System.Drawing.Size(222, 20);
            this.cbxPos.TabIndex = 17;
            this.cbxPos.SelectedIndexChanged += new System.EventHandler(this.cbxPos_SelectedIndexChanged);
            // 
            // rdbDefault
            // 
            this.rdbDefault.AutoSize = true;
            this.rdbDefault.Checked = true;
            this.rdbDefault.Location = new System.Drawing.Point(86, 60);
            this.rdbDefault.Name = "rdbDefault";
            this.rdbDefault.Size = new System.Drawing.Size(47, 16);
            this.rdbDefault.TabIndex = 15;
            this.rdbDefault.TabStop = true;
            this.rdbDefault.Text = "默认";
            this.rdbDefault.UseVisualStyleBackColor = true;
            // 
            // panBranch
            // 
            this.panBranch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panBranch.Controls.Add(this.rdbCashier);
            this.panBranch.Controls.Add(this.rdbDefault);
            this.panBranch.Controls.Add(this.cbxPos);
            this.panBranch.Controls.Add(this.label6);
            this.panBranch.Controls.Add(this.rdbInSingle);
            this.panBranch.Controls.Add(this.cbxBranch);
            this.panBranch.Controls.Add(this.rdbFloor);
            this.panBranch.Controls.Add(this.label5);
            this.panBranch.Controls.Add(this.label4);
            this.panBranch.Enabled = false;
            this.panBranch.Location = new System.Drawing.Point(13, 172);
            this.panBranch.Name = "panBranch";
            this.panBranch.Size = new System.Drawing.Size(320, 88);
            this.panBranch.TabIndex = 12;
            // 
            // rdbCashier
            // 
            this.rdbCashier.AutoSize = true;
            this.rdbCashier.Location = new System.Drawing.Point(245, 60);
            this.rdbCashier.Name = "rdbCashier";
            this.rdbCashier.Size = new System.Drawing.Size(47, 16);
            this.rdbCashier.TabIndex = 18;
            this.rdbCashier.Tag = "p20001";
            this.rdbCashier.Text = "收银";
            this.rdbCashier.UseVisualStyleBackColor = true;
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(345, 337);
            this.Controls.Add(this.panBranch);
            this.Controls.Add(this.gbxMode);
            this.Controls.Add(this.panel1);
            this.Name = "FormLogin";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "登录";
            this.Load += new System.EventHandler(this.SetForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panBranch.ResumeLayout(false);
            this.panBranch.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox gbxMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtHid;
        private System.Windows.Forms.Label lblGroupId;
        private System.Windows.Forms.RadioButton rdbKeyboardEnable;
        private System.Windows.Forms.RadioButton rdbKeyboardDisable;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rdbFloor;
        private System.Windows.Forms.ComboBox cbxBranch;
        private System.Windows.Forms.RadioButton rdbInSingle;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbxPos;
        private System.Windows.Forms.RadioButton rdbDefault;
        private System.Windows.Forms.Panel panBranch;
        private System.Windows.Forms.RadioButton rdbCashier;
    }
}