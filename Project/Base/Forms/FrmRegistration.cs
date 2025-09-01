//using ATXLicense;
//using QLicense;
using System;
using System.Windows.Forms;
//using static Architexor.Application;
using Architexor.Core;

namespace Architexor.Forms
{
	public partial class FrmRegistration : Form
	{
		public FrmRegistration()
		{
			InitializeComponent();
		}

		private void FrmRegistration_Load(object sender, EventArgs e)
		{
			//License _lic = new License();
			//txtDeviceID.Text = LicenseHandler.GenerateUID(_lic.AppName);

			Text = "License ";
			txtVersion.Text = string.Format("{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

			RefreshStatus();

			ATXUser user = Constants.thisUser;
			txtUserName.Text = user.FirstName + " " + user.LastName;
			txtEmail.Text = user.Email;
		}

		private void RefreshStatus()
		{
			/*Application.thisApp.CheckLicense();

			lblCategory1.Text = "";
			lblCategoryStatus1.Text = "";
			lblExpirationDate1.Text = "";

			foreach (License _lic in Application.thisLicenses)
			{
				lblCategory1.Text = "Subscription";
				lblCategoryStatus1.Text = _lic.Type.ToString();
				lblExpirationDate1.Text = _lic.ExpirationDate.ToShortDateString();
			}

			if (Application.thisLicenses.Count == 0)
			{
				btnOK.Enabled = true;
				btnOK.Text = "Request License";
			}
			else
			{
				btnOK.Enabled = true;
				btnOK.Text = "Request Feature";
			}*/
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnRequestLicense_Click(object sender, EventArgs e)
		{
			/*if (txtUserName.Text == "")
			{
				MessageBox.Show("Please input your name.", "Warning");
				return;
			}
			if (txtEmail.Text == "")
			{
				MessageBox.Show("Please input your email.", "Warning");
				return;
			}

			if (btnOK.Text == "Request License")
			{
				Application.thisApp.RequestLicense(txtUserName.Text, txtEmail.Text, txtDeviceID.Text);
			}
			else
			{
				//	Request Feature
				MessageBox.Show("This will be available soon.", "Message");
			}*/
		}

		private void btnHelp_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(Constants.FRONTEND);
		}
	}
}
