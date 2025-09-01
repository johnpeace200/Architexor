using System.Collections.Generic;
using ConnectorTool.Base;
using ConnectorTool.Engine;
using ConnectorTool.Location;
using ConnectorTool.Information;
using Autodesk.Revit.DB;
using Architexor.Models.ConnectorTool;
using System.Linq;
using Architexor.Utils;

namespace ConnectorTool.Object
{
	public class TConVoidObj : TObject
	{
		#region Fields
		#endregion

		/// <summary>
		/// The new host face in case of recessing into main element
		/// </summary>
		public Face NewHostFace
		{
			get
			{
				return GetNewHostFace();
			}
		}

		protected TConVoidObj()
		{

		}

		public TConVoidObj(TConnector connector)
		{
			m_Connector = connector;

			Initialize();
		}

		protected override void Initialize()
		{

		}

		protected override bool Validate()
		{
			return true;
		}

		public override bool CreateInstance()
		{
			TConLocation conLoc = m_Connector.ConLocation;
			TConElemInfo elemInfo = m_Connector.ConInfo.ConElemInfo;
			Document doc = Architexor.ConnectorTool.Application.GetUiApplication().ActiveUIDocument.Document;

			if(elemInfo.HostFace.Reference == null)
			{
				//	If the tool has lost the reference, it decides the Host Face again
				LocationCurve locationCurve = elemInfo.SecondaryElement.Location as LocationCurve;
				Curve curve = locationCurve.Curve;
				SetComparisonResult CompareResult = SetComparisonResult.BothEmpty;
				if (Util.GetFaceList(elemInfo.PrimaryElement, out List<PlanarFace> faceList))
				{
					foreach (PlanarFace face in faceList)
					{
						CompareResult = face.Intersect(curve);
						if (CompareResult == SetComparisonResult.Overlap)
						{
							elemInfo.HostFace = face;
						}
					}
				}
			}
			Instance = doc.Create.NewFamilyInstance(
				elemInfo.HostFace
				, conLoc.DatumPos
				, conLoc.ConDirection
				, m_Connector.ConInfo.ConSymbol);
			SetInstanceParams(Instance);
			doc.Regenerate();

			if (InstanceVoidCutUtils.CanBeCutWithVoid(elemInfo.PrimaryElement))
				InstanceVoidCutUtils.AddInstanceVoidCut(doc, elemInfo.PrimaryElement, doc.GetElement(Instance.Id));
			if (InstanceVoidCutUtils.CanBeCutWithVoid(elemInfo.SecondaryElement))
				InstanceVoidCutUtils.AddInstanceVoidCut(doc, elemInfo.SecondaryElement, doc.GetElement(Instance.Id));
			return true;
		}

		/// <summary>
		/// In case of Recessing into main element we reselect the host face of connector
		/// So we use the face of cutting element.
		/// </summary>
		/// <returns></returns>
		public Face GetNewHostFace()
		{
			TConLocation conLoc = m_Connector.ConLocation;
			Element host = m_Connector.ConInfo.ConElemInfo.PrimaryElement;

			Face hostFace = null;
			Document doc = Architexor.ConnectorTool.Application.GetUiApplication().ActiveUIDocument.Document;
			if (Util.GetFaceList(host, out List<PlanarFace> faceList))
			{
				foreach (PlanarFace geomFace in faceList)
				{
					if(host.GetGeneratingElementIds(geomFace).Any(x => Instance.Id == x)
						&& Architexor.Utils.Util.IsEqual((geomFace as PlanarFace).FaceNormal, conLoc.HostFaceNormal))
					{
						hostFace = geomFace;
					}
				}
			}
			if (hostFace == null)
			{
				//	Get the original host face
				//	Decide the Host Face again
				LocationCurve locationCurve = m_Connector.ConInfo.ConElemInfo.SecondaryElement.Location as LocationCurve;
				Curve curve = locationCurve.Curve;
				SetComparisonResult CompareResult = SetComparisonResult.BothEmpty;
				if (Util.GetFaceList(host, out faceList))
				{
					foreach (PlanarFace face in faceList)
					{
						CompareResult = face.Intersect(curve);
						if (CompareResult == SetComparisonResult.Overlap)
						{
							hostFace = face;
						}
					}
				}
			}
				
			return hostFace;
		}

		/// <summary>
		/// Set the parameters of instance
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		private bool SetInstanceParams(FamilyInstance instance)
		{
			TConLocation conLoc = m_Connector.ConLocation;
			TConElemInfo elemInfo = m_Connector.ConInfo.ConElemInfo;
			TConTypeParam conType = m_Connector.ConInfo.ConTypeParam;
			TConPositionParam conPos = m_Connector.ConInfo.ConPositionParam;
			TConFinFCParam conFinFC = m_Connector.ConInfo.ConFinFCParam;
			TConFixingParam conFixing = m_Connector.ConInfo.ConFixingParam;

			bool bSetResult = false;
			ParameterSet paramList = instance.Parameters;

			foreach (Parameter para in paramList)
			{
				switch (para.Definition.Name)
				{
					//case "Geometry_TConnector_Height":
					//	bSetResult = para.Set(Util.MmToIU(conType.Geometry_TConnector_Height));
					//	break;
					//case "Geometry_TConnector_Width":
					//	bSetResult = para.Set(Util.MmToIU(conType.Geometry_TConnector_Width));
					//	break;
					//case "Geometry_TConnector_BackPlate_Thk":
					//	bSetResult = para.Set(Util.MmToIU(conType.Geometry_TConnector_BackPlate_Thk));
					//	break;
					//case "Geometry_TConnector_Depth":
					//	bSetResult = para.Set(Util.MmToIU(conType.Geometry_TConnector_Depth));
					//	break;
					//case "Geometry_TConnector_FinPlate_Thk":
					//	bSetResult = para.Set(Util.MmToIU(conType.Geometry_TConnector_FinPlate_Thk));
					//	break;
					case "Gap_BackPlate_RecessDepth":
						if (conPos.RecessCase == RecessCase.Primary)
							bSetResult = para.Set(Util.MmToIU(conPos.Gap_BackPlate_RecessDepth));
						else
							bSetResult = para.Set(0);
						break;
					case "Gap_BackPlate_Top":
						bSetResult = para.Set(Util.MmToIU(conPos.Gap_BackPlate_Top));
						break;
					case "Gap_BackPlate_Bottom":
						bSetResult = para.Set(Util.MmToIU(conPos.Gap_BackPlate_Bottom));
						break;
					case "Gap_BackPlate_SideA":
						bSetResult = para.Set(Util.MmToIU(conPos.Gap_BackPlate_SideA));
						break;
					case "Gap_BackPlate_SideB":
						bSetResult = para.Set(Util.MmToIU(conPos.Gap_BackPlate_SideB));
						break;
					case "Gap_BackPlate_Front":
						if (conPos.RecessCase == RecessCase.Secondary)
							bSetResult = para.Set(Util.MmToIU(conPos.Gap_BackPlate_Front));
						else
							bSetResult = para.Set(Util.MmToIU(conPos.Gap_BackPlate_RecessDepth - conType.Geometry_TConnector_BackPlate_Thk));
						break;
					case "Gap_FinPlate_Front":
						bSetResult = para.Set(Util.MmToIU(conFinFC.Gap_FinPlate_Front));
						break;
					case "Gap_FinPlate_Top":
						bSetResult = para.Set(Util.MmToIU(conFinFC.Gap_FinPlate_Top));
						break;
					case "Gap_FinPlate_Bottom":
						bSetResult = para.Set(Util.MmToIU(conFinFC.Gap_FinPlate_Bottom));
						break;
					case "Gap_FinPlate_Sides":
						bSetResult = para.Set(Util.MmToIU(conFinFC.Gap_FinPlate_Sides));
						break;
					case "Gap_Fillet_Depth":
						if(conFinFC.BFilletNotch)
							bSetResult = para.Set(Util.MmToIU(conFinFC.Gap_Fillet_Depth));
						else
							bSetResult = para.Set(0.0);
						break;
					case "Gap_Fillet_Width":
						bSetResult = para.Set(Util.MmToIU(conFinFC.Gap_Fillet_Width));
						break;
					case "Gap_Fillet_Top":
						bSetResult = para.Set(Util.MmToIU(conFinFC.Gap_Fillet_Top));
						break;
					case "Gap_Fillet_Bottom":
						bSetResult = para.Set(Util.MmToIU(conFinFC.Gap_Fillet_Bottom));
						break;
					case "Gap_CoverBoard_Depth":
						if (conFinFC.BCoverPlate)
							bSetResult = para.Set(Util.MmToIU(conFinFC.Gap_CoverBoard_Depth));
						else
							bSetResult = para.Set(0.0);
						break;
					case "Gap_CoverBoard_Width":
						bSetResult = para.Set(Util.MmToIU(conFinFC.Gap_CoverBoard_Width));
						break;
					case "Gap_CoverBoard_Length":
						bSetResult = para.Set(Util.MmToIU(conFinFC.Gap_CoverBoard_Length));
						break;
					case "Gap_MainSecondary_Between":
						bSetResult = para.Set(Util.MmToIU(conPos.Gap_MainSecondary_Between));
						break;
					case "Secondary_Width":
						bSetResult = para.Set(Util.MmToIU(elemInfo.Secondary_Element_Width));
						break;
					case "Secondary_Height":
						bSetResult = para.Set(Util.MmToIU(elemInfo.Secondary_Element_Height));
						break;
					case "Main_Depth":
						bSetResult = para.Set(Util.MmToIU(elemInfo.Primary_Element_Depth));
						break;
					case "Main_Plugging_Depth":
						if (conFixing.PriPlug != null)
							bSetResult = para.Set(Util.MmToIU(conFixing.PriPlug.depth));
						else
							bSetResult = para.Set(0);
						break;
					case "Main_Plugging_Diameter":
						if (conFixing.PriPlug != null)
							bSetResult = para.Set(Util.MmToIU(conFixing.PriPlug.diameter));
						break;
					case "Main_Fixing_Length":
						if (conFixing.PriFixing != null)
							bSetResult = para.Set(Util.MmToIU(conFixing.PriFixing.Length));
						break;
					case "Main_Hole_Diameter":
						if (conFixing.BPriBoltHole)
							bSetResult = para.Set(Util.MmToIU(conFixing.PriBoltHoleDiameter));
						else
							bSetResult = para.Set(0);
						break;
					case "Main_Fixing":
						if (conFixing.PriFixing != null)
						{
							FamilySymbol symbol = conFixing.PriFixing.Symbol as FamilySymbol;
							para.Set(symbol.Id);
						}
						break;
					case "Secondary_Fixing":
						if (conFixing.SecFixing != null)
						{
							FamilySymbol symbol = conFixing.SecFixing.Symbol as FamilySymbol;
							para.Set(symbol.Id);
						}
						break;
					case "Secondary_Fixing_Length_SideA":
						//if(conFixing.SecFixing)
						//	TODO
						//bSetResult = para.Set(Util.MmToIU(conFixing.PriFixing.Length));
						break;
					case "Secondary_Fixing_Length_SideB":
						//	TODO
						//bSetResult = para.Set(Util.MmToIU(conFixing.PriFixing.Length));
						break;
					case "Secondary_Plugging_Diameter":
						if (conFixing.SecPlug != null)
							bSetResult = para.Set(Util.MmToIU(conFixing.SecPlug.diameter));
						break;
					case "Secondary_Plugging_Depth_SideA":
						if (conFixing.SecPlug != null &&
							(conFixing.SecPlugSideType == PluggingSideType.SideA || conFixing.SecPlugSideType == PluggingSideType.Both))
							bSetResult = para.Set(Util.MmToIU(conFixing.SecPlug.depth));
						else
							bSetResult = para.Set(0);
						break;
					case "Secondary_Plugging_Depth_SideB":
						if (conFixing.SecPlug != null &&
							(conFixing.SecPlugSideType == PluggingSideType.SideB || conFixing.SecPlugSideType == PluggingSideType.Both))
							bSetResult = para.Set(Util.MmToIU(conFixing.SecPlug.depth));
						else
							bSetResult = para.Set(0);
						break;
					case "Secondary_Hole_Diameter":
						if (conFixing.BSecHole)
						{
							bSetResult = para.Set(Util.MmToIU(conFixing.SecHoleDiameter));
						}
						else
						{
							bSetResult = para.Set(0);
						}
						break;
					case "Position_TConnector_HOffset":
						if(conPos.HorizontalFixed)
							bSetResult = para.Set(Util.MmToIU(elemInfo.Secondary_Element_Width / 2));
						else
							bSetResult = para.Set(Util.MmToIU(conPos.LeftOffset) + Util.MmToIU(conType.Geometry_TConnector_Width / 2));
						break;
					case "Position_TConnector_VOffset":
						if (conPos.VerticalFixed)
							bSetResult = para.Set(Util.MmToIU(elemInfo.Secondary_Element_Height / 2));
						else
							bSetResult = para.Set(Util.MmToIU(conPos.TopOffset) + Util.MmToIU(conType.Geometry_TConnector_Height / 2));
						break;
					//	Default Parameters
					//case "Hole Core Aluminium_Diameter":
					//	bSetResult = para.Set(Util.MmToIU(17));
					//	break;
					//case "Hole Flange Concrete_Diameter":
					//	bSetResult = para.Set(Util.MmToIU(17));
					//	break;
					//case "Hole Flange Timber_Diameter":
					//	bSetResult = para.Set(Util.MmToIU(7.5));
					//	break;
					default:
						break;
				}
			}

			return bSetResult;
		}
	}
}