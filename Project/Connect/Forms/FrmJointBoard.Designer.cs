
using System.Drawing;
using System.Windows.Forms;

namespace Architexor.Connect.Forms
{
	partial class FrmJointBoard
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
			Label label6;
			Label label5;
			Label label4;
			Label label3;
			Label label2;
			Label label1;
			Label lblAroundCentrePartLength;
			Label lblPreview;
			Panel pnPreview;
			Label lblPartSelection;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmJointBoard));
			txtVersion = new Label();
			btnHelp = new Button();
			btnCancel = new Button();
			btnOK = new Button();
			txtMinLengthLimit = new TextBox();
			chkFlip = new CheckBox();
			chkAddHorzJB = new CheckBox();
			txtPartRecessDepth = new TextBox();
			txtJointBoardDepth = new TextBox();
			txtSideAGap = new TextBox();
			txtSideBGap = new TextBox();
			txtOverallWidth = new TextBox();
			txtGapBetweenPanels = new TextBox();
			cmbMaterial = new ComboBox();
			btnSelectMaterial = new Button();
			btnSelectElement = new Button();
			btnClearElement = new Button();
			cmbPreset = new ComboBox();
			btnPresetDelete = new Button();
			btnPresetSave = new Button();
			btnPresetSelect = new Button();
			lblConnectionCount = new Label();
			picPreview = new PictureBox();
			pnPartSelectionTitle = new Panel();
			groupBox1 = new GroupBox();
			groupBox2 = new GroupBox();
			groupBox3 = new GroupBox();
			groupBox4 = new GroupBox();
			groupBox5 = new GroupBox();
			label6 = new Label();
			label5 = new Label();
			label4 = new Label();
			label3 = new Label();
			label2 = new Label();
			label1 = new Label();
			lblAroundCentrePartLength = new Label();
			lblPreview = new Label();
			pnPreview = new Panel();
			lblPartSelection = new Label();
			pnPreview.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
			pnPartSelectionTitle.SuspendLayout();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			groupBox3.SuspendLayout();
			groupBox4.SuspendLayout();
			groupBox5.SuspendLayout();
			SuspendLayout();
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.BackColor = Color.Transparent;
			label6.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			label6.Location = new Point(7, 50);
			label6.Margin = new Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new Size(155, 13);
			label6.TabIndex = 59;
			label6.Text = "Exclude Half Lap lengths under";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.BackColor = Color.Transparent;
			label5.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			label5.Location = new Point(5, 23);
			label5.Margin = new Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new Size(107, 13);
			label5.TabIndex = 57;
			label5.Text = "1 - Joint Board Depth";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.BackColor = Color.Transparent;
			label4.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			label4.Location = new Point(5, 53);
			label4.Margin = new Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new Size(106, 13);
			label4.TabIndex = 56;
			label4.Text = "2 - Joint Board Width";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.BackColor = Color.Transparent;
			label3.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			label3.Location = new Point(5, 83);
			label3.Margin = new Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new Size(112, 13);
			label3.TabIndex = 55;
			label3.Text = "3 - Part Recess Depth";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.BackColor = Color.Transparent;
			label2.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			label2.Location = new Point(5, 113);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(127, 13);
			label2.TabIndex = 54;
			label2.Text = "4 - Tolerance Gap Side A";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.BackColor = Color.Transparent;
			label1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			label1.Location = new Point(5, 143);
			label1.Margin = new Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new Size(127, 13);
			label1.TabIndex = 53;
			label1.Text = "5 - Tolerance Gap Side B";
			// 
			// lblAroundCentrePartLength
			// 
			lblAroundCentrePartLength.AutoSize = true;
			lblAroundCentrePartLength.BackColor = Color.Transparent;
			lblAroundCentrePartLength.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblAroundCentrePartLength.Location = new Point(5, 173);
			lblAroundCentrePartLength.Margin = new Padding(4, 0, 4, 0);
			lblAroundCentrePartLength.Name = "lblAroundCentrePartLength";
			lblAroundCentrePartLength.Size = new Size(148, 13);
			lblAroundCentrePartLength.TabIndex = 43;
			lblAroundCentrePartLength.Text = "6 - Tolerance between panels";
			// 
			// lblPreview
			// 
			lblPreview.AutoSize = true;
			lblPreview.BackColor = Color.Transparent;
			lblPreview.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblPreview.Location = new Point(0, 6);
			lblPreview.Margin = new Padding(4, 0, 4, 0);
			lblPreview.Name = "lblPreview";
			lblPreview.Size = new Size(103, 13);
			lblPreview.TabIndex = 43;
			lblPreview.Text = "Preview - Guides";
			// 
			// pnPreview
			// 
			pnPreview.BackColor = Color.FromArgb(153, 180, 209);
			pnPreview.BorderStyle = BorderStyle.FixedSingle;
			pnPreview.Controls.Add(lblPreview);
			pnPreview.Location = new Point(370, 9);
			pnPreview.Margin = new Padding(4, 3, 4, 3);
			pnPreview.Name = "pnPreview";
			pnPreview.Size = new Size(402, 27);
			pnPreview.TabIndex = 86;
			pnPreview.Tag = "";
			// 
			// lblPartSelection
			// 
			lblPartSelection.AutoSize = true;
			lblPartSelection.BackColor = Color.Transparent;
			lblPartSelection.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblPartSelection.Location = new Point(-1, 5);
			lblPartSelection.Margin = new Padding(4, 0, 4, 0);
			lblPartSelection.Name = "lblPartSelection";
			lblPartSelection.Size = new Size(165, 13);
			lblPartSelection.TabIndex = 43;
			lblPartSelection.Text = "Part Selection && Parameters";
			// 
			// txtVersion
			// 
			txtVersion.AutoSize = true;
			txtVersion.BackColor = SystemColors.Control;
			txtVersion.BorderStyle = BorderStyle.FixedSingle;
			txtVersion.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			txtVersion.Location = new Point(102, 742);
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
			btnHelp.Location = new Point(8, 741);
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
			btnCancel.Location = new Point(685, 741);
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
			btnOK.Location = new Point(590, 741);
			btnOK.Margin = new Padding(4, 3, 4, 3);
			btnOK.Name = "btnOK";
			btnOK.Size = new Size(88, 27);
			btnOK.TabIndex = 24;
			btnOK.Text = "&OK";
			btnOK.UseVisualStyleBackColor = true;
			btnOK.Click += btnOK_Click;
			// 
			// txtMinLengthLimit
			// 
			txtMinLengthLimit.BackColor = Color.White;
			txtMinLengthLimit.BorderStyle = BorderStyle.FixedSingle;
			txtMinLengthLimit.Location = new Point(262, 47);
			txtMinLengthLimit.Margin = new Padding(4, 3, 4, 3);
			txtMinLengthLimit.Name = "txtMinLengthLimit";
			txtMinLengthLimit.Size = new Size(76, 23);
			txtMinLengthLimit.TabIndex = 60;
			txtMinLengthLimit.Text = "100";
			txtMinLengthLimit.TextAlign = HorizontalAlignment.Right;
			txtMinLengthLimit.KeyPress += txtMinLimitLength_KeyPress;
			// 
			// chkFlip
			// 
			chkFlip.AutoSize = true;
			chkFlip.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			chkFlip.Location = new Point(9, 22);
			chkFlip.Margin = new Padding(4, 3, 4, 3);
			chkFlip.Name = "chkFlip";
			chkFlip.Size = new Size(64, 17);
			chkFlip.TabIndex = 55;
			chkFlip.Text = "Flip side";
			chkFlip.UseVisualStyleBackColor = true;
			// 
			// chkAddHorzJB
			// 
			chkAddHorzJB.AutoSize = true;
			chkAddHorzJB.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			chkAddHorzJB.Location = new Point(9, 50);
			chkAddHorzJB.Margin = new Padding(4, 3, 4, 3);
			chkAddHorzJB.Name = "chkAddHorzJB";
			chkAddHorzJB.Size = new Size(213, 17);
			chkAddHorzJB.TabIndex = 56;
			chkAddHorzJB.Text = "Add horizontal Joint board to Lintel && Sill";
			chkAddHorzJB.UseVisualStyleBackColor = true;
			chkAddHorzJB.Visible = false;
			// 
			// txtPartRecessDepth
			// 
			txtPartRecessDepth.BackColor = Color.White;
			txtPartRecessDepth.BorderStyle = BorderStyle.FixedSingle;
			txtPartRecessDepth.Location = new Point(262, 81);
			txtPartRecessDepth.Margin = new Padding(4, 3, 4, 3);
			txtPartRecessDepth.Name = "txtPartRecessDepth";
			txtPartRecessDepth.Size = new Size(76, 23);
			txtPartRecessDepth.TabIndex = 39;
			txtPartRecessDepth.Text = "20";
			txtPartRecessDepth.TextAlign = HorizontalAlignment.Right;
			// 
			// txtJointBoardDepth
			// 
			txtJointBoardDepth.BackColor = Color.White;
			txtJointBoardDepth.BorderStyle = BorderStyle.FixedSingle;
			txtJointBoardDepth.Location = new Point(262, 21);
			txtJointBoardDepth.Margin = new Padding(4, 3, 4, 3);
			txtJointBoardDepth.Name = "txtJointBoardDepth";
			txtJointBoardDepth.Size = new Size(76, 23);
			txtJointBoardDepth.TabIndex = 44;
			txtJointBoardDepth.Text = "20";
			txtJointBoardDepth.TextAlign = HorizontalAlignment.Right;
			txtJointBoardDepth.KeyPress += txtJointBoardDepth_KeyPress;
			// 
			// txtSideAGap
			// 
			txtSideAGap.BackColor = Color.White;
			txtSideAGap.BorderStyle = BorderStyle.FixedSingle;
			txtSideAGap.Location = new Point(262, 111);
			txtSideAGap.Margin = new Padding(4, 3, 4, 3);
			txtSideAGap.Name = "txtSideAGap";
			txtSideAGap.Size = new Size(76, 23);
			txtSideAGap.TabIndex = 46;
			txtSideAGap.Text = "2";
			txtSideAGap.TextAlign = HorizontalAlignment.Right;
			txtSideAGap.KeyPress += txtSideAGap_KeyPress;
			// 
			// txtSideBGap
			// 
			txtSideBGap.BackColor = Color.White;
			txtSideBGap.BorderStyle = BorderStyle.FixedSingle;
			txtSideBGap.Location = new Point(262, 141);
			txtSideBGap.Margin = new Padding(4, 3, 4, 3);
			txtSideBGap.Name = "txtSideBGap";
			txtSideBGap.Size = new Size(76, 23);
			txtSideBGap.TabIndex = 48;
			txtSideBGap.Text = "2";
			txtSideBGap.TextAlign = HorizontalAlignment.Right;
			txtSideBGap.KeyPress += txtSideBGap_KeyPress;
			// 
			// txtOverallWidth
			// 
			txtOverallWidth.BackColor = Color.White;
			txtOverallWidth.BorderStyle = BorderStyle.FixedSingle;
			txtOverallWidth.Location = new Point(262, 51);
			txtOverallWidth.Margin = new Padding(4, 3, 4, 3);
			txtOverallWidth.Name = "txtOverallWidth";
			txtOverallWidth.Size = new Size(76, 23);
			txtOverallWidth.TabIndex = 50;
			txtOverallWidth.Text = "200";
			txtOverallWidth.TextAlign = HorizontalAlignment.Right;
			txtOverallWidth.KeyPress += txtOverallWidth_KeyPress;
			// 
			// txtGapBetweenPanels
			// 
			txtGapBetweenPanels.BackColor = Color.White;
			txtGapBetweenPanels.BorderStyle = BorderStyle.FixedSingle;
			txtGapBetweenPanels.Location = new Point(262, 171);
			txtGapBetweenPanels.Margin = new Padding(4, 3, 4, 3);
			txtGapBetweenPanels.Name = "txtGapBetweenPanels";
			txtGapBetweenPanels.Size = new Size(76, 23);
			txtGapBetweenPanels.TabIndex = 52;
			txtGapBetweenPanels.Text = "1";
			txtGapBetweenPanels.TextAlign = HorizontalAlignment.Right;
			txtGapBetweenPanels.KeyPress += txtGapBetweenPanels_KeyPress;
			// 
			// cmbMaterial
			// 
			cmbMaterial.BackColor = Color.White;
			cmbMaterial.FormattingEnabled = true;
			cmbMaterial.Location = new Point(103, 24);
			cmbMaterial.Margin = new Padding(4, 3, 4, 3);
			cmbMaterial.Name = "cmbMaterial";
			cmbMaterial.Size = new Size(234, 23);
			cmbMaterial.TabIndex = 48;
			// 
			// btnSelectMaterial
			// 
			btnSelectMaterial.BackColor = SystemColors.Control;
			btnSelectMaterial.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnSelectMaterial.Location = new Point(8, 23);
			btnSelectMaterial.Margin = new Padding(4, 3, 4, 3);
			btnSelectMaterial.Name = "btnSelectMaterial";
			btnSelectMaterial.Size = new Size(88, 27);
			btnSelectMaterial.TabIndex = 31;
			btnSelectMaterial.Text = "Select";
			btnSelectMaterial.UseVisualStyleBackColor = true;
			btnSelectMaterial.Click += btnSelectMaterial_Click;
			// 
			// btnSelectElement
			// 
			btnSelectElement.BackColor = SystemColors.Control;
			btnSelectElement.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnSelectElement.Location = new Point(8, 24);
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
			btnClearElement.Location = new Point(103, 24);
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
			cmbPreset.Location = new Point(103, 22);
			cmbPreset.Margin = new Padding(4, 3, 4, 3);
			cmbPreset.Name = "cmbPreset";
			cmbPreset.Size = new Size(234, 23);
			cmbPreset.TabIndex = 47;
			// 
			// btnPresetDelete
			// 
			btnPresetDelete.BackColor = SystemColors.Control;
			btnPresetDelete.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnPresetDelete.Location = new Point(8, 82);
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
			btnPresetSave.Location = new Point(8, 52);
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
			btnPresetSelect.Location = new Point(8, 22);
			btnPresetSelect.Margin = new Padding(4, 3, 4, 3);
			btnPresetSelect.Name = "btnPresetSelect";
			btnPresetSelect.Size = new Size(88, 27);
			btnPresetSelect.TabIndex = 41;
			btnPresetSelect.Text = "Select";
			btnPresetSelect.UseVisualStyleBackColor = true;
			btnPresetSelect.Click += btnPresetSelect_Click;
			// 
			// lblConnectionCount
			// 
			lblConnectionCount.AutoSize = true;
			lblConnectionCount.Location = new Point(813, 899);
			lblConnectionCount.Margin = new Padding(4, 0, 4, 0);
			lblConnectionCount.Name = "lblConnectionCount";
			lblConnectionCount.Size = new Size(0, 15);
			lblConnectionCount.TabIndex = 54;
			// 
			// picPreview
			// 
			picPreview.BackColor = Color.White;
			picPreview.BackgroundImageLayout = ImageLayout.None;
			picPreview.Image = (Image)resources.GetObject("picPreview.Image");
			picPreview.Location = new Point(370, 37);
			picPreview.Margin = new Padding(4, 3, 4, 3);
			picPreview.Name = "picPreview";
			picPreview.Size = new Size(402, 697);
			picPreview.SizeMode = PictureBoxSizeMode.Zoom;
			picPreview.TabIndex = 97;
			picPreview.TabStop = false;
			// 
			// pnPartSelectionTitle
			// 
			pnPartSelectionTitle.BackColor = Color.FromArgb(153, 180, 209);
			pnPartSelectionTitle.BorderStyle = BorderStyle.FixedSingle;
			pnPartSelectionTitle.Controls.Add(lblPartSelection);
			pnPartSelectionTitle.Location = new Point(9, 9);
			pnPartSelectionTitle.Margin = new Padding(4, 3, 4, 3);
			pnPartSelectionTitle.Name = "pnPartSelectionTitle";
			pnPartSelectionTitle.Size = new Size(350, 27);
			pnPartSelectionTitle.TabIndex = 84;
			pnPartSelectionTitle.Tag = "";
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(label6);
			groupBox1.Controls.Add(chkAddHorzJB);
			groupBox1.Controls.Add(txtMinLengthLimit);
			groupBox1.Controls.Add(chkFlip);
			groupBox1.Location = new Point(9, 520);
			groupBox1.Margin = new Padding(4, 3, 4, 3);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new Padding(4, 3, 4, 3);
			groupBox1.Size = new Size(349, 82);
			groupBox1.TabIndex = 98;
			groupBox1.TabStop = false;
			groupBox1.Text = "Part Modifiers";
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(label5);
			groupBox2.Controls.Add(txtSideAGap);
			groupBox2.Controls.Add(label4);
			groupBox2.Controls.Add(txtGapBetweenPanels);
			groupBox2.Controls.Add(label3);
			groupBox2.Controls.Add(txtOverallWidth);
			groupBox2.Controls.Add(label2);
			groupBox2.Controls.Add(txtSideBGap);
			groupBox2.Controls.Add(label1);
			groupBox2.Controls.Add(txtJointBoardDepth);
			groupBox2.Controls.Add(lblAroundCentrePartLength);
			groupBox2.Controls.Add(txtPartRecessDepth);
			groupBox2.Location = new Point(9, 307);
			groupBox2.Margin = new Padding(4, 3, 4, 3);
			groupBox2.Name = "groupBox2";
			groupBox2.Padding = new Padding(4, 3, 4, 3);
			groupBox2.Size = new Size(349, 204);
			groupBox2.TabIndex = 99;
			groupBox2.TabStop = false;
			groupBox2.Text = "Parameters";
			// 
			// groupBox3
			// 
			groupBox3.Controls.Add(cmbMaterial);
			groupBox3.Controls.Add(btnSelectMaterial);
			groupBox3.Location = new Point(9, 239);
			groupBox3.Margin = new Padding(4, 3, 4, 3);
			groupBox3.Name = "groupBox3";
			groupBox3.Padding = new Padding(4, 3, 4, 3);
			groupBox3.Size = new Size(349, 61);
			groupBox3.TabIndex = 100;
			groupBox3.TabStop = false;
			groupBox3.Text = "Select Material Type for Joint Board";
			// 
			// groupBox4
			// 
			groupBox4.Controls.Add(btnSelectElement);
			groupBox4.Controls.Add(btnClearElement);
			groupBox4.Location = new Point(9, 171);
			groupBox4.Margin = new Padding(4, 3, 4, 3);
			groupBox4.Name = "groupBox4";
			groupBox4.Padding = new Padding(4, 3, 4, 3);
			groupBox4.Size = new Size(349, 61);
			groupBox4.TabIndex = 101;
			groupBox4.TabStop = false;
			groupBox4.Text = "Select Parts to be Joined";
			// 
			// groupBox5
			// 
			groupBox5.Controls.Add(cmbPreset);
			groupBox5.Controls.Add(btnPresetSelect);
			groupBox5.Controls.Add(btnPresetDelete);
			groupBox5.Controls.Add(btnPresetSave);
			groupBox5.Location = new Point(9, 45);
			groupBox5.Margin = new Padding(4, 3, 4, 3);
			groupBox5.Name = "groupBox5";
			groupBox5.Padding = new Padding(4, 3, 4, 3);
			groupBox5.Size = new Size(349, 119);
			groupBox5.TabIndex = 102;
			groupBox5.TabStop = false;
			groupBox5.Text = "Preset";
			// 
			// FrmJointBoard
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = SystemColors.Control;
			ClientSize = new Size(782, 777);
			Controls.Add(groupBox5);
			Controls.Add(groupBox4);
			Controls.Add(groupBox3);
			Controls.Add(groupBox2);
			Controls.Add(groupBox1);
			Controls.Add(txtVersion);
			Controls.Add(btnHelp);
			Controls.Add(btnCancel);
			Controls.Add(picPreview);
			Controls.Add(btnOK);
			Controls.Add(pnPreview);
			Controls.Add(pnPartSelectionTitle);
			Controls.Add(lblConnectionCount);
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			KeyPreview = true;
			Margin = new Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "FrmJointBoard";
			ShowIcon = false;
			Text = "JOINT BOARD";
			Load += FrmJointBoard_Load;
			pnPreview.ResumeLayout(false);
			pnPreview.PerformLayout();
			((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
			pnPartSelectionTitle.ResumeLayout(false);
			pnPartSelectionTitle.PerformLayout();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			groupBox3.ResumeLayout(false);
			groupBox4.ResumeLayout(false);
			groupBox5.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private System.Windows.Forms.TextBox txtJointBoardDepth;
		private System.Windows.Forms.TextBox txtSideAGap;
		private System.Windows.Forms.TextBox txtSideBGap;
		private System.Windows.Forms.TextBox txtOverallWidth;
		private System.Windows.Forms.TextBox txtGapBetweenPanels;
		private System.Windows.Forms.Label lblConnectionCount;
		private System.Windows.Forms.CheckBox chkFlip;
		private System.Windows.Forms.CheckBox chkAddHorzJB;
		private System.Windows.Forms.Label txtVersion;
		private System.Windows.Forms.Button btnHelp;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.PictureBox picPreview;
		private System.Windows.Forms.TextBox txtPartRecessDepth;
		private System.Windows.Forms.Button btnSelectMaterial;
		private System.Windows.Forms.Button btnSelectElement;
		private System.Windows.Forms.Button btnClearElement;
		private System.Windows.Forms.Button btnPresetDelete;
		private System.Windows.Forms.Button btnPresetSave;
		private System.Windows.Forms.Button btnPresetSelect;
		private System.Windows.Forms.ComboBox cmbPreset;
		private System.Windows.Forms.ComboBox cmbMaterial;
        private System.Windows.Forms.TextBox txtMinLengthLimit;
		private System.Windows.Forms.Panel pnPartSelectionTitle;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox5;
	}
}