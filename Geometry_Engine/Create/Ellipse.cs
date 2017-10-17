using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Ellipse Ellipse(Plane plane, double xRadius, double yRadius)
        {
            Vector normal = plane.Normal.GetNormalised();

            Point centre = new Point(plane.Origin.X, plane.Origin.Y, plane.Origin.Z);

            return new Ellipse(centre, normal, xRadius, yRadius);
        }

    }
}
