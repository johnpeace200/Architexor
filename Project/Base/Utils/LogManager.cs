using Architexor.Core;
using Microsoft.Win32;
using System;

namespace Architexor.Utils
{
	public static class LogManager
	{
		public enum WarningLevel
		{
			Verbose,
			Warning,
			Error
		}

		public static void Write(WarningLevel wl, string sPositionInfo, string sMessage)
		{
			try
			{
				RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run\" + Constants.BRAND + "\\ErrorLog", true);
				string sWarningLevel = "Verbose";
				if (wl == WarningLevel.Warning) sWarningLevel = "Warning";
				else if (wl == WarningLevel.Error) sWarningLevel = "Error";
				key.SetValue(DateTime.Now.ToString("s"), string.Format("{0}\tPosition : {1}\t Detailed message : {2}", sWarningLevel, sPositionInfo, sMessage));
				key.Close();
			}
			catch(Exception)
			{ }
			/*TextWriter writer = new StreamWriter("error.log");
			string sWarningLevel = "Verbose";
			if (wl == WarningLevel.Warning) sWarningLevel = "Warning";
			else if (wl == WarningLevel.Error) sWarningLevel = "Error";
			writer.WriteLine(string.Format("{0}\tPosition : {1}\t Detailed message : {2}", sWarningLevel, sPositionInfo, sMessage));
			writer.Close();*/
		}
	}
}
