using Architexor.Core.Controls;
using System.Windows.Forms;

namespace Architexor.Widgets.ConnectorTool
{
    partial class UsrTConnectorPosition
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UsrTConnectorPosition));
			this.groHostPri = new System.Windows.Forms.GroupBox();
			this.txtNhMSideA = new System.Windows.Forms.TextBox();
			this.txtNhMSideB = new System.Windows.Forms.TextBox();
			this.txtNhMBottom = new System.Windows.Forms.TextBox();
			this.txtNhMTop = new System.Windows.Forms.TextBox();
			this.txtNhMDepth = new System.Windows.Forms.TextBox();
			this.chkNhM = new System.Windows.Forms.CheckBox();
			this.picHostPrimary = new System.Windows.Forms.PictureBox();
			this.groPosition = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtPosHLeft = new System.Windows.Forms.TextBox();
			this.txtPosHRight = new System.Windows.Forms.TextBox();
			this.groPositionVertical = new System.Windows.Forms.GroupBox();
			this.txtPosVTop = new System.Windows.Forms.TextBox();
			this.txtPosVBottom = new System.Windows.Forms.TextBox();
			this.txtNhSGapMainSec = new System.Windows.Forms.TextBox();
			this.picPositionVertical = new System.Windows.Forms.PictureBox();
			this.picPositionHorizontal = new System.Windows.Forms.PictureBox();
			this.groHostSec = new System.Windows.Forms.GroupBox();
			this.txtNhSBottom = new System.Windows.Forms.TextBox();
			this.txtNhSSideB = new System.Windows.Forms.TextBox();
			this.txtNhSSideA = new System.Windows.Forms.TextBox();
			this.txtNhSTop = new System.Windows.Forms.TextBox();
			this.txtNhSDepth = new System.Windows.Forms.TextBox();
			this.chkNhS = new System.Windows.Forms.CheckBox();
			this.picHostSecondary = new System.Windows.Forms.PictureBox();
			this.chkPosWidthFixedToCentre = new Architexor.Core.Controls.CCheckBox();
			this.chkPosFixedToSecondary = new Architexor.Core.Controls.CCheckBox();
			this.groHostPri.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picHostPrimary)).BeginInit();
			this.groPosition.SuspendLayout();
			this.groPositionVertical.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picPositionVertical)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picPositionHorizontal)).BeginInit();
			this.groHostSec.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picHostSecondary)).BeginInit();
			this.SuspendLayout();
			// 
			// groHostPri
			// 
			this.groHostPri.Controls.Add(this.txtNhMSideA);
			this.groHostPri.Controls.Add(this.txtNhMSideB);
			this.groHostPri.Controls.Add(this.txtNhMBottom);
			this.groHostPri.Controls.Add(this.txtNhMTop);
			this.groHostPri.Controls.Add(this.txtNhMDepth);
			this.groHostPri.Controls.Add(this.chkNhM);
			this.groHostPri.Controls.Add(this.picHostPrimary);
			this.groHostPri.Location = new System.Drawing.Point(326, -3);
			this.groHostPri.Name = "groHostPri";
			this.groHostPri.Size = new System.Drawing.Size(300, 485);
			this.groHostPri.TabIndex = 5;
			this.groHostPri.TabStop = false;
			// 
			// txtNhMSideA
			// 
			this.txtNhMSideA.BackColor = System.Drawing.Color.White;
			this.txtNhMSideA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtNhMSideA.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtNhMSideA.Location = new System.Drawing.Point(31, 360);
			this.txtNhMSideA.Name = "txtNhMSideA";
			this.txtNhMSideA.Size = new System.Drawing.Size(48, 22);
			this.txtNhMSideA.TabIndex = 16;
			this.txtNhMSideA.Text = "10";
			this.txtNhMSideA.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtNhMSideA.TextChanged += new System.EventHandler(this.TxtChangingRecessToMainParams);
			// 
			// txtNhMSideB
			// 
			this.txtNhMSideB.BackColor = System.Drawing.Color.White;
			this.txtNhMSideB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtNhMSideB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtNhMSideB.Location = new System.Drawing.Point(31, 431);
			this.txtNhMSideB.Name = "txtNhMSideB";
			this.txtNhMSideB.Size = new System.Drawing.Size(48, 22);
			this.txtNhMSideB.TabIndex = 15;
			this.txtNhMSideB.Text = "10";
			this.txtNhMSideB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtNhMSideB.TextChanged += new System.EventHandler(this.TxtChangingRecessToMainParams);
			// 
			// txtNhMBottom
			// 
			this.txtNhMBottom.BackColor = System.Drawing.Color.White;
			this.txtNhMBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtNhMBottom.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtNhMBottom.Location = new System.Drawing.Point(31, 212);
			this.txtNhMBottom.Name = "txtNhMBottom";
			this.txtNhMBottom.Size = new System.Drawing.Size(48, 22);
			this.txtNhMBottom.TabIndex = 14;
			this.txtNhMBottom.Text = "10";
			this.txtNhMBottom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtNhMBottom.TextChanged += new System.EventHandler(this.TxtChangingRecessToMainParams);
			// 
			// txtNhMTop
			// 
			this.txtNhMTop.BackColor = System.Drawing.Color.White;
			this.txtNhMTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtNhMTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtNhMTop.Location = new System.Drawing.Point(31, 77);
			this.txtNhMTop.Name = "txtNhMTop";
			this.txtNhMTop.Size = new System.Drawing.Size(48, 22);
			this.txtNhMTop.TabIndex = 13;
			this.txtNhMTop.Text = "10";
			this.txtNhMTop.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtNhMTop.TextChanged += new System.EventHandler(this.TxtChangingRecessToMainParams);
			// 
			// txtNhMDepth
			// 
			this.txtNhMDepth.BackColor = System.Drawing.Color.White;
			this.txtNhMDepth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtNhMDepth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtNhMDepth.Location = new System.Drawing.Point(109, 332);
			this.txtNhMDepth.Name = "txtNhMDepth";
			this.txtNhMDepth.Size = new System.Drawing.Size(48, 22);
			this.txtNhMDepth.TabIndex = 12;
			this.txtNhMDepth.Text = "10";
			this.txtNhMDepth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtNhMDepth.TextChanged += new System.EventHandler(this.TxtChangingRecessToMainParams);
			// 
			// chkNhM
			// 
			this.chkNhM.AutoSize = true;
			this.chkNhM.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkNhM.Location = new System.Drawing.Point(72, 27);
			this.chkNhM.Name = "chkNhM";
			this.chkNhM.Size = new System.Drawing.Size(131, 20);
			this.chkNhM.TabIndex = 11;
			this.chkNhM.Tag = "RecessToMain";
			this.chkNhM.Text = "Host in Primary";
			this.chkNhM.UseVisualStyleBackColor = true;
			this.chkNhM.CheckedChanged += new System.EventHandler(this.ChkNhM_CheckedChanged);
			// 
			// picHostPrimary
			// 
			this.picHostPrimary.Image = global::ATXComponents.Properties.Resources.picHostPri;
			this.picHostPrimary.Location = new System.Drawing.Point(10, 61);
			this.picHostPrimary.Name = "picHostPrimary";
			this.picHostPrimary.Size = new System.Drawing.Size(278, 415);
			this.picHostPrimary.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picHostPrimary.TabIndex = 1;
			this.picHostPrimary.TabStop = false;
			// 
			// groPosition
			// 
			this.groPosition.Controls.Add(this.chkPosWidthFixedToCentre);
			this.groPosition.Controls.Add(this.label1);
			this.groPosition.Controls.Add(this.txtPosHLeft);
			this.groPosition.Controls.Add(this.txtPosHRight);
			this.groPosition.Controls.Add(this.groPositionVertical);
			this.groPosition.Controls.Add(this.picPositionHorizontal);
			this.groPosition.Location = new System.Drawing.Point(6, -3);
			this.groPosition.Name = "groPosition";
			this.groPosition.Size = new System.Drawing.Size(300, 485);
			this.groPosition.TabIndex = 4;
			this.groPosition.TabStop = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(67, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(131, 16);
			this.label1.TabIndex = 9;
			this.label1.Text = "Connctor Location";
			// 
			// txtPosHLeft
			// 
			this.txtPosHLeft.BackColor = System.Drawing.Color.White;
			this.txtPosHLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtPosHLeft.Location = new System.Drawing.Point(41, 451);
			this.txtPosHLeft.Name = "txtPosHLeft";
			this.txtPosHLeft.Size = new System.Drawing.Size(48, 20);
			this.txtPosHLeft.TabIndex = 3;
			this.txtPosHLeft.Text = "100";
			this.txtPosHLeft.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtPosHLeft.TextChanged += new System.EventHandler(this.TxtPosHLeft_TextChanged);
			// 
			// txtPosHRight
			// 
			this.txtPosHRight.BackColor = System.Drawing.Color.White;
			this.txtPosHRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtPosHRight.Location = new System.Drawing.Point(41, 342);
			this.txtPosHRight.Name = "txtPosHRight";
			this.txtPosHRight.Size = new System.Drawing.Size(48, 20);
			this.txtPosHRight.TabIndex = 3;
			this.txtPosHRight.Text = "100";
			this.txtPosHRight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtPosHRight.TextChanged += new System.EventHandler(this.TxtPosHRight_TextChanged);
			// 
			// groPositionVertical
			// 
			this.groPositionVertical.Controls.Add(this.txtPosVTop);
			this.groPositionVertical.Controls.Add(this.txtPosVBottom);
			this.groPositionVertical.Controls.Add(this.txtNhSGapMainSec);
			this.groPositionVertical.Controls.Add(this.chkPosFixedToSecondary);
			this.groPositionVertical.Controls.Add(this.picPositionVertical);
			this.groPositionVertical.Location = new System.Drawing.Point(9, 60);
			this.groPositionVertical.Name = "groPositionVertical";
			this.groPositionVertical.Size = new System.Drawing.Size(278, 257);
			this.groPositionVertical.TabIndex = 15;
			this.groPositionVertical.TabStop = false;
			// 
			// txtPosVTop
			// 
			this.txtPosVTop.BackColor = System.Drawing.Color.White;
			this.txtPosVTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtPosVTop.Location = new System.Drawing.Point(32, 5);
			this.txtPosVTop.Name = "txtPosVTop";
			this.txtPosVTop.Size = new System.Drawing.Size(48, 20);
			this.txtPosVTop.TabIndex = 3;
			this.txtPosVTop.Text = "100";
			this.txtPosVTop.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtPosVTop.TextChanged += new System.EventHandler(this.TxtPosVTop_TextChanged);
			// 
			// txtPosVBottom
			// 
			this.txtPosVBottom.BackColor = System.Drawing.Color.White;
			this.txtPosVBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtPosVBottom.Location = new System.Drawing.Point(32, 163);
			this.txtPosVBottom.Name = "txtPosVBottom";
			this.txtPosVBottom.Size = new System.Drawing.Size(48, 20);
			this.txtPosVBottom.TabIndex = 3;
			this.txtPosVBottom.Text = "100";
			this.txtPosVBottom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtPosVBottom.TextChanged += new System.EventHandler(this.TxtPosVBottom_TextChanged);
			// 
			// txtNhSGapMainSec
			// 
			this.txtNhSGapMainSec.BackColor = System.Drawing.Color.White;
			this.txtNhSGapMainSec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtNhSGapMainSec.Location = new System.Drawing.Point(101, 178);
			this.txtNhSGapMainSec.Name = "txtNhSGapMainSec";
			this.txtNhSGapMainSec.Size = new System.Drawing.Size(48, 20);
			this.txtNhSGapMainSec.TabIndex = 10;
			this.txtNhSGapMainSec.Text = "5";
			this.txtNhSGapMainSec.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtNhSGapMainSec.TextChanged += new System.EventHandler(this.TxtChangingRecessToMainParams);
			// 
			// picPositionVertical
			// 
			this.picPositionVertical.Image = ((System.Drawing.Image)(resources.GetObject("picPositionVertical.Image")));
			this.picPositionVertical.Location = new System.Drawing.Point(0, 1);
			this.picPositionVertical.Name = "picPositionVertical";
			this.picPositionVertical.Size = new System.Drawing.Size(278, 255);
			this.picPositionVertical.TabIndex = 0;
			this.picPositionVertical.TabStop = false;
			// 
			// picPositionHorizontal
			// 
			this.picPositionHorizontal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.picPositionHorizontal.Image = ((System.Drawing.Image)(resources.GetObject("picPositionHorizontal.Image")));
			this.picPositionHorizontal.Location = new System.Drawing.Point(9, 60);
			this.picPositionHorizontal.Name = "picPositionHorizontal";
			this.picPositionHorizontal.Size = new System.Drawing.Size(278, 419);
			this.picPositionHorizontal.TabIndex = 14;
			this.picPositionHorizontal.TabStop = false;
			// 
			// groHostSec
			// 
			this.groHostSec.Controls.Add(this.txtNhSBottom);
			this.groHostSec.Controls.Add(this.txtNhSSideB);
			this.groHostSec.Controls.Add(this.txtNhSSideA);
			this.groHostSec.Controls.Add(this.txtNhSTop);
			this.groHostSec.Controls.Add(this.txtNhSDepth);
			this.groHostSec.Controls.Add(this.chkNhS);
			this.groHostSec.Controls.Add(this.picHostSecondary);
			this.groHostSec.Location = new System.Drawing.Point(645, -3);
			this.groHostSec.Name = "groHostSec";
			this.groHostSec.Size = new System.Drawing.Size(300, 485);
			this.groHostSec.TabIndex = 6;
			this.groHostSec.TabStop = false;
			// 
			// txtNhSBottom
			// 
			this.txtNhSBottom.BackColor = System.Drawing.Color.White;
			this.txtNhSBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtNhSBottom.Location = new System.Drawing.Point(36, 212);
			this.txtNhSBottom.Name = "txtNhSBottom";
			this.txtNhSBottom.Size = new System.Drawing.Size(48, 20);
			this.txtNhSBottom.TabIndex = 17;
			this.txtNhSBottom.Text = "10";
			this.txtNhSBottom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtNhSBottom.TextChanged += new System.EventHandler(this.TxtChangingRecessToMainParams);
			// 
			// txtNhSSideB
			// 
			this.txtNhSSideB.BackColor = System.Drawing.Color.White;
			this.txtNhSSideB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtNhSSideB.Location = new System.Drawing.Point(36, 431);
			this.txtNhSSideB.Name = "txtNhSSideB";
			this.txtNhSSideB.Size = new System.Drawing.Size(48, 20);
			this.txtNhSSideB.TabIndex = 16;
			this.txtNhSSideB.Text = "10";
			this.txtNhSSideB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtNhSSideB.TextChanged += new System.EventHandler(this.TxtChangingRecessToMainParams);
			// 
			// txtNhSSideA
			// 
			this.txtNhSSideA.BackColor = System.Drawing.Color.White;
			this.txtNhSSideA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtNhSSideA.Location = new System.Drawing.Point(36, 363);
			this.txtNhSSideA.Name = "txtNhSSideA";
			this.txtNhSSideA.Size = new System.Drawing.Size(48, 20);
			this.txtNhSSideA.TabIndex = 15;
			this.txtNhSSideA.Text = "10";
			this.txtNhSSideA.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtNhSSideA.TextChanged += new System.EventHandler(this.TxtChangingRecessToMainParams);
			// 
			// txtNhSTop
			// 
			this.txtNhSTop.BackColor = System.Drawing.Color.White;
			this.txtNhSTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtNhSTop.Location = new System.Drawing.Point(36, 77);
			this.txtNhSTop.Name = "txtNhSTop";
			this.txtNhSTop.Size = new System.Drawing.Size(48, 20);
			this.txtNhSTop.TabIndex = 14;
			this.txtNhSTop.Text = "10";
			this.txtNhSTop.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtNhSTop.TextChanged += new System.EventHandler(this.TxtChangingRecessToMainParams);
			// 
			// txtNhSDepth
			// 
			this.txtNhSDepth.BackColor = System.Drawing.Color.White;
			this.txtNhSDepth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtNhSDepth.Location = new System.Drawing.Point(123, 332);
			this.txtNhSDepth.Name = "txtNhSDepth";
			this.txtNhSDepth.Size = new System.Drawing.Size(48, 20);
			this.txtNhSDepth.TabIndex = 13;
			this.txtNhSDepth.Text = "10";
			this.txtNhSDepth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtNhSDepth.TextChanged += new System.EventHandler(this.TxtChangingRecessToMainParams);
			// 
			// chkNhS
			// 
			this.chkNhS.AutoSize = true;
			this.chkNhS.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkNhS.Location = new System.Drawing.Point(76, 27);
			this.chkNhS.Name = "chkNhS";
			this.chkNhS.Size = new System.Drawing.Size(153, 20);
			this.chkNhS.TabIndex = 12;
			this.chkNhS.Tag = "RecessToSec";
			this.chkNhS.Text = "Host in Secondary";
			this.chkNhS.UseVisualStyleBackColor = true;
			this.chkNhS.CheckedChanged += new System.EventHandler(this.ChkNhS_CheckedChanged);
			// 
			// picHostSecondary
			// 
			this.picHostSecondary.Image = global::ATXComponents.Properties.Resources.picHostSec;
			this.picHostSecondary.Location = new System.Drawing.Point(11, 61);
			this.picHostSecondary.Name = "picHostSecondary";
			this.picHostSecondary.Size = new System.Drawing.Size(278, 415);
			this.picHostSecondary.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picHostSecondary.TabIndex = 1;
			this.picHostSecondary.TabStop = false;
			// 
			// chkPosWidthFixedToCentre
			// 
			this.chkPosWidthFixedToCentre.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(177)))), ((int)(((byte)(95)))));
			this.chkPosWidthFixedToCentre.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.chkPosWidthFixedToCentre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkPosWidthFixedToCentre.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkPosWidthFixedToCentre.Location = new System.Drawing.Point(228, 397);
			this.chkPosWidthFixedToCentre.Name = "chkPosWidthFixedToCentre";
			this.chkPosWidthFixedToCentre.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.chkPosWidthFixedToCentre.Size = new System.Drawing.Size(28, 21);
			this.chkPosWidthFixedToCentre.TabIndex = 8;
			this.chkPosWidthFixedToCentre.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkPosWidthFixedToCentre.UseVisualStyleBackColor = false;
			this.chkPosWidthFixedToCentre.CheckedChanged += new System.EventHandler(this.ChkPosWidthFixedToCentre_CheckedChanged);
			// 
			// chkPosFixedToSecondary
			// 
			this.chkPosFixedToSecondary.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.chkPosFixedToSecondary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(177)))), ((int)(((byte)(95)))));
			this.chkPosFixedToSecondary.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.chkPosFixedToSecondary.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.chkPosFixedToSecondary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkPosFixedToSecondary.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkPosFixedToSecondary.Location = new System.Drawing.Point(219, 83);
			this.chkPosFixedToSecondary.Name = "chkPosFixedToSecondary";
			this.chkPosFixedToSecondary.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.chkPosFixedToSecondary.Size = new System.Drawing.Size(28, 21);
			this.chkPosFixedToSecondary.TabIndex = 8;
			this.chkPosFixedToSecondary.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkPosFixedToSecondary.UseVisualStyleBackColor = false;
			this.chkPosFixedToSecondary.CheckedChanged += new System.EventHandler(this.ChkPosFixedToSecondary_CheckedChanged);
			// 
			// UsrTConnectorPosition
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.groHostSec);
			this.Controls.Add(this.groPosition);
			this.Controls.Add(this.groHostPri);
			this.Name = "UsrTConnectorPosition";
			this.Size = new System.Drawing.Size(945, 485);
			this.groHostPri.ResumeLayout(false);
			this.groHostPri.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picHostPrimary)).EndInit();
			this.groPosition.ResumeLayout(false);
			this.groPosition.PerformLayout();
			this.groPositionVertical.ResumeLayout(false);
			this.groPositionVertical.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picPositionVertical)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picPositionHorizontal)).EndInit();
			this.groHostSec.ResumeLayout(false);
			this.groHostSec.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picHostSecondary)).EndInit();
			this.ResumeLayout(false);

        }

#endregion
        private System.Windows.Forms.PictureBox picHostPrimary;
        private GroupBox groHostPri;
		private PictureBox picPositionVertical;
		private GroupBox groPosition;
		private Label label1;
		private GroupBox groHostSec;
		private PictureBox picHostSecondary;
		private CheckBox chkNhM;
		private CheckBox chkNhS;
		private TextBox txtNhMDepth;
		private TextBox txtNhMTop;
		private TextBox txtNhMBottom;
		private TextBox txtNhMSideB;
		private TextBox txtNhMSideA;
		private TextBox txtNhSDepth;
		private TextBox txtNhSTop;
		private TextBox txtNhSSideA;
		private TextBox txtNhSSideB;
		private TextBox txtNhSBottom;
        private TextBox txtNhSGapMainSec;
        private CCheckBox chkPosFixedToSecondary;
		private CCheckBox chkPosWidthFixedToCentre;
		private TextBox txtPosVTop;
		private TextBox txtPosVBottom;
        private GroupBox groPositionVertical;
        private PictureBox picPositionHorizontal;
		private TextBox txtPosHLeft;
		private TextBox txtPosHRight;
	}
}
