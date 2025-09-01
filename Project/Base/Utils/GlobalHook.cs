using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Architexor.Utils
{
	public static class GlobalHook
	{
		private static IKeyboardMouseEvents m_GlobalHook;

		public static void Subscribe()
		{
			// Note: for the application hook, use the Hook.AppEvents() instead
			m_GlobalHook = Hook.GlobalEvents();

			//			m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
			m_GlobalHook.KeyPress += GlobalHookKeyPress;
		}

		private static void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
		{
			//Console.WriteLine("KeyPress: \t{0}", e.KeyChar);

			if (e.KeyChar == 13)
			{
				CompleteSelection();
			}
		}

		//		private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
		//		{
		//			Console.WriteLine("MouseDown: \t{0}; \t System Timestamp: \t{1}", e.Button, e.Timestamp);

		// uncommenting the following line will suppress the middle mouse button click
		// if (e.Buttons == MouseButtons.Middle) { e.Handled = true; }
		//		}

		public static void Unsubscribe()
		{
			//			m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
			m_GlobalHook.KeyPress -= GlobalHookKeyPress;

			//	It is recommended to dispose it
			m_GlobalHook.Dispose();
		}


		private static void CompleteSelection()
		{
			var rvtWindow = Autodesk.Windows.ComponentManager.ApplicationWindow;
			var list = new List<IntPtr>();
			var flag = WindowsHelper.EnumChildWindows(rvtWindow,
						 (hwnd, l) =>
						 {
							 StringBuilder windowText = new StringBuilder(200);
							 WindowsHelper.GetWindowText(hwnd, windowText, windowText.Capacity);
							 StringBuilder className = new StringBuilder(200);
							 WindowsHelper.GetClassName(hwnd, className, className.Capacity);
							 if ((windowText.ToString().Equals("完成", StringComparison.Ordinal) ||
							windowText.ToString().Equals("Finish", StringComparison.Ordinal)) &&
							className.ToString().Contains("Button"))
							 {
								 list.Add(hwnd);
								 return false;
							 }
							 return true;
						 }, new IntPtr(0));

			var complete = list.FirstOrDefault();
			WindowsHelper.SendMessage(complete, 245, 0, 0);
		}
	}
}
