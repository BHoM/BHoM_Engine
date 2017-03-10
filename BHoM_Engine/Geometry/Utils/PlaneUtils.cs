using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BHoM.Geometry
{
    public static class PlaneUtils
    {
        internal static bool PointsInSamePlane(double[] pnts, int length)
        {
            return Create.PlaneFromPointArray(pnts, length) != null;
        }
    }

}
