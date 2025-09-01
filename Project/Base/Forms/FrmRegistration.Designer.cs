
namespace Architexor.Forms
{
	partial class FrmRegistration
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
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label12;
			this.txtVersion = new System.Windows.Forms.Label();
			this.btnHelp = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.txtDeviceID = new System.Windows.Forms.TextBox();
			this.txtUserName = new System.Windows.Forms.TextBox();
			this.txtEmail = new System.Windows.Forms.TextBox();
			this.lblCategory1 = new System.Windows.Forms.Label();
			this.lblCategoryStatus1 = new System.Windows.Forms.Label();
			this.lblExpirationDate1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			label5 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label12 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtVersion
			// 
			this.txtVersion.AutoSize = true;
			this.txtVersion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(187)))), ((int)(((byte)(179)))));
			this.txtVersion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtVersion.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtVersion.Location = new System.Drawing.Point(86, 154);
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
			this.btnHelp.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnHelp.Location = new System.Drawing.Point(6, 153);
			this.btnHelp.Name = "btnHelp";
			this.btnHelp.Size = new System.Drawing.Size(75, 23);
			this.btnHelp.TabIndex = 28;
			this.btnHelp.Text = "Help";
			this.btnHelp.UseVisualStyleBackColor = true;
			this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
			// 
			// btnClose
			// 
			this.btnClose.BackColor = System.Drawing.SystemColors.Control;
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnClose.Location = new System.Drawing.Point(419, 153);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 25;
			this.btnClose.Text = "&Cancel";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.SystemColors.Control;
			this.btnOK.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnOK.Location = new System.Drawing.Point(292, 153);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(121, 23);
			this.btnOK.TabIndex = 24;
			this.btnOK.Text = "&Request License";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnRequestLicense_Click);
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.BackColor = System.Drawing.Color.Transparent;
			label5.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label5.Location = new System.Drawing.Point(12, 24);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(89, 15);
			label5.TabIndex = 57;
			label5.Text = "Name.Surname";
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.BackColor = System.Drawing.Color.Transparent;
			label9.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label9.Location = new System.Drawing.Point(12, 50);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(85, 15);
			label9.TabIndex = 56;
			label9.Text = "Email Address";
			// 
			// label12
			// 
			label12.AutoSize = true;
			label12.BackColor = System.Drawing.Color.Transparent;
			label12.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label12.Location = new System.Drawing.Point(12, 76);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(58, 15);
			label12.TabIndex = 55;
			label12.Text = "Device ID";
			// 
			// txtDeviceID
			// 
			this.txtDeviceID.BackColor = System.Drawing.Color.White;
			this.txtDeviceID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtDeviceID.Location = new System.Drawing.Point(179, 74);
			this.txtDeviceID.Name = "txtDeviceID";
			this.txtDeviceID.ReadOnly = true;
			this.txtDeviceID.Size = new System.Drawing.Size(296, 20);
			this.txtDeviceID.TabIndex = 39;
			// 
			// txtUserName
			// 
			this.txtUserName.BackColor = System.Drawing.Color.White;
			this.txtUserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtUserName.Location = new System.Drawing.Point(179, 22);
			this.txtUserName.Name = "txtUserName";
			this.txtUserName.Size = new System.Drawing.Size(296, 20);
			this.txtUserName.TabIndex = 44;
			// 
			// txtEmail
			// 
			this.txtEmail.BackColor = System.Drawing.Color.White;
			this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtEmail.Location = new System.Drawing.Point(179, 48);
			this.txtEmail.Name = "txtEmail";
			this.txtEmail.Size = new System.Drawing.Size(296, 20);
			this.txtEmail.TabIndex = 50;
			// 
			// lblCategory1
			// 
			this.lblCategory1.BackColor = System.Drawing.Color.Transparent;
			this.lblCategory1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCategory1.Location = new System.Drawing.Point(12, 122);
			this.lblCategory1.Name = "lblCategory1";
			this.lblCategory1.Size = new System.Drawing.Size(127, 28);
			this.lblCategory1.TabIndex = 43;
			this.lblCategory1.Text = "Subscription";
			this.lblCategory1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblCategoryStatus1
			// 
			this.lblCategoryStatus1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCategoryStatus1.Location = new System.Drawing.Point(145, 127);
			this.lblCategoryStatus1.Name = "lblCategoryStatus1";
			this.lblCategoryStatus1.Size = new System.Drawing.Size(83, 19);
			this.lblCategoryStatus1.TabIndex = 8;
			this.lblCategoryStatus1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblExpirationDate1
			// 
			this.lblExpirationDate1.AutoSize = true;
			this.lblExpirationDate1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblExpirationDate1.Location = new System.Drawing.Point(312, 127);
			this.lblExpirationDate1.Name = "lblExpirationDate1";
			this.lblExpirationDate1.Size = new System.Drawing.Size(0, 19);
			this.lblExpirationDate1.TabIndex = 6;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(label5);
			this.groupBox1.Controls.Add(label9);
			this.groupBox1.Controls.Add(this.txtEmail);
			this.groupBox1.Controls.Add(label12);
			this.groupBox1.Controls.Add(this.txtUserName);
			this.groupBox1.Controls.Add(this.txtDeviceID);
			this.groupBox1.Location = new System.Drawing.Point(6, 7);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(488, 106);
			this.groupBox1.TabIndex = 111;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "User Information";
			// 
			// FrmRegistration
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(502, 181);
			this.Controls.Add(this.lblCategory1);
			this.Controls.Add(this.lblCategoryStatus1);
			this.Controls.Add(this.lblExpirationDate1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.txtVersion);
			this.Controls.Add(this.btnHelp);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FrmRegistration";
			this.ShowIcon = false;
			this.Text = "LICENSE";
			this.Load += new System.EventHandler(this.FrmRegistration_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label lblExpirationDate1;
		private System.Windows.Forms.Label txtVersion;
		private System.Windows.Forms.Button btnHelp;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox txtDeviceID;
		private System.Windows.Forms.TextBox txtUserName;
		private System.Windows.Forms.TextBox txtEmail;
		private System.Windows.Forms.Label lblCategoryStatus1;
        private System.Windows.Forms.Label lblCategory1;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}

