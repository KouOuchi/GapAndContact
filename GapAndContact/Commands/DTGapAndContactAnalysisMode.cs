using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Denture.Commands;
using Denture.Utilities;
using GapCondition;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace GapAndContact.Commands
{
    /// <summary>
    /// This simple example provides a false color based on the world z-coordinate.
    /// For details, see the implementation of the FalseColor() function.
    /// </summary>
    class DTGapAndContactAnalysisMode : Rhino.Display.VisualAnalysisMode
    {
        Interval m_z_range = new Interval(-0.1, 0.1);
        Interval m_hue_range = new Interval(0, 0.5 * Math.PI / 3);
        bool m_show_isocurves = false;

        public override string Name { get { return "DTGapAndContactAnalysis"; } }
        public override Rhino.Display.VisualAnalysisMode.AnalysisStyle Style { get { return AnalysisStyle.FalseColor; } }

        public override bool ObjectSupportsAnalysisMode(Rhino.DocObjects.RhinoObject obj)
        {
            if (obj is Rhino.DocObjects.MeshObject && obj.Attributes.GetUserString("gapcon").Equals("source"))
                return true;
            return false;
        }

        protected override void UpdateVertexColors(Rhino.DocObjects.RhinoObject obj, Mesh[] meshes)
        {
            // A "mapping tag" is used to determine if the colors need to be set
            Rhino.Render.MappingTag mt = GetMappingTag(obj.RuntimeSerialNumber);

            //var meshObj = obj as MeshObject;
            //if (meshObj == null) return;
            //var meshTarget = meshObj.MeshGeometry;

            for (int mi = 0; mi < meshes.Length; mi++)
            {
                var meshPaint = meshes[mi];

                if (meshPaint.VertexColors.Tag.Id != this.Id)
                {
                    // The mesh's mapping tag is different from ours. Either the mesh has
                    // no false colors, has false colors set by another analysis mode, has
                    // false colors set using different m_z_range[]/m_hue_range[] values, or
                    // the mesh has been moved.  In any case, we need to set the false
                    // colors to the ones we want.
                    Color[] colors = new System.Drawing.Color[meshPaint.Vertices.Count];

                    for (int i = 0; i < meshPaint.Vertices.Count; i++)
                    {
                        Point3d point = meshPaint.Vertices[i];
                        SearchData data = new SearchData(
                            DTGapAndContactRtreeResource.GetInstance().MeshOfTree,
                            point);
                        // Use the first vertex in the mesh to define a start sphere
                        double distance = point.DistanceTo(
                            DTGapAndContactRtreeResource.GetInstance().MeshOfTree.Vertices[0]);
                        Sphere sphere = new Sphere(point, distance * 1.05);

                        if (DTGapAndContactRtreeResource.GetInstance().Tree.Search(sphere, SearchCallback, data))
                        {
                            if (i % 5000 == 0)
                            {
                                DisplayUtil.Print(string.Format("===>i:{0} dist:{1}", i, data.Distance));
                            }

                            colors[i] = DistanceColor(data.Distance);
                        }
                    }
                    meshPaint.VertexColors.SetColors(colors);
                    // set the mesh's color tag
                    meshPaint.VertexColors.Tag = mt;
                }
            }
        }

        public override bool ShowIsoCurves
        {
            get
            {
                // Most shaded analysis modes that work on breps have the option of
                // showing or hiding isocurves.  Run the built-in Rhino ZebraAnalysis
                // to see how Rhino handles the user interface.  If controlling
                // iso-curve visability is a feature you want to support, then provide
                // user interface to set this member variable.
                return m_show_isocurves;
            }
        }

        /// <summary>
        /// Returns a mapping tag that is used to detect when a mesh's colors need to
        /// be set.
        /// </summary>
        /// <returns></returns>
        Rhino.Render.MappingTag GetMappingTag(uint serialNumber)
        {
            Rhino.Render.MappingTag mt = new Rhino.Render.MappingTag();
            mt.Id = this.Id;

            // Since the false colors that are shown will change if the mesh is
            // transformed, we have to initialize the transformation.
            mt.MeshTransform = Transform.Identity;

            // This is a 32 bit CRC or the information used to set the false colors.
            // For this example, the m_z_range and m_hue_range intervals control the
            // colors, so we calculate their crc.
            uint crc = RhinoMath.CRC32(serialNumber, m_z_range.T0);
            crc = RhinoMath.CRC32(crc, m_z_range.T1);
            crc = RhinoMath.CRC32(crc, m_hue_range.T0);
            crc = RhinoMath.CRC32(crc, m_hue_range.T1);
            mt.MappingCRC = crc;
            return mt;
        }

        System.Drawing.Color DistanceColor(double z)
        {
            // range is sorted
            var range =
                DTGapAndContactPanel.GetPanelInstance().GetMinDistanceRanges();

            // error;
            if (range.Count == 0) return Color.White;

            Color target_color = range.Values.ElementAt(0);

            foreach (double k in range.Keys)
            {
                if (k >= z)
                    break;

                target_color = range[k];
            }

            return target_color;
        }

        void SearchCallback(object sender, RTreeEventArgs e)
        {
            SearchData data = e.Tag as SearchData;
            if (data == null)
                return;
            data.HitCount = data.HitCount + 1;
            Point3f vertex = data.Mesh.Vertices[e.Id];
            double distance = data.Point.DistanceTo(vertex);
            if (data.Index == -1 || data.Distance > distance)
            {
                // shrink the sphere to help improve the test
                e.SearchSphere = new Sphere(data.Point, distance);
                data.Index = e.Id;
                data.Distance = distance;
            }
        }


        class SearchData
        {
            public SearchData(Mesh mesh, Point3d point)
            {
                Point = point;
                Mesh = mesh;
                HitCount = 0;
                Index = -1;
                Distance = 0;
            }

            public int HitCount { get; set; }
            public Point3d Point { get; private set; }
            public Mesh Mesh { get; private set; }
            public int Index { get; set; }
            public double Distance { get; set; }
        }


    }

}