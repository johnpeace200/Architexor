using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using Architexor.Request;
using Architexor.Utils;
using Architexor.Utils.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace Architexor.Controllers
{
	public class PropertyMarker : Controller
	{
		public const string FamilyName = "ATX_3D Marker";

		public static Family Marker = null;

		protected float m_fScale = 1.0f;
		public float Scale
		{
			get { return m_fScale; }
			set { m_fScale = value; }
		}

		protected int m_nThickness = 10;
		public int Thickness
		{
			get { return m_nThickness; }
			set { m_nThickness = value; }
		}

		protected bool m_bIncludeArrow = true;
		public bool IncludeArrow
		{
			get { return m_bIncludeArrow; }
			set { m_bIncludeArrow = value; }
		}

		protected bool m_bFlip = false;
		public bool Flip
		{
			get { return m_bFlip; }
			set { m_bFlip = value; }
		}

        //	Variables to use for internal functions
#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
        protected ForgeTypeId m_UnitType = UnitTypeId.Millimeters;
#else
		/// <summary>
		/// Display Unit Type
		/// </summary>
		protected DisplayUnitType m_UnitType = DisplayUnitType.DUT_MILLIMETERS;
#endif

		private List<Parameter> m_PublicParams = new List<Parameter>();
		public List<Parameter> PublicParams
		{
			get { return m_PublicParams; }
			set { m_PublicParams = value; }
		}

		private List<List<Parameter>> m_PrivateInstanceParams = new List<List<Parameter>>();
		public List<List<Parameter>> PrivateInstanceParams
		{
			get { return m_PrivateInstanceParams; }
			set { m_PrivateInstanceParams = value; }
		}

		private List<List<Parameter>> m_PrivateTypeParams = new List<List<Parameter>>();
		public List<List<Parameter>> PrivateTypeParams
		{
			get { return m_PrivateTypeParams; }
			set { m_PrivateTypeParams = value; }
		}

		private List<Parameter> m_SelectedTopParams = new List<Parameter>();
		public List<Parameter> SelectedTopParams
		{
			get { return m_SelectedTopParams; }
			set { m_SelectedTopParams = value; }
		}

		private List<Parameter> m_SelectedBottomParams = new List<Parameter>();
		public List<Parameter> SelectedBottomParams
		{
			get { return m_SelectedBottomParams; }
			set { m_SelectedBottomParams = value; }
		}

		private int m_nCurrentElement = 0;
		public int CurrentElement
		{
			get { return m_nCurrentElement; }
		}

		//	Members for Update feature
		private List<FamilyInstance> m_3DParameters = new List<FamilyInstance>();
		public List<FamilyInstance> PropertyMarkers
		{
			get { return m_3DParameters; }
			set { m_3DParameters = value; }
		}

		public override void Initialize()
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;

			//  Initialize Connector controller
			if (!CheckFamily())
			{
				return;
			}

			if (m_Elements.Count == 0)
				return;

			m_PublicParams.Clear();
			m_PrivateInstanceParams.Clear();
			m_PrivateTypeParams.Clear();

			int i, j, k;
			for(i = 0; i < m_Elements.Count; i++)
			{
				Element e = m_Elements[i];
				List<Parameter> paras = new List<Parameter>();
				foreach(Parameter param in e.Parameters)
				{
					InternalDefinition id = param.Definition as InternalDefinition;
					if (id != null
						&& id.BuiltInParameter == BuiltInParameter.ELEM_CATEGORY_PARAM_MT)
						continue;

					for(j = 0; j < paras.Count; j++)
					{
						if (paras[j].Id == param.Id)
							break;
					}
					if (j == paras.Count)
					{
						//if(param.HasValue)
						//if (param.Definition.Name == "Area")
						paras.Add(param);
					}
				}
				m_PrivateInstanceParams.Add(paras);

				paras = new List<Parameter>();
				Element elemType = null;
				if((elemType = doc.GetElement(m_Elements[i].GetTypeId())) != null)
				{
					foreach (Parameter param in elemType.Parameters)
					{
						InternalDefinition id = param.Definition as InternalDefinition;
						if (id != null
							&& id.BuiltInParameter == BuiltInParameter.ELEM_CATEGORY_PARAM_MT)
							continue;

						for (j = 0; j < paras.Count; j++)
						{
							if (paras[j].Id == param.Id)
								break;
						}
						if (j == paras.Count)
						{
							//if(param.HasValue)
							//if (param.Definition.Name == "Area")
							paras.Add(param);
						}
					}
					m_PrivateTypeParams.Add(paras);
				}
				else
				{
					m_PrivateTypeParams.Add(paras);
				}
			}
			for (j = 0; j < m_PrivateInstanceParams[0].Count; j++)
			{
				Parameter param = m_PrivateInstanceParams[0][j];
				for (i = 1; i < m_Elements.Count; i++)
				{
					bool bExist = false;
					for (k = 0; k < m_PrivateInstanceParams[i].Count; k++)
					{
						Parameter p = m_PrivateInstanceParams[i][k];
						if (p.Id == param.Id)
						{
							bExist = true;
							break;
						}
					}
					if (!bExist)
						break;
				}
				if (i == m_Elements.Count)
				{
					m_PublicParams.Add(param);
				}
			}
			for (j = 0; j < m_PrivateTypeParams[0].Count; j++)
			{
				Parameter param = m_PrivateTypeParams[0][j];
				for (i = 1; i < m_Elements.Count; i++)
				{
					bool bExist = false;
					for (k = 0; k < m_PrivateTypeParams[i].Count; k++)
					{
						Parameter p = m_PrivateTypeParams[i][k];
						if (p.Id == param.Id)
						{
							bExist = true;
							break;
						}
					}
					if (!bExist)
						break;
				}
				if (i == m_Elements.Count)
				{
					for(i = 0; i < m_PublicParams.Count; i++ )
					{
						if (m_PublicParams[i].Id == param.Id)
							break;
					}
					if(i == m_PublicParams.Count)
						m_PublicParams.Add(param);
				}
			}

			m_nCurrentElement = 0;
			Selection sel = m_uiApp.ActiveUIDocument.Selection;
			sel.SetElementIds(new List<ElementId>() {m_Elements[m_nCurrentElement].Id});
		}

		public void SelectNextElement()
		{
			m_nCurrentElement++;
			if (m_nCurrentElement >= m_Elements.Count)
				m_nCurrentElement = m_Elements.Count - 1;

			Selection sel = m_uiApp.ActiveUIDocument.Selection;
			sel.SetElementIds(new List<ElementId>() { m_Elements[m_nCurrentElement].Id });
		}

		public void SelectPreviousElement()
		{
			m_nCurrentElement--;
			if (m_nCurrentElement <= 0)
				m_nCurrentElement = 0;

			Selection sel = m_uiApp.ActiveUIDocument.Selection;
			sel.SetElementIds(new List<ElementId>() { m_Elements[m_nCurrentElement].Id });
		}

		public override bool ProcessRequest(ArchitexorRequestId reqId)
		{
			try
			{
				switch (reqId)
				{
					case ArchitexorRequestId.SelectPropertyElements:
						SelectElements();
						return false;
					case ArchitexorRequestId.SelectPropertyElementsToUpdate:
						Select3DParameters();
						return false;
					case ArchitexorRequestId.ArrangePropertyMarkers:
						if (Marker == null)
						{
							TaskDialog.Show("Error", "Can't find the family. Please contact developer.");
							return false;
						}

						{
							Document doc = m_uiApp.ActiveUIDocument.Document;
							TransactionGroup tg = new(doc);
							tg.Start("Generate Markers");
							foreach (Element e in m_Elements)
							{
								CreateInstance(e);
							}

							tg.Assimilate();
						}
						return true;
					case ArchitexorRequestId.SelectNextPropertyElement:
						SelectNextElement();
						return false;
					case ArchitexorRequestId.SelectPreviousPropertyElement:
						SelectPreviousElement();
						return false;
					case ArchitexorRequestId.UpdatePropertyMarkers:
						{
							Document doc = m_uiApp.ActiveUIDocument.Document;
							TransactionGroup tg = new(doc);
							tg.Start("Update Markers");
							foreach (FamilyInstance ins in m_3DParameters)
							{
								UpdateParameter(doc, ins);
							}
							tg.Assimilate();
						}
						break;
					default:
						break;
				}
				return true;
			}
			catch (Exception _)
			{
                TaskDialog.Show("Error", _.Message);
                return false;
			}
		}

		private void SelectElements()
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;
			Selection sel = m_uiApp.ActiveUIDocument.Selection;

			//  Select a wall
			try
			{
				GlobalHook.Subscribe();
				IList<Reference> refers = sel.PickObjects(ObjectType.Element, "Please select elements.");
				GlobalHook.Unsubscribe();

				if (refers.Count == 0)
					return;
				//	return Result.Cancelled;

				m_Elements.Clear();
				foreach (Reference refer in refers)
				{
					Element elem = doc.GetElement(refer);
					m_Elements.Add(elem);
				}

				Initialize();
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
			}
		}

		private void Select3DParameters()
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;
			Selection sel = m_uiApp.ActiveUIDocument.Selection;

			//  Select a wall
			try
			{
				GlobalHook.Subscribe();
				FamilyPickFilter selFilter = new(FamilyName);
				IList<Reference> refers = sel.PickObjects(ObjectType.Element, selFilter, "Please select elements.");
				GlobalHook.Unsubscribe();

				if (refers.Count == 0)
					return;
				//	return Result.Cancelled;

				m_Elements.Clear();
				m_3DParameters.Clear();
				foreach (Reference refer in refers)
				{
					FamilyInstance ins = doc.GetElement(refer) as FamilyInstance;
					m_3DParameters.Add(ins);

					Element elem = ins.Host;
					m_Elements.Add(elem);
				}

				Initialize();
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
			}
		}

		protected Face GetBaseFace(Element e, bool bTop = true)
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;

#if REVIT2024 || REVIT2025
            if (e is Wall
                || e.Category.Id.Value.Equals((int)BuiltInCategory.OST_Roofs)
                || e.Category.Id.Value.Equals((int)BuiltInCategory.OST_Floors))
#else
			if (e is Wall
				|| e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Roofs)
				|| e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Floors))
#endif
            {
				List<Face> faces = Util.GetBaseFacesOfElement(doc, e, bTop);
				Face largestFace = null;
				foreach (Face f in faces)
				{
					if (largestFace == null
						|| f.Area > largestFace.Area)
						largestFace = f;
				}
				return largestFace;
			}
			else if(e is Part)
			{
				Element source = Util.GetSourceElementOfPart(doc, e as Part);
#if REVIT2024 || REVIT2025
                if (source is Wall
                    || source.Category.Id.Value.Equals((int)BuiltInCategory.OST_Roofs)
                    || source.Category.Id.Value.Equals((int)BuiltInCategory.OST_Floors))
#else
                if (source is Wall
					|| source.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Roofs)
					|| source.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Floors))
#endif
                    return Util.GetBaseFaceOfPart(doc, e as Part, bTop);
			}

			//	We get the largest face
			{
				Util.GetFaceList(e, out List<PlanarFace> faceList);
				PlanarFace largestFace = null;
				foreach (PlanarFace f in faceList)
				{
					if (largestFace == null
						|| f.Area > largestFace.Area)
						largestFace = f;
				}
				return largestFace;
			}
		}

		protected static string GetParameterValues(Element e, string sParams)
		{
			string[] arrParams = sParams.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			ParameterSet paras = e.Parameters;
			//if (m_uiApp == null)
			//	return "No values";
			Document doc = e.Document;// m_uiApp.ActiveUIDocument.Document;
			Element type = doc.GetElement(e.GetTypeId());
			ParameterSet typeparas = type != null ? type.Parameters : null;
			string sText = "";
			try
			{
				for (int i = 0; i < arrParams.Length; i += 2)
				{
					bool bHas = false;
					foreach (Parameter p in paras)
                    {
#if REVIT2024 || REVIT2025
                        if (p.Id.Value == int.Parse(arrParams[i])
#else
                        if (p.Id.IntegerValue == int.Parse(arrParams[i])
#endif
                            && p.HasValue)
						{
							switch (p.StorageType)
							{
								case StorageType.String:
									sText += p.AsString() + "\r\n";
									break;
								default:
									sText += p.AsValueString() + "\r\n";
									break;
							}
							bHas = true;
							break;
						}
					}
					if (!bHas && typeparas != null)
					{
						foreach (Parameter p in typeparas)
                        {
#if REVIT2024 || REVIT2025
                            if (p.Id.Value == int.Parse(arrParams[i])
                                && p.HasValue)
#else
                            if (p.Id.IntegerValue == int.Parse(arrParams[i])
								&& p.HasValue)
#endif
                            {
                                switch (p.StorageType)
								{
									case StorageType.String:
										sText += p.AsString() + "\r\n";
										break;
									default:
										sText += p.AsValueString() + "\r\n";
										break;
								}
								bHas = true;
								break;
							}
						}
					}
				}
				if (string.IsNullOrWhiteSpace(sText))
					sText = "No values";
				return sText;
			}
			catch(Exception)
			{
				return "No values";
			}
		}

		protected string CreateInstance(Element e)
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;
			Transaction trans = new(doc);
			trans.Start("Creating Property Marker");
			FamilyInstance instance;

			double fThickness = UnitUtils.ConvertToInternalUnits(m_nThickness, m_UnitType);
			float fScale = m_fScale;

			try
			{
				//	Get the largest surface
				Face hostFace = GetBaseFace(e, !m_bFlip);

				if (hostFace == null)
				{
					trans.RollBack();
					return "Can not find the face.";
				}

				//	Calculate pos, Get the longest edge
				List<Curve> boundary = Util.GetOptimizedBoundaryCurves(hostFace);
				Curve longestEdge = null;
				double Umin = double.MaxValue
					, Umax = double.MinValue
					, Vmin = double.MaxValue
					, Vmax = double.MinValue;
				foreach(Curve c in boundary)
				{
					if (longestEdge == null || c.Length > longestEdge.Length)
						longestEdge = c;
				}
				foreach (EdgeArray ea in hostFace.EdgeLoops)
				{
					foreach (Edge edge in ea)
					{
						foreach (UV uv in edge.TessellateOnFace(hostFace))
						{
							Umin = Math.Min(Umin, uv.U);
							Umax = Math.Max(Umax, uv.U);
							Vmin = Math.Min(Vmin, uv.V);
							Vmax = Math.Max(Vmax, uv.V);
						}
					}
				}

				BoundingBoxXYZ bbxyz = e.get_BoundingBox(doc.ActiveView);
				XYZ max = bbxyz.Max, min = bbxyz.Min;

				XYZ pos = (hostFace.Evaluate(new UV(Umin, Vmin)) +
					hostFace.Evaluate(new UV(Umax, Vmax))) / 2;
				if (e is FamilyInstance
					&& (e as FamilyInstance).Location is LocationPoint)
					pos = ((e as FamilyInstance).Location as LocationPoint).Point;
				pos = (hostFace.GetSurface() as Plane).ProjectOnto((max + min) / 2);

				instance = doc.Create.NewFamilyInstance(
					hostFace
					, pos
					, longestEdge.GetEndPoint(1) - longestEdge.GetEndPoint(0)
					, GetFamilySymbol(Marker));

				ParameterSet paras = e.Parameters;
				string sParams = "";
				if (m_SelectedTopParams.Count == 0)
				{
					instance.LookupParameter("ATX_Show_Text_Top").Set(0);
				}
				else
				{
					foreach (Parameter p in m_SelectedTopParams)
                    {
#if REVIT2024 || REVIT2025
                        sParams += p.Id.Value.ToString() + "\r\n" + p.Definition.Name + "\r\n";
#else
                        sParams += p.Id.IntegerValue.ToString() + "\r\n" + p.Definition.Name + "\r\n";
#endif
                    }
                    instance.LookupParameter("ATX_GUID_Top").Set(sParams);
					instance.LookupParameter("ATX_Text_Top").Set(GetParameterValues(e, sParams));
				}

				if (m_SelectedBottomParams.Count == 0)
				{
					instance.LookupParameter("ATX_Show_Text_Bottom").Set(0);
				}
				else
				{
					sParams = "";
					foreach (Parameter p in m_SelectedBottomParams)
                    {
#if REVIT2024 || REVIT2025
                        sParams += p.Id.Value.ToString() + "\r\n" + p.Definition.Name + "\r\n";
#else
                        sParams += p.Id.IntegerValue.ToString() + "\r\n" + p.Definition.Name + "\r\n";
#endif
                    }
                    instance.LookupParameter("ATX_GUID_Bottom").Set(sParams);
					instance.LookupParameter("ATX_Text_Bottom").Set(GetParameterValues(e, sParams));
				}

				instance.LookupParameter("Text_Thickness").Set(fThickness);
				instance.LookupParameter("Arrow_Thickness").Set(fThickness);
				instance.LookupParameter("ATX_Structural Span Direction Arrow Scale").Set(fScale);
				instance.LookupParameter("ATX_Show_Structural Span Direction Arrow")?.Set(m_bIncludeArrow ? 1 : 0);
				trans.Commit();
			}
			catch(Exception ex)
			{
				trans.RollBack();
				return ex.Message;
			}
			return "";
		}

		private FamilySymbol GetFamilySymbol(Family family)
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;
			ISet<ElementId> familySymbolIds = family.GetFamilySymbolIds();
			
			foreach(ElementId eId in familySymbolIds)
			{
				FamilySymbol familySymbol = doc.GetElement(eId) as FamilySymbol;
				if (!familySymbol.IsActive)
					familySymbol.Activate();
				return familySymbol;
			}
			return null;
		}

		private bool CheckFamily()
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;
			Marker = null;

			List<Family> families = new(
				new FilteredElementCollector(doc)
					.WhereElementIsNotElementType()
					.OfClass(typeof(Family))
					.Where(ins => ins.Name == FamilyName)
					.ToList()
					.Cast<Family>()
					);

			if(families.Count > 0)
			{
				Marker = families[0];
				return true;
			}

			Transaction trans = new(doc);
			trans.Start("Load 3D Marker Family");
			try
			{
				string url = Assembly.GetExecutingAssembly().Location;
				url = url.Substring(0, url.LastIndexOf("\\")) + "\\";

				bool bRet = Marker != null || doc.LoadFamily(url + FamilyName + ".rfa", out Marker);
				if(bRet)
				{
					trans.Commit();
					return true;
				}

				trans.RollBack();
				return false;
			}
			catch(Exception)
			{
				trans.RollBack();
				MessageBox.Show("Can not find the family. Please contact the developer", "Error");
				return false;
			}
		}

		public void UpdateParameter(
			Document doc,
			FamilyInstance instance
			)
		{
			double fThickness = UnitUtils.ConvertToInternalUnits(m_nThickness, m_UnitType);
			float fScale = m_fScale;

			Transaction trans = new(doc);
			trans.Start("Update Marker Parameters");
			
			Element host = instance.Host;
			Face hostFace = host.GetGeometryObjectFromReference(instance.HostFace) as Face;
			Face newHostFace = null;

			//	Flip if needed
			if(m_bFlip)
            {
#if REVIT2024 || REVIT2025
                if (host is Wall
                || host.Category.Id.Value.Equals((int)BuiltInCategory.OST_Roofs)
                || host.Category.Id.Value.Equals((int)BuiltInCategory.OST_Floors))
#else
                if (host is Wall
				|| host.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Roofs)
				|| host.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Floors))
#endif
                {
                    List<Face> faces = Util.GetBaseFacesOfElement(doc, host, true);
					Face largestFace = null;
					foreach (Face f in faces)
					{
						if (largestFace == null
							|| f.Area > largestFace.Area)
							largestFace = f;
					}

					if(Util.IsEqual(hostFace, largestFace))
					{
						faces = Util.GetBaseFacesOfElement(doc, host, false);
						largestFace = null;
						foreach (Face f in faces)
						{
							if (largestFace == null
								|| f.Area > largestFace.Area)
								largestFace = f;
						}
						newHostFace = largestFace;
					}
					else
					{
						newHostFace = largestFace;
					}
				}
				else if (host is Part)
				{
					Element source = Util.GetSourceElementOfPart(doc, host as Part);
#if REVIT2024 || REVIT2025
                    if (source is Wall
                        || source.Category.Id.Value.Equals((int)BuiltInCategory.OST_Roofs)
                        || source.Category.Id.Value.Equals((int)BuiltInCategory.OST_Floors))
#else
                    if (source is Wall
						|| source.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Roofs)
						|| source.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Floors))
#endif
                    {
                        Face largestFace = Util.GetBaseFaceOfPart(doc, host as Part, true);
						if(Util.IsEqual(hostFace, largestFace))
						{
							newHostFace = Util.GetBaseFaceOfPart(doc, host as Part, false);
						}
						else
						{
							newHostFace = largestFace;
						}
					}
				}

				if(newHostFace != null)
				{
					XYZ pos = (newHostFace.GetSurface() as Plane).ProjectOnto((instance.Location as LocationPoint).Point);
					XYZ direction = instance.HandOrientation;

					doc.Delete(instance.Id);

					instance = doc.Create.NewFamilyInstance(
						newHostFace
						, pos
						, direction
						, GetFamilySymbol(Marker));
				}
			}

			string sParams = "";
			if (m_SelectedTopParams.Count == 0)
			{
				instance.LookupParameter("ATX_Show_Text_Top")?.Set(0);
			}
			else
			{
				foreach (Parameter p in m_SelectedTopParams)
                {
#if REVIT2024 || REVIT2025
                    sParams += p.Id.Value.ToString() + "\r\n" + p.Definition.Name + "\r\n";
#else
                    sParams += p.Id.IntegerValue.ToString() + "\r\n" + p.Definition.Name + "\r\n";
#endif
                }
                instance.LookupParameter("ATX_GUID_Top")?.Set(sParams);
				instance.LookupParameter("ATX_Text_Top")?.Set(GetParameterValues(host, sParams));
			}

			if (m_SelectedBottomParams.Count == 0)
			{
				instance.LookupParameter("ATX_Show_Text_Bottom")?.Set(0);
			}
			else
			{
				sParams = "";
				foreach (Parameter p in m_SelectedBottomParams)
                {
#if REVIT2024 || REVIT2025
                    sParams += p.Id.Value.ToString() + "\r\n" + p.Definition.Name + "\r\n";
#else
                    sParams += p.Id.IntegerValue.ToString() + "\r\n" + p.Definition.Name + "\r\n";
#endif
                }
                instance.LookupParameter("ATX_GUID_Bottom")?.Set(sParams);
				instance.LookupParameter("ATX_Text_Bottom")?.Set(GetParameterValues(host, sParams));
			}

			instance.LookupParameter("Text_Thickness")?.Set(fThickness);
			instance.LookupParameter("Arrow_Thickness")?.Set(fThickness);
			instance.LookupParameter("ATX_Structural Span Direction Arrow Scale")?.Set(fScale);
			instance.LookupParameter("ATX_Show_Structural Span Direction Arrow")?.Set(m_bIncludeArrow ? 1 : 0);

			trans.Commit();
		}

		private static List<FamilyInstance> m_MarkersToChange = new List<FamilyInstance>();
		public static bool DocChangedHandler(DocumentChangedEventArgs args)
		{
			Document doc = args.GetDocument();

			//	Get All Marker Instances
			List<FamilyInstance> instances = new(
				new FilteredElementCollector(doc)
					.WhereElementIsNotElementType()
					.OfClass(typeof(FamilyInstance))
					.Where(ins => (ins as FamilyInstance).Symbol.Family.Name == FamilyName)
					.ToList()
					.Cast<FamilyInstance>()
					);

			List<ElementId> deleted = (List<ElementId>)args.GetDeletedElementIds();
			List<ElementId> modified = (List<ElementId>)args.GetModifiedElementIds();
			List<string> transactions = (List<string>)args.GetTransactionNames();
			if (transactions[0] == "ATX_3D Marker Management")
				return false;
			for (int i = 0; i < instances.Count; i++)
			{
				if(instances[i].HostFace == null)
				{
					m_MarkersToChange.Add(instances[i]);
					continue;
				}
				if (modified.Find(x => x == instances[i].Id) != null)
				{
					m_MarkersToChange.Add(instances[i]);
					continue;
				}
				if (deleted.Find(x => x == instances[i].HostFace.ElementId) != null)
				{
					m_MarkersToChange.Add(instances[i]);
					continue;
				}
				if (modified.Find(x => x == instances[i].HostFace.ElementId) != null)
				{
					m_MarkersToChange.Add(instances[i]);
				}
			}

			if (m_MarkersToChange.Count > 0)
				return true;
			//Application.thisApp.GetUIContApp().Idling += OnIdlingEvent;

			return false;
		}

		public static void DocOpenedHandler(DocumentOpenedEventArgs args)
		{
			Document doc = args.Document;
		}

		public static void OnIdlingEvent(object sender, IdlingEventArgs e)
		{
			UIApplication uiApp = sender as UIApplication;
			Document doc = uiApp.ActiveUIDocument.Document;

			Transaction trans = new Transaction(doc);
			trans.Start("ATX_3D Marker Management");
			for(int i = 0; i < m_MarkersToChange.Count; i++)
			{
				FamilyInstance instance = m_MarkersToChange[i];
				if (!instance.IsValidObject)
					continue;
				if(instance.HostFace == null)
				{
					doc.Delete(instance.Id);
					continue;
				}

				Element host = doc.GetElement(instance.HostFace.ElementId);
				if(host == null)
				{
					doc.Delete(instance.Id);
				}
				else
				{
					string sParams = instance.LookupParameter("ATX_GUID_Top").AsString();
					string sText = GetParameterValues(host, sParams);
					instance.LookupParameter("ATX_Text_Top").Set(sText);

					sParams = instance.LookupParameter("ATX_GUID_Bottom").AsString();
					sText = GetParameterValues(host, sParams);
					instance.LookupParameter("ATX_Text_Bottom").Set(sText);

					//	Get the orientation
					Reference refer = instance.HostFace;
					Element elem = instance.Host;
					Face f = elem.GetGeometryObjectFromReference(refer) as Face;
					List<Curve> boundary = Util.GetOptimizedBoundaryCurves(f);

					//	Get the longest and shortest curve
					Curve longest = null, shortest = null;
					foreach(Curve c in boundary)
					{
						if (longest == null || c.Length > longest.Length)
							longest = c;
						if (shortest == null || c.Length < shortest.Length)
							shortest = c;
					}

					if (Util.IsEqual(instance.HandOrientation
						, (longest.GetEndPoint(1) - longest.GetEndPoint(0)).Normalize())
						|| Util.IsEqual(instance.HandOrientation
						, (longest.GetEndPoint(1) - longest.GetEndPoint(0)).Normalize().Negate()))
					{
						instance.LookupParameter("ATX_Structural Span Direction")?.Set("Long");
					}
					else if (Util.IsEqual(instance.HandOrientation
						, (shortest.GetEndPoint(1) - shortest.GetEndPoint(0)).Normalize())
						|| Util.IsEqual(instance.HandOrientation
						, (shortest.GetEndPoint(1) - shortest.GetEndPoint(0)).Normalize().Negate()))
					{
						instance.LookupParameter("ATX_Structural Span Direction")?.Set("Short");
					}
					else
					{
						instance.LookupParameter("ATX_Structural Span Direction")?.Set("Skew");
					}

					//doc.GetElement(refer)
				}
			}
			m_MarkersToChange.Clear();
			trans.Commit();
		}
	}
}
