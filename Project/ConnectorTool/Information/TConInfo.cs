using System.Drawing;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using System;
using ConPoint = System.Drawing.Point;
using Architexor.Models.ConnectorTool;
using System.Windows.Forms;

namespace ConnectorTool.Information
{
	/// <summary>
	/// The Connector Information
	/// </summary>
	public class TConInfo
	{
		#region Fields

		FamilySymbol m_ConSymbol;

		#endregion

		#region Properties
		/// <summary>
		/// The connected elements
		/// </summary>
		public TConElemInfo ConElemInfo { get; set; } = new TConElemInfo();

		/// <summary>
		/// The connection family
		/// </summary>
		public FamilySymbol ConSymbol
		{
			get
			{
				return m_ConSymbol;
			}
			set
			{
				m_ConSymbol = value;
				ConTypeParam.GetParameters(ConSymbol);
			}
		}

		/// <summary>
		/// The type parameters of connection
		/// </summary>
		public TConTypeParam ConTypeParam { get; } = new TConTypeParam();

		/// <summary>
		/// The position parameters
		/// </summary>
		public TConPositionParam ConPositionParam { get; set; }

		/// <summary>
		/// The recess to secondary element parameters
		/// </summary>
		public TConFinFCParam ConFinFCParam { get; set; } = new TConFinFCParam();

		/// <summary>
		/// The drill to main element parameters
		/// </summary>
		public TConFixingParam ConFixingParam { get; set; } = new TConFixingParam();

		#endregion

		#region methods

		public void DrawPreview(System.Drawing.Graphics graPreview, Size sizePreview, int prv_Type)
		{
			graPreview.Clear(System.Drawing.Color.White);
			
			#region GetData for Drawing
			// parameter of connection
			int fLength = (int)ConTypeParam.Geometry_TConnector_Height;
			int nBackWidth = (int)ConTypeParam.Geometry_TConnector_Width;
			int nBackThickness = (int)ConTypeParam.Geometry_TConnector_BackPlate_Thk;
			int nFinWidth = (int)ConTypeParam.Geometry_TConnector_Depth;
			int nFinThickness = (int)ConTypeParam.Geometry_TConnector_FinPlate_Thk;

			//	parameter of element information
			int nmainB;
			int nmainH;
			if (ConElemInfo.PrimaryElement.Category.Name == "Structural Columns")
			{
				nmainH = (int)ConElemInfo.Primary_Element_Depth;
				nmainB = (int)ConElemInfo.Primary_Element_Width;
			}
			else if (ConElemInfo.PrimaryElement.Category.Name == "Walls")
			{
				nmainH = 1000;
				nmainB = (int)ConElemInfo.Primary_Element_Depth;
			}
			else
			{
				nmainH = 1000;
				nmainB = (int)ConElemInfo.Primary_Element_Depth;
			}
			int nsecH = (int)ConElemInfo.Secondary_Element_Height;
			int nsecB = (int)ConElemInfo.Secondary_Element_Width;
			if(nsecH == 0 || nsecB == 0)
			{
				MessageBox.Show("Can't get the width and height of the secondary element.");
				return;
			}

			double scale = ((double)sizePreview.Height - 10) / (double)(nmainH >= nsecH ? nmainH : nsecH);
			if (ConElemInfo.PrimaryElement.Category.Name != "Structural Columns")
			{
				scale *= 2;
			}

			//	parameter of recess
			bool bRecessToMain = ConPositionParam.RecessCase == RecessCase.Primary;

			int nSideA = 0, nSideB = 0, nDepth = 0, nTop = 0, nBtm = 0;

			int nFinFront = (int)ConFinFCParam.Gap_FinPlate_Front;
			int nFinLeft = (int)ConFinFCParam.Gap_FinPlate_Sides;
			int nFinTop = (int)ConFinFCParam.Gap_FinPlate_Top;
			int nFinBtm = (int)ConFinFCParam.Gap_FinPlate_Bottom;

			int nGap = (int)ConPositionParam.Gap_MainSecondary_Between;

			ConPoint ptDim1 = new ConPoint();
			ConPoint ptDim2 = new ConPoint();
			int nTopOffset = 0, nLeftOffset = 0;

			//	Create solid brush for Primary Element
			SolidBrush brPri = new SolidBrush(System.Drawing.Color.FromArgb(242, 220, 189));

			//	Create solid brush for Connector
			SolidBrush brCon = new SolidBrush(System.Drawing.Color.Gray);

			//	Create solid brush for Space
			SolidBrush brSpace = new SolidBrush(System.Drawing.Color.White);

			//	Calculate scale of drawing			
			//double scale = ((double)sizePreview.Height - 10) / (double)nmainH;

			//	Set the origin point of coordinate system
			ConPoint originPlan = new ConPoint(10 + (int)(scale * nmainB), -sizePreview.Height / 2);

			//graPreview.DrawRectangle(pSolid, 0, 0, sizePreview.Width - 1, sizePreview.Height - 1);
			int CenterX = 0, nSpace = 0;

			nSideA = (int)ConPositionParam.Gap_BackPlate_SideA;
			nSideB = (int)ConPositionParam.Gap_BackPlate_SideB;
			nDepth = (int)ConPositionParam.Gap_BackPlate_RecessDepth;
			nTop = (int)ConPositionParam.Gap_BackPlate_Top;
			nBtm = (int)ConPositionParam.Gap_BackPlate_Bottom;
			nSpace = nGap;
			if (bRecessToMain)
			{
				CenterX = -nDepth;
			}
			else
			{
				CenterX = nDepth;
			}

			int nLeftErr = 0;
			#endregion

			ConPoint origin;
			List<ConPoint> ptSpacelist;
			List<ConPoint> ptEleConlist;

			if (prv_Type == 0)
			{
				#region Draw PlanView
				//	Draw TConnector in Plan View
				//	Draw Primary Element
				List<ConPoint> ptEleColumnlist = new List<ConPoint>
				{
					new ConPoint(10, 10),
					new ConPoint(10, (sizePreview.Height - 10)),
					new ConPoint(10 + (int)(nmainB * scale), (sizePreview.Height - 10)),
					new ConPoint(10 + (int)(nmainB * scale), 10),
					new ConPoint(10, 10)
				};
				DrawMember(graPreview, ptEleColumnlist, brPri);

				origin = originPlan;
				
				//	Draw Secondary Element
				List<ConPoint> ptEleBeamlist = new List<ConPoint>
				{
					GetRealPoint(originPlan, new ConPoint(nSpace, - nsecB / 2), scale),
					new ConPoint((int)(sizePreview.Width - 10), GetRealPoint(originPlan, new ConPoint((int)((sizePreview.Width - 35)), -nsecB / 2), scale).Y),
					new ConPoint((int)(sizePreview.Width - 10), GetRealPoint(originPlan, new ConPoint((int)((sizePreview.Width - 35)), nsecB / 2), scale).Y),
					GetRealPoint(originPlan, new ConPoint(nSpace, nsecB / 2), scale),
					GetRealPoint(originPlan, new ConPoint(nSpace, -nsecB / 2), scale)
				};
				DrawMember(graPreview, ptEleBeamlist, brPri);
				
				//	draw Space around connector
				//	Calculate position of connector			

				if (!ConPositionParam.HorizontalFixed)
				{
					switch (ConPositionParam.HAnchorPoint)
					{
						case HorizontalAnchorPoint.LEFT:
							originPlan = new ConPoint(originPlan.X, -GetRealPoint(originPlan, new ConPoint(originPlan.X, (int)(nsecB / 2 - nBackWidth / 2 - ConPositionParam.LeftOffset)), scale).Y);
							nLeftOffset = ((int)(nBackWidth / 2 + ConPositionParam.LeftOffset));
							//nLeftErr = 13;
							break;
						case HorizontalAnchorPoint.CENTRE:
							originPlan = new ConPoint(originPlan.X, -GetRealPoint(originPlan, new ConPoint(originPlan.X, (int)(nsecB / 2 - ConPositionParam.LeftOffset)), scale).Y);
							nLeftOffset = (int)(ConPositionParam.LeftOffset);
							//nLeftErr = 5;
							break;
						case HorizontalAnchorPoint.RIGHT:
							originPlan = new ConPoint(originPlan.X, -GetRealPoint(originPlan, new ConPoint(originPlan.X, (int)(nsecB / 2 + nBackWidth / 2 - ConPositionParam.LeftOffset)), scale).Y);
							nLeftOffset = ((int)(ConPositionParam.LeftOffset - nBackWidth / 2));
							//nLeftErr = -6;
							break;
						default:
							break;
					}
				}
				else
				{
					nLeftOffset = nsecB / 2;
				}
				//	Draw LeftOffset dimension
				ptDim1 = new ConPoint((int)(sizePreview.Width - 10), GetRealPoint(origin, new ConPoint((int)((sizePreview.Width - 35)), nsecB / 2), scale).Y);
				ptDim1.X -= 30;
				ptDim2.Y = ptDim1.Y + (int)(nLeftOffset * scale);
				ptDim2.X = ptDim1.X;
				DrawDimension(graPreview, ptDim1, ptDim2, nLeftOffset.ToString(), DimensionType.Normal_Vertical, 1);

				ptDim1 = ptDim2;
				ptDim2 = new ConPoint((int)(sizePreview.Width - 10), GetRealPoint(origin, new ConPoint((int)((sizePreview.Width - 35)), -nsecB / 2), scale).Y);
				ptDim2.X = ptDim1.X;
				DrawDimension(graPreview, ptDim1, ptDim2, (nsecB - nLeftOffset).ToString(), DimensionType.Normal_Vertical, 1);

				//	Draw space around connector
				if (bRecessToMain)
				{
					ptSpacelist = new List<ConPoint>
					{
						new ConPoint(CenterX, nBackWidth / 2 + nSideB + nLeftErr),
						new ConPoint(CenterX, -nBackWidth / 2 - nSideA + nLeftErr),
						new ConPoint(0, -nBackWidth / 2 - nSideA + nLeftErr),
						new ConPoint(0, -nFinThickness / 2 - nFinLeft),
						new ConPoint(CenterX + nBackThickness + nFinWidth + nFinFront, -nFinThickness / 2 - nFinLeft),
						new ConPoint(CenterX + nBackThickness + nFinWidth + nFinFront, nFinThickness / 2 + nFinLeft),
						new ConPoint(0, nFinThickness / 2 + nFinLeft),
						new ConPoint(0, nBackWidth / 2 + nSideB + nLeftErr),
					};
					DrawMember(graPreview, originPlan, ptSpacelist, scale, brSpace);
				}
				else
				{
					int nSpaceWid = nBackThickness + nFinWidth - nDepth;

					ptSpacelist = new List<ConPoint>
					{
						new ConPoint(nGap, nBackWidth / 2 + nSideB + nLeftErr),
						new ConPoint(nGap, -nBackWidth / 2 - nSideA + nLeftErr),
						new ConPoint(CenterX + nGap, -nBackWidth / 2 - nSideA + nLeftErr),
						new ConPoint(CenterX + nGap, -nFinThickness / 2 - nFinLeft),
						new ConPoint(nBackThickness + nFinWidth + nFinFront, -nFinThickness / 2 - nFinLeft),
						new ConPoint(nBackThickness + nFinWidth + nFinFront, nFinThickness / 2 + nFinLeft),
						new ConPoint(CenterX + nGap, nFinThickness / 2 + nFinLeft),
						new ConPoint(CenterX + nGap, nBackWidth / 2 + nSideB + nLeftErr),
					};
					DrawMember(graPreview, originPlan, ptSpacelist, scale, brSpace);
				}

				//	Draw gap space and dimension if NoRecess is true
				//	Draw gap space
				ptSpacelist = new List<ConPoint>
					{
						new ConPoint(GetRealPoint(originPlan, new ConPoint(nSpace, - nsecB / 2), scale).X - 1, GetRealPoint(originPlan, new ConPoint(nSpace, - nsecB / 2), scale).Y),
						new ConPoint(GetRealPoint(originPlan, new ConPoint(0, - nsecB / 2), scale).X + 1, GetRealPoint(originPlan, new ConPoint(0, - nsecB / 2), scale).Y),
						new ConPoint(GetRealPoint(originPlan, new ConPoint(0, nsecB / 2), scale).X + 1, GetRealPoint(originPlan, new ConPoint(0, nsecB / 2), scale).Y),
						new ConPoint(GetRealPoint(originPlan, new ConPoint(nSpace, nsecB / 2), scale).X - 1, GetRealPoint(originPlan, new ConPoint(nSpace, nsecB / 2), scale).Y),
					};
				DrawMember(graPreview, ptSpacelist, null);

				//	Draw gap dimension
				ptDim1 = new ConPoint(10 + (int)(nmainB * scale), 20);
				ptDim2 = GetRealPoint(originPlan, new ConPoint(nSpace, nsecB / 2), scale);
				ptDim2.Y += 5;
				ptDim1.Y = ptDim2.Y;
				DrawDimension(graPreview, ptDim1, ptDim2, nGap.ToString(), DimensionType.Extension_Horizontal_Right_Up, 1);

				//	Draw Connector
				if (!bRecessToMain)
				{
					CenterX = 0;
				}
				ptEleConlist = new List<ConPoint>
				{
					new ConPoint(CenterX, nBackWidth / 2),
					new ConPoint(CenterX, -nBackWidth / 2),
					new ConPoint(CenterX + nBackThickness, -nBackWidth / 2),
					new ConPoint(CenterX + nBackThickness, -nFinThickness / 2),
					new ConPoint(CenterX + nBackThickness + nFinWidth, -nFinThickness / 2),
					new ConPoint(CenterX + nBackThickness + nFinWidth, nFinThickness / 2),
					new ConPoint(CenterX + nBackThickness, nFinThickness / 2),
					new ConPoint(CenterX + nBackThickness, nBackWidth / 2),
					new ConPoint(CenterX, nBackWidth / 2)
				};
				DrawMember(graPreview, originPlan, ptEleConlist, scale, brCon);

				//	Draw nFinSide dimension
				ptDim1 = GetRealPoint(originPlan, new ConPoint(0, nFinThickness / 2 + nFinLeft), scale);
				ptDim1.X = originPlan.X + (int)((nDepth + nFinWidth / 2) * scale);
				ptDim2 = GetRealPoint(originPlan, new ConPoint(CenterX + nBackThickness + nFinWidth, nFinThickness / 2), scale);
				ptDim2.X = ptDim1.X;
				DrawDimension(graPreview, ptDim1, ptDim2, nFinLeft.ToString(), DimensionType.Extension_Horizontal_Right_Up, 1);

				//	Draw nFinFront dimension
				ptDim1.X = originPlan.X + (int)((CenterX + nBackThickness + nFinWidth) * scale);
				ptDim2.X = ptDim1.X + (int)(nFinFront * scale);
				ptDim1.Y = -originPlan.Y - (int)(nFinThickness / 2 * scale);
				ptDim2.Y = ptDim1.Y;
				DrawDimension(graPreview, ptDim1, ptDim2, nFinFront.ToString(), DimensionType.Extension_Horizontal_Right_Down, 1);

				//	Draw nDepth dimension
				if (bRecessToMain)
				{
					ptDim1 = GetRealPoint(originPlan, new ConPoint(CenterX, nBackWidth / 2), scale);
					ptDim1.Y += (int)(nFinWidth * scale);
					ptDim2.X = ptDim1.X + (int)(nDepth * scale);
					ptDim2.Y = ptDim1.Y;
					DrawDimension(graPreview, ptDim1, ptDim2, nDepth.ToString(), DimensionType.Extension_Horizontal_Right_Down, 1);
				}
				else
				{
					ptDim1 = GetRealPoint(originPlan, new ConPoint(nDepth, nBackWidth / 2), scale);
					ptDim1.Y += (int)(nFinWidth * scale);
					ptDim2.X = ptDim1.X - (int)(nDepth * scale);
					ptDim2.Y = ptDim1.Y;
					DrawDimension(graPreview, ptDim1, ptDim2, nDepth.ToString(), DimensionType.Extension_Horizontal_Right_Down, 1);
				}

				//	Draw nSideA dimension
				ptDim2 = GetRealPoint(originPlan, new ConPoint(CenterX, -nBackWidth / 2 - nSideA), scale);
				ptDim2.X -= 10;
				ptDim1 = GetRealPoint(originPlan, new ConPoint(CenterX, -nBackWidth / 2), scale);
				ptDim1.X = ptDim2.X;
				ptDim2.Y -= 2;
				ptDim1.Y -= 2;
				DrawDimension(graPreview, ptDim1, ptDim2, nSideA.ToString(), DimensionType.Extension_Horizontal_Left_Down, 1);

				//	Draw nSideB dimension
				ptDim1 = GetRealPoint(originPlan, new ConPoint(CenterX, nBackWidth / 2 + nSideB), scale);
				ptDim2 = GetRealPoint(originPlan, new ConPoint(CenterX, nBackWidth / 2), scale);
				ptDim1.X -= 10;
				ptDim2.X = ptDim1.X;
				DrawDimension(graPreview, ptDim1, ptDim2, nSideB.ToString(), DimensionType.Extension_Horizontal_Left_Up, 1);

				//	Draw FinThickness dimension
				ptDim1 = GetRealPoint(originPlan, new ConPoint(CenterX + nBackThickness + nFinWidth, -nFinThickness / 2), scale);
				ptDim2 = GetRealPoint(originPlan, new ConPoint(CenterX + nBackThickness + nFinWidth, nFinThickness / 2), scale);
				ptDim2.X += 17;
				ptDim1.X = ptDim2.X;
				DrawDimension(graPreview, ptDim1, ptDim2, nFinThickness.ToString(), DimensionType.Extension_Horizontal_Right_Up, 1);

				//	Draw BackWidth dimension
				ptDim1 = GetRealPoint(originPlan, new ConPoint(CenterX, nBackWidth / 2), scale);
				ptDim2 = GetRealPoint(originPlan, new ConPoint(CenterX, -nBackWidth / 2), scale);
				ptDim2.X -= 40;
				ptDim1.X = ptDim2.X;
				DrawDimension(graPreview, ptDim2, ptDim1, nBackWidth.ToString(), DimensionType.Normal_Vertical, 1);
				#endregion
			}
			else
			{
				#region Draw FrontView
				ConPoint originFront = new ConPoint(5 + (int)(scale * nmainB), (int)(sizePreview.Height / 2));
				//draw main element
				List<ConPoint> ptFrontViewColumnlist = new List<ConPoint>
				{
					new ConPoint(originFront.X, 5),
					new ConPoint(5, 5),
					new ConPoint(5, sizePreview.Height - 5),
					new ConPoint(originFront.X, sizePreview.Height - 5),
					new ConPoint(originFront.X, 5)
				};
				DrawMember(graPreview, ptFrontViewColumnlist, brPri);

				//      draw second element
				double scalePlan = scale;
				scale = (double)(sizePreview.Height - 30) / (double)(nsecH > nsecB ? nsecH : nsecB);
				List<ConPoint> ptFrontViewBeamlist = new List<ConPoint>
				{
					new ConPoint(originFront.X + (int)(nSpace * scalePlan), originFront.Y - (int)(nsecH / 2 * scale)),
					new ConPoint(sizePreview.Width - 5, originFront.Y - (int)(nsecH / 2 * scale)),
					new ConPoint(sizePreview.Width - 5, originFront.Y + (int)(nsecH / 2 * scale)),
					new ConPoint(originFront.X + (int)(nSpace * scalePlan), originFront.Y + (int)(nsecH / 2 * scale)),
					new ConPoint(originFront.X + (int)(nSpace * scalePlan), originFront.Y - (int)(nsecH / 2 * scale)),
				};
				DrawMember(graPreview, ptFrontViewBeamlist, brPri);


				origin = originFront;
				if (!ConPositionParam.VerticalFixed)
				{
					switch (ConPositionParam.VAnchorPoint)
					{
						case VerticalAnchorPoint.TOP:
							originFront = new ConPoint(originFront.X, originFront.Y + (int)((-nsecH / 2 + ConPositionParam.TopOffset + fLength / 2) * scale));
							nTopOffset = (int)(ConPositionParam.TopOffset);
							break;
						case VerticalAnchorPoint.MIDDLE:
							originFront = new ConPoint(originFront.X, originFront.Y + (int)((-nsecH / 2 + ConPositionParam.TopOffset) * scale));
							nTopOffset = (int)(ConPositionParam.TopOffset) - fLength / 2;
							break;
						case VerticalAnchorPoint.BOTTOM:
							originFront = new ConPoint(originFront.X, originFront.Y + (int)((-nsecH / 2 + ConPositionParam.TopOffset - fLength / 2) * scale));
							nTopOffset = (int)ConPositionParam.TopOffset - fLength;
							break;
						default:
							break;
					}
				}
				else
				{
					nTopOffset = (nsecH / 2 - fLength / 2);
				}

				CenterX = (int)(CenterX * scalePlan);
				//draw connector space

				if (nTop == 0)
				{
					nTop = nTopOffset;
				}

				if (nBtm == 0)
				{
					nBtm = (int)((nsecH - fLength) - nTopOffset);
				}
				if (nFinTop == 0)
				{
					nFinTop = nTopOffset;
				}
				if (nFinBtm == 0)
				{
					nFinBtm = (int)((nsecH - fLength) - nTopOffset);
				}
				if (!bRecessToMain)
				{
					int nSpaceWid = nBackThickness + nFinWidth - nDepth - nGap;
					int nErrX = (int)(nGap * scalePlan);

					ptSpacelist = new List<ConPoint>
					{
						new ConPoint(originFront.X + nErrX, originFront.Y - (int)((fLength / 2 + nTop) * scale)),
						new ConPoint(originFront.X + (int)((nDepth) * scalePlan) + nErrX, originFront.Y - (int)((fLength / 2 + nTop) * scale)),
						new ConPoint(originFront.X + (int)((nDepth) * scalePlan) + nErrX, originFront.Y - (int)((fLength / 2 + nFinTop) * scale)),
						new ConPoint(originFront.X + (int)((nDepth + nSpaceWid + nFinFront) * scalePlan) + nErrX, originFront.Y - (int)((fLength / 2 + nFinTop) * scale)),
						new ConPoint(originFront.X + (int)((nDepth + nSpaceWid + nFinFront) * scalePlan) + nErrX, originFront.Y + (int)((fLength / 2 + nFinBtm) * scale)),
						new ConPoint(originFront.X + (int)((nDepth) * scalePlan) + nErrX, originFront.Y + (int)((fLength / 2 + nFinBtm) * scale)),
						new ConPoint(originFront.X + (int)((nDepth) * scalePlan) + nErrX, originFront.Y + (int)((fLength / 2 + nBtm) * scale)),
						new ConPoint(originFront.X + nErrX, originFront.Y + (int)((fLength / 2 + nBtm) * scale)),
						new ConPoint(originFront.X + nErrX, originFront.Y - (int)((fLength / 2 + nTop) * scale)),
					};
					DrawMember(graPreview, ptSpacelist, brSpace);
				}
				else
				{
					ptSpacelist = new List<ConPoint>
					{
						new ConPoint(CenterX + originFront.X, originFront.Y - (int)((fLength / 2 + nTop) * scale)),
						new ConPoint(CenterX + originFront.X + (int)((nDepth) * scalePlan), originFront.Y - (int)((fLength / 2 + nTop) * scale)),
						new ConPoint(CenterX + originFront.X + (int)((nDepth) * scalePlan), originFront.Y - (int)((fLength / 2 + nFinTop) * scale)),
						new ConPoint(CenterX + originFront.X + (int)((nFinWidth + nDepth + nFinFront) * scalePlan), originFront.Y - (int)((fLength / 2 + nFinTop) * scale)),
						new ConPoint(CenterX + originFront.X + (int)((nFinWidth + nDepth + nFinFront) * scalePlan), originFront.Y + (int)((fLength / 2 + nFinBtm) * scale)),
						new ConPoint(CenterX + originFront.X + (int)((nDepth) * scalePlan), originFront.Y + (int)((fLength / 2 + nFinBtm) * scale)),
						new ConPoint(CenterX + originFront.X + (int)((nDepth) * scalePlan), originFront.Y + (int)((fLength / 2 + nBtm) * scale)),
						new ConPoint(CenterX + originFront.X, originFront.Y + (int)((fLength / 2 + nBtm) * scale)),
						new ConPoint(CenterX + originFront.X, originFront.Y - (int)((fLength / 2 + nTop) * scale)),
					};
					DrawMember(graPreview, ptSpacelist, brSpace);
				}

				//	Draw gap space and dimension if NoRecess is true
				if (nSpace != 0)
				{
					//	Draw gap space
					ptSpacelist = new List<ConPoint>
					{
						new ConPoint(originFront.X + 1, originFront.Y - (int)(nsecH / 2 * scale)),
						new ConPoint(originFront.X + (int)(nGap * scalePlan) - 1, originFront.Y - (int)(nsecH / 2 * scale)),
						new ConPoint(originFront.X + (int)(nGap * scalePlan) - 1, originFront.Y + (int)(nsecH / 2 * scale)),
						new ConPoint(originFront.X + 1, originFront.Y + (int)(nsecH / 2 * scale)),new ConPoint(originFront.X, originFront.Y - (int)(nsecH / 2 * scale)),
					};
					DrawMember(graPreview, ptSpacelist, null);

					//	Draw Gap dimension
					ptDim1 = new ConPoint(originFront.X, 5);
					ptDim2 = new ConPoint(originFront.X + (int)(nSpace * scalePlan), originFront.Y - (int)(nsecH / 2 * scale));
					ptDim1.Y = ptDim2.Y;
					ptDim1.Y += 5;
					ptDim2.Y += 5;
					if (nTopOffset > 110)
					{
						DrawDimension(graPreview, ptDim1, ptDim2, nGap.ToString(), DimensionType.Extension_Horizontal_Right_Up, 1);
					}
					else
					{

						DrawDimension(graPreview, ptDim1, ptDim2, nGap.ToString(), DimensionType.Extension_Horizontal_Right_Down, 1);
					}
				}

				//	draw connector
				if (!bRecessToMain)
				{
					CenterX = 0;
				}
				ptEleConlist = new List<ConPoint>
				{
					new ConPoint(CenterX + originFront.X, originFront.Y - (int)(fLength / 2 * scale)),
					new ConPoint(CenterX + originFront.X + (int)((nFinWidth + nBackThickness) * scalePlan), originFront.Y - (int)(fLength / 2 * scale)),
					new ConPoint(CenterX + originFront.X + (int)((nFinWidth + nBackThickness) * scalePlan), originFront.Y - (int)(fLength / 2 * scale) + (int)(fLength * scale)),
					new ConPoint(CenterX + originFront.X, originFront.Y - (int)(fLength / 2 * scale) + (int)(fLength * scale)),
					new ConPoint(CenterX + originFront.X, originFront.Y - (int)(fLength / 2 * scale)),
				};
				DrawMember(graPreview, ptEleConlist, brCon);

				//	Draw Backplate edge line
				Pen pDash = new Pen(System.Drawing.Color.FromArgb(0, 127, 254), 2)
				{
					DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot
				};
				graPreview.DrawLine(pDash, new ConPoint((int)(nBackThickness * scale) + CenterX + originFront.X, originFront.Y - (int)(fLength / 2 * scale)), new ConPoint((int)(nBackThickness * scale) + CenterX + originFront.X, originFront.Y - (int)(fLength / 2 * scale) + (int)(fLength * scale)));
				pDash.Dispose();

				//	Draw nFinFront dimension
				ptDim1 = new ConPoint(CenterX + originFront.X + (int)((nFinWidth + nBackThickness) * scalePlan), originFront.Y - (int)(fLength / 2 * scale));
				ptDim2.X = ptDim1.X + (int)(nFinFront * scalePlan);
				ptDim1.Y = originFront.Y;
				ptDim2.Y = ptDim1.Y;
				DrawDimension(graPreview, ptDim1, ptDim2, nFinFront.ToString(), DimensionType.Extension_Horizontal_Right_Down, 1);

				//	Draw nTop dimension
				ptDim1 = new ConPoint(originFront.X - (int)(CenterX / 2 + nBackThickness / 2), originFront.Y - (int)((fLength / 2 + nTop) * scale));
				ptDim2 = new ConPoint(originFront.X - (int)(CenterX / 2 + nBackThickness / 2), originFront.Y - (int)((fLength / 2) * scale));
				if (nTopOffset < 50)
				{
					DrawDimension(graPreview, ptDim1, ptDim2, nTop.ToString(), DimensionType.Extension_Horizontal_Left_Down, 1);
				}
				else
				{
					DrawDimension(graPreview, ptDim1, ptDim2, nTop.ToString(), DimensionType.Extension_Horizontal_Left_Up, 1);
				}

				//Draw nFinTop dimension
				ptDim1 = new ConPoint(originFront.X, originFront.Y - (int)((fLength / 2 + nFinTop) * scale));
				ptDim2 = new ConPoint(originFront.X, originFront.Y - (int)(fLength / 2 * scale));
				ptDim2.X = ptDim1.X;
				ptDim1.X += 20;
				ptDim2.X += 20;
				ptDim1.Y -= 1;
				ptDim2.Y -= 1;
				if (nTopOffset < 50)
				{
					DrawDimension(graPreview, ptDim1, ptDim2, nFinTop.ToString(), DimensionType.Extension_Horizontal_Right_Down, 1);
				}
				else
				{
					DrawDimension(graPreview, ptDim1, ptDim2, nFinTop.ToString(), DimensionType.Extension_Horizontal_Right_Up, 1);
				}

				//Draw nBtm dimension
				ptDim1 = new ConPoint(originFront.X - (int)(CenterX / 2 + nBackThickness / 2), originFront.Y + (int)((fLength / 2 + nBtm) * scale));
				ptDim2 = new ConPoint(originFront.X - (int)(CenterX / 2 + nBackThickness / 2), originFront.Y + (int)((fLength / 2) * scale));
				if (nTopOffset < 50)
				{
					DrawDimension(graPreview, ptDim2, ptDim1, nBtm.ToString(), DimensionType.Extension_Horizontal_Left_Down, 1);
				}
				else
				{
					DrawDimension(graPreview, ptDim2, ptDim1, nBtm.ToString(), DimensionType.Extension_Horizontal_Left_Up, 1);
				}

				//Draw nFinBtm dimension
				ptDim2 = new ConPoint(originFront.X + (int)((nFinWidth + nSideB) * scalePlan), originFront.Y + (int)((fLength / 2 + nFinBtm) * scale));
				ptDim1 = new ConPoint(originFront.X + (int)(nFinWidth * scalePlan), originFront.Y - (int)(fLength / 2 * scale) + (int)(fLength * scale));
				ptDim2.X = ptDim1.X;
				ptDim1.X -= 35;
				ptDim2.X -= 35;
				if (nTopOffset < 50)
				{
					DrawDimension(graPreview, ptDim1, ptDim2, nFinBtm.ToString(), DimensionType.Extension_Horizontal_Right_Down, 1);
				}
				else
				{
					DrawDimension(graPreview, ptDim1, ptDim2, nFinBtm.ToString(), DimensionType.Extension_Horizontal_Right_Up, 1);
				}

				//Draw nTopOffset dimension
				ptDim1 = new ConPoint(sizePreview.Width - 5, origin.Y - (int)(nsecH / 2 * scale));
				ptDim2.X = ptDim1.X;
				ptDim2.Y = ptDim1.Y + (int)(nTopOffset * scale);
				ptDim1.X -= 30;
				ptDim2.X -= 30;
				DrawDimension(graPreview, ptDim2, ptDim1, nTopOffset.ToString(), DimensionType.Normal_Vertical, 1);

				//Draw fLength dimension
				ptDim1 = ptDim2;
				ptDim2.Y += (int)(fLength * scale);
				DrawDimension(graPreview, ptDim2, ptDim1, fLength.ToString(), DimensionType.Normal_Vertical, 1);

				//Draw nBottomOffset dimension
				ptDim1 = ptDim2;
				ptDim2.Y = new ConPoint(sizePreview.Width - 5, origin.Y + (int)(nsecH / 2 * scale)).Y;
				DrawDimension(graPreview, ptDim2, ptDim1, (nsecH - fLength - nTopOffset).ToString(), DimensionType.Normal_Vertical, 1);
				#endregion
			}

		}

		/// <summary>
		/// The method for draw member using GetRealPoint method
		/// </summary>
		/// <param name="e">Graphics of preview</param>
		/// <param name="origin">Origin point of member</param>
		/// <param name="ptList">Point list for drawing memeber</param>
		/// <param name="scale">scale for drawing member</param>
		private void DrawMember(Graphics e, ConPoint origin, List<ConPoint> ptList, double scale, Brush bru)
		{
			List<ConPoint> realPtList = new List<ConPoint>();
			for (int i = 0; i < ptList.Count; i++)
			{
				realPtList.Add(GetRealPoint(origin, ptList[i], scale));
			}
			// The pen for visible line
			Pen pSolid = new Pen(System.Drawing.Color.FromArgb(0, 127, 254), 2)
			{
				DashStyle = System.Drawing.Drawing2D.DashStyle.Solid
			};

			ConPoint[] pts = new ConPoint[realPtList.Count];
			realPtList.CopyTo(pts);

			// Draw the contour
			e.DrawPolygon(pSolid, pts);

			// Fill the backcolor
			e.FillPolygon(bru, pts);

			pSolid.Dispose();
		}

		/// <summary>
		/// The method for draw member using calculated points
		/// </summary>
		/// <param name="e">Graphics of preview</param>
		/// <param name="ptList">Point list for drawing memeber</param>
		private void DrawMember(Graphics e, List<ConPoint> ptList, Brush bru)
		{
			// The pen for visible line
			Pen pSolid = new Pen(System.Drawing.Color.FromArgb(0, 127, 254), 2)
			{
				DashStyle = System.Drawing.Drawing2D.DashStyle.Solid
			};

			ConPoint[] pts = new ConPoint[ptList.Count];
			ptList.CopyTo(pts);

			if (bru != null)
			{
				// Draw the contour
				e.DrawPolygon(pSolid, pts);
				// Fill the backcolor
				e.FillPolygon(bru, pts);
			}
			else if (bru == null)
			{
				e.FillPolygon(Brushes.White, pts);
			}
			pSolid.Dispose();
		}

		public enum DimensionType
		{
			Normal_Horizontal,
			Normal_Vertical,
			Extension_Horizontal_Right_Up,
			Extension_Horizontal_Right_Down,
			Extension_Horizontal_Left_Up,
			Extension_Horizontal_Left_Down
		}

		/// <summary>
		/// The method for drawing the dimension
		/// </summary>
		/// <param name="g">Graphis of Preview</param>
		/// <param name="pt1">Left coordination of dimension</param>
		/// <param name="pt2">Right coordination of dimension</param>
		/// <param name="txt">Text of dimension</param>
		/// <param name="scale">The scale of Text</param>
		private void DrawDimension(Graphics e, ConPoint pt1, ConPoint pt2, string txt, DimensionType dimType, double scale)
		{
			if (Convert.ToDouble(txt) <= 0) return;
			Pen dim_pen = new Pen(System.Drawing.Color.Black, 1)
			{
				StartCap = System.Drawing.Drawing2D.LineCap.SquareAnchor,
				EndCap = System.Drawing.Drawing2D.LineCap.SquareAnchor
			};
			e.DrawLine(dim_pen, pt1, pt2);
			using (System.Drawing.Font font = new System.Drawing.Font("Times New Roman", (float)(3 * scale), System.Drawing.GraphicsUnit.Millimeter))
			{
				PointF txt_point = new PointF();
				int pt_txt_x = (pt1.X + pt2.X) / 2;

				ConPoint exStartPt = new ConPoint();
				ConPoint exMiddlePt = new ConPoint();
				ConPoint exEndPt = new ConPoint();

				switch (dimType)
				{
					case DimensionType.Normal_Horizontal:
						if (pt1.X == pt2.X)
						{
							txt_point = new ConPoint(pt1.X, (int)((pt1.Y + (pt2.Y - pt2.Y) / 2)));
						}
						else
						{
							txt_point = new ConPoint((int)((pt1.X + (pt2.X - pt2.X) / 2) * 0.6), pt1.Y);
						}
						e.DrawString(txt, font, Brushes.Black, txt_point);
						break;

					case DimensionType.Normal_Vertical:
						txt_point = new ConPoint((int)(pt1.X * 1), (int)((pt1.Y + (pt2.Y - pt1.Y) / 2) * 0.9));
						e.DrawString(txt, font, Brushes.Black, txt_point, new StringFormat(StringFormatFlags.DirectionVertical | StringFormatFlags.NoClip));
						break;

					case DimensionType.Extension_Horizontal_Right_Up:
						//Draw extension line
						exStartPt.X = pt1.X + Math.Abs(pt2.X - pt1.X) / 2;
						exStartPt.Y = pt1.Y + Math.Abs(pt2.Y - pt1.Y) / 2;

						exMiddlePt.X = exStartPt.X + 10;
						exMiddlePt.Y = exStartPt.Y - 10;

						exEndPt.X = exMiddlePt.X + 25;
						exEndPt.Y = exMiddlePt.Y;

						txt_point = new ConPoint(exMiddlePt.X + Math.Abs(exMiddlePt.X - exEndPt.X) / 2 - 7, exMiddlePt.Y - 13);

						e.DrawLine(Pens.Black, exStartPt, exMiddlePt);
						e.DrawLine(Pens.Black, exMiddlePt, exEndPt);

						//Draw dimension text
						e.DrawString(txt, font, Brushes.Black, txt_point);
						break;

					case DimensionType.Extension_Horizontal_Right_Down:
						//Draw extension line
						exStartPt.X = pt1.X + Math.Abs(pt2.X - pt1.X) / 2;
						exStartPt.Y = pt1.Y + Math.Abs(pt2.Y - pt1.Y) / 2;

						exMiddlePt.X = exStartPt.X + 10;
						exMiddlePt.Y = exStartPt.Y + 10;


						exEndPt.X = exMiddlePt.X + 25;
						exEndPt.Y = exMiddlePt.Y;

						txt_point = new ConPoint(exMiddlePt.X + Math.Abs(exMiddlePt.X - exEndPt.X) / 2 - 7, exMiddlePt.Y - 13);

						e.DrawLine(Pens.Black, exStartPt, exMiddlePt);
						e.DrawLine(Pens.Black, exMiddlePt, exEndPt);

						//Draw dimension text
						e.DrawString(txt, font, Brushes.Black, txt_point);
						break;

					case DimensionType.Extension_Horizontal_Left_Up:
						//Draw extension line
						exStartPt.X = pt1.X + Math.Abs(pt2.X - pt1.X) / 2;
						exStartPt.Y = pt1.Y + Math.Abs(pt2.Y - pt1.Y) / 2;

						exMiddlePt.X = exStartPt.X - 10;
						exMiddlePt.Y = exStartPt.Y - 10;

						exEndPt.X = exMiddlePt.X - 25;
						exEndPt.Y = exMiddlePt.Y;

						txt_point = new ConPoint(exMiddlePt.X - (exMiddlePt.X - exEndPt.X) / 2 - 7, exMiddlePt.Y - 13);

						e.DrawLine(Pens.Black, exStartPt, exMiddlePt);
						e.DrawLine(Pens.Black, exMiddlePt, exEndPt);

						//Draw dimension text
						e.DrawString(txt, font, Brushes.Black, txt_point);
						break;

					case DimensionType.Extension_Horizontal_Left_Down:
						//Draw extension line
						exStartPt.X = pt1.X + Math.Abs(pt2.X - pt1.X) / 2;
						exStartPt.Y = pt1.Y + Math.Abs(pt2.Y - pt1.Y) / 2;

						exMiddlePt.X = exStartPt.X - 10;
						exMiddlePt.Y = exStartPt.Y + 10;

						exEndPt.X = exMiddlePt.X - 25;
						exEndPt.Y = exMiddlePt.Y;

						txt_point = new ConPoint(exMiddlePt.X - (exMiddlePt.X - exEndPt.X) / 2 - 7, exMiddlePt.Y - 13);

						e.DrawLine(Pens.Black, exStartPt, exMiddlePt);
						e.DrawLine(Pens.Black, exMiddlePt, exEndPt);

						//Draw dimension text
						e.DrawString(txt, font, Brushes.Black, txt_point);
						break;

					default:
						break;
				}
			}
			dim_pen.Dispose();
		}

		/// <summary>
		/// The method for getting the real point
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="point"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		private ConPoint GetRealPoint(ConPoint origin, ConPoint point, double scale)
		{
			return new ConPoint((int)(point.X * scale + origin.X), -(int)(point.Y * scale + origin.Y));
		}
		#endregion
	}
}
