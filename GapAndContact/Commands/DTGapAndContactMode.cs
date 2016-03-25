using System;
using System.Collections.Generic;
using Denture.Utilities;
using GapCondition;
using GapAndContact.Commands;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace Denture.Commands
{
    [System.Runtime.InteropServices.Guid("B7D32D59-BCD8-4D1D-8D1D-091EA974836D")]
    public class DTGapAndContactModeOn : Rhino.Commands.Command
    {

        public override string EnglishName { get { return "DTGapAndContactModeOn"; } }

        protected override Rhino.Commands.Result RunCommand(RhinoDoc doc, Rhino.Commands.RunMode mode)
        {
            // make sure our custom visual analysis mode is registered
            var zmode = Rhino.Display.VisualAnalysisMode.Register(typeof(DTGapAndContactAnalysisMode));

            RhinoApp.WriteLine("GapAndContactAnalysis is on.");

            var filter = Rhino.DocObjects.ObjectType.Mesh;
            var objs = doc.Objects.FindByObjectType(filter);
            int count = 0;
            foreach (var obj in objs)
            {
                // see if this object is alreay in Z analysis mode
                if (obj.InVisualAnalysisMode(zmode))
                    continue;

                if (obj.EnableVisualAnalysisMode(zmode, true))
                    count++;

            }

            if (count > 0)
            {
                prepare(doc);

                if (!DTGapAndContactPanel.GetPanelInstance().IsVisible)
                {
                    // show panel
                    DTGapAndContactPanel.GetPanelInstance().Show();
                }

                return Rhino.Commands.Result.Success;
            }
            else
            {
                return Rhino.Commands.Result.Failure;
            }
        }

        private void prepare(RhinoDoc doc)
        {
            DTGapAndContactRtreeResource.DestroyInstance();

            IList<Mesh> meshes = new List<Mesh>();
            foreach (var o in doc.Objects)
            {
                if (o.Attributes.GetUserString("gapcon").Equals("target"))
                {
                    Mesh mesh = o.Geometry as Mesh;
                    if (mesh != null)
                    {
                        // initilize resource
                        DTGapAndContactRtreeResource.GetInstance().AddMesheOfTree(mesh);
                        break;
                    }
                }
            }
        }
    }

    [System.Runtime.InteropServices.Guid("93B6719C-E7C4-45D2-97EC-E9A2A1EF9E9E")]
    public class DTGapAndContactModeOff : Rhino.Commands.Command
    {
        public override string EnglishName { get { return "DTGapAndContactModeOff"; } }

        protected override Rhino.Commands.Result RunCommand(RhinoDoc doc, Rhino.Commands.RunMode mode)
        {
            var zmode = Rhino.Display.VisualAnalysisMode.Find(typeof(DTGapAndContactAnalysisMode));
            // If zmode is null, we've never registered the mode so we know it hasn't been used
            if (zmode != null)
            {
                foreach (Rhino.DocObjects.RhinoObject obj in doc.Objects)
                {
                    obj.EnableVisualAnalysisMode(zmode, false);
                }
                doc.Views.Redraw();
            }
            RhinoApp.WriteLine("GapAndContactAnalysis is off.");

            // close panel
            DTGapAndContactPanel.DestroyPanelInstance();

            return Rhino.Commands.Result.Success;
        }
    }

    public class DTGapAndContactRtreeResource
    {
        private static DTGapAndContactRtreeResource instance;

        private RTree tree = null;
        private Mesh mesh = null;

        public static DTGapAndContactRtreeResource GetInstance()
        {
            if (instance == null)
                instance = new DTGapAndContactRtreeResource();

            return instance;
        }

        public static void DestroyInstance()
        {
            if (instance != null)
            {
                if (instance.tree != null)
                {
                    instance.tree.Clear();
                }

                instance.mesh = null;
                instance = null;
            }
        }

        protected DTGapAndContactRtreeResource()
        {
            tree = new RTree();
            mesh = null;
        }

        #region operations and properties
        public void AddMesheOfTree(Mesh m)
        {
            mesh = m;

            int elementId = 0;
            for (int i = 0; i < m.Vertices.Count; i++)
            {
                tree.Insert(m.Vertices[i], elementId++);
            }

        }

        public RTree Tree
        {
            get
            {
                return tree;
            }
        }

        public Mesh MeshOfTree
        {
            get
            {
                return mesh;
            }
        }
        #endregion
    }
}