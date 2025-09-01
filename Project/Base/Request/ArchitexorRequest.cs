using System.Threading;

namespace Architexor.Request
{
	//	A list of requests the SplitWallSettings dialog has available
	public enum ArchitexorRequestId : int
	{
		None = 0,
		Split = 0x10,
		SplitWall = 0x11,
		SplitFloor = 0x12,
		SelectSplitElements = 0x13,
		SelectAdjacentElements = 0x14,
		SelectOpeningsForLC = 0x15,
		SelectOpeningsForAroundCentre = 0x16,
		SelectOpeningsForCentre = 0x17,
		SelectOpeningsForEqualDistanceBetween = 0x18,
		PrepareSplit = 0x19,
		SelectStartPoint = 0x1A,
		AnalysisSplit = 0x1B,
		ViewElementInRevit = 0x1C,
		ShowFailureList = 0x1D,

		ArrangeConnectors = 0x20,
		HalfLap = 0x21,
		JointBoard = 0x22,
		SelectParentElements = 0x23,

		//  3D Properties
		PropertyMarker = 0x30,
		SelectPropertyElements = 0x31,
		ArrangePropertyMarkers = 0x32,
		SelectNextPropertyElement = 0x33,
		SelectPreviousPropertyElement = 0x34,
		PropertyMarkerUpdate = 0x35,
		SelectPropertyElementsToUpdate = 0x36,
		UpdatePropertyMarkers = 0x037,

		//	Glulam T Connector
		GlulamTConnector = 0x40,
		SelectElementsForTConnector = 0x41,
		ArrangeTConnectors = 0x42,

		//	Auto Dimensioning
		AutoDimensioning = 0x50,
		DetectAndSelectExteriorWalls = 0x51,
		SelectGroup = 0x53,
		GenerateDimension = 0x54,

		//	Auto Tagging
		AutoTagging = 0x60,
		InitRectCache = 0x61,
		ShowCache = 0x65,
		GetTaggingElements = 0x62,
		SelectTaggingGroup = 0x63,
		GenerateTags = 0x64,
	}

	//	A class around a variable holding the current request.
	//	<remarks>
	//		Access to it is made thread-safe, even though we don't necessarily
	//		need it if we always disable the dialog between individual requests.
	//	</remarks>
	public class ArchitexorRequest
	{
		// Storing the value as a plain Int makes using the interlocking mechanism simpler
		private int m_request = (int)ArchitexorRequestId.None;

		//  Take - The Idling handler calls this to obtain the latest request. 
		//  <remarks>
		//      This is not a getter! It takes the request and replaces it
		//      with 'None' to indicate that the request has been "passed on".
		//  </remarks>
		public ArchitexorRequestId Take()
		{
			return (ArchitexorRequestId)Interlocked.Exchange(ref m_request, (int)ArchitexorRequestId.None);
		}

		//  Make - The Dialog calls this when the user presses a command button there. 
		//  <remarks>
		//      It replaces any older request previously made.
		//  </remarks>
		public void Make(ArchitexorRequestId request)
		{
			Interlocked.Exchange(ref m_request, (int)request);
		}
	}
}
