using System.Drawing;
using System.Windows.Forms;

namespace Architexor.Core.Controls
{
	public class DoubleBufferedPictureBox : PictureBox
	{
		public DoubleBufferedPictureBox()
		{
			DoubleBuffered = true;
			SetStyle(ControlStyles.AllPaintingInWmPaint |
										ControlStyles.UserPaint |
										ControlStyles.OptimizedDoubleBuffer, true);
			UpdateStyles();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// Do nothing here to avoid clearing the background
		}
	}
}
