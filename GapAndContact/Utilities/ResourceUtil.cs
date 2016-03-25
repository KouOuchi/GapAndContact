using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denture.Utilities
{
    /// <summary>
    /// Define string that is used  in project.
    /// </summary>
    public static class ResourceUtil
    {
        #region String

        public const string SelCentralIncisorLeft = "Select the central incisor in left";
        public const string SelCentralIncisorRight = "Select the central incisor in right";
        public const string SelLateralIncisorLeft = "Select the lateral Incisor left";
        public const string SelLateralIncisorRight = "Select the lateral Incisor right";
        public const string SelCuspidLeft = "Select the cuspid in left";
        public const string SelCuspidRight = "Select the cuspid in right";
        public const string SelFirstPremolorLeft = "Select the first premolor in left";
        public const string SelFirstPremolorRight = "Select the first premolor in right.";
        public const string SelSecondtPremolorLeft = "Select the second premolor in left";
        public const string SelSecondPremolorRight = "Select the second premolor in right.";
        public const string SelFirstMolorLeft = "Select the first molor in left";
        public const string SelFirstMolorRight = "Select the first molor in right";
        public const string SelSecondtMolorLeft = "Select the second molor in left";
        public const string SelSecondMolorRight = "Select the second molor in right";
        public const string SelPointInMedianLine = "Select one point in median line";
        public const string SelCurveInPlate = "Select Curve in Plate";
        public const string SelSurfaceInPlate = "Select Surface in Plate";
        public const string SelMeshInPlate = "Select Mesh in Plate";
        public const string SelPalatineBoneToExtend = "Select Palatine Bone To Extend";
        public const string SelShrinkInnerCurve = "Shrink Inner Curve";

        public const string SelMesh = "Select mesh";
        public const string SelMeshFace = "Select mesh face";
        public const string SelPlate = "Select Plate";
        public const string Sel = "Select ";
        public const string SelPlane = "Select plane";
        public const string SelInnerCurve = "Select inner curve";
        public const string SelCurve = "Select curve";
        
        public const string Parent = "Parent";
        public const string MedianLine = "MedianLine";
        public const string MedianPlane = "MedianPlane";
        public const string PointA = "PointA";
        public const string PointB = "PointB";
        public const string ProjectedHorseShoeInnerCurve = "ProjectedHorseShoeInnerCurve";
        public const string ForthHorseShoeInnerCurveWithoutBase = "ForthHorseShoeInnerCurveWithoutBase";
        public const string Radial = "Radial";
        public const string RadialA = "RadialParA";
        public const string RadialB = "RadialParB";
        public const string InGingival = "InGingival";
        public const string OutGingival = "OutGingival";
        public const string SecondOffsetGin = "SecondOffsetGingival";
        public const string ThirOffsetGin = "ThirOffsetGingival";
        public const string SecondOffsetGinProjected = "SecondOffsetGingivalProjected";
        public const string ThirOffsetGinProjected = "ThirOffsetGingivalProjected";
        public const string EdgeInnerCurve = "EdgeInnerCurve";
        public const string LeftSplitPointOfEdgeInnerCurve = "Left Split Point Of Edge Inner Curve";
        public const string RightSplitPointOfEdgeInnerCurve = "Right Split Point Of Edge Inner Curve";
        public const string LeftProjectedEdgeInnerCurve = "Left Projected Inner Curve";
        public const string CenterProjectedEdgeInnerCurve = "Center Projected Inner Curve";
        public const string RightProjectedEdgeInnerCurve = "Right Projected Inner Curve";
        public const string OriginalHorseCurve = "Original Horse Curve";
        public const string BackPlateCurve = "BackPlateCurve";
        public const string BackPlateSurface = "BackPlateSurface";
        public const string LeftHnPoint = "Left HN Point";
        public const string RightHnPoint = "Right HN Point";
        public const string PipeSurface = "PipeSurface";
        public const string PipeCurve = "PipeCurve";
        //public const string OffsetTargetSurface = "OffsetTargetSurface";
        public const string PalatinePlane = "PalatinePlane";
        public const string MidPointProjectedHorseCurve = "MidPointProjectedHorseCurve";
        public const string MidpointProjectdGingival = "MidpointProjectdGingival";
        public const string FirstRadialExtend = "FirstRadialExtention";
        public const string SecondRadialExtend = "SecondRadialExtention";
        public static string ObiMesh = "ObiMesh";
        public static string InsideObiMesh = "InsideObiMesh";
        public static string Patch = "Patch";
        public static string SafePatch = "SafePatch";
        public static string SupportPillarsForMandible = "SupportPillarsForMandible";
        public static string SupportPillarsForMaxillar = "SupportPillarsForMaxillar";
        public static string SupportCurve_Inner_normal = "SupportCurve_Inner_normal";
        public static string SupportCurve_Inner_swing_front = "SupportCurve_Inner_swing_front";
        public static string SupportCurve_Inner_swing_back = "SupportCurve_Inner_swing_back";
        public static string SupportCurve_Outer_normal = "SupportCurve_Outer_normal";
        public static string SupportCurve_Outer_swing_front = "SupportCurve_Outer_swing_front";
        public static string SupportCurve_Outer_swing_back = "SupportCurve_Outer_swing_back";
        public static string ConnectionLine = "ConnectionLine";
        public static string SupportCAMBound = "SupportCAMBound";
        public static string SupportCAMBound2 = "SupportCAMBound2";
        public static string FlippedSupportCAMBound2 = "FlippedSupportCAMBound2";
        public static string PlateBound = "PlateBound";
        public static string PatchSurface = "PatchSurface";
        public static string Garbage = "Garbage";
        public  static string FouthOffsetGin = "Fouth Offset Gin";
        public  static string JigHolder = "jig-holder-area";	 
        public static string ClosedGingival123 = "ClosedGingival123";
        public static string ClosedGingival4567 = "ClosedGingival4567";
        public static string Outline_A_17 = "Outline_A_17";
        public static string Outline_A17 = "Outline_A17";
        public static string SupportCAMBound0 = "SupportCAMBound0";
        public static string SupportCAMBound0N = "SupportCAMBound0N";
        public static string SupportCAMBound0S = "SupportCAMBound0S";
        public static string TopPlateOutline = "TopPlateOutline";
        public static string BottomPlateOutline = "BottomPlateOutline";
        public static string ForthHorseShoeAndPlateOutline = "ForthHorseShoeAndPlateOutline";
        //public static string FlippedOutline_A17 = "FlippedOutline_A_17";
        public static string Material = "Material";
        public static string GapAndContactTargetNorthCenter = "GapAndContactTargetNotrhCenter";
        public static string GapAndContactTargetWestBottom = "GapAndContactTargetWestBottom";
        public static string GapAndContactTargetEastBottom = "GapAndContactTargetEastBottom";
        public static string GapAndContactTargetPlaneProject = "GapAndContactTargetPlaneProject";
        public static string GapAndContactSourceNorthCenter = "GapAndContactSourceNotrhCenter";
        public static string GapAndContactSourceWestBottom = "GapAndContactSourceWestBottom";
        public static string GapAndContactSourceEastBottom = "GapAndContactSourceEastBottom";
        public static string GapAndContactSourcePlaneProject = "GapAndContactSourcePlaneProject";
        public static string GapAndContactOriginalTeeth = "GapAndContactOriginalTeeth";
        public static string GapAndContactTargetPC = "GapAndContactTargetPC";
        public static string GapAndContactSourcePC = "GapAndContactSourcePC";

        //Command String
        public const string ComExtractConnectedMeshFace = "-_ExtractConnectedMeshFaces ";
        public const string ComExtractMeshFacesByDraftAngle = "_-ExtractMeshFacesByDraftAngle ";
        public const string ComStartAngleFromCameraDir = "StartAngleFromCameraDir={0} ";
        public const string ComEndAngleFromCameraDir = "EndAngleFromCameraDir={0} ";
        public const string OptionSelectFacesLessThan = "SelectFaces=LessThan ";
        public const string OptionBorderOnlyNo = "BorderOnly=No ";
        public const string OptionBorderOnlyYes = "BorderOnly=Yes ";
        public const string OptionAngleForComExtract = "_Angle {0} {1},{2},{3} ";
        public const string ComPause = "_Pause ";
        public const string ComEnterEnd = "_EnterEnd ";
        public const string ComImport = "Import ";
        public const string ComCancel = "Cancel ";
        public const string ComSelNone = "SelNone "; //Deselected object.
        public const string ComZoomAllExtents = "Zoom All Extents ";
        public const string ComSetDisplayModeModeRendered = "SetDisplayMode Viewport=All Mode Rendered";
        public const string ComSetActiveViewPort = "SetActiveViewport Top";
        public const string ComSetMaximizedViewport = "SetMaximizedViewport Top";
        public const string ComClearUndo = "_ClearUndo ";
        public const string ComSelID = "SelID ";
        public const string ComSelIDDetail = "SelID {0} ";
        public const string ComPlaneThroughPt = "_PlaneThroughPt ";
        public const string ComProject = "_Project Loose=No DeleteInput=Yes";
        public const string ComPatch = "_Patch";
        public const string ComSelSrf = "_SelSrf";

        // adjust to data.
        public const string ComPatchDefault =
            "_-Patch PointSpacing=1  USpans=60  VSpans=60  Stiffness=7 AdjustTangency=Yes AutomaticTrim=Yes EnterEnd";
        public const string ComPatchDefault2 =
            "_-Patch PointSpacing=1.0  USpans=60  VSpans=60  Stiffness=7 AdjustTangency=Yes AutomaticTrim=Yes EnterEnd";
        public const string ComMesh = "_Mesh";
        public const string ComDupBoder = "_DupBorder";
        public const string ComSelectClpsestPointToPointB = "ClosestPt Object SelID";
        public const string StringSpace = " ";
        public const string ComCurveThroughPt = "_CurveThroughPt";
        public const string ComJoin = "_Join";
        public const string ComInterpCrv = "_InterpCrv";
        public const string ComScale2D = "Scale2D";
        public const string ComTrim = "_Trim";
        public const string ComSplitPoint = "_Split Point";
        public const string ComSplit = "_Split ";
        public const string ComCloseCrv = "CloseCrv";
        public const string ComSplitwithDetailPoint = "_Split Point {0} {1} {2} {3} {4} {5} ";

        public const string CentralIncisorLeft = "Central incisor in left";
        public const string CentralIncisorRight = "Central incisor in right";
        public const string LateralIncisorLeft = "Lateral Incisor left";
        public const string LateralIncisorRight = "Lateral Incisor right";
        public const string CuspidLeft = "Cuspid in left";
        public const string CuspidRight = "Cuspid in right";
        public const string FirstPremolorLeft = "First premolor in left";
        public const string FirstPremolorRight = "First premolor in right";
        public const string SecondPremolorLeft = "Second premolor in left";
        public const string SecondPremolorRight = "Second premolor in right";
        public const string FirstMolorLeft = "First molor in left";
        public const string FirstMolorRight = "First molor in right";
        public const string SecondMolorLeft = "Second molor in left";
        public const string SecondMolorRight = "Second molor in right";
        public const string MaxillaTray = "Maxilla Plate";
        public const string MandibleTray = "Mandible Plate";
        public const string No = "No";
        public const string Yes = "Yes";
        public const string DrawBottomEdgeCurve = "Draw Bottom Edge Cure";
        public const string TrimObject = "Trim Object";
        public const string Accept = "Accept";
        public const string Trim = "Trim";
        public const string DrawBottomCurve = "Draw Bottom Curve";
        public const string SplitTheCurve = "Split the Curve";
        

        //Infomation
        public const string InfoNoSelectedPlate = "No Selected Plate";
        public const string InfoAllDimplesIsNotDetected = "All Dimples Is Not Detected";
        public const string InfoNoMedianLine = "The Plate have no median line";
        public const string InfoMedianLineExist = "The median line is exist";
        public const string InfoNoInnerCurve = "You must create inner curve (DTCreateInnerCurve) first!";
        public const string InfoNoDetectMaxillaOrMandiblePlate = "You must identify plate's type (DTDetectMaxillaorMandibleTray)!";
        public const string InfoUnknowPlate = "You have selected unknown plate!";
        public const string InfoNoOffsetTargetSurface = "Unknow Offset Target Surface \n";
        public const string InfoNoBackPlateSurface = "Unknow Back Plate Surface \n";
        public const string InfoNoPipeCurve = "Unknow Pipe Curve";
        public const string InfoNoLeftSplitPointInEgdeInnerCurve = "Unknow left split point in EgdeInnerCurve";

        public static  List<string> DimpleNameList = new List<string>(new string[]
	    {
            CentralIncisorLeft,
            CentralIncisorRight,
            LateralIncisorLeft,
            LateralIncisorRight,
            CuspidLeft,
            CuspidRight,
            FirstPremolorLeft,
            FirstPremolorRight,
            SecondPremolorLeft,
            SecondPremolorRight,
            FirstMolorLeft,
            FirstMolorRight,
            SecondMolorLeft,
            SecondMolorRight
	        
	    });

        public static List<string> DimpleNameListToCreateGIngival = new List<string>(new string[]
	    {
            SecondMolorRight,
            FirstMolorRight,
            SecondPremolorRight,
            FirstPremolorRight,
            CuspidRight,
            LateralIncisorRight,
            CentralIncisorRight,
            CentralIncisorLeft,
            LateralIncisorLeft,
            CuspidLeft,
            FirstPremolorLeft,
            SecondPremolorLeft,
            FirstMolorLeft,
            SecondMolorLeft
	    });


        public static List<string> DimpleNameArrangement = new List<string>(
            new string[]
            {
                SecondMolorLeft,
                FirstMolorLeft,
                SecondPremolorLeft,
                FirstPremolorLeft,
                CuspidLeft,
                LateralIncisorLeft,
                CentralIncisorLeft,
                CentralIncisorRight,
                LateralIncisorRight,
                CuspidRight,
                FirstPremolorRight,
                SecondPremolorRight,
                FirstMolorRight,
                SecondMolorRight
            });

        #endregion
        #region Cosn Int
        public const int ExtendStartPoint = 10;
        public const int ExtendEndPoint = 40;
        public const int DistanceBetweenPointAandB = 30;
        public const int TouleranceProjectPointBToTray = 0;
        public const double RadiusOfCircleInMedianline = 10;
        public const int ExtendPalatinBone = 30;
        public const double ScaleInnerCure = 0.7;
        public const double LineExtendInnerCurve = 40;

        #endregion

    }
}
