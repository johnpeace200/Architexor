using System.Collections.Generic;

namespace Architexor.Models.ConnectorTool
{
	/// <summary>
	/// Enum for determining the Horizontal AnchorPostion of Connection to SecondaryElement
	/// </summary>
	public enum HorizontalAnchorPoint
	{
		LEFT,
		CENTRE,
		RIGHT
	}

	/// <summary>
	/// Enum for determining the Vertical AnchorPostion of Connection to SecondaryElement
	/// </summary>
	public enum VerticalAnchorPoint
	{
		TOP,
		MIDDLE,
		BOTTOM
	}

	/// <summary>
	/// The case of recessing of backplate(into primary element or secondary element)
	/// </summary>
	public enum RecessCase
	{
		Primary,
		Secondary
	}

	public enum ConnectionType
	{
		None = 0,
		Bolt = 1,
		Dowel = 2,
		Screw = 3
	}

	public class FixingCandidate
	{
		public string Name;
		public object Symbol;
		public double Diameter = 0;
		public double Length = 0;
		public ConnectionType ConnectionType;

		public override string ToString()
		{
			return Name;
		}
	}

	public class Fixing
	{
		public bool Show { get; set; } = true;
		public string Name { get; set; }
		public double Diameter { get; set; }
		public double Length { get; set; }
		public string DiameterParameter { get; set; }
		public string LengthParameter { get; set; }
		public bool IsTypeParameter { get; set; }

		/*
        public static bool operator ==(Fixing lhs, Fixing rhs)
        {
            bool status = false;
            if (lhs.Name == rhs.Name && lhs.Diameter == rhs.Diameter
            && lhs.Length == rhs.Length)
            {
                status = true;
            }
            return status;
        }
        public static bool operator !=(Fixing lhs, Fixing rhs)
        {
            bool status = false;
            if (lhs.Name != rhs.Name || lhs.Diameter != rhs.Diameter
            || lhs.Length != rhs.Length)
            {
                status = true;
            }
            return status;
        }
        */
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return base.ToString();
		}
	}

	public class Plug
	{
		public bool valid;
		public double diameter;
		public double depth;
		public bool show;
	}

	/// <summary>
	/// This is pluggingSideType if you drill to secondary element
	/// <para SideA> is left in front of secondary element</para>
	/// <para SideB> is right in front of secondary element</para>
	/// <para Both>is both side of secondary element</para>
	/// </summary>
	public enum PluggingSideType
	{
		SideA,
		SideB,
		Both
	}

	/// <summary>
	/// This class is responsible for interacting with UsrTConnectorPosition Form
	/// </summary>
	public class TConPositionParam
	{
		#region Properties
		/// <summary>
		/// HorizontalCentre of Connection Fixed to Centreline of SecondaryElement
		/// </summary>
		public bool HorizontalFixed { get; set; }
		/// <summary>
		/// VerticalCentre of Connection Fixed to Centreline of SecondaryElement
		/// </summary>
		public bool VerticalFixed { get; set; }

		/// <summary>
		/// HorizontalAnchorPoint of Connection
		/// </summary>
		public HorizontalAnchorPoint HAnchorPoint { get; set; }

		/// <summary>
		/// The Spacing between LeftEdge of SecondaryElement and Horizontal Centre of FinPlate
		/// </summary>
		public double LeftOffset { get; set; }

		/// <summary>
		/// VerticalAnchorPoint of Connection
		/// </summary>
		public VerticalAnchorPoint VAnchorPoint { get; set; }

		/// <summary>
		/// The Spacing between TopEdge of SecondaryElement and Vertical Centre of FinPlate
		/// </summary>
		public double TopOffset { get; set; }

		/// <summary>
		/// The Height of Secondary Element in Connector
		/// </summary>
		public double Secondary_Height { get; set; }

		/// <summary>
		/// The Width of Secondary Element in Connector
		/// </summary>
		public double Secondary_Width { get; set; }

		/// <summary>
		/// The recessing case of backplate
		/// </summary>
		public RecessCase RecessCase { get; set; }

		/// <summary>
		/// The depth of backplate(recess into main)
		/// </summary>
		public double Gap_BackPlate_RecessDepth { get; set; }

		/// <summary>
		/// The depth of backplate(recess into secondary)
		/// </summary>
		public double Gap_BackPlate_Front { get; set; }

		/// <summary>
		/// The top gap of backplate
		/// </summary>
		public double Gap_BackPlate_Top { get; set; }

		/// <summary>
		/// The bottom gap of backplate
		/// </summary>
		public double Gap_BackPlate_Bottom { get; set; }

		/// <summary>
		/// The left gap of backplate
		/// </summary>
		public double Gap_BackPlate_SideA { get; set; }

		/// <summary>
		/// The right gap of backplate
		/// </summary>
		public double Gap_BackPlate_SideB { get; set; }

		/// <summary>
		/// The gap between main and secondary element
		/// </summary>
		public double Gap_MainSecondary_Between { get; set; }

		#endregion

		public TConPositionParam()
		{
			//	Default Parameters
			HorizontalFixed = true;
			VerticalFixed = true;
			HAnchorPoint = HorizontalAnchorPoint.LEFT;
			LeftOffset = 50;
			VAnchorPoint = VerticalAnchorPoint.TOP;
			TopOffset = 50;
			Secondary_Height = 600;
			Secondary_Width = 300;
			RecessCase = RecessCase.Primary;
			Gap_BackPlate_RecessDepth = 20;
			Gap_BackPlate_Front = 0;
			Gap_BackPlate_Top = 10;
			Gap_BackPlate_Bottom = 10;
			Gap_BackPlate_SideA = 10;
			Gap_BackPlate_SideB = 10;
			Gap_MainSecondary_Between = 5;
		}
	}

	/// <summary>
	/// This class is responsible for interacting with UsrTConnectorFixings Form
	/// </summary>
	public class TConFinFCParam
	{
		#region Properties

		/// <summary>
		/// The Front Gap of finplate
		/// </summary>
		public double Gap_FinPlate_Front { get; set; }

		/// <summary>
		/// The top gap of finplate
		/// </summary>
		public double Gap_FinPlate_Top { get; set; }

		/// <summary>
		/// The bottom gap of finplate
		/// </summary>
		public double Gap_FinPlate_Bottom { get; set; }

		/// <summary>
		/// The side gap of finplate
		/// </summary>
		public double Gap_FinPlate_Sides { get; set; }

		/// <summary>
		/// whether show the weld notch
		/// </summary>
		public bool BFilletNotch { get; set; }

		/// <summary>
		/// The top gap of weld notch
		/// </summary>
		public double Gap_Fillet_Top { get; set; }

		/// <summary>
		/// The bottom gap of weld notch
		/// </summary>
		public double Gap_Fillet_Bottom { get; set; }

		/// <summary>
		/// The side gap of weld notch
		/// </summary>
		public double Gap_Fillet_Width { get; set; }

		/// <summary>
		/// The end gap of weld notch
		/// </summary>
		public double Gap_Fillet_Depth { get; set; }

		/// <summary>
		/// whether allow void of the coverplate
		/// </summary>
		public bool BCoverPlate { get; set; }

		/// <summary>
		/// The length of coverplate
		/// </summary>
		public double Gap_CoverBoard_Length { get; set; }

		/// <summary>
		/// The width of coverplate
		/// </summary>
		public double Gap_CoverBoard_Width { get; set; }

		/// <summary>
		/// The depth of coverplate
		/// </summary>
		public double Gap_CoverBoard_Depth { get; set; }

		#endregion

		public TConFinFCParam()
		{
			Gap_FinPlate_Front = 10;
			Gap_FinPlate_Top = 10;
			Gap_FinPlate_Bottom = 10;
			Gap_FinPlate_Sides = 10;
			BFilletNotch = false;
			Gap_Fillet_Depth = 5;
			Gap_Fillet_Width = 5;
			BCoverPlate = false;
			Gap_CoverBoard_Length = 100;
			Gap_CoverBoard_Width = 100;
			Gap_CoverBoard_Depth = 20;
		}
	}

	/// <summary>
	/// This class is responsible for interacting with UsrTConnectorDrillToMain Form
	/// </summary>
	public class TConFixingParam
	{
		#region Properties

		// For Primary Element
		/// <summary>
		/// The type name of primary bolt
		/// </summary>
		public FixingCandidate PriFixing { get; set; }

		/// <summary>
		/// Whether show the Bolt
		/// </summary>
		public bool BPriBoltHole { get; set; }
		/// <summary>
		/// Diameter of Bolt Hole
		/// </summary>
		public double PriBoltHoleDiameter { get; set; }

		/// <summary>
		/// The plug of bolt in primary element
		/// </summary>
		public Plug PriPlug { get; set; }

		// For Secondary Element
/// <summary>
		/// Type of Bolt
		/// </summary>
		public FixingCandidate SecFixing { get; set; }

		/// <summary>
		/// Whether show the Bolt
		/// </summary>
		public bool BSecHole { get; set; }

		/// <summary>
		/// Diameter of Bolt Hole
		/// </summary>
		public double SecHoleDiameter { get; set; }

		/// <summary>
		/// This is Whether you will reduce the BoltHole from edges of Secondary Elem
		/// </summary>
		public bool BSecReducedHole { get; set; }

		/// <summary>
		/// The Reduced SideA (Left in front of Secondary Elem)
		/// </summary>
		public double SecReducedSideA { get; set; }

		/// <summary>
		/// The Reduced SideB( Right in front of Secondary Elem)
		/// </summary>
		public double SecReducedSideB { get; set; }

		/// <summary>
		/// The plugging SideType in Secondary Elem
		/// </summary>
		public PluggingSideType SecPlugSideType { get; set; }

		public Plug SecPlug { get; set; }

		public TConFixingParam()
		{
			// For primary element
			PriFixing = new FixingCandidate();
			BPriBoltHole = false;
			PriBoltHoleDiameter = 20;
			PriPlug = new Plug();

			// For secondary element
			SecFixing = new FixingCandidate();
			BSecHole = false;
			SecHoleDiameter = 20;
			BSecReducedHole = false;
			SecReducedSideA = 0;
			SecReducedSideB = 0;
			SecPlugSideType = PluggingSideType.SideA;
			SecPlug = new Plug();
		}
		#endregion
	}
}
