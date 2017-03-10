using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHoM.Geometry
{
    public static partial class Create
    {
        public static Plane Plane(Point origin, Vector normal)
        {
            return new Plane(origin, normal);
        }

        public static Plane PlaneFrom3Points(double[] pnts, int length)
        {
            double[] v1 = ArrayUtils.Sub(pnts, length, 0, length);
            double[] v2 = ArrayUtils.Sub(pnts, 2 * length, 0, length);
            Vector normal = new Vector(ArrayUtils.Normalise(ArrayUtils.CrossProduct(v1, v2)));
            Point origin = new Point(Utils.SubArray<double>(pnts, 0, 4));

            return new Plane(origin, normal);
        }

        public static Plane PlaneFromPointArray(double[] pnts, int length)
        {
            if (pnts.Length > 3 * length)
            {
                double[] planePts = new double[3 * length];
                double[] currentPoint = Utils.SubArray<double>(pnts, 0, length);
                double[] nextPoint = Utils.SubArray<double>(pnts, length, length);
                double[] currentVector = null;
                Array.Copy(currentPoint, planePts, length);

                int counter = 1;

                while (ArrayUtils.Equal(currentPoint, nextPoint, 0.0001))
                {
                    currentPoint = nextPoint;
                    nextPoint = Utils.SubArray<double>(pnts, length * (++counter), length);
                }
                Array.Copy(nextPoint, 0, planePts, length, length);

                currentVector = ArrayUtils.Sub(nextPoint, currentPoint);

                for (int i = counter; i < pnts.Length / length - 1; i++)
                {
                    currentPoint = nextPoint;
                    nextPoint = Utils.SubArray<double>(pnts, length * (i + 1), length);
                    if (!ArrayUtils.Equal(currentPoint, nextPoint))
                    {
                        if (ArrayUtils.Parallel(currentVector, ArrayUtils.Sub(nextPoint, currentPoint), 0.0001) == 0)
                        {
                            Array.Copy(nextPoint, 0, planePts, 2 * length, length);
                            Plane plane = PlaneFrom3Points(planePts, length);
                            return plane.InPlane(pnts, length, 0.0001) ? plane : null;
                        }
                    }
                }
            }
            return null;
        }
    }

}
