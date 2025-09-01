namespace Architexor.BasicSplit.Forms
{
	partial class FrmSplit
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
			GroupBox groupBox7;
			Panel panel1;
			Label label4;
			Panel panel2;
			Label label6;
			Panel panel3;
			Label label7;
			Label label1;
			GroupBox groupBox1;
			Label label3;
			Label label2;
			GroupBox groupBox2;
			Label label5;
			GroupBox groupBox3;
			GroupBox groupBox4;
			GroupBox groupBox5;
			GroupBox groupBox6;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSplit));
			rdoBounding = new RadioButton();
			rdoGross = new RadioButton();
			rdoNet = new RadioButton();
			txtCill = new TextBox();
			txtLintel = new TextBox();
			btnClearLC = new Button();
			btnSelectLC = new Button();
			txtCentredWidth = new TextBox();
			btnClearAroundCentre = new Button();
			btnSelectAroundCentre = new Button();
			btnClearCentre = new Button();
			btnSelectCentre = new Button();
			btnClearEqual = new Button();
			btnSelectEqual = new Button();
			btnClearAdjacent = new Button();
			btnSelectAdjacent = new Button();
			rdoPickStartPt = new RadioButton();
			rdoRightLeft = new RadioButton();
			rdoLeftRight = new RadioButton();
			btnSelect = new Button();
			panelThumnails = new FlowLayoutPanel();
			btnAIPick = new Button();
			txtWidth = new TextBox();
			tlpIndividualArea = new TableLayoutPanel();
			picPreview = new PictureBox();
			panelIndividualStats = new Panel();
			tlpIndividualPart = new TableLayoutPanel();
			panelTotalStats = new Panel();
			tlpTotalSelectedOpenings = new TableLayoutPanel();
			tlpTotalSelectedElements = new TableLayoutPanel();
			tlpTotalPart = new TableLayoutPanel();
			tlpTotalArea = new TableLayoutPanel();
			btnOK = new Button();
			btnCancel = new Button();
			btnHelp = new Button();
			lblVersion = new Label();
			btnApply = new Button();
			btnClear = new Button();
			grpDebug = new GroupBox();
			txtDebug_PanelIndex = new TextBox();
			chkDebug_SplitLineNumbers = new CheckBox();
			chkDebug_SplitPoints = new CheckBox();
			chkDebug_LocationCurve = new CheckBox();
			chkDebug_BoundaryPoints = new CheckBox();
			btnViewInRevit = new Button();
			btnTrain = new Button();
			btnSetting = new Button();
			btnApplyToAll = new Button();
			groupBox7 = new GroupBox();
			panel1 = new Panel();
			label4 = new Label();
			panel2 = new Panel();
			label6 = new Label();
			panel3 = new Panel();
			label7 = new Label();
			label1 = new Label();
			groupBox1 = new GroupBox();
			label3 = new Label();
			label2 = new Label();
			groupBox2 = new GroupBox();
			label5 = new Label();
			groupBox3 = new GroupBox();
			groupBox4 = new GroupBox();
			groupBox5 = new GroupBox();
			groupBox6 = new GroupBox();
			groupBox7.SuspendLayout();
			panel1.SuspendLayout();
			panel2.SuspendLayout();
			panel3.SuspendLayout();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			groupBox3.SuspendLayout();
			groupBox4.SuspendLayout();
			groupBox5.SuspendLayout();
			groupBox6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
			panelIndividualStats.SuspendLayout();
			panelTotalStats.SuspendLayout();
			grpDebug.SuspendLayout();
			SuspendLayout();
			// 
			// groupBox7
			// 
			groupBox7.Controls.Add(rdoBounding);
			groupBox7.Controls.Add(rdoGross);
			groupBox7.Controls.Add(rdoNet);
			groupBox7.Location = new Point(782, 620);
			groupBox7.Margin = new Padding(4, 3, 4, 3);
			groupBox7.Name = "groupBox7";
			groupBox7.Padding = new Padding(4, 3, 4, 3);
			groupBox7.Size = new Size(152, 103);
			groupBox7.TabIndex = 13;
			groupBox7.TabStop = false;
			groupBox7.Text = "Area Display Mode";
			// 
			// rdoBounding
			// 
			rdoBounding.AutoSize = true;
			rdoBounding.Location = new Point(15, 76);
			rdoBounding.Margin = new Padding(4, 3, 4, 3);
			rdoBounding.Name = "rdoBounding";
			rdoBounding.Size = new Size(77, 19);
			rdoBounding.TabIndex = 2;
			rdoBounding.TabStop = true;
			rdoBounding.Text = "Bounding";
			rdoBounding.UseVisualStyleBackColor = true;
			rdoBounding.CheckedChanged += rdoBounding_CheckedChanged;
			// 
			// rdoGross
			// 
			rdoGross.AutoSize = true;
			rdoGross.Location = new Point(15, 50);
			rdoGross.Margin = new Padding(4, 3, 4, 3);
			rdoGross.Name = "rdoGross";
			rdoGross.Size = new Size(54, 19);
			rdoGross.TabIndex = 1;
			rdoGross.TabStop = true;
			rdoGross.Text = "Gross";
			rdoGross.UseVisualStyleBackColor = true;
			rdoGross.CheckedChanged += rdoGross_CheckedChanged;
			// 
			// rdoNet
			// 
			rdoNet.AutoSize = true;
			rdoNet.Location = new Point(15, 23);
			rdoNet.Margin = new Padding(4, 3, 4, 3);
			rdoNet.Name = "rdoNet";
			rdoNet.Size = new Size(44, 19);
			rdoNet.TabIndex = 0;
			rdoNet.TabStop = true;
			rdoNet.Text = "Net";
			rdoNet.UseVisualStyleBackColor = true;
			rdoNet.CheckedChanged += rdoNet_CheckedChanged;
			// 
			// panel1
			// 
			panel1.BackColor = Color.FromArgb(153, 180, 209);
			panel1.Controls.Add(label4);
			panel1.Location = new Point(1244, 10);
			panel1.Margin = new Padding(4, 3, 4, 3);
			panel1.Name = "panel1";
			panel1.Size = new Size(223, 29);
			panel1.TabIndex = 75;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
			label4.Location = new Point(4, 7);
			label4.Margin = new Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new Size(127, 13);
			label4.TabIndex = 0;
			label4.Text = "Parameters && Actions";
			// 
			// panel2
			// 
			panel2.BackColor = Color.FromArgb(153, 180, 209);
			panel2.Controls.Add(label6);
			panel2.Location = new Point(285, 10);
			panel2.Margin = new Padding(4, 3, 4, 3);
			panel2.Name = "panel2";
			panel2.Size = new Size(223, 29);
			panel2.TabIndex = 76;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
			label6.ForeColor = SystemColors.ControlText;
			label6.Location = new Point(4, 7);
			label6.Margin = new Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new Size(52, 13);
			label6.TabIndex = 0;
			label6.Text = "Preview";
			// 
			// panel3
			// 
			panel3.BackColor = Color.FromArgb(153, 180, 209);
			panel3.Controls.Add(label7);
			panel3.Location = new Point(10, 10);
			panel3.Margin = new Padding(4, 3, 4, 3);
			panel3.Name = "panel3";
			panel3.Size = new Size(264, 29);
			panel3.TabIndex = 77;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
			label7.ForeColor = SystemColors.ControlText;
			label7.Location = new Point(4, 7);
			label7.Margin = new Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new Size(91, 13);
			label7.TabIndex = 0;
			label7.Text = "Total Elements";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(1309, 96);
			label1.Margin = new Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new Size(68, 15);
			label1.TabIndex = 5;
			label1.Text = "Part length:";
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(txtCill);
			groupBox1.Controls.Add(label3);
			groupBox1.Controls.Add(txtLintel);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(btnClearLC);
			groupBox1.Controls.Add(btnSelectLC);
			groupBox1.Location = new Point(1246, 130);
			groupBox1.Margin = new Padding(4, 3, 4, 3);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new Padding(4, 3, 4, 3);
			groupBox1.Size = new Size(220, 121);
			groupBox1.TabIndex = 7;
			groupBox1.TabStop = false;
			groupBox1.Text = "Lintel && Sill";
			// 
			// txtCill
			// 
			txtCill.Location = new Point(136, 88);
			txtCill.Margin = new Padding(4, 3, 4, 3);
			txtCill.Name = "txtCill";
			txtCill.Size = new Size(73, 23);
			txtCill.TabIndex = 10;
			txtCill.TextChanged += txtCill_TextChanged;
			txtCill.KeyPress += txtCill_KeyPress;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(26, 91);
			label3.Margin = new Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new Size(105, 15);
			label3.TabIndex = 9;
			label3.Text = "Sill bearing length:";
			// 
			// txtLintel
			// 
			txtLintel.Location = new Point(136, 58);
			txtLintel.Margin = new Padding(4, 3, 4, 3);
			txtLintel.Name = "txtLintel";
			txtLintel.Size = new Size(73, 23);
			txtLintel.TabIndex = 8;
			txtLintel.TextChanged += txtLintel_TextChanged;
			txtLintel.KeyPress += txtLintel_KeyPress;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(12, 61);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(119, 15);
			label2.TabIndex = 7;
			label2.Text = "Lintel bearing length:";
			// 
			// btnClearLC
			// 
			btnClearLC.Location = new Point(114, 23);
			btnClearLC.Margin = new Padding(4, 3, 4, 3);
			btnClearLC.Name = "btnClearLC";
			btnClearLC.Size = new Size(96, 27);
			btnClearLC.TabIndex = 2;
			btnClearLC.Text = "Clear";
			btnClearLC.UseVisualStyleBackColor = true;
			btnClearLC.Click += btnClearLC_Click;
			// 
			// btnSelectLC
			// 
			btnSelectLC.Location = new Point(10, 23);
			btnSelectLC.Margin = new Padding(4, 3, 4, 3);
			btnSelectLC.Name = "btnSelectLC";
			btnSelectLC.Size = new Size(96, 27);
			btnSelectLC.TabIndex = 1;
			btnSelectLC.Text = "Select";
			btnSelectLC.UseVisualStyleBackColor = true;
			btnSelectLC.Click += btnSelectLC_Click;
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(txtCentredWidth);
			groupBox2.Controls.Add(label5);
			groupBox2.Controls.Add(btnClearAroundCentre);
			groupBox2.Controls.Add(btnSelectAroundCentre);
			groupBox2.Location = new Point(1246, 258);
			groupBox2.Margin = new Padding(4, 3, 4, 3);
			groupBox2.Name = "groupBox2";
			groupBox2.Padding = new Padding(4, 3, 4, 3);
			groupBox2.Size = new Size(220, 93);
			groupBox2.TabIndex = 8;
			groupBox2.TabStop = false;
			groupBox2.Text = "Around Centre";
			// 
			// txtCentredWidth
			// 
			txtCentredWidth.Location = new Point(136, 58);
			txtCentredWidth.Margin = new Padding(4, 3, 4, 3);
			txtCentredWidth.Name = "txtCentredWidth";
			txtCentredWidth.Size = new Size(73, 23);
			txtCentredWidth.TabIndex = 8;
			txtCentredWidth.TextChanged += txtCentredWidth_TextChanged;
			txtCentredWidth.KeyPress += txtCentredWidth_KeyPress;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new Point(63, 61);
			label5.Margin = new Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new Size(68, 15);
			label5.TabIndex = 7;
			label5.Text = "Part length:";
			// 
			// btnClearAroundCentre
			// 
			btnClearAroundCentre.Location = new Point(114, 23);
			btnClearAroundCentre.Margin = new Padding(4, 3, 4, 3);
			btnClearAroundCentre.Name = "btnClearAroundCentre";
			btnClearAroundCentre.Size = new Size(96, 27);
			btnClearAroundCentre.TabIndex = 2;
			btnClearAroundCentre.Text = "Clear";
			btnClearAroundCentre.UseVisualStyleBackColor = true;
			btnClearAroundCentre.Click += btnClearAroundCentre_Click;
			// 
			// btnSelectAroundCentre
			// 
			btnSelectAroundCentre.Location = new Point(10, 23);
			btnSelectAroundCentre.Margin = new Padding(4, 3, 4, 3);
			btnSelectAroundCentre.Name = "btnSelectAroundCentre";
			btnSelectAroundCentre.Size = new Size(96, 27);
			btnSelectAroundCentre.TabIndex = 1;
			btnSelectAroundCentre.Text = "Select";
			btnSelectAroundCentre.UseVisualStyleBackColor = true;
			btnSelectAroundCentre.Click += btnSelectAroundCentre_Click;
			// 
			// groupBox3
			// 
			groupBox3.Controls.Add(btnClearCentre);
			groupBox3.Controls.Add(btnSelectCentre);
			groupBox3.Location = new Point(1246, 359);
			groupBox3.Margin = new Padding(4, 3, 4, 3);
			groupBox3.Name = "groupBox3";
			groupBox3.Padding = new Padding(4, 3, 4, 3);
			groupBox3.Size = new Size(220, 62);
			groupBox3.TabIndex = 9;
			groupBox3.TabStop = false;
			groupBox3.Text = "Centre of Opening";
			// 
			// btnClearCentre
			// 
			btnClearCentre.Location = new Point(114, 23);
			btnClearCentre.Margin = new Padding(4, 3, 4, 3);
			btnClearCentre.Name = "btnClearCentre";
			btnClearCentre.Size = new Size(96, 27);
			btnClearCentre.TabIndex = 2;
			btnClearCentre.Text = "Clear";
			btnClearCentre.UseVisualStyleBackColor = true;
			btnClearCentre.Click += btnClearCentre_Click;
			// 
			// btnSelectCentre
			// 
			btnSelectCentre.Location = new Point(10, 23);
			btnSelectCentre.Margin = new Padding(4, 3, 4, 3);
			btnSelectCentre.Name = "btnSelectCentre";
			btnSelectCentre.Size = new Size(96, 27);
			btnSelectCentre.TabIndex = 1;
			btnSelectCentre.Text = "Select";
			btnSelectCentre.UseVisualStyleBackColor = true;
			btnSelectCentre.Click += btnSelectCentre_Click;
			// 
			// groupBox4
			// 
			groupBox4.Controls.Add(btnClearEqual);
			groupBox4.Controls.Add(btnSelectEqual);
			groupBox4.Location = new Point(1246, 428);
			groupBox4.Margin = new Padding(4, 3, 4, 3);
			groupBox4.Name = "groupBox4";
			groupBox4.Padding = new Padding(4, 3, 4, 3);
			groupBox4.Size = new Size(220, 62);
			groupBox4.TabIndex = 10;
			groupBox4.TabStop = false;
			groupBox4.Text = "Equal distance between openings";
			// 
			// btnClearEqual
			// 
			btnClearEqual.Location = new Point(114, 23);
			btnClearEqual.Margin = new Padding(4, 3, 4, 3);
			btnClearEqual.Name = "btnClearEqual";
			btnClearEqual.Size = new Size(96, 27);
			btnClearEqual.TabIndex = 2;
			btnClearEqual.Text = "Clear";
			btnClearEqual.UseVisualStyleBackColor = true;
			btnClearEqual.Click += btnClearEqual_Click;
			// 
			// btnSelectEqual
			// 
			btnSelectEqual.Location = new Point(10, 23);
			btnSelectEqual.Margin = new Padding(4, 3, 4, 3);
			btnSelectEqual.Name = "btnSelectEqual";
			btnSelectEqual.Size = new Size(96, 27);
			btnSelectEqual.TabIndex = 1;
			btnSelectEqual.Text = "Select";
			btnSelectEqual.UseVisualStyleBackColor = true;
			btnSelectEqual.Click += btnSelectEqual_Click;
			// 
			// groupBox5
			// 
			groupBox5.Controls.Add(btnClearAdjacent);
			groupBox5.Controls.Add(btnSelectAdjacent);
			groupBox5.Location = new Point(1246, 498);
			groupBox5.Margin = new Padding(4, 3, 4, 3);
			groupBox5.Name = "groupBox5";
			groupBox5.Padding = new Padding(4, 3, 4, 3);
			groupBox5.Size = new Size(220, 62);
			groupBox5.TabIndex = 11;
			groupBox5.TabStop = false;
			groupBox5.Text = "Adjacent Walls/Beams";
			// 
			// btnClearAdjacent
			// 
			btnClearAdjacent.Location = new Point(114, 23);
			btnClearAdjacent.Margin = new Padding(4, 3, 4, 3);
			btnClearAdjacent.Name = "btnClearAdjacent";
			btnClearAdjacent.Size = new Size(96, 27);
			btnClearAdjacent.TabIndex = 2;
			btnClearAdjacent.Text = "Clear";
			btnClearAdjacent.UseVisualStyleBackColor = true;
			btnClearAdjacent.Click += btnClearAdjacent_Click;
			// 
			// btnSelectAdjacent
			// 
			btnSelectAdjacent.Location = new Point(10, 23);
			btnSelectAdjacent.Margin = new Padding(4, 3, 4, 3);
			btnSelectAdjacent.Name = "btnSelectAdjacent";
			btnSelectAdjacent.Size = new Size(96, 27);
			btnSelectAdjacent.TabIndex = 1;
			btnSelectAdjacent.Text = "Select";
			btnSelectAdjacent.UseVisualStyleBackColor = true;
			btnSelectAdjacent.Click += btnSelectAdjacent_Click;
			// 
			// groupBox6
			// 
			groupBox6.Controls.Add(rdoPickStartPt);
			groupBox6.Controls.Add(rdoRightLeft);
			groupBox6.Controls.Add(rdoLeftRight);
			groupBox6.Location = new Point(1246, 568);
			groupBox6.Margin = new Padding(4, 3, 4, 3);
			groupBox6.Name = "groupBox6";
			groupBox6.Padding = new Padding(4, 3, 4, 3);
			groupBox6.Size = new Size(220, 103);
			groupBox6.TabIndex = 12;
			groupBox6.TabStop = false;
			groupBox6.Text = "Direction";
			// 
			// rdoPickStartPt
			// 
			rdoPickStartPt.AutoSize = true;
			rdoPickStartPt.Location = new Point(15, 76);
			rdoPickStartPt.Margin = new Padding(4, 3, 4, 3);
			rdoPickStartPt.Name = "rdoPickStartPt";
			rdoPickStartPt.Size = new Size(162, 19);
			rdoPickStartPt.TabIndex = 2;
			rdoPickStartPt.TabStop = true;
			rdoPickStartPt.Text = "Choose unique start point";
			rdoPickStartPt.UseVisualStyleBackColor = true;
			rdoPickStartPt.CheckedChanged += rdoPickStartPt_CheckedChanged;
			// 
			// rdoRightLeft
			// 
			rdoRightLeft.AutoSize = true;
			rdoRightLeft.Location = new Point(15, 50);
			rdoRightLeft.Margin = new Padding(4, 3, 4, 3);
			rdoRightLeft.Name = "rdoRightLeft";
			rdoRightLeft.Size = new Size(90, 19);
			rdoRightLeft.TabIndex = 1;
			rdoRightLeft.TabStop = true;
			rdoRightLeft.Text = "Right to Left";
			rdoRightLeft.UseVisualStyleBackColor = true;
			rdoRightLeft.CheckedChanged += rdoRightLeft_CheckedChanged;
			// 
			// rdoLeftRight
			// 
			rdoLeftRight.AutoSize = true;
			rdoLeftRight.Location = new Point(15, 23);
			rdoLeftRight.Margin = new Padding(4, 3, 4, 3);
			rdoLeftRight.Name = "rdoLeftRight";
			rdoLeftRight.Size = new Size(90, 19);
			rdoLeftRight.TabIndex = 0;
			rdoLeftRight.TabStop = true;
			rdoLeftRight.Text = "Left to Right";
			rdoLeftRight.UseVisualStyleBackColor = true;
			rdoLeftRight.CheckedChanged += rdoLeftRight_CheckedChanged;
			// 
			// btnSelect
			// 
			btnSelect.Location = new Point(9, 45);
			btnSelect.Margin = new Padding(4, 3, 4, 3);
			btnSelect.Name = "btnSelect";
			btnSelect.Size = new Size(127, 27);
			btnSelect.TabIndex = 0;
			btnSelect.Text = "Select";
			btnSelect.UseVisualStyleBackColor = true;
			btnSelect.Click += btnSelect_Click;
			// 
			// panelThumnails
			// 
			panelThumnails.AutoScroll = true;
			panelThumnails.BackColor = Color.White;
			panelThumnails.BorderStyle = BorderStyle.FixedSingle;
			panelThumnails.Location = new Point(10, 78);
			panelThumnails.Margin = new Padding(4, 3, 4, 3);
			panelThumnails.Name = "panelThumnails";
			panelThumnails.Padding = new Padding(0, 0, 0, 6);
			panelThumnails.Size = new Size(263, 440);
			panelThumnails.TabIndex = 1;
			// 
			// btnAIPick
			// 
			btnAIPick.Enabled = false;
			btnAIPick.Location = new Point(1242, 46);
			btnAIPick.Margin = new Padding(4, 3, 4, 3);
			btnAIPick.Name = "btnAIPick";
			btnAIPick.Size = new Size(108, 27);
			btnAIPick.TabIndex = 4;
			btnAIPick.Text = "AI Pick";
			btnAIPick.UseVisualStyleBackColor = true;
			btnAIPick.Click += btnAIPick_Click;
			// 
			// txtWidth
			// 
			txtWidth.Location = new Point(1382, 92);
			txtWidth.Margin = new Padding(4, 3, 4, 3);
			txtWidth.Name = "txtWidth";
			txtWidth.Size = new Size(73, 23);
			txtWidth.TabIndex = 6;
			txtWidth.TextChanged += txtWidth_TextChanged;
			txtWidth.KeyPress += txtWidth_KeyPress;
			// 
			// tlpIndividualArea
			// 
			tlpIndividualArea.ColumnCount = 2;
			tlpIndividualArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
			tlpIndividualArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
			tlpIndividualArea.Location = new Point(0, 0);
			tlpIndividualArea.Margin = new Padding(4, 3, 4, 3);
			tlpIndividualArea.Name = "tlpIndividualArea";
			tlpIndividualArea.RowCount = 3;
			tlpIndividualArea.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpIndividualArea.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpIndividualArea.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpIndividualArea.Size = new Size(264, 47);
			tlpIndividualArea.TabIndex = 69;
			// 
			// picPreview
			// 
			picPreview.BackColor = Color.White;
			picPreview.BorderStyle = BorderStyle.FixedSingle;
			picPreview.Location = new Point(285, 10);
			picPreview.Margin = new Padding(4, 3, 4, 3);
			picPreview.Name = "picPreview";
			picPreview.Size = new Size(950, 599);
			picPreview.TabIndex = 70;
			picPreview.TabStop = false;
			// 
			// panelIndividualStats
			// 
			panelIndividualStats.Controls.Add(tlpIndividualArea);
			panelIndividualStats.Controls.Add(tlpIndividualPart);
			panelIndividualStats.Location = new Point(975, 618);
			panelIndividualStats.Margin = new Padding(4, 3, 4, 3);
			panelIndividualStats.Name = "panelIndividualStats";
			panelIndividualStats.Size = new Size(264, 155);
			panelIndividualStats.TabIndex = 71;
			// 
			// tlpIndividualPart
			// 
			tlpIndividualPart.ColumnCount = 2;
			tlpIndividualPart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
			tlpIndividualPart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
			tlpIndividualPart.Location = new Point(20, 90);
			tlpIndividualPart.Margin = new Padding(4, 3, 4, 3);
			tlpIndividualPart.Name = "tlpIndividualPart";
			tlpIndividualPart.RowCount = 1;
			tlpIndividualPart.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
			tlpIndividualPart.Size = new Size(233, 20);
			tlpIndividualPart.TabIndex = 70;
			// 
			// panelTotalStats
			// 
			panelTotalStats.Controls.Add(tlpTotalSelectedOpenings);
			panelTotalStats.Controls.Add(tlpTotalSelectedElements);
			panelTotalStats.Controls.Add(tlpTotalPart);
			panelTotalStats.Controls.Add(tlpTotalArea);
			panelTotalStats.Location = new Point(10, 527);
			panelTotalStats.Margin = new Padding(4, 3, 4, 3);
			panelTotalStats.Name = "panelTotalStats";
			panelTotalStats.Size = new Size(264, 210);
			panelTotalStats.TabIndex = 72;
			// 
			// tlpTotalSelectedOpenings
			// 
			tlpTotalSelectedOpenings.ColumnCount = 2;
			tlpTotalSelectedOpenings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
			tlpTotalSelectedOpenings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
			tlpTotalSelectedOpenings.Location = new Point(15, 54);
			tlpTotalSelectedOpenings.Margin = new Padding(4, 3, 4, 3);
			tlpTotalSelectedOpenings.Name = "tlpTotalSelectedOpenings";
			tlpTotalSelectedOpenings.RowCount = 8;
			tlpTotalSelectedOpenings.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpTotalSelectedOpenings.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpTotalSelectedOpenings.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpTotalSelectedOpenings.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpTotalSelectedOpenings.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpTotalSelectedOpenings.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpTotalSelectedOpenings.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpTotalSelectedOpenings.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpTotalSelectedOpenings.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
			tlpTotalSelectedOpenings.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
			tlpTotalSelectedOpenings.Size = new Size(233, 74);
			tlpTotalSelectedOpenings.TabIndex = 72;
			// 
			// tlpTotalSelectedElements
			// 
			tlpTotalSelectedElements.ColumnCount = 2;
			tlpTotalSelectedElements.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
			tlpTotalSelectedElements.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
			tlpTotalSelectedElements.Location = new Point(0, 0);
			tlpTotalSelectedElements.Margin = new Padding(4, 3, 4, 3);
			tlpTotalSelectedElements.Name = "tlpTotalSelectedElements";
			tlpTotalSelectedElements.RowCount = 2;
			tlpTotalSelectedElements.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpTotalSelectedElements.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpTotalSelectedElements.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
			tlpTotalSelectedElements.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
			tlpTotalSelectedElements.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
			tlpTotalSelectedElements.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
			tlpTotalSelectedElements.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
			tlpTotalSelectedElements.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
			tlpTotalSelectedElements.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
			tlpTotalSelectedElements.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
			tlpTotalSelectedElements.Size = new Size(233, 74);
			tlpTotalSelectedElements.TabIndex = 71;
			// 
			// tlpTotalPart
			// 
			tlpTotalPart.ColumnCount = 2;
			tlpTotalPart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
			tlpTotalPart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
			tlpTotalPart.Location = new Point(4, 187);
			tlpTotalPart.Margin = new Padding(4, 3, 4, 3);
			tlpTotalPart.Name = "tlpTotalPart";
			tlpTotalPart.RowCount = 1;
			tlpTotalPart.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
			tlpTotalPart.Size = new Size(233, 20);
			tlpTotalPart.TabIndex = 70;
			// 
			// tlpTotalArea
			// 
			tlpTotalArea.ColumnCount = 2;
			tlpTotalArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
			tlpTotalArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
			tlpTotalArea.Location = new Point(4, 117);
			tlpTotalArea.Margin = new Padding(4, 3, 4, 3);
			tlpTotalArea.Name = "tlpTotalArea";
			tlpTotalArea.RowCount = 3;
			tlpTotalArea.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpTotalArea.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpTotalArea.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
			tlpTotalArea.Size = new Size(233, 47);
			tlpTotalArea.TabIndex = 69;
			// 
			// btnOK
			// 
			btnOK.DialogResult = DialogResult.OK;
			btnOK.Location = new Point(9, 744);
			btnOK.Margin = new Padding(4, 3, 4, 3);
			btnOK.Name = "btnOK";
			btnOK.Size = new Size(96, 27);
			btnOK.TabIndex = 73;
			btnOK.Text = "OK";
			btnOK.UseVisualStyleBackColor = true;
			btnOK.Click += btnOK_Click;
			// 
			// btnCancel
			// 
			btnCancel.DialogResult = DialogResult.Cancel;
			btnCancel.Location = new Point(112, 744);
			btnCancel.Margin = new Padding(4, 3, 4, 3);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(96, 27);
			btnCancel.TabIndex = 74;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			btnCancel.Click += btnCancel_Click;
			// 
			// btnHelp
			// 
			btnHelp.DialogResult = DialogResult.OK;
			btnHelp.Location = new Point(317, 744);
			btnHelp.Margin = new Padding(4, 3, 4, 3);
			btnHelp.Name = "btnHelp";
			btnHelp.Size = new Size(96, 27);
			btnHelp.TabIndex = 78;
			btnHelp.Text = "Help";
			btnHelp.UseVisualStyleBackColor = true;
			btnHelp.Click += btnHelp_Click;
			// 
			// lblVersion
			// 
			lblVersion.BackColor = Color.FromArgb(153, 180, 209);
			lblVersion.BorderStyle = BorderStyle.FixedSingle;
			lblVersion.Location = new Point(421, 745);
			lblVersion.Margin = new Padding(4, 0, 4, 0);
			lblVersion.Name = "lblVersion";
			lblVersion.Size = new Size(59, 24);
			lblVersion.TabIndex = 79;
			lblVersion.Text = "2.0.0";
			lblVersion.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// btnApply
			// 
			btnApply.DialogResult = DialogResult.OK;
			btnApply.Location = new Point(1371, 744);
			btnApply.Margin = new Padding(4, 3, 4, 3);
			btnApply.Name = "btnApply";
			btnApply.Size = new Size(96, 27);
			btnApply.TabIndex = 80;
			btnApply.Text = "Apply";
			btnApply.UseVisualStyleBackColor = true;
			btnApply.Click += btnApply_Click;
			// 
			// btnClear
			// 
			btnClear.Location = new Point(147, 45);
			btnClear.Margin = new Padding(4, 3, 4, 3);
			btnClear.Name = "btnClear";
			btnClear.Size = new Size(127, 27);
			btnClear.TabIndex = 81;
			btnClear.Text = "Clear";
			btnClear.UseVisualStyleBackColor = true;
			btnClear.Click += btnClear_Click;
			// 
			// grpDebug
			// 
			grpDebug.Controls.Add(txtDebug_PanelIndex);
			grpDebug.Controls.Add(chkDebug_SplitLineNumbers);
			grpDebug.Controls.Add(chkDebug_SplitPoints);
			grpDebug.Controls.Add(chkDebug_LocationCurve);
			grpDebug.Controls.Add(chkDebug_BoundaryPoints);
			grpDebug.Location = new Point(1092, 10);
			grpDebug.Margin = new Padding(4, 3, 4, 3);
			grpDebug.Name = "grpDebug";
			grpDebug.Padding = new Padding(4, 3, 4, 3);
			grpDebug.Size = new Size(144, 167);
			grpDebug.TabIndex = 82;
			grpDebug.TabStop = false;
			grpDebug.Text = "Debug";
			// 
			// txtDebug_PanelIndex
			// 
			txtDebug_PanelIndex.Location = new Point(7, 136);
			txtDebug_PanelIndex.Margin = new Padding(4, 3, 4, 3);
			txtDebug_PanelIndex.Name = "txtDebug_PanelIndex";
			txtDebug_PanelIndex.Size = new Size(116, 23);
			txtDebug_PanelIndex.TabIndex = 4;
			// 
			// chkDebug_SplitLineNumbers
			// 
			chkDebug_SplitLineNumbers.AutoSize = true;
			chkDebug_SplitLineNumbers.Location = new Point(8, 110);
			chkDebug_SplitLineNumbers.Margin = new Padding(4, 3, 4, 3);
			chkDebug_SplitLineNumbers.Name = "chkDebug_SplitLineNumbers";
			chkDebug_SplitLineNumbers.Size = new Size(126, 19);
			chkDebug_SplitLineNumbers.TabIndex = 3;
			chkDebug_SplitLineNumbers.Text = "Split Line Numbers";
			chkDebug_SplitLineNumbers.UseVisualStyleBackColor = true;
			chkDebug_SplitLineNumbers.CheckedChanged += chkDebug_SplitLineNumbers_CheckedChanged;
			// 
			// chkDebug_SplitPoints
			// 
			chkDebug_SplitPoints.AutoSize = true;
			chkDebug_SplitPoints.Location = new Point(8, 83);
			chkDebug_SplitPoints.Margin = new Padding(4, 3, 4, 3);
			chkDebug_SplitPoints.Name = "chkDebug_SplitPoints";
			chkDebug_SplitPoints.Size = new Size(85, 19);
			chkDebug_SplitPoints.TabIndex = 2;
			chkDebug_SplitPoints.Text = "Split Points";
			chkDebug_SplitPoints.UseVisualStyleBackColor = true;
			chkDebug_SplitPoints.CheckedChanged += chkDebug_SplitPoints_CheckedChanged;
			// 
			// chkDebug_LocationCurve
			// 
			chkDebug_LocationCurve.AutoSize = true;
			chkDebug_LocationCurve.Location = new Point(8, 57);
			chkDebug_LocationCurve.Margin = new Padding(4, 3, 4, 3);
			chkDebug_LocationCurve.Name = "chkDebug_LocationCurve";
			chkDebug_LocationCurve.Size = new Size(106, 19);
			chkDebug_LocationCurve.TabIndex = 1;
			chkDebug_LocationCurve.Text = "Location Curve";
			chkDebug_LocationCurve.UseVisualStyleBackColor = true;
			chkDebug_LocationCurve.CheckedChanged += chkDebug_LocationCurve_CheckedChanged;
			// 
			// chkDebug_BoundaryPoints
			// 
			chkDebug_BoundaryPoints.AutoSize = true;
			chkDebug_BoundaryPoints.Location = new Point(8, 30);
			chkDebug_BoundaryPoints.Margin = new Padding(4, 3, 4, 3);
			chkDebug_BoundaryPoints.Name = "chkDebug_BoundaryPoints";
			chkDebug_BoundaryPoints.Size = new Size(113, 19);
			chkDebug_BoundaryPoints.TabIndex = 0;
			chkDebug_BoundaryPoints.Text = "Boundary Points";
			chkDebug_BoundaryPoints.UseVisualStyleBackColor = true;
			chkDebug_BoundaryPoints.CheckedChanged += chkDebug_BoundaryPoints_CheckedChanged;
			// 
			// btnViewInRevit
			// 
			btnViewInRevit.Location = new Point(958, 17);
			btnViewInRevit.Margin = new Padding(4, 3, 4, 3);
			btnViewInRevit.Name = "btnViewInRevit";
			btnViewInRevit.Size = new Size(127, 27);
			btnViewInRevit.TabIndex = 83;
			btnViewInRevit.Text = "View";
			btnViewInRevit.UseVisualStyleBackColor = true;
			btnViewInRevit.Visible = false;
			btnViewInRevit.Click += btnViewInRevit_Click;
			// 
			// btnTrain
			// 
			btnTrain.DialogResult = DialogResult.OK;
			btnTrain.Enabled = false;
			btnTrain.Location = new Point(1256, 744);
			btnTrain.Margin = new Padding(4, 3, 4, 3);
			btnTrain.Name = "btnTrain";
			btnTrain.Size = new Size(96, 27);
			btnTrain.TabIndex = 84;
			btnTrain.Text = "Train";
			btnTrain.UseVisualStyleBackColor = true;
			btnTrain.Visible = false;
			btnTrain.Click += btnTrain_Click;
			// 
			// btnSetting
			// 
			btnSetting.DialogResult = DialogResult.OK;
			btnSetting.Location = new Point(215, 744);
			btnSetting.Margin = new Padding(4, 3, 4, 3);
			btnSetting.Name = "btnSetting";
			btnSetting.Size = new Size(96, 27);
			btnSetting.TabIndex = 85;
			btnSetting.Text = "Settings";
			btnSetting.UseVisualStyleBackColor = true;
			btnSetting.Click += btnSetting_Click;
			// 
			// btnApplyToAll
			// 
			btnApplyToAll.Location = new Point(1358, 46);
			btnApplyToAll.Margin = new Padding(4, 3, 4, 3);
			btnApplyToAll.Name = "btnApplyToAll";
			btnApplyToAll.Size = new Size(108, 27);
			btnApplyToAll.TabIndex = 86;
			btnApplyToAll.Text = "Apply To All";
			btnApplyToAll.UseVisualStyleBackColor = true;
			btnApplyToAll.Click += btnApplyToAll_Click;
			// 
			// FrmSplit
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1476, 780);
			Controls.Add(btnApplyToAll);
			Controls.Add(btnSetting);
			Controls.Add(btnTrain);
			Controls.Add(btnViewInRevit);
			Controls.Add(grpDebug);
			Controls.Add(btnClear);
			Controls.Add(btnApply);
			Controls.Add(lblVersion);
			Controls.Add(btnHelp);
			Controls.Add(panel3);
			Controls.Add(panel2);
			Controls.Add(panel1);
			Controls.Add(btnCancel);
			Controls.Add(btnOK);
			Controls.Add(panelTotalStats);
			Controls.Add(panelIndividualStats);
			Controls.Add(picPreview);
			Controls.Add(groupBox7);
			Controls.Add(groupBox6);
			Controls.Add(groupBox5);
			Controls.Add(groupBox4);
			Controls.Add(groupBox3);
			Controls.Add(groupBox2);
			Controls.Add(groupBox1);
			Controls.Add(txtWidth);
			Controls.Add(label1);
			Controls.Add(btnAIPick);
			Controls.Add(panelThumnails);
			Controls.Add(btnSelect);
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			Icon = (Icon)resources.GetObject("$this.Icon");
			Margin = new Padding(4, 3, 4, 3);
			Name = "FrmSplit";
			Text = "FrmSplit";
			Load += FrmSplit_Load;
			groupBox7.ResumeLayout(false);
			groupBox7.PerformLayout();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			groupBox3.ResumeLayout(false);
			groupBox4.ResumeLayout(false);
			groupBox5.ResumeLayout(false);
			groupBox6.ResumeLayout(false);
			groupBox6.PerformLayout();
			((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
			panelIndividualStats.ResumeLayout(false);
			panelTotalStats.ResumeLayout(false);
			grpDebug.ResumeLayout(false);
			grpDebug.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnSelect;
		private System.Windows.Forms.FlowLayoutPanel panelThumnails;
		private System.Windows.Forms.Button btnAIPick;
		private System.Windows.Forms.TextBox txtWidth;
		private System.Windows.Forms.Button btnClearLC;
		private System.Windows.Forms.Button btnSelectLC;
		private System.Windows.Forms.TextBox txtCill;
		private System.Windows.Forms.TextBox txtLintel;
		private System.Windows.Forms.TextBox txtCentredWidth;
		private System.Windows.Forms.Button btnClearAroundCentre;
		private System.Windows.Forms.Button btnSelectAroundCentre;
		private System.Windows.Forms.Button btnClearCentre;
		private System.Windows.Forms.Button btnSelectCentre;
		private System.Windows.Forms.Button btnClearEqual;
		private System.Windows.Forms.Button btnSelectEqual;
		private System.Windows.Forms.Button btnClearAdjacent;
		private System.Windows.Forms.Button btnSelectAdjacent;
		private System.Windows.Forms.RadioButton rdoPickStartPt;
		private System.Windows.Forms.RadioButton rdoRightLeft;
		private System.Windows.Forms.RadioButton rdoLeftRight;
		private System.Windows.Forms.RadioButton rdoBounding;
		private System.Windows.Forms.RadioButton rdoGross;
		private System.Windows.Forms.RadioButton rdoNet;
		private System.Windows.Forms.TableLayoutPanel tlpIndividualArea;
		private System.Windows.Forms.PictureBox picPreview;
		private System.Windows.Forms.Panel panelIndividualStats;
		private System.Windows.Forms.TableLayoutPanel tlpIndividualPart;
		private System.Windows.Forms.Panel panelTotalStats;
		private System.Windows.Forms.TableLayoutPanel tlpTotalPart;
		private System.Windows.Forms.TableLayoutPanel tlpTotalArea;
		private System.Windows.Forms.TableLayoutPanel tlpTotalSelectedElements;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnHelp;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.GroupBox grpDebug;
		private System.Windows.Forms.TextBox txtDebug_PanelIndex;
		private System.Windows.Forms.CheckBox chkDebug_SplitLineNumbers;
		private System.Windows.Forms.CheckBox chkDebug_SplitPoints;
		private System.Windows.Forms.CheckBox chkDebug_LocationCurve;
		private System.Windows.Forms.CheckBox chkDebug_BoundaryPoints;
		private System.Windows.Forms.TableLayoutPanel tlpTotalSelectedOpenings;
		private System.Windows.Forms.Button btnViewInRevit;
		private System.Windows.Forms.Button btnTrain;
		private System.Windows.Forms.Button btnSetting;
		private System.Windows.Forms.Button btnApplyToAll;
	}
}