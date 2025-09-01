using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATXComponents.Widgets
{
	public partial class ProgressDialog : Form
	{
		public ProgressDialog()
		{
			InitializeComponent();

			SetMinimum(0);
			SetMaximum(100);
		}

		public void SetMessage(string message)
		{
			if (lblMessage.InvokeRequired)
			{
				lblMessage.Invoke(new Action(() => lblMessage.Text = message));
			}
			else
			{
				lblMessage.Text = message;
			}
		}

		public void SetDescription(string description)
		{
			if (lblDescription.InvokeRequired)
			{
				lblDescription.Invoke(new Action(() => lblDescription.Text = description));
			}
			else
			{
				lblDescription.Text = description;
			}
		}

		public void SetMinimum(int minimum)
		{
			progressBar.Minimum = minimum;
		}

		public void SetMaximum(int maximum)
		{
			progressBar.Maximum = maximum;
		}

		public void SetProgress(int value)
		{
			progressBar.Value = value;
		}

		public void UpdateProgress(int value)
		{
			if (progressBar.InvokeRequired)
			{
				progressBar.Invoke(new Action(() => progressBar.Value = value));
			}
			else
			{
				progressBar.Value = value;
			}
		}
	}
}
