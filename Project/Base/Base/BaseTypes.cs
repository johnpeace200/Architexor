using Autodesk.Revit.DB;
using Architexor.Utils;

namespace Architexor.Base
{
	/// <remarks>Type of element to split</remarks>
	public enum SplitElementType
	{
		Wall = 0x01,
		Floor = 0x02,
		Part = 0x03
	};

	/// <remarks>Option to split the opening</remarks>
	public enum OpeningSelectOption
	{
		None = 0,
		LintelAndSill = 1,
		AroundCentreOfOpening = 2,
		CentreOfOpening = 3,
		EqualDistanceBetweenOpenings = 4
	};

	/// <summary>
	/// Summary
	/// </summary>
	/// <remarks>Opening Element</remarks>
	public class COpening
	{
		/// <summary>
		/// Point of the element
		/// </summary>
		public ElementId Parent { get; set; }
		/// <summary>
		/// Point of the element
		/// </summary>
		public ElementId Id { get; set; }
		/// <summary>
		/// Area of the opening
		/// </summary>
		/// <remarks></remarks>
		public double Area { get; set; }
		//	Followings have no meaning when this is for global
		/// <summary>
		/// Location Point of the opening. Should be the vertical middle and horizontal centre point
		/// TODO :
		/// </summary>
		public XYZ Location { get; set; }
		/// <summary>
		/// Project the Location point to LocationCurve
		/// </summary>
		public XYZ ProjectedLocation { get; set; }
		/// <summary>
		/// Width of the opening
		/// </summary>
		/// <remarks></remarks>
		public double Width { get; set; }
		/// <summary>
		/// Height of the opening
		/// </summary>
		/// <remarks></remarks>
		public double Height { get; set; }
		/// <summary>
		/// Boundary points of this opening
		/// </summary>
		public XYZ[] Points { get; set; }
		/// <summary>
		/// Flag if this opening is on the boundary edge of the parent element
		/// </summary>
		public bool IntersectWithBoundaryEdge { get; set; }

		/// <summary>
		/// Option to split this opening
		/// </summary>
		public OpeningSelectOption Option { get; set; }
		public void ComputeArea()
		{
			Area = Util.ComputeArea(Points);
		}
	}

	public struct SSplitPoint
	{
		/// <summary>
		/// This is the index in the SelectedOpenings array of one Element
		/// </summary>
		public int ReferenceToOpening { get; set; }
		/// <summary>
		/// This is reserved field
		///		Next EQ Option OpeningParameter Index : EQ Option
		///		Left or Right : LS option and Around Centre option (0 : Left with bottom, 1 : Right with bottom, 2: Left without bottom, 3: Right without bottom
		/// </summary>
		public int Reserved { get; set; }
		/// <summary>
		/// Point which the split line will be placed on. (This point is located on the LocationCurve)
		/// </summary>
		public XYZ Point { get; set; }
	}

	public struct SSplitLine
	{
		/// <summary>
		/// Index in the SplitPoints array of one Element
		/// </summary>
		public int ReferenceToSplitPoint { get; set; }
		/// <summary>
		/// Points of this split line. 0 is nearer to the LocationCurve than 1
		/// </summary>
		public XYZ[] Points { get; set; }
		/// <summary>
		/// Append margins to both side of the line to use for sketch lines
		/// </summary>
		public Curve SplitCurveWithMargin { get; set; }
		/// <summary>
		/// if true, this line is cut by the farest edge from LocationCurve
		/// </summary>
		public bool IsCutByEdge { get; set; }
		/// <summary>
		/// if true, this line is cut by LocationCurve
		/// </summary>
		public bool IsCutByLocationCurve { get; set; }
		/// <summary>
		/// Split Option of the opening which makes this split line
		/// </summary>
		public OpeningSelectOption Option { get; set; }
		public int Reserved { get; set; }
	}

	public class CPanel
	{
		public double NetArea { get; set; }
		public double BoundingArea { get; set; }
		public XYZ[] Points { get; set; }
		public XYZ[] Point2Ds { get; set; }
		public XYZ MaxPoint2D { get; set; }
		public XYZ MinPoint2D { get; set; }
		public void ComputeNetArea()
		{
			NetArea = Util.ComputeArea(Points);
		}
	}

	public class CCommonSettings
	{
		/// <summary>
		/// Display Unit of this tool
		/// </summary>
#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
		public static ForgeTypeId Unit { get; set; } = UnitTypeId.Millimeters;
#else
		public static DisplayUnitType Unit { get; set; } = DisplayUnitType.DUT_MILLIMETERS;
#endif
		/// <summary>
		/// Display Square Unit of this tool
		/// </summary>
#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
		public static ForgeTypeId SquareUnit { get; set; } = UnitTypeId.SquareMillimeters;
#else
		public static DisplayUnitType SquareUnit { get; set; } = DisplayUnitType.DUT_SQUARE_MILLIMETERS;
#endif

		public CCommonSettings()
		{
		}
	}
}
