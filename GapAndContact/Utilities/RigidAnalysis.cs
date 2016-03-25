using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GapAndContact.Utilities
{
    class RigidAnalysis
    {
        private MatrixConverter conv = new MatrixConverter();

        public InertiaInfo Analyze(RhinoDoc doc, Mesh mesh, MeshFace[] facelist)
        {
            GravityInfo ginfo = new GravityInfo();
            InertiaInfo iinfo = new InertiaInfo();

            GetGravity(ref ginfo, mesh,facelist);

            iinfo.calc_inertia_tensor_center = ginfo.calc_vol_center;
            iinfo.gravity_center = ginfo.center;

            foreach (var f in facelist)
            {
                CalcInertia(ref iinfo, mesh, f);
            }

            Matrix I = iinfo.calc_inertia_tensor_inertia.Duplicate();
            Mat mat_util = new Mat();

            //nVector3d ev =
            //    mat_util.calc_eigenvalue(ref I);

            Matrix R = Mat.CreateIdentity33();
            mat_util.diagonalization_of_symmetric_matrix(ref I, ref R);
            Matrix Rt = Mat.CreateIdentity33();
            mat_util.get_trans_matrix(ref R, ref Rt);

            double max_d = 0;
            int iz = 0;
            Matrix P = Mat.CreateIdentity33();

            for (int i = 0; i < 3; i++)
            {
                if (max_d < Math.Abs(I[i, i]))
                {
                    iz = i;
                    max_d = Math.Abs(I[i, i]);
                }
            }

            int ix = (iz + 1) % 3;
            int iy = (ix + 1) % 3;

            if (Math.Abs(I[ix, ix]) < Math.Abs(I[iy, iy]))
            {
                ix = iy;
                iy = (iz + 1) % 3;
            }

            for (int j = 0; j < 3; j++)
            {
                P[0, j] = Rt[ix, j];
                P[1, j] = Rt[iy, j];
                P[2, j] = Rt[iz, j];
            }

            // P is the result
            iinfo.calc_inertia_tensor_inertia = P;

            return iinfo;
        }

        enum GravityMethod
        {
            Vol,
            Tri
        }
        private void GetGravity(ref GravityInfo ginfo, Mesh mesh, MeshFace[] facelist)
        {
            foreach (var f in facelist)
            {
                GetGravityByFaceSub(ref ginfo, mesh, f, GravityMethod.Vol);
            }
            if (ginfo.vol != 0)
            {
                Vector3d c = ginfo.calc_vol_center / ginfo.vol;
                ginfo.center = new Point3d(c.X, c.Y, c.Z);
            }
        }

        private void GetGravityByFaceSub(ref GravityInfo ginfo, Mesh mesh, MeshFace f, GravityMethod gm)
        {
            if (gm == GravityMethod.Vol)
            {
                CalcGravityByFace(ref ginfo, mesh, f);
            }
            else
            {
                CalcGravityByTriangle(ref ginfo, mesh, f);
            }
        }

        private void CalcGravityByFace(ref GravityInfo ginfo, Mesh m, MeshFace f)
        {
            Vector3d a = new Vector3d(m.Vertices[f.A]);
            Vector3d b = new Vector3d(m.Vertices[f.B]);
            Vector3d c = new Vector3d(m.Vertices[f.C]);

/*
            double vol = Vector3d.Multiply(Vector3d.CrossProduct(a, b), c) / 6.0;
            Vector3d center = (a + b + c) / 4 * vol;
            ginfo.calc_vol_center += center;
            ginfo.vol += vol;
*/
            Vector3d ab = b - a;
            Vector3d ac = c - a;

            double area = Math.Sqrt(ab.Length * ab.Length * ac.Length * ac.Length -  Vector3d.Multiply(ab, ac) * Vector3d.Multiply(ab, ac));
            Vector3d center = (a + b + c) / 3 * area;
            ginfo.calc_vol_center += center;
            ginfo.vol += area;
        }

        private void CalcGravityByTriangle(ref GravityInfo ginfo, Mesh m, MeshFace f)
        {
            Vector3d a = new Vector3d(m.Vertices[f.A]);
            Vector3d b = new Vector3d(m.Vertices[f.B]);
            Vector3d c = new Vector3d(m.Vertices[f.C]);
            Vector3d d = new Vector3d(m.Vertices[f.D]);

            double vol = Vector3d.Multiply(Vector3d.CrossProduct(a, b), c) / 6.0;
            Vector3d center;
            //if (f.IsTriangle)
            {
                center = (a + b + c) / 3 * vol;
            }
            //else
            //{
            //    center = (a + b + c + d) / 4 * vol;
            //}

            ginfo.calc_vol_center += center;
            ginfo.vol += vol;
            ginfo.center = new Point3d(center);
        }

        private void CalcInertia(ref InertiaInfo iinfo, Mesh mesh, MeshFace f)
        {
            Vector3d a = new Vector3d(mesh.Vertices[f.A])
                - iinfo.calc_inertia_tensor_center;
            Vector3d b = new Vector3d(mesh.Vertices[f.B])
                - iinfo.calc_inertia_tensor_center;
            Vector3d c = new Vector3d(mesh.Vertices[f.C])
                - iinfo.calc_inertia_tensor_center;

            CalcInertiaByFaceSub(ref iinfo, mesh, a, b, c);
        }
        private void CalcInertiaByFaceSub(ref InertiaInfo iinfo, Mesh mesh, Vector3d a, Vector3d b, Vector3d c)
        {
            double ax = a.X;
            double ay = a.Y;
            double az = a.Z;

            double bx = b.X;
            double by = b.Y;
            double bz = b.Z;

            double cx = c.X;
            double cy = c.Y;
            double cz = c.Z;

            double ax2 = ax * ax;
            double ay2 = ay * ay;
            double az2 = az * az;

            double bx2 = bx * bx;
            double by2 = by * by;
            double bz2 = bz * bz;

            double cx2 = cx * cx;
            double cy2 = cy * cy;
            double cz2 = cz * cz;

            double m = Vector3d.Multiply(Vector3d.CrossProduct(a, b), c) / 6.0;

            Matrix I = Mat.CreateIdentity33();

            I[0, 0] = m * (ay2 + ay * by + by2 + by * cy + cy2 + cy * ay + az2 + az * bz + bz2 + bz * cz + cz2 + cz * az) / 10;
            I[0, 1] = m * (-2 * ax * ay - ax * by - ax * cy - bx * ay - 2 * bx * by - bx * cy - cx * ay - cx * by - 2 * cx * cy) / 20;
            I[0, 2] = m * (-2 * ax * az - ax * bz - ax * cz - bx * az - 2 * bx * bz - bx * cz - cx * az - cx * bz - 2 * cx * cz) / 20;
            I[1, 0] = I[0, 1];
            I[1, 1] = m * (az2 + az * bz + bz2 + bz * cz + cz2 + cz * az + ax2 + ax * bx + bx2 + bx * cx + cx2 + cx * ax) / 10;
            I[1, 2] = m * (-2 * ay * az - ay * bz - ay * cz - by * az - 2 * by * bz - by * cz - cy * az - cy * bz - 2 * cy * cz) / 20;
            I[2, 0] = I[0, 2];
            I[2, 1] = I[1, 2];
            I[2, 2] = m * (ax2 + ax * bx + bx2 + bx * cx + cx2 + cx * ax + ay2 + ay * by + by2 + by * cy + cy2 + cy * ay) / 10;

            // check valid
            Matrix mcheck = iinfo.calc_inertia_tensor_inertia + I;
            Transform check = conv.ConvertToTransform(mcheck);
            if (check.IsValid)
            {
                iinfo.calc_inertia_tensor_inertia
                    = mcheck;
            }
        }

        private void CalcInertiaByTriangleSub(ref InertiaInfo iinfo, Point3d pa, Point3d pb, Point3d pc)
        {
            Vector3d a = new Vector3d(pa)
                - iinfo.calc_inertia_tensor_center;
            Vector3d b = new Vector3d(pb)
                - iinfo.calc_inertia_tensor_center;
            Vector3d c = new Vector3d(pc)
                - iinfo.calc_inertia_tensor_center;

            double ax = a.X;
            double ay = a.Y;
            double az = a.Z;

            double bx = b.X;
            double by = b.Y;
            double bz = b.Z;

            double cx = c.X;
            double cy = c.Y;
            double cz = c.Z;

            double ax2 = ax * ax;
            double ay2 = ay * ay;
            double az2 = az * az;

            double bx2 = bx * bx;
            double by2 = by * by;
            double bz2 = bz * bz;

            double cx2 = cx * cx;
            double cy2 = cy * cy;
            double cz2 = cz * cz;

            double m = Vector3d.Multiply(Vector3d.CrossProduct(a, b), c) / 6.0;

            Matrix I = Mat.CreateIdentity33();

            I[0, 0] = m * (ay2 + ay * by + by2 + by * cy + cy2 + cy * ay + az2 + az * bz + bz2 + bz * cz + cz2 + cz * az) / 10;
            I[0, 1] = m * (-2 * ax * ay - ax * by - ax * cy - bx * ay - 2 * bx * by - bx * cy - cx * ay - cx * by - 2 * cx * cy) / 20;
            I[0, 2] = m * (-2 * ax * az - ax * bz - ax * cz - bx * az - 2 * bx * bz - bx * cz - cx * az - cx * bz - 2 * cx * cz) / 20;
            I[1, 0] = I[0, 1];
            I[1, 1] = m * (az2 + az * bz + bz2 + bz * cz + cz2 + cz * az + ax2 + ax * bx + bx2 + bx * cx + cx2 + cx * ax) / 10;
            I[1, 2] = m * (-2 * ay * az - ay * bz - ay * cz - by * az - 2 * by * bz - by * cz - cy * az - cy * bz - 2 * cy * cz) / 20;
            I[2, 0] = I[0, 2];
            I[2, 1] = I[1, 2];
            I[2, 2] = m * (ax2 + ax * bx + bx2 + bx * cx + cx2 + cx * ax + ay2 + ay * by + by2 + by * cy + cy2 + cy * ay) / 10;

            iinfo.calc_inertia_tensor_inertia
                = iinfo.calc_inertia_tensor_inertia + I;
        }

    }
}
