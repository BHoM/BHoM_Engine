using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BH.oM.Geometry
{
    public static partial class Create
    {
        #region Arc
        /// <summary>
        /// Construct an arc using start point, end point and base plane
        /// </summary>
        /// <param name="startpoint"></param>
        /// <param name="endpoint"></param>
        /// <param name="plane"></param>
        public static Arc Arc(Point startpoint, Point endpoint, Plane plane)
        {
            double[] v1 = ArrayUtils.Sub(startpoint, plane.Origin);
            double[] v2 = ArrayUtils.Sub(endpoint, plane.Origin);

            double radius = ArrayUtils.Length(v1);

            double[] localXAxis = Math.Abs(plane.Normal.Z) == 1 ? new double[] { plane.Normal.Z, 0, 0, 0 } : ArrayUtils.CrossProduct(plane.Normal, new double[] { 0, 0, 1, 0 });
            double[] localYAxis = ArrayUtils.CrossProduct(plane.Normal, localXAxis);

            double[] crossProduct = ArrayUtils.Normalise(ArrayUtils.CrossProduct(v1, v2));
            double multiplier1 = ArrayUtils.DotProduct(v1, localYAxis) > 0 ? 1 : -1;
            double multiplier2 = ArrayUtils.DotProduct(crossProduct, plane.Normal) > 0 ? 1 : -1;

            double startAngle = ArrayUtils.Angle(localXAxis, v1) * multiplier1;
            double arcAngle = ArrayUtils.Angle(v1, v2) * multiplier2;

            //double end = (endAngle - startAngle);

            return new Arc().Initialise(startAngle, startAngle + arcAngle, radius, plane);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="radius"></param>
        /// <param name="plane"></param>
        public static Arc Arc(double startAngle, double endAngle, double radius, Plane plane)
        {
            return new Arc().Initialise(startAngle, endAngle, radius, plane);
        }

        private static Arc Initialise(this Arc arc, double startAngle, double endAngle, double radius, Plane plane)
        {
            double[] controlPoints = new double[12];
            double[] centre = plane.Origin;
            int dimensions = 3;

            double angle = startAngle;
            double increment = (endAngle - startAngle) / 2;
            int offset = 0;
            for (int i = 0; i < 3; i++)
            {
                controlPoints[offset] = radius * Math.Cos(angle);
                controlPoints[1 + offset] = radius * Math.Sin(angle);
                controlPoints[2 + offset] = 0;
                controlPoints[3 + offset] = 1;
                angle += increment;
                offset += dimensions + 1;
            }
            double rotationAngle = ArrayUtils.Angle(plane.Normal, BH.oM.Geometry.Vector.ZAxis());
            if (rotationAngle > 0)
            {
                Vector rotationAxis = BH.oM.Geometry.Vector.CrossProduct(plane.Normal, BH.oM.Geometry.Vector.ZAxis());
                Transform t = Transform.Rotation(BH.oM.Geometry.Point.Origin, rotationAxis, rotationAngle);
                controlPoints = ArrayUtils.MultiplyMany(t, controlPoints);
            }
            controlPoints = ArrayUtils.MultiplyMany(Transform.Translation(plane.Origin - BH.oM.Geometry.Point.Origin), controlPoints);
            arc.SetControlPoints(controlPoints);
            return arc;
        }
        #endregion

    }
}
