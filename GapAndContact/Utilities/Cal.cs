using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GapAndContact.Utilities
{
    internal class Cal
    {
        public int cal_resolve_a_equation(ref double[] co, int degree, ref double[] xp)
        {
            int nxs = 0;

            int max_nxs = degree * (degree + 1) / 2;
            double[] xp1 = new double[max_nxs];
            double[] val = new double[max_nxs];
            int nxs1 = cal_resolve_a_equation_sub(ref co, degree, ref xp1, ref val);

            for (int i = 0; i < nxs1; i++)
            {
                if (0 == val[i])
                {
                    xp[nxs] = xp1[i];
                    nxs++;
                }
            }
            return nxs;
        }

        public int cal_resolve_a_equation_sub
        (ref double[] co, int degree, ref double[] xp, ref double[] val)
        {
            int nxs = 0;
            if (1 == degree)
            {
                if (0 != co[0])
                {
                    xp[0] = -co[0] / co[1];
                    val[0] = 0;
                    nxs = 1;
                }
                return nxs;
            }

            double[] co1 = new double[degree];
            int max_nxs = degree * (degree - 1) / 2;
            double[] xp1 = new double[max_nxs];
            double[] val1 = new double[max_nxs];


            for (int i = 0; i < degree; i++)
            {
                co1[i] = co[i + 1] * (i + 1);
            }
            int nxs1 = cal_resolve_a_equation_sub(ref co1, degree - 1, ref xp1, ref val1);

            double max_d = 0;
            for (int i = 0; i < degree; i++)
            {
                double temp_d = Math.Abs(co[i] / co[degree]);
                int beki = degree - i;
                double rbeki = 1.0 / beki;
                temp_d = Math.Pow(temp_d, rbeki);
                max_d = Math.Max(max_d, temp_d);
            }

            if (0 == nxs1)
            {
                double x;
                bool flag = cal_resolve_a_equation_by_newton
                  (ref co, degree, max_d, out x);
                if (flag)
                {
                    xp[0] = x;
                    val[0] = 0;
                    nxs = 1;
                }
            }
            else
            {
                double x1 = xp1[0];
                double v1 = cal_polynomial(ref co, degree, x1);
                double v0;
                double x0;
                if (0 != v1)
                {
                    x0 = x1 - max_d;
                    v0 = cal_polynomial(ref co, degree, x0);
                    double dv = v1 - v0;
                    if (v1 * dv > 0)
                    {
                        double x;
                        bool flag = cal_resolve_a_equation_by_newton
                          (ref co, degree, x0, out x);
                        if (flag)
                        {
                            xp[0] = x;
                            val[0] = 0;
                            nxs = 1;
                        }
                    }
                }
                xp[nxs] = x1;
                val[nxs] = v1;
                nxs++;
                v0 = v1;
                x0 = x1;
                for (int i = 1; i < nxs1; i++)
                {
                    x1 = xp1[i];
                    double vv1 = cal_polynomial(ref co, degree, x1);
                    if (v0 * vv1 < 0)
                    {
                        double x;
                        bool flag = cal_resolve_a_equation_by_newton
                          (ref co, degree, (x0 + x1) / 2, out x);
                        if (flag)
                        {
                            xp[nxs] = x;
                            val[nxs] = 0;
                            nxs++;
                        }
                    }
                    xp[nxs] = x1;
                    val[nxs] = v1;
                    nxs++;
                    v0 = v1;
                    x0 = x1;
                }
                x0 = xp1[nxs1 - 1];
                v0 = cal_polynomial(ref co, degree, x0);
                if (0 != v0)
                {
                    x1 = x0 + max_d;
                    v1 = cal_polynomial(ref co, degree, x1);
                    double dv = v1 - v0;
                    if (v0 * dv < 0)
                    {
                        double x;
                        bool flag = cal_resolve_a_equation_by_newton
                          (ref co, degree, x1, out x);
                        if (flag)
                        {
                            xp[nxs] = x;
                            val[nxs] = 0;
                            nxs++;
                        }
                    }
                }

            }

            return nxs;
        }

        bool cal_resolve_a_equation_by_newton
        (ref double[] co, int degree, double x0, out double xp)
        {
            double t, dt, dt_old, v0 = 0D, dv0, v1, dv1;
            int count;

            double d, dn;
            int i;

            t = x0;
            dt_old = 0;
            for (count = 0; count < 20; count++)
            {
                v0 = cal_polynomial(ref co, degree, t);
                dv0 = cal_differential_of_a_polynomial(ref co, degree, t);
                if (0 == v0)
                    break;
                if (0 == dv0)
                    break;
                dt = -v0 / dv0;
                if (dt == 0)
                    break;

                t += dt; dt_old = dt;
            }

            d = Math.Abs(t);
            dn = 1;


            if (0D != v0)
            {
                dt_old = 0;
                for (count = 0; count < 20; count++)
                {
                    v0 = cal_polynomial(ref co, degree, t);
                    dv0 = cal_differential_of_a_polynomial(ref co, degree, t);
                    if (0 == v0)
                        break;
                    if (0 == dv0)
                        break;
                    dt = -v0 / dv0;
                    if (dt == 0)
                        break;

                    if (dt * dt_old < 0)
                        break;
                    t += dt; dt_old = dt;
                }
            }


            if (0 == v0)
            {
                xp = t;
                return true;
            }
            else
            {
                xp = double.NaN;
                return false;
            }
        }

        bool cal_resolve_a_equation_by_newton
        (ref double[] co, int degree, ref double[] def,
         double epsilon, ref double[] parameterp, ref double[] valuep)
        {
            double t, dt, dt_old, v0, dv0, v1, dv1;
            int count;

            double d, dn;
            int i;
            d = Math.Max(Math.Abs(def[0]), Math.Abs(def[1]));
            dn = 1;
            epsilon = 0;
            for (i = 0; i < degree; i++)
            {
                epsilon = Math.Max(Math.Abs(co[i]) * dn, epsilon);
                dn *= d;
            }
            epsilon *= 1.0e-12;

            v0 = cal_polynomial(ref co, degree, def[0]);
            v1 = cal_polynomial(ref co, degree, def[1]);
            if (v0 * v1 >= 0)
                return false;
            dv0 = cal_differential_of_a_polynomial(ref co, degree, def[0]);
            dv1 = cal_differential_of_a_polynomial(ref co, degree, def[1]);

            if (Math.Abs(dv0) > Math.Abs(dv1)) t = def[0];
            else t = def[1];

            dt_old = 0;
            for (count = 0; count < 20; count++)
            {
                v0 = cal_polynomial(ref co, degree, t);
                dv0 = cal_differential_of_a_polynomial(ref co, degree, t);
                if (Math.Abs(v0) <= epsilon)
                    break;
                if (Math.Abs(dv0) <= epsilon)
                    break;
                dt = -v0 / dv0;
                if (dt == 0)
                    break;

                t += dt; dt_old = dt;
            }

            d = Math.Abs(t);
            dn = 1;
            epsilon = 0;
            for (i = 0; i < degree; i++)
            {
                epsilon = Math.Max(Math.Abs(co[i]) * dn, epsilon);
                dn *= d;
            }
            epsilon *= 1.0e-12;

            if (Math.Abs(v0) >= epsilon)
            {
                dt_old = 0;
                for (count = 0; count < 20; count++)
                {
                    v0 = cal_polynomial(ref co, degree, t);
                    dv0 = cal_differential_of_a_polynomial(ref co, degree, t);
                    if (Math.Abs(v0) <= epsilon)
                        break;
                    if (Math.Abs(dv0) <= epsilon)
                        break;
                    dt = -v0 / dv0;
                    if (dt == 0)
                        break;

                    if (dt * dt_old < 0)
                        break;
                    t += dt; dt_old = dt;
                }
            }


            if (Math.Abs(v0) < epsilon)
            {
                parameterp[0] = t;
                valuep[0] = v0;
                return true;
            }
            else
                return false;
        }

        double cal_polynomial(ref double[] co, int degree, double t)
        {
            int i;
            double a;
            for (a = 0, i = degree; i >= 0; i--)
            {
                double dmax = Math.Max(Math.Abs(a * t), Math.Abs(co[i]));
                a = a * t + co[i];
                if (dmax > 0)
                {
                    if (Math.Abs(a / dmax) < 1.0e-12)
                    {
                        a = 0;
                    }
                }
            }
            return a;
        }

        double cal_differential_of_a_polynomial(ref double[] co, int degree, double t)
        {
            int i;
            double a;
            for (a = 0, i = degree; i > 0; i--) a = a * t + co[i] * i;
            return a;
        }

    }
}
