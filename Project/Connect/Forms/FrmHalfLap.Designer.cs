
using System.Drawing;
using System.Windows.Forms;

namespace Architexor.Connect.Forms
{
	partial class FrmHalfLap
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
			Panel pnPreview;
			Label lblPreview;
			Label lblPartSelection;
			Label label3;
			Label label5;
			Label label4;
			Label label2;
			Label label1;
			Label lblAroundCentrePartLength;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmHalfLap));
			btnSelectElement = new Button();
			btnClearElement = new Button();
			cmbPreset = new ComboBox();
			btnPresetDelete = new Button();
			btnPresetSave = new Button();
			btnPresetSelect = new Button();
			txtMinLengthLimit = new TextBox();
			chkFlip = new CheckBox();
			chkAddHorzHL = new CheckBox();
			txtHLDepth = new TextBox();
			txtSideAGap = new TextBox();
			txtSideBGap = new TextBox();
			txtLapWidth = new TextBox();
			txtGapBetweenPanels = new TextBox();
			optPushToB = new RadioButton();
			optPushToA = new RadioButton();
			optCentred = new RadioButton();
			txtVersion = new Label();
			btnHelp = new Button();
			btnCancel = new Button();
			btnOK = new Button();
			pnPartSelectionTitle = new Panel();
			picPreview = new PictureBox();
			groupBox1 = new GroupBox();
			groupBox2 = new GroupBox();
			groupBox3 = new GroupBox();
			groupBox4 = new GroupBox();
			groupBox5 = new GroupBox();
			pnPreview = new Panel();
			lblPreview = new Label();
			lblPartSelection = new Label();
			label3 = new Label();
			label5 = new Label();
			label4 = new Label();
			label2 = new Label();
			label1 = new Label();
			lblAroundCentrePartLength = new Label();
			pnPreview.SuspendLayout();
			pnPartSelectionTitle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			groupBox3.SuspendLayout();
			groupBox4.SuspendLayout();
			groupBox5.SuspendLayout();
			SuspendLayout();
			// 
			// pnPreview
			// 
			pnPreview.BackColor = Color.FromArgb(153, 180, 209);
			pnPreview.BorderStyle = BorderStyle.FixedSingle;
			pnPreview.Controls.Add(lblPreview);
			pnPreview.Location = new Point(363, 6);
			pnPreview.Margin = new Padding(4, 3, 4, 3);
			pnPreview.Name = "pnPreview";
			pnPreview.Size = new Size(402, 27);
			pnPreview.TabIndex = 102;
			pnPreview.Tag = "";
			// 
			// lblPreview
			// 
			lblPreview.AutoSize = true;
			lblPreview.BackColor = Color.Transparent;
			lblPreview.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblPreview.Location = new Point(4, 5);
			lblPreview.Margin = new Padding(4, 0, 4, 0);
			lblPreview.Name = "lblPreview";
			lblPreview.Size = new Size(103, 13);
			lblPreview.TabIndex = 43;
			lblPreview.Text = "Preview - Guides";
			// 
			// lblPartSelection
			// 
			lblPartSelection.AutoSize = true;
			lblPartSelection.BackColor = Color.Transparent;
			lblPartSelection.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblPartSelection.Location = new Point(1, 5);
			lblPartSelection.Margin = new Padding(4, 0, 4, 0);
			lblPartSelection.Name = "lblPartSelection";
			lblPartSelection.Size = new Size(165, 13);
			lblPartSelection.TabIndex = 43;
			lblPartSelection.Text = "Part Selection && Parameters";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.BackColor = Color.Transparent;
			label3.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			label3.Location = new Point(15, 55);
			label3.Margin = new Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new Size(155, 13);
			label3.TabIndex = 57;
			label3.Text = "Exclude Half Lap lengths under";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.BackColor = Color.Transparent;
			label5.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			label5.Location = new Point(13, 29);
			label5.Margin = new Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new Size(70, 13);
			label5.TabIndex = 57;
			label5.Text = "1 - Set Depth";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.BackColor = Color.Transparent;
			label4.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			label4.Location = new Point(13, 59);
			label4.Margin = new Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new Size(83, 13);
			label4.TabIndex = 56;
			label4.Text = "2 - Set lap width";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.BackColor = Color.Transparent;
			label2.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			label2.Location = new Point(13, 88);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(127, 13);
			label2.TabIndex = 54;
			label2.Text = "3 - Tolerance Gap Side A";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.BackColor = Color.Transparent;
			label1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			label1.Location = new Point(13, 118);
			label1.Margin = new Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new Size(127, 13);
			label1.TabIndex = 53;
			label1.Text = "4 - Tolerance Gap Side B";
			// 
			// lblAroundCentrePartLength
			// 
			lblAroundCentrePartLength.AutoSize = true;
			lblAroundCentrePartLength.BackColor = Color.Transparent;
			lblAroundCentrePartLength.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblAroundCentrePartLength.Location = new Point(13, 148);
			lblAroundCentrePartLength.Margin = new Padding(4, 0, 4, 0);
			lblAroundCentrePartLength.Name = "lblAroundCentrePartLength";
			lblAroundCentrePartLength.Size = new Size(120, 13);
			lblAroundCentrePartLength.TabIndex = 43;
			lblAroundCentrePartLength.Text = "5 - Gap between panels";
			// 
			// btnSelectElement
			// 
			btnSelectElement.BackColor = SystemColors.Control;
			btnSelectElement.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnSelectElement.Location = new Point(16, 28);
			btnSelectElement.Margin = new Padding(4, 3, 4, 3);
			btnSelectElement.Name = "btnSelectElement";
			btnSelectElement.Size = new Size(88, 27);
			btnSelectElement.TabIndex = 40;
			btnSelectElement.Text = "Select";
			btnSelectElement.UseVisualStyleBackColor = true;
			btnSelectElement.Click += btnSelectElement_Click;
			// 
			// btnClearElement
			// 
			btnClearElement.BackColor = SystemColors.Control;
			btnClearElement.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnClearElement.Location = new Point(111, 28);
			btnClearElement.Margin = new Padding(4, 3, 4, 3);
			btnClearElement.Name = "btnClearElement";
			btnClearElement.Size = new Size(88, 27);
			btnClearElement.TabIndex = 41;
			btnClearElement.Text = "Remove";
			btnClearElement.UseVisualStyleBackColor = true;
			btnClearElement.Click += btnClearElement_Click;
			// 
			// cmbPreset
			// 
			cmbPreset.BackColor = Color.White;
			cmbPreset.FormattingEnabled = true;
			cmbPreset.Location = new Point(106, 22);
			cmbPreset.Margin = new Padding(4, 3, 4, 3);
			cmbPreset.Name = "cmbPreset";
			cmbPreset.Size = new Size(234, 23);
			cmbPreset.TabIndex = 47;
			// 
			// btnPresetDelete
			// 
			btnPresetDelete.BackColor = SystemColors.Control;
			btnPresetDelete.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnPresetDelete.Location = new Point(12, 82);
			btnPresetDelete.Margin = new Padding(4, 3, 4, 3);
			btnPresetDelete.Name = "btnPresetDelete";
			btnPresetDelete.Size = new Size(88, 27);
			btnPresetDelete.TabIndex = 43;
			btnPresetDelete.Text = "Delete";
			btnPresetDelete.UseVisualStyleBackColor = true;
			btnPresetDelete.Click += btnPresetDelete_Click;
			// 
			// btnPresetSave
			// 
			btnPresetSave.BackColor = SystemColors.Control;
			btnPresetSave.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnPresetSave.Location = new Point(12, 52);
			btnPresetSave.Margin = new Padding(4, 3, 4, 3);
			btnPresetSave.Name = "btnPresetSave";
			btnPresetSave.Size = new Size(88, 27);
			btnPresetSave.TabIndex = 42;
			btnPresetSave.Text = "Save";
			btnPresetSave.UseVisualStyleBackColor = true;
			btnPresetSave.Click += btnPresetSave_Click;
			// 
			// btnPresetSelect
			// 
			btnPresetSelect.BackColor = SystemColors.Control;
			btnPresetSelect.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnPresetSelect.Location = new Point(12, 22);
			btnPresetSelect.Margin = new Padding(4, 3, 4, 3);
			btnPresetSelect.Name = "btnPresetSelect";
			btnPresetSelect.Size = new Size(88, 27);
			btnPresetSelect.TabIndex = 41;
			btnPresetSelect.Text = "Select";
			btnPresetSelect.UseVisualStyleBackColor = true;
			btnPresetSelect.Click += btnPresetSelect_Click;
			// 
			// txtMinLengthLimit
			// 
			txtMinLengthLimit.BackColor = Color.White;
			txtMinLengthLimit.BorderStyle = BorderStyle.FixedSingle;
			txtMinLengthLimit.Location = new Point(266, 52);
			txtMinLengthLimit.Margin = new Padding(4, 3, 4, 3);
			txtMinLengthLimit.Name = "txtMinLengthLimit";
			txtMinLengthLimit.Size = new Size(76, 23);
			txtMinLengthLimit.TabIndex = 58;
			txtMinLengthLimit.Text = "100";
			txtMinLengthLimit.TextAlign = HorizontalAlignment.Right;
			txtMinLengthLimit.KeyPress += txtMinLimitLength_KeyPress;
			// 
			// chkFlip
			// 
			chkFlip.AutoSize = true;
			chkFlip.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			chkFlip.Location = new Point(14, 27);
			chkFlip.Margin = new Padding(4, 3, 4, 3);
			chkFlip.Name = "chkFlip";
			chkFlip.Size = new Size(64, 17);
			chkFlip.TabIndex = 55;
			chkFlip.Text = "Flip side";
			chkFlip.UseVisualStyleBackColor = true;
			// 
			// chkAddHorzHL
			// 
			chkAddHorzHL.AutoSize = true;
			chkAddHorzHL.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			chkAddHorzHL.Location = new Point(14, 54);
			chkAddHorzHL.Margin = new Padding(4, 3, 4, 3);
			chkAddHorzHL.Name = "chkAddHorzHL";
			chkAddHorzHL.Size = new Size(216, 17);
			chkAddHorzHL.TabIndex = 56;
			chkAddHorzHL.Text = "Included Half Lap for Lintel && Sill bearing";
			chkAddHorzHL.UseVisualStyleBackColor = true;
			chkAddHorzHL.Visible = false;
			// 
			// txtHLDepth
			// 
			txtHLDepth.BackColor = Color.White;
			txtHLDepth.BorderStyle = BorderStyle.FixedSingle;
			txtHLDepth.Location = new Point(264, 24);
			txtHLDepth.Margin = new Padding(4, 3, 4, 3);
			txtHLDepth.Name = "txtHLDepth";
			txtHLDepth.Size = new Size(76, 23);
			txtHLDepth.TabIndex = 44;
			txtHLDepth.TextAlign = HorizontalAlignment.Right;
			txtHLDepth.KeyPress += txtDepth_KeyPress;
			// 
			// txtSideAGap
			// 
			txtSideAGap.BackColor = Color.White;
			txtSideAGap.BorderStyle = BorderStyle.FixedSingle;
			txtSideAGap.Location = new Point(264, 83);
			txtSideAGap.Margin = new Padding(4, 3, 4, 3);
			txtSideAGap.Name = "txtSideAGap";
			txtSideAGap.Size = new Size(76, 23);
			txtSideAGap.TabIndex = 46;
			txtSideAGap.Text = "1";
			txtSideAGap.TextAlign = HorizontalAlignment.Right;
			txtSideAGap.KeyPress += txtSideAGap_KeyPress;
			// 
			// txtSideBGap
			// 
			txtSideBGap.BackColor = Color.White;
			txtSideBGap.BorderStyle = BorderStyle.FixedSingle;
			txtSideBGap.Location = new Point(264, 113);
			txtSideBGap.Margin = new Padding(4, 3, 4, 3);
			txtSideBGap.Name = "txtSideBGap";
			txtSideBGap.Size = new Size(76, 23);
			txtSideBGap.TabIndex = 48;
			txtSideBGap.Text = "1";
			txtSideBGap.TextAlign = HorizontalAlignment.Right;
			txtSideBGap.KeyPress += txtSideBGap_KeyPress;
			// 
			// txtLapWidth
			// 
			txtLapWidth.BackColor = Color.White;
			txtLapWidth.BorderStyle = BorderStyle.FixedSingle;
			txtLapWidth.Location = new Point(264, 54);
			txtLapWidth.Margin = new Padding(4, 3, 4, 3);
			txtLapWidth.Name = "txtLapWidth";
			txtLapWidth.Size = new Size(76, 23);
			txtLapWidth.TabIndex = 50;
			txtLapWidth.Text = "50";
			txtLapWidth.TextAlign = HorizontalAlignment.Right;
			txtLapWidth.KeyPress += txtLapWidth_KeyPress;
			// 
			// txtGapBetweenPanels
			// 
			txtGapBetweenPanels.BackColor = Color.White;
			txtGapBetweenPanels.BorderStyle = BorderStyle.FixedSingle;
			txtGapBetweenPanels.Location = new Point(264, 143);
			txtGapBetweenPanels.Margin = new Padding(4, 3, 4, 3);
			txtGapBetweenPanels.Name = "txtGapBetweenPanels";
			txtGapBetweenPanels.Size = new Size(76, 23);
			txtGapBetweenPanels.TabIndex = 52;
			txtGapBetweenPanels.Text = "0.01";
			txtGapBetweenPanels.TextAlign = HorizontalAlignment.Right;
			txtGapBetweenPanels.KeyPress += txtGapBetweenPanels_KeyPress;
			// 
			// optPushToB
			// 
			optPushToB.AutoSize = true;
			optPushToB.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			optPushToB.Location = new Point(181, 54);
			optPushToB.Margin = new Padding(4, 3, 4, 3);
			optPushToB.Name = "optPushToB";
			optPushToB.Size = new Size(132, 17);
			optPushToB.TabIndex = 2;
			optPushToB.TabStop = true;
			optPushToB.Text = "Extend into B from split";
			optPushToB.UseVisualStyleBackColor = true;
			// 
			// optPushToA
			// 
			optPushToA.AutoSize = true;
			optPushToA.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			optPushToA.Location = new Point(13, 54);
			optPushToA.Margin = new Padding(4, 3, 4, 3);
			optPushToA.Name = "optPushToA";
			optPushToA.Size = new Size(132, 17);
			optPushToA.TabIndex = 1;
			optPushToA.TabStop = true;
			optPushToA.Text = "Extend into A from split";
			optPushToA.UseVisualStyleBackColor = true;
			// 
			// optCentred
			// 
			optCentred.AutoSize = true;
			optCentred.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			optCentred.Location = new Point(13, 28);
			optCentred.Margin = new Padding(4, 3, 4, 3);
			optCentred.Name = "optCentred";
			optCentred.Size = new Size(106, 17);
			optCentred.TabIndex = 0;
			optCentred.TabStop = true;
			optCentred.Text = "Centred from split";
			optCentred.UseVisualStyleBackColor = true;
			// 
			// txtVersion
			// 
			txtVersion.AutoSize = true;
			txtVersion.BackColor = Color.FromArgb(172, 187, 179);
			txtVersion.BorderStyle = BorderStyle.FixedSingle;
			txtVersion.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			txtVersion.Location = new Point(99, 743);
			txtVersion.Margin = new Padding(4, 0, 4, 0);
			txtVersion.MaximumSize = new Size(87, 23);
			txtVersion.MinimumSize = new Size(87, 23);
			txtVersion.Name = "txtVersion";
			txtVersion.Size = new Size(87, 23);
			txtVersion.TabIndex = 57;
			txtVersion.Text = "1.0.0";
			txtVersion.TextAlign = ContentAlignment.MiddleRight;
			// 
			// btnHelp
			// 
			btnHelp.BackColor = SystemColors.Control;
			btnHelp.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnHelp.Location = new Point(6, 742);
			btnHelp.Margin = new Padding(4, 3, 4, 3);
			btnHelp.Name = "btnHelp";
			btnHelp.Size = new Size(88, 27);
			btnHelp.TabIndex = 28;
			btnHelp.Text = "Help";
			btnHelp.UseVisualStyleBackColor = true;
			btnHelp.Click += btnHelp_Click;
			// 
			// btnCancel
			// 
			btnCancel.BackColor = SystemColors.Control;
			btnCancel.DialogResult = DialogResult.Cancel;
			btnCancel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnCancel.Location = new Point(678, 742);
			btnCancel.Margin = new Padding(4, 3, 4, 3);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(88, 27);
			btnCancel.TabIndex = 25;
			btnCancel.Text = "&Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			btnCancel.Click += btnCancel_Click;
			// 
			// btnOK
			// 
			btnOK.BackColor = SystemColors.Control;
			btnOK.DialogResult = DialogResult.OK;
			btnOK.Enabled = false;
			btnOK.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnOK.Location = new Point(583, 742);
			btnOK.Margin = new Padding(4, 3, 4, 3);
			btnOK.Name = "btnOK";
			btnOK.Size = new Size(88, 27);
			btnOK.TabIndex = 24;
			btnOK.Text = "&OK";
			btnOK.UseVisualStyleBackColor = true;
			btnOK.Click += btnOK_Click;
			// 
			// pnPartSelectionTitle
			// 
			pnPartSelectionTitle.BackColor = Color.FromArgb(153, 180, 209);
			pnPartSelectionTitle.BorderStyle = BorderStyle.FixedSingle;
			pnPartSelectionTitle.Controls.Add(lblPartSelection);
			pnPartSelectionTitle.Location = new Point(6, 6);
			pnPartSelectionTitle.Margin = new Padding(4, 3, 4, 3);
			pnPartSelectionTitle.Name = "pnPartSelectionTitle";
			pnPartSelectionTitle.Size = new Size(350, 27);
			pnPartSelectionTitle.TabIndex = 100;
			pnPartSelectionTitle.Tag = "";
			// 
			// picPreview
			// 
			picPreview.BackColor = Color.White;
			picPreview.BackgroundImageLayout = ImageLayout.None;
			picPreview.Image = (Image)resources.GetObject("picPreview.Image");
			picPreview.Location = new Point(363, 37);
			picPreview.Margin = new Padding(4, 3, 4, 3);
			picPreview.Name = "picPreview";
			picPreview.Size = new Size(404, 697);
			picPreview.SizeMode = PictureBoxSizeMode.Zoom;
			picPreview.TabIndex = 113;
			picPreview.TabStop = false;
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(cmbPreset);
			groupBox1.Controls.Add(btnPresetSelect);
			groupBox1.Controls.Add(btnPresetDelete);
			groupBox1.Controls.Add(btnPresetSave);
			groupBox1.Location = new Point(6, 40);
			groupBox1.Margin = new Padding(4, 3, 4, 3);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new Padding(4, 3, 4, 3);
			groupBox1.Size = new Size(350, 118);
			groupBox1.TabIndex = 117;
			groupBox1.TabStop = false;
			groupBox1.Text = "Preset";
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(label3);
			groupBox2.Controls.Add(chkFlip);
			groupBox2.Controls.Add(txtMinLengthLimit);
			groupBox2.Controls.Add(chkAddHorzHL);
			groupBox2.Location = new Point(6, 512);
			groupBox2.Margin = new Padding(4, 3, 4, 3);
			groupBox2.Name = "groupBox2";
			groupBox2.Padding = new Padding(4, 3, 4, 3);
			groupBox2.Size = new Size(350, 85);
			groupBox2.TabIndex = 118;
			groupBox2.TabStop = false;
			groupBox2.Text = "Part Modifiers";
			// 
			// groupBox3
			// 
			groupBox3.Controls.Add(optPushToB);
			groupBox3.Controls.Add(optCentred);
			groupBox3.Controls.Add(optPushToA);
			groupBox3.Location = new Point(6, 418);
			groupBox3.Margin = new Padding(4, 3, 4, 3);
			groupBox3.Name = "groupBox3";
			groupBox3.Padding = new Padding(4, 3, 4, 3);
			groupBox3.Size = new Size(350, 84);
			groupBox3.TabIndex = 119;
			groupBox3.TabStop = false;
			groupBox3.Text = "Connection Location";
			// 
			// groupBox4
			// 
			groupBox4.Controls.Add(label5);
			groupBox4.Controls.Add(label4);
			groupBox4.Controls.Add(txtGapBetweenPanels);
			groupBox4.Controls.Add(label2);
			groupBox4.Controls.Add(txtLapWidth);
			groupBox4.Controls.Add(label1);
			groupBox4.Controls.Add(txtSideBGap);
			groupBox4.Controls.Add(lblAroundCentrePartLength);
			groupBox4.Controls.Add(txtSideAGap);
			groupBox4.Controls.Add(txtHLDepth);
			groupBox4.Location = new Point(6, 235);
			groupBox4.Margin = new Padding(4, 3, 4, 3);
			groupBox4.Name = "groupBox4";
			groupBox4.Padding = new Padding(4, 3, 4, 3);
			groupBox4.Size = new Size(350, 174);
			groupBox4.TabIndex = 120;
			groupBox4.TabStop = false;
			groupBox4.Text = "Parameters";
			// 
			// groupBox5
			// 
			groupBox5.Controls.Add(btnSelectElement);
			groupBox5.Controls.Add(btnClearElement);
			groupBox5.Location = new Point(6, 166);
			groupBox5.Margin = new Padding(4, 3, 4, 3);
			groupBox5.Name = "groupBox5";
			groupBox5.Padding = new Padding(4, 3, 4, 3);
			groupBox5.Size = new Size(350, 63);
			groupBox5.TabIndex = 121;
			groupBox5.TabStop = false;
			groupBox5.Text = "Select Parts to be joined";
			// 
			// FrmHalfLap
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = SystemColors.Control;
			ClientSize = new Size(772, 774);
			Controls.Add(groupBox5);
			Controls.Add(groupBox4);
			Controls.Add(groupBox3);
			Controls.Add(groupBox2);
			Controls.Add(txtVersion);
			Controls.Add(groupBox1);
			Controls.Add(btnHelp);
			Controls.Add(btnCancel);
			Controls.Add(btnOK);
			Controls.Add(picPreview);
			Controls.Add(pnPreview);
			Controls.Add(pnPartSelectionTitle);
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			KeyPreview = true;
			Margin = new Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "FrmHalfLap";
			ShowIcon = false;
			Text = "HALF LAP";
			Load += FrmHalfLap_Load;
			pnPreview.ResumeLayout(false);
			pnPreview.PerformLayout();
			pnPartSelectionTitle.ResumeLayout(false);
			pnPartSelectionTitle.PerformLayout();
			((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			groupBox4.ResumeLayout(false);
			groupBox4.PerformLayout();
			groupBox5.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.PictureBox picPreview;
		private System.Windows.Forms.Button btnSelectElement;
		private System.Windows.Forms.Button btnClearElement;
		private System.Windows.Forms.ComboBox cmbPreset;
		private System.Windows.Forms.Button btnPresetDelete;
		private System.Windows.Forms.Button btnPresetSave;
		private System.Windows.Forms.Button btnPresetSelect;
		private System.Windows.Forms.Panel pnPartSelectionTitle;
		private System.Windows.Forms.Label txtVersion;
		private System.Windows.Forms.Button btnHelp;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.CheckBox chkFlip;
		private System.Windows.Forms.CheckBox chkAddHorzHL;
		private System.Windows.Forms.TextBox txtHLDepth;
		private System.Windows.Forms.TextBox txtSideAGap;
		private System.Windows.Forms.TextBox txtSideBGap;
		private System.Windows.Forms.TextBox txtLapWidth;
		private System.Windows.Forms.TextBox txtGapBetweenPanels;
		private System.Windows.Forms.RadioButton optPushToB;
		private System.Windows.Forms.RadioButton optPushToA;
		private System.Windows.Forms.RadioButton optCentred;
        private System.Windows.Forms.TextBox txtMinLengthLimit;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox5;
	}
}