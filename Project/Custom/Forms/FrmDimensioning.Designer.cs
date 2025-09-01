namespace Architexor.Forms
{
	partial class FrmDimensioning
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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.GroupBox groupBox2;
			System.Windows.Forms.Label label10;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label3;
			this.btnGenerateDimensions = new System.Windows.Forms.Button();
			this.lblReferenceCount = new System.Windows.Forms.Label();
			this.txtGroupIndex = new System.Windows.Forms.TextBox();
			this.btnSelectGroup = new System.Windows.Forms.Button();
			this.btnGroupExteriorWalls = new System.Windows.Forms.Button();
			this.lblGroupCount = new System.Windows.Forms.Label();
			this.lblExteriorWallCount = new System.Windows.Forms.Label();
			this.btnGetExteriorWalls = new System.Windows.Forms.Button();
			this.chkSkipTiny = new System.Windows.Forms.CheckBox();
			this.txtThreshold = new System.Windows.Forms.TextBox();
			this.chkLeaderVisible = new System.Windows.Forms.CheckBox();
			this.chkAvoidOverlap = new System.Windows.Forms.CheckBox();
			this.cmbTextPosition = new System.Windows.Forms.ComboBox();
			this.cmbOrientation = new System.Windows.Forms.ComboBox();
			this.txtGapToBaseline = new System.Windows.Forms.TextBox();
			this.txtOffsetFromOrigin = new System.Windows.Forms.TextBox();
			this.lstViews = new System.Windows.Forms.CheckedListBox();
			this.dgDimReferOptions = new System.Windows.Forms.DataGridView();
			this.ApplyTo = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.ReferenceType = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.ConsiderOpenings = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.OpeningReferenceType = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.ConsiderIntersections = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.ConsiderGrids = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			label1 = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label6 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			groupBox2 = new System.Windows.Forms.GroupBox();
			label10 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgDimReferOptions)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(13, 13);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(33, 13);
			label1.TabIndex = 1;
			label1.Text = "View:";
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(this.btnGenerateDimensions);
			groupBox1.Controls.Add(this.lblReferenceCount);
			groupBox1.Controls.Add(label6);
			groupBox1.Controls.Add(this.txtGroupIndex);
			groupBox1.Controls.Add(this.btnSelectGroup);
			groupBox1.Controls.Add(label5);
			groupBox1.Controls.Add(this.btnGroupExteriorWalls);
			groupBox1.Controls.Add(this.lblGroupCount);
			groupBox1.Controls.Add(label4);
			groupBox1.Controls.Add(this.lblExteriorWallCount);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(this.btnGetExteriorWalls);
			groupBox1.Location = new System.Drawing.Point(172, 12);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(302, 219);
			groupBox1.TabIndex = 3;
			groupBox1.TabStop = false;
			groupBox1.Text = "Outside Dimensioning";
			// 
			// btnGenerateDimensions
			// 
			this.btnGenerateDimensions.Location = new System.Drawing.Point(15, 182);
			this.btnGenerateDimensions.Name = "btnGenerateDimensions";
			this.btnGenerateDimensions.Size = new System.Drawing.Size(131, 27);
			this.btnGenerateDimensions.TabIndex = 14;
			this.btnGenerateDimensions.Text = "Generate Dimensions";
			this.btnGenerateDimensions.UseVisualStyleBackColor = true;
			this.btnGenerateDimensions.Click += new System.EventHandler(this.btnGenerateDimensions_Click);
			// 
			// lblReferenceCount
			// 
			this.lblReferenceCount.Location = new System.Drawing.Point(265, 151);
			this.lblReferenceCount.Name = "lblReferenceCount";
			this.lblReferenceCount.Size = new System.Drawing.Size(25, 13);
			this.lblReferenceCount.TabIndex = 13;
			this.lblReferenceCount.Text = "10";
			this.lblReferenceCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(165, 151);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(94, 13);
			label6.TabIndex = 12;
			label6.Text = "Reference Count: ";
			// 
			// txtGroupIndex
			// 
			this.txtGroupIndex.Location = new System.Drawing.Point(266, 119);
			this.txtGroupIndex.Name = "txtGroupIndex";
			this.txtGroupIndex.Size = new System.Drawing.Size(25, 20);
			this.txtGroupIndex.TabIndex = 11;
			this.txtGroupIndex.Text = "1";
			this.txtGroupIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// btnSelectGroup
			// 
			this.btnSelectGroup.Location = new System.Drawing.Point(15, 116);
			this.btnSelectGroup.Name = "btnSelectGroup";
			this.btnSelectGroup.Size = new System.Drawing.Size(131, 27);
			this.btnSelectGroup.TabIndex = 10;
			this.btnSelectGroup.Text = "Select Group";
			this.btnSelectGroup.UseVisualStyleBackColor = true;
			this.btnSelectGroup.Click += new System.EventHandler(this.btnSelectGroup_Click);
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(188, 122);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(71, 13);
			label5.TabIndex = 9;
			label5.Text = "Group Index: ";
			// 
			// btnGroupExteriorWalls
			// 
			this.btnGroupExteriorWalls.Location = new System.Drawing.Point(15, 83);
			this.btnGroupExteriorWalls.Name = "btnGroupExteriorWalls";
			this.btnGroupExteriorWalls.Size = new System.Drawing.Size(131, 27);
			this.btnGroupExteriorWalls.TabIndex = 8;
			this.btnGroupExteriorWalls.Text = "Group Exterior Walls";
			this.btnGroupExteriorWalls.UseVisualStyleBackColor = true;
			this.btnGroupExteriorWalls.Click += new System.EventHandler(this.btnGroupExteriorWalls_Click);
			// 
			// lblGroupCount
			// 
			this.lblGroupCount.Location = new System.Drawing.Point(265, 89);
			this.lblGroupCount.Name = "lblGroupCount";
			this.lblGroupCount.Size = new System.Drawing.Size(25, 13);
			this.lblGroupCount.TabIndex = 7;
			this.lblGroupCount.Text = "10";
			this.lblGroupCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(188, 89);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(73, 13);
			label4.TabIndex = 6;
			label4.Text = "Group Count: ";
			// 
			// lblExteriorWallCount
			// 
			this.lblExteriorWallCount.Location = new System.Drawing.Point(265, 24);
			this.lblExteriorWallCount.Name = "lblExteriorWallCount";
			this.lblExteriorWallCount.Size = new System.Drawing.Size(25, 13);
			this.lblExteriorWallCount.TabIndex = 5;
			this.lblExteriorWallCount.Text = "10";
			this.lblExteriorWallCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(158, 24);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(103, 13);
			label2.TabIndex = 4;
			label2.Text = "Exterior Wall Count: ";
			// 
			// btnGetExteriorWalls
			// 
			this.btnGetExteriorWalls.Location = new System.Drawing.Point(15, 24);
			this.btnGetExteriorWalls.Name = "btnGetExteriorWalls";
			this.btnGetExteriorWalls.Size = new System.Drawing.Size(131, 53);
			this.btnGetExteriorWalls.TabIndex = 3;
			this.btnGetExteriorWalls.Text = "Detect and Select Exterior Walls";
			this.btnGetExteriorWalls.UseVisualStyleBackColor = true;
			this.btnGetExteriorWalls.Click += new System.EventHandler(this.btnGetExteriorWalls_Click);
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(label10);
			groupBox2.Controls.Add(this.chkSkipTiny);
			groupBox2.Controls.Add(this.txtThreshold);
			groupBox2.Controls.Add(this.chkLeaderVisible);
			groupBox2.Controls.Add(this.chkAvoidOverlap);
			groupBox2.Controls.Add(this.cmbTextPosition);
			groupBox2.Controls.Add(label9);
			groupBox2.Controls.Add(this.cmbOrientation);
			groupBox2.Controls.Add(label8);
			groupBox2.Controls.Add(this.txtGapToBaseline);
			groupBox2.Controls.Add(label7);
			groupBox2.Controls.Add(this.txtOffsetFromOrigin);
			groupBox2.Controls.Add(label3);
			groupBox2.Location = new System.Drawing.Point(481, 13);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(269, 218);
			groupBox2.TabIndex = 4;
			groupBox2.TabStop = false;
			groupBox2.Text = "Options";
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.Location = new System.Drawing.Point(65, 191);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(84, 13);
			label10.TabIndex = 24;
			label10.Text = "Threshold(Inch):";
			// 
			// chkSkipTiny
			// 
			this.chkSkipTiny.AutoSize = true;
			this.chkSkipTiny.Location = new System.Drawing.Point(34, 165);
			this.chkSkipTiny.Name = "chkSkipTiny";
			this.chkSkipTiny.Size = new System.Drawing.Size(127, 17);
			this.chkSkipTiny.TabIndex = 23;
			this.chkSkipTiny.Text = "Skip Tiny Dimensions";
			this.chkSkipTiny.UseVisualStyleBackColor = true;
			// 
			// txtThreshold
			// 
			this.txtThreshold.Location = new System.Drawing.Point(167, 188);
			this.txtThreshold.Name = "txtThreshold";
			this.txtThreshold.Size = new System.Drawing.Size(33, 20);
			this.txtThreshold.TabIndex = 22;
			this.txtThreshold.Text = "1";
			this.txtThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// chkLeaderVisible
			// 
			this.chkLeaderVisible.AutoSize = true;
			this.chkLeaderVisible.Location = new System.Drawing.Point(155, 136);
			this.chkLeaderVisible.Name = "chkLeaderVisible";
			this.chkLeaderVisible.Size = new System.Drawing.Size(89, 17);
			this.chkLeaderVisible.TabIndex = 21;
			this.chkLeaderVisible.Text = "Show Leader";
			this.chkLeaderVisible.UseVisualStyleBackColor = true;
			// 
			// chkAvoidOverlap
			// 
			this.chkAvoidOverlap.AutoSize = true;
			this.chkAvoidOverlap.Location = new System.Drawing.Point(34, 136);
			this.chkAvoidOverlap.Name = "chkAvoidOverlap";
			this.chkAvoidOverlap.Size = new System.Drawing.Size(93, 17);
			this.chkAvoidOverlap.TabIndex = 20;
			this.chkAvoidOverlap.Text = "Avoid Overlap";
			this.chkAvoidOverlap.UseVisualStyleBackColor = true;
			// 
			// cmbTextPosition
			// 
			this.cmbTextPosition.FormattingEnabled = true;
			this.cmbTextPosition.Items.AddRange(new object[] {
            "Up",
            "Auto"});
			this.cmbTextPosition.Location = new System.Drawing.Point(167, 99);
			this.cmbTextPosition.Name = "cmbTextPosition";
			this.cmbTextPosition.Size = new System.Drawing.Size(93, 21);
			this.cmbTextPosition.TabIndex = 19;
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(78, 102);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(71, 13);
			label9.TabIndex = 18;
			label9.Text = "Text Position:";
			// 
			// cmbOrientation
			// 
			this.cmbOrientation.FormattingEnabled = true;
			this.cmbOrientation.Items.AddRange(new object[] {
            "Aligned",
            "Axis"});
			this.cmbOrientation.Location = new System.Drawing.Point(167, 72);
			this.cmbOrientation.Name = "cmbOrientation";
			this.cmbOrientation.Size = new System.Drawing.Size(93, 21);
			this.cmbOrientation.TabIndex = 17;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(88, 75);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(61, 13);
			label8.TabIndex = 16;
			label8.Text = "Orientation:";
			// 
			// txtGapToBaseline
			// 
			this.txtGapToBaseline.Location = new System.Drawing.Point(167, 45);
			this.txtGapToBaseline.Name = "txtGapToBaseline";
			this.txtGapToBaseline.Size = new System.Drawing.Size(33, 20);
			this.txtGapToBaseline.TabIndex = 15;
			this.txtGapToBaseline.Text = "2";
			this.txtGapToBaseline.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(33, 48);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(116, 13);
			label7.TabIndex = 14;
			label7.Text = "Gap to BaseLine(Inch):";
			// 
			// txtOffsetFromOrigin
			// 
			this.txtOffsetFromOrigin.Location = new System.Drawing.Point(167, 19);
			this.txtOffsetFromOrigin.Name = "txtOffsetFromOrigin";
			this.txtOffsetFromOrigin.Size = new System.Drawing.Size(33, 20);
			this.txtOffsetFromOrigin.TabIndex = 13;
			this.txtOffsetFromOrigin.Text = "0.5";
			this.txtOffsetFromOrigin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(31, 22);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(118, 13);
			label3.TabIndex = 12;
			label3.Text = "Offset from Origin(Inch):";
			// 
			// lstViews
			// 
			this.lstViews.FormattingEnabled = true;
			this.lstViews.Location = new System.Drawing.Point(12, 32);
			this.lstViews.Name = "lstViews";
			this.lstViews.Size = new System.Drawing.Size(154, 394);
			this.lstViews.TabIndex = 0;
			// 
			// dgDimReferOptions
			// 
			this.dgDimReferOptions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgDimReferOptions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ApplyTo,
            this.ReferenceType,
            this.ConsiderOpenings,
            this.OpeningReferenceType,
            this.ConsiderIntersections,
            this.ConsiderGrids});
			this.dgDimReferOptions.Location = new System.Drawing.Point(172, 237);
			this.dgDimReferOptions.Name = "dgDimReferOptions";
			this.dgDimReferOptions.Size = new System.Drawing.Size(643, 183);
			this.dgDimReferOptions.TabIndex = 26;
			// 
			// ApplyTo
			// 
			this.ApplyTo.HeaderText = "ApplyTo";
			this.ApplyTo.Items.AddRange(new object[] {
            "Outside",
            "Inside"});
			this.ApplyTo.Name = "ApplyTo";
			// 
			// ReferenceType
			// 
			this.ReferenceType.HeaderText = "Wall Reference Type";
			this.ReferenceType.Items.AddRange(new object[] {
            "Wall Centerline",
            "Wall Face",
            "Core Centerline",
            "Core Face"});
			this.ReferenceType.Name = "ReferenceType";
			// 
			// ConsiderOpenings
			// 
			this.ConsiderOpenings.HeaderText = "Consider Openings";
			this.ConsiderOpenings.Name = "ConsiderOpenings";
			// 
			// OpeningReferenceType
			// 
			this.OpeningReferenceType.HeaderText = "Opening Reference Type";
			this.OpeningReferenceType.Items.AddRange(new object[] {
            "Width",
            "Center"});
			this.OpeningReferenceType.Name = "OpeningReferenceType";
			// 
			// ConsiderIntersections
			// 
			this.ConsiderIntersections.HeaderText = "Consider Intersections";
			this.ConsiderIntersections.Name = "ConsiderIntersections";
			// 
			// ConsiderGrids
			// 
			this.ConsiderGrids.HeaderText = "Consider Grids";
			this.ConsiderGrids.Name = "ConsiderGrids";
			// 
			// FrmDimensioning
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(825, 439);
			this.Controls.Add(this.dgDimReferOptions);
			this.Controls.Add(groupBox2);
			this.Controls.Add(groupBox1);
			this.Controls.Add(label1);
			this.Controls.Add(this.lstViews);
			this.Name = "FrmDimensioning";
			this.ShowIcon = false;
			this.Text = "Auto Dimensioning";
			this.Load += new System.EventHandler(this.FrmDimensioning_Load);
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgDimReferOptions)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckedListBox lstViews;
		private System.Windows.Forms.Button btnGetExteriorWalls;
		private System.Windows.Forms.Label lblExteriorWallCount;
		private System.Windows.Forms.Label lblGroupCount;
		private System.Windows.Forms.Button btnGroupExteriorWalls;
		private System.Windows.Forms.Button btnSelectGroup;
		private System.Windows.Forms.TextBox txtGroupIndex;
		private System.Windows.Forms.Label lblReferenceCount;
		private System.Windows.Forms.Button btnGenerateDimensions;
		private System.Windows.Forms.TextBox txtOffsetFromOrigin;
		private System.Windows.Forms.TextBox txtGapToBaseline;
		private System.Windows.Forms.ComboBox cmbOrientation;
		private System.Windows.Forms.ComboBox cmbTextPosition;
		private System.Windows.Forms.CheckBox chkAvoidOverlap;
		private System.Windows.Forms.CheckBox chkLeaderVisible;
		private System.Windows.Forms.CheckBox chkSkipTiny;
		private System.Windows.Forms.TextBox txtThreshold;
		private System.Windows.Forms.DataGridView dgDimReferOptions;
		private System.Windows.Forms.DataGridViewComboBoxColumn ApplyTo;
		private System.Windows.Forms.DataGridViewComboBoxColumn ReferenceType;
		private System.Windows.Forms.DataGridViewCheckBoxColumn ConsiderOpenings;
		private System.Windows.Forms.DataGridViewComboBoxColumn OpeningReferenceType;
		private System.Windows.Forms.DataGridViewCheckBoxColumn ConsiderIntersections;
		private System.Windows.Forms.DataGridViewCheckBoxColumn ConsiderGrids;
	}
}