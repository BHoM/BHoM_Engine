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

        public static Ellipse Ellipse(Point centre, double radius1 = 1, double radius2 = 1, Vector axis1 = null, Vector axis2 = null)
        {
            if (axis1 == null) { axis1 = Vector.XAxis; }
            if (axis2 == null) { axis2 = Vector.YAxis; }

            return new Ellipse(centre, axis1, axis2, radius1,radius2);
        }
    }
}
