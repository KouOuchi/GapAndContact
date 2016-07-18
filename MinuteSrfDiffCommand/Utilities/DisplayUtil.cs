using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Rhino;
using Rhino.DocObjects;

namespace Denture.Utilities
{
    public static class DisplayUtil
    {
        #region Func
        /// <summary>
        /// Change object color
        /// </summary>
        /// <param name="rhino_object"></param>
        /// <param name="doc"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool ChangeColor(RhinoObject rhino_object, RhinoDoc doc, Color color)
        {
            if (rhino_object != null)
            {
                // Make sure the object has it's material source set to "material_from_object"
                rhino_object.Attributes.MaterialSource = Rhino.DocObjects.ObjectMaterialSource.MaterialFromObject;

                // Make sure the object has a material assigned
                int material_index = rhino_object.Attributes.MaterialIndex;
                if (material_index < 0)
                {
                    // Create a new material based on Rhino's default material
                    material_index = doc.Materials.Add();
                    // Assign the new material (index) to the object.
                    rhino_object.Attributes.MaterialIndex = material_index;
                }

                if (material_index >= 0)
                {
                    Rhino.DocObjects.Material mat = doc.Materials[material_index];
                    mat.AmbientColor = color;
                    mat.DiffuseColor = color;
                    mat.EmissionColor = color;
                    mat.CommitChanges();

                    //Don't forget to update the object, if necessary
                    rhino_object.CommitChanges();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Print note on Rhino board  
        /// </summary>
        /// <param name="text"></param>
        public static void Print(string text)
        {
            RhinoApp.WriteLine(text);
        }
        #endregion
    }
}
