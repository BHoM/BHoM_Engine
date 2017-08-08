using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Verify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsValid(IBHoMGeometry geometry)
        {
            return _IsValid(geometry as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool _IsValid(IBHoMGeometry geometry)
        {
            return true;
        }

        /***************************************************/

        private static bool _IsValid(Point point)
        {
            return !(double.IsNaN(point.X) || double.IsNaN(point.Y) || double.IsNaN(point.Z));
        }

        /***************************************************/

        private static bool _IsValid(Vector v)
        {
            return !(double.IsNaN(v.X) || double.IsNaN(v.Y) || double.IsNaN(v.Z));
        }
    }
}
