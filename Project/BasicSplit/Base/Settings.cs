using Color = System.Drawing.Color;

namespace Architexor.BasicSplit.Base
{
	/// <summary>
	/// Settings for preview
	/// </summary>
	public struct PreviewSetting
	{
		/// <summary>
		/// Color of the boundary lines of panel
		/// </summary>
		public Color PanelBoundaryColor { get; set; }
		/// <summary>
		/// Color of the element to split
		/// </summary>
		public Color ElementColor { get; set; }
		/// <summary>
		/// Color of split lines
		/// </summary>
		public Color SplitLineColor { get; set; }
		/// <summary>
		/// Color of openings
		/// </summary>
		public Color OpeningColor { get; set; }
		/// <summary>
		/// Color of the dimension text
		/// </summary>
		public Color DimensionColor { get; set; }
		/// <summary>
		/// Color of the boundary points
		/// </summary>
		public Color BoundaryPointColor { get; set; }
		/// <summary>
		/// Color of the LocationCurve
		/// </summary>
		public Color LocationCurveColor { get; set; }
		/// <summary>
		/// Color of split points
		/// </summary>
		public Color SplitPointColor { get; set; }
		/// <summary>
		/// Color of split line numbers
		/// </summary>
		public Color SplitLineNumberColor { get; set; }
		/// <summary>
		/// Color of points of panels
		/// </summary>
		public Color PanelPointColor { get; set; }
		/// <summary>
		/// Flag to calculate area of the panels
		/// </summary>
		public int AreaType { get; set; }
	}

	public class Settings
	{
		/// <summary>
		/// Settings for preview
		/// </summary>
		public static PreviewSetting PreviewSettings = new PreviewSetting();

		public static int MinimumPanelWidth = 1000;

		public static void Initialize()
		{			
			PreviewSettings.ElementColor			= Color.Black;
			PreviewSettings.PanelBoundaryColor		= Color.Black;//Color.FromArgb(0, 127, 254);
			PreviewSettings.OpeningColor			= Color.Black;//Color.Blue;
			PreviewSettings.SplitLineColor			= Color.Green;
			PreviewSettings.DimensionColor			= Color.Black;//Color.Red;

			PreviewSettings.BoundaryPointColor		= Color.Transparent;
			PreviewSettings.LocationCurveColor		= Color.Transparent;
			PreviewSettings.SplitPointColor			= Color.Transparent;
			PreviewSettings.SplitLineNumberColor	= Color.Transparent;
			PreviewSettings.PanelPointColor			= Color.Transparent;

			PreviewSettings.AreaType				= 1;
		}
	}
}
