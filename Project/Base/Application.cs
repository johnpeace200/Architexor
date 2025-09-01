using System.Diagnostics;
using System.Reflection;
using Architexor.Core;
using Architexor.Forms;
using Architexor.Request;
using Architexor.Utils;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System;
using System.IO;
using System.Windows.Forms;

namespace Architexor.Base
{
	public abstract class ArchitexorApplication : IExternalApplication
	{
		internal static UIControlledApplication UIContApp = null;

		private readonly List<IExternal> m_Forms = new();
		private readonly List<Assembly> m_Assemblies = new();

		//public static List<License> thisLicenses = new List<License>();

		private EventHandler<DocumentChangedEventArgs> m_hDocChanged = null;
		private EventHandler<DocumentOpenedEventArgs> m_hDocOpened = null;
		private EventHandler<ViewActivatedEventArgs> m_hViewActivated = null;

		private PushButton m_btnShowHideHLVoids = null;

		public Result OnShutdown(UIControlledApplication application)
		{
			for (int i = 0; i < m_Forms.Count; i++)
			{
				if (m_Forms[i].IVisible())
					m_Forms[i].IClose();
			}

			PresetManager.Serialize(false);

			UnsubscribeFromChanges(GetUiApplication());
			UnsubscribeFromOpen(GetUiApplication());
			application.ViewActivated -= m_hViewActivated;
			m_hViewActivated = null;

			return Result.Succeeded;
			//	Check for update
			if (UpdateHelper.CheckForUpdate(int.Parse(UIContApp.ControlledApplication.VersionNumber)))
			{
				try
				{
					string url = Assembly.GetExecutingAssembly().Location;
					url = url.Substring(0, url.LastIndexOf("\\")) + "\\";
					//	Get the url of the plugin
					Process.Start(url + "AutoUpdater.exe");
				}
				catch (Exception e)
				{
					Autodesk.Revit.UI.TaskDialog.Show("Error", e.Message);
				}
			}

			return Result.Succeeded;
		}

		public virtual Result OnStartup(UIControlledApplication application)
		{
			//	Read Setting
			try
			{
				string sSettings = File.ReadAllText("settings.ini");
				string[] settings = sSettings.Split('\n');
				foreach (string setting in settings)
				{
					string name = setting.Split('=')[0], value = setting.Split('=')[1];
					switch (name)
					{
						case "API_ENDPOINT":
							//Constants.API_ENDPOINT = value;
							break;
						default:
							break;
					}
				}
			}
			catch (Exception) { }

			UIContApp = application;

			PushButton btn;
			BitmapImage largeImage;

			//CheckLicense();

			SubscribeToOpen(GetUiApplication());
			SubscribeToChanges(GetUiApplication());

			m_hViewActivated = new EventHandler<ViewActivatedEventArgs>(OnViewActivated);
			application.ViewActivated += m_hViewActivated;

			//	Create a custom ribbon tab
			string tabName = Constants.BRAND;
			try
			{
				application.CreateRibbonTab(tabName);
			}
			catch (Exception) { }

			string url = Assembly.GetExecutingAssembly().Location;

			//	Get the url of the BasicSplit plugin
			url = url.Substring(0, url.LastIndexOf("\\")) + "\\" + "BasicSplit.dll";
			
			if (File.Exists(url))
			{
				//	Load the plugin
				Assembly assembly = Assembly.LoadFrom(url);
				m_Assemblies.Add(assembly);

				if (application.GetRibbonPanels(tabName).Find(x => x.Name == "Split") == null)
				{
					//	Create push buttons
					PushButtonData btnSplitWall = new("btnSplitWall", "Split Wall", url, "Architexor.Commands.SplitWall");
					PushButtonData btnSplitFloor = new("btnSplitFloor", "Split Slab", url, "Architexor.Commands.SplitFloor");
					//PushButtonData btnMergeParts = new PushButtonData("btnMergeParts", "Merge Parts", url, "Architexor.Commands.MergeParts");
					//btnMergeParts.AvailabilityClassName = "PanelTool.CommandAvailabilities.MergePartsAvailability";

					//  Create a ribbon panel
					RibbonPanel panelSplit = application.CreateRibbonPanel(tabName, "Split");
					//RibbonPanel panelModify = application.CreateRibbonPanel(tabName, "Modify");

					btn = panelSplit.AddItem(btnSplitWall) as PushButton;
					largeImage = GetEmbeddedImage("btn_splitwall.png");
					btn.LargeImage = largeImage;

					btn = panelSplit.AddItem(btnSplitFloor) as PushButton;
					largeImage = GetEmbeddedImage("btn_splitslab.png");
					btn.LargeImage = largeImage;
#if DEBUG
					PushButtonData btnAutoSplit = new("btnAutoSplit", "Auto Split", url, "Architexor.Commands.AutoSplit");
					PushButtonData btnSplitConfiguration = new("btnSplitConfiguration", "Configuration", url, "Architexor.Commands.AnalysisSplit");

					panelSplit.AddSeparator();
					panelSplit.AddStackedItems(btnAutoSplit, btnSplitConfiguration);
#endif

					//panelModify.AddItem(btnMergeParts);
				}
			}

			//	Connect Plugin
			url = url.Substring(0, url.LastIndexOf("\\")) + "\\" + "Connect.dll";
			if (File.Exists(url))
			{
				//	Load the plugin
				Assembly assembly = Assembly.LoadFrom(url);
				m_Assemblies.Add(assembly);

				if (application.GetRibbonPanels(tabName).Find(x => x.Name == "Connectors") == null)
				{
					RibbonPanel panelConnect = application.CreateRibbonPanel(tabName, "Connectors");

					PushButtonData btnHalfLapConnection = new("btnHalfLapConnection", "Half Lap", url, "Architexor.Commands.HalfLapConnection");
					btn = panelConnect.AddItem(btnHalfLapConnection) as PushButton;
					largeImage = GetEmbeddedImage("btn_halflap.png");
					btn.LargeImage = largeImage;

					PushButtonData btnJointBoardConnection = new("btnJointLapConnection", "Joint Board", url, "Architexor.Commands.JointBoardConnection");
					btn = panelConnect.AddItem(btnJointBoardConnection) as PushButton;
					largeImage = GetEmbeddedImage("btn_jointboard.png");
					btn.LargeImage = largeImage;

#if DEBUG
					panelConnect.AddSeparator();

					PushButtonData btnAutoConnectors = new("btnAutoConnectors", "Auto Arrange", url, "Architexor.Commands.AutoArrangeConnectors");
					PushButtonData btnConnectorConfiguration = new("btnConnectorConfiguration", "Configuration", url, "Architexor.Commands.ConnectorConfiguration");
					panelConnect.AddStackedItems(btnAutoConnectors, btnConnectorConfiguration);
#endif

					RibbonPanel panelModify = application.CreateRibbonPanel(tabName, "Modify");
					PulldownButtonData groupModify = new("groupConnectorModify", "Modify");
					PulldownButton group = panelModify.AddItem(groupModify) as PulldownButton;
					largeImage = GetEmbeddedImage("btn_modify.png");
					group.LargeImage = largeImage;

					//PushButtonData btnUpdateParameter = new("btnUpdateParameter", "Update 3D Parameters", url, "Architexor.Commands.CmdUpdate3DParameters");
					//group.AddPushButton(btnUpdateParameter);

					//	PushButtonData btnUpdateFrame = new("btnUpdateFrame", "Update Frame", url, "Architexor.Commands.UpdateFrame");
					//	group.AddPushButton(btnUpdateFrame);

					PushButtonData btnShowHideHLVoids = new ToggleButtonData("btnShowHideHLVoids", "Show HL Voids", url, "Architexor.Commands.ShowHideHLVoids");
					m_btnShowHideHLVoids = group.AddPushButton(btnShowHideHLVoids);
				}
			}

			//	Glulam T Connector Plugin
			url = url.Substring(0, url.LastIndexOf("\\")) + "\\" + "ConnectorTool.dll";
			if (File.Exists(url))
			{
				//	Load the plugin
				Assembly assembly = Assembly.LoadFrom(url);
				m_Assemblies.Add(assembly);

				if (application.GetRibbonPanels(tabName).Find(x => x.Name == "Glulam") == null)
				{
					RibbonPanel panelTConnector = application.CreateRibbonPanel(tabName, "Glulam");

					PushButtonData btnTConnector = new("btnTConnector", "Glulam T Connector", url, "Architexor.Commands.GlulamTConnector");
					btn = panelTConnector.AddItem(btnTConnector) as PushButton;
					largeImage = GetEmbeddedImage("btn_t_connector.png");
					btn.LargeImage = largeImage;
				}
			}

			url = Assembly.GetExecutingAssembly().Location;
			url = url.Substring(0, url.LastIndexOf("\\")) + "\\" + "PropertyMarker.dll";
			if (File.Exists(url))
			{
				//	Load the plugin
				Assembly assembly = Assembly.LoadFrom(url);
				m_Assemblies.Add(assembly);

				if (application.GetRibbonPanels(tabName).Find(x => x.Name == "3D Parameters") == null)
				{
					RibbonPanel panel3DParameters = application.CreateRibbonPanel(tabName, "3D Parameters");

					PushButtonData btn3DParameters = new("btn3DParameters", "3D Parameters", url, "Architexor.Commands.CmdPropertyMarker");
					btn = panel3DParameters.AddItem(btn3DParameters) as PushButton;
					largeImage = GetEmbeddedImage("btn_3dparameters.png");
					btn.LargeImage = largeImage;
				}
			}

			url = Assembly.GetExecutingAssembly().Location;
			//	Registration
			{
			//	RibbonPanel panelLicense = application.CreateRibbonPanel(tabName, "Register");
			//	PushButtonData btnLicense = new("btnLicense", "Register", url, "Architexor.Commands.License");
			//	btn = panelLicense.AddItem(btnLicense) as PushButton;

			//	largeImage = GetEmbeddedImage("btn_registration.png");
			//	btn.LargeImage = largeImage;
			}

			//	Load Custom(small) Plugins
			url = Assembly.GetExecutingAssembly().Location;
			url = url.Substring(0, url.LastIndexOf("\\")) + "\\" + "Custom.dll";
			if (File.Exists(url))
			{
				//	Load the plugin
				Assembly assembly = Assembly.LoadFrom(url);
				m_Assemblies.Add(assembly);

				if (application.GetRibbonPanels(tabName).Find(x => x.Name == "Automation") == null)
				{
					RibbonPanel panelArchilab = application.CreateRibbonPanel(tabName, "Automation");
					PulldownButtonData groupAutomation = new("groupAutomation", "Automation");
					PulldownButton group = panelArchilab.AddItem(groupAutomation) as PulldownButton;
					largeImage = GetEmbeddedImage("btn_modify.png");
					group.LargeImage = largeImage;

					PushButtonData btnAutoTag = new("btnAutoTag", "Auto Tag", url, "Architexor.Commands.AutoTag");
					group.AddPushButton(btnAutoTag);
					//PushButtonData btnAutoDimension = new("btnAutoDimension", "Auto Dimension", url, "Architexor.Commands.AutoDimension");
					//group.AddPushButton(btnAutoDimension);
				}
			}

			PresetManager.Serialize();

			url = Assembly.GetExecutingAssembly().Location;
			url = url.Substring(0, url.LastIndexOf("\\")) + "\\" + "ATXComponents.dll";
			if (File.Exists(url))
			{
				Assembly.LoadFrom(url);
			}

			return Result.Succeeded;
		}

		public string ToggleButton()
		{
			if (m_btnShowHideHLVoids.ItemText == "Show HL Voids")
				m_btnShowHideHLVoids.ItemText = "Hide HL Voids";
			else
				m_btnShowHideHLVoids.ItemText = "Show HL Voids";
			return m_btnShowHideHLVoids.ItemText;
		}

		public Type GetClassType(string sClassName)
		{
			foreach (Assembly assembly in m_Assemblies)
			{
				IEnumerable<Type> types = null;
				try { types = assembly.ExportedTypes; }
				catch (Exception) { continue; }
				foreach (Type t in types)
				{
					if (t.Name == sClassName)
					{
						return t;
					}
				}
			}
			return null;
		}

		public object GetClassInstance(string sClassName)
		{
			foreach (Assembly assembly in m_Assemblies)
			{
				IEnumerable<Type> types = null;
				try { types = assembly.ExportedTypes; }
				catch (Exception) { continue; }
				foreach (Type t in types)
				{
					if (t.Name == sClassName)
					{
						return Activator.CreateInstance(t);
					}
				}
			}
			return null;
		}

		public object GetClassInstance(string sClassName, params object[] args)
		{
			foreach (Assembly assembly in m_Assemblies)
			{
				foreach (Type t in assembly.ExportedTypes)
				{
					if (t.Name == sClassName)
					{
						return Activator.CreateInstance(t, args);
					}
				}
			}
			return null;
		}

		//	This method creates and shows a modeless dialog, unless it already exists.
		//	<remarks>
		//		The external command invokes this on the end-user's request
		//	</remarks>
		public void DoRequest(UIApplication uiapp, ArchitexorRequestId reqId)
		{
			for (int i = m_Forms.Count - 1; i >= 0; i--)
			{
				IExternal f = m_Forms[i];
				if (f.IIsDisposed())
				{
					f.IClose();
					m_Forms.RemoveAt(i);
					continue;
				}

				if ((f.GetRequestId() == ArchitexorRequestId.SplitWall
					|| f.GetRequestId() == ArchitexorRequestId.SplitFloor)
					&& (reqId == ArchitexorRequestId.SplitWall
					|| reqId == ArchitexorRequestId.SplitFloor))
				{
					f.IClose();
					m_Forms.Remove(f);
				}

				if (f.GetRequestId() == ArchitexorRequestId.AnalysisSplit
					&& reqId == ArchitexorRequestId.AnalysisSplit)
				{
					f.IClose();
					m_Forms.Remove(f);
				}

				if ((f.GetRequestId() == ArchitexorRequestId.HalfLap) && (reqId == ArchitexorRequestId.HalfLap)
					|| (f.GetRequestId() == ArchitexorRequestId.JointBoard) && (reqId == ArchitexorRequestId.JointBoard)
					|| (f.GetRequestId() == ArchitexorRequestId.PropertyMarker || f.GetRequestId() == ArchitexorRequestId.PropertyMarkerUpdate) && (reqId == ArchitexorRequestId.PropertyMarker || reqId == ArchitexorRequestId.PropertyMarkerUpdate)
					|| (f.GetRequestId() == ArchitexorRequestId.GlulamTConnector) && (reqId == ArchitexorRequestId.GlulamTConnector)
					|| (f.GetRequestId() == ArchitexorRequestId.AutoDimensioning) && (reqId == ArchitexorRequestId.AutoDimensioning)
					|| (f.GetRequestId() == ArchitexorRequestId.AutoTagging) && (reqId == ArchitexorRequestId.AutoTagging))
				{
					f.IClose();
					m_Forms.Remove(f);
				}
			}

			// We give the objects to the new dialog;
			// The dialog becomes the owner responsible fore disposing them, eventually.
			IExternal form = null;

			switch (reqId)
			{
				case ArchitexorRequestId.SplitWall:
					form = (IExternal)GetClassInstance("FrmSplit", reqId, uiapp);
					break;
				case ArchitexorRequestId.SplitFloor:
					form = (IExternal)GetClassInstance("FrmSplit", reqId, uiapp);
					break;
				case ArchitexorRequestId.AnalysisSplit:
					form = (IExternal)GetClassInstance("FrmSplit", reqId, uiapp);
					break;
				case ArchitexorRequestId.HalfLap:
					form = (IExternal)GetClassInstance("FrmHalfLap", reqId, uiapp);
					break;
				case ArchitexorRequestId.JointBoard:
					form = (IExternal)GetClassInstance("FrmJointBoard", reqId, uiapp);
					break;
				case ArchitexorRequestId.PropertyMarker:
				case ArchitexorRequestId.PropertyMarkerUpdate:
					form = (IExternal)GetClassInstance("Frm3DProperties", reqId, uiapp);
					break;
				case ArchitexorRequestId.GlulamTConnector:
					form = (IExternal)GetClassInstance("FrmGlulamTConnector", reqId, uiapp);
					break;
				case ArchitexorRequestId.AutoDimensioning:
					form = (IExternal)GetClassInstance("FrmDimensioning", reqId, uiapp);
					break;
				case ArchitexorRequestId.AutoTagging:
					//form = (IExternal)GetClassInstance("FrmTagging", reqId, uiapp);
					form = (IExternal)GetClassInstance("AutoTaggingWindow", reqId, uiapp);
					break;
				default:
					break;
			}

			if (form != null)
			{
				form.IShow();
				m_Forms.Add(form);
			}
		}

		//	Waking up the dialog from its waiting state.
		public void WakeRequestUp(ArchitexorRequestId reqId, bool bFinish = false)
		{
			foreach (IExternal f in m_Forms)
			{
				if (f.GetRequestId() == reqId)
				{
					f.WakeUp(bFinish);
				}
			}
		}

		public bool CheckLicense()
		{
			return false;
			/*try
			{
				string _msg = string.Empty;
				LicenseStatus _status = LicenseStatus.UNDEFINED;
				byte[] _certPublicKeyData;

				//	Read public key from assembly
				Assembly _assembly = Assembly.GetExecutingAssembly();
				using (MemoryStream _mem = new())
				{
					_assembly.GetManifestResourceStream("Architexor.Base.LicenseVerify.cer").CopyTo(_mem);

					_certPublicKeyData = _mem.ToArray();
				}

				RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run\" + Constants.BRAND, true);
				if (key == null)
				{
					key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run\" + Constants.BRAND, true);
				}
				else
				{
					Constants.thisUser.Id = key.GetValue("UserId") != null ? key.GetValue("UserId").ToString() : "";
					Constants.thisUser.FirstName = key.GetValue("FirstName") != null ? key.GetValue("FirstName").ToString() : "";
					Constants.thisUser.LastName = key.GetValue("LastName") != null ? key.GetValue("LastName").ToString() : "";
					Constants.thisUser.Email = key.GetValue("Email") != null ? key.GetValue("Email").ToString() : "";

					//	Read local licenses
					int nNo = 1;
					thisLicenses.Clear();
					while (key.GetValue("License" + nNo) != null)
					{
						License _lic = (License)LicenseHandler.ParseLicenseFromBASE64String(typeof(License), key.GetValue("License" + nNo).ToString(), _certPublicKeyData, out _status, out _msg);

						if (_lic.ExpirationDate < DateTime.Now)
							_status = LicenseStatus.CRACKED;
						else
						{
							thisLicenses.Add(_lic);
						}
						nNo++;
					}
				}

				try
				{
					License _lic = new();
					LicenseStatus _fromServerStatus = LicenseStatus.UNDEFINED;

					string sDeviceId = LicenseHandler.GenerateUID(_lic.AppName);
					string sRet = ApiService.PostSync(Constants.API_ENDPOINT + "device/getToken", "{\"deviceId\":\"" + sDeviceId + "\"}");

					JObject jObj = JObject.Parse(sRet);
					sDeviceId = jObj.GetValue("_id").ToString();
					jObj = JObject.Parse(jObj.GetValue("userId").ToString());
					Constants.thisUser.Id = jObj.GetValue("_id").ToString();
					Constants.thisUser.Token = jObj.GetValue("token").ToString();
					Constants.thisUser.FirstName = jObj.GetValue("first_name").ToString();
					Constants.thisUser.LastName = jObj.GetValue("last_name").ToString();
					Constants.thisUser.Email = jObj.GetValue("email").ToString();

					key.SetValue("FirstName", Constants.thisUser.FirstName);
					key.SetValue("LastName", Constants.thisUser.LastName);
					key.SetValue("Email", Constants.thisUser.Email);
					key.SetValue("UserId", Constants.thisUser.Id);
					key.SetValue("DeviceId", sDeviceId);

					//	Get Subscriptions registered
					sRet = ApiService.GetResponse(Constants.API_ENDPOINT + "subscription?userId=" + HttpUtility.UrlEncode(Constants.thisUser.Id));

					JArray jArr = JArray.Parse(sRet);
					thisLicenses.Clear();
					//	Clear local license
					if (key.GetValue("ExpirationDate") != null)
					{
						key.DeleteValue("ExpirationDate");
						key.DeleteValue("Subscription");
					}
					int nNo = 1;
					while (key.GetValue("License" + nNo) != null)
					{
						key.DeleteValue("License" + nNo);
						key.DeleteValue("DeviceId" + nNo);
						nNo++;
					}
					nNo = 1;
					foreach (string s in jArr)
					{
						_lic = (License)LicenseHandler.ParseLicenseFromBASE64String(typeof(License), s, _certPublicKeyData, out _fromServerStatus, out _msg);
						if (_lic.ExpirationDate < DateTime.Now)
							_fromServerStatus = LicenseStatus.EXPIRED;
						else
						{
							thisLicenses.Add(_lic);
							key.SetValue("License" + nNo, s);
							key.SetValue("DeviceId" + nNo, _lic.UID);
							if (nNo == 1)
							{
								key.SetValue("ExpirationDate", _lic.ExpirationDate);
								key.SetValue("Subscription", _lic.Type);
							}
							nNo++;
						}
					}
				}
				catch (WebException)
				{
					//	Need to Request License first
				}
				catch (Exception e) { MessageBox.Show(e.Message); }

				if (key != null)
					key.Close();
			}
			catch (Exception e)
			{
				TaskDialog.Show("Error", e.Message);
			}
			return false;*/
		}

		public void RequestLicense(string sName, string sEmail, string sDeviceId)
		{
			/*try
			{
				SmtpClient SmtpServer = new("smtp.live.com");
				var mail = new MailMessage
				{
					From = new MailAddress(Constants.CONTACT_DEVELOPER),
					Subject = "License Request",
					IsBodyHtml = true
				};
				mail.To.Add(Constants.CONTACT_DEVELOPER);
				string htmlBody;
				htmlBody = sName + " has requested the free subscription of " + Constants.BRAND + " tools. Please reply to " + sEmail;
				mail.Body = htmlBody;
				SmtpServer.Port = 587;
				SmtpServer.UseDefaultCredentials = false;
				SmtpServer.Credentials = new System.Net.NetworkCredential(Constants.CONTACT_DEVELOPER, "Zxl20200305");
				SmtpServer.EnableSsl = true;
				//SmtpServer.Send(mail);
			}
			catch (Exception)
			{
				MessageBox.Show("Failed to send email. Please contact the provider(" + Constants.CONTACT_DEVELOPER + ") manually.");
			}

			string _msg = string.Empty;
			LicenseStatus _status = LicenseStatus.UNDEFINED;
			byte[] _certPublicKeyData;

			//Read public key from assembly
			Assembly _assembly = Assembly.GetExecutingAssembly();
			using (MemoryStream _mem = new())
			{
				_assembly.GetManifestResourceStream("Architexor.Base.LicenseVerify.cer").CopyTo(_mem);

				_certPublicKeyData = _mem.ToArray();
			}

			try
			{
				string sRet = "", sFirstName = "", sLastName = "";
				if (sName.IndexOf(".") > 0)
				{
					sFirstName = sName.Substring(0, sName.IndexOf("."));
					sLastName = sName.Substring(sName.IndexOf(".") + 1);
				}
				else if (sName.IndexOf(" ") > 0)
				{
					sFirstName = sName.Substring(0, sName.IndexOf(" "));
					sLastName = sName.Substring(sName.IndexOf(" ") + 1);
				}
				else
				{
					sFirstName = sName;
				}

				//	Register User
				JObject jObj = new JObject();
				jObj.Add("first_name", sFirstName);
				jObj.Add("last_name", sLastName);
				jObj.Add("email", sEmail);
				sRet = ApiService.PostSync(Constants.API_ENDPOINT + "user", jObj.ToString());
				jObj = JObject.Parse(sRet);
				Constants.thisUser.Id = jObj.GetValue("_id").ToString();
				Constants.thisUser.Token = jObj.GetValue("token").ToString();
				Constants.thisUser.FirstName = jObj.GetValue("first_name").ToString();
				Constants.thisUser.LastName = jObj.GetValue("last_name").ToString();
				Constants.thisUser.Email = jObj.GetValue("email").ToString();

				if (jObj.ContainsKey("licenses"))
				{
					List<JToken> licenses = jObj.GetValue("licenses").ToList();
					foreach (var license in licenses)
					{
						License _lic = new();
						_lic = (License)LicenseHandler.ParseLicenseFromBASE64String(typeof(License), license.ToString(), _certPublicKeyData, out _status, out _msg);

						if (_lic.ExpirationDate < DateTime.Now)
							_status = LicenseStatus.EXPIRED;
						else
						{
							thisLicenses.Add(_lic);
						}
					}
				}

				//	Register Device
				jObj = new JObject();
				jObj.Add("deviceId", sDeviceId);
				sRet = ApiService.PostSync(Constants.API_ENDPOINT + "device", jObj.ToString());
				jObj = JObject.Parse(sRet);
				sDeviceId = jObj.GetValue("_id").ToString();

				MessageBox.Show("License request sent successfully.");
			}
			catch (WebException wex)
			{
				if (wex.Message.Contains("403"))
				{
					MessageBox.Show("You have already requested the license on the another device. Please contact support to register more.");
				}
			}
			catch (Exception e)
			{
				MessageBox.Show("License request failed. Error: " + e.Message);
			}*/
		}

		public UIControlledApplication GetUIContApp()
		{
			return UIContApp;
		}

		public static UIApplication GetUiApplication()
		{
			string versionNumber = UIContApp.ControlledApplication.VersionNumber;
			string fieldName = versionNumber switch
			{
				"2017" or "2018" or "2019" or "2020" or "2021" or "2022" or "2023" or "2024" or "2025" => "m_uiapplication",
				_ => "m_uiapplication",
			};
			var fieldInfo = UIContApp.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

			var uiApplication = (UIApplication)fieldInfo?.GetValue(UIContApp);

			return uiApplication;
		}

		/// <summary>
		///   Subscription to DocumentChanged
		/// </summary>
		/// <remarks>
		///   We hold the delegate to remember we we have subscribed.
		/// </remarks>
		/// 
		private void SubscribeToChanges(UIApplication uiapp)
		{
			if (m_hDocChanged == null)
			{
				m_hDocChanged = new EventHandler<DocumentChangedEventArgs>(DocChangedHandler);
				uiapp.Application.DocumentChanged += m_hDocChanged;
			}
		}

		private void SubscribeToOpen(UIApplication uiapp)
		{
			if (m_hDocOpened == null)
			{
				m_hDocOpened = new EventHandler<DocumentOpenedEventArgs>(DocOpenedHandler);
				uiapp.Application.DocumentOpened += m_hDocOpened;
			}
		}

		/// <summary>
		///   Unsubscribing from DocumentChanged event
		/// </summary>
		/// 
		private void UnsubscribeFromChanges(UIApplication uiapp)
		{
			if (m_hDocChanged != null)
			{
				uiapp.Application.DocumentChanged -= m_hDocChanged;
				m_hDocChanged = null;
			}
		}

		/// <summary>
		///   Unsubscribing from DocumentOpened event
		/// </summary>
		/// 
		private void UnsubscribeFromOpen(UIApplication uiapp)
		{
			if (m_hDocOpened != null)
			{
				uiapp.Application.DocumentOpened -= m_hDocOpened;
				m_hDocOpened = null;
			}
		}

		/// <summary>
		///   DocumentChanged Handler
		/// </summary>
		/// <remarks>
		///   It monitors changes to the element that is being analyzed.
		///   If the element was changed, we ask it to restart the analysis.
		///   If the element was deleted, we ask the analyzer to stop.
		/// </remarks>
		/// 
		public void DocChangedHandler(object sender, DocumentChangedEventArgs args)
		{
			//Controller controller;
			try
			{
				//controller = (Controller)thisApp.GetClassInstance("HalfLap");
				//if (controller != null)
				//{
				//	MethodInfo mi = controller.GetType().GetMethod("DocChangedHandler");
				//	mi.Invoke(controller, new object[] { args });
				//}
				MethodInfo mi = GetClassType("HalfLap")?.GetMethod("DocChangedHandler");
				bool bHasHL = false;
				if (mi != null)
					bHasHL = (bool)mi.Invoke(null, new object[] { args });

				// 				controller = (Controller)thisApp.GetClassInstance("JointBoard");
				// 				if (controller != null)
				// 				{
				// 					MethodInfo mi = controller.GetType().GetMethod("DocChangedHandler");
				// 					mi.Invoke(controller, new object[] { args });
				// 				}
				mi = GetClassType("JointBoard")?.GetMethod("DocChangedHandler");
				bool bHasJB = false;
				if (mi != null)
				{
					bHasJB = (bool)mi.Invoke(null, new object[] { args });
				}

				//mi = thisApp.GetClassType("SOWBeam")?.GetMethod("DocChangedHandler");
				//bool bHasSOW = (bool)mi.Invoke(null, new object[] { args });
				//bool bHasSOW = SOWBeam.DocChangedHandler(args);

				//controller = (Controller)(new PropertyMarker());
				//((PropertyMarker)controller).DocChangedHandler(args);
				mi = GetClassType("PropertyMarker")?.GetMethod("DocChangedHandler");
				bool bHas3D = false;
				if (mi != null)
				{
					bHas3D = (bool)mi.Invoke(null, new object[] { args });
				}

				if (bHasHL || bHasJB || bHas3D)// || bHasSOW
				{
					GetUIContApp().Idling += OnIdlingEvent;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Can not load Connector tool. Please contact developer.");
				MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
			}
		}

		public void DocOpenedHandler(object sender, DocumentOpenedEventArgs args)
		{
			//Controller controller;
			try
			{
				// 				controller = (Controller)thisApp.GetClassInstance("HalfLap");
				// 				if (controller != null)
				// 				{
				// 					MethodInfo mi = controller.GetType().GetMethod("DocOpenedHandler");
				// 					mi.Invoke(controller, new object[] { args });
				// 				}
				MethodInfo mi = GetClassType("HalfLap")?.GetMethod("DocOpenedHandler");
				if (mi != null)
					mi.Invoke(null, new object[] { args });
				// 
				// 				controller = (Controller)thisApp.GetClassInstance("JointBoard");
				// 				if (controller != null)
				// 				{
				// 					MethodInfo mi = controller.GetType().GetMethod("DocOpenedHandler");
				// 					mi.Invoke(controller, new object[] { args });
				// 				}
				mi = GetClassType("JointBoard")?.GetMethod("DocOpenedHandler");
				if (mi != null)
					mi.Invoke(null, new object[] { args });

				mi = GetClassType("PropertyMarker")?.GetMethod("DocOpenedHandler");
				if (mi != null)
					mi.Invoke(null, new object[] { args });
			}
			catch (Exception ex)
			{
				MessageBox.Show("Can not load Connector tool. Please contact developer.");
				MessageBox.Show(ex.StackTrace);
			}

			//	Register Updater
			//var su = new SimpleUpdater(args.Document, args.Document.Application.ActiveAddInId);
		}

		public void OnIdlingEvent(object sender, IdlingEventArgs e)
		{
			MethodInfo mi = GetClassType("HalfLap")?.GetMethod("OnIdlingEvent");
			if (mi != null) mi.Invoke(null, new object[] { sender, e });

			mi = GetClassType("JointBoard")?.GetMethod("OnIdlingEvent");
			if (mi != null) mi.Invoke(null, new object[] { sender, e });

			//SOWBeam.OnIdlingEvent(sender, e);
			//mi = thisApp.GetClassType("SOWBeam").GetMethod("OnIdlingEvent");
			//mi.Invoke(null, new object[] { sender, e });

			mi = GetClassType("PropertyMarker")?.GetMethod("OnIdlingEvent");
			if (mi != null) mi.Invoke(null, new object[] { sender, e });

			GetUIContApp().Idling -= OnIdlingEvent;
		}

		public void OnViewActivated(object sender, ViewActivatedEventArgs args)
		{
			for (int i = m_Forms.Count - 1; i >= 0; i--)
			{
				IExternal f = m_Forms[i];
				if (f.IIsDisposed())
				{
					f.IClose();
					m_Forms.RemoveAt(i);
					continue;
				}

				f.IClose();
				m_Forms.Remove(f);
			}
		}

		public static void GetGUID()
		{
			//return uiApp.ActiveUIDocument.Document.ProjectInformation.UniqueId;
		}

		public BitmapImage GetEmbeddedImage(string resourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();

			string[] names = assembly.GetManifestResourceNames();
			string prefix = "";
			foreach (string name in names)
			{
				if (name.IndexOf(".Resources.") > 0
					&& name.IndexOf(".Properties.Resources.") < 0)
				{
					prefix = name.Substring(0, name.IndexOf(".Resources"));
					prefix += ".Resources.";
					break;
				}
			}

			using (Stream stream = assembly.GetManifestResourceStream(prefix + resourceName))
			{
				if (stream == null)
					throw new FileNotFoundException($"Resource '{resourceName}' not found in assembly.");

				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = stream;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				return bitmapImage;
			}
		}
	}
}
