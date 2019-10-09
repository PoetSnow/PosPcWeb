namespace PrintSqlite
{
    partial class FrmMain
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
        #region Windows 窗体设计器生成的代码
        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.lvPrints = new System.Windows.Forms.ListView();
            this.cmsPrint = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiPrintTest = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPausePrinting = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRestorePrint = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCancelPrinting = new System.Windows.Forms.ToolStripMenuItem();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsRightMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiShow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.msRightMenu = new System.Windows.Forms.MenuStrip();
            this.tsmiRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRefreshList = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReboot = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQuery = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetForm = new System.Windows.Forms.ToolStripMenuItem();
            this.tmsiNoticeForm = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.getDataStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvPrintWait = new System.Windows.Forms.DataGridView();
            this.SerialNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FlowNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HotelCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OddNumbers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConsumptionID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConsumptionState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BusinessPointCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BusinessPointName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TableNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GuestName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumberOfPeople = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrderPeople = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrderTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BanquetCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BanquetName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BanquetNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BanquetUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BanquetPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BanquetAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProjectCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProjectEnglishName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProjectOtherName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Unit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UnitEnglishName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Only = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProduceBarcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProduceStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProduceNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProducePort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Printer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PrinterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Practice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Requirement = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GuestPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cook = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Salesman = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DepartmentCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WineCardNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Remarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DocumentName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SendingTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CompletionTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PrintState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PrintMark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.printStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.cmsPrint.SuspendLayout();
            this.cmsRightMenu.SuspendLayout();
            this.msRightMenu.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrintWait)).BeginInit();
            this.SuspendLayout();
            // 
            // lvPrints
            // 
            this.lvPrints.BackColor = System.Drawing.SystemColors.Window;
            this.lvPrints.ContextMenuStrip = this.cmsPrint;
            this.lvPrints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvPrints.LargeImageList = this.imgList;
            this.lvPrints.Location = new System.Drawing.Point(0, 0);
            this.lvPrints.MultiSelect = false;
            this.lvPrints.Name = "lvPrints";
            this.lvPrints.Size = new System.Drawing.Size(784, 293);
            this.lvPrints.SmallImageList = this.imgList;
            this.lvPrints.TabIndex = 0;
            this.lvPrints.UseCompatibleStateImageBehavior = false;
            this.lvPrints.SelectedIndexChanged += new System.EventHandler(this.lvPrints_SelectedIndexChanged);
            // 
            // cmsPrint
            // 
            this.cmsPrint.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiPrintTest,
            this.tsmiPausePrinting,
            this.tsmiRestorePrint,
            this.tsmiCancelPrinting});
            this.cmsPrint.Name = "cmsPrint";
            this.cmsPrint.Size = new System.Drawing.Size(137, 92);
            // 
            // tsmiPrintTest
            // 
            this.tsmiPrintTest.Name = "tsmiPrintTest";
            this.tsmiPrintTest.Size = new System.Drawing.Size(136, 22);
            this.tsmiPrintTest.Text = "打印测试页";
            this.tsmiPrintTest.Click += new System.EventHandler(this.tsmiPrintTest_Click);
            // 
            // tsmiPausePrinting
            // 
            this.tsmiPausePrinting.Name = "tsmiPausePrinting";
            this.tsmiPausePrinting.Size = new System.Drawing.Size(136, 22);
            this.tsmiPausePrinting.Text = "暂停打印";
            this.tsmiPausePrinting.Click += new System.EventHandler(this.tsmiPausePrinting_Click);
            // 
            // tsmiRestorePrint
            // 
            this.tsmiRestorePrint.Name = "tsmiRestorePrint";
            this.tsmiRestorePrint.Size = new System.Drawing.Size(136, 22);
            this.tsmiRestorePrint.Text = "恢复打印";
            this.tsmiRestorePrint.Click += new System.EventHandler(this.tsmiRestorePrint_Click);
            // 
            // tsmiCancelPrinting
            // 
            this.tsmiCancelPrinting.Name = "tsmiCancelPrinting";
            this.tsmiCancelPrinting.Size = new System.Drawing.Size(136, 22);
            this.tsmiCancelPrinting.Text = "取消打印";
            this.tsmiCancelPrinting.Click += new System.EventHandler(this.tsmiCancelPrinting_Click);
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "print_default.png");
            this.imgList.Images.SetKeyName(1, "print_warning.png");
            this.imgList.Images.SetKeyName(2, "print_error.png");
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.cmsRightMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "快点云Pos出品";
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // cmsRightMenu
            // 
            this.cmsRightMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiShow,
            this.tsmiExit});
            this.cmsRightMenu.Name = "cmsRightMenu";
            this.cmsRightMenu.Size = new System.Drawing.Size(137, 48);
            // 
            // tsmiShow
            // 
            this.tsmiShow.Name = "tsmiShow";
            this.tsmiShow.Size = new System.Drawing.Size(136, 22);
            this.tsmiShow.Text = "显示主界面";
            this.tsmiShow.Click += new System.EventHandler(this.tsmiShow_Click);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(136, 22);
            this.tsmiExit.Text = "退出";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // msRightMenu
            // 
            this.msRightMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiRefresh,
            this.tsmiRefreshList,
            this.tsmiReboot,
            this.tsmiQuery,
            this.tsmiSetForm,
            this.tmsiNoticeForm});
            this.msRightMenu.Location = new System.Drawing.Point(0, 0);
            this.msRightMenu.Name = "msRightMenu";
            this.msRightMenu.Size = new System.Drawing.Size(784, 25);
            this.msRightMenu.TabIndex = 42;
            this.msRightMenu.Text = "menuStrip1";
            // 
            // tsmiRefresh
            // 
            this.tsmiRefresh.Name = "tsmiRefresh";
            this.tsmiRefresh.Size = new System.Drawing.Size(104, 21);
            this.tsmiRefresh.Text = "刷新打印机列表";
            this.tsmiRefresh.Click += new System.EventHandler(this.tsmiRefresh_Click);
            // 
            // tsmiRefreshList
            // 
            this.tsmiRefreshList.Name = "tsmiRefreshList";
            this.tsmiRefreshList.Size = new System.Drawing.Size(92, 21);
            this.tsmiRefreshList.Text = "刷新出品记录";
            this.tsmiRefreshList.Click += new System.EventHandler(this.tsmiRefreshList_Click);
            // 
            // tsmiReboot
            // 
            this.tsmiReboot.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.tsmiReboot.Name = "tsmiReboot";
            this.tsmiReboot.Size = new System.Drawing.Size(68, 21);
            this.tsmiReboot.Text = "重启程序";
            this.tsmiReboot.Click += new System.EventHandler(this.tsmiReboot_Click);
            // 
            // tsmiQuery
            // 
            this.tsmiQuery.Name = "tsmiQuery";
            this.tsmiQuery.Size = new System.Drawing.Size(92, 21);
            this.tsmiQuery.Text = "查询出品数据";
            this.tsmiQuery.Click += new System.EventHandler(this.tsmiQuery_Click);
            // 
            // tsmiSetForm
            // 
            this.tsmiSetForm.Name = "tsmiSetForm";
            this.tsmiSetForm.ShowShortcutKeys = false;
            this.tsmiSetForm.Size = new System.Drawing.Size(80, 21);
            this.tsmiSetForm.Text = "设置打印机";
            this.tsmiSetForm.Click += new System.EventHandler(this.tsmiSetForm_Click);
            // 
            // tmsiNoticeForm
            // 
            this.tmsiNoticeForm.Name = "tmsiNoticeForm";
            this.tmsiNoticeForm.Size = new System.Drawing.Size(68, 21);
            this.tmsiNoticeForm.Text = "设置格式";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getDataStatusLabel,
            this.printStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 535);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 26);
            this.statusStrip1.TabIndex = 43;
            // 
            // getDataStatusLabel
            // 
            this.getDataStatusLabel.AutoToolTip = true;
            this.getDataStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.getDataStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.getDataStatusLabel.ForeColor = System.Drawing.Color.Red;
            this.getDataStatusLabel.Name = "getDataStatusLabel";
            this.getDataStatusLabel.Size = new System.Drawing.Size(84, 21);
            this.getDataStatusLabel.Text = "获取数据状态";
            this.getDataStatusLabel.ToolTipText = "获取数据状态";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lvPrints);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvPrintWait);
            this.splitContainer1.Size = new System.Drawing.Size(784, 510);
            this.splitContainer1.SplitterDistance = 293;
            this.splitContainer1.TabIndex = 44;
            // 
            // dgvPrintWait
            // 
            this.dgvPrintWait.AllowUserToAddRows = false;
            this.dgvPrintWait.AllowUserToDeleteRows = false;
            this.dgvPrintWait.AllowUserToOrderColumns = true;
            this.dgvPrintWait.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrintWait.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SerialNumber,
            this.FlowNumber,
            this.HotelCode,
            this.OddNumbers,
            this.ConsumptionID,
            this.ConsumptionState,
            this.BusinessPointCode,
            this.BusinessPointName,
            this.TableNumber,
            this.TableName,
            this.GuestName,
            this.NumberOfPeople,
            this.OrderPeople,
            this.OrderTime,
            this.BanquetCode,
            this.BanquetName,
            this.BanquetNumber,
            this.BanquetUnit,
            this.BanquetPrice,
            this.BanquetAmount,
            this.ProjectCode,
            this.ProjectName,
            this.ProjectEnglishName,
            this.ProjectOtherName,
            this.Unit,
            this.UnitEnglishName,
            this.Number,
            this.Only,
            this.Price,
            this.Amount,
            this.ProduceBarcode,
            this.ProduceStatus,
            this.ProduceNumber,
            this.ProducePort,
            this.Printer,
            this.PrinterName,
            this.Practice,
            this.Requirement,
            this.GuestPosition,
            this.Cook,
            this.Salesman,
            this.DepartmentCategory,
            this.WineCardNumber,
            this.Remarks,
            this.DocumentName,
            this.SendingTime,
            this.CompletionTime,
            this.PrintState,
            this.PrintMark});
            this.dgvPrintWait.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPrintWait.Location = new System.Drawing.Point(0, 0);
            this.dgvPrintWait.Name = "dgvPrintWait";
            this.dgvPrintWait.ReadOnly = true;
            this.dgvPrintWait.RowTemplate.Height = 23;
            this.dgvPrintWait.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPrintWait.Size = new System.Drawing.Size(784, 213);
            this.dgvPrintWait.TabIndex = 49;
            // 
            // SerialNumber
            // 
            this.SerialNumber.DataPropertyName = "SerialNumber";
            this.SerialNumber.HeaderText = "序号";
            this.SerialNumber.Name = "SerialNumber";
            this.SerialNumber.ReadOnly = true;
            // 
            // FlowNumber
            // 
            this.FlowNumber.DataPropertyName = "FlowNumber";
            this.FlowNumber.HeaderText = "流水号";
            this.FlowNumber.Name = "FlowNumber";
            this.FlowNumber.ReadOnly = true;
            this.FlowNumber.Visible = false;
            // 
            // HotelCode
            // 
            this.HotelCode.DataPropertyName = "HotelCode";
            this.HotelCode.HeaderText = "酒店代码";
            this.HotelCode.Name = "HotelCode";
            this.HotelCode.ReadOnly = true;
            this.HotelCode.Visible = false;
            // 
            // OddNumbers
            // 
            this.OddNumbers.DataPropertyName = "OddNumbers";
            this.OddNumbers.HeaderText = "单号";
            this.OddNumbers.Name = "OddNumbers";
            this.OddNumbers.ReadOnly = true;
            this.OddNumbers.Visible = false;
            // 
            // ConsumptionID
            // 
            this.ConsumptionID.DataPropertyName = "ConsumptionID";
            this.ConsumptionID.HeaderText = "消费Id";
            this.ConsumptionID.Name = "ConsumptionID";
            this.ConsumptionID.ReadOnly = true;
            this.ConsumptionID.Visible = false;
            // 
            // ConsumptionState
            // 
            this.ConsumptionState.DataPropertyName = "ConsumptionState";
            this.ConsumptionState.HeaderText = "消费状态";
            this.ConsumptionState.Name = "ConsumptionState";
            this.ConsumptionState.ReadOnly = true;
            // 
            // BusinessPointCode
            // 
            this.BusinessPointCode.DataPropertyName = "BusinessPointCode";
            this.BusinessPointCode.HeaderText = "营业点代码";
            this.BusinessPointCode.Name = "BusinessPointCode";
            this.BusinessPointCode.ReadOnly = true;
            this.BusinessPointCode.Visible = false;
            // 
            // BusinessPointName
            // 
            this.BusinessPointName.DataPropertyName = "BusinessPointName";
            this.BusinessPointName.HeaderText = "营业点名称";
            this.BusinessPointName.Name = "BusinessPointName";
            this.BusinessPointName.ReadOnly = true;
            // 
            // TableNumber
            // 
            this.TableNumber.DataPropertyName = "TableNumber";
            this.TableNumber.HeaderText = "台号";
            this.TableNumber.Name = "TableNumber";
            this.TableNumber.ReadOnly = true;
            // 
            // TableName
            // 
            this.TableName.DataPropertyName = "TableName";
            this.TableName.HeaderText = "台名";
            this.TableName.Name = "TableName";
            this.TableName.ReadOnly = true;
            // 
            // GuestName
            // 
            this.GuestName.DataPropertyName = "GuestName";
            this.GuestName.HeaderText = "客人姓名";
            this.GuestName.Name = "GuestName";
            this.GuestName.ReadOnly = true;
            // 
            // NumberOfPeople
            // 
            this.NumberOfPeople.DataPropertyName = "NumberOfPeople";
            this.NumberOfPeople.HeaderText = "人数";
            this.NumberOfPeople.Name = "NumberOfPeople";
            this.NumberOfPeople.ReadOnly = true;
            // 
            // OrderPeople
            // 
            this.OrderPeople.DataPropertyName = "OrderPeople";
            this.OrderPeople.HeaderText = "点菜人";
            this.OrderPeople.Name = "OrderPeople";
            this.OrderPeople.ReadOnly = true;
            // 
            // OrderTime
            // 
            this.OrderTime.DataPropertyName = "OrderTime";
            this.OrderTime.HeaderText = "点菜时间";
            this.OrderTime.Name = "OrderTime";
            this.OrderTime.ReadOnly = true;
            // 
            // BanquetCode
            // 
            this.BanquetCode.DataPropertyName = "BanquetCode";
            this.BanquetCode.HeaderText = "酒席编码";
            this.BanquetCode.Name = "BanquetCode";
            this.BanquetCode.ReadOnly = true;
            // 
            // BanquetName
            // 
            this.BanquetName.DataPropertyName = "BanquetName";
            this.BanquetName.HeaderText = "酒席名称";
            this.BanquetName.Name = "BanquetName";
            this.BanquetName.ReadOnly = true;
            // 
            // BanquetNumber
            // 
            this.BanquetNumber.DataPropertyName = "BanquetNumber";
            this.BanquetNumber.HeaderText = "酒席数量";
            this.BanquetNumber.Name = "BanquetNumber";
            this.BanquetNumber.ReadOnly = true;
            // 
            // BanquetUnit
            // 
            this.BanquetUnit.DataPropertyName = "BanquetUnit";
            this.BanquetUnit.HeaderText = "酒席单位";
            this.BanquetUnit.Name = "BanquetUnit";
            this.BanquetUnit.ReadOnly = true;
            // 
            // BanquetPrice
            // 
            this.BanquetPrice.DataPropertyName = "BanquetPrice";
            this.BanquetPrice.HeaderText = "酒席单价";
            this.BanquetPrice.Name = "BanquetPrice";
            this.BanquetPrice.ReadOnly = true;
            // 
            // BanquetAmount
            // 
            this.BanquetAmount.DataPropertyName = "BanquetAmount";
            this.BanquetAmount.HeaderText = "酒席金额";
            this.BanquetAmount.Name = "BanquetAmount";
            this.BanquetAmount.ReadOnly = true;
            // 
            // ProjectCode
            // 
            this.ProjectCode.DataPropertyName = "ProjectCode";
            this.ProjectCode.HeaderText = "项目编码";
            this.ProjectCode.Name = "ProjectCode";
            this.ProjectCode.ReadOnly = true;
            // 
            // ProjectName
            // 
            this.ProjectName.DataPropertyName = "ProjectName";
            this.ProjectName.HeaderText = "项目名称";
            this.ProjectName.Name = "ProjectName";
            this.ProjectName.ReadOnly = true;
            // 
            // ProjectEnglishName
            // 
            this.ProjectEnglishName.DataPropertyName = "ProjectEnglishName";
            this.ProjectEnglishName.HeaderText = "项目英文名";
            this.ProjectEnglishName.Name = "ProjectEnglishName";
            this.ProjectEnglishName.ReadOnly = true;
            // 
            // ProjectOtherName
            // 
            this.ProjectOtherName.DataPropertyName = "ProjectOtherName";
            this.ProjectOtherName.HeaderText = "项目其它名";
            this.ProjectOtherName.Name = "ProjectOtherName";
            this.ProjectOtherName.ReadOnly = true;
            // 
            // Unit
            // 
            this.Unit.DataPropertyName = "Unit";
            this.Unit.HeaderText = "单位";
            this.Unit.Name = "Unit";
            this.Unit.ReadOnly = true;
            // 
            // UnitEnglishName
            // 
            this.UnitEnglishName.DataPropertyName = "UnitEnglishName";
            this.UnitEnglishName.HeaderText = "单位英文名";
            this.UnitEnglishName.Name = "UnitEnglishName";
            this.UnitEnglishName.ReadOnly = true;
            // 
            // Number
            // 
            this.Number.DataPropertyName = "Number";
            this.Number.HeaderText = "数量";
            this.Number.Name = "Number";
            this.Number.ReadOnly = true;
            // 
            // Only
            // 
            this.Only.DataPropertyName = "Only";
            this.Only.HeaderText = "条只";
            this.Only.Name = "Only";
            this.Only.ReadOnly = true;
            // 
            // Price
            // 
            this.Price.DataPropertyName = "Price";
            this.Price.HeaderText = "单价";
            this.Price.Name = "Price";
            this.Price.ReadOnly = true;
            // 
            // Amount
            // 
            this.Amount.DataPropertyName = "Amount";
            this.Amount.HeaderText = "金额";
            this.Amount.Name = "Amount";
            this.Amount.ReadOnly = true;
            // 
            // ProduceBarcode
            // 
            this.ProduceBarcode.DataPropertyName = "ProduceBarcode";
            this.ProduceBarcode.HeaderText = "出品条码";
            this.ProduceBarcode.Name = "ProduceBarcode";
            this.ProduceBarcode.ReadOnly = true;
            // 
            // ProduceStatus
            // 
            this.ProduceStatus.DataPropertyName = "ProduceStatus";
            this.ProduceStatus.HeaderText = "出品状态";
            this.ProduceStatus.Name = "ProduceStatus";
            this.ProduceStatus.ReadOnly = true;
            // 
            // ProduceNumber
            // 
            this.ProduceNumber.DataPropertyName = "ProduceNumber";
            this.ProduceNumber.HeaderText = "出品次数";
            this.ProduceNumber.Name = "ProduceNumber";
            this.ProduceNumber.ReadOnly = true;
            // 
            // ProducePort
            // 
            this.ProducePort.DataPropertyName = "ProducePort";
            this.ProducePort.HeaderText = "出品端口";
            this.ProducePort.Name = "ProducePort";
            this.ProducePort.ReadOnly = true;
            // 
            // Printer
            // 
            this.Printer.DataPropertyName = "Printer";
            this.Printer.HeaderText = "打印机";
            this.Printer.Name = "Printer";
            this.Printer.ReadOnly = true;
            // 
            // PrinterName
            // 
            this.PrinterName.DataPropertyName = "PrinterName";
            this.PrinterName.HeaderText = "打印机名称";
            this.PrinterName.Name = "PrinterName";
            this.PrinterName.ReadOnly = true;
            // 
            // Practice
            // 
            this.Practice.DataPropertyName = "Practice";
            this.Practice.HeaderText = "作法";
            this.Practice.Name = "Practice";
            this.Practice.ReadOnly = true;
            // 
            // Requirement
            // 
            this.Requirement.DataPropertyName = "Requirement";
            this.Requirement.HeaderText = "要求";
            this.Requirement.Name = "Requirement";
            this.Requirement.ReadOnly = true;
            // 
            // GuestPosition
            // 
            this.GuestPosition.DataPropertyName = "GuestPosition";
            this.GuestPosition.HeaderText = "客位";
            this.GuestPosition.Name = "GuestPosition";
            this.GuestPosition.ReadOnly = true;
            // 
            // Cook
            // 
            this.Cook.DataPropertyName = "Cook";
            this.Cook.HeaderText = "厨师";
            this.Cook.Name = "Cook";
            this.Cook.ReadOnly = true;
            // 
            // Salesman
            // 
            this.Salesman.DataPropertyName = "Salesman";
            this.Salesman.HeaderText = "推销员";
            this.Salesman.Name = "Salesman";
            this.Salesman.ReadOnly = true;
            // 
            // DepartmentCategory
            // 
            this.DepartmentCategory.DataPropertyName = "DepartmentCategory";
            this.DepartmentCategory.HeaderText = "部门类别";
            this.DepartmentCategory.Name = "DepartmentCategory";
            this.DepartmentCategory.ReadOnly = true;
            // 
            // WineCardNumber
            // 
            this.WineCardNumber.DataPropertyName = "WineCardNumber";
            this.WineCardNumber.HeaderText = "酒卡号";
            this.WineCardNumber.Name = "WineCardNumber";
            this.WineCardNumber.ReadOnly = true;
            // 
            // Remarks
            // 
            this.Remarks.DataPropertyName = "Remarks";
            this.Remarks.HeaderText = "备注";
            this.Remarks.Name = "Remarks";
            this.Remarks.ReadOnly = true;
            // 
            // DocumentName
            // 
            this.DocumentName.DataPropertyName = "DocumentName";
            this.DocumentName.HeaderText = "文档名称";
            this.DocumentName.Name = "DocumentName";
            this.DocumentName.ReadOnly = true;
            // 
            // SendingTime
            // 
            this.SendingTime.DataPropertyName = "SendingTime";
            this.SendingTime.HeaderText = "发送时间";
            this.SendingTime.Name = "SendingTime";
            this.SendingTime.ReadOnly = true;
            // 
            // CompletionTime
            // 
            this.CompletionTime.DataPropertyName = "CompletionTime";
            this.CompletionTime.HeaderText = "完成时间";
            this.CompletionTime.Name = "CompletionTime";
            this.CompletionTime.ReadOnly = true;
            // 
            // PrintState
            // 
            this.PrintState.DataPropertyName = "PrintState";
            this.PrintState.HeaderText = "打印状态";
            this.PrintState.Name = "PrintState";
            this.PrintState.ReadOnly = true;
            // 
            // PrintMark
            // 
            this.PrintMark.DataPropertyName = "PrintMark";
            this.PrintMark.HeaderText = "打印标识";
            this.PrintMark.Name = "PrintMark";
            this.PrintMark.ReadOnly = true;
            this.PrintMark.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // printStatusLabel
            // 
            this.printStatusLabel.AutoToolTip = true;
            this.printStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.printStatusLabel.ForeColor = System.Drawing.Color.Red;
            this.printStatusLabel.Name = "printStatusLabel";
            this.printStatusLabel.Size = new System.Drawing.Size(60, 21);
            this.printStatusLabel.Text = "打印状态";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.msRightMenu);
            this.Name = "FrmMain";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " 快点云Pos系统";
            this.Activated += new System.EventHandler(this.FrmMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FromMain_FormClosing);
            this.Load += new System.EventHandler(this.PrintFrom_Load);
            this.Shown += new System.EventHandler(this.FromMain_Shown);
            this.cmsPrint.ResumeLayout(false);
            this.cmsRightMenu.ResumeLayout(false);
            this.msRightMenu.ResumeLayout(false);
            this.msRightMenu.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrintWait)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.ListView lvPrints;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip cmsRightMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiShow;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.ContextMenuStrip cmsPrint;
        private System.Windows.Forms.ToolStripMenuItem tsmiPrintTest;
        private System.Windows.Forms.ToolStripMenuItem tsmiPausePrinting;
        private System.Windows.Forms.ToolStripMenuItem tsmiRestorePrint;
        private System.Windows.Forms.ToolStripMenuItem tsmiCancelPrinting;
        private System.Windows.Forms.MenuStrip msRightMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiRefresh;
        private System.Windows.Forms.ToolStripMenuItem tsmiRefreshList;
        private System.Windows.Forms.ToolStripMenuItem tsmiQuery;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetForm;
        private System.Windows.Forms.ToolStripMenuItem tmsiNoticeForm;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem tsmiReboot;
        private System.Windows.Forms.DataGridView dgvPrintWait;
        private System.Windows.Forms.DataGridViewTextBoxColumn SerialNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn FlowNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn HotelCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn OddNumbers;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConsumptionID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConsumptionState;
        private System.Windows.Forms.DataGridViewTextBoxColumn BusinessPointCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn BusinessPointName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TableNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn TableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn GuestName;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumberOfPeople;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderPeople;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn BanquetCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn BanquetName;
        private System.Windows.Forms.DataGridViewTextBoxColumn BanquetNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn BanquetUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn BanquetPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn BanquetAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProjectCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProjectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProjectEnglishName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProjectOtherName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Unit;
        private System.Windows.Forms.DataGridViewTextBoxColumn UnitEnglishName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn Only;
        private System.Windows.Forms.DataGridViewTextBoxColumn Price;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProduceBarcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProduceStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProduceNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProducePort;
        private System.Windows.Forms.DataGridViewTextBoxColumn Printer;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrinterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Practice;
        private System.Windows.Forms.DataGridViewTextBoxColumn Requirement;
        private System.Windows.Forms.DataGridViewTextBoxColumn GuestPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cook;
        private System.Windows.Forms.DataGridViewTextBoxColumn Salesman;
        private System.Windows.Forms.DataGridViewTextBoxColumn DepartmentCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn WineCardNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn Remarks;
        private System.Windows.Forms.DataGridViewTextBoxColumn DocumentName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SendingTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn CompletionTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrintState;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrintMark;
        private System.Windows.Forms.ToolStripStatusLabel getDataStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel printStatusLabel;
    }
}
