using System;
using System.Windows.Forms;

namespace Architexor.Core.Controls
{
	public interface IToolTips : IDisposable
	{
		void Add(Control control, String text);
		void Batch(Control control, String text);
		void ApplyBatched();
		void RemoveAll();
	}
}