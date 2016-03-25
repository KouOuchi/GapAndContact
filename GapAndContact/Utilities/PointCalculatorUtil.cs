using System;
using System.Collections.Generic;
using System.Linq;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using RMA.Rhino;

namespace Denture.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    [Rhino.Commands.CommandStyle(Rhino.Commands.Style.ScriptRunner)]
    public static class PointCalculatorUtil
    {
       
#region Static Func
        /// <summary>
        /// Get mid point between two points.
        /// </summary>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <returns>Poind3d</returns>
        public static Point3d GetMidPointInTwoPoints(Point3d pointA, Point3d pointB)
        {
            return new Point3d((pointA.X + pointB.X) / 2, 
                               (pointA.Y + pointB.Y) / 2, 
                               (pointA.Z + pointB.Z) / 2);
        }

        /// <summary>
        /// Get the mid point in Ienumerator<Point3f>
        /// </summary>
        /// <param name="lsPoint"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Point3f GetMidPoindInList(IEnumerator<Point3f> lsPoint,int count)
        {
            Point3f midPoint = new Point3f(0,0,0);
            while (lsPoint.MoveNext())
            {
                midPoint.X += lsPoint.Current.X;
                midPoint.Y += lsPoint.Current.Y;
                midPoint.Z += lsPoint.Current.Z;
            }
            
            return new Point3f(midPoint.X/count, 
                               midPoint.Y/count,
                               midPoint.Z/count);
        }

        /// <summary>
        /// Get the mid point in List
        /// </summary>
        /// <param name="lsPoint"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Point3d GetMidPoindInList(Point3d[] lsPoint, int count)
        {
            Point3d midPoint = new Point3d(0, 0, 0);
            foreach (var point3D in lsPoint)
            {
                midPoint.X += point3D.X;
                midPoint.Y += point3D.Y;
                midPoint.Z += point3D.Z;
            }

            return new Point3d(midPoint.X / count,
                               midPoint.Y / count,
                               midPoint.Z / count);
        }

        /// <summary>
        ///Get the mid point of the box that bounding selected mesh. 
        /// </summary>
        /// <returns>Point3d</returns>
        public static Point3d GetMidPointFromBox(MeshObject meshobj)
        {
            return meshobj.MeshGeometry.GetBoundingBox(true).Center;//Get the bounding box of the selected mesh    

        }

        /// <summary>
        /// Get the mid point that inside the selected mesh
        /// </summary>
        /// <returns></returns>
        public static Point3d GetMidPointInMesh(MeshObject meshobj)
        {
            if (meshobj != null)
            {
                IEnumerator<Point3f> lspoint = meshobj.MeshGeometry.Vertices.GetEnumerator();
                lspoint.MoveNext();

                return GetMidPoindInList(lspoint, meshobj.MeshGeometry.Vertices.Count);
            }

            return Point3d.Unset;
        }
        /// <summary>
        /// Get distance between two points
        /// </summary>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <returns></returns>
        public static double DistanceBetweenTwoPoints(Point3d pointA, Point3d pointB)
        {
            return pointA.DistanceTo(pointB);
        }

        /// <summary>
        /// Get closest point between two meshes
        /// </summary>
        /// <param name="meshStart"></param>
        /// <param name="meshTarget"></param>
        /// <returns></returns>
        public static Point3d ClosestPointBetweenTwoMesh(Mesh meshStart, Mesh meshTarget)
        {
            var vertices = meshStart.Vertices.GetEnumerator();

            List<Point3d> points = new List<Point3d>();
            while (vertices.MoveNext())
            {
                var vertex = vertices.Current;
                points.Add(new Point3d(vertex));
            }

            Dictionary<Point3d, double> dictances = new Dictionary<Point3d, double>();
            if (points.Count != 0)
            {
                foreach (var point in points)
                {
                    try
                    {
                        if (!dictances.ContainsKey(point))
                            dictances.Add(point, point.DistanceTo(meshTarget.ClosestPoint(point)));
                    }
                    catch (Exception ex)
                    {
                        RhUtil.RhinoApp().Print(ex.ToString());
                    }

                }

                if (dictances.Count != 0)
                {
                    List<KeyValuePair<Point3d, double>> sortList = dictances.ToList();
                    sortList.Sort((firstPair, nextPair) =>
                    {
                        return firstPair.Value.CompareTo(nextPair.Value);
                    });

                    return sortList[0].Key;
                }
            }
            return Point3d.Unset;
        }

        public static Point3d ClosestPointBetweenMeshAndLine(
            Mesh mesh, Line line)
        {
            var vertices = mesh.Vertices.GetEnumerator();
            double minDist = double.MaxValue;
            Point3d minPt = Point3d.Unset;
            while (vertices.MoveNext())
            {
                var vertex = vertices.Current;
                Point3d pt = new Point3d(vertex);
                double dist = line.DistanceTo(pt, false);
                if (dist < minDist)
                {
                    minDist = dist;
                    minPt = pt;
                }
            }

            return minPt;
        }

        public static Point3d GetPoint(RhinoDoc doc, string layerName, int parentLayerIndex)
        {
            if (doc.Layers[parentLayerIndex] != null)
            {
                Layer[] layers = doc.Layers[parentLayerIndex].GetChildren();
                foreach (var layer in layers)
                {
                    if (layer.Name == layerName)
                    {
                        RhinoObject[] objects = doc.Objects.FindByLayer(layer);
                        if (objects.Length != 0)
                        {
                            if (objects.First().ObjectType == ObjectType.Point)
                                return (objects.First() as PointObject).PointGeometry.Location;
                        }
                    }
                }
            }
            return Point3d.Unset;
        }

        public static List<Point3d> GetIntersectionBetweenTwoCurve( Curve ficurve,Curve seCurve)
        {
            CurveIntersections secobj = Rhino.Geometry.Intersect.Intersection.CurveCurve(ficurve,
                        seCurve, 0.01, 0.02);
            var sec = secobj.GetEnumerator();
            Point3d point = Point3d.Unset;
            List<Point3d> secpoints = new List<Point3d>();
            while (sec.MoveNext())
            {
                secpoints.Add(sec.Current.PointA);
            }
            return secpoints;
        }

        public static Point3d GetMaxZPoint(Point3d[] points)
        {
            if (points.Length == 0) return Point3d.Unset;

            SortedDictionary<double, Point3d> sorter
                = new SortedDictionary<double,Point3d>();

            foreach (var p in points)
                sorter.Add(p.Z, p);

            return sorter.Last().Value;
        }

        #endregion
    }
}
