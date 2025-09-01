#define CONNECTION_PARAMS
using Architexor.Widgets.ConnectorTool;
using System.Drawing;
using System.Windows.Forms;

namespace ConnectorTool.Forms
{
    partial class FrmGlulamTConnector
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
			GroupBox groupBox1;
			btnSetFixings = new Button();
			btnConPosition = new Button();
			btnSetFinFilletCover = new Button();
			lblPreview = new Label();
			pnPreview = new Panel();
			lblDrawPreview = new Label();
			cmbPreset = new ComboBox();
			btnPresetDelete = new Button();
			btnPresetSave = new Button();
			btnPresetSelect = new Button();
			btnSelectElement = new Button();
			btnClearElement = new Button();
			pnPreviewContent = new Panel();
			picPreviewPlan = new PictureBox();
			pnPreviewTitle = new Panel();
			btnPreview = new Button();
			panel2 = new Panel();
			button1 = new Button();
			label1 = new Label();
			pnPreviousNext = new Panel();
			chkApplytoAll = new CheckBox();
			btnNext = new Button();
			btnPrevious = new Button();
			lblPreviousNext = new Label();
			picPreviewFront = new PictureBox();
			txtVersion = new Label();
			btnHelp = new Button();
			btnCancel = new Button();
			btnOK = new Button();
			cmbModel = new ComboBox();
			btnSelectModel = new Button();
			cmbType = new ComboBox();
			btnSelectType = new Button();
			lblCustom = new Label();
			panel1 = new Panel();
			groFixing = new UsrTConnectorFixings();
			groFinFilletCover = new UsrTConnectorFinFilletCover();
			groPosition = new UsrTConnectorPosition();
			groupBox2 = new GroupBox();
			groupBox3 = new GroupBox();
			groupBox4 = new GroupBox();
			groupBox1 = new GroupBox();
			groupBox1.SuspendLayout();
			pnPreview.SuspendLayout();
			pnPreviewContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)picPreviewPlan).BeginInit();
			pnPreviewTitle.SuspendLayout();
			panel2.SuspendLayout();
			pnPreviousNext.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)picPreviewFront).BeginInit();
			panel1.SuspendLayout();
			groupBox2.SuspendLayout();
			groupBox3.SuspendLayout();
			groupBox4.SuspendLayout();
			SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(btnSetFixings);
			groupBox1.Controls.Add(btnConPosition);
			groupBox1.Controls.Add(btnSetFinFilletCover);
			groupBox1.Location = new Point(842, 8);
			groupBox1.Margin = new Padding(4, 3, 4, 3);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new Padding(4, 3, 4, 3);
			groupBox1.Size = new Size(226, 119);
			groupBox1.TabIndex = 148;
			groupBox1.TabStop = false;
			groupBox1.Text = "Customisation";
			// 
			// btnSetFixings
			// 
			btnSetFixings.BackColor = SystemColors.Control;
			btnSetFixings.DialogResult = DialogResult.OK;
			btnSetFixings.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnSetFixings.Location = new Point(8, 82);
			btnSetFixings.Margin = new Padding(4, 3, 4, 3);
			btnSetFixings.Name = "btnSetFixings";
			btnSetFixings.Size = new Size(210, 27);
			btnSetFixings.TabIndex = 60;
			btnSetFixings.Tag = "2";
			btnSetFixings.Text = "Fixings";
			btnSetFixings.UseVisualStyleBackColor = true;
			btnSetFixings.Click += BtnConnectionParametersChanged;
			// 
			// btnConPosition
			// 
			btnConPosition.BackColor = SystemColors.Control;
			btnConPosition.DialogResult = DialogResult.OK;
			btnConPosition.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnConPosition.Location = new Point(8, 22);
			btnConPosition.Margin = new Padding(4, 3, 4, 3);
			btnConPosition.Name = "btnConPosition";
			btnConPosition.Size = new Size(210, 27);
			btnConPosition.TabIndex = 58;
			btnConPosition.Tag = "0";
			btnConPosition.Text = "Connector Position";
			btnConPosition.UseVisualStyleBackColor = true;
			btnConPosition.Click += BtnConnectionParametersChanged;
			// 
			// btnSetFinFilletCover
			// 
			btnSetFinFilletCover.BackColor = SystemColors.Control;
			btnSetFinFilletCover.DialogResult = DialogResult.OK;
			btnSetFinFilletCover.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnSetFinFilletCover.Location = new Point(8, 52);
			btnSetFinFilletCover.Margin = new Padding(4, 3, 4, 3);
			btnSetFinFilletCover.Name = "btnSetFinFilletCover";
			btnSetFinFilletCover.Size = new Size(210, 27);
			btnSetFinFilletCover.TabIndex = 59;
			btnSetFinFilletCover.Tag = "1";
			btnSetFinFilletCover.Text = "Fin/Fillet/Cover Board";
			btnSetFinFilletCover.UseVisualStyleBackColor = true;
			btnSetFinFilletCover.Click += BtnConnectionParametersChanged;
			// 
			// lblPreview
			// 
			lblPreview.AutoSize = true;
			lblPreview.BackColor = Color.Transparent;
			lblPreview.Font = new Font("Calibri", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblPreview.Location = new Point(0, -2);
			lblPreview.Margin = new Padding(4, 0, 4, 0);
			lblPreview.Name = "lblPreview";
			lblPreview.Size = new Size(71, 19);
			lblPreview.TabIndex = 43;
			lblPreview.Text = "PREVIEW";
			// 
			// pnPreview
			// 
			pnPreview.BackColor = Color.FromArgb(153, 180, 209);
			pnPreview.BorderStyle = BorderStyle.FixedSingle;
			pnPreview.Controls.Add(lblDrawPreview);
			pnPreview.Location = new Point(9, 140);
			pnPreview.Margin = new Padding(4, 3, 4, 3);
			pnPreview.Name = "pnPreview";
			pnPreview.Size = new Size(484, 27);
			pnPreview.TabIndex = 120;
			pnPreview.Tag = "";
			// 
			// lblDrawPreview
			// 
			lblDrawPreview.AutoSize = true;
			lblDrawPreview.BackColor = Color.Transparent;
			lblDrawPreview.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblDrawPreview.Location = new Point(4, 5);
			lblDrawPreview.Margin = new Padding(4, 0, 4, 0);
			lblDrawPreview.Name = "lblDrawPreview";
			lblDrawPreview.Size = new Size(52, 13);
			lblDrawPreview.TabIndex = 43;
			lblDrawPreview.Text = "Preview";
			// 
			// cmbPreset
			// 
			cmbPreset.BackColor = Color.White;
			cmbPreset.FormattingEnabled = true;
			cmbPreset.Location = new Point(119, 23);
			cmbPreset.Margin = new Padding(4, 3, 4, 3);
			cmbPreset.Name = "cmbPreset";
			cmbPreset.Size = new Size(244, 23);
			cmbPreset.TabIndex = 47;
			// 
			// btnPresetDelete
			// 
			btnPresetDelete.BackColor = SystemColors.Control;
			btnPresetDelete.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnPresetDelete.Location = new Point(8, 82);
			btnPresetDelete.Margin = new Padding(4, 3, 4, 3);
			btnPresetDelete.Name = "btnPresetDelete";
			btnPresetDelete.Size = new Size(105, 27);
			btnPresetDelete.TabIndex = 43;
			btnPresetDelete.Text = "Delete Saved";
			btnPresetDelete.UseVisualStyleBackColor = true;
			btnPresetDelete.Click += BtnPresetDelete_Click;
			// 
			// btnPresetSave
			// 
			btnPresetSave.BackColor = SystemColors.Control;
			btnPresetSave.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnPresetSave.Location = new Point(8, 52);
			btnPresetSave.Margin = new Padding(4, 3, 4, 3);
			btnPresetSave.Name = "btnPresetSave";
			btnPresetSave.Size = new Size(105, 27);
			btnPresetSave.TabIndex = 42;
			btnPresetSave.Text = "Select From";
			btnPresetSave.UseVisualStyleBackColor = true;
			btnPresetSave.Click += BtnPresetSave_Click;
			// 
			// btnPresetSelect
			// 
			btnPresetSelect.BackColor = SystemColors.Control;
			btnPresetSelect.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnPresetSelect.Location = new Point(8, 22);
			btnPresetSelect.Margin = new Padding(4, 3, 4, 3);
			btnPresetSelect.Name = "btnPresetSelect";
			btnPresetSelect.Size = new Size(105, 27);
			btnPresetSelect.TabIndex = 41;
			btnPresetSelect.Text = "Save As";
			btnPresetSelect.UseVisualStyleBackColor = true;
			btnPresetSelect.Click += BtnPresetSelect_Click;
			// 
			// btnSelectElement
			// 
			btnSelectElement.BackColor = SystemColors.Control;
			btnSelectElement.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnSelectElement.Location = new Point(7, 22);
			btnSelectElement.Margin = new Padding(4, 3, 4, 3);
			btnSelectElement.Name = "btnSelectElement";
			btnSelectElement.Size = new Size(86, 27);
			btnSelectElement.TabIndex = 40;
			btnSelectElement.Text = "Select";
			btnSelectElement.UseVisualStyleBackColor = true;
			btnSelectElement.Click += BtnSelect_Click;
			// 
			// btnClearElement
			// 
			btnClearElement.BackColor = SystemColors.Control;
			btnClearElement.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnClearElement.Location = new Point(7, 52);
			btnClearElement.Margin = new Padding(4, 3, 4, 3);
			btnClearElement.Name = "btnClearElement";
			btnClearElement.Size = new Size(86, 27);
			btnClearElement.TabIndex = 41;
			btnClearElement.Text = "Remove";
			btnClearElement.UseVisualStyleBackColor = true;
			btnClearElement.Click += BtnRemove_Click;
			// 
			// pnPreviewContent
			// 
			pnPreviewContent.BackColor = Color.White;
			pnPreviewContent.BorderStyle = BorderStyle.FixedSingle;
			pnPreviewContent.Controls.Add(picPreviewPlan);
			pnPreviewContent.Controls.Add(pnPreviewTitle);
			pnPreviewContent.Controls.Add(pnPreviousNext);
			pnPreviewContent.Controls.Add(picPreviewFront);
			pnPreviewContent.Location = new Point(9, 166);
			pnPreviewContent.Margin = new Padding(4, 3, 4, 3);
			pnPreviewContent.Name = "pnPreviewContent";
			pnPreviewContent.Size = new Size(484, 571);
			pnPreviewContent.TabIndex = 127;
			pnPreviewContent.Tag = "";
			// 
			// picPreviewPlan
			// 
			picPreviewPlan.Location = new Point(5, 272);
			picPreviewPlan.Margin = new Padding(4, 3, 4, 3);
			picPreviewPlan.Name = "picPreviewPlan";
			picPreviewPlan.Size = new Size(472, 260);
			picPreviewPlan.TabIndex = 144;
			picPreviewPlan.TabStop = false;
			picPreviewPlan.Paint += picPreviewPlan_Paint;
			// 
			// pnPreviewTitle
			// 
			pnPreviewTitle.BackColor = Color.FromArgb(195, 190, 166);
			pnPreviewTitle.BorderStyle = BorderStyle.FixedSingle;
			pnPreviewTitle.Controls.Add(btnPreview);
			pnPreviewTitle.Controls.Add(panel2);
			pnPreviewTitle.Controls.Add(lblPreview);
			pnPreviewTitle.Location = new Point(7, 540);
			pnPreviewTitle.Margin = new Padding(4, 3, 4, 3);
			pnPreviewTitle.Name = "pnPreviewTitle";
			pnPreviewTitle.Size = new Size(114, 26);
			pnPreviewTitle.TabIndex = 126;
			pnPreviewTitle.Tag = "";
			// 
			// btnPreview
			// 
			btnPreview.BackColor = SystemColors.Control;
			btnPreview.DialogResult = DialogResult.OK;
			btnPreview.Enabled = false;
			btnPreview.FlatStyle = FlatStyle.Flat;
			btnPreview.Font = new Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnPreview.Location = new Point(78, -1);
			btnPreview.Margin = new Padding(4, 3, 4, 3);
			btnPreview.Name = "btnPreview";
			btnPreview.Size = new Size(35, 27);
			btnPreview.TabIndex = 57;
			btnPreview.Text = "▶";
			btnPreview.UseVisualStyleBackColor = false;
			btnPreview.Click += BtnPreview_Click;
			// 
			// panel2
			// 
			panel2.BackColor = SystemColors.Control;
			panel2.BorderStyle = BorderStyle.FixedSingle;
			panel2.Controls.Add(button1);
			panel2.Controls.Add(label1);
			panel2.Location = new Point(-1, -1);
			panel2.Margin = new Padding(4, 3, 4, 3);
			panel2.Name = "panel2";
			panel2.Size = new Size(112, 26);
			panel2.TabIndex = 126;
			panel2.Tag = "";
			// 
			// button1
			// 
			button1.BackColor = Color.FromArgb(222, 167, 117);
			button1.DialogResult = DialogResult.OK;
			button1.Enabled = false;
			button1.FlatStyle = FlatStyle.Flat;
			button1.Font = new Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			button1.Location = new Point(78, -1);
			button1.Margin = new Padding(4, 3, 4, 3);
			button1.Name = "button1";
			button1.Size = new Size(35, 23);
			button1.TabIndex = 57;
			button1.Text = "▶";
			button1.UseVisualStyleBackColor = false;
			button1.Click += BtnPreview_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.BackColor = Color.Transparent;
			label1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			label1.Location = new Point(0, 5);
			label1.Margin = new Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new Size(57, 13);
			label1.TabIndex = 43;
			label1.Text = "PREVIEW";
			// 
			// pnPreviousNext
			// 
			pnPreviousNext.BackColor = SystemColors.Control;
			pnPreviousNext.BorderStyle = BorderStyle.FixedSingle;
			pnPreviousNext.Controls.Add(chkApplytoAll);
			pnPreviousNext.Controls.Add(btnNext);
			pnPreviousNext.Controls.Add(btnPrevious);
			pnPreviousNext.Controls.Add(lblPreviousNext);
			pnPreviousNext.Location = new Point(118, 540);
			pnPreviousNext.Margin = new Padding(4, 3, 4, 3);
			pnPreviousNext.Name = "pnPreviousNext";
			pnPreviousNext.Size = new Size(358, 26);
			pnPreviousNext.TabIndex = 131;
			pnPreviousNext.Tag = "";
			// 
			// chkApplytoAll
			// 
			chkApplytoAll.AutoSize = true;
			chkApplytoAll.Enabled = false;
			chkApplytoAll.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			chkApplytoAll.Location = new Point(232, 5);
			chkApplytoAll.Margin = new Padding(4, 3, 4, 3);
			chkApplytoAll.Name = "chkApplytoAll";
			chkApplytoAll.RightToLeft = RightToLeft.Yes;
			chkApplytoAll.Size = new Size(100, 17);
			chkApplytoAll.TabIndex = 140;
			chkApplytoAll.Text = "APPLY TO ALL";
			chkApplytoAll.UseVisualStyleBackColor = true;
			chkApplytoAll.CheckedChanged += ChkApplytoAll_CheckedChanged;
			// 
			// btnNext
			// 
			btnNext.BackColor = SystemColors.Control;
			btnNext.BackgroundImageLayout = ImageLayout.None;
			btnNext.Enabled = false;
			btnNext.FlatStyle = FlatStyle.Flat;
			btnNext.Font = new Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnNext.ForeColor = SystemColors.ControlText;
			btnNext.Location = new Point(175, -1);
			btnNext.Margin = new Padding(4, 3, 4, 3);
			btnNext.Name = "btnNext";
			btnNext.Size = new Size(37, 27);
			btnNext.TabIndex = 44;
			btnNext.Tag = "Next";
			btnNext.Text = ">";
			btnNext.UseVisualStyleBackColor = false;
			btnNext.Click += BtnNextandPrevious;
			// 
			// btnPrevious
			// 
			btnPrevious.BackColor = SystemColors.Control;
			btnPrevious.BackgroundImageLayout = ImageLayout.None;
			btnPrevious.Enabled = false;
			btnPrevious.FlatStyle = FlatStyle.Flat;
			btnPrevious.Font = new Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnPrevious.Location = new Point(139, -1);
			btnPrevious.Margin = new Padding(4, 3, 4, 3);
			btnPrevious.Name = "btnPrevious";
			btnPrevious.Size = new Size(37, 27);
			btnPrevious.TabIndex = 44;
			btnPrevious.Tag = "Previous";
			btnPrevious.Text = "<";
			btnPrevious.UseVisualStyleBackColor = false;
			btnPrevious.Click += BtnNextandPrevious;
			// 
			// lblPreviousNext
			// 
			lblPreviousNext.AutoSize = true;
			lblPreviousNext.BackColor = Color.Transparent;
			lblPreviousNext.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblPreviousNext.Location = new Point(13, 5);
			lblPreviousNext.Margin = new Padding(4, 0, 4, 0);
			lblPreviousNext.Name = "lblPreviousNext";
			lblPreviousNext.Size = new Size(96, 13);
			lblPreviousNext.TabIndex = 43;
			lblPreviousNext.Text = "PREVIOUS/NEXT";
			// 
			// picPreviewFront
			// 
			picPreviewFront.Location = new Point(12, 14);
			picPreviewFront.Margin = new Padding(4, 3, 4, 3);
			picPreviewFront.Name = "picPreviewFront";
			picPreviewFront.Size = new Size(449, 248);
			picPreviewFront.TabIndex = 142;
			picPreviewFront.TabStop = false;
			picPreviewFront.Paint += picPreviewFront_Paint;
			// 
			// txtVersion
			// 
			txtVersion.AutoSize = true;
			txtVersion.BackColor = Color.FromArgb(172, 187, 179);
			txtVersion.BorderStyle = BorderStyle.FixedSingle;
			txtVersion.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			txtVersion.Location = new Point(104, 745);
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
			btnHelp.Location = new Point(9, 744);
			btnHelp.Margin = new Padding(4, 3, 4, 3);
			btnHelp.Name = "btnHelp";
			btnHelp.Size = new Size(88, 27);
			btnHelp.TabIndex = 28;
			btnHelp.Text = "Help";
			btnHelp.UseVisualStyleBackColor = true;
			btnHelp.Click += BtnHelp_Click;
			// 
			// btnCancel
			// 
			btnCancel.BackColor = SystemColors.Control;
			btnCancel.DialogResult = DialogResult.Cancel;
			btnCancel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnCancel.Location = new Point(1527, 745);
			btnCancel.Margin = new Padding(4, 3, 4, 3);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(88, 27);
			btnCancel.TabIndex = 25;
			btnCancel.Text = "&Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			btnCancel.Click += BtnClose_Click;
			// 
			// btnOK
			// 
			btnOK.BackColor = SystemColors.Control;
			btnOK.DialogResult = DialogResult.OK;
			btnOK.Enabled = false;
			btnOK.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnOK.Location = new Point(1429, 745);
			btnOK.Margin = new Padding(4, 3, 4, 3);
			btnOK.Name = "btnOK";
			btnOK.Size = new Size(88, 27);
			btnOK.TabIndex = 24;
			btnOK.Text = "&OK";
			btnOK.UseVisualStyleBackColor = true;
			btnOK.Click += BtnOK_Click;
			// 
			// cmbModel
			// 
			cmbModel.BackColor = Color.White;
			cmbModel.FormattingEnabled = true;
			cmbModel.Location = new Point(84, 53);
			cmbModel.Margin = new Padding(4, 3, 4, 3);
			cmbModel.Name = "cmbModel";
			cmbModel.Size = new Size(235, 23);
			cmbModel.TabIndex = 52;
			cmbModel.SelectedIndexChanged += CmbModel_SelectedIndexChanged;
			// 
			// btnSelectModel
			// 
			btnSelectModel.BackColor = SystemColors.Control;
			btnSelectModel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnSelectModel.Location = new Point(8, 52);
			btnSelectModel.Margin = new Padding(4, 3, 4, 3);
			btnSelectModel.Name = "btnSelectModel";
			btnSelectModel.Size = new Size(70, 27);
			btnSelectModel.TabIndex = 51;
			btnSelectModel.Text = "Model";
			btnSelectModel.UseVisualStyleBackColor = true;
			// 
			// cmbType
			// 
			cmbType.BackColor = Color.White;
			cmbType.FormattingEnabled = true;
			cmbType.Location = new Point(84, 23);
			cmbType.Margin = new Padding(4, 3, 4, 3);
			cmbType.Name = "cmbType";
			cmbType.Size = new Size(235, 23);
			cmbType.TabIndex = 50;
			cmbType.SelectedIndexChanged += CmbType_SelectedIndexChanged;
			// 
			// btnSelectType
			// 
			btnSelectType.BackColor = SystemColors.Control;
			btnSelectType.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnSelectType.Location = new Point(8, 22);
			btnSelectType.Margin = new Padding(4, 3, 4, 3);
			btnSelectType.Name = "btnSelectType";
			btnSelectType.Size = new Size(70, 27);
			btnSelectType.TabIndex = 49;
			btnSelectType.Text = "Type";
			btnSelectType.UseVisualStyleBackColor = true;
			// 
			// lblCustom
			// 
			lblCustom.AutoSize = true;
			lblCustom.BackColor = Color.Transparent;
			lblCustom.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblCustom.Location = new Point(4, 5);
			lblCustom.Margin = new Padding(4, 0, 4, 0);
			lblCustom.Name = "lblCustom";
			lblCustom.Size = new Size(152, 13);
			lblCustom.TabIndex = 43;
			lblCustom.Text = "Customisation Parameters";
			// 
			// panel1
			// 
			panel1.BackColor = Color.FromArgb(153, 180, 209);
			panel1.BorderStyle = BorderStyle.FixedSingle;
			panel1.Controls.Add(lblCustom);
			panel1.Location = new Point(500, 140);
			panel1.Margin = new Padding(4, 3, 4, 3);
			panel1.Name = "panel1";
			panel1.Size = new Size(1114, 27);
			panel1.TabIndex = 133;
			panel1.Tag = "";
			// 
			// groFixing
			// 
			groFixing.BackColor = SystemColors.Control;
			groFixing.BorderStyle = BorderStyle.FixedSingle;
			groFixing.Location = new Point(500, 166);
			groFixing.Margin = new Padding(5, 3, 5, 3);
			groFixing.Name = "groFixing";
			groFixing.PriBoltHoleDiameter = 20D;
			groFixing.PriFixing = null;
			groFixing.PriPlug = null;
			groFixing.SecFixing = null;
			groFixing.SecHoleDiameter = 20D;
			groFixing.SecPlug = null;
			groFixing.SecPlugSideType = Architexor.Models.ConnectorTool.PluggingSideType.SideA;
			groFixing.SecReducedSideA = 0D;
			groFixing.SecReducedSideB = 0D;
			groFixing.ShowPriBoltHole = true;
			groFixing.ShowSecHole = false;
			groFixing.ShowSecReducedHole = false;
			groFixing.Size = new Size(1114, 571);
			groFixing.TabIndex = 145;
			// 
			// groFinFilletCover
			// 
			groFinFilletCover.BackColor = Color.White;
			groFinFilletCover.BorderStyle = BorderStyle.FixedSingle;
			groFinFilletCover.Gap_CoverBoard_Depth = 10D;
			groFinFilletCover.Gap_CoverBoard_Length = 10D;
			groFinFilletCover.Gap_CoverBoard_Width = 10D;
			groFinFilletCover.Gap_FinPlate_Bottom = 10D;
			groFinFilletCover.Gap_FinPlate_Front = 10D;
			groFinFilletCover.Gap_FinPlate_Sides = 10D;
			groFinFilletCover.Gap_FinPlate_Top = 10D;
			groFinFilletCover.GapFillBottom = 10D;
			groFinFilletCover.GapFillEnd = 10D;
			groFinFilletCover.GapFillSide = 10D;
			groFinFilletCover.GapFillTop = 10D;
			groFinFilletCover.Location = new Point(499, 166);
			groFinFilletCover.Margin = new Padding(5, 3, 5, 3);
			groFinFilletCover.Name = "groFinFilletCover";
			groFinFilletCover.ShowCoverPlate = false;
			groFinFilletCover.ShowFilletNotch = false;
			groFinFilletCover.Size = new Size(1114, 571);
			groFinFilletCover.TabIndex = 146;
			// 
			// groPosition
			// 
			groPosition.BackColor = Color.White;
			groPosition.BorderStyle = BorderStyle.FixedSingle;
			groPosition.Gap_BackPlate_Bottom = 10D;
			groPosition.Gap_BackPlate_Front = 0D;
			groPosition.Gap_BackPlate_Left = 10D;
			groPosition.Gap_BackPlate_RecessDepth = 20D;
			groPosition.Gap_BackPlate_Right = 10D;
			groPosition.Gap_BackPlate_Top = 10D;
			groPosition.Gap_MainSecondary_Between = 5D;
			groPosition.Geometry_Connector_Height = 0D;
			groPosition.Geometry_Connector_Width = 0D;
			groPosition.HorizontalFixed = true;
			groPosition.LeftOffset = 50D;
			groPosition.Location = new Point(499, 166);
			groPosition.Margin = new Padding(5, 3, 5, 3);
			groPosition.Name = "groPosition";
			groPosition.RecessElement = Architexor.Models.ConnectorTool.RecessCase.Primary;
			groPosition.SecElemHeight = 600D;
			groPosition.SecElemWidth = 300D;
			groPosition.Size = new Size(1114, 571);
			groPosition.TabIndex = 147;
			groPosition.TopOffset = 50D;
			groPosition.VerticalFixed = true;
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(cmbModel);
			groupBox2.Controls.Add(btnSelectType);
			groupBox2.Controls.Add(btnSelectModel);
			groupBox2.Controls.Add(cmbType);
			groupBox2.Location = new Point(504, 8);
			groupBox2.Margin = new Padding(4, 3, 4, 3);
			groupBox2.Name = "groupBox2";
			groupBox2.Padding = new Padding(4, 3, 4, 3);
			groupBox2.Size = new Size(329, 119);
			groupBox2.TabIndex = 149;
			groupBox2.TabStop = false;
			groupBox2.Text = "Select Type and Model";
			// 
			// groupBox3
			// 
			groupBox3.Controls.Add(cmbPreset);
			groupBox3.Controls.Add(btnPresetDelete);
			groupBox3.Controls.Add(btnPresetSelect);
			groupBox3.Controls.Add(btnPresetSave);
			groupBox3.Location = new Point(122, 8);
			groupBox3.Margin = new Padding(4, 3, 4, 3);
			groupBox3.Name = "groupBox3";
			groupBox3.Padding = new Padding(4, 3, 4, 3);
			groupBox3.Size = new Size(373, 119);
			groupBox3.TabIndex = 150;
			groupBox3.TabStop = false;
			groupBox3.Text = "Preset";
			// 
			// groupBox4
			// 
			groupBox4.Controls.Add(btnSelectElement);
			groupBox4.Controls.Add(btnClearElement);
			groupBox4.Location = new Point(9, 8);
			groupBox4.Margin = new Padding(4, 3, 4, 3);
			groupBox4.Name = "groupBox4";
			groupBox4.Padding = new Padding(4, 3, 4, 3);
			groupBox4.Size = new Size(102, 119);
			groupBox4.TabIndex = 151;
			groupBox4.TabStop = false;
			groupBox4.Text = "Select";
			// 
			// FrmGlulamTConnector
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = SystemColors.Control;
			ClientSize = new Size(1622, 779);
			Controls.Add(groupBox4);
			Controls.Add(groupBox3);
			Controls.Add(groupBox2);
			Controls.Add(groupBox1);
			Controls.Add(txtVersion);
			Controls.Add(btnHelp);
			Controls.Add(panel1);
			Controls.Add(btnCancel);
			Controls.Add(btnOK);
			Controls.Add(pnPreview);
			Controls.Add(pnPreviewContent);
			Controls.Add(groFixing);
			Controls.Add(groFinFilletCover);
			Controls.Add(groPosition);
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			Margin = new Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "FrmGlulamTConnector";
			ShowInTaskbar = false;
			Text = "GLULAM T-CONNECTOR";
			Load += CreateTConnectorForm_Load;
			groupBox1.ResumeLayout(false);
			pnPreview.ResumeLayout(false);
			pnPreview.PerformLayout();
			pnPreviewContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)picPreviewPlan).EndInit();
			pnPreviewTitle.ResumeLayout(false);
			pnPreviewTitle.PerformLayout();
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			pnPreviousNext.ResumeLayout(false);
			pnPreviousNext.PerformLayout();
			((System.ComponentModel.ISupportInitialize)picPreviewFront).EndInit();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox3.ResumeLayout(false);
			groupBox4.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}
		#endregion
		private System.Windows.Forms.ComboBox cmbPreset;
		private System.Windows.Forms.Button btnPresetDelete;
		private System.Windows.Forms.Button btnPresetSave;
		private System.Windows.Forms.Button btnPresetSelect;
		private System.Windows.Forms.Button btnSelectElement;
		private System.Windows.Forms.Button btnClearElement;
		private System.Windows.Forms.Label txtVersion;
		private System.Windows.Forms.Button btnHelp;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnPreview;
		private System.Windows.Forms.ComboBox cmbModel;
		private System.Windows.Forms.Button btnSelectModel;
		private System.Windows.Forms.ComboBox cmbType;
		private System.Windows.Forms.Button btnSelectType;
		private System.Windows.Forms.Button btnPrevious;
		private System.Windows.Forms.Button btnNext;
		private System.Windows.Forms.Label lblPreview;
		private System.Windows.Forms.Panel pnPreview;
		private System.Windows.Forms.Label lblDrawPreview;
		private System.Windows.Forms.Panel pnPreviewContent;
		private System.Windows.Forms.Panel pnPreviewTitle;
		private System.Windows.Forms.Label lblCustom;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel pnPreviousNext;
		private System.Windows.Forms.Label lblPreviousNext;
		private System.Windows.Forms.PictureBox picPreviewFront;
		private System.Windows.Forms.PictureBox picPreviewPlan;
		private UsrTConnectorPosition groPosition;
		private UsrTConnectorFinFilletCover groFinFilletCover;
		private UsrTConnectorFixings groFixing;
		private System.Windows.Forms.CheckBox chkApplytoAll;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSetFixings;
		private System.Windows.Forms.Button btnConPosition;
		private System.Windows.Forms.Button btnSetFinFilletCover;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
	}
}