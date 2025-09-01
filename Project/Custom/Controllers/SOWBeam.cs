using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Architexor.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Document = Autodesk.Revit.DB.Document;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using Architexor;

namespace Architexor.Controllers
{
	public class SOWBeam : Controller
	{
		public const string SOWBeamFamilyName = "sow-beam";

		public static List<SOWDefaultParam> SOWDefaultParams;

		/// <summary>
		/// SOW Beam Family
		/// </summary>
		public static Family SOWB = null;

		/// <summary>
		/// Count of King Studs
		/// </summary>
		protected int m_nKingStuds = 1;
		/// <summary>
		/// Count of Trimmers
		/// </summary>
		protected int m_nTrimmers = 1;

		public int KingStuds { get => m_nKingStuds; set => m_nKingStuds = value; }

		public int Trimmers { get => m_nTrimmers; set => m_nTrimmers = value; }

		//	Variables to use for internal functions
#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
		protected ForgeTypeId m_UnitType = UnitTypeId.Millimeters;
#else
		/// <summary>
		/// Display Unit Type
		/// </summary>
		protected DisplayUnitType m_UnitType = DisplayUnitType.DUT_MILLIMETERS;
#endif

		/// <summary>
		/// Initialize and prepare the tool
		/// </summary>
		public override void Initialize()
		{
			//  Initialize Connector controller
			if (!CheckFamily())
			{
				return;
			}
		}

		/// <summary>
		/// Process request handler from FrmConnectSettings
		/// </summary>
		public override bool ProcessRequest(ArchitexorRequestId reqId)
		{
			try
			{
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		#region Create necessary families
		/// <summary>
		/// Check if Joint Board family is already created
		/// </summary>
		private bool CheckFamily()
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;

			if (SOWB != null && !SOWB.IsValidObject) SOWB = null;
			if (SOWB != null)
			{
				return true;
			}

			FilteredElementCollector collector = new FilteredElementCollector(doc);
			ICollection<Element> collection = collector.OfClass(typeof(Family)).ToElements();

			foreach (Element e in collection)
			{
				Family f = e as Family;
				if (f.Name == SOWBeamFamilyName)
				{
					SOWB = f;
				}
			}

			if (SOWB != null)
			{
				return true;
			}
			else
			{
				Transaction trans = new(doc);
				trans.Start("Load Family");

				string url = Assembly.GetExecutingAssembly().Location;

				//	Get the url of the plugin
				url = url.Substring(0, url.LastIndexOf("\\")) + "\\" + SOWBeamFamilyName + ".rfa";
				bool bRet1 = doc.LoadFamily(url, out SOWB);
				if (bRet1)
				{
					trans.Commit();
				}
				else
				{
					trans.RollBack();
				}
				return bRet1;
			}
		}
		#endregion

		public class SOWDefaultParam
		{
			/// <summary>
			/// Below Boundary Length of SOW Beam
			/// </summary>
			public double MinLengthOfBeam { get; set; }

			/// <summary>
			/// Above Boundary Length of SOW Beam
			/// </summary>
			public double MaxLengthOfBeam { get; set; }

			/// <summary>
			/// Default Count of Trimmers
			/// </summary>
			public int Trimmers { get; set; }

			/// <summary>
			/// Default Count of Studs
			/// </summary>
			public int KingStuds { get; set; }

			public SOWDefaultParam(double fMinLength, double fMaxLength, int nTrimmers, int nKingStuds)
			{
				MinLengthOfBeam = fMinLength;
				MaxLengthOfBeam = fMaxLength;
				Trimmers = nTrimmers;
				KingStuds = nKingStuds;
			}
		}

		public class SOWInstance
		{
			public Document Doc;
			public ElementId Id;
			public int Flag = 0;
		}

		private static SOWInstance m_SOWInstance = new();

		public static void UpdateParameter(Document doc, FamilyInstance instance)
		{
			Parameter param;

			try
			{
				param = instance.LookupParameter("TrueLength");
				double fTrueLength = param.AsDouble();

				// Initialize the Default Parameters of SOW Beam
				SOWDefaultParams = new List<SOWDefaultParam>()
				{
					new SOWDefaultParam(0, 4, 1, 1),
					new SOWDefaultParam(4, 8, 2, 1),
					new SOWDefaultParam(8, 10, 3, 1),
					new SOWDefaultParam(10, 14, 3, 2)
				};

				SOWDefaultParam defaultParam = SOWDefaultParams.Where(x => x.MinLengthOfBeam < fTrueLength && x.MaxLengthOfBeam > fTrueLength).FirstOrDefault();

				if (defaultParam == null)
				{
					defaultParam = SOWDefaultParams.Last();
				}

				Transaction trans = new(doc);
				trans.Start("Update Parameters");

				param = instance.LookupParameter("TrimmerStudCount");
				param.Set(defaultParam.Trimmers);
				param = instance.LookupParameter("KingStudCount");
				param.Set(defaultParam.KingStuds);

				trans.Commit();
			}
			catch (Exception ex)
			{
				TaskDialog.Show("Error", ex.Message);
			}
		}

		public static bool DocChangedHandler(DocumentChangedEventArgs args)
		{
			try
			{
				Document doc = args.GetDocument();
				bool bHas = false;

				// first we check if the element was deleted
				ICollection<ElementId> elems = args.GetDeletedElementIds();

				if (elems.Count > 0)
				{
					m_SOWInstance = null;
				}

				elems = args.GetAddedElementIds();
				if (elems.Count > 0)
				{
					foreach (ElementId eId in elems)
					{
						Element e = doc.GetElement(eId);
						FamilyInstance ins = e as FamilyInstance;
						if (ins != null && ins.Symbol.FamilyName == SOWBeamFamilyName)
						{
							m_SOWInstance = new()
							{
								Doc = doc,
								Id = ins.Id,
								Flag = 2
							};
							bHas = true;

							break;
						}
					}
				}

				// we check if the element was edited
				//ICollection<ElementId> elemIds = args.GetModifiedElementIds();
				//if (elemIds.Count > 0)
				//{
				//  if(m_SOWInstance !=null)
				//    m_SOWInstance.Flag = 2;
				//  bHas = true;
				//}

				return bHas;
			}
			catch (Exception ex)
			{
				TaskDialog.Show("Error", ex.Message);
				return false;
			}
		}

		public static void DocOpenedHandler(DocumentOpenedEventArgs args)
		{

		}

		public static void OnIdlingEvent(object sender, IdlingEventArgs e)
		{
			UIApplication uiApp = sender as UIApplication;
			Document doc = uiApp.ActiveUIDocument.Document;

			TransactionGroup tg = new(doc);
			tg.Start("SOW Management");
			try
			{
				FamilyInstance ins = doc.GetElement(m_SOWInstance.Id) as FamilyInstance;

				if (m_SOWInstance.Flag == 2)
				{
					m_SOWInstance.Flag = 0;

					UpdateParameter(doc, ins);
				}

				tg.Assimilate();
			}
			catch (Exception ex)
			{
				tg.RollBack();
				TaskDialog.Show("Error", ex.Message);
			}

			//Application.thisApp.GetUIContApp().Idling -= OnIdlingEvent;
		}
	}
}
