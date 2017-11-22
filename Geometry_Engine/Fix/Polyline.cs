using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Fix
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Polyline MergeColinearSegments(this Polyline curve, double angTol, bool positive)
        {
            double dottol = Math.Cos(angTol);
            List<Line> segments = curve.Explode();
            List<Point> pverts = new List<Point>();
            int lc = segments.Count;
            for (int i = 0; i < lc; i++)
            {
                Vector v1 = segments[i].GetDirection();
                Vector v2 = segments[(i + 1) % lc].GetDirection();
                if (positive)
                {
                    if (v1.GetDotProduct(v2) < dottol)
                    {
                        pverts.Add(segments[i].End);
                    }
                }
                else
                {
                    if (v1.GetDotProduct(v2) > -dottol)
                    {
                        pverts.Add(segments[i].End);
                    }
                }
            }
            pverts.Add(pverts[0]);
            return new Polyline(pverts);
        }

        public static Polyline RemoveZeroSegments(this Polyline contour, double tolerance)
        {
            List<Line> crvs = contour.Explode();
            List<Point> pverts = new List<Point> { crvs[0].Start };
            foreach (Line crv in crvs)
            {
                if (crv.GetLength() > tolerance)
                {
                    pverts.Add(crv.End);
                }
            }
            if (pverts.Count == 1)
            {
                return null;
            }
            return new Polyline(pverts);
        }
    }
}
