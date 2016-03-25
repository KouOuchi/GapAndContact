using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

// move to denture
namespace GapAndContact.Utilities
{
    internal class GravityInfo
    {
        public Point3d center;
        public double vol;
        public Vector3d calc_vol_center;

        public GravityInfo()
        {
            center = new Point3d();
            vol = 0.0;
            calc_vol_center = new Vector3d();
        }
    };

    internal class InertiaInfo
    {
        public Matrix calc_inertia_tensor_inertia;
        public Vector3d calc_inertia_tensor_center;
        public Point3d gravity_center;

        public InertiaInfo()
        {
            calc_inertia_tensor_inertia = Mat.CreateIdentity33();
            calc_inertia_tensor_center = new Vector3d();
        }
    };
}
