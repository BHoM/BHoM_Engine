using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BHoM.Geometry
{

    public static class XCircle
    {
      
        public static void CreateNurbForm(this Circle circle)
        {
            double root2on2 = Math.Sqrt(2) / 2;
            double radius = circle.Radius;
            circle.SetKnots(new double[] { 0, 0, 0, Math.PI / 2, Math.PI / 2, Math.PI, Math.PI, 3 * Math.PI / 2, 3 * Math.PI / 2, 2 * Math.PI, 2 * Math.PI, 2 * Math.PI });
            circle.SetWeights(new double[] { 1, root2on2, 1, root2on2, 1, root2on2, 1, root2on2, 1 });
            circle.SetControlPoints(new double[]
            {
                radius, 0, 0, 1,
                radius, radius, 0, 1,
                0, radius, 0, 1,
               -radius, radius, 0, 1,
               -radius, 0, 0, 1,
               -radius,-radius, 0, 1,
                0,-radius, 0, 1,
                radius,-radius, 0, 1,
                radius, 0, 0, 1
            });

            if (circle.Plane.Normal.Z < 1)
            {
                Vector axis = new Vector(ArrayUtils.CrossProduct(circle.Plane.Normal, new double[] { 0, 0, 1, 0 }));
                double angle = ArrayUtils.Angle(circle.Plane.Normal, new double[] { 0, 0, 1, 0 });
                Transform t = Transform.Rotation(Point.Origin, axis, angle);
                t = Transform.Translation(circle.Plane.Origin - Point.Origin) * t;
                circle.SetControlPoints(ArrayUtils.MultiplyMany(t, circle.ControlPointVector));
            }
            else
            {
                Transform t = Transform.Translation(circle.Plane.Origin - Point.Origin);
                circle.SetControlPoints(ArrayUtils.MultiplyMany(t, circle.ControlPointVector));
            }
        }
    }
}
