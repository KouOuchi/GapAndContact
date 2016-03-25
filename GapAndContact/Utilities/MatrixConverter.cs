using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace GapAndContact.Utilities
{
    class MatrixConverter
    {
        internal Transform ConvertToTransform(Matrix m)
        {
            Transform t = Transform.Identity;

            if (m.RowCount == 4 && m.ColumnCount == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        t[i, j] = m[i, j];
                    }
                }
            }
            else if (m.RowCount == 3 && m.ColumnCount == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        t[i, j] = m[i, j];
                    }
                }
            }
            else
            {
                throw new Exception("row/col missmatch");
            }
            return t;
        }

        internal Transform GetTranslate(Matrix m)
        {
            if (m.RowCount != 4 || m.ColumnCount != 4) throw new Exception("row/col missmatch");

            return Transform.Translation(m[3, 0], m[3, 1], m[3, 2]);
        }
        internal Transform GetRotation(Matrix m)
        {
            if (m.RowCount <= 2 || m.ColumnCount <= 2) throw new Exception("row/col missmatch");

            Transform t = Transform.Identity;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    t[i, j] = m[i, j];
                }
            }            

            return t;
        }
    }
}
