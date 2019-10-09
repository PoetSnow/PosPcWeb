namespace GsBrowser
{
    partial class FormLocal
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
            this.cbxIsFullScreen = new System.Windows.Forms.CheckBox();
            this.cbxEnableKeyboard = new System.Windows.Forms.CheckBox();
            this.cbxIsHandwrite = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbxIsFullScreen
            // 
            this.cbxIsFullScreen.AutoSize = true;
            this.cbxIsFullScreen.Checked = true;
            this.cbxIsFullScreen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxIsFullScreen.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbxIsFullScreen.Location = new System.Drawing.Point(22, 17);
            this.cbxIsFullScreen.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.cbxIsFullScreen.Name = "cbxIsFullScreen";
            this.cbxIsFullScreen.Size = new System.Drawing.Size(111, 31);
            this.cbxIsFullScreen.TabIndex = 0;
            this.cbxIsFullScreen.Text = "启用全屏";
            this.cbxIsFullScreen.UseVisualStyleBackColor = true;
            this.cbxIsFullScreen.CheckedChanged += new System.EventHandler(this.cbxIsFullScreen_CheckedChanged);
            // 
            // cbxEnableKeyboard
            // 
            this.cbxEnableKeyboard.AutoSize = true;
            this.cbxEnableKeyboard.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbxEnableKeyboard.Location = new System.Drawing.Point(22, 95);
            this.cbxEnableKeyboard.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.cbxEnableKeyboard.Name = "cbxEnableKeyboard";
            this.cbxEnableKeyboard.Size = new System.Drawing.Size(151, 31);
            this.cbxEnableKeyboard.TabIndex = 1;
            this.cbxEnableKeyboard.Text = "启用屏幕键盘";
            this.cbxEnableKeyboard.UseVisualStyleBackColor = true;
            this.cbxEnableKeyboard.CheckedChanged += new System.EventHandler(this.cbxEnableKeyboard_CheckedChanged);
            // 
            // cbxIsHandwrite
            // 
            this.cbxIsHandwrite.AutoSize = true;
            this.cbxIsHandwrite.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbxIsHandwrite.Location = new System.Drawing.Point(22, 56);
            this.cbxIsHandwrite.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.cbxIsHandwrite.Name = "cbxIsHandwrite";
            this.cbxIsHandwrite.Size = new System.Drawing.Size(151, 31);
            this.cbxIsHandwrite.TabIndex = 2;
            this.cbxIsHandwrite.Text = "启用手写功能";
            this.cbxIsHandwrite.UseVisualStyleBackColor = true;
            this.cbxIsHandwrite.CheckedChanged += new System.EventHandler(this.cbxIsHandwrite_CheckedChanged);
            // 
            // FormLocal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.cbxIsHandwrite);
            this.Controls.Add(this.cbxEnableKeyboard);
            this.Controls.Add(this.cbxIsFullScreen);
            this.Font = new System.Drawing.Font("Webdings", 8.25F);
            this.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.Name = "FormLocal";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "本地参数设置";
            this.Deactivate += new System.EventHandler(this.FormLocal_Deactivate);
            this.Load += new System.EventHandler(this.FormLocal_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbxIsFullScreen;
        private System.Windows.Forms.CheckBox cbxEnableKeyboard;
        private System.Windows.Forms.CheckBox cbxIsHandwrite;
    }
}