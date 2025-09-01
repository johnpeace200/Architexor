using Architexor.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace Architexor.Utils
{
	public class PresetManager
	{
		private static readonly List<Preset> m_Presets = new();

		public static List<Preset> GetPresetsByCategory(string sCategory)
		{
			List<Preset> presets = new();
			foreach(Preset p in m_Presets)
			{
				if (p.Category.Equals(sCategory))
					presets.Add(p);
			}
			return presets;
		}

		public static Preset GetPreset(string sCategory, string sName)
		{
			foreach (Preset p in m_Presets)
			{
				if (p.Category.Equals(sCategory)
					&& p.Properties.Find(x => x.Key == "Name").Value.ToString() == sName)
				{
					return p;
				}
			}
			return default;
		}

		public static void AddPreset(string sCategory, Preset preset)
		{
			preset.Category = sCategory;
			m_Presets.Add(preset);
		}

		public static void RemovePreset(string sCategory, string sName)
		{
			for(int i = 0; i < m_Presets.Count; i++)
			{
				Preset preset = m_Presets[i];
				if(preset.Category.Equals(sCategory))
				{
					if(preset.Properties.Find(x => x.Key == "Name").Value.ToString() == sName)
					{
						m_Presets.RemoveAt(i);
						return;
					}
				}
			}
		}

		public static bool IsPresetKeyValid(string sCategory, string sKey)
		{
			foreach (Preset preset in m_Presets)
			{
				if (preset.Category.Equals(sCategory))
				{
					if (preset.Properties.Find(x => x.Key == "Name").Value.ToString() == sKey)
					{
						return false;
					}
				}
			}
			return true;
		}

		public static void Serialize(bool isRead = true)
		{
			if(isRead)
			{
				//XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Preset>));
				try
				{
					//TextReader reader = new StreamReader("Presets.xml");
					//m_Presets = (List<Preset>)xmlSerializer.Deserialize(reader);
					//reader.Close();

					RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run\" + Constants.BRAND + "\\Preset");
					if (key != null)
					{
						string[] keys = key.GetValueNames();
						foreach(string sKey in keys)
						{
							Preset preset = new()
							{
								Category = sKey.Substring(0, sKey.IndexOf(" ")),
								Properties = new List<Property>()
								{
									new Property() {Key = "Name", Value = sKey.Substring(sKey.IndexOf(" ") + 1)}
								}
							};
							string[] values = ((string)key.GetValue(sKey)).Split(':', ';');
							for (int i = 0; i < values.Length - 1; i += 2)
							{
								preset.Properties.Add(new Property() { Key = values[i], Value = values[i + 1]});
							}
							m_Presets.Add(preset);
						}
						key.Close();
					}
				}
				catch(Exception ex)
				{
					LogManager.Write(LogManager.WarningLevel.Warning, "PresetManager::Serialize(iRead = true)", ex.Message);
				}
			}
			else
			{
				try
				{
					RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
					RegistryKey subkey = key.CreateSubKey(Constants.BRAND);
					key.Close();

					subkey.CreateSubKey("ErrorLog");
					try
					{
						subkey.DeleteSubKey("Preset");
					} catch (Exception) { }
					key = subkey.CreateSubKey("Preset");
					subkey.Close();

					foreach(Preset preset in m_Presets)
					{
						string sKey = preset.Category;
						string sValue = "";
						foreach(Property property in preset.Properties)
						{
							if (property.Key == "Name")
								sKey += " " + property.Value;
							else
								sValue += property.Key + ":" + property.Value + ";";
						}
						key.SetValue(sKey, sValue);
					}
					key.Close();

					//	Avoid to file write because we need administrator privilege to write to C: Drive, but sometimes user can't get it.
					//XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Preset>));
					//TextWriter writer = new StreamWriter("Presets.xml");
					//xmlSerializer.Serialize(writer, m_Presets);
					//writer.Close();
				}
				catch(Exception ex)
				{
					LogManager.Write(LogManager.WarningLevel.Warning, "PresetManager::Serialize(iRead = false)", ex.Message);
				}
			}
		}
	}

	public struct Preset
	{
		public string Category { get; set; }
		public List<Property> Properties { get; set; }
	}

	public struct Property
	{
		public string Key { get; set; }
		public object Value { get; set; }
	}
}
