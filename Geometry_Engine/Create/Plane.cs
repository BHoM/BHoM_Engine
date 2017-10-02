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

        public static Plane Plane(Point p1, Point p2, Point p3)
        {
            Vector normal = Query.GetCrossProduct(p2 - p1, p3 - p1).GetNormalised();
            return new Plane(p1.GetClone(), normal);
        }
    }
}
