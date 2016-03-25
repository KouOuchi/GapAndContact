using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Denture.Utilities;
using GapCondition;
using GapAndContact.Commands;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using RMA.Rhino;
using Common;
using Rhino.UI;
using System.IO;
using GapAndContact.Utilities;
using System.Diagnostics;

namespace Denture.Commands
{

    [System.Runtime.InteropServices.Guid("D973C31C-15A1-43C1-91C7-A9475B9F8270"),
    Rhino.Commands.CommandStyle(Rhino.Commands.Style.ScriptRunner)]
    public class DTGapAndContactBasic : Command
    {
        #region Member
        private double DIV = 10.0;

        private static readonly log4net.ILog log = LogManagerWrapper.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static DTGapAndContactBasic _instance;
        #endregion

        #region Constructor
        public DTGapAndContactBasic()
        {
            _instance = this;
        }
        #endregion

        ///<summary>The only instance of the DTMedienLine command.</summary>
        public static DTGapAndContactBasic Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "DTGapAndContactBasic"; }
        }

        /// <summary>
        /// The command draw the median line based on two points.
        /// Two point are mid point of two centran incisor dimple and two cuspid dimple.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Result rh = Result.Nothing;

            try
            {
                ObjectUtil.DeSelectObject();

                List<RhinoObject> sources =
                    ObjectUtil.SelectMultiRhinoObjectByUser(ObjectType.Mesh, "Select Source Meses");

                ObjectUtil.DeSelectObject();

                List<RhinoObject> targets =
                    ObjectUtil.SelectMultiRhinoObjectByUser(ObjectType.Mesh, "Select Target Meses");

                foreach (var m in sources)
                {
                    m.Attributes.SetUserString("gapcon", "source");
                    m.CommitChanges();
                }

                foreach (var m in targets)
                {
                    m.Attributes.SetUserString("gapcon", "target");
                    m.CommitChanges();
                }

                RhinoApp.RunScript("_DTGapAndContactModeOn", true);
                doc.Views.Redraw();
            }
            catch (Exception e)
            {
                log.Error(e);
                DisplayUtil.Print(string.Format("Error. {0}", e.Message));
                rh = Result.Failure;
            }
            finally
            {
                doc.Views.Redraw();
            }

            return rh;
        }
    }
}
