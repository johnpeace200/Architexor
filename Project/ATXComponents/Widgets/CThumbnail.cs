using System;
using System.Drawing;
using System.Windows.Forms;

namespace ATXComponents.Widgets
{
	public class CThumbnail : PictureBox
	{
		public int Index { get; set; }
		protected static Color SELECTED_BACKCOLOR = Color.FromArgb(255, 0, 120, 215);
		protected Button btnDelete = new Button();

		public CThumbnail()
		{
			Height = 125;
			Cursor = Cursors.Hand;
			Margin = new Padding(0);

			Controls.Add(btnDelete);
			btnDelete.Size = new System.Drawing.Size(25, 25);
			btnDelete.Text = "X";
			btnDelete.BackColor = System.Drawing.SystemColors.Control;
			btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnDelete.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			btnDelete.Location = new System.Drawing.Point(160, 10);
			btnDelete.Name = "btnDelete";
			btnDelete.UseVisualStyleBackColor = false;
			btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
		}

		public void SetSelected(bool bSelected)
		{
			if(bSelected)
			{
				BackColor = SELECTED_BACKCOLOR;
			}
			else
			{
				BackColor = Color.Transparent;
			}
		}

		public void btnDelete_Click(object sender, EventArgs e)
		{
			Parent.Controls.Remove(this);
			Dispose(true);
		}
	}
}
