
namespace Architexor.Forms
{
	partial class Frm3DProperties
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
			System.Windows.Forms.Label lblElementSelection;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			this.btnSelectElement = new System.Windows.Forms.Button();
			this.btnClearElement = new System.Windows.Forms.Button();
			this.chkIncludeArrow = new System.Windows.Forms.CheckBox();
			this.chkFlipSide = new System.Windows.Forms.CheckBox();
			this.txtThickness = new System.Windows.Forms.TextBox();
			this.txtScale = new System.Windows.Forms.TextBox();
			this.lstSelectedParameters = new System.Windows.Forms.ListBox();
			this.cmbFilter = new System.Windows.Forms.ComboBox();
			this.txtSearch = new System.Windows.Forms.TextBox();
			this.btnNext = new System.Windows.Forms.Button();
			this.btnPrev = new System.Windows.Forms.Button();
			this.btnAddPrivateParameter = new System.Windows.Forms.Button();
			this.lstParameters = new System.Windows.Forms.ListBox();
			this.pnSettings = new System.Windows.Forms.Panel();
			this.txtVersion = new System.Windows.Forms.Label();
			this.btnHelp = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnUp = new System.Windows.Forms.Button();
			this.btnDown = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			lblElementSelection = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			this.pnSettings.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnSelectElement
			// 
			this.btnSelectElement.BackColor = System.Drawing.SystemColors.Control;
			this.btnSelectElement.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSelectElement.Location = new System.Drawing.Point(9, 40);
			this.btnSelectElement.Name = "btnSelectElement";
			this.btnSelectElement.Size = new System.Drawing.Size(109, 23);
			this.btnSelectElement.TabIndex = 40;
			this.btnSelectElement.Text = "Select";
			this.btnSelectElement.UseVisualStyleBackColor = true;
			this.btnSelectElement.Click += new System.EventHandler(this.btnSelectElement_Click);
			// 
			// btnClearElement
			// 
			this.btnClearElement.BackColor = System.Drawing.SystemColors.Control;
			this.btnClearElement.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnClearElement.Location = new System.Drawing.Point(124, 40);
			this.btnClearElement.Name = "btnClearElement";
			this.btnClearElement.Size = new System.Drawing.Size(109, 23);
			this.btnClearElement.TabIndex = 41;
			this.btnClearElement.Text = "Remove";
			this.btnClearElement.UseVisualStyleBackColor = true;
			this.btnClearElement.Click += new System.EventHandler(this.btnClearElement_Click);
			// 
			// lblElementSelection
			// 
			lblElementSelection.AutoSize = true;
			lblElementSelection.BackColor = System.Drawing.Color.Transparent;
			lblElementSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			lblElementSelection.Location = new System.Drawing.Point(1, 4);
			lblElementSelection.Name = "lblElementSelection";
			lblElementSelection.Size = new System.Drawing.Size(98, 13);
			lblElementSelection.TabIndex = 43;
			lblElementSelection.Text = "Select Elements";
			// 
			// chkIncludeArrow
			// 
			this.chkIncludeArrow.AutoSize = true;
			this.chkIncludeArrow.Checked = true;
			this.chkIncludeArrow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkIncludeArrow.Location = new System.Drawing.Point(11, 48);
			this.chkIncludeArrow.Name = "chkIncludeArrow";
			this.chkIncludeArrow.Size = new System.Drawing.Size(167, 17);
			this.chkIncludeArrow.TabIndex = 1;
			this.chkIncludeArrow.Text = "Include Structural Span Arrow";
			this.chkIncludeArrow.UseVisualStyleBackColor = true;
			// 
			// chkFlipSide
			// 
			this.chkFlipSide.AutoSize = true;
			this.chkFlipSide.Location = new System.Drawing.Point(11, 23);
			this.chkFlipSide.Name = "chkFlipSide";
			this.chkFlipSide.Size = new System.Drawing.Size(64, 17);
			this.chkFlipSide.TabIndex = 0;
			this.chkFlipSide.Text = "Flip side";
			this.chkFlipSide.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.BackColor = System.Drawing.Color.Transparent;
			label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label1.Location = new System.Drawing.Point(2, 4);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(70, 13);
			label1.TabIndex = 43;
			label1.Text = "Parameters";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.BackColor = System.Drawing.Color.Transparent;
			label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label5.Location = new System.Drawing.Point(11, 23);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(56, 13);
			label5.TabIndex = 71;
			label5.Text = "Thickness";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.BackColor = System.Drawing.Color.Transparent;
			label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label4.Location = new System.Drawing.Point(11, 49);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(34, 13);
			label4.TabIndex = 70;
			label4.Text = "Scale";
			// 
			// txtThickness
			// 
			this.txtThickness.BackColor = System.Drawing.Color.White;
			this.txtThickness.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtThickness.Location = new System.Drawing.Point(146, 20);
			this.txtThickness.Name = "txtThickness";
			this.txtThickness.Size = new System.Drawing.Size(65, 20);
			this.txtThickness.TabIndex = 68;
			this.txtThickness.Text = "10";
			this.txtThickness.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtScale
			// 
			this.txtScale.BackColor = System.Drawing.Color.White;
			this.txtScale.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtScale.Location = new System.Drawing.Point(146, 46);
			this.txtScale.Name = "txtScale";
			this.txtScale.Size = new System.Drawing.Size(65, 20);
			this.txtScale.TabIndex = 69;
			this.txtScale.Text = "1";
			this.txtScale.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// lstSelectedParameters
			// 
			this.lstSelectedParameters.FormattingEnabled = true;
			this.lstSelectedParameters.Location = new System.Drawing.Point(8, 19);
			this.lstSelectedParameters.Name = "lstSelectedParameters";
			this.lstSelectedParameters.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstSelectedParameters.Size = new System.Drawing.Size(207, 342);
			this.lstSelectedParameters.TabIndex = 46;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.BackColor = System.Drawing.Color.Transparent;
			label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label3.Location = new System.Drawing.Point(242, 42);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(29, 13);
			label3.TabIndex = 72;
			label3.Text = "Filter";
			// 
			// cmbFilter
			// 
			this.cmbFilter.FormattingEnabled = true;
			this.cmbFilter.Items.AddRange(new object[] {
            "Instance",
            "Type",
            "Both",
            "Common"});
			this.cmbFilter.Location = new System.Drawing.Point(315, 39);
			this.cmbFilter.Name = "cmbFilter";
			this.cmbFilter.Size = new System.Drawing.Size(148, 21);
			this.cmbFilter.TabIndex = 70;
			this.cmbFilter.SelectedIndexChanged += new System.EventHandler(this.cmbFilter_SelectedIndexChanged);
			// 
			// txtSearch
			// 
			this.txtSearch.BackColor = System.Drawing.Color.White;
			this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtSearch.Location = new System.Drawing.Point(239, 65);
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.Size = new System.Drawing.Size(224, 20);
			this.txtSearch.TabIndex = 69;
			this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
			// 
			// btnNext
			// 
			this.btnNext.BackColor = System.Drawing.SystemColors.Control;
			this.btnNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnNext.Location = new System.Drawing.Point(302, 611);
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(58, 23);
			this.btnNext.TabIndex = 62;
			this.btnNext.Text = "Next";
			this.btnNext.UseVisualStyleBackColor = true;
			this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
			// 
			// btnPrev
			// 
			this.btnPrev.BackColor = System.Drawing.SystemColors.Control;
			this.btnPrev.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnPrev.Location = new System.Drawing.Point(238, 611);
			this.btnPrev.Name = "btnPrev";
			this.btnPrev.Size = new System.Drawing.Size(58, 23);
			this.btnPrev.TabIndex = 61;
			this.btnPrev.Text = "Previous";
			this.btnPrev.UseVisualStyleBackColor = true;
			this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
			// 
			// btnAddPrivateParameter
			// 
			this.btnAddPrivateParameter.BackColor = System.Drawing.SystemColors.Control;
			this.btnAddPrivateParameter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnAddPrivateParameter.Location = new System.Drawing.Point(406, 611);
			this.btnAddPrivateParameter.Name = "btnAddPrivateParameter";
			this.btnAddPrivateParameter.Size = new System.Drawing.Size(58, 23);
			this.btnAddPrivateParameter.TabIndex = 60;
			this.btnAddPrivateParameter.Text = "Add";
			this.btnAddPrivateParameter.UseVisualStyleBackColor = true;
			this.btnAddPrivateParameter.Click += new System.EventHandler(this.btnAddParameter_Click);
			// 
			// lstParameters
			// 
			this.lstParameters.FormattingEnabled = true;
			this.lstParameters.Location = new System.Drawing.Point(239, 84);
			this.lstParameters.Name = "lstParameters";
			this.lstParameters.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstParameters.Size = new System.Drawing.Size(224, 524);
			this.lstParameters.Sorted = true;
			this.lstParameters.TabIndex = 57;
			this.lstParameters.DoubleClick += new System.EventHandler(this.lstParameters_DoubleClick);
			// 
			// pnSettings
			// 
			this.pnSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
			this.pnSettings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnSettings.Controls.Add(lblElementSelection);
			this.pnSettings.Location = new System.Drawing.Point(9, 9);
			this.pnSettings.Name = "pnSettings";
			this.pnSettings.Size = new System.Drawing.Size(224, 24);
			this.pnSettings.TabIndex = 113;
			this.pnSettings.Tag = "";
			// 
			// txtVersion
			// 
			this.txtVersion.AutoSize = true;
			this.txtVersion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(187)))), ((int)(((byte)(179)))));
			this.txtVersion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtVersion.Location = new System.Drawing.Point(136, 647);
			this.txtVersion.MaximumSize = new System.Drawing.Size(75, 20);
			this.txtVersion.MinimumSize = new System.Drawing.Size(75, 20);
			this.txtVersion.Name = "txtVersion";
			this.txtVersion.Size = new System.Drawing.Size(75, 20);
			this.txtVersion.TabIndex = 57;
			this.txtVersion.Text = "1.0.0";
			this.txtVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnHelp
			// 
			this.btnHelp.BackColor = System.Drawing.SystemColors.Control;
			this.btnHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnHelp.Location = new System.Drawing.Point(9, 645);
			this.btnHelp.Name = "btnHelp";
			this.btnHelp.Size = new System.Drawing.Size(109, 23);
			this.btnHelp.TabIndex = 28;
			this.btnHelp.Text = "Help";
			this.btnHelp.UseVisualStyleBackColor = true;
			this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCancel.Location = new System.Drawing.Point(355, 645);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(109, 23);
			this.btnCancel.TabIndex = 25;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.SystemColors.Control;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Enabled = false;
			this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnOK.Location = new System.Drawing.Point(238, 645);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(109, 23);
			this.btnOK.TabIndex = 24;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(label1);
			this.panel1.Location = new System.Drawing.Point(239, 9);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(224, 24);
			this.panel1.TabIndex = 125;
			this.panel1.Tag = "";
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox1.Controls.Add(label5);
			this.groupBox1.Controls.Add(label4);
			this.groupBox1.Controls.Add(this.txtScale);
			this.groupBox1.Controls.Add(this.txtThickness);
			this.groupBox1.Location = new System.Drawing.Point(9, 70);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(224, 76);
			this.groupBox1.TabIndex = 130;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Parameters";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.chkIncludeArrow);
			this.groupBox2.Controls.Add(this.chkFlipSide);
			this.groupBox2.Location = new System.Drawing.Point(9, 155);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(224, 76);
			this.groupBox2.TabIndex = 131;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Modifiers";
			// 
			// btnRemove
			// 
			this.btnRemove.BackColor = System.Drawing.SystemColors.Control;
			this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnRemove.Location = new System.Drawing.Point(159, 367);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(58, 23);
			this.btnRemove.TabIndex = 47;
			this.btnRemove.Text = "Remove";
			this.btnRemove.UseVisualStyleBackColor = true;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnUp
			// 
			this.btnUp.BackColor = System.Drawing.SystemColors.Control;
			this.btnUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnUp.Location = new System.Drawing.Point(7, 367);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(58, 23);
			this.btnUp.TabIndex = 48;
			this.btnUp.Text = "Up";
			this.btnUp.UseVisualStyleBackColor = true;
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnDown
			// 
			this.btnDown.BackColor = System.Drawing.SystemColors.Control;
			this.btnDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnDown.Location = new System.Drawing.Point(71, 367);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(58, 23);
			this.btnDown.TabIndex = 49;
			this.btnDown.Text = "Down";
			this.btnDown.UseVisualStyleBackColor = true;
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.btnDown);
			this.groupBox3.Controls.Add(this.lstSelectedParameters);
			this.groupBox3.Controls.Add(this.btnUp);
			this.groupBox3.Controls.Add(this.btnRemove);
			this.groupBox3.Location = new System.Drawing.Point(9, 241);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(224, 397);
			this.groupBox3.TabIndex = 132;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Selected";
			// 
			// Frm3DProperties
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(471, 676);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnSelectElement);
			this.Controls.Add(this.btnClearElement);
			this.Controls.Add(label3);
			this.Controls.Add(this.txtVersion);
			this.Controls.Add(this.cmbFilter);
			this.Controls.Add(this.txtSearch);
			this.Controls.Add(this.btnNext);
			this.Controls.Add(this.btnHelp);
			this.Controls.Add(this.btnPrev);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnAddPrivateParameter);
			this.Controls.Add(this.lstParameters);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.pnSettings);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Frm3DProperties";
			this.ShowIcon = false;
			this.Text = "3D PARAMETERS";
			this.Load += new System.EventHandler(this.Frm3DProperties_Load);
			this.pnSettings.ResumeLayout(false);
			this.pnSettings.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnSelectElement;
		private System.Windows.Forms.Button btnClearElement;
		private System.Windows.Forms.Panel pnSettings;
		private System.Windows.Forms.Label txtVersion;
		private System.Windows.Forms.Button btnHelp;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox chkIncludeArrow;
		private System.Windows.Forms.CheckBox chkFlipSide;
		private System.Windows.Forms.TextBox txtThickness;
		private System.Windows.Forms.TextBox txtScale;
		private System.Windows.Forms.ListBox lstSelectedParameters;
		private System.Windows.Forms.ListBox lstParameters;
		private System.Windows.Forms.Button btnAddPrivateParameter;
		private System.Windows.Forms.Button btnPrev;
		private System.Windows.Forms.Button btnNext;
		private System.Windows.Forms.TextBox txtSearch;
		private System.Windows.Forms.ComboBox cmbFilter;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.GroupBox groupBox3;
	}
}