using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace GapAndContact.Utilities
{
    class Mat
    {
        public static Matrix CreateIdentity33()
        {
            Matrix m = new Matrix(3, 3);
            m.SetDiagonal(1.0);
            return m;
        }
        public static Matrix CreateIdentity44()
        {
            Matrix m = new Matrix(4, 4);
            m.SetDiagonal(1.0);
            return m;
        }

        public Vector3d calc_eigenvalue(ref Matrix m)
        {
            double a = m[0, 0];
            double b = m[0, 1];
            double c = m[0, 2];

            double d = m[1, 0];
            double e = m[1, 1];
            double f = m[1, 2];

            double g = m[2, 0];
            double h = m[2, 1];
            double i = m[2, 2];

            ///////////////////////////////////////
            //
            //  | a - x     b        c     |
            //  | d         e - x    f     | = 0
            //  | g         h        i - x |
            //
            ///////////////////////////////////////
            double[] co = new double[4];
            co[3] = -1;
            co[2] = a + e + i;
            co[1] = -a * e - e * i - i * a + f * h + c * g + b * d;
            co[0] = a * e * i + b * f * g + c * d * h - a * f * h - c * e * g - b * d * i;

            double[] x = new double[3];
            int nxs = new Cal().cal_resolve_a_equation(ref co, 3, ref x);

            Vector3d ev = new Vector3d();
            if (3 == nxs)
            {
                ev.X = x[0];
                ev.Y = x[1];
                ev.Z = x[2];
            }
            return ev;
        }

        public bool diagonalization_of_symmetric_matrix
            (ref Matrix me, ref Matrix rotation_matrixp)
        {
            bool result = false;
            Matrix R = Mat.CreateIdentity33();
            double max_d = 0;
            for (int i = 0; i < 3; i++)
            {
                if (Math.Abs(me[i, i]) > max_d)
                {
                    max_d = Math.Abs(me[i, i]);
                }
            }

            for (int icount = 0; icount < 30; icount++)
            {
                int pi = 0, pj = 0;
                double pivot = 0;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = i; j < 3; j++)
                    {
                        if (i != j && Math.Abs(me[i, j]) > pivot)
                        {
                            pivot = Math.Abs(me[i, j]);
                            pi = i;
                            pj = j;
                        }
                    }
                }
                if (pivot < max_d * 1.0e-12)
                {
                    result = true;
                    break;
                }
                int ii = pi;
                int jj = pj;
                double theta = 0;
                double aii = me[ii, ii];
                double ajj = me[jj, jj];
                double aij = me[pi, pj];
                if (Math.Abs(aii - ajj) < Math.Max(Math.Abs(aii), Math.Abs(ajj)) * 1.0e-12)
                {
                    theta = Math.PI / 4;
                }
                else
                {
                    theta = Math.Atan(2.0 * aij / (aii - ajj)) / 2.0;
                }

                Matrix J = Mat.CreateIdentity33();
                J[ii, ii] = Math.Cos(theta);
                J[jj, jj] = J[ii, ii];
                J[ii, jj] = -Math.Sin(theta);
                J[jj, ii] = -J[ii, jj];

                Matrix Jt = Mat.CreateIdentity33();
                Jt[ii, ii] = J[ii, ii];
                Jt[jj, jj] = J[jj, jj];
                Jt[ii, jj] = J[jj, ii];
                Jt[jj, ii] = J[ii, jj];
                Matrix I0 = me;
                Matrix I1 = Jt * I0;
                Matrix I2 = I1 * J;

                me = I2;

                R = R * J;
            }

            if (result)
            {
                rotation_matrixp = R;
            }

            return result;
        }

        internal void get_trans_matrix(ref Matrix R, ref Matrix Rt)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Rt[i, j] = R[j, i];
                }
            }
        }
    }
}