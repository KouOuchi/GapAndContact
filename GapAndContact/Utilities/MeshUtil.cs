using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using RMA.Rhino;

namespace Denture.Utilities
{
    internal static class MeshUtil
    {
        #region Func
        public static Guid CreateMeshOutLine(RhinoDoc doc, int parentIndex, MeshObject plate,
            double angle, double offset)
        {
            ObjectUtil.DeSelectObject();

            if (angle != 0.0)
            {
                RhinoApp.RunScript(string.Format("CPlane Rotate X {0} _Enter", angle), true);
                DisplayUtil.Print("rotate");
            }

            RhinoObject[] curObjs = ObjectUtil.GetObjectInLayer(doc, doc.Layers.CurrentLayer.Id);

            ObjectUtil.SelectObjectByID(plate.Id);
            RhinoApp.RunScript("MeshOutline", true);

            DisplayUtil.Print("meshoutline");

            // get difference from plate layer objects
            RhinoObject[] created = null;
            RhinoObject[] deleted = null;
            ObjectUtil.GetObjectDiffInLayer(doc, doc.Layers.CurrentLayer.Id, curObjs, out created, out deleted);

            List<Curve> target_cvs = new List<Curve>();
            foreach (var c in created)
            {
                Curve check = c.Geometry as Curve;
                if (check.GetLength() < 0.1) continue;

                target_cvs.Add(check);
            }

            Curve[] joinCrv = Rhino.Geometry.Curve.JoinCurves(target_cvs);
            Guid joinId = doc.Objects.AddCurve(joinCrv[0]);
            doc.Views.Redraw();
            ObjectUtil.DeSelectObject();
            doc.Objects.Find(joinId).Select(true);

            // cleanup curve
            RhinoApp.RunScript("-_Rebuild PointCount=150 Degree=3 DeleteInput=Yes OutPutLayer=Input _Enter", true);
            doc.Objects.Find(joinId).Select(true);
            RhinoApp.RunScript("-_Fair Tolerance=0.01 _Enter", true);

            //RhinoApp.RunScript("-_RebuildCrvNonUniform RequestedTolerance=0.01 MaxPointCount=150 DeleteInput=Yes _Enter", true);

            // offset curve if needed
            Curve projected_target = null;
            Curve target_cv = doc.Objects.Find(joinId).Geometry as Curve;
            if (target_cv == null) return Guid.Empty;

            if (offset != 0.0)
            {
                // 1999-2379/1766-2125 fallback
                Curve[] boundProjOffs = target_cv.Offset(doc.Views.ActiveView.ActiveViewport.ConstructionPlane(), offset, 0.01,
                    CurveOffsetCornerStyle.Smooth);

                if (boundProjOffs == null)
                {
                    boundProjOffs = target_cv.Offset(doc.Views.ActiveView.ActiveViewport.ConstructionPlane(), offset, 0.1,
                       CurveOffsetCornerStyle.Smooth);
                }

                Curve[] bou = null;

                if (boundProjOffs.Length != 1)
                {
                    bou = Rhino.Geometry.Curve.JoinCurves(boundProjOffs);
                }
                else
                {
                    bou = boundProjOffs;
                }

                projected_target = Rhino.Geometry.Curve.ProjectToPlane(bou[0],
                    doc.Views.ActiveView.ActiveViewport.ConstructionPlane());
            }
            else
            {
                projected_target = Rhino.Geometry.Curve.ProjectToPlane(target_cv,
                    doc.Views.ActiveView.ActiveViewport.ConstructionPlane());
            }

            if (angle != 0.0)
            {
                RhinoApp.RunScript("_CPlane _Previous _Enter", true);
            }

            // remove gurbage
            doc.Objects.Delete(joinId, true);

            return doc.Objects.AddCurve(projected_target);
        }
        #endregion
    }
}
