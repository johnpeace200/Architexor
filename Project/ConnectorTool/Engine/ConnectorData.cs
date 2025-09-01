using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ConnectorTool.Information;
using ConnectorTool.Object;
using Architexor.Models.ConnectorTool;
using Architexor.Utils;
using RevitElement = Autodesk.Revit.DB.Element;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace ConnectorTool.Engine
{
	/// <summary>
	/// data class contains information to create framing
	/// </summary>
	public class ConnectorData
	{
		#region Fields
		/// <summary>
		/// object manage all connection types
		/// </summary>
		private readonly TypeManager m_conTypeMgr = new TypeManager();

		/// <summary>
		/// Fixing candidate list
		/// </summary>
		private readonly List<FixingCandidate> m_FixingCandidates = new List<FixingCandidate>();

		/// <summary>
		/// List of Basic Information of Connection
		/// </summary>
		private readonly List<TConElemInfo> m_conElemInfoList = new List<TConElemInfo>();

		/// <summary>
		/// List of Connection
		/// </summary>
		private List<TConInfo> m_conInfoList = new List<TConInfo>();

		/// <summary>
		/// connection's type
		/// </summary>
		private FamilySymbol m_conSymbol;

		/// <summary>
		/// The List of Connector
		/// </summary>
		readonly List<TConnector> m_conList = new List<TConnector>();

		#endregion

		#region Properties

		/// <summary>
		/// object manage all connection types
		/// </summary>
		public TypeManager ConnectionTypeMgr { get => m_conTypeMgr; }

		public List<FixingCandidate> FixingCandidatesList { get => m_FixingCandidates; }

		/// <summary>
		/// connection's family
		/// </summary>
		public Family ConnectionFamily { get; }

		/// <summary>
		/// connection's type
		/// </summary>
		public FamilySymbol ConnectionSymbol { get => m_conSymbol; }

		/// <summary>
		/// List of ConnectionInstance
		/// </summary>
		public List<TConInfo> ConInfoList
		{
			get
			{
				return m_conInfoList;
			}
			set
			{
				m_conInfoList = value;
				m_conList.Clear();
				foreach (TConInfo conInfo in ConInfoList)
				{
					TConnector connector = new TConnector(conInfo);
					m_conList.Add(connector);
				}
			}
		}

		/// <summary>
		/// List of ConnectionInstance
		/// </summary>
		public List<TConElemInfo> ConElemInfoList { get => m_conElemInfoList; }

		/// <summary>
		/// Determine the Wake of Main Form
		/// </summary>
		public bool CanWakeUp { get; set; } = true;

		#endregion

		#region Methods
		/// <summary>
		/// it is only used for object factory method
		/// </summary>
		public ConnectorData()
		{
			//	Read Families
			Initialize();

			if (m_conTypeMgr.Symbols.Count > 0)// && m_boltList.Count > 0 && m_screwList.Count > 0 && m_dowelList.Count > 0)
			{
				m_conSymbol = m_conTypeMgr.Symbols[0];
				ConnectionFamily = m_conTypeMgr.Families[0];
			}
			else
			{
				//	If families not exist, we don't run the tool.
				CanWakeUp = false;
				TaskDialog.Show("Error", "Can not find families. Please contact developer.");
				return;
			}
		}

		/// <summary>
		/// cast object to FamilySymbol and set as connection's type
		/// </summary>
		/// <param name="symbol">FamilySymbol object</param>
		/// <returns></returns>
		public void SetConnectionSymbol(FamilySymbol symbol)
		{
			m_conSymbol = symbol;
		}

		/// <summary>
		/// initialize list of connection and void models
		/// </summary>
		private void Initialize()
		{
			Document doc = Architexor.ConnectorTool.Application.GetUiApplication().ActiveUIDocument.Document;

			Categories categories = doc.Settings.Categories;
			// Get the FamilySymbols of Structural Connection
			BuiltInCategory bipconnection = BuiltInCategory.OST_StructConnections;
			ElementId idconnection = categories.get_Item(bipconnection).Id;
			ElementCategoryFilter filterConnection = new ElementCategoryFilter(bipconnection);
			ElementClassFilter filterSymbol = new ElementClassFilter(typeof(FamilySymbol));
			LogicalAndFilter filterConnectionSymbol = new LogicalAndFilter(filterConnection, filterSymbol);
			FilteredElementCollector connectionCollector = new FilteredElementCollector(doc);
			IList<RevitElement> connectionList = connectionCollector.WherePasses(filterConnectionSymbol).ToElements();
			foreach (FamilySymbol symbol in connectionList)
			{
				ElementId categoryId = symbol.Category.Id;
				string omniTitle = symbol.get_Parameter(BuiltInParameter.OMNICLASS_DESCRIPTION).AsString();
				if (idconnection.Equals(categoryId))
				{
					switch (omniTitle)
					{
						case "Wood Connectors":
							m_conTypeMgr.AddSymbol(symbol);
							break;
						case "Bolts and Nuts":
							AddFixing(ConnectionType.Bolt, symbol);
							break;
						case "Screws":
							AddFixing(ConnectionType.Screw, symbol);
							break;
						case "Staples":
							AddFixing(ConnectionType.Dowel, symbol);
							break;
						default:
							break;
					}
				}
			}
		}

		private void AddFixing(ConnectionType ct, FamilySymbol symbol)
		{
			if(m_FixingCandidates.Find(x => x.Name == symbol.Name) != null)
			{
				return;
			}

			FixingCandidate fc = new FixingCandidate()
			{
				Name = symbol.Name,
				ConnectionType = ct,
				Symbol = symbol
			};

			double diameter = 0, length = 0;
			ParameterSet paramset = symbol.Parameters;
			foreach (Parameter param in paramset)
			{
				if (param.StorageType == StorageType.Double)
				{
					switch (param.Definition.Name)
					{
						case "Diameter":
							diameter = Util.IUToMm(param.AsDouble());
							break;
						case "Length":
							length = Util.IUToMm(param.AsDouble());
							break;
						default:
							break;
					}
				}
			}

			fc.Diameter = diameter;
			fc.Length = length;
			m_FixingCandidates.Add(fc);

			/*Family family = symbol.Family;
			Document doc = Application.GetUiApplication().ActiveUIDocument.Document;
			if (m_FixingCandidates.Find(x => x.FamilyName == family.Name) == null)
			{
				FixingCandidate candidate = new FixingCandidate()
				{
					FamilyName = family.Name
				};
				foreach(Parameter p in symbol.Parameters)
				{
					candidate.TypeParameters.Add(p.Definition.Name);
				}
				foreach(ElementId eId in family.GetFamilySymbolIds())
				{
					FamilySymbol sym = doc.GetElement(eId) as FamilySymbol;
					candidate.Types.Add(sym.Name);
				}

				List<FamilyInstance> instances = new(
				new FilteredElementCollector(doc)
					.WhereElementIsNotElementType()
					.OfClass(typeof(FamilyInstance))
					.Where(x => (x as FamilyInstance).Symbol.Id == symbol.Id)
					.ToList()
					.Cast<FamilyInstance>()
					);
				if (instances.Count > 0)
				{
					foreach (Parameter p in instances[0].Parameters)
					{
						candidate.InstanceParameters.Add(p.Definition.Name);
					}
				}
				else {
					PlanarFace face = null;
					List<FamilyInstance> totalElements = new(
						new FilteredElementCollector(doc)
							.WhereElementIsNotElementType()
							.OfClass(typeof(FamilyInstance))
							.ToList()
							.Cast<FamilyInstance>()
							);
					GeometryElement ge = null;
					foreach (Element e in totalElements) {
						ge = e.get_Geometry(new Options()
						{
							ComputeReferences = true
						});
						if (ge != null) break;
					}
					
					foreach (GeometryObject geomObj in ge)
					{
						if (geomObj is Solid)
						{
							foreach (Face f in (geomObj as Solid).Faces)
							{
								face = f as PlanarFace;
								break;
							}
						}
						if (face != null)
							break;
					}

					Transaction t = new Transaction(doc);
					t.Start("Temporary");
					try
					{
						if (!symbol.IsActive)
							symbol.Activate();

						XYZ direction = new XYZ(0, 0, 1);
						if (Architexor.Utils.Util.IsEqual(direction, face.FaceNormal))
							direction = new XYZ(1, 0, 0);
						FamilyInstance instance = doc.Create.NewFamilyInstance(
							face
							, face.Origin
							, direction
							, symbol);
						foreach (Parameter p in instance.Parameters)
						{
							candidate.InstanceParameters.Add(p.Definition.Name);
						}
						t.RollBack();
					}
					catch (Exception ex)
					{
						t.RollBack();
					}
				}
				m_FixingCandidates.Add(candidate);
			}*/
		}

		/// <summary>
		/// Get Basic Information of Connector
		/// <paramref name="elems"/> The Selected Elements</para>
		/// </summary>
		/// <returns></returns>
		public bool GetConElemInfoList(List<RevitElement> elems)
		{
			Document doc = Architexor.ConnectorTool.Application.GetUiApplication().ActiveUIDocument.Document;

			List<RevitElement> priElementList = new List<RevitElement>();
			List<RevitElement> secElementList = new List<RevitElement>();
			foreach (RevitElement element in elems)
			{
				//	Primary Element can be Column, Wall and Beam
				if (element.Category.Name == "Structural Columns" || element.Category.Name == "Walls" || element.Category.Name == "Structural Framing")
				{
					if(element.Category.Name == "Structural Columns")
					{
						//	Change Column Style
						Parameter p = element.get_Parameter(BuiltInParameter.SLANTED_COLUMN_TYPE_PARAM);
						if(p.AsInteger() == (int)SlantedOrVerticalColumnType.CT_Vertical)
						{
							Transaction trans = new Transaction(doc);
							trans.Start("Change column style");
							p.Set((int)SlantedOrVerticalColumnType.CT_EndPoint);
							trans.Commit();
							trans.Start("Change column style");
							p.Set((int)SlantedOrVerticalColumnType.CT_Vertical);
							trans.Commit();
						}
					}
					priElementList.Add(element);
				}
				//	Secondary Element can be Beam
				if (element.Category.Name == "Structural Framing")
				{
					secElementList.Add(element);
				}
				else
					continue;
			}
			if (priElementList.Count > 0 && secElementList.Count > 0)
			{
				foreach (RevitElement priElement in priElementList)
				{
					foreach (RevitElement secElement in secElementList)
					{
						if (secElement.Id == priElement.Id)
						{
							continue;
						}

						LocationCurve locationCurve = secElement.Location as LocationCurve;
						Curve curve = locationCurve.Curve;
						SetComparisonResult CompareResult = SetComparisonResult.BothEmpty;
						if (Util.GetFaceList(priElement, out List<PlanarFace> faceList))
						{
							foreach (PlanarFace face in faceList)
							{
								CompareResult = face.Intersect(curve);
								List<Curve> boundary = Architexor.Utils.Util.GetOptimizedBoundaryCurves(face);
								if (CompareResult == SetComparisonResult.Overlap)
								{
									TConElemInfo conElemInfo = new TConElemInfo()
									{
										PrimaryElement = priElement,
										SecondaryElement = secElement,
										HostFace = face
									};
									if (conElemInfo.GetPrimaryElementDimension()
										&& conElemInfo.GetSecondaryElementDimension())
									{
										conElemInfo.Secondary_Element_Length = curve.Length;
										m_conElemInfoList.Add(conElemInfo);
									}
									break;
								}
							}
						}
					}
				}
				return true;
			}
			return false;
		}

		public List<TConnector> GetConnectorList()
		{
			return m_conList;
		}
		#endregion
	}
}