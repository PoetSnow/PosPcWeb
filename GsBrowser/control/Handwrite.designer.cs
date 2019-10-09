namespace Handwrite
{
    partial class Handwrite
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.btnSelectColor = new System.Windows.Forms.Button();
            this.btnClean = new System.Windows.Forms.Button();
            this.PictureboxInk = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn08 = new System.Windows.Forms.Button();
            this.btn07 = new System.Windows.Forms.Button();
            this.btn06 = new System.Windows.Forms.Button();
            this.btn05 = new System.Windows.Forms.Button();
            this.btn04 = new System.Windows.Forms.Button();
            this.btn03 = new System.Windows.Forms.Button();
            this.btn02 = new System.Windows.Forms.Button();
            this.btn01 = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PictureboxInk)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSelectColor
            // 
            this.btnSelectColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectColor.Location = new System.Drawing.Point(352, 227);
            this.btnSelectColor.Name = "btnSelectColor";
            this.btnSelectColor.Size = new System.Drawing.Size(87, 55);
            this.btnSelectColor.TabIndex = 9;
            this.btnSelectColor.Text = "选择颜色";
            this.btnSelectColor.UseVisualStyleBackColor = true;
            this.btnSelectColor.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnClean
            // 
            this.btnClean.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClean.Location = new System.Drawing.Point(441, 227);
            this.btnClean.Name = "btnClean";
            this.btnClean.Size = new System.Drawing.Size(86, 55);
            this.btnClean.TabIndex = 8;
            this.btnClean.Text = "清除";
            this.btnClean.UseVisualStyleBackColor = true;
            this.btnClean.Click += new System.EventHandler(this.btnClean_Click);
            // 
            // PictureboxInk
            // 
            this.PictureboxInk.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PictureboxInk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PictureboxInk.Location = new System.Drawing.Point(0, 0);
            this.PictureboxInk.Name = "PictureboxInk";
            this.PictureboxInk.Size = new System.Drawing.Size(350, 350);
            this.PictureboxInk.TabIndex = 6;
            this.PictureboxInk.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.btn08);
            this.panel1.Controls.Add(this.btn07);
            this.panel1.Controls.Add(this.btn06);
            this.panel1.Controls.Add(this.btn05);
            this.panel1.Controls.Add(this.btn04);
            this.panel1.Controls.Add(this.btn03);
            this.panel1.Controls.Add(this.btn02);
            this.panel1.Controls.Add(this.btn01);
            this.panel1.Location = new System.Drawing.Point(351, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(178, 220);
            this.panel1.TabIndex = 10;
            // 
            // btn08
            // 
            this.btn08.Location = new System.Drawing.Point(90, 166);
            this.btn08.Name = "btn08";
            this.btn08.Size = new System.Drawing.Size(85, 50);
            this.btn08.TabIndex = 15;
            this.btn08.UseVisualStyleBackColor = true;
            this.btn08.Click += new System.EventHandler(this.selectText_Click);
            // 
            // btn07
            // 
            this.btn07.Location = new System.Drawing.Point(3, 166);
            this.btn07.Name = "btn07";
            this.btn07.Size = new System.Drawing.Size(85, 50);
            this.btn07.TabIndex = 14;
            this.btn07.UseVisualStyleBackColor = true;
            this.btn07.Click += new System.EventHandler(this.selectText_Click);
            // 
            // btn06
            // 
            this.btn06.Location = new System.Drawing.Point(90, 112);
            this.btn06.Name = "btn06";
            this.btn06.Size = new System.Drawing.Size(85, 50);
            this.btn06.TabIndex = 13;
            this.btn06.UseVisualStyleBackColor = true;
            this.btn06.Click += new System.EventHandler(this.selectText_Click);
            // 
            // btn05
            // 
            this.btn05.Location = new System.Drawing.Point(3, 112);
            this.btn05.Name = "btn05";
            this.btn05.Size = new System.Drawing.Size(85, 50);
            this.btn05.TabIndex = 12;
            this.btn05.UseVisualStyleBackColor = true;
            this.btn05.Click += new System.EventHandler(this.selectText_Click);
            // 
            // btn04
            // 
            this.btn04.Location = new System.Drawing.Point(90, 58);
            this.btn04.Name = "btn04";
            this.btn04.Size = new System.Drawing.Size(85, 50);
            this.btn04.TabIndex = 11;
            this.btn04.UseVisualStyleBackColor = true;
            this.btn04.Click += new System.EventHandler(this.selectText_Click);
            // 
            // btn03
            // 
            this.btn03.Location = new System.Drawing.Point(3, 58);
            this.btn03.Name = "btn03";
            this.btn03.Size = new System.Drawing.Size(85, 50);
            this.btn03.TabIndex = 10;
            this.btn03.UseVisualStyleBackColor = true;
            this.btn03.Click += new System.EventHandler(this.selectText_Click);
            // 
            // btn02
            // 
            this.btn02.Location = new System.Drawing.Point(90, 4);
            this.btn02.Name = "btn02";
            this.btn02.Size = new System.Drawing.Size(85, 50);
            this.btn02.TabIndex = 9;
            this.btn02.UseVisualStyleBackColor = true;
            this.btn02.Click += new System.EventHandler(this.selectText_Click);
            // 
            // btn01
            // 
            this.btn01.Location = new System.Drawing.Point(3, 4);
            this.btn01.Name = "btn01";
            this.btn01.Size = new System.Drawing.Size(85, 50);
            this.btn01.TabIndex = 8;
            this.btn01.UseVisualStyleBackColor = true;
            this.btn01.Click += new System.EventHandler(this.selectText_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(441, 287);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(86, 60);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDel
            // 
            this.btnDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDel.Location = new System.Drawing.Point(352, 287);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(87, 60);
            this.btnDel.TabIndex = 12;
            this.btnDel.Text = "退格";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.button1_Click);
            // 
            // Handwrite
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.btnDel);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnSelectColor);
            this.Controls.Add(this.btnClean);
            this.Controls.Add(this.PictureboxInk);
            this.Name = "Handwrite";
            this.Size = new System.Drawing.Size(530, 350);
            this.Load += new System.EventHandler(this.Handwrite_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureboxInk)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button btnSelectColor;
        private System.Windows.Forms.Button btnClean;
        private System.Windows.Forms.PictureBox PictureboxInk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn08;
        private System.Windows.Forms.Button btn07;
        private System.Windows.Forms.Button btn06;
        private System.Windows.Forms.Button btn05;
        private System.Windows.Forms.Button btn04;
        private System.Windows.Forms.Button btn03;
        private System.Windows.Forms.Button btn02;
        private System.Windows.Forms.Button btn01;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnDel;
    }
}
