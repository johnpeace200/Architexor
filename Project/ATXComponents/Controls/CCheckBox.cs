using System.Drawing;
using System.Windows.Forms;

namespace Architexor.Core.Controls
{
	public class CCheckBox : CheckBox
	{
		public CCheckBox()
		{
			this.TextAlign = ContentAlignment.MiddleRight;
		}

		public override bool AutoSize
		{
			get { return base.AutoSize; }
			set { base.AutoSize = false; }
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			int h = this.ClientSize.Height - 2;
			Rectangle rc = new Rectangle(new Point(5, 1), new Size(h, h));
			e.Graphics.Clear(this.BackColor);
			//ControlPaint.DrawCheckBox(e.Graphics, rc,
			//	this.Checked ? ButtonState.Checked : ButtonState.Normal);
			if (this.Checked)
			{
				using (Font wing = new Font("Wingdings", 14f))
					e.Graphics.DrawString("ü", wing, Brushes.Black, rc);
			}
			//	Uncomment this line to draw the border
		//	e.Graphics.DrawRectangle(Pens.DarkSlateBlue, rc);
		}
	}
}
