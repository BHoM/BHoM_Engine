using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        public static Vector GetAreaScaleVector(Polyline polyline, Point origin, double scaleFactor)
        {
            List<Line> lines = polyline.SubParts();
            double length1 = lines[0].Length();
            double length2 = lines[1].Length();
            double area = length1 * length2;
            double lengthRatio = length1 / length2;
            double scaledArea = scaleFactor * area;
            double AreaFactor = Math.Sqrt(scaledArea * lengthRatio) / length1;
            return new Vector() { X=AreaFactor, Y=AreaFactor, Z=AreaFactor };
        }

        public static IGeometry IScaleArea(this IGeometry geometry, Point origin, double scaleFactor)
        {
            Vector scaleVector = GetAreaScaleVector(geometry as dynamic, origin, scaleFactor);
            if (scaleVector is null)
            {
                return null;
            }
            return Scale(geometry as dynamic, origin, scaleVector);
        }

        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static Vector GetAreaScaleVector(IGeometry geometry, Point origin, double scaleFactor)
        {
            BH.Engine.Base.Compute.RecordError($"Area scaling for {geometry.GetType().FullName} is unimplemented at this time.");
            return null;
        }
    }
}
