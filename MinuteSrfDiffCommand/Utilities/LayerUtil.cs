using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace Denture.Utilities
{
    public static class LayerUtil
    {
        /// <summary>
        /// Create a new layer as a parent and save the object to it
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="obj"></param>
        public static Guid CreateParentLayer(RhinoDoc doc, RhinoObject obj)
        {
            //Create a new parent Layer.
            Layer parent = new Layer();
            parent.Name = string.Format(obj.Id + ResourceUtil.Parent);
            if (doc.Layers.Find(parent.Name, true) < 0)
            {
                int parentIndex = doc.Layers.Add(parent);
                obj.Attributes.LayerIndex = parentIndex;
                obj.CommitChanges();
                return doc.Layers[parentIndex].Id;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Add the sub layer of the parent
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parentLayerId"></param>
        /// <param name="obj"></param>
        /// <param name="childLayerName"></param>
        public static Guid CreateChildLayer(RhinoDoc doc, Guid parentLayerId, RhinoObject obj, string childLayerName)
        {
            if (doc.Layers.Find(parentLayerId, true) >= 0)
            {
                Layer parent = doc.Layers[doc.Layers.Find(parentLayerId, true)];
                Layer childLayer = null;
                int childLayerindex = 0;
                Layer[] layers = parent.GetChildren();
                if (layers != null)
                {
                    foreach (var layer in parent.GetChildren())
                    {
                        if (layer.Name == childLayerName)
                        {
                            childLayer = layer;
                            childLayerindex = layer.LayerIndex;
                            break;
                        }
                    }
                    if (childLayer == null)
                    {
                        childLayer = new Layer() { Name = childLayerName };
                        childLayer.ParentLayerId = parentLayerId;
                        childLayerindex = doc.Layers.Add(childLayer);
                    }
                }
                else
                {
                    childLayer = new Layer() { Name = childLayerName };
                    childLayer.ParentLayerId = parentLayerId;
                    childLayerindex = doc.Layers.Add(childLayer);

                }

                //obj.Attributes.LayerIndex = doc.Layers.Find(childLayerName,false);
                obj.Attributes.LayerIndex = childLayerindex;
                obj.CommitChanges();
                return doc.Layers[obj.Attributes.LayerIndex].Id;
            }
            return Guid.Empty;
        }

        public static void DeleteChildLayer(RhinoDoc doc, int parentindex, string layername)
        {
            if (doc.Layers[parentindex] != null)
            {
                Layer[] layers = doc.Layers[parentindex].GetChildren();
                if (layers.Length != 0)
                {
                    foreach (var layer in layers)
                    {
                        if (layer.Name == layername)
                        {
                            if (doc.Objects.FindByLayer(layer).Length > 0)
                            {
                                foreach (var o in doc.Objects.FindByLayer(layer))
                                {
                                    doc.Objects.Delete(o.Id, true);
                                }
                            }

                            doc.Layers.Delete(doc.Layers.Find(layername, false), true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The plate has many object(store in layer)
        /// Return the object in plate.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parentindex"></param>
        /// <param name="layername"></param>
        /// <returns></returns>
        public static RhinoObject SearchingObjectInPlate(RhinoDoc doc, int parentindex, string layername)
        {
            if (doc.Layers[parentindex] != null)
            {
                Layer[] layers = doc.Layers[parentindex].GetChildren();
                if (layers.Length != 0)
                {
                    foreach (var layer in layers)
                    {
                        if (layer.Name == layername)
                        {
                            if (doc.Objects.FindByLayer(layer).Length > 0)
                                return doc.Objects.FindByLayer(layer).First();
                            else
                                return null;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// After you run Split(MeshWithCurve), retrieve objects.
        /// Note: Includes Parent Layer
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parentindex"></param>
        /// <param name="layername"></param>
        /// <returns></returns>
        public static RhinoObject[] SearchingObjectsInPlateWithParent(RhinoDoc doc, int parentindex, string layername)
        {
            if (doc.Layers[parentindex] != null)
            {
                if (doc.Layers[parentindex].Name == layername)
                    return doc.Objects.FindByLayer(doc.Layers[parentindex]);

                Layer[] layers = doc.Layers[parentindex].GetChildren();
                if (layers.Length != 0)
                {
                    foreach (var layer in layers)
                    {
                        if (layer.Name == layername)
                        {
                            return doc.Objects.FindByLayer(layer);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// show/hide layer
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parentindex"></param>
        /// <param name="layername"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public static bool SetLayerVisible(RhinoDoc doc, int parentindex, string layername, bool visible)
        {
            Debug.Assert(parentindex != -1, "invalid parentindex");

            if (doc.Layers[parentindex] != null)
            {
                if (doc.Layers[parentindex].ParentLayerId == Guid.Empty && doc.Layers[parentindex].Name.Equals(layername))
                {
                    // layer is top level layer
                    doc.Layers[parentindex].IsVisible = visible;
                    doc.Layers[parentindex].CommitChanges();
                    return true;
                }
                else
                {
                    // layer is sub level layer
                    Layer[] layers = doc.Layers[parentindex].GetChildren();
                    if (layers.Length != 0)
                    {
                        foreach (var layer in layers)
                        {
                            if (layer.Name == layername)
                            {
                                layer.IsVisible = visible;
                                layer.CommitChanges();
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public static bool SetLayerVisibleWChildren(RhinoDoc doc, int parentindex, bool visible)
        {
            Debug.Assert(parentindex != -1, "invalid parentindex");

            if (doc.Layers[parentindex] != null)
            {
                //top
                foreach (var o in doc.Objects.FindByLayer(doc.Layers[parentindex]))
                {
                    o.Attributes.Visible = visible;
                    o.CommitChanges();
                }
                doc.Layers[parentindex].IsVisible = visible;
                doc.Layers[parentindex].SetPersistentVisibility(visible);
                doc.Layers[parentindex].CommitChanges();

                Layer[] layers = doc.Layers[parentindex].GetChildren();
                if (layers != null && layers.Length != 0)
                {
                    foreach (var layer in layers)
                    {
                        layer.IsVisible = visible;
                        layer.CommitChanges();
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Check layer in parent layer
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parentIndex"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static bool CheckSubLayerInParent(RhinoDoc doc, int parentIndex, string layerName)
        {
            if (doc.Layers[parentIndex] != null)
            {
                Layer[] layers = doc.Layers[parentIndex].GetChildren();
                bool flag = false;
                if (layers != null)
                    foreach (var layer in layers)
                    {
                        if (layer.Name == layerName)
                            flag = true;
                    }
                if (flag == true)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// update parent layer
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="oldLayerIndex"></param>
        /// <param name="newname"></param>
        /// <param name="obj"></param>
        public static int UpdateParentLayer(RhinoDoc doc, int oldLayerIndex, string newname, RhinoObject obj)
        {
            Layer newLayer = doc.Layers[oldLayerIndex];
            if (newLayer != null)
            {
                newLayer.Name = newname;
                RhinoObject[] objs = doc.Objects.FindByLayer(doc.Layers[oldLayerIndex]);
                if (objs != null)
                {
                    foreach (var oj in objs)
                    {
                        doc.Objects.Delete(oj.Id, true);
                    }
                }
                obj.Attributes.LayerIndex = oldLayerIndex;
                obj.CommitChanges();

                doc.Layers.Modify(newLayer, oldLayerIndex, true);

                return newLayer.LayerIndex;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Add new object to childlayer
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parentIndex"></param>
        /// <param name="childName"></param>
        /// <param name="obj"></param>
        public static void UpdateChildLayer(RhinoDoc doc, int parentIndex, string childName, RhinoObject obj)
        {
            if (doc.Layers[parentIndex] != null)
            {
                Layer[] layers = doc.Layers[parentIndex].GetChildren();
                if (layers != null)
                {
                    foreach (var layer in layers)
                    {
                        if (layer.Name == childName)
                        {
                            obj.Attributes.LayerIndex = doc.Layers.Find(layer.Name, true);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Hide or Show all object in layer
        /// </summary>
        public static void HideObjectInLayer(RhinoDoc doc, int parentIndex, string layerName, bool hide)
        {
            if (doc.Layers[parentIndex] == null)
                return;
            Layer[] layers = doc.Layers[parentIndex].GetChildren();
            if (layers == null)
                return;
            foreach (var layer in layers)
            {
                if (layer.Name == layerName)
                {
                    layer.IsVisible = !hide;
                    doc.Layers.Modify(layer, doc.Layers.Find(layer.Id, true), true);
                    break;
                }
            }
        }

        public static Guid GetCurrentPlateID(RhinoDoc doc)
        {
            string suffix = "Parent";

            Guid index = Guid.Empty;
            int parent_count = 0;
            foreach (var layer in doc.Layers)
            {
                if (layer.Name.LastIndexOf(suffix) >= 0)
                {
                    ++parent_count;
                    RhinoObject[] objs = doc.Objects.FindByLayer(layer);
                    if (objs.Count() == 1) index = objs[0].Id;
                }
            }

            if (parent_count <= 1)
            {
                // parent layer count is 0 or 1
                return index;
            }
            else
            {
                // parent layer count > 1
                foreach (var layer in doc.Layers)
                {
                    if (layer.Name.LastIndexOf(suffix) >= 0)
                    {
                        if (layer.Name == doc.Layers.CurrentLayer.Name)
                        {
                            RhinoObject[] objs = doc.Objects.FindByLayer(layer);
                            if (objs.Count() == 1) return layer.Id;
                        }
                        else
                        {
                            foreach (var child in layer.GetChildren())
                            {
                                if (child.Name == doc.Layers.CurrentLayer.Name)
                                {
                                    RhinoObject[] objs = doc.Objects.FindByLayer(child);
                                    if (objs.Count() == 1) return objs[0].Id;
                                }
                            }
                        }
                    }
                }

                // can't specify parent layer 
                return Guid.Empty;
            }
        }

        public static int GetCurrentPlateLayerID(RhinoDoc doc)
        {
            string suffix = "Parent";

            int index = -1;
            int parent_count = 0;
            foreach (var layer in doc.Layers)
            {
                if (layer.Name.LastIndexOf(suffix) >= 0)
                {
                    ++parent_count;
                    index = layer.LayerIndex;
                }
            }

            if (parent_count <= 1)
            {
                // parent layer count is 0 or 1
                return index;
            }
            else
            {
                // parent layer count > 1
                foreach (var layer in doc.Layers)
                {
                    if (layer.Name.LastIndexOf(suffix) >= 0)
                    {
                        if (layer.Name == doc.Layers.CurrentLayer.Name)
                        {
                            return layer.LayerIndex;
                        }
                        else
                        {
                            foreach (var child in layer.GetChildren())
                            {
                                if (child.Name == doc.Layers.CurrentLayer.Name)
                                {
                                    return child.LayerIndex;
                                }
                            }
                        }
                    }
                }

                // can't specify parent layer 
                return -2;
            }
        }

        internal static void CreateChildLayer(RhinoDoc doc, Guid parentLayerId, List<RhinoObject> objs, string childLayerName)
        {
            if (doc.Layers.Find(parentLayerId, true) >= 0)
            {
                Layer childLayer = null;
                foreach (var layer in doc.Layers)
                {
                    if (layer.Name == childLayerName)
                    {
                        childLayer = layer;
                        break;
                    }
                }
                if (childLayer == null)
                {
                    childLayer = new Layer() { Name = childLayerName };
                    childLayer.ParentLayerId = parentLayerId;
                    doc.Layers.Add(childLayer);
                }

                foreach (var obj in objs)
                {
                    obj.Attributes.LayerIndex = doc.Layers.Find(childLayerName, false);
                    obj.CommitChanges();
                }
            }
        }

        public static Guid SearchingLayerInPlate(RhinoDoc doc, int parentindex, string layername)
        {
            if (doc.Layers[parentindex] != null)
            {
                Layer[] layers = doc.Layers[parentindex].GetChildren();
                if (layers.Length != 0)
                {
                    foreach (var layer in layers)
                    {
                        if (layer.Name == layername)
                        {
                            return layer.Id;
                        }
                    }
                }
            }
            return Guid.Empty;
        }

        public static Guid SearchingLayerInTop(RhinoDoc doc, string layername)
        {
            foreach (var l in doc.Layers)
            {
                if (l.ParentLayerId == Guid.Empty)
                {
                    if (l.Name == layername)
                    {
                        return l.Id;
                    }
                }
            }
            return Guid.Empty;
        }
        public static RhinoObject SearchingObjectInTop(RhinoDoc doc, string layername)
        {
            Guid layerId = SearchingLayerInTop(doc, layername);
            if (layerId == Guid.Empty) return null;

            Layer layer = doc.Layers[doc.Layers.Find(layerId, true)];
            if (layer == null) return null;

            return doc.Objects.FindByLayer(layer).First();
        }
    }
}
