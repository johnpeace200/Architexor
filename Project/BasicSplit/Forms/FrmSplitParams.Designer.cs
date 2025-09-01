
namespace Architexor.BasicSplit.Forms
{
	partial class FrmSplitParams
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
			System.Windows.Forms.Panel pnPreviewTag;
			System.Windows.Forms.Label lblPreview;
			System.Windows.Forms.Label label6;
			this.label1 = new System.Windows.Forms.Label();
			this.cmbUnit = new System.Windows.Forms.ComboBox();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.btnClose = new System.Windows.Forms.Button();
			this.pnPreviewContent = new System.Windows.Forms.Panel();
			this.lblPanelBoundaryColor = new System.Windows.Forms.Label();
			this.btnPanelBoundaryColor = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.btnDimensionColor = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.btnSplitLineColor = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.btnOpeningColor = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.btnElementColor = new System.Windows.Forms.Button();
			this.txtMinPanelWidth = new System.Windows.Forms.TextBox();
			pnPreviewTag = new System.Windows.Forms.Panel();
			lblPreview = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			pnPreviewTag.SuspendLayout();
			this.pnPreviewContent.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnPreviewTag
			// 
			pnPreviewTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(209)))));
			pnPreviewTag.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			pnPreviewTag.Controls.Add(lblPreview);
			pnPreviewTag.Location = new System.Drawing.Point(8, 64);
			pnPreviewTag.Name = "pnPreviewTag";
			pnPreviewTag.Size = new System.Drawing.Size(239, 19);
			pnPreviewTag.TabIndex = 4;
			// 
			// lblPreview
			// 
			lblPreview.AutoSize = true;
			lblPreview.BackColor = System.Drawing.Color.Transparent;
			lblPreview.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			lblPreview.Location = new System.Drawing.Point(3, -2);
			lblPreview.Name = "lblPreview";
			lblPreview.Size = new System.Drawing.Size(64, 19);
			lblPreview.TabIndex = 0;
			lblPreview.Text = "Preview";
			// 
			// label6
			// 
			label6.Location = new System.Drawing.Point(5, 35);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(133, 23);
			label6.TabIndex = 6;
			label6.Text = "Minimum Panel Width:";
			label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(5, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(133, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Unit:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbUnit
			// 
			this.cmbUnit.Enabled = false;
			this.cmbUnit.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmbUnit.FormattingEnabled = true;
			this.cmbUnit.Items.AddRange(new object[] {
            "mm"});
			this.cmbUnit.Location = new System.Drawing.Point(144, 9);
			this.cmbUnit.Name = "cmbUnit";
			this.cmbUnit.Size = new System.Drawing.Size(100, 23);
			this.cmbUnit.TabIndex = 1;
			this.cmbUnit.Text = "mm";
			// 
			// btnClose
			// 
			this.btnClose.BackColor = System.Drawing.SystemColors.Control;
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnClose.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnClose.Location = new System.Drawing.Point(172, 263);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 3;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = false;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// pnPreviewContent
			// 
			this.pnPreviewContent.BackColor = System.Drawing.SystemColors.Control;
			this.pnPreviewContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnPreviewContent.Controls.Add(this.lblPanelBoundaryColor);
			this.pnPreviewContent.Controls.Add(this.btnPanelBoundaryColor);
			this.pnPreviewContent.Controls.Add(this.label5);
			this.pnPreviewContent.Controls.Add(this.btnDimensionColor);
			this.pnPreviewContent.Controls.Add(this.label4);
			this.pnPreviewContent.Controls.Add(this.btnSplitLineColor);
			this.pnPreviewContent.Controls.Add(this.label3);
			this.pnPreviewContent.Controls.Add(this.btnOpeningColor);
			this.pnPreviewContent.Controls.Add(this.label2);
			this.pnPreviewContent.Controls.Add(this.btnElementColor);
			this.pnPreviewContent.Location = new System.Drawing.Point(8, 82);
			this.pnPreviewContent.Name = "pnPreviewContent";
			this.pnPreviewContent.Size = new System.Drawing.Size(239, 175);
			this.pnPreviewContent.TabIndex = 5;
			// 
			// lblPanelBoundaryColor
			// 
			this.lblPanelBoundaryColor.AutoSize = true;
			this.lblPanelBoundaryColor.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblPanelBoundaryColor.Location = new System.Drawing.Point(15, 143);
			this.lblPanelBoundaryColor.Name = "lblPanelBoundaryColor";
			this.lblPanelBoundaryColor.Size = new System.Drawing.Size(130, 15);
			this.lblPanelBoundaryColor.TabIndex = 17;
			this.lblPanelBoundaryColor.Text = "Panel Boundary Color:";
			// 
			// btnPanelBoundaryColor
			// 
			this.btnPanelBoundaryColor.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnPanelBoundaryColor.Location = new System.Drawing.Point(151, 139);
			this.btnPanelBoundaryColor.Name = "btnPanelBoundaryColor";
			this.btnPanelBoundaryColor.Size = new System.Drawing.Size(75, 23);
			this.btnPanelBoundaryColor.TabIndex = 16;
			this.btnPanelBoundaryColor.UseVisualStyleBackColor = true;
			this.btnPanelBoundaryColor.Click += new System.EventHandler(this.btnPanelBoundaryColor_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(43, 111);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(102, 15);
			this.label5.TabIndex = 15;
			this.label5.Text = "Dimension Color:";
			// 
			// btnDimensionColor
			// 
			this.btnDimensionColor.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnDimensionColor.Location = new System.Drawing.Point(151, 107);
			this.btnDimensionColor.Name = "btnDimensionColor";
			this.btnDimensionColor.Size = new System.Drawing.Size(75, 23);
			this.btnDimensionColor.TabIndex = 14;
			this.btnDimensionColor.UseVisualStyleBackColor = true;
			this.btnDimensionColor.Click += new System.EventHandler(this.btnDimensionColor_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(52, 78);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(93, 15);
			this.label4.TabIndex = 13;
			this.label4.Text = "Split Line Color:";
			// 
			// btnSplitLineColor
			// 
			this.btnSplitLineColor.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSplitLineColor.Location = new System.Drawing.Point(151, 74);
			this.btnSplitLineColor.Name = "btnSplitLineColor";
			this.btnSplitLineColor.Size = new System.Drawing.Size(75, 23);
			this.btnSplitLineColor.TabIndex = 12;
			this.btnSplitLineColor.UseVisualStyleBackColor = true;
			this.btnSplitLineColor.Click += new System.EventHandler(this.btnSplitLineColor_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(56, 46);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(89, 15);
			this.label3.TabIndex = 11;
			this.label3.Text = "Opening Color:";
			// 
			// btnOpeningColor
			// 
			this.btnOpeningColor.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnOpeningColor.Location = new System.Drawing.Point(151, 42);
			this.btnOpeningColor.Name = "btnOpeningColor";
			this.btnOpeningColor.Size = new System.Drawing.Size(75, 23);
			this.btnOpeningColor.TabIndex = 10;
			this.btnOpeningColor.UseVisualStyleBackColor = true;
			this.btnOpeningColor.Click += new System.EventHandler(this.btnOpeningColor_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(14, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(131, 15);
			this.label2.TabIndex = 9;
			this.label2.Text = "Wall(Slab, Part) Color:";
			// 
			// btnElementColor
			// 
			this.btnElementColor.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnElementColor.Location = new System.Drawing.Point(151, 10);
			this.btnElementColor.Name = "btnElementColor";
			this.btnElementColor.Size = new System.Drawing.Size(75, 23);
			this.btnElementColor.TabIndex = 8;
			this.btnElementColor.UseVisualStyleBackColor = true;
			this.btnElementColor.Click += new System.EventHandler(this.btnElementColor_Click);
			// 
			// txtMinPanelWidth
			// 
			this.txtMinPanelWidth.Location = new System.Drawing.Point(144, 37);
			this.txtMinPanelWidth.Name = "txtMinPanelWidth";
			this.txtMinPanelWidth.Size = new System.Drawing.Size(100, 20);
			this.txtMinPanelWidth.TabIndex = 7;
			// 
			// FrmSplitParams
			// 
			this.AcceptButton = this.btnClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(256, 294);
			this.Controls.Add(this.txtMinPanelWidth);
			this.Controls.Add(label6);
			this.Controls.Add(this.pnPreviewContent);
			this.Controls.Add(pnPreviewTag);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.cmbUnit);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FrmSplitParams";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "SETTINGS";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSplitParams_FormClosing);
			this.Load += new System.EventHandler(this.FrmSplitParams_Load);
			pnPreviewTag.ResumeLayout(false);
			pnPreviewTag.PerformLayout();
			this.pnPreviewContent.ResumeLayout(false);
			this.pnPreviewContent.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cmbUnit;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Panel pnPreviewContent;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnDimensionColor;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnSplitLineColor;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnOpeningColor;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnElementColor;
		private System.Windows.Forms.Label lblPanelBoundaryColor;
		private System.Windows.Forms.Button btnPanelBoundaryColor;
		private System.Windows.Forms.TextBox txtMinPanelWidth;
	}
}