using ConnectorTool.Base;
using Autodesk.Revit.DB;
using ConnectorTool.Information;
using RevitPosition = Autodesk.Revit.DB.XYZ;
using RevitDirection = Autodesk.Revit.DB.XYZ;
using Architexor.Models.ConnectorTool;
using Architexor.Utils;

namespace ConnectorTool.Location
{
	public class TConLocation: TLocation
	{
		#region Properties
		/// <summary>
		/// The Normal Vector of Host face
		/// </summary>
		public RevitDirection HostFaceNormal { get; set; }

		/// <summary>
		/// The Location of Connection
		/// </summary>
		public RevitPosition ConPosition { get; private set; } = RevitDirection.Zero;

		/// <summary>
		/// The Direction of Connection
		/// </summary>
		public RevitDirection ConDirection { get; private set; } = RevitDirection.Zero;

		/// <summary>
		/// The DatumPoint for installing the Connector
		/// </summary>
		public RevitPosition DatumPos { get; set; }

		/// <summary>
		/// Set whether the object is flipped or not.
		/// </summary>
		public bool IsFlip { get; set; } = false;
		#endregion

		#region Methods

		/// <summary>
		/// Constructor
		/// </summary>
		public TConLocation()
		{
			Initialize();
		}

		/// <summary>
		/// Initialize
		/// </summary>
		protected override void Initialize()
		{
				
		}

		/// <summary>
		/// Get the Datum Point - This is the intersecting point between host face and curve of secondary element
		/// </summary>
		/// <param name="elemInfo"> The information of connected elements</param>
		/// <returns></returns>
		private bool GetDatumPoint(TConElemInfo elemInfo)
		{
			// The ProjectingPoint of SecondaryElement to the PrimaryElement
			RevitDirection projectingPoint = elemInfo.GetOrigin();
			Face hostFace = elemInfo.HostFace;
			
			if (hostFace != null)
			{
				Surface surface = hostFace.GetSurface();
				UV uV = new UV();
				double dis = 0;
				surface.Project(projectingPoint, out uV, out dis);
				
				IntersectionResult intersectionRes;
				
				bool b = hostFace.IsInside(uV, out intersectionRes);

				XYZ pos = new XYZ();
				//pos = intersectionRes.XYZPoint;
				IntersectionResult intersection = hostFace.Project(projectingPoint);
				if (intersection != null)
				{
					DatumPos = intersection.XYZPoint;
					ConDirection = hostFace.ComputeNormal(intersection.UVPoint).CrossProduct(XYZ.BasisZ.Negate());
					HostFaceNormal = hostFace.ComputeNormal(intersection.UVPoint);
				}
				else
				{
					DatumPos = (surface as Plane).ProjectOnto(projectingPoint);
					ConDirection = (hostFace as PlanarFace).FaceNormal.CrossProduct(XYZ.BasisZ.Negate());
					HostFaceNormal = (hostFace as PlanarFace).FaceNormal;
					return true;
				}
			}
			else
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Set the Parameters of Type.
		/// </summary>
		public bool GetLocationParameters(TConElemInfo elemInfo)
		{
			if (!GetDatumPoint(elemInfo))
				return false;

			return true;
		}

		#endregion
	}
}

