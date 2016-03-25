using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using Rhino.Geometry.Intersect;
using RMA.OpenNURBS.ValueTypes;
using Point3d = Rhino.Geometry.Point3d;
using System.Diagnostics;

namespace Denture.Utilities
{
    [Rhino.Commands.CommandStyle(Rhino.Commands.Style.ScriptRunner)]
    public static class ObjectUtil
    {
        /// <summary>
        /// Get mesh object which is extracted by selected mesh face.
        /// </summary>
        /// <returns>Rhino Object</returns>
        public static RhinoObject GetMeshObjectByExtract(ObjectType objectType, string displaytext)
        {
            if (Rhino.RhinoApp.RunScript(ResourceUtil.ComExtractConnectedMeshFace, false)) //Call ExtractConnectedMeshFaces Rhino command
            {
                return SelectRhinoObjectByUser(objectType, displaytext);
            }
            return null;
        }

        /// <summary>
        /// Get a object which is selected by user.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="displaytext"></param>
        /// <returns></returns>
        public static RhinoObject SelectRhinoObjectByUser(ObjectType objectType, string displaytext)
        {
            ObjRef refObjectMesh;
            var rc = Rhino.Input.RhinoGet.GetOneObject(displaytext, false, objectType, out refObjectMesh);
            if (rc != Result.Success)
            {
                return null;
            }

            var meshobj = refObjectMesh.Object();  //Get the object.
            if (meshobj != null)
                return meshobj;
            
            return null;
        }

        /// <summary>
        /// Get a object which is selected by user.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="displaytext"></param>
        /// <returns></returns>
        public static ObjRef SelectRhinoObjref(ObjectType objectType, string displaytext)
        {
            ObjRef refObjectMesh;
            var rc = Rhino.Input.RhinoGet.GetOneObject(displaytext, false, objectType, out refObjectMesh);
            if (rc != Result.Success)
            {
                return null;
            }

            return refObjectMesh;
        }

        public static List<Point3d> IntersecCurveandCurve(Curve fiCurve, Curve seCurve)
        {
            List<Point3d> lspoint = new List<Point3d>();
            CurveIntersections secobj = Rhino.Geometry.Intersect.Intersection.CurveCurve(fiCurve, seCurve, 0.01, 0.02);
            var sec = secobj.GetEnumerator();
            if (sec != null)
            {
                while (sec.MoveNext())
                {
                    if (sec.Current != null)
                    {
                        lspoint.Add(sec.Current.PointA);
                    }
                }
            }

            return lspoint;
        }

        /// <summary>
        /// Get multi objects which is selected by user.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="displaytext"></param>
        /// <returns></returns>
        public static List<RhinoObject> SelectMultiRhinoObjectByUser(ObjectType objectType, string displaytext)
        {
            ObjRef[] refObjectMeshs;
            var rc = Rhino.Input.RhinoGet.GetMultipleObjects(displaytext, false, objectType, out refObjectMeshs);
            if (rc != Result.Success)
            {
                return null;
            }

            List<RhinoObject> objects = new List<RhinoObject>();
            foreach (var mesh in refObjectMeshs)
            {
                var meshobj = mesh.Object();  //Get the object.
                if (meshobj != null)
                    objects.Add(meshobj);
            }

            return objects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static MeshObject GetDimple(string text)
        {
            // Select the mesh face that is the start face to extract.
            if (SelectRhinoObjectByUser(ObjectType.MeshFace, ResourceUtil.Sel + text) != null)
            {
                return (MeshObject) GetMeshObjectByExtract(ObjectType.Mesh, ResourceUtil.SelMesh);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curves"></param>
        /// <returns></returns>
        public static Curve GetLongestCurve(Curve[] curves)
        {
            Curve curve = null;
            double len = double.MinValue;
            foreach (var curve1 in curves)
            {
                if (len < curve1.GetLength())
                {
                    len = curve1.GetLength();
                    curve = curve1;
                }
            }
            return curve;
        }

        public static Curve GetHighestCurve(Curve[] curves)
        {
            Curve crv = null;
            double maxH = double.MinValue;
            foreach (var c in curves)
            {
                var bbox = c.GetBoundingBox(true);
                if (bbox.Max.Z > maxH)
                {
                    crv = c;
                    maxH = bbox.Max.Z;
                }
            }
            return crv;
        }


        /// <summary>
        /// Call ClearUndo command in rhino
        /// This will clear the undo buffer and avoid accumulating geometry on the undo stack.
        /// </summary>
        public static void FreeMemory()
        {
            Rhino.RhinoApp.RunScript(ResourceUtil.ComClearUndo, false);
        }
        
        /// <summary>
        /// Select object by Id
        /// </summary>
        /// <param name="objecId"></param>
        public static bool SelectObjectByID(Guid objecId)
        {
            string comformat = string.Format(ResourceUtil.ComSelID + "{0}", objecId);
            return RhinoApp.RunScript(comformat, true);
        }
        /// <summary>
        /// Deselect object
        /// </summary>
        public static void DeSelectObject()
        {
            RhinoApp.RunScript(ResourceUtil.ComSelNone, true);
        }

        public static void DeselectAll(RhinoDoc doc)
        {
            IEnumerable<RhinoObject> selectedObjs = doc.Objects.GetSelectedObjects(false, false);
            foreach (RhinoObject obj in selectedObjs)
            {
                doc.Objects.Select(obj.Id, false);
            }
            doc.Views.Redraw();
        }

        public static void SelectAnObject(RhinoDoc doc, Guid id)
        {
            doc.Objects.Select(id, true);
            doc.Views.Redraw();
        }

        /// <summary>
        /// Get the plane in a brepobj
        /// BrepObject is a surface.
        /// </summary>
        /// <param name="brepObj"></param>
        /// <returns></returns>
        public static Rhino.Geometry.Plane GetPlane(BrepObject brepObj)
        {
            Rhino.Geometry.Plane plane = new Rhino.Geometry.Plane();

            if (brepObj.BrepGeometry.IsSurface)
            {
                BrepSurfaceList lsSurfaceList = brepObj.BrepGeometry.Surfaces;
                IEnumerator<Surface> ienumSurface = lsSurfaceList.GetEnumerator();
                ienumSurface.MoveNext();
                Surface surface = ienumSurface.Current;
                if (surface != null)
                {
                    surface.TryGetPlane(out plane);
                    return plane;
                }
            }
            return Rhino.Geometry.Plane.Unset;
        }

        /// <summary>
        ///Get surface from brepObject 
        /// </summary>
        /// <param name="brepObj"></param>
        /// <returns></returns>
        public static Rhino.Geometry.Surface GetSurface(BrepObject brepObj)
        {
            if (brepObj.BrepGeometry.IsSurface)
            {
                BrepSurfaceList lsSurfaceList = brepObj.BrepGeometry.Surfaces;
                IEnumerator<Surface> ienumSurface = lsSurfaceList.GetEnumerator();
                ienumSurface.MoveNext();
                return ienumSurface.Current;
            }
            return null;
        }

        /// <summary>
        ///Extend value Nord,East,West,South. 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="extendvalue"></param>
        /// <returns></returns>
        public static Surface ExtendSurface(Surface surface,double extendvalue)
        {
            surface = surface.Extend(IsoStatus.East, extendvalue,true);
            surface = surface.Extend(IsoStatus.North, extendvalue, true);
            surface = surface.Extend(IsoStatus.West, extendvalue, true);
            surface = surface.Extend(IsoStatus.South, extendvalue, true);

            return surface;
        }

        /// <summary>
        /// Transform plate
        /// </summary>
        /// <param name="plateId"></param>
        /// <param name="trans"></param>
        /// <param name="doc"></param>
        public static Guid TransformPlate(Guid plateId, int parentIndex,Transform trans, RhinoDoc doc)
        {
            Guid newPlateId = doc.Objects.Transform(plateId, trans, true); // transform the plate
            if(newPlateId != plateId)
            LayerUtil.UpdateParentLayer(doc, parentIndex,string.Format(newPlateId+ ResourceUtil.Parent),doc.Objects.Find(newPlateId));
            
            //Transform all object in parent layer
            Layer[] layers = doc.Layers[parentIndex].GetChildren();
            if (layers != null)
            {
                foreach (var layer in layers)
                {
                    RhinoObject[] obj = doc.Objects.FindByLayer(layer);
                    if (obj != null)
                    {
                        foreach (var rhinoObject in obj)
                        {
                            Guid newObjId = doc.Objects.Transform(rhinoObject, trans, true);
                            if(newObjId != rhinoObject.Id)
                                LayerUtil.UpdateChildLayer(doc,parentIndex,layer.Name,doc.Objects.Find(newObjId));
                        }
                    }
                }
            }

            doc.Views.Redraw();
            //RenameKey(Repository.Plates, plateId, newPlateId);
            return newPlateId;
        }
        /// <summary>
        /// Get all object in layer
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parentIndex"></param>
        /// <param name="childlayerName"></param>
        /// <returns></returns>
        public static RhinoObject[] GetObjectinLayer(RhinoDoc doc, int parentIndex, string childlayerName)
        {
            RhinoObject[] obj = null;
            if (doc.Layers[parentIndex] != null)
            {
                Layer[] layers = doc.Layers[parentIndex].GetChildren();
                if (layers.Length != 0)
                {
                    foreach (var layer in layers)
                    {
                        if (layer.Name == childlayerName)
                        {
                            obj = doc.Objects.FindByLayer(layer);
                        }
                    }
                }
            }
            return obj;
        }
        /// <summary>
        /// Replace key in dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="fromKey"></param>
        /// <param name="toKey"></param>
        public static void RenameKey<TKey, TValue>(IDictionary<TKey, TValue> dic,
                                      TKey fromKey, TKey toKey)
        {
            TValue value = dic[fromKey];
            dic.Remove(fromKey);
            dic.Add(toKey, value);
        }
        /// <summary>
        /// Auto select the plate in current layer
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        internal static MeshObject GetTargetPlate(RhinoDoc doc)
        {
            // get current layer name
            string layername = doc.Layers.CurrentLayer.Name;

            // Get all of the objects on the layer. If layername is bogus, 
            // return null.
            Rhino.DocObjects.RhinoObject[] rhobjs = doc.Objects.FindByLayer(layername);
            if (rhobjs == null || rhobjs.Length < 1)
                return null;

            for (int i = 0; i < rhobjs.Count(); i++)
            {
                if (rhobjs[i].ObjectType == ObjectType.Mesh)
                {
                    return (MeshObject)rhobjs[i];
                }
            }

            return null;
        }

        /// <summary>
        /// To get difference 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static RhinoObject[] GetObjectInLayer(RhinoDoc doc, Guid layer_id)
        {
           return doc.Objects.FindByLayer(doc.Layers[doc.Layers.Find(layer_id, true)]);
        }
        public static void GetObjectDiffInLayer(RhinoDoc doc, Guid layer_id, RhinoObject[] prevObjs, out RhinoObject[] createdObjs, out RhinoObject[] deletedObjs)
        {
            RhinoObject[] curObjs = GetObjectInLayer(doc, layer_id);
            IList<RhinoObject> created = new List<RhinoObject>();
            IList<RhinoObject> deleted = new List<RhinoObject>();

            foreach (RhinoObject c in curObjs)
            {
                bool found = false;
                foreach (RhinoObject p in prevObjs)
                {
                    if (c.Id.Equals(p.Id))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) created.Add(c);
            }
            foreach (RhinoObject p in prevObjs)
            {
                bool found = false;
                foreach (RhinoObject c in curObjs)
                {
                    if (p.Id.Equals(c.Id))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) deleted.Add(p);
            }

            createdObjs = created.ToArray<RhinoObject>();
            deletedObjs = deleted.ToArray<RhinoObject>();
        }

        /// <summary>
        /// Create a dupboder of surface
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static CurveObject DupboderSurface(Brep surface,RhinoDoc doc)
        {
            Rhino.Geometry.Curve[] curves = surface.DuplicateEdgeCurves(true);
            double tol = doc.ModelAbsoluteTolerance * 2.1;
            curves = Rhino.Geometry.Curve.JoinCurves(curves, tol);
            return (CurveObject) doc.Objects.Find(doc.Objects.AddCurve(curves.First()));
        }

        public static List<Point3d> GetPointsFromCurve(CurveObject curveobj)
        {
            double parameterlenght;
            List<Point3d> points = new List<Point3d>();
            double le = curveobj.CurveGeometry.GetLength();
            if (curveobj.CurveGeometry.LengthParameter(le, out parameterlenght))
            {
                double step = parameterlenght / 15;
                double para = 0;

                while (para < parameterlenght)
                {
                    Point3d pointOnCurve;
                    pointOnCurve = curveobj.CurveGeometry.PointAt(para);
                    //Check point is on curve
                    {
                        double t;
                        curveobj.CurveGeometry.ClosestPoint(pointOnCurve, out t);
                        if(pointOnCurve.X ==  curveobj.CurveGeometry.PointAt(t).X)
                            points.Add(pointOnCurve);
                    }
                    para += step;
                }
                return points;
            }
            return points;
        }

        public static Curve OffsetCrv(Curve srcCrv, Point3d directionPoint, Rhino.Geometry.Vector3d normal, double offsetVal)
        {
            // Rebuild curve before offset
            int ptCnt = (int)srcCrv.GetLength();
            if (ptCnt < 3)
            {
                ptCnt = 3;
            }

            bool isSrcClosed = srcCrv.IsClosed;
            Curve result = null;
            while (ptCnt > 3)
            {
                var crv = srcCrv.Rebuild(ptCnt, 3, false);

                Curve[] crvList = crv.Offset(directionPoint, normal, offsetVal, 0.01, CurveOffsetCornerStyle.Smooth);
                if (crvList.Length > 1)
                {
                    crvList = Curve.JoinCurves(crvList, 0.01);
                }
                Debug.Assert(crvList.Length == 1);

                result = crvList[0];

                if (isSrcClosed)
                {
                    if (crvList[0].IsClosed)
                    {
                        break;
                    }
                    else
                    {
                        ptCnt /= 2;
                        if (ptCnt < 3)
                        {
                            ptCnt = 3;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public static Curve TrimCrvByCrv(Curve srcCrv, Curve cuttingCrv)
        {
            Curve crv = srcCrv.DuplicateCurve();
            CurveIntersections ints = Intersection.CurveCurve(crv, cuttingCrv, 0.01, 0.01);
            if (ints.Count != 2)
            {
                return crv;
            }

            double st1 = ints[0].ParameterA;
            double st2 = ints[1].ParameterA;
            if (st1 > st2)
            {
                double tmp = st2;
                st2 = st1;
                st1 = tmp;
            }
            Curve st1Crv = crv.Trim(crv.Domain.Min, st1);
            Curve st2Crv = crv.Trim(st2, crv.Domain.Max);

            double ct1 = ints[0].ParameterB;
            double ct2 = ints[1].ParameterB;
            if (ct1 > ct2)
            {
                double tmp = ct2;
                ct2 = ct1;
                ct1 = tmp;
            }
            Curve ctCrv = cuttingCrv.Trim(ct1, ct2);

            Curve[] jointCrvs = Curve.JoinCurves(new Curve[] { st1Crv, st2Crv, ctCrv }, 0.01);
            Debug.Assert(jointCrvs.Length == 1);

            return jointCrvs[0];
        }

        public static Curve GetClosestCurveToPoint(Curve[] curves, Rhino.Geometry.Point3d point, bool order)
        {
            Dictionary<Curve, double> diccurve = new Dictionary<Curve, double>();
            foreach (var curve in curves)
            {
                double t;
                curve.ClosestPoint(point, out t);
                diccurve.Add(curve, point.DistanceTo(curve.PointAt(t)));
            }

            List<KeyValuePair<Curve, double>> myList = diccurve.ToList();

            myList.Sort(
                delegate(KeyValuePair<Curve, double> firstPair,
                    KeyValuePair<Curve, double> nextPair)
                {
                    return firstPair.Value.CompareTo(nextPair.Value);
                }
                );
            if (order)
                return myList.First().Key;
            else
            {
                return myList.Last().Key;
            }
        }

        public static Point3d GetClosestPoint(Point3d[] points, Point3d targetpoint)
        {
            double len = double.MaxValue;
            Point3d poi = Point3d.Unset;
            foreach (var point3D in points)
            {
                if (len > targetpoint.DistanceTo(point3D))
                {
                    poi = point3D;
                    len = targetpoint.DistanceTo(point3D);
                }
            }
            return poi;
        }
    }
}
